using NFun.SyntaxParsing.SyntaxNodes;

namespace NFun.SyntaxParsing.Visitors
{
    public static class SyntaxTreeDeepFieldSearch
    {
        public static ISyntaxNode GetDescendantNodeOrNull(this ISyntaxNode root, int nodeId)
        {
            if (root.OrderNumber == nodeId) 
                return root;
            
            foreach (var child in root.Children)
            {
                var result = GetDescendantNodeOrNull(child, nodeId);
                if (result != null)
                    return result;
            }
            return null;
        }
        public static bool ComeOver(this ISyntaxNode root, ISyntaxNodeVisitor<VisitorEnterResult> enterVisitor,
            ISyntaxNodeVisitor<bool> exitVisitor)
        {
            var enterResult = root.Accept(enterVisitor);
            if (enterResult == VisitorEnterResult.Failed)
                return false;
            if (enterResult == VisitorEnterResult.Skip)
                return true;
            
            foreach (var child in root.Children)
                if (!child.ComeOver(enterVisitor, exitVisitor))
                    return false;

            var res =  root.Accept(exitVisitor);
            return res;
        }
        
        public static bool ComeOver(this ISyntaxNode root, ISyntaxNodeVisitor<VisitorEnterResult> enterVisitor)
        {
            var enterResult = root.Accept(enterVisitor);
            if (enterResult == VisitorEnterResult.Failed)
                return false;
            if (enterResult == VisitorEnterResult.Skip)
                return true;
            
            foreach (var child in root.Children)
                if (!child.ComeOver(enterVisitor))
                    return false;

            return true;
        }
    }

    
}