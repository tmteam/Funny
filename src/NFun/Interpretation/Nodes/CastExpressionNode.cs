using System;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.Interpretation.Nodes
{
    internal class CastExpressionNode: IExpressionNode
    {
        public static IExpressionNode GetConvertedOrOriginOrThrow(IExpressionNode origin, FunnyType to)
        {
            if (origin.Type == to)
                return origin;
            var converter = VarTypeConverter.GetConverterOrThrow(origin.Type, to, origin.Interval);
            return new CastExpressionNode(origin, to, converter, origin.Interval);
        }
        private readonly IExpressionNode _origin;
        private readonly Func<object, object> _converter;
        public CastExpressionNode(
            IExpressionNode origin, 
            FunnyType targetType, 
            Func<object, object> converter, 
            Interval interval)
        {
            _origin = origin;
            Type = targetType;
            _converter = converter;
            Interval = interval;
        }
        public Interval Interval { get; }
        public FunnyType Type { get; }
        public object Calc()
        {
            var res = _origin.Calc();
            return _converter(res);
        }
    }
}