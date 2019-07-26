using System;
using System.Collections.Generic;
using System.Linq;
using NFun.HindleyMilner;
using NFun.HindleyMilner.Tyso;
using NFun.Interpritation.Functions;
using NFun.ParseErrors;
using NFun.Runtime;
using NFun.SyntaxParsing;
using NFun.SyntaxParsing.SyntaxNodes;
using NFun.SyntaxParsing.Visitors;
using NFun.Types;

namespace NFun.Interpritation
{
    public static class RuntimeBuilder
    {
        public static FunRuntime Build(
            SyntaxTree syntaxTree,
            FunctionsDictionary functionsDictionary)
        {
            var userFunctions = new List<UserFunction>();
            //get topology sort of the functions
            var functionSolveOrder  = FindFunctionsSolvingOrderOrThrow(syntaxTree);
            
            //build user functions
            foreach (var functionSyntaxNode in functionSolveOrder)
            {
               var userFunction =  BuildFunctionAndPutItToDictionary(functionSyntaxNode, functionsDictionary);
               userFunctions.Add(userFunction);
            }

            var algorithm = new HmAlgorithmAdapter(functionsDictionary);
            if(!algorithm.ComeOver(syntaxTree))
                throw ErrorFactory.TypesNotSolved(syntaxTree);

            //solve body
            var bodyTypeSolving = algorithm.Solve();
            if (!bodyTypeSolving.IsSolved)
                throw ErrorFactory.TypesNotSolved(syntaxTree);

            foreach (var syntaxNode in syntaxTree.Children)
            {
                //function nodes were solved above
                if(syntaxNode is UserFunctionDefenitionSyntaxNode)
                    continue;
                
                //set types to nodes
                syntaxNode.ComeOver(new ApplyHmResultVisitor(bodyTypeSolving, SolvedTypeConverter.SetGenericsToAny));
            }
            
            var variables = new VariableDictionary(); 
            var equations = new List<Equation>();

            foreach (var lexRoot in syntaxTree.Nodes)
            {
                if (lexRoot is EquationSyntaxNode node)
                {
                    var equation = BuildEquationAndPutItToVariables(node, functionsDictionary, variables);
                    equations.Add(equation);
                }
                else if (lexRoot is VarDefenitionSyntaxNode varDef)
                {
                    var variableSource = new VariableSource(
                        varDef.Id, 
                        varDef.VarType, 
                        varDef.Interval, 
                        varDef.Attributes);
                    if (!variables.TryAdd(variableSource))
                    {
                        var allUsages = variables.GetUsages(variableSource.Name);
                        throw ErrorFactory.VariableIsDeclaredAfterUsing(allUsages);
                    }
                }
                else if(lexRoot is UserFunctionDefenitionSyntaxNode)
                    continue;//user function was built above
                else 
                    throw  new InvalidOperationException($"Type {lexRoot} is not supported as tree root");
            }   
            return new FunRuntime(equations, variables, userFunctions);
        }

        private static Equation BuildEquationAndPutItToVariables(EquationSyntaxNode equation,FunctionsDictionary functionsDictionary, VariableDictionary variables)
        {
            var expression = ExpressionBuilderVisitor.BuildExpression(equation.Expression, functionsDictionary, variables);
            var newSource = new VariableSource(equation.Id, equation.OutputType, equation.Attributes) {
                IsOutput = true
            };
            
            if (!variables.TryAdd(newSource))
            {
                //some equation referenced the source before
                var usages = variables.GetUsages(equation.Id);
                if (usages.Source.IsOutput)
                    throw ErrorFactory.OutputNameWithDifferentCase(equation.Id, equation.Expression.Interval);
                else
                    throw ErrorFactory.CannotUseOutputValueBeforeItIsDeclared(usages);
            }

            //ReplaceInputType
            if(newSource.Type != expression.Type)
                throw FunParseException.ErrorStubToDo($"Equation types mismatch. Expected: {newSource.Type} but was: {expression.Type}");            
            return new Equation(equation.Id, expression);
        }

