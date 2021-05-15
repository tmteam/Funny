using NFun.Runtime;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.Interpritation.Nodes
{
    public class StructFieldAccessExpressionNode : IExpressionNode
    {
        private readonly string _fieldName;
        private readonly IExpressionNode _source;
        public StructFieldAccessExpressionNode(string fieldName, IExpressionNode source, Interval interval)
        {
            _fieldName = fieldName;
            _source = source;
            Type = source.Type.StructTypeSpecification[fieldName];
            Interval = interval;
        }
        public Interval Interval { get; }
        public VarType Type { get; }
        public object Calc() => ((FunnyStruct) _source.Calc()).GetValue(_fieldName);
        public IExpressionNode Fork(ForkScope scope) => new StructFieldAccessExpressionNode(_fieldName, _source.Fork(scope), Interval);
    }
}