using NFun.TestTools;
using NFun.Tic;
using NUnit.Framework;

namespace NFun.SyntaxTests
{
    [TestFixture]
    public class AnonymousFunTest
    {
        [TestCase( @"y = [11.0,20.0,1.0,2.0].filter(fun(i)= i>10)",new[]{11.0,20.0})]
        [TestCase( @"y = [11.0,20.0,1.0,2.0].filter(fun(i)= i>10)",new[]{11.0,20.0})]
        [TestCase( @"y = [11,20,1,2].filter(fun(i:int)= i>10)",new[]{11,20})]
        [TestCase( @"y = [11,20,1,2].filter(fun(i:int)= i>10)",new[]{11,20})]
        [TestCase( @"y = [11,20,1,2].filter(fun(i:int):bool = i>10)",new[]{11,20})]
        [TestCase( @"y = [11,20,1,2].filter(fun(i:int):bool =; i>10)",new[]{11,20})]
        [TestCase( @"y = map([1,2,3], fun(i:int)=i*i)",new[]{1,4,9})]
        [TestCase( @"y = map([1,2,3], fun(i:int):real  =i*i)",new[]{1.0,4,9})]
        [TestCase( @"y = map([1,2,3], fun(i:int):int64  =i*i)",new long[]{1,4,9})]
        [TestCase( @"y = [1,2,3] . map(fun(i:int)=i*i)",new[]{1,4,9})]
        [TestCase( @"y = [1.0,2.0,3.0] . map(fun(i)=i*i)",new[]{1.0,4.0,9.0})]
        [TestCase( @"y = [1.0,2.0,3.0] . fold(fun(i,j)=i+j)",6.0)]
        [TestCase( @"y = [1.0,2.0,3.0] . fold(fun(i:real,j)=i+j)",6.0)]
        [TestCase( @"y = [1.0,2.0,3.0] . fold(fun(i,j:real):real=i+j)",6.0)]
        [TestCase( @"y = [1.0,2.0,3.0] . fold(fun(i,j):real=i+j)",6.0)]

        [TestCase( @"y = fold([1.0,2.0,3.0],fun(i,j)=i+j)",6.0)]
        [TestCase( @"y = [1,2,3] . fold(fun(i:int, j:int)=i+j)",6)]
        [TestCase( @"y = fold([1,2,3],fun(i:int, j:int)=i+j)",6)]
        [TestCase( "y = [1.0,2.0,3.0].any(fun(i)= i == 1.0)",true)]
        [TestCase( "y = [1.0,2.0,3.0].any(fun(i)= i == 0.0)",false)]
        [TestCase( "y = [1.0,2.0,3.0].all(fun(i)= i >0)",true)]
        [TestCase( "y = [1.0,2.0,3.0].all(fun(i)= i >1.0)",false)]
        [TestCase( "f(m:real[], p):bool = m.all(fun(i)= i>p) \r y = f([1.0,2.0,3.0],1.0)",false)]

        [TestCase("y = [-7,-2,0,1,2,3].filter(fun it>0)", new[] { 1, 2, 3 })]
        [TestCase("y = [-1,-2,0,1,2,3].filter((fun it>0)).fold(fun(i,j)= i+j)", 6 )]
        [TestCase("y = [-1,-2,0,1,2,3].filter((fun it>0)).filter(fun(i)=i>2)", new[]{3})]
        [TestCase("y = [-1,-2,0,1,2,3].filter((fun it>0)).map(fun(i)=i*i).map(fun(i:int)=i*i)", new[]{1,16,81})]
        [TestCase("y = [-1,-2,0,1,2,3].filter((fun it>0)).map(fun(i)=i*i).map(fun(i)=i*i)", new[]{1,16,81})]
        [TestCase("y = [-1,-2,0.0,1,2,3].filter((fun it>0)).map(fun(i)=i*i).map(fun(i)=i*i)", new[]{1.0,16.0,81.0})]

        [TestCase("y = [-1,-2,0,1,2,3].filter((fun it>0)).fold(fun(a,b)= a+b)", 6 )]
        [TestCase("y = [-1,-2,0,1,2,3].filter((fun it>0)).filter(fun(a)=a>2)", new[]{3})]
        [TestCase("y = [-1,-2,0,1,2,3].filter((fun it>0)).map(fun(a)=a*a).map(fun(b)=b*b)", new[]{1,16,81})]

