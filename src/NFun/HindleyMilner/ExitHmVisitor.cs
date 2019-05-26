using System.Collections.Generic;
using System.Linq;
using NFun.BuiltInFunctions;
using NFun.HindleyMilner.Tyso;
using NFun.Interpritation;
using NFun.Interpritation.Functions;
using NFun.SyntaxParsing.SyntaxNodes;
using NFun.SyntaxParsing.Visitors;
using NFun.Types;

namespace NFun.HindleyMilner
{
    class ExitHmVisitor: ISyntaxNodeVisitor<bool>
    {
        private readonly HmVisitorState _state;
        private readonly FunctionsDictionary _dictionary;
        public ExitHmVisitor(HmVisitorState state, FunctionsDictionary dictionary)
        {
            _state = state;
            _dictionary = dictionary;
        }

        public bool Visit(ArraySyntaxNode node)=> _state.CurrentSolver.SetArrayInit(node.OrderNumber, node.Expressions.Select(e=>e.OrderNumber).ToArray());
        /// <summary>
        /// User fuctions are not supported by the visitor
        /// </summary>
        public bool Visit(UserFunctionDefenitionSyntaxNode node) => false;

        public bool Visit(ProcArrayInit node)
        {
            if (node.Step == null)
                return _state.CurrentSolver.SetProcArrayInit(node.OrderNumber, node.From.OrderNumber, node.To.OrderNumber);
            else
                return _state.CurrentSolver.SetProcArrayInit(node.OrderNumber, node.From.OrderNumber, node.To.OrderNumber,node.Step.OrderNumber);
        }

        public bool Visit(AnonymCallSyntaxNode anonymFunNode)
        {
            _state.ExitScope();
            return true;
        }

        public bool Visit(EquationSyntaxNode node)
           => _state.CurrentSolver.SetDefenition(node.Id, node.OrderNumber, node.Expression.OrderNumber);

        public bool Visit(FunCallSyntaxNode node)
        {

            if (node.IsOperator)
            {
                switch (node.Id)
                {
                    case CoreFunNames.Negate:
                        return _state.CurrentSolver.SetNegateOp(
                            node.OrderNumber, 
                            node.Args[0].OrderNumber);
                    case CoreFunNames.Multiply:
                    case CoreFunNames.Add:
                    case CoreFunNames.Substract:
                    case CoreFunNames.Remainder:
                        return _state.CurrentSolver.SetArithmeticalOp(
                            node.OrderNumber, 
                            node.Args[0].OrderNumber,
                            node.Args[1].OrderNumber);
                    case CoreFunNames.BitShiftLeft:
                    case CoreFunNames.BitShiftRight:
                        return _state.CurrentSolver.SetBitShiftOperator(
                            node.OrderNumber, 
                            node.Args[0].OrderNumber, 
                            node.Args[1].OrderNumber);
                    case CoreFunNames.LessOrEqual:
                    case CoreFunNames.Less:
                    case CoreFunNames.MoreOrEqual:
                    case CoreFunNames.More:
                        return _state.CurrentSolver.SetComparationOperator(
                            node.OrderNumber,
                            node.Args[0].OrderNumber, 
                            node.Args[1].OrderNumber);
                }
            }
            var argsCount = node.Args.Length;

            //check for recursion call
            var funAlias = AdpterHelper.GetFunAlias(node.Id, argsCount) ;
            var funType = _state.CurrentSolver.GetOrNull(funAlias);
            if (funType != null && funType.Name.Id == NTypeName.FunId 
                                && funType.Arguments.Length-1 == node.Args.Length)
            {
                //Recursive function call. We don't know its signature yet. That's why we set "functional variable",
                //instead of usual function call
                var res =  _state.CurrentSolver.SetInvoke(node.OrderNumber, funAlias,
                    node.Args.Select(a => a.OrderNumber).ToArray());
                return res;
            }
            

            var candidates = _dictionary.GetNonGeneric(node.Id).Where(n=>n.ArgTypes.Length == argsCount).ToList();

            if (candidates.Count == 0)
            {
                var genericCandidate = _dictionary.GetGenericOrNull(node.Id, argsCount);
                if (genericCandidate == null)
                    return false;
                var callDef = ToCallDef(node, genericCandidate);
                return _state.CurrentSolver.SetCall(callDef);

            }
            if (candidates.Count == 1)
                return _state.CurrentSolver.SetCall(ToCallDef(node, candidates[0]));

            //User functions get priority
            var userFunctions = candidates.Where(c => c is ConcreteUserFunctionPrototype).ToList();
            if (userFunctions.Count == 1)
                return _state.CurrentSolver.SetCall(ToCallDef(node, userFunctions[0]));
            
            return _state.CurrentSolver.SetOverloadCall(candidates.Select(ToFunSignature).ToArray(), node.OrderNumber,
                node.Args.Select(a => a.OrderNumber).ToArray());
        }

