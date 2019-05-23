using NFun.HindleyMilner.Tyso;
using NUnit.Framework;

namespace NFun.HmTests
{
    public class SolveAdapterTest
    {
        private NsHumanizerSolver solver;

        [SetUp]
        public void Init()
        {
            solver = new NsHumanizerSolver();
        }
        [Test]

        public void TextCannotBeUsedInArithmetical()
        {
            solver.SetConst(0, FType.Text);
            solver.SetConst(1, FType.Real);
            Assert.IsFalse( solver.SetArithmeticalOp(2, 0, 1));
        }
        
        [Test]
        public void SimpleArithmeticalOp()
        {
            //3   0 2 1
            //y = x + 1
            
            solver.SetVar(0, "x");
            solver.SetArithmeticalOp(2, 0, 1);
            solver.SetDefenition("y", 3, 2);
            
            var res = solver.Solve();
            Assert.AreEqual(0,res.GenericsCount);
            Assert.AreEqual(FType.Real, res.GetVarType("x"));
            Assert.AreEqual(FType.Real, res.GetVarType("y"));
        }
        
        [Test]
        public void ComplexArithmeticalOp()
        {
            //5   0 4 1 3 2 
            //y = a + 1 * b
            
            solver.SetVar(0, "a");
            solver.SetConst(1, FType.Int32);
            solver.SetVar(2, "b");
            solver.SetArithmeticalOp(3, 1, 2);
            solver.SetArithmeticalOp(4, 0, 3);
            solver.SetDefenition("y", 5, 4);
            
            var res = solver.Solve();
            Assert.AreEqual(0,res.GenericsCount);
            Assert.AreEqual(FType.Real, res.GetVarType("a"));
            Assert.AreEqual(FType.Real, res.GetVarType("b"));
            Assert.AreEqual(FType.Real, res.GetVarType("y"));
        }

        #region  limitCall Expirement
        [Test]
        public void LimitCall_ArithmeticalTypesAreIncostistent_Error()
        {
            //  0 2 1  4 3
            //( x / 2 )<<3
            solver.SetVar(0, "x");
            Assert.IsTrue(solver.SetCall(new CallDef(FType.Real, new[] {2,0, 1})));
            Assert.IsFalse(solver.SetBitShiftOperator(4, 2, 3));
        }
        
