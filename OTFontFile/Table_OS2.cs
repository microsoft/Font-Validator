using System;
using System.Diagnostics;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_
    /// </summary>
    public class Table_OS2 : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_OS2(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            version                 = 0,
            xAvgCharWidth           = 2,
            usWeightClass           = 4,
            usWidthClass            = 6,
            fsType                  = 8,
            ySubscriptXSize         = 10,
            ySubscriptYSize         = 12,
            ySubscriptXOffset       = 14,
            ySubscriptYOffset       = 16,
            ySuperscriptXSize       = 18,
            ySuperscriptYSize       = 20,
            ySuperscriptXOffset     = 22,
            ySuperscriptYOffset     = 24,
            yStrikeoutSize          = 26,
            yStrikeoutPosition      = 28,
            sFamilyClass            = 30,
            panose_byte1            = 32,
            panose_byte2            = 33,
            panose_byte3            = 34,
            panose_byte4            = 35,
            panose_byte5            = 36,
            panose_byte6            = 37,
            panose_byte7            = 38,
            panose_byte8            = 39,
            panose_byte9            = 40,
            panose_byte10           = 41,
            ulUnicodeRange1         = 42,
            ulUnicodeRange2         = 46,
            ulUnicodeRange3         = 50,
            ulUnicodeRange4         = 54,
            achVendID               = 58,
            fsSelection             = 62,
            usFirstCharIndex        = 64,
            usLastCharIndex         = 66,
            sTypoAscender           = 68,
            sTypoDescender          = 70,
            sTypoLineGap            = 72,
            usWinAscent             = 74,
            usWinDescent            = 76, // version 0 specified through this field
            ulCodePageRange1        = 78,
            ulCodePageRange2        = 82, // version 1 specified through this field
            sxHeight                = 86,
            sCapHeight              = 88,
            usDefaultChar           = 90,
            usBreakChar             = 92,
            usMaxContext            = 94  // version 2 and version 3 specified through this field
        }


        /************************
         * property accessors
         */

        public ushort version
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.version);}
        }

        public short xAvgCharWidth   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.xAvgCharWidth);}
        }

        public ushort usWeightClass   
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.usWeightClass);}
        }

        public ushort usWidthClass   
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.usWidthClass);}
        }

        public ushort fsType   
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.fsType);}
        }

        public short ySubscriptXSize   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.ySubscriptXSize);}
        }

        public short ySubscriptYSize   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.ySubscriptYSize);}
        }

        public short ySubscriptXOffset   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.ySubscriptXOffset);}
        }

        public short ySubscriptYOffset   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.ySubscriptYOffset);}
        }

        public short ySuperscriptXSize   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.ySuperscriptXSize);}
        }

        public short ySuperscriptYSize   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.ySuperscriptYSize);}
        }

        public short ySuperscriptXOffset   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.ySuperscriptXOffset);}
        }

        public short ySuperscriptYOffset   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.ySuperscriptYOffset);}
        }

        public short yStrikeoutSize   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.yStrikeoutSize);}
        }

        public short yStrikeoutPosition   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.yStrikeoutPosition);}
        }

        public short sFamilyClass   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.sFamilyClass);}
        }

        public byte panose_byte1
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.panose_byte1);}
        }

        public byte panose_byte2
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.panose_byte2);}
        }

        public byte panose_byte3
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.panose_byte3);}
        }

        public byte panose_byte4
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.panose_byte4);}
        }

        public byte panose_byte5
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.panose_byte5);}
        }

        public byte panose_byte6
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.panose_byte6);}
        }

        public byte panose_byte7
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.panose_byte7);}
        }

        public byte panose_byte8
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.panose_byte8);}
        }

        public byte panose_byte9
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.panose_byte9);}
        }

        public byte panose_byte10
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.panose_byte10);}
        }

        public bool isUnicodeRangeBitSet( byte byBit )
        {
            bool bResult = false;
            if( byBit > 127 )
            {
                throw new ArgumentOutOfRangeException( "Valid Bit request are are to 127" );
            }

            if( byBit < 32 )
            {                
                if( (ulUnicodeRange1 & 1<<byBit) != 0 )
                {
                    bResult = true;
                }
            }
            else if( byBit < 64 )
            {
                if( (ulUnicodeRange2 & 1<<(byBit - 32)) != 0 )
                {
                    bResult = true;
                }

            }
            else if( byBit < 96 )
            {
                if( (ulUnicodeRange3 & 1<<(byBit - 64)) != 0 )
                {
                    bResult = true;
                }
            }
            else
            {
                if( (ulUnicodeRange4 & 1<<(byBit - 96)) != 0 )
                {
                    bResult = true;
                }
            }


            return bResult;
        }

        public uint ulUnicodeRange1
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.ulUnicodeRange1);}
        }

        public uint ulUnicodeRange2
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.ulUnicodeRange2);}
        }

        public uint ulUnicodeRange3
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.ulUnicodeRange3);}
        }

        public uint ulUnicodeRange4
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.ulUnicodeRange4);}
        }

        public byte[] achVendID
        {
            get
            {
                byte[] fourBytes = new byte[4];
                System.Buffer.BlockCopy(m_bufTable.GetBuffer(), (int)FieldOffsets.achVendID, fourBytes, 0, 4);
                return fourBytes;
            }
        }

        public ushort fsSelection   
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.fsSelection);}
        }

        public ushort usFirstCharIndex   
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.usFirstCharIndex);}
        }

        public ushort usLastCharIndex   
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.usLastCharIndex);}
        }

        public short sTypoAscender   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.sTypoAscender);}
        }

        public short sTypoDescender   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.sTypoDescender);}
        }

        public short sTypoLineGap   
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.sTypoLineGap);}
        }

        public ushort usWinAscent   
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.usWinAscent);}
        }

        public ushort usWinDescent   
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.usWinDescent);}
        }

        public uint ulCodePageRange1
        {
            get
            {
                if( version < 0x0001 )
                {
                    throw new InvalidOperationException();
                }
                
                return m_bufTable.GetUint((uint)FieldOffsets.ulCodePageRange1);
            }
        }

        public uint ulCodePageRange2
        {
            get
            {
                if( version < 0x0001 )
                {
                    throw new InvalidOperationException();
                }

                return m_bufTable.GetUint((uint)FieldOffsets.ulCodePageRange2);
            }
        }

        public short sxHeight   
        {
            get
            {
                if( version < 0x0002 )
                {
                    throw new InvalidOperationException();
                }

                return m_bufTable.GetShort((uint)FieldOffsets.sxHeight);
            }
        }

        public short sCapHeight   
        {
            get
            {
                if( version < 0x0002 )
                {
                    throw new InvalidOperationException();
                }

                return m_bufTable.GetShort((uint)FieldOffsets.sCapHeight);
            }
        }

        public ushort usDefaultChar   
        {
            get
            { 
                if( version < 0x0002 )
                {
                    throw new InvalidOperationException();
                }

                return m_bufTable.GetUshort((uint)FieldOffsets.usDefaultChar);
            }
        }

        public ushort usBreakChar   
        {
            get
            {
                if( version < 0x0002 )
                {
                    throw new InvalidOperationException();
                }

                return m_bufTable.GetUshort((uint)FieldOffsets.usBreakChar);
            }
        }

        public ushort usMaxContext 
        {
            get 
            {
                if( version < 0x0002 )
                {
                    throw new InvalidOperationException();
                }

                return m_bufTable.GetUshort((uint)FieldOffsets.usMaxContext);
            }
        }



        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new OS2_cache(this);
            }

            return m_cache;
        }
        
        public class OS2_cache : DataCache
        {

            protected ushort m_version;
            protected short m_xAvgCharWidth;
            protected ushort m_usWeightClass;
            protected ushort m_usWidthClass;
            protected ushort m_fsType;
            protected short m_ySubscriptXSize;
            protected short m_ySubscriptYSize;
            protected short m_ySubscriptXOffset;
            protected short m_ySubscriptYOffset;
            protected short m_ySuperscriptXSize;
            protected short m_ySuperscriptYSize;
            protected short m_ySuperscriptXOffset;
            protected short m_ySuperscriptYOffset;
            protected short m_yStrikeoutSize;
            protected short m_yStrikeoutPosition;
            protected short m_sFamilyClass;
            protected byte m_panose_byte1;
            protected byte m_panose_byte2;
            protected byte m_panose_byte3;
            protected byte m_panose_byte4;
            protected byte m_panose_byte5;
            protected byte m_panose_byte6;
            protected byte m_panose_byte7;
            protected byte m_panose_byte8;
            protected byte m_panose_byte9;
            protected byte m_panose_byte10;
            protected uint m_ulUnicodeRange1;
            protected uint m_ulUnicodeRange2;
            protected uint m_ulUnicodeRange3;
            protected uint m_ulUnicodeRange4;
            protected byte[] m_achVendID;
            protected ushort m_fsSelection;
            protected ushort m_usFirstCharIndex;
            protected ushort m_usLastCharIndex;
            protected short m_sTypoAscender;
            protected short m_sTypoDescender;
            protected short m_sTypoLineGap;
            protected ushort m_usWinAscent;
            protected ushort m_usWinDescent;
            protected uint m_ulCodePageRange1;
            protected uint m_ulCodePageRange2;
            protected short m_sxHeight;
            protected short m_sCapHeight;
            protected ushort m_usDefaultChar;
            protected ushort m_usBreakChar;
            protected ushort m_usMaxContext;

            // constructor
            public OS2_cache(Table_OS2 OwnerTable)
            {
                // copy the data from the owner table's MBOBuffer
                // and store it in the cache variables
                m_version                = OwnerTable.version;
                m_xAvgCharWidth            = OwnerTable.xAvgCharWidth;
                m_usWeightClass            = OwnerTable.usWeightClass;
                m_usWidthClass            = OwnerTable.usWidthClass;
                m_fsType                = OwnerTable.fsType;
                m_ySubscriptXSize        = OwnerTable.ySubscriptXSize;
                m_ySubscriptYSize        = OwnerTable.ySubscriptYSize;
                m_ySubscriptXOffset        = OwnerTable.ySubscriptXOffset;
                m_ySubscriptYOffset        = OwnerTable.ySubscriptYOffset;
                m_ySuperscriptXSize        = OwnerTable.ySuperscriptXSize;
                m_ySuperscriptYSize        = OwnerTable.ySuperscriptYSize;
                m_ySuperscriptXOffset    = OwnerTable.ySuperscriptXOffset;
                m_ySuperscriptYOffset    = OwnerTable.ySuperscriptYOffset;
                m_yStrikeoutSize        = OwnerTable.yStrikeoutSize;
                m_yStrikeoutPosition    = OwnerTable.yStrikeoutPosition;
                m_sFamilyClass            = OwnerTable.sFamilyClass;
                m_panose_byte1            = OwnerTable.panose_byte1;
                m_panose_byte2            = OwnerTable.panose_byte2;
                m_panose_byte3            = OwnerTable.panose_byte3;
                m_panose_byte4            = OwnerTable.panose_byte4;
                m_panose_byte5            = OwnerTable.panose_byte5;
                m_panose_byte6            = OwnerTable.panose_byte6;
                m_panose_byte7            = OwnerTable.panose_byte7;
                m_panose_byte8            = OwnerTable.panose_byte8;
                m_panose_byte9            = OwnerTable.panose_byte9;
                m_panose_byte10            = OwnerTable.panose_byte10;
                m_ulUnicodeRange1        = OwnerTable.ulUnicodeRange1;
                m_ulUnicodeRange2        = OwnerTable.ulUnicodeRange2;
                m_ulUnicodeRange3        = OwnerTable.ulUnicodeRange3;
                m_ulUnicodeRange4        = OwnerTable.ulUnicodeRange4;
                m_achVendID                = OwnerTable.achVendID;
                m_fsSelection            = OwnerTable.fsSelection;
                m_usFirstCharIndex        = OwnerTable.usFirstCharIndex;
                m_usLastCharIndex        = OwnerTable.usLastCharIndex;
                m_sTypoAscender            = OwnerTable.sTypoAscender;
                m_sTypoDescender        = OwnerTable.sTypoDescender;
                m_sTypoLineGap            = OwnerTable.sTypoLineGap;
                m_usWinAscent            = OwnerTable.usWinAscent;
                m_usWinDescent            = OwnerTable.usWinDescent;
                if( version > 0x000 )
                {
                    m_ulCodePageRange1        = OwnerTable.ulCodePageRange1;
                    m_ulCodePageRange2        = OwnerTable.ulCodePageRange2;
                    if( version > 0x001 ) // version 2 and 3
                    {
                        m_sxHeight                = OwnerTable.sxHeight;
                        m_sCapHeight            = OwnerTable.sCapHeight;
                        m_usDefaultChar            = OwnerTable.usDefaultChar;
                        m_usBreakChar            = OwnerTable.usBreakChar;
                        m_usMaxContext            = OwnerTable.usMaxContext;
                    }
                }
            }

            /************************
            * property accessors
            */

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

            public short xAvgCharWidth   
            {
                get
                {
                    return m_xAvgCharWidth;
                }
                set
                {
                    m_xAvgCharWidth = value;
                    m_bDirty = true;
                }
            }

            public ushort usWeightClass   
            {
                get
                {
                    return m_usWeightClass;
                }
                set
                {
                    m_usWeightClass = value;
                    m_bDirty = true;
                }
            }

            public ushort usWidthClass   
            {
                get
                {
                    return m_usWidthClass;
                }
                set
                {
                    m_usWidthClass = value;
                    m_bDirty = true;
                }
            }

            public ushort fsType   
            {
                get
                {
                    return m_fsType;
                }
                set
                {
                    m_fsType = value;
                    m_bDirty = true;
                }
            }

            public short ySubscriptXSize   
            {
                get
                {
                    return m_ySubscriptXSize;
                }
                set
                {
                    m_ySubscriptXSize = value;
                    m_bDirty = true;
                }
            }

            public short ySubscriptYSize   
            {
                get
                {
                    return m_ySubscriptYSize;
                }
                set
                {
                    m_ySubscriptYSize = value;
                    m_bDirty = true;
                }
            }

            public short ySubscriptXOffset   
            {
                get
                {
                    return m_ySubscriptXOffset;
                }
                set
                {
                    m_ySubscriptXOffset = value;
                    m_bDirty = true;
                }
            }

            public short ySubscriptYOffset   
            {
                get
                {
                    return m_ySubscriptYOffset;
                }
                set
                {
                    m_ySubscriptYOffset = value;
                    m_bDirty = true;
                }
            }

            public short ySuperscriptXSize   
            {
                get
                {
                    return m_ySuperscriptXSize;
                }
                set
                {
                    m_ySuperscriptXSize = value;
                    m_bDirty = true;
                }
            }

            public short ySuperscriptYSize   
            {
                get
                {
                    return m_ySuperscriptYSize;
                }
                set
                {
                    m_ySuperscriptYSize = value;
                    m_bDirty = true;
                }
            }

            public short ySuperscriptXOffset   
            {
                get
                {
                    return m_ySuperscriptXOffset;
                }
                set
                {
                    m_ySuperscriptXOffset = value;
                    m_bDirty = true;
                }
            }

            public short ySuperscriptYOffset   
            {
                get
                {
                    return m_ySuperscriptYOffset;
                }
                set
                {
                    m_ySuperscriptYOffset = value;
                    m_bDirty = true;
                }
            }

            public short yStrikeoutSize   
            {
                get
                {
                    return m_yStrikeoutSize;
                }
                set
                {
                    m_yStrikeoutSize = value;
                    m_bDirty = true;
                }
            }

            public short yStrikeoutPosition   
            {
                get
                {
                    return m_yStrikeoutPosition;
                }
                set
                {
                    m_yStrikeoutPosition = value;
                    m_bDirty = true;
                }
            }

            public short sFamilyClass   
            {
                get
                {
                    return m_sFamilyClass;
                }
                set
                {
                    m_sFamilyClass = value;
                    m_bDirty = true;
                }
            }

            public byte panose_byte1
            {
                get
                {
                    return m_panose_byte1;
                }
                set
                {
                    m_panose_byte1 = value;
                    m_bDirty = true;
                }
            }

            public byte panose_byte2
            {
                get
                {
                    return m_panose_byte2;
                }
                set
                {
                    m_panose_byte2 = value;
                    m_bDirty = true;
                }
            }

            public byte panose_byte3
            {
                get
                {
                    return m_panose_byte3;
                }
                set
                {
                    m_panose_byte3 = value;
                    m_bDirty = true;
                }
            }

            public byte panose_byte4
            {
                get
                {
                    return m_panose_byte4;
                }
                set
                {
                    m_panose_byte4 = value;
                    m_bDirty = true;
                }
            }

            public byte panose_byte5
            {
                get
                {
                    return m_panose_byte5;
                }
                set
                {
                    m_panose_byte5 = value;
                    m_bDirty = true;
                }
            }

            public byte panose_byte6
            {
                get
                {
                    return m_panose_byte6;
                }
                set
                {
                    m_panose_byte6 = value;
                    m_bDirty = true;
                }
            }

            public byte panose_byte7
            {
                get
                {
                    return m_panose_byte7;
                }
                set
                {
                    m_panose_byte7 = value;
                    m_bDirty = true;
                }
            }

            public byte panose_byte8
            {
                get
                {
                    return m_panose_byte8;
                }
                set
                {
                    m_panose_byte8 = value;
                    m_bDirty = true;
                }
            }

            public byte panose_byte9
            {
                get
                {
                    return m_panose_byte9;
                }
                set
                {
                    m_panose_byte9 = value;
                    m_bDirty = true;
                }
            }

            public byte panose_byte10
            {
                get
                {
                    return m_panose_byte10;
                }
                set
                {
                    m_panose_byte10 = value;
                    m_bDirty = true;
                }
            }

            public uint ulUnicodeRange1
            {
                get
                {
                    return m_ulUnicodeRange1;
                }
                set
                {
                    m_ulUnicodeRange1 = value;
                    m_bDirty = true;
                }
            }

            public uint ulUnicodeRange2
            {
                get
                {
                    return m_ulUnicodeRange2;
                }
                set
                {
                    m_ulUnicodeRange2 = value;
                    m_bDirty = true;
                }
            }

            public uint ulUnicodeRange3
            {
                get
                {
                    return m_ulUnicodeRange3;
                }
                set
                {
                    m_ulUnicodeRange3 = value;
                    m_bDirty = true;
                }
            }

            public uint ulUnicodeRange4
            {
                get
                {
                    return m_ulUnicodeRange4;
                }
                set
                {
                    m_ulUnicodeRange4 = value;
                    m_bDirty = true;
                }
            }

            public byte[] achVendID
            {
                get
                {
                    return m_achVendID;                    
                }
                set 
                {                    
                    byte[] fourByte;
                    fourByte = value;

                    if( value.Length != 4 )
                    {
                        throw new ApplicationException( "Only a four byte array is applicable" );
                    }

                    m_achVendID = value;
                    m_bDirty = true;
                }
            }

            public ushort fsSelection   
            {
                get
                {
                    return m_fsSelection;
                }
                set
                {
                    m_fsSelection = value;
                    m_bDirty = true;
                }
            }

            public ushort usFirstCharIndex   
            {
                get
                {
                    return m_usFirstCharIndex;
                }
                set
                {
                    m_usFirstCharIndex = value;
                    m_bDirty = true;
                }
            }

            public ushort usLastCharIndex   
            {
                get
                {
                    return m_usLastCharIndex;
                }
                set
                {
                    m_usLastCharIndex = value;
                    m_bDirty = true;
                }
            }

            public short sTypoAscender   
            {
                get
                {
                    return m_sTypoAscender;
                }
                set
                {
                    m_sTypoAscender = value;
                    m_bDirty = true;
                }
            }

            public short sTypoDescender   
            {
                get
                {
                    return m_sTypoDescender;
                }
                set
                {
                    m_sTypoDescender = value;
                    m_bDirty = true;
                }
            }

            public short sTypoLineGap   
            {
                get
                {
                    return m_sTypoLineGap;
                }
                set
                {
                    m_sTypoLineGap = value;
                    m_bDirty = true;
                }
            }

            public ushort usWinAscent   
            {
                get
                {                    
                    return m_usWinAscent;
                }
                set
                {                    
                    m_usWinAscent = value;
                    m_bDirty = true;
                }
            }

            public ushort usWinDescent   
            {
                get
                {
                    return m_usWinDescent;
                }
                set
                {
                    m_usWinDescent = value;
                    m_bDirty = true;
                }
            }

            public uint ulCodePageRange1
            {
                get
                {
                    if( m_version < 0x0001 )
                    {
                        throw new InvalidOperationException();
                    }

                    return m_ulCodePageRange1;
                }
                set
                {
                    if( m_version < 0x0001 )
                    {
                        throw new InvalidOperationException();
                    }

                    m_ulCodePageRange1 = value;
                    m_bDirty = true;
                }
            }

            public uint ulCodePageRange2
            {
                get
                {
                    if( m_version < 0x0001 )
                    {
                        throw new InvalidOperationException();
                    }

                    return m_ulCodePageRange2;
                }
                set
                {
                    if( m_version < 0x0001 )
                    {
                        throw new InvalidOperationException();
                    }

                    m_ulCodePageRange2 = value;
                    m_bDirty = true;
                }
            }

            public short sxHeight   
            {
                get
                {
                    if( m_version < 0x0002 )
                    {
                        throw new InvalidOperationException();
                    }

                    return m_sxHeight;
                }
                set
                {
                    if( m_version < 0x0002 )
                    {
                        throw new InvalidOperationException();
                    }

                    m_sxHeight = value;
                    m_bDirty = true;
                }
            }

            public short sCapHeight   
            {
                get
                {
                    if( m_version < 0x0002 )
                    {
                        throw new InvalidOperationException();
                    }

                    return m_sCapHeight;
                }
                set
                {
                    if( m_version < 0x0002 )
                    {
                        throw new InvalidOperationException();
                    }

                    m_sCapHeight = value;
                    m_bDirty = true;
                }
            }

            public ushort usDefaultChar   
            {
                get
                {
                    if( m_version < 0x0002 )
                    {
                        throw new InvalidOperationException();
                    }

                    return m_usDefaultChar;
                }
                set
                {
                    if( m_version < 0x0002 )
                    {
                        throw new InvalidOperationException();
                    }

                    m_usDefaultChar = value;
                    m_bDirty = true;
                }
            }

            public ushort usBreakChar   
            {
                get
                {
                    if( m_version < 0x0002 )
                    {
                        throw new InvalidOperationException();
                    }
                    
                    return m_usBreakChar;
                }
                set
                {
                    if( m_version < 0x0002 )
                    {
                        throw new InvalidOperationException();
                    }

                    m_usBreakChar = value;
                    m_bDirty = true;
                }
            }

            public ushort usMaxContext 
            {
                get
                {
                    if( m_version < 0x0002 )
                    {
                        throw new InvalidOperationException();
                    }

                    return m_usMaxContext;
                }
                set
                {
                    if( m_version < 0x0002 )
                    {
                        throw new InvalidOperationException();
                    }

                    m_usMaxContext = value;
                    m_bDirty = true;
                }
            }
            public override OTTable GenerateTable()
            {
                MBOBuffer newbuf;

                switch( m_version )
                {
                    case 0x0000:
                        newbuf = new MBOBuffer( 78 );
                        break;
                    case 0x0001:
                        newbuf = new MBOBuffer( 86 );
                        break;
                    case 0x0002:
                        newbuf = new MBOBuffer( 96 );
                        break;
                    case 0x0003:
                        goto case 0x0002;
                    default:
                        goto case 0x0002; // version 3 is default                        
                }


                newbuf.SetUshort( m_version,            (uint)Table_OS2.FieldOffsets.version );
                newbuf.SetShort( m_xAvgCharWidth,        (uint)Table_OS2.FieldOffsets.xAvgCharWidth );
                newbuf.SetUshort( m_usWeightClass,        (uint)Table_OS2.FieldOffsets.usWeightClass );
                newbuf.SetUshort( m_usWidthClass,        (uint)Table_OS2.FieldOffsets.usWidthClass );
                newbuf.SetUshort( m_fsType,                (uint)Table_OS2.FieldOffsets.fsType );
                newbuf.SetShort( m_ySubscriptXSize,        (uint)Table_OS2.FieldOffsets.ySubscriptXSize );
                newbuf.SetShort( m_ySubscriptYSize,        (uint)Table_OS2.FieldOffsets.ySubscriptYSize );
                newbuf.SetShort( m_ySubscriptXOffset,    (uint)Table_OS2.FieldOffsets.ySubscriptXOffset );
                newbuf.SetShort( m_ySubscriptYOffset,    (uint)Table_OS2.FieldOffsets.ySubscriptYOffset );
                newbuf.SetShort( m_ySuperscriptXSize,    (uint)Table_OS2.FieldOffsets.ySuperscriptXSize );
                newbuf.SetShort( m_ySuperscriptYSize,    (uint)Table_OS2.FieldOffsets.ySuperscriptYSize );
                newbuf.SetShort( m_ySuperscriptXOffset,    (uint)Table_OS2.FieldOffsets.ySuperscriptXOffset );
                newbuf.SetShort( m_ySuperscriptYOffset,    (uint)Table_OS2.FieldOffsets.ySuperscriptYOffset );
                newbuf.SetShort( m_yStrikeoutSize,        (uint)Table_OS2.FieldOffsets.yStrikeoutSize );
                newbuf.SetShort( m_yStrikeoutPosition,    (uint)Table_OS2.FieldOffsets.yStrikeoutPosition );
                newbuf.SetShort( m_sFamilyClass,        (uint)Table_OS2.FieldOffsets.sFamilyClass );
                newbuf.SetByte( m_panose_byte1,            (uint)Table_OS2.FieldOffsets.panose_byte1 );
                newbuf.SetByte( m_panose_byte2,            (uint)Table_OS2.FieldOffsets.panose_byte2 );
                newbuf.SetByte( m_panose_byte3,            (uint)Table_OS2.FieldOffsets.panose_byte3 );
                newbuf.SetByte( m_panose_byte4,            (uint)Table_OS2.FieldOffsets.panose_byte4 );
                newbuf.SetByte( m_panose_byte5,            (uint)Table_OS2.FieldOffsets.panose_byte5 );
                newbuf.SetByte( m_panose_byte6,            (uint)Table_OS2.FieldOffsets.panose_byte6 );
                newbuf.SetByte( m_panose_byte7,            (uint)Table_OS2.FieldOffsets.panose_byte7 );
                newbuf.SetByte( m_panose_byte8,            (uint)Table_OS2.FieldOffsets.panose_byte8 );
                newbuf.SetByte( m_panose_byte9,            (uint)Table_OS2.FieldOffsets.panose_byte9 );
                newbuf.SetByte( m_panose_byte10,        (uint)Table_OS2.FieldOffsets.panose_byte10 );                
                newbuf.SetUint( m_ulUnicodeRange1,        (uint)Table_OS2.FieldOffsets.ulUnicodeRange1 );
                newbuf.SetUint( m_ulUnicodeRange2,        (uint)Table_OS2.FieldOffsets.ulUnicodeRange2 );
                newbuf.SetUint( m_ulUnicodeRange3,        (uint)Table_OS2.FieldOffsets.ulUnicodeRange3 );
                newbuf.SetUint( m_ulUnicodeRange4,        (uint)Table_OS2.FieldOffsets.ulUnicodeRange4 );
                for( int i = 0; i < 4; i++ )
                {
                    newbuf.SetByte( m_achVendID[i],        (uint)(Table_OS2.FieldOffsets.achVendID + i));
                }
                        
                newbuf.SetUshort( m_fsSelection,        (uint)Table_OS2.FieldOffsets.fsSelection );
                newbuf.SetUshort( m_usFirstCharIndex,    (uint)Table_OS2.FieldOffsets.usFirstCharIndex );
                newbuf.SetUshort( m_usLastCharIndex,    (uint)Table_OS2.FieldOffsets.usLastCharIndex );
                newbuf.SetShort( m_sTypoAscender,        (uint)Table_OS2.FieldOffsets.sTypoAscender );
                newbuf.SetShort( m_sTypoDescender,        (uint)Table_OS2.FieldOffsets.sTypoDescender );
                newbuf.SetShort( m_sTypoLineGap,        (uint)Table_OS2.FieldOffsets.sTypoLineGap );
                newbuf.SetUshort( m_usWinAscent,        (uint)Table_OS2.FieldOffsets.usWinAscent );
                newbuf.SetUshort( m_usWinDescent,        (uint)Table_OS2.FieldOffsets.usWinDescent );

                // version 1.0
                if( version > 0x000 ) 
                {
                    newbuf.SetUint( m_ulCodePageRange1,        (uint)Table_OS2.FieldOffsets.ulCodePageRange1 );
                    newbuf.SetUint( m_ulCodePageRange2,        (uint)Table_OS2.FieldOffsets.ulCodePageRange2 );

                    // vewrsion 2 & 3
                    if( version > 0x001 )
                    {
                        newbuf.SetShort( m_sxHeight,            (uint)Table_OS2.FieldOffsets.sxHeight );
                        newbuf.SetShort( m_sCapHeight,            (uint)Table_OS2.FieldOffsets.sCapHeight );
                        newbuf.SetUshort( m_usDefaultChar,        (uint)Table_OS2.FieldOffsets.usDefaultChar );
                        newbuf.SetUshort( m_usBreakChar,        (uint)Table_OS2.FieldOffsets.usBreakChar );
                        newbuf.SetUshort( m_usMaxContext,        (uint)Table_OS2.FieldOffsets.usMaxContext );
                    }
                }

                // put the buffer into a Table_maxp object and return it
                Table_OS2 OS2Table = new Table_OS2("OS/2", newbuf);

                return OS2Table;    
            }
        }
        

    }
}
