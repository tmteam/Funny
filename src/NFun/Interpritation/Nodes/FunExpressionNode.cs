using System.Linq;
using NFun.Interpritation.Functions;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.Interpritation.Nodes
{
    public class FunExpressionNode : IExpressionNode
    {
        private readonly FunctionBase _fun;
        private readonly IExpressionNode[] _argsNodes;

        public FunExpressionNode(FunctionBase fun, IExpressionNode[] argsNodes, Interval interval)
        {
            _fun = fun;
            _argsNodes = argsNodes;
            Interval = interval;
        }

       

        public Interval Interval { get; }
        public VarType Type => _fun.ReturnType;
        public object Calc()
        {
            var argValues = _argsNodes
                .Select(a => a.Calc())
                .ToArray();
            return _fun.Calc(argValues);
        }

        public void Apply(IExpressionNodeVisitor visitor)
        {
            visitor.Visit(this , _fun.Name, _argsNodes.Select(a=>a.Type).ToArray());
            foreach (var expressionNode in _argsNodes)
            {
                expressionNode.Apply(visitor);
            }
        }
    }
}