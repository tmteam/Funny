using System.Collections.Generic;
using NFun.SyntaxParsing.Visitors;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.SyntaxParsing.SyntaxNodes
{
    public class ConstantSyntaxNode : ISyntaxNode
    {
        public VarType OutputType { get; set; }
        public int OrderNumber { get; set; }

        
        public ConstantSyntaxNode(object value, VarType varType, Interval interval)
        {
            OutputType = varType;
            Interval = interval;
            Value = value;
        }

        public bool IsInBrackets { get; set; }
        public object Value { get; }
        public Interval Interval { get; set; }
        public T Visit<T>(ISyntaxNodeVisitor<T> visitor) => visitor.Visit(this);
        public IEnumerable<ISyntaxNode> Children => new ISyntaxNode[0];
    }
}