// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.Tests;
using Xunit;

namespace System.Numerics.Tests
{
    public partial class Decimal64Tests
    {

        private static void AssertEqualAndHaveSameQuantum(Decimal64 expected, Decimal64 result)
        {
            Assert.Equal(expected, result);
            Assert.True(Decimal64.HaveSameQuantum(expected, result));
        }

        private static void AssertEqualAndHaveSameQuantumOrBothNan(Decimal64 expected, Decimal64 result)
        {
            if (!(Decimal64.IsNaN(expected) && Decimal64.IsNaN(result)))
            {
                AssertEqualAndHaveSameQuantum(expected, result);
            }
        }

        // TODO I think this is the same as AssertEqualAndHaveSameQuantum, but more efficient. Maybe remove AssertEqualAndHaveSameQuantum?
        private static void AssertBitwiseEqual(Decimal64 expected, Decimal64 result)
        {
            Assert.Equal(expected._value, result._value);
        }

        [Fact]
        public static void Epsilon()
        {
            Assert.Equal(0x0000_0000_0000_0001UL, Decimal64.Epsilon._value);
        }

        [Fact]
        public static void PositiveInfinity()
        {
            Assert.Equal(0x7800_0000_0000_0000UL, Decimal64.PositiveInfinity._value);
        }

        [Fact]
        public static void NegativeInfinity()
        {
            Assert.Equal(0xF800_0000_0000_0000UL, Decimal64.NegativeInfinity._value);
        }

        [Fact]
        public static void NaN()
        {
            Assert.Equal(0xFC00_0000_0000_0000, Decimal64.NaN._value);
        }

        [Fact]
        public static void MinValue()
        {
            Assert.Equal(0xF7FB_86F2_6FC0_FFFFUL, Decimal64.MinValue._value);
        }

        [Fact]
        public static void MaxValue()
        {
            Assert.Equal(0x77FB_86F2_6FC0_FFFFUL, Decimal64.MaxValue._value);
        }

        [Fact]
        public static void Ctor_Empty()
        {
            var value = new Decimal64();
            Assert.Equal(0x0000_0000_0000_0000UL, value._value);
        }

        [Fact]
        public static void Ctor()
        {
            // TODO
            throw new NotImplementedException();
        }