        [TestCase("y = [-1,-2,0,1,2,3].filter(fun it>0).map(fun(a)=a*a).map(fun(b:int)=b*b)", new[]{1,16,81})]
        [TestCase("y = [-1,-2,0,1,2,3].filter(fun it>0).filter(fun(a:int)=a>2)", new[]{3})]
        [TestCase("y = [-1,-2,0,1,2,3].filter(fun it>0).fold(fun(a:int,b)= a+b)", 6 )]
        [TestCase(@"car3(g) = g(2);   y = car3(fun(x)=x-1)   ", 1)]
        [TestCase(@"car4(g) = g(2);   y = car4(fun it)   ", 2)]
        [TestCase(@"call5(f, x) = f(x); y = call5(fun(x)=x+1,  1)", 2)]
        [TestCase(@"call6(f, x) = f(x); y = call6(fun(x)=x+1.0, 1.0)", 2.0)]
        [TestCase(@"call7(f, x) = f(x); y = call7(fun(x:real)=x+1.0, 1.0)", 2.0)]
        [TestCase(@"call8(f) = fun(i)=f(i); y = call8(fun(x)=x+1)(2)", 3)]
        [TestCase(@"call9(f) = fun(i)=f(i); y = (fun(x)=x+1).call9()(2)", 3)]
        [TestCase(@"mult(x)= fun(y)=x*y;    y = mult(2)(3)", 6)]
        [TestCase(@"mult(x)= fun(y)=fun(z)=x*y*z;    y = mult(2)(3)(4)", 24)]
        [TestCase(@"mult(x)= fun(y)=fun(z)=x*y*z;    y:real = mult(2)(3)(4)", 24.0)]
        [TestCase(@"mult(x)= fun(y)=fun(z)=x*y*z;    y:int = mult(2)(3)(4)", 24)]
        [TestCase(@"mult()= fun(x)=fun(y)=fun(z)=x* y*z; y = mult()(2)(3)(4)", 24)]
        [TestCase(@"mult()= fun(x)=fun(y)=fun(z)=x* y*z; y:int = mult()(2)(3)(4)", 24)]
        [TestCase(@"mult()= fun(x)=fun(y)=fun(z)=x* y*z; y:real = mult()(2)(3)(4)", 24.0)]

        [TestCase(@"y = (fun(x)=x+1)(3.0)", 4.0)]
        [TestCase(@"f = fun(x)=x+1; y = f(3.0)", 4.0)]
        [TestCase(@"f = fun(a)=fun(b)=a+b; y = f(3.0)(5.0)", 8.0)]
        public void AnonymousFunctions_ConstantEquation(string expr, object expected)
        {
            TraceLog.IsEnabled = true;
            var runtime = expr.Build();
            runtime.AssertInputsCount(0,"Unexpected inputs on constant equations");
            runtime.Calc().AssertResultHas("y", expected);
        }
        
        [TestCase( "y = [1.0,2.0,3.0].map(fun(i)= i*x1*x2)",3.0,4.0, new []{12.0,24.0,36.0})]
        [TestCase( "x1:int\rx2:int\ry = [1,2,3].map(fun(i:int)= i*x1*x2)",3,4, new []{12,24,36})]
        [TestCase( "y = [1.0,2.0,3.0].fold(fun(i,j)= i*x1 - j*x2)",2.0,3.0, -17.0)]
        [TestCase( "y = [1.0,2.0,3.0].fold(fun(i,j)= i*x1 - j*x2)",3.0,4.0, -27.0)]
        [TestCase( "y = [1.0,2.0,3.0].fold(fun(i,j)= i*x1 - j*x2)",0.0,0.0, 0.0)]
        public void AnonymousFunctions_TwoArgumentsEquation(string expr, object x1,object x2, object expected) => 
            expr.Calc(("x1", x1), ("x2", x2)).AssertReturns("y", expected);

        [TestCase( "y = [1.0,2.0,3.0].all(fun(i)= i >x)",1.0, false)]
        [TestCase( "y = [1.0,2.0,3.0].map(fun(i)= i*x)",3.0, new []{3.0,6.0,9.0})]
        [TestCase( "y = [1.0,2.0,3.0].all(fun(i)= i >x)",1.0, false)]
        [TestCase( "x:int\r y = [1,2,3].all(fun(i:int)= i >x)",1, false)]
        [TestCase( @"y = [1.0,2.0,3.0].fold(fun(i,j)= x)",123.0, 123.0)]
        [TestCase( @"y = [1.0,2.0,3.0].fold(fun(i,j)= i+j+x)",2.0,10.0)]
        public void AnonymousFunctions_SingleArgumentEquation(string expr, object arg, object expected) => 
            expr.Calc(("x", arg)).AssertReturns("y", expected);

