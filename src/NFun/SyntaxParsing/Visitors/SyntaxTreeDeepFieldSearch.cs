using NFun.SyntaxParsing.SyntaxNodes;

namespace NFun.SyntaxParsing.Visitors
{
    public static class SyntaxTreeDeepFieldSearch
    {
        public static ISyntaxNode FindNodeByOrderNumOrNull(this ISyntaxNode root, int nodeId)
        {
            if (root.OrderNumber == nodeId) 
                return root;
            
            foreach (var child in root.Children)
            {
                var result = FindNodeByOrderNumOrNull(child, nodeId);
                if (result != null)
                    return result;
            }
            return null;
        }
        public static ISyntaxNode FindVarDefenitionOrNull(this ISyntaxNode root, string nodeName)
        {
            if (root is TypedVarDefSyntaxNode v && v.Id == nodeName)
                return root;
            if (root is VarDefenitionSyntaxNode vd && vd.Id == nodeName)
                return root;

            foreach (var child in root.Children)
            {
                var result = FindVarDefenitionOrNull(child, nodeName);
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
            {
                if (!child.ComeOver(enterVisitor, exitVisitor))
                    return false;
            }
            return root.Accept(exitVisitor);
        }

        private static bool ComeOver(this ISyntaxNode root, 
            ISyntaxNode parent, int childNumber,
            ISyntaxNodeVisitor<VisitorEnterResult> enterVisitor,
            ISyntaxNodeVisitor<bool> exitVisitor)
        {
            var enterResult = root.Accept(enterVisitor);

            if (enterResult == VisitorEnterResult.Failed)
                return false;
            if (enterResult == VisitorEnterResult.Skip)
                return true;

            int i = 0;
            foreach (var child in root.Children)
            {
                if (!child.ComeOver(root, i, enterVisitor, exitVisitor))
                    return false;
                i++;

            }
            var res = root.Accept(exitVisitor);
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