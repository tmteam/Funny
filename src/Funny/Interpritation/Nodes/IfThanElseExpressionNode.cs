using System;
using System.Collections.Generic;
using System.Linq;
using Funny.Runtime;
using Funny.Types;

namespace Funny.Interpritation.Nodes
{
    public class IfThanElseExpressionNode: IExpressionNode
    {
        private readonly IfCaseExpressionNode[] _ifCaseNodes;
        private readonly IExpressionNode _elseNode;
        private Func<object, object> caster = null;
        public IfThanElseExpressionNode(IfCaseExpressionNode[] ifCaseNodes, IExpressionNode elseNode)
        {
            _ifCaseNodes = ifCaseNodes;
            _elseNode = elseNode;
            Type = GetUpTypeConverion(ifCaseNodes.Select(c => c.Type).Append(elseNode.Type));
            switch (Type.BaseType)
            {
                case PrimitiveVarType.Bool:
                    caster = o => o;
                    break;
                case PrimitiveVarType.Int:
                    caster = o => o;
                    break;
                case PrimitiveVarType.Real:
                    caster = o => Convert.ToDouble(o);
                    break;
                case PrimitiveVarType.ArrayOf:
                    caster = o => o;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object Calc()
        {
            
            foreach (var ifCase in _ifCaseNodes)
            {
                if (ifCase.IsSatisfied())
                    return caster(ifCase.Calc());
            }
            
            return caster(_elseNode.Calc());
        }
        public VarType Type { get; }


        VarType GetUpTypeConverion(IEnumerable<VarType> types)
        {
            bool hasNumeric = false;
            bool hasInt = false;
            bool hasReal = false;
            bool hasBool = false;
            bool hasArray = false;
            foreach (var varType in types)
            {
                switch (varType.BaseType)
                {
                    case PrimitiveVarType.Bool:
                        hasBool = true;
                        break;
                    case PrimitiveVarType.Int:
                        hasInt = true;
                        break;
                    case PrimitiveVarType.Real:
                        hasReal = true;
                        break;
                    case PrimitiveVarType.ArrayOf:
                        hasArray = true;
                        break;
                        ;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (hasArray)
            {
                if(hasInt|| hasBool || hasReal)
                    throw new OutpuCastParseException("Cannot convert array type to primitive");
                return VarType.ArrayOf(VarType.RealType);
            }
            if (hasBool)
            {
                if(hasInt||hasReal)
                    throw new OutpuCastParseException("Cannot convert bool type to number type");
                return VarType.BoolType;
            }

            if (hasReal)
                return VarType.RealType;
            if (hasInt)
                return VarType.IntType;
            
            throw new NotSupportedException("IfThenElse upcast for unknown types");
        }
    }
}