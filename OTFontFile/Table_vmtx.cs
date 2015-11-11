using System;
using System.Collections;
using System.Diagnostics;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_vmtx.
    /// </summary>
    public class Table_vmtx : OTTable
    {
        protected Table_vhea m_vheaTable;
        protected ushort m_nGlyphsInTheFont;
        protected ushort m_nLongVerMetrics;

        /************************
         * constructors
         */        
        public Table_vmtx(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
            m_nLongVerMetrics = 0;
            m_nGlyphsInTheFont = 0;
        }

        public Table_vmtx(OTTag tag, MBOBuffer buf, Table_vhea vheaTable, ushort nGlyphsInTheFont) : base(tag, buf)
        {
            m_vheaTable = vheaTable;
            m_nGlyphsInTheFont = nGlyphsInTheFont;

            Debug.Assert(m_vheaTable != null);

            m_nLongVerMetrics = m_vheaTable.numOfLongVerMetrics;            
        }

        /************************
         * vMetric class
         */

        public class vMetric
        {
            public ushort advanceHeight;
            public short  topSideBearing;
        }

        /************************
         * accessors
         */

        public vMetric GetVMetric(uint i, OTFont fontOwner)
        {
            vMetric vm = new vMetric();
            m_nLongVerMetrics = (ushort)GetNumOfLongVerMetrics(fontOwner); 
            m_nGlyphsInTheFont = fontOwner.GetMaxpNumGlyphs();
            
            if( i >= m_nGlyphsInTheFont )
            {
                throw new ArgumentOutOfRangeException();
            }

            vm = GetVMetric( i );            

            return vm;
        }

        // This method is for the data cache but should be used later once the 
        // the construtor that sets the number of glyphs is used
        public vMetric GetVMetric(uint i)
        {
            vMetric vm = new vMetric();

            if( m_nGlyphsInTheFont == 0 )
            {
                throw new ArgumentException( "Number of Glyphs has not been set." );
            }
            else if ( i >= m_nGlyphsInTheFont )
            {
                throw new ArgumentOutOfRangeException();
            }

            if( i < m_nLongVerMetrics )
            {
                vm.advanceHeight  = m_bufTable.GetUshort(i*4);
                vm.topSideBearing = m_bufTable.GetShort(i*4+2);
            }
            else
            {
                vm.advanceHeight  = m_bufTable.GetUshort(((uint)m_nLongVerMetrics-1)*4);
                vm.topSideBearing = m_bufTable.GetShort((uint)m_nLongVerMetrics*4 + (i-(uint)m_nLongVerMetrics)*2);
            }

            return vm;
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

        public ushort numOfLongVerMetrics
        {
            get {return m_nLongVerMetrics;}            
        }

        // NOTE: This set method should be removed later
        public Table_vhea vheaTable
        {
            get {return m_vheaTable;}
            set
            {
                m_vheaTable = value;
                m_nLongVerMetrics = m_vheaTable.numOfLongVerMetrics;
            }
        }

        /************************
         * protected methods
         */

        protected uint GetNumOfLongVerMetrics(OTFont fontOwner)
        {
            if (m_nLongVerMetrics == 0)
            {
                // get the vhea table (to get access to the numOfLongVerMetrics property)
                Table_vhea vheaTable = (Table_vhea)fontOwner.GetTable("vhea");

                if (vheaTable != null)
                {
                    m_nLongVerMetrics = vheaTable.numOfLongVerMetrics;
                }
            }

            Debug.Assert(m_nLongVerMetrics != 0);
            return m_nLongVerMetrics;
        }
        
        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new vmtx_cache(this);
            }

            return m_cache;
        }
        
        public class vmtx_cache : DataCache
        {
            protected ArrayList m_vMetric; // vMetric[]
            protected Table_vhea m_vheaTable;
            protected ushort m_nGlyphsInTheFont;
            protected ushort m_nLongVerMetrics;
            
            // constructor
            public vmtx_cache(Table_vmtx OwnerTable)
            {
                // assign variables
                m_vheaTable = OwnerTable.vheaTable;
                m_nGlyphsInTheFont = OwnerTable.numOfGlyphsInTheFont;
                m_nLongVerMetrics = OwnerTable.numOfLongVerMetrics;

                // For ease of use we will store all entrys as double entrys and fix up the single
                // entry arrays when we write out the table
                m_vMetric = new ArrayList( m_nGlyphsInTheFont );
                for( ushort i = 0; i < m_nGlyphsInTheFont; i++ )
                {                
                    m_vMetric.Add( OwnerTable.GetVMetric( i ));                    
                }                
            }

            
            public ushort numOfLongVerMetrics
            {
                get {return m_nLongVerMetrics;}            
            }

            // Just as you would expect this returns a requested VMetric
            public vMetric getVMetric(ushort nIndex)
            {
                vMetric vMetricResult = new vMetric();
                
                if( nIndex >= m_nGlyphsInTheFont )
                {
                    throw new ArgumentOutOfRangeException("Tried to access a Vertical Matric that did not exist.");                
                }
                else if( nIndex >= m_nLongVerMetrics )
                {                                    
                    vMetricResult.advanceHeight = ((vMetric)m_vMetric[m_nLongVerMetrics - 1]).advanceHeight;
                    vMetricResult.topSideBearing = ((vMetric)m_vMetric[nIndex]).topSideBearing;
                }
                else
                {
                    vMetricResult.advanceHeight = ((vMetric)m_vMetric[nIndex]).advanceHeight;
                    vMetricResult.topSideBearing = ((vMetric)m_vMetric[nIndex]).topSideBearing;        
                }

                return vMetricResult;
            }

            // Sets a VMetric
            public bool setVMetric( ushort nIndex, short nTopSideBearing )
            {
                bool bResult = true;

                if( nIndex < m_nLongVerMetrics )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "The Vertical Matric your trying to set requires both TopSideBearing and AdvanceHeight." );
                }                    
                else if( nIndex >= m_nGlyphsInTheFont )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Tried to access a Vertical Matric that does not exist.");
                }    
                else
                {
                    ((vMetric)m_vMetric[nIndex]).topSideBearing = nTopSideBearing;
                    m_bDirty = true;
                }
                
                return bResult;                
            }

            // Sets a VMetric
            public bool setVMetric( ushort nIndex, ushort nAdvanceHeight, short nTopSideBearing )
            {
                bool bResult = true;

                if( nIndex >= m_nLongVerMetrics )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "The Vertical Matric your trying to set requires only uses TopSideBearing." );
                }                    
                else if( nIndex >= m_nGlyphsInTheFont )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Tried to access a Vertical Matric that does not exist.");
                }    
                else
                {
                    ((vMetric)m_vMetric[nIndex]).advanceHeight = nAdvanceHeight;
                    ((vMetric)m_vMetric[nIndex]).topSideBearing = nTopSideBearing;
                    m_bDirty = true;
                }
                
                return bResult;                
            }

            // To be used if this is a single variable array of Vertical Matrix if so the nIndex
            // Needs to be >= m_nLongVerMetrics
            public bool addVerticalMetric( ushort nIndex, short nTopSideBearing )
            {
                bool bResult = true;
                if( nIndex >= m_nLongVerMetrics )
                {
                    bResult = addToVerticalMetric( nIndex, 0, nTopSideBearing );    
                }
                else
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException("Tried to add a single variable Vertical Matric array to a double array position.");
                }

                return bResult;
            }

            // To be used if this is a double variable array of Vertical Matrix if so the nIndex
            // Needs to be <= m_nLongVerMetrics
            public bool addVerticalMetric( ushort nIndex, ushort nAdvanceHeight, short nTopSideBearing )
            {
                bool bResult = true;
                if( nIndex <= m_nLongVerMetrics )
                {
                    bResult = addToVerticalMetric( nIndex, nAdvanceHeight, nTopSideBearing );    
                }
                else
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException("Tried to add a double variable Vertical Matric array to a single array position.");
                }

                return bResult;
            }

            // Does the real work of adding
            protected bool addToVerticalMetric( ushort nIndex, ushort nAdvanceHeight, short nTopSideBearing )
            {
                bool bResult = true;

                // NOTE: This might not be OK we might want to add a glyph that is much greater the the number that is there now
                if( nIndex <= m_nGlyphsInTheFont )
                {
                    // if we are adding a vertical matrix that is less the the number of long vertical
                    // metrics we need to adjust for this so we know how to write it later and fix up the vhea_table
                    if( nIndex < m_nLongVerMetrics || (nIndex == m_nLongVerMetrics && nAdvanceHeight != 0))
                    {
                        m_nLongVerMetrics++;
                        Table_vhea.vhea_cache vheaCache = (Table_vhea.vhea_cache)m_vheaTable.GetCache();
                        vheaCache.numOfLongVerMetrics++;
                    }

                    // NOTE: Table maxp and ltsh numGlyphs isn't being dynamically updated
                    m_nGlyphsInTheFont++;
                     
                    vMetric vm = new vMetric();

                    vm.advanceHeight = nAdvanceHeight;
                    vm.topSideBearing = nTopSideBearing;
                    
                    m_vMetric.Insert( nIndex, vm );
                    m_bDirty = true;
                }
                else
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException("Tried to add a Vertical Matric to a position greater than the number of Glyphs + 1.");
                    
                }

                return bResult;
            }

            // Pulls out a Vertical Matric and updates vhea if necessary
            public bool removeVerticalMetric( ushort nIndex )
            {
                bool bResult = true;

                if( nIndex < m_nGlyphsInTheFont )
                {
                    // if we are removing a vertical matrix that is less the the number of long vertical
                    // metrics we need to adjust for this so we know how to write it later and fix up the vhea_table
                    if( nIndex < m_nLongVerMetrics )
                    {
                        m_nLongVerMetrics--;
                        Table_vhea.vhea_cache vheaCache = (Table_vhea.vhea_cache)m_vheaTable.GetCache();
                        vheaCache.numOfLongVerMetrics--;
                    }

                    // NOTE: Table maxp and ltsh numGlyphs isn't being dynamically updated
                    m_nGlyphsInTheFont--;

                    m_vMetric.RemoveAt( nIndex );
                    m_bDirty = true;
                }
                else
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException("Tried to remove a Vertical Matric that did not exist.");                    
                }

                return bResult;
            }

            // generate a new table from the cached data
            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer((uint)(m_nLongVerMetrics * 4) + (uint)((m_nGlyphsInTheFont - m_nLongVerMetrics) * 2));

                for( ushort i = 0; i < m_nGlyphsInTheFont; i++ )
                {
                    if( i < m_nLongVerMetrics )
                    {
                        newbuf.SetUshort( ((vMetric)m_vMetric[i]).advanceHeight,    (uint)(i * 4 ));
                        newbuf.SetShort ( ((vMetric)m_vMetric[i]).topSideBearing,    (uint)((i * 4) + 2));
                    }
                    else
                    {
                        newbuf.SetShort ( ((vMetric)m_vMetric[i]).topSideBearing,    (uint)(((m_nLongVerMetrics - 1)* 4) + (i * 2)));    
                    }
                }

                // put the buffer into a Table_vhmtx object and return it
                Table_vmtx vmtxTable = new Table_vmtx("vmtx", newbuf);
            
                return vmtxTable;
            }
        }

    }
}
