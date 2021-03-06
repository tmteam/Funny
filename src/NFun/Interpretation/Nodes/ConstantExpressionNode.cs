using System;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.Interpretation.Nodes
{
    internal class ConstantExpressionNode : IExpressionNode
    {
        private readonly object _value;

        public static ConstantExpressionNode CreateConcrete(FunnyType primitive, ulong value, Interval interval) =>
            primitive.BaseType switch
            {
                BaseFunnyType.Real => new ConstantExpressionNode((double)value, FunnyType.Real, interval),
                BaseFunnyType.Int64 => new ConstantExpressionNode((long)value, FunnyType.Int64, interval),
                BaseFunnyType.Int32 => new ConstantExpressionNode((int)value, FunnyType.Int32, interval),
                BaseFunnyType.Int16 => new ConstantExpressionNode((short)value, FunnyType.Int16, interval),
                BaseFunnyType.UInt64 => new ConstantExpressionNode((ulong)value, FunnyType.UInt64, interval),
                BaseFunnyType.UInt32 => new ConstantExpressionNode((uint)value, FunnyType.UInt32, interval),
                BaseFunnyType.UInt16 => new ConstantExpressionNode((ushort)value, FunnyType.UInt16, interval),
                BaseFunnyType.UInt8 => new ConstantExpressionNode((byte)value, FunnyType.UInt8, interval),
                _ => throw new ArgumentOutOfRangeException(nameof(primitive), primitive, null)
            };

        public static ConstantExpressionNode CreateConcrete(FunnyType primitive, long value, Interval interval) =>
            primitive.BaseType switch
            {
                BaseFunnyType.Real => new ConstantExpressionNode((double)value, FunnyType.Real, interval),
                BaseFunnyType.Int64 => new ConstantExpressionNode((long)value, FunnyType.Int64, interval),
                BaseFunnyType.Int32 => new ConstantExpressionNode((int)value, FunnyType.Int32, interval),
                BaseFunnyType.Int16 => new ConstantExpressionNode((short)value, FunnyType.Int16, interval),
                BaseFunnyType.UInt64 => new ConstantExpressionNode((ulong)value, FunnyType.UInt64, interval),
                BaseFunnyType.UInt32 => new ConstantExpressionNode((uint)value, FunnyType.UInt32, interval),
                BaseFunnyType.UInt16 => new ConstantExpressionNode((ushort)value, FunnyType.UInt16, interval),
                BaseFunnyType.UInt8 => new ConstantExpressionNode((byte)value, FunnyType.UInt8, interval),
                _ => throw new ArgumentOutOfRangeException(nameof(primitive), primitive, null)
            };

        public ConstantExpressionNode(object objVal, FunnyType type, Interval interval)
        {
            _value = objVal;
            Interval = interval;
            Type = type;
        }

        public FunnyType Type { get; }
        public Interval Interval { get; }
        public object Calc() => _value;
    }
}