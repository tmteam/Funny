using System.Collections.Generic;
using NFun.ParseErrors;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.Interpritation.Nodes
{
    public class IfCaseExpressionNode : IExpressionNode
    {
        private readonly IExpressionNode _condition;

        public IfCaseExpressionNode(IExpressionNode condition, IExpressionNode body, Interval interval)
        {
            if (condition.Type != VarType.Bool)
                throw ErrorFactory.IfConditionIsNotBool(condition);
            
            _condition = condition;
            Body = body;
            Interval = interval;
        }

        public IExpressionNode Body { get; }
        public VarType Type => Body.Type;
      

        public Interval Interval { get; }
        public bool IsSatisfied() => _condition.Calc().To<bool>();
        public object Calc() => Body.Calc();

        public void Apply(IExpressionNodeVisitor visitor)
        {
            visitor.Visit(this);
            _condition.Apply(visitor);
            Body.Apply(visitor);

        }
    }
}