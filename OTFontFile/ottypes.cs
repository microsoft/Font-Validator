using System;
using System.Collections;
using System.Diagnostics;

namespace OTFontFile
{
    public struct BigUn
    {
        uint m_char32;

        BigUn(char c)
        {
            m_char32 = c;
        }

        BigUn(uint char32)
        {
            m_char32 = char32;
        }

        BigUn(char SurrogateHigh, char SurrogateLow)
        {
            m_char32 = (uint)SurrogatePairToUnicodeScalar(SurrogateHigh, SurrogateLow);
        }

        public static bool IsHighSurrogate(char c)
        {
            return (c >= 0xd800 && c <= 0xdbff);
        }

        public static bool IsLowSurrogate(char c)
        {
            return (c >= 0xdc00 && c <= 0xdfff);
        }

        public static BigUn SurrogatePairToUnicodeScalar(char SurrogateHigh, char SurrogateLow)
        {
            // validate parameters

            if (!IsHighSurrogate(SurrogateHigh))
            {
                throw new ArgumentOutOfRangeException("SurrogateHigh");
            }

            if (!IsLowSurrogate(SurrogateLow))
            {
                throw new ArgumentOutOfRangeException("SurrogateLow");
            }

            // calculate and return value

            uint retval =  ((uint)SurrogateHigh - 0xd800) * 0x0400 + ((uint)SurrogateLow - 0xdc00) + 0x10000;
            return (BigUn)retval;
        }

        static public explicit operator uint(BigUn char32)
        {            
            return char32.m_char32;
        }

        static public explicit operator BigUn(uint char32)
        {
            BigUn bu = new BigUn(char32);
            return bu;
        }

        static public bool operator < ( BigUn bg1, BigUn bg2 )
        {            
            return bg1.m_char32 < bg2.m_char32;
        }

        static public bool operator > ( BigUn bg1, BigUn bg2 )
        {            
            return bg1.m_char32 > bg2.m_char32;
        }

        static public bool operator == ( BigUn bg1, BigUn bg2 )
        {            
            return bg1.m_char32 == bg2.m_char32;
        }

        static public bool operator != ( BigUn bg1, BigUn bg2 )
        {            
            return bg1.m_char32 != bg2.m_char32;
        }

        public override bool Equals( Object o )
        {
            return this == (BigUn)o;
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }
    }

    public struct OTF2Dot14
    {
        private short valAsShort;

        public OTF2Dot14(short valAsShort)
        {
            this.valAsShort=valAsShort;
        }

        public short ValAsShort
        {
            get { return this.valAsShort; }
            set { this.valAsShort=value; }

        }

        public ushort Mantissa
        {
            get 
            {
                int sh=Math.Abs(this.valAsShort);
                int mantissa=(sh>>14);
                return (ushort)mantissa;
            }
        }
        public ushort Fraction
        {
            get 
            {
                int sh=Math.Abs(this.valAsShort);
                int fraction=sh&0x3fff;
                return (ushort)fraction;
            }
        }

        public static explicit operator double(OTF2Dot14 valAsShort)
        {
            return ((double)valAsShort.ValAsShort/(double)0x4000);
        }

        public override int GetHashCode()
        {
            return (int)(uint)this;
        }
        
        static public bool operator == (OTF2Dot14 f1, OTF2Dot14 f2)
        {
            if ((object)f1 == null && (object)f2 == null)
            {
                return true;
            }
            else if ((object)f1 == null || (object)f2 == null)
            {
                return false;
            }
            else
            {
                return (f1.valAsShort == f2.valAsShort);
            }
        }

        static public bool operator != (OTF2Dot14 f1, OTF2Dot14 f2)
        {
            return (!(f1 == f2));
        }

        public override bool Equals(object obj)
        {
            return (this == (OTF2Dot14)obj);
        }

    }

    public struct OTFixed
    {
        public short mantissa;
        public ushort fraction;

        public OTFixed(short Mantissa, ushort Fraction)
        {
            mantissa = Mantissa;
            fraction = Fraction;
        }

        public OTFixed(double fixValue)
        {
            mantissa = (short)Math.Round(fixValue, 0);
            fraction = (ushort)Math.Round((fixValue - mantissa) * 65536, 0);
        }

