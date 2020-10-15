using System;
using NFun;
using NFun.Exceptions;
using NUnit.Framework;

namespace Funny.Tests.Operators
{
    public class IntegerBitOperatorsTest
    {
        [TestCase("y:int64 = 1 & 1",(Int64)1)]
        [TestCase("y:int64 = 1 & 2",(Int64)0)]
        [TestCase("y:int64 = 2 & 2",(Int64)2)]
        [TestCase("y:int64 = 1 & 3",(Int64)1)]
        [TestCase("y:int64 = 1 & 3",(Int64)1)]

        [TestCase("y:int32 = 1 & 1",(Int32)1)]
        [TestCase("y:int32 = 1 & 2",(Int32)0)]
        [TestCase("y:int32 = 2 & 2",(Int32)2)]
        [TestCase("y:int32 = 1 & 3",(Int32)1)]
        [TestCase("y:int16 = 1 & 1",(Int16)1)]
        [TestCase("y:int16 = 1 & 2",(Int16)0)]
        [TestCase("y:int16 = 2 & 2",(Int16)2)]
        [TestCase("y:int16 = 1 & 3",(Int16)1)]
        
        [TestCase("y:uint64 = 1 & 1",(UInt64)1)]
        [TestCase("y:uint64 = 1 & 2",(UInt64)0)]
        [TestCase("y:uint64 = 2 & 2",(UInt64)2)]
        [TestCase("y:uint64 = 1 & 3",(UInt64)1)]
        [TestCase("y:uint32 = 1 & 1",(UInt32)1)]
        [TestCase("y:uint32 = 1 & 2",(UInt32)0)]
        [TestCase("y:uint32 = 2 & 2",(UInt32)2)]
        [TestCase("y:uint32 = 1 & 3",(UInt32)1)]
        [TestCase("y:uint16 = 1 & 1",(UInt16)1)]
        [TestCase("y:uint16 = 1 & 2",(UInt16)0)]
        [TestCase("y:uint16 = 2 & 2",(UInt16)2)]
        [TestCase("y:uint16 = 1 & 3",(UInt16)1)]
        
        [TestCase("y:uint8 = 1 & 1",(byte)1)]
        [TestCase("y:uint8 = 1 & 2",(byte)0)]
        [TestCase("y:uint8 = 2 & 2",(byte)2)]
        [TestCase("y:uint8 = 1 & 3",(byte)1)]

        
        [TestCase("y = 0xFFFFFFFF & 0x0", (long)0)]
        [TestCase("y = 0xFFFFFFFF & 0xFFFFFFFF", (long)0xFFFFFFFF)]
        [TestCase("y:uint64 = 0xFFFFFFFF_FFFFFFFF & 0xFFFFFFFF_FFFFFFFF",(UInt64)0xFFFFFFFF_FFFFFFFF)]
        [TestCase("y:uint64 = 0xFFFFFFFF_FFFFFFFF & 0",(UInt64)0)]
        [TestCase("y:uint64 = 0 & 0xFFFFFFFF_FFFFFFFF",(UInt64)0)]
        [TestCase("y:uint32 = 0xFFFFFFFF & 0xFFFFFFFF",(UInt32)0xFFFFFFFF)]
        [TestCase("y:uint32 = 0xFFFFFFFF & 0",(UInt32)0)]
        [TestCase("y:uint32 = 0 & 0xFFFFFFFF",(UInt32)0)]
        [TestCase("y:uint16 = 0xFFFF & 0xFFFF",(UInt16)0xFFFF)]
        [TestCase("y:uint16 = 0xFFFF & 0",(UInt16)0)]
        [TestCase("y:uint16 = 0 & 0xFFFF",(UInt16)0)]
        [TestCase("y:uint8 = 0xFF & 0xFF",(byte)0xFF)]
        [TestCase("y:uint8 = 0xFF & 0",(byte)0)]
        [TestCase("y:uint8 = 0 & 0xFF",(byte)0)]
        public void ConstantBitAnd(string expression, object expected) 
            => TestTools.AssertConstantCalc("y",expression, expected);
        
        [TestCase("y = 0 | 2",2)]
        [TestCase("y = 1 | 2",3)]
        [TestCase("y = 1 | 4",5)]
        
