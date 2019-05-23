using System;
using System.Collections.Generic;
using System.Linq;

namespace NFun.HindleyMilner.Tyso
{
    public class NsResult
    {
        public const int NestedDepth = 100;
        private readonly IList<SolvingNode> _nodes;
        private readonly IList<SolvingNode> _allTypes;
        private readonly Dictionary<string, SolvingNode> _variablesMap;
        public string[] VarNames => _variablesMap.Keys.ToArray();
        public static NsResult NotSolvedResult() => new NsResult(false);
        
        private NsResult(bool isSolved)
        {
            _allTypes = new List<SolvingNode>();
            _variablesMap = new Dictionary<string, SolvingNode>();
            IsSolved = isSolved;
        }
        public NsResult(IList<SolvingNode> nodes, IList<SolvingNode> allTypes, Dictionary<string, SolvingNode> variablesMap)
        {
            IsSolved = true;
            _nodes = nodes;
            _allTypes = allTypes;
            _variablesMap = variablesMap;
            int genericsCount = 0;
            _genMap = new Dictionary<SolvingNode, int>();
            foreach (var type in allTypes)
            {
                var concreteNode = GetConcrete(type, NestedDepth);
                if (concreteNode.Behavior is GenericTypeBehaviour) 
                {
                    if(_genMap.ContainsKey(concreteNode))
                        continue;
                    _genMap.Add(concreteNode, genericsCount);
                    genericsCount++;
                }
            }
            GenericsCount = genericsCount;
        }

        public int GenericsCount { get; }
        public bool IsSolved { get; private set; }

        private readonly Dictionary<SolvingNode, int> _genMap;
        public FType GetVarType(string varId) => ConvertToHmType2(_variablesMap[varId], NestedDepth);

        public FType GetVarTypeOrNull(string varId)
        {
            if (!_variablesMap.TryGetValue(varId, out var solvingNode))
                return null;
            return ConvertToHmType2(solvingNode, NestedDepth);
        }


        public FType GetNodeType(int nodeId) => ConvertToHmType2(_nodes[nodeId], NestedDepth);

        private SolvingNode GetConcrete(SolvingNode node, int nestedCount)
        {
            if(nestedCount<0)
                throw new StackOverflowException("Get Concrete raise SO");
            if (node.Behavior is ReferenceBehaviour eq)
                return GetConcrete(eq.Node, nestedCount-1);
            if (node.Behavior is LcaNodeBehaviour lca && lca.OtherNodes.Length == 1)
                return lca.OtherNodes.First();
            return node;
        }
      
        private FType ConvertToHmType2(SolvingNode node,int nestedCount)
        {
            if(nestedCount<0)
                throw new StackOverflowException("ConvertToHmType2 raise SO");
            var concreteNode = GetConcrete(node, nestedCount-1);
            var beh = concreteNode.Behavior;
            
            
            if (beh is GenericTypeBehaviour)
            {
                if(!_genMap.TryGetValue(concreteNode, out var val))
                    throw new InvalidOperationException("Generic is not in the map");
                //Generic type there!
                return FType.Generic(val);
            }
            var type = beh.MakeType(nestedCount-1);

            SolvingNode[] arguments = type.Arguments
                .Select(a => SolvingNode.CreateStrict(ConvertToHmType2(a, nestedCount-1)))
                .ToArray();
            if(type.Name.Equals(NTypeName.SomeInteger))
                return new FType(NTypeName.Int32);
            
            return new FType(type.Name, arguments);
            
        }

        public FType MakeFunDefenition()
        {
            //maxNodeId is return type.
            if(!_nodes.Any())
                throw new InvalidOperationException();
            var outputNode = _nodes.LastOrDefault((n => n != null));
            if(outputNode==null)
                throw new InvalidOperationException();
            outputNode = GetConcrete(outputNode,NestedDepth);
            List<FType> args = new List<FType>();
            foreach (var solvingNode in _variablesMap)
            {
                var concrete = GetConcrete(solvingNode.Value,NestedDepth);
                if(concrete== outputNode)
                    continue;
                args.Add(ConvertToHmType2(concrete,NestedDepth));
            }
            return FType.Fun(ConvertToHmType2(outputNode,NestedDepth),args.ToArray());
        }

        public FType GetNodeTypeOrNull(int nodeId)
        {
            if (_nodes.Count <= nodeId)
                return null;
            var node = _nodes[nodeId];
            if (node == null)
                return null;
            return ConvertToHmType2(node, NestedDepth);
        }
    }
}