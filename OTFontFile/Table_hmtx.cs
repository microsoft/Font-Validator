using System;
using System.Diagnostics;
using System.Collections;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_hmtx.
    /// </summary>
    public class Table_hmtx : OTTable
    {

        protected Table_hhea m_hheaTable;
        protected ushort m_nGlyphsInTheFont;
        protected ushort m_nNumberOfHMetrics;

        /************************
         * constructors
         */
        
        
        public Table_hmtx(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        public Table_hmtx(OTTag tag, MBOBuffer buf, Table_hhea hheaTable, ushort nGlyphsInTheFont) : base(tag, buf)
        {
            m_hheaTable = hheaTable;
            m_nGlyphsInTheFont = nGlyphsInTheFont;

            Debug.Assert(m_hheaTable != null);

            m_nNumberOfHMetrics = m_hheaTable.numberOfHMetrics;            
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
        }


        /************************
         * accessor methods
         */

        public class longHorMetric
        {
            public ushort advanceWidth;
            public short lsb;
        }

        public longHorMetric GetHMetric(uint i, OTFont fontOwner)
        {
            if (i >= GetNumberOfHMetrics(fontOwner))
            {
                throw new ArgumentOutOfRangeException("i");
            }

            longHorMetric hm = new longHorMetric();
            hm.advanceWidth = m_bufTable.GetUshort(i*4);
            hm.lsb          = m_bufTable.GetShort(i*4+2);

            return hm;
        }

        public longHorMetric GetOrMakeHMetric(uint i, OTFont fontOwner)
        {
            longHorMetric hm = null;
            m_nGlyphsInTheFont = fontOwner.GetMaxpNumGlyphs();            
            m_nNumberOfHMetrics = GetNumberOfHMetrics(fontOwner);
            uint nlsb = GetNumLeftSideBearingEntries(fontOwner);
            uint CalcTableLength = (uint)m_nNumberOfHMetrics*4 + nlsb*2;

            if( i >= m_nGlyphsInTheFont )
            {
                throw new ArgumentOutOfRangeException();
            }            

            // only try to parse out the data if the table length is correct
            if (CalcTableLength == GetLength())
            {
                hm = GetOrMakeHMetric( i );                
            }

            return hm;
        }

        // This method is for the data cache but should be used later once the 
        // the construtor that sets the number of glyphs is used
        public longHorMetric GetOrMakeHMetric( uint i )
        {
            if( m_nGlyphsInTheFont == 0 )
            {
                throw new ArgumentException( "Number of Glyphs has not been set." );
            }
            else if( i >= m_nGlyphsInTheFont )
            {
                throw new ArgumentOutOfRangeException();
            }
            
            longHorMetric hm = new longHorMetric();

            if (i < m_nNumberOfHMetrics)
            {
                hm.advanceWidth = m_bufTable.GetUshort(i*4);
                hm.lsb          = m_bufTable.GetShort(i*4+2);
            }
            else
            {
                hm.advanceWidth = m_bufTable.GetUshort(((uint)m_nNumberOfHMetrics-1)*4);
                hm.lsb          = m_bufTable.GetShort((uint)m_nNumberOfHMetrics*4 + ((i - m_nNumberOfHMetrics)*2));
            }
            
            return hm;
        }

        public uint GetNumLeftSideBearingEntries(OTFont fontOwner)
        {
            uint nhm = GetNumberOfHMetrics(fontOwner);
            uint numGlyphs = fontOwner.GetMaxpNumGlyphs();
            return numGlyphs - nhm;
        }

        public short GetLeftSideBearing(uint i, OTFont fontOwner)
        {
            if (i >= GetNumLeftSideBearingEntries(fontOwner))
            {
                throw new ArgumentOutOfRangeException("i");
            }

            uint nhm = GetNumberOfHMetrics(fontOwner);

            return m_bufTable.GetShort(nhm*4 + i*2);
        }

        public bool IsMonospace(OTFont fontOwner)
        {
            bool bHmtxMono = true;
            uint numberOfHMetrics = GetNumberOfHMetrics(fontOwner);
            if (numberOfHMetrics > 2)
            {
                // find the first non-zero width after the null glyph
                ushort nFirstWidth = 0;
                uint nFirstWidthPos = 1;
                for (uint i=nFirstWidthPos; i<numberOfHMetrics; i++)
                {
                    nFirstWidth = GetHMetric(i, fontOwner).advanceWidth;
                    if (nFirstWidth != 0)
                    {
                        nFirstWidthPos = i;
                        break;
                    }
                }

                if (nFirstWidth != 0)
                {
                    for (uint i=(uint)nFirstWidthPos+1; i<numberOfHMetrics; i++)
                    {
                        ushort nWidth = GetHMetric(i, fontOwner).advanceWidth;
                        if (nWidth != 0 && nWidth != nFirstWidth)
                        {
                            bHmtxMono = false;
                            break;
                        }
                    }
                }
            }

            return bHmtxMono;
        }
        
        // NOTE: This set method should be removed later
        public ushort numOfGlyphsInTheFont
        {    
            get {return m_nGlyphsInTheFont;}
            set
            {
                m_nGlyphsInTheFont = value;                
            }
        }

        public ushort numOfHMetrics
        {
            get {return m_nNumberOfHMetrics;}            
        }

        // NOTE: This set method should be removed later
        public Table_hhea hheaTable
        {
            get {return m_hheaTable;}
            set
            {
                m_hheaTable = value;
                m_nNumberOfHMetrics = m_hheaTable.numberOfHMetrics;
            }
        }


        /************************
         * protected methods
         */

        protected ushort GetNumberOfHMetrics(OTFont fontOwner)
        {

            Table_hhea hheaTable = (Table_hhea)fontOwner.GetTable("hhea");
            if (hheaTable != null)
            {
                m_nNumberOfHMetrics = hheaTable.numberOfHMetrics;
            }

            //Debug.Assert(numberOfHMetrics != 0);
            return m_nNumberOfHMetrics;
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new hmtx_cache(this);
            }

            return m_cache;
        }
        
        public class hmtx_cache : DataCache
        {
            protected  ArrayList m_longHorMetric; // longHorMetric[]
            protected Table_hhea m_hheaTable;
            protected ushort m_nGlyphsInTheFont;
            protected ushort m_nNumberOfHMetrics;
            
            // constructor
            public hmtx_cache(Table_hmtx OwnerTable)
            {
                // assign
                m_hheaTable = OwnerTable.hheaTable;
                m_nGlyphsInTheFont = OwnerTable.numOfGlyphsInTheFont;
                m_nNumberOfHMetrics = OwnerTable.numOfHMetrics;

                // For ease of use we will store all entrys as double entrys and fix up the single
                // entry arrays (left side bearing only ) when we write out the table
                m_longHorMetric = new ArrayList( m_nGlyphsInTheFont );
                for( ushort i = 0; i < m_nGlyphsInTheFont; i++ )
                {
                    m_longHorMetric.Add( OwnerTable.GetOrMakeHMetric( i ));                    
                }

                
            }

            // Just as you would expect this returns a requested longHorMetric
            public longHorMetric GetOrMakeHMetric( ushort nIndex )
            {
                longHorMetric lm = new longHorMetric();
                
                if( nIndex >= m_nGlyphsInTheFont )
                {
                    throw new ArgumentOutOfRangeException("Tried to access a Horiz Matric that does not exist.");                    
                }
                else if( nIndex >= m_nNumberOfHMetrics )
                {
                    lm.advanceWidth = ((longHorMetric)m_longHorMetric[m_nNumberOfHMetrics - 1]).advanceWidth;
                    lm.lsb = ((longHorMetric)m_longHorMetric[m_nNumberOfHMetrics - 1]).lsb;
                }
                else
                {                    
                    lm.advanceWidth = ((longHorMetric)m_longHorMetric[nIndex]).advanceWidth;
                    lm.lsb = ((longHorMetric)m_longHorMetric[nIndex]).lsb;
                }

                return lm;
            }

            // Sets a LeftSideBearing
            public bool setLongHorMetric( ushort nIndex, short nLeftSideBearing )
            {
                bool bResult = true;

                if( nIndex < m_nNumberOfHMetrics )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Horiz Matric requires both LeftSideBearing and AdvanceWidth" );
                }                    
                else if( nIndex >= m_nGlyphsInTheFont )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Tried to access a Horiz Matric that does not exist." );
                }                
                else
                {
                    ((longHorMetric)m_longHorMetric[nIndex]).lsb = nLeftSideBearing;
                    m_bDirty = true;                        
                }

                return bResult;                
            }


            // Sets a longHorMetric
            public bool setLongHorMetric( ushort nIndex, ushort nAdvanceWidth, short nLeftSideBearing )
            {
                bool bResult = true;

                if( nIndex >= m_nNumberOfHMetrics )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "The Horiz Matric your trying to set requires only LeftSideBearing." );
                }                    
                else if( nIndex >= m_nGlyphsInTheFont )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Tried to access a Horiz Matric that does not exist.");
                }                
                else
                {
                    ((longHorMetric)m_longHorMetric[nIndex]).advanceWidth = nAdvanceWidth;
                    ((longHorMetric)m_longHorMetric[nIndex]).lsb = nLeftSideBearing;
                    m_bDirty = true;
                }

                return bResult;                
            }

            // To be used if this is a single variable array of Horiz Matrix if so the nIndex
            // Needs to be >= m_nLongVerMetrics
            public bool addLongHorMetric( ushort nIndex, short nLeftSideBearing )
            {
                bool bResult = true;

                if( nIndex < m_nNumberOfHMetrics )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Both LeftSideBearing and AdvanceWidth are required for this entry." );
                }            
                else
                {
                    bResult = addToLongHorMetric( nIndex, 0, nLeftSideBearing );    
                }

                return bResult;
            }

            // To be used if this is a double variable array of Vertical Matrix if so the nIndex
            // Needs to be <= m_nLongVerMetrics
            public bool addLongHorMetric( ushort nIndex, ushort nAdvanceWidth, short nLeftSideBearing )
            {
                bool bResult = true;
                if( nIndex > m_nNumberOfHMetrics )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Only LeftSideBearing can be set." );
                }    
                else
                {
                    bResult = addToLongHorMetric( nIndex, nAdvanceWidth, nLeftSideBearing );        
                }

                return bResult;
            }

            // Does the real work of adding
            protected bool addToLongHorMetric( ushort nIndex, ushort nAdvanceWidth, short nLeftSideBearing )
            {
                bool bResult = true;

                // NOTE: This might not be OK we might want to add a glyph that is much greater the the number that is there now
                if( nIndex >= m_nGlyphsInTheFont )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException("Tried to add a Vertical Matric to a position greater than the number of Glyphs + 1.");
                }
                else
                {
                    // if we are adding a horiz matrix that is less the the number of NumberOfHMetrics
                    // we need to adjust for this so we know how to write it later and fix up the hhea_table
                    if( nIndex < m_nNumberOfHMetrics || (nIndex == m_nNumberOfHMetrics && nAdvanceWidth != 0))
                    {
                        m_nNumberOfHMetrics++;
                        Table_hhea.hhea_cache hheaCache = (Table_hhea.hhea_cache)m_hheaTable.GetCache();
                        hheaCache.numberOfHMetrics++;
                    }                    

                    // NOTE: Table maxp and ltsh numGlyphs isn't being dynamically updated
                    m_nGlyphsInTheFont++;

                    longHorMetric lhm = new longHorMetric();
                    lhm.advanceWidth = nAdvanceWidth;
                    lhm.lsb = nLeftSideBearing;

                    m_longHorMetric.Insert( nIndex, lhm );                    
                    m_bDirty = true;                
                }

                return bResult;
            }

            // Pulls out a Vertical Matric and updates hhea if necessary
            public bool removeLongHorMetric( ushort nIndex )
            {
                bool bResult = true;

                if( nIndex >= m_nGlyphsInTheFont )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException("Tried to remove a Vertical Matric that did not exist.");                    
                }
                else
                {
                    // if we are removing a vertical matrix that is less the the number of long vertical
                    // metrics we need to adjust for this so we know how to write it later and fix up the hhea_table
                    if( nIndex < m_nNumberOfHMetrics )
                    {
                        m_nNumberOfHMetrics--;
                        Table_hhea.hhea_cache hheaCache = (Table_hhea.hhea_cache)m_hheaTable.GetCache();
                        hheaCache.numberOfHMetrics--;
                    }

                    // NOTE: Table maxp and ltsh numGlyphs isn't being dynamically updated
                    m_nGlyphsInTheFont--;                    

                    m_longHorMetric.RemoveAt( nIndex );
                    m_bDirty = true;
                }                

                return bResult;
            }

            // generate a new table from the cached data
            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer((uint)(m_nNumberOfHMetrics * 4) + (uint)((m_nGlyphsInTheFont - m_nNumberOfHMetrics) * 2));

                for( ushort i = 0; i < m_nGlyphsInTheFont; i++ )
                {
                    if( i < m_nNumberOfHMetrics )
                    {
                        newbuf.SetUshort( ((longHorMetric)m_longHorMetric[i]).advanceWidth,    (uint)(i * 4 ));
                        newbuf.SetShort ( ((longHorMetric)m_longHorMetric[i]).lsb,    (uint)((i * 4) + 2));
                    }
                    else
                    {
                        newbuf.SetShort ( ((longHorMetric)m_longHorMetric[i]).lsb,    (uint)(((m_nNumberOfHMetrics - 1)* 4) + (i * 2)));    
                    }
                }

                // put the buffer into a Table_vhmtx object and return it
                Table_hmtx hmtxTable = new Table_hmtx("hmtx", newbuf);
            
                return hmtxTable;
            }
        }
    

    }
}
