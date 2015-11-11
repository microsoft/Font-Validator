using System;
using System.Diagnostics;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_EBDT.
    /// </summary>
    public class Table_EBDT : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_EBDT(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            version     = 0,
            StartOfData = 4
        }


        /************************
         * internal classes
         */

        public class bigGlyphMetrics : ICloneable
        {
            public enum FieldOffsets
            {
                height       = 0,
                width        = 1,
                horiBearingX = 2,
                horiBearingY = 3,
                horiAdvance  = 4,
                vertBearingX = 5,
                vertBearingY = 6,
                vertAdvance  = 7,
            }

            public const ushort bufSize = 8;

            public byte  height;
            public byte  width;
            public sbyte horiBearingX;
            public sbyte horiBearingY;
            public byte  horiAdvance;
            public sbyte vertBearingX;
            public sbyte vertBearingY;
            public byte  vertAdvance;

            public object Clone()
            {
                bigGlyphMetrics bgm = new bigGlyphMetrics();
                bgm.height = height;
                bgm.width = width;
                bgm.horiBearingX = horiBearingX;
                bgm.horiBearingY = horiBearingY;
                bgm.horiAdvance = horiAdvance;
                bgm.vertBearingX = vertBearingX;
                bgm.vertBearingY = vertBearingY;
                bgm.vertAdvance = vertAdvance;

                return bgm;
            }
        }

        public class smallGlyphMetrics : ICloneable
        {
            public enum FieldOffsets
            {
                height   = 0,
                width    = 1,
                BearingX = 2,
                BearingY = 3,
                Advance  = 4,
            }
            
            public const ushort bufSize = 5;

            public byte  height;
            public byte  width;
            public sbyte BearingX;
            public sbyte BearingY;
            public byte  Advance;

            public object Clone()
            {
                smallGlyphMetrics sgm = new smallGlyphMetrics();
                sgm.height = height;
                sgm.width = width;
                sgm.BearingX = BearingX;
                sgm.BearingY = BearingY;
                sgm.Advance = Advance;
            

                return sgm;
            }
        }

        public class ebdtComponent : ICloneable
        {
            public enum FieldOffsets
            {
                glyphCode = 0,
                xOffset   = 2,
                yOffset   = 3
            }

            public const ushort bufSize = 4;

            public ushort glyphCode;
            public sbyte  xOffset;
            public sbyte  yOffset;

            public object Clone()
            {
                ebdtComponent ec = new ebdtComponent();
                ec.glyphCode = glyphCode;
                ec.xOffset = xOffset;
                ec.yOffset = yOffset;

                return ec;                
            }
        }

        /************************
         * accessors
         */
        
        public OTFixed version
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.version);}
        }

        public smallGlyphMetrics GetSmallMetrics( Table_EBLC.indexSubTable cIndexSubTable, uint nGlyphIndex, uint nStartGlyphIndex )
        {
            smallGlyphMetrics sgm = null;
            int nIndexFormat = cIndexSubTable.header.indexFormat;
            int nImageFormat = cIndexSubTable.header.imageFormat;

            Debug.Assert( nImageFormat == 1 || nImageFormat == 2 || nImageFormat == 8);

            // These images all have the same metrics as described in the indexSubTable so should have the proper image format
            // Debug.Assert( nIndexFormat != 2 && nIndexFormat != 5 ); 
            // commented out the above assert because batang.ttc violates this too much

            if (nImageFormat == 1 || nImageFormat == 2 || nImageFormat == 8)
            {
                if( nGlyphIndex >= nStartGlyphIndex )
                {
                    //EBDT table starts off with a version data so it would never be possible for this to be set to 0
                    uint nImageFormatOffset = getImageFormatOffset( cIndexSubTable, nGlyphIndex, nStartGlyphIndex );                    
                    
                    if( nImageFormatOffset != 0 )
                    {
                        // All of the supported image formats start with this data first
                        sgm = new smallGlyphMetrics();
                        sgm.height   = m_bufTable.GetByte( nImageFormatOffset + (uint)smallGlyphMetrics.FieldOffsets.height );
                        sgm.width    = m_bufTable.GetByte( nImageFormatOffset + (uint)smallGlyphMetrics.FieldOffsets.width );
                        sgm.BearingX = m_bufTable.GetSbyte( nImageFormatOffset + (uint)smallGlyphMetrics.FieldOffsets.BearingX );
                        sgm.BearingY = m_bufTable.GetSbyte( nImageFormatOffset + (uint)smallGlyphMetrics.FieldOffsets.BearingY );
                        sgm.Advance  = m_bufTable.GetByte( nImageFormatOffset + (uint)smallGlyphMetrics.FieldOffsets.Advance );
                    }
                }
            }

            return sgm;
        }

        public bigGlyphMetrics GetBigMetrics( Table_EBLC.indexSubTable cIndexSubTable, uint nGlyphIndex, uint nStartGlyphIndex )
        {
            bigGlyphMetrics bgm = null;
            int nIndexFormat = cIndexSubTable.header.indexFormat;
            int nImageFormat = cIndexSubTable.header.imageFormat;

            Debug.Assert(nImageFormat == 6 || nImageFormat == 7 || nImageFormat == 9);

            // These images all have the same metrics as described in the indexSubTable so should have the proper image format
            //Debug.Assert( nIndexFormat != 2 && nIndexFormat != 5 );

            if (nImageFormat == 6 || nImageFormat == 7 || nImageFormat == 9)
            {
                
                if( nGlyphIndex >= nStartGlyphIndex )
                {                    
                    try
                    {
                        uint nImageFormatOffset = getImageFormatOffset( cIndexSubTable, nGlyphIndex, nStartGlyphIndex );

                        if( nImageFormatOffset != 0 && nImageFormatOffset + bigGlyphMetrics.bufSize <= m_bufTable.GetLength())
                        {
                            // All of the supported image formats start with this data first
                            bgm = new bigGlyphMetrics();
                            bgm.height       = m_bufTable.GetByte ( nImageFormatOffset + (uint)bigGlyphMetrics.FieldOffsets.height );
                            bgm.width        = m_bufTable.GetByte ( nImageFormatOffset + (uint)bigGlyphMetrics.FieldOffsets.width );
                            bgm.horiBearingX = m_bufTable.GetSbyte( nImageFormatOffset + (uint)bigGlyphMetrics.FieldOffsets.horiBearingX );
                            bgm.horiBearingY = m_bufTable.GetSbyte( nImageFormatOffset + (uint)bigGlyphMetrics.FieldOffsets.horiBearingY );
                            bgm.horiAdvance  = m_bufTable.GetByte ( nImageFormatOffset + (uint)bigGlyphMetrics.FieldOffsets.horiAdvance );
                            bgm.vertBearingX = m_bufTable.GetSbyte( nImageFormatOffset + (uint)bigGlyphMetrics.FieldOffsets.vertBearingX );
                            bgm.vertBearingY = m_bufTable.GetSbyte( nImageFormatOffset + (uint)bigGlyphMetrics.FieldOffsets.vertBearingY );
                            bgm.vertAdvance  = m_bufTable.GetByte ( nImageFormatOffset + (uint)bigGlyphMetrics.FieldOffsets.vertAdvance );
                        }
                    }
                    catch(Exception)
                    {
                        bgm = null;
                    }
                }
            }

            return bgm;
        }

        public ushort GetNumComponents( Table_EBLC.indexSubTable cIndexSubTable, uint nGlyphIndex, uint nStartGlyphIndex )
        {
            ushort nNumComponents = 0;
            int nIndexFormat = cIndexSubTable.header.indexFormat;
            int nImageFormat = cIndexSubTable.header.imageFormat;

            Debug.Assert(nImageFormat == 8 || nImageFormat == 9);

            // These images all have the same metrics as described in the indexSubTable so should have the proper image format
            // Debug.Assert( nIndexFormat != 2 && nIndexFormat != 5 );
            // commented out the above assert because batang.ttc violates this too much

            if (nImageFormat == 8 || nImageFormat == 9)
            {
                if( nGlyphIndex >= nStartGlyphIndex )
                {
                    uint nImageFormatOffset = getImageFormatOffset( cIndexSubTable, nGlyphIndex, nStartGlyphIndex );
                
                    if( nImageFormatOffset != 0 )
                    {
                        if( nImageFormat == 8 )
                        {
                            nNumComponents = m_bufTable.GetUshort( (uint)(nImageFormatOffset + smallGlyphMetrics.bufSize + 1));
                        }
                        else // nImageFormat = 9
                        {
                            nNumComponents = m_bufTable.GetUshort( (uint)(nImageFormatOffset + bigGlyphMetrics.bufSize));
                        }                    
                    }    
                }
            }

            return nNumComponents;
        }

        public ebdtComponent GetComponent( Table_EBLC.indexSubTable cIndexSubTable, uint nGlyphIndex, uint nStartGlyphIndex, uint nComponent )
        {
            ebdtComponent ebdtc = null;
            int nIndexFormat = cIndexSubTable.header.indexFormat;
            int nImageFormat = cIndexSubTable.header.imageFormat;

            Debug.Assert( nIndexFormat != 2 && nIndexFormat != 5 );

            // These images all have the same metrics as described in the indexSubTable so should have the proper image format
            Debug.Assert( nIndexFormat != 3 && nIndexFormat != 5 ); 

            if (nImageFormat == 8 || nImageFormat == 9)
            {                
                if( nGlyphIndex >= nStartGlyphIndex )
                {
                    uint nImageFormatOffset = getImageFormatOffset( cIndexSubTable, nGlyphIndex, nStartGlyphIndex );
                    
                    if( nImageFormatOffset != 0 )
                    {
                        ebdtc = new ebdtComponent();
                        if( nImageFormat == 8 )
                        {
                            nImageFormatOffset +=  smallGlyphMetrics.bufSize + 3 + (nComponent * ebdtComponent.bufSize);                        
                        }
                        else // nImageFormat = 9
                        {
                            nImageFormatOffset +=  smallGlyphMetrics.bufSize + 2 + (nComponent * ebdtComponent.bufSize);                        
                        }
                        
                        ebdtc.glyphCode = m_bufTable.GetUshort( nImageFormatOffset + (uint)ebdtComponent.FieldOffsets.glyphCode );
                        ebdtc.xOffset = m_bufTable.GetSbyte( nImageFormatOffset + (uint)ebdtComponent.FieldOffsets.xOffset );
                        ebdtc.yOffset = m_bufTable.GetSbyte( nImageFormatOffset + (uint)ebdtComponent.FieldOffsets.yOffset );
                    }    
                }
            }

            return ebdtc;
        }

        public byte[] GetImageData( Table_EBLC.indexSubTable cIndexSubTable, uint nGlyphIndex, uint nStartGlyphIndex )
        {
            int nIndexFormat = cIndexSubTable.header.indexFormat;
            int nImageFormat = cIndexSubTable.header.imageFormat;
            byte[] bufImageData = null;

            // 8 and 9 are composites so their image data should be retrieved through the composite glyphs
            Debug.Assert( nImageFormat != 8 && nImageFormat != 9 );

            if( nGlyphIndex >= nStartGlyphIndex )
            {
                uint nImageFormatOffset = getImageFormatOffset( cIndexSubTable, nGlyphIndex, nStartGlyphIndex );
                    
                if( nImageFormatOffset > 0 )
                {
                    uint nImageLength = 0;
                
                    if( nImageFormat == 1 || nImageFormat == 2 )
                    {                        
                        nImageFormatOffset += smallGlyphMetrics.bufSize;                        
                    }
                    else if( nImageFormat == 5 )
                    {                        
                    }
                    else if( nImageFormat == 6 || nImageFormat == 7 )
                    {
                        nImageFormatOffset += bigGlyphMetrics.bufSize;                        
                    }    
                
                    nImageLength = getImageLength( cIndexSubTable, nGlyphIndex, nStartGlyphIndex, nImageFormatOffset );

                    if( nImageLength > 0 )
                    {
                        bufImageData = new byte[nImageLength];
                        System.Buffer.BlockCopy( m_bufTable.GetBuffer(), (int)nImageFormatOffset, bufImageData, 0, (int)nImageLength );
                    }
                }    
            }

            return bufImageData;    
            
        }


        private uint getImageFormatOffset( Table_EBLC.indexSubTable cIndexSubTable, uint nGlyphIndex, uint nStartGlyphIndex )
        {
            int nIndexFormat = cIndexSubTable.header.indexFormat;
            uint nGlyphImageDataOffset = 0;

            switch( nIndexFormat )
            {
                case 1:
                {                            
                    nGlyphImageDataOffset = cIndexSubTable.header.imageDataOffset + ((Table_EBLC.indexSubTable1)cIndexSubTable).GetOffset( (uint)(nGlyphIndex - nStartGlyphIndex));
                    break;
                }
                case 2:
                {
                    nGlyphImageDataOffset = cIndexSubTable.header.imageDataOffset + 4 + bigGlyphMetrics.bufSize;
                    nGlyphImageDataOffset += (uint)(((Table_EBLC.indexSubTable2)cIndexSubTable).imageSize * (nGlyphIndex - nStartGlyphIndex));
                    break;
                }
                case 3:
                {                            
                    nGlyphImageDataOffset = cIndexSubTable.header.imageDataOffset + ((Table_EBLC.indexSubTable3)cIndexSubTable).GetOffset( (uint)(nGlyphIndex - nStartGlyphIndex));
                    break;
                }
                case 4:
                {
                    Table_EBLC.indexSubTable4 ist = (Table_EBLC.indexSubTable4)cIndexSubTable;
                        
                    for( uint i = 0; i < ist.numGlyphs; i++ )
                    {
                        if( ist.GetCodeOffsetPair( i ).glyphCode == nGlyphIndex )
                        {
                            nGlyphImageDataOffset = ist.header.imageDataOffset + ist.GetCodeOffsetPair( i ).offset;
                            break;
                        }
                    }
                    break;
                }
                case 5:
                {
                
                    for( uint i = 0; i < ((Table_EBLC.indexSubTable5)cIndexSubTable).numGlyphs; i++ )
                    {
                        if( ((Table_EBLC.indexSubTable5)cIndexSubTable).GetGlyphCode( i ) == nGlyphIndex )
                        {
                            nGlyphImageDataOffset = cIndexSubTable.header.imageDataOffset + 4 + bigGlyphMetrics.bufSize + 4;
                            nGlyphImageDataOffset += (uint)(((Table_EBLC.indexSubTable5)cIndexSubTable).numGlyphs * 2);
                            nGlyphImageDataOffset += (uint)(i * ((Table_EBLC.indexSubTable5)cIndexSubTable).imageSize);
                            break;
                        }
                    }
                    break;
                }
            }

            return nGlyphImageDataOffset;
        }

        public uint getImageLength( Table_EBLC.indexSubTable cIndexSubTable, uint nGlyphIndex, uint nStartGlyphIndex, uint nImageOffset )
        {
            int nIndexFormat = cIndexSubTable.header.indexFormat;
            uint nImageLength = 0;

            switch( nIndexFormat )
            {
                case 1:
                {    
                    nImageLength = cIndexSubTable.header.imageDataOffset + ((Table_EBLC.indexSubTable1)cIndexSubTable).GetOffset( (uint)((nGlyphIndex + 1) - nStartGlyphIndex));
                    nImageLength -= nImageOffset;
                    break;
                }
                case 2:
                {
                    nImageLength = ((Table_EBLC.indexSubTable2)cIndexSubTable).imageSize;
                    break;
                }
                case 3:
                {                            
                    nImageLength = cIndexSubTable.header.imageDataOffset + ((Table_EBLC.indexSubTable3)cIndexSubTable).GetOffset( (uint)((nGlyphIndex + 1) - nStartGlyphIndex));
                    nImageLength -= nImageOffset;                        
                    break;
                }
                case 4:
                {
                    Table_EBLC.indexSubTable4 ist = (Table_EBLC.indexSubTable4)cIndexSubTable;
                        
                    for( uint i = 0; i < ist.numGlyphs; i++ )
                    {
                        if( ist.GetCodeOffsetPair( i ).glyphCode == nGlyphIndex )
                        {
                            nImageLength = ist.header.imageDataOffset + ist.GetCodeOffsetPair( i + 1 ).offset;
                            nImageLength -= nImageOffset;                            
                        }
                    }
                    break;
                }
                case 5:
                {                
                    nImageLength = ((Table_EBLC.indexSubTable5)cIndexSubTable).imageSize;                    
                    break;
                }
            }

            return nImageLength;
        }

        public byte [,] GetBitmapImage(Table_EBLC.bitmapSizeTable bst, ushort glyphID)
        {
            byte [,] bits = null;

            Table_EBLC.indexSubTableArray ista = bst.FindIndexSubTableArray(glyphID);
            if (ista != null)
            {
                Table_EBLC.indexSubTable ist = bst.GetIndexSubTable(ista);
            
                if (ist.header.imageFormat < 8)
                {
                    // simple bitmap
                    byte [] encodedDataBuf = GetImageData(ist, glyphID, ista.firstGlyphIndex);

                    byte width=0, height=0;

                    switch (ist.header.imageFormat)
                    {
                        case 0:
                            throw new ApplicationException("illegal image format: 0");
                            //break;
                        case 1:
                        case 2:
                            smallGlyphMetrics sgm = this.GetSmallMetrics(ist, glyphID, ista.firstGlyphIndex);
                            width = sgm.width;
                            height = sgm.height;
                            break;
                        case 3:
                        case 4:
                            throw new ApplicationException("illegal image format: " + ist.header.imageFormat);
                            //break;
                        case 5:
                        switch (ist.header.indexFormat)
                        {
                            case 2:
                                Table_EBLC.indexSubTable2 ist2 = (Table_EBLC.indexSubTable2)ist;
                                width = ist2.bigMetrics.width;
                                height = ist2.bigMetrics.height;
                                break;
                            case 5:
                                Table_EBLC.indexSubTable5 ist5 = (Table_EBLC.indexSubTable5)ist;
                                width = ist5.bigMetrics.width;
                                height = ist5.bigMetrics.height;
                                break;

                        }
                            break;
                        case 6:
                        case 7:
                            bigGlyphMetrics bgm = this.GetBigMetrics(ist, glyphID, ista.firstGlyphIndex);
                            width = bgm.width;
                            height = bgm.height;
                            break;
                    }

                    if (encodedDataBuf != null)
                    {
                        bits = DecodeImageData(ist, width, height, bst.bitDepth, encodedDataBuf);
                    }
                    else
                    {
                        //Debug.Assert(false);
                    }
                }
                else if (ist.header.imageFormat <10)
                {
                    // composite bitmap
                    throw new ApplicationException("TODO: impelement bitmap composites");
                }
                else
                {
                    Debug.Assert(false, "illegal image format");
                }
            }

            return bits;
        }

        protected byte[,] DecodeImageData(Table_EBLC.indexSubTable ist, byte width, byte height, byte bitDepth, byte[] databuf)
        {
            byte [,] bits = null;

            switch (ist.header.imageFormat)
            {
                case 0:
                    throw new ApplicationException("illegal image format: 0");
                    //break;
                case 1:
                    bits = DecodeImageDataFmt16(width, height, bitDepth, databuf);
                    break;
                case 2:
                    bits = DecodeImageDataFmt257(width, height, bitDepth, databuf);
                    break;
                case 3:
                    throw new ApplicationException("illegal image format: 3");
                    //break;
                case 4:
                    throw new ApplicationException("illegal image format: 4");
                    //break;
                case 5:
                    bits = DecodeImageDataFmt257(width, height, bitDepth, databuf);
                    break;
                case 6:
                    bits = DecodeImageDataFmt16(width, height, bitDepth, databuf);
                    break;
                case 7:
                    bits = DecodeImageDataFmt257(width, height, bitDepth, databuf);
                    break;
                default:
                    break;
            }

            return bits;
        }

        protected byte[,] DecodeImageDataFmt16(byte width, byte height, byte bitDepth, byte[] databuf)
        {
            // spec says data is byte aligned 
            // - the fine print seems to say that each row is byte aligned, not each pixel
            byte [,] bits = new byte[height, width]; // [rows, cols]

            switch (bitDepth)
            {
                case 0:
                    throw new ApplicationException("bit depth = 0");
                    //break;
                case 1:
                    int nBytesInRow = width/8;
                    if (width % 8 != 0) nBytesInRow++;
                    for (int row=0; row<height; row++)
                    {
                        for (int col=0; col<width; col++)
                        {
                            int nBytePos = row*nBytesInRow + col / 8;
                            bits[row, col] = (byte)((databuf[nBytePos] >> (7-col%8)) & 1);
                        }
                    }
                    break;
                case 8:
                    for (int row=0; row<height; row++)
                    {
                        for (int col=0; col<width; col++)
                        {
                            bits[row, col] = databuf[row*width + col];
                        }
                    }
                    break;
                default:
                    throw new ApplicationException("unsupported bitDepth: " + bitDepth);
            }

            return bits;
        }

        protected byte[,] DecodeImageDataFmt257(byte width, byte height, byte bitDepth, byte[] databuf)
        {
            // spec says data is bit aligned 
            byte [,] bits = new byte[height, width]; // [rows, cols]

            switch (bitDepth)
            {
                case 0:
                    throw new ApplicationException("bit depth = 0");
                    //break;
                case 1:
                    for (int row=0; row<height; row++)
                    {
                        for (int col=0; col<width; col++)
                        {
                            int nBytePos = (row*width + col)/8;
                            int nBitPos  = (row*width + col)%8;
                            bits[row, col] = (byte)((databuf[nBytePos] >> (7-nBitPos)) & 1);
                        }
                    }
                    break;
                case 8:
                    for (int row=0; row<height; row++)
                    {
                        for (int col=0; col<width; col++)
                        {
                            bits[row, col] = databuf[row*width + col];
                        }
                    }
                    break;
                default:
                    throw new ApplicationException("unsupported bitDepth: " + bitDepth);
            }

            return bits;
        }

        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new EBDT_cache( this );
            }

            return m_cache;
        }
        
        public class EBDT_cache : DataCache
        {
            protected OTFixed m_version;
            protected MBOBuffer m_newbuf;

            // constructor
            public EBDT_cache(Table_EBDT OwnerTable)
            {
                // copy the data from the owner table's MBOBuffer
                // and store it in the cache variables
                m_version = OwnerTable.version;
            }

            public void setCache( MBOBuffer newbuf )
            {
                m_newbuf = newbuf;
            }

            public override OTTable GenerateTable()
            {
                if( null == m_newbuf )
                {
                    throw new ArgumentNullException( "EBDT cache buffer needs to be set by the EBLC cache first." );
                }

                m_newbuf.SetFixed( m_version,    (uint)Table_EBDT.FieldOffsets.version );

                // put the buffer into a Table_EBDT object and return it
                Table_EBDT EBDTTable = new Table_EBDT( "EBDT", m_newbuf );
            
                return EBDTTable;
            }
        }
        

    }
}