        private static UserFunction BuildFunctionAndPutItToDictionary(
            UserFunctionDefenitionSyntaxNode functionSyntaxNode,
            FunctionsDictionary functionsDictionary)
        {
            var funAlias = functionSyntaxNode.GetFunAlias();

            //introduce function variable here
            var visitorInitState = CreateVisitorStateFor(functionSyntaxNode);

            //solving each function
            var typeSolving = new HmAlgorithmAdapter(functionsDictionary, visitorInitState);

            if (!typeSolving.ComeOver(functionSyntaxNode.Body))
                throw FunParseException.ErrorStubToDo($"Function '{functionSyntaxNode.Id}' is not solved");

            var setFunTypeResult = visitorInitState.CurrentSolver.SetFunDefenition(funAlias,
                functionSyntaxNode.OrderNumber,
                functionSyntaxNode.Body.OrderNumber);
            if (!setFunTypeResult.IsSuccesfully)
            {
                if (setFunTypeResult.Error == SetTypeResultError.VariableDefenitionDuplicates)
                    throw ErrorFactory.FunctionAlreadyExist(functionSyntaxNode);
                else
                    throw ErrorFactory.FunctionTypesNotSolved(functionSyntaxNode);
            }
                
            
            // solve the types
            var types = typeSolving.Solve();
            if (!types.IsSolved)
                throw ErrorFactory.TypesNotSolved(functionSyntaxNode);

            var isGeneric = types.GenericsCount > 0;
            //set types to nodes
            functionSyntaxNode.ComeOver(new ApplyHmResultVisitor(types, SolvedTypeConverter.SaveGenerics));
            var funType = types.GetVarType(funAlias, SolvedTypeConverter.SaveGenerics);
            
            if (isGeneric)
            {
                var prototype = new GenericUserFunctionPrototype(functionSyntaxNode.Id,
                    funType.FunTypeSpecification.Output,
                    funType.FunTypeSpecification.Inputs);
                //add prototype to dictionary for future use
                functionsDictionary.Add(prototype);
                return BuildGenericFunction(functionSyntaxNode, prototype, functionsDictionary);
            }
            else
            {
                //make function prototype
                var prototype = new ConcreteUserFunctionPrototype(functionSyntaxNode.Id,
                    funType.FunTypeSpecification.Output,
                    funType.FunTypeSpecification.Inputs);
                //add prototype to dictionary for future use
                functionsDictionary.Add(prototype);
                return BuildConcreteFunction(functionSyntaxNode, prototype, functionsDictionary);
            }
        }

        private static UserFunction BuildGenericFunction(
            UserFunctionDefenitionSyntaxNode lexFunction, 
            GenericUserFunctionPrototype prototype, 
            FunctionsDictionary functionsDictionary)
        {
            var vars = new VariableDictionary();
            for (int i = 0; i < lexFunction.Args.Count ; i++)
            {
                var id = lexFunction.Args[i].Id;
                if (!vars.TryAdd(new VariableSource(id, prototype.ArgTypes[i])))
                {
                    throw ErrorFactory.FunctionArgumentDuplicates(lexFunction, lexFunction.Args[i]);
                }

            }
            var expression = ExpressionBuilderVisitor
                .BuildExpression(lexFunction.Body, functionsDictionary, vars);
            
            ExpressionHelper.CheckForUnknownVariables(
                lexFunction.Args.Select(a=>a.Id).ToArray(), vars);
            
            var function = new UserFunction(
                name: lexFunction.Id, 
                variables: vars.GetAllSources(),
                isReturnTypeStrictlyTyped: lexFunction.ReturnType!= VarType.Empty, 
                expression: expression);
            
            prototype.SetActual(function, lexFunction.Interval);
            
            return function;
        }
        
        private static UserFunction BuildConcreteFunction(
            UserFunctionDefenitionSyntaxNode lexFunction, 
            ConcreteUserFunctionPrototype prototype, 
            FunctionsDictionary functionsDictionary)
        {
            var vars = new VariableDictionary();
            for (int i = 0; i < lexFunction.Args.Count ; i++)
            {
                var variableSource = CreateVariableSourceForArgument(
                    lexFunction.Args[i], 
                    prototype.ArgTypes[i]);
                if (!vars.TryAdd(variableSource))
                {
                    throw ErrorFactory.FunctionArgumentDuplicates(lexFunction, lexFunction.Args[i]);
                }
            }
            var expression = ExpressionBuilderVisitor
                .BuildExpression(lexFunction.Body, functionsDictionary, vars);
            
            ExpressionHelper.CheckForUnknownVariables(
                lexFunction.Args.Select(a=>a.Id).ToArray(), vars);
            
            var function = new UserFunction(
                name: lexFunction.Id, 
                variables: vars.GetAllSources(), 
                isReturnTypeStrictlyTyped: lexFunction.ReturnType!= VarType.Empty, 
                expression: expression);
            prototype.SetActual(function, lexFunction.Interval);
            return function;
        }