        public bool Visit(IfThenElseSyntaxNode node)
        {
            return _state.CurrentSolver.ApplyLcaIf(node.OrderNumber,
                node.Ifs.Select(i => i.Condition.OrderNumber).ToArray(),
                node.Ifs.Select(i => i.Expression.OrderNumber).Append(node.ElseExpr.OrderNumber).ToArray());
        }
        public bool Visit(IfCaseSyntaxNode node)=> true;
        public bool Visit(ListOfExpressionsSyntaxNode node)=> true;

        public bool Visit(ConstantSyntaxNode node)
        {
            return _state.CurrentSolver.SetConst(node.OrderNumber, 
                AdpterHelper.ConvertToHmType(node.OutputType));
        }

        public bool Visit(SyntaxTree node)=> true;
       
        public bool Visit(TypedVarDefSyntaxNode node)
            => _state.CurrentSolver.SetVarType(node.Id, AdpterHelper.ConvertToHmType(node.VarType));
            
        public bool Visit(VarDefenitionSyntaxNode node)
            => _state.CurrentSolver.SetVarType(node.Id, AdpterHelper.ConvertToHmType(node.VarType));
        public bool Visit(VariableSyntaxNode node)
        {
            var originId = node.Id;
            
            var localId = _state.GetActualName(node.Id);
            if (_state.CurrentSolver.HasVariable(localId))
                return _state.CurrentSolver.SetVar(node.OrderNumber, localId);
            
            if (_state.CurrentSolver.HasVariable(originId))
                return _state.CurrentSolver.SetVar(node.OrderNumber, originId);
            
            var userFunctions 
                = _dictionary.GetNonGeneric(originId).OfType<ConcreteUserFunctionPrototype>().ToList();
            
            //ambiguous function reference
            //Several functions fits
            if (userFunctions.Count > 1)
                return false;
            
            //if there is no functions - set variable with local name
            if (userFunctions.Count == 0)
                return _state.CurrentSolver.SetVar(node.OrderNumber, localId);
            

            //Make fun variable:
            _state.CurrentSolver.SetVarType(
                originId,
                userFunctions[0].GetHmFunctionalType());
            return _state.CurrentSolver.SetVar(node.OrderNumber, originId);
        }
        
        
        private static FunSignature ToFunSignature(FunctionBase fun) 
            =>
                new FunSignature( AdpterHelper.ConvertToHmType(fun.ReturnType), 
                    fun.ArgTypes.Select(AdpterHelper.ConvertToHmType).ToArray());

        private static CallDef ToCallDef(FunCallSyntaxNode node, FunctionBase fun)
        {
            var ids = new[] {node.OrderNumber}.Concat(node.Args.Select(a => a.OrderNumber)).ToArray();
            var types = new[] {fun.ReturnType}.Concat(fun.ArgTypes).Select(AdpterHelper.ConvertToHmType).ToArray();

            var callDef = new CallDef(types, ids);
            return callDef;
        }
        private static CallDef ToCallDef(FunCallSyntaxNode node, GenericFunctionBase fun)
        {
            var ids = new[] {node.OrderNumber}.Concat(node.Args.Select(a => a.OrderNumber)).ToArray();
            var types = new[] {fun.ReturnType}.Concat(fun.ArgTypes).Select(AdpterHelper.ConvertToHmType).ToArray();

            var callDef = new CallDef(types, ids);
            return callDef;
        }
    }
}