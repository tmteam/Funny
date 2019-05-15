using System;
using System.Collections.Generic;
using System.Linq;
using NFun.BuiltInFunctions;
using NFun.Interpritation.Functions;
using NFun.Interpritation.Nodes;
using NFun.ParseErrors;
using NFun.Parsing;
using NFun.Runtime;
using NFun.SyntaxParsing.Visitors;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.Interpritation
{
    public class ExpressionBuilderVisitor: ISyntaxNodeVisitor<IExpressionNode> {
        private readonly FunctionsDictionary _functions;
        private readonly VariableDictionary _variables;

        public static IExpressionNode BuildExpression(ISyntaxNode node, FunctionsDictionary functions,
            VariableDictionary variables) =>
            node.Visit(new ExpressionBuilderVisitor(functions, variables));

        public ExpressionBuilderVisitor(FunctionsDictionary functions, VariableDictionary variables)
        {
            _functions = functions;
            _variables = variables;
        }


        public IExpressionNode Visit(AnonymCallSyntaxNode node)
        {
            if (node.Defenition==null)
                throw ErrorFactory.AnonymousFunDefenitionIsMissing(node);

            if(node.Body==null)
                throw ErrorFactory.AnonymousFunBodyIsMissing(node);
            
            
            //Anonym fun arguments list
            var argumentLexNodes = node.ArgumentsDefenition;
            
            //Prepare local variable scope
            //Capture all outerscope variables
            var localVariables = new VariableDictionary(_variables.GetAllSources());
            
            var arguments = new List<VariableSource>();
            foreach (var arg in argumentLexNodes)
            {
                //Convert argument node
                var varNode = ConvertToArgumentNodeOrThrow(arg);
                var source = new VariableSource(varNode.Name, varNode.Type);
                //collect argument
                arguments.Add(source);
                //add argument to local scope
                if (!localVariables.TryAdd(source))
                {   //Check for duplicated arg-names

                    //If outer-scope contains the conflict variable name
                    if (_variables.GetSourceOrNull(varNode.Name) != null)
                        throw ErrorFactory.AnonymousFunctionArgumentConflictsWithOuterScope(varNode, node.Defenition);
                    else //else it is duplicated arg name
                        throw ErrorFactory.AnonymousFunctionArgumentDuplicates(varNode, node.Defenition);
                }
            }

            var originVariables = localVariables.GetAllSources().Select(s=>s.Name).ToArray();
            var expr = ExpressionBuilderVisitor.BuildExpression(node.Body, _functions, localVariables);
            
            //New variables are new closured
            var closured =  localVariables.GetAllUsages()
                .Where(s => !originVariables.Contains(s.Source.Name))
                .ToList();

            //Add closured vars to outer-scope dictionary
            foreach (var newVar in closured)
                _variables.TryAdd(newVar); //add full usage info to allow analyze outer errors
            
            var fun = new UserFunction("anonymous", arguments.ToArray(), expr);
            return new FunVariableExpressionNode(fun, node.Interval);
        }

        public IExpressionNode Visit(ArraySyntaxNode node)
        {
            var nodes = node.Expressions.Select(ReadNode).ToArray();
            return new ArrayExpressionNode(nodes,node.Interval, node.OutputType);
            
        }

        public IExpressionNode Visit(FunCallSyntaxNode node)
        {
            var id = node.Id;//.ToLower();
            
            var children= new List<IExpressionNode>();
            var childrenTypes = new List<VarType>();
            foreach (var argLexNode in node.Args)
            {
                var argNode =  ReadNode(argLexNode);
                children.Add(argNode);
                childrenTypes.Add(argNode.Type);
            }

            var function = _functions.GetOrNull(id, childrenTypes.ToArray());
            if (function == null)
                throw ErrorFactory.FunctionNotFound( node.Id, node.Interval, children, _functions);
            return function.CreateWithConvertionOrThrow(children, node.Interval);
        }

        public IExpressionNode Visit(IfThenElseSyntaxNode node)
        {
            var ifNodes = new List<IfCaseExpressionNode>();
            foreach (var ifNode in node.Ifs)
            {
                var condition = ReadNode(ifNode.Condition);
                var expr = ReadNode(ifNode.Expr);
                ifNodes.Add(new IfCaseExpressionNode(condition, expr,node.Interval));
            }

            var elseNode = ReadNode(node.ElseExpr);
            return new IfThanElseExpressionNode(
                ifNodes.ToArray(), 
                elseNode,
                elseNode.Interval, 
                node.OutputType);        
        }

        
        public IExpressionNode Visit(NumberSyntaxNode node) => GetValueNode(node);

        public static IExpressionNode GetValueNode(NumberSyntaxNode node)
        {
            var val = node.Value;
            try
            {
                if (val.Length > 2)
                {
                    if (val == "true")
                        return new ValueExpressionNode(true, node.Interval);
                    if (val == "false")
                        return new ValueExpressionNode(false, node.Interval);
                }

                var number = TokenHelper.ToNumber(val);
                if (number is int inum)
                    return new ValueExpressionNode(inum, node.Interval);
                else
                    return new ValueExpressionNode((double) number, node.Interval);
            }
            catch (FormatException)
            {
                throw ErrorFactory.CannotParseNumber(node);
            }
        }

        public IExpressionNode Visit(ProcArrayInit node)
        {
            var start = ReadNode(node.From);
            
            var end = ReadNode(node.To);
            
            if (node.Step == null)
                return new RangeIntFunction().CreateWithConvertionOrThrow(new[] {start, end}, node.Interval);

            var step = ReadNode(node.Step);
            if(step.Type== VarType.Real)
                return new RangeWithStepRealFunction().CreateWithConvertionOrThrow(new[] {start, end, step},node.Interval);
            
            if (step.Type!= VarType.Int32)
                throw ErrorFactory.ArrayInitializerTypeMismatch(step.Type, node);

            
            return new RangeWithStepIntFunction().CreateWithConvertionOrThrow(new[] {start, end, step},node.Interval);        }

       
        public IExpressionNode Visit(TextSyntaxNode node)
            =>
                new ValueExpressionNode(node.Value, node.Interval);
    
      
        public IExpressionNode Visit(VariableSyntaxNode node)
            => GetOrAddVariableNode(node);

        #region not an expression
        public IExpressionNode Visit(EquationSyntaxNode node) => Bad(node);

        public IExpressionNode Visit(IfThenSyntaxNode node) => Bad(node);
        
        public IExpressionNode Visit(ListOfExpressionsSyntaxNode node)=> Bad(node);

        public IExpressionNode Visit(SyntaxTree node)=> Bad(node);

        public IExpressionNode Visit(TypedVarDefSyntaxNode node)=> Bad(node);

        public IExpressionNode Visit(UserFunctionDefenitionSyntaxNode node)=> Bad(node);

        public IExpressionNode Visit(VarDefenitionSyntaxNode node)=> Bad(node);

        #endregion
        private IExpressionNode Bad(ISyntaxNode node)=> throw ErrorFactory.NotAnExpression(node);

        private IExpressionNode ReadNode(ISyntaxNode node) => node.Visit(this);
        private IExpressionNode GetOrAddVariableNode(VariableSyntaxNode varNode)
        {
            
            var lower = varNode.Id;
            if (_variables.GetSourceOrNull(lower) == null)
            {
                var funVars = _functions.GetNonGeneric(lower);
                if (funVars.Count > 1)
                    throw ErrorFactory.AmbiguousFunctionChoise(funVars, varNode);
                if (funVars.Count == 1)
                    return new FunVariableExpressionNode(funVars[0], varNode.Interval);
            }
            var node = _variables.CreateVarNode(varNode.Id, varNode.Interval, varNode.OutputType);
            if(node.Source.Name!= varNode.Id)
                throw ErrorFactory.InputNameWithDifferentCase(varNode.Id, node.Source.Name, varNode.Interval);
            return node;
        }
        
        private FunArgumentExpressionNode ConvertToArgumentNodeOrThrow(ISyntaxNode node)
        {
            if(node is VariableSyntaxNode varNode)
                return new FunArgumentExpressionNode(varNode.Id, VarType.Real, node.Interval);
            if(node is TypedVarDefSyntaxNode typeVarNode)
                return new FunArgumentExpressionNode(typeVarNode.Id, typeVarNode.VarType, node.Interval);
            
            throw ErrorFactory.InvalidArgTypeDefenition(node);
        }
    }
}