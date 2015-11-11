using System;
using System.Diagnostics;




namespace OTFontFile
{
    // MBO means Motorola Byte Order (most significant byte stored in lowest memory address)
    public class MBOBuffer
    {
        /******************
         * constructors
         */
        
        
        public MBOBuffer()
        {
            m_filepos = -1; // -1 means not read from a file
            m_length = 0;
            m_nPadBytes = 0;
            m_buf = null;

            m_cachedChecksum = 0;
            m_bValidChecksumAvailable = false;
        }

        
        public MBOBuffer(uint length)
        {
            m_filepos = -1; // -1 means not read from a file
            m_length = length;
            m_nPadBytes = (uint)CalcPadBytes((int)length, 4);
            m_buf = new byte[m_length + m_nPadBytes];

            m_cachedChecksum = 0;
            m_bValidChecksumAvailable = false;
        }


        public MBOBuffer(uint filepos, uint length)
        {
            m_filepos = filepos;
            m_length = length;
            m_nPadBytes = (uint)CalcPadBytes((int)length, 4);
            m_buf = new byte[m_length + m_nPadBytes];

            m_cachedChecksum = 0;
            m_bValidChecksumAvailable = false;
        }


        /************************
         * public static methods
         */
        
        
        public static int CalcPadBytes(int nLength, int nByteAlignment)
        {
            int nPadBytes = 0;
            int nRemainderBytes = nLength % nByteAlignment;

            if (nRemainderBytes != 0)
            {
                nPadBytes = nByteAlignment - nRemainderBytes;
            }

            return nPadBytes;
        }


        // get a short from a buffer that is storing data in MBO
        public static short GetMBOshort(byte[] buf, uint offset)
        {
            return (short)(buf[offset]<<8 | buf[offset+1]);
        }


        public static ushort GetMBOushort(byte[] buf, uint offset)
        {
            return (ushort)(buf[offset]<<8 | buf[offset+1]);
        }


        public static int GetMBOint(byte[] buf, uint offset)
        {
            return buf[offset]<<24 | buf[offset+1]<<16 | buf[offset+2]<<8 | buf[offset+3];
        }


        public static uint GetMBOuint(byte[] buf, uint offset)
        {
            return (uint)(buf[offset]<<24 | buf[offset+1]<<16 | buf[offset+2]<<8 | buf[offset+3]);
        }

        public static bool BinaryEqual(MBOBuffer buf1, MBOBuffer buf2)
        {
            bool bEqual = true;

            if (buf1.GetLength() != buf2.GetLength())
            {
                bEqual = false;
            }
            else
            {
                byte [] b1 = buf1.GetBuffer();
                byte [] b2 = buf2.GetBuffer();
                for (int i=0; i<b1.Length; i++)
                {
                    if (b1[i] != b2[i])
                    {
                        bEqual = false;
                        break;
                    }
                }
            }

            return bEqual;
        }


        /************************
         * public methods
         */


        public byte[] GetBuffer()
        {
            return m_buf;
        }

        public uint GetLength()
        {
            return m_length;
        }

        public uint GetPaddedLength()
        {
            return m_length + m_nPadBytes;
        }


        // get/set

        public sbyte GetSbyte(uint offset)
        {
            return (sbyte)m_buf[offset];
        }

        public void SetSbyte(sbyte value, uint offset)
        {
            m_buf[offset] = (byte)value;
            
            m_bValidChecksumAvailable = false;
        }
        

        public byte GetByte(uint offset)
        {
                return m_buf[offset];
        }

        public void SetByte(byte value, uint offset)
        {
            m_buf[offset] = value;
            
            m_bValidChecksumAvailable = false;
        }
        

        public short GetShort(uint offset)
        {
            return (short)(m_buf[offset]<<8 | m_buf[offset+1]);
        }

        public void SetShort(short value, uint offset)
        {
            m_buf[offset  ] = (byte)(value >> 8);
            m_buf[offset+1] = (byte)value;

            m_bValidChecksumAvailable = false;
        }


        public ushort GetUshort(uint offset)
        {
            return (ushort)(m_buf[offset]<<8 | m_buf[offset+1]);
        }

        public void SetUshort(ushort value, uint offset)
        {
            m_buf[offset  ] = (byte)(value >> 8);
            m_buf[offset+1] = (byte)value;

            m_bValidChecksumAvailable = false;
        }


        public int GetInt(uint offset)
        {
            return m_buf[offset]<<24 | m_buf[offset+1]<<16 | m_buf[offset+2]<<8 | m_buf[offset+3];
        }

        public void SetInt(int value, uint offset)
        {
            m_buf[offset  ] = (byte)(value >> 24);
            m_buf[offset+1] = (byte)(value >> 16);
            m_buf[offset+2] = (byte)(value >> 8);
            m_buf[offset+3] = (byte)value;

            m_bValidChecksumAvailable = false;
        }


