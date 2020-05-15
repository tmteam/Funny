﻿using System.Collections.Generic;
using System.Text;
using NFun.Tic.SolvingStates;
using NFun.TypeInferenceCalculator;
using NUnit.Framework;
using Array = System.Array;

namespace NFun.Tic.Tests
{
    public class UserFunctionsTest
    {
        [SetUp] public void Initiazlize() => TraceLog.IsEnabled = true;
        [TearDown] public void Finalize() => TraceLog.IsEnabled = false;

        [Test]
        public void SortOneTimeUserFunction()
        {
        // fun swapIfNotSorted(T_0[],Int32):T_0[]  where T_0: <>

        //                       6  32   1      0         4      5
        //sortOneTime(input) = fold([0..count(input)], input, swapIfNotSorted)";
        //Exit: 16.Tvar input: Empty
        //Exit:19.IntConst 0:int
        //Exit:21.VAR input
        //Exit:20.Call count(21, 20)
        //Exit: 18.Call range(19, 20, 18)
        //Exit: 22.VAR input
        //Exit:23.VAR swapIfNotSorted
        //Exit:17.Call fold(18, 22, 23, 17)



            var graph = new GraphBuilder();
            graph.SetVar("input", 0);
            var fundef = graph.SetFunDef("sortOneTime", 6, null, "input");

            graph.SetSizeOfArrayCall(0, 1); //count
            graph.SetIntConst(2, Primitive.U8);
            graph.SetRangeCall(2, 1, 3); //range
            graph.SetVar("input", 4);

            var tOfSwap = graph.InitializeVarNode(isComparable: true);
            graph.SetVarType("swapIfNotSorted",
                Fun.Of(new IState[] {SolvingStates.Array.Of(tOfSwap), Primitive.I32}, SolvingStates.Array.Of(tOfSwap)));
            graph.SetVar("swapIfNotSorted", 5);

            graph.SetfoldCall(3, 4, 5, 6);

            var result = graph.Solve();
            var generic = result.AssertAndGetSingleGeneric(null, null, true);
            result.AssertNamed(SolvingStates.Array.Of(generic), "input");
            result.AssertNamed(Fun.Of(SolvingStates.Array.Of(generic), SolvingStates.Array.Of(generic)), "sortOneTime");
        }

        [Test]
        public void SortOneTimeUserFunction_WithSameOrderAsInNfun()
        {
            // fun swapIfNotSorted(T_0[],Int32):T_0[]  where T_0: <>

            //                       17     18   19  20     21       22      23
            //sortOneTime(input) = fold( range(0, count(input)), input, swapIfNotSorted)";
            
            //Nfun Trace:
            //Exit: 16.Tvar input: Empty
            //Exit:19.IntConst 0:int
            //Exit:21.VAR input
            //Exit:20.Call count(21, 20)
            //Exit: 18.Call range(19, 20, 18)
            //Exit: 22.VAR input
            //Exit:23.VAR swapIfNotSorted
            //Exit:17.Call fold(18, 22, 23, 17)

            var graph = new GraphBuilder();
            var fundef = graph.SetFunDef("sortOneTime", 17, null, "input");

            //Exit:19.IntConst 0:int
            graph.SetIntConst(19, Primitive.U8);
            //Exit:21.VAR input
            graph.SetVar("input", 21);
            //Exit:20.Call count(21, 20)
            graph.SetSizeOfArrayCall(21, 20); //count
            //Exit: 18.Call range(19, 20, 18)
            graph.SetRangeCall(19, 20, 18); //range
            //Exit: 22.VAR input

            graph.SetVar("input", 22);
            //Exit:23.VAR swapIfNotSorted
            var tOfSwap = graph.InitializeVarNode(isComparable: true);
            graph.SetVarType("swapIfNotSorted",
                Fun.Of(new IState[] {SolvingStates.Array.Of(tOfSwap), Primitive.I32}, SolvingStates.Array.Of(tOfSwap)));
            graph.SetVar("swapIfNotSorted", 23);
            //Exit:17.Call fold(18, 22, 23, 17)
            graph.SetfoldCall(18, 22, 23, 17);

            var result = graph.Solve();
            var generic = result.AssertAndGetSingleGeneric(null, null, true);
            result.AssertNamed(SolvingStates.Array.Of(generic), "input");
            result.AssertNamed(Fun.Of(SolvingStates.Array.Of(generic), SolvingStates.Array.Of(generic)), "sortOneTime");
        }
    }
}