        [TestCase( "z = x*2\r y = [1.0,2.0,3.0].map(fun(i)= i*z)",2.0, new[]{4.0,8.0, 12.0}, 4.0)]
        [TestCase( "z = x*2\r y = [1.0,2.0,3.0].map(fun(i)= i*z)",1.0, new[]{2.0,4.0, 6.0}, 2.0)]
        public void AnonymousFunctions_SingleArgument_twoEquations(string expr, double arg, object yExpected, object zExpected) =>
            expr.Calc("x", arg).AssertReturns(("y", yExpected), ("z", zExpected));

        
        [TestCase( @"y = [11.0,20.0,1.0,2.0].filter (fun it>10)",new[]{11.0,20.0})]
        [TestCase( @"y = [11.0,20.0,1.0,2.0].filter (fun it>10)",new[]{11.0,20.0})]
        [TestCase(@"y = [11.0,20.0,1.0,2.0].filter  (fun it>10)", new[] { 11.0, 20.0 })]

        [TestCase( @"y = [11.0,20.0,1.0,2.0].filter((fun it>10))",new[]{11.0,20.0})]
        [TestCase(@"y = filter([11.0,20.0,1.0,2.0], (fun it>10))", new[] { 11.0, 20.0 })]
        [TestCase(@"y = filter([11.0,20.0,1.0,2.0], fun it>10) ", new[] { 11.0, 20.0 })]

        [TestCase(@"y:int[] = [11,20,1,2].filter(fun it>10)", new[]{11,20})]
        [TestCase( @"y:int[] = map([1,2,3], (fun it*it))",new[]{1,4,9})]
        [TestCase( @"y:int[] = [1,2,3].map(fun it*it)",new[]{1,4,9})]
        [TestCase( @"y = [1.0,2.0,3.0] . map((fun it*it))",new[]{1.0,4.0,9.0})]
        [TestCase( @"y = [1.0,2.0,3.0] . fold(fun it1+it2)",6.0)]
        [TestCase(@"y = fold([1.0,2.0,3.0],(fun it1+it2))", 6.0)]

        [TestCase(@"y = [1,2,3].fold(fun it1+it2)", 6)]
        [TestCase( "y = [1.0,2.0,3.0].any (fun it==1.0)",true)]
        [TestCase( "y = [1.0,2.0,3.0].any (fun it == 0.0)",false)]
        [TestCase( "y = [1.0,2.0,3.0].all (fun it >0)",true)]
        [TestCase( "y = [1.0,2.0,3.0].all (fun it >1.0)",false)]
        [TestCase( "f(m:real[], p):bool = m.all(fun it>p) \r y = f([1.0,2.0,3.0],1.0)",false)]

        [TestCase("y = [-7,-2,0,1,2,3].filter (fun it>0)", new[] { 1, 2, 3 })]
        [TestCase("y = [-1,-2,0,1,2,3].filter (fun it>0) .fold(fun it1+it2)", 6 )]
        [TestCase("y = [-1,-2,0,1,2,3].filter (fun it>0).filter(fun it>2)", new[]{3})]
        [TestCase("y:int[] = [-1,-2,0,1,2,3].filter (fun it>0).map(fun it*it).map(fun it*it)", new[]{1,16,81})]
        [TestCase("y = [-1,-2,0,1,2,3].filter (fun it>0).map(fun it*it).map(fun it*it)", new[]{1,16,81})]

        [TestCase("y = [-1,-2,0,1,2,3].filter(fun it>0).fold(fun it1+it2)", 6 )]
        [TestCase("y = [-1,-2,0,1,2,3].filter(fun it>0).filter(fun it>2)", new[]{3})]
        [TestCase("y = [-1,-2,0,1,2,3].filter(fun it>0).map(fun it*it).map(fun it*it)", new[]{1,16,81})]

        [TestCase("y:int[] = [-1,-2,0,1,2,3].filter(fun it>0).map(fun it*it).map(fun it*it)", new[]{1,16,81})]
        [TestCase("y = [-1,-2,0,1,2,3].filter(fun it>0).filter(fun it>2)", new[]{3})]
        [TestCase("y:int = [-1,-2,0,1,2,3].filter(fun it>0).fold(fun it1+it2)", 6 )]
        [TestCase("y:real = [-1,-2,0,1,2,3].filter(fun it>0).fold(fun it1+it2)", 6.0)]