        [TestCase("y:int64 = 1 | 1",(Int64)1)]
        [TestCase("y:int64 = 1 | 2",(Int64)3)]
        [TestCase("y:int64 = 2 | 2",(Int64)2)]
        [TestCase("y:int64 = 1 | 3",(Int64)3)]
        [TestCase("y:int64 = 1 | 3",(Int64)3)]

        [TestCase("y:int32 = 1 | 1",(Int32)1)]
        [TestCase("y:int32 = 1 | 2",(Int32)3)]
        [TestCase("y:int32 = 2 | 2",(Int32)2)]
        [TestCase("y:int32 = 1 | 3",(Int32)3)]
        [TestCase("y:int16 = 1 | 1",(Int16)1)]
        [TestCase("y:int16 = 1 | 2",(Int16)3)]
        [TestCase("y:int16 = 2 | 2",(Int16)2)]
        [TestCase("y:int16 = 1 | 3",(Int16)3)]
        
        [TestCase("y:uint64 = 1 | 1",(UInt64)1)]
        [TestCase("y:uint64 = 1 | 2",(UInt64)3)]
        [TestCase("y:uint64 = 2 | 2",(UInt64)2)]
        [TestCase("y:uint64 = 1 | 3",(UInt64)3)]
        [TestCase("y:uint32 = 1 | 1",(UInt32)1)]
        [TestCase("y:uint32 = 1 | 2",(UInt32)3)]
        [TestCase("y:uint32 = 2 | 2",(UInt32)2)]
        [TestCase("y:uint32 = 1 | 3",(UInt32)3)]
        [TestCase("y:uint16 = 1 | 1",(UInt16)1)]
        [TestCase("y:uint16 = 1 | 2",(UInt16)3)]
        [TestCase("y:uint16 = 2 | 2",(UInt16)2)]
        [TestCase("y:uint16 = 1 | 3",(UInt16)3)]
        
        [TestCase("y:uint8 = 1 | 1",(byte)1)]
        [TestCase("y:uint8 = 1 | 2",(byte)3)]
        [TestCase("y:uint8 = 2 | 2",(byte)2)]
        [TestCase("y:uint8 = 1 | 3",(byte)3)]
        
        [TestCase("y = 0xFFFFFFFF | 0x0",(long)0xFFFFFFFF)]
        [TestCase("y = 0xFFFFFFFF | 0xFFFFFFFF",(long)0xFFFFFFFF)]
        
        [TestCase("y:uint64 = 0xFFFFFFFF_FFFFFFFF | 0xFFFFFFFF_FFFFFFFF",(UInt64)0xFFFFFFFF_FFFFFFFF)]
        [TestCase("y:uint64 = 0xFFFFFFFF_FFFFFFFF | 0",(UInt64)0xFFFFFFFF_FFFFFFFF)]
        [TestCase("y:uint64 = 0 | 0xFFFFFFFF_FFFFFFFF",(UInt64)0xFFFFFFFF_FFFFFFFF)]
        [TestCase("y:uint32 = 0xFFFFFFFF | 0xFFFFFFFF",(UInt32)0xFFFFFFFF)]
        [TestCase("y:uint32 = 0xFFFFFFFF | 0",(UInt32)0xFFFFFFFF)]
        [TestCase("y:uint32 = 0 | 0xFFFFFFFF",(UInt32)0xFFFFFFFF)]
        [TestCase("y:uint16 = 0xFFFF | 0xFFFF",(UInt16)0xFFFF)]
        [TestCase("y:uint16 = 0xFFFF | 0",(UInt16)0xFFFF)]
        [TestCase("y:uint16 = 0 | 0xFFFF",(UInt16)0xFFFF)]
        [TestCase("y:uint8 = 0xFF | 0xFF",(byte)0xFF)]
        [TestCase("y:uint8 = 0xFF | 0",(byte)0xFF)]
        [TestCase("y:uint8 = 0 | 0xFF",(byte)0xFF)]
        public void ConstantBitOr(string expression, object expected) 
            => TestTools.AssertConstantCalc("y",expression, expected);

