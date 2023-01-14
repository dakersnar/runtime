// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.CompilerServices;

namespace System.Numerics
{
    internal static partial class IeeeDecimalNumber
    {

        private static void unpack_bid32(Decimal32 x, out int s, out int e, out int k, ref ulong c)
        { s = x >> 31;
            if ((x & (3ull<<29)) == (3ull<<29))
            { if ((x & (0xFull<<27)) == (0xFull<<27))
                { if ((x & (0x1Full<<26)) != (0x1Full<<26)) return_binary32_inf(s);
                // if ((x & (1ul<<25))!=0) __set_status_flags(pfpsf, BID_INVALID_EXCEPTION);
                return_binary32_nan(s, ((((x) & 0xFFFFFul) > 999999ul) ? 0 :
                        (((unsigned long long) x) << 44)),0ull);
                }
                e = ((x >> 21) & ((1ull<<8)-1)) - 101;
                c = (1ull<<23) + (x & ((1ull<<21)-1));
                if ((unsigned long)(c) > 9999999ul) zero;
                k = 0;
            }
                      else
           {
                        e = ((x >> 23) & ((1ull << 8)-1)) -101;
             c = x & ((1ull << 23)-1);
             if (c == 0) return_binary32_zero(s);
             k = clz32_nz(c) - 8;
             c = c << k;
           }
        }


        // get full 64x64bit product
        //
        private static void __mul_64x64_to_128(ref ulong[] P, ulong CX, ulong CY)
        {
            ulong CXH, CXL, CYH, CYL, PL, PH, PM, PM2;
            CXH = (CX) >> 32;
            CXL = (uint)(CX);
            CYH = (CY) >> 32;
            CYL = (uint)(CY);

            PM = CXH * CYL;
            PH = CXH * CYH;
            PL = CXL * CYL;
            PM2 = CXL * CYH;
            PH += (PM >> 32);
            PM = (ulong)((uint)PM) + PM2 + (PL >> 32);

            (P)[1] = PH + (PM >> 32);
            (P)[0] = (PM << 32) + (uint)PL;
        }

        public static void __add_carry_out(ref ulong S, ref ulong CY, ulong X, ulong Y)
        {
            ulong X1 = X;
            S = X + Y;
            CY = (S<X1) ? 1UL : 0UL;
        }
        public static void __add_carry_in_out(ref ulong S, ref ulong CY, ulong X, ulong Y, ulong CI)
        {
            ulong X1;
            X1 = X + CI;
            S = X1 + Y;
            CY = ((S<X1) || (X1<CI)) ? 1UL : 0UL;
        }

        private static void __mul_64x256_to_320(ref ulong[] P, ulong A, ref ulong[] B)
        {
            ulong[] lP0 = new ulong[2];
            ulong[] lP1 = new ulong[2];
            ulong[] lP2 = new ulong[2];
            ulong[] lP3 = new ulong[2];
            ulong lC = 0; // Will be overridden

            __mul_64x64_to_128(ref lP0, A, (B)[0]);
            __mul_64x64_to_128(ref lP1, A, (B)[1]);
            __mul_64x64_to_128(ref lP2, A, (B)[2]);
            __mul_64x64_to_128(ref lP3, A, (B)[3]);
            (P)[0] = lP0[0];
            __add_carry_out(ref (P)[1], ref lC, lP1[0], lP0[1]);
            __add_carry_in_out(ref (P)[2], ref lC, lP2[0], lP1[1], lC);
            __add_carry_in_out(ref (P)[3], ref lC, lP3[0], lP2[1], lC);
            (P)[4] = lP3[1] + lC;
        }

    // Shift 4-part 2^196 * x3 + 2^128 * x2 + 2^64 * x1 + x0
    // right by "c" bits (must have c < 64)
    private static void srl256_short(ref ulong[] z, int c)
        {
            (z[2]) = ((z[3]) << (64 - (c))) + ((z[2]) >> (c));
            (z[3]) = ((z[4]) << (64 - (c))) + ((z[3]) >> (c));
            (z[4]) = ((z[5]) << (64 - (c))) + ((z[4]) >> (c));
            (z[5]) = (z[5]) >> (c);
        }

        // Compare "<" two 2-part unsigned integers
        private static bool lt128(ulong x_hi, ulong x_lo, ulong y_hi, ulong y_lo)
        {
            return (((x_hi) < (y_hi)) || (((x_hi) == (y_hi)) && ((x_lo) < (y_lo))));
        }

        // Likewise "<="
        private static bool le128(ulong x_hi, ulong x_lo, ulong y_hi, ulong y_lo)
        {
            return (((x_hi) < (y_hi)) || (((x_hi) == (y_hi)) && ((x_lo) <= (y_lo))));
        }

