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
            yield return new object[] { Decimal64.PositiveInfinity, false };                                            // Positive Infinity
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
            yield return new object[] { new Decimal64(-845512408, -9) };   // -8.45512408
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

        // Generic Math Tests
        internal static Half MinNormal => BitConverter.UInt16BitsToHalf(0x0400);

        internal static Half MaxSubnormal => BitConverter.UInt16BitsToHalf(0x03FF);

        internal static Half NegativeOne => BitConverter.UInt16BitsToHalf(0xBC00);

        internal static Half NegativeTwo => BitConverter.UInt16BitsToHalf(0xC000);

        internal static Half NegativeZero => BitConverter.UInt16BitsToHalf(0x8000);

        internal static Half One => BitConverter.UInt16BitsToHalf(0x3C00);

        internal static Half Two => BitConverter.UInt16BitsToHalf(0x4000);

        internal static Half Zero => BitConverter.UInt16BitsToHalf(0x0000);

        private static void AssertBitwiseEqual(Half expected, Half actual)
        {
            ushort expectedBits = BitConverter.HalfToUInt16Bits(expected);
            ushort actualBits = BitConverter.HalfToUInt16Bits(actual);

            if (expectedBits == actualBits)
            {
                return;
            }

            if (Half.IsNaN(expected) && Half.IsNaN(actual))
            {
                return;
            }

            throw new Xunit.Sdk.EqualException(expected, actual);
        }

        //
        // IAdditionOperators
        //

        [Fact]
        public static void op_AdditionTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(Half.NegativeInfinity, One));
            AssertBitwiseEqual(Half.MinValue, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(Half.MinValue, One));
            AssertBitwiseEqual(Zero, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(NegativeOne, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(-MinNormal, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(-MaxSubnormal, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(-Half.Epsilon, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(NegativeZero, One));
            AssertBitwiseEqual(Half.NaN, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(Half.NaN, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(Zero, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(Half.Epsilon, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(MaxSubnormal, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(MinNormal, One));
            AssertBitwiseEqual(Two, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(One, One));
            AssertBitwiseEqual(Half.MaxValue, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(Half.MaxValue, One));
            AssertBitwiseEqual(Half.PositiveInfinity, AdditionOperatorsHelper<Half, Half, Half>.op_Addition(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void op_CheckedAdditionTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(Half.NegativeInfinity, One));
            AssertBitwiseEqual(Half.MinValue, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(Half.MinValue, One));
            AssertBitwiseEqual(Zero, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(NegativeOne, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(-MinNormal, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(-MaxSubnormal, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(-Half.Epsilon, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(NegativeZero, One));
            AssertBitwiseEqual(Half.NaN, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(Half.NaN, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(Zero, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(Half.Epsilon, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(MaxSubnormal, One));
            AssertBitwiseEqual(One, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(MinNormal, One));
            AssertBitwiseEqual(Two, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(One, One));
            AssertBitwiseEqual(Half.MaxValue, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(Half.MaxValue, One));
            AssertBitwiseEqual(Half.PositiveInfinity, AdditionOperatorsHelper<Half, Half, Half>.op_CheckedAddition(Half.PositiveInfinity, One));
        }

        //
        // IAdditiveIdentity
        //

        [Fact]
        public static void AdditiveIdentityTest()
        {
            AssertBitwiseEqual(Zero, AdditiveIdentityHelper<Half, Half>.AdditiveIdentity);
        }

        //
        // IBinaryNumber
        //

        [Fact]
        public static void AllBitsSetTest()
        {
            Assert.Equal((ushort)0xFFFF, BitConverter.HalfToUInt16Bits(BinaryNumberHelper<Half>.AllBitsSet));
            Assert.Equal((ushort)0, (ushort)~BitConverter.HalfToUInt16Bits(BinaryNumberHelper<Half>.AllBitsSet));
        }

        [Fact]
        public static void IsPow2Test()
        {
            Assert.False(BinaryNumberHelper<Half>.IsPow2(Half.NegativeInfinity));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(Half.MinValue));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(NegativeOne));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(-MinNormal));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(-MaxSubnormal));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(-Half.Epsilon));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(NegativeZero));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(Half.NaN));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(Zero));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(Half.Epsilon));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(MaxSubnormal));
            Assert.True(BinaryNumberHelper<Half>.IsPow2(MinNormal));
            Assert.True(BinaryNumberHelper<Half>.IsPow2(One));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(Half.MaxValue));
            Assert.False(BinaryNumberHelper<Half>.IsPow2(Half.PositiveInfinity));
        }

        [Fact]
        public static void Log2Test()
        {
            AssertBitwiseEqual(Half.NaN, BinaryNumberHelper<Half>.Log2(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.NaN, BinaryNumberHelper<Half>.Log2(Half.MinValue));
            AssertBitwiseEqual(Half.NaN, BinaryNumberHelper<Half>.Log2(NegativeOne));
            AssertBitwiseEqual(Half.NaN, BinaryNumberHelper<Half>.Log2(-MinNormal));
            AssertBitwiseEqual(Half.NaN, BinaryNumberHelper<Half>.Log2(-MaxSubnormal));
            AssertBitwiseEqual(Half.NaN, BinaryNumberHelper<Half>.Log2(-Half.Epsilon));
            AssertBitwiseEqual(Half.NegativeInfinity, BinaryNumberHelper<Half>.Log2(NegativeZero));
            AssertBitwiseEqual(Half.NaN, BinaryNumberHelper<Half>.Log2(Half.NaN));
            AssertBitwiseEqual(Half.NegativeInfinity, BinaryNumberHelper<Half>.Log2(Zero));
            AssertBitwiseEqual((Half)(-24.0f), BinaryNumberHelper<Half>.Log2(Half.Epsilon));
            AssertBitwiseEqual((Half)(-14.0f), BinaryNumberHelper<Half>.Log2(MaxSubnormal));
            AssertBitwiseEqual((Half)(-14.0f), BinaryNumberHelper<Half>.Log2(MinNormal));
            AssertBitwiseEqual(Zero, BinaryNumberHelper<Half>.Log2(One));
            AssertBitwiseEqual((Half)16.0f, BinaryNumberHelper<Half>.Log2(Half.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, BinaryNumberHelper<Half>.Log2(Half.PositiveInfinity));
        }

        //
        // IComparisonOperators
        //

        [Fact]
        public static void op_GreaterThanTest()
        {
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(Half.NegativeInfinity, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(Half.MinValue, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(NegativeOne, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(-MinNormal, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(-MaxSubnormal, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(-Half.Epsilon, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(NegativeZero, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(Half.NaN, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(Zero, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(Half.Epsilon, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(MaxSubnormal, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(MinNormal, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(One, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(Half.MaxValue, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThan(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void op_GreaterThanOrEqualTest()
        {
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(Half.NegativeInfinity, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(Half.MinValue, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(NegativeOne, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(-MinNormal, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(-MaxSubnormal, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(-Half.Epsilon, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(NegativeZero, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(Half.NaN, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(Zero, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(Half.Epsilon, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(MaxSubnormal, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(MinNormal, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(One, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(Half.MaxValue, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_GreaterThanOrEqual(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void op_LessThanTest()
        {
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(Half.NegativeInfinity, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(Half.MinValue, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(NegativeOne, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(-MinNormal, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(-MaxSubnormal, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(-Half.Epsilon, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(NegativeZero, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(Half.NaN, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(Zero, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(Half.Epsilon, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(MaxSubnormal, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(MinNormal, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(One, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(Half.MaxValue, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThan(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void op_LessThanOrEqualTest()
        {
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(Half.NegativeInfinity, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(Half.MinValue, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(NegativeOne, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(-MinNormal, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(-MaxSubnormal, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(-Half.Epsilon, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(NegativeZero, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(Half.NaN, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(Zero, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(Half.Epsilon, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(MaxSubnormal, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(MinNormal, One));
            Assert.True(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(One, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(Half.MaxValue, One));
            Assert.False(ComparisonOperatorsHelper<Half, Half, bool>.op_LessThanOrEqual(Half.PositiveInfinity, One));
        }

        //
        // IDecrementOperators
        //

        [Fact]
        public static void op_DecrementTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, DecrementOperatorsHelper<Half>.op_Decrement(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.MinValue, DecrementOperatorsHelper<Half>.op_Decrement(Half.MinValue));
            AssertBitwiseEqual(NegativeTwo, DecrementOperatorsHelper<Half>.op_Decrement(NegativeOne));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_Decrement(-MinNormal));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_Decrement(-MaxSubnormal));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_Decrement(-Half.Epsilon));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_Decrement(NegativeZero));
            AssertBitwiseEqual(Half.NaN, DecrementOperatorsHelper<Half>.op_Decrement(Half.NaN));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_Decrement(Zero));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_Decrement(Half.Epsilon));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_Decrement(MaxSubnormal));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_Decrement(MinNormal));
            AssertBitwiseEqual(Zero, DecrementOperatorsHelper<Half>.op_Decrement(One));
            AssertBitwiseEqual(Half.MaxValue, DecrementOperatorsHelper<Half>.op_Decrement(Half.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, DecrementOperatorsHelper<Half>.op_Decrement(Half.PositiveInfinity));
        }

        [Fact]
        public static void op_CheckedDecrementTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, DecrementOperatorsHelper<Half>.op_CheckedDecrement(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.MinValue, DecrementOperatorsHelper<Half>.op_CheckedDecrement(Half.MinValue));
            AssertBitwiseEqual(NegativeTwo, DecrementOperatorsHelper<Half>.op_CheckedDecrement(NegativeOne));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_CheckedDecrement(-MinNormal));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_CheckedDecrement(-MaxSubnormal));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_CheckedDecrement(-Half.Epsilon));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_CheckedDecrement(NegativeZero));
            AssertBitwiseEqual(Half.NaN, DecrementOperatorsHelper<Half>.op_CheckedDecrement(Half.NaN));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_CheckedDecrement(Zero));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_CheckedDecrement(Half.Epsilon));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_CheckedDecrement(MaxSubnormal));
            AssertBitwiseEqual(NegativeOne, DecrementOperatorsHelper<Half>.op_CheckedDecrement(MinNormal));
            AssertBitwiseEqual(Zero, DecrementOperatorsHelper<Half>.op_CheckedDecrement(One));
            AssertBitwiseEqual(Half.MaxValue, DecrementOperatorsHelper<Half>.op_CheckedDecrement(Half.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, DecrementOperatorsHelper<Half>.op_CheckedDecrement(Half.PositiveInfinity));
        }

        //
        // IDivisionOperators
        //

        [Fact]
        public static void op_DivisionTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, DivisionOperatorsHelper<Half, Half, Half>.op_Division(Half.NegativeInfinity, Two));
            AssertBitwiseEqual((Half)(-32750.0f), DivisionOperatorsHelper<Half, Half, Half>.op_Division(Half.MinValue, Two));
            AssertBitwiseEqual((Half)(-0.5f), DivisionOperatorsHelper<Half, Half, Half>.op_Division(NegativeOne, Two));
            AssertBitwiseEqual((Half)(-3.05E-05f), DivisionOperatorsHelper<Half, Half, Half>.op_Division(-MinNormal, Two));
            AssertBitwiseEqual((Half)(-3.05E-05f), DivisionOperatorsHelper<Half, Half, Half>.op_Division(-MaxSubnormal, Two));
            AssertBitwiseEqual(NegativeZero, DivisionOperatorsHelper<Half, Half, Half>.op_Division(-Half.Epsilon, Two));
            AssertBitwiseEqual(NegativeZero, DivisionOperatorsHelper<Half, Half, Half>.op_Division(NegativeZero, Two));
            AssertBitwiseEqual(Half.NaN, DivisionOperatorsHelper<Half, Half, Half>.op_Division(Half.NaN, Two));
            AssertBitwiseEqual(Zero, DivisionOperatorsHelper<Half, Half, Half>.op_Division(Zero, Two));
            AssertBitwiseEqual(Zero, DivisionOperatorsHelper<Half, Half, Half>.op_Division(Half.Epsilon, Two));
            AssertBitwiseEqual((Half)3.05E-05f, DivisionOperatorsHelper<Half, Half, Half>.op_Division(MaxSubnormal, Two));
            AssertBitwiseEqual((Half)3.05E-05f, DivisionOperatorsHelper<Half, Half, Half>.op_Division(MinNormal, Two));
            AssertBitwiseEqual((Half)0.5f, DivisionOperatorsHelper<Half, Half, Half>.op_Division(One, Two));
            AssertBitwiseEqual((Half)32750.0f, DivisionOperatorsHelper<Half, Half, Half>.op_Division(Half.MaxValue, Two));
            AssertBitwiseEqual(Half.PositiveInfinity, DivisionOperatorsHelper<Half, Half, Half>.op_Division(Half.PositiveInfinity, Two));
        }

        [Fact]
        public static void op_CheckedDivisionTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(Half.NegativeInfinity, Two));
            AssertBitwiseEqual((Half)(-32750.0f), DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(Half.MinValue, Two));
            AssertBitwiseEqual((Half)(-0.5f), DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(NegativeOne, Two));
            AssertBitwiseEqual((Half)(-3.05E-05f), DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(-MinNormal, Two));
            AssertBitwiseEqual((Half)(-3.05E-05f), DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(-MaxSubnormal, Two));
            AssertBitwiseEqual(NegativeZero, DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(-Half.Epsilon, Two));
            AssertBitwiseEqual(NegativeZero, DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(NegativeZero, Two));
            AssertBitwiseEqual(Half.NaN, DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(Half.NaN, Two));
            AssertBitwiseEqual(Zero, DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(Zero, Two));
            AssertBitwiseEqual(Zero, DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(Half.Epsilon, Two));
            AssertBitwiseEqual((Half)3.05E-05f, DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(MaxSubnormal, Two));
            AssertBitwiseEqual((Half)3.05E-05f, DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(MinNormal, Two));
            AssertBitwiseEqual((Half)0.5f, DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(One, Two));
            AssertBitwiseEqual((Half)32750.0f, DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(Half.MaxValue, Two));
            AssertBitwiseEqual(Half.PositiveInfinity, DivisionOperatorsHelper<Half, Half, Half>.op_CheckedDivision(Half.PositiveInfinity, Two));
        }

        //
        // IEqualityOperators
        //

        [Fact]
        public static void op_EqualityTest()
        {
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(Half.NegativeInfinity, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(Half.MinValue, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(NegativeOne, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(-MinNormal, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(-MaxSubnormal, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(-Half.Epsilon, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(NegativeZero, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(Half.NaN, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(Zero, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(Half.Epsilon, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(MaxSubnormal, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(MinNormal, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(One, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(Half.MaxValue, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Equality(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void op_InequalityTest()
        {
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(Half.NegativeInfinity, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(Half.MinValue, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(NegativeOne, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(-MinNormal, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(-MaxSubnormal, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(-Half.Epsilon, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(NegativeZero, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(Half.NaN, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(Zero, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(Half.Epsilon, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(MaxSubnormal, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(MinNormal, One));
            Assert.False(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(One, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(Half.MaxValue, One));
            Assert.True(EqualityOperatorsHelper<Half, Half, bool>.op_Inequality(Half.PositiveInfinity, One));
        }

        //
        // IFloatingPoint
        //

        [Fact]
        public static void GetExponentByteCountTest()
        {
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(Half.NegativeInfinity));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(Half.MinValue));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(NegativeOne));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(-MinNormal));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(-MaxSubnormal));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(-Half.Epsilon));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(NegativeZero));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(Half.NaN));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(Zero));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(Half.Epsilon));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(MaxSubnormal));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(MinNormal));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(One));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(Half.MaxValue));
            Assert.Equal(1, FloatingPointHelper<Half>.GetExponentByteCount(Half.PositiveInfinity));
        }

        [Fact]
        public static void GetExponentShortestBitLengthTest()
        {
            Assert.Equal(5, FloatingPointHelper<Half>.GetExponentShortestBitLength(Half.NegativeInfinity));
            Assert.Equal(4, FloatingPointHelper<Half>.GetExponentShortestBitLength(Half.MinValue));
            Assert.Equal(0, FloatingPointHelper<Half>.GetExponentShortestBitLength(NegativeOne));
            Assert.Equal(5, FloatingPointHelper<Half>.GetExponentShortestBitLength(-MinNormal));
            Assert.Equal(5, FloatingPointHelper<Half>.GetExponentShortestBitLength(-MaxSubnormal));
            Assert.Equal(5, FloatingPointHelper<Half>.GetExponentShortestBitLength(-Half.Epsilon));
            Assert.Equal(5, FloatingPointHelper<Half>.GetExponentShortestBitLength(NegativeZero));
            Assert.Equal(5, FloatingPointHelper<Half>.GetExponentShortestBitLength(Half.NaN));
            Assert.Equal(5, FloatingPointHelper<Half>.GetExponentShortestBitLength(Zero));
            Assert.Equal(5, FloatingPointHelper<Half>.GetExponentShortestBitLength(Half.Epsilon));
            Assert.Equal(5, FloatingPointHelper<Half>.GetExponentShortestBitLength(MaxSubnormal));
            Assert.Equal(5, FloatingPointHelper<Half>.GetExponentShortestBitLength(MinNormal));
            Assert.Equal(0, FloatingPointHelper<Half>.GetExponentShortestBitLength(One));
            Assert.Equal(4, FloatingPointHelper<Half>.GetExponentShortestBitLength(Half.MaxValue));
            Assert.Equal(5, FloatingPointHelper<Half>.GetExponentShortestBitLength(Half.PositiveInfinity));
        }

        [Fact]
        public static void GetSignificandByteCountTest()
        {
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(Half.NegativeInfinity));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(Half.MinValue));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(NegativeOne));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(-MinNormal));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(-MaxSubnormal));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(-Half.Epsilon));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(NegativeZero));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(Half.NaN));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(Zero));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(Half.Epsilon));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(MaxSubnormal));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(MinNormal));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(One));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(Half.MaxValue));
            Assert.Equal(2, FloatingPointHelper<Half>.GetSignificandByteCount(Half.PositiveInfinity));
        }

        [Fact]
        public static void GetSignificandBitLengthTest()
        {
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(Half.NegativeInfinity));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(Half.MinValue));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(NegativeOne));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(-MinNormal));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(-MaxSubnormal));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(-Half.Epsilon));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(NegativeZero));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(Half.NaN));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(Zero));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(Half.Epsilon));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(MaxSubnormal));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(MinNormal));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(One));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(Half.MaxValue));
            Assert.Equal(11, FloatingPointHelper<Half>.GetSignificandBitLength(Half.PositiveInfinity));
        }

        [Fact]
        public static void TryWriteExponentBigEndianTest()
        {
            Span<byte> destination = stackalloc byte[1];
            int bytesWritten = 0;

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(Half.NegativeInfinity, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x10 }, destination.ToArray()); // +16

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(Half.MinValue, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x0F }, destination.ToArray()); // +15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(NegativeOne, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x00 }, destination.ToArray()); // +0

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(-MinNormal, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF2 }, destination.ToArray()); // -14

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(-MaxSubnormal, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(-Half.Epsilon, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(NegativeZero, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(Half.NaN, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x10 }, destination.ToArray()); // +16

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(Zero, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(Half.Epsilon, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(MaxSubnormal, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(MinNormal, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF2 }, destination.ToArray()); // -14

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(One, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x00 }, destination.ToArray()); // +0

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(Half.MaxValue, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x0F }, destination.ToArray()); // +15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentBigEndian(Half.PositiveInfinity, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x10 }, destination.ToArray()); // +16

            Assert.False(FloatingPointHelper<Half>.TryWriteExponentBigEndian(default, Span<byte>.Empty, out bytesWritten));
            Assert.Equal(0, bytesWritten);
            Assert.Equal(new byte[] { 0x10 }, destination.ToArray());
        }

        [Fact]
        public static void TryWriteExponentLittleEndianTest()
        {
            Span<byte> destination = stackalloc byte[1];
            int bytesWritten = 0;

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(Half.NegativeInfinity, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x10 }, destination.ToArray()); // +16

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(Half.MinValue, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x0F }, destination.ToArray()); // +15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(NegativeOne, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x00 }, destination.ToArray()); // +0

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(-MinNormal, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF2 }, destination.ToArray()); // -14

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(-MaxSubnormal, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(-Half.Epsilon, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(NegativeZero, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(Half.NaN, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x10 }, destination.ToArray()); // +16

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(Zero, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(Half.Epsilon, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(MaxSubnormal, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF1 }, destination.ToArray()); // -15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(MinNormal, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0xF2 }, destination.ToArray()); // -14

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(One, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x00 }, destination.ToArray()); // +0

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(Half.MaxValue, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x0F }, destination.ToArray()); // +15

            Assert.True(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(Half.PositiveInfinity, destination, out bytesWritten));
            Assert.Equal(1, bytesWritten);
            Assert.Equal(new byte[] { 0x10 }, destination.ToArray()); // +16

            Assert.False(FloatingPointHelper<Half>.TryWriteExponentLittleEndian(default, Span<byte>.Empty, out bytesWritten));
            Assert.Equal(0, bytesWritten);
            Assert.Equal(new byte[] { 0x10 }, destination.ToArray());
        }

        [Fact]
        public static void TryWriteSignificandBigEndianTest()
        {
            Span<byte> destination = stackalloc byte[2];
            int bytesWritten = 0;

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(Half.NegativeInfinity, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x04, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(Half.MinValue, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x07, 0xFF }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(NegativeOne, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x04, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(-MinNormal, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x04, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(-MaxSubnormal, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x03, 0xFF }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(-Half.Epsilon, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x01 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(NegativeZero, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(Half.NaN, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x06, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(Zero, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(Half.Epsilon, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x01 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(MaxSubnormal, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x03, 0xFF }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(MinNormal, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x04, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(One, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x04, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(Half.MaxValue, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x07, 0xFF }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(Half.PositiveInfinity, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x04, 0x00 }, destination.ToArray());

            Assert.False(FloatingPointHelper<Half>.TryWriteSignificandBigEndian(default, Span<byte>.Empty, out bytesWritten));
            Assert.Equal(0, bytesWritten);
            Assert.Equal(new byte[] { 0x04, 0x00 }, destination.ToArray());
        }

        [Fact]
        public static void TryWriteSignificandLittleEndianTest()
        {
            Span<byte> destination = stackalloc byte[2];
            int bytesWritten = 0;

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(Half.NegativeInfinity, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x04 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(Half.MinValue, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0xFF, 0x07 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(NegativeOne, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x04 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(-MinNormal, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x04 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(-MaxSubnormal, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0xFF, 0x03 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(-Half.Epsilon, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x01, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(NegativeZero, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(Half.NaN, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x06 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(Zero, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(Half.Epsilon, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x01, 0x00 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(MaxSubnormal, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0xFF, 0x03 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(MinNormal, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x04 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(One, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x04 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(Half.MaxValue, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0xFF, 0x07 }, destination.ToArray());

            Assert.True(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(Half.PositiveInfinity, destination, out bytesWritten));
            Assert.Equal(2, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x04 }, destination.ToArray());

            Assert.False(FloatingPointHelper<Half>.TryWriteSignificandLittleEndian(default, Span<byte>.Empty, out bytesWritten));
            Assert.Equal(0, bytesWritten);
            Assert.Equal(new byte[] { 0x00, 0x04 }, destination.ToArray());
        }

        //
        // IIncrementOperators
        //

        [Fact]
        public static void op_IncrementTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, IncrementOperatorsHelper<Half>.op_Increment(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.MinValue, IncrementOperatorsHelper<Half>.op_Increment(Half.MinValue));
            AssertBitwiseEqual(Zero, IncrementOperatorsHelper<Half>.op_Increment(NegativeOne));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_Increment(-MinNormal));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_Increment(-MaxSubnormal));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_Increment(-Half.Epsilon));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_Increment(NegativeZero));
            AssertBitwiseEqual(Half.NaN, IncrementOperatorsHelper<Half>.op_Increment(Half.NaN));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_Increment(Zero));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_Increment(Half.Epsilon));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_Increment(MaxSubnormal));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_Increment(MinNormal));
            AssertBitwiseEqual(Two, IncrementOperatorsHelper<Half>.op_Increment(One));
            AssertBitwiseEqual(Half.MaxValue, IncrementOperatorsHelper<Half>.op_Increment(Half.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, IncrementOperatorsHelper<Half>.op_Increment(Half.PositiveInfinity));
        }

        [Fact]
        public static void op_CheckedIncrementTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, IncrementOperatorsHelper<Half>.op_CheckedIncrement(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.MinValue, IncrementOperatorsHelper<Half>.op_CheckedIncrement(Half.MinValue));
            AssertBitwiseEqual(Zero, IncrementOperatorsHelper<Half>.op_CheckedIncrement(NegativeOne));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_CheckedIncrement(-MinNormal));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_CheckedIncrement(-MaxSubnormal));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_CheckedIncrement(-Half.Epsilon));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_CheckedIncrement(NegativeZero));
            AssertBitwiseEqual(Half.NaN, IncrementOperatorsHelper<Half>.op_CheckedIncrement(Half.NaN));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_CheckedIncrement(Zero));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_CheckedIncrement(Half.Epsilon));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_CheckedIncrement(MaxSubnormal));
            AssertBitwiseEqual(One, IncrementOperatorsHelper<Half>.op_CheckedIncrement(MinNormal));
            AssertBitwiseEqual(Two, IncrementOperatorsHelper<Half>.op_CheckedIncrement(One));
            AssertBitwiseEqual(Half.MaxValue, IncrementOperatorsHelper<Half>.op_CheckedIncrement(Half.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, IncrementOperatorsHelper<Half>.op_CheckedIncrement(Half.PositiveInfinity));
        }

        //
        // IMinMaxValue
        //

        [Fact]
        public static void MaxValueTest()
        {
            AssertBitwiseEqual(Half.MaxValue, MinMaxValueHelper<Half>.MaxValue);
        }

        [Fact]
        public static void MinValueTest()
        {
            AssertBitwiseEqual(Half.MinValue, MinMaxValueHelper<Half>.MinValue);
        }

        //
        // IModulusOperators
        //

        [Fact]
        public static void op_ModulusTest()
        {
            AssertBitwiseEqual(Half.NaN, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(Half.NegativeInfinity, Two));

            // https://github.com/dotnet/runtime/issues/67993
            // AssertBitwiseEqual(NegativeZero, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(Half.MinValue, PositiveTwo));

            AssertBitwiseEqual(NegativeOne, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(NegativeOne, Two));
            AssertBitwiseEqual(-MinNormal, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(-MinNormal, Two));
            AssertBitwiseEqual(-MaxSubnormal, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(-MaxSubnormal, Two));
            AssertBitwiseEqual(-Half.Epsilon, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(-Half.Epsilon, Two)); ;

            // https://github.com/dotnet/runtime/issues/67993
            // AssertBitwiseEqual(NegativeZero, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(NegativeZero, PositiveTwo));

            AssertBitwiseEqual(Half.NaN, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(Half.NaN, Two));
            AssertBitwiseEqual(Zero, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(Zero, Two));
            AssertBitwiseEqual(Half.Epsilon, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(Half.Epsilon, Two));
            AssertBitwiseEqual(MaxSubnormal, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(MaxSubnormal, Two));
            AssertBitwiseEqual(MinNormal, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(MinNormal, Two));
            AssertBitwiseEqual(One, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(One, Two));
            AssertBitwiseEqual(Zero, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(Half.MaxValue, Two));
            AssertBitwiseEqual(Half.NaN, ModulusOperatorsHelper<Half, Half, Half>.op_Modulus(Half.PositiveInfinity, Two));
        }

        //
        // IMultiplicativeIdentity
        //

        [Fact]
        public static void MultiplicativeIdentityTest()
        {
            AssertBitwiseEqual(One, MultiplicativeIdentityHelper<Half, Half>.MultiplicativeIdentity);
        }

        //
        // IMultiplyOperators
        //

        [Fact]
        public static void op_MultiplyTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(Half.NegativeInfinity, Two));
            AssertBitwiseEqual(Half.NegativeInfinity, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(Half.MinValue, Two));
            AssertBitwiseEqual(NegativeTwo, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(NegativeOne, Two));
            AssertBitwiseEqual((Half)(-0.0001221f), MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(-MinNormal, Two));
            AssertBitwiseEqual((Half)(-0.00012195f), MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(-MaxSubnormal, Two));
            AssertBitwiseEqual((Half)(-1E-07f), MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(-Half.Epsilon, Two));
            AssertBitwiseEqual(NegativeZero, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(NegativeZero, Two));
            AssertBitwiseEqual(Half.NaN, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(Half.NaN, Two));
            AssertBitwiseEqual(Zero, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(Zero, Two));
            AssertBitwiseEqual((Half)1E-07f, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(Half.Epsilon, Two));
            AssertBitwiseEqual((Half)0.00012195f, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(MaxSubnormal, Two));
            AssertBitwiseEqual((Half)0.0001221f, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(MinNormal, Two));
            AssertBitwiseEqual(Two, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(One, Two));
            AssertBitwiseEqual(Half.PositiveInfinity, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(Half.MaxValue, Two));
            AssertBitwiseEqual(Half.PositiveInfinity, MultiplyOperatorsHelper<Half, Half, Half>.op_Multiply(Half.PositiveInfinity, Two));
        }

        [Fact]
        public static void op_CheckedMultiplyTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(Half.NegativeInfinity, Two));
            AssertBitwiseEqual(Half.NegativeInfinity, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(Half.MinValue, Two));
            AssertBitwiseEqual(NegativeTwo, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(NegativeOne, Two));
            AssertBitwiseEqual((Half)(-0.0001221f), MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(-MinNormal, Two));
            AssertBitwiseEqual((Half)(-0.00012195f), MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(-MaxSubnormal, Two));
            AssertBitwiseEqual((Half)(-1E-07f), MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(-Half.Epsilon, Two));
            AssertBitwiseEqual(NegativeZero, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(NegativeZero, Two));
            AssertBitwiseEqual(Half.NaN, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(Half.NaN, Two));
            AssertBitwiseEqual(Zero, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(Zero, Two));
            AssertBitwiseEqual((Half)1E-07f, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(Half.Epsilon, Two));
            AssertBitwiseEqual((Half)0.00012195f, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(MaxSubnormal, Two));
            AssertBitwiseEqual((Half)0.0001221f, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(MinNormal, Two));
            AssertBitwiseEqual(Two, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(One, Two));
            AssertBitwiseEqual(Half.PositiveInfinity, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(Half.MaxValue, Two));
            AssertBitwiseEqual(Half.PositiveInfinity, MultiplyOperatorsHelper<Half, Half, Half>.op_CheckedMultiply(Half.PositiveInfinity, Two));
        }

        //
        // INumber
        //

        [Fact]
        public static void ClampTest()
        {
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(Half.NegativeInfinity, One, (Half)63.0f));
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(Half.MinValue, One, (Half)63.0f));
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(NegativeOne, One, (Half)63.0f));
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(-MinNormal, One, (Half)63.0f));
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(-MaxSubnormal, One, (Half)63.0f));
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(-Half.Epsilon, One, (Half)63.0f));
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(NegativeZero, One, (Half)63.0f));
            AssertBitwiseEqual(Half.NaN, NumberHelper<Half>.Clamp(Half.NaN, One, (Half)63.0f));
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(Zero, One, (Half)63.0f));
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(Half.Epsilon, One, (Half)63.0f));
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(MaxSubnormal, One, (Half)63.0f));
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(MinNormal, One, (Half)63.0f));
            AssertBitwiseEqual(One, NumberHelper<Half>.Clamp(One, One, (Half)63.0f));
            AssertBitwiseEqual((Half)63.0f, NumberHelper<Half>.Clamp(Half.MaxValue, One, (Half)63.0f));
            AssertBitwiseEqual((Half)63.0f, NumberHelper<Half>.Clamp(Half.PositiveInfinity, One, (Half)63.0f));
        }

        [Fact]
        public static void MaxTest()
        {
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(Half.NegativeInfinity, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(Half.MinValue, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(NegativeOne, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(-MinNormal, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(-MaxSubnormal, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(-Half.Epsilon, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(NegativeZero, One));
            AssertBitwiseEqual(Half.NaN, NumberHelper<Half>.Max(Half.NaN, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(Zero, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(Half.Epsilon, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(MaxSubnormal, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(MinNormal, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Max(One, One));
            AssertBitwiseEqual(Half.MaxValue, NumberHelper<Half>.Max(Half.MaxValue, One));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberHelper<Half>.Max(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void MaxNumberTest()
        {
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(Half.NegativeInfinity, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(Half.MinValue, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(NegativeOne, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(-MinNormal, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(-MaxSubnormal, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(-Half.Epsilon, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(NegativeZero, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(Half.NaN, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(Zero, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(Half.Epsilon, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(MaxSubnormal, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(MinNormal, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MaxNumber(One, One));
            AssertBitwiseEqual(Half.MaxValue, NumberHelper<Half>.MaxNumber(Half.MaxValue, One));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberHelper<Half>.MaxNumber(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void MinTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberHelper<Half>.Min(Half.NegativeInfinity, One));
            AssertBitwiseEqual(Half.MinValue, NumberHelper<Half>.Min(Half.MinValue, One));
            AssertBitwiseEqual(NegativeOne, NumberHelper<Half>.Min(NegativeOne, One));
            AssertBitwiseEqual(-MinNormal, NumberHelper<Half>.Min(-MinNormal, One));
            AssertBitwiseEqual(-MaxSubnormal, NumberHelper<Half>.Min(-MaxSubnormal, One));
            AssertBitwiseEqual(-Half.Epsilon, NumberHelper<Half>.Min(-Half.Epsilon, One));
            AssertBitwiseEqual(NegativeZero, NumberHelper<Half>.Min(NegativeZero, One));
            AssertBitwiseEqual(Half.NaN, NumberHelper<Half>.Min(Half.NaN, One));
            AssertBitwiseEqual(Zero, NumberHelper<Half>.Min(Zero, One));
            AssertBitwiseEqual(Half.Epsilon, NumberHelper<Half>.Min(Half.Epsilon, One));
            AssertBitwiseEqual(MaxSubnormal, NumberHelper<Half>.Min(MaxSubnormal, One));
            AssertBitwiseEqual(MinNormal, NumberHelper<Half>.Min(MinNormal, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Min(One, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Min(Half.MaxValue, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.Min(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void MinNumberTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberHelper<Half>.MinNumber(Half.NegativeInfinity, One));
            AssertBitwiseEqual(Half.MinValue, NumberHelper<Half>.MinNumber(Half.MinValue, One));
            AssertBitwiseEqual(NegativeOne, NumberHelper<Half>.MinNumber(NegativeOne, One));
            AssertBitwiseEqual(-MinNormal, NumberHelper<Half>.MinNumber(-MinNormal, One));
            AssertBitwiseEqual(-MaxSubnormal, NumberHelper<Half>.MinNumber(-MaxSubnormal, One));
            AssertBitwiseEqual(-Half.Epsilon, NumberHelper<Half>.MinNumber(-Half.Epsilon, One));
            AssertBitwiseEqual(NegativeZero, NumberHelper<Half>.MinNumber(NegativeZero, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MinNumber(Half.NaN, One));
            AssertBitwiseEqual(Zero, NumberHelper<Half>.MinNumber(Zero, One));
            AssertBitwiseEqual(Half.Epsilon, NumberHelper<Half>.MinNumber(Half.Epsilon, One));
            AssertBitwiseEqual(MaxSubnormal, NumberHelper<Half>.MinNumber(MaxSubnormal, One));
            AssertBitwiseEqual(MinNormal, NumberHelper<Half>.MinNumber(MinNormal, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MinNumber(One, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MinNumber(Half.MaxValue, One));
            AssertBitwiseEqual(One, NumberHelper<Half>.MinNumber(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void SignTest()
        {
            Assert.Equal(-1, NumberHelper<Half>.Sign(Half.NegativeInfinity));
            Assert.Equal(-1, NumberHelper<Half>.Sign(Half.MinValue));
            Assert.Equal(-1, NumberHelper<Half>.Sign(NegativeOne));
            Assert.Equal(-1, NumberHelper<Half>.Sign(-MinNormal));
            Assert.Equal(-1, NumberHelper<Half>.Sign(-MaxSubnormal));
            Assert.Equal(-1, NumberHelper<Half>.Sign(-Half.Epsilon));

            Assert.Equal(0, NumberHelper<Half>.Sign(NegativeZero));
            Assert.Equal(0, NumberHelper<Half>.Sign(Zero));

            Assert.Equal(1, NumberHelper<Half>.Sign(Half.Epsilon));
            Assert.Equal(1, NumberHelper<Half>.Sign(MaxSubnormal));
            Assert.Equal(1, NumberHelper<Half>.Sign(MinNormal));
            Assert.Equal(1, NumberHelper<Half>.Sign(One));
            Assert.Equal(1, NumberHelper<Half>.Sign(Half.MaxValue));
            Assert.Equal(1, NumberHelper<Half>.Sign(Half.PositiveInfinity));

            Assert.Throws<ArithmeticException>(() => NumberHelper<Half>.Sign(Half.NaN));
        }

        //
        // INumberBase
        //

        [Fact]
        public static void OneTest()
        {
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.One);
        }

        [Fact]
        public static void RadixTest()
        {
            Assert.Equal(2, NumberBaseHelper<Half>.Radix);
        }

        [Fact]
        public static void ZeroTest()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.Zero);
        }

        [Fact]
        public static void AbsTest()
        {
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.Abs(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.MaxValue, NumberBaseHelper<Half>.Abs(Half.MinValue));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.Abs(NegativeOne));
            AssertBitwiseEqual(MinNormal, NumberBaseHelper<Half>.Abs(-MinNormal));
            AssertBitwiseEqual(MaxSubnormal, NumberBaseHelper<Half>.Abs(-MaxSubnormal));
            AssertBitwiseEqual(Half.Epsilon, NumberBaseHelper<Half>.Abs(-Half.Epsilon));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.Abs(NegativeZero));
            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.Abs(Half.NaN));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.Abs(Zero));
            AssertBitwiseEqual(Half.Epsilon, NumberBaseHelper<Half>.Abs(Half.Epsilon));
            AssertBitwiseEqual(MaxSubnormal, NumberBaseHelper<Half>.Abs(MaxSubnormal));
            AssertBitwiseEqual(MinNormal, NumberBaseHelper<Half>.Abs(MinNormal));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.Abs(One));
            AssertBitwiseEqual(Half.MaxValue, NumberBaseHelper<Half>.Abs(Half.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.Abs(Half.PositiveInfinity));
        }

        [Fact]
        public static void CreateCheckedFromByteTest()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<byte>(0x00));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<byte>(0x01));
            AssertBitwiseEqual((Half)127.0f, NumberBaseHelper<Half>.CreateChecked<byte>(0x7F));
            AssertBitwiseEqual((Half)128.0f, NumberBaseHelper<Half>.CreateChecked<byte>(0x80));
            AssertBitwiseEqual((Half)255.0f, NumberBaseHelper<Half>.CreateChecked<byte>(0xFF));
        }

        [Fact]
        public static void CreateCheckedFromCharTest()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<char>((char)0x0000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<char>((char)0x0001));
            AssertBitwiseEqual((Half)32767.0f, NumberBaseHelper<Half>.CreateChecked<char>((char)0x7FFF));
            AssertBitwiseEqual((Half)32768.0f, NumberBaseHelper<Half>.CreateChecked<char>((char)0x8000));
            AssertBitwiseEqual((Half)65535.0f, NumberBaseHelper<Half>.CreateChecked<char>((char)0xFFFF));
        }

        [Fact]
        public static void CreateCheckedFromDecimalTest()
        {

            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateChecked<decimal>(decimal.MinValue));
            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateChecked<decimal>(-1.0m));
            AssertBitwiseEqual(Half.NegativeZero, NumberBaseHelper<Half>.CreateChecked<decimal>(-0.0m));
            AssertBitwiseEqual(Half.Zero, NumberBaseHelper<Half>.CreateChecked<decimal>(+0.0m));
            AssertBitwiseEqual(Half.One, NumberBaseHelper<Half>.CreateChecked<decimal>(+1.0m));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateChecked<decimal>(decimal.MaxValue));
        }

        [Fact]
        public static void CreateCheckedFromDoubleTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateChecked<double>(double.NegativeInfinity));
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateChecked<double>(double.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateChecked<double>(-1.0));

            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<double>(-DoubleTests_GenericMath.MinNormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<double>(-DoubleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<double>(-double.Epsilon));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<double>(-0.0));

            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<double>(+0.0));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<double>(double.Epsilon));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<double>(DoubleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<double>(DoubleTests_GenericMath.MinNormal));

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<double>(1.0));

            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateChecked<double>(double.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateChecked<double>(double.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateChecked<double>(double.NaN));
        }

        [Fact]
        public static void CreateCheckedFromHalfTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateChecked<Half>(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.MinValue, NumberBaseHelper<Half>.CreateChecked<Half>(Half.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateChecked<Half>(Half.NegativeOne));

            AssertBitwiseEqual(-MinNormal, NumberBaseHelper<Half>.CreateChecked<Half>(-MinNormal));
            AssertBitwiseEqual(-MaxSubnormal, NumberBaseHelper<Half>.CreateChecked<Half>(-MaxSubnormal));
            AssertBitwiseEqual(-Half.Epsilon, NumberBaseHelper<Half>.CreateChecked<Half>(-Half.Epsilon));
            AssertBitwiseEqual(Half.NegativeZero, NumberBaseHelper<Half>.CreateChecked<Half>(Half.NegativeZero));

            AssertBitwiseEqual(Half.Zero, NumberBaseHelper<Half>.CreateChecked<Half>(Half.Zero));
            AssertBitwiseEqual(Half.Epsilon, NumberBaseHelper<Half>.CreateChecked<Half>(Half.Epsilon));
            AssertBitwiseEqual(MaxSubnormal, NumberBaseHelper<Half>.CreateChecked<Half>(MaxSubnormal));
            AssertBitwiseEqual(MinNormal, NumberBaseHelper<Half>.CreateChecked<Half>(MinNormal));

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<Half>(Half.One));

            AssertBitwiseEqual(Half.MaxValue, NumberBaseHelper<Half>.CreateChecked<Half>(Half.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateChecked<Half>(Half.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateChecked<Half>(Half.NaN));
        }

        [Fact]
        public static void CreateCheckedFromInt16Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<short>(0x0000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<short>(0x0001));
            AssertBitwiseEqual((Half)32767.0f, NumberBaseHelper<Half>.CreateChecked<short>(0x7FFF));
            AssertBitwiseEqual((Half)(-32768.0f), NumberBaseHelper<Half>.CreateChecked<short>(unchecked((short)0x8000)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateChecked<short>(unchecked((short)0xFFFF)));
        }

        [Fact]
        public static void CreateCheckedFromInt32Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<int>(0x00000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<int>(0x00000001));
            AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateChecked<int>(0x7FFFFFFF));
            AssertBitwiseEqual((Half)(-2147483648.0f), NumberBaseHelper<Half>.CreateChecked<int>(unchecked((int)0x80000000)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateChecked<int>(unchecked((int)0xFFFFFFFF)));
        }

        [Fact]
        public static void CreateCheckedFromInt64Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<long>(0x0000000000000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<long>(0x0000000000000001));
            AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateChecked<long>(0x7FFFFFFFFFFFFFFF));
            AssertBitwiseEqual((Half)(-9223372036854775808.0f), NumberBaseHelper<Half>.CreateChecked<long>(unchecked(unchecked((long)0x8000000000000000))));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateChecked<long>(unchecked(unchecked((long)0xFFFFFFFFFFFFFFFF))));
        }

        [Fact]
        public static void CreateCheckedFromInt128Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<Int128>(new Int128(0x0000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<Int128>(new Int128(0x0000_0000_0000_0000, 0x0000_0000_0000_0001)));
            AssertBitwiseEqual((Half)170141183460469231731687303715884105727.0, NumberBaseHelper<Half>.CreateChecked<Int128>(new Int128(0x7FFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
            AssertBitwiseEqual((Half)(-170141183460469231731687303715884105728.0f), NumberBaseHelper<Half>.CreateChecked<Int128>(new Int128(0x8000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateChecked<Int128>(new Int128(0xFFFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
        }

        [Fact]
        public static void CreateCheckedFromIntPtrTest()
        {
            if (Environment.Is64BitProcess)
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<nint>(unchecked((nint)0x0000000000000000)));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<nint>(unchecked((nint)0x0000000000000001)));
                AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateChecked<nint>(unchecked((nint)0x7FFFFFFFFFFFFFFF)));
                AssertBitwiseEqual((Half)(-9223372036854775808.0f), NumberBaseHelper<Half>.CreateChecked<nint>(unchecked((nint)0x8000000000000000)));
                AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateChecked<nint>(unchecked((nint)0xFFFFFFFFFFFFFFFF)));
            }
            else
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<nint>((nint)0x00000000));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<nint>((nint)0x00000001));
                AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateChecked<nint>((nint)0x7FFFFFFF));
                AssertBitwiseEqual((Half)(-2147483648.0f), NumberBaseHelper<Half>.CreateChecked<nint>(unchecked((nint)0x80000000)));
                AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateChecked<nint>(unchecked((nint)0xFFFFFFFF)));
            }
        }

        [Fact]
        public static void CreateCheckedFromNFloatTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateChecked<NFloat>(NFloat.NegativeInfinity));
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateChecked<NFloat>(NFloat.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateChecked<NFloat>(-1.0f));

            if (Environment.Is64BitProcess)
            {
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<NFloat>((NFloat)(-DoubleTests_GenericMath.MinNormal)));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<NFloat>((NFloat)(-DoubleTests_GenericMath.MaxSubnormal)));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<NFloat>(-NFloat.Epsilon));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<NFloat>(-0.0f));

                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<NFloat>(+0.0f));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<NFloat>(NFloat.Epsilon));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<NFloat>((NFloat)DoubleTests_GenericMath.MaxSubnormal));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<NFloat>((NFloat)DoubleTests_GenericMath.MinNormal));
            }
            else
            {
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<NFloat>(-SingleTests_GenericMath.MinNormal));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<NFloat>(-SingleTests_GenericMath.MaxSubnormal));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<NFloat>(-NFloat.Epsilon));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<NFloat>(-0.0f));

                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<NFloat>(+0.0f));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<NFloat>(NFloat.Epsilon));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<NFloat>(SingleTests_GenericMath.MaxSubnormal));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<NFloat>(SingleTests_GenericMath.MinNormal));
            }

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<NFloat>(1.0f));

            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateChecked<NFloat>(NFloat.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateChecked<NFloat>(NFloat.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateChecked<NFloat>(NFloat.NaN));
        }

        [Fact]
        public static void CreateCheckedFromSByteTest()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<sbyte>(0x00));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<sbyte>(0x01));
            AssertBitwiseEqual((Half)127.0f, NumberBaseHelper<Half>.CreateChecked<sbyte>(0x7F));
            AssertBitwiseEqual((Half)(-128.0f), NumberBaseHelper<Half>.CreateChecked<sbyte>(unchecked((sbyte)0x80)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateChecked<sbyte>(unchecked((sbyte)0xFF)));
        }

        [Fact]
        public static void CreateCheckedFromSingleTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateChecked<float>(float.NegativeInfinity));
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateChecked<float>(float.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateChecked<float>(-1.0f));

            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<float>(-SingleTests_GenericMath.MinNormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<float>(-SingleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<float>(-float.Epsilon));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateChecked<float>(-0.0f));

            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<float>(+0.0f));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<float>(float.Epsilon));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<float>(SingleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<float>(SingleTests_GenericMath.MinNormal));

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<float>(1.0f));

            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateChecked<float>(float.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateChecked<float>(float.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateChecked<float>(float.NaN));
        }

        [Fact]
        public static void CreateCheckedFromUInt16Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<ushort>(0x0000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<ushort>(0x0001));
            AssertBitwiseEqual((Half)32767.0f, NumberBaseHelper<Half>.CreateChecked<ushort>(0x7FFF));
            AssertBitwiseEqual((Half)32768.0f, NumberBaseHelper<Half>.CreateChecked<ushort>(0x8000));
            AssertBitwiseEqual((Half)65535.0f, NumberBaseHelper<Half>.CreateChecked<ushort>(0xFFFF));
        }

        [Fact]
        public static void CreateCheckedFromUInt32Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<uint>(0x00000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<uint>(0x00000001));
            AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateChecked<uint>(0x7FFFFFFF));
            AssertBitwiseEqual((Half)2147483648.0f, NumberBaseHelper<Half>.CreateChecked<uint>(0x80000000));
            AssertBitwiseEqual((Half)4294967295.0f, NumberBaseHelper<Half>.CreateChecked<uint>(0xFFFFFFFF));
        }

        [Fact]
        public static void CreateCheckedFromUInt64Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<ulong>(0x0000000000000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<ulong>(0x0000000000000001));
            AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateChecked<ulong>(0x7FFFFFFFFFFFFFFF));
            AssertBitwiseEqual((Half)9223372036854775808.0f, NumberBaseHelper<Half>.CreateChecked<ulong>(0x8000000000000000));
            AssertBitwiseEqual((Half)18446744073709551615.0f, NumberBaseHelper<Half>.CreateChecked<ulong>(0xFFFFFFFFFFFFFFFF));
        }

        [Fact]
        public static void CreateCheckedFromUInt128Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<UInt128>(new UInt128(0x0000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<UInt128>(new UInt128(0x0000_0000_0000_0000, 0x0000_0000_0000_0001)));
            AssertBitwiseEqual((Half)170141183460469231731687303715884105727.0f, NumberBaseHelper<Half>.CreateChecked<UInt128>(new UInt128(0x7FFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
            AssertBitwiseEqual((Half)170141183460469231731687303715884105728.0f, NumberBaseHelper<Half>.CreateChecked<UInt128>(new UInt128(0x8000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateChecked<UInt128>(new UInt128(0xFFFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
        }

        [Fact]
        public static void CreateCheckedFromUIntPtrTest()
        {
            if (Environment.Is64BitProcess)
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<nuint>(unchecked((nuint)0x0000000000000000)));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<nuint>(unchecked((nuint)0x0000000000000001)));
                AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateChecked<nuint>(unchecked((nuint)0x7FFFFFFFFFFFFFFF)));

                // https://github.com/dotnet/roslyn/issues/60714
                // AssertBitwiseEqual((Half)9223372036854775808.0f, NumberBaseHelper<Half>.CreateChecked<nuint>(unchecked((nuint)0x8000000000000000)));
                // AssertBitwiseEqual((Half)18446744073709551615.0f,NumberBaseHelper<Half>.CreateChecked<nuint>(unchecked((nuint)0xFFFFFFFFFFFFFFFF)));
            }
            else
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateChecked<nuint>((nuint)0x00000000));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateChecked<nuint>((nuint)0x00000001));
                AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateChecked<nuint>((nuint)0x7FFFFFFF));

                // https://github.com/dotnet/roslyn/issues/60714
                // AssertBitwiseEqual((Half)2147483648.0f, NumberBaseHelper<Half>.CreateChecked<nuint>((nuint)0x80000000));
                // AssertBitwiseEqual((Half)4294967295.0f, NumberBaseHelper<Half>.CreateChecked<nuint>((nuint)0xFFFFFFFF));
            }
        }

        [Fact]
        public static void CreateSaturatingFromByteTest()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<byte>(0x00));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<byte>(0x01));
            AssertBitwiseEqual((Half)127.0f, NumberBaseHelper<Half>.CreateSaturating<byte>(0x7F));
            AssertBitwiseEqual((Half)128.0f, NumberBaseHelper<Half>.CreateSaturating<byte>(0x80));
            AssertBitwiseEqual((Half)255.0f, NumberBaseHelper<Half>.CreateSaturating<byte>(0xFF));
        }

        [Fact]
        public static void CreateSaturatingFromCharTest()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<char>((char)0x0000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<char>((char)0x0001));
            AssertBitwiseEqual((Half)32767.0f, NumberBaseHelper<Half>.CreateSaturating<char>((char)0x7FFF));
            AssertBitwiseEqual((Half)32768.0f, NumberBaseHelper<Half>.CreateSaturating<char>((char)0x8000));
            AssertBitwiseEqual((Half)65535.0f, NumberBaseHelper<Half>.CreateSaturating<char>((char)0xFFFF));
        }

        [Fact]
        public static void CreateSaturatingFromDecimalTest()
        {

            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateSaturating<decimal>(decimal.MinValue));
            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateSaturating<decimal>(-1.0m));
            AssertBitwiseEqual(Half.NegativeZero, NumberBaseHelper<Half>.CreateSaturating<decimal>(-0.0m));
            AssertBitwiseEqual(Half.Zero, NumberBaseHelper<Half>.CreateSaturating<decimal>(+0.0m));
            AssertBitwiseEqual(Half.One, NumberBaseHelper<Half>.CreateSaturating<decimal>(+1.0m));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateSaturating<decimal>(decimal.MaxValue));
        }

        [Fact]
        public static void CreateSaturatingFromDoubleTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateSaturating<double>(double.NegativeInfinity));
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateSaturating<double>(double.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateSaturating<double>(-1.0));

            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<double>(-DoubleTests_GenericMath.MinNormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<double>(-DoubleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<double>(-double.Epsilon));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<double>(-0.0));

            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<double>(+0.0));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<double>(double.Epsilon));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<double>(DoubleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<double>(DoubleTests_GenericMath.MinNormal));

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<double>(1.0));

            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateSaturating<double>(double.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateSaturating<double>(double.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateSaturating<double>(double.NaN));
        }

        [Fact]
        public static void CreateSaturatingFromHalfTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateSaturating<Half>(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.MinValue, NumberBaseHelper<Half>.CreateSaturating<Half>(Half.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateSaturating<Half>(Half.NegativeOne));

            AssertBitwiseEqual(-MinNormal, NumberBaseHelper<Half>.CreateSaturating<Half>(-MinNormal));
            AssertBitwiseEqual(-MaxSubnormal, NumberBaseHelper<Half>.CreateSaturating<Half>(-MaxSubnormal));
            AssertBitwiseEqual(-Half.Epsilon, NumberBaseHelper<Half>.CreateSaturating<Half>(-Half.Epsilon));
            AssertBitwiseEqual(Half.NegativeZero, NumberBaseHelper<Half>.CreateSaturating<Half>(Half.NegativeZero));

            AssertBitwiseEqual(Half.Zero, NumberBaseHelper<Half>.CreateSaturating<Half>(Half.Zero));
            AssertBitwiseEqual(Half.Epsilon, NumberBaseHelper<Half>.CreateSaturating<Half>(Half.Epsilon));
            AssertBitwiseEqual(MaxSubnormal, NumberBaseHelper<Half>.CreateSaturating<Half>(MaxSubnormal));
            AssertBitwiseEqual(MinNormal, NumberBaseHelper<Half>.CreateSaturating<Half>(MinNormal));

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<Half>(Half.One));

            AssertBitwiseEqual(Half.MaxValue, NumberBaseHelper<Half>.CreateSaturating<Half>(Half.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateSaturating<Half>(Half.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateSaturating<Half>(Half.NaN));
        }

        [Fact]
        public static void CreateSaturatingFromInt16Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<short>(0x0000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<short>(0x0001));
            AssertBitwiseEqual((Half)32767.0f, NumberBaseHelper<Half>.CreateSaturating<short>(0x7FFF));
            AssertBitwiseEqual((Half)(-32768.0f), NumberBaseHelper<Half>.CreateSaturating<short>(unchecked((short)0x8000)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateSaturating<short>(unchecked((short)0xFFFF)));
        }

        [Fact]
        public static void CreateSaturatingFromInt32Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<int>(0x00000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<int>(0x00000001));
            AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateSaturating<int>(0x7FFFFFFF));
            AssertBitwiseEqual((Half)(-2147483648.0f), NumberBaseHelper<Half>.CreateSaturating<int>(unchecked((int)0x80000000)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateSaturating<int>(unchecked((int)0xFFFFFFFF)));
        }

        [Fact]
        public static void CreateSaturatingFromInt64Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<long>(0x0000000000000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<long>(0x0000000000000001));
            AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateSaturating<long>(0x7FFFFFFFFFFFFFFF));
            AssertBitwiseEqual((Half)(-9223372036854775808.0f), NumberBaseHelper<Half>.CreateSaturating<long>(unchecked(unchecked((long)0x8000000000000000))));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateSaturating<long>(unchecked(unchecked((long)0xFFFFFFFFFFFFFFFF))));
        }

        [Fact]
        public static void CreateSaturatingFromInt128Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<Int128>(new Int128(0x0000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<Int128>(new Int128(0x0000_0000_0000_0000, 0x0000_0000_0000_0001)));
            AssertBitwiseEqual((Half)170141183460469231731687303715884105727.0f, NumberBaseHelper<Half>.CreateSaturating<Int128>(new Int128(0x7FFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
            AssertBitwiseEqual((Half)(-170141183460469231731687303715884105728.0f), NumberBaseHelper<Half>.CreateSaturating<Int128>(new Int128(0x8000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateSaturating<Int128>(new Int128(0xFFFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
        }

        [Fact]
        public static void CreateSaturatingFromIntPtrTest()
        {
            if (Environment.Is64BitProcess)
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<nint>(unchecked((nint)0x0000000000000000)));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<nint>(unchecked((nint)0x0000000000000001)));
                AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateSaturating<nint>(unchecked((nint)0x7FFFFFFFFFFFFFFF)));
                AssertBitwiseEqual((Half)(-9223372036854775808.0f), NumberBaseHelper<Half>.CreateSaturating<nint>(unchecked((nint)0x8000000000000000)));
                AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateSaturating<nint>(unchecked((nint)0xFFFFFFFFFFFFFFFF)));
            }
            else
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<nint>((nint)0x00000000));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<nint>((nint)0x00000001));
                AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateSaturating<nint>((nint)0x7FFFFFFF));
                AssertBitwiseEqual((Half)(-2147483648.0f), NumberBaseHelper<Half>.CreateSaturating<nint>(unchecked((nint)0x80000000)));
                AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateSaturating<nint>(unchecked((nint)0xFFFFFFFF)));
            }
        }

        [Fact]
        public static void CreateSaturatingFromNFloatTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateSaturating<NFloat>(NFloat.NegativeInfinity));
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateSaturating<NFloat>(NFloat.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateSaturating<NFloat>(-1.0f));

            if (Environment.Is64BitProcess)
            {
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<NFloat>((NFloat)(-DoubleTests_GenericMath.MinNormal)));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<NFloat>((NFloat)(-DoubleTests_GenericMath.MaxSubnormal)));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(-NFloat.Epsilon));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(-0.0f));

                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(+0.0f));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(NFloat.Epsilon));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<NFloat>((NFloat)DoubleTests_GenericMath.MaxSubnormal));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<NFloat>((NFloat)DoubleTests_GenericMath.MinNormal));
            }
            else
            {
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(-SingleTests_GenericMath.MinNormal));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(-SingleTests_GenericMath.MaxSubnormal));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(-NFloat.Epsilon));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(-0.0f));

                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(+0.0f));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(NFloat.Epsilon));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(SingleTests_GenericMath.MaxSubnormal));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<NFloat>(SingleTests_GenericMath.MinNormal));
            }

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<NFloat>(1.0f));

            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateSaturating<NFloat>(NFloat.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateSaturating<NFloat>(NFloat.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateSaturating<NFloat>(NFloat.NaN));
        }

        [Fact]
        public static void CreateSaturatingFromSByteTest()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<sbyte>(0x00));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<sbyte>(0x01));
            AssertBitwiseEqual((Half)127.0f, NumberBaseHelper<Half>.CreateSaturating<sbyte>(0x7F));
            AssertBitwiseEqual((Half)(-128.0f), NumberBaseHelper<Half>.CreateSaturating<sbyte>(unchecked((sbyte)0x80)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateSaturating<sbyte>(unchecked((sbyte)0xFF)));
        }

        [Fact]
        public static void CreateSaturatingFromSingleTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateSaturating<float>(float.NegativeInfinity));
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateSaturating<float>(float.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateSaturating<float>(-1.0f));

            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<float>(-SingleTests_GenericMath.MinNormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<float>(-SingleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<float>(-float.Epsilon));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateSaturating<float>(-0.0f));

            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<float>(+0.0f));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<float>(float.Epsilon));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<float>(SingleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<float>(SingleTests_GenericMath.MinNormal));

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<float>(1.0f));

            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateSaturating<float>(float.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateSaturating<float>(float.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateSaturating<float>(float.NaN));
        }

        [Fact]
        public static void CreateSaturatingFromUInt16Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<ushort>(0x0000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<ushort>(0x0001));
            AssertBitwiseEqual((Half)32767.0f, NumberBaseHelper<Half>.CreateSaturating<ushort>(0x7FFF));
            AssertBitwiseEqual((Half)32768.0f, NumberBaseHelper<Half>.CreateSaturating<ushort>(0x8000));
            AssertBitwiseEqual((Half)65535.0f, NumberBaseHelper<Half>.CreateSaturating<ushort>(0xFFFF));
        }

        [Fact]
        public static void CreateSaturatingFromUInt32Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<uint>(0x00000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<uint>(0x00000001));
            AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateSaturating<uint>(0x7FFFFFFF));
            AssertBitwiseEqual((Half)2147483648.0f, NumberBaseHelper<Half>.CreateSaturating<uint>(0x80000000));
            AssertBitwiseEqual((Half)4294967295.0f, NumberBaseHelper<Half>.CreateSaturating<uint>(0xFFFFFFFF));
        }

        [Fact]
        public static void CreateSaturatingFromUInt64Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<ulong>(0x0000000000000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<ulong>(0x0000000000000001));
            AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateSaturating<ulong>(0x7FFFFFFFFFFFFFFF));
            AssertBitwiseEqual((Half)9223372036854775808.0f, NumberBaseHelper<Half>.CreateSaturating<ulong>(0x8000000000000000));
            AssertBitwiseEqual((Half)18446744073709551615.0f, NumberBaseHelper<Half>.CreateSaturating<ulong>(0xFFFFFFFFFFFFFFFF));
        }

        [Fact]
        public static void CreateSaturatingFromUInt128Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<UInt128>(new UInt128(0x0000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<UInt128>(new UInt128(0x0000_0000_0000_0000, 0x0000_0000_0000_0001)));
            AssertBitwiseEqual((Half)170141183460469231731687303715884105727.0f, NumberBaseHelper<Half>.CreateSaturating<UInt128>(new UInt128(0x7FFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
            AssertBitwiseEqual((Half)170141183460469231731687303715884105728.0f, NumberBaseHelper<Half>.CreateSaturating<UInt128>(new UInt128(0x8000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateSaturating<UInt128>(new UInt128(0xFFFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
        }

        [Fact]
        public static void CreateSaturatingFromUIntPtrTest()
        {
            if (Environment.Is64BitProcess)
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<nuint>(unchecked((nuint)0x0000000000000000)));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<nuint>(unchecked((nuint)0x0000000000000001)));
                AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateSaturating<nuint>(unchecked((nuint)0x7FFFFFFFFFFFFFFF)));

                // https://github.com/dotnet/roslyn/issues/60714
                // AssertBitwiseEqual((Half)9223372036854775808.0f, NumberBaseHelper<Half>.CreateSaturating<nuint>(unchecked((nuint)0x8000000000000000)));
                // AssertBitwiseEqual((Half)18446744073709551615.0f, NumberBaseHelper<Half>.CreateSaturating<nuint>(unchecked((nuint)0xFFFFFFFFFFFFFFFF)));
            }
            else
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateSaturating<nuint>((nuint)0x00000000));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateSaturating<nuint>((nuint)0x00000001));
                AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateSaturating<nuint>((nuint)0x7FFFFFFF));

                // https://github.com/dotnet/roslyn/issues/60714
                // AssertBitwiseEqual((Half)2147483648.0f, NumberBaseHelper<Half>.CreateSaturating<nuint>((nuint)0x80000000));
                // AssertBitwiseEqual((Half)4294967295.0f, NumberBaseHelper<Half>.CreateSaturating<nuint>((nuint)0xFFFFFFFF));
            }
        }

        [Fact]
        public static void CreateTruncatingFromByteTest()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<byte>(0x00));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<byte>(0x01));
            AssertBitwiseEqual((Half)127.0f, NumberBaseHelper<Half>.CreateTruncating<byte>(0x7F));
            AssertBitwiseEqual((Half)128.0f, NumberBaseHelper<Half>.CreateTruncating<byte>(0x80));
            AssertBitwiseEqual((Half)255.0f, NumberBaseHelper<Half>.CreateTruncating<byte>(0xFF));
        }

        [Fact]
        public static void CreateTruncatingFromCharTest()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<char>((char)0x0000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<char>((char)0x0001));
            AssertBitwiseEqual((Half)32767.0f, NumberBaseHelper<Half>.CreateTruncating<char>((char)0x7FFF));
            AssertBitwiseEqual((Half)32768.0f, NumberBaseHelper<Half>.CreateTruncating<char>((char)0x8000));
            AssertBitwiseEqual((Half)65535.0f, NumberBaseHelper<Half>.CreateTruncating<char>((char)0xFFFF));
        }

        [Fact]
        public static void CreateTruncatingFromDecimalTest()
        {

            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateTruncating<decimal>(decimal.MinValue));
            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateTruncating<decimal>(-1.0m));
            AssertBitwiseEqual(Half.NegativeZero, NumberBaseHelper<Half>.CreateTruncating<decimal>(-0.0m));
            AssertBitwiseEqual(Half.Zero, NumberBaseHelper<Half>.CreateTruncating<decimal>(+0.0m));
            AssertBitwiseEqual(Half.One, NumberBaseHelper<Half>.CreateTruncating<decimal>(+1.0m));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateTruncating<decimal>(decimal.MaxValue));
        }

        [Fact]
        public static void CreateTruncatingFromDoubleTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateTruncating<double>(double.NegativeInfinity));
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateTruncating<double>(double.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateTruncating<double>(-1.0));

            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<double>(-DoubleTests_GenericMath.MinNormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<double>(-DoubleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<double>(-double.Epsilon));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<double>(-0.0));

            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<double>(+0.0));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<double>(double.Epsilon));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<double>(DoubleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<double>(DoubleTests_GenericMath.MinNormal));

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<double>(1.0));

            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateTruncating<double>(double.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateTruncating<double>(double.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateTruncating<double>(double.NaN));
        }

        [Fact]
        public static void CreateTruncatingFromHalfTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateTruncating<Half>(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.MinValue, NumberBaseHelper<Half>.CreateTruncating<Half>(Half.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateTruncating<Half>(Half.NegativeOne));

            AssertBitwiseEqual(-MinNormal, NumberBaseHelper<Half>.CreateTruncating<Half>(-MinNormal));
            AssertBitwiseEqual(-MaxSubnormal, NumberBaseHelper<Half>.CreateTruncating<Half>(-MaxSubnormal));
            AssertBitwiseEqual(-Half.Epsilon, NumberBaseHelper<Half>.CreateTruncating<Half>(-Half.Epsilon));
            AssertBitwiseEqual(Half.NegativeZero, NumberBaseHelper<Half>.CreateTruncating<Half>(Half.NegativeZero));

            AssertBitwiseEqual(Half.Zero, NumberBaseHelper<Half>.CreateTruncating<Half>(Half.Zero));
            AssertBitwiseEqual(Half.Epsilon, NumberBaseHelper<Half>.CreateTruncating<Half>(Half.Epsilon));
            AssertBitwiseEqual(MaxSubnormal, NumberBaseHelper<Half>.CreateTruncating<Half>(MaxSubnormal));
            AssertBitwiseEqual(MinNormal, NumberBaseHelper<Half>.CreateTruncating<Half>(MinNormal));

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<Half>(Half.One));

            AssertBitwiseEqual(Half.MaxValue, NumberBaseHelper<Half>.CreateTruncating<Half>(Half.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateTruncating<Half>(Half.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateTruncating<Half>(Half.NaN));
        }

        [Fact]
        public static void CreateTruncatingFromInt16Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<short>(0x0000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<short>(0x0001));
            AssertBitwiseEqual((Half)32767.0f, NumberBaseHelper<Half>.CreateTruncating<short>(0x7FFF));
            AssertBitwiseEqual((Half)(-32768.0f), NumberBaseHelper<Half>.CreateTruncating<short>(unchecked((short)0x8000)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateTruncating<short>(unchecked((short)0xFFFF)));
        }

        [Fact]
        public static void CreateTruncatingFromInt32Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<int>(0x00000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<int>(0x00000001));
            AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateTruncating<int>(0x7FFFFFFF));
            AssertBitwiseEqual((Half)(-2147483648.0f), NumberBaseHelper<Half>.CreateTruncating<int>(unchecked((int)0x80000000)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateTruncating<int>(unchecked((int)0xFFFFFFFF)));
        }

        [Fact]
        public static void CreateTruncatingFromInt64Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<long>(0x0000000000000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<long>(0x0000000000000001));
            AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateTruncating<long>(0x7FFFFFFFFFFFFFFF));
            AssertBitwiseEqual((Half)(-9223372036854775808.0f), NumberBaseHelper<Half>.CreateTruncating<long>(unchecked(unchecked((long)0x8000000000000000))));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateTruncating<long>(unchecked(unchecked((long)0xFFFFFFFFFFFFFFFF))));
        }

        [Fact]
        public static void CreateTruncatingFromInt128Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<Int128>(new Int128(0x0000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<Int128>(new Int128(0x0000_0000_0000_0000, 0x0000_0000_0000_0001)));
            AssertBitwiseEqual((Half)170141183460469231731687303715884105727.0f, NumberBaseHelper<Half>.CreateTruncating<Int128>(new Int128(0x7FFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
            AssertBitwiseEqual((Half)(-170141183460469231731687303715884105728.0f), NumberBaseHelper<Half>.CreateTruncating<Int128>(new Int128(0x8000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateTruncating<Int128>(new Int128(0xFFFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
        }

        [Fact]
        public static void CreateTruncatingFromIntPtrTest()
        {
            if (Environment.Is64BitProcess)
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<nint>(unchecked((nint)0x0000000000000000)));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<nint>(unchecked((nint)0x0000000000000001)));
                AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateTruncating<nint>(unchecked((nint)0x7FFFFFFFFFFFFFFF)));
                AssertBitwiseEqual((Half)(-9223372036854775808.0f), NumberBaseHelper<Half>.CreateTruncating<nint>(unchecked((nint)0x8000000000000000)));
                AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateTruncating<nint>(unchecked((nint)0xFFFFFFFFFFFFFFFF)));
            }
            else
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<nint>((nint)0x00000000));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<nint>((nint)0x00000001));
                AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateTruncating<nint>((nint)0x7FFFFFFF));
                AssertBitwiseEqual((Half)(-2147483648.0f), NumberBaseHelper<Half>.CreateTruncating<nint>(unchecked((nint)0x80000000)));
                AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateTruncating<nint>(unchecked((nint)0xFFFFFFFF)));
            }
        }

        [Fact]
        public static void CreateTruncatingFromNFloatTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateTruncating<NFloat>(NFloat.NegativeInfinity));
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateTruncating<NFloat>(NFloat.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateTruncating<NFloat>(-1.0f));

            if (Environment.Is64BitProcess)
            {
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<NFloat>((NFloat)(-DoubleTests_GenericMath.MinNormal)));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<NFloat>((NFloat)(-DoubleTests_GenericMath.MaxSubnormal)));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(-NFloat.Epsilon));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(-0.0f));

                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(+0.0f));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(NFloat.Epsilon));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<NFloat>((NFloat)DoubleTests_GenericMath.MaxSubnormal));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<NFloat>((NFloat)DoubleTests_GenericMath.MinNormal));
            }
            else
            {
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(-SingleTests_GenericMath.MinNormal));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(-SingleTests_GenericMath.MaxSubnormal));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(-NFloat.Epsilon));
                AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(-0.0f));

                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(+0.0f));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(NFloat.Epsilon));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(SingleTests_GenericMath.MaxSubnormal));
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<NFloat>(SingleTests_GenericMath.MinNormal));
            }

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<NFloat>(1.0f));

            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateTruncating<NFloat>(NFloat.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateTruncating<NFloat>(NFloat.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateTruncating<NFloat>(NFloat.NaN));
        }

        [Fact]
        public static void CreateTruncatingFromSByteTest()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<sbyte>(0x00));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<sbyte>(0x01));
            AssertBitwiseEqual((Half)127.0f, NumberBaseHelper<Half>.CreateTruncating<sbyte>(0x7F));
            AssertBitwiseEqual((Half)(-128.0f), NumberBaseHelper<Half>.CreateTruncating<sbyte>(unchecked((sbyte)0x80)));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.CreateTruncating<sbyte>(unchecked((sbyte)0xFF)));
        }

        [Fact]
        public static void CreateTruncatingFromSingleTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateTruncating<float>(float.NegativeInfinity));
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.CreateTruncating<float>(float.MinValue));

            AssertBitwiseEqual(Half.NegativeOne, NumberBaseHelper<Half>.CreateTruncating<float>(-1.0f));

            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<float>(-SingleTests_GenericMath.MinNormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<float>(-SingleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<float>(-float.Epsilon));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.CreateTruncating<float>(-0.0f));

            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<float>(+0.0f));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<float>(float.Epsilon));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<float>(SingleTests_GenericMath.MaxSubnormal));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<float>(SingleTests_GenericMath.MinNormal));

            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<float>(1.0f));

            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateTruncating<float>(float.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateTruncating<float>(float.PositiveInfinity));

            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.CreateTruncating<float>(float.NaN));
        }

        [Fact]
        public static void CreateTruncatingFromUInt16Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<ushort>(0x0000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<ushort>(0x0001));
            AssertBitwiseEqual((Half)32767.0f, NumberBaseHelper<Half>.CreateTruncating<ushort>(0x7FFF));
            AssertBitwiseEqual((Half)32768.0f, NumberBaseHelper<Half>.CreateTruncating<ushort>(0x8000));
            AssertBitwiseEqual((Half)65535.0f, NumberBaseHelper<Half>.CreateTruncating<ushort>(0xFFFF));
        }

        [Fact]
        public static void CreateTruncatingFromUInt32Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<uint>(0x00000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<uint>(0x00000001));
            AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateTruncating<uint>(0x7FFFFFFF));
            AssertBitwiseEqual((Half)2147483648.0f, NumberBaseHelper<Half>.CreateTruncating<uint>(0x80000000));
            AssertBitwiseEqual((Half)4294967295.0f, NumberBaseHelper<Half>.CreateTruncating<uint>(0xFFFFFFFF));
        }

        [Fact]
        public static void CreateTruncatingFromUInt64Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<ulong>(0x0000000000000000));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<ulong>(0x0000000000000001));
            AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateTruncating<ulong>(0x7FFFFFFFFFFFFFFF));
            AssertBitwiseEqual((Half)9223372036854775808.0f, NumberBaseHelper<Half>.CreateTruncating<ulong>(0x8000000000000000));
            AssertBitwiseEqual((Half)18446744073709551615.0f, NumberBaseHelper<Half>.CreateTruncating<ulong>(0xFFFFFFFFFFFFFFFF));
        }

        [Fact]
        public static void CreateTruncatingFromUInt128Test()
        {
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<UInt128>(new UInt128(0x0000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<UInt128>(new UInt128(0x0000_0000_0000_0000, 0x0000_0000_0000_0001)));
            AssertBitwiseEqual((Half)170141183460469231731687303715884105727.0f, NumberBaseHelper<Half>.CreateTruncating<UInt128>(new UInt128(0x7FFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
            AssertBitwiseEqual((Half)170141183460469231731687303715884105728.0f, NumberBaseHelper<Half>.CreateTruncating<UInt128>(new UInt128(0x8000_0000_0000_0000, 0x0000_0000_0000_0000)));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.CreateTruncating<UInt128>(new UInt128(0xFFFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)));
        }

        [Fact]
        public static void CreateTruncatingFromUIntPtrTest()
        {
            if (Environment.Is64BitProcess)
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<nuint>(unchecked((nuint)0x0000000000000000)));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<nuint>(unchecked((nuint)0x0000000000000001)));
                AssertBitwiseEqual((Half)9223372036854775807.0f, NumberBaseHelper<Half>.CreateTruncating<nuint>(unchecked((nuint)0x7FFFFFFFFFFFFFFF)));

                // https://github.com/dotnet/roslyn/issues/60714
                // AssertBitwiseEqual((Half)9223372036854775808.0f, NumberBaseHelper<Half>.CreateTruncating<nuint>(unchecked((nuint)0x8000000000000000)));
                // AssertBitwiseEqual((Half)18446744073709551615.0f, NumberBaseHelper<Half>.CreateTruncating<nuint>(unchecked((nuint)0xFFFFFFFFFFFFFFFF)));
            }
            else
            {
                AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.CreateTruncating<nuint>((nuint)0x00000000));
                AssertBitwiseEqual(One, NumberBaseHelper<Half>.CreateTruncating<nuint>((nuint)0x00000001));
                AssertBitwiseEqual((Half)2147483647.0f, NumberBaseHelper<Half>.CreateTruncating<nuint>((nuint)0x7FFFFFFF));

                // https://github.com/dotnet/roslyn/issues/60714
                // AssertBitwiseEqual((Half)2147483648.0f, NumberBaseHelper<Half>.CreateTruncating<nuint>((nuint)0x80000000));
                // AssertBitwiseEqual((Half)4294967295.0f, NumberBaseHelper<Half>.CreateTruncating<nuint>((nuint)0xFFFFFFFF));
            }
        }

        [Fact]
        public static void IsCanonicalTest()
        {
            Assert.True(NumberBaseHelper<Half>.IsCanonical(Half.NegativeInfinity));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(Half.MinValue));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(NegativeOne));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(-MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(-MaxSubnormal));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(-Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(NegativeZero));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(Half.NaN));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(Zero));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(MaxSubnormal));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(One));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(Half.MaxValue));
            Assert.True(NumberBaseHelper<Half>.IsCanonical(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsComplexNumberTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(Half.NegativeInfinity));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(Half.MinValue));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(-Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(Half.NaN));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(Zero));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(One));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsComplexNumber(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsEvenIntegerTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsEvenInteger(Half.NegativeInfinity));
            Assert.True(NumberBaseHelper<Half>.IsEvenInteger(Half.MinValue));
            Assert.False(NumberBaseHelper<Half>.IsEvenInteger(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsEvenInteger(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsEvenInteger(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsEvenInteger(-Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsEvenInteger(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsEvenInteger(Half.NaN));
            Assert.True(NumberBaseHelper<Half>.IsEvenInteger(Zero));
            Assert.False(NumberBaseHelper<Half>.IsEvenInteger(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsEvenInteger(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsEvenInteger(MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsEvenInteger(One));
            Assert.True(NumberBaseHelper<Half>.IsEvenInteger(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsEvenInteger(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsFiniteTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsFinite(Half.NegativeInfinity));
            Assert.True(NumberBaseHelper<Half>.IsFinite(Half.MinValue));
            Assert.True(NumberBaseHelper<Half>.IsFinite(NegativeOne));
            Assert.True(NumberBaseHelper<Half>.IsFinite(-MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsFinite(-MaxSubnormal));
            Assert.True(NumberBaseHelper<Half>.IsFinite(-Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsFinite(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsFinite(Half.NaN));
            Assert.True(NumberBaseHelper<Half>.IsFinite(Zero));
            Assert.True(NumberBaseHelper<Half>.IsFinite(Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsFinite(MaxSubnormal));
            Assert.True(NumberBaseHelper<Half>.IsFinite(MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsFinite(One));
            Assert.True(NumberBaseHelper<Half>.IsFinite(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsFinite(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsImaginaryNumberTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(Half.NegativeInfinity));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(Half.MinValue));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(-Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(Half.NaN));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(Zero));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(One));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsImaginaryNumber(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsInfinityTest()
        {
            Assert.True(NumberBaseHelper<Half>.IsInfinity(Half.NegativeInfinity));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(Half.MinValue));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(-Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(Half.NaN));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(Zero));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(One));
            Assert.False(NumberBaseHelper<Half>.IsInfinity(Half.MaxValue));
            Assert.True(NumberBaseHelper<Half>.IsInfinity(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsIntegerTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsInteger(Half.NegativeInfinity));
            Assert.True(NumberBaseHelper<Half>.IsInteger(Half.MinValue));
            Assert.True(NumberBaseHelper<Half>.IsInteger(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsInteger(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsInteger(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsInteger(-Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsInteger(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsInteger(Half.NaN));
            Assert.True(NumberBaseHelper<Half>.IsInteger(Zero));
            Assert.False(NumberBaseHelper<Half>.IsInteger(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsInteger(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsInteger(MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsInteger(One));
            Assert.True(NumberBaseHelper<Half>.IsInteger(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsInteger(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsNaNTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsNaN(Half.NegativeInfinity));
            Assert.False(NumberBaseHelper<Half>.IsNaN(Half.MinValue));
            Assert.False(NumberBaseHelper<Half>.IsNaN(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsNaN(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsNaN(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsNaN(-Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsNaN(NegativeZero));
            Assert.True(NumberBaseHelper<Half>.IsNaN(Half.NaN));
            Assert.False(NumberBaseHelper<Half>.IsNaN(Zero));
            Assert.False(NumberBaseHelper<Half>.IsNaN(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsNaN(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsNaN(MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsNaN(One));
            Assert.False(NumberBaseHelper<Half>.IsNaN(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsNaN(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsNegativeTest()
        {
            Assert.True(NumberBaseHelper<Half>.IsNegative(Half.NegativeInfinity));
            Assert.True(NumberBaseHelper<Half>.IsNegative(Half.MinValue));
            Assert.True(NumberBaseHelper<Half>.IsNegative(NegativeOne));
            Assert.True(NumberBaseHelper<Half>.IsNegative(-MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsNegative(-MaxSubnormal));
            Assert.True(NumberBaseHelper<Half>.IsNegative(-Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsNegative(NegativeZero));
            Assert.True(NumberBaseHelper<Half>.IsNegative(Half.NaN));
            Assert.False(NumberBaseHelper<Half>.IsNegative(Zero));
            Assert.False(NumberBaseHelper<Half>.IsNegative(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsNegative(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsNegative(MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsNegative(One));
            Assert.False(NumberBaseHelper<Half>.IsNegative(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsNegative(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsNegativeInfinityTest()
        {
            Assert.True(NumberBaseHelper<Half>.IsNegativeInfinity(Half.NegativeInfinity));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(Half.MinValue));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(-Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(Half.NaN));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(Zero));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(One));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsNegativeInfinity(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsNormalTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsNormal(Half.NegativeInfinity));
            Assert.True(NumberBaseHelper<Half>.IsNormal(Half.MinValue));
            Assert.True(NumberBaseHelper<Half>.IsNormal(NegativeOne));
            Assert.True(NumberBaseHelper<Half>.IsNormal(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsNormal(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsNormal(-Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsNormal(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsNormal(Half.NaN));
            Assert.False(NumberBaseHelper<Half>.IsNormal(Zero));
            Assert.False(NumberBaseHelper<Half>.IsNormal(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsNormal(MaxSubnormal));
            Assert.True(NumberBaseHelper<Half>.IsNormal(MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsNormal(One));
            Assert.True(NumberBaseHelper<Half>.IsNormal(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsNormal(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsOddIntegerTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(Half.NegativeInfinity));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(Half.MinValue));
            Assert.True(NumberBaseHelper<Half>.IsOddInteger(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(-Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(Half.NaN));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(Zero));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsOddInteger(One));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsOddInteger(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsPositiveTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsPositive(Half.NegativeInfinity));
            Assert.False(NumberBaseHelper<Half>.IsPositive(Half.MinValue));
            Assert.False(NumberBaseHelper<Half>.IsPositive(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsPositive(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsPositive(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsPositive(-Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsPositive(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsPositive(Half.NaN));
            Assert.True(NumberBaseHelper<Half>.IsPositive(Zero));
            Assert.True(NumberBaseHelper<Half>.IsPositive(Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsPositive(MaxSubnormal));
            Assert.True(NumberBaseHelper<Half>.IsPositive(MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsPositive(One));
            Assert.True(NumberBaseHelper<Half>.IsPositive(Half.MaxValue));
            Assert.True(NumberBaseHelper<Half>.IsPositive(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsPositiveInfinityTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(Half.NegativeInfinity));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(Half.MinValue));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(-Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(Half.NaN));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(Zero));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(One));
            Assert.False(NumberBaseHelper<Half>.IsPositiveInfinity(Half.MaxValue));
            Assert.True(NumberBaseHelper<Half>.IsPositiveInfinity(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsRealNumberTest()
        {
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(Half.NegativeInfinity));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(Half.MinValue));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(NegativeOne));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(-MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(-MaxSubnormal));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(-Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsRealNumber(Half.NaN));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(Zero));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(MaxSubnormal));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(One));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(Half.MaxValue));
            Assert.True(NumberBaseHelper<Half>.IsRealNumber(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsSubnormalTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsSubnormal(Half.NegativeInfinity));
            Assert.False(NumberBaseHelper<Half>.IsSubnormal(Half.MinValue));
            Assert.False(NumberBaseHelper<Half>.IsSubnormal(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsSubnormal(-MinNormal));
            Assert.True(NumberBaseHelper<Half>.IsSubnormal(-MaxSubnormal));
            Assert.True(NumberBaseHelper<Half>.IsSubnormal(-Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsSubnormal(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsSubnormal(Half.NaN));
            Assert.False(NumberBaseHelper<Half>.IsSubnormal(Zero));
            Assert.True(NumberBaseHelper<Half>.IsSubnormal(Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsSubnormal(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsSubnormal(MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsSubnormal(One));
            Assert.False(NumberBaseHelper<Half>.IsSubnormal(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsSubnormal(Half.PositiveInfinity));
        }

        [Fact]
        public static void IsZeroTest()
        {
            Assert.False(NumberBaseHelper<Half>.IsZero(Half.NegativeInfinity));
            Assert.False(NumberBaseHelper<Half>.IsZero(Half.MinValue));
            Assert.False(NumberBaseHelper<Half>.IsZero(NegativeOne));
            Assert.False(NumberBaseHelper<Half>.IsZero(-MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsZero(-MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsZero(-Half.Epsilon));
            Assert.True(NumberBaseHelper<Half>.IsZero(NegativeZero));
            Assert.False(NumberBaseHelper<Half>.IsZero(Half.NaN));
            Assert.True(NumberBaseHelper<Half>.IsZero(Zero));
            Assert.False(NumberBaseHelper<Half>.IsZero(Half.Epsilon));
            Assert.False(NumberBaseHelper<Half>.IsZero(MaxSubnormal));
            Assert.False(NumberBaseHelper<Half>.IsZero(MinNormal));
            Assert.False(NumberBaseHelper<Half>.IsZero(One));
            Assert.False(NumberBaseHelper<Half>.IsZero(Half.MaxValue));
            Assert.False(NumberBaseHelper<Half>.IsZero(Half.PositiveInfinity));
        }

        [Fact]
        public static void MaxMagnitudeTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.MaxMagnitude(Half.NegativeInfinity, One));
            AssertBitwiseEqual(Half.MinValue, NumberBaseHelper<Half>.MaxMagnitude(Half.MinValue, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitude(NegativeOne, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitude(-MinNormal, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitude(-MaxSubnormal, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitude(-Half.Epsilon, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitude(NegativeZero, One));
            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.MaxMagnitude(Half.NaN, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitude(Zero, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitude(Half.Epsilon, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitude(MaxSubnormal, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitude(MinNormal, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitude(One, One));
            AssertBitwiseEqual(Half.MaxValue, NumberBaseHelper<Half>.MaxMagnitude(Half.MaxValue, One));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.MaxMagnitude(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void MaxMagnitudeNumberTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, NumberBaseHelper<Half>.MaxMagnitudeNumber(Half.NegativeInfinity, One));
            AssertBitwiseEqual(Half.MinValue, NumberBaseHelper<Half>.MaxMagnitudeNumber(Half.MinValue, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitudeNumber(NegativeOne, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitudeNumber(-MinNormal, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitudeNumber(-MaxSubnormal, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitudeNumber(-Half.Epsilon, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitudeNumber(NegativeZero, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitudeNumber(Half.NaN, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitudeNumber(Zero, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitudeNumber(Half.Epsilon, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitudeNumber(MaxSubnormal, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitudeNumber(MinNormal, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MaxMagnitudeNumber(One, One));
            AssertBitwiseEqual(Half.MaxValue, NumberBaseHelper<Half>.MaxMagnitudeNumber(Half.MaxValue, One));
            AssertBitwiseEqual(Half.PositiveInfinity, NumberBaseHelper<Half>.MaxMagnitudeNumber(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void MinMagnitudeTest()
        {
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MinMagnitude(Half.NegativeInfinity, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MinMagnitude(Half.MinValue, One));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.MinMagnitude(NegativeOne, One));
            AssertBitwiseEqual(-MinNormal, NumberBaseHelper<Half>.MinMagnitude(-MinNormal, One));
            AssertBitwiseEqual(-MaxSubnormal, NumberBaseHelper<Half>.MinMagnitude(-MaxSubnormal, One));
            AssertBitwiseEqual(-Half.Epsilon, NumberBaseHelper<Half>.MinMagnitude(-Half.Epsilon, One));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.MinMagnitude(NegativeZero, One));
            AssertBitwiseEqual(Half.NaN, NumberBaseHelper<Half>.MinMagnitude(Half.NaN, One));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.MinMagnitude(Zero, One));
            AssertBitwiseEqual(Half.Epsilon, NumberBaseHelper<Half>.MinMagnitude(Half.Epsilon, One));
            AssertBitwiseEqual(MaxSubnormal, NumberBaseHelper<Half>.MinMagnitude(MaxSubnormal, One));
            AssertBitwiseEqual(MinNormal, NumberBaseHelper<Half>.MinMagnitude(MinNormal, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MinMagnitude(One, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MinMagnitude(Half.MaxValue, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MinMagnitude(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void MinMagnitudeNumberTest()
        {
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MinMagnitudeNumber(Half.NegativeInfinity, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MinMagnitudeNumber(Half.MinValue, One));
            AssertBitwiseEqual(NegativeOne, NumberBaseHelper<Half>.MinMagnitudeNumber(NegativeOne, One));
            AssertBitwiseEqual(-MinNormal, NumberBaseHelper<Half>.MinMagnitudeNumber(-MinNormal, One));
            AssertBitwiseEqual(-MaxSubnormal, NumberBaseHelper<Half>.MinMagnitudeNumber(-MaxSubnormal, One));
            AssertBitwiseEqual(-Half.Epsilon, NumberBaseHelper<Half>.MinMagnitudeNumber(-Half.Epsilon, One));
            AssertBitwiseEqual(NegativeZero, NumberBaseHelper<Half>.MinMagnitudeNumber(NegativeZero, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MinMagnitudeNumber(Half.NaN, One));
            AssertBitwiseEqual(Zero, NumberBaseHelper<Half>.MinMagnitudeNumber(Zero, One));
            AssertBitwiseEqual(Half.Epsilon, NumberBaseHelper<Half>.MinMagnitudeNumber(Half.Epsilon, One));
            AssertBitwiseEqual(MaxSubnormal, NumberBaseHelper<Half>.MinMagnitudeNumber(MaxSubnormal, One));
            AssertBitwiseEqual(MinNormal, NumberBaseHelper<Half>.MinMagnitudeNumber(MinNormal, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MinMagnitudeNumber(One, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MinMagnitudeNumber(Half.MaxValue, One));
            AssertBitwiseEqual(One, NumberBaseHelper<Half>.MinMagnitudeNumber(Half.PositiveInfinity, One));
        }

        //
        // ISignedNumber
        //

        [Fact]
        public static void NegativeOneTest()
        {
            Assert.Equal(NegativeOne, SignedNumberHelper<Half>.NegativeOne);
        }

        //
        // ISubtractionOperators
        //

        [Fact]
        public static void op_SubtractionTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(Half.NegativeInfinity, One));
            AssertBitwiseEqual(Half.MinValue, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(Half.MinValue, One));
            AssertBitwiseEqual(NegativeTwo, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(NegativeOne, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(-MinNormal, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(-MaxSubnormal, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(-Half.Epsilon, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(NegativeZero, One));
            AssertBitwiseEqual(Half.NaN, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(Half.NaN, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(Zero, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(Half.Epsilon, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(MaxSubnormal, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(MinNormal, One));
            AssertBitwiseEqual(Zero, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(One, One));
            AssertBitwiseEqual(Half.MaxValue, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(Half.MaxValue, One));
            AssertBitwiseEqual(Half.PositiveInfinity, SubtractionOperatorsHelper<Half, Half, Half>.op_Subtraction(Half.PositiveInfinity, One));
        }

        [Fact]
        public static void op_CheckedSubtractionTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(Half.NegativeInfinity, One));
            AssertBitwiseEqual(Half.MinValue, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(Half.MinValue, One));
            AssertBitwiseEqual(NegativeTwo, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(NegativeOne, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(-MinNormal, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(-MaxSubnormal, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(-Half.Epsilon, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(NegativeZero, One));
            AssertBitwiseEqual(Half.NaN, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(Half.NaN, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(Zero, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(Half.Epsilon, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(MaxSubnormal, One));
            AssertBitwiseEqual(NegativeOne, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(MinNormal, One));
            AssertBitwiseEqual(Zero, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(One, One));
            AssertBitwiseEqual(Half.MaxValue, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(Half.MaxValue, One));
            AssertBitwiseEqual(Half.PositiveInfinity, SubtractionOperatorsHelper<Half, Half, Half>.op_CheckedSubtraction(Half.PositiveInfinity, One));
        }

        //
        // IUnaryNegationOperators
        //

        [Fact]
        public static void op_UnaryNegationTest()
        {
            AssertBitwiseEqual(Half.PositiveInfinity, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.MaxValue, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(Half.MinValue));
            AssertBitwiseEqual(One, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(NegativeOne));
            AssertBitwiseEqual(MinNormal, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(-MinNormal));
            AssertBitwiseEqual(MaxSubnormal, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(-MaxSubnormal));
            AssertBitwiseEqual(Half.Epsilon, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(-Half.Epsilon));
            AssertBitwiseEqual(Zero, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(NegativeZero));
            AssertBitwiseEqual(Half.NaN, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(Half.NaN));
            AssertBitwiseEqual(NegativeZero, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(Zero));
            AssertBitwiseEqual(-Half.Epsilon, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(Half.Epsilon));
            AssertBitwiseEqual(-MaxSubnormal, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(MaxSubnormal));
            AssertBitwiseEqual(-MinNormal, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(MinNormal));
            AssertBitwiseEqual(NegativeOne, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(One));
            AssertBitwiseEqual(Half.MinValue, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(Half.MaxValue));
            AssertBitwiseEqual(Half.NegativeInfinity, UnaryNegationOperatorsHelper<Half, Half>.op_UnaryNegation(Half.PositiveInfinity));
        }

        [Fact]
        public static void op_CheckedUnaryNegationTest()
        {
            AssertBitwiseEqual(Half.PositiveInfinity, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.MaxValue, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(Half.MinValue));
            AssertBitwiseEqual(One, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(NegativeOne));
            AssertBitwiseEqual(MinNormal, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(-MinNormal));
            AssertBitwiseEqual(MaxSubnormal, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(-MaxSubnormal));
            AssertBitwiseEqual(Half.Epsilon, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(-Half.Epsilon));
            AssertBitwiseEqual(Zero, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(NegativeZero));
            AssertBitwiseEqual(Half.NaN, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(Half.NaN));
            AssertBitwiseEqual(NegativeZero, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(Zero));
            AssertBitwiseEqual(-Half.Epsilon, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(Half.Epsilon));
            AssertBitwiseEqual(-MaxSubnormal, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(MaxSubnormal));
            AssertBitwiseEqual(-MinNormal, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(MinNormal));
            AssertBitwiseEqual(NegativeOne, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(One));
            AssertBitwiseEqual(Half.MinValue, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(Half.MaxValue));
            AssertBitwiseEqual(Half.NegativeInfinity, UnaryNegationOperatorsHelper<Half, Half>.op_CheckedUnaryNegation(Half.PositiveInfinity));
        }

        //
        // IUnaryPlusOperators
        //

        [Fact]
        public static void op_UnaryPlusTest()
        {
            AssertBitwiseEqual(Half.NegativeInfinity, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(Half.NegativeInfinity));
            AssertBitwiseEqual(Half.MinValue, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(Half.MinValue));
            AssertBitwiseEqual(NegativeOne, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(NegativeOne));
            AssertBitwiseEqual(-MinNormal, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(-MinNormal));
            AssertBitwiseEqual(-MaxSubnormal, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(-MaxSubnormal));
            AssertBitwiseEqual(-Half.Epsilon, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(-Half.Epsilon));
            AssertBitwiseEqual(NegativeZero, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(NegativeZero));
            AssertBitwiseEqual(Half.NaN, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(Half.NaN));
            AssertBitwiseEqual(Zero, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(Zero));
            AssertBitwiseEqual(Half.Epsilon, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(Half.Epsilon));
            AssertBitwiseEqual(MaxSubnormal, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(MaxSubnormal));
            AssertBitwiseEqual(MinNormal, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(MinNormal));
            AssertBitwiseEqual(One, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(One));
            AssertBitwiseEqual(Half.MaxValue, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(Half.MaxValue));
            AssertBitwiseEqual(Half.PositiveInfinity, UnaryPlusOperatorsHelper<Half, Half>.op_UnaryPlus(Half.PositiveInfinity));
        }

        //
        // IParsable and ISpanParsable
        //

        [Theory]
        [MemberData(nameof(HalfTests.Parse_Valid_TestData), MemberType = typeof(HalfTests))]
        public static void ParseValidStringTest(string value, NumberStyles style, IFormatProvider provider, Half expected)
        {
            bool isDefaultProvider = provider == null || provider == NumberFormatInfo.CurrentInfo;
            Half result;
            if ((style & ~(NumberStyles.Float | NumberStyles.AllowThousands)) == 0 && style != NumberStyles.None)
            {
                // Use Parse(string) or Parse(string, IFormatProvider)
                if (isDefaultProvider)
                {
                    Assert.True(ParsableHelper<Half>.TryParse(value, null, out result));
                    Assert.Equal(expected, result);

                    Assert.Equal(expected, ParsableHelper<Half>.Parse(value, null));
                }

                Assert.Equal(expected, ParsableHelper<Half>.Parse(value, provider));
            }

            // Use Parse(string, NumberStyles, IFormatProvider)
            Assert.True(NumberBaseHelper<Half>.TryParse(value, style, provider, out result));
            Assert.Equal(expected, result);

            Assert.Equal(expected, NumberBaseHelper<Half>.Parse(value, style, provider));

            if (isDefaultProvider)
            {
                // Use Parse(string, NumberStyles) or Parse(string, NumberStyles, IFormatProvider)
                Assert.True(NumberBaseHelper<Half>.TryParse(value, style, NumberFormatInfo.CurrentInfo, out result));
                Assert.Equal(expected, result);

                Assert.Equal(expected, NumberBaseHelper<Half>.Parse(value, style, null));
                Assert.Equal(expected, NumberBaseHelper<Half>.Parse(value, style, NumberFormatInfo.CurrentInfo));
            }
        }

        [Theory]
        [MemberData(nameof(HalfTests.Parse_Invalid_TestData), MemberType = typeof(HalfTests))]
        public static void ParseInvalidStringTest(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            bool isDefaultProvider = provider == null || provider == NumberFormatInfo.CurrentInfo;
            Half result;
            if ((style & ~(NumberStyles.Float | NumberStyles.AllowThousands)) == 0 && style != NumberStyles.None && (style & NumberStyles.AllowLeadingWhite) == (style & NumberStyles.AllowTrailingWhite))
            {
                // Use Parse(string) or Parse(string, IFormatProvider)
                if (isDefaultProvider)
                {
                    Assert.False(ParsableHelper<Half>.TryParse(value, null, out result));
                    Assert.Equal(default(Half), result);

                    Assert.Throws(exceptionType, () => ParsableHelper<Half>.Parse(value, null));
                }

                Assert.Throws(exceptionType, () => ParsableHelper<Half>.Parse(value, provider));
            }

            // Use Parse(string, NumberStyles, IFormatProvider)
            Assert.False(NumberBaseHelper<Half>.TryParse(value, style, provider, out result));
            Assert.Equal(default(Half), result);

            Assert.Throws(exceptionType, () => NumberBaseHelper<Half>.Parse(value, style, provider));

            if (isDefaultProvider)
            {
                // Use Parse(string, NumberStyles) or Parse(string, NumberStyles, IFormatProvider)
                Assert.False(NumberBaseHelper<Half>.TryParse(value, style, NumberFormatInfo.CurrentInfo, out result));
                Assert.Equal(default(Half), result);

                Assert.Throws(exceptionType, () => NumberBaseHelper<Half>.Parse(value, style, null));
                Assert.Throws(exceptionType, () => NumberBaseHelper<Half>.Parse(value, style, NumberFormatInfo.CurrentInfo));
            }
        }

        [Theory]
        [MemberData(nameof(HalfTests.Parse_ValidWithOffsetCount_TestData), MemberType = typeof(HalfTests))]
        public static void ParseValidSpanTest(string value, int offset, int count, NumberStyles style, IFormatProvider provider, Half expected)
        {
            bool isDefaultProvider = provider == null || provider == NumberFormatInfo.CurrentInfo;
            Half result;
            if ((style & ~(NumberStyles.Float | NumberStyles.AllowThousands)) == 0 && style != NumberStyles.None)
            {
                // Use Parse(string) or Parse(string, IFormatProvider)
                if (isDefaultProvider)
                {
                    Assert.True(SpanParsableHelper<Half>.TryParse(value.AsSpan(offset, count), null, out result));
                    Assert.Equal(expected, result);

                    Assert.Equal(expected, SpanParsableHelper<Half>.Parse(value.AsSpan(offset, count), null));
                }

                Assert.Equal(expected, SpanParsableHelper<Half>.Parse(value.AsSpan(offset, count), provider: provider));
            }

            Assert.Equal(expected, NumberBaseHelper<Half>.Parse(value.AsSpan(offset, count), style, provider));

            Assert.True(NumberBaseHelper<Half>.TryParse(value.AsSpan(offset, count), style, provider, out result));
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(HalfTests.Parse_Invalid_TestData), MemberType = typeof(HalfTests))]
        public static void ParseInvalidSpanTest(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            if (value != null)
            {
                Assert.Throws(exceptionType, () => NumberBaseHelper<Half>.Parse(value.AsSpan(), style, provider));

                Assert.False(NumberBaseHelper<Half>.TryParse(value.AsSpan(), style, provider, out Half result));
                Assert.Equal((Half)0, result);
            }
        }
    }
}
