using System;
using System.Linq;
using NFun.Runtime;
using NFun.Types;

namespace NFun.Interpritation.Nodes
{
    public class ArrayExpressionNode : IExpressionNode
    {
        private IExpressionNode[] _elements;

        public ArrayExpressionNode(IExpressionNode[] elements)
        {
            _elements = elements;
            if (!elements.Any())
                Type = VarType.ArrayOf(VarType.Anything);
            else
            {
                var elementType = elements[0].Type;

                for (int i = 1; i < elements.Length; i++)
                {
                    var iType = elements[i].Type;
                    if (iType != elementType)
                        throw new NotImplementedException("Array contains different types");
                }
                Type = VarType.ArrayOf(elementType);
            }
        }

        public VarType Type { get; }
        public object Calc()
            => FunArray.By(_elements.Select(e => e.Calc()));
    }
}