        public static IEnumerable<object[]> IsFinite_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, false };                                            // Negative Infinity
            yield return new object[] { Decimal64.MinValue, true };                                                     // Min Negative Special Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), true };     // Min Negative Normal Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MinQExponent, 0x0020_0000_0000_0000), true };     // Max Negative Special Encoding
            yield return new object[] { -Decimal64.Epsilon, true };                                                     // Max Negative Normal Encoding
            yield return new object[] { Decimal64.NegativeZero, true };                                                 // Negative Zero
            yield return new object[] { Decimal64.NaN, false };                                                         // NaN
            yield return new object[] { Decimal64.Zero, true };                                                         // Positive Zero
            yield return new object[] { Decimal64.Epsilon, true };                                                      // Min Positive Normal Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MinQExponent, 0x0020_0000_0000_0000), true };    // Min Positive Special Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), true };    // Max Positive Normal Encoding
            yield return new object[] { Decimal64.MaxValue, true };                                                     // Max Positive Special Encoding
            yield return new object[] { Decimal64.PositiveInfinity, false };                                            // Positive Infinity
        }

        [Theory]
        [MemberData(nameof(IsFinite_TestData))]
        public static void IsFinite(Decimal64 value, bool expected)
        {
            Assert.Equal(expected, Decimal64.IsFinite(value));
        }

        public static IEnumerable<object[]> IsInfinity_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, true };                                             // Negative Infinity
            yield return new object[] { Decimal64.MinValue, false };                                                    // Min Negative Special Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), false };    // Min Negative Normal Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MinQExponent, 0x0020_0000_0000_0000), false };    // Max Negative Special Encoding
            yield return new object[] { -Decimal64.Epsilon, false };                                                    // Max Negative Normal Encoding
            yield return new object[] { Decimal64.NegativeZero, false };                                                // Negative Zero
            yield return new object[] { Decimal64.NaN, false };                                                         // NaN
            yield return new object[] { Decimal64.Zero, false };                                                        // Positive Zero
            yield return new object[] { Decimal64.Epsilon, false };                                                     // Min Positive Normal Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MinQExponent, 0x0020_0000_0000_0000), false };   // Min Positive Special Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), false };   // Max Positive Normal Encoding
            yield return new object[] { Decimal64.MaxValue, false };                                                    // Max Positive Special Encoding
            yield return new object[] { Decimal64.PositiveInfinity, true };                                             // Positive Infinity
        }

        [Theory]
        [MemberData(nameof(IsInfinity_TestData))]
        public static void IsInfinity(Decimal64 value, bool expected)
        {
            Assert.Equal(expected, Decimal64.IsInfinity(value));
        }

        public static IEnumerable<object[]> IsNaN_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, false };                                            // Negative Infinity
            yield return new object[] { Decimal64.MinValue, false };                                                    // Min Negative Special Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), false };    // Min Negative Normal Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MinQExponent, 0x0020_0000_0000_0000), false };    // Max Negative Special Encoding
            yield return new object[] { -Decimal64.Epsilon, false };                                                    // Max Negative Normal Encoding
            yield return new object[] { Decimal64.NegativeZero, false };                                                // Negative Zero
            yield return new object[] { Decimal64.NaN, true };                                                          // NaN
            yield return new object[] { Decimal64.Zero, false };                                                        // Positive Zero
            yield return new object[] { Decimal64.Epsilon, false };                                                     // Min Positive Normal Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MinQExponent, 0x0020_0000_0000_0000), false };   // Min Positive Special Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), false };   // Max Positive Normal Encoding
            yield return new object[] { Decimal64.MaxValue, false };                                                    // Max Positive Special Encoding
            yield return new object[] { Decimal64.PositiveInfinity, false };                                            // Positive Infinity
        }

        [Theory]
        [MemberData(nameof(IsNaN_TestData))]
        public static void IsNaN(Decimal64 value, bool expected)
        {
            Assert.Equal(expected, Decimal64.IsNaN(value));
        }

        public static IEnumerable<object[]> IsNegative_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, true };                                             // Negative Infinity
            yield return new object[] { Decimal64.MinValue, true };                                                     // Min Negative Special Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), true };     // Min Negative Normal Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MinQExponent, 0x0020_0000_0000_0000), true };     // Max Negative Special Encoding
            yield return new object[] { -Decimal64.Epsilon, true };                                                     // Max Negative Normal Encoding
            yield return new object[] { Decimal64.NegativeZero, true };                                                 // Negative Zero
            yield return new object[] { Decimal64.NaN, false };                                                         // NaN
            yield return new object[] { Decimal64.Zero, false };                                                        // Positive Zero
            yield return new object[] { Decimal64.Epsilon, false };                                                     // Min Positive Normal Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MinQExponent, 0x0020_0000_0000_0000), false };   // Min Positive Special Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), false };   // Max Positive Normal Encoding
            yield return new object[] { Decimal64.MaxValue, false };                                                    // Max Positive Special Encoding
            yield return new object[] { Decimal64.PositiveInfinity, false };                                            // Positive Infinity
        }

        [Theory]
        [MemberData(nameof(IsNegative_TestData))]
        public static void IsNegative(Decimal64 value, bool expected)
        {
            Assert.Equal(expected, Decimal64.IsNegative(value));
        }

        public static IEnumerable<object[]> IsNegativeInfinity_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, true };                                             // Negative Infinity
            yield return new object[] { Decimal64.MinValue, false };                                                    // Min Negative Special Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), false };    // Min Negative Normal Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MinQExponent, 0x0020_0000_0000_0000), false };    // Max Negative Special Encoding
            yield return new object[] { -Decimal64.Epsilon, false };                                                    // Max Negative Normal Encoding
            yield return new object[] { Decimal64.NegativeZero, false };                                                // Negative Zero
            yield return new object[] { Decimal64.NaN, false };                                                         // NaN
            yield return new object[] { Decimal64.Zero, false };                                                        // Positive Zero
            yield return new object[] { Decimal64.Epsilon, false };                                                     // Min Positive Normal Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MinQExponent, 0x0020_0000_0000_0000), false };   // Min Positive Special Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), false };   // Max Positive Normal Encoding
            yield return new object[] { Decimal64.MaxValue, false };                                                    // Max Positive Special Encoding
            yield return new object[] { Decimal64.PositiveInfinity, false };                                            // Positive Infinity
        }

        [Theory]
        [MemberData(nameof(IsNegativeInfinity_TestData))]
        public static void IsNegativeInfinity(Decimal64 value, bool expected)
        {
            Assert.Equal(expected, Decimal64.IsNegativeInfinity(value));
        }

        public static IEnumerable<object[]> IsNormal_TestData() // TODO this should be identical to IsFinite, correct?
        {
            yield return new object[] { Decimal64.NegativeInfinity, false };                                            // Negative Infinity
            yield return new object[] { Decimal64.MinValue, true };                                                     // Min Negative Special Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), true };     // Min Negative Normal Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MinQExponent, 0x0020_0000_0000_0000), true };     // Max Negative Special Encoding
            yield return new object[] { -Decimal64.Epsilon, true };                                                     // Max Negative Normal Encoding
            yield return new object[] { Decimal64.NegativeZero, true };                                                 // Negative Zero
            yield return new object[] { Decimal64.NaN, false };                                                         // NaN
            yield return new object[] { Decimal64.Zero, true };                                                         // Positive Zero
            yield return new object[] { Decimal64.Epsilon, true };                                                      // Min Positive Normal Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MinQExponent, 0x0020_0000_0000_0000), true };    // Min Positive Special Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), true };    // Max Positive Normal Encoding
            yield return new object[] { Decimal64.MaxValue, true };                                                     // Max Positive Special Encoding
            yield return new object[] { Decimal64.PositiveInfinity, false };                                            // Positive Infinity
        }

        [Theory]
        [MemberData(nameof(IsNormal_TestData))]
        public static void IsNormal(Decimal64 value, bool expected)
        {
            Assert.Equal(expected, Decimal64.IsNormal(value));
        }

        public static IEnumerable<object[]> IsPositiveInfinity_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, false };                                            // Negative Infinity
            yield return new object[] { Decimal64.MinValue, false };                                                    // Min Negative Special Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), false };    // Min Negative Normal Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MinQExponent, 0x0020_0000_0000_0000), false };    // Max Negative Special Encoding
            yield return new object[] { -Decimal64.Epsilon, false };                                                    // Max Negative Normal Encoding
            yield return new object[] { Decimal64.NegativeZero, false };                                                // Negative Zero
            yield return new object[] { Decimal64.NaN, false };                                                         // NaN
            yield return new object[] { Decimal64.Zero, false };                                                        // Positive Zero
            yield return new object[] { Decimal64.Epsilon, false };                                                     // Min Positive Normal Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MinQExponent, 0x0020_0000_0000_0000), false };   // Min Positive Special Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), false };   // Max Positive Normal Encoding
            yield return new object[] { Decimal64.MaxValue, false };                                                    // Max Positive Special Encoding
            yield return new object[] { Decimal64.PositiveInfinity, true };                                             // Positive Infinity
        }

        [Theory]
        [MemberData(nameof(IsPositiveInfinity_TestData))]
        public static void IsPositiveInfinity(Decimal64 value, bool expected)
        {
            Assert.Equal(expected, Decimal64.IsPositiveInfinity(value));
        }

        public static IEnumerable<object[]> IsSubnormal_TestData() // TODO confirm that this should be false for everything
        {
            yield return new object[] { Decimal64.NegativeInfinity, false};                                            // Negative Infinity
            yield return new object[] { Decimal64.MinValue, false};                                                    // Min Negative Special Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), false};    // Min Negative Normal Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MinQExponent, 0x0020_0000_0000_0000), false};    // Max Negative Special Encoding
            yield return new object[] { -Decimal64.Epsilon, false};                                                    // Max Negative Normal Encoding
            yield return new object[] { Decimal64.NegativeZero, false};                                                // Negative Zero
            yield return new object[] { Decimal64.NaN, false};                                                         // NaN
            yield return new object[] { Decimal64.Zero, false};                                                        // Positive Zero
            yield return new object[] { Decimal64.Epsilon, false};                                                     // Min Positive Normal Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MinQExponent, 0x0020_0000_0000_0000), false};   // Min Positive Special Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF), false};   // Max Positive Normal Encoding
            yield return new object[] { Decimal64.MaxValue, false};                                                    // Max Positive Special Encoding
            yield return new object[] { Decimal64.PositiveInfinity, false};                                            // Positive Infinity
        }

        [Theory]
        [MemberData(nameof(IsSubnormal_TestData))]
        public static void IsSubnormal(Decimal64 value, bool expected)
        {
            Assert.Equal(expected, Decimal64.IsSubnormal(value));
        }

        public static IEnumerable<object[]> CompareTo_ThrowsArgumentException_TestData()
        {
            yield return new object[] { "a" };
            yield return new object[] { 234.0 };
        }

        [Theory]
        [MemberData(nameof(CompareTo_ThrowsArgumentException_TestData))]
        public static void CompareTo_ThrowsArgumentException(object obj)
        {
            Assert.Throws<ArgumentException>(() => Decimal64.MaxValue.CompareTo(obj));
        }

        public static IEnumerable<object[]> CompareTo_TestData()
        {
            yield return new object[] { Decimal64.MaxValue, Decimal64.MaxValue, 0 };
            yield return new object[] { Decimal64.MaxValue, Decimal64.MinValue, 1 };
            yield return new object[] { Decimal64.Epsilon, -Decimal64.Epsilon, 1 };
            yield return new object[] { Decimal64.MaxValue, new Decimal64(0), 1 };
            yield return new object[] { Decimal64.MaxValue, Decimal64.Epsilon, 1 };
            yield return new object[] { Decimal64.MaxValue, Decimal64.PositiveInfinity, -1 };
            yield return new object[] { Decimal64.MinValue, Decimal64.MaxValue, -1 };
            yield return new object[] { Decimal64.MaxValue, Decimal64.NaN, 1 };
            yield return new object[] { Decimal64.NaN, Decimal64.NaN, 0 };
            yield return new object[] { Decimal64.NaN, new Decimal64(0), -1 };
            yield return new object[] { Decimal64.MaxValue, null, 1 };
            yield return new object[] { Decimal64.MinValue, Decimal64.NegativeInfinity, 1 };
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.MinValue, -1 };
            yield return new object[] { Decimal64.NegativeZero, Decimal64.NegativeInfinity, 1 };
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.NegativeZero, -1 };
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.NegativeInfinity, 0 };
            yield return new object[] { Decimal64.PositiveInfinity, Decimal64.PositiveInfinity, 0 };
            yield return new object[] { new Decimal64(-180, 0), new Decimal64(-180, 0), 0 }; // TODO is this constant chosen arbitrarily or does this need to be adjusted for Decimal64?
            yield return new object[] { new Decimal64(180, 0), new Decimal64(180, 0), 0 };
            yield return new object[] { new Decimal64(-180, 0), new Decimal64(180, 0), -1 };
            yield return new object[] { new Decimal64(180, 0), new Decimal64(-180, 0), 1 };
            yield return new object[] { (Decimal64)(-65535), (object)null, 1 }; // TODO is this constant chosen arbitrarily or does this need to be adjusted for Decimal64?
            // TODO we should have some test cases covering cohort comparisons
        }

        [Theory]
        [MemberData(nameof(CompareTo_TestData))]
        public static void CompareTo(Decimal64 value, object obj, int expected)
        {
            if (obj is Decimal64 other)
            {
                Assert.Equal(expected, Math.Sign(value.CompareTo(other)));

                if (Decimal64.IsNaN(value) || Decimal64.IsNaN(other))
                {
                    Assert.False(value >= other);
                    Assert.False(value > other);
                    Assert.False(value <= other);
                    Assert.False(value < other);
                }
                else
                {
                    if (expected >= 0)
                    {
                        Assert.True(value >= other);
                        Assert.False(value < other);
                    }
                    if (expected > 0)
                    {
                        Assert.True(value > other);
                        Assert.False(value <= other);
                    }
                    if (expected <= 0)
                    {
                        Assert.True(value <= other);
                        Assert.False(value > other);
                    }
                    if (expected < 0)
                    {
                        Assert.True(value < other);
                        Assert.False(value >= other);
                    }
                }
            }

            Assert.Equal(expected, Math.Sign(value.CompareTo(obj)));
        }

        public static IEnumerable<object[]> Equals_TestData()
        {
            yield return new object[] { Decimal64.MaxValue, Decimal64.MaxValue, true };
            yield return new object[] { Decimal64.MaxValue, Decimal64.MinValue, false };
            yield return new object[] { Decimal64.MaxValue, new Decimal64(0), false };
            yield return new object[] { Decimal64.NaN, Decimal64.NaN, true };
            yield return new object[] { Decimal64.MaxValue, 789.0f, false }; // TODO is this constant chosen arbitrarily or does this need to be adjusted for Decimal64?
            yield return new object[] { Decimal64.MaxValue, "789", false };
            // TODO we should have some test cases covering cohort comparisons
        }

        [Theory]
        [MemberData(nameof(Equals_TestData))]
        public static void EqualsTest(Decimal64 value, object obj, bool expected)
        {
            Assert.Equal(expected, value.Equals(obj));
        }

        public static IEnumerable<object[]> ExplicitConversion_ToSingle_TestData()
        {
            throw new NotImplementedException();
/*            (Half Original, float Expected)[] data = // Fraction is shifted left by 42, Exponent is -15 then +127 = +112
            {
                (BitConverter.UInt16BitsToHalf(0b0_01111_0000000000), 1f), // 1
                (BitConverter.UInt16BitsToHalf(0b1_01111_0000000000), -1f), // -1
                (Half.MaxValue, 65504f), // 65500
                (Half.MinValue, -65504f), // -65500
                (BitConverter.UInt16BitsToHalf(0b0_01011_1001100110), 0.0999755859375f), // 0.1ish
                (BitConverter.UInt16BitsToHalf(0b1_01011_1001100110), -0.0999755859375f), // -0.1ish
                (BitConverter.UInt16BitsToHalf(0b0_10100_0101000000), 42f), // 42
                (BitConverter.UInt16BitsToHalf(0b1_10100_0101000000), -42f), // -42
                (Half.PositiveInfinity, float.PositiveInfinity), // PosInfinity
                (Half.NegativeInfinity, float.NegativeInfinity), // NegInfinity
                (BitConverter.UInt16BitsToHalf(0b0_11111_1000000000), BitConverter.Int32BitsToSingle(0x7FC00000)), // Positive Quiet NaN
                (Half.NaN, float.NaN), // Negative Quiet NaN
                (BitConverter.UInt16BitsToHalf(0b0_11111_1010101010), BitConverter.Int32BitsToSingle(0x7FD54000)), // Positive Signalling NaN - Should preserve payload
                (BitConverter.UInt16BitsToHalf(0b1_11111_1010101010), BitConverter.Int32BitsToSingle(unchecked((int)0xFFD54000))), // Negative Signalling NaN - Should preserve payload
                (Half.Epsilon, 1/16777216f), // PosEpsilon = 0.000000059605...
                (BitConverter.UInt16BitsToHalf(0), 0), // 0
                (BitConverter.UInt16BitsToHalf(0b1_00000_0000000000), -0f), // -0
                (BitConverter.UInt16BitsToHalf(0b0_10000_1001001000), 3.140625f), // 3.140625
                (BitConverter.UInt16BitsToHalf(0b1_10000_1001001000), -3.140625f), // -3.140625
                (BitConverter.UInt16BitsToHalf(0b0_10000_0101110000), 2.71875f), // 2.71875
                (BitConverter.UInt16BitsToHalf(0b1_10000_0101110000), -2.71875f), // -2.71875
                (BitConverter.UInt16BitsToHalf(0b0_01111_1000000000), 1.5f), // 1.5
                (BitConverter.UInt16BitsToHalf(0b1_01111_1000000000), -1.5f), // -1.5
                (BitConverter.UInt16BitsToHalf(0b0_01111_1000000001), 1.5009765625f), // 1.5009765625
                (BitConverter.UInt16BitsToHalf(0b1_01111_1000000001), -1.5009765625f), // -1.5009765625
                (BitConverter.UInt16BitsToHalf(0b0_00001_0000000000), BitConverter.Int32BitsToSingle(0x38800000)), // smallest normal
                (BitConverter.UInt16BitsToHalf(0b0_00000_1111111111), BitConverter.Int32BitsToSingle(0x387FC000)), // largest subnormal
                (BitConverter.UInt16BitsToHalf(0b0_00000_1000000000), BitConverter.Int32BitsToSingle(0x38000000)), // middle subnormal
                (BitConverter.UInt16BitsToHalf(0b0_00000_0111111111), BitConverter.Int32BitsToSingle(0x37FF8000)), // just below middle subnormal
                (BitConverter.UInt16BitsToHalf(0b0_00000_0000000001), BitConverter.Int32BitsToSingle(0x33800000)), // smallest subnormal
                (BitConverter.UInt16BitsToHalf(0b1_00000_0000000001), BitConverter.Int32BitsToSingle(unchecked((int)0xB3800000))), // highest negative subnormal
                (BitConverter.UInt16BitsToHalf(0b1_00000_0111111111), BitConverter.Int32BitsToSingle(unchecked((int)0xB7FF8000))), // just above negative middle subnormal
                (BitConverter.UInt16BitsToHalf(0b1_00000_1000000000), BitConverter.Int32BitsToSingle(unchecked((int)0xB8000000))), // negative middle subnormal
                (BitConverter.UInt16BitsToHalf(0b1_00000_1111111111), BitConverter.Int32BitsToSingle(unchecked((int)0xB87FC000))), // lowest negative subnormal
                (BitConverter.UInt16BitsToHalf(0b1_00001_0000000000), BitConverter.Int32BitsToSingle(unchecked((int)0xB8800000))) // highest negative normal
            };

            foreach ((Half original, float expected) in data)
            {
                yield return new object[] { original, expected };
            }*/
        }

        [MemberData(nameof(ExplicitConversion_ToSingle_TestData))]
        [Theory]
        public static void ExplicitConversion_ToSingle(Decimal64 value, float expected) // Check the underlying bits for verifying NaNs
        {
            float f = (float)value;
            Assert.Equal(BitConverter.SingleToInt32Bits(expected), BitConverter.SingleToInt32Bits(f));
        }

        public static IEnumerable<object[]> ExplicitConversion_ToDouble_TestData()
        {
            throw new NotImplementedException();
/*            (Half Original, double Expected)[] data = // Fraction is shifted left by 42, Exponent is -15 then +127 = +112
            {
                (BitConverter.UInt16BitsToHalf(0b0_01111_0000000000), 1d), // 1
                (BitConverter.UInt16BitsToHalf(0b1_01111_0000000000), -1d), // -1
                (Half.MaxValue, 65504d), // 65500
                (Half.MinValue, -65504d), // -65500
                (BitConverter.UInt16BitsToHalf(0b0_01011_1001100110), 0.0999755859375d), // 0.1ish
                (BitConverter.UInt16BitsToHalf(0b1_01011_1001100110), -0.0999755859375d), // -0.1ish
                (BitConverter.UInt16BitsToHalf(0b0_10100_0101000000), 42d), // 42
                (BitConverter.UInt16BitsToHalf(0b1_10100_0101000000), -42d), // -42
                (Half.PositiveInfinity, double.PositiveInfinity), // PosInfinity
                (Half.NegativeInfinity, double.NegativeInfinity), // NegInfinity
                (BitConverter.UInt16BitsToHalf(0b0_11111_1000000000), BitConverter.Int64BitsToDouble(0x7FF80000_00000000)), // Positive Quiet NaN
                (Half.NaN, double.NaN), // Negative Quiet NaN
                (BitConverter.UInt16BitsToHalf(0b0_11111_1010101010), BitConverter.Int64BitsToDouble(0x7FFAA800_00000000)), // Positive Signalling NaN - Should preserve payload
                (BitConverter.UInt16BitsToHalf(0b1_11111_1010101010), BitConverter.Int64BitsToDouble(unchecked((long)0xFFFAA800_00000000))), // Negative Signalling NaN - Should preserve payload
                (Half.Epsilon, 1/16777216d), // PosEpsilon = 0.000000059605...
                (BitConverter.UInt16BitsToHalf(0), 0d), // 0
                (BitConverter.UInt16BitsToHalf(0b1_00000_0000000000), -0d), // -0
                (BitConverter.UInt16BitsToHalf(0b0_10000_1001001000), 3.140625d), // 3.140625
                (BitConverter.UInt16BitsToHalf(0b1_10000_1001001000), -3.140625d), // -3.140625
                (BitConverter.UInt16BitsToHalf(0b0_10000_0101110000), 2.71875d), // 2.71875
                (BitConverter.UInt16BitsToHalf(0b1_10000_0101110000), -2.71875d), // -2.71875
                (BitConverter.UInt16BitsToHalf(0b0_01111_1000000000), 1.5d), // 1.5
                (BitConverter.UInt16BitsToHalf(0b1_01111_1000000000), -1.5d), // -1.5
                (BitConverter.UInt16BitsToHalf(0b0_01111_1000000001), 1.5009765625d), // 1.5009765625
                (BitConverter.UInt16BitsToHalf(0b1_01111_1000000001), -1.5009765625d), // -1.5009765625
                (BitConverter.UInt16BitsToHalf(0b0_00001_0000000000), BitConverter.Int64BitsToDouble(0x3F10000000000000)), // smallest normal
                (BitConverter.UInt16BitsToHalf(0b0_00000_1111111111), BitConverter.Int64BitsToDouble(0x3F0FF80000000000)), // largest subnormal
                (BitConverter.UInt16BitsToHalf(0b0_00000_1000000000), BitConverter.Int64BitsToDouble(0x3F00000000000000)), // middle subnormal
                (BitConverter.UInt16BitsToHalf(0b0_00000_0111111111), BitConverter.Int64BitsToDouble(0x3EFFF00000000000)), // just below middle subnormal
                (BitConverter.UInt16BitsToHalf(0b0_00000_0000000001), BitConverter.Int64BitsToDouble(0x3E70000000000000)), // smallest subnormal
                (BitConverter.UInt16BitsToHalf(0b1_00000_0000000001), BitConverter.Int64BitsToDouble(unchecked((long)0xBE70000000000000))), // highest negative subnormal
                (BitConverter.UInt16BitsToHalf(0b1_00000_0111111111), BitConverter.Int64BitsToDouble(unchecked((long)0xBEFFF00000000000))), // just above negative middle subnormal
                (BitConverter.UInt16BitsToHalf(0b1_00000_1000000000), BitConverter.Int64BitsToDouble(unchecked((long)0xBF00000000000000))), // negative middle subnormal
                (BitConverter.UInt16BitsToHalf(0b1_00000_1111111111), BitConverter.Int64BitsToDouble(unchecked((long)0xBF0FF80000000000))), // lowest negative subnormal
                (BitConverter.UInt16BitsToHalf(0b1_00001_0000000000), BitConverter.Int64BitsToDouble(unchecked((long)0xBF10000000000000))) // highest negative normal
            };

            foreach ((Half original, double expected) in data)
            {
                yield return new object[] { original, expected };
            }*/
        }

        [MemberData(nameof(ExplicitConversion_ToDouble_TestData))]
        [Theory]
        public static void ExplicitConversion_ToDouble(Decimal64 value, double expected) // Check the underlying bits for verifying NaNs
        {
            double d = (double)value;
            Assert.Equal(BitConverter.DoubleToInt64Bits(expected), BitConverter.DoubleToInt64Bits(d));
        }

        // ---------- Start of To-decimal64 conversion tests ----------
        public static IEnumerable<object[]> ExplicitConversion_FromSingle_TestData()
        {
            throw new NotImplementedException();
            /*(float, Half)[] data =
            {
                (MathF.PI, BitConverter.UInt16BitsToHalf(0b0_10000_1001001000)), // 3.140625
                (MathF.E, BitConverter.UInt16BitsToHalf(0b0_10000_0101110000)), // 2.71875
                (-MathF.PI, BitConverter.UInt16BitsToHalf(0b1_10000_1001001000)), // -3.140625
                (-MathF.E, BitConverter.UInt16BitsToHalf(0b1_10000_0101110000)), // -2.71875
                (float.MaxValue, Half.PositiveInfinity), // Overflow
                (float.MinValue, Half.NegativeInfinity), // Overflow
                (float.PositiveInfinity, Half.PositiveInfinity), // Overflow
                (float.NegativeInfinity, Half.NegativeInfinity), // Overflow
                (float.NaN, Half.NaN), // Quiet Negative NaN
                (BitConverter.Int32BitsToSingle(0x7FC00000), BitConverter.UInt16BitsToHalf(0b0_11111_1000000000)), // Quiet Positive NaN
                (BitConverter.Int32BitsToSingle(unchecked((int)0xFFD55555)),
                    BitConverter.UInt16BitsToHalf(0b1_11111_1010101010)), // Signalling Negative NaN
                (BitConverter.Int32BitsToSingle(0x7FD55555), BitConverter.UInt16BitsToHalf(0b0_11111_1010101010)), // Signalling Positive NaN
                (float.Epsilon, BitConverter.UInt16BitsToHalf(0)), // Underflow
                (-float.Epsilon, BitConverter.UInt16BitsToHalf(0b1_00000_0000000000)), // Underflow
                (1f, BitConverter.UInt16BitsToHalf(0b0_01111_0000000000)), // 1
                (-1f, BitConverter.UInt16BitsToHalf(0b1_01111_0000000000)), // -1
                (0f, BitConverter.UInt16BitsToHalf(0)), // 0
                (-0f, BitConverter.UInt16BitsToHalf(0b1_00000_0000000000)), // -0
                (42f, BitConverter.UInt16BitsToHalf(0b0_10100_0101000000)), // 42
                (-42f, BitConverter.UInt16BitsToHalf(0b1_10100_0101000000)), // -42
                (0.1f, BitConverter.UInt16BitsToHalf(0b0_01011_1001100110)), // 0.0999755859375
                (-0.1f, BitConverter.UInt16BitsToHalf(0b1_01011_1001100110)), // -0.0999755859375
                (1.5f, BitConverter.UInt16BitsToHalf(0b0_01111_1000000000)), // 1.5
                (-1.5f, BitConverter.UInt16BitsToHalf(0b1_01111_1000000000)), // -1.5
                (1.5009765625f, BitConverter.UInt16BitsToHalf(0b0_01111_1000000001)), // 1.5009765625
                (-1.5009765625f, BitConverter.UInt16BitsToHalf(0b1_01111_1000000001)), // -1.5009765625
                (BitConverter.Int32BitsToSingle(0x38800000), BitConverter.UInt16BitsToHalf(0b0_00001_0000000000)), // smallest normal
                (BitConverter.Int32BitsToSingle(0x387FC000), BitConverter.UInt16BitsToHalf(0b0_00000_1111111111)), // largest subnormal
                (BitConverter.Int32BitsToSingle(0x38000000), BitConverter.UInt16BitsToHalf(0b0_00000_1000000000)), // middle subnormal
                (BitConverter.Int32BitsToSingle(0x37FF8000), BitConverter.UInt16BitsToHalf(0b0_00000_0111111111)), // just below middle subnormal
                (BitConverter.Int32BitsToSingle(0x33800000), BitConverter.UInt16BitsToHalf(0b0_00000_0000000001)), // smallest subnormal
                (BitConverter.Int32BitsToSingle(unchecked((int)0xB3800000)),
                    BitConverter.UInt16BitsToHalf(0b1_00000_0000000001)), // highest negative subnormal
                (BitConverter.Int32BitsToSingle(unchecked((int)0xB7FF8000)),
                    BitConverter.UInt16BitsToHalf(0b1_00000_0111111111)), // just above negative middle subnormal
                (BitConverter.Int32BitsToSingle(unchecked((int)0xB8000000)),
                    BitConverter.UInt16BitsToHalf(0b1_00000_1000000000)), // negative middle subnormal
                (BitConverter.Int32BitsToSingle(unchecked((int)0xB87FC000)),
                    BitConverter.UInt16BitsToHalf(0b1_00000_1111111111)), // lowest negative subnormal
                (BitConverter.Int32BitsToSingle(unchecked((int)0xB8800000)),
                    BitConverter.UInt16BitsToHalf(0b1_00001_0000000000)), // highest negative normal
                (BitConverter.Int32BitsToSingle(0b0_10001001_00000000111000000000001),
                                  BitConverter.UInt16BitsToHalf(0b0_11001_0000000100)), // 1027.5+ULP rounds up
                (BitConverter.Int32BitsToSingle(0b0_10001001_00000000111000000000000),
                                  BitConverter.UInt16BitsToHalf(0b0_11001_0000000100)), // 1027.5 rounds to even
                (BitConverter.Int32BitsToSingle(0b0_10001001_00000000110111111111111),
                                  BitConverter.UInt16BitsToHalf(0b0_11001_0000000011)), // 1027.5-ULP rounds down
                (BitConverter.Int32BitsToSingle(unchecked((int)0b1_10001001_00000000110111111111111)),
                                                 BitConverter.UInt16BitsToHalf(0b1_11001_0000000011)), // -1027.5+ULP rounds towards zero
                (BitConverter.Int32BitsToSingle(unchecked((int)0b1_10001001_00000000111000000000000)),
                                                 BitConverter.UInt16BitsToHalf(0b1_11001_0000000100)), // -1027.5 rounds to even
                (BitConverter.Int32BitsToSingle(unchecked((int)0b1_10001001_00000000111000000000001)),
                                                 BitConverter.UInt16BitsToHalf(0b1_11001_0000000100)), // -1027.5-ULP rounds away from zero
                (BitConverter.Int32BitsToSingle(0b0_01110000_00000001110000000000001),
                                 BitConverter.UInt16BitsToHalf(0b0_00000_1000000100)), // subnormal + ULP rounds up
                (BitConverter.Int32BitsToSingle(0b0_01110000_00000001110000000000000),
                                 BitConverter.UInt16BitsToHalf(0b0_00000_1000000100)), // subnormal rounds to even
                (BitConverter.Int32BitsToSingle(0b0_01110000_00000001101111111111111),
                                 BitConverter.UInt16BitsToHalf(0b0_00000_1000000011)), // subnormal - ULP rounds down
                (BitConverter.Int32BitsToSingle(unchecked((int)0b1_01110000_00000001101111111111111)),
                                                BitConverter.UInt16BitsToHalf(0b1_00000_1000000011)), // neg subnormal + ULP rounds higher
                (BitConverter.Int32BitsToSingle(unchecked((int)0b1_01110000_00000001110000000000000)),
                                                BitConverter.UInt16BitsToHalf(0b1_00000_1000000100)), // neg subnormal rounds to even
                (BitConverter.Int32BitsToSingle(unchecked((int)0b1_01110000_00000001101111111111111)),
                                                BitConverter.UInt16BitsToHalf(0b1_00000_1000000011)), // neg subnormal - ULP rounds lower,
                (BitConverter.Int32BitsToSingle(0x33000000), BitConverter.UInt16BitsToHalf(0b0_00000_000000000)), // (half-precision minimum subnormal / 2) should underflow to zero
            };

            foreach ((float original, Half expected) in data)
            {
                yield return new object[] { original, expected };
            }*/
        }

        [MemberData(nameof(ExplicitConversion_FromSingle_TestData))]
        [Theory]
        public static void ExplicitConversion_FromSingle(float f, Decimal64 expected) // Check the underlying bits for verifying NaNs
        {
            Decimal64 result = (Decimal64)f;
            AssertBitwiseEqual(expected, result);
        }

        public static IEnumerable<object[]> ExplicitConversion_FromDouble_TestData()
        {
            throw new NotImplementedException();
            /*(double, Half)[] data =
            {
                (Math.PI, BitConverter.UInt16BitsToHalf(0b0_10000_1001001000)), // 3.140625
                (Math.E, BitConverter.UInt16BitsToHalf(0b0_10000_0101110000)), // 2.71875
                (-Math.PI, BitConverter.UInt16BitsToHalf(0b1_10000_1001001000)), // -3.140625
                (-Math.E, BitConverter.UInt16BitsToHalf(0b1_10000_0101110000)), // -2.71875
                (double.MaxValue, Half.PositiveInfinity), // Overflow
                (double.MinValue, Half.NegativeInfinity), // Overflow
                (double.PositiveInfinity, Half.PositiveInfinity), // Overflow
                (double.NegativeInfinity, Half.NegativeInfinity), // Overflow
                (double.NaN, Half.NaN), // Quiet Negative NaN
                (BitConverter.Int64BitsToDouble(0x7FF80000_00000000),
                    BitConverter.UInt16BitsToHalf(0b0_11111_1000000000)), // Quiet Positive NaN
                (BitConverter.Int64BitsToDouble(unchecked((long)0xFFFAAAAA_AAAAAAAA)),
                    BitConverter.UInt16BitsToHalf(0b1_11111_1010101010)), // Signalling Negative NaN
                (BitConverter.Int64BitsToDouble(0x7FFAAAAA_AAAAAAAA),
                    BitConverter.UInt16BitsToHalf(0b0_11111_1010101010)), // Signalling Positive NaN
                (double.Epsilon, BitConverter.UInt16BitsToHalf(0)), // Underflow
                (-double.Epsilon, BitConverter.UInt16BitsToHalf(0b1_00000_0000000000)), // Underflow
                (1d, BitConverter.UInt16BitsToHalf(0b0_01111_0000000000)), // 1
                (-1d, BitConverter.UInt16BitsToHalf(0b1_01111_0000000000)), // -1
                (0d, BitConverter.UInt16BitsToHalf(0)), // 0
                (-0d, BitConverter.UInt16BitsToHalf(0b1_00000_0000000000)), // -0
                (42d, BitConverter.UInt16BitsToHalf(0b0_10100_0101000000)), // 42
                (-42d, BitConverter.UInt16BitsToHalf(0b1_10100_0101000000)), // -42
                (0.1d, BitConverter.UInt16BitsToHalf(0b0_01011_1001100110)), // 0.0999755859375
                (-0.1d, BitConverter.UInt16BitsToHalf(0b1_01011_1001100110)), // -0.0999755859375
                (1.5d, BitConverter.UInt16BitsToHalf(0b0_01111_1000000000)), // 1.5
                (-1.5d, BitConverter.UInt16BitsToHalf(0b1_01111_1000000000)), // -1.5
                (1.5009765625d, BitConverter.UInt16BitsToHalf(0b0_01111_1000000001)), // 1.5009765625
                (-1.5009765625d, BitConverter.UInt16BitsToHalf(0b1_01111_1000000001)), // -1.5009765625
                (BitConverter.Int64BitsToDouble(0x3F10000000000000),
                    BitConverter.UInt16BitsToHalf(0b0_00001_0000000000)), // smallest normal
                (BitConverter.Int64BitsToDouble(0x3F0FF80000000000),
                    BitConverter.UInt16BitsToHalf(0b0_00000_1111111111)), // largest subnormal
                (BitConverter.Int64BitsToDouble(0x3f00000000000000),
                    BitConverter.UInt16BitsToHalf(0b0_00000_1000000000)), // middle subnormal
                (BitConverter.Int64BitsToDouble(0x3EFFF00000000000),
                    BitConverter.UInt16BitsToHalf(0b0_00000_0111111111)), // just below middle subnormal
                (BitConverter.Int64BitsToDouble(0x3E70000000000000),
                    BitConverter.UInt16BitsToHalf(0b0_00000_0000000001)), // smallest subnormal
                (BitConverter.Int64BitsToDouble(unchecked((long)0xBE70000000000000)),
                    BitConverter.UInt16BitsToHalf(0b1_00000_0000000001)), // highest negative subnormal
                (BitConverter.Int64BitsToDouble(unchecked((long)0xBEFFF00000000000)),
                    BitConverter.UInt16BitsToHalf(0b1_00000_0111111111)), // just above negative middle subnormal
                (BitConverter.Int64BitsToDouble(unchecked((long)0xBF00000000000000)),
                    BitConverter.UInt16BitsToHalf(0b1_00000_1000000000)), // negative middle subnormal
                (BitConverter.Int64BitsToDouble(unchecked((long)0xBF0FF80000000000)),
                    BitConverter.UInt16BitsToHalf(0b1_00000_1111111111)), // lowest negative subnormal
                (BitConverter.Int64BitsToDouble(unchecked((long)0xBF10000000000000)),
                    BitConverter.UInt16BitsToHalf(0b1_00001_0000000000)), // highest negative normal
                (BitConverter.Int64BitsToDouble(0x40900E0000000001),
                    BitConverter.UInt16BitsToHalf(0b0_11001_0000000100)), // 1027.5+ULP rounds up
                (BitConverter.Int64BitsToDouble(0x40900E0000000000),
                    BitConverter.UInt16BitsToHalf(0b0_11001_0000000100)), // 1027.5 rounds to even
                (BitConverter.Int64BitsToDouble(0x40900DFFFFFFFFFF),
                    BitConverter.UInt16BitsToHalf(0b0_11001_0000000011)), // 1027.5-ULP rounds down
                (BitConverter.Int64BitsToDouble(unchecked((long)0xC0900DFFFFFFFFFF)),
                    BitConverter.UInt16BitsToHalf(0b1_11001_0000000011)), // -1027.5+ULP rounds towards zero
                (BitConverter.Int64BitsToDouble(unchecked((long)0xC0900E0000000000)),
                    BitConverter.UInt16BitsToHalf(0b1_11001_0000000100)), // -1027.5 rounds to even
                (BitConverter.Int64BitsToDouble(unchecked((long)0xC0900E0000000001)),
                    BitConverter.UInt16BitsToHalf(0b1_11001_0000000100)), // -1027.5-ULP rounds away from zero
                (BitConverter.Int64BitsToDouble(0x3F001C0000000001),
                    BitConverter.UInt16BitsToHalf(0b0_00000_1000000100)), // subnormal + ULP rounds up
                (BitConverter.Int64BitsToDouble(0x3F001C0000000001),
                    BitConverter.UInt16BitsToHalf(0b0_00000_1000000100)), // subnormal rounds to even
                (BitConverter.Int64BitsToDouble(0x3F001BFFFFFFFFFF),
                    BitConverter.UInt16BitsToHalf(0b0_00000_1000000011)), // subnormal - ULP rounds down
                (BitConverter.Int64BitsToDouble(unchecked((long)0xBF001BFFFFFFFFFF)),
                    BitConverter.UInt16BitsToHalf(0b1_00000_1000000011)), // neg subnormal + ULP rounds higher
                (BitConverter.Int64BitsToDouble(unchecked((long)0xBF001C0000000000)),
                    BitConverter.UInt16BitsToHalf(0b1_00000_1000000100)), // neg subnormal rounds to even
                (BitConverter.Int64BitsToDouble(unchecked((long)0xBF001C0000000001)),
                    BitConverter.UInt16BitsToHalf(0b1_00000_1000000100)), // neg subnormal - ULP rounds lower
                (BitConverter.Int64BitsToDouble(0x3E60000000000000), BitConverter.UInt16BitsToHalf(0b0_00000_000000000)), // (half-precision minimum subnormal / 2) should underflow to zero
            };

            foreach ((double original, Half expected) in data)
            {
                yield return new object[] { original, expected };
            }*/
        }

        [MemberData(nameof(ExplicitConversion_FromDouble_TestData))]
        [Theory]
        public static void ExplicitConversion_FromDouble(double d, Decimal64 expected) // Check the underlying bits for verifying NaNs
        {
            Decimal64 result = (Decimal64)d;
            AssertBitwiseEqual(expected, result);
        }
        public static IEnumerable<object[]> Parse_Valid_TestData()
        {
            NumberStyles defaultStyle = NumberStyles.Float | NumberStyles.AllowThousands;

            NumberFormatInfo emptyFormat = NumberFormatInfo.CurrentInfo;

            var dollarSignCommaSeparatorFormat = new NumberFormatInfo()
            {
                CurrencySymbol = "$",
                CurrencyGroupSeparator = ","
            };

            var decimalSeparatorFormat = new NumberFormatInfo()
            {
                NumberDecimalSeparator = "."
            };

            NumberFormatInfo invariantFormat = NumberFormatInfo.InvariantInfo;

            //                                                          Decimal64(sign, q, c) = -1^sign * 10^q * c
            yield return new object[] { "-123", defaultStyle, null, new Decimal64(true, 0, 123) };
            yield return new object[] { "0", defaultStyle, null, Decimal64.Zero }; // TODO what kind of zero do we want to store here?
            yield return new object[] { "123", defaultStyle, null, new Decimal64(false, 0, 123) };
            yield return new object[] { "  123  ", defaultStyle, null, new Decimal64(false, 0, 123) };
            yield return new object[] { "567.89", defaultStyle, null, new Decimal64(false, -2, 56789) };
            yield return new object[] { "-567.89", defaultStyle, null, new Decimal64(true, -2, 56789) };
            yield return new object[] { "1E23", defaultStyle, null, new Decimal64(false, 23, 1) };

            yield return new object[] { emptyFormat.NumberDecimalSeparator + "234", defaultStyle, null, new Decimal64(false, -3, 234) };
            yield return new object[] { "234" + emptyFormat.NumberDecimalSeparator, defaultStyle, null, new Decimal64(false, 0, 234) };
            yield return new object[] { new string('0', 72) + "3" + new string('0', 38) + emptyFormat.NumberDecimalSeparator, defaultStyle, null, new Decimal64(false, 23, 3000000000000000) }; // could be wrong
            yield return new object[] { new string('0', 73) + "3" + new string('0', 38) + emptyFormat.NumberDecimalSeparator, defaultStyle, null, new Decimal64(false, 23, 3000000000000000) }; // could be wrong

            // Trailing zeros add sig figs and should be accounted for
            yield return new object[] { "1.000", defaultStyle, null, new Decimal64(false, -3, 1000) };
            yield return new object[] { "1.0000000000000000", defaultStyle, null, new Decimal64(false, -15, 1_000_000_000_000_000) };

            yield return new object[] { "100000000000005.0", defaultStyle, invariantFormat, new Decimal64(false, -1, 1000000000000050) };
            yield return new object[] { "1000000000000005.0", defaultStyle, invariantFormat, new Decimal64(false, 0, 1000000000000005) }; // The extra sigfig is not representable, unlike above
            yield return new object[] { "10000000000000005.0", defaultStyle, invariantFormat, new Decimal64(false, 1, 1000000000000000) }; // 10^16 + 5 is not exactly representable
            yield return new object[] { "10000000000000005.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001", defaultStyle, invariantFormat, new Decimal64(false, 1, 1000000000000001) };
            yield return new object[] { "10000000000000005.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001", defaultStyle, invariantFormat, new Decimal64(false, 1, 1000000000000001) };
            yield return new object[] { "10000000000000005.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001", defaultStyle, invariantFormat, new Decimal64(false, 1, 1000000000000001) };
            yield return new object[] { "5.005", defaultStyle, invariantFormat, new Decimal64(false, -3, 5005) };
            yield return new object[] { "5.050", defaultStyle, invariantFormat, new Decimal64(false, -3, 5050) };
            yield return new object[] { "5.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005", defaultStyle, invariantFormat, new Decimal64(false, -15, 5000000000000000) };
            yield return new object[] { "5.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005", defaultStyle, invariantFormat, new Decimal64(false, -15, 5000000000000000) };
            yield return new object[] { "5.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005", defaultStyle, invariantFormat, new Decimal64(false, -15, 5000000000000000) };
            yield return new object[] { "5.005000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", defaultStyle, invariantFormat, new Decimal64(false, -15, 5005000000000000) };
            yield return new object[] { "5.0050000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", defaultStyle, invariantFormat, new Decimal64(false, -15, 5005000000000000) };
            yield return new object[] { "5.0050000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", defaultStyle, invariantFormat, new Decimal64(false, -15, 5005000000000000) };

            yield return new object[] { "5005.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", defaultStyle, invariantFormat, new Decimal64(false, -12, 5005000000000000) };
            yield return new object[] { "50050.0", defaultStyle, invariantFormat, new Decimal64(false, -1, 500500) };
            yield return new object[] { "5005", defaultStyle, invariantFormat, new Decimal64(false, 0, 5005) };
            yield return new object[] { "050050", defaultStyle, invariantFormat, new Decimal64(false, 0, 50050) };
            yield return new object[] { "0.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", defaultStyle, invariantFormat, Decimal64.Zero };
            yield return new object[] { "0.005", defaultStyle, invariantFormat, new Decimal64(false, -3, 5) };
            yield return new object[] { "0.0500", defaultStyle, invariantFormat, new Decimal64(false, -4, 500) };
            yield return new object[] { "6250000000000000000000000000000000e-12", defaultStyle, invariantFormat, new Decimal64(false, 6, 6250000000000000) };
            yield return new object[] { "6250000e0", defaultStyle, invariantFormat, new Decimal64(false, 0, 6250000) };
            yield return new object[] { "6250100e-5", defaultStyle, invariantFormat, new Decimal64(false, -5, 6250100) };
            yield return new object[] { "625010.00e-4", defaultStyle, invariantFormat, new Decimal64(false, -6, 62501000) };
            yield return new object[] { "62500e-4", defaultStyle, invariantFormat, new Decimal64(false, -4, 62500) };
            yield return new object[] { "62500", defaultStyle, invariantFormat, new Decimal64(false, 0, 62500) };

            yield return new object[] { "123.1", NumberStyles.AllowDecimalPoint, null, new Decimal64(false, -1, 1231) };
            yield return new object[] { "1,000", NumberStyles.AllowThousands, null, new Decimal64(false, 0, 1000) };
            yield return new object[] { "1,000.0", NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, null, new Decimal64(false, -1, 10000) };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, new Decimal64(false, 0, 123) };
            yield return new object[] { "123.567", NumberStyles.Any, emptyFormat, new Decimal64(false, -3, 123567) };
            yield return new object[] { "123", NumberStyles.Float, emptyFormat, new Decimal64(false, 0, 123) };
            yield return new object[] { "$1,000", NumberStyles.Currency, dollarSignCommaSeparatorFormat, new Decimal64(false, 0, 1000) };
            yield return new object[] { "$1000", NumberStyles.Currency, dollarSignCommaSeparatorFormat, new Decimal64(false, 0, 1000) };
            yield return new object[] { "123.123", NumberStyles.Float, decimalSeparatorFormat, new Decimal64(false, -3, 123123) };
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, decimalSeparatorFormat, new Decimal64(true, 0, 123) }; // TODO HalfTests and SingleTests have this output -123, but I'm not exactly sure why

            yield return new object[] { "NaN", NumberStyles.Any, invariantFormat, Decimal64.NaN };
            yield return new object[] { "Infinity", NumberStyles.Any, invariantFormat, Decimal64.PositiveInfinity };
            yield return new object[] { "-Infinity", NumberStyles.Any, invariantFormat, Decimal64.NegativeInfinity };
        }

        [Theory]
        [MemberData(nameof(Parse_Valid_TestData))]
        public static void Parse(string value, NumberStyles style, IFormatProvider provider, Decimal64 expected)
        {
            bool isDefaultProvider = provider == null || provider == NumberFormatInfo.CurrentInfo;
            Decimal64 result;
            if ((style & ~(NumberStyles.Float | NumberStyles.AllowThousands)) == 0 && style != NumberStyles.None)
            {
                // Use Parse(string) or Parse(string, IFormatProvider)
                if (isDefaultProvider)
                {
                    Assert.True(Decimal64.TryParse(value, out result));
                    AssertEqualAndHaveSameQuantum(expected, result);

                    AssertEqualAndHaveSameQuantum(expected, Decimal64.Parse(value));
                }

                Assert.True(Decimal64.TryParse(value, provider: provider, out result));
                AssertEqualAndHaveSameQuantum(expected, result);

                AssertEqualAndHaveSameQuantum(expected, Decimal64.Parse(value, provider: provider));
            }

            // Use Parse(string, NumberStyles, IFormatProvider)
            Assert.True(Decimal64.TryParse(value, style, provider, out result));

            AssertEqualAndHaveSameQuantumOrBothNan(expected, result);
            AssertEqualAndHaveSameQuantumOrBothNan(expected, Decimal64.Parse(value, style, provider));

            if (isDefaultProvider)
            {
                // Use Parse(string, NumberStyles) or Parse(string, NumberStyles, IFormatProvider)
                Assert.True(Decimal64.TryParse(value, style, NumberFormatInfo.CurrentInfo, out result));
                AssertEqualAndHaveSameQuantum(expected, result);

                AssertEqualAndHaveSameQuantum(expected, Decimal64.Parse(value));
                AssertEqualAndHaveSameQuantum(expected, Decimal64.Parse(value, style));
                AssertEqualAndHaveSameQuantum(expected, Decimal64.Parse(value, style, NumberFormatInfo.CurrentInfo));
            }
        }

        public static IEnumerable<object[]> Parse_Invalid_TestData()
        {
            NumberStyles defaultStyle = NumberStyles.Float;

            var dollarSignDecimalSeparatorFormat = new NumberFormatInfo();
            dollarSignDecimalSeparatorFormat.CurrencySymbol = "$";
            dollarSignDecimalSeparatorFormat.NumberDecimalSeparator = ".";

            yield return new object[] { null, defaultStyle, null, typeof(ArgumentNullException) };
            yield return new object[] { "", defaultStyle, null, typeof(FormatException) };
            yield return new object[] { " ", defaultStyle, null, typeof(FormatException) };
            yield return new object[] { "Garbage", defaultStyle, null, typeof(FormatException) };

            yield return new object[] { "ab", defaultStyle, null, typeof(FormatException) }; // Hex value
            yield return new object[] { "(123)", defaultStyle, null, typeof(FormatException) }; // Parentheses
            yield return new object[] { (100.0f).ToString("C0"), defaultStyle, null, typeof(FormatException) }; // Currency

            yield return new object[] { (123.456f).ToString(), NumberStyles.Integer, null, typeof(FormatException) }; // Decimal
            yield return new object[] { "  " + (123.456f).ToString(), NumberStyles.None, null, typeof(FormatException) }; // Leading space
            yield return new object[] { (123.456f).ToString() + "   ", NumberStyles.None, null, typeof(FormatException) }; // Leading space
            yield return new object[] { "1E23", NumberStyles.None, null, typeof(FormatException) }; // Exponent

            yield return new object[] { "ab", NumberStyles.None, null, typeof(FormatException) }; // Negative hex value
            yield return new object[] { "  123  ", NumberStyles.None, null, typeof(FormatException) }; // Trailing and leading whitespace
        }

        [Theory]
        [MemberData(nameof(Parse_Invalid_TestData))]
        public static void Parse_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            bool isDefaultProvider = provider == null || provider == NumberFormatInfo.CurrentInfo;
            Decimal64 result;
            if ((style & ~(NumberStyles.Float | NumberStyles.AllowThousands)) == 0 && style != NumberStyles.None && (style & NumberStyles.AllowLeadingWhite) == (style & NumberStyles.AllowTrailingWhite))
            {
                // Use Parse(string) or Parse(string, IFormatProvider)
                if (isDefaultProvider)
                {
                    Assert.False(Decimal64.TryParse(value, out result));
                    Assert.Equal(default(Decimal64), result);

                    Assert.Throws(exceptionType, () => Decimal64.Parse(value));
                }

                Assert.False(Decimal64.TryParse(value, provider: provider, out result));
                Assert.Equal(default(Decimal64), result);

                Assert.Throws(exceptionType, () => Decimal64.Parse(value, provider: provider));
            }

            // Use Parse(string, NumberStyles, IFormatProvider)
            Assert.False(Decimal64.TryParse(value, style, provider, out result));
            Assert.Equal(default(Decimal64), result);

            Assert.Throws(exceptionType, () => Decimal64.Parse(value, style, provider));

            if (isDefaultProvider)
            {
                // Use Parse(string, NumberStyles) or Parse(string, NumberStyles, IFormatProvider)
                Assert.False(Decimal64.TryParse(value, style, NumberFormatInfo.CurrentInfo, out result));
                Assert.Equal(default(Decimal64), result);

                Assert.Throws(exceptionType, () => Decimal64.Parse(value, style));
                Assert.Throws(exceptionType, () => Decimal64.Parse(value, style, NumberFormatInfo.CurrentInfo));
            }
        }

        public static IEnumerable<object[]> Parse_ValidWithOffsetCount_TestData()
        {
            foreach (object[] inputs in Parse_Valid_TestData())
            {
                yield return new object[] { inputs[0], 0, ((string)inputs[0]).Length, inputs[1], inputs[2], inputs[3] };
            }

            const NumberStyles DefaultStyle = NumberStyles.Float | NumberStyles.AllowThousands;

            yield return new object[] { "-123", 1, 3, DefaultStyle, null, new Decimal64(false, 0, 123) };
            yield return new object[] { "-123", 0, 3, DefaultStyle, null, new Decimal64(true, 0, 12) };
            yield return new object[] { "1E23", 0, 3, DefaultStyle, null, new Decimal64(false, 2, 1) };
            yield return new object[] { "123", 0, 2, NumberStyles.Float, new NumberFormatInfo(), new Decimal64(false, 0, 12) };
            yield return new object[] { "$1,000", 1, 3, NumberStyles.Currency, new NumberFormatInfo() { CurrencySymbol = "$", CurrencyGroupSeparator = "," }, new Decimal64(false, 0, 10) };
            yield return new object[] { "(123)", 1, 3, NumberStyles.AllowParentheses, new NumberFormatInfo() { NumberDecimalSeparator = "." }, new Decimal64(false, 0, 123) };
            yield return new object[] { "-Infinity", 1, 8, NumberStyles.Any, NumberFormatInfo.InvariantInfo, Decimal64.PositiveInfinity };
        }

        [Theory]
        [MemberData(nameof(Parse_ValidWithOffsetCount_TestData))]
        public static void Parse_Span_Valid(string value, int offset, int count, NumberStyles style, IFormatProvider provider, Decimal64 expected)
        {
            bool isDefaultProvider = provider == null || provider == NumberFormatInfo.CurrentInfo;
            Decimal64 result;
            if ((style & ~(NumberStyles.Float | NumberStyles.AllowThousands)) == 0 && style != NumberStyles.None)
            {
                // Use Parse(string) or Parse(string, IFormatProvider)
                if (isDefaultProvider)
                {
                    Assert.True(Decimal64.TryParse(value.AsSpan(offset, count), out result));
                    AssertEqualAndHaveSameQuantum(expected, result);

                    AssertEqualAndHaveSameQuantum(expected, Decimal64.Parse(value.AsSpan(offset, count)));
                }

                Assert.True(Decimal64.TryParse(value.AsSpan(offset, count), provider: provider, out result));
                AssertEqualAndHaveSameQuantum(expected, result);

                AssertEqualAndHaveSameQuantum(expected, Decimal64.Parse(value.AsSpan(offset, count), provider: provider));
            }

            AssertEqualAndHaveSameQuantumOrBothNan(expected, Decimal64.Parse(value.AsSpan(offset, count), style, provider));

            Assert.True(Decimal64.TryParse(value.AsSpan(offset, count), style, provider, out result));
            AssertEqualAndHaveSameQuantumOrBothNan(expected, result);
        }

        [Theory]
        [MemberData(nameof(Parse_Invalid_TestData))]
        public static void Parse_Span_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            if (value != null)
            {
                Assert.Throws(exceptionType, () => Decimal64.Parse(value.AsSpan(), style, provider));

                Assert.False(Decimal64.TryParse(value.AsSpan(), style, provider, out Decimal64 result));
                Assert.Equal(default(Decimal64), result);
            }
        }

        public static IEnumerable<object[]> ToString_TestData()
        {
            yield return new object[] { new Decimal64(-4570, 0), "G", null, "-4570" };
            yield return new object[] { new Decimal64(0, 0), "G", null, "0" };
            yield return new object[] { new Decimal64(4570, 0), "G", null, "4570" };
            yield return new object[] { new Decimal64(45700, -1), "G", null, "4570.0" };

            yield return new object[] { Decimal64.NaN, "G", null, "NaN" };

            yield return new object[] { new Decimal64(2468, 0), "N", null, "2,468.00" };

            // Changing the negative pattern doesn't do anything without also passing in a format string
            var customNegativePattern = new NumberFormatInfo() { NumberNegativePattern = 0 };
            yield return new object[] { new Decimal64(-6310, 0), "G", customNegativePattern, "-6310" };

            var customNegativeSignDecimalGroupSeparator = new NumberFormatInfo()
            {
                NegativeSign = "#",
                NumberDecimalSeparator = "~",
                NumberGroupSeparator = "*"
            };
            yield return new object[] { new Decimal64(-2468, 0), "N", customNegativeSignDecimalGroupSeparator, "#2*468~00" };
            yield return new object[] { 2468.0f, "N", customNegativeSignDecimalGroupSeparator, "2*468~00" };

            var customNegativeSignGroupSeparatorNegativePattern = new NumberFormatInfo()
            {
                NegativeSign = "xx", // Set to trash to make sure it doesn't show up
                NumberGroupSeparator = "*",
                NumberNegativePattern = 0
            };
            yield return new object[] { new Decimal64(-2468, 0), "N", customNegativeSignGroupSeparatorNegativePattern, "(2*468.00)" };

            NumberFormatInfo invariantFormat = NumberFormatInfo.InvariantInfo;
            yield return new object[] { Decimal64.NaN, "G", invariantFormat, "NaN" };
            yield return new object[] { Decimal64.PositiveInfinity, "G", invariantFormat, "Infinity" };
            yield return new object[] { Decimal64.NegativeInfinity, "G", invariantFormat, "-Infinity" };
        }

        public static IEnumerable<object[]> ToString_TestData_NotNetFramework()
        {
            foreach (var testData in ToString_TestData())
            {
                yield return testData;
            }

            yield return new object[] { Decimal64.MinValue, "G", null, "-9.999999999999999E384" }; // 369 + 15
            yield return new object[] { Decimal64.MaxValue, "G", null, "9.999999999999999E384" };

            yield return new object[] { Decimal64.Epsilon, "G", null, "1E-398" };

            NumberFormatInfo invariantFormat = NumberFormatInfo.InvariantInfo;
            yield return new object[] { Decimal64.Epsilon, "G", invariantFormat, "1E-398" };

            yield return new object[] { new Decimal64(325, -1), "C100", invariantFormat, "\u00A432.5000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" };
            yield return new object[] { new Decimal64(325, -1), "P100", invariantFormat, "3,250.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000 %" };
            yield return new object[] { new Decimal64(325, -1), "E100", invariantFormat, "3.2500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000E+001" };
            yield return new object[] { new Decimal64(325, -1), "F100", invariantFormat, "32.5000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" };
            yield return new object[] { new Decimal64(325, -1), "N100", invariantFormat, "32.5000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" };
        }

        [Fact]
        public static void Test_ToString_NotNetFramework()
        {
            using (new ThreadCultureChange(CultureInfo.InvariantCulture))
            {
                foreach (object[] testdata in ToString_TestData_NotNetFramework())
                {
                    ToStringTest((Decimal64)testdata[0], (string)testdata[1], (IFormatProvider)testdata[2], (string)testdata[3]);
                }
            }
        }

        private static void ToStringTest(Decimal64 d, string format, IFormatProvider provider, string expected)
        {
            bool isDefaultProvider = provider == null;
            if (string.IsNullOrEmpty(format) || format.ToUpperInvariant() == "G")
            {
                if (isDefaultProvider)
                {
                    Assert.Equal(expected, d.ToString());
                    Assert.Equal(expected, d.ToString((IFormatProvider)null));
                }
                Assert.Equal(expected, d.ToString(provider));
            }
            if (isDefaultProvider)
            {
                Assert.Equal(expected.Replace('e', 'E'), d.ToString(format.ToUpperInvariant())); // If format is upper case, then exponents are printed in upper case
                Assert.Equal(expected.Replace('E', 'e'), d.ToString(format.ToLowerInvariant())); // If format is lower case, then exponents are printed in lower case
                Assert.Equal(expected.Replace('e', 'E'), d.ToString(format.ToUpperInvariant(), null));
                Assert.Equal(expected.Replace('E', 'e'), d.ToString(format.ToLowerInvariant(), null));
            }
            Assert.Equal(expected.Replace('e', 'E'), d.ToString(format.ToUpperInvariant(), provider));
            Assert.Equal(expected.Replace('E', 'e'), d.ToString(format.ToLowerInvariant(), provider));
        }

        [Fact]
        public static void ToString_InvalidFormat_ThrowsFormatException()
        {
            Decimal64 d = new Decimal64(123, 0);
            Assert.Throws<FormatException>(() => d.ToString("Y")); // Invalid format
            Assert.Throws<FormatException>(() => d.ToString("Y", null)); // Invalid format
            long intMaxPlus1 = (long)int.MaxValue + 1;
            string intMaxPlus1String = intMaxPlus1.ToString();
            Assert.Throws<FormatException>(() => d.ToString("E" + intMaxPlus1String));
        }

        [Fact]
        public static void TryFormat()
        {
            using (new ThreadCultureChange(CultureInfo.InvariantCulture))
            {
                foreach (object[] testdata in ToString_TestData())
                {
                    Decimal64 localI = (Decimal64)testdata[0];
                    string localFormat = (string)testdata[1];
                    IFormatProvider localProvider = (IFormatProvider)testdata[2];
                    string localExpected = (string)testdata[3];

                    try
                    {
                        char[] actual;
                        int charsWritten;

                        // Just right
                        actual = new char[localExpected.Length];
                        Assert.True(localI.TryFormat(actual.AsSpan(), out charsWritten, localFormat, localProvider));
                        Assert.Equal(localExpected.Length, charsWritten);
                        Assert.Equal(localExpected, new string(actual));

                        // Longer than needed
                        actual = new char[localExpected.Length + 1];
                        Assert.True(localI.TryFormat(actual.AsSpan(), out charsWritten, localFormat, localProvider));
                        Assert.Equal(localExpected.Length, charsWritten);
                        Assert.Equal(localExpected, new string(actual, 0, charsWritten));

                        // Too short
                        if (localExpected.Length > 0)
                        {
                            actual = new char[localExpected.Length - 1];
                            Assert.False(localI.TryFormat(actual.AsSpan(), out charsWritten, localFormat, localProvider));
                            Assert.Equal(0, charsWritten);
                        }
                    }
                    catch (Exception exc)
                    {
                        throw new Exception($"Failed on `{localI}`, `{localFormat}`, `{localProvider}`, `{localExpected}`. {exc}");
                    }
                }
            }
        }

        public static IEnumerable<object[]> ToStringRoundtrip_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity };
            yield return new object[] { Decimal64.MinValue };
            yield return new object[] { -Decimal64.Pi };
            yield return new object[] { -Decimal64.E };
            yield return new object[] { new Decimal64(-845512408, -9)};    // -8.45512408
            yield return new object[] { new Decimal64(-8455124080, -10) }; // -8.455124080
            yield return new object[] { new Decimal64(-0, 0) };            // -0
            yield return new object[] { new Decimal64(-0, -1) };           // -0.0
            yield return new object[] { Decimal64.NaN };
            yield return new object[] { new Decimal64(0, 0) };             // -0
            yield return new object[] { new Decimal64(0, -1) };            // -0.0
            yield return new object[] { new Decimal64(845512408, -9) };    // -8.45512408
            yield return new object[] { new Decimal64(8455124080, -10) };  // -8.455124080
            yield return new object[] { Decimal64.Epsilon };
            yield return new object[] { Decimal64.E };
            yield return new object[] { Decimal64.Pi };
            yield return new object[] { Decimal64.MaxValue };
            yield return new object[] { Decimal64.PositiveInfinity };

            yield return new object[] { new Decimal64(false, Decimal64.MinQExponent, 0x0020_0000_0000_0000) };   // Min Positive Special Encoding
            yield return new object[] { new Decimal64(false, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF) };   // Max Positive Normal Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MaxQExponent, 0x001F_FFFF_FFFF_FFFF) };    // Min Negative Normal Encoding
            yield return new object[] { new Decimal64(true, Decimal64.MinQExponent, 0x0020_0000_0000_0000) };    // Max Negative Special Encoding
            // TODO do we need more cases?
        }

        [Theory]
        [MemberData(nameof(ToStringRoundtrip_TestData))]
        public static void ToStringRoundtrip(object o_value)
        {
            Decimal64 value = (Decimal64)o_value;
            Decimal64 result = Decimal64.Parse(value.ToString());
            AssertBitwiseEqual(value, result);
        }

        [Theory]
        [MemberData(nameof(ToStringRoundtrip_TestData))]
        public static void ToStringRoundtrip_R(object o_value)
        {
            Decimal64 value = (Decimal64)o_value;
            Decimal64 result = Decimal64.Parse(value.ToString("R"));
            AssertBitwiseEqual(value, result);
        }

        // TODO do we want something like the following for conversions between Decimal128 or Decimal32?
