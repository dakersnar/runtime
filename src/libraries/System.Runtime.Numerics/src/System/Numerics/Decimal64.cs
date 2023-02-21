// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Numerics
{
    /// <summary>
    /// An IEEE 754 compliant decimal64 type.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Decimal64
        : IComparable<Decimal64>,
          IComparable,
          ISpanFormattable,
          ISpanParsable<Decimal64>,
          IEquatable<Decimal64>,
          IFloatingPoint<Decimal64>,
          /*IDecimalFloatingPointIeee754<Decimal64>,*/ //PLATINUM
          IMinMaxValue<Decimal64>
    {

        private const NumberStyles DefaultParseStyle = NumberStyles.Float | NumberStyles.AllowThousands;

        //
        // Constants for manipulating the private bit-representation
        //

        internal const ulong SignMask = 0x8000_0000_0000_0000;
        internal const int SignShift = 63;

        internal const ulong CombinationMask = 0x7FFC_0000_0000_0000;
        internal const int CombinationShift = 50;
        internal const int CombinationWidth = 13;
        internal const ushort ShiftedCombinationMask = (ushort)(CombinationMask >> CombinationShift);

        internal const ulong TrailingSignificandMask = 0x0003_FFFF_FFFF_FFFF;
        internal const int TrailingSignificandWidth = 50;

        internal const short EMax = 384;
        internal const short EMin = -383;

        internal const byte Precision = 16;
        internal const ushort ExponentBias = 398;

        internal const short MaxQExponent = EMax - Precision + 1;
        internal const short MinQExponent = EMin - Precision + 1;

        internal const long MaxSignificand = 9_999_999_999_999_999; // 16 digits
        internal const long MinSignificand = -9_999_999_999_999_999; // 16 digits

        // The 5 bits that classify the value as NaN, Infinite, or Finite
        // If the Classification bits are set to 11111, the value is NaN
        // If the Classification bits are set to 11110, the value is Infinite
        // Otherwise, the value is Finite
        internal const ulong ClassificationMask = 0x7C00_0000_0000_0000;
        internal const ulong NaNMask = 0x7C00_0000_0000_0000;
        internal const ulong SNaNMask = 0x7E00_0000_0000_0000;
        internal const ulong InfinityMask = 0x7800_0000_0000_0000;

        // If the Classification bits are set to 11XXX, we encode the significand one way. Otherwise, we encode it a different way
        internal const ulong SpecialEncodingMask = 0x6000_0000_0000_0000;

        // Finite significands are encoded in two different ways, depending on whether the most significant 4 bits of the significand are 0xxx or 100x. Test the MSB to classify.
        internal const ulong SignificandEncodingTypeMask = 1UL << (TrailingSignificandWidth + 3);

        // Constants representing the private bit-representation for various default values.
        // See either IEEE-754 2019 section 3.5 or https://en.wikipedia.org/wiki/Decimal64_floating-point_format for a breakdown of the encoding.

        // PositiveZero Bits
        // Hex:                   0x0000_0000_0000_0000
        // Binary:                0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000
        // Split into sections:   0 | 000_0000_000 | 0_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000
        // Section labels:        a | b            | c
        //
        // a. Sign bit.
        // b. Biased Exponent, which is q + ExponentBias (398). 000_0000_000 == 0 == -398 + 398, so this is encoding a q of -398.
        // c. Significand, set to 0.
        //
        // Encoded value:         0 x 10^-398

        private const ulong PositiveZeroBits = 0x0000_0000_0000_0000;
        private const ulong NegativeZeroBits = SignMask | PositiveZeroBits;


        // Epsilon Bits
        // Hex:                   0x0000_0000_0000_0001
        // Binary:                0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001
        // Split into sections:   0 | 000_0000_000 | 0_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001
        // Section labels:        a | b            | c
        //
        // a. Sign bit.
        // b. Biased Exponent, which is q + ExponentBias (398). 000_0000_000 == 0 == -398 + 398, so this is encoding a q of -398.
        // c. Significand, set to 1.
        //
        // Encoded value:         1 x 10^-398

        private const ulong EpsilonBits = 0x0000_0000_0000_0001;

        // PositiveInfinityBits
        // Hex:                   0x7800_0000_0000_0000
        // Binary:                0111_1000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000
        // Split into sections:   0 | 111_1000_0000_00 | 00_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000
        // Section labels:        a | b                | c
        //
        // a. Sign bit.
        // b. Combination field G0 through G12. G0-G4 == 11110 encodes infinity.
        // c. Trailing significand.
        // Note: Canonical infinity has everything after G5 set to 0.

        private const ulong PositiveInfinityBits = 0x7800_0000_0000_0000;
        private const ulong NegativeInfinityBits = SignMask | PositiveInfinityBits;

        // QNanBits Bits
        // Hex:                   0xFC00_0000_0000_0000
        // Binary:                1111_1100_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000
        // Split into sections:   1 | 111_1100_0000_00 | 00_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000
        // Section labels:        a | b                | c
        //
        // a. Sign bit (ignored for NaN).
        // b. Combination field G0 through G12. G0-G4 == 11111 encodes NaN.
        // c. Trailing significand. Can be used to encode a payload, to distinguish different NaNs.
        // Note: Canonical NaN has G6-G12 as 0 and the encoding of the payload also canonical.

        private const ulong QNanBits = SignMask | NaNMask;
        private const ulong SNanBits = SignMask | SNaNMask;


        // MaxValueBits
        // Hex:                   0x77FB_86F2_6FC0_FFFF
        // Binary:                0111_0111_1111_1011_1000_0110_1111_0010_0110_1111_1100_0000_1111_1111_1111_1111
        // Split into sections:   0 | 11 | 1_0111_1111_1 | 011_1000_0110_1111_0010_0110_1111_1100_0000_1111_1111_1111_1111
        //                        0 | 11 | 10_1111_1111  | [10_0]011_1000_0110_1111_0010_0110_1111_1100_0000_1111_1111_1111_1111
        // Section labels:        a | b  | c             | d
        //
        // a. Sign bit.
        // b. G0 and G1 of the combination field, "11" indicates this version of encoding.
        // c. Biased Exponent, which is q + ExponentBias (398). 10_1111_1111 == 767 == 369 + 398, so this is encoding a q of 369 (which is MaxQExponent).
        // d. Significand. Section b. indicates an implied prefix of [100]. [10_0]011_1000_0110_1111_0010_0110_1111_1100_0000_1111_1111_1111_1111 == 9,999,999,999,999,999.
        //
        // Encoded value:         9,999,999,999,999,999 x 10^369

        private const ulong MaxValueBits = 0x77FB_86F2_6FC0_FFFF;
        private const ulong MinValueBits = SignMask | MaxValueBits;

        // PositiveOneBits Bits
        // Hex:                   0x2CA0_0000_0000_0001
        // Binary:                0010_1100_1010_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001
        // Split into sections:   0 | 010_1100_101 | 0_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001
        //                        0 | 01_0110_0101 | 0_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001
        // Section labels:        a | b            | c
        //
        // a. Sign bit.
        // b. Biased Exponent, which is q + ExponentBias (398). 01_0110_0101 == 398 == 0 + 398, so this is encoding a q of 0.
        // c. Significand, set to 1.
        //
        // Encoded value:         1 x 10^0

        private const ulong PositiveOneBits = 0x2CA0_0000_0000_0001; // TODO write a unit test for all these constants
        private const ulong NegativeOneBits = SignMask | PositiveOneBits;

        /*
                private const ulong EBits = 0; // TODO
                private const ulong PiBits = 0; // TODO
                private const ulong TauBits = 0; // TODO*/

        internal readonly ulong _value;

        /// <summary>Initializes a new instance of the <see cref="Decimal64" /> struct, representing the value <paramref name="significand"/> * 10 ^ <paramref name="exponent"/></summary>
        /// <param name="significand">The signed integral significand.</param>
        /// <param name="exponent">The signed base-10 exponent.</param>
        public Decimal64(long significand, int exponent)
        {
            throw new NotImplementedException();
        }

        //
        // Internal Constructors and Decoders
        //

        internal Decimal64(ulong value)
        {
            _value = value;
        }

        // Constructs a Decimal64 representing a value in the form (-1)^s * 10^q * c, where
        // * s is 0 or 1
        // * q is any integer MinQExponent <= q <= MaxQExponent
        // * c is the significand represented by a digit string of the form
        //   `d0 d1 d2 ... dp-1`, where p is Precision. c is an integer with 0 <= c < 10^p.
        internal Decimal64(bool sign, short q, ulong c)
        {
            Debug.Assert(q >= MinQExponent && q <= MaxQExponent);
            Debug.Assert(q + ExponentBias >= 0);
            Debug.Assert(c <= MaxSignificand);

            ulong trailing_sig = c & TrailingSignificandMask;

            // Two types of combination encodings for finite numbers
            ulong combination = 0;

            if ((c & SignificandEncodingTypeMask) == 0)
            {
                // We are encoding a significand that has the most significand 4 bits set to 0xyz
                combination |= (ulong)(q + ExponentBias) << 3;
                combination |= 0b0111 & (c >> TrailingSignificandWidth); // combination = (biased_exponent, xyz)
            }
            else
            {
                // We are encoding a significand that has the most significand 4 bits set to 100x
                combination |= 0b11 << (CombinationWidth - 2);
                combination |= (ulong)(q + ExponentBias) << 1;
                combination |= 0b0001 & (c >> CombinationShift); // combination = (11, biased_exponent, x)
            }

            _value = ((sign ? 1UL : 0UL) << SignShift) + (combination << CombinationShift) + trailing_sig;
        }

        internal ushort BiasedExponent
        {
            get
            {
                ulong bits = _value;
                return ExtractBiasedExponentFromBits(bits);
            }
        }

        internal short Exponent
        {
            get
            {
                return (short)(BiasedExponent - ExponentBias);
            }
        }

        internal ulong Significand
        {
            get
            {
                return ExtractSignificandFromBits(_value);
            }
        }

        internal ulong TrailingSignificand
        {
            get
            {
                return _value & TrailingSignificandMask;
            }
        }

        // returns garbage for infinity and NaN, TODO maybe fix this
        internal static ushort ExtractBiasedExponentFromBits(ulong bits)
        {
            ushort combination = (ushort)((bits >> CombinationShift) & ShiftedCombinationMask);

            // Two types of encodings for finite numbers
            if ((bits & SpecialEncodingMask) == SpecialEncodingMask)
            {
                // G0 and G1 are 11, exponent is stored in G2:G(CombinationWidth - 1)
                return (ushort)(combination >> 1);
            }
            else
            {
                // G0 and G1 are not 11, exponent is stored in G0:G(CombinationWidth - 3)
                return (ushort)(combination >> 3);
            }
        }

        // returns garbage for infinity and NaN, TODO maybe fix this
        internal static ulong ExtractSignificandFromBits(ulong bits)
        {
            ushort combination = (ushort)((bits >> CombinationShift) & ShiftedCombinationMask);

            // Two types of encodings for finite numbers
            ulong significand;
            if ((bits & SpecialEncodingMask) == SpecialEncodingMask)
            {
                // G0 and G1 are 11, 4 MSBs of significand are 100x, where x is G(CombinationWidth)
                significand = (ulong)(0b1000 | (combination & 0b1));
            }
            else
            {
                // G0 and G1 are not 11, 4 MSBs of significand are 0xyz, where G(CombinationWidth - 2):G(CombinationWidth)
                significand = (ulong)(combination & 0b111);
            }
            significand <<= TrailingSignificandWidth;
            significand += bits & TrailingSignificandMask;
            return significand;
        }

        // IEEE 754 specifies NaNs to be propagated
        internal static Decimal64 Negate(Decimal64 value)
        {
            return IsNaN(value) ? value : new Decimal64(value._value ^ SignMask);
        }

        private static ulong StripSign(Decimal64 value)
        {
            return value._value & ~SignMask;
        }

        //
        // Parsing (INumberBase, IParsable, ISpanParsable)
        //

        /// <summary>
        /// Parses a <see cref="Decimal64"/> from a <see cref="string"/> in the default parse style.
        /// </summary>
        /// <param name="s">The input to be parsed.</param>
        /// <returns>The equivalent <see cref="Decimal64"/> value representing the input string. If the input exceeds Decimal64's range, a <see cref="Decimal64.PositiveInfinity"/> or <see cref="Decimal64.NegativeInfinity"/> is returned. </returns>
        public static Decimal64 Parse(string s)
        {
            if (s == null) ThrowHelper.ThrowArgumentNullException("s");
            return IeeeDecimalNumber.ParseDecimal64(s, DefaultParseStyle, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Parses a <see cref="Decimal64"/> from a <see cref="string"/> in the given <see cref="NumberStyles"/>.
        /// </summary>
        /// <param name="s">The input to be parsed.</param>
        /// <param name="style">The <see cref="NumberStyles"/> used to parse the input.</param>
        /// <returns>The equivalent <see cref="Decimal64"/> value representing the input string. If the input exceeds Decimal64's range, a <see cref="Decimal64.PositiveInfinity"/> or <see cref="Decimal64.NegativeInfinity"/> is returned. </returns>
        public static Decimal64 Parse(string s, NumberStyles style)
        {
            IeeeDecimalNumber.ValidateParseStyleFloatingPoint(style);
            if (s == null) ThrowHelper.ThrowArgumentNullException("s");
            return IeeeDecimalNumber.ParseDecimal64(s, style, NumberFormatInfo.CurrentInfo);
        }

        /// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)" />
        public static Decimal64 Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        {
            IeeeDecimalNumber.ValidateParseStyleFloatingPoint(DefaultParseStyle); // TODO I copied this from NumberFormatInfo to IeeeDecimalNumber, is that ok?
            return IeeeDecimalNumber.ParseDecimal64(s, DefaultParseStyle, NumberFormatInfo.GetInstance(provider));
        }

        /// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)" />
        public static Decimal64 Parse(string s, IFormatProvider? provider)
        {
            if (s == null) ThrowHelper.ThrowArgumentNullException("s");
            return IeeeDecimalNumber.ParseDecimal64(s, DefaultParseStyle, NumberFormatInfo.GetInstance(provider));
        }

        /// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)" />
        public static Decimal64 Parse(ReadOnlySpan<char> s, NumberStyles style = DefaultParseStyle, IFormatProvider? provider = null)
        {
            IeeeDecimalNumber.ValidateParseStyleFloatingPoint(style);
            return IeeeDecimalNumber.ParseDecimal64(s, style, NumberFormatInfo.GetInstance(provider));
        }

        /// <inheritdoc cref="INumberBase{TSelf}.Parse(string, NumberStyles, IFormatProvider?)" />
        public static Decimal64 Parse(string s, NumberStyles style, IFormatProvider? provider)
        {
            IeeeDecimalNumber.ValidateParseStyleFloatingPoint(style);
            if (s == null) ThrowHelper.ThrowArgumentNullException("s");
            return IeeeDecimalNumber.ParseDecimal64(s, style, NumberFormatInfo.GetInstance(provider));
        }

        /// <summary>
        /// Tries to parse a <see cref="Decimal64"/> from a <see cref="string"/> in the default parse style.
        /// </summary>
        /// <param name="s">The input to be parsed.</param>
        /// <param name="result">The equivalent <see cref="Decimal64"/> value representing the input string if the parse was successful. If the input exceeds Decimal64's range, a <see cref="Decimal64.PositiveInfinity"/> or <see cref="Decimal64.NegativeInfinity"/> is returned. If the parse was unsuccessful, a default <see cref="Decimal64"/> value is returned.</param>
        /// <returns><see langword="true" /> if the parse was successful, <see langword="false" /> otherwise.</returns>
        public static bool TryParse([NotNullWhen(true)] string? s, out Decimal64 result)
        {
            if (s == null)
            {
                result = default;
                return false;
            }
            return TryParse(s, DefaultParseStyle, provider: null, out result);
        }

        /// <summary>
        /// Tries to parse a <see cref="Decimal64"/> from a <see cref="ReadOnlySpan{Char}"/> in the default parse style.
        /// </summary>
        /// <param name="s">The input to be parsed.</param>
        /// <param name="result">The equivalent <see cref="Decimal64"/> value representing the input string if the parse was successful. If the input exceeds Decimal64's range, a <see cref="Decimal64.PositiveInfinity"/> or <see cref="Decimal64.NegativeInfinity"/> is returned. If the parse was unsuccessful, a default <see cref="Decimal64"/> value is returned.</param>
        /// <returns><see langword="true" /> if the parse was successful, <see langword="false" /> otherwise.</returns>
        public static bool TryParse(ReadOnlySpan<char> s, out Decimal64 result)
        {
            return TryParse(s, DefaultParseStyle, provider: null, out result);
        }

        /// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)" />
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Decimal64 result) => TryParse(s, DefaultParseStyle, provider, out result);

        /// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)" />
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Decimal64 result) => TryParse(s, DefaultParseStyle, provider, out result);

        /// <inheritdoc cref="INumberBase{TSelf}.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?, out TSelf)" />
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out Decimal64 result)
        {
            IeeeDecimalNumber.ValidateParseStyleFloatingPoint(style);
            return IeeeDecimalNumber.TryParseDecimal64(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

        /// <inheritdoc cref="INumberBase{TSelf}.TryParse(string?, NumberStyles, IFormatProvider?, out TSelf)" />
        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out Decimal64 result)
        {
            IeeeDecimalNumber.ValidateParseStyleFloatingPoint(style);

            if (s == null)
            {
                result = Zero;
                return false;
            }

            return IeeeDecimalNumber.TryParseDecimal64(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

        //
        // Misc. Methods (IComparable, IEquatable)
        //

        /// <summary>
        /// Compares this object to another object, returning an integer that indicates the relationship.
        /// </summary>
        /// <returns>A value less than zero if this is less than <paramref name="obj"/>, zero if this is equal to <paramref name="obj"/>, or a value greater than zero if this is greater than <paramref name="obj"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="obj"/> is not of type <see cref="Decimal64"/>.</exception>
        public int CompareTo(object? obj) => throw new NotImplementedException();

        /// <summary>
        /// Compares this object to another object, returning an integer that indicates the relationship.
        /// </summary>
        /// <returns>A value less than zero if this is less than <paramref name="other"/>, zero if this is equal to <paramref name="other"/>, or a value greater than zero if this is greater than <paramref name="other"/>.</returns>
        public int CompareTo(Decimal64 other) => throw new NotImplementedException();

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified <paramref name="obj"/>.
        /// </summary>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return (obj is Decimal64 other) && Equals(other);
        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified <paramref name="other"/> value.
        /// </summary>
        public bool Equals(Decimal64 other)
        {
            return this == other
                || (IsNaN(this) && IsNaN(other));
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            // TODO we know that NaNs and Zeros should hash the same. Should values of the same cohort have the same hash?
            throw new NotImplementedException();
        }

        // 5.5.2 of the IEEE Spec
        [CLSCompliant(false)]
        public static ulong EncodeDecimal(Decimal64 x) => throw new NotImplementedException();
        [CLSCompliant(false)]
        public static Decimal64 DecodeDecimal(ulong x) => throw new NotImplementedException();
        [CLSCompliant(false)]
        public static ulong EncodeBinary(Decimal64 x) => throw new NotImplementedException();
        [CLSCompliant(false)]
        public static Decimal64 DecodeBinary(ulong x) => throw new NotImplementedException();

        //
        // Formatting (IFormattable, ISpanFormattable)
        //

        /// <summary>
        /// Returns a string representation of the current value.
        /// </summary>
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string representation of the current value using the specified <paramref name="format"/>.
        /// </summary>
        public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string representation of the current value with the specified <paramref name="provider"/>.
        /// </summary>
        public string ToString(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string representation of the current value using the specified <paramref name="format"/> and <paramref name="provider"/>.
        /// </summary>
        public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? provider) // TODO the interface calls this second param "formatProvider". Which do we want?
        {
            // TODO this is temporary Formatting for debugging
            if (IsNaN(this))
            {
                return "NaN";
            }
            else if (IsPositiveInfinity(this))
            {
                return "Infinity";
            }
            else if (IsNegativeInfinity(this))
            {
                return "-Infinity";
            }

            return (IsPositive(this) ? "" : "-") + Significand.ToString() + "E" + Exponent.ToString();
        }

        /// <summary>
        /// Tries to format the value of the current Decimal64 instance into the provided span of characters.
        /// </summary>
        /// <param name="destination">When this method returns, this instance's value formatted as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, the number of characters that were written in <paramref name="destination"/>.</param>
        /// <param name="format">A span containing the characters that represent a standard or custom format string that defines the acceptable format for <paramref name="destination"/>.</param>
        /// <param name="provider">An optional object that supplies culture-specific formatting information for <paramref name="destination"/>.</param>
        /// <returns></returns>
        public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        {
            throw new NotImplementedException();
        }


        //
        // Explicit Convert To Decimal64
        // (T -> Decimal64 is lossy)
        //

        /// <summary>Explicitly converts a <see cref="nint" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        public static explicit operator Decimal64(nint value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a <see cref="nuint" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        [CLSCompliant(false)]
        public static explicit operator Decimal64(nuint value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a <see cref="long" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        public static explicit operator Decimal64(long value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a <see cref="ulong" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        [CLSCompliant(false)]
        public static explicit operator Decimal64(ulong value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a <see cref="Int128" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        public static explicit operator Decimal64(Int128 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a <see cref="UInt128" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        [CLSCompliant(false)]
        public static explicit operator Decimal64(UInt128 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a <see cref="Half" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        public static explicit operator Decimal64(Half value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a <see cref="float" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        public static explicit operator Decimal64(float value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a <see cref="double" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        public static explicit operator Decimal64(double value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a <see cref="decimal" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        public static explicit operator Decimal64(decimal value) => throw new NotImplementedException();

        // public static explicit operator Decimal64(Decimal128 value) => throw new NotImplementedException(); TODO

        //
        // Explicit Convert From Decimal64
        // (Decimal64 -> T is lossy)
        // - Includes a "checked" conversion if T cannot represent infinity and NaN
        //

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="byte" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="byte" /> value.</returns>
        public static explicit operator byte(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="byte" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="byte" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="byte" />.</exception>
        public static explicit operator checked byte(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="sbyte" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="sbyte" /> value.</returns>
        [CLSCompliant(false)]
        public static explicit operator sbyte(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="sbyte" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="sbyte" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="sbyte" />.</exception>
        [CLSCompliant(false)]
        public static explicit operator checked sbyte(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="char" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="char" /> value.</returns>
        public static explicit operator char(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="char" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="char" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="char" />.</exception>
        public static explicit operator checked char(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="short" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="short" /> value.</returns>
        public static explicit operator short(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="short" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="short" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="short" />.</exception>
        public static explicit operator checked short(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="ushort" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="ushort" /> value.</returns>
        [CLSCompliant(false)]
        public static explicit operator ushort(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="ushort" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="ushort" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="ushort" />.</exception>
        [CLSCompliant(false)]
        public static explicit operator checked ushort(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="int" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="int" /> value.</returns>
        public static explicit operator int(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="int" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="int" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="int" />.</exception>
        public static explicit operator checked int(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="uint" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="uint" /> value.</returns>
        [CLSCompliant(false)]
        public static explicit operator uint(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="uint" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="uint" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="uint" />.</exception>
        [CLSCompliant(false)]
        public static explicit operator checked uint(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="nint" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="nint" /> value.</returns>
        public static explicit operator nint(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="nint" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="nint" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="nint" />.</exception>
        public static explicit operator checked nint(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="nuint" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="nuint" /> value.</returns>
        [CLSCompliant(false)]
        public static explicit operator nuint(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="nuint" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="nuint" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="nuint" />.</exception>
        [CLSCompliant(false)]
        public static explicit operator checked nuint(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="long" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="long" /> value.</returns>
        public static explicit operator long(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="long" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="long" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="long" />.</exception>
        public static explicit operator checked long(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="ulong" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="ulong" /> value.</returns>
        [CLSCompliant(false)]
        public static explicit operator ulong(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="ulong" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="ulong" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="ulong" />.</exception>
        [CLSCompliant(false)]
        public static explicit operator checked ulong(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="Int128" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="Int128" /> value.</returns>
        public static explicit operator Int128(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="Int128" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="Int128" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="Int128" />.</exception>
        public static explicit operator checked Int128(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="UInt128" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="UInt128" /> value.</returns>
        [CLSCompliant(false)]
        public static explicit operator UInt128(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="UInt128" /> value, throwing an overflow exception for any values that fall outside the representable range.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="UInt128" /> value.</returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <see cref="UInt128" />.</exception>
        [CLSCompliant(false)]
        public static explicit operator checked UInt128(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="Half" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="Half" /> value.</returns>
        public static explicit operator Half(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="float" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="float" /> value.</returns>
        public static explicit operator float(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="double" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="double" /> value.</returns>
        public static explicit operator double(Decimal64 value) => throw new NotImplementedException();

        /// <summary>Explicitly converts a 64-bit decimal floating-point value to its nearest representable <see cref="decimal" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable <see cref="decimal" /> value.</returns>
        public static explicit operator decimal(Decimal64 value) => throw new NotImplementedException();

        //
        // Implicit Convert To Decimal64
        // (T -> Decimal64 is not lossy)
        //

        /// <summary>Implicitly converts a <see cref="byte" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        public static implicit operator Decimal64(byte value) => throw new NotImplementedException();

        /// <summary>Implicitly converts a <see cref="sbyte" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        [CLSCompliant(false)]
        public static implicit operator Decimal64(sbyte value) => throw new NotImplementedException();

        /// <summary>Implicitly converts a <see cref="char" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        public static implicit operator Decimal64(char value) => throw new NotImplementedException();

        /// <summary>Implicitly converts a <see cref="short" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        public static implicit operator Decimal64(short value) => throw new NotImplementedException();

        /// <summary>Implicitly converts a <see cref="ushort" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        [CLSCompliant(false)]
        public static implicit operator Decimal64(ushort value) => throw new NotImplementedException();

        /// <summary>Implicitly converts a <see cref="int" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        [CLSCompliant(false)]
        public static explicit operator Decimal64(int value) => throw new NotImplementedException();

        /// <summary>Implicitly converts a <see cref="uint" /> value to its nearest representable 64-bit decimal floating-point value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to its nearest representable 64-bit decimal floating-point value.</returns>
        [CLSCompliant(false)]
        public static explicit operator Decimal64(uint value) => throw new NotImplementedException();


        //
        // Implicit Convert From Decimal64
        // (Decimal64 -> T is not lossy)
        //

        // public static implicit operator Decimal128(Decimal64 value) => throw new NotImplementedException(); TODO

        //
        // IAdditionOperators
        //

        /// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
        public static Decimal64 operator +(Decimal64 left, Decimal64 right) => throw new NotImplementedException();

        //
        // IAdditiveIdentity
        //

        /// <inheritdoc cref="IAdditiveIdentity{TSelf, TResult}.AdditiveIdentity" />
        static Decimal64 IAdditiveIdentity<Decimal64, Decimal64>.AdditiveIdentity => new Decimal64(PositiveZeroBits); // TODO make sure this is a zero such that the quantum of any other value is preserved on addition


        //
        // IComparisonOperators
        //

        /// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
        public static bool operator <(Decimal64 left, Decimal64 right) => throw new NotImplementedException();

        /// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
        public static bool operator >(Decimal64 left, Decimal64 right) => throw new NotImplementedException();

        /// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
        public static bool operator <=(Decimal64 left, Decimal64 right) => throw new NotImplementedException();

        /// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
        public static bool operator >=(Decimal64 left, Decimal64 right) => throw new NotImplementedException();

        //
        // IDecimalFloatingPointIeee754
        //

        /// <inheritdoc cref="IDecimalFloatingPointIeee754{TSelf}.Quantize(TSelf, TSelf)" />
        public static Decimal64 Quantize(Decimal64 x, Decimal64 y)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IDecimalFloatingPointIeee754{TSelf}.GetQuantum(TSelf)" />
        public static Decimal64 GetQuantum(Decimal64 x) => throw new NotImplementedException();

        /// <inheritdoc cref="IDecimalFloatingPointIeee754{TSelf}.HaveSameQuantum(TSelf, TSelf)" />
        public static bool HaveSameQuantum(Decimal64 x, Decimal64 y)
        {
            return x.Exponent == y.Exponent
                || (IsInfinity(x) && IsInfinity(y))
                || (IsNaN(x) && IsNaN(y));
        }

        //
        // IDecrementOperators
        //

        /// <inheritdoc cref="IDecrementOperators{TSelf}.op_Decrement(TSelf)" />
        public static Decimal64 operator --(Decimal64 value) => throw new NotImplementedException();

        //
        // IDivisionOperators
        //

        /// <inheritdoc cref="IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
        public static Decimal64 operator /(Decimal64 left, Decimal64 right) => throw new NotImplementedException();

        //
        // IEqualityOperators
        //

        // Fast access for 10^n where n is 0:(Precision - 1)
        private static readonly ulong[] s_powers10 = new ulong[] {
                1,
                10,
                100,
                1000,
                10000,
                100000,
                1000000,
                10000000,
                100000000,
                1000000000,
                10000000000,
                100000000000,
                1000000000000,
                10000000000000,
                100000000000000,
                1000000000000000
        };

        /// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)" />
        public static bool operator ==(Decimal64 left, Decimal64 right) // TODO we can probably do this faster
        {
            if (IsNaN(left) || IsNaN(right))
            {
                // IEEE defines that NaN is not equal to anything, including itself.
                return false;
            }

            if (IsZero(left) && IsZero(right))
            {
                // IEEE defines that positive and negative zero are equivalent.
                return true;
            }

            bool sameSign = IsPositive(left) == IsPositive(right);

            if (IsInfinity(left) || IsInfinity(right))
            {
                if (IsInfinity(left) && IsInfinity(right) && sameSign)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // IEEE defines that two values of the same cohort are numerically equivalent

            ulong leftSignificand = left.Significand;
            ulong rightSignificand = right.Significand;
            short leftQ = left.Exponent;
            short rightQ = right.Exponent;
            int diffQ = leftQ - rightQ;

            bool sameNumericalValue = false;
            if (int.Abs(diffQ) < Precision) // If diffQ is >= Precision, the non-zero finite values have exponents too far apart for them to possibly be equal
            {
                try
                {
                    if (diffQ < 0)
                    {
                        // leftQ is smaller than rightQ, scale leftSignificand
                        leftSignificand = checked(leftSignificand * s_powers10[int.Abs(diffQ)]);
                    }
                    else
                    {
                        // rightQ is smaller than (or equal to) leftQ, scale rightSignificand
                        rightSignificand = checked(rightSignificand * s_powers10[diffQ]);
                    }
                }
                catch
                {
                    // multiplication overflowed, return false
                    return false;
                }

                if (leftSignificand == rightSignificand)
                {
                    sameNumericalValue = true;
                }
            }

            return sameNumericalValue && sameSign;
        }

        /// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)" />
        public static bool operator !=(Decimal64 left, Decimal64 right) => throw new NotImplementedException();

        //
        // IExponentialFunctions PLATINUM
        //

        /*        /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp" />
                public static Decimal64 Exp(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="IExponentialFunctions{TSelf}.ExpM1(TSelf)" />
                public static Decimal64 ExpM1(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp2(TSelf)" />
                public static Decimal64 Exp2(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp2M1(TSelf)" />
                public static Decimal64 Exp2M1(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp10(TSelf)" />
                public static Decimal64 Exp10(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp10M1(TSelf)" />
                public static Decimal64 Exp10M1(Decimal64 x) => throw new NotImplementedException();*/

        //
        // IFloatingPoint
        //

        /// <inheritdoc cref="IFloatingPoint{TSelf}.Ceiling(TSelf)" />
        public static Decimal64 Ceiling(Decimal64 x) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.Floor(TSelf)" />
        public static Decimal64 Floor(Decimal64 x) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf)" />
        public static Decimal64 Round(Decimal64 x) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf, int)" />
        public static Decimal64 Round(Decimal64 x, int digits) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf, MidpointRounding)" />
        public static Decimal64 Round(Decimal64 x, MidpointRounding mode) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf, int, MidpointRounding)" />
        public static Decimal64 Round(Decimal64 x, int digits, MidpointRounding mode) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.Truncate(TSelf)" />
        public static Decimal64 Truncate(Decimal64 x) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.GetExponentByteCount()" />
        int IFloatingPoint<Decimal64>.GetExponentByteCount() => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.GetExponentShortestBitLength()" />
        int IFloatingPoint<Decimal64>.GetExponentShortestBitLength() => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.GetSignificandBitLength()" />
        int IFloatingPoint<Decimal64>.GetSignificandBitLength() => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.GetSignificandByteCount()" />
        int IFloatingPoint<Decimal64>.GetSignificandByteCount() => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteExponentBigEndian(Span{byte}, out int)" />
        bool IFloatingPoint<Decimal64>.TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteExponentLittleEndian(Span{byte}, out int)" />
        bool IFloatingPoint<Decimal64>.TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteSignificandBigEndian(Span{byte}, out int)" />
        bool IFloatingPoint<Decimal64>.TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteSignificandLittleEndian(Span{byte}, out int)" />
        bool IFloatingPoint<Decimal64>.TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        //
        // IFloatingPointConstants
        //

        /// <inheritdoc cref="IFloatingPointConstants{TSelf}.E" />
        public static Decimal64 E => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPointConstants{TSelf}.Pi" />
        public static Decimal64 Pi => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPointConstants{TSelf}.Tau" />
        public static Decimal64 Tau => throw new NotImplementedException();

        //
        // IFloatingPointIeee754
        //

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Epsilon" />
        public static Decimal64 Epsilon => new Decimal64(EpsilonBits);                      //  1E-101

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.NaN" />
        public static Decimal64 NaN => new Decimal64(QNanBits);                             //  0.0 / 0.0

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.NegativeInfinity" />
        public static Decimal64 NegativeInfinity => new Decimal64(NegativeInfinityBits);    // -1.0 / 0.0

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.NegativeZero" />
        public static Decimal64 NegativeZero => new Decimal64(NegativeZeroBits);            // -0E-101

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.PositiveInfinity" />
        public static Decimal64 PositiveInfinity => new Decimal64(PositiveInfinityBits);    //  1.0 / 0.0

        /*        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Atan2(TSelf, TSelf)" />
                public static Decimal64 Atan2(Decimal64 y, Decimal64 x) => throw new NotImplementedException(); // PLATINUM

                /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Atan2Pi(TSelf, TSelf)" />
                public static Decimal64 Atan2Pi(Decimal64 y, Decimal64 x) => throw new NotImplementedException();*/ // PLATINUM

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.BitDecrement(TSelf)" />
        public static Decimal64 BitDecrement(Decimal64 x) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.BitIncrement(TSelf)" />
        public static Decimal64 BitIncrement(Decimal64 x) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.FusedMultiplyAdd(TSelf, TSelf, TSelf)" />
        public static Decimal64 FusedMultiplyAdd(Decimal64 left, Decimal64 right, Decimal64 addend) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Ieee754Remainder(TSelf, TSelf)" />
        public static Decimal64 Ieee754Remainder(Decimal64 left, Decimal64 right) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.ILogB(TSelf)" />
        public static int ILogB(Decimal64 x) => throw new NotImplementedException();

        // /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Lerp(TSelf)" />
        public static Decimal64 Lerp(Decimal64 value1, Decimal64 value2, Decimal64 amount) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.ReciprocalEstimate(TSelf)" />
        public static Decimal64 ReciprocalEstimate(Decimal64 x) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.ReciprocalSqrtEstimate(TSelf)" />
        public static Decimal64 ReciprocalSqrtEstimate(Decimal64 x) => throw new NotImplementedException();

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.ScaleB(TSelf, int)" />
        public static Decimal64 ScaleB(Decimal64 x, int n) => throw new NotImplementedException();

        // /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Compound(TSelf, TSelf)" />
        // public static Decimal64 Compound(Half x, Decimal64 n) => throw new NotImplementedException(); // PLATINUM

        //
        // IHyperbolicFunctions PLATINUM
        //

        /*        /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Acosh(TSelf)" />
                public static Decimal64 Acosh(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Asinh(TSelf)" />
                public static Decimal64 Asinh(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Atanh(TSelf)" />
                public static Decimal64 Atanh(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Cosh(TSelf)" />
                public static Decimal64 Cosh(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Sinh(TSelf)" />
                public static Decimal64 Sinh(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Tanh(TSelf)" />
                public static Decimal64 Tanh(Decimal64 x) => throw new NotImplementedException();*/

        //
        // IIncrementOperators
        //

        /// <inheritdoc cref="IIncrementOperators{TSelf}.op_Increment(TSelf)" />
        public static Decimal64 operator ++(Decimal64 value) => throw new NotImplementedException();

        //
        // ILogarithmicFunctions PLATINUM
        //

        /*        /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log(TSelf)" />
                public static Decimal64 Log(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log(TSelf, TSelf)" />
                public static Decimal64 Log(Decimal64 x, Decimal64 newBase) => throw new NotImplementedException();

                /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log10(TSelf)" />
                public static Decimal64 Log10(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.LogP1(TSelf)" />
                public static Decimal64 LogP1(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log2(TSelf)" />
                public static Decimal64 Log2(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log2P1(TSelf)" />
                public static Decimal64 Log2P1(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log10P1(TSelf)" />
                public static Decimal64 Log10P1(Decimal64 x) => throw new NotImplementedException();*/

        //
        // IMinMaxValue
        //

        /// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue" />
        public static Decimal64 MaxValue => new Decimal64(MaxValueBits);                    //  9.999999E90

        /// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue" />
        public static Decimal64 MinValue => new Decimal64(MinValueBits);                    // -9.999999E90

        //
        // IModulusOperators
        //

        /// <inheritdoc cref="IModulusOperators{TSelf, TOther, TResult}.op_Modulus(TSelf, TOther)" />
        public static Decimal64 operator %(Decimal64 left, Decimal64 right) => throw new NotImplementedException();

        //
        // IMultiplicativeIdentity
        //

        /// <inheritdoc cref="IMultiplicativeIdentity{TSelf, TResult}.MultiplicativeIdentity" />
        public static Decimal64 MultiplicativeIdentity => new Decimal64(PositiveZeroBits); // TODO make sure this is a zero such that the quantum of any other value is preserved on multiplication

        //
        // IMultiplyOperators
        //

        /// <inheritdoc cref="IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
        public static Decimal64 operator *(Decimal64 left, Decimal64 right) => throw new NotImplementedException();

        //
        // INumber
        //

        /// <inheritdoc cref="INumber{TSelf}.Clamp(TSelf, TSelf, TSelf)" />
        public static Decimal64 Clamp(Decimal64 value, Decimal64 min, Decimal64 max) => throw new NotImplementedException();

        /// <inheritdoc cref="INumber{TSelf}.CopySign(TSelf, TSelf)" />
        public static Decimal64 CopySign(Decimal64 value, Decimal64 sign) => throw new NotImplementedException();

        /// <inheritdoc cref="INumber{TSelf}.Max(TSelf, TSelf)" />
        public static Decimal64 Max(Decimal64 x, Decimal64 y) => throw new NotImplementedException();

        /// <inheritdoc cref="INumber{TSelf}.MaxNumber(TSelf, TSelf)" />
        public static Decimal64 MaxNumber(Decimal64 x, Decimal64 y) => throw new NotImplementedException();

        /// <inheritdoc cref="INumber{TSelf}.Min(TSelf, TSelf)" />
        public static Decimal64 Min(Decimal64 x, Decimal64 y) => throw new NotImplementedException();

        /// <inheritdoc cref="INumber{TSelf}.MinNumber(TSelf, TSelf)" />
        public static Decimal64 MinNumber(Decimal64 x, Decimal64 y) => throw new NotImplementedException();

        /// <inheritdoc cref="INumber{TSelf}.Sign(TSelf)" />
        public static int Sign(Decimal64 value) => throw new NotImplementedException();


        //
        // INumberBase (well defined/commonly used values)
        //

        /// <inheritdoc cref="INumberBase{TSelf}.One" />
        public static Decimal64 One => new Decimal64(PositiveOneBits);                      //  1E0

        /// <inheritdoc cref="INumberBase{TSelf}.Radix" />
        static int INumberBase<Decimal64>.Radix => 10; // TODO this should be exposed implicitly as it is required by IEEE

        /// <inheritdoc cref="INumberBase{TSelf}.Zero" />
        public static Decimal64 Zero => new Decimal64(PositiveZeroBits);                    // -0E-101

        /// <inheritdoc cref="INumberBase{TSelf}.Abs(TSelf)" />
        public static Decimal64 Abs(Decimal64 value)
        {
            return new Decimal64(value._value & ~SignMask);
        }

        /// <inheritdoc cref="INumberBase{TSelf}.CreateChecked{TOther}(TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Decimal64 CreateChecked<TOther>(TOther value)
            where TOther : INumberBase<TOther>
        {
            Decimal64 result;

            if (typeof(TOther) == typeof(Decimal64))
            {
                result = (Decimal64)(object)value;
            }
            else if (!TryConvertFromChecked(value, out result) && !TOther.TryConvertToChecked(value, out result))
            {
                ThrowHelper.ThrowNotSupportedException();
            }

            return result;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.CreateSaturating{TOther}(TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Decimal64 CreateSaturating<TOther>(TOther value)
            where TOther : INumberBase<TOther>
        {
            Decimal64 result;

            if (typeof(TOther) == typeof(Decimal64))
            {
                result = (Decimal64)(object)value;
            }
            else if (!TryConvertFromSaturating(value, out result) && !TOther.TryConvertToSaturating(value, out result))
            {
                ThrowHelper.ThrowNotSupportedException();
            }

            return result;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.CreateTruncating{TOther}(TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Decimal64 CreateTruncating<TOther>(TOther value)
            where TOther : INumberBase<TOther>
        {
            Decimal64 result;

            if (typeof(TOther) == typeof(Decimal64))
            {
                result = (Decimal64)(object)value;
            }
            else if (!TryConvertFromTruncating(value, out result) && !TOther.TryConvertToTruncating(value, out result))
            {
                ThrowHelper.ThrowNotSupportedException();
            }

            return result;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsCanonical(TSelf)" />
        static bool INumberBase<Decimal64>.IsCanonical(Decimal64 value) => throw new NotImplementedException(); // TODO this should be exposed implicitly as it is required by IEEE

        /// <inheritdoc cref="INumberBase{TSelf}.IsComplexNumber(TSelf)" />
        static bool INumberBase<Decimal64>.IsComplexNumber(Decimal64 value) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.IsEvenInteger(TSelf)" />
        public static bool IsEvenInteger(Decimal64 value) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.IsFinite(TSelf)" />
        public static bool IsFinite(Decimal64 value)
        {
            return StripSign(value) < PositiveInfinityBits;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsImaginaryNumber(TSelf)" />
        static bool INumberBase<Decimal64>.IsImaginaryNumber(Decimal64 value) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.IsInfinity(TSelf)" />
        public static bool IsInfinity(Decimal64 value)
        {
            return (value._value & ClassificationMask) == InfinityMask;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsInteger(TSelf)" />
        public static bool IsInteger(Decimal64 value) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.IsNaN(TSelf)" />
        public static bool IsNaN(Decimal64 value)
        {
            return (value._value & ClassificationMask) == NaNMask;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsNegative(TSelf)" />
        public static bool IsNegative(Decimal64 value)
        {
            return (int)(value._value) < 0;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsNegativeInfinity(TSelf)" />
        public static bool IsNegativeInfinity(Decimal64 value)
        {
            return IsInfinity(value) && IsNegative(value);
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsNormal(TSelf)" />
        public static bool IsNormal(Decimal64 value) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.IsOddInteger(TSelf)" />
        public static bool IsOddInteger(Decimal64 value) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.IsPositive(TSelf)" />
        public static bool IsPositive(Decimal64 value)
        {
            return (int)(value._value) >= 0;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsPositiveInfinity(TSelf)" />
        public static bool IsPositiveInfinity(Decimal64 value)
        {
            return IsInfinity(value) && IsPositive(value);
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsRealNumber(TSelf)" />
        public static bool IsRealNumber(Decimal64 value) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.IsSubnormal(TSelf)" />
        public static bool IsSubnormal(Decimal64 value) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.IsZero(TSelf)" />
        static bool INumberBase<Decimal64>.IsZero(Decimal64 value) // TODO this should be exposed implicitly as it is required by IEEE (see private function below)
        {
            return value.Significand == 0;
        }

        private static bool IsZero(Decimal64 value)
        {
            return value.Significand == 0;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitude(TSelf, TSelf)" />
        public static Decimal64 MaxMagnitude(Decimal64 x, Decimal64 y) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitudeNumber(TSelf, TSelf)" />
        public static Decimal64 MaxMagnitudeNumber(Decimal64 x, Decimal64 y) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitude(TSelf, TSelf)" />
        public static Decimal64 MinMagnitude(Decimal64 x, Decimal64 y) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitudeNumber(TSelf, TSelf)" />
        public static Decimal64 MinMagnitudeNumber(Decimal64 x, Decimal64 y) => throw new NotImplementedException();

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromChecked{TOther}(TOther, out TSelf)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<Decimal64>.TryConvertFromChecked<TOther>(TOther value, out Decimal64 result) => TryConvertFromChecked(value, out result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060 // Remove unused parameter
        private static bool TryConvertFromChecked<TOther>(TOther value, out Decimal64 result)
#pragma warning restore IDE0060 // Remove unused parameter
            where TOther : INumberBase<TOther>
        {
            if (typeof(TOther) == typeof(byte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(char))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(double))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Half))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(short))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(int))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(long))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(float))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(uint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                throw new NotImplementedException();
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromSaturating{TOther}(TOther, out TSelf)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<Decimal64>.TryConvertFromSaturating<TOther>(TOther value, out Decimal64 result) => TryConvertFromSaturating(value, out result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060 // Remove unused parameter
        private static bool TryConvertFromSaturating<TOther>(TOther value, out Decimal64 result)
#pragma warning restore IDE0060 // Remove unused parameter
            where TOther : INumberBase<TOther>
        {
            if (typeof(TOther) == typeof(byte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(char))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(double))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Half))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(short))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(int))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(long))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(float))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(uint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                throw new NotImplementedException();
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromTruncating{TOther}(TOther, out TSelf)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<Decimal64>.TryConvertFromTruncating<TOther>(TOther value, out Decimal64 result) => TryConvertFromTruncating(value, out result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060 // Remove unused parameter TODO remove this
        private static bool TryConvertFromTruncating<TOther>(TOther value, out Decimal64 result)
#pragma warning restore IDE0060 // Remove unused parameter
            where TOther : INumberBase<TOther>
        {
            if (typeof(TOther) == typeof(byte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(char))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(double))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Half))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(short))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(int))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(long))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(float))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(uint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                throw new NotImplementedException();
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToChecked{TOther}(TSelf, out TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<Decimal64>.TryConvertToChecked<TOther>(Decimal64 value, [MaybeNullWhen(false)] out TOther result)
        {
            if (typeof(TOther) == typeof(byte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(char))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(double))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Half))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(short))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(int))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(long))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(float))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(uint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                throw new NotImplementedException();
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToSaturating{TOther}(TSelf, out TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<Decimal64>.TryConvertToSaturating<TOther>(Decimal64 value, [MaybeNullWhen(false)] out TOther result)
        {
            if (typeof(TOther) == typeof(byte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(char))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(double))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Half))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(short))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(int))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(long))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Complex))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(float))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(uint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                throw new NotImplementedException();
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToTruncating{TOther}(TSelf, out TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<Decimal64>.TryConvertToTruncating<TOther>(Decimal64 value, [MaybeNullWhen(false)] out TOther result)
        {
            if (typeof(TOther) == typeof(byte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(char))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(double))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Half))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(short))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(int))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(long))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(Complex))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(float))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(uint))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                throw new NotImplementedException();
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                throw new NotImplementedException();
            }
            else
            {
                result = default;
                return false;
            }
        }

        //
        // IPowerFunctions // PLATINUM
        //

        /*/// <inheritdoc cref="IPowerFunctions{TSelf}.Pow(TSelf, TSelf)" />
                public static Decimal64 Pow(Decimal64 x, Decimal64 y) => throw new NotImplementedException();*/

        //
        // IRootFunctions
        //

        /*/// <inheritdoc cref="IRootFunctions{TSelf}.Cbrt(TSelf)" />
        public static Decimal64 Cbrt(Decimal64 x) => throw new NotImplementedException(); // PLATINUM

        /// <inheritdoc cref="IRootFunctions{TSelf}.Hypot(TSelf, TSelf)" />
        public static Decimal64 Hypot(Decimal64 x, Decimal64 y) => throw new NotImplementedException(); // PLATINUM


        /// <inheritdoc cref="IRootFunctions{TSelf}.RootN(TSelf, int)" />
        public static Decimal64 RootN(Decimal64 x, int n) => throw new NotImplementedException();*/ // PLATINUM

        /// <inheritdoc cref="IRootFunctions{TSelf}.Sqrt(TSelf)" />
        public static Decimal64 Sqrt(Decimal64 x) => throw new NotImplementedException();

        //
        // ISignedNumber
        //

        /// <inheritdoc cref="ISignedNumber{TSelf}.NegativeOne" />
        public static Decimal64 NegativeOne => new Decimal64(NegativeOneBits);              // -1E0

        //
        // ISubtractionOperators
        //

        /// <inheritdoc cref="ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)" />
        public static Decimal64 operator -(Decimal64 left, Decimal64 right) => throw new NotImplementedException();

        //
        // ITrigonometricFunctions PLATINUM
        //

        /*        /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Acos(TSelf)" />
                public static Decimal64 Acos(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.AcosPi(TSelf)" />
                public static Decimal64 AcosPi(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Asin(TSelf)" />
                public static Decimal64 Asin(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.AsinPi(TSelf)" />
                public static Decimal64 AsinPi(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Atan(TSelf)" />
                public static Decimal64 Atan(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.AtanPi(TSelf)" />
                public static Decimal64 AtanPi(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Cos(TSelf)" />
                public static Decimal64 Cos(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.CosPi(TSelf)" />
                public static Decimal64 CosPi(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Sin(TSelf)" />
                public static Decimal64 Sin(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinCos(TSelf)" />
                public static (Decimal64 Sin, Decimal64 Cos) SinCos(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinCosPi(TSelf)" />
                public static (Decimal64 SinPi, Decimal64 CosPi) SinCosPi(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinPi(TSelf)" />
                public static Decimal64 SinPi(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Tan(TSelf)" />
                public static Decimal64 Tan(Decimal64 x) => throw new NotImplementedException();

                /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.TanPi(TSelf)" />
                public static Decimal64 TanPi(Decimal64 x) => throw new NotImplementedException();*/

        //
        // IUnaryNegationOperators
        //

        /// <inheritdoc cref="IUnaryNegationOperators{TSelf, TResult}.op_UnaryNegation(TSelf)" />
        public static Decimal64 operator -(Decimal64 value) => Negate(value);

        //
        // IUnaryPlusOperators
        //

        /// <inheritdoc cref="IUnaryPlusOperators{TSelf, TResult}.op_UnaryPlus(TSelf)" />
        public static Decimal64 operator +(Decimal64 value) => value;
    }
}
