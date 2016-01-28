using System;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_GDEF.
    /// </summary>
    public class Table_GDEF : OTTable
    {
        /************************
         * constructors
         */
                
        public Table_GDEF(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            Version                  = 0,
            GlyphClassDefOffset      = 4,
            AttachListOffset         = 6,
            LigCaretListOffset       = 8,
            MarkAttachClassDefOffset = 10,
            MarkGlyphSetsDefOffset   = 12
        }

        /************************
         * classes
         */

        public class AttachListTable
        {
            public AttachListTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetAttachListTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                CoverageOffset     = 0,
                GlyphCount         = 2,
                AttachPointOffsets = 4
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.AttachPointOffsets + (uint)GlyphCount*2;
            }

            public ushort CoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetAttachListTable + (uint)FieldOffsets.CoverageOffset);}
            }

            public ushort GlyphCount
            {
                get {return m_bufTable.GetUshort(m_offsetAttachListTable + (uint)FieldOffsets.GlyphCount);}
            }

            public ushort GetAttachPointOffset(uint i)
            {
                return m_bufTable.GetUshort(m_offsetAttachListTable + (uint)FieldOffsets.AttachPointOffsets + i*2);
            }

            public OTL.CoverageTable GetCoverageTable()
            {
                ushort offset = (ushort)(m_offsetAttachListTable + CoverageOffset);
                return new OTL.CoverageTable(offset, m_bufTable);
            }

            public AttachPointTable GetAttachPointTable(uint i)
            {
                ushort offset = GetAttachPointOffset(i);                 
                return new AttachPointTable(offset, m_bufTable);
            }

            protected ushort m_offsetAttachListTable;
            protected MBOBuffer m_bufTable;
        }

        public class AttachPointTable
        {
            public AttachPointTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetAttachPointTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                PointCount = 0,
                PointIndexArray = 2
            }

            public ushort PointCount
            {
                get {return m_bufTable.GetUshort(m_offsetAttachPointTable + (uint)FieldOffsets.PointCount);}
            }

            public ushort GetPointIndex(uint i)
            {
                return m_bufTable.GetUshort(m_offsetAttachPointTable + (uint)FieldOffsets.PointIndexArray + i*2);
            }

            protected ushort m_offsetAttachPointTable;
            protected MBOBuffer m_bufTable;
        }

        public class LigCaretListTable
        {
            public LigCaretListTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetLigCaretListTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                CoverageOffset  = 0,
                LigGlyphCount   = 2,
                LigGlyphOffsets = 4
            }


            public uint CalcLength()
            {
                return (uint)FieldOffsets.LigGlyphOffsets + (uint)LigGlyphCount*2;
            }


            public ushort CoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetLigCaretListTable + (uint)FieldOffsets.CoverageOffset);}
            }

            public ushort LigGlyphCount
            {
                get {return m_bufTable.GetUshort(m_offsetLigCaretListTable + (uint)FieldOffsets.LigGlyphCount);}
            }

            public ushort GetLigGlyphOffset(uint i)
            {
                return m_bufTable.GetUshort(m_offsetLigCaretListTable + (uint)FieldOffsets.LigGlyphOffsets + i*2);
            }

            public OTL.CoverageTable GetCoverageTable()
            {
                ushort offset = (ushort)(m_offsetLigCaretListTable + CoverageOffset);
                return new OTL.CoverageTable(offset, m_bufTable);
            }

            public LigGlyphTable GetLigGlyphTable(uint i)
            {
                ushort offset = GetLigGlyphOffset(i);                 
                return new LigGlyphTable((ushort)(m_offsetLigCaretListTable + offset), m_bufTable);
            }

            protected ushort m_offsetLigCaretListTable;
            protected MBOBuffer m_bufTable;
        }

        public class LigGlyphTable
        {
            public LigGlyphTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetLigGlyphTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                CaretCount        = 0,
                CaretValueOffsets = 2
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.CaretValueOffsets + (uint)CaretCount*2;
            }

            public ushort CaretCount
            {
                get {return m_bufTable.GetUshort(m_offsetLigGlyphTable + (uint)FieldOffsets.CaretCount);}
            }

            public ushort GetCaretValueOffset(uint i)
            {
                return m_bufTable.GetUshort(m_offsetLigGlyphTable + (uint)FieldOffsets.CaretValueOffsets + i*2);
            }

            public CaretValueTable GetCaretValueTable(uint i)
            {
                ushort offset = (ushort)(m_offsetLigGlyphTable + GetCaretValueOffset(i));
                return new CaretValueTable(offset, m_bufTable);
            }

            protected ushort m_offsetLigGlyphTable;
            protected MBOBuffer m_bufTable;
        }

        public class CaretValueTable
        {
            public CaretValueTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetCaretValueTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                CaretValueFormat = 0
            }
            public enum FieldOffsets1
            {
                Coordinate = 2
            }
            public enum FieldOffsets2
            {
                CaretValuePoint = 2
            }
            public enum FieldOffsets3
            {
                Coordinate        = 2,
                DeviceTableOffset = 3
            }

            public ushort CaretValueFormat
            {
                get {return m_bufTable.GetUshort(m_offsetCaretValueTable + (uint)FieldOffsets.CaretValueFormat);}
            }

            // format 1 only!
            public ushort F1Coordinate
            {
                get
                {
                    if (CaretValueFormat != 1)
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_bufTable.GetUshort(m_offsetCaretValueTable + (uint)FieldOffsets1.Coordinate);
                }
            }

            // format 2 only!
            public ushort F2CaretValuePoint
            {
                get
                {
                    if (CaretValueFormat != 2)
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_bufTable.GetUshort(m_offsetCaretValueTable + (uint)FieldOffsets2.CaretValuePoint);
                }
            }

            // format 3 only!
            public ushort F3Coordinate
            {
                get
                {
                    if (CaretValueFormat != 3)
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_bufTable.GetUshort(m_offsetCaretValueTable + (uint)FieldOffsets3.Coordinate);
                }
            }

            public ushort F3DeviceTableOffset
            {
                get
                {
                    if (CaretValueFormat != 3)
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_bufTable.GetUshort(m_offsetCaretValueTable + (uint)FieldOffsets3.DeviceTableOffset);
                }
            }

            public OTL.DeviceTable F3GetDeviceTable()
            {
                if (CaretValueFormat != 3)
                {
                    throw new System.InvalidOperationException();
                }

                ushort offset = (ushort)(m_offsetCaretValueTable + F3DeviceTableOffset);
                return new OTL.DeviceTable(offset, m_bufTable);
            }


            protected ushort m_offsetCaretValueTable;
            protected MBOBuffer m_bufTable;
        }

        public class MarkGlyphSetsDefTable
        {
            public MarkGlyphSetsDefTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetMarkGlyphSetsDefTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                MarkSetTableFormat = 0, // uint16
                MarkSetCount       = 2, // uint16
                Coverage           = 4  // ULONG [MarkSetCount]
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.Coverage + (uint)MarkSetCount*4;
            }

            public ushort MarkSetTableFormat
            {
                get {return m_bufTable.GetUshort(m_offsetMarkGlyphSetsDefTable + (uint)FieldOffsets.MarkSetTableFormat);}
            }

            public ushort MarkSetCount
            {
                get {return m_bufTable.GetUshort(m_offsetMarkGlyphSetsDefTable + (uint)FieldOffsets.MarkSetCount);}
            }

            public uint GetCoverage(uint i)
            {
                return m_bufTable.GetUint(m_offsetMarkGlyphSetsDefTable + (uint)FieldOffsets.Coverage + i*4);
            }

            protected ushort m_offsetMarkGlyphSetsDefTable;
            protected MBOBuffer m_bufTable;
        }

        /************************
         * accessors
         */
        
        public OTFixed Version
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.Version);}
        }

        public ushort GlyphClassDefOffset
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.GlyphClassDefOffset);}
        }

        public ushort AttachListOffset
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.AttachListOffset);}
        }

        public ushort LigCaretListOffset
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.LigCaretListOffset);}
        }

        public ushort MarkAttachClassDefOffset
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.MarkAttachClassDefOffset);}
        }

        public ushort MarkGlyphSetsDefOffset
        {
            get {
                if ( Version.GetUint() > 0x00010000 )
                    return m_bufTable.GetUshort((uint)FieldOffsets.MarkGlyphSetsDefOffset);
                else
                    return 0;
            }
        }

        public OTL.ClassDefTable GetGlyphClassDefTable()
        {
            OTL.ClassDefTable cdt = null;

            if (GlyphClassDefOffset != 0)
            {
                cdt = new OTL.ClassDefTable(GlyphClassDefOffset, m_bufTable);
            }

            return cdt;
        }

        public AttachListTable GetAttachListTable()
        {
            AttachListTable alt = null;

            if (AttachListOffset != 0)
            {
                alt = new AttachListTable(AttachListOffset, m_bufTable);
            }

            return alt;
        }

        public LigCaretListTable GetLigCaretListTable()
        {
            LigCaretListTable lclt = null;

            if (LigCaretListOffset != 0)
            {
                lclt = new LigCaretListTable(LigCaretListOffset, m_bufTable);
            }

            return lclt;
        }

        public OTL.ClassDefTable GetMarkAttachClassDefTable()
        {
            OTL.ClassDefTable cdt = null;

            if (MarkAttachClassDefOffset != 0)
            {
                cdt = new OTL.ClassDefTable(MarkAttachClassDefOffset, m_bufTable);
            }

            return cdt;
        }

        public MarkGlyphSetsDefTable GetMarkGlyphSetsDefTable()
        {
            MarkGlyphSetsDefTable mgsdt = null;

            if (MarkGlyphSetsDefOffset != 0)
            {
                mgsdt = new MarkGlyphSetsDefTable(MarkGlyphSetsDefOffset, m_bufTable);
            }

            return mgsdt;
        }

        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new GDEF_cache();
            }

            return m_cache;
        }
        
        public class GDEF_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        

    }
}
