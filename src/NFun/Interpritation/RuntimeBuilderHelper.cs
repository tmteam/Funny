using System;
using System.Collections.Generic;
using System.Linq;
using NFun.ParseErrors;
using NFun.Runtime;
using NFun.SyntaxParsing;
using NFun.SyntaxParsing.SyntaxNodes;
using NFun.SyntaxParsing.Visitors;
using NFun.Tokenization;
using NFun.TypeInference;
using NFun.TypeInference.Solving;
using NFun.Types;

namespace NFun.Interpritation
{
    public static class RuntimeBuilderHelper
    {
        public static TiResult SolveOrThrow(SyntaxTree syntaxTree, FunctionsDictionary functionsDictionary)
        {
            var bodyTypeSolving =  LangTiHelper.SetupTiOrNull(syntaxTree, functionsDictionary)?.Solve();

            if (bodyTypeSolving == null)
                throw ErrorFactory.TypesNotSolved(syntaxTree);
            if (bodyTypeSolving?.IsSolved != true)
            {
                var failedNodeOrNull = syntaxTree.GetDescendantNodeOrNull(bodyTypeSolving.FailedNodeId);
                ThrowTiError(syntaxTree, bodyTypeSolving.Result, failedNodeOrNull);
            }
            return bodyTypeSolving;
        }
        public static void ThrowIfNotSolved(ISyntaxNode functionSyntaxNode, TiResult types)
        {
            if (types.IsSolved) return;
            var failedNodeOrNull = functionSyntaxNode.GetDescendantNodeOrNull(types.FailedNodeId);
            ThrowTiError(functionSyntaxNode, types.Result, failedNodeOrNull);
        }

        private static void ThrowTiError(ISyntaxNode root, TiSolveResult result,ISyntaxNode failedNodeOrNull)
        {
            switch (result)
            {
                case TiSolveResult.Solved:
                    throw new InvalidOperationException();
                case TiSolveResult.NotSolvedOverloadWithSeveralCandidates:
                    throw ErrorFactory.AmbiguousFunctionChoise(failedNodeOrNull);
                case TiSolveResult.NotSolvedNoFunctionFits:
                    throw ErrorFactory.FunctionIsNotExists(failedNodeOrNull);
                default:
                    throw ErrorFactory.TypesNotSolved(root);
            }
        }

        public static void ThrowIfSomeVariablesNotExistsInTheList(this VariableDictionary resultVariables, IEnumerable<string> list )
        {
            var unknownVariables = resultVariables.GetAllUsages()
                .Where(u=> !list.Contains(u.Source.Name)).ToList();
            if (unknownVariables.Any())
            {
                throw ErrorFactory.UnknownVariables(unknownVariables.SelectMany(u => u.Usages));
            }        
        }
        /// <summary>
        /// Gets order of calculating the functions, based on its co using.
        /// </summary>
        public static UserFunctionDefenitionSyntaxNode[] FindFunctionSolvingOrderOrThrow(this SyntaxTree syntaxTree)
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
        public static VariableSource CreateVariableSourceForArgument(
            TypedVarDefSyntaxNode argSyntax,
            VarType actualType)
        {
            if(argSyntax.VarType != VarType.Empty)
                return VariableSource.CreateWithStrictTypeLabel(argSyntax.Id, actualType, argSyntax.Interval);
            else
                return VariableSource.CreateWithoutStrictTypeLabel(
                    name: argSyntax.Id,
                    type: actualType);
        }
    }
}