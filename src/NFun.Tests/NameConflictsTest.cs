using System.Linq;
using NFun;
using NFun.ParseErrors;
using NFun.Types;
using NUnit.Framework;

namespace Funny.Tests
{
    public class NameConflictsTest
    {
        [TestCase("concat = 1+2","concat",3)]
        [TestCase("min = 1+2","min",3)]
        [TestCase("max = 1+2","max",3)]
        [TestCase("foo(x) = x +1\r foo = 1+2","foo",3)]
        public void OutputNameOverloadsBuiltinFunctionName(string expr, string output, object expected) 
            => FunBuilder.BuildDefault(expr).Calculate().AssertHas(VarVal.New(output,expected));

        [TestCase("foo(x) = x +1\r foo = 1+2 \r y = foo*3 ","y",9)]
        [TestCase("concat = 1+2 \r y = concat*3 ","y",9)]
        [TestCase("foo(x) = x +1\r foo = 1+2 \ry = foo*3  ","y",9)]
        [TestCase("foo(x) = x +1\r foo = 1+2 \ry = foo*3 \r  ","y",9)]
        [TestCase("concat = 1+2 \r y = concat*3 \r ","y",9)]
        public void OverloadOutputUsesInOtherEquation(string expr, string output, object expected) 
            => FunBuilder.BuildDefault(expr).Calculate().AssertHas(VarVal.New(output,expected));
        
        [TestCase("min",3, "min:int \r y = min*3 ","y",9)]
        [TestCase("max",3, "max:int \r max*3 ","out",9)]
        public void InputNameOverloadsBuiltinFunctionName(string iname, object ival, string expr, string oname, object oval) 
            => FunBuilder.BuildDefault(expr)
                .Calculate(VarVal.New(iname, ival))
                .AssertHas(VarVal.New(oname, oval));

        [TestCase("y = min*3 ")]
        [TestCase("max*3 ")]
        [TestCase("foo(x) = x +1\r y=foo*3 ")]
        [TestCase("\r y=foo*3 \r foo(x) = x +1")]
        [TestCase("foo(x) = x +1\r foo*3 ")]
        public void ObviousFails(string expr)
            => Assert.Throws<FunParseException>(() => FunBuilder.BuildDefault(expr));
    }
}