        [TestCase("y:int64 = 0 ^ 1",(Int64)1)]
        [TestCase("y:int64 = 1 ^ 0",(Int64)1)]
        [TestCase("y:int64 = 1 ^ 1",(Int64)0)]
        [TestCase("y:int64 = 0 ^ 0",(Int64)0)]
        [TestCase("y:int32 = 0 ^ 1",(Int32)1)]
        [TestCase("y:int32 = 1 ^ 0",(Int32)1)]
        [TestCase("y:int32 = 1 ^ 1",(Int32)0)]
        [TestCase("y:int32 = 0 ^ 0",(Int32)0)]
        [TestCase("y:int16 = 0 ^ 1",(Int16)1)]
        [TestCase("y:int16 = 1 ^ 0",(Int16)1)]
        [TestCase("y:int16 = 1 ^ 1",(Int16)0)]
        [TestCase("y:int16 = 0 ^ 0",(Int16)0)]
        
      
        [TestCase("y:uint64 = 0 ^ 1",(UInt64)1)]
        [TestCase("y:uint64 = 1 ^ 0",(UInt64)1)]
        [TestCase("y:uint64 = 1 ^ 1",(UInt64)0)]
        [TestCase("y:uint64 = 0 ^ 0",(UInt64)0)]
        [TestCase("y:uint32 = 0 ^ 1",(UInt32)1)]
        [TestCase("y:uint32 = 1 ^ 0",(UInt32)1)]
        [TestCase("y:uint32 = 1 ^ 1",(UInt32)0)]
        [TestCase("y:uint32 = 0 ^ 0",(UInt32)0)]
        [TestCase("y:uint16 = 0 ^ 1",(UInt16)1)]
        [TestCase("y:uint16 = 1 ^ 0",(UInt16)1)]
        [TestCase("y:uint16 = 1 ^ 1",(UInt16)0)]
        [TestCase("y:uint16 = 0 ^ 0",(UInt16)0)]
        [TestCase("y:uint8 = 0 ^ 1",(byte)1)]
        [TestCase("y:uint8 = 1 ^ 0",(byte)1)]
        [TestCase("y:uint8 = 1 ^ 1",(byte)0)]
        [TestCase("y:uint8 = 0 ^ 0",(byte)0)]
        
        [TestCase("y = 0xFFFFFFFF ^ 0x0", (long)0xFFFFFFFF)]
        [TestCase("y = 0xFFFFFFFF ^ 0xFFFFFFFF",(long)0)]
        
        [TestCase("y:uint64 = 0xFFFFFFFF_FFFFFFFF ^ 0xFFFFFFFF_FFFFFFFF",(UInt64)0)]
        [TestCase("y:uint64 = 0xFFFFFFFF_FFFFFFFF ^ 0",(UInt64)0xFFFFFFFF_FFFFFFFF)]
        [TestCase("y:uint64 = 0 ^ 0xFFFFFFFF_FFFFFFFF",(UInt64)0xFFFFFFFF_FFFFFFFF)]

        [TestCase("y:uint32 = 0xFFFFFFFF ^ 0xFFFFFFFF",(UInt32)0)]
        [TestCase("y:uint32 = 0xFFFFFFFF ^ 0",(UInt32)0xFFFFFFFF)]
        [TestCase("y:uint32 = 0 ^ 0xFFFFFFFF",(UInt32)0xFFFFFFFF)]
        
        [TestCase("y:uint16 = 0xFFFF ^ 0xFFFF",(UInt16)0)]
        [TestCase("y:uint16 = 0xFFFF ^ 0",(UInt16)0xFFFF)]
        [TestCase("y:uint16 = 0 ^ 0xFFFF",(UInt16)0xFFFF)]
        
        [TestCase("y:uint8 = 0xFF ^ 0xFF",(byte)0)]
        [TestCase("y:uint8 = 0xFF ^ 0",(byte)0xFF)]
        [TestCase("y:uint8 = 0 ^ 0xFF",(byte)0xFF)]
        public void ConstantBitXor(string expression, object expected) 
            => TestTools.AssertConstantCalc("y",expression, expected);
        
        [TestCase("y = 1 << 3",8)]
        [TestCase("y = 8 >> 3",1)]
        [TestCase("y:int64 = 1 << 3",(Int64)8)]
        [TestCase("y:int64 = 8 >> 3",(Int64)1)]
        [TestCase("y:int32 = 1 << 3",(Int32)8)]
        [TestCase("y:int32 = 8 >> 3",(Int32)1)]
        
