// -----------------------------------------------------------------------
//
// Copyright (C) 1995-2004 Jean-loup Gailly and Mark Adler
//
//   The ZLIB software is provided 'as-is', without any express or implied
//   warranty.  In no event will the authors be held liable for any damages
//   arising from the use of this software.
//
//   Permission is granted to anyone to use this software for any purpose,
//   including commercial applications, and to alter it and redistribute it
//   freely, subject to the following restrictions:
//
//   1. The origin of this software must not be misrepresented; you must not
//      claim that you wrote the original software. If you use this software
//      in a product, an acknowledgment in the product documentation would be
//      appreciated but is not required.
//   2. Altered source versions must be plainly marked as such, and must not be
//      misrepresented as being the original software.
//   3. This notice may not be removed or altered from any source distribution.
//
//   Jean-loup Gailly jloup@gzip.org
//   Mark Adler madler@alumni.caltech.edu
//
// -----------------------------------------------------------------------

// ------------------------------------------------------------------
//
// Copyright (c) 2009-2011 Dino Chiesa and Microsoft Corporation.
// All rights reserved.
//
// This code module is part of DotNetZip, a zipfile class library.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License.
// See the file License.txt for the license details.
// More info on: http://dotnetzip.codeplex.com
//


using System;

namespace Karaboss {

public sealed class Adler
{
    // largest prime smaller than 65536
    private static readonly uint BASE = 65521;
    // NMAX is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1) <= 2^32-1
    private static readonly int NMAX = 5552;

    public static uint Adler32(byte[] buf) {
        uint result = Adler32(0, null, 0, 0);
        result = Adler32(result, buf, 0, buf.Length);
        return result;
    }

    public static uint Adler32(uint adler, byte[] buf, int index, int len)
    {
        if (buf == null)
            return 1;

        uint s1 = (uint) (adler & 0xffff);
        uint s2 = (uint) ((adler >> 16) & 0xffff);

        while (len > 0)
        {
            int k = len < NMAX ? len : NMAX;
            len -= k;
            while (k >= 16)
            {
                //s1 += (buf[index++] & 0xff); s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                s1 += buf[index++]; s2 += s1;
                k -= 16;
            }
            if (k != 0)
            {
                do
                {
                    s1 += buf[index++];
                    s2 += s1;
                }
                while (--k != 0);
            }
            s1 %= BASE;
            s2 %= BASE;
        }
        return (uint)((s2 << 16) | s1);
    }
}


}
