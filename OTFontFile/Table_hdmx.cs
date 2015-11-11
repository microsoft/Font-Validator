using System;
using System.Collections;
using System.Diagnostics;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_hdmx.
    /// </summary>
    public class Table_hdmx : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_hdmx(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
            m_numGlyphs = 0;
        }


        public Table_hdmx(OTTag tag, MBOBuffer buf, ushort nNumGlyphs ) : base(tag, buf)
        {
            m_numGlyphs = nNumGlyphs;
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            TableVersionNumber  = 0,
            NumberDeviceRecords = 2,
            SizeofDeviceRecord  = 4,
            DeviceRecord        = 8
        }

        /************************
         * DeviceRecord class
         */

        public class DeviceRecord
        {
            public DeviceRecord(uint offset, MBOBuffer bufTable, ushort numGlyphs)
            {
                m_offsetDeviceRecord = offset;
                m_bufTable = bufTable;
                m_numGlyphs = numGlyphs;
            }

            public enum FieldOffsets
            {
                PixelSize = 0,
                MaxWidth  = 1,
                Widths    = 2
            }

            public byte PixelSize
            {
                get {return m_bufTable.GetByte(m_offsetDeviceRecord + (uint)FieldOffsets.PixelSize);}
            }

            public byte MaxWidth
            {
                get {return m_bufTable.GetByte(m_offsetDeviceRecord + (uint)FieldOffsets.MaxWidth);}
            }

            public byte GetWidth(uint i)
            {
                if (i >= m_numGlyphs)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return m_bufTable.GetByte(m_offsetDeviceRecord + (uint)FieldOffsets.Widths + i);
            }

            public uint GetNumPadBytes()
            {
                uint nPadBytes = 0;
                uint unpaddedLength = (uint)m_numGlyphs + 2;
                if ((unpaddedLength & 3) != 0)
                {
                    nPadBytes = 4 - unpaddedLength % 4;
                }
                return nPadBytes;
            }

            public uint GetPadByte(uint i)
            {
                if (i >= GetNumPadBytes())
                {
                    throw new ArgumentOutOfRangeException();
                }

                uint offset = m_offsetDeviceRecord + (uint)FieldOffsets.Widths + m_numGlyphs + i;

                if (offset <= m_bufTable.GetLength())
                    return m_bufTable.GetByte(offset);
                else
                    return 1;
            }

            public ushort numGlyphs
            {
                get
                {
                    return m_numGlyphs;
                }
            }

            uint m_offsetDeviceRecord;
            MBOBuffer m_bufTable;
            ushort m_numGlyphs;
        }


        /************************
         * accessors
         */


        public ushort TableVersionNumber
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.TableVersionNumber);}
        }

        public short NumberDeviceRecords
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.NumberDeviceRecords);}
        }

        public int SizeofDeviceRecord
        {
            get {return m_bufTable.GetInt((uint)FieldOffsets.SizeofDeviceRecord);}
        }

        // NOTE: We will want remove the set here once the new constructor
        // that includes numGlyph is being used
        public ushort numGlyphs
        {
            get
            {
                return m_numGlyphs;
            }
            set
            {
                m_numGlyphs = value;
            }
        }

        public DeviceRecord GetDeviceRecord(uint i, ushort numGlyphs)
        {
            DeviceRecord dr = null;

            if (i < NumberDeviceRecords)
            {
                uint offset = (uint)FieldOffsets.DeviceRecord + i*(uint)SizeofDeviceRecord;
                dr = new DeviceRecord(offset, m_bufTable, numGlyphs);
            }

            return dr;
        }

        /************************
         * protected methods
         */

        protected ushort GetNumGlyphs(OTFont fontOwner)
        {
            if (m_numGlyphs == 0)
            {
                Table_maxp maxpTable = (Table_maxp)fontOwner.GetTable("maxp");

                if (maxpTable != null)
                {
                    m_numGlyphs = maxpTable.NumGlyphs;
                }
            }

            Debug.Assert(m_numGlyphs != 0, "m_numGlyphs == 0");
            return m_numGlyphs;
        }



        /************************
         * member data
         */
    
        protected ushort m_numGlyphs;


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new hdmx_cache( this );
            }

            return m_cache;
        }
        
        public class hdmx_cache : DataCache
        {
            protected ushort m_TableVersionNumber;
            protected short m_NumberDeviceRecords;
            protected ArrayList m_DeviceRecords; // DeviceRecordCache[]

            protected ushort m_NumGlyphs;
            

            // constructor
            public hdmx_cache(Table_hdmx OwnerTable )
            {
                m_TableVersionNumber = OwnerTable.TableVersionNumber;
                m_NumberDeviceRecords = OwnerTable.NumberDeviceRecords;

                m_NumGlyphs = OwnerTable.numGlyphs;

                // put all of the device records in an ArrayList
                m_DeviceRecords = new ArrayList( m_NumberDeviceRecords );
                for( short i = 0; i < m_NumberDeviceRecords; i++ )
                {
                    DeviceRecord dr = OwnerTable.GetDeviceRecord( (uint)i, m_NumGlyphs );
                    DeviceRecordCache drc = new DeviceRecordCache( dr );
                    m_DeviceRecords.Add( drc );
                }
            }

            public ushort TableVersionNumber
            {
                get
                {
                    return m_TableVersionNumber;
                }
                set
                {
                    m_TableVersionNumber = value;
                    m_bDirty = true;
                }
            }

            // Can only view this item
            public int SizeofDeviceRecord
            {
                get
                {
                    return (int)Table_hdmx.DeviceRecord.FieldOffsets.Widths + m_NumGlyphs + getNumPadBytes();
                }
            }

            // Number of records can only be set by adding or removing them
            public short NumberDeviceRecords
            {
                get
                {
                    return m_NumberDeviceRecords;
                }
            }

            public bool addDeviceRecord( ushort nIndex, byte bPixelSize, byte bMaxWidth, byte[] bWidths )
            {
                bool bResult = true;

                if( nIndex > m_NumberDeviceRecords )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of Records." );
                }
                else if( bWidths.Length != m_NumGlyphs )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "bWidth[] array size needs to match the number of glyphs." );
                }

                m_NumberDeviceRecords++;
                m_bDirty = true;

                return bResult;
            }


            public bool removeDeviceRecord( ushort nIndex )
            {
                bool bResult = true;

                if( nIndex >= m_NumberDeviceRecords )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of Records." );
                }

                m_NumberDeviceRecords--;
                m_bDirty = true;

                return bResult;
            }

            public byte getDeviceRecordPixelSize( ushort nIndex )
            {
                if( nIndex >= m_NumberDeviceRecords )
                {
                    throw new ArgumentOutOfRangeException( "Index is greater than number of Records." );
                }

                return ((DeviceRecordCache)m_DeviceRecords[nIndex]).PixelSize;
            }

            public bool setDeviceRecordPixelSize( ushort nIndex, byte bPixelSize )
            {
                bool bResult = true;

                if( nIndex >= m_NumberDeviceRecords )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of Records." );
                }

                ((DeviceRecordCache)m_DeviceRecords[nIndex]).PixelSize = bPixelSize;

                return bResult;
            }

            public byte getDeviceRecordMaxWidth( ushort nIndex )
            {
                if( nIndex >= m_NumberDeviceRecords )
                {
                    throw new ArgumentOutOfRangeException( "Index is greater than number of Records." );
                }

                return ((DeviceRecordCache)m_DeviceRecords[nIndex]).MaxWidth;
            }

            // NOTE: I am not sure this should be allowed to be set if max width is a function
            // of the widths[] field thewn this should always be computed
            public bool setDeviceRecordMaxWidth( ushort nIndex, byte bMaxWidth )
            {
                bool bResult = true;

                if( nIndex >= m_NumberDeviceRecords )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of Records." );
                }

                ((DeviceRecordCache)m_DeviceRecords[nIndex]).MaxWidth = bMaxWidth;

                return bResult;
            }

            // Cycles through each DeviceRecord adding Width for that DeviceRecord to Gylph at nIndex
            public bool addGylphWidthToDeviceRecords( ushort nIndex, byte[] bWidthsPerRecord )
            {
                bool bResult = true;
                
                if( nIndex != (m_NumGlyphs + 1))
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of Glyphs + 1." );
                }
                else if( bWidthsPerRecord.Length != m_NumberDeviceRecords )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "bWidth[] array size needs to match the number of Device Records." );
                }

                // NOTE if MaxWidth is a function of the widths[] we could make a check here and adjust if necessary

                for( ushort i = 0; i < m_NumberDeviceRecords; i++ )
                {
                    ((DeviceRecordCache)m_DeviceRecords[nIndex]).addWidth( nIndex, bWidthsPerRecord[i] );    
                }

                m_NumGlyphs++;
                m_bDirty = true;

                return bResult;
            }

            public bool removeGlyphWidthFromDeviceRecords( ushort nIndex )
            {
                bool bResult = true;

                if( nIndex >= m_NumGlyphs )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of Glyphs." );
                }

                // NOTE if MaxWidth is a function of the widths[] we could make a check here and adjust if necessary

                for( ushort i = 0; i < m_NumberDeviceRecords; i++ )
                {
                    ((DeviceRecordCache)m_DeviceRecords[nIndex]).removeWidth( nIndex );
                }

                m_NumGlyphs--;
                m_bDirty = true;

                return bResult;
            }

            private int getNumPadBytes( )
            {
                int nPadBytes = 0;
                int unpaddedLength = m_NumGlyphs + 2;
                if ((unpaddedLength & 3) != 0)
                {
                    nPadBytes = 4 - unpaddedLength % 4;
                }
                return nPadBytes;
            }                

            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer( 8 + (uint)(SizeofDeviceRecord * m_NumberDeviceRecords));

                // populate the buffer                
                newbuf.SetUshort( m_TableVersionNumber,        (uint)Table_hdmx.FieldOffsets.TableVersionNumber );
                newbuf.SetShort( m_NumberDeviceRecords,        (uint)Table_hdmx.FieldOffsets.NumberDeviceRecords );
                newbuf.SetInt( SizeofDeviceRecord,            (uint)Table_hdmx.FieldOffsets.SizeofDeviceRecord );                

                for( short i = 0; i < m_NumberDeviceRecords; i++ )
                {
                    newbuf.SetByte(((DeviceRecordCache)m_DeviceRecords[i]).PixelSize, (uint)Table_hdmx.FieldOffsets.DeviceRecord + (uint)(i * SizeofDeviceRecord ) + (uint)DeviceRecord.FieldOffsets.PixelSize );
                    newbuf.SetByte(((DeviceRecordCache)m_DeviceRecords[i]).MaxWidth, (uint)Table_hdmx.FieldOffsets.DeviceRecord + (uint)(i * SizeofDeviceRecord ) + (uint)DeviceRecord.FieldOffsets.MaxWidth );

                    for( ushort ii = 0; ii < m_NumGlyphs; ii++ )
                    {
                        newbuf.SetByte(((DeviceRecordCache)m_DeviceRecords[i]).getWidth( ii ), (uint)Table_hdmx.FieldOffsets.DeviceRecord + (uint)(i * SizeofDeviceRecord ) + (uint)(DeviceRecord.FieldOffsets.Widths + ii ));
                    }

                    // Pad the end with zeros                
                    for( uint ii = 0; ii < getNumPadBytes(); ii++ )
                    {
                        newbuf.SetByte( 0, (uint)Table_hdmx.FieldOffsets.DeviceRecord + (uint)(i * SizeofDeviceRecord ) + (uint)DeviceRecord.FieldOffsets.Widths + m_NumGlyphs + ii );
                    }                    
                }

                // put the buffer into a Table_hdmx object and return it
                Table_hdmx hdmxTable = new Table_hdmx("hdmx", newbuf);

                return hdmxTable;
            }

            // private inner class to hold device record information for the cache
            private class DeviceRecordCache
            {
                private byte m_PixelSize;
                private byte m_MaxWidth;
                private ArrayList m_Widths;
                
                // constructor converts a DeviceRecord to this DeviceRecordCache
                public DeviceRecordCache( DeviceRecord dr )
                {
                    m_PixelSize = dr.PixelSize;
                    m_MaxWidth = dr.MaxWidth;

                    m_Widths = new ArrayList( dr.numGlyphs );
                    
                    for( ushort i = 0; i < dr.numGlyphs; i++ )
                    {
                        m_Widths.Add( dr.GetWidth( i ));
                    }
                }
                
                public byte PixelSize
                {
                    get
                    {
                        return m_PixelSize;
                    }
                    set
                    {
                        m_PixelSize = value;
                    }
                }

                public byte MaxWidth
                {
                    get
                    {
                        return m_MaxWidth;
                    }
                    set
                    {
                        m_MaxWidth = value;
                    }
                }

                public byte getWidth( ushort nIndex )
                {
                    
                    return (byte)m_Widths[nIndex];
                }

                public void setWidth( ushort nIndex, byte bWidth )
                {
                    
                    m_Widths[nIndex]= bWidth;
                }

                public void addWidth( ushort nIndex, byte bWidth )
                {
                    m_Widths.Insert( nIndex, bWidth );
                }

                public void removeWidth( ushort nIndex )
                {
                    m_Widths.RemoveAt( nIndex );    
                }            
            }
        }
    }
}