        [Test]
        public void LimitCall_UpcastArgType_EquationSolved()
        {
            // 3   0 2 1   5   4 
            // y = a / b;  a = 1:int
            solver.SetVar(0,"a");
            solver.SetVar(1,"b");
            solver.SetCall(new CallDef(FType.Real, new []{2,0,1}));
            solver.SetDefenition("y", 3, 2);
            solver.SetConst(4,FType.Int32);
            Assert.IsTrue(solver.SetDefenition("a", 5, 4));

            var solvation = solver.Solve();
            Assert.AreEqual(FType.Real, solvation.GetVarType("y"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("a"));
            Assert.AreEqual(0, solvation.GenericsCount);
        }
        
        [Test]
        public void LimitCall_ComplexEquations_TypesSolved()
        {
            // 3   0 2 1  7   4 6  5  11  8 10 9
            // r = x + y; i = y << 2; x = 3 /  2
            solver.SetVar(0, "x");
            solver.SetVar(1, "y");
            solver.SetArithmeticalOp(2,0,1);
            solver.SetDefenition("r", 3, 2);

            solver.SetVar(4, "y");
            solver.SetConst(5, FType.Int32);
            solver.SetBitShiftOperator(6,4,5);
            solver.SetDefenition("i", 7, 6);

            solver.SetConst(8, FType.Int32);
            solver.SetConst(9, FType.Int32);
            solver.SetCall(new CallDef(FType.Real, new[] {10, 8, 9}));
            solver.SetDefenition("x", 11, 10);

            var solvation = solver.Solve();

            Assert.AreEqual(FType.Real, solvation.GetVarType("x"));
            Assert.AreEqual(FType.Real, solvation.GetVarType("r"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("y"));
        }
       
        
               
        #endregion
        
        
        
        [Test]
        public void ArithmeticalTypesAreIncostistent_Error()
        {
            //  0 2 1  4 3
            //( x / 2 )<<3
            solver.SetVar(0, "x");
            Assert.IsTrue(solver.SetCall(new CallDef(FType.Real, new[] {2,0, 1})));
            Assert.IsFalse(solver.SetBitShiftOperator(4, 2, 3));
        }
        [Test]
        public void TwoIntTypes_ResultTypeIsInteger()
        {
            //  1   0  2 4 3
            //  x = 1; x<<3 
            solver.SetConst(0, FType.Int32);
            solver.SetDefenition("x", 1, 0);
            Assert.IsTrue(solver.SetBitShiftOperator(4, 2, 3));
            Assert.AreEqual(FType.Int32, solver.Solve().GetVarType("x"));
        }

        #region lca-If
            
        [Test]
        public void ConstantLcaIf_ResultTypeIsInteger()
        {
            //  4   3    1   0      2
            //  x = if(true) 1 else 2 
            solver.SetConst(0, FType.Int32);
            solver.SetConst(1, FType.Bool);
            solver.SetConst(2, FType.Int32);
            solver.ApplyLcaIf(3, new[] {1}, new[] {0, 2});
            solver.SetDefenition("x", 4, 3);
            Assert.AreEqual(FType.Int32, solver.Solve().GetVarType("x"));
        }
        [Test]
        public void ConstantLcaOneOfThemIf_ResultTypeIsInteger()
        {
            //  4   3    1   0      2
            //  x = if(true) 1 else 2 
            solver.SetConst(0, FType.Int32);
            solver.SetConst(1, FType.Bool);
            solver.SetConst(2, FType.Int32);
            solver.ApplyLcaIf(3, new[] {1}, new[] {0, 2});
            solver.SetDefenition("x", 4, 3);
            Assert.AreEqual(FType.Int32, solver.Solve().GetVarType("x"));
        }
        #endregion
        
        
        [Test]
        public void FunctionSumDefenition_AllTypesAreReal()
        {
            //   5             0 2 1 4 3
            //  myfun(a,b,c) = a + b + c 
            solver.SetVar(0, "a");
            solver.SetVar(1, "b");
            solver.SetArithmeticalOp(2,0,1);
            solver.SetVar(3, "c");
            solver.SetArithmeticalOp(4,2,3);
            solver.SetDefenition("myFun", 5, 4);

            var solvation = solver.Solve();
            Assert.AreEqual(FType.Real, solvation.GetVarType("myFun"));
            Assert.AreEqual(FType.Real, solvation.GetVarType("a"));
            Assert.AreEqual(FType.Real, solvation.GetVarType("b"));
            Assert.AreEqual(FType.Real, solvation.GetVarType("c"));
        }
        
        [Test]
        public void FunctionSumDefenition_SingleInputIsInt_OtherTypesAreRealW()
        {
            //   5                 0 2 1 4 3
            //  myfun(a:int,b,c) = a + b + c 
            solver.SetVar(0, "a");
            solver.SetConst(0, FType.Int32);//todo
            solver.SetVar(1, "b");
            solver.SetArithmeticalOp(2,0,1);
            solver.SetVar(3, "c");
            solver.SetArithmeticalOp(4,2,3);
            solver.SetDefenition("myFun", 5, 4);

            var solvation = solver.Solve();
            Assert.AreEqual(FType.Real, solvation.GetVarType("myFun"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("a"));
            Assert.AreEqual(FType.Real, solvation.GetVarType("b"));
            Assert.AreEqual(FType.Real, solvation.GetVarType("c"));
        }
        
        [Test]
        public void FunctionSumDefenition_retunsInt_AllTypesAreInt()
        {
            //   5                   0 2 1 4 3
            //  myfun(a,b,c):int64 = a + b + c 
            solver.SetVarType("myFun", FType.Int64); 
            solver.SetVar(0, "a");
            solver.SetVar(1, "b");
            solver.SetArithmeticalOp(2,0,1);
            solver.SetVar(3, "c");
            solver.SetArithmeticalOp(4,2,3);
            solver.SetDefenition("myFun", 5, 4);
            var solvation = solver.Solve();
            Assert.AreEqual(FType.Int64, solvation.GetVarType("myFun"));
            Assert.AreEqual(FType.Int64, solvation.GetVarType("a"));
            Assert.AreEqual(FType.Int64, solvation.GetVarType("b"));
            Assert.AreEqual(FType.Int64, solvation.GetVarType("c"));
        }
        
        
        
        [Test]
        public void SummReducecByBitShift_AllTypesAreInt()
        {
            //  0 2 1  4 3
            //( x + y )<<3
            solver.SetVar(0, "x");
            solver.SetVar(1, "y");
            Assert.IsTrue(solver.SetArithmeticalOp(2,0,1));
            Assert.IsTrue(solver.SetBitShiftOperator(4, 2, 3));
            var solvation = solver.Solve();
            
            Assert.AreEqual(FType.Int32, solvation.GetVarType("x"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("y"));
        }
        
        [Test]
        public void TwoTypesAreLong_ItsSumIsLong()
        {
            //1   0   3    2  7   4 6 5
            //a = 1l; b = 1l; x = a + b
            solver.SetConst(0, FType.Int64);
            solver.SetDefenition("a", 1, 0);
            solver.SetConst(2, FType.Int64);
            solver.SetDefenition("b", 3, 2);

            solver.SetVar( 4,"a");
            solver.SetVar( 5,"b");
            solver.SetArithmeticalOp(6, 4, 5);
            solver.SetDefenition("x", 7, 6);

            var solvation = solver.Solve();

            Assert.AreEqual(FType.Int64, solvation.GetVarType("x"));
        }
        
        [Test]
        public void UpcastArgTypeThatIsAfter_EquationSolved()
        {
            // 3      2    0,1    5   4 
            // y = sumReal(a,b);  a = 1:int
            solver.SetVar(0,"a");
            solver.SetVar(1,"b");
            solver.SetCall(new CallDef(FType.Real, new []{2,0,1}));
            solver.SetDefenition("y", 3, 2);
            solver.SetConst(4,FType.Int32);
            Assert.IsTrue(solver.SetDefenition("a", 5, 4));

            var solvation = solver.Solve();
            Assert.AreEqual(FType.Real, solvation.GetVarType("y"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("a"));
            Assert.AreEqual(0, solvation.GenericsCount);
        }
        
        [Test]
        public void UpcastArgTypes_BothTypesLimitedAfter_EquationSolved()
        {
            // 3      2    0,1    5   4      9   6 8  7     
            // y = sumReal(a,b);  a = 1:int; b = a << 2
            solver.SetVar(0,"a");
            solver.SetVar(1,"b");
            solver.SetCall(new CallDef(FType.Real, new []{2,0,1}));
            solver.SetDefenition("y", 3, 2);
            solver.SetConst(4,FType.Int32);
            Assert.IsTrue(solver.SetDefenition("a", 5, 4));
            solver.SetVar(6, "a");
            solver.SetConst(7,FType.Int32);
            Assert.IsTrue(solver.SetBitShiftOperator(8, 6, 7));
            Assert.IsTrue(solver.SetDefenition("b", 9, 8));
            
            var solvation = solver.Solve();
            Assert.AreEqual(FType.Real, solvation.GetVarType("y"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("a"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("b"));
            Assert.AreEqual(0, solvation.GenericsCount);
        }
        
        [Test]
        public void UpcastArgTypeThatIsBefore_EquationSolved()
        {
            // 1      0   5      4    2 3    
            // a = 1:int; y = sumReal(a,b);   
            solver.SetConst(0,FType.Int32);
            solver.SetDefenition("a", 1, 0);
                
            solver.SetVar(2,"a");
            solver.SetVar(3,"b");
            
            solver.SetCall(new CallDef(FType.Real, new []{4,2,3}));
            Assert.IsTrue(solver.SetDefenition("y", 5, 4));
            
            var solvation = solver.Solve();
            Assert.AreEqual(FType.Real, solvation.GetVarType("y"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("a"));
            Assert.AreEqual(0, solvation.GenericsCount);
        }
        
        [Test]
        public void UpcastArgType_ArithmOp_EquationSolved()
        {
            // 1    0        5   2 4 3   7   6 
            // a = 1.0:real; y = a + b;  b = 1:int
            solver.SetConst(0,FType.Real);
            solver.SetDefenition("a", 1, 0);
            
            solver.SetVar(2,"a");
            solver.SetVar(3,"b");
            solver.SetArithmeticalOp(4, 2, 3);
            solver.SetDefenition("y", 5, 4);

            solver.SetConst(6,FType.Int32);
            Assert.IsTrue(solver.SetDefenition("b",7,6));

            var solvation = solver.Solve();
            Assert.AreEqual(FType.Real, solvation.GetVarType("y"));
            Assert.AreEqual(FType.Real, solvation.GetVarType("a"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("b"));
            Assert.AreEqual(0, solvation.GenericsCount);
        }
        
        [Test]
        public void ComplexEquations_TypesSolved()
        {
            // 3   0 2 1  7   4 6  5  11  8 10 9
            // r = x + y; i = y << 2; x = 3 /  2
            solver.SetVar(0, "x");
            solver.SetVar(1, "y");
            solver.SetArithmeticalOp(2,0,1);
            solver.SetDefenition("r", 3, 2);

            solver.SetVar(4, "y");
            solver.SetConst(5, FType.Int32);
            solver.SetBitShiftOperator(6,4,5);
            solver.SetDefenition("i", 7, 6);

            solver.SetConst(8, FType.Int32);
            solver.SetConst(9, FType.Int32);
            solver.SetCall(new CallDef(FType.Real, new[] {10, 8, 9}));
            solver.SetDefenition("x", 11, 10);

            var solvation = solver.Solve();

            Assert.AreEqual(FType.Real, solvation.GetVarType("x"));
            Assert.AreEqual(FType.Real, solvation.GetVarType("r"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("y"));
        }
        [Test]
        public void Arithm_WithTwoInts_EqualsInt()
        {
            solver.SetConst(0, FType.Int32);
            solver.SetConst(1, FType.Int32);
            solver.SetArithmeticalOp(2, 0, 1);
            solver.SetDefenition("y", 3, 2);
            var solvation = solver.Solve();
            Assert.IsTrue(solvation.IsSolved);
            Assert.AreEqual(FType.Int32, solvation.GetVarType("y"));
        }
        [Test]
        public void ConstantIf_ResultTypeIsInteger()
        {
            //  4   3    1   0      2
            //  x = if(true) 1 else 2 
            solver.SetConst(0, FType.Int32);
            solver.SetConst(1, FType.Bool);
            solver.SetConst(2, FType.Int32);
            solver.ApplyLcaIf(3, new[] {1}, new[] {0,2});
            solver.SetDefenition("x", 4, 3);
            
            Assert.AreEqual(FType.Int32, solver.Solve().GetVarType("x"));
        }
        [Test]
        public void If_WithTwoInts_equalInt()
        {
            //4    3     1   0      2
            //y  = if (true) 1 else 2
            solver.SetConst(0, FType.Int32);
            solver.SetConst(1, FType.Bool);
            solver.SetConst(2, FType.Int32);

            solver.ApplyLcaIf(3, new[] {1}, new[] {0,2});
            solver.SetDefenition("y", 4, 3);
            var solvation = solver.Solve();
            Assert.IsTrue(solvation.IsSolved);

            Assert.AreEqual(FType.Int32, solvation.GetVarType("y"));
        }
        [Test]
        public void If_withOneIntAndOneVar_equalReal()
        {
            //4    3     1   0      2
            //y  = if (true) 1 else x
            solver.SetConst(0, FType.Int32);
            solver.SetConst(1, FType.Bool);
            solver.SetVar(2,"x");
            Assert.IsTrue(solver.ApplyLcaIf(3, new[] {1}, new[] {0,2}));
            Assert.IsTrue(solver.SetDefenition("y", 4, 3));
            
            var solvation = solver.Solve();
            Assert.IsTrue(solvation.IsSolved);

            Assert.AreEqual(0,solvation.GenericsCount);
            Assert.AreEqual(FType.Int32, solvation.GetVarType("y"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("x"));
        }
        
        [Test]
        public void If_withMultipleAncestorRules_EquationSolved()
        {
            //4     3     1   0      2  6    5   10    7 9 8
            //y1  = if (true) 1 else x; y2 = y1; y3 = y1 * 2
            solver.SetConst(0, FType.Int32);
            solver.SetConst(1, FType.Bool);
            solver.SetVar(2,"x");
            Assert.IsTrue(solver.ApplyLcaIf(3, new[] {1}, new[] {0,2}));
            solver.SetVar(5,"y1");
            Assert.IsTrue(solver.SetDefenition("y2", 6, 5));
            solver.SetVar(7,"y1");
            solver.SetConst(8, FType.Int32);
            solver.SetArithmeticalOp(9, 7, 8);
            Assert.IsTrue(solver.SetDefenition("y3", 10, 9));
            
            
            var solvation = solver.Solve();
            Assert.IsTrue(solvation.IsSolved);

            Assert.AreEqual(0,solvation.GenericsCount);
            Assert.AreEqual(FType.Real, solvation.GetVarType("y1"));
            Assert.AreEqual(FType.Real, solvation.GetVarType("y2"));
            Assert.AreEqual(FType.Real, solvation.GetVarType("y3"));

            Assert.AreEqual(FType.Int32, solvation.GetVarType("x"));
        }
        
        [Test]
        public void MultipleAncestors_EquationSolved()
        {
            //1     0     3    2   7    4 6 5
            //y1  = y0;  y2 = y1; y3 = y2 * 2
            solver.SetVar(0,"y0");
            solver.SetDefenition("y1",1,0);
            
            solver.SetVar(2,"y1");
            solver.SetDefenition("y2",3,2);

            solver.SetVar(4,"y2");
            solver.SetConst(6, FType.Int32);
            solver.SetArithmeticalOp(6, 4, 5);
            solver.SetDefenition("y3",7,6);

            var solvation = solver.Solve();
            Assert.IsTrue(solvation.IsSolved);

            Assert.AreEqual(0,solvation.GenericsCount);
            Assert.AreEqual(FType.Int32, solvation.GetVarType("y1"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("y2"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("y3"));
        }
        
        [Test]
        public void ReverseMultipleAncestors_EquationSolved()
        {
            //1     0     3    2   7    4 6 5
            //y1  = y0;  y2 = y1; y3 = y0 * 2
            solver.SetVar(0,"y0");
            solver.SetDefenition("y1",1,0);
            
            solver.SetVar(2,"y1");
            solver.SetDefenition("y2",3,2);

            solver.SetVar(4,"y0");
            solver.SetConst(6, FType.Int32);
            solver.SetArithmeticalOp(6, 4, 5);
            solver.SetDefenition("y3",7,6);

            var solvation = solver.Solve();
            Assert.IsTrue(solvation.IsSolved);

            Assert.AreEqual(0,solvation.GenericsCount);
            Assert.AreEqual(FType.Int32, solvation.GetVarType("y1"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("y2"));
            Assert.AreEqual(FType.Int32, solvation.GetVarType("y3"));
        }
        [Test]
        public void TypeSpecified_PutHighterType_EquationSOlved()
        {
            //         1    0  
            //a:real;  a = 1:int32

            solver.SetVarType("a", FType.Real);
            
            solver.SetConst(0, FType.Int32);
            Assert.True(solver.SetDefenition("a", 1,0));
            var solvation = solver.Solve();
            Assert.AreEqual(FType.Real, solvation.GetVarType("a"));
        }

        [Test]
        public void TypeLimitSet_ThanChangedToLower_LowerLimitAccepted()
        {
            //1   0       3   2
            //a = 1:int;  a = 1.0:int64

            solver.SetConst(0, FType.Int32);
            Assert.True(solver.SetDefenition("a", 1,0));
            solver.SetConst(2, FType.Int64);
            
            Assert.True(solver.SetDefenition("a", 3, 2));
             
            var solvation = solver.Solve();
            Assert.IsTrue(solvation.IsSolved);

            Assert.AreEqual(FType.Int64, solvation.GetVarType("a"));
        }
        [Test]
        public void TypeLimitSet_ThanChangedToHigher_LowerLimitAccepted()
        {
            //1   0          3   2
            //a = 1:int64;  a = 1.0:int32

            solver.SetConst(0, FType.Int64);
            Assert.True(solver.SetDefenition("a", 1,0));
            solver.SetConst(2, FType.Int32);
            
            Assert.True(solver.SetDefenition("a", 3, 2));
             
            var solvation = solver.Solve();
            Assert.IsTrue(solvation.IsSolved);

            Assert.AreEqual(FType.Int64, solvation.GetVarType("a"));
        }
        
        [Test]
        public void ArgTypesAreSpecified_OutputTypeIsCorrect()
        {
            //          3   1 2 0
            //a,b:long; y = a + b #y:long
            solver.SetVarType("a", FType.Int64);
            solver.SetVarType("b", FType.Int64);

            solver.SetVar(0,"a");
            solver.SetVar(1,"b");
            solver.SetArithmeticalOp(2, 1, 0);
            solver.SetDefenition("y", 3, 2);
            var solvation = solver.Solve();
            Assert.IsTrue(solvation.IsSolved);

            Assert.AreEqual(FType.Int64, solvation.GetVarType("y"));
        }
        [Test]
        public void ArgTypesSpecified_InvalidOperationCausesError()
        {
            //        1 2 0
            //x:real; x<< 3 #error
            solver.SetVarType("x", FType.Real);
            
            solver.SetVar(1,"x");
            solver.SetConst(0, FType.Real);
            Assert.IsFalse(solver.SetBitShiftOperator(2, 1, 0));
        }   
    }
}