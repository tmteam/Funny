using System.Runtime.CompilerServices;
using NFun;
using NFun.Interpritation;
using NFun.Runtime;
using NFun.Types;

namespace NFun.InfinityProfiling.Sets
{
    public class ProfileCalculateSet: IProfileSet
    {
        private readonly FlatMutableFunctionDictionary _dictionary;
        private readonly FunRuntime _constTrueRuntime;
        private readonly FunRuntime _const1Runtime;
        private readonly FunRuntime _constTextRuntime;
        private readonly FunRuntime _constBoolArrayRuntime;
        private readonly FunRuntime _constRealArrayRuntime;
        private readonly FunRuntime _calcSingleBool;
        private readonly FunRuntime _calcReal;
        private readonly FunRuntime _calcText;
        private readonly FunRuntime _calcKxb;
        private readonly FunRuntime _calcRealArray;
        private readonly FunRuntime _calcFourArgs;
        private readonly FunRuntime _constMultiplyArrayItems;
        private readonly FunRuntime _constDummyBubbleSort;
        private readonly FunRuntime _constEverything;
        private readonly FunRuntime _calcIntOp;
        private readonly FunRuntime _calcRealOp;
        private readonly FunRuntime _calcBoolOp;
        private readonly FunRuntime _calcSquareEquation;
        private readonly FunRuntime _calcTextOp;
        private readonly FunRuntime _calcInterpolation;
        private readonly FunRuntime _calcGenericFunc;
        private readonly FunRuntime _constBool;
        private readonly FunRuntime _constInterpolation;
        private readonly FunRuntime _constSquareEquation;
        private readonly FunRuntime _constGenericFunc;
        private readonly FunRuntime _primitiveConstIntSimpleArithmetics;
        private readonly FunRuntime _primitiveConstRealSimpleArithmetics;
        private readonly FunRuntime _primitiveConstBoolSimpleArithmetics;
        private readonly FunRuntime _primitiveCalcReal2Var;
        private readonly FunRuntime _primitiveCalcInt2Var;
        public ProfileCalculateSet()
        {

            _dictionary = BaseFunctions.CreateDefaultDictionary();
            _primitiveConstIntSimpleArithmetics = Build(Scripts.PrimitiveConstIntSimpleArithmetics);
            _primitiveConstRealSimpleArithmetics = Build(Scripts.PrimitiveConstRealSimpleArithmetics);
            _primitiveConstBoolSimpleArithmetics = Build(Scripts.PrimitiveConstBoolSimpleArithmetics);
            _primitiveCalcReal2Var = Build(Scripts.PrimitiveCalcReal2Var);
            _primitiveCalcInt2Var = Build(Scripts.PrimitiveCalcInt2Var);

            _calcIntOp = Build(Scripts.CalcIntOp);
            _calcRealOp = Build(Scripts.CalcRealOp);
            _calcBoolOp = Build(Scripts.CalcBoolOp);
            _calcSquareEquation = Build(Scripts.CalcSquareEquation);
            _calcTextOp = Build(Scripts.CalcTextOp);
            _calcInterpolation = Build(Scripts.CalcInterpolation);
            _calcGenericFunc = Build(Scripts.CalcGenericFunc);

            _constTrueRuntime = Build(Scripts.ConstTrue);
            _const1Runtime = Build(Scripts.Const1);
            _constTextRuntime = Build(Scripts.ConstText);
            _constBoolArrayRuntime = Build(Scripts.ConstBoolArray);
            _constRealArrayRuntime = Build(Scripts.ConstRealArray);
            _calcSingleBool = Build(Scripts.CalcSingleBool);
            _constBool = Build(Scripts.ConstBoolOp);
            _calcReal = Build(Scripts.CalcSingleReal);
            _calcText = Build(Scripts.CalcSingleText);
            _calcKxb = Build(Scripts.CalcKxb);
            _calcRealArray = Build(Scripts.CalcRealArray);
            _calcFourArgs = Build(Scripts.CalcFourArgs);
            _constMultiplyArrayItems = Build(Scripts.MultiplyArrayItems);
            _constDummyBubbleSort = Build(Scripts.DummyBubbleSort);
            _constEverything = Build(Scripts.Everything);

            _constInterpolation = Build(Scripts.ConstInterpolation);
            _constSquareEquation = Build(Scripts.ConstSquareEquation);
            _constGenericFunc = Build(Scripts.ConstGenericFunc);
        }