        private static float return_binary32(int s, int e, ulong c)
        {
           uint i = ((uint)s << 31) + ((uint)e << 23) + (uint)c;
           return BitConverter.UInt32BitsToSingle(i);
        }

public static float Decimal32ToSingle(Decimal32 x)
        {
            ulong c_prov;
            ulong[] c = new ulong[2]; // array containing 2 ulongs at 128 bits total (TODO we could use a UInt128)
            ulong[] m_min = new ulong[2]; // array containing 2 ulongs at 128 bits total (TODO we could use a UInt128)
            int s, e, k, e_out;
            ulong[] r = new ulong[4]; // array containing 4 ulongs at 256 bits total
            ulong[] z = new ulong[6]; // array containing 6 ulongs for 384 bits total

            /*#if DECIMAL_CALL_BY_REFERENCE
            #if !DECIMAL_GLOBAL_ROUNDING
              _IDEC_round rnd_mode = *prnd_mode;
            #endif
            #endif*/


            // Unpack decimal floating-point number x into sign,exponent,coefficient
            // In special cases, call the macros provided
            // Coefficient is normalized in the binary sense with postcorrection k,
            // so that x = 10^e * c / 2^k and the range of c is:
            //
            // 2^23 <= c < 2^24   (decimal32)
            // 2^53 <= c < 2^54   (decimal64)
            // 2^112 <= c < 2^113 (decimal128)

            unpack_bid32(x, out s, out e, out k, ref c[1]); // TODO finish implementing this function and have it return 0, inf, or NaN in those edge cases

            // Correct to 2^112 <= c < 2^113 with corresponding exponent adding 113-24=89
            // Thus a shift of 25 given that we've already upacked in c[1]

            c[1] = c[1] << 25;
            c[0] = 0;
            k += 89;

            // Check for "trivial" overflow, when 10^e * 1 > 2^{sci_emax+1}, just to
            // keep tables smaller (it would be intercepted later otherwise).
            //
            // (Note that we may have normalized the coefficient, but we have a
            //  corresponding exponent postcorrection to account for; this can
            //  afford to be conservative anyway.)
            //
            // We actually check if e >= ceil((sci_emax + 1) * log_10(2))
            // which in this case is e >= ceil(128 * log_10(2)) = 39

            if (e >= 39)
            {
                //__set_status_flags(pfpsf, BID_OVERFLOW_INEXACT_EXCEPTION);
                return (s == 1) ? float.NegativeInfinity : float.PositiveInfinity;
            }
            // Also check for "trivial" underflow, when 10^e * 2^113 <= 2^emin * 1/4,
            // so test e <= floor((emin - 115) * log_10(2))
            // In this case just fix ourselves at that value for uniformity.
            //
            // This is important not only to keep the tables small but to maintain the
            // testing of the round/sticky words as a correct rounding method

            if (e <= -80)
                e = -80;

            // Look up the breakpoint and approximate exponent

            m_min = (bid_breakpoints_binary32 + 80)[e];
            e_out = (bid_exponents_binary32 + 80)[e] - k;

            // Choose provisional exponent and reciprocal multiplier based on breakpoint

            if (c[1] <= m_min[1])
            {
                r = (bid_multipliers1_binary32 + 80)[e];
            }
            else
            {
                r = (bid_multipliers2_binary32 + 80)[e];
                e_out++;
            }

            // Do the reciprocal multiplication

            __mul_64x256_to_320(ref z, c[1], ref r);
            z[5] = z[4]; z[4] = z[3]; z[3] = z[2]; z[2] = z[1]; z[1] = z[0]; z[0] = 0;

            // Check for exponent underflow and compensate by shifting the product
            // Cut off the process at precision+2, since we can't really shift further
            if (e_out < 1)
            {
                int d;
                d = 1 - e_out;
                if (d > 26)
                    d = 26;
                e_out = 1;
                srl256_short(ref z, d);
            }
            c_prov = z[5];

            // Round using round-sticky words
            // If we spill into the next binade, correct
            // Flag underflow where it may be needed even for |result| = SNN

            if (lt128
                (bid_roundbound_128[/*(rnd_mode << 2) rnd_mode should always be 0*/ + ((s & 1) << 1) + (c_prov & 1)].
                 w[1],
                 bid_roundbound_128[((s & 1) << 1) +
                                (c_prov & 1)][0], z[4], z[3]))
            {
                c_prov++;
                if (c_prov == (1UL << 24))
                {
                    c_prov = 1UL << 23;
                    e_out++;
                }
            }
            // Check for overflow

            if (e_out >= 255)
            {
                //__set_status_flags(pfpsf, BID_OVERFLOW_INEXACT_EXCEPTION);
                // the original implementation calls return_binary32_ovf,
                // which sometimes returns inf sometimes returns max based on rounding mode.
                return (s == 1) ?  float.NegativeInfinity : float.PositiveInfinity;
            }
            // Modify exponent for a tiny result, otherwise lop the implicit bit

            if (c_prov < (1UL << 23))
            {
                e_out = 0;
            }
            else
            {
                c_prov &= ((1UL << 23) - 1);
            }

            // Set the inexact and underflow flag as appropriate (tiny after rounding)
/*
            if ((z[4] != 0) || (z[3] != 0))
            {
                __set_status_flags(pfpsf, BID_INEXACT_EXCEPTION);
                if (e_out == 0)
                    __set_status_flags(pfpsf, BID_UNDERFLOW_EXCEPTION);
            }*/
            // Package up the result as a binary floating-point number


            return return_binary32(s, e_out, c_prov);
        }
    }
}
