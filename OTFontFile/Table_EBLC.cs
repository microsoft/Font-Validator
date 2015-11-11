using System;
using System.Collections;
using System.Diagnostics;


namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_EBLC.
    /// </summary>
    public class Table_EBLC : OTTable
    {
        protected Table_EBDT m_tableEDBT;
        /************************
         * constructors
         */
        
        
        public Table_EBLC(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        public Table_EBLC(OTTag tag, MBOBuffer buf, Table_EBDT tableEDBT ) : base(tag, buf)
        {
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            version  = 0,
            numSizes = 4,
            FirstbitmapSizeTable = 8
        }


        /************************
         * internal classes
         */

        public class bitmapSizeTable
        {
            public bitmapSizeTable(uint offset, MBOBuffer bufTable)
            {
                m_bstOffset = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                indexSubTableArrayOffset = 0,
                indexTablesSize          = 4,
                numberOfIndexSubTables   = 8,
                colorRef                 = 12,
                hori                     = 16,
                vert                     = 28,
                startGlyphIndex          = 40,
                endGlyphIndex            = 42,
                ppemX                    = 44,
                ppemY                    = 45,
                bitDepth                 = 46,
                flags                    = 47
            }
            
            public const ushort bufSize = 48;

            public uint indexSubTableArrayOffset
            {
                get {return m_bufTable.GetUint(m_bstOffset + (uint)FieldOffsets.indexSubTableArrayOffset);}
            }

            public uint indexTablesSize
            {
                get {return m_bufTable.GetUint(m_bstOffset + (uint)FieldOffsets.indexTablesSize);}
            }

            public uint numberOfIndexSubTables
            {
                get {return m_bufTable.GetUint(m_bstOffset + (uint)FieldOffsets.numberOfIndexSubTables);}
            }

            public uint colorRef
            {
                get {return m_bufTable.GetUint(m_bstOffset + (uint)FieldOffsets.colorRef);}
            }

            public sbitLineMetrics hori
            {
                get {return new sbitLineMetrics(m_bstOffset + (uint)FieldOffsets.hori, m_bufTable);}
            }

            public sbitLineMetrics vert
            {
                get {return new sbitLineMetrics(m_bstOffset + (uint)FieldOffsets.vert, m_bufTable);}
            }

            public ushort startGlyphIndex
            {
                get {return m_bufTable.GetUshort(m_bstOffset + (uint)FieldOffsets.startGlyphIndex);}
            }

            public ushort endGlyphIndex
            {
                get {return m_bufTable.GetUshort(m_bstOffset + (uint)FieldOffsets.endGlyphIndex);}
            }

            public byte ppemX
            {
                get {return m_bufTable.GetByte(m_bstOffset + (uint)FieldOffsets.ppemX);}
            }

            public byte ppemY
            {
                get {return m_bufTable.GetByte(m_bstOffset + (uint)FieldOffsets.ppemY);}
            }

            public byte bitDepth
            {
                get {return m_bufTable.GetByte(m_bstOffset + (uint)FieldOffsets.bitDepth);}
            }

            public sbyte flags
            {
                get {return m_bufTable.GetSbyte(m_bstOffset + (uint)FieldOffsets.flags);}
            }


            // methods
            public indexSubTableArray FindIndexSubTableArray(ushort idGlyph)
            {
                indexSubTableArray ista = null;

                for (uint i=0; i<numberOfIndexSubTables; i++)
                {
                    if (indexSubTableArrayOffset < m_bufTable.GetLength())
                    {
                        uint istaOffset = indexSubTableArrayOffset + i*8;
                        ista = new indexSubTableArray(istaOffset, m_bufTable);
                        if (idGlyph >= ista.firstGlyphIndex && idGlyph <= ista.lastGlyphIndex)
                        {
                            break;
                        }
                        else
                        {
                            ista = null;
                        }
                    }
                }

                return ista;
            }

            public indexSubTable GetIndexSubTable(indexSubTableArray ista)
            {
                indexSubTable ist = null;

                uint offset = indexSubTableArrayOffset + ista.additionalOffsetToIndexSubtable;

                if (offset + (uint)indexSubHeader.FieldOffsets.indexFormat + 2 <= m_bufTable.GetLength())
                {

                    ushort indexFormat = m_bufTable.GetUshort(offset + (uint)indexSubHeader.FieldOffsets.indexFormat);

                    switch(indexFormat)
                    {
                        case 1:
                        {
                            indexSubTable1 ist1 = new indexSubTable1(offset, m_bufTable, ista);
                            ist = ist1;
                            break;
                        }
                        case 2:
                        {
                            indexSubTable2 ist2 = new indexSubTable2(offset, m_bufTable, ista);
                            ist = ist2;
                            ist2.bigMetrics = new Table_EBDT.bigGlyphMetrics();
                            ist2.imageSize             = m_bufTable.GetUint(offset + indexSubTable.headerLength);
                            ist2.bigMetrics.height       = m_bufTable.GetByte (offset + indexSubTable.headerLength + 4);
                            ist2.bigMetrics.width        = m_bufTable.GetByte (offset + indexSubTable.headerLength + 5);
                            ist2.bigMetrics.horiBearingX = m_bufTable.GetSbyte(offset + indexSubTable.headerLength + 6);
                            ist2.bigMetrics.horiBearingY = m_bufTable.GetSbyte(offset + indexSubTable.headerLength + 7);
                            ist2.bigMetrics.horiAdvance  = m_bufTable.GetByte (offset + indexSubTable.headerLength + 8);
                            ist2.bigMetrics.vertBearingX = m_bufTable.GetSbyte(offset + indexSubTable.headerLength + 9);
                            ist2.bigMetrics.vertBearingY = m_bufTable.GetSbyte(offset + indexSubTable.headerLength + 10);
                            ist2.bigMetrics.vertAdvance  = m_bufTable.GetByte (offset + indexSubTable.headerLength + 11);
                            break;
                        }
                        case 3:
                        {
                            indexSubTable3 ist3 = new indexSubTable3(offset, m_bufTable, ista);
                            ist = ist3;
                            break;
                        }
                        case 4:
                        {
                            indexSubTable4 ist4 = new indexSubTable4(offset, m_bufTable, ista);
                            ist = ist4;
                            ist4.numGlyphs = m_bufTable.GetUint(offset + indexSubTable.headerLength);
                            break;
                        }
                        case 5:
                        {
                            indexSubTable5 ist5 = new indexSubTable5(offset, m_bufTable, ista);
                            ist = ist5;
                            ist5.bigMetrics = new Table_EBDT.bigGlyphMetrics();
                            ist5.imageSize               = m_bufTable.GetUint(offset + indexSubTable.headerLength);
                            ist5.bigMetrics.height       = m_bufTable.GetByte (offset + indexSubTable.headerLength + 4);
                            ist5.bigMetrics.width        = m_bufTable.GetByte (offset + indexSubTable.headerLength + 5);
                            ist5.bigMetrics.horiBearingX = m_bufTable.GetSbyte(offset + indexSubTable.headerLength + 6);
                            ist5.bigMetrics.horiBearingY = m_bufTable.GetSbyte(offset + indexSubTable.headerLength + 7);
                            ist5.bigMetrics.horiAdvance  = m_bufTable.GetByte (offset + indexSubTable.headerLength + 8);
                            ist5.bigMetrics.vertBearingX = m_bufTable.GetSbyte(offset + indexSubTable.headerLength + 9);
                            ist5.bigMetrics.vertBearingY = m_bufTable.GetSbyte(offset + indexSubTable.headerLength + 10);
                            ist5.bigMetrics.vertAdvance  = m_bufTable.GetByte (offset + indexSubTable.headerLength + 11);
                            ist5.numGlyphs               = m_bufTable.GetUint(offset + indexSubTable.headerLength + 12);
                            break;
                        }
                    }
                }

                return ist;
            }

            protected uint m_bstOffset;
            protected MBOBuffer m_bufTable;
        }

        public class sbitLineMetrics
        {
            public sbitLineMetrics(uint offset, MBOBuffer bufTable)
            {
                m_slmOffset = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                ascender              = 0,
                descender             = 1,
                widthMax              = 2,
                caretSlopeNumerator   = 3,
                caretSlopeDenominator = 4,
                caretOffset           = 5,
                minOriginSB           = 6,
                minAdvanceSB          = 7,
                maxBeforeBL           = 8,
                minAfterBL            = 9,
                pad1                  = 10,
                pad2                  = 11
            }

            public sbyte ascender
            {
                get {return m_bufTable.GetSbyte(m_slmOffset + (uint)FieldOffsets.ascender);}
            }

            public sbyte descender
            {
                get {return m_bufTable.GetSbyte(m_slmOffset + (uint)FieldOffsets.descender);}
            }

            public byte  widthMax
            {
                get {return m_bufTable.GetByte(m_slmOffset + (uint)FieldOffsets.widthMax);}
            }

            public sbyte caretSlopeNumerator
            {
                get {return m_bufTable.GetSbyte(m_slmOffset + (uint)FieldOffsets.caretSlopeNumerator);}
            }

            public sbyte caretSlopeDenominator
            {
                get {return m_bufTable.GetSbyte(m_slmOffset + (uint)FieldOffsets.caretSlopeDenominator);}
            }

            public sbyte caretOffset
            {
                get {return m_bufTable.GetSbyte(m_slmOffset + (uint)FieldOffsets.caretOffset);}
            }

            public sbyte minOriginSB
            {
                get {return m_bufTable.GetSbyte(m_slmOffset + (uint)FieldOffsets.minOriginSB);}
            }

            public sbyte minAdvanceSB
            {
                get {return m_bufTable.GetSbyte(m_slmOffset + (uint)FieldOffsets.minAdvanceSB);}
            }

            public sbyte maxBeforeBL
            {
                get {return m_bufTable.GetSbyte(m_slmOffset + (uint)FieldOffsets.maxBeforeBL);}
            }

            public sbyte minAfterBL
            {
                get {return m_bufTable.GetSbyte(m_slmOffset + (uint)FieldOffsets.minAfterBL);}
            }

            public sbyte pad1
            {
                get {return m_bufTable.GetSbyte(m_slmOffset + (uint)FieldOffsets.pad1);}
            }

            public sbyte pad2
            {
                get {return m_bufTable.GetSbyte(m_slmOffset + (uint)FieldOffsets.pad2);}
            }

            protected uint m_slmOffset;
            protected MBOBuffer m_bufTable;
        }

        public class indexSubTableArray
        {
            public indexSubTableArray(uint offset, MBOBuffer bufTable)
            {
                m_istaOffset = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                firstGlyphIndex                 = 0,
                lastGlyphIndex                  = 2,
                additionalOffsetToIndexSubtable = 4
            }

            public const ushort bufSize = 8;

            public ushort firstGlyphIndex
            {
                get {return m_bufTable.GetUshort(m_istaOffset + (uint)FieldOffsets.firstGlyphIndex);}
            }

            public ushort lastGlyphIndex
            {
                get {return m_bufTable.GetUshort(m_istaOffset + (uint)FieldOffsets.lastGlyphIndex);}
            }

            public uint additionalOffsetToIndexSubtable
            {
                get {return m_bufTable.GetUint(m_istaOffset + (uint)FieldOffsets.additionalOffsetToIndexSubtable);}
            }

            protected uint m_istaOffset;
            protected MBOBuffer m_bufTable;
        }

        public class indexSubHeader
        {
            public indexSubHeader(uint offsetIndexSubHeader, MBOBuffer  bufTable)
            {
                m_offsetIndexSubHeader = offsetIndexSubHeader;
                m_bufTable = bufTable;
            }


            public enum FieldOffsets
            {
                indexFormat = 0,
                imageFormat = 2,
                imageDataOffset = 4
            }

            public ushort indexFormat
            {
                get {return m_bufTable.GetUshort(m_offsetIndexSubHeader + (uint)FieldOffsets.indexFormat);}
            }

            public ushort imageFormat
            {
                get {return m_bufTable.GetUshort(m_offsetIndexSubHeader + (uint)FieldOffsets.imageFormat);}
            }

            public uint imageDataOffset
            {
                get {return m_bufTable.GetUint(m_offsetIndexSubHeader + (uint)FieldOffsets.imageDataOffset);}
            }

            protected uint m_offsetIndexSubHeader;
            protected MBOBuffer m_bufTable;
        }


        // indexSubTable: base class for various formats of index subtables

        public abstract class indexSubTable
        {
            public indexSubTable(uint indexSubTableOffset, MBOBuffer EBLCTableBuf, indexSubTableArray ista)
            {
                m_indexSubTableOffset = indexSubTableOffset;
                m_EBLCTableBuf = EBLCTableBuf;

                header = new indexSubHeader(indexSubTableOffset, EBLCTableBuf);
                m_ista = ista;
            }

            public abstract uint GetGlyphImageOffset(ushort idGlyph);

            public virtual uint GetGlyphImageLength(ushort idGlyph)
            {
                return GetGlyphImageOffset((ushort)(idGlyph+1)) - GetGlyphImageOffset(idGlyph);
            }


            public ushort GetImageFormat()
            {
                return header.imageFormat;
            }

            public indexSubTableArray GetIndexSubTableArray()
            {
                return m_ista;
            }

            public indexSubHeader header;
            public static uint headerLength = 8;


            internal uint m_indexSubTableOffset;
            internal MBOBuffer m_EBLCTableBuf;
            internal indexSubTableArray m_ista;
        }

        public class indexSubTable1: indexSubTable
        {
            public indexSubTable1(uint indexSubTableOffset, MBOBuffer EBLCTableBuf, indexSubTableArray ista) 
                : base(indexSubTableOffset, EBLCTableBuf, ista)
            {
            }

            public uint GetOffset(uint arrayIndex)
            {
                return m_EBLCTableBuf.GetUint(m_indexSubTableOffset + headerLength + arrayIndex*4);
            }

            public override uint GetGlyphImageOffset(ushort idGlyph)
            {
                uint nOffset = 0xffffffff;

                if (m_ista.firstGlyphIndex <= idGlyph && m_ista.lastGlyphIndex >= idGlyph)
                {
                    nOffset = GetOffset((uint)idGlyph-(uint)m_ista.firstGlyphIndex) + header.imageDataOffset;
                }

                return nOffset;
            }

        }

        public class indexSubTable2: indexSubTable
        {
            public indexSubTable2(uint indexSubTableOffset, MBOBuffer EBLCTableBuf, indexSubTableArray ista)
                : base(indexSubTableOffset, EBLCTableBuf, ista)
            {
            }


            public override uint GetGlyphImageOffset(ushort idGlyph)
            {
                uint nOffset = 0xffffffff;

                if (m_ista.firstGlyphIndex <= idGlyph && m_ista.lastGlyphIndex >= idGlyph)
                {
                    nOffset = header.imageDataOffset + imageSize*((uint)idGlyph-(uint)m_ista.firstGlyphIndex);
                }

                return nOffset;
            }

            public override uint GetGlyphImageLength(ushort idGlyph)
            {
                return imageSize;
            }


            public uint imageSize;
            public Table_EBDT.bigGlyphMetrics bigMetrics;
        }

        public class indexSubTable3: indexSubTable
        {
            public indexSubTable3(uint indexSubTableOffset, MBOBuffer EBLCTableBuf, indexSubTableArray ista)
                : base(indexSubTableOffset, EBLCTableBuf, ista)
            {
            }

            public uint GetOffset(uint arrayIndex)
            {
                return m_EBLCTableBuf.GetUshort(m_indexSubTableOffset + headerLength + arrayIndex*2);
            }

            public override uint GetGlyphImageOffset(ushort idGlyph)
            {
                uint nOffset = 0xffffffff;

                if (m_ista.firstGlyphIndex <= idGlyph && m_ista.lastGlyphIndex >= idGlyph)
                {
                    nOffset = GetOffset((uint)idGlyph-(uint)m_ista.firstGlyphIndex) + header.imageDataOffset;
                }

                return nOffset;
            }
        }

        public class indexSubTable4: indexSubTable
        {
            public indexSubTable4(uint indexSubTableOffset, MBOBuffer EBLCTableBuf, indexSubTableArray ista) 
                : base(indexSubTableOffset, EBLCTableBuf, ista)
            {
            }

            public class codeOffsetPair
            {
                public ushort glyphCode;
                public ushort offset;
            }

            public codeOffsetPair GetCodeOffsetPair(uint i)
            {
                codeOffsetPair cop = new codeOffsetPair();
                cop.glyphCode = m_EBLCTableBuf.GetUshort(m_indexSubTableOffset + headerLength + 4 + 4*i);
                cop.offset    = m_EBLCTableBuf.GetUshort(m_indexSubTableOffset + headerLength + 4 + 4*i + 2);
                return cop;
            }

            public override uint GetGlyphImageOffset(ushort idGlyph)
            {
                uint nOffset = 0xffffffff;

                if (m_ista.firstGlyphIndex <= idGlyph && m_ista.lastGlyphIndex >= idGlyph)
                {
                    for (uint i=0; i<numGlyphs; i++)
                    {
                        codeOffsetPair cop = GetCodeOffsetPair(i);
                        if (cop.glyphCode == idGlyph)
                        {
                            nOffset = cop.offset;
                            break;
                        }
                    }
                }

                return nOffset;
            }

            public uint numGlyphs;
        }

        public class indexSubTable5: indexSubTable
        {
            public indexSubTable5(uint indexSubTableOffset, MBOBuffer EBLCTableBuf, indexSubTableArray ista) 
                : base(indexSubTableOffset, EBLCTableBuf, ista)
            {
            }

            public uint imageSize;
            public Table_EBDT.bigGlyphMetrics bigMetrics;
            public uint numGlyphs;

            public ushort GetGlyphCode(uint i)
            {
                uint glyphCodeArrayOffset = m_indexSubTableOffset + headerLength + 4 + 8 + 4;
                return m_EBLCTableBuf.GetUshort(glyphCodeArrayOffset + i*2);
            }

            public override uint GetGlyphImageOffset(ushort idGlyph)
            {
                uint nOffset = 0xffffffff;

                if (m_ista.firstGlyphIndex <= idGlyph && m_ista.lastGlyphIndex >= idGlyph)
                {
                    for (uint i=0; i<numGlyphs; i++)
                    {
                        if (GetGlyphCode(i) == idGlyph)
                        {
                            nOffset = i*imageSize;
                            break;
                        }
                    }
                }

                return nOffset;
            }

            public override uint GetGlyphImageLength(ushort idGlyph)
            {
                return imageSize;
            }

        }


        /************************
         * accessors
         */


        public OTFixed version
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.version);}
        }

        // To be used by the cache to updated EDBT data
        public Table_EBDT getTableEDBT()
        {
            if( m_tableEDBT == null )
            {
                throw new ArgumentNullException( "Table EDBT has not been set." );
            }

            return m_tableEDBT;
        }

        public bool setTableEDBT( Table_EBDT tableEDBT )
        {
            bool bResult = true;

            if( tableEDBT == null )
            {
                bResult = false;
                throw new ArgumentNullException( "tableEDBT has not been set and is null." );
            }
            else
            {
                m_tableEDBT = tableEDBT;
            }

            return bResult;
        }

        public uint numSizes
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.numSizes);}
        }

        public bitmapSizeTable GetBitmapSizeTable(uint i)
        {
            bitmapSizeTable bst = null;

            if (i < numSizes)
            {
                uint bstOffset = (uint)FieldOffsets.FirstbitmapSizeTable + i*48;
                bst = new bitmapSizeTable(bstOffset, m_bufTable);
            }

            return bst;
        }

        public bitmapSizeTable FindBitmapSizeTable(byte ppemX, byte ppemY)
        {
            bitmapSizeTable bst = null;
            for (uint i=0; i<this.numSizes; i++)
            {
                bitmapSizeTable t = GetBitmapSizeTable(i);
                if (t.ppemX == ppemX && t.ppemY == ppemY)
                {
                    bst = t;
                    break;
                }
            }

            return bst;
        }

        public indexSubTableArray GetIndexSubTableArray(bitmapSizeTable bst, uint i)
        {
            indexSubTableArray ista = null;

            if (bst.indexSubTableArrayOffset < m_bufTable.GetLength())
            {
                uint istaOffset = bst.indexSubTableArrayOffset + i*8;
                if (istaOffset + indexSubTableArray.bufSize <= m_bufTable.GetLength())
                {
                    ista = new indexSubTableArray(istaOffset, m_bufTable);
                }
            }

            return ista;
        }

        public indexSubTableArray[] GetIndexSubTableArray(bitmapSizeTable bst)
        {
            indexSubTableArray[] ista = null;

            if (bst.indexSubTableArrayOffset < m_bufTable.GetLength())
            {
                ista = new indexSubTableArray[bst.numberOfIndexSubTables];

                for (uint i=0; i<bst.numberOfIndexSubTables; i++)
                {
                    ista[i] = GetIndexSubTableArray(bst, i);
                }
            }

            return ista;
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new EBLC_cache( this );
            }

            return m_cache;
        }
        
        public class EBLC_cache : DataCache
        {
            protected OTFixed m_version;
            protected uint m_numSizes;
            protected ArrayList m_bitmapSizeTables; // bitmapSizeTable[]
            protected Table_EBDT m_tableEBDT = null; 

            // constructor
            public EBLC_cache( Table_EBLC OwnerTable )
            {
                // copy the data from the owner table's MBOBuffer
                // and store it in the cache variables
                m_version = OwnerTable.version;
                m_numSizes = OwnerTable.numSizes;
                m_tableEBDT = OwnerTable.getTableEDBT();

                m_bitmapSizeTables = new ArrayList( (int)m_numSizes );

                for( uint i = 0; i < m_numSizes; i++ )
                {
                    bitmapSizeTable bst = OwnerTable.GetBitmapSizeTable( i );
                    m_bitmapSizeTables.Add( new bitmapSizeTableCache( OwnerTable, bst ));
                }

            }

            public bitmapSizeTableCache getBitmapSizeTableCache( int nIndex )
            {                
                bitmapSizeTableCache bstc = null;

                if( nIndex >= m_numSizes )
                {
                    throw new ArgumentOutOfRangeException( "nIndex is out of range." ); 
                }                    
                else
                {
                    bstc = (bitmapSizeTableCache)((bitmapSizeTableCache)m_bitmapSizeTables[nIndex]).Clone();    
                }

                return bstc;
            }

            public bool setBitmapSizeTableCache( int nIndex, bitmapSizeTableCache bstc )
            {
                bool bResult = true;

                if( bstc == null )
                {
                    bResult = false;
                    throw new ArgumentNullException( "Argument can not be null" ); 
                }
                else if( nIndex >= m_numSizes )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is out of range." ); 
                }
                else
                {
                    m_bitmapSizeTables[nIndex] = bstc.Clone();    
                    m_bDirty = true;
                }                    

                return bResult;
            }

            public bool addBitmapSizeTableCache( uint nIndex, bitmapSizeTableCache bstc )
            {
                bool bResult = true;

                if( bstc == null )
                {
                    bResult = false;
                    throw new ArgumentNullException( "Argument can not be null" ); 
                }
                else if( nIndex > m_numSizes )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is out of range." ); 
                }
                else
                {
                    m_bitmapSizeTables.Insert( (int)nIndex, bstc.Clone());
                    m_numSizes++;
                    m_bDirty = true;
                }                    

                return bResult;
            }

            public bool removeBitmapSizeTableCache( uint nIndex )
            {
                bool bResult = true;

                if( nIndex >= m_numSizes )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is out of range." ); 
                }
                if( m_numSizes == 1 )
                {
                    bResult = false;
                    throw new ArgumentException( "There has to be at least one BitmapSizeTable" ); 
                }
                else
                {
                    m_bitmapSizeTables.RemoveAt( (int)nIndex );
                    m_numSizes--;
                    m_bDirty = true;
                }                

                return bResult;
            }            

            public override OTTable GenerateTable()
            {
                uint nBufSize = (uint)Table_EBLC.FieldOffsets.FirstbitmapSizeTable;
        
                for( ushort i = 0; i < m_numSizes; i++ )
                {
                    bitmapSizeTableCache bstc = (bitmapSizeTableCache)m_bitmapSizeTables[i];
                    nBufSize += bitmapSizeTable.bufSize;
                    nBufSize += bstc.indexSubTablesSize;
                }

                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer( nBufSize );

                // Determine the size of the EBDTTable and create its buffer
                uint nEBDTBufSize = (uint)Table_EBDT.FieldOffsets.StartOfData;
                for( ushort i = 0; i < m_numSizes; i++ )
                {
                    bitmapSizeTableCache bstc = (bitmapSizeTableCache)m_bitmapSizeTables[i];
                    for( int ii = 0; ii < bstc.numberOfIndexSubTables; ii++ )
                    {
                        indexSubTableArrayCache istac = bstc.getIndexSubTableArrayCache( ii );
                        nEBDTBufSize += istac.indexSubTable.imageDataSize();
                    }
                }            

                // create a Motorola Byte Order buffer for the EBDT table
                MBOBuffer bufEBDT = new MBOBuffer( nEBDTBufSize );
                
                newbuf.SetFixed( m_version,        (uint)Table_EBLC.FieldOffsets.version );
                newbuf.SetUint( m_numSizes,        (uint)Table_EBLC.FieldOffsets.numSizes );

                //Set up initial offsets
                uint idxArrOffset = (uint)Table_EBLC.FieldOffsets.FirstbitmapSizeTable + (bitmapSizeTable.bufSize * m_numSizes);                
                uint imageDataOffset = (uint)Table_EBDT.FieldOffsets.StartOfData; //EBDTTable        

                for( ushort i = 0; i < m_numSizes; i++ )
                {
                    bitmapSizeTableCache bstc = (bitmapSizeTableCache)m_bitmapSizeTables[i];
                    //Set the offset to the bitmapSizeTable
                    uint bstOffset = (uint)(Table_EBLC.FieldOffsets.FirstbitmapSizeTable + (i * bitmapSizeTable.bufSize));

                    newbuf.SetUint( idxArrOffset, bstOffset );
                    newbuf.SetUint( bstc.indexSubTablesSize,        bstOffset +  (uint)bitmapSizeTable.FieldOffsets.indexTablesSize ); 
                    newbuf.SetUint( bstc.numberOfIndexSubTables,    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.numberOfIndexSubTables );
                    newbuf.SetUint( bstc.colorRef,                    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.colorRef );
                    // hori
                    newbuf.SetSbyte( bstc.hori.ascender,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.ascender );
                    newbuf.SetSbyte( bstc.hori.descender,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.descender );
                    newbuf.SetByte( bstc.hori.widthMax,                    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.widthMax );
                    newbuf.SetSbyte( bstc.hori.caretSlopeNumerator,        bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.caretSlopeNumerator );
                    newbuf.SetSbyte( bstc.hori.caretSlopeDenominator,    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.caretSlopeDenominator );
                    newbuf.SetSbyte( bstc.hori.caretOffset,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.caretOffset );
                    newbuf.SetSbyte( bstc.hori.minOriginSB,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.minOriginSB );
                    newbuf.SetSbyte( bstc.hori.minAdvanceSB,            bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.minAdvanceSB );
                    newbuf.SetSbyte( bstc.hori.maxBeforeBL,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.maxBeforeBL );
                    newbuf.SetSbyte( bstc.hori.minAfterBL,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.minAfterBL );
                    newbuf.SetSbyte( bstc.hori.pad1,                    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.pad1 );
                    newbuf.SetSbyte( bstc.hori.pad2,                    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.pad2 );
                    //vert
                    newbuf.SetSbyte( bstc.vert.ascender,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.ascender );
                    newbuf.SetSbyte( bstc.vert.descender,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.descender );
                    newbuf.SetByte( bstc.vert.widthMax,                    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.widthMax );
                    newbuf.SetSbyte( bstc.vert.caretSlopeNumerator,        bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.caretSlopeNumerator );
                    newbuf.SetSbyte( bstc.vert.caretSlopeDenominator,    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.caretSlopeDenominator );
                    newbuf.SetSbyte( bstc.vert.caretOffset,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.caretOffset );
                    newbuf.SetSbyte( bstc.vert.minOriginSB,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.minOriginSB );
                    newbuf.SetSbyte( bstc.vert.minAdvanceSB,            bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.minAdvanceSB );
                    newbuf.SetSbyte( bstc.vert.maxBeforeBL,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.maxBeforeBL );
                    newbuf.SetSbyte( bstc.vert.minAfterBL,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.minAfterBL );
                    newbuf.SetSbyte( bstc.vert.pad1,                    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.pad1 );
                    newbuf.SetSbyte( bstc.vert.pad2,                    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.hori + (uint)sbitLineMetrics.FieldOffsets.pad2 );
                    
                    newbuf.SetUshort( bstc.startGlyphIndex,    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.startGlyphIndex );
                    newbuf.SetUshort( bstc.endGlyphIndex,    bstOffset +  (uint)bitmapSizeTable.FieldOffsets.endGlyphIndex );
                    newbuf.SetByte( bstc.ppemX,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.ppemX );
                    newbuf.SetByte( bstc.ppemY,                bstOffset +  (uint)bitmapSizeTable.FieldOffsets.ppemY );
                    newbuf.SetByte( bstc.bitDepth,            bstOffset +  (uint)bitmapSizeTable.FieldOffsets.bitDepth );
                    newbuf.SetSbyte( bstc.flags,            bstOffset +  (uint)bitmapSizeTable.FieldOffsets.flags );                    

                    uint idxSubTableOffset = idxArrOffset + (bstc.numberOfIndexSubTables * indexSubTableArray.bufSize);
                    

                    // Write this bitmapSizeTable indexSubTableArray and indexSubTable                    
                    for( int ii = 0; ii < bstc.numberOfIndexSubTables; ii++ )
                    {
                        
                        // Write out the indexSubTableArray
                        indexSubTableArrayCache istac = bstc.getIndexSubTableArrayCache( ii );
                        newbuf.SetUshort( istac.firstGlyphIndex,            idxArrOffset + (uint)indexSubTableArray.FieldOffsets.firstGlyphIndex + (uint)(ii * indexSubTableArray.bufSize));
                        newbuf.SetUshort( istac.lastGlyphIndex,                idxArrOffset + (uint)indexSubTableArray.FieldOffsets.lastGlyphIndex + (uint)(ii * indexSubTableArray.bufSize));                        
                        newbuf.SetUint( (idxSubTableOffset - idxArrOffset),    idxArrOffset + (uint)indexSubTableArray.FieldOffsets.additionalOffsetToIndexSubtable + (uint)(ii * indexSubTableArray.bufSize));
                        
                        // Write out the indexSubTable, The header is the same for all indexFormats
                        newbuf.SetUshort( istac.indexSubTable.indexFormat,    idxSubTableOffset + (uint)indexSubHeader.FieldOffsets.indexFormat );
                        newbuf.SetUshort( istac.indexSubTable.imageFormat,    idxSubTableOffset + (uint)indexSubHeader.FieldOffsets.imageFormat );
                        newbuf.SetUint( imageDataOffset,                    idxSubTableOffset + (uint)indexSubHeader.FieldOffsets.imageDataOffset );                        
                        
                        uint imageOffset = 0;
                        switch( istac.indexSubTable.indexFormat )
                        {
                            case 1:
                            {                                
                                indexSubTableCache1 istc = (indexSubTableCache1)istac.indexSubTable;                                
                                for( ushort iii = istac.firstGlyphIndex; iii <= istac.lastGlyphIndex; iii++ )
                                {
                                    ushort nIndex = (ushort)(iii - istac.firstGlyphIndex);
                                    // offset + header length + (uint)offsetArray[iii]
                                    newbuf.SetUint( imageOffset, idxSubTableOffset + indexSubTable.headerLength + (uint)(nIndex * 4));                                
                                    
                                    // Write image data for this indexSubTable to EBDT buffer
                                    imageCache ic = istc.getImageCache( iii, istac.firstGlyphIndex );
                                    writeEBDTBuffer( bufEBDT, (imageDataOffset + imageOffset), ic, istac.indexSubTable.imageFormat );
                                    imageOffset += ic.imageDataSize();
                                }
                                // Add the last one so size can be determined
                                newbuf.SetUint( imageOffset, idxSubTableOffset + indexSubTable.headerLength + ((uint)(istac.lastGlyphIndex - istac.firstGlyphIndex + 1) * 4));
                                
                                break;
                            }
                            case 2:
                            {
                                indexSubTableCache2 istc = (indexSubTableCache2)istac.indexSubTable;                                
                                
                                // offset + header length 
                                newbuf.SetUint( istc.imageSize, idxSubTableOffset + indexSubTable.headerLength );
                                //BigMetrics, + 12 = indexSubTable.headerLength + (uint)imageSize
                                newbuf.SetByte( istc.bigMetrics.height,            idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.height );
                                newbuf.SetByte( istc.bigMetrics.width,            idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.width );
                                newbuf.SetSbyte( istc.bigMetrics.horiBearingX,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiBearingX );
                                newbuf.SetSbyte( istc.bigMetrics.horiBearingY,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiBearingY );
                                newbuf.SetByte( istc.bigMetrics.horiAdvance,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiAdvance );
                                newbuf.SetSbyte( istc.bigMetrics.vertBearingX,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertBearingX );
                                newbuf.SetSbyte( istc.bigMetrics.vertBearingY,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertBearingY );
                                newbuf.SetByte( istc.bigMetrics.vertAdvance,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertAdvance );

                                for( ushort iii = istac.firstGlyphIndex; iii <= istac.lastGlyphIndex; iii++ )
                                {                                
                                    // Write image data for this indexSubTable to EBDT buffer
                                    imageCache ic = istc.getImageCache( iii, istac.firstGlyphIndex );
                                    writeEBDTBuffer( bufEBDT, (imageDataOffset + imageOffset), ic, istac.indexSubTable.imageFormat );
                                    imageOffset += ic.imageDataSize();
                                }
                                break;
                            }
                            case 3:
                            {
                                indexSubTableCache3 istc = (indexSubTableCache3)istac.indexSubTable;                                
                                
                                for( ushort iii = istac.firstGlyphIndex; iii <= istac.lastGlyphIndex; iii++ )
                                {
                                    ushort nIndex = (ushort)(iii - istac.firstGlyphIndex);

                                    // offset + header length + ushort offsetArray[iii]
                                    newbuf.SetUshort( (ushort)imageOffset, idxSubTableOffset + indexSubTable.headerLength + (uint)(nIndex * 2 ));
                                    // Write image data for this indexSubTable to EBDT buffer
                                    imageCache ic = istc.getImageCache( iii, istac.firstGlyphIndex );
                                    writeEBDTBuffer( bufEBDT, (imageDataOffset + imageOffset), ic, istac.indexSubTable.imageFormat );
                                    imageOffset += ic.imageDataSize();
                                }
                                // Add the last one so size can be determined
                                newbuf.SetUshort( (ushort)imageOffset, idxSubTableOffset + indexSubTable.headerLength + ((uint)(istac.lastGlyphIndex - istac.firstGlyphIndex + 1) * 2 ));
                                break;
                            }
                            case 4:
                            {
                                indexSubTableCache4 istc = (indexSubTableCache4)istac.indexSubTable;                                
                                // offset + header length 
                                newbuf.SetUint( istc.numGlyphs, idxSubTableOffset + indexSubTable.headerLength );
                                for( ushort iii = 0; iii < istc.numGlyphs; iii++ )
                                {
                                    // offset + header length + (uint)numGlyphs + (4)codeOffsetPair[iii]
                                    newbuf.SetUshort( istc.getGlyphCode(iii), idxSubTableOffset + 12 + (uint)(iii * 4));
                                    newbuf.SetUshort( (ushort)imageOffset, idxSubTableOffset + 12 + (uint)(iii * 4) + 2 );
                                    // Write image data for this indexSubTable to EBDT buffer
                                    imageCache ic = istc.getImageCache( istc.getGlyphCode(iii));
                                    writeEBDTBuffer( bufEBDT, (imageDataOffset + imageOffset), ic, istac.indexSubTable.imageFormat );
                                    imageOffset += ic.imageDataSize();
                                }
                                // Add the last codeOffsetPair so size can be determined
                                newbuf.SetUshort( 0, idxSubTableOffset + 12 + (uint)(istc.numGlyphs * 4 ));
                                newbuf.SetUshort( (ushort)imageOffset, idxSubTableOffset + 12 + (uint)(istc.numGlyphs * 4 ) + 2 );                                
                                break;
                            }
                            case 5:
                            {
                                indexSubTableCache5 istc = (indexSubTableCache5)istac.indexSubTable;                                
                                // offset + header length 
                                newbuf.SetUint( istc.imageSize, idxSubTableOffset + indexSubTable.headerLength );

                                //BigMetrics, + 12 = indexSubTable.headerLength + (uint)imageSize
                                newbuf.SetByte( istc.bigMetrics.height,            idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.height );
                                newbuf.SetByte( istc.bigMetrics.width,            idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.width );
                                newbuf.SetSbyte( istc.bigMetrics.horiBearingX,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiBearingX );
                                newbuf.SetSbyte( istc.bigMetrics.horiBearingY,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiBearingY );
                                newbuf.SetByte( istc.bigMetrics.horiAdvance,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiAdvance );
                                newbuf.SetSbyte( istc.bigMetrics.vertBearingX,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertBearingX );
                                newbuf.SetSbyte( istc.bigMetrics.vertBearingY,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertBearingY );
                                newbuf.SetByte( istc.bigMetrics.vertAdvance,    idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertAdvance );

                                newbuf.SetUint( istc.numGlyphs, idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.bufSize );

                                for( ushort iii = 0; iii < istc.numGlyphs; iii++ )
                                {
                                    // offset + header length + (uint)imageSize + bigGlyphMetrics.bufSize + big(uint)numGlyphs + (ushort)glyphCodeArray[iii}
                                    newbuf.SetUshort( istc.getGlyphCode(iii), idxSubTableOffset + 12 + (uint)Table_EBDT.bigGlyphMetrics.bufSize + 4 + (uint)(iii * 2 ));
                                    
                                    // Write image data for this indexSubTable to EBDT buffer
                                    imageCache ic = istc.getImageCache( istc.getGlyphCode(iii));
                                    writeEBDTBuffer( bufEBDT, (imageDataOffset + imageOffset), ic, istac.indexSubTable.imageFormat );
                                    imageOffset += ic.imageDataSize();
                                }                            
                                break;
                            }
                        }

                        // update imageDataOffset for the next SubTable
                        imageDataOffset += imageOffset;

                        // This will take care of any byte boundaries pads required by some indexSubTables
                        idxSubTableOffset += istac.indexSubTable.indexSubTableSize();            
                    }

                    idxArrOffset += bstc.indexSubTablesSize;                    
                }    
                
                // Put the EBDT buf to that table
                Table_EBDT.EBDT_cache EBDTCache = (Table_EBDT.EBDT_cache)m_tableEBDT.GetCache();
                EBDTCache.setCache( bufEBDT );

                // put the buffer into a Table_EBLC object and return it
                Table_EBLC EBLCTable = new Table_EBLC( "EBLC", newbuf );
            
                return EBLCTable;
            }

            
            private void writeEBDTBuffer( MBOBuffer bufEBDT, uint imageDataOffset, imageCache ic, ushort imageFormat )
            {                
                switch( imageFormat )
                {
                    case 1:
                    {
                        imageCache1 ic1 = (imageCache1)ic;

                        bufEBDT.SetByte( ic1.smallMetrics.height,        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.height );
                        bufEBDT.SetByte( ic1.smallMetrics.width,        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.width );
                        bufEBDT.SetSbyte( ic1.smallMetrics.BearingX,    imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.BearingX );
                        bufEBDT.SetSbyte( ic1.smallMetrics.BearingY,    imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.BearingY );
                        bufEBDT.SetByte( ic1.smallMetrics.Advance,        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.Advance );
                        
                        System.Buffer.BlockCopy( ic1.imageData, 0, bufEBDT.GetBuffer(), (int)(imageDataOffset + Table_EBDT.smallGlyphMetrics.bufSize), ic1.imageData.Length );
                        break;                    
                    }
                    case 2:
                    {
                        imageCache2 ic2 = (imageCache2)ic;

                        bufEBDT.SetByte( ic2.smallMetrics.height,        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.height );
                        bufEBDT.SetByte( ic2.smallMetrics.width,        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.width );
                        bufEBDT.SetSbyte( ic2.smallMetrics.BearingX,    imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.BearingX );
                        bufEBDT.SetSbyte( ic2.smallMetrics.BearingY,    imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.BearingY );
                        bufEBDT.SetByte( ic2.smallMetrics.Advance,        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.Advance );
                        
                        System.Buffer.BlockCopy( ic2.imageData, 0, bufEBDT.GetBuffer(), (int)(imageDataOffset + Table_EBDT.smallGlyphMetrics.bufSize), ic2.imageData.Length );
                        break;
                    }
                    case 5:
                    {
                        imageCache5 ic5 = (imageCache5)ic;
                    
                        System.Buffer.BlockCopy( ic5.imageData, 0, bufEBDT.GetBuffer(), (int)imageDataOffset, ic5.imageData.Length );
                        break;
                    }
                    case 6:
                    {
                        imageCache6 ic6 = (imageCache6)ic;

                        bufEBDT.SetByte( ic6.bigMetrics.height,            imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.height );
                        bufEBDT.SetByte( ic6.bigMetrics.width,            imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.width );
                        bufEBDT.SetSbyte( ic6.bigMetrics.horiBearingX,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiBearingX );
                        bufEBDT.SetSbyte( ic6.bigMetrics.horiBearingY,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiBearingY );
                        bufEBDT.SetByte( ic6.bigMetrics.horiAdvance,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiAdvance );
                        bufEBDT.SetSbyte( ic6.bigMetrics.vertBearingX,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertBearingX );
                        bufEBDT.SetSbyte( ic6.bigMetrics.vertBearingY,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertBearingY );
                        bufEBDT.SetByte( ic6.bigMetrics.vertAdvance,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertAdvance );                        
                        
                        System.Buffer.BlockCopy( ic6.imageData, 0, bufEBDT.GetBuffer(), (int)(imageDataOffset + Table_EBDT.bigGlyphMetrics.bufSize), ic6.imageData.Length );
                        break;
                    }
                    case 7:
                    {
                        imageCache7 ic7 = (imageCache7)ic;

                        bufEBDT.SetByte( ic7.bigMetrics.height,            imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.height );
                        bufEBDT.SetByte( ic7.bigMetrics.width,            imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.width );
                        bufEBDT.SetSbyte( ic7.bigMetrics.horiBearingX,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiBearingX );
                        bufEBDT.SetSbyte( ic7.bigMetrics.horiBearingY,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiBearingY );
                        bufEBDT.SetByte( ic7.bigMetrics.horiAdvance,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiAdvance );
                        bufEBDT.SetSbyte( ic7.bigMetrics.vertBearingX,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertBearingX );
                        bufEBDT.SetSbyte( ic7.bigMetrics.vertBearingY,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertBearingY );
                        bufEBDT.SetByte( ic7.bigMetrics.vertAdvance,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertAdvance );                        
                        
                        System.Buffer.BlockCopy( ic7.imageData, 0, bufEBDT.GetBuffer(), (int)(imageDataOffset + Table_EBDT.bigGlyphMetrics.bufSize), ic7.imageData.Length );
                        break;
                    }
                    case 8:
                    {
                        imageCache8 ic8 = (imageCache8)ic;

                        bufEBDT.SetByte( ic8.smallMetrics.height,        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.height );
                        bufEBDT.SetByte( ic8.smallMetrics.width,        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.width );
                        bufEBDT.SetSbyte( ic8.smallMetrics.BearingX,    imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.BearingX );
                        bufEBDT.SetSbyte( ic8.smallMetrics.BearingY,    imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.BearingY );
                        bufEBDT.SetByte( ic8.smallMetrics.Advance,        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.FieldOffsets.Advance );                    
                        //pad
                        bufEBDT.SetByte( 0,                        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.bufSize );    
                        bufEBDT.SetUshort( ic8.numComponents,    imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.bufSize + 1 );    
                        
                        for( ushort i = 0; i < ic8.numComponents; i++ )
                        {
                            Table_EBDT.ebdtComponent ec = ic8.getComponent( i );
                            bufEBDT.SetUshort( ec.glyphCode,    imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.bufSize + 3 + (uint)(Table_EBDT.ebdtComponent.bufSize * i ));
                            bufEBDT.SetSbyte( ec.xOffset,        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.bufSize + 3 + (uint)(Table_EBDT.ebdtComponent.bufSize * i ) + 2 );
                            bufEBDT.SetSbyte( ec.yOffset,        imageDataOffset + (uint)Table_EBDT.smallGlyphMetrics.bufSize + 3 + (uint)(Table_EBDT.ebdtComponent.bufSize * i ) + 3 );    
                        }
                        break;
                    }
                    case 9:
                    {
                        imageCache9 ic9 = (imageCache9)ic;

                        bufEBDT.SetByte( ic9.bigMetrics.height,            imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.height );
                        bufEBDT.SetByte( ic9.bigMetrics.width,            imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.width );
                        bufEBDT.SetSbyte( ic9.bigMetrics.horiBearingX,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiBearingX );
                        bufEBDT.SetSbyte( ic9.bigMetrics.horiBearingY,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiBearingY );
                        bufEBDT.SetByte( ic9.bigMetrics.horiAdvance,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.horiAdvance );
                        bufEBDT.SetSbyte( ic9.bigMetrics.vertBearingX,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertBearingX );
                        bufEBDT.SetSbyte( ic9.bigMetrics.vertBearingY,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertBearingY );
                        bufEBDT.SetByte( ic9.bigMetrics.vertAdvance,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.FieldOffsets.vertAdvance );                            
                        
                        bufEBDT.SetUshort( ic9.numComponents,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.bufSize );    
                        
                        for( ushort i = 0; i < ic9.numComponents; i++ )
                        {
                            Table_EBDT.ebdtComponent ec = ic9.getComponent( i );
                            bufEBDT.SetUshort( ec.glyphCode,    imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.bufSize + 2 + (uint)(Table_EBDT.ebdtComponent.bufSize * i ));
                            bufEBDT.SetSbyte( ec.xOffset,        imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.bufSize + 2 + (uint)(Table_EBDT.ebdtComponent.bufSize * i ) + 2 );
                            bufEBDT.SetSbyte( ec.yOffset,        imageDataOffset + (uint)Table_EBDT.bigGlyphMetrics.bufSize + 2 + (uint)(Table_EBDT.ebdtComponent.bufSize * i ) + 3 );    
                        }
                        break;
                    }
                }                
            }

            public class bitmapSizeTableCache : ICloneable
            {
                protected ArrayList m_indexSubTableArray;
                protected uint m_numberOfIndexSubTables;
                protected uint m_colorRef;
                protected sbitLineMetricsCache m_hori;
                protected sbitLineMetricsCache m_vert;
                protected ushort m_startGlyphIndex;
                protected ushort m_endGlyphIndex;
                protected byte m_ppemX;
                protected byte m_ppemY;
                protected byte m_bitDepth;
                protected sbyte m_flags;

                public bitmapSizeTableCache()
                {
                }

                public bitmapSizeTableCache( Table_EBLC OwnerTable, bitmapSizeTable bst )
                {
                    m_numberOfIndexSubTables = bst.numberOfIndexSubTables;                    
                    m_indexSubTableArray = new ArrayList( (int)m_numberOfIndexSubTables );
                    indexSubTableArray[] ista = OwnerTable.GetIndexSubTableArray( bst );
                    for( int i = 0; i < m_numberOfIndexSubTables; i++ )
                    {                        
                        indexSubTableCache istc = getEBLCIndexSubTable( OwnerTable, bst, ista[i] );
                        indexSubTableArrayCache istac = new indexSubTableArrayCache( ista[i].firstGlyphIndex, ista[i].lastGlyphIndex, istc );
                        m_indexSubTableArray.Add( istac );

                        indexSubTable ic = bst.GetIndexSubTable(ista[i]);

                        //ista[i].additionalOffsetToIndexSubtable += bst.indexSubTableArrayOffset;                        
                    }

                    m_colorRef = bst.colorRef;
                    hori = sbitLineMetricsCache.FromSbitLineMetrics(bst.hori);
                    vert = sbitLineMetricsCache.FromSbitLineMetrics(bst.vert);
                    m_startGlyphIndex = bst.startGlyphIndex;
                    m_endGlyphIndex = bst.endGlyphIndex;
                    ppemX = bst.ppemX;
                    ppemY = bst.ppemY;
                    bitDepth = bst.bitDepth;
                    flags = bst.flags;        
                }                

                public uint indexSubTablesSize
                {
                    get
                    {
                        uint nSize = 0;

                        for( int i = 0; i < numberOfIndexSubTables; i++ )
                        {
                            nSize += indexSubTableArray.bufSize;
                            nSize += ((indexSubTableArrayCache)m_indexSubTableArray[i]).indexSubTable.indexSubTableSize();                    
                        }

                        return nSize;
                    }
                }

                // This can only be set by adding or removing Sub Tables
                public uint numberOfIndexSubTables
                {
                    get{ return m_numberOfIndexSubTables; }
                }                
                
                public uint colorRef
                {
                    get{ return m_colorRef; }
                    set{ m_colorRef = value; }
                }
                
                public sbitLineMetricsCache hori
                {
                    get{return (sbitLineMetricsCache)m_hori.Clone();}                    
                    set{m_hori = (sbitLineMetricsCache)value.Clone(); }                    
                }
                
                public sbitLineMetricsCache vert
                {
                    get{return (sbitLineMetricsCache)m_vert.Clone(); }                    
                    set{m_vert = (sbitLineMetricsCache)value.Clone(); }                    
                }

                // Can only be set by what is in the indexSubTableArray
                public ushort startGlyphIndex
                {
                    get{ return m_startGlyphIndex; }
                }

                // Can only be set by what is in the indexSubTableArray
                public ushort endGlyphIndex
                {
                    get{ return m_endGlyphIndex; }                    
                }

                public byte ppemX
                {
                    get{ return m_ppemX; }
                    set{ m_ppemX = value; }
                }

                public byte ppemY
                {
                    get{ return m_ppemY; }
                    set{ m_ppemY = value; }
                }

                public byte bitDepth
                {
                    get{ return m_bitDepth; }
                    set{ m_bitDepth = value; }
                }

                public sbyte flags
                {
                    get{ return m_flags; }
                    set{ m_flags = value; }
                }

                public indexSubTableArrayCache getIndexSubTableArrayCache( int nIndex )
                {
                    indexSubTableArrayCache istac = null;

                    if( nIndex >= numberOfIndexSubTables )
                    {
                        throw new ArgumentOutOfRangeException( "nIndex is out of range." ); 
                    }                    
                    else
                    {
                        istac = (indexSubTableArrayCache)((indexSubTableArrayCache)m_indexSubTableArray[nIndex]).Clone();    
                    }

                    return istac;
                }

                public bool setIndexSubTableArrayCache( int nIndex, indexSubTableArrayCache istac )
                {
                    bool bResult = true;

                    if( istac == null )
                    {
                        bResult = false;
                        throw new ArgumentNullException( "Argument can not be null" ); 
                    }
                    else if( nIndex >= numberOfIndexSubTables )
                    {
                        bResult = false;
                        throw new ArgumentOutOfRangeException( "nIndex is out of range." ); 
                    }
                    else
                    {
                        m_indexSubTableArray[nIndex] = istac.Clone();
                        checkAndSetGlyphRange();
                    }                    

                    return bResult;
                }

                public bool addIndexSubTableArrayCache( int nIndex, indexSubTableArrayCache istac )
                {
                    bool bResult = true;

                    if( istac == null )
                    {
                        bResult = false;
                        throw new ArgumentNullException( "Argument can not be null" ); 
                    }
                    else if( nIndex > numberOfIndexSubTables )
                    {
                        bResult = false;
                        throw new ArgumentOutOfRangeException( "nIndex is out of range." ); 
                    }
                    else
                    {
                        m_indexSubTableArray.Insert( nIndex, istac.Clone());    
                        m_numberOfIndexSubTables++;
                        checkAndSetGlyphRange();
                    }
                    

                    return bResult;
                }

                public bool removeIndexSubTableArrayCache( int nIndex )
                {
                    bool bResult = true;

                    if( nIndex >= numberOfIndexSubTables )
                    {
                        bResult = false;
                        throw new ArgumentOutOfRangeException( "nIndex is out of range." ); 
                    }
                    else
                    {
                        m_indexSubTableArray.RemoveAt( nIndex );
                        m_numberOfIndexSubTables--;
                        checkAndSetGlyphRange();
                    }
                    
                    return bResult;
                }

                public object Clone()
                {
                    bitmapSizeTableCache bstc = new bitmapSizeTableCache();
                    bstc.m_numberOfIndexSubTables = m_numberOfIndexSubTables;
                    bstc.m_indexSubTableArray = (ArrayList)m_indexSubTableArray.Clone();
                    
                    bstc.hori = hori;
                    bstc.vert = vert;
                    bstc.colorRef = colorRef;
                    bstc.hori = hori;
                    bstc.vert = vert;
                    bstc.m_startGlyphIndex = m_startGlyphIndex;
                    bstc.m_endGlyphIndex = m_endGlyphIndex;
                    bstc.ppemX = ppemX;
                    bstc.ppemY = ppemY;
                    bstc.bitDepth = bitDepth;
                    bstc.flags = flags;        
                    return null;
                }

                private void checkAndSetGlyphRange()
                {
                    ushort nLowestGlyph = ((indexSubTableArray)m_indexSubTableArray[0]).firstGlyphIndex;
                    ushort nHighestGlyph = 0;

                    for( int i = 0; i < numberOfIndexSubTables; i++ )
                    {
                        if( ((indexSubTableArray)m_indexSubTableArray[0]).firstGlyphIndex < nLowestGlyph )
                        {
                            nLowestGlyph = ((indexSubTableArray)m_indexSubTableArray[0]).firstGlyphIndex;
                        }

                        if( ((indexSubTableArray)m_indexSubTableArray[0]).lastGlyphIndex > nHighestGlyph )
                        {
                            nHighestGlyph = ((indexSubTableArray)m_indexSubTableArray[0]).lastGlyphIndex;
                        }
                    }
                    
                    m_startGlyphIndex = nLowestGlyph;
                    m_endGlyphIndex = nHighestGlyph;
                }

                private indexSubTableCache getEBLCIndexSubTable( Table_EBLC OwnerTable, bitmapSizeTable bst, indexSubTableArray ista )
                {
                    indexSubTable ist = bst.GetIndexSubTable(ista);
                    Table_EBDT tableEDBT = OwnerTable.getTableEDBT();
                    indexSubTableCache istc = null;                                        

                    switch( ist.header.indexFormat )
                    {
                        case 1:
                        {    
                            ArrayList cImageCache = new ArrayList();
                            for( uint i = ista.firstGlyphIndex; i <= ista.lastGlyphIndex; i++ )
                            {
                                cImageCache.Add( getEBDTImageFormat( tableEDBT, ist, i, ista.firstGlyphIndex ));
                            }
                            istc = new indexSubTableCache1( ist.header.indexFormat, ist.header.imageFormat, cImageCache );
                            break;
                        }
                        case 2:
                        {        
                            ArrayList cImageCache = new ArrayList();
                            for( uint i = ista.firstGlyphIndex; i <= ista.lastGlyphIndex; i++ )
                            {
                                cImageCache.Add( getEBDTImageFormat( tableEDBT, ist, i, ista.firstGlyphIndex ));
                            }
                            uint nImageSize = ((indexSubTable2)ist).imageSize;
                            Table_EBDT.bigGlyphMetrics bgm = ((indexSubTable2)ist).bigMetrics;
                            istc = new indexSubTableCache2( ist.header.indexFormat, ist.header.imageFormat, cImageCache, nImageSize, bgm );
                            break;
                        }
                        case 3:
                        {    
                            ArrayList cImageCache = new ArrayList();
                            for( uint i = ista.firstGlyphIndex; i <= ista.lastGlyphIndex; i++ )
                            {
                                cImageCache.Add( getEBDTImageFormat( tableEDBT, ist, i, ista.firstGlyphIndex ));
                            }
                            istc = new indexSubTableCache3( ist.header.indexFormat, ist.header.imageFormat, cImageCache );
                            break;
                        }
                        case 4:
                        {    
                            ArrayList cImageCache = new ArrayList();
                            ArrayList cGlyphCodes = new ArrayList();
                            
                            for( uint i = 0; i < ((indexSubTable4)ist).numGlyphs; i++ )
                            {
                                ushort nGlyphCode = ((indexSubTable4)ist).GetCodeOffsetPair( i ).glyphCode;
                                cGlyphCodes.Add( nGlyphCode );
                                cImageCache.Add( getEBDTImageFormat( tableEDBT, ist, nGlyphCode, ista.firstGlyphIndex ));
                            }
                            istc = new indexSubTableCache4( ist.header.indexFormat, ist.header.imageFormat, cImageCache, cGlyphCodes );
                            break;
                        }
                        case 5:
                        {                            
                            ArrayList cImageCache = new ArrayList();                            
                            uint nImageSize = ((indexSubTable5)ist).imageSize;
                            Table_EBDT.bigGlyphMetrics bgm = ((indexSubTable5)ist).bigMetrics;
                            ArrayList cGlyphCodes = new ArrayList();

                            for( uint i = 0; i < ((indexSubTable5)ist).numGlyphs; i++ )
                            {
                                ushort nGlyphCode = ((indexSubTable5)ist).GetGlyphCode( i );
                                cGlyphCodes.Add( nGlyphCode );
                                cImageCache.Add( getEBDTImageFormat( tableEDBT, ist, nGlyphCode, ista.firstGlyphIndex ));
                            }
                            istc = new indexSubTableCache5( ist.header.indexFormat, ist.header.imageFormat, cImageCache, nImageSize, bgm, cGlyphCodes );
                            break;
                        }                        
                        default:
                        {
                            Debug.Assert( false, "unsupported index format" );
                            break;
                        }
                        
                    }

                    return istc;
                }

                private imageCache getEBDTImageFormat( Table_EBDT tableEDBT , indexSubTable ist, uint nGlyphIndex, uint nStartGlyphIndex )
                {                    
                    imageCache ic = null;
                    
            
                    switch( ist.header.imageFormat )
                    {
                        case 1:
                        {
                            Table_EBDT.smallGlyphMetrics sgm = tableEDBT.GetSmallMetrics( ist, nGlyphIndex, nStartGlyphIndex );
                            byte[] bData = tableEDBT.GetImageData( ist, nGlyphIndex, nStartGlyphIndex );
                            ic = new imageCache1( sgm, bData );                            
                            break;
                        }
                        case 2:
                        {
                            Table_EBDT.smallGlyphMetrics sgm = tableEDBT.GetSmallMetrics( ist, nGlyphIndex, nStartGlyphIndex );
                            byte[] bData = tableEDBT.GetImageData( ist, nGlyphIndex, nStartGlyphIndex );
                            ic = new imageCache2( sgm, bData );
                            break;
                        }                        
                        case 5:
                        {
                            byte[] bData = tableEDBT.GetImageData( ist, nGlyphIndex, nStartGlyphIndex );
                            ic = new imageCache5( bData );                            
                            break;
                        }        
                        case 6:
                        {
                            Table_EBDT.bigGlyphMetrics bgm = tableEDBT.GetBigMetrics( ist, nGlyphIndex, nStartGlyphIndex );
                            byte[] bData = tableEDBT.GetImageData( ist, nGlyphIndex, nStartGlyphIndex );
                            ic = new imageCache6( bgm, bData );
                            break;
                        }        
                        case 7:
                        {
                            Table_EBDT.bigGlyphMetrics bgm = tableEDBT.GetBigMetrics( ist, nGlyphIndex, nStartGlyphIndex );
                            byte[] bData = tableEDBT.GetImageData( ist, nGlyphIndex, nStartGlyphIndex );
                            ic = new imageCache7( bgm, bData );                        
                            break;
                        }        
                        case 8:
                        {
                            Table_EBDT.smallGlyphMetrics sgm = tableEDBT.GetSmallMetrics( ist, nGlyphIndex, nStartGlyphIndex );
                            ushort nNumComp = tableEDBT.GetNumComponents( ist, nGlyphIndex, nStartGlyphIndex );
                            ArrayList components = new ArrayList( nNumComp );
                            for( uint i = 0; i < nNumComp; i++ )
                            {
                                components.Add( tableEDBT.GetComponent( ist, nGlyphIndex, nStartGlyphIndex, i ));                                
                            }
                            ic = new imageCache8( sgm, nNumComp, components );                    
                            break;
                        }        
                        case 9:
                        {
                            Table_EBDT.bigGlyphMetrics bgm = tableEDBT.GetBigMetrics( ist, nGlyphIndex, nStartGlyphIndex );
                            ushort nNumComp = tableEDBT.GetNumComponents( ist, nGlyphIndex, nStartGlyphIndex );
                            ArrayList components = new ArrayList( nNumComp );
                            for( uint i = 0; i < nNumComp; i++ )
                            {
                                components.Add( tableEDBT.GetComponent( ist, nGlyphIndex, nStartGlyphIndex, i ));                                
                            }
                            ic = new imageCache9( bgm, nNumComp, components );                        
                            break;
                        }        
                        default:
                            Debug.Assert( false, "unsupported image format" );
                            break;
                        
                    }

                    return ic;
                }                
            }

            public class indexSubTableArrayCache : ICloneable
            {
                protected ushort m_firstGlyphIndex;
                protected ushort m_lastGlyphIndex;
                protected indexSubTableCache m_indexSubTable;

                public indexSubTableArrayCache(  ushort nFirstGlyphIndex, ushort nLastGlyphIndex, indexSubTableCache istc )
                {
                    firstGlyphIndex = nFirstGlyphIndex;
                    lastGlyphIndex = nLastGlyphIndex;
                    indexSubTable = istc;
                }

                public ushort firstGlyphIndex
                {
                    get{ return m_firstGlyphIndex; }
                    set{ m_firstGlyphIndex = value; }
                }

                public ushort lastGlyphIndex
                {
                    get{ return m_lastGlyphIndex; }
                    set{ m_lastGlyphIndex = value; }
                }

                public indexSubTableCache indexSubTable
                {
                    get{ return (indexSubTableCache)m_indexSubTable.Clone(); }
                    set{ m_indexSubTable = (indexSubTableCache)value.Clone(); }
                }

                public object Clone()
                {
                    return new indexSubTableArrayCache( firstGlyphIndex, lastGlyphIndex, indexSubTable );
                }                
            }

            public abstract class indexSubTableCache : ICloneable
            {
                protected ushort m_indexFormat;
                protected ushort m_imageFormat;
                protected ArrayList m_imageCache; //imageCache[]

                public indexSubTableCache( ushort nIndexFormat, ushort nImageFormat, ArrayList cImageCache )
                {
                    indexFormat = nIndexFormat;
                    imageFormat = nImageFormat;
                    m_imageCache = (ArrayList)cImageCache.Clone();
                }
            
                public ushort indexFormat
                {
                    get{ return m_indexFormat; }
                    set{ m_indexFormat = value; }
                }

                public ushort imageFormat
                {
                    get{ return m_imageFormat; }
                    set{ m_imageFormat = value; }
                }                

                public uint imageDataSize()
                {
                    uint nSize = 0;

                    for( int i = 0; i < m_imageCache.Count; i++ )
                    {
                        imageCache ic = (imageCache)m_imageCache[i];
                        nSize += ic.imageDataSize();
                    }

                    return nSize;

                }

                public abstract uint indexSubTableSize();
                public abstract object Clone();
                
            }

            public class indexSubTableCache1 : indexSubTableCache
            {
                public indexSubTableCache1( ushort nIndexFormat, ushort nImageFormat, ArrayList cImageCache ) : base( nIndexFormat, nImageFormat, cImageCache )
                {
                }

                public imageCache getImageCache( ushort nGylphIndex, ushort nFirstGlyphIndex )
                {
                    imageCache ic = (imageCache)m_imageCache[nGylphIndex - nFirstGlyphIndex];
    
                    return ic;
                }

                override public uint indexSubTableSize()
                {
                    //header size + (number of images + 1 * uint)
                    uint nSize = (uint)(8 + ((m_imageCache.Count + 1) * 4));                                

                    return nSize;
                }

                override public object Clone()
                {
                    return new indexSubTableCache1( indexFormat, imageFormat, m_imageCache );
                }
            }

            public class indexSubTableCache2 : indexSubTableCache
            {
                protected uint m_imageSize;
                protected Table_EBDT.bigGlyphMetrics m_bigMetrics;

                public indexSubTableCache2( ushort nIndexFormat, ushort nImageFormat, ArrayList cImageCache, uint nImageSize, Table_EBDT.bigGlyphMetrics cBigMetrics ) : base( nIndexFormat, nImageFormat, cImageCache )
                {
                    imageSize = nImageSize;
                    bigMetrics = cBigMetrics;                    
                }
            
                public uint imageSize
                {
                    get{ return m_imageSize; }
                    set{ m_imageSize = value; }
                }

                public Table_EBDT.bigGlyphMetrics bigMetrics
                {
                    get{return (Table_EBDT.bigGlyphMetrics)m_bigMetrics.Clone();}
                    set{ m_bigMetrics = (Table_EBDT.bigGlyphMetrics)value.Clone(); }
                }

                public imageCache getImageCache( ushort nGylphIndex, ushort nFirstGlyphIndex )
                {
                    imageCache ic = (imageCache)m_imageCache[nGylphIndex - nFirstGlyphIndex];
    
                    return ic;
                }

                override public uint indexSubTableSize()
                {
                    //header size + imageSize(uint) + bigGlyphMetrics
                    uint nSize = 12 + Table_EBDT.bigGlyphMetrics.bufSize;

                    return nSize;
                }

                override public object Clone()
                {
                    return new indexSubTableCache2( indexFormat, imageFormat, m_imageCache, imageSize, bigMetrics );
                }

            }

            public class indexSubTableCache3 : indexSubTableCache
            {
                public indexSubTableCache3( ushort nIndexFormat, ushort nImageFormat, ArrayList cImageCache ) : base( nIndexFormat, nImageFormat, cImageCache )
                {
                }

                public imageCache getImageCache( ushort nGylphIndex, ushort nFirstGlyphIndex )
                {
                    imageCache ic = (imageCache)m_imageCache[nGylphIndex - nFirstGlyphIndex];
    
                    return ic;
                }

                override public uint indexSubTableSize()
                {
                    //header size + (number of images * ushort) + 1
                    uint nSize = (uint)(8 + ((m_imageCache.Count + 1) * 2));
                    
                    return nSize;
                }

                override public object Clone()
                {
                    return new indexSubTableCache3( indexFormat, imageFormat, m_imageCache );
                }
            }

            public class indexSubTableCache4 : indexSubTableCache
            {
                protected ArrayList m_glyphCode;

                public indexSubTableCache4( ushort nIndexFormat, ushort nImageFormat, ArrayList cImageCache, ArrayList cGlyphCode ) : base( nIndexFormat, nImageFormat, cImageCache )
                {
                    m_glyphCode = (ArrayList)cGlyphCode.Clone();                    
                }

                public uint numGlyphs
                {
                    get{ return (uint)m_glyphCode.Count; }
                }

                public ushort getGlyphCode( ushort nIndex ) 
                {                    
                    return (ushort)m_glyphCode[nIndex];                    
                }

                public imageCache getImageCache( ushort nGylphCode )
                {
                    imageCache ic = null;
                    for( ushort i = 0; i < numGlyphs; i++ )
                    {
                        if( nGylphCode == getGlyphCode( i ))
                        {
                            ic = (imageCache)m_imageCache[i];
                        }
                    }    
                    return ic;
                }

                override public uint indexSubTableSize()
                {
                    //header size + numGlyphs(uint) + (numGlyphs * size of codeOffset pair)
                    uint nSize = (uint)(12 + ((numGlyphs + 1) * 4));
                    
                    return nSize;
                }

                override public object Clone()
                {
                    return new indexSubTableCache4( indexFormat, imageFormat, m_imageCache, m_glyphCode );
                }
            }

            public class indexSubTableCache5 : indexSubTableCache
            {
                protected uint m_imageSize;
                protected Table_EBDT.bigGlyphMetrics m_bigMetrics;
                protected ArrayList m_glyphCode;

                public indexSubTableCache5( ushort nIndexFormat, ushort nImageFormat, ArrayList cImageCache, uint nImageSize, Table_EBDT.bigGlyphMetrics cBigMetrics, ArrayList cGlyphCode ) : base( nIndexFormat, nImageFormat, cImageCache )
                {
                    imageSize = nImageSize;
                    bigMetrics = cBigMetrics;                
                    m_glyphCode = (ArrayList)cGlyphCode.Clone();
                }
                
            
                public uint imageSize
                {
                    get{ return m_imageSize; }
                    set{ m_imageSize = value; }
                }

                public Table_EBDT.bigGlyphMetrics bigMetrics
                {
                    get{ return (Table_EBDT.bigGlyphMetrics)m_bigMetrics.Clone(); }
                    set{ m_bigMetrics = (Table_EBDT.bigGlyphMetrics)value.Clone(); }
                }

                public uint numGlyphs
                {
                    get{ return (uint)m_glyphCode.Count; }
                }

                public ushort getGlyphCode( ushort nIndex ) 
                {                    
                    return (ushort)m_glyphCode[nIndex];                    
                }

                public imageCache getImageCache( ushort nGylphCode )
                {
                    imageCache ic = null;
                    for( ushort i = 0; i < numGlyphs; i++ )
                    {
                        if( nGylphCode == getGlyphCode( i ))
                        {
                            ic = (imageCache)m_imageCache[i];
                        }
                    }    
                    return ic;
                }

                override public uint indexSubTableSize()
                {
                    //header size + imageSize(uint) + bigGlyphMetrics + numGlyphs(uint) + (numGlyphs * ushort)
                    uint nSize = (uint)(12 + Table_EBDT.bigGlyphMetrics.bufSize + 4 + (m_glyphCode.Count * 2));                
                    
                    return nSize;
                }

                override public object Clone()
                {
                    return new indexSubTableCache5( indexFormat, imageFormat, m_imageCache, imageSize, bigMetrics, m_glyphCode );
                }
            }

            public abstract class imageCache : ICloneable
            {
                public abstract uint imageDataSize();
                public abstract object Clone();                
            }

            public class imageCache1 : imageCache
            {
                protected Table_EBDT.smallGlyphMetrics m_smallMetrics;
                protected byte[] m_imageData;

                public imageCache1( Table_EBDT.smallGlyphMetrics cSmallMetrics, byte[] bImageData )
                {                        
                    smallMetrics = cSmallMetrics;
                    imageData = bImageData;
                }

                public Table_EBDT.smallGlyphMetrics smallMetrics
                {
                    get{ return (Table_EBDT.smallGlyphMetrics)m_smallMetrics.Clone(); }
                    set{ m_smallMetrics = (Table_EBDT.smallGlyphMetrics)value.Clone(); }
                }

                public byte[] imageData
                {
                    get{ return ( byte[])m_imageData.Clone(); }
                    set{ m_imageData = ( byte[])value.Clone(); }
                }

                override public uint imageDataSize()
                {
                    uint nSize = Table_EBDT.smallGlyphMetrics.bufSize;
                    nSize += (uint)m_imageData.Length;

                    return nSize;
                }

                override public object Clone()
                {
                    return new imageCache1( smallMetrics, imageData );
                }
            }

            public class imageCache2 : imageCache
            {
                protected Table_EBDT.smallGlyphMetrics m_smallMetrics;
                protected byte[] m_imageData;

                public imageCache2( Table_EBDT.smallGlyphMetrics cSmallMetrics, byte[] bImageData )
                {                        
                    smallMetrics = cSmallMetrics;
                    imageData = bImageData;
                }

                public Table_EBDT.smallGlyphMetrics smallMetrics
                {
                    get{ return (Table_EBDT.smallGlyphMetrics)m_smallMetrics.Clone(); }
                    set{ m_smallMetrics = (Table_EBDT.smallGlyphMetrics)value.Clone(); }
                }

                public byte[] imageData
                {
                    get{ return ( byte[])m_imageData.Clone(); }
                    set{ m_imageData = ( byte[])value.Clone(); }
                }

                override public uint imageDataSize()
                {
                    uint nSize = Table_EBDT.smallGlyphMetrics.bufSize;
                    nSize = (uint)m_imageData.Length;

                    return nSize;
                }

                override public object Clone()
                {
                    return new imageCache2( smallMetrics, imageData );
                }
            }

        
            public class imageCache5 : imageCache
            {
                protected byte[] m_imageData;

                public imageCache5( byte[] bImageData )
                {                    
                    imageData = bImageData;                    
                }
            
                public byte[] imageData
                {
                    get{ return ( byte[])m_imageData.Clone(); }
                    set{ m_imageData = ( byte[])value.Clone(); }
                }

                override public uint imageDataSize()
                {
                    uint nSize = (uint)m_imageData.Length;

                    return nSize;
                }

                override public object Clone()
                {
                    return new imageCache5( imageData );
                }
            }

            public class imageCache6 : imageCache
            {
                protected Table_EBDT.bigGlyphMetrics m_bigMetrics;
                protected byte[] m_imageData;

                public imageCache6( Table_EBDT.bigGlyphMetrics cBigMetrics, byte[] bImageData )
                {                    
                    bigMetrics = cBigMetrics;
                    imageData = bImageData;
                }

                public Table_EBDT.bigGlyphMetrics bigMetrics
                {
                    get{ return (Table_EBDT.bigGlyphMetrics)m_bigMetrics.Clone(); }
                    set{ m_bigMetrics = (Table_EBDT.bigGlyphMetrics)value.Clone(); }
                }

                public byte[] imageData
                {
                    get{ return ( byte[])m_imageData.Clone(); }
                    set{ m_imageData = ( byte[])value.Clone(); }
                }

                override public uint imageDataSize()
                {
                    uint nSize = Table_EBDT.bigGlyphMetrics.bufSize;
                    nSize += (uint)m_imageData.Length;

                    return nSize;
                }

                override public object Clone()
                {
                    return new imageCache6( bigMetrics, imageData );
                }
            }

            public class imageCache7 : imageCache
            {
                protected Table_EBDT.bigGlyphMetrics m_bigMetrics;
                protected byte[] m_imageData;

                public imageCache7( Table_EBDT.bigGlyphMetrics cBigMetrics, byte[] bImageData )
                {    
                    bigMetrics = cBigMetrics;
                    imageData = bImageData;
                }

                public Table_EBDT.bigGlyphMetrics bigMetrics
                {
                    get{ return (Table_EBDT.bigGlyphMetrics)m_bigMetrics.Clone(); }
                    set{ m_bigMetrics = (Table_EBDT.bigGlyphMetrics)value.Clone(); }
                }

                public byte[] imageData
                {
                    get{ return ( byte[])m_imageData.Clone(); }
                    set{ m_imageData = ( byte[])value.Clone(); }
                }

                override public uint imageDataSize()
                {
                    uint nSize = Table_EBDT.bigGlyphMetrics.bufSize;
                    nSize += (uint)m_imageData.Length;

                    return nSize;
                }

                override public object Clone()
                {
                    return new imageCache7( bigMetrics, imageData );
                }
            }

            public class imageCache8 : imageCache
            {
                protected Table_EBDT.smallGlyphMetrics m_smallMetrics;
                //protected byte m_pad;
                protected ushort m_numComponents;
                protected ArrayList m_componentArray; //Table_EBDT.ebdtComponent[]

                public imageCache8( Table_EBDT.smallGlyphMetrics cSmallMetrics, ushort nNumComponents, ArrayList cComponentArray )
                {                    
                    smallMetrics = cSmallMetrics;
                    //pad = bPad;
                    numComponents = nNumComponents;
                    m_componentArray = (ArrayList)cComponentArray.Clone();
                }

                public Table_EBDT.smallGlyphMetrics smallMetrics
                {
                    get{ return (Table_EBDT.smallGlyphMetrics)m_smallMetrics.Clone(); }
                    set{ m_smallMetrics = (Table_EBDT.smallGlyphMetrics)value.Clone(); }
                }

                /*
                public byte pad
                {
                    get{ return m_pad; }
                    set{m_pad = value; }
                }
                */

                public ushort numComponents
                {
                    get{ return m_numComponents; }
                    set{ m_numComponents = value; }
                }

                public Table_EBDT.ebdtComponent getComponent( ushort nIndex )
                {
                    Table_EBDT.ebdtComponent ec = (Table_EBDT.ebdtComponent)m_componentArray[nIndex];
                    
                    return ec;
                }

                override public uint imageDataSize()
                {
                    uint nSize = Table_EBDT.smallGlyphMetrics.bufSize;
                    nSize += 1; //pad
                    nSize += 2; //ushort (numComponents)
                    nSize += (uint)(numComponents * Table_EBDT.ebdtComponent.bufSize);

                    return nSize;
                }

                override public object Clone()
                {
                    return new imageCache8( smallMetrics, numComponents, m_componentArray );
                }
            }

            public class imageCache9 : imageCache
            {
                protected Table_EBDT.bigGlyphMetrics m_bigMetrics;        
                protected ushort m_numComponents;
                protected ArrayList m_componentArray; //Table_EBDT.ebdtComponent[]

                public imageCache9( Table_EBDT.bigGlyphMetrics cBigMetrics, ushort nNumComponents, ArrayList cComponentArray )
                {                    
                    bigMetrics = cBigMetrics;
                    numComponents = nNumComponents;
                    m_componentArray = (ArrayList)cComponentArray.Clone();
                }

                public Table_EBDT.bigGlyphMetrics bigMetrics
                {
                    get{ return (Table_EBDT.bigGlyphMetrics)m_bigMetrics.Clone(); }
                    set{ m_bigMetrics = (Table_EBDT.bigGlyphMetrics)value.Clone(); }
                }                

                public ushort numComponents
                {
                    get{ return m_numComponents; }
                    set{ m_numComponents = value; }
                }

                public Table_EBDT.ebdtComponent getComponent( ushort nIndex )
                {
                    Table_EBDT.ebdtComponent ec = (Table_EBDT.ebdtComponent)m_componentArray[nIndex];
                    
                    return ec;
                }

                override public uint imageDataSize()
                {
                    uint nSize = Table_EBDT.bigGlyphMetrics.bufSize;
                    nSize += 2; //ushort (numComponents)
                    nSize += (uint)(numComponents * Table_EBDT.ebdtComponent.bufSize);

                    return nSize;
                }

                override public object Clone()
                {
                    return new imageCache9( bigMetrics, numComponents, m_componentArray );
                }

            }    

            public class sbitLineMetricsCache : ICloneable
            {
                public sbyte ascender;
                public sbyte descender;
                public byte  widthMax;
                public sbyte caretSlopeNumerator;
                public sbyte caretSlopeDenominator;
                public sbyte caretOffset;
                public sbyte minOriginSB;
                public sbyte minAdvanceSB;
                public sbyte maxBeforeBL;
                public sbyte minAfterBL;
                public sbyte pad1;
                public sbyte pad2;

                static public sbitLineMetricsCache FromSbitLineMetrics(sbitLineMetrics slm)
                {
                    sbitLineMetricsCache slmc = new sbitLineMetricsCache();

                    slmc.ascender              = slm.ascender;
                    slmc.descender             = slm.descender;
                    slmc.widthMax              = slm.widthMax;
                    slmc.caretSlopeNumerator   = slm.caretSlopeNumerator;
                    slmc.caretSlopeDenominator = slm.caretSlopeDenominator;
                    slmc.caretOffset           = slm.caretOffset;
                    slmc.minOriginSB           = slm.minOriginSB;
                    slmc.minAdvanceSB          = slm.minAdvanceSB;
                    slmc.maxBeforeBL           = slm.maxBeforeBL;
                    slmc.minAfterBL            = slm.minAfterBL;
                    slmc.pad1                  = slm.pad1;
                    slmc.pad2                  = slm.pad2;

                    return slmc;
                }

                public object Clone()
                {
                    sbitLineMetricsCache slmc = new sbitLineMetricsCache();

                    slmc.ascender              = ascender;
                    slmc.descender             = descender;
                    slmc.widthMax              = widthMax;
                    slmc.caretSlopeNumerator   = caretSlopeNumerator;
                    slmc.caretSlopeDenominator = caretSlopeDenominator;
                    slmc.caretOffset           = caretOffset;
                    slmc.minOriginSB           = minOriginSB;
                    slmc.minAdvanceSB          = minAdvanceSB;
                    slmc.maxBeforeBL           = maxBeforeBL;
                    slmc.minAfterBL            = minAfterBL;
                    slmc.pad1                  = pad1;
                    slmc.pad2                  = pad2;

                    return slmc;
                }
            }


        }
    }
}
