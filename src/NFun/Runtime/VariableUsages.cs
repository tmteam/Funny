using System.Collections.Generic;
using NFun.Interpritation.Nodes;

namespace NFun.Runtime
{
    public class VariableUsages
    {
        public readonly VariableSource Source;
        public readonly LinkedList<VariableExpressionNode> Nodes = new LinkedList<VariableExpressionNode>();

        public VariableUsages(VariableSource source)
        {
            Source = source;
        }
    }
}