        [TestCase("y:uint64 = 1 << 3",(UInt64)8)]
        [TestCase("y:uint64 = 8 >> 3",(UInt64)1)]
        [TestCase("y:uint32 = 1 << 3",(UInt32)8)]
        [TestCase("y:uint32 = 8 >> 3",(UInt32)1)]
        
        // Todo ui16 i16, u8 does not support shift.
        // Should they?
        //[TestCase("y:int16 = 1 << 3",(Int16)8)]
        //[TestCase("y:int16 = 8 >> 3",(Int16)1)]
        //[TestCase("y:uint16 = 1 << 3",(UInt16)8)]
        //[TestCase("y:uint16 = 8 >> 3",(UInt16)1)]
        //[TestCase("y:byte = 1 << 3",(byte)8)]
        //[TestCase("y:byte = 8 >> 3",(byte)1)]
        public void ConstantBitShift(string expression, object expected) 
            => TestTools.AssertConstantCalc("y",expression, expected);
        
        [TestCase("y:int16 = ~1",         (Int16)(-2))]
        [TestCase("y:int16 = ~-1",        (Int16)0)]
        [TestCase("y:int16 = ~0xF0F",     (Int16) (-3856))]
        [TestCase("y:uint16 = ~1",        (UInt16)0xFFFE)]
        [TestCase("y:uint16 = ~0xF0F0",   (UInt16)0x0F0F)]
        [TestCase("y = ~1",               (int)-2)]
        [TestCase("y = ~-1",              (int)0)]
        [TestCase("y:int = ~1",           (int)-2)]
        [TestCase("y:int = ~-1",          (int)0)]
        [TestCase("y:int = ~0x00F0F0F0",  (int)-15790321)]
        [TestCase("y:uint = ~1",          (uint)0xFFFFFFFE)]
        [TestCase("y:uint = ~0xF0F0F0F0", (uint)0xF0F0F0F)]
       
        [TestCase("y:int64 = ~1",           (long)-2)]
        [TestCase("y:int64 = ~-1",          (long) 0)]
        [TestCase("y:int64 = ~0xF0F0F0F0",  (long)-4042322161)]
        [TestCase("y:uint64 = ~1",          (ulong)0xFFFF_FFFF_FFFF_FFFE)]
        [TestCase("y:uint64 = ~0xF0F0F0F0", (ulong)0xFFFF_FFFF_0F0F_0F0F)]
        
        [TestCase("y = 1 == ~~1",         true)]
        [TestCase("y = 0 == ~~0",         true)]
        [TestCase("y = -1 == ~~-1",       true)]
        [TestCase("y = 0xA == ~~0xA",     true)]
        [TestCase("y = -0xA == ~~-0xA",   true)]

        [TestCase("y = 0xABCD_EF01 == ~~0xABCD_EF01",  true)]
        [TestCase("y = -0xABCD_EF01 == ~~-0xABCD_EF01",true)]
        public void ConstantBitInvert(string expression, object expected) 
            => TestTools.AssertConstantCalc("y",expression, expected);

        // Todo ui16 i16, u8 does not support shift.
        // Should they?
        //[TestCase("y:byte = 1<<9")]
        //[TestCase("y:byte = 255<<1")]
        //[TestCase("y:uint16 = 0xFFFF<<1")]
        //[TestCase("y:uint16 = 0x1<<17")]
        //[TestCase("y:int16 = 0x1<<17")]

        [TestCase("y:uint32 = 0xFFFF_ffff<<1")]
        [TestCase("y:uint32 = 0x1<<33")]
        [TestCase("y:uint64 = 0xFFFF_ffff_FFFF_ffff<<1")]
        [TestCase("y:uint64 = 0x1<<65")]
        [TestCase("y:int32 = 0x1<<33")]
        [TestCase("y:int64 = 0x1<<65")]
        public void Oops(string expr)
        {
            var rt = FunBuilder.Build(expr);
            Assert.Throws<FunParseException>(
                ()=> rt.Calculate());
        }
        [TestCase("y = ^2")]
        [TestCase("y = 2^^")]
        [TestCase("y = ^^2")]
        [TestCase("y = -")]
        [TestCase("y = ~")]
        [TestCase("~y=3")]
        [TestCase("y = ~")]
        [TestCase("y = ~-")]
        [TestCase("y = ~1.5")]
        public void ObviouslyFails(string expr) =>
            Assert.Throws<FunParseException>(
                ()=> FunBuilder.Build(expr));
    }
}