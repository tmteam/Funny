using System;
using System.Collections;
using System.Linq;
using NFun.Runtime;
using NFun.Runtime.Arrays;
using NUnit.Framework;

namespace Funny.Tests.UnitTests
{
    [TestFixture]
    public class FunArrayTest
    {
        [TestCase(new[]{1,2,3}, null, null, null, new[]{1,2,3})]
        [TestCase(new[]{1,2,3}, 1, null, null, new[]{2,3})]
        [TestCase(new[]{1,2,3}, 1, 1, null, new[]{2})]
        [TestCase(new[]{1,2,3}, 1, 1, 1, new[]{2})]
        [TestCase(new[]{1,2,3}, 1, 1, 2, new[]{2})]
        [TestCase(new[]{1,2,3}, 1, 1, 100, new[]{2})]
        [TestCase(new[]{1,2,3}, null, 1, null, new[]{1,2})]
        [TestCase(new[]{1,2,3}, null, 1, 1, new[]{1,2})]
        [TestCase(new[]{1,2,3}, null, 1, 2, new[]{1})]
        [TestCase(new[]{1,2,3}, null, 1, 100, new[]{1})]
        [TestCase(new[]{1,2,3}, null, 1, 100, new[]{1})]
        [TestCase(new[]{1,2,3}, 100, null, null, new int[0])]
        [TestCase(new[]{1,2,3}, 100, 200, null, new int[0])]
        [TestCase(new[]{1,2,3}, 100, 200, 2, new int[0])]
        [TestCase(new[]{1,2,3}, 0, 3, null, new[]{1,2,3})]
        [TestCase(new[]{1,2,3}, 0, 200, null, new[]{1,2,3})]
        [TestCase(new[]{1,2,3}, 0, 200, 1, new[]{1,2,3})]
        [TestCase(new[]{1,2,3}, null, 0, null, new []{1})]
        [TestCase(new[]{1,2,3}, null, 0, 100, new []{1})]
        [TestCase(new[]{1,2,3}, 0, 0, 100, new []{1})]
        [TestCase(new[]{1,2,3}, null, null, 2, new []{1,3})]
        [TestCase(new[]{1,2,3,4}, null, 3, 2, new []{1,3})]
        [TestCase(new[]{1,2,3,4}, 0, 3, 2, new []{1,3})]
        [TestCase(new int[0], null, 0, 100, new int[0])]
        [TestCase(new int[0], 0, 0, 100, new int[0])]
        [TestCase(new int[0], 1, 3, null, new int[0])]
        [TestCase(new int[0], null, null, null, new int[0])]
        public void IntArr_Slice_ReturnsExpected(int[] origin, int? start, int? end, int? step, int[] expected)
        {
            var funOrigin = new ImmutableFunArray(origin);
            var funExpected = new ImmutableFunArray(expected);
            CollectionAssert.AreEquivalent(funExpected,funOrigin.Slice(start,end,step));
        }
        [Test]
        public void IntArr_Equialent_IsEquivalentReturnsTrue()
        {
            var a1 = new ImmutableFunArray(new [] {1, 2, 3});
            var a2 = new ImmutableFunArray(new [] {1, 2, 3});
            Assert.IsTrue(a1.IsEquivalent(a2));
        }
        [Test]
        public void IntArr_NotEquialent_IsEquivalentReturnsFalse()
        {
            var a1 = new ImmutableFunArray(new [] {1, 2, 3});
            var a2 = new ImmutableFunArray(new [] {1, 2});
            Assert.IsFalse(a1.IsEquivalent(a2));
        }
        [Test]
        public void IntArr_BothEmpty_IsEquivalentReturnsTrue()
        {
            var a1 = new ImmutableFunArray(new object[0]);
            var a2 = new ImmutableFunArray(new object[0]);
            Assert.IsTrue(a1.IsEquivalent(a2));
        }
        
