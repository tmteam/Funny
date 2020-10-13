using System.Collections.Generic;
using System.Linq;
using NFun.Tic.SolvingStates;

namespace NFun.Tic
{
    public class TicResultsWithGenerics : ITicResults
    {
        private readonly IList<TicNode> _typeVariables;

        private readonly SmallStringDictionary<TicNode> _namedNodes;

        private readonly IList<TicNode> _syntaxNodes;
        public TicResultsWithGenerics(
            IList<TicNode> typeVariables, 
            SmallStringDictionary<TicNode> namedNodes, 
            IList<TicNode> syntaxNodes)
        {
            _typeVariables = typeVariables;
            _namedNodes = namedNodes;
            _syntaxNodes = syntaxNodes;
        }

        public TicNode GetVariableNode(string variableName) =>
            _namedNodes[variableName];
        public TicNode GetSyntaxNodeOrNull(int syntaxNode)
        {
            if (syntaxNode >= _syntaxNodes.Count)
                return null;
            return _syntaxNodes[syntaxNode];
        }

        /// <summary>
        /// Gap for tests
        /// </summary>
        public IEnumerable<TicNode> GenericNodes => 
            _typeVariables
                .Union(_namedNodes.Values)
                .Union(_syntaxNodes)
                .Where(t => t?.State is ConstrainsState);

        public bool HasGenerics
        {
            get
            {
                //For performance optimization
                foreach (var node in _typeVariables)
                {
                    if (node?.State is ConstrainsState)
                        return true;
                }
                foreach (var node in _namedNodes.ValuesWithDefaults)
                {
                    if (node?.State is ConstrainsState)
                        return true;
                }
                foreach (var node in _syntaxNodes)
                {
                    if (node?.State is ConstrainsState)
                        return true;
                }

                return false;
            }
        }
        /// <summary>
        /// GAP for tests
        /// </summary>
        public int GenericsCount => GenericsStates.Length;

        private ConstrainsState[] _genericsStates = null;

        public ConstrainsState[] GenericsStates
        {
            get
            {
                if (_genericsStates == null)
                {
                    List<ConstrainsState> states = new List<ConstrainsState>();
                    foreach (var node in _typeVariables)
                    {
                        if (node?.State is ConstrainsState c)
                            states.Add(c);
                    }

                    foreach (var node in _namedNodes.ValuesWithDefaults)
                    {
                        if (node?.State is ConstrainsState c)
                            states.Add(c);
                    }

                    foreach (var node in _syntaxNodes)
                    {
                        if (node?.State is ConstrainsState c)
                            states.Add(c);
                    }

                    _genericsStates = states.ToArray();
                }
                return _genericsStates;
            }
        }
    }
}