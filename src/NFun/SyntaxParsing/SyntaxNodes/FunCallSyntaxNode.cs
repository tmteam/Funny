using System.Collections.Generic;
using NFun.SyntaxParsing.Visitors;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.SyntaxParsing.SyntaxNodes
{
    public class FunCallSyntaxNode: ISyntaxNode
    {
        public VarType OutputType { get; set; }
        public int NodeNumber { get; set; }

        public FunCallSyntaxNode(string id, ISyntaxNode[] args, Interval interval, bool isOperator = false)
        {
            Id = id;
            Args = args;
            Interval = interval;
            IsOperator = isOperator;
        }

        public bool IsInBrackets { get; set; }
        public string Id { get; }
        public ISyntaxNode[] Args { get; }
        public Interval Interval { get; set; }
        public bool IsOperator { get; }
        public T Visit<T>(ISyntaxNodeVisitor<T> visitor) => visitor.Visit(this);
        
        public IEnumerable<ISyntaxNode> Children => Args;

    }
}