        [TestCase(@"y = [[1,2],[3,4],[5,6]].map(fun  it.map(fun it+1).sum())", new[]{5,9,13})]
        [TestCase(@"y = [[1,2],[3,4],[5,6]].fold(-10, fun it1+ it2.sum())", 11)]
        [TestCase(@"y = (fun it+1)(3.0)", 4.0)]
        [TestCase(@"f = (fun it+1); y = f(3.0)", 4.0)]
        [TestCase(@"f = ((fun it+1)); y = f(3.0)", 4.0)]
        [TestCase(@"y = ((fun it+1))(3.0)", 4.0)]
        [TestCase(@"y = (((fun it+1)))(3.0)", 4.0)]

        [TestCase(@"car3(g) = g(2); y = car3((fun it-1))   ", 1)]
        [TestCase(@"car4(g) = g(2); y =   car4(fun it)   ", 2)]
        [TestCase(@"car41(g) = g(2); y =   car41 (fun it)   ", 2)]
        [TestCase(@"car4(g) = g(2); y =   car4((fun it))   ", 2)]

        [TestCase(@"call5(f, x) = f(x); y = call5((fun it+1),  1)", 2)]
        [TestCase(@"call6(f, x) = f(x); y = call6((fun it+1.0), 1.0)", 2.0)]

        [TestCase(@"call8(f) = (fun f(it)); y = call8((fun it+1))(2)", 3)]
        [TestCase(@"call9(f) = (fun f(it)); y = ((fun it+1)).call9()(2)", 3)]

        [TestCase(@"call10(f,x) = (fun f(x,it)); y =  max.call10(3)(2)", 3)]
        [TestCase(@"call11() = fun it; y =  call11()(2)", 2)]
        [TestCase(@"call12 = (fun it); y =  call12(2)", 2)]
        [TestCase("ids:int[]=[1,2,3,4]; age:int = 1;  ;y:int[] = ids.filter(fun it>age).map(fun it+1)",new int[]{3,4,5})]
        public void SuperAnonymousFunctions_ConstantEquation(string expr, object expected)
        {
            var runtime = expr.Build();
            runtime.AssertInputsCount(0,"Unexpected inputs on constant equations");
            runtime.Calc().AssertResultHas("y", expected);
        }

        [TestCase( "y = [1.0,2.0,3.0].map(fun it*x1*x2)",3.0,4.0, new []{12.0,24.0,36.0})]
        [TestCase( "x1:int\rx2:int\ry = [1,2,3].map(fun it*x1*x2)",3,4, new []{12,24,36})]
        [TestCase( "y = [1.0,2.0,3.0].fold(fun it1*x1 - it2*x2)",2.0,3.0, -17.0)]
        [TestCase( "y = [1.0,2.0,3.0].fold(fun it1*x1 - it2*x2)",3.0,4.0, -27.0)]
        [TestCase( "y = [1.0,2.0,3.0].fold(fun it1*x1 - it2*x2)",0.0,0.0, 0.0)]
        public void SuperAnonymousFunctions_TwoArgumentsEquation(string expr, object x1,object x2, object expected) =>
            expr.Calc(("x1", x1), ("x2", x2)).AssertReturns("y", expected);

        [TestCase( "y = [1.0,2.0,3.0].all (fun it >x)",1.0, false)]
        [TestCase( "y = [1.0,2.0,3.0].map (fun it*x)",3.0, new []{3.0,6.0,9.0})]
        [TestCase( "y = [1.0,2.0,3.0].all (fun it >x)",1.0, false)]
        [TestCase( "x:int\r y = [1,2,3].all (fun it >x)",1, false)]
        [TestCase( @"y = [1.0,2.0,3.0].fold(fun x)",123.0, 123.0)]
        [TestCase( @"y = [1.0,2.0,3.0].fold(fun it1+it2+x)",2.0,10.0)]
        [TestCase(@"y = [1.0,2.0,3.0].fold(fun it2+x)", 2.0, 5.0)]
        [TestCase(@"y = [1.0,2.0,3.0].fold(fun it1+x)", 2.0, 5.0)]
        [TestCase(@"y = [[1,2],[3,4],[5,6]].map(fun it.map(fun it+x).sum()).sum()", 1, 27)]

        public void SuperAnonymousFunctions_SingleArgumentEquation(string expr, object arg, object expected) => 
            expr.Calc("x", arg).AssertReturns("y", expected);

