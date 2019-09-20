namespace NFun.SyntaxParsing.Visitors
{
    
    public class SetNodeNumberVisitor: EnterVisitorBase
    {
        private int _lastNum;

        public SetNodeNumberVisitor(int startNum = 0)
        {
            _lastNum = startNum;
        }

        protected override VisitorEnterResult DefaultVisit(ISyntaxNode node)
        {
            node.OrderNumber = _lastNum++;
            return VisitorEnterResult.Continue;
        }
    }
}