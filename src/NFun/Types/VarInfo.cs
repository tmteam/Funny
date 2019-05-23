using NFun.SyntaxParsing;

namespace NFun.Types
{
    public struct VarInfo
    {
        public readonly bool IsOutput;
        public readonly VarType Type;
        public readonly string Name;
        public readonly VarAttribute[] Attributes;

        public VarInfo(bool isOutput, VarType type, string name, VarAttribute[] attributes = null)
        {
            IsOutput = isOutput;
            Type = type;
            Name = name;
            Attributes = attributes ??new VarAttribute[0];
        }

        public override bool Equals(object obj)
        {
            if(obj is VarInfo v)
                return Equals(v);
            return false;
        }

        public bool Equals(VarInfo other)
        {
            return IsOutput == other.IsOutput 
                   && Type.Equals(other.Type) 
                   && string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IsOutput.GetHashCode();
                hashCode = (hashCode * 397) ^ Type.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"({(IsOutput?"out":"in")}) {Name}:{Type}";
        }
    }
}