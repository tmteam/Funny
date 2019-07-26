using System;
using NFun.ParseErrors;
using NFun.Tokenization;
using NFun.Types;

namespace NFun.Interpritation.Functions
{
    public class ConcreteUserFunctionPrototype: FunctionBase
    {
        public ConcreteUserFunctionPrototype(string name, VarType returnType, VarType[] argTypes) : base(name,  returnType, argTypes)
        {
        }

        private UserFunction _function;
        public void SetActual(UserFunction function, Interval interval)
        {
            _function = function;

            if (ReturnType != function.ReturnType)
                throw ErrorFactory.InvalidOutputType(function, interval);
        }

        public override object Calc(object[] args)
        {
            if(_function== null)
                throw new InvalidOperationException("Function prototype cannot be called");
            return _function.Calc(args);
        }
    }
}