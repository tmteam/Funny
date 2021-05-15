using System;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.Interpritation.Nodes
{
    public class ConstantExpressionNode: IExpressionNode
    {
        private readonly object _value;
        public static ConstantExpressionNode CreateConcrete(VarType primitive, ulong value, Interval interval)
        {
            switch (primitive.BaseType)
            {
                case BaseVarType.Real:   return new ConstantExpressionNode((double)value, VarType.Real, interval);
                case BaseVarType.Int64:  return new ConstantExpressionNode((long) value, VarType.Int64, interval);
                case BaseVarType.Int32:  return new ConstantExpressionNode((int)  value, VarType.Int32, interval);
                case BaseVarType.Int16:  return new ConstantExpressionNode((short)value, VarType.Int16, interval);
                case BaseVarType.UInt64: return new ConstantExpressionNode((ulong)value, VarType.UInt64, interval);
                case BaseVarType.UInt32: return new ConstantExpressionNode((uint)value, VarType.UInt32, interval);
                case BaseVarType.UInt16: return new ConstantExpressionNode((ushort)value, VarType.UInt16, interval);
                case BaseVarType.UInt8:  return new ConstantExpressionNode((byte)value, VarType.UInt8, interval);
                default:
                    throw new ArgumentOutOfRangeException(nameof(primitive), primitive, null);
            }
        }
        public static ConstantExpressionNode CreateConcrete(VarType primitive, long value, Interval interval)
        {
            switch (primitive.BaseType)
            {
                case BaseVarType.Real:   return new ConstantExpressionNode((double)value, VarType.Real, interval);
                case BaseVarType.Int64:  return new ConstantExpressionNode((long) value, VarType.Int64, interval);
                case BaseVarType.Int32:  return new ConstantExpressionNode((int)  value, VarType.Int32, interval);
                case BaseVarType.Int16:  return new ConstantExpressionNode((short)value, VarType.Int16, interval);
                case BaseVarType.UInt64: return new ConstantExpressionNode((ulong)value, VarType.UInt64, interval);
                case BaseVarType.UInt32: return new ConstantExpressionNode((uint)value, VarType.UInt32, interval);
                case BaseVarType.UInt16: return new ConstantExpressionNode((ushort)value, VarType.UInt16, interval);
                case BaseVarType.UInt8:  return new ConstantExpressionNode((byte)value, VarType.UInt8, interval);
                default:
                    throw new ArgumentOutOfRangeException(nameof(primitive), primitive, null);
            }
        }

        public ConstantExpressionNode(object objVal, VarType type, Interval interval)
        {
            _value = objVal;
            Interval = interval;
            Type = type;
        }

        public VarType Type { get; }
        public Interval Interval { get; }
        public object Calc() => _value;
        public IExpressionNode Fork(ForkScope scope) => this;
    }
}