        [TestCase( "z = x*2\r y = [1.0,2.0,3.0].map(fun it*z)",2.0, new[]{4.0,8.0, 12.0}, 4.0)]
        [TestCase( "z = x*2\r y = [1.0,2.0,3.0].map(fun it*z)",1.0, new[]{2.0,4.0, 6.0}, 2.0)]
        public void SuperAnonymousFunctions_SingleArgument_twoEquations(string expr, double arg, object yExpected, object zExpected) => 
            expr.Calc("x", arg).AssertReturns(("y", yExpected), ("z", zExpected));

        [TestCase("y = [1.0].fold(fun (it1+it2)")]
        [TestCase("y = [1.0].fold(fun It1+it2))")]
        [TestCase("y = [1.0].map(fun It))")]
        [TestCase("y = [1.0].map(fun It1))")]
        [TestCase("y = it")]
        [TestCase("y = it2")]
        [TestCase("y = it1")]
        [TestCase("y = it3")]
        [TestCase("it = x")]
        [TestCase("it1 = x")]
        [TestCase("it2 = x")]
        [TestCase("it3 = x")]
        [TestCase("y = [1.0].fold (it1+it2))")]
        [TestCase("y = [1.0].fold(fun it+it2)")]
        [TestCase("y = [1.0].fold(fun it1+it)")]
        [TestCase("y = [1.0].fold(fun it)")]

        [TestCase("y = [1.0].fold(fun (it1+it2+it3)")]

        [TestCase("y = fold(fun (x) it1+it2)")]
        [TestCase("[1.0,2.0].map(fun it1*it1)")]
        [TestCase("[1.0,2.0].map(fun it1*it)")]
        [TestCase( "x:bool\r y = [1,2,3].all(fun ))")]

        [TestCase( "x:bool\r y = [1,2,3].all(fun i>x))")]
        [TestCase( "x:bool\r y = [1,2,3].all(i>x fun ))")]
        [TestCase( "f(m:real[], p):bool = m.all(fun it>zzz) \r y = f([1.0,2.0,3.0],1.0)")]
        [TestCase( "f(m:real[], p):bool = m.all((fun it>zzz}) \r y = f([1.0,2.0,3.0],1.0)")]
        [TestCase( "x:bool \r y = x and ([1.0,2.0,3.0].all({it >=1.0))")]
        [TestCase( "y = [-x,-x,-x].all(it < 0.0)")]
        [TestCase( "z = [-x,-x,-x] \r  y = z.all(z < 0.0)")]
        [TestCase( "y = [x,2.0,3.0].all((fun it >1.0)")]
        [TestCase("y:int[] = [-1,-2,0,1,2,3].filter (fun it>0).map(fun it1*it2).map(fun it1*it2)")]
        [TestCase("y = [-1,-2,0,1,2,3].filter (fun it>0).map(fun it1*it2).map(fun it1*it2)")]
        [TestCase("y = [1.0].fold(((i,j)->i+j)")]
        [TestCase("y = fold(((i,j),k)->i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((i*2,j)->i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((2,j)->i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((j)->i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((j)->j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((i,j,k)->i+j+k)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((i)->i)")]
        [TestCase("[1.0,2.0].map((i,i)->i+1)")]
        [TestCase("[1.0,2.0].fold((i,i)->i+1)")]
        [TestCase( "x:bool\r y = [1,2,3].all((i)-> i>x)")]
        [TestCase( "f(m:real[], p):bool = m.all((i)-> i>zzz) \r y = f([1.0,2.0,3.0],1.0)")]
        [TestCase( "x:bool \r y = x and ([1.0,2.0,3.0].all((x)-> x >=1.0))")]
        [TestCase( "y = [-x,-x,-x].all((x)-> x < 0.0)")]
        [TestCase( "z = [-x,-x,-x] \r  y = z.all((z)-> z < 0.0)")]
        [TestCase( "y = [x,2.0,3.0].all((x)-> x >1.0)")]
        [TestCase(@"car3(g) = g(2);   y = car3(x->x-1)   ")]
        [TestCase(@"call5(f, x) = f(x); y = call5(x->x+1,  1)")]
        [TestCase(@"call6(f, x) = f(x); y = call6(x->x+1.0, 1.0)")]
        [TestCase(@"call7(f, x) = f(x); y = call7(((x:real)->x+1.0), 1.0)")]
        [TestCase(@"call8(f) = i->f(i); y = call8(x->x+1)(2)")]
        [TestCase(@"call9(f) = i->f(i); y = (x->x+1).call9()(2)")]
        [TestCase(@"mult(x)=y->z->x*y*z;    y = mult(2)(3)(4)")]
        [TestCase(@"mult()= x->y->z->x* y*z; y = mult()(2)(3)(4)")]
        [TestCase( "y = [1.0,2.0,3.0].all((i)-> i >x)")]
        [TestCase( "y = [1.0,2.0,3.0].map((i)-> i*x)")]
        [TestCase( "y = [1.0,2.0,3.0].all((i)-> i >x)")]
        [TestCase( "x:int\r y = [1,2,3].all((i:int)-> i >x)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((i,j)-> x)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((i,j)->i+j+x)")]
        [TestCase( @"y = [11,20,1,2].filter((i:int) -> i>10)")]
        [TestCase( @"y = [11,20,1,2].filter(i:int -> i>10)")]

        
        [TestCase("y = [1.0].fold(((i,j)->i+j)")]
        [TestCase("y = fold(((i,j),k)->i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((i*2,j)->i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((2,j)->i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((j)->i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((j)->j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((i,j,k)->i+j+k)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold((i)->i)")]
        [TestCase("[1.0,2.0].map((i,i)->i+1)")]
        [TestCase("[1.0,2.0].fold((i,i)->i+1)")]
        [TestCase( "x:bool\r y = [1,2,3].all((i)-> i>x)")]
        [TestCase( "f(m:real[], p):bool = m.all((i)-> i>zzz) \r y = f([1.0,2.0,3.0],1.0)")]
        [TestCase( "x:bool \r y = x and ([1.0,2.0,3.0].all((x)-> x >=1.0))")]
        [TestCase( "y = [-x,-x,-x].all((x)-> x < 0.0)")]
        [TestCase( "z = [-x,-x,-x] \r  y = z.all((z)-> z < 0.0)")]
        [TestCase( "y = [x,2.0,3.0].all((x)-> x >1.0)")]
        