        private FunRuntime Build(string expr) => FunBuilder.With(expr).With(_dictionary).Build();

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void PrimitivesConstTrue() => _constTrueRuntime.Calculate();

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void PrimitivesConstBool() => _constBool.Calculate();

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void PrimitivesConst1() => _const1Runtime.Calculate();

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void ConstText() => _constTextRuntime.Calculate();

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void ConstBoolArray() => _constBoolArrayRuntime.Calculate();
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void ConstRealArray() => _constRealArrayRuntime.Calculate();

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void ConstInterpolation() => _constInterpolation.Calculate();

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void ConstGenericFunc() => _constGenericFunc.Calculate();

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void ConstSquareEquation() => _constSquareEquation.Calculate();

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void PrimitiveConstIntSimpleArithmetics() => _primitiveConstIntSimpleArithmetics.Calculate();
        [MethodImpl(MethodImplOptions.NoOptimization)]

        public void PrimitiveConstRealSimpleArithmetics() => _primitiveConstRealSimpleArithmetics.Calculate();
        [MethodImpl(MethodImplOptions.NoOptimization)]

        public void PrimitiveConstBoolSimpleArithmetics() => _primitiveConstBoolSimpleArithmetics.Calculate();
        [MethodImpl(MethodImplOptions.NoOptimization)]

        public void PrimitiveCalcReal2Var() => _primitiveCalcReal2Var.Calculate(VarVal.New("a",42.1),VarVal.New("b",24.0));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void PrimitiveCalcInt2Var() => _primitiveCalcInt2Var.Calculate(VarVal.New("a",42),VarVal.New("b",24));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void PrimitiveCalcSingleBool() => _calcSingleBool.Calculate(VarVal.New("x",true));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void PrimitiveCalcSingleReal() => _calcReal.Calculate(VarVal.New("x", 42.1));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void CalcSingleText()=> _calcText.Calculate(VarVal.New("x", "kavabanga"));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void PrimitiveCalcIntOp() => _calcIntOp.Calculate(VarVal.New("x",1));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void PrimitiveCalcRealOp() => _calcRealOp.Calculate(VarVal.New("x",2.0));
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void PrimitiveCalcBoolOp()=> _calcBoolOp.Calculate(VarVal.New("x",true));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void CalcTextOp() => _calcTextOp.Calculate(VarVal.New("x","vasa"));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void CalcInterpolation() => _calcInterpolation.Calculate(VarVal.New("a",2.0),VarVal.New("b",3.4));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void CalcGenericFunc() => _calcGenericFunc.Calculate(VarVal.New("a",1.0),VarVal.New("b",2.0),VarVal.New("c",3.0));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void CalcSquareEquation() => _calcSquareEquation.Calculate(VarVal.New("a",1.0),VarVal.New("b",10.1),VarVal.New("c",1.2));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void PrimitivesCalcKxb()=> _calcKxb.Calculate(VarVal.New("x", 12.3));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void CalcRealArray()=> _calcRealArray.Calculate(VarVal.New("x", 3.14));
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void CalcFourArgs()=> _calcFourArgs.Calculate(
            VarVal.New("a", 1.1),
            VarVal.New("b", 42.1),
            VarVal.New("c", 4.1),
            VarVal.New("d", "kotopes")
            );
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void ComplexConstMultiArrays()=> _constMultiplyArrayItems.Calculate();
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void ComplexDummyBubble() => _constDummyBubbleSort.Calculate();
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void ComplexConstEverything() => _constEverything.Calculate();
    }
}