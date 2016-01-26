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
using System.Text;
using System.Collections;

using Compat;

namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_CFF.
    /// </summary>
    public class Table_CFF : OTTable
    {
        /************************
         * constructors
         */
        
        public Table_CFF(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            major = 0,
            minor = 1,
            hdrSize = 2,
            offSize = 3
        }

        /************************
         * accessors
         */

        public byte major
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.major);}
        }

        public byte minor
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.minor);}
        }
        
        public byte hdrSize
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.hdrSize);}
        }

        public byte offSize
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.offSize);}
        }

        public INDEXData Name
        {
            get {
                if (m_Name == null)
                    m_Name = new INDEXData(hdrSize, m_bufTable);
                return m_Name;
            }
        }

        public INDEXData TopDICT
        {
            get {
                if (m_TopDICT == null)
                    m_TopDICT = new INDEXData(hdrSize + Name.size, m_bufTable);
                return m_TopDICT;
            }
        }

        public INDEXData String
        {
            get {
                if (m_String == null)
                    m_String = new INDEXData(hdrSize + Name.size + TopDICT.size, m_bufTable);
                return m_String;
            }
        }

        public INDEXData GlobalSubr
        {
            get {
                if (m_GlobalSubr == null)
                    m_GlobalSubr = new INDEXData(hdrSize + Name.size + TopDICT.size + String.size, m_bufTable);
                return m_GlobalSubr;
            }
        }

        public INDEXData GetINDEX(int offset)
        {
            return new INDEXData((uint) offset, m_bufTable);
        }

        public DICTData GetTopDICT( uint i )
        {
            return new DICTData( TopDICT.GetData(i), String );
        }

        public DICTData GetPrivate( DICTData thisTopDICT )
        {
            if (thisTopDICT.sizePrivate == 0)
                return null;

            byte [] buf = new byte[thisTopDICT.sizePrivate];
            System.Buffer.BlockCopy(m_bufTable.GetBuffer(), thisTopDICT.offsetPrivate, buf, 0, thisTopDICT.sizePrivate);
            return new DICTData( buf, String, true );
        }

        public DICTData GetDICT( byte[] data )
        {
            return new DICTData( data, String );
        }

        private static Encoding encoding = Encoding.GetEncoding("us-ascii",
                                                                new EncoderExceptionFallback(),
                                                                new DecoderExceptionFallback());

        public class INDEXData
        {
            public ushort count;
            byte offSize;
            uint lastOffset;
            public uint size;

            public INDEXData(uint offset_buf, MBOBuffer bufTable)
            {
                m_offset = offset_buf;
                m_bufTable = bufTable;
                count = m_bufTable.GetUshort(m_offset);
                size = 2;

                if ( count > 0 )
                {
                    offSize = m_bufTable.GetByte(m_offset + 2 );

                    lastOffset = GetOffset( count );

                    size += (uint) offSize * ( (uint) count + 1 )
                        + lastOffset;
                }
            }

            public uint GetOffset(uint i)
            {
                if ( count == 0 )
                    return 1;

                uint offset = 2 + 1 + (uint) offSize * ( i );
                uint val = 0;
                switch(offSize)
                {
                    case 1:
                        val = m_bufTable.GetByte(m_offset + offset);
                        break;
                    case 2:
                        val = m_bufTable.GetUshort(m_offset + offset);
                        break;
                    case 3:
                        val = m_bufTable.GetUint24(m_offset + offset);
                        break;
                    case 4:
                        val = m_bufTable.GetUint(m_offset + offset);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Illegal offSize:0x"
                                                              + offSize.ToString("x2") );
                }
                return val;
            }

            public byte[] GetData(uint i)
            {
                if (i >= count)
                    return null;

                uint length = GetOffset(i + 1) - GetOffset(i);
                byte [] buf = new byte[length];
                uint offset = m_offset + 2 + (uint) offSize * ( (uint) count + 1 ) + GetOffset(i);
                System.Buffer.BlockCopy(m_bufTable.GetBuffer(), (int)offset, buf, 0, (int)length);
                return buf;
            }

            public string GetString(uint i)
            {
                return encoding.GetString(GetData(i));
            }

            public string GetUTFString(uint i)
            {
                return System.Text.UTF8Encoding.UTF8.GetString(GetData(i));
            }

            public string StringForID(ushort sid)
            {
                return StringForID(sid, false);
            }

            public string StringForID(ushort sid, bool allowUTF8)
            {
                if ( sid < CFF.StdStrings.Length )
                {
                    return CFF.StdStrings[sid];
                }
                else
                {
                    if ( !allowUTF8 )
                        return GetString( (uint) (sid - CFF.StdStrings.Length) );
                    else
                        return GetUTFString( (uint) (sid - CFF.StdStrings.Length) );
                }
            }

            public string StringForID(int sid)
            {
                return StringForID((ushort) sid);
            }

            public string StringForID(int sid, bool allowUTF8)
            {
                return StringForID((ushort) sid, allowUTF8);
            }

            public uint begin
            {
                get {
                    return m_offset;
                }
            }

            public uint end
            {
                get {
                    return m_offset + size;
                }
            }

            uint m_offset;
            MBOBuffer m_bufTable;
        }

        public class DICTData
        {
            private INDEXData m_String;

            public int offsetCharStrings;
            public int offsetPrivate;
            public int sizePrivate;
            public int offsetFDArray;

            public int offsetCharset;
            public int offsetEncoding;

            public string ROS;
            public int offsetFDSelect;

            public int Subrs;

            private int sidFullName;
            private int sidFontName;

            public DICTData( byte[] data, INDEXData String ): this(data, String, false )
            {
            }

            public DICTData( byte[] data, INDEXData String, bool isPrivate )
            {
                uint cursor = 0;
                Stack operandStack = new Stack();
                m_String = String;
                ROS = null;

                while( cursor < data.Length )
                {
                    uint advance = 1;
                    if ( data[cursor] >= 32 && data[cursor] <= 246 )
                    {
                        operandStack.Push( (data[cursor]-139) );
                    }
                    else if ( data[cursor] >= 247 && data[cursor] <= 250 )
                    {
                        operandStack.Push( ((data[cursor]-247)*256+data[cursor+1]+108) );
                        advance = 2;
                    }
                    else if ( data[cursor] >= 251 && data[cursor] <= 254 )
                    {
                        operandStack.Push( (-(data[cursor]-251)*256-data[cursor+1]-108) );
                        advance = 2;
                    }
                    else if ( data[cursor] == 28 )
                    {
                        operandStack.Push( (data[cursor+1]<<8|data[cursor+2]) );
                        advance = 3;
                    }
                    else if ( data[cursor] == 29 )
                    {
                        operandStack.Push( (data[cursor+1]<<24|data[cursor+2]<<16|data[cursor+3]<<8|data[cursor+4]) );
                        advance = 5;
                    }
                    else if ( data[cursor] == 30 )
                    {
                        string realstr = "";
                        while ( (data[cursor+advance] & 0x0f) != 0x0f )
                        {
                            int[] x = { data[cursor+advance] >> 4, data[cursor+advance] & 0x0f };
                            for (int i = 0; i < 2 ; i++)
                            {
                                switch(x[i])
                                {
                                    case 10:
                                        realstr += ".";
                                        break;
                                    case 11:
                                        realstr += "E";
                                        break;
                                    case 12:
                                        realstr += "E-";
                                        break;
                                    case 14:
                                        realstr += "-";
                                        break;
                                    case 15:
                                        /* do nothing */
                                        break;
                                    case 13:
                                        throw new ArgumentOutOfRangeException("Invalid Nibble encountered at pos "
                                                                              + cursor );
                                        break;
                                    default:
                                        /* 0 - 9, hopefully! */
                                        realstr += x[i].ToString("d1");
                                        break;
                                }
                            }
                            advance ++;
                        }
                        /* the last half byte */
                        int y = data[cursor+advance] >> 4 ;
                        switch(y)
                        {
                            case 10:
                                realstr += ".";
                                break;
                            case 11:
                                realstr += "E";
                                break;
                            case 12:
                                realstr += "E-";
                                break;
                            case 14:
                                realstr += "-";
                                break;
                            case 15:
                                /* do nothing */
                                break;
                            case 13:
                                throw new ArgumentOutOfRangeException("Invalid Nibble encountered at pos "
                                                                      + cursor );
                                break;
                            default:
                                /* 0 - 9, hopefully! */
                                realstr += y.ToString("d1");
                                break;
                        }
                        operandStack.Push(realstr);
                        advance ++;
                    }
                    else if ( !isPrivate && data[cursor] >= 0 && data[cursor] <= 21 )
                    {
                        string op = "";
                        switch(data[cursor])
                        {
                            case 0x00:
                                int sidversion = (int) operandStack.Pop();
                                op = "version";
                                break;
                            case 0x01:
                                int sidNotice = (int) operandStack.Pop();
                                op = "Notice";
                                break;
                            case 0x02:
                                sidFullName = (int) operandStack.Pop();
                                op = "FullName";
                                break;
                            case 0x03:
                                int sidFamilyName = (int) operandStack.Pop();
                                op = "FamilyName";
                                break;
                            case 0x04:
                                int sidWeight = (int) operandStack.Pop();
                                op = "Weight";
                                break;
                            case 0x05:
                                operandStack.Pop();
                                operandStack.Pop();
                                operandStack.Pop();
                                operandStack.Pop();
                                op = "FontBBox";
                                break;
                            case 0x0d:
                                operandStack.Pop();
                                op = "UniqueID";
                                break;
                            case 0x0e:
                                operandStack.Clear();
                                op = "XUID";
                                break;
                            case 0x0f:
                                offsetCharset = (int) operandStack.Pop();
                                op = "charset";
                                break;
                            case 0x10:
                                offsetEncoding = (int) operandStack.Pop();
                                op = "Encoding";
                                break;
                            case 0x11:
                                offsetCharStrings = (int) operandStack.Pop();
                                op = "CharStrings";
                                break;
                            case 0x12:
                                offsetPrivate = (int) operandStack.Pop();
                                sizePrivate   = (int) operandStack.Pop();
                                op = "Private";
                                break;
                            case 0x0c:
                                switch(data[cursor+1])
                                {
                                    case 0x00:
                                        int sidCopyright = (int) operandStack.Pop();
                                        op = "Copyright";
                                        break;
                                    case 0x01:
                                        operandStack.Pop();
                                        op = "isFixedPitch";
                                        break;
                                    case 0x02:
                                        operandStack.Pop();
                                        op = "ItalicAngle";
                                        break;
                                    case 0x03:
                                        operandStack.Pop();
                                        op = "UnderlinePosition";
                                        break;
                                    case 0x04:
                                        operandStack.Pop();
                                        op = "UnderlineThickness";
                                        break;
                                    case 0x05:
                                        operandStack.Pop();
                                        op = "PaintType";
                                        break;
                                    case 0x06:
                                        int CharstringType = (int) operandStack.Pop();
                                        if ( CharstringType != 2 )
                                            throw new ArgumentOutOfRangeException("Invalid CharstringType:" + CharstringType );
                                        op = "CharstringType";
                                        break;
                                    case 0x07:
                                        operandStack.Clear();
                                        op = "FontMatrix";
                                        break;
                                    case 0x08:
                                        operandStack.Pop();
                                        op = "StrokeWidth";
                                        break;
                                    case 0x14:
                                        operandStack.Pop();
                                        op = "SyntheticBase";
                                        break;
                                    case 0x15:
                                        operandStack.Pop();
                                        op = "PostScript";
                                        break;
                                    case 0x16:
                                        operandStack.Pop();
                                        op = "BaseFontName";
                                        break;
                                    case 0x17:
                                        operandStack.Clear();
                                        op = "BaseFontBlend";
                                        break;
                                    case 0x1e:
                                        op = "ROS";
                                        int supplement  = (int) operandStack.Pop();
                                        int sidOrdering = (int) operandStack.Pop();
                                        int sidRegistry = (int) operandStack.Pop();
                                        ROS = m_String.StringForID(sidRegistry) + " " + m_String.StringForID(sidOrdering) + " " + supplement;
                                        break;
                                    case 0x1f:
                                        object oCIDFontVersion = operandStack.Pop();
                                        op = "CIDFontVersion";
                                        break;
                                    case 0x20:
                                        operandStack.Pop();
                                        op = "CIDFontRevision";
                                        break;
                                    case 0x21:
                                        operandStack.Pop();
                                        op = "CIDFontType";
                                        break;
                                    case 0x22:
                                        operandStack.Pop();
                                        op = "CIDCount";
                                        break;
                                    case 0x23:
                                        operandStack.Pop();
                                        op = "UIDBase";
                                        break;
                                    case 0x24:
                                        offsetFDArray  = (int) operandStack.Pop();
                                        op = "FDArray";
                                        break;
                                    case 0x25:
                                        offsetFDSelect = (int) operandStack.Pop();
                                        op = "FDSelect";
                                        break;
                                    case 0x26:
                                        sidFontName = (int) operandStack.Pop();
                                        op = "FontName";
                                        break;
                                    default:
                                        operandStack.Clear();
                                        op = "<2-byte>+0x" + data[cursor+1].ToString("x2");
                                        throw new ArgumentOutOfRangeException("Invalid <2-byte> op:"
                                                                              + data[cursor+1].ToString("x2") + " at pos " + cursor );
                                }
                                advance = 2;
                                break;

                            default:
                                operandStack.Clear();
                                op = "0x" + data[cursor].ToString("x2");
                                throw new ArgumentOutOfRangeException("Invalid op:"
                                                                      + data[cursor].ToString("x2") + " at pos " + cursor );
                        }
                    }
                    else if ( isPrivate && data[cursor] >= 0 && data[cursor] <= 21 )
                    {
                        string op = "";
                        switch(data[cursor])
                        {
                            case 0x06:
                                operandStack.Clear();
                                op = "BlueValues";
                                break;
                            case 0x07:
                                operandStack.Clear();
                                op = "OtherBlues";
                                break;
                            case 0x08:
                                operandStack.Clear();
                                op = "FamilyBlues";
                                break;
                            case 0x09:
                                operandStack.Clear();
                                op = "FamilyOtherBlues";
                                break;
                            case 0x0a:
                                operandStack.Pop();
                                op = "StdHW";
                                break;
                            case 0x0b:
                                operandStack.Pop();
                                op = "StdVW";
                                break;
                            case 0x13:
                                Subrs = (int) operandStack.Pop();
                                op = "Subrs";
                                break;
                            case 0x14:
                                operandStack.Pop();
                                op = "defaultWidthX";
                                break;
                            case 0x15:
                                operandStack.Pop();
                                op = "nominalWidthX";
                                break;
                            case 0x0c:
                                switch(data[cursor+1])
                                {
                                    case 0x09:
                                        operandStack.Pop();
                                        op = "BlueScale";
                                        break;
                                    case 0x0a:
                                        operandStack.Pop();
                                        op = "BlueShift";
                                        break;
                                    case 0x0b:
                                        operandStack.Pop();
                                        op = "BlueFuzz";
                                        break;
                                    case 0x0c:
                                        operandStack.Clear();
                                        op = "StemSnapH";
                                        break;
                                    case 0x0d:
                                        operandStack.Clear();
                                        op = "StemSnapV";
                                        break;
                                    case 0x0e:
                                        operandStack.Pop();
                                        op = "ForceBold";
                                        break;
                                    case 0x11:
                                        operandStack.Pop();
                                        op = "LanguageGroup";
                                        break;
                                    case 0x12:
                                        operandStack.Pop();
                                        op = "ExpansionFactor";
                                        break;
                                    case 0x13:
                                        operandStack.Pop();
                                        op = "initialRandomSeed";
                                        break;
                                    default:
                                        operandStack.Clear();
                                        op = "<2-byte>+0x" + data[cursor+1].ToString("x2");
                                        throw new ArgumentOutOfRangeException("Invalid <2-byte> op:"
                                                                              + data[cursor+1].ToString("x2") + " at pos " + cursor );
                                }
                                advance = 2;
                                break;
                            default:
                                operandStack.Clear();
                                op = "0x" + data[cursor].ToString("x2");
                                throw new ArgumentOutOfRangeException("Invalid op:"
                                                                      + data[cursor].ToString("x2") + " at pos " + cursor );
                        }
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Reserved Byte encountered:"
                                                              + data[cursor].ToString("x2") + " at pos " + cursor );
                    }
                    cursor += advance;
                }
                if (cursor != data.Length)
                    throw new ArgumentOutOfRangeException( "operand stack ends unexpectedly, cursor not equal to data length:"
                                                           + cursor + "!=" + data.Length );
                if (operandStack.Count != 0)
                    throw new ArgumentOutOfRangeException( "operand stack not empty:"
                                                           + operandStack.Count );
            }

            public string FullName
            {
                get {
                    return m_String.StringForID(sidFullName);
                }
            }

            public string FontName
            {
                get {
                    return m_String.StringForID(sidFontName);
                }
            }
        }

        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new CFF_cache();
            }

            return m_cache;
        }
        
        public class CFF_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }

        private INDEXData m_Name;
        private INDEXData m_TopDICT;
        private INDEXData m_String;
        private INDEXData m_GlobalSubr;
    }
}
