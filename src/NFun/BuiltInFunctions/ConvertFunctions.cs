using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using NFun.Interpritation.Functions;
using NFun.Runtime;
using NFun.Types;

namespace NFun.BuiltInFunctions
{
    public class ToIntFromRealFunction : FunctionBase
    {
        public ToIntFromRealFunction(string name = "toInt") : base(name, VarType.Int32, VarType.Real){}
        public override object Calc(object[] args)
        {
            try {
                return Convert.ToInt32(args.Get<double>(0));
            }
            catch (Exception e) {
                throw new FunRuntimeException($"Number '{args[0]}' cannot be converted into int", e);
            }
        }
    }
    
    public class ToInt16FromInt16Function : FunctionBase
    {
        public ToInt16FromInt16Function() : base("toInt16", VarType.Int16, VarType.Int16){}
        public override object Calc(object[] args) => args.Get<short>(0);
    }
    
    public class ToInt32FromInt32Function : FunctionBase
    {
        public ToInt32FromInt32Function(string name) : base(name, VarType.Int32, VarType.Int32){}
        public override object Calc(object[] args) => args.Get<int>(0);
    }
    public class ToInt64FromInt64Function : FunctionBase
    {
        public ToInt64FromInt64Function() : base("toInt64", VarType.Int64, VarType.Int64){}
        public override object Calc(object[] args) => args.Get<long>(0);
    }
    public class ToUint16FromUint16Function : FunctionBase
    {
        public ToUint16FromUint16Function() : base("toUint16", VarType.UInt16, VarType.UInt16){}
        public override object Calc(object[] args) => args.Get<ushort>(0);
    }
    public class ToUint32FromUint32Function : FunctionBase
    {
        public ToUint32FromUint32Function() : base("toUint32", VarType.UInt32, VarType.UInt32){}
        public override object Calc(object[] args) => args.Get<uint>(0);
    }
    
    public class ToUint64FromUint64Function : FunctionBase
    {
        public ToUint64FromUint64Function() : base("toUint64", VarType.UInt64, VarType.UInt64){}
        public override object Calc(object[] args) => args.Get<ulong>(0);
    }
    public class ToRealFromRealFunction : FunctionBase
    {
        public ToRealFromRealFunction() : base("toReal", VarType.Real, VarType.Real){}
        public override object Calc(object[] args) => args.Get<double>(0);
    }
    
    
    public class ToUtf8Function : FunctionBase
    {
        public ToUtf8Function() : base("toUtf8", VarType.ArrayOf(VarType.Int32), VarType.Text){}
        public override object Calc(object[] args) => FunArray.By(
            Encoding.UTF8.GetBytes( args.Get<object>(0).ToString()).Select(c=> (object)Convert.ToInt32(c)));
    }
    public class ToUnicodeFunction : FunctionBase
    {
        public ToUnicodeFunction() : base("toUnicode", VarType.ArrayOf(VarType.Int32), VarType.Text){}
        public override object Calc(object[] args) => FunArray.By(
            Encoding.Unicode.GetBytes(args.Get<object>(0).ToString()).Select(c=> (object)Convert.ToInt32(c)));
    }
    public class ToBitsFromIntFunction : FunctionBase
    {
        public ToBitsFromIntFunction() : base("toBits", VarType.ArrayOf(VarType.Bool), VarType.Int32){}
        public override object Calc(object[] args) => FunArray.By(
            new BitArray(BitConverter.GetBytes(args.Get<int>(0))).Cast<bool>().Cast<object>());
    }
    public class ToBytesFromIntFunction : FunctionBase
    {
        public ToBytesFromIntFunction() 
            : base("toBytes", VarType.ArrayOf(VarType.Int32), VarType.Int32){}
        public override object Calc(object[] args) => FunArray.By(
            BitConverter.GetBytes(args.Get<int>(0)).Select(c=> (object)Convert.ToInt32(c)));
    }
    
    public class ToRealFromTextFunction : FunctionBase
    {
        public ToRealFromTextFunction() : base("toReal", VarType.Real, VarType.Text){}
        public override object Calc(object[] args)
        {
            try {
                return Double.Parse(args.GetTextOrThrow(0), CultureInfo.InvariantCulture);
            }
            catch (Exception e) {
                throw new FunRuntimeException($"Text '{args[0]}' cannot be parsed into real", e);
            }
        }
    }
    
    public class ToTextFunction : FunctionBase
    {
        public ToTextFunction() : base("toText", VarType.Text, VarType.Anything){}
        public override object Calc(object[] args) => ToText(args.Get<object>(0));

        string ToText(object val)
        {
            if (val is FunArray f)
                return $"[{string.Join(",", f.Select(ToText))}]";
            if (val is double d)
                return d.ToString(CultureInfo.InvariantCulture);
            else
                return val.ToString();
        }
    }
    public class ToIntFromTextFunction : FunctionBase
    {
        public ToIntFromTextFunction(string name = "toInt") : base(name, VarType.Int32, VarType.Text){}
        public override object Calc(object[] args)
        {
            try {
                return int.Parse(args.GetTextOrThrow(0));
            }
            catch (Exception e) {
                throw new FunRuntimeException($"Text '{args[0]}' cannot be parsed into int", e);
            }
        }
    }
    public class ToIntFromBytesFunction : FunctionBase
    {
        public ToIntFromBytesFunction() : base("toInt", VarType.Int32, VarType.ArrayOf(VarType.Int32)){}
        public override object Calc(object[] args)
        {
            try {
                var val = ((IFunArray) args[0]);
                if(val.Count>4)
                    throw new FunRuntimeException("Array is too long");
                byte[] arr;
                if (val.Count == 4)
                    arr = val.Select(Convert.ToByte).ToArray();
                else
                    arr = val.Concat(new int[4 - val.Count].Cast<object>()).Select(Convert.ToByte).ToArray();
                return BitConverter.ToInt32(arr, 0);            }
            catch (Exception e) {
                throw new FunRuntimeException($"Array '{args[0]}' cannot be converted into int", e);
            }
            
        }
    }
}