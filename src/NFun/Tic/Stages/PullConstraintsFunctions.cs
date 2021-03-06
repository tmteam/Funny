using System;
using System.Runtime.CompilerServices;
using NFun.Tic.SolvingStates;

namespace NFun.Tic.Stages
{
    public class PullConstraintsFunctions: IStateCombination2dimensionalVisitor
    {
        public static IStateCombination2dimensionalVisitor SingleTone { get; } = new PullConstraintsFunctions();
        
        public bool Apply(StatePrimitive ancestor, StatePrimitive descendant, TicNode _, TicNode __) =>
            descendant.CanBeImplicitlyConvertedTo(ancestor);
        public bool Apply(StatePrimitive ancestor, ConstrainsState descendant, TicNode _, TicNode __) 
            => !descendant.HasDescendant || descendant.Descedant.CanBeImplicitlyConvertedTo(ancestor);
        public bool Apply(StatePrimitive ancestor, ICompositeState descendant, TicNode _, TicNode __) 
            => descendant.CanBeImplicitlyConvertedTo(ancestor);
        public bool Apply(ConstrainsState ancestor, StatePrimitive descendant, TicNode ancestorNode, TicNode descendantNode)
            => ApplyAncestorConstrains(ancestorNode, ancestor, descendant);
        public bool Apply(ConstrainsState ancestor, ConstrainsState descendant, TicNode ancestorNode, TicNode descendantNode)
        {
            var ancestorCopy = ancestor.GetCopy();
            ancestorCopy.AddDescedant(descendant.Descedant);
            var result = ancestorCopy.GetOptimizedOrNull();
            if (result == null)
                return false;
            ancestorNode.State = result;
            return true;
        }
        public bool Apply(ConstrainsState ancestor, ICompositeState descendant, TicNode ancestorNode, TicNode descendantNode)
            => ApplyAncestorConstrains(ancestorNode, ancestor, descendant);
        public bool Apply(ICompositeState ancestor, StatePrimitive descendant, TicNode _, TicNode __) => false;
        public bool Apply(ICompositeState ancestor, ConstrainsState descendant, TicNode ancestorNode, TicNode descendantNode)
        {
            if (ancestor is StateArray ancArray)
            {
                var result = SolvingFunctions.TransformToArrayOrNull(descendantNode.Name, descendant);
                if (result == null)
                    return false;
                result.ElementNode.AddAncestor(ancArray.ElementNode);
                descendantNode.State = result;
                descendantNode.RemoveAncestor(ancestorNode);
            }
            else if (ancestor is StateFun ancFun)
            {
                var result = SolvingFunctions.TransformToFunOrNull(
                    descendantNode.Name, descendant, ancFun);
                if (result == null)
                    return false;
                result.RetNode.AddAncestor(ancFun.RetNode);
                for (int i = 0; i < result.ArgsCount; i++)
                    result.ArgNodes[i].AddAncestor(ancFun.ArgNodes[i]);
                descendantNode.State = result;
                descendantNode.RemoveAncestor(ancestorNode);
            }
            else if (ancestor is StateStruct ancStruct)
            {
         
                var result = SolvingFunctions.TransformToStructOrNull(descendant, ancStruct);
                if (result == null)
                    return false;
                //todo Зачем это тут, если и так структура ссылается на оригинальные поля?
                /*foreach (var ancField in ancStruct.Fields)
                {
                    var descField = result.GetFieldOrNull(ancField.Key);
                    if (descField != ancField.Value)
                    {
                        descField.AddAncestor(ancField.Value);
                    }
                }*/
                descendantNode.State = result;
                //descendantNode.RemoveAncestor(ancestorNode);
            }
            return true;
        }

        public bool Apply(ICompositeState ancestor, ICompositeState descendant, TicNode ancestorNode, TicNode descendantNode)
        {
            if (ancestor.GetType() != descendant.GetType())
                return false;
            if (ancestor is StateArray ancArray)
            {
                var descArray = (StateArray) descendant;
                if (descArray.ElementNode != ancArray.ElementNode)
                {
                    descArray.ElementNode.AddAncestor(ancArray.ElementNode);
                }
                descendantNode.RemoveAncestor(ancestorNode);

            }
            else if (ancestor is StateFun ancFun)
            {
                var descFun = (StateFun) descendant;

                if (descFun.ArgsCount != ancFun.ArgsCount)
                    return false;
                descFun.RetNode.AddAncestor(ancFun.RetNode);
                for (int i = 0; i < descFun.ArgsCount; i++)
                    ancFun.ArgNodes[i].AddAncestor(descFun.ArgNodes[i]);
                descendantNode.RemoveAncestor(ancestorNode);

            }
            else if (ancestor is StateStruct ancStruct)
            {
                var descStruct = (StateStruct) descendant;
                // desc node has to have all ancestors fields that has exast same type as desc type
                // (implicit field convertion is not allowed)
                foreach (var ancField in ancStruct.Fields)
                {
                    var descField = descStruct.GetFieldOrNull(ancField.Key);
                    if (descField == null)
                        descendantNode.State = descStruct.With(ancField.Key, ancField.Value);
                    else
                    {
                        SolvingFunctions.MergeInplace(ancField.Value, descField);
                        if (ancField.Value.State is StateRefTo) 
                            ancestorNode.State = ancestor.GetNonReferenced();
                        if (descField.State is StateRefTo)
                            descendantNode.State = descendant.GetNonReferenced();
                    }
                }
                // descendantNode.RemoveAncestor(ancestorNode);
            }
            else
            {
                throw new NotSupportedException($"Composite type {ancestor.GetType().Name} is not supported");
            }

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ApplyAncestorConstrains(TicNode ancestorNode, ConstrainsState ancestor, ITypeState typeDesc)
        {
            var ancestorCopy = ancestor.GetCopy();
            ancestorCopy.AddDescedant(typeDesc);
            var result = ancestorCopy.GetOptimizedOrNull();
            if (result == null)
                return false;
            ancestorNode.State = result;
            return true;
        }
    }
}