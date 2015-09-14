// Copyright (c) Hin-Tak Leung

// All rights reserved.

// MIT License

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the ""Software""), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;

using Compat.Win32APIs.NLStoUnicodeTable;
using Compat.Win32APIs.DBCSLeadBytes;

namespace Compat.Win32APIs
{
    public class CPInfo
    {
        public uint MaxCharSize;
        public int DefaultChar;
        public int UnicodeDefaultChar;
        public string name;

        public CPInfo(uint iMaxCharSize, int iDefaultChar, int iUnicodeDefaultChar, string iname) {
            MaxCharSize = iMaxCharSize;
            DefaultChar = iDefaultChar;
            UnicodeDefaultChar = iUnicodeDefaultChar;
            name = iname;
        }
    }
    public class CPInfoTable
    {
        private static readonly Dictionary<uint, CPInfo> CPInfoDict =
            new Dictionary<uint, CPInfo>
            {
                { 437, new CPInfo( 1, 0x003f, 0x003f, "OEM United States" ) },
                { 708, new CPInfo( 1, 0x003f, 0x003f, "ANSI/OEM Arabic; ASMO 708" ) },
                { 737, new CPInfo( 1, 0x003f, 0x003f, "OEM Greek 437G" ) },
                { 775, new CPInfo( 1, 0x003f, 0x003f, "OEM Baltic" ) },
                { 850, new CPInfo( 1, 0x003f, 0x003f, "OEM Multilingual Latin 1" ) },
                { 852, new CPInfo( 1, 0x003f, 0x003f, "OEM Slovak Latin 2" ) },
                { 855, new CPInfo( 1, 0x003f, 0x003f, "OEM Cyrillic" ) },
                { 857, new CPInfo( 1, 0x003f, 0x003f, "OEM Turkish" ) },
                { 860, new CPInfo( 1, 0x003f, 0x003f, "OEM Portuguese" ) },
                { 861, new CPInfo( 1, 0x003f, 0x003f, "OEM Icelandic" ) },
                { 862, new CPInfo( 1, 0x003f, 0x003f, "OEM Hebrew" ) },
                { 863, new CPInfo( 1, 0x003f, 0x003f, "OEM Canadian French" ) },
                { 864, new CPInfo( 1, 0x003f, 0x003f, "OEM Arabic" ) },
                { 865, new CPInfo( 1, 0x003f, 0x003f, "OEM Nordic" ) },
                { 866, new CPInfo( 1, 0x003f, 0x003f, "OEM Russian" ) },
                { 869, new CPInfo( 1, 0x003f, 0x003f, "OEM Greek" ) },
                { 874, new CPInfo( 1, 0x003f, 0x003f, "ANSI/OEM Thai" ) },
                { 932, new CPInfo( 2, 0x003f, 0x30fb, "ANSI/OEM Japanese Shift-JIS" ) },
                { 936, new CPInfo( 2, 0x003f, 0x003f, "ANSI/OEM Simplified Chinese GBK" ) },
                { 949, new CPInfo( 2, 0x003f, 0x003f, "ANSI/OEM Korean Unified Hangul" ) },
                { 950, new CPInfo( 2, 0x003f, 0x003f, "ANSI/OEM Traditional Chinese Big5" ) },
                { 1250, new CPInfo( 1, 0x003f, 0x003f, "ANSI Eastern Europe" ) },
                { 1251, new CPInfo( 1, 0x003f, 0x003f, "ANSI Cyrillic" ) },
                { 1252, new CPInfo( 1, 0x003f, 0x003f, "ANSI Latin 1" ) },
                { 1253, new CPInfo( 1, 0x003f, 0x003f, "ANSI Greek" ) },
                { 1254, new CPInfo( 1, 0x003f, 0x003f, "ANSI Turkish" ) },
                { 1255, new CPInfo( 1, 0x003f, 0x003f, "ANSI Hebrew" ) },
                { 1256, new CPInfo( 1, 0x003f, 0x003f, "ANSI Arabic" ) },
                { 1257, new CPInfo( 1, 0x003f, 0x003f, "ANSI Baltic" ) },
                { 1258, new CPInfo( 1, 0x003f, 0x003f, "ANSI/OEM Viet Nam" ) },
                { 1361, new CPInfo( 2, 0x003f, 0x003f, "Korean Johab" ) },
                { 10000, new CPInfo( 1, 0x003f, 0x003f, "Mac Roman" ) },
            };

        public CPInfo this[uint index] {
            get
            {
                return CPInfoDict[index];
            }
        }

        public int this[uint codepage, ushort bytestream] {
            get {
                try {
                    switch ( codepage ) {
                        case 437:  return new cp437()[bytestream];
                        case 708:  return new cp708()[bytestream];
                        case 737:  return new cp737()[bytestream];
                        case 775:  return new cp775()[bytestream];
                        case 850:  return new cp850()[bytestream];
                        case 852:  return new cp852()[bytestream];
                        case 855:  return new cp855()[bytestream];
                        case 857:  return new cp857()[bytestream];
                        case 860:  return new cp860()[bytestream];
                        case 861:  return new cp861()[bytestream];
                        case 862:  return new cp862()[bytestream];
                        case 863:  return new cp863()[bytestream];
                        case 864:  return new cp864()[bytestream];
                        case 865:  return new cp865()[bytestream];
                        case 866:  return new cp866()[bytestream];
                        case 869:  return new cp869()[bytestream];
                        case 874:  return new cp874()[bytestream];
                        case 932:  return new cp932()[bytestream];
                        case 936:
                            if (bytestream < 0xC000)
                                return new cp936_l0xC0()[bytestream];
                            else
                                return new cp936_u0xC0()[bytestream];
                        case 949:  return new cp949()[bytestream];
                        case 950:  return new cp950()[bytestream];
                        case 1250: return new cp1250()[bytestream];
                        case 1251: return new cp1251()[bytestream];
                        case 1252: return new cp1252()[bytestream];
                        case 1253: return new cp1253()[bytestream];
                        case 1254: return new cp1254()[bytestream];
                        case 1255: return new cp1255()[bytestream];
                        case 1256: return new cp1256()[bytestream];
                        case 1257: return new cp1257()[bytestream];
                        case 1258: return new cp1258()[bytestream];
                        case 1361: return new cp1361()[bytestream];
                        case 10000: return new cp10000()[bytestream];
                        default:
                            throw new NotImplementedException("UnImplemented CodePage Lookup:" + codepage);
                    }
                }
                catch (KeyNotFoundException ex)
                {
                    // Look up failing is okay - NLS does not map.
                    return -1;
                }
            }
        }

        public bool this[uint codepage, byte dbcs_byte] {
            get {
                switch ( codepage ) {
                    case 932:  return new dbcs932()[dbcs_byte];
                    case 936:  return new dbcs936()[dbcs_byte];
                    case 949:  return new dbcs949()[dbcs_byte];
                    case 950:  return new dbcs950()[dbcs_byte];
                    case 1361: return new dbcs1361()[dbcs_byte];
                    default:
                        throw new NotImplementedException("UnImplemented DBCS Lookup:" + codepage);
                }
            }
        }
    }
}
