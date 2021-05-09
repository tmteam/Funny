using System;
using System.Collections.Generic;
using System.Linq;
using NFun.Interpritation;
using NFun.Interpritation.Functions;
using NFun.Types;

namespace NFun.FluentApi
{
    public class FunnyContextBuilder
    {
        internal static readonly FunnyContextBuilder Empty = new();
        
        private readonly List<(string, object)> _constantList = new();
        private readonly List<IConcreteFunction> _concreteFunctions = new();
        public FunnyContextBuilder WithConstant(string id, object value) {
            _constantList.Add((id,value));
            return this;
        }

        public FunnyContextBuilder WithFunction<Tin, TOut>(string name, Func<Tin, TOut> function)
        {
            _concreteFunctions.Add(LambdaWrapperFactory.Create(name, function));
            return this;
        }

        internal IFunBuilder CreateRuntimeBuilder(string expression)
        {
            IFunBuilder builder = FunBuilder.With(expression);
            if (_constantList.Any())
            {
                var constants = new ConstantList();
                foreach (var constant in _constantList)
                {
                    var converter = FunnyTypeConverters.GetInputConverter(constant.Item2.GetType());
                    constants.AddConstant(new VarVal(constant.Item1,constant.Item2,converter.FunnyType));   
                }
                builder = ((FunBuilder) builder).With(constants);
            }
            if (_concreteFunctions.Any())
                builder = ((FunBuilder) builder).WithFunctions(_concreteFunctions.ToArray());

            return builder;
        }
        
        public IFunnyContext<TInput, TOutput> ForCalc<TInput, TOutput>() 
            => new FunnyContextSingle<TInput, TOutput>(this);
        public IFunnyContext<TInput, TOutput> ForCalcMany<TInput, TOutput>() where TOutput : new()
            => new FunnyContextMany<TInput, TOutput>(this);
        public IFunnyContext<TInput> ForCalc<TInput>()
            => new FunnyContext<TInput>(this);
    }
}