        public uint GetUint()
        {
            return (uint)(mantissa<<16 | fraction);
        }

        public double GetDouble()
        {
            return (double)mantissa + (double)fraction/65536.0;
        }

        public string GetHexString()
        {
            return "0x" + mantissa.ToString("X") + fraction.ToString("X");
        }

        public override string ToString()
        {
            double number = Math.Round(this.GetDouble(), 3);
            
            return number.ToString();
        }

        static public bool operator == (OTFixed f1, OTFixed f2)
        {
            if ((object)f1 == null && (object)f2 == null)
            {
                return true;
            }
            else if ((object)f1 == null || (object)f2 == null)
            {
                return false;
            }
            else
            {
                return (f1.GetUint() == f2.GetUint());
            }
        }

        static public bool operator != (OTFixed f1, OTFixed f2)
        {
            return (!(f1 == f2));
        }

        public override bool Equals(object obj)
        {
            return (this == (OTFixed)obj);
        }

        public override int GetHashCode()
        {
            return (int)this.GetUint();
        }
        
    }



    public class OTTag
    {
        /***************
         * constructors
         */

        public OTTag(byte[] tagbuf)
        {
            m_tag = new byte[4];
            for (int i=0; i<4; i++)
            {
                m_tag[i] = tagbuf[i];
            }
        }

        public OTTag(byte[] tagbuf, uint offset)
        {
            m_tag = new byte[4];
            for (int i=0; i<4; i++)
            {
                m_tag[i] = tagbuf[i+offset];
            }
        }

        /************************
         * operators
         */

        static public implicit operator byte[](OTTag tag)
        {
            return tag.GetBytes();
        }

        static public implicit operator OTTag(uint tagvalue) 
        {
            byte[] buf = new byte[4];

            buf[0] = (byte)((tagvalue&0xff000000) >> 24);
            buf[1] = (byte)((tagvalue&0x00ff0000) >> 16);
            buf[2] = (byte)((tagvalue&0x0000ff00) >> 8);
            buf[3] = (byte)(tagvalue&0x000000ff);

            return new OTTag(buf);
        }

        static public implicit operator uint(OTTag tag)
        {
            byte[] buf = tag.GetBytes();

            uint tagValue = 0;

            tagValue += (uint)(buf[0]<<24);
            tagValue += (uint)(buf[1]<<16);
            tagValue += (uint)(buf[2]<<8);
            tagValue += (uint)(buf[3]);

            return tagValue;
        }

        static public implicit operator OTTag(string value)
        {
            byte[] tagbuf = new byte[4];

            for (int i=0; i<4; i++)
            {
                tagbuf[i] = (byte)value[i];
            }

            return new OTTag(tagbuf);
        }

        static public implicit operator string (OTTag tag)
        {
            byte[] buf = tag.GetBytes();

            string s = "";

            for (int i=0; i<4; i++)
            {
                s += (char)buf[i];
            }

            return s;
        }

        static public bool operator == (OTTag t1, OTTag t2)
        {
            if ((object)t1 == null && (object)t2 == null)
            {
                return true;
            }
            else if ((object)t1 == null || (object)t2 == null)
            {
                return false;
            }
            else
            {
                uint u1 = t1;
                uint u2 = t2;
                return (u1 == u2);
            }
        }

        static public bool operator != (OTTag t1, OTTag t2)
        {
            return (!(t1 == t2));
        }


        /*****************
         * public methods
         */
        
        
        public override bool Equals(object obj)
        {
            return (this == (OTTag)obj);
        }

        public override int GetHashCode()
        {
            return (int)(uint)this;
        }
        
        public byte[] GetBytes()
        {
            return m_tag;
        }


        public bool IsValid()
        {
            bool bRet = true;


            for (int i=0; i<4; i++)
            {
                if (m_tag[i] < 32 || m_tag[i] > 126)
                {
                    bRet = false;
                    break;
                }
            }

            return bRet;
        }


        /**************
         * member data
         */
        byte[] m_tag;
    }

    public class DirectoryEntry
    {
        public DirectoryEntry()
        {
            m_buf = new MBOBuffer(16);
        }