        private static VariableSource CreateVariableSourceForArgument(
            TypedVarDefSyntaxNode argSyntax,
            VarType actualType)
        {
            if(argSyntax.VarType != VarType.Empty)
                return new VariableSource(argSyntax.Id, actualType, argSyntax.Interval);
            else
                return new VariableSource(
                    name: argSyntax.Id,
                    type: actualType);
        }

        public static HmVisitorState CreateVisitorStateFor(UserFunctionDefenitionSyntaxNode node)
        {
            var visitorState = new HmVisitorState(new HmHumanizerSolver());
            
            //Add user function as a functional variable
            //make outputType
            var outputType = visitorState.CreateTypeNode(node.ReturnType);
            
            //create input variables
            var argTypes = new List<SolvingNode>();
            foreach (var argNode in node.Args)
            {
                if (visitorState.HasAlias(argNode.Id))
                    throw ErrorFactory.FunctionArgumentDuplicates(node, argNode);

                var inputAlias = AdpterHelper.GetArgAlias(argNode.Id, node.GetFunAlias());

                //make aliases for input variables
                visitorState.AddVariableAliase(argNode.Id, inputAlias);
                
                if (argNode.VarType.BaseType == BaseVarType.Empty)
                {
                    //variable type is not specified
                    var genericVarType = visitorState.CurrentSolver.SetNewVarOrThrow(inputAlias);
                    argTypes.Add(genericVarType);
                }
                else
                {
                    //variable type is specified
                    var hmType = argNode.VarType.ConvertToHmType();
                    visitorState.CurrentSolver.SetVarType(inputAlias, hmType);
                    argTypes.Add(SolvingNode.CreateStrict(hmType));
                }
                    
            }
            //set function variable defenition
            visitorState.CurrentSolver
                .SetVarType(node.GetFunAlias(), FType.Fun(outputType, argTypes.ToArray()));
            return visitorState;
        }
        
        /// <summary>
        /// Gets order of calculating the functions, based on its co using.
        /// </summary>
        private static UserFunctionDefenitionSyntaxNode[] FindFunctionsSolvingOrderOrThrow(SyntaxTree syntaxTree)
        {
            var userFunctions = syntaxTree.Children.OfType<UserFunctionDefenitionSyntaxNode>().ToList();

            var userFunctionsNames = new Dictionary<string, int>();
            int i = 0;
            foreach (var userFunction in userFunctions)
            {
                var alias = userFunction.GetFunAlias();
                if (userFunctionsNames.ContainsKey(alias))
                    throw ErrorFactory.FunctionAlreadyExist(userFunction);
                userFunctionsNames.Add(alias, i);
                i++;
            }

            int[][] dependenciesGraph = new int[i][];
            int j = 0;
            foreach (var userFunction in userFunctions)
            {
                var visitor = new FindFunctionDependenciesVisitor(userFunctionsNames);
                if (!userFunction.ComeOver(visitor))
                    throw new InvalidOperationException("User fun come over");
                dependenciesGraph[j] = visitor.GetFoundDependencies();
                j++;
            }

            var sortResults = GraphTools.SortCycledTopology(dependenciesGraph);

            var functionSolveOrder = new UserFunctionDefenitionSyntaxNode[sortResults.NodeNames.Length];
            for (int k = 0; k < sortResults.NodeNames.Length; k++)
                functionSolveOrder[k] = userFunctions[sortResults.NodeNames[k]];
            
            if (sortResults.HasCycle)
                //if functions has cycle, then function sovle order is cycled
                throw ErrorFactory.ComplexRecursion(functionSolveOrder);
            
          
            return functionSolveOrder;
        }
    }
    
}
