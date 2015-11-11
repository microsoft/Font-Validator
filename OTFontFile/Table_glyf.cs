using System;
using System.Collections;
using System.Diagnostics;


namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_glyf.
    /// </summary>
    public class Table_glyf : OTTable
    {
        public Table_glyf(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /*
         *        ENUMS
         */

        public enum FieldOffsets
        {
            numCont=0,
            xMin=2,
            yMin=4,
            xMax=6,
            yMax=8,
            nextAfterHeader=10,
        }

        public enum MaskFlagKnot : byte 
        {
            isOnCurve=0x01,
            isXByte=0x02,
            isYByte=0x04,
            toRepeat=0x08,
            isXSameOrPozitive=0x10,
            isYSameOrPozitive=0x20
        }

        public enum MaskFlagComponent : short
        {
            ARG_1_AND_2_ARE_WORDS        =    0x0001,
            ARGS_ARE_XY_VALUES            =    0x0002,
            ROUND_XY_TO_GRID            =    0x0004,
            WE_HAVE_A_SCALE                =    0x0008,
            RESERVED                    =    0x0010,
            MORE_COMPONENTS                =    0x0020,
            WE_HAVE_AN_X_AND_Y_SCALE    =    0x0040,
            WE_HAVE_A_TWO_BY_TWO        =    0x0080,
            WE_HAVE_INSTRUCTIONS        =    0x0100,
            USE_MY_METRICS                =    0x0200,
            OVERLAP_COMPOUND            =    0x0400,
            SCALED_COMPOSITE_OFFSET        =    0x0800,
            UNSCALED_COMPONENT_OFFSET    =    0x1000
        }



        /*===============================================================
         * 
         *            OTFontFile:        MEMBERS, PROPERTIES
         * 
         *==============================================================*/


        /************************
         * nested classes
         */

        public class header
        {
            public header (uint offset, MBOBuffer bufTable)
            {
                m_offsetHeader = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                numberOfContours = 0,
                xMin             = 2,
                yMin             = 4,
                xMax             = 6,
                yMax             = 8
            }

            public short numberOfContours
            {
                get {return m_bufTable.GetShort(m_offsetHeader + (uint)FieldOffsets.numberOfContours);}
            }

            public short xMin
            {
                get {return m_bufTable.GetShort(m_offsetHeader + (uint)FieldOffsets.xMin);}
            }

            public short yMin
            {
                get {return m_bufTable.GetShort(m_offsetHeader + (uint)FieldOffsets.yMin);}
            }

            public short xMax
            {
                get {return m_bufTable.GetShort(m_offsetHeader + (uint)FieldOffsets.xMax);}
            }

            public short yMax
            {
                get {return m_bufTable.GetShort(m_offsetHeader + (uint)FieldOffsets.yMax);}
            }


            public SimpleGlyph GetSimpleGlyph()
            {
                SimpleGlyph sg = null;

                if (numberOfContours >= 0)
                {
                    sg = new SimpleGlyph(this, m_offsetHeader + 10, m_bufTable);
                }

                return sg;
            }

            public CompositeGlyph GetCompositeGlyph()
            {
                CompositeGlyph cg = null;

                if (numberOfContours < 0)
                {
                    cg = new CompositeGlyph(this, m_offsetHeader + 10, m_bufTable);
                }

                return cg;
            }

            public uint CalcGlyphLength()
            {
                uint length = 0;

                if (numberOfContours >= 0)
                {
                    SimpleGlyph sg = null;
                    sg = new SimpleGlyph(this, m_offsetHeader + 10, m_bufTable);
                    if (sg != null)
                    {
                        length = sg.CalcLength();
                    }
                }
                else
                {
                    CompositeGlyph cg = null;
                    cg = new CompositeGlyph(this, m_offsetHeader + 10, m_bufTable);
                    if (cg != null)
                    {
                        length = cg.CalcLength();
                    }
                }

                return 10 + length;
            }

            public uint m_offsetHeader;
            MBOBuffer m_bufTable;
        }


        public class SimpleGlyph
        {
            public SimpleGlyph(header h, uint offset, MBOBuffer bufTable)
            {
                m_header = h;
                m_offsetSimpleGlyph = offset;
                m_bufTable = bufTable;
            }

            public ushort GetEndPtOfContour(uint i)
            {
                if (i >= m_header.numberOfContours)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return m_bufTable.GetUshort(m_offsetSimpleGlyph + i*2);
            }

            public ushort instructionLength
            {
                get {return m_bufTable.GetUshort(m_offsetSimpleGlyph + (uint)m_header.numberOfContours*2);}
            }

            public byte GetInstruction(uint i)
            {
                if (i >= instructionLength)
                {
                    throw new ArgumentOutOfRangeException();
                }

                uint offset = m_offsetSimpleGlyph + (uint)m_header.numberOfContours*2 + 2;
                return m_bufTable.GetByte(offset + i);
            }

            public struct Coordinate
            {
                public bool bOnCurve;
                public short x, y;
            }

            public Coordinate[] GetDecodedRelativeCoordinates()
            {
                // allocate the Coordinate array
                int nCoordinates = GetEndPtOfContour((uint)(m_header.numberOfContours-1))+1;
                Coordinate[] arrCoordinates = new Coordinate[nCoordinates];

                // process the flags
                byte [] arrFlags = new Byte[nCoordinates];
                uint offset = m_offsetSimpleGlyph + (uint)m_header.numberOfContours*2 + 2 + instructionLength;
                uint iCoordinate = 0;
                while (iCoordinate < nCoordinates)
                {
                    byte flags = m_bufTable.GetByte(offset++);
                    arrFlags[iCoordinate++] = flags;
                    if ((flags & (byte)MaskFlagKnot.toRepeat) != 0)
                    {
                        // repeat flag is set, next byte is number of additional times this set of flags is repeated
                        byte n = m_bufTable.GetByte(offset++);
                        for (int j=0; j<n; j++)
                        {
                            arrFlags[iCoordinate++] = flags;
                        }
                    }
                }
                for (iCoordinate = 0; iCoordinate < nCoordinates; iCoordinate++)
                {
                    if ((arrFlags[iCoordinate] & (byte)(MaskFlagKnot.isOnCurve)) != 0)
                    {
                        arrCoordinates[iCoordinate].bOnCurve = true;
                    }
                }

                // process the x coordinates

                for (iCoordinate = 0; iCoordinate < nCoordinates; iCoordinate++)
                {
                    if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isXByte))!=0)
                    {
                        byte x = m_bufTable.GetByte(offset++);
                        if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isXSameOrPozitive))!=0)
                        {
                            arrCoordinates[iCoordinate].x = x;
                        }
                        else
                        {
                            arrCoordinates[iCoordinate].x = (short)(0-x);
                        }
                    }
                    else
                    {
                        if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isXSameOrPozitive))!=0)
                        {
                            arrCoordinates[iCoordinate].x = 0;
                        }
                        else
                        {
                            arrCoordinates[iCoordinate].x = m_bufTable.GetShort(offset);
                            offset += 2;
                        }
                    }
                }
                
                
                // process the y coordinates

                for (iCoordinate = 0; iCoordinate < nCoordinates; iCoordinate++)
                {
                    if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isYByte))!=0)
                    {
                        byte y = m_bufTable.GetByte(offset++);
                        if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isYSameOrPozitive))!=0)
                        {
                            arrCoordinates[iCoordinate].y = y;
                        }
                        else
                        {
                            arrCoordinates[iCoordinate].y = (short)(0-y);
                        }
                    }
                    else
                    {
                        if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isYSameOrPozitive))!=0)
                        {
                            arrCoordinates[iCoordinate].y = 0;
                        }
                        else
                        {
                            arrCoordinates[iCoordinate].y = m_bufTable.GetShort(offset);
                            offset += 2;
                        }
                    }
                }
                


                return arrCoordinates;
            }

            public uint CalcLength()
            {
                // allocate the Coordinate array
                int nCoordinates = GetEndPtOfContour((uint)(m_header.numberOfContours-1))+1;
                Coordinate[] arrCoordinates = new Coordinate[nCoordinates];

                // process the flags
                byte [] arrFlags = new Byte[nCoordinates];
                uint offset = m_offsetSimpleGlyph + (uint)m_header.numberOfContours*2 + 2 + instructionLength;
                uint iCoordinate = 0;
                while (iCoordinate < nCoordinates)
                {
                    byte flags = m_bufTable.GetByte(offset++);
                    arrFlags[iCoordinate++] = flags;
                    if ((flags & (byte)MaskFlagKnot.toRepeat) != 0)
                    {
                        // repeat flag is set, next byte is number of additional times this set of flags is repeated
                        byte n = m_bufTable.GetByte(offset++);
                        for (int j=0; j<n; j++)
                        {
                            arrFlags[iCoordinate++] = flags;
                        }
                    }
                }
                for (iCoordinate = 0; iCoordinate < nCoordinates; iCoordinate++)
                {
                    if ((arrFlags[iCoordinate] & (byte)(MaskFlagKnot.isOnCurve)) != 0)
                    {
                        arrCoordinates[iCoordinate].bOnCurve = true;
                    }
                }

                // process the x coordinates

                for (iCoordinate = 0; iCoordinate < nCoordinates; iCoordinate++)
                {
                    if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isXByte))!=0)
                    {
                        byte x = m_bufTable.GetByte(offset++);
                        if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isXSameOrPozitive))!=0)
                        {
                            arrCoordinates[iCoordinate].x = x;
                        }
                        else
                        {
                            arrCoordinates[iCoordinate].x = (short)(0-x);
                        }
                    }
                    else
                    {
                        if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isXSameOrPozitive))!=0)
                        {
                            arrCoordinates[iCoordinate].x = 0;
                        }
                        else
                        {
                            arrCoordinates[iCoordinate].x = m_bufTable.GetShort(offset);
                            offset += 2;
                        }
                    }
                }
                
                
                // process the y coordinates

                for (iCoordinate = 0; iCoordinate < nCoordinates; iCoordinate++)
                {
                    if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isYByte))!=0)
                    {
                        byte y = m_bufTable.GetByte(offset++);
                        if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isYSameOrPozitive))!=0)
                        {
                            arrCoordinates[iCoordinate].y = y;
                        }
                        else
                        {
                            arrCoordinates[iCoordinate].y = (short)(0-y);
                        }
                    }
                    else
                    {
                        if ((arrFlags[iCoordinate]&(byte)(MaskFlagKnot.isYSameOrPozitive))!=0)
                        {
                            arrCoordinates[iCoordinate].y = 0;
                        }
                        else
                        {
                            arrCoordinates[iCoordinate].y = m_bufTable.GetShort(offset);
                            offset += 2;
                        }
                    }
                }

                // offset should now point just past the data in this glyph
                
                return offset - m_offsetSimpleGlyph;
            }

            header m_header;
            uint m_offsetSimpleGlyph;
            MBOBuffer m_bufTable;
        }

        public class CompositeGlyph
        {
            public CompositeGlyph(header h, uint offset, MBOBuffer bufTable)
            {
                m_header = h;
                m_offsetCompositeGlyph = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                flags      = 0,
                glyphIndex = 2,
                argument1  = 4
            }

            public enum Flags
            {
                ARG_1_AND_2_ARE_WORDS     = 0x0001,
                ARGS_ARE_XY_VALUES        = 0x0002,
                ROUND_XY_TO_GRID          = 0x0004,
                WE_HAVE_A_SCALE           = 0x0008,
                NON_OVERLAPPING           = 0x0010,
                MORE_COMPONENTS           = 0x0020,
                WE_HAVE_AN_X_AND_Y_SCALE  = 0x0040,
                WE_HAVE_A_TWO_BY_TWO      = 0x0080,
                WE_HAVE_INSTRUCTIONS      = 0x0100,
                USE_MY_METRICS            = 0x0200,
                OVERLAP_COMPOUND          = 0x0400,
                SCALED_COMPONENT_OFFSET   = 0x0800,
                UNSCALED_COMPONENT_OFFSET = 0x1000
            }

            public ushort flags
            {
                get {return m_bufTable.GetUshort(m_offsetCompositeGlyph + (uint)FieldOffsets.flags);}
            }

            public ushort glyphIndex
            {
                get {return m_bufTable.GetUshort(m_offsetCompositeGlyph + (uint)FieldOffsets.glyphIndex);}
            }

            public ushort arg1
            {
                get
                {
                    if ((flags & (uint)Flags.ARG_1_AND_2_ARE_WORDS) != 0)
                    {
                        return m_bufTable.GetUshort(m_offsetCompositeGlyph + (uint)FieldOffsets.argument1);
                    }
                    else
                    {
                        return m_bufTable.GetByte(m_offsetCompositeGlyph + (uint)FieldOffsets.argument1);
                    }
                }
            }

            public ushort arg2
            {
                get
                {
                    uint offset = m_offsetCompositeGlyph + (uint)FieldOffsets.argument1;
                    if ((flags & (uint)Flags.ARG_1_AND_2_ARE_WORDS) != 0)
                    {
                        offset += 2;
                        return m_bufTable.GetUshort(offset);
                    }
                    else
                    {
                        offset += 1;
                        return m_bufTable.GetByte(offset);
                    }
                }
            }

            public OTF2Dot14 scale
            {
                get
                {
                    if ( (flags & (uint)CompositeGlyph.Flags.WE_HAVE_A_SCALE) == 0)
                    {
                        throw new ApplicationException("WE_HAVE_A_SCALE flag not set");
                    }
                    uint offset = m_offsetCompositeGlyph + (uint)FieldOffsets.argument1;
                    if ( (flags & (uint)CompositeGlyph.Flags.ARG_1_AND_2_ARE_WORDS) != 0)
                    {
                        offset += 4;
                    }
                    else
                    {
                        offset += 2;
                    }
                    
                    return m_bufTable.GetF2Dot14(offset);
                }
            }

            public OTF2Dot14 xscale
            {
                get
                {
                    if ( (flags & (uint)CompositeGlyph.Flags.WE_HAVE_AN_X_AND_Y_SCALE) == 0)
                    {
                        throw new ApplicationException("WE_HAVE_AN_X_AND_Y_SCALE flag not set");
                    }
                    uint offset = m_offsetCompositeGlyph + (uint)FieldOffsets.argument1;
                    if ( (flags & (uint)CompositeGlyph.Flags.ARG_1_AND_2_ARE_WORDS) != 0)
                    {
                        offset += 4;
                    }
                    else
                    {
                        offset += 2;
                    }
                    
                    return m_bufTable.GetF2Dot14(offset);
                }
            }

            public OTF2Dot14 yscale
            {
                get
                {
                    if ( (flags & (uint)CompositeGlyph.Flags.WE_HAVE_AN_X_AND_Y_SCALE) == 0)
                    {
                        throw new ApplicationException("WE_HAVE_AN_X_AND_Y_SCALE flag not set");
                    }
                    uint offset = m_offsetCompositeGlyph + (uint)FieldOffsets.argument1;
                    if ( (flags & (uint)CompositeGlyph.Flags.ARG_1_AND_2_ARE_WORDS) != 0)
                    {
                        offset += 4;
                    }
                    else
                    {
                        offset += 2;
                    }
                    offset += 2;

                    return m_bufTable.GetF2Dot14(offset);
                }
            }

            public OTF2Dot14 xscale_2x2
            {
                get
                {
                    if ( (flags & (uint)CompositeGlyph.Flags.WE_HAVE_AN_X_AND_Y_SCALE) == 0)
                    {
                        throw new ApplicationException("WE_HAVE_AN_X_AND_Y_SCALE flag not set");
                    }
                    uint offset = m_offsetCompositeGlyph + (uint)FieldOffsets.argument1;
                    if ( (flags & (uint)CompositeGlyph.Flags.ARG_1_AND_2_ARE_WORDS) != 0)
                    {
                        offset += 4;
                    }
                    else
                    {
                        offset += 2;
                    }
                    
                    return m_bufTable.GetF2Dot14(offset);
                }
            }

            public OTF2Dot14 scale01_2x2
            {
                get
                {
                    if ( (flags & (uint)CompositeGlyph.Flags.WE_HAVE_AN_X_AND_Y_SCALE) == 0)
                    {
                        throw new ApplicationException("WE_HAVE_AN_X_AND_Y_SCALE flag not set");
                    }
                    uint offset = m_offsetCompositeGlyph + (uint)FieldOffsets.argument1;
                    if ( (flags & (uint)CompositeGlyph.Flags.ARG_1_AND_2_ARE_WORDS) != 0)
                    {
                        offset += 4;
                    }
                    else
                    {
                        offset += 2;
                    }
                    offset += 2;

                    return m_bufTable.GetF2Dot14(offset);
                }
            }

            public OTF2Dot14 scale10_2x2
            {
                get
                {
                    if ( (flags & (uint)CompositeGlyph.Flags.WE_HAVE_AN_X_AND_Y_SCALE) == 0)
                    {
                        throw new ApplicationException("WE_HAVE_AN_X_AND_Y_SCALE flag not set");
                    }
                    uint offset = m_offsetCompositeGlyph + (uint)FieldOffsets.argument1;
                    if ( (flags & (uint)CompositeGlyph.Flags.ARG_1_AND_2_ARE_WORDS) != 0)
                    {
                        offset += 4;
                    }
                    else
                    {
                        offset += 2;
                    }
                    offset += 4;
                    
                    return m_bufTable.GetF2Dot14(offset);
                }
            }

            public OTF2Dot14 yscale_2x2
            {
                get
                {
                    if ( (flags & (uint)CompositeGlyph.Flags.WE_HAVE_AN_X_AND_Y_SCALE) == 0)
                    {
                        throw new ApplicationException("WE_HAVE_AN_X_AND_Y_SCALE flag not set");
                    }
                    uint offset = m_offsetCompositeGlyph + (uint)FieldOffsets.argument1;
                    if ( (flags & (uint)CompositeGlyph.Flags.ARG_1_AND_2_ARE_WORDS) != 0)
                    {
                        offset += 4;
                    }
                    else
                    {
                        offset += 2;
                    }
                    offset += 6;

                    return m_bufTable.GetF2Dot14(offset);
                }
            }

            public bool AnyComponentsHaveInstructions()
            {
                bool bInstructions = false;

                if ((flags & (uint)Flags.WE_HAVE_INSTRUCTIONS) != 0)
                {
                    bInstructions = true;
                }
                else
                {
                    CompositeGlyph nextCG = GetNextCompositeGlyph();
                    while (nextCG!=null)
                    {
                        if ((nextCG.flags & (uint)Flags.WE_HAVE_INSTRUCTIONS) != 0)
                        {
                            bInstructions = true;
                            break;
                        }
                        nextCG = nextCG.GetNextCompositeGlyph();
                    }
                }

                return bInstructions;
            }

            public ushort GetNumInstructions()
            {
                ushort nInstructions = 0;

                if (AnyComponentsHaveInstructions())
                {
                    // instructions follow the last component
                    CompositeGlyph lastCG = this;
                    CompositeGlyph nextCG = GetNextCompositeGlyph();
                    while (nextCG!=null)
                    {
                        lastCG = nextCG;
                        nextCG = lastCG.GetNextCompositeGlyph();
                    }

                    uint nOffset = lastCG.m_offsetCompositeGlyph + lastCG.GetCompositeGlyphLength();

                    nInstructions = m_bufTable.GetUshort(nOffset);
                }

                return nInstructions;
            }

            public byte GetInstruction(uint i)
            {
                byte byteInstruction = 0;
                ushort nInstructions = 0;

                if (AnyComponentsHaveInstructions())
                {
                    // instructions follow the last component
                    CompositeGlyph lastCG = this;
                    CompositeGlyph nextCG = GetNextCompositeGlyph();
                    while (nextCG!=null)
                    {
                        lastCG = nextCG;
                        nextCG = lastCG.GetNextCompositeGlyph();
                    }

                    uint nOffset = lastCG.m_offsetCompositeGlyph + lastCG.GetCompositeGlyphLength();

                    nInstructions = m_bufTable.GetUshort(nOffset);

                    if (i < nInstructions)
                    {
                        byteInstruction = m_bufTable.GetByte(nOffset + 2 + i);
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    throw new ApplicationException("WE_HAVE_INSTRUCTIONS flag is clear");
                }

                return byteInstruction;
            }

            public uint GetCompositeGlyphLength()
            {
                uint nLen = 4; // 2 for the flags field, and 2 for the glyph index field

                // component positioning info

                if ( (flags & (uint)CompositeGlyph.Flags.ARG_1_AND_2_ARE_WORDS) ==  (uint)CompositeGlyph.Flags.ARG_1_AND_2_ARE_WORDS)
                {
                    nLen += 4;
                }
                else
                {
                    nLen += 2;
                }

                // component scaler

                if ( (flags & (uint)CompositeGlyph.Flags.WE_HAVE_A_SCALE) == (uint)CompositeGlyph.Flags.WE_HAVE_A_SCALE)
                {
                    nLen += 2;
                }
                else if ( (flags & (uint)CompositeGlyph.Flags.WE_HAVE_AN_X_AND_Y_SCALE) ==  (uint)CompositeGlyph.Flags.WE_HAVE_AN_X_AND_Y_SCALE)
                {
                    nLen += 4;
                }
                else if ( (flags & (uint)CompositeGlyph.Flags.WE_HAVE_A_TWO_BY_TWO) ==  (uint)CompositeGlyph.Flags.WE_HAVE_A_TWO_BY_TWO)
                {
                    nLen += 8;
                }

                return nLen;
            }

            public CompositeGlyph GetNextCompositeGlyph()
            {
                CompositeGlyph nextCompGlyph = null;

                if ((flags & (uint)Flags.MORE_COMPONENTS) == (uint)Flags.MORE_COMPONENTS)
                {
                    uint offset = m_offsetCompositeGlyph + GetCompositeGlyphLength();

                    nextCompGlyph = new CompositeGlyph(m_header, offset, m_bufTable);
                }

                return nextCompGlyph;
            }

            public uint CalcLength()
            {
                uint length = 0;

                if (AnyComponentsHaveInstructions())
                {
                    length += (uint)2 + GetNumInstructions();
                }

                CompositeGlyph nextCG = this;
                while (nextCG!=null)
                {
                    length += nextCG.GetCompositeGlyphLength();
                    nextCG = nextCG.GetNextCompositeGlyph();
                }

                return length;
            }

            header m_header;
            uint m_offsetCompositeGlyph;
            MBOBuffer m_bufTable;
        }



        /************************
         * accessors
         */

        public MBOBuffer Buffer
        {
            get { return this.m_bufTable; }
        }

        public header GetGlyphHeader(uint iGlyph, OTFont fontOwner)
        {
            if (iGlyph >= fontOwner.GetMaxpNumGlyphs())
            {
                throw new ArgumentOutOfRangeException("iGlyph");
            }

            header h = null;

            Table_loca locaTable = (Table_loca)fontOwner.GetTable("loca");
            if (locaTable != null)
            {
                int offsGlyph, length;
                if (locaTable.GetEntryGlyf((int)iGlyph, 
                    out offsGlyph, out length, fontOwner))
                {
                    if (length!=0)
                    {
                        h = new header((uint)offsGlyph, m_bufTable);
                    }
                }
            }

            return h;
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            return m_cache;
        }

        public void BuildCache(OTFont fontOwner)
        {
            m_cache = new glyf_cache(this, fontOwner);
        }
        
        public class glyf_cache : DataCache
        {
            // constructor

            public glyf_cache(Table_glyf OwnerTable, OTFont OwnerFont)
            {
                m_arrGlyphs = new ArrayList();


                // get each glyph from the glyf table

                for (uint iGlyph=0; iGlyph<OwnerFont.GetMaxpNumGlyphs(); iGlyph++)
                {
                    header h = OwnerTable.GetGlyphHeader(iGlyph, OwnerFont);
                    if (h == null)
                    {
                        m_arrGlyphs.Add(null);
                    }
                    else
                    {
                        glyph_base gb = GetGlyphLogicalData(h);
                        m_arrGlyphs.Add(gb);
                    }
                }

            }

            static public glyph_simple GetSimpleGlyphLogicalData(header h)
            {
                glyph_simple gs = null;
                SimpleGlyph sg = h.GetSimpleGlyph();
                if (sg != null)
                {
                    // glyph is a simple glyph

                    gs = new glyph_simple(h.numberOfContours, h.xMin, h.yMin, h.xMax, h.yMax);

                    SimpleGlyph.Coordinate[] coords = sg.GetDecodedRelativeCoordinates();
                    glyph_simple.Coordinate16 prevPoint;
                    prevPoint.x = prevPoint.y = 0;
                    glyph_simple.Coordinate16 newPoint;
                    for (uint iContour=0; iContour<h.numberOfContours; iContour++)
                    {
                        // convert the coordinates in this contour from relative to absolute
                        uint startCoord = 0;
                        if (iContour > 0)
                        {
                            startCoord = (uint)sg.GetEndPtOfContour(iContour-1)+1;
                        }
                        uint endCoord = sg.GetEndPtOfContour(iContour);

                        int nPoints = (int)(endCoord - startCoord + 1);
                        glyph_simple.contour Contour = new glyph_simple.contour(nPoints);

                        for (uint iCoord = startCoord; iCoord <= endCoord; iCoord++)
                        {
                            newPoint.x = (short)(prevPoint.x + coords[iCoord].x);
                            newPoint.y = (short)(prevPoint.y + coords[iCoord].y);
                            newPoint.bOnCurve = coords[iCoord].bOnCurve;
                            Contour.m_arrPoints.Add(newPoint);
                            prevPoint = newPoint;
                        }

                        gs.m_arrContours.Add(Contour);

                        // copy the hinting instructions
                        gs.m_arrInstructions = new byte[sg.instructionLength];
                        for (uint i=0; i<sg.instructionLength; i++)
                        {
                            gs.m_arrInstructions[i] = sg.GetInstruction(i);
                        }
                    }
                }
                return gs;
            }

            static public glyph_composite GetCompositeGlyphLogicalData(header h)
            {
                glyph_composite gc = null;

                CompositeGlyph cg = h.GetCompositeGlyph();
                if (cg != null)
                {
                    // glyph is a composite glyph

                    gc = new glyph_composite(h.xMin, h.yMin, h.xMax, h.yMax);

                    // copy any hinting instructions
                    if (cg.AnyComponentsHaveInstructions())
                    {
                        ushort nInstructions = cg.GetNumInstructions();
                        gc.m_arrInstructions = new byte[nInstructions];
                        for (uint i=0; i<nInstructions; i++)
                        {
                            gc.m_arrInstructions[i] = cg.GetInstruction(i);
                        }
                    }

                    while (cg != null)
                    {
                        glyph_composite.component cc = new glyph_composite.component();
                        cc.flags   = cg.flags;
                        cc.glyphid = cg.glyphIndex;
                        cc.arg1    = cg.arg1;
                        cc.arg2    = cg.arg2;
                        if ((cg.flags & (ushort)CompositeGlyph.Flags.WE_HAVE_A_SCALE) != 0)
                        {
                            cc.scale = cg.scale;
                        }
                        else if ((cg.flags & (ushort)CompositeGlyph.Flags.WE_HAVE_AN_X_AND_Y_SCALE) != 0)
                        {
                            cc.xscale = cg.xscale;
                            cc.yscale = cg.yscale;
                        }
                        else if ((cg.flags & (ushort)CompositeGlyph.Flags.WE_HAVE_A_TWO_BY_TWO) != 0)
                        {
                            cc.xscale_2x2  = cg.xscale_2x2;
                            cc.scale01_2x2 = cg.scale01_2x2;
                            cc.scale10_2x2 = cg.scale10_2x2;
                            cc.yscale_2x2  = cg.yscale_2x2;
                        }

                        gc.m_arrComponents.Add(cc);

                        cg = cg.GetNextCompositeGlyph();
                    }
                }

                return gc;
            }

            static public glyph_base GetGlyphLogicalData(Table_glyf.header h)
            {
                glyph_base gb = null;

                SimpleGlyph sg = h.GetSimpleGlyph();
                if (sg != null)
                {
                    // glyph is a simple glyph

                    gb = GetSimpleGlyphLogicalData(h);
                }
                else
                {
                    CompositeGlyph cg = h.GetCompositeGlyph();
                    if (cg != null)
                    {
                        // glyph is a composite glyph

                        gb = GetCompositeGlyphLogicalData(h);
                    }
                }

                return gb;
            }


            // nested classes

            public class glyph_base
            {
                public glyph_base(short nContours, short xMin, short yMin, short xMax, short yMax)
                {
                    m_numberOfContours = nContours;
                    m_xMin = xMin;
                    m_yMin = yMin;
                    m_xMax = xMax;
                    m_yMax = yMax;
                }

                public short m_numberOfContours;
                public short m_xMin, m_yMin, m_xMax, m_yMax;
            }

            // glyph_simple contains the logical data of a simple glyph in a more practical form
            public class glyph_simple : glyph_base
            {
                public glyph_simple(short nContours, short xMin, short yMin, short xMax, short yMax)
                    : base(nContours, xMin, yMin, xMax, yMax)
                {
                    m_arrContours = new ArrayList(nContours);
                    m_arrInstructions = new byte[0];
                }

                public contour GetContour(int i)
                {
                    return (contour)m_arrContours[i];
                }

                public class contour
                {
                    public contour(int nPoints)
                    {
                        m_arrPoints = new ArrayList(nPoints);
                    }

                    public Coordinate16 GetCoordinates(int i)
                    {
                        return (Coordinate16)m_arrPoints[i];
                    }

                    // note: these points use absolute coordinates, not relative coordinates!
                    public ArrayList m_arrPoints;
                }

                public struct Coordinate16
                {
                    public short x, y;
                    public bool bOnCurve;
                }

                public ArrayList m_arrContours;
                public byte [] m_arrInstructions;
            }

            public class glyph_composite : glyph_base
            {
                public glyph_composite(short xMin, short yMin, short xMax, short yMax)
                    : base(-1, xMin, yMin, xMax, yMax)
                {
                    m_arrComponents = new ArrayList();
                    m_arrInstructions = new byte[0];
                }

                public class component
                {
                    public ushort flags;
                    public ushort glyphid;
                    public ushort arg1;
                    public ushort arg2;

                    public OTF2Dot14 scale;

                    public OTF2Dot14 xscale;
                    public OTF2Dot14 yscale;

                    public OTF2Dot14 xscale_2x2;
                    public OTF2Dot14 scale01_2x2;
                    public OTF2Dot14 scale10_2x2;
                    public OTF2Dot14 yscale_2x2;

                }

                public ArrayList m_arrComponents;
                public byte [] m_arrInstructions;
            }


            // generate a new table from the cached data
            public override OTTable GenerateTable()
            {
                // not yet implemented!

                // probably need to generate a loca table at the same time, and provide a method to retrieve it

                return null;
            }

            public ArrayList m_arrGlyphs;
        }
        

    }
}
