using NFun;
using NFun.ParseErrors;
using NFun.Types;
using NUnit.Framework;

namespace Funny.Tests
{
    [TestFixture]
    public class TextTest
    {
        [TestCase("y = ''", "")]
        [TestCase("y = 'hi'", "hi")]
        [TestCase("y = 'World'", "World")]
        [TestCase("y = 'foo'.concat('bar')", "foobar")]
        [TestCase("y = 'foo'.reverse()", "oof")]
        [TestCase("y = ['bar'[1]]", "a")]
        [TestCase("y = 'bar'[1]", 'a')]
        [TestCase("y = '01234'[:]", "01234")]
        [TestCase("y = '01234'[2:4]", "234")]
        [TestCase("y = '01234'[1:1]", "1")]
        [TestCase("y = ''[0:0]", "")]
        [TestCase("y = ''[1:]", "")]
        [TestCase("y = ''[:1]", "")]
        [TestCase("y = ''[::1]", "")]
        [TestCase("y = ''[::3]", "")]
        [TestCase("y = '01234'[1:2]", "12")]
        [TestCase("y = '01234'[2:]", "234")]
        [TestCase("y = '01234'[::2]", "024")]
        [TestCase("y = '01234'[::1]", "01234")]
        [TestCase("y = '01234'[::]", "01234")]
        [TestCase("y = '0123456789'[2:9:3]", "258")]
        [TestCase("y = '0123456789'[1:8:2]", "1357")]
        [TestCase("y = '0123456789'[1:8:]", "12345678")]
        [TestCase("y = '0123456789'[1:8:] == '12345678'", true)]
        [TestCase("y = '0123456789'[1:8:] != '12345678'", false)]
        [TestCase("y = 'abc' == 'abc'", true)]
        [TestCase("y = 'abc' == 'cba'", false)]
        [TestCase("y = 'abc' == 'cba'.reverse()", true)]
        [TestCase("y = 'abc' == 'abc'.reverse()", false)]
        [TestCase("y = 'abc'.concat('def') == 'abcdef'", true)]
        [TestCase("y = 'abc'.concat('de') == 'abcdef'", false)]
        [TestCase("y = '{0}'", "0")]
        [TestCase("y = 'hi {42}'", "hi 42")]
        [TestCase("y = '{42}hi'", "42hi")]
        [TestCase("y = 'hello {42} world'", "hello 42 world")]
        [TestCase("y = 'hello {42+1} world'", "hello 43 world")]
        [TestCase("y = '{''}'", "")]
        [TestCase("y = 'hi {42} and {21}'", "hi 42 and 21")]
        [TestCase("y = 'hi {42+13} and {21-1}'", "hi 55 and 20")]
        [TestCase("y = '{0+1} {1+2} {2+3}'", "1 3 5")]
        [TestCase("y = 'pre {'p{42-1*2}m{21-1+10*3}a'} mid {'p{42-2}m{21-1}a'} fin'", "pre p40m50a mid p40m20a fin")]
        [TestCase("y = 'pre1{'pre2{2-2}after2'}after1'", "pre1pre20after2after1")]
        [TestCase("y = 'pre1 {'inside'} after1'", "pre1 inside after1")]
        [TestCase("y = 'pre'.concat((1+2).toText())","pre3")]
        [TestCase("y = 'a b '.concat((1+2).toText()).split(' ')", new[]{"a","b","3"})]
        public void TextConstantEquation(string expr, object expected) =>
            FunBuilder
                .BuildDefault(expr)
                .AssertBuildJetAndCalculateConstant(VarVal.New("y", expected));


        [TestCase(42,"y = x.toText().concat('lalala')", "42lalala")]
        [TestCase(42, "y = 'pre{x-1*2}mid{x*x/x}fin'", "pre40mid42fin")]

        public void SingleVariableEquation(object input, string expr, object expected) =>
            FunBuilder
                .BuildDefault(expr)
                .Calculate(VarVal.New("x", input))
                .AssertReturns(VarVal.New("y", expected));

        [TestCase("y='  \\\\'","  \\")]
        [TestCase("y='\\t'","\t")]
        [TestCase("y='\\n'","\n")]
        [TestCase("y='\\''","'")]
        [TestCase("y='\\r'","\r")]
        [TestCase("y='\\v'","\v")]
        [TestCase("y='\\f'","\f")]
        [TestCase("y='\\\"'","\"")]
        [TestCase("y='\\\\'","\\")]
        [TestCase("y='e\\''","e'")]
        [TestCase("y='#\\r'","#\r")]
        [TestCase("y=' \\r\r'"," \r\r")]
        [TestCase("y='\\r\r'","\r\r")]
        [TestCase("y='  \\\\  '","  \\  ")]
        [TestCase("y='John: \\'fuck you!\\', he stops.'", "John: 'fuck you!', he stops.") ]
        [TestCase("y='w\\t'","w\t")]
        [TestCase("y='w\\\\\\t'","w\\\t")]
        [TestCase("y='q\\t'","q\t")]
        [TestCase("y='w\\\"'","w\"")]
        [TestCase("y=' \\r'"," \r")]
        [TestCase("y='\t \\n'","\t \n")]
        [TestCase("y='q\\tg'","q\tg")]
        [TestCase("y='e\\\\mm\\''","e\\mm'")]
        [TestCase("y=' \\r\r'"," \r\r")]
        [TestCase("y='\t \\n\n'","\t \n\n")]
        [TestCase("y='pre \\{lalala\\} after'","pre {lalala} after")]
        public void EscapedTest(string expr,string expected) =>
            FunBuilder
                .BuildDefault(expr)
                .AssertBuildJetAndCalculateConstant(VarVal.New("y", expected));

        [TestCase("y='hell")]
        [TestCase("y=hell'")]
        [TestCase("y='")]
        [TestCase("y = '")]
        [TestCase("'\\'")]
        [TestCase("'some\\'")]
        [TestCase("'\\GGG'")]
        [TestCase("'\\q'")]
        [TestCase("y = 'hi'+5")]
        [TestCase("y = ''+10")]
        [TestCase("y = ''+true")]
        [TestCase("y = 'hi'+5+true")]
        [TestCase("y = 'hi'+' '+'world'")]
        [TestCase("y = 'arr: '+ [1,2,3]")]
        [TestCase("y = 'arr: '+ [[1,2],[3]]")]
        [TestCase("y='hello'world'")]
        [TestCase("y='hello''world'")]

        [TestCase("y='hello{}world'")]
        [TestCase("y='hello'{0}world'")]
        [TestCase("y='hello{0}'world'")]
        [TestCase("y='hello\"{0}world'")]
        [TestCase("y='hello{0}\"world'")]
        [TestCase("y='hello{0}\"world{0}'")]
        [TestCase("y='hello{0}world'{0}'")]
        [TestCase("y='hello{0}world{0}''")]
        [TestCase("y='hello{0}world{0}")]
        [TestCase("y='hello{{0}world{0}'")]
        [TestCase("y='hello{0{}world{0}'")]
        [TestCase("y='hello{0}world{{0}'")]
        [TestCase("y='hello{0}world{0{}'")]
        [TestCase("y='hello{'i'{0}}world{{0}'")]
        [TestCase("y='hello{{0}'i'}world{{0}'")]
        [TestCase("y={0}'hello'world'")]
        [TestCase("y='pre {0}''fin'")]
        [TestCase("y='pre {0}''mid{1}fin'")]
        public void ObviousFails(string expr) => 
            Assert.Throws<FunParseException>(()=> FunBuilder.BuildDefault(expr));
    }
}