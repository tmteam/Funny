using System.Linq;
using Funny.Runtime;
using NUnit.Framework;

namespace Funny.Tests
{
    public static class TestTools
    {
        public static void AssertReturns(this CalculationResult result, double delta, params Var[] vars)
        {
            Assert.AreEqual(vars.Length, result.Results.Length);
            Assert.Multiple(() =>
            {
                foreach (var variable in vars) 
                    AssertHas(result, variable.Name, variable.Value, delta);
            });
        }
        public static void AssertReturns(this CalculationResult result,  params Var[] vars)
        {
            AssertReturns(result,0, vars);
        }
        public static void AssertHas(this CalculationResult result, string name, double value, double delta = 0)
        {
            var res = result.Results.FirstOrDefault(r => r.Name == name);
            Assert.IsNotNull(res, $"variable {name} not found");
            Assert.AreEqual (value, res.Value, delta, $"var \"{name}\" expected: {value}, but was: {res.Value}");
        }
    }
}