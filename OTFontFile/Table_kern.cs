using System;
using System.Collections;
using System.Diagnostics;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_kern.
    /// </summary>
    public class Table_kern : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_kern(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            version             = 0,
            nTables             = 2,
            FirstSubTableHeader = 4
        }


        /************************
         * internal classes
         */

        public class SubTableHeader
        {
            public SubTableHeader(uint offset, MBOBuffer bufTable)
            {
                m_offsetSubTableHeader = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                version  = 0,
                length   = 2,
                coverage = 4
            }
            public ushort version
            {
                get {return m_bufTable.GetUshort(m_offsetSubTableHeader + (uint)FieldOffsets.version);}
            }

            public ushort length
            {
                get {return m_bufTable.GetUshort(m_offsetSubTableHeader + (uint)FieldOffsets.length);}
            }

            public ushort coverage
            {
                get {return m_bufTable.GetUshort(m_offsetSubTableHeader + (uint)FieldOffsets.coverage);}
            }

            public bool GetHorizontalBit()
            {
                return ((coverage & 0x0001) == 0x0001);
            }

            public bool GetMinimumBit()
            {
                return ((coverage & 0x0002) == 0x0002);
            }

            public bool GetCrossStreamBit()
            {
                return ((coverage & 0x0004) == 0x0004);
            }

            public bool GetOverrideBit()
            {
                return ((coverage & 0x0008) == 0x0008);
            }

            public ushort GetFormat()
            {
                return (ushort)(coverage >> 8);
            }

            MBOBuffer m_bufTable;
            uint m_offsetSubTableHeader;
        }

        public class SubTable
        {
            public SubTable(uint offset, MBOBuffer bufTable)
            {
                m_offsetSubTable = offset;
                m_bufTable = bufTable;
            }

            public enum HeaderFieldOffsets
            {
                version  = 0,
                length   = 2,
                coverage = 4
            }
            public ushort version
            {
                get {return m_bufTable.GetUshort(m_offsetSubTable + (uint)HeaderFieldOffsets.version);}
            }

            public ushort length
            {
                get {return m_bufTable.GetUshort(m_offsetSubTable + (uint)HeaderFieldOffsets.length);}
            }

            public ushort coverage
            {
                get {return m_bufTable.GetUshort(m_offsetSubTable + (uint)HeaderFieldOffsets.coverage);}
            }

            virtual public ushort CalculatedLength()
            {
                return length; // overrides should calculate the correct length, not return the actual length
            }

            public ushort GetFormat()
            {
                return (ushort)(coverage >> 8);
            }

            protected MBOBuffer m_bufTable;
            protected uint m_offsetSubTable;
        }

        public class SubTableFormat0 : SubTable
        {
            // constructor
            public SubTableFormat0(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public enum FieldOffsets
            {
                nPairs        = 6,
                searchRange   = 8,
                entrySelector = 10,
                rangeShift    = 12,
                FirstKernPair = 14
            }

            public ushort nPairs
            {
                get {return m_bufTable.GetUshort(m_offsetSubTable + (uint)FieldOffsets.nPairs);}
            }

            public ushort searchRange
            {
                get {return m_bufTable.GetUshort(m_offsetSubTable + (uint)FieldOffsets.searchRange);}
            }

            public ushort entrySelector
            {
                get {return m_bufTable.GetUshort(m_offsetSubTable + (uint)FieldOffsets.entrySelector);}
            }

            public ushort rangeShift
            {
                get {return m_bufTable.GetUshort(m_offsetSubTable + (uint)FieldOffsets.rangeShift);}
            }

            public void GetKerningPairAndValue(int iPair, ref ushort left, ref ushort right, ref short kernvalue)
            {
                if (iPair >= nPairs)
                {
                    throw new ArgumentOutOfRangeException("iPair");
                }

                uint offset = m_offsetSubTable + (uint)FieldOffsets.FirstKernPair + (uint)iPair*6;
                left      = m_bufTable.GetUshort(offset);
                right     = m_bufTable.GetUshort(offset + 2);
                kernvalue = m_bufTable.GetShort(offset + 4);
            }

            override public ushort CalculatedLength()
            {
                return (ushort)(FieldOffsets.FirstKernPair + nPairs*6);
            }

        }

        public class SubTableFormat2 : SubTable
        {
            public SubTableFormat2(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            // NOTE: SubTableFormat2 is not suppported under windows
        }

        /************************
         * private methods
         */

        private uint GetSubTableHeaderOffset(uint i)
        {
            uint sthOffset = (uint)FieldOffsets.FirstSubTableHeader;
            for (uint n=0; n<i; n++)
            {
                sthOffset += m_bufTable.GetUshort(sthOffset + (uint)SubTableHeader.FieldOffsets.length);
            }
            return sthOffset;
        }

        /************************
         * accessors
         */

        public ushort version
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.version);}
        }

        public ushort nTables
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.nTables);}
        }

        public SubTableHeader GetSubTableHeader(uint i)
        {
            SubTableHeader sth = null;

            if (i < nTables)
            {
                uint sthOffset = GetSubTableHeaderOffset(i);
                // check for subtable header extending past end of table
                if (sthOffset+6 <= m_bufTable.GetLength())
                {
                    sth = new SubTableHeader(sthOffset, m_bufTable);
                }
            }

            return sth;
        }

        public SubTable GetSubTable(uint i)
        {
            SubTable st = null;

            SubTableHeader sth = GetSubTableHeader(i);
            if (sth != null)
            {
                if (sth.GetFormat() == 0)
                {
                    uint offsetSubTable = GetSubTableHeaderOffset(i);
                    st = new SubTableFormat0(offsetSubTable, m_bufTable);
                }
                else if (sth.GetFormat() == 2)
                {
                }
            }

            return st;
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new kern_cache(this);
            }

            return m_cache;
        }
        
        public class kern_cache : DataCache
        {
            // the cached data
            protected ushort m_version;
            protected ushort m_nTables;
            ArrayList m_SubTable; // SubTableFormat0[] since we only support format 0

            public kern_cache(Table_kern OwnerTable)
            {
                m_version = OwnerTable.version;
                m_nTables = 0;
                ushort nTablesTemp = OwnerTable.nTables;

                m_SubTable = new ArrayList( m_nTables );

                for( ushort i = 0; i < nTablesTemp; i ++ )
                {    
                    SubTable SubTableTemp = OwnerTable.GetSubTable( i );

                    //NOTE: Since these subtables could be Format 2 and we don't support them we will strip them out
                    if( null != SubTableTemp && 0 == SubTableTemp.GetFormat() )
                    {
                        m_SubTable.Add( SubTableTemp );
                        m_nTables++;
                    }
                }

            }


            // accessor methods

            public ushort version
            {
                get
                {
                    return m_version;
                }
                set 
                {
                    m_version = value;
                    m_bDirty = true;
                }
            }

            // Cant set number of tables this is done by adding or removing tables
            public ushort nTables
            {
                get
                {
                    return m_nTables;
                }                
            }

            public SubTable getSubTable( ushort nIndex )
            {
                if( nIndex >= m_nTables )
                {
                    throw new ArgumentOutOfRangeException( "Index is greater than number of tables." );
                }

                SubTable st = (SubTable)m_SubTable[nIndex];

                return st;
            }

            public bool setSubTable( ushort nIndex, SubTable st )
            {
                bool bResult = true;

                if( nIndex >= m_nTables )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of tables." );
                }
                else if( null == st )
                {
                    bResult = false;
                    throw new ArgumentNullException();
                }
                else if( !(st is SubTableFormat0 )) //NOTE: We only support format0
                {
                    bResult = false;
                    throw new ArgumentException( "The SubTable variable needs to SubTableFormat0." );
                }

                m_SubTable[nIndex] = st;
                m_bDirty = true;

                return bResult;
            }
    
            public bool addSubTable( ushort nIndex, SubTable st )
            {
                bool bResult = true;

                if( nIndex > m_nTables )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of tables." );
                }
                else if( null == st )
                {
                    bResult = false;
                    throw new ArgumentNullException();
                }
                else if( !(st is SubTableFormat0 )) //NOTE: We only support format0
                {
                    bResult = false;
                    throw new ArgumentException( "The SubTable variable needs to SubTableFormat0." );
                }

                m_SubTable.Insert( nIndex, st );
                m_nTables++;
                m_bDirty = true;

                return bResult;
            }

            public bool removeSubTable( ushort nIndex )
            {
                bool bResult = true;

                if( nIndex >= m_nTables )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of tables." );
                }

                m_SubTable.RemoveAt( nIndex );
                m_nTables--;
                m_bDirty = true;

                return bResult;
            }

            public override OTTable GenerateTable()
            {
                uint iSizeOfTables = 0;
                ushort nLeft = 0;
                ushort nRight = 0;
                short nValue = 0;
                // Find the sizeof all of the tables
                for( ushort i = 0; i < m_nTables; i++ )
                {
                    iSizeOfTables += (uint)((SubTableFormat0)m_SubTable[i]).CalculatedLength();    
                }                                                  
                                                  
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer( (uint)FieldOffsets.FirstSubTableHeader + iSizeOfTables );

                newbuf.SetUshort( m_version,    (uint)FieldOffsets.version );
                newbuf.SetUshort( m_nTables,    (uint)FieldOffsets.nTables );

                uint iOffset = (uint)FieldOffsets.FirstSubTableHeader;                //Fill in the tables
                for( int i = 0; i < m_nTables; i++ )
                {
                    newbuf.SetUshort( ((SubTableFormat0)m_SubTable[i]).version,            (uint)iOffset );
                    iOffset += 2;
                    newbuf.SetUshort( ((SubTableFormat0)m_SubTable[i]).length,            (uint)iOffset );
                    iOffset += 2;
                    newbuf.SetUshort( ((SubTableFormat0)m_SubTable[i]).coverage,        (uint)iOffset );
                    iOffset += 2;
                    newbuf.SetUshort( ((SubTableFormat0)m_SubTable[i]).nPairs,            (uint)iOffset );
                    iOffset += 2;
                    newbuf.SetUshort( ((SubTableFormat0)m_SubTable[i]).searchRange,        (uint)iOffset );
                    iOffset += 2;
                    newbuf.SetUshort( ((SubTableFormat0)m_SubTable[i]).entrySelector,    (uint)iOffset );
                    iOffset += 2;
                    newbuf.SetUshort( ((SubTableFormat0)m_SubTable[i]).rangeShift,        (uint)iOffset );
                    iOffset += 2;

                    // Cycle through all of the kerning pairs
                    for( int ii = 0; ii < ((SubTableFormat0)m_SubTable[i]).nPairs; ii++ )
                    {                        
                        ((SubTableFormat0)m_SubTable[i]).GetKerningPairAndValue( ii, ref nLeft, ref nRight, ref nValue );
                        newbuf.SetUshort( nLeft,                (uint)iOffset );
                        iOffset += 2;
                        newbuf.SetUshort( nRight,                (uint)iOffset );
                        iOffset += 2;
                        newbuf.SetShort( nValue,                (uint)iOffset );
                        iOffset += 2;        
                    }
                }

                // put the buffer into a Table_vhmtx object and return it
                Table_kern kernTable = new Table_kern("kern", newbuf);
            
                return kernTable;
            }
        }
        

    }
}