        [Test]
        public void IntArrOfArr_BothEmpty_IsEquivalentReturnsTrue()
        {
            var a1 = new ImmutableFunArray(new object[0]);
            var a2 = new ImmutableFunArray(new object[0]);
            Assert.IsTrue(a1.IsEquivalent(a2));
        }
        
        [Test]
        public void IntArrOfArr_Equivalent_IsEquivalentReturnsTrue()
        {

            var a1 = new ImmutableFunArray(
                new ImmutableFunArray(new object[0]),
                new ImmutableFunArray(new object[] {1, 2}),
                new ImmutableFunArray(new object[] {1, 2,3})
                );
            var a2 = new ImmutableFunArray(
                new ImmutableFunArray(new object[0]),
                new ImmutableFunArray(new object[] {1, 2}),
                new ImmutableFunArray(new object[] {1, 2,3})
            );

            Assert.IsTrue(a1.IsEquivalent(a2));
        }
        [Test]
        public void IntArrOfArr_NotEquivalent_IsEquivalentReturnsFalse()
        {

            var a1 = new ImmutableFunArray(
                new ImmutableFunArray(new object[0]),
                new ImmutableFunArray(new object[] {1, 2}),
                new ImmutableFunArray(new object[] {1, 2,3})
            );
            var a2 = new ImmutableFunArray(
                new ImmutableFunArray(new object[0]),
                new ImmutableFunArray(new object[] {1, 2}),
                new ImmutableFunArray(new object[] {1, 2})
            );

            Assert.IsFalse(a1.IsEquivalent(a2));
        }
        [Test]
        public void TextArrOfArr_Equivalent_IsEquivalentReturnsTrue()
        {

            var a1 = new ImmutableFunArray(
                new ImmutableFunArray(new object[0]),
                new ImmutableFunArray(new object[] {"1", "2"}),
                new ImmutableFunArray(new object[] {"1", "2","3"})
            );
            var a2 = new ImmutableFunArray(
                new ImmutableFunArray(new object[0]),
                new ImmutableFunArray(new object[] {"1", "2"}),
                new ImmutableFunArray(new object[] {"1", "2","3"})
            );

            Assert.IsTrue(a1.IsEquivalent(a2));
        }

        [Test]
        public void ExceptMultiDimensional()
        {
            var a1 = new ImmutableFunArray(
                new ImmutableFunArray(new[] {1.0, 2.0}),
                new ImmutableFunArray(new[] {3.0, 4.0}));
            var a2 = new ImmutableFunArray(
                new ImmutableFunArray(new[] {3.0, 4.0}),
                new ImmutableFunArray(new[] {1.0}),
                new ImmutableFunArray(new[] {4.0})
                );
            var res = a1.Except(a2).ToArray();
            Assert.AreEqual(1,res.Length);
            CollectionAssert.AreEquivalent(new[]{1.0,2.0}, ((IEnumerable)res[0]).Cast<object>());
        }
        [Test]
        public void Equals_ItemsAreEqual_returnsTrue()
        {
            var a1 = new ImmutableFunArray(new[] {1.0, 2.0});
            var a2 = new ImmutableFunArray(new[] {1.0, 2.0});
            Assert.IsTrue(a1.Equals(a2));
            Assert.IsTrue(a2.Equals(a1));
            Assert.AreEqual(a1.GetHashCode(), a2.GetHashCode());
        }
        
        [Test]
        public void TextArrOfArr_NotEquivalent_IsEquivalentReturnsFalse()
        {

            var a1 = new ImmutableFunArray(
                new ImmutableFunArray(new object[0]),
                new ImmutableFunArray(new object[] {"1", "2"}),
                new ImmutableFunArray(new object[] {"1", "2","3"})
            );
            var a2 = new ImmutableFunArray(
                new ImmutableFunArray(new object[]{"lalala"}),
                new ImmutableFunArray(new object[] {"1", "2"}),
                new ImmutableFunArray(new object[] {"1", "2","3"})
            );

            Assert.IsFalse(a1.IsEquivalent(a2));
        }
    }
}