        public uint GetUint24( uint offset )
        {
            return ( uint )
                (m_buf[offset]<<16 | m_buf[offset+1] << 8 | m_buf[offset+2]);
        }

        public uint GetUint(uint offset)
        {
            return (uint)(m_buf[offset]<<24 | m_buf[offset+1]<<16 | m_buf[offset+2]<<8 | m_buf[offset+3]);
        }

        public void SetUint(uint value, uint offset)
        {
            m_buf[offset  ] = (byte)(value >> 24);
            m_buf[offset+1] = (byte)(value >> 16);
            m_buf[offset+2] = (byte)(value >> 8);
            m_buf[offset+3] = (byte)value;

            m_bValidChecksumAvailable = false;
        }

        public long GetLong(uint offset)
        {
            return (long)(
                (ulong)m_buf[offset  ] << 56 |
                (ulong)m_buf[offset+1] << 48 |
                (ulong)m_buf[offset+2] << 40 |
                (ulong)m_buf[offset+3] << 32 |
                (ulong)m_buf[offset+4] << 24 |
                (ulong)m_buf[offset+5] << 16 |
                (ulong)m_buf[offset+6] <<  8 |
                (ulong)m_buf[offset+7]);

        }

        public void SetLong(long value, uint offset)
        {
            m_buf[offset  ] = (byte)(value >> 56);
            m_buf[offset+1] = (byte)(value >> 48);
            m_buf[offset+2] = (byte)(value >> 40);
            m_buf[offset+3] = (byte)(value >> 32);
            m_buf[offset+4] = (byte)(value >> 24);
            m_buf[offset+5] = (byte)(value >> 16);
            m_buf[offset+6] = (byte)(value >> 8);
            m_buf[offset+7] = (byte)value;

            m_bValidChecksumAvailable = false;
        }


        public ulong GetUlong(uint offset)
        {
            return 
                (ulong)m_buf[offset  ] << 56 |
                (ulong)m_buf[offset+1] << 48 |
                (ulong)m_buf[offset+2] << 40 |
                (ulong)m_buf[offset+3] << 32 |
                (ulong)m_buf[offset+4] << 24 |
                (ulong)m_buf[offset+5] << 16 |
                (ulong)m_buf[offset+6] <<  8 |
                (ulong)m_buf[offset+7];
        }

        public void SetUlong(ulong value, uint offset)
        {
            m_buf[offset  ] = (byte)(value >> 56);
            m_buf[offset+1] = (byte)(value >> 48);
            m_buf[offset+2] = (byte)(value >> 40);
            m_buf[offset+3] = (byte)(value >> 32);
            m_buf[offset+4] = (byte)(value >> 24);
            m_buf[offset+5] = (byte)(value >> 16);
            m_buf[offset+6] = (byte)(value >> 8);
            m_buf[offset+7] = (byte)value;

            m_bValidChecksumAvailable = false;
        }


        public OTFixed GetFixed(uint offset)
        {
            OTFixed f;
            f.mantissa = GetShort(offset);
            f.fraction = GetUshort(offset+2);
            return f;
        }

        public void SetFixed(OTFixed value, uint offset)
        {
            uint n = value.GetUint();
            SetUint(n, offset);
        }

        public OTF2Dot14 GetF2Dot14(uint offset)
        {
            return new OTF2Dot14(this.GetShort(offset));
        }

        public OTTag GetTag(uint offset)
        {
            OTTag t = new OTTag(GetBuffer(), offset);
            return t;
        }

        public void SetTag(OTTag tag, uint offset)
        {
            byte [] buf = tag.GetBytes();
            for (int i=0; i<4; i++) m_buf[offset+i] = buf[i];
            m_bValidChecksumAvailable = false;
        }





        public long GetFilePos()
        {
            return m_filepos;
        }

        public uint CalcChecksum()
        {
            if (!m_bValidChecksumAvailable)
            {
                m_cachedChecksum = CalculateChecksum();
                m_bValidChecksumAvailable = true;
            }
            return m_cachedChecksum;
        }



        /************************
         * private methods
         */


        private uint CalculateChecksum()
        {
            Debug.Assert(m_length != 0);

            uint sum = 0;

            uint nLongs = (m_length + 3) / 4;

            for (uint i = 0; i < nLongs; i++)
            {
                sum += GetUint(i*4);
            }

            return sum;
        }
        

        /************************
         * member data
         */


        long m_filepos; // file position from which this buffer was read, -1 if not from file
        uint m_length; // number of data bytes
        uint m_nPadBytes; // number of padding bytes on the end
        byte[] m_buf;

        uint m_cachedChecksum;
        bool m_bValidChecksumAvailable;
    }
}
