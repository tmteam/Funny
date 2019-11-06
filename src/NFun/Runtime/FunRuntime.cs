using System;
using System.Collections.Generic;
using System.Linq;
using NFun.Interpritation;
using NFun.Interpritation.Functions;
using NFun.Types;

namespace NFun.Runtime
{
    public class FunRuntime
    {
        public void ApplyEntry(IExpressionNodeVisitor visitor)
        {
            foreach (var userFunction in UserFunctions)
            {
                userFunction.Apply(visitor);
            }

            foreach (var varInfo in Inputs)
            {
                visitor.VisitInput(varInfo);
            }

            foreach (var equation in _equations)
            {
                visitor.Visit(equation);
                equation.Expression.Apply(visitor);
            }
        }
        public VarInfo[] Inputs => _variables.GetAllSources()
            .Where(v => !v.IsOutput)
            .Select(s => new VarInfo(false,  s.Type,s.Name, s.IsStrictTyped, s.Attributes)).ToArray();

        public VarInfo[] Outputs => _variables.GetAllSources()
            .Where(v => v.IsOutput)
            .Select(s => new VarInfo(true,  s.Type,s.Name, s.IsStrictTyped, s.Attributes)).ToArray();

        public IEnumerable<UserFunction> UserFunctions { get; }

        private readonly IList<Equation> _equations;
        private readonly IVariableDictionary _variables;
      
        public FunRuntime(IList<Equation> equations, IVariableDictionary variables, List<UserFunction> userFunctions)
        {
            _equations = equations;
            _variables = variables;
            UserFunctions = userFunctions;
        }

        public CalculationResult Calculate(params VarVal[] vars)
        {
            foreach (var value in vars)
            {
                var varName = value.Name;
                var source = _variables.GetSourceOrNull(varName);
                if(source==null)
                    throw new ArgumentException($"unexpected input '{value.Name}'");
                source.SetConvertedValue(value.Value);
            }
            
            var ans = new VarVal[_equations.Count];
            for (int i = 0; i < _equations.Count; i++)
            { 
                var e = _equations[i];
                ans[i] = new VarVal(e.Id, e.Expression.Calc(), e.Expression.Type);
                _variables.GetSourceOrNull(e.Id).Value = ans[i].Value;
            }
            return new CalculationResult(ans);
        }
    }
}