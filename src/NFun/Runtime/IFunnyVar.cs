using System;
using NFun.SyntaxParsing;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.Runtime
{
    public interface IFunnyVar
    {
        /// <summary>
        /// Variable name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Variable attributes
        /// </summary>
        FunnyAttribute[] Attributes { get; }

        /// <summary>
        /// Type of variable
        /// </summary>
        FunnyType Type { get; }

        /// <summary>
        /// internal representation of value
        /// </summary>
        object FunnyValue { get; }

        /// <summary>
        /// The variable is calculated in the script and can be used as one of the results of the script
        /// </summary>
        bool IsOutput { get; }

        /// <summary>
        /// Represents current CLR value of the funny variable
        /// </summary>
        object Value { get; set; }
    }

    internal class VariableSource : IFunnyVar
    {
        internal object InternalFunnyValue;

        private readonly FunnyVarAccess _access;

        internal static VariableSource CreateWithStrictTypeLabel(
            string name,
            FunnyType type,
            Interval typeSpecificationIntervalOrNull,
            FunnyVarAccess access,
            FunnyAttribute[] attributes = null)
            => new(name, type, typeSpecificationIntervalOrNull, access, attributes);

        internal static VariableSource CreateWithoutStrictTypeLabel(
            string name, FunnyType type, FunnyVarAccess access, FunnyAttribute[] attributes = null)
            => new(name, type, access, attributes);

        private VariableSource(
            string name,
            FunnyType type,
            Interval typeSpecificationIntervalOrNull,
            FunnyVarAccess access,
            FunnyAttribute[] attributes = null)
        {
            _access = access;
            InternalFunnyValue = type.GetDefaultValueOrNull();
            TypeSpecificationIntervalOrNull = typeSpecificationIntervalOrNull;
            Attributes = attributes ?? Array.Empty<FunnyAttribute>();
            Name = name;
            Type = type;
        }

        public bool IsOutput => _access.HasFlag(FunnyVarAccess.Output);


        private VariableSource(string name, FunnyType type, FunnyVarAccess access, FunnyAttribute[] attributes = null)
        {
            _access = access;
            InternalFunnyValue = type.GetDefaultValueOrNull();
            Attributes = attributes ?? Array.Empty<FunnyAttribute>();
            Name = name;
            Type = type;
        }

        public FunnyAttribute[] Attributes { get; }
        public string Name { get; }
        internal Interval? TypeSpecificationIntervalOrNull { get; }
        public FunnyType Type { get; }

        public object FunnyValue => InternalFunnyValue;

        public object Value
        {
            get => FunnyTypeConverters.GetOutputConverter(Type).ToClrObject(InternalFunnyValue);
            set => InternalFunnyValue = FunnyTypeConverters.ConvertInputOrThrow(value,Type);
        }
    }

    internal enum FunnyVarAccess
    {
        NoInfo = 0,

        /// <summary>
        /// Funny variable is input, so can be modified from the outside before calculation
        /// </summary>
        Input = 1 << 0,

        /// <summary>
        /// Funny variable is output so it can be considered as the result of the calculation
        /// </summary>
        Output = 1 << 1,
    }
}