using System;
using System.Collections;


namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_EBSC.
    /// </summary>
    public class Table_EBSC : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_EBSC(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            version               = 0,
            numSizes              = 4,
            FirstBitmapScaleTable = 8
        }


        /************************
         * internal classes
         */

        public class bitmapScaleTable
        {
            public bitmapScaleTable(uint offset, MBOBuffer bufTable)
            {
                m_bstOffset = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                hori            = 0,
                vert            = 12,
                ppemX           = 24,
                ppemY           = 25,
                substitutePpemX = 26,
                substitutePpemY = 27
            }
            public static uint length = 28;

            public Table_EBLC.sbitLineMetrics hori
            {
                get {return new Table_EBLC.sbitLineMetrics(m_bstOffset + (uint)FieldOffsets.hori, m_bufTable);}
            }

            public Table_EBLC.sbitLineMetrics vert
            {
                get {return new Table_EBLC.sbitLineMetrics(m_bstOffset + (uint)FieldOffsets.vert, m_bufTable);}
            }

            public byte ppemX
            {
                get {return m_bufTable.GetByte(m_bstOffset + (uint)FieldOffsets.ppemX);}
            }

            public byte ppemY
            {
                get {return m_bufTable.GetByte(m_bstOffset + (uint)FieldOffsets.ppemY);}
            }

            public byte substitutePpemX
            {
                get {return m_bufTable.GetByte(m_bstOffset + (uint)FieldOffsets.substitutePpemX);}
            }

            public byte substitutePpemY
            {
                get {return m_bufTable.GetByte(m_bstOffset + (uint)FieldOffsets.substitutePpemY);}
            }


            protected uint m_bstOffset;
            protected MBOBuffer m_bufTable;
        }
        
        
        
        /************************
         * accessors
         */

        public OTFixed version
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.version);}
        }

        public uint numSizes
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.numSizes);}
        }

        public bitmapScaleTable GetBitmapScaleTable(uint i)
        {
            bitmapScaleTable bst = null;

            if (i < numSizes)
            {
                uint bstOffset = (uint)FieldOffsets.FirstBitmapScaleTable + i*bitmapScaleTable.length;
                bst = new bitmapScaleTable(bstOffset, m_bufTable);
            }

            return bst;
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new EBSC_cache( this );
            }

            return m_cache;
        }
        
        public class EBSC_cache : DataCache
        {
            protected OTFixed m_version;
            protected uint m_numSizes;
            protected ArrayList m_bitmapScaleTables; // bitmapScaleTableCache[]

                
            // constructor
            public EBSC_cache(Table_EBSC OwnerTable)
            {
                // copy the data from the owner table's MBOBuffer
                // and store it in the cache variables
                m_version = OwnerTable.version;
                m_numSizes = OwnerTable.numSizes;

                m_bitmapScaleTables = new ArrayList( (int)m_numSizes );

                for( uint i = 0; i < m_numSizes; i++ )
                {
                    m_bitmapScaleTables.Add( OwnerTable.GetBitmapScaleTable( i ));
                }

            }

            // accessors for the cached data
            public OTFixed version
            {
                get{ return m_version; }
                set
                {
                    m_version = value;
                    m_bDirty = true;
                }
            }

            // This can only be set by adding or removing bitmapScaleTables
            public uint numSizes
            {
                get{ return m_numSizes; }
                
            }
            
            public bitmapScaleTable getBitmapScaleTable( uint nIndex )
            {
                bitmapScaleTable bst = null;

                if( nIndex > m_numSizes )
                {
                    throw new ArgumentOutOfRangeException( "Index is greater than number of sizes." );
                }
                else
                {
                    bst = (bitmapScaleTable)m_bitmapScaleTables[(int)nIndex];
                }

                return bst;        
            }

            public bool setBitmapScaleTable( bitmapScaleTable cBitmapScaleTable, uint nIndex )
            {
                bool bResult = true;
                
                if( cBitmapScaleTable == null )
                {
                    bResult = false;
                    throw new ArgumentNullException();
                }
                else if( nIndex > m_numSizes )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of sizes." );
                }
                else
                {
                    bitmapScaleTableCache bstc = bitmapScaleTableCache.FromBitmapScaleTable(cBitmapScaleTable);

                    m_bitmapScaleTables[(int)nIndex] = bstc;
                    m_bDirty = true;
                }

                return bResult;
            }

            public bool addBitmapScaleTable( bitmapScaleTable cBitmapScaleTable, uint nIndex )
            {
                bool bResult = true;
                
                if( cBitmapScaleTable == null )
                {
                    bResult = false;
                    throw new ArgumentNullException();
                }
                else if( nIndex > m_numSizes )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of sizes." );
                }
                else
                {
                    bitmapScaleTableCache bstc = bitmapScaleTableCache.FromBitmapScaleTable(cBitmapScaleTable);


                    m_bitmapScaleTables.Insert( (int)nIndex, bstc );
                    m_bDirty = true;
                }

                return bResult;
            }

            public bool removeBitmapScaleTable( uint nIndex )
            {
                bool bResult = true;

                if( nIndex > m_numSizes )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Index is greater than number of sizes." );
                }
                else
                {
                    m_bitmapScaleTables.RemoveAt( (int)nIndex );
                }

                return bResult;
            }

            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer( (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(Table_EBSC.bitmapScaleTable.length * m_numSizes));

                // populate the buffer                
                newbuf.SetFixed( m_version,                (uint)Table_EBSC.FieldOffsets.version );
                newbuf.SetUint( m_numSizes,                (uint)Table_EBSC.FieldOffsets.numSizes );                

                for( uint i = 0; i < m_numSizes; i++ )
                {
                    bitmapScaleTable bst = (bitmapScaleTable)m_bitmapScaleTables[(int)i];

                    newbuf.SetSbyte( bst.hori.ascender, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length)); 
                    newbuf.SetSbyte( bst.hori.descender, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 1 );
                    newbuf.SetByte( bst.hori.widthMax, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 2 );
                    newbuf.SetSbyte( bst.hori.caretSlopeNumerator, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 3 );
                    newbuf.SetSbyte( bst.hori.caretSlopeDenominator, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 4 );
                    newbuf.SetSbyte( bst.hori.caretOffset, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 5 );
                    newbuf.SetSbyte( bst.hori.minOriginSB, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 6 );
                    newbuf.SetSbyte( bst.hori.minAdvanceSB, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 7 );
                    newbuf.SetSbyte( bst.hori.maxBeforeBL, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 8 );
                    newbuf.SetSbyte( bst.hori.minAfterBL, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 9 );
                    newbuf.SetSbyte( bst.hori.pad1, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 10 );
                    newbuf.SetSbyte( bst.hori.pad2, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 11 );

                    
                    newbuf.SetSbyte( bst.vert.ascender, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 12 ); 
                    newbuf.SetSbyte( bst.vert.descender, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 13 );
                    newbuf.SetByte( bst.vert.widthMax, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 14 );
                    newbuf.SetSbyte( bst.vert.caretSlopeNumerator, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 15 );
                    newbuf.SetSbyte( bst.vert.caretSlopeDenominator, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 16 );
                    newbuf.SetSbyte( bst.vert.caretOffset, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 17 );
                    newbuf.SetSbyte( bst.vert.minOriginSB, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 18 );
                    newbuf.SetSbyte( bst.vert.minAdvanceSB, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 19 );
                    newbuf.SetSbyte( bst.vert.maxBeforeBL, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 20 );
                    newbuf.SetSbyte( bst.vert.minAfterBL, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 21 );
                    newbuf.SetSbyte( bst.vert.pad1, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 22 );
                    newbuf.SetSbyte( bst.vert.pad2, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 23 );

                    newbuf.SetByte( bst.ppemX, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 24 );
                    newbuf.SetByte( bst.ppemY, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 25 );
                    newbuf.SetByte( bst.substitutePpemX, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 26 );
                    newbuf.SetByte( bst.substitutePpemY, (uint)Table_EBSC.FieldOffsets.FirstBitmapScaleTable + (uint)(i * Table_EBSC.bitmapScaleTable.length) + 27 );                    
                }

                // put the buffer into a Table_EBSC object and return it
                Table_EBSC EBSCTable = new Table_EBSC("EBSC", newbuf);

                return EBSCTable;
            }


            public class bitmapScaleTableCache : ICloneable
            {
                public Table_EBLC.EBLC_cache.sbitLineMetricsCache hori;
                public Table_EBLC.EBLC_cache.sbitLineMetricsCache vert;
                public byte ppemX;
                public byte ppemY;
                public byte substitutePpemX;
                public byte substitutePpemY;

                static public bitmapScaleTableCache FromBitmapScaleTable(bitmapScaleTable bst)
                {
                    bitmapScaleTableCache bstc = new bitmapScaleTableCache();
                    bstc.hori = Table_EBLC.EBLC_cache.sbitLineMetricsCache.FromSbitLineMetrics(bst.hori);
                    bstc.vert = Table_EBLC.EBLC_cache.sbitLineMetricsCache.FromSbitLineMetrics(bst.vert);
                    bstc.ppemX = bst.ppemX;
                    bstc.ppemY = bst.ppemY;
                    bstc.substitutePpemX = bst.substitutePpemX;
                    bstc.substitutePpemY = bst.substitutePpemY;
                    return bstc;
                }

                public object Clone()
                {
                    bitmapScaleTableCache bstc = new bitmapScaleTableCache();
                    bstc.hori = (Table_EBLC.EBLC_cache.sbitLineMetricsCache)hori.Clone();
                    bstc.vert = (Table_EBLC.EBLC_cache.sbitLineMetricsCache)vert.Clone();
                    bstc.ppemX = ppemX;
                    bstc.ppemY = ppemY;
                    bstc.substitutePpemX = substitutePpemX;
                    bstc.substitutePpemY = substitutePpemY;
                    return bstc;
                }
            }
        }
        

    }
}