        public DirectoryEntry(MBOBuffer buf)
        {
            Debug.Assert(buf.GetLength() == 16);
            m_buf = buf;
        }

        public enum FieldOffsets
        {
            tag      = 0,
            checkSum = 4,
            offset   = 8,
            length   = 12
        }

        public DirectoryEntry(DirectoryEntry obj)
        {
            if (m_buf == null)
                m_buf = new MBOBuffer(16);
            tag = new OTTag(obj.tag.GetBytes());
            checkSum = obj.checkSum;
            offset   = obj.offset;
            length   = obj.length;
        }

        public OTTag tag
        {
            get {return new OTTag(m_buf.GetBuffer());}
            set {m_buf.SetTag(value, (uint)FieldOffsets.tag);}
        }

        public uint checkSum
        {
            get {return m_buf.GetUint((uint)FieldOffsets.checkSum);}
            set {m_buf.SetUint(value, (uint)FieldOffsets.checkSum);}
        }

        public uint offset
        {
            get {return m_buf.GetUint((uint)FieldOffsets.offset);}
            set {m_buf.SetUint(value, (uint)FieldOffsets.offset);}
        }

        public uint length
        {
            get {return m_buf.GetUint((uint)FieldOffsets.length);}
            set {m_buf.SetUint(value, (uint)FieldOffsets.length);}
        }

        public MBOBuffer m_buf;
    }

    public class OffsetTable
    {
        // constructor
        public OffsetTable(MBOBuffer buf)
        {
            Debug.Assert(buf.GetLength() == 12);
            m_buf = buf;

            DirectoryEntries = new System.Collections.ArrayList();
        }

        public OffsetTable(OTFixed version, ushort nTables)
        {
            m_buf = new MBOBuffer(12);

            sfntVersion = version;
            numTables = nTables;

            if (nTables != 0)
            {
                // these values are truly undefined when numTables is zero
                // since there is no power of 2 that is less that or equal to zero
                searchRange   = (ushort)(util.MaxPower2LE(nTables) * 16);
                entrySelector = util.Log2(util.MaxPower2LE(nTables));
                rangeShift    = (ushort)(nTables*16 - searchRange);
            }

            DirectoryEntries = new System.Collections.ArrayList();
        }

        public enum FieldOffsets
        {
            sfntVersion   = 0,
            numTables     = 4,
            searchRange   = 6,
            entrySelector = 8,
            rangeShift    = 10
        }

        public uint CalcOffsetTableChecksum()
        {
            return m_buf.CalcChecksum();
        }

        public uint CalcDirectoryEntriesChecksum()
        {
            uint sum = 0;

            for (int i=0; i<DirectoryEntries.Count; i++)
            {
                DirectoryEntry de = (DirectoryEntry)DirectoryEntries[i];
                sum += de.tag + de.checkSum + de.offset + de.length;
            }

            return sum;
        }

        // accessors

        public    OTFixed sfntVersion
        {
            get {return m_buf.GetFixed((uint)FieldOffsets.sfntVersion);}
            set {m_buf.SetFixed(value, (uint)FieldOffsets.sfntVersion);}
        }

        public    ushort  numTables
        {
            get {return m_buf.GetUshort((uint)FieldOffsets.numTables);}
            set {m_buf.SetUshort(value, (uint)FieldOffsets.numTables);}
        }

        public    ushort  searchRange
        {
            get {return m_buf.GetUshort((uint)FieldOffsets.searchRange);}
            set {m_buf.SetUshort(value, (uint)FieldOffsets.searchRange);}
        }

        public    ushort    entrySelector
        {
            get {return m_buf.GetUshort((uint)FieldOffsets.entrySelector);}
            set {m_buf.SetUshort(value, (uint)FieldOffsets.entrySelector);}
        }

        public    ushort    rangeShift
        {
            get {return m_buf.GetUshort((uint)FieldOffsets.rangeShift);}
            set {m_buf.SetUshort(value, (uint)FieldOffsets.rangeShift);}
        }


        // member data

        public MBOBuffer m_buf;
        public System.Collections.ArrayList DirectoryEntries;
    }

}