        [TestCase("y = [1.0].fold((fun(i,j)=i+j)")]
        [TestCase("y = fold(fun((i,j),k)=i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold(fun(i*2,j)=i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold(fun(2,j)=i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold(fun(j)=i+j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold(fun(j)=j)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold(fun(i,j,k)=i+j+k)")]
        [TestCase( @"y = [1.0,2.0,3.0].fold(fun(i)=i)")]
        [TestCase("[1.0,2.0].map(fun(i,i)=i+1)")]
        [TestCase("[1.0,2.0].fold(fun(i,i)=i+1)")]
        [TestCase( "x:bool\r y = [1,2,3].all(fun(i)= i>x)")]
        [TestCase( "f(m:real[], p):bool = m.all(fun(i)= i>zzz) \r y = f([1.0,2.0,3.0],1.0)")]
        [TestCase( "x:bool \r y = x and ([1.0,2.0,3.0].all(fun(x)= x >=1.0))")]
        [TestCase( "y = [-x,-x,-x].all(fun(x)= x < 0.0)")]
        [TestCase( "z = [-x,-x,-x] \r  y = z.all(fun(z)= z < 0.0)")]
        [TestCase( "y = [x,2.0,3.0].all(fun(x)= x >1.0)")]
        [TestCase( "y = [x,2.0,3.0].all(fun(x):real  x >1.0)")]
        [TestCase( "y = [x,2.0,3.0].all(fun(x):real ==  x >1.0)")]
        [TestCase( "y = [x,2.0,3.0].all(fun(x):real = =  x >1.0)")]
        [TestCase( "y = [x,2.0,3.0].all(fun(x):=real)")]
        [TestCase( "y = [x,2.0,3.0].all(fun(x):real)")]

        [TestCase( "y = [1.0,2.0,3.0].all(fun x = x >1.0)")]
        [TestCase( "y = [1.0,2.0,3.0].all(fun x) = x >1.0)")]

        [TestCase( "y = [1.0,2.0,3.0].all(fun (x = x >1.0)")]
        [TestCase( "y = [1.0,2.0,3.0].all(fun x,y = x >1.0)")]
        [TestCase("y = [-1,-2,0,1,2,3].filter((fun it>0)).filter(fun(i):real=i>2)")]
        [TestCase("y = [-1,-2,0,1,2,3].filter(fun(i):real=i>2)")]
        public void ObviouslyFailsOnParse(string expr) => expr.AssertObviousFailsOnParse();

    }
}