using System.Collections.Generic;
using NFun.SyntaxParsing.Visitors;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.SyntaxParsing
{
    public interface ISyntaxNode
    {    
        VarType OutputType { get; set; }
        int OrderNumber { get; set; }
        bool IsInBrackets { get; set; }
        Interval Interval { get; set; }
        T Visit<T>(ISyntaxNodeVisitor<T> visitor);
        IEnumerable<ISyntaxNode> Children { get; }
    }
}