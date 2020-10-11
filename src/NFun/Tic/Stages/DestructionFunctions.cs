using NFun.Tic.SolvingStates;

namespace NFun.Tic
{
    public class DestructionFunctions : IStateCombinationFunctions
    {
        public static DestructionFunctions Singletone { get; } = new DestructionFunctions();
        public bool Apply(StatePrimitive ancestor, StatePrimitive descendant, TicNode _, TicNode __)
            => true;

        public bool Apply(StatePrimitive ancestor, ConstrainsState descendant, TicNode ancestorNode, TicNode descendantNode)
        {
            if (descendant.Fits(ancestor))
            {
                TraceLog.Write("p+c: ");
                
                if (descendant.Prefered != null && descendant.Fits(descendant.Prefered))
                    descendantNode.State = descendant.Prefered;
                else
                    descendantNode.State = ancestor;
            }
            return true;
        }

        public bool Apply(StatePrimitive ancestor, ICompositeState descendant, TicNode _, TicNode __)
            => true;

        public bool Apply(ConstrainsState ancestor, StatePrimitive descendant, TicNode ancestorNode, TicNode descendantNode)
        {
            if (ancestor.Fits(descendant))
            {
                TraceLog.Write("c+p: ");
                ancestorNode.State = descendant;
            }
            return true;
        }

        public bool Apply(ConstrainsState ancestor, ConstrainsState descendant, TicNode ancestorNode, TicNode descendantNode)
        {
            TraceLog.Write("c+c: ");

            var result = ancestor.MergeOrNull(descendant);
            if (result == null)
                return true;

            if (result is StatePrimitive)
            {
                descendantNode.State = ancestorNode.State = result;
                return true;
            }

            if (ancestorNode.Type == TicNodeType.TypeVariable ||
                descendantNode.Type != TicNodeType.TypeVariable)
            {
                ancestorNode.State = result;
                descendantNode.State = new StateRefTo(ancestorNode);
            }
            else
            {
                descendantNode.State = result;
                ancestorNode.State = new StateRefTo(descendantNode);
            }
            descendantNode.Ancestors.Remove(ancestorNode);
            return true;
        }

        public bool Apply(ConstrainsState ancestor, ICompositeState descendant, TicNode ancestorNode, TicNode descendantNode)
        {
            TraceLog.Write("c+af: ");
            if (ancestor.Fits(descendant))
            {
                ancestorNode.State = new StateRefTo(descendantNode);
            }
            return true;
        }

        public bool Apply(ICompositeState ancestor, StatePrimitive descendant, TicNode _, TicNode __)
            => false;

        public bool Apply(ICompositeState ancestor, ConstrainsState descendant, TicNode ancestorNode, TicNode descendantNode)
        {
            if (descendant.Fits(ancestor))
            {
                TraceLog.Write("a+c: ");
                descendantNode.State = new StateRefTo(ancestorNode);
            }
            return true;
        }

        public bool Apply(ICompositeState ancestor, ICompositeState descendant, TicNode ancestorNode, TicNode descendantNode)
        {
            if (ancestor is StateArray ancArray)
            {
                if (descendant is StateArray descArray)
                {
                    TraceLog.Write("a+a: ");
                    SolvingFunctions.Destruction(descArray.ElementNode, ancArray.ElementNode);
                }
                return true;
            }
            if (ancestor is StateFun ancFun)
            {
                if (descendant is StateFun descFun)
                {
                    TraceLog.Write("f+f: ");
                    if (ancFun.ArgsCount == descFun.ArgsCount)
                    {
                        for (int i = 0; i < ancFun.ArgsCount; i++)
                            SolvingFunctions.Destruction(descFun.ArgNodes[i], ancFun.ArgNodes[i]);
                        SolvingFunctions.Destruction(ancFun.RetNode, descFun.RetNode);
                    }
                }
            }
            return true;
        }
    }
}