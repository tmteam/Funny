using NFun.BuiltInFunctions;
using NFun.Exceptions;
using NFun.Runtime;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.Interpritation.Nodes
{
    public class VariableExpressionNode: IExpressionNode
    {
        public VariableExpressionNode(VariableSource source, Interval interval)
        {
            Source = source;
            Interval = interval;
        }

        public VariableSource Source { get; }
        public Interval Interval { get; }
        public VarType Type => Source.Type;
        public object Calc() => Source.InternalFunnyValue;
        public IExpressionNode Fork(ForkScope scope) => 
            new VariableExpressionNode(scope.ForkVariableSource(Source), Interval);

        public override string ToString() => $"{Source.Name}: {Source.FunnyValue}";

    }
}