/*        public static IEnumerable<object[]> RoundTripFloat_CornerCases()
        {
            // Magnitude smaller than 2^-24 maps to 0
            yield return new object[] { (Half)(5.2e-20f), 0 };
            yield return new object[] { (Half)(-5.2e-20f), 0 };
            // Magnitude smaller than 2^(map to subnormals
            yield return new object[] { (Half)(1.52e-5f), 1.52e-5f };
            yield return new object[] { (Half)(-1.52e-5f), -1.52e-5f };
            // Normal numbers
            yield return new object[] { (Half)(55.77f), 55.75f };
            yield return new object[] { (Half)(-55.77f), -55.75f };
            // Magnitude smaller than 2^(map to infinity
            yield return new object[] { (Half)(1.7e38f), float.PositiveInfinity };
            yield return new object[] { (Half)(-1.7e38f), float.NegativeInfinity };
            // Infinity and NaN map to infinity and Nan
            yield return new object[] { Half.PositiveInfinity, float.PositiveInfinity };
            yield return new object[] { Half.NegativeInfinity, float.NegativeInfinity };
            yield return new object[] { Half.NaN, float.NaN };
        }

        [Theory]
        [MemberData(nameof(RoundTripFloat_CornerCases))]
        public static void ToSingle(Half half, float verify)
        {
            float f = (float)half;
            Assert.Equal(f, verify, precision: 1);
        }*/

        [Fact]
        public static void EqualityMethodAndOperator()
        {
            Assert.True(Decimal64.NaN.Equals(Decimal64.NaN));
            Assert.False(Decimal64.NaN == Decimal64.NaN);
            Assert.Equal(Decimal64.NaN, Decimal64.NaN);
        }

        public static IEnumerable<object[]> MaxMagnitudeNumber_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.PositiveInfinity, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.PositiveInfinity, Decimal64.NegativeInfinity, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.MinValue, Decimal64.MaxValue, Decimal64.MaxValue };
            yield return new object[] { Decimal64.MaxValue, Decimal64.MinValue, Decimal64.MaxValue };
            yield return new object[] { Decimal64.NaN, Decimal64.NaN, Decimal64.NaN };
            yield return new object[] { Decimal64.NaN, Decimal64.One, Decimal64.One };
            yield return new object[] { Decimal64.One, Decimal64.NaN, Decimal64.One };
            yield return new object[] { Decimal64.PositiveInfinity, Decimal64.NaN, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.NaN, Decimal64.NegativeInfinity };
            yield return new object[] { Decimal64.NaN, Decimal64.PositiveInfinity, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.NaN, Decimal64.NegativeInfinity, Decimal64.NegativeInfinity };
            yield return new object[] { new Decimal64(-0, 0), new Decimal64(0, 0), new Decimal64(0, 0) };
            yield return new object[] { new Decimal64(0, 0), new Decimal64(-0, 0), new Decimal64(0, 0) };
            yield return new object[] { new Decimal64(2, 0), new Decimal64(-3, 0), new Decimal64(-3, 0) };
            yield return new object[] { new Decimal64(-3, 0), new Decimal64(2, 0), new Decimal64(-3, 0) };
            yield return new object[] { new Decimal64(3, 0), new Decimal64(-2, 0), new Decimal64(3, 0) };
            yield return new object[] { new Decimal64(-2, 0), new Decimal64(3, 0), new Decimal64(3, 0) };
        }

        [Theory]
        [MemberData(nameof(MaxMagnitudeNumber_TestData))]
        public static void MaxMagnitudeNumberTest(Decimal64 x, Decimal64 y, Decimal64 expectedResult)
        {
            AssertBitwiseEqual(expectedResult, Decimal64.MaxMagnitudeNumber(x, y));
        }

        public static IEnumerable<object[]> MaxNumber_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.PositiveInfinity, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.PositiveInfinity, Decimal64.NegativeInfinity, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.MinValue, Decimal64.MaxValue, Decimal64.MaxValue };
            yield return new object[] { Decimal64.MaxValue, Decimal64.MinValue, Decimal64.MaxValue };
            yield return new object[] { Decimal64.NaN, Decimal64.NaN, Decimal64.NaN };
            yield return new object[] { Decimal64.NaN, Decimal64.One, Decimal64.One };
            yield return new object[] { Decimal64.One, Decimal64.NaN, Decimal64.One };
            yield return new object[] { Decimal64.PositiveInfinity, Decimal64.NaN, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.NaN, Decimal64.NegativeInfinity };
            yield return new object[] { Decimal64.NaN, Decimal64.PositiveInfinity, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.NaN, Decimal64.NegativeInfinity, Decimal64.NegativeInfinity };
            yield return new object[] { new Decimal64(-0, 0), new Decimal64(0, 0), new Decimal64(0, 0) };
            yield return new object[] { new Decimal64(0, 0), new Decimal64(-0, 0), new Decimal64(0, 0) };
            yield return new object[] { new Decimal64(2, 0), new Decimal64(-3, 0), new Decimal64(2, 0) };
            yield return new object[] { new Decimal64(-3, 0), new Decimal64(2, 0), new Decimal64(2, 0) };
            yield return new object[] { new Decimal64(3, 0), new Decimal64(-2, 0), new Decimal64(3, 0) };
            yield return new object[] { new Decimal64(-2, 0), new Decimal64(3, 0), new Decimal64(3, 0) };
        }

        [Theory]
        [MemberData(nameof(MaxNumber_TestData))]
        public static void MaxNumberTest(Decimal64 x, Decimal64 y, Decimal64 expectedResult)
        {
            AssertBitwiseEqual(expectedResult, Decimal64.MaxNumber(x, y));
        }

        public static IEnumerable<object[]> MinMagnitudeNumber_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.PositiveInfinity, Decimal64.NegativeInfinity };
            yield return new object[] { Decimal64.PositiveInfinity, Decimal64.NegativeInfinity, Decimal64.NegativeInfinity };
            yield return new object[] { Decimal64.MinValue, Decimal64.MaxValue, Decimal64.MinValue };
            yield return new object[] { Decimal64.MaxValue, Decimal64.MinValue, Decimal64.MinValue };
            yield return new object[] { Decimal64.NaN, Decimal64.NaN, Decimal64.NaN };
            yield return new object[] { Decimal64.NaN, Decimal64.One, Decimal64.One };
            yield return new object[] { Decimal64.One, Decimal64.NaN, Decimal64.One };
            yield return new object[] { Decimal64.PositiveInfinity, Decimal64.NaN, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.NaN, Decimal64.NegativeInfinity };
            yield return new object[] { Decimal64.NaN, Decimal64.PositiveInfinity, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.NaN, Decimal64.NegativeInfinity, Decimal64.NegativeInfinity };
            yield return new object[] { new Decimal64(-0, 0), new Decimal64(0, 0), new Decimal64(-0, 0) };
            yield return new object[] { new Decimal64(0, 0), new Decimal64(-0, 0), new Decimal64(-0, 0) };
            yield return new object[] { new Decimal64(2, 0), new Decimal64(-3, 0), new Decimal64(2, 0) };
            yield return new object[] { new Decimal64(-3, 0), new Decimal64(2, 0), new Decimal64(2, 0) };
            yield return new object[] { new Decimal64(3, 0), new Decimal64(-2, 0), new Decimal64(-2, 0) };
            yield return new object[] { new Decimal64(-2, 0), new Decimal64(3, 0), new Decimal64(-2, 0) };
        }

        [Theory]
        [MemberData(nameof(MinMagnitudeNumber_TestData))]
        public static void MinMagnitudeNumberTest(Decimal64 x, Decimal64 y, Decimal64 expectedResult)
        {
            AssertBitwiseEqual(expectedResult, Decimal64.MinMagnitudeNumber(x, y));
        }

        public static IEnumerable<object[]> MinNumber_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.PositiveInfinity, Decimal64.NegativeInfinity };
            yield return new object[] { Decimal64.PositiveInfinity, Decimal64.NegativeInfinity, Decimal64.NegativeInfinity };
            yield return new object[] { Decimal64.MinValue, Decimal64.MaxValue, Decimal64.MinValue };
            yield return new object[] { Decimal64.MaxValue, Decimal64.MinValue, Decimal64.MinValue };
            yield return new object[] { Decimal64.NaN, Decimal64.NaN, Decimal64.NaN };
            yield return new object[] { Decimal64.NaN, Decimal64.One, Decimal64.One };
            yield return new object[] { Decimal64.One, Decimal64.NaN, Decimal64.One };
            yield return new object[] { Decimal64.PositiveInfinity, Decimal64.NaN, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.NaN, Decimal64.NegativeInfinity };
            yield return new object[] { Decimal64.NaN, Decimal64.PositiveInfinity, Decimal64.PositiveInfinity };
            yield return new object[] { Decimal64.NaN, Decimal64.NegativeInfinity, Decimal64.NegativeInfinity };
            yield return new object[] { new Decimal64(-0, 0), new Decimal64(0, 0), new Decimal64(-0, 0) };
            yield return new object[] { new Decimal64(0, 0), new Decimal64(-0, 0), new Decimal64(-0, 0) };
            yield return new object[] { new Decimal64(2, 0), new Decimal64(-3, 0), new Decimal64(-3, 0) };
            yield return new object[] { new Decimal64(-3, 0), new Decimal64(2, 0), new Decimal64(-3, 0) };
            yield return new object[] { new Decimal64(3, 0), new Decimal64(-2, 0), new Decimal64(-2, 0) };
            yield return new object[] { new Decimal64(-2, 0), new Decimal64(3, 0), new Decimal64(-2, 0) };
        }

        [Theory]
        [MemberData(nameof(MinNumber_TestData))]
        public static void MinNumberTest(Decimal64 x, Decimal64 y, Decimal64 expectedResult)
        {
            AssertBitwiseEqual(expectedResult, Decimal64.MinNumber(x, y));
        }

        /* TODO, eventually test the following here:
         * ExpM1
         * Exp2
         * Exp2M1
         * Exp10
         * Exp10M1
         * LogP1
         * Log2P1
         * Log10P1
         * Hypot
         * RootN
         * AcosPi
         * AsinPi
         * Atan2Pi
         * AtanPi
         * CosPi
         * SinPi
         * TanPi
         */

        public static IEnumerable<object[]> BitDecrement_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.NegativeInfinity };
            yield return new object[] { new Decimal64(-3141592653589793, -15), new Decimal64(new Decimal64(-3141592653589793, -15)._value + 1) };    // value: -(pi)
            yield return new object[] { new Decimal64(-2718281828459045, -15), new Decimal64(new Decimal64(-2718281828459045, -15)._value + 1) };    // value: -(e)
            yield return new object[] { new Decimal64(-2302585092994045, -15), new Decimal64(new Decimal64(-2302585092994045, -15)._value + 1) };    // value: -(ln(10))
            yield return new object[] { new Decimal64(-2302585092994045, -15), new Decimal64(new Decimal64(-2302585092994045, -15)._value + 1) };    // value: -(pi / 2)
            yield return new object[] { new Decimal64(-1442695040888963, -15), new Decimal64(new Decimal64(-1442695040888963, -15)._value + 1) };    // value: -(log2(e))
            yield return new object[] { new Decimal64(-1414213562373095, -15), new Decimal64(new Decimal64(-1414213562373095, -15)._value + 1) };    // value: -(sqrt(2))
            yield return new object[] { new Decimal64(-1128379167095512, -15), new Decimal64(new Decimal64(-1128379167095512, -15)._value + 1) };    // value: -(2 / sqrt(pi))
            yield return new object[] { new Decimal64(-1000000000000000, -15), new Decimal64(new Decimal64(-1000000000000000, -15)._value + 1) };    // value: -1
            yield return new object[] { new Decimal64(-7853981633974483, -16), new Decimal64(new Decimal64(-7853981633974483, -16)._value + 1) };    // value: -(pi / 4)
            yield return new object[] { new Decimal64(-7071067811865475, -16), new Decimal64(new Decimal64(-7071067811865475, -16)._value + 1) };    // value: -(1 / sqrt(2))
            yield return new object[] { new Decimal64(-6931471805599453, -16), new Decimal64(new Decimal64(-6931471805599453, -16)._value + 1) };    // value: -(ln(2))
            yield return new object[] { new Decimal64(-6366197723675814, -16), new Decimal64(new Decimal64(-6366197723675814, -16)._value + 1) };    // value: -(2 / pi)
            yield return new object[] { new Decimal64(-4342944819032518, -16), new Decimal64(new Decimal64(-4342944819032518, -16)._value + 1) };    // value: -(log10(e))
            yield return new object[] { new Decimal64(-3183098861837907, -16), new Decimal64(new Decimal64(-3183098861837907, -16)._value + 1) };    // value: -(1 / pi)
            yield return new object[] { Decimal64.NegativeZero, -Decimal64.Epsilon };
            yield return new object[] { Decimal64.NaN, Decimal64.NaN };
            yield return new object[] { Decimal64.Zero, -Decimal64.Epsilon };
            yield return new object[] { new Decimal64(3183098861837907, -16), new Decimal64(new Decimal64(3183098861837907, -16)._value - 1) };    // value: (1 / pi)
            yield return new object[] { new Decimal64(4342944819032518, -16), new Decimal64(new Decimal64(4342944819032518, -16)._value - 1) };    // value: (log10(e))
            yield return new object[] { new Decimal64(6366197723675814, -16), new Decimal64(new Decimal64(6366197723675814, -16)._value - 1) };    // value: (2 / pi)
            yield return new object[] { new Decimal64(6931471805599453, -16), new Decimal64(new Decimal64(6931471805599453, -16)._value - 1) };    // value: (ln(2))
            yield return new object[] { new Decimal64(7071067811865475, -16), new Decimal64(new Decimal64(7071067811865475, -16)._value - 1) };    // value: (1 / sqrt(2))
            yield return new object[] { new Decimal64(7853981633974483, -16), new Decimal64(new Decimal64(7853981633974483, -16)._value - 1) };    // value: (pi / 4)
            yield return new object[] { new Decimal64(1000000000000000, -15), new Decimal64(new Decimal64(1000000000000000, -15)._value - 1) };    // value: 1
            yield return new object[] { new Decimal64(1128379167095512, -15), new Decimal64(new Decimal64(1128379167095512, -15)._value - 1) };    // value: (2 / sqrt(pi))
            yield return new object[] { new Decimal64(1414213562373095, -15), new Decimal64(new Decimal64(1414213562373095, -15)._value - 1) };    // value: (sqrt(2))
            yield return new object[] { new Decimal64(1442695040888963, -15), new Decimal64(new Decimal64(1442695040888963, -15)._value - 1) };    // value: (log2(e))
            yield return new object[] { new Decimal64(2302585092994045, -15), new Decimal64(new Decimal64(2302585092994045, -15)._value - 1) };    // value: (pi / 2)
            yield return new object[] { new Decimal64(2302585092994045, -15), new Decimal64(new Decimal64(2302585092994045, -15)._value - 1) };    // value: (ln(10))
            yield return new object[] { new Decimal64(2718281828459045, -15), new Decimal64(new Decimal64(2718281828459045, -15)._value - 1) };    // value: (e)
            yield return new object[] { new Decimal64(3141592653589793, -15), new Decimal64(new Decimal64(3141592653589793, -15)._value - 1) };    // value: (pi)
            yield return new object[] { Decimal64.PositiveInfinity, Decimal64.MaxValue };
        }

        [Theory]
        [MemberData(nameof(BitDecrement_TestData))]
        public static void BitDecrement(Decimal64 value, Decimal64 expectedResult)
        {
            AssertBitwiseEqual(expectedResult, Decimal64.BitDecrement(value));
        }

        public static IEnumerable<object[]> BitIncrement_TestData()
        {
            yield return new object[] { Decimal64.NegativeInfinity, Decimal64.MinValue };
            yield return new object[] { new Decimal64(-3141592653589793, -15), new Decimal64(new Decimal64(-3141592653589793, -15)._value - 1) };    // value: -(pi)
            yield return new object[] { new Decimal64(-2718281828459045, -15), new Decimal64(new Decimal64(-2718281828459045, -15)._value - 1) };    // value: -(e)
            yield return new object[] { new Decimal64(-2302585092994045, -15), new Decimal64(new Decimal64(-2302585092994045, -15)._value - 1) };    // value: -(ln(10))
            yield return new object[] { new Decimal64(-2302585092994045, -15), new Decimal64(new Decimal64(-2302585092994045, -15)._value - 1) };    // value: -(pi / 2)
            yield return new object[] { new Decimal64(-1442695040888963, -15), new Decimal64(new Decimal64(-1442695040888963, -15)._value - 1) };    // value: -(log2(e))
            yield return new object[] { new Decimal64(-1414213562373095, -15), new Decimal64(new Decimal64(-1414213562373095, -15)._value - 1) };    // value: -(sqrt(2))
            yield return new object[] { new Decimal64(-1128379167095512, -15), new Decimal64(new Decimal64(-1128379167095512, -15)._value - 1) };    // value: -(2 / sqrt(pi))
            yield return new object[] { new Decimal64(-1000000000000000, -15), new Decimal64(new Decimal64(-1000000000000000, -15)._value - 1) };    // value: -1
            yield return new object[] { new Decimal64(-7853981633974483, -16), new Decimal64(new Decimal64(-7853981633974483, -16)._value - 1) };    // value: -(pi / 4)
            yield return new object[] { new Decimal64(-7071067811865475, -16), new Decimal64(new Decimal64(-7071067811865475, -16)._value - 1) };    // value: -(1 / sqrt(2))
            yield return new object[] { new Decimal64(-6931471805599453, -16), new Decimal64(new Decimal64(-6931471805599453, -16)._value - 1) };    // value: -(ln(2))
            yield return new object[] { new Decimal64(-6366197723675814, -16), new Decimal64(new Decimal64(-6366197723675814, -16)._value - 1) };    // value: -(2 / pi)
            yield return new object[] { new Decimal64(-4342944819032518, -16), new Decimal64(new Decimal64(-4342944819032518, -16)._value - 1) };    // value: -(log10(e))
            yield return new object[] { new Decimal64(-3183098861837907, -16), new Decimal64(new Decimal64(-3183098861837907, -16)._value - 1) };    // value: -(1 / pi)
            yield return new object[] { Decimal64.NegativeZero, Decimal64.Epsilon };
            yield return new object[] { Decimal64.NaN, Decimal64.NaN };
            yield return new object[] { Decimal64.Zero, Decimal64.Epsilon };
            yield return new object[] { new Decimal64(3183098861837907, -16), new Decimal64(new Decimal64(3183098861837907, -16)._value + 1) };    // value: (1 / pi)
            yield return new object[] { new Decimal64(4342944819032518, -16), new Decimal64(new Decimal64(4342944819032518, -16)._value + 1) };    // value: (log10(e))
            yield return new object[] { new Decimal64(6366197723675814, -16), new Decimal64(new Decimal64(6366197723675814, -16)._value + 1) };    // value: (2 / pi)
            yield return new object[] { new Decimal64(6931471805599453, -16), new Decimal64(new Decimal64(6931471805599453, -16)._value + 1) };    // value: (ln(2))
            yield return new object[] { new Decimal64(7071067811865475, -16), new Decimal64(new Decimal64(7071067811865475, -16)._value + 1) };    // value: (1 / sqrt(2))
            yield return new object[] { new Decimal64(7853981633974483, -16), new Decimal64(new Decimal64(7853981633974483, -16)._value + 1) };    // value: (pi / 4)
            yield return new object[] { new Decimal64(1000000000000000, -15), new Decimal64(new Decimal64(1000000000000000, -15)._value + 1) };    // value: 1
            yield return new object[] { new Decimal64(1128379167095512, -15), new Decimal64(new Decimal64(1128379167095512, -15)._value + 1) };    // value: (2 / sqrt(pi))
            yield return new object[] { new Decimal64(1414213562373095, -15), new Decimal64(new Decimal64(1414213562373095, -15)._value + 1) };    // value: (sqrt(2))
            yield return new object[] { new Decimal64(1442695040888963, -15), new Decimal64(new Decimal64(1442695040888963, -15)._value + 1) };    // value: (log2(e))
            yield return new object[] { new Decimal64(2302585092994045, -15), new Decimal64(new Decimal64(2302585092994045, -15)._value + 1) };    // value: (pi / 2)
            yield return new object[] { new Decimal64(2302585092994045, -15), new Decimal64(new Decimal64(2302585092994045, -15)._value + 1) };    // value: (ln(10))
            yield return new object[] { new Decimal64(2718281828459045, -15), new Decimal64(new Decimal64(2718281828459045, -15)._value + 1) };    // value: (e)
            yield return new object[] { new Decimal64(3141592653589793, -15), new Decimal64(new Decimal64(3141592653589793, -15)._value + 1) };    // value: (pi)
            yield return new object[] { Decimal64.PositiveInfinity, Decimal64.PositiveInfinity };
        }

        [Theory]
        [MemberData(nameof(BitIncrement_TestData))]
        public static void BitIncrement(Decimal64 value, Decimal64 expectedResult)
        {
            AssertBitwiseEqual(expectedResult, Decimal64.BitIncrement(value));
        }
    }
}
