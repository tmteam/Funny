using NFun.SyntaxParsing.SyntaxNodes;

namespace NFun.SyntaxParsing.Visitors
{
    public interface ISyntaxNodeVisitor<out T>
    {
        T Visit(ArrowAnonymFunctionSyntaxNode arrowAnonymFunNode);
        T Visit(ArraySyntaxNode node);
        T Visit(EquationSyntaxNode node);
        T Visit(FunCallSyntaxNode node);
        T Visit(IfThenElseSyntaxNode node);
        T Visit(IfCaseSyntaxNode node);
        T Visit(ListOfExpressionsSyntaxNode node);
        T Visit(ConstantSyntaxNode node);
        T Visit(GenericIntSyntaxNode node);
        T Visit(SyntaxTree node);
        T Visit(TypedVarDefSyntaxNode node);
        T Visit(UserFunctionDefinitionSyntaxNode node);
        T Visit(VarDefinitionSyntaxNode node);
        T Visit(NamedIdSyntaxNode node);
        T Visit(ResultFunCallSyntaxNode node);
        T Visit(SuperAnonymFunctionSyntaxNode arrowAnonymFunNode);
        T Visit(StructFieldAccessSyntaxNode node);
        T Visit(StructInitSyntaxNode node);
    }
}