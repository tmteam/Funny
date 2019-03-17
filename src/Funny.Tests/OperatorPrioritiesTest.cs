using System.Collections.Generic;
using Funny.Runtime;
using Funny.Types;
using NUnit.Framework;

namespace Funny.Tests
{
    [TestFixture]
    public class OperatorPrioritiesTest
    {
        [TestCase("y=(x1<>0) and (x2<>0) ==  (x3<>0)","y=(x1<>0) and ((x2<>0)==(x3<>0))")]
        [TestCase("y=(x1<>0) ==  (x2<>0) and (x3<>0)","y=((x1<>0)==(x2<>0)) and (x3<>0)")]

        [TestCase("y=(x1<>0) and (x2<>0) or  (x3<>0)","y=((x1<>0) and (x2<>0)) or (x3<>0)")]
        [TestCase("y=(x1<>0) or  (x2<>0) and (x3<>0)","y=(x1<>0) or ((x2<>0) and (x3<>0))")]
        
        [TestCase("y=(x1<>0)  <> (x2<>0)  or (x3<>0)","y=((x1<>0)<>(x2<>0)) or (x3<>0)")]
        [TestCase("y=(x1<>0)  or (x2<>0)  <> (x3<>0)","y=(x1<>0) or ((x2<>0)<>(x3<>0))")]

        [TestCase("y=(x1<>0)  <> (x2<>0) and (x3<>0)","y=((x1<>0)<>(x2<>0)) and (x3<>0)")]
        [TestCase("y=(x1<>0) and (x2<>0)  <> (x3<>0)","y=(x1<>0) and ((x2<>0)<>(x3<>0))")]

        [TestCase("y=(x1<>0)  == (x2<>0) or  (x3<>0)","y=((x1<>0)==(x2<>0)) or  (x3<>0)")]
        [TestCase("y=(x1<>0)  or (x2<>0)  == (x3<>0)","y=(x1<>0)  or ((x2<>0)==(x3<>0))")]
        
        [TestCase("y=(x1<>0)  == (x2<>0) <>  (x3<>0)","y=((x1<>0)==(x2<>0)) <>  (x3<>0)")]
        [TestCase("y=(x1<>0)  <> (x2<>0)  == (x3<>0)","y=((x1<>0)<>(x2<>0))  == (x3<>0)")]

        public void DiscreetePriorities(string actualExpr, string expectedExpr)
        {
            var allCombinations = new List<Var[]>();
            foreach (var x1 in new[] {0, 1})
            foreach (var x2 in new[] {0, 1})
            foreach (var x3 in new[] {0, 1})
                allCombinations.Add(new[]{
                    Var.New("x1",x1),
                    Var.New("x2",x2),
                    Var.New("x3",x3)});

            Assert.Multiple(()=>{
                foreach (var inputs in allCombinations)
                {
                    var actual = Interpreter
                        .BuildOrThrow(actualExpr)
                        .Calculate(inputs)
                        .GetResultOf("y");

                    var expected = Interpreter
                        .BuildOrThrow(expectedExpr)
                        .Calculate(inputs)
                        .GetResultOf("y");

                    if (!actual.Equals(expected))
                        Assert.Fail($"On x1={inputs[0].Value} x2={inputs[1].Value} x3={inputs[2].Value}\r" +
                                    $"Eq: {actualExpr}\r" +
                                    $"Expected: {expected}\r" +
                                    $"But was: {actual} ");
                }
            });
        }
       
        [TestCase("y = 1+2*3",  "y = 1+(2*3)")]
        [TestCase("y = 2*3+1",  "y = (2*3)+1")]
        [TestCase("y = 1+4/2",  "y = 1+(4/2)")]
        [TestCase("y = 4/2+1",  "y = (4/2)+1")]
        [TestCase("y = 5*4/2",  "y = (5*4)/2")]
        [TestCase("y = 4/2*5",  "y = (4/2)*5")]
        [TestCase("y = 2^3*4",  "y = (2^3)*4")]
        [TestCase("y = 4*2^3",  "y = 4*(2^3)")]
        [TestCase("y = 2^3/4",  "y = (2^3)/4")]
        [TestCase("y = 4/2^3",  "y = 4/(2^3)")]
        [TestCase("y = 2^3+4",  "y = (2^3)+4")]
        [TestCase("y = 4+2^3",  "y = 4+(2^3)")]
        
        public void ArithmeticPriorities(string actualExpr, string expectedExpr)
        {
            var expected = Interpreter.BuildOrThrow(expectedExpr).Calculate().Get("y");
            
            Interpreter
                .BuildOrThrow(actualExpr)
                .Calculate()
                .AssertReturns(new Var("y", expected.Value, expected.Type));
        }
        
    }
}