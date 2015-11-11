using System;
using System.Diagnostics;

namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_GPOS.
    /// </summary>
    public class Table_GPOS : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_GPOS(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * field offset values
         */

        public enum FieldOffsets
        {
            Version           = 0,
            ScriptListOffset  = 4,
            FeatureListOffset = 6,
            LookupListOffset  = 8
        }




        /************************
         * classes
         */

        public class ValueRecord
        {
            public ValueRecord(uint offset, MBOBuffer bufTable, uint offsetPosTable, ushort ValueFormat)
            {
                m_offsetValueRecord = offset;
                m_bufTable = bufTable;
                m_offsetPosTable = offsetPosTable;
                m_ValueFormat = ValueFormat;

                // calculate the offsets to each field
                ushort FieldOffset = 0;
                if (XPlacementPresent)
                {
                    XPlacementFieldOffset = FieldOffset;
                    FieldOffset += 2;
                }
                if (YPlacementPresent)
                {
                    YPlacementFieldOffset = FieldOffset;
                    FieldOffset += 2;
                }
                if (XAdvancePresent)
                {
                    XAdvanceFieldOffset = FieldOffset;
                    FieldOffset += 2;
                }
                if (YAdvancePresent)
                {
                    YAdvanceFieldOffset = FieldOffset;
                    FieldOffset += 2;
                }
                if (XPlaDevicePresent)
                {
                    XPlaDeviceFieldOffset = FieldOffset;
                    FieldOffset += 2;
                }
                if (YPlaDevicePresent)
                {
                    YPlaDeviceFieldOffset = FieldOffset;
                    FieldOffset += 2;
                }
                if (XAdvDevicePresent)
                {
                    XAdvDeviceFieldOffset = FieldOffset;
                    FieldOffset += 2;
                }
                if (YAdvDevicePresent)
                {
                    YAdvDeviceFieldOffset = FieldOffset;
                    FieldOffset += 2;
                }
            }

            public static uint SizeOfValueRecord(ushort ValueFormat)
            {
                uint bit = 0x0001;
                uint SizeOfValueRecord = 0;
                for (uint i=0; i<8; i++)
                {
                    if ((ValueFormat & bit) != 0)
                    {
                        SizeOfValueRecord += 2;
                    }
                    bit = bit << 1;
                }
                return SizeOfValueRecord;
            }


            public ushort XPlacement
            {
                get
                {
                    if (!XPlacementPresent)
                    {
                        throw new InvalidOperationException(" is not present in ValueFormat field");
                    }
                    return m_bufTable.GetUshort(m_offsetValueRecord + (uint)XPlacementFieldOffset);
                }
            }

            public ushort YPlacement
            {
                get
                {
                    if (!YPlacementPresent)
                    {
                        throw new InvalidOperationException(" is not present in ValueFormat field");
                    }
                    return m_bufTable.GetUshort(m_offsetValueRecord + (uint)YPlacementFieldOffset);
                }
            }

            public ushort XAdvance
            {
                get
                {
                    if (!XAdvancePresent)
                    {
                        throw new InvalidOperationException(" is not present in ValueFormat field");
                    }
                    return m_bufTable.GetUshort(m_offsetValueRecord + (uint)XAdvanceFieldOffset);
                }
            }

            public ushort YAdvance
            {
                get
                {
                    if (!YAdvancePresent)
                    {
                        throw new InvalidOperationException(" is not present in ValueFormat field");
                    }
                    return m_bufTable.GetUshort(m_offsetValueRecord + (uint)YAdvanceFieldOffset);
                }
            }

            public ushort XPlaDeviceOffset
            {
                get
                {
                    if (!XPlaDevicePresent)
                    {
                        throw new InvalidOperationException(" is not present in ValueFormat field");
                    }
                    return m_bufTable.GetUshort(m_offsetValueRecord + (uint)XPlaDeviceFieldOffset);
                }
            }

            public ushort YPlaDeviceOffset
            {
                get
                {
                    if (!YPlaDevicePresent)
                    {
                        throw new InvalidOperationException(" is not present in ValueFormat field");
                    }
                    return m_bufTable.GetUshort(m_offsetValueRecord + (uint)YPlaDeviceFieldOffset);
                }
            }

            public ushort XAdvDeviceOffset
            {
                get
                {
                    if (!XAdvDevicePresent)
                    {
                        throw new InvalidOperationException(" is not present in ValueFormat field");
                    }
                    return m_bufTable.GetUshort(m_offsetValueRecord + (uint)XAdvDeviceFieldOffset);
                }
            }

            public ushort YAdvDeviceOffset
            {
                get
                {
                    if (!YAdvDevicePresent)
                    {
                        throw new InvalidOperationException(" is not present in ValueFormat field");
                    }
                    return m_bufTable.GetUshort(m_offsetValueRecord + (uint)YAdvDeviceFieldOffset);
                }
            }

            public OTL.DeviceTable GetXPlaDeviceTable()
            {
                OTL.DeviceTable dt = null;

                if (XPlaDevicePresent)
                {
                    if (XPlaDeviceOffset != 0)
                    {
                        dt = new OTL.DeviceTable(m_offsetPosTable + XPlaDeviceOffset, m_bufTable);
                    }
                }

                return dt;
            }

            public OTL.DeviceTable GetYPlaDeviceTable()
            {
                OTL.DeviceTable dt = null;

                if (YPlaDevicePresent)
                {
                    if (YPlaDeviceOffset != 0)
                    {
                        dt = new OTL.DeviceTable(m_offsetPosTable + YPlaDeviceOffset, m_bufTable);
                    }
                }

                return dt;
            }

            public OTL.DeviceTable GetXAdvDeviceTable()
            {
                OTL.DeviceTable dt = null;

                if (XAdvDevicePresent)
                {
                    if (XAdvDeviceOffset != 0)
                    {
                        dt = new OTL.DeviceTable(m_offsetPosTable + XAdvDeviceOffset, m_bufTable);
                    }
                }

                return dt;
            }

            public OTL.DeviceTable GetYAdvDeviceTable()
            {
                OTL.DeviceTable dt = null;

                if (YAdvDevicePresent)
                {
                    if (YAdvDeviceOffset != 0)
                    {
                        dt = new OTL.DeviceTable(m_offsetPosTable + YAdvDeviceOffset, m_bufTable);
                    }
                }

                return dt;
            }



            public bool XPlacementPresent
            {
                get {return ((m_ValueFormat & 0x0001) != 0);}
            }

            public bool YPlacementPresent
            {
                get {return ((m_ValueFormat & 0x0002) != 0);}
            }

            public bool XAdvancePresent
            {
                get {return ((m_ValueFormat & 0x0004) != 0);}
            }

            public bool YAdvancePresent
            {
                get {return ((m_ValueFormat & 0x0008) != 0);}
            }

            public bool XPlaDevicePresent
            {
                get {return ((m_ValueFormat & 0x0010) != 0);}
            }

            public bool YPlaDevicePresent
            {
                get {return ((m_ValueFormat & 0x0020) != 0);}
            }

            public bool XAdvDevicePresent
            {
                get {return ((m_ValueFormat & 0x0040) != 0);}
            }

            public bool YAdvDevicePresent
            {
                get {return ((m_ValueFormat & 0x0080) != 0);}
            }


            protected uint m_offsetValueRecord;
            protected MBOBuffer m_bufTable;
            protected uint m_offsetPosTable;
            protected ushort m_ValueFormat;

            protected ushort XPlacementFieldOffset;
            protected ushort YPlacementFieldOffset;
            protected ushort XAdvanceFieldOffset;
            protected ushort YAdvanceFieldOffset;
            protected ushort XPlaDeviceFieldOffset;
            protected ushort YPlaDeviceFieldOffset;
            protected ushort XAdvDeviceFieldOffset;
            protected ushort YAdvDeviceFieldOffset;

        }

        public class AnchorTable
        {
            public AnchorTable(uint offset, MBOBuffer bufTable)
            {
                m_offsetAnchorTable = offset;
                m_bufTable = bufTable;
            }


            public enum FieldOffsets
            {
                AnchorFormat = 0,
            }

            // nested classes
            public class AnchorFormat1
            {
                public AnchorFormat1(uint offset, MBOBuffer bufTable)
                {
                    m_offsetAnchorTable = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    AnchorFormat = 0,
                    XCoordinate = 2,
                    YCoordinate = 4
                }

                public uint CalcLength()
                {
                    return 6;
                }

                // accessors

                public ushort AnchorFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.AnchorFormat);}
                }

                public ushort XCoordinate
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.XCoordinate);}
                }

                public ushort YCoordinate
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.YCoordinate);}
                }

                protected uint m_offsetAnchorTable;
                protected MBOBuffer m_bufTable;
            }

            public class AnchorFormat2
            {
                public AnchorFormat2(uint offset, MBOBuffer bufTable)
                {
                    m_offsetAnchorTable = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    AnchorFormat = 0,
                    XCoordinate  = 2,
                    YCoordinate  = 4,
                    AnchorPoint  = 6
                }

                public uint CalcLength()
                {
                    return 8;
                }

                // accessors

                public ushort AnchorFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.AnchorFormat);}
                }

                public ushort XCoordinate
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.XCoordinate);}
                }

                public ushort YCoordinate
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.YCoordinate);}
                }

                public ushort AnchorPoint
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.AnchorPoint);}
                }

                protected uint m_offsetAnchorTable;
                protected MBOBuffer m_bufTable;
            }

            public class AnchorFormat3
            {
                public AnchorFormat3(uint offset, MBOBuffer bufTable)
                {
                    m_offsetAnchorTable = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    AnchorFormat       = 0,
                    XCoordinate        = 2,
                    YCoordinate        = 4,
                    XDeviceTableOffset = 6,
                    YDeviceTableOffset = 8
                }

                public uint CalcLength()
                {
                    return 10;
                }

                // accessors

                public ushort AnchorFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.AnchorFormat);}
                }

                public ushort XCoordinate
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.XCoordinate);}
                }

                public ushort YCoordinate
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.YCoordinate);}
                }

                public ushort XDeviceTableOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.XDeviceTableOffset);}
                }

                public ushort YDeviceTableOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.YDeviceTableOffset);}
                }

                public OTL.DeviceTable GetXDeviceTable()
                {
                    OTL.DeviceTable dt = null;
                    if (XDeviceTableOffset != 0)
                    {
                        dt = new OTL.DeviceTable(m_offsetAnchorTable + XDeviceTableOffset, m_bufTable);
                    }
                    return dt;
                }

                public OTL.DeviceTable GetYDeviceTable()
                {
                    OTL.DeviceTable dt = null;
                    if (YDeviceTableOffset != 0)
                    {
                        dt = new OTL.DeviceTable(m_offsetAnchorTable + YDeviceTableOffset, m_bufTable);
                    }
                    return dt;
                }

                protected uint m_offsetAnchorTable;
                protected MBOBuffer m_bufTable;
            }
            

            // accessors

            public ushort AnchorFormat
            {
                get {return m_bufTable.GetUshort(m_offsetAnchorTable + (uint)FieldOffsets.AnchorFormat);}
            }

            public AnchorFormat1 GetAnchorFormat1()
            {
                if (AnchorFormat != 1)
                {
                    throw new InvalidOperationException("format not 1");
                }
                return new AnchorFormat1(m_offsetAnchorTable, m_bufTable);
            }

            public AnchorFormat2 GetAnchorFormat2()
            {
                if (AnchorFormat != 2)
                {
                    throw new InvalidOperationException("format not 2");
                }
                return new AnchorFormat2(m_offsetAnchorTable, m_bufTable);
            }

            public AnchorFormat3 GetAnchorFormat3()
            {
                if (AnchorFormat != 3)
                {
                    throw new InvalidOperationException("format not 3");
                }
                return new AnchorFormat3(m_offsetAnchorTable, m_bufTable);
            }

            protected uint m_offsetAnchorTable;
            protected MBOBuffer m_bufTable;
        }

        public class MarkArray
        {
            public MarkArray(uint offset, MBOBuffer bufTable)
            {
                m_offsetMarkArray = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                MarkCount       = 0,
                MarkRecordArray = 2,
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.MarkRecordArray + (uint)MarkCount*4;
            }

            public class MarkRecord
            {
                public ushort Class;
                public ushort MarkAnchorOffset;
            }

            public ushort MarkCount
            {
                get {return m_bufTable.GetUshort(m_offsetMarkArray + (uint)FieldOffsets.MarkCount);}
            }

            public MarkRecord GetMarkRecord(uint i)
            {
                MarkRecord mr = null;

                if (i < MarkCount)
                {
                    mr = new MarkRecord();
                    uint offset = m_offsetMarkArray + (uint)FieldOffsets.MarkRecordArray + i*4;
                    mr.Class            = m_bufTable.GetUshort(offset);
                    mr.MarkAnchorOffset = m_bufTable.GetUshort(offset + 2);
                }

                return mr;
            }

            protected uint m_offsetMarkArray;
            protected MBOBuffer m_bufTable;
        }



        // Lookup Type 1: single adjustment positioning subtable

        public class SinglePos : OTL.SubTable
        {
            public SinglePos(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetSinglePos = offset;
            }

            public enum FieldOffsets
            {
                PosFormat   = 0
            }


            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                return 1;
            }



            // nested classes

            public class SinglePosFormat1
            {
                public SinglePosFormat1(uint offset, MBOBuffer bufTable)
                {
                    m_offsetSinglePos = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    PosFormat   = 0,
                    Coverage    = 2,
                    ValueFormat = 4,
                    Value       = 6
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.Value + ValueRecord.SizeOfValueRecord(ValueFormat);
                }

                public ushort PosFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetSinglePos + (uint)FieldOffsets.PosFormat);}
                }

                public ushort Coverage
                {
                    get {return m_bufTable.GetUshort(m_offsetSinglePos + (uint)FieldOffsets.Coverage);}
                }

                OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetSinglePos + Coverage, m_bufTable);
                }

                public ushort ValueFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetSinglePos + (uint)FieldOffsets.ValueFormat);}
                }

                public ValueRecord Value
                {
                    get
                    {
                        uint offset = m_offsetSinglePos + (uint)FieldOffsets.Value;
                        return new ValueRecord(offset, m_bufTable, m_offsetSinglePos, ValueFormat);
                    }
                }

                protected uint m_offsetSinglePos;
                protected MBOBuffer m_bufTable;
            }

            public class SinglePosFormat2
            {
                public SinglePosFormat2(uint offset, MBOBuffer bufTable)
                {
                    m_offsetSinglePos = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    PosFormat        = 0,
                    Coverage         = 2,
                    ValueFormat      = 4,
                    ValueCount       = 6,
                    ValueRecordArray = 8
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.ValueRecordArray + ValueCount*ValueRecord.SizeOfValueRecord(ValueFormat);
                }

                public ushort PosFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetSinglePos + (uint)FieldOffsets.PosFormat);}
                }

                public ushort Coverage
                {
                    get {return m_bufTable.GetUshort(m_offsetSinglePos + (uint)FieldOffsets.Coverage);}
                }

                public ushort ValueFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetSinglePos + (uint)FieldOffsets.ValueFormat);}
                }

                public ushort ValueCount
                {
                    get
                    {
                        return m_bufTable.GetUshort(m_offsetSinglePos + (uint)FieldOffsets.ValueCount);
                    }
                }

                public ValueRecord GetValue(uint i)
                {
                    ValueRecord vr = null;
                    if (i < ValueCount)
                    {
                        uint sizeofValueRecord = ValueRecord.SizeOfValueRecord(ValueFormat);
                        uint offset = m_offsetSinglePos + (uint)FieldOffsets.ValueRecordArray + i*sizeofValueRecord;
                        vr = new ValueRecord(offset, m_bufTable, m_offsetSinglePos, ValueFormat);
                    }
                    return vr;
                }

                OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetSinglePos + Coverage, m_bufTable);
                }

                protected uint m_offsetSinglePos;
                protected MBOBuffer m_bufTable;
            }

            // accessors

            public ushort PosFormat
            {
                get {return m_bufTable.GetUshort(m_offsetSinglePos + (uint)FieldOffsets.PosFormat);}
            }

            public SinglePosFormat1 GetSinglePosFormat1()
            {
                if (PosFormat != 1)
                {
                    throw new System.InvalidOperationException("this object is not format 1");
                }

                return new SinglePosFormat1(m_offsetSinglePos, m_bufTable);
            }

            public SinglePosFormat2 GetSinglePosFormat2()
            {
                if (PosFormat != 2)
                {
                    throw new System.InvalidOperationException("this object is not format 2");
                }

                return new SinglePosFormat2(m_offsetSinglePos, m_bufTable);
            }


            protected uint m_offsetSinglePos;
        }


        // Lookup Type 2: Pair Adjustment Positioning Subtable
        
        public class PairPos : OTL.SubTable
        {
            public PairPos(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetPairPos = offset;
            }

            public enum FieldOffsets
            {
                PosFormat    = 0,
                Coverage     = 2,
                ValueFormat1 = 4,
                ValueFormat2 = 6,
            }


            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                return 2;
            }



            // nested classes

            public class PairPosFormat1
            {
                public PairPosFormat1(uint offset, MBOBuffer bufTable)
                {
                    m_offsetPairPos = offset;
                    m_bufTable = bufTable;
                }


                public enum FieldOffsets
                {
                    PosFormat      = 0,
                    Coverage       = 2,
                    ValueFormat1   = 4,
                    ValueFormat2   = 6,
                    PairSetCount   = 8,
                    PairSetOffsets = 10
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.PairSetOffsets + (uint)PairSetCount*2;
                }


                public class PairSetTable
                {
                    public PairSetTable(uint offset, MBOBuffer bufTable, uint offsetPosTable, ushort ValueFormat1, ushort ValueFormat2)
                    {
                        m_offsetPairSetTable = offset;
                        m_bufTable = bufTable;
                        m_offsetPosTable = offsetPosTable;
                        m_ValueFormat1 = ValueFormat1;
                        m_ValueFormat2 = ValueFormat2;
                    }

                    public enum FieldOffsets
                    {
                        PairValueCount       = 0,
                        PairValueRecordArray = 2
                    }
                
                    public uint CalcLength()
                    {
                        uint sizeofPairValueRecord = 2 + ValueRecord.SizeOfValueRecord(m_ValueFormat1) + ValueRecord.SizeOfValueRecord(m_ValueFormat2);
                        return (uint)FieldOffsets.PairValueRecordArray + PairValueCount*sizeofPairValueRecord;
                    }

                    public ushort PairValueCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetPairSetTable);}
                    }

                    public PairValueRecord GetPairValueRecord(uint i)
                    {
                        PairValueRecord pvr = null;

                        if (i < PairValueCount)
                        {
                            uint sizeofPairValueRecord = 2 + ValueRecord.SizeOfValueRecord(m_ValueFormat1) + ValueRecord.SizeOfValueRecord(m_ValueFormat2);
                            uint offset = m_offsetPairSetTable + (uint)FieldOffsets.PairValueRecordArray + i*sizeofPairValueRecord;
                            pvr = new PairValueRecord(offset, m_bufTable, m_offsetPairSetTable, m_ValueFormat1, m_ValueFormat2);
                        }

                        return pvr;
                    }

                    protected uint m_offsetPairSetTable;
                    protected MBOBuffer m_bufTable;
                    protected uint m_offsetPosTable;
                    protected ushort m_ValueFormat1;
                    protected ushort m_ValueFormat2;
                }

                public class PairValueRecord
                {
                    public PairValueRecord(uint offset, MBOBuffer bufTable, uint offsetPairSetTable, ushort ValueFormat1, ushort ValueFormat2)
                    {
                        m_offsetPairValueRecord = offset;
                        m_bufTable = bufTable;
                        m_offsetPairSetTable = offsetPairSetTable;
                        m_ValueFormat1 = ValueFormat1;
                        m_ValueFormat2 = ValueFormat2;
                    }

                    public enum FieldOffsets
                    {
                        SecondGlyph = 0,
                        Value1      = 2,
                    }

                    // accessors

                    public ushort SecondGlyph
                    {
                        get {return m_bufTable.GetUshort(m_offsetPairValueRecord + (uint)FieldOffsets.SecondGlyph);}
                    }

                    public ValueRecord Value1
                    {
                        get
                        {
                            uint offset = m_offsetPairValueRecord + (uint)FieldOffsets.Value1;
                            return new ValueRecord(offset, m_bufTable, m_offsetPairSetTable, m_ValueFormat1);
                        }
                    }

                    public ValueRecord Value2
                    {
                        get
                        {
                            uint offset = m_offsetPairValueRecord + (uint)FieldOffsets.Value1 + ValueRecord.SizeOfValueRecord(m_ValueFormat1);
                            return new ValueRecord(offset, m_bufTable, m_offsetPairSetTable, m_ValueFormat2);
                        }
                    }


                    protected uint m_offsetPairValueRecord;
                    protected MBOBuffer m_bufTable;
                    protected uint m_offsetPairSetTable;
                    protected ushort m_ValueFormat1;
                    protected ushort m_ValueFormat2;
                }


                // accessors

                public ushort PosFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.PosFormat);}
                }

                public ushort Coverage
                {
                    get {return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.Coverage);}
                }

                public ushort ValueFormat1
                {
                    get {return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.ValueFormat1);}
                }

                public ushort ValueFormat2
                {
                    get {return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.ValueFormat2);}
                }

                public ushort PairSetCount
                {
                    get
                    {
                        if (PosFormat != 1)
                        {
                            throw new System.InvalidOperationException();
                        }
                        return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.PairSetCount);
                    }
                }

                public ushort GetPairSetOffset(uint i)
                {
                    if (PosFormat != 1)
                    {
                        throw new System.InvalidOperationException();
                    }
                    uint offset = m_offsetPairPos + (uint)FieldOffsets.PairSetOffsets + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public PairSetTable GetPairSetTable(uint i)
                {
                    if (PosFormat != 1)
                    {
                        throw new System.InvalidOperationException();
                    }

                    PairSetTable pst = null;

                    if (i < PairSetCount)
                    {
                        uint offset = m_offsetPairPos + GetPairSetOffset(i);
                        return new PairSetTable(offset, m_bufTable, m_offsetPairPos, ValueFormat1, ValueFormat2);
                    }

                    return pst;
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetPairPos + Coverage, m_bufTable);
                }

                protected uint m_offsetPairPos;
                protected MBOBuffer m_bufTable;
            }

            public class PairPosFormat2
            {
                public PairPosFormat2(uint offset, MBOBuffer bufTable)
                {
                    m_offsetPairPos = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    PosFormat     = 0,
                    Coverage      = 2,
                    ValueFormat1  = 4,
                    ValueFormat2  = 6,
                    ClassDef1     = 8,
                    ClassDef2     = 10,
                    Class1Count   = 12,
                    Class2Count   = 14,
                    Class1Records = 16
                }

                public uint CalcLength()
                {
                    uint sizeofClass2 = ValueRecord.SizeOfValueRecord(ValueFormat1) + ValueRecord.SizeOfValueRecord(ValueFormat2);
                    return (uint)FieldOffsets.Class1Records + (uint)Class1Count*Class2Count*sizeofClass2;
                }


                // nested classes

                public class Class1Record
                {
                    public Class1Record(uint offset, MBOBuffer bufTable, ushort Class2Count, uint offsetPosTable, ushort ValueFormat1, ushort ValueFormat2)
                    {
                        m_offsetClass1Record = offset;
                        m_bufTable = bufTable;
                        m_Class2Count = Class2Count;
                        m_offsetPosTable = offsetPosTable;
                        m_ValueFormat1 = ValueFormat1;
                        m_ValueFormat2 = ValueFormat2;
                    }

                    public Class2Record GetClass2Record(uint i)
                    {
                        Class2Record c2r = null;

                        if (i < m_Class2Count)
                        {
                            ushort sizeofClass2Record = (ushort)(ValueRecord.SizeOfValueRecord(m_ValueFormat1) + ValueRecord.SizeOfValueRecord(m_ValueFormat2));
                            uint offset = m_offsetClass1Record + i*sizeofClass2Record;
                            c2r = new Class2Record(offset, m_bufTable, m_offsetPosTable, m_ValueFormat1, m_ValueFormat2);
                        }

                        return c2r;
                    }

                    protected uint m_offsetClass1Record;
                    protected MBOBuffer m_bufTable;
                    protected ushort m_Class2Count;
                    protected uint m_offsetPosTable;
                    protected ushort m_ValueFormat1;
                    protected ushort m_ValueFormat2;
                }

                public class Class2Record
                {
                    public Class2Record(uint offset, MBOBuffer bufTable, uint offsetPosTable, ushort ValueFormat1, ushort ValueFormat2)
                    {
                        m_offsetClass2Record = offset;
                        m_bufTable = bufTable;
                        m_offsetPosTable = offsetPosTable;
                        m_ValueFormat1 = ValueFormat1;
                        m_ValueFormat2 = ValueFormat2;
                        m_offsetValue1 = 0;
                        m_offsetValue2 = (ushort)ValueRecord.SizeOfValueRecord(ValueFormat1);
                    }

                    // accessors

                    public ValueRecord Value1
                    {
                        get
                        {
                            uint offset = m_offsetClass2Record + (uint)m_offsetValue1;
                            return new ValueRecord(offset, m_bufTable, m_offsetPosTable, m_ValueFormat1);
                        }
                    }

                    public ValueRecord Value2
                    {
                        get
                        {
                            uint offset = m_offsetClass2Record + (uint)m_offsetValue2;
                            return new ValueRecord(offset, m_bufTable, m_offsetPosTable, m_ValueFormat2);
                        }
                    }


                    protected uint m_offsetClass2Record;
                    protected MBOBuffer m_bufTable;
                    protected uint m_offsetPosTable;
                    protected ushort m_ValueFormat1;
                    protected ushort m_ValueFormat2;
                    protected uint m_offsetValue1;
                    protected uint m_offsetValue2;
                }

                // accessors

                public ushort PosFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.PosFormat);}
                }

                public ushort Coverage
                {
                    get {return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.Coverage);}
                }

                public ushort ValueFormat1
                {
                    get {return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.ValueFormat1);}
                }

                public ushort ValueFormat2
                {
                    get {return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.ValueFormat2);}
                }

                public ushort ClassDef1Offset
                {
                    get
                    {
                        if (PosFormat != 2)
                        {
                            throw new System.InvalidOperationException();
                        }
                        return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.ClassDef1);
                    }
                }

                public ushort ClassDef2Offset
                {
                    get
                    {
                        if (PosFormat != 2)
                        {
                            throw new System.InvalidOperationException();
                        }
                        return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.ClassDef2);
                    }
                }

                public ushort Class1Count
                {
                    get
                    {
                        if (PosFormat != 2)
                        {
                            throw new System.InvalidOperationException();
                        }
                        return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.Class1Count);
                    }
                }

                public ushort Class2Count
                {
                    get
                    {
                        if (PosFormat != 2)
                        {
                            throw new System.InvalidOperationException();
                        }
                        return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.Class2Count);
                    }
                }

                public Class1Record GetClass1Record(uint i)
                {
                    if (PosFormat != 2)
                    {
                        throw new System.InvalidOperationException();
                    }

                    Class1Record c1r = null;

                    if (i < Class1Count)
                    {
                        ushort sizeofClass1Record = (ushort)( Class2Count * ValueRecord.SizeOfValueRecord(ValueFormat2));
                        uint offset = m_offsetPairPos + (uint)FieldOffsets.Class1Records + i*sizeofClass1Record;
                        c1r = new Class1Record(offset, m_bufTable, Class2Count, m_offsetPairPos, ValueFormat1, ValueFormat2);
                    }

                    return c1r;
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetPairPos + Coverage, m_bufTable);
                }

                public OTL.ClassDefTable GetClassDef1Table()
                {
                    return new OTL.ClassDefTable(m_offsetPairPos + ClassDef1Offset, m_bufTable);
                }

                public OTL.ClassDefTable GetClassDef2Table()
                {
                    return new OTL.ClassDefTable(m_offsetPairPos + ClassDef2Offset, m_bufTable);
                }

                protected uint m_offsetPairPos;
                protected MBOBuffer m_bufTable;
            }

            
            
            // accessors

            public ushort PosFormat
            {
                get {return m_bufTable.GetUshort(m_offsetPairPos + (uint)FieldOffsets.PosFormat);}
            }

            public PairPosFormat1 GetPairPosFormat1()
            {
                if (PosFormat != 1)
                {
                    throw new InvalidOperationException("this object is not format 1");
                }

                return new PairPosFormat1(m_offsetPairPos, m_bufTable);
            }

            public PairPosFormat2 GetPairPosFormat2()
            {
                if (PosFormat != 2)
                {
                    throw new InvalidOperationException("this object is not format 2");
                }

                return new PairPosFormat2(m_offsetPairPos, m_bufTable);
            }

            protected uint m_offsetPairPos;
        }


        // Lookup Type 3: Cursive Attachment Positioning Subtable
        
        public class CursivePos : OTL.SubTable
        {
            public CursivePos(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetCursivePos = offset;
            }

            public enum FieldOffsets
            {
                PosFormat            = 0,
                Coverage             = 2,
                EntryExitCount       = 4,
                EntryExitRecordArray = 6
            }

            
            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                return 2;
            }



            // public methods

            public uint CalcLength()
            {
                return (uint)FieldOffsets.EntryExitRecordArray + (uint)EntryExitCount*4;
            }

            // nested classes

            public class EntryExitRecord
            {
                public EntryExitRecord(uint offset, MBOBuffer bufTable, uint offsetCursivePos)
                {
                    m_offsetEntryExitRecord = offset;
                    m_bufTable = bufTable;
                    m_offsetCursivePos = offsetCursivePos;
                }

                public enum FieldOffsets
                {
                    EntryAnchorOffset = 0,
                    ExitAnchorOffset = 2
                }


                public ushort EntryAnchorOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetEntryExitRecord + (uint)FieldOffsets.EntryAnchorOffset);}
                }

                public ushort ExitAnchorOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetEntryExitRecord + (uint)FieldOffsets.ExitAnchorOffset);}
                }

                public AnchorTable GetEntryAnchorTable()
                {
                    AnchorTable at = null;

                    if (EntryAnchorOffset != 0)
                    {
                        uint offset = m_offsetCursivePos + EntryAnchorOffset;
                        at = new AnchorTable(offset, m_bufTable);
                    }

                    return at;
                }

                public AnchorTable GetExitAnchorTable()
                {
                    AnchorTable at = null;

                    if (ExitAnchorOffset != 0)
                    {
                        uint offset = m_offsetCursivePos + ExitAnchorOffset;
                        at = new AnchorTable(offset, m_bufTable);
                    }

                    return at;
                }

                protected uint m_offsetEntryExitRecord;
                protected MBOBuffer m_bufTable;
                protected uint m_offsetCursivePos;
            }
            
            
            // accessors

            public ushort PosFormat
            {
                get {return m_bufTable.GetUshort(m_offsetCursivePos + (uint)FieldOffsets.PosFormat);}
            }

            public ushort Coverage
            {
                get {return m_bufTable.GetUshort(m_offsetCursivePos + (uint)FieldOffsets.Coverage);}
            }

            public ushort EntryExitCount
            {
                get {return m_bufTable.GetUshort(m_offsetCursivePos + (uint)FieldOffsets.EntryExitCount);}
            }

            public EntryExitRecord GetEntryExitRecord(uint i)
            {
                EntryExitRecord eer = null;

                if (i < EntryExitCount)
                {
                    ushort sizeofEntryExitRecord = 4;
                    uint offset = m_offsetCursivePos + (uint)FieldOffsets.EntryExitRecordArray + i*sizeofEntryExitRecord;
                    eer = new EntryExitRecord(offset, m_bufTable, m_offsetCursivePos);
                }

                return eer;
            }

            public OTL.CoverageTable GetCoverageTable()
            {
                return new OTL.CoverageTable(m_offsetCursivePos + Coverage, m_bufTable);
            }


            protected uint m_offsetCursivePos;
        }

        
        // Lookup Type 4: MarkToBase Attachment Positioning Subtable
        
        public class MarkBasePos : OTL.SubTable
        {
            public MarkBasePos(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetMarkBasePos = offset;
            }

            public enum FieldOffsets
            {
                PosFormat          = 0,
                MarkCoverageOffset = 2,
                BaseCoverageOffset = 4,
                ClassCount         = 6,
                MarkArrayOffset    = 8,
                BaseArrayOffset    = 10
            }

            // public methods

            public uint CalcLength()
            {
                return 12;
            }


            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                return 1;
            }



            // nested classes

            public class BaseArrayTable
            {
                public BaseArrayTable(uint offset, MBOBuffer bufTable, ushort ClassCount)
                {
                    m_offsetBaseArrayTable = offset;
                    m_bufTable = bufTable;
                    m_ClassCount = ClassCount;
                }

                public enum FieldOffsets
                {
                    BaseCount       = 0,
                    BaseRecordArray = 2
                }

                public uint CalcLength()
                {
                    ushort sizeofBaseRecord = (ushort)(2 * m_ClassCount);
                    return (uint)FieldOffsets.BaseRecordArray + (uint)BaseCount*sizeofBaseRecord;
                }

                public ushort BaseCount
                {
                    get {return m_bufTable.GetUshort(m_offsetBaseArrayTable + (uint)FieldOffsets.BaseCount);}
                }

                public BaseRecord GetBaseRecord(uint i)
                {
                    BaseRecord br = null;

                    if (i < BaseCount)
                    {
                        ushort sizeofBaseRecord = (ushort)(2 * m_ClassCount);
                        uint offset = m_offsetBaseArrayTable + (uint)FieldOffsets.BaseRecordArray + i*sizeofBaseRecord;
                        br = new BaseRecord(offset, m_bufTable, m_offsetBaseArrayTable, m_ClassCount);
                    }

                    return br;
                }


                protected uint m_offsetBaseArrayTable;
                protected MBOBuffer m_bufTable;
                protected ushort m_ClassCount;
            }

            public class BaseRecord
            {
                public BaseRecord(uint offset, MBOBuffer bufTable, uint offsetBaseArrayTable, ushort ClassCount)
                {
                    m_offsetBaseRecord = offset;
                    m_bufTable = bufTable;
                    m_offsetBaseArrayTable = offsetBaseArrayTable;
                    m_ClassCount = ClassCount;
                }

                public ushort GetBaseAnchorOffset(uint i)
                {
                    return m_bufTable.GetUshort(m_offsetBaseRecord + i*2);
                }

                public AnchorTable GetBaseAnchorTable(uint i)
                {
                    return new AnchorTable(m_offsetBaseArrayTable + GetBaseAnchorOffset(i), m_bufTable);
                }

                protected uint m_offsetBaseRecord;
                protected MBOBuffer m_bufTable;
                protected uint m_offsetBaseArrayTable;
                protected ushort m_ClassCount;
            }

            // accessors

            public ushort PosFormat
            {
                get {return m_bufTable.GetUshort(m_offsetMarkBasePos + (uint)FieldOffsets.PosFormat);}
            }

            public ushort MarkCoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkBasePos + (uint)FieldOffsets.MarkCoverageOffset);}
            }
                
            public ushort BaseCoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkBasePos + (uint)FieldOffsets.BaseCoverageOffset);}
            }
                
            public ushort ClassCount
            {
                get {return m_bufTable.GetUshort(m_offsetMarkBasePos + (uint)FieldOffsets.ClassCount);}
            }
                
            public ushort MarkArrayOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkBasePos + (uint)FieldOffsets.MarkArrayOffset);}
            }
                
            public ushort BaseArrayOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkBasePos + (uint)FieldOffsets.BaseArrayOffset);}
            }

            public OTL.CoverageTable GetMarkCoverageTable()
            {
                uint offset = m_offsetMarkBasePos + MarkCoverageOffset;
                return new OTL.CoverageTable(offset, m_bufTable);
            }

            public OTL.CoverageTable GetBaseCoverageTable()
            {
                uint offset = m_offsetMarkBasePos + BaseCoverageOffset;
                return new OTL.CoverageTable(offset, m_bufTable);
            }

            public MarkArray GetMarkArrayTable()
            {
                uint offset = m_offsetMarkBasePos + MarkArrayOffset;
                return new MarkArray(offset, m_bufTable);
            }

            public BaseArrayTable GetBaseArrayTable()
            {
                uint offset = m_offsetMarkBasePos + BaseArrayOffset;
                return new BaseArrayTable(offset, m_bufTable, ClassCount);
            }
                
            protected uint m_offsetMarkBasePos;
        }


        // Lookup Type 5: MarkToLigature Attachment Positioning Subtable
        
        public class MarkLigPos : OTL.SubTable
        {
            public MarkLigPos(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetMarkLigPos = offset;
            }

            public enum FieldOffsets
            {
                PosFormat              = 0,
                MarkCoverageOffset     = 2,
                LigatureCoverageOffset = 4,
                ClassCount             = 6,
                MarkArrayOffset        = 8,
                LigatureArrayOffset    = 10
            }

            // public methods

            public uint CalcLength()
            {
                return 12;
            }



            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                uint nLength = 0;

                // walk the ligature attach tables
                LigatureArray LigArr = GetLigatureArrayTable();
                for (uint i=0; i<LigArr.LigatureCount; i++)
                {
                    LigatureAttach la = LigArr.GetLigatureAttachTable(i);
                    
                    if (la.ComponentCount > nLength)
                    {
                        nLength = la.ComponentCount;
                    }
                }

                return nLength;
            }



            // nested classes

            public class LigatureArray
            {
                public LigatureArray(uint offset, MBOBuffer bufTable, ushort ClassCount)
                {
                    m_offsetLigatureArray = offset;
                    m_bufTable = bufTable;
                    m_ClassCount = ClassCount;
                }

                public enum FieldOffsets
                {
                    LigatureCount = 0,
                    LigatureAttachOffsets = 2
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.LigatureAttachOffsets + (uint)LigatureCount*2;
                }

                public ushort LigatureCount
                {
                    get{return m_bufTable.GetUshort(m_offsetLigatureArray + (uint)FieldOffsets.LigatureCount);}
                }

                public ushort GetLigatureAttachOffset(uint i)
                {
                    uint offset = m_offsetLigatureArray + (uint)FieldOffsets.LigatureAttachOffsets + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public LigatureAttach GetLigatureAttachTable(uint i)
                {
                    LigatureAttach la = null;

                    if (i < LigatureCount)
                    {
                        uint offset = m_offsetLigatureArray + GetLigatureAttachOffset(i);
                        la = new LigatureAttach(offset, m_bufTable, m_ClassCount);
                    }

                    return la;
                }

                protected uint m_offsetLigatureArray;
                protected MBOBuffer m_bufTable;
                protected ushort m_ClassCount;
            }

            public class LigatureAttach
            {
                public LigatureAttach(uint offset, MBOBuffer bufTable, ushort ClassCount)
                {
                    m_offsetLigatureAttach = offset;
                    m_bufTable = bufTable;
                    m_ClassCount = ClassCount;
                }

                public enum FieldOffsets
                {
                    ComponentCount = 0,
                    ComponentRecordArray = 2
                }

                public uint CalcLength()
                {
                    ushort sizeofComponentRecord = (ushort)(2 * m_ClassCount);
                    return (uint)FieldOffsets.ComponentRecordArray + (uint)ComponentCount*sizeofComponentRecord;
                }

                public ushort ComponentCount
                {
                    get{return m_bufTable.GetUshort(m_offsetLigatureAttach + (uint)FieldOffsets.ComponentCount);}
                }

                public ComponentRecord GetComponentRecord(uint i)
                {
                    ComponentRecord cr = null;

                    if (i < ComponentCount)
                    {
                        ushort sizeofComponentRecord = (ushort)(2 * m_ClassCount);
                        uint offset = m_offsetLigatureAttach + (uint)FieldOffsets.ComponentRecordArray + i*sizeofComponentRecord;
                        cr = new ComponentRecord(offset, m_bufTable, m_offsetLigatureAttach, m_ClassCount);
                    }

                    return cr;
                }

                protected uint m_offsetLigatureAttach;
                protected MBOBuffer m_bufTable;
                protected ushort m_ClassCount;
            }

            public class ComponentRecord
            {
                public ComponentRecord(uint offset, MBOBuffer bufTable, uint offsetLigatureAttach, ushort ClassCount)
                {
                    m_offsetComponentRecord = offset;
                    m_bufTable = bufTable;
                    m_offsetLigatureAttach = offsetLigatureAttach;
                    m_ClassCount = ClassCount;
                }

                public ushort GetLigatureAnchorOffset(uint i)
                {
                    return m_bufTable.GetUshort(m_offsetComponentRecord + i*2);
                }

                public AnchorTable GetLigatureAnchorTable(uint i)
                {
                    AnchorTable at = null;
                    uint offset = GetLigatureAnchorOffset(i);
                    if (offset != 0)
                    {
                        at = new AnchorTable(m_offsetLigatureAttach + offset, m_bufTable);
                    }
                    return at;
                }


                protected uint m_offsetComponentRecord;
                protected MBOBuffer m_bufTable;
                protected uint m_offsetLigatureAttach;
                protected ushort m_ClassCount;
            }

            // accessors

            public ushort PosFormat
            {
                get {return m_bufTable.GetUshort(m_offsetMarkLigPos + (uint)FieldOffsets.PosFormat);}
            }

            public ushort MarkCoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkLigPos + (uint)FieldOffsets.MarkCoverageOffset);}
            }

            public ushort LigatureCoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkLigPos + (uint)FieldOffsets.LigatureCoverageOffset);}
            }

            public ushort ClassCount
            {
                get {return m_bufTable.GetUshort(m_offsetMarkLigPos + (uint)FieldOffsets.ClassCount);}
            }

            public ushort MarkArrayOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkLigPos + (uint)FieldOffsets.MarkArrayOffset);}
            }

            public ushort LigatureArrayOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkLigPos + (uint)FieldOffsets.LigatureArrayOffset);}
            }

            public OTL.CoverageTable GetMarkCoverageTable()
            {
                uint offset = m_offsetMarkLigPos + MarkCoverageOffset;
                return new OTL.CoverageTable(offset, m_bufTable);
            }

            public OTL.CoverageTable GetLigatureCoverageTable()
            {
                uint offset = m_offsetMarkLigPos + LigatureCoverageOffset;
                return new OTL.CoverageTable(offset, m_bufTable);
            }

            public MarkArray GetMarkArrayTable()
            {
                uint offset = m_offsetMarkLigPos + MarkArrayOffset;
                return new MarkArray(offset, m_bufTable);
            }

            public LigatureArray GetLigatureArrayTable()
            {
                uint offset = m_offsetMarkLigPos + LigatureArrayOffset;
                return new LigatureArray(offset, m_bufTable, ClassCount);
            }

            protected uint m_offsetMarkLigPos;
        }


        // Lookup Type 6: MarkToMark Attachment Positioning Subtable
        
        public class MarkMarkPos : OTL.SubTable
        {
            public MarkMarkPos(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetMarkMarkPos = offset;
            }

            public enum FieldOffsets
            {
                PosFormat           = 0,
                Mark1CoverageOffset = 2,
                Mark2CoverageOffset = 4,
                ClassCount          = 6,
                Mark1ArrayOffset    = 8,
                Mark2ArrayOffset    = 10
            }

            // public methods

            public uint CalcLength()
            {
                return 12;
            }

            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                return 2;
            }



            // nested classes

            public class Mark2Array
            {
                public Mark2Array(uint offset, MBOBuffer bufTable, ushort ClassCount)
                {
                    m_offsetMark2Array = offset;
                    m_bufTable = bufTable;
                    m_ClassCount = ClassCount;
                }

                public enum FieldOffsets
                {
                    Mark2Count       = 0,
                    Mark2RecordArray = 2
                }

                public uint CalcLength()
                {
                    ushort sizeofMark2Record = (ushort)(2 * m_ClassCount);
                    return (uint)FieldOffsets.Mark2RecordArray + (uint)Mark2Count*sizeofMark2Record;
                }

                public ushort Mark2Count
                {
                    get {return m_bufTable.GetUshort(m_offsetMark2Array + (uint)FieldOffsets.Mark2Count);}
                }

                public Mark2Record GetMark2Record(uint i)
                {
                    Mark2Record m2r = null;

                    if (i < Mark2Count)
                    {
                        ushort sizeofMark2Record = (ushort)(2 * m_ClassCount);
                        uint offset = m_offsetMark2Array + (uint)FieldOffsets.Mark2RecordArray + i*sizeofMark2Record;
                        m2r = new Mark2Record(offset, m_bufTable, m_offsetMark2Array, m_ClassCount);
                    }

                    return m2r;
                }

                protected uint m_offsetMark2Array;
                protected MBOBuffer m_bufTable;
                protected ushort m_ClassCount;
            }

            public class Mark2Record
            {
                public Mark2Record(uint offset, MBOBuffer bufTable, uint offsetMark2Array, ushort ClassCount)
                {
                    m_offsetMark2Record = offset;
                    m_bufTable = bufTable;
                    m_offsetMark2Array = offsetMark2Array;
                    m_ClassCount = ClassCount;
                }

                public ushort GetMark2AnchorOffset(uint i)
                {
                    return m_bufTable.GetUshort(m_offsetMark2Record + i*2);
                }

                public AnchorTable GetMark2AnchorTable(uint i)
                {
                    return new AnchorTable(m_offsetMark2Array + GetMark2AnchorOffset(i), m_bufTable);
                }

                protected uint m_offsetMark2Record;
                protected MBOBuffer m_bufTable;
                protected uint m_offsetMark2Array;
                protected ushort m_ClassCount;
            }


            // accessors

            public ushort PosFormat
            {
                get {return m_bufTable.GetUshort(m_offsetMarkMarkPos + (uint)FieldOffsets.PosFormat);}
            }

            public ushort Mark1CoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkMarkPos + (uint)FieldOffsets.Mark1CoverageOffset);}
            }

            public ushort Mark2CoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkMarkPos + (uint)FieldOffsets.Mark2CoverageOffset);}
            }

            public ushort ClassCount
            {
                get {return m_bufTable.GetUshort(m_offsetMarkMarkPos + (uint)FieldOffsets.ClassCount);}
            }

            public ushort Mark1ArrayOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkMarkPos + (uint)FieldOffsets.Mark1ArrayOffset);}
            }

            public ushort Mark2ArrayOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMarkMarkPos + (uint)FieldOffsets.Mark2ArrayOffset);}
            }


            public OTL.CoverageTable GetMark1CoverageTable()
            {
                return new OTL.CoverageTable(m_offsetMarkMarkPos + Mark1CoverageOffset, m_bufTable);
            }

            public OTL.CoverageTable GetMark2CoverageTable()
            {
                return new OTL.CoverageTable(m_offsetMarkMarkPos + Mark2CoverageOffset, m_bufTable);
            }

            public MarkArray GetMark1ArrayTable()
            {
                return new MarkArray(m_offsetMarkMarkPos + Mark1ArrayOffset, m_bufTable);
            }

            public Mark2Array GetMark2ArrayTable()
            {
                return new Mark2Array(m_offsetMarkMarkPos + Mark2ArrayOffset, m_bufTable, ClassCount);
            }

            protected uint m_offsetMarkMarkPos;
        }


        // Lookup Type 7: Contextual Positioning Subtable
        
        public class ContextPos : OTL.SubTable
        {
            public ContextPos(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetContextPos = offset;
            }

            public enum FieldOffsets
            {
                PosFormat = 0
            }

            
            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                uint nLength = 0;

                if (PosFormat == 1)
                {
                    ContextPosFormat1 cpf1 = GetContextPosFormat1();
                    for (uint iPosRuleSet=0; iPosRuleSet<cpf1.PosRuleSetCount; iPosRuleSet++)
                    {
                        ContextPosFormat1.PosRuleSet prs = cpf1.GetPosRuleSet(iPosRuleSet);
                        for (uint iPosRule = 0; iPosRule < prs.PosRuleCount; iPosRule++)
                        {
                            ContextPosFormat1.PosRule pr = prs.GetPosRuleTable(iPosRule);
                            if (pr.GlyphCount > nLength)
                            {
                                nLength = pr.GlyphCount;
                            }
                        }
                    }
                }
                else if (PosFormat == 2)
                {
                    ContextPosFormat2 cpf2 = GetContextPosFormat2();
                    for (uint iPosClassSet = 0; iPosClassSet < cpf2.PosClassSetCount; iPosClassSet++)
                    {
                        ContextPosFormat2.PosClassSet pcs = cpf2.GetPosClassSetTable(iPosClassSet);
                        for (uint iPosClassRule = 0; iPosClassRule < pcs.PosClassRuleCount; iPosClassRule++)
                        {
                            ContextPosFormat2.PosClassRule pcr = pcs.GetPosClassRuleTable(iPosClassRule);
                            if (pcr.GlyphCount > nLength)
                            {
                                nLength = pcr.GlyphCount;
                            }
                        }
                    }
                }
                else if (PosFormat == 3)
                {
                    ContextPosFormat3 cpf3 = GetContextPosFormat3();
                    if (cpf3.GlyphCount > nLength)
                    {
                        nLength = cpf3.GlyphCount;
                    }
                }

                return nLength;
            }



            // nested classes

            public class ContextPosFormat1
            {
                public ContextPosFormat1(uint offset, MBOBuffer bufTable)
                {
                    m_offsetContextPos = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    PosFormat         = 0,
                    CoverageOffset    = 2,
                    PosRuleSetCount   = 4,
                    PosRuleSetOffsets = 6
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.PosRuleSetOffsets + (uint)PosRuleSetCount*2;
                }

                // nested classes

                public class PosRuleSet
                {
                    public PosRuleSet(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetPosRuleSet = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        PosRuleCount   = 0,
                        PosRuleOffsets = 2
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.PosRuleOffsets + (uint)PosRuleCount*2;
                    }

                    public ushort PosRuleCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetPosRuleSet + (uint)FieldOffsets.PosRuleCount);}
                    }

                    public ushort GetPosRuleOffset(uint i)
                    {
                        uint offset = m_offsetPosRuleSet + (uint)FieldOffsets.PosRuleOffsets + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public PosRule GetPosRuleTable(uint i)
                    {
                        PosRule pr = null;

                        if (i < PosRuleCount)
                        {
                            uint offset = m_offsetPosRuleSet + GetPosRuleOffset(i);
                            pr = new PosRule(offset, m_bufTable);
                        }

                        return pr;
                    }

                    protected uint m_offsetPosRuleSet;
                    protected MBOBuffer m_bufTable;
                }

                public class PosRule
                {
                    public PosRule(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetPosRule = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        GlyphCount    = 0,
                        PosCount      = 2,
                        InputGlyphIDs = 4
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.InputGlyphIDs + ((uint)GlyphCount-1)*2 +  (uint)PosCount*4;
                    }

                    public ushort GlyphCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetPosRule + (uint)FieldOffsets.GlyphCount);}
                    }

                    public ushort PosCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetPosRule + (uint)FieldOffsets.PosCount);}
                    }

                    public ushort GetInputGlyphID(uint i)
                    {
                        uint offset = m_offsetPosRule + (uint)FieldOffsets.InputGlyphIDs + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public PosLookupRecord GetPosLookupRecord(uint i)
                    {
                        PosLookupRecord plr = null;

                        if (i < PosCount)
                        {
                            uint offset = m_offsetPosRule + (uint)FieldOffsets.InputGlyphIDs + ((uint)GlyphCount-1)*2 + i*4;
                            plr = new PosLookupRecord(offset, m_bufTable);
                        }

                        return plr;
                    }

                    protected uint m_offsetPosRule;
                    protected MBOBuffer m_bufTable;
                }


                // accessors

                public ushort PosFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.PosFormat);}
                }

                public ushort CoverageOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.CoverageOffset);}
                }

                public ushort PosRuleSetCount
                {
                    get {return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.PosRuleSetCount);}
                }

                public ushort GetPosRuleSetOffset(uint i)
                {
                    return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.PosRuleSetOffsets + i*2);
                }

                public PosRuleSet GetPosRuleSet(uint i)
                {

                    PosRuleSet prs = null;

                    if (i < PosRuleSetCount)
                    {
                        uint offset = m_offsetContextPos + GetPosRuleSetOffset(i);
                        prs = new PosRuleSet(offset, m_bufTable);
                    }

                    return prs;
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetContextPos + CoverageOffset, m_bufTable);
                }

                protected uint m_offsetContextPos;
                protected MBOBuffer m_bufTable;
            }

            public class ContextPosFormat2
            {
                public ContextPosFormat2(uint offset, MBOBuffer bufTable)
                {
                    m_offsetContextPos = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    PosFormat          = 0,
                    CoverageOffset     = 2,
                    ClassDefOffset     = 4,
                    PosClassSetCount   = 6,
                    PosClassSetOffsets = 8
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.PosClassSetOffsets + (uint)PosClassSetCount*2;
                }

                // nested classes

                public class PosClassSet
                {
                    public PosClassSet(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetPosClassSet = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        PosClassRuleCount   = 0,
                        PosClassRuleOffsets = 2,
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.PosClassRuleOffsets + (uint)PosClassRuleCount*2;
                    }

                    public ushort PosClassRuleCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetPosClassSet + (uint)FieldOffsets.PosClassRuleCount);}
                    }

                    public ushort GetPosClassRuleOffset(uint i)
                    {
                        uint offset = m_offsetPosClassSet + (uint)FieldOffsets.PosClassRuleOffsets + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public PosClassRule GetPosClassRuleTable(uint i)
                    {
                        PosClassRule pcr = null;

                        if (i < PosClassRuleCount)
                        {
                            uint offset = m_offsetPosClassSet + GetPosClassRuleOffset(i);
                            pcr = new PosClassRule(offset, m_bufTable);
                        }

                        return pcr;
                    }

                    protected uint m_offsetPosClassSet;
                    protected MBOBuffer m_bufTable;
                }

                public class PosClassRule
                {
                    public PosClassRule(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetPosClassRule = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        GlyphCount = 0,
                        PosCount   = 2,
                        Classes    = 4
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.Classes + ((uint)GlyphCount-1)*2 + (uint)PosCount*4;
                    }

                    public ushort GlyphCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetPosClassRule + (uint)FieldOffsets.GlyphCount);}
                    }

                    public ushort PosCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetPosClassRule + (uint)FieldOffsets.PosCount);}
                    }

                    public ushort GetClass(uint i)
                    {
                        return m_bufTable.GetUshort(m_offsetPosClassRule + (uint)FieldOffsets.Classes + i*2);
                    }

                    public PosLookupRecord GetPosLookupRecord (uint i)
                    {
                        uint offset = m_offsetPosClassRule + (uint)FieldOffsets.Classes + (uint)(GlyphCount-1)*2 + i*4;
                        return new PosLookupRecord(offset, m_bufTable);
                    }

                    protected uint m_offsetPosClassRule;
                    protected MBOBuffer m_bufTable;
                }


                // accessors

                public ushort PosFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.PosFormat);}
                }

                public ushort CoverageOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.CoverageOffset);}
                }

                public ushort ClassDefOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.ClassDefOffset);}
                }

                public OTL.ClassDefTable GetClassDefTable()
                {
                    return new OTL.ClassDefTable(m_offsetContextPos + ClassDefOffset, m_bufTable);
                }

                public ushort PosClassSetCount
                {
                    get{return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.PosClassSetCount);}
                }

                public ushort GetPosClassSetOffset(uint i)
                {
                    return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.PosClassSetOffsets + i*2);
                }

                public PosClassSet GetPosClassSetTable(uint i)
                {

                    PosClassSet pcs = null;

                    if (i < PosClassSetCount)
                    {
                        pcs = new PosClassSet(m_offsetContextPos + GetPosClassSetOffset(i), m_bufTable);
                    }

                    return pcs;
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetContextPos + CoverageOffset, m_bufTable);
                }

                protected uint m_offsetContextPos;
                protected MBOBuffer m_bufTable;
            }

            public class ContextPosFormat3
            {
                public ContextPosFormat3(uint offset, MBOBuffer bufTable)
                {
                    m_offsetContextPos = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    PosFormat       = 0,
                    GlyphCount      = 2,
                    PosCount        = 4,
                    CoverageOffsets = 6
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.CoverageOffsets + (uint)GlyphCount*2 + (uint)PosCount*4;
                }

                // accessors

                public ushort PosFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.PosFormat);}
                }

                public ushort GlyphCount
                {
                    get {return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.GlyphCount);}
                }

                public ushort PosCount
                {
                    get {return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.PosCount);}
                }

                public ushort GetCoverageOffset(uint i)
                {
                    if (i >= GlyphCount)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.CoverageOffsets + i*2);
                }

                public PosLookupRecord GetPosLookupRecord(uint i)
                {
                    PosLookupRecord plr = null;

                    if (i < PosCount)
                    {
                        uint offset = m_offsetContextPos + (uint)FieldOffsets.CoverageOffsets + (uint)GlyphCount*2 + i*4;
                        plr = new PosLookupRecord(offset, m_bufTable);
                    }

                    return plr;
                }

                public OTL.CoverageTable GetCoverageTable(uint i)
                {
                    return new OTL.CoverageTable(m_offsetContextPos + GetCoverageOffset(i), m_bufTable);
                }


                protected uint m_offsetContextPos;
                protected MBOBuffer m_bufTable;
            }


            public class PosLookupRecord
            {
                public PosLookupRecord(uint offset, MBOBuffer bufTable)
                {
                    m_offsetPosLookupRecord = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SequenceIndex = 0,
                    LookupListIndex = 2
                }

                // accessors

                public ushort SequenceIndex
                {
                    get {return m_bufTable.GetUshort(m_offsetPosLookupRecord + (uint)FieldOffsets.SequenceIndex);}
                }

                public ushort LookupListIndex
                {
                    get {return m_bufTable.GetUshort(m_offsetPosLookupRecord + (uint)FieldOffsets.LookupListIndex);}
                }

                protected uint m_offsetPosLookupRecord;
                protected MBOBuffer m_bufTable;
            }


            // accessors

            public ushort PosFormat
            {
                get {return m_bufTable.GetUshort(m_offsetContextPos + (uint)FieldOffsets.PosFormat);}
            }

            public ContextPosFormat1 GetContextPosFormat1()
            {
                if (PosFormat != 1)
                {
                    throw new InvalidOperationException("format not 1");
                }

                return new ContextPosFormat1(m_offsetContextPos, m_bufTable);
            }

            public ContextPosFormat2 GetContextPosFormat2()
            {
                if (PosFormat != 2)
                {
                    throw new InvalidOperationException("format not 2");
                }

                return new ContextPosFormat2(m_offsetContextPos, m_bufTable);
            }

            public ContextPosFormat3 GetContextPosFormat3()
            {
                if (PosFormat != 3)
                {
                    throw new InvalidOperationException("format not 3");
                }

                return new ContextPosFormat3(m_offsetContextPos, m_bufTable);
            }



            protected uint m_offsetContextPos;
        }

        // Lookup Type 8: Chaining Contextual Positioning Subtable
        
        public class ChainContextPos : OTL.SubTable
        {
            public ChainContextPos(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetChainContextPos = offset;
            }

            public enum FieldOffsets
            {
                PosFormat = 0
            }


            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                uint nLength = 0;

                if (PosFormat == 1)
                {
                    ChainContextPosFormat1 ccpf1 = GetChainContextPosFormat1();
                    for (uint iChainPosRuleSet = 0; iChainPosRuleSet < ccpf1.ChainPosRuleSetCount; iChainPosRuleSet++)
                    {
                        ChainContextPosFormat1.ChainPosRuleSet cprs = ccpf1.GetChainPosRuleSetTable(iChainPosRuleSet);
                        for (uint iChainPosRule = 0; iChainPosRule < cprs.ChainPosRuleCount; iChainPosRule++)
                        {
                            ChainContextPosFormat1.ChainPosRule cpr = cprs.GetChainPosRuleTable(iChainPosRule);
                            uint tempLength = (uint)(cpr.InputGlyphCount + cpr.LookaheadGlyphCount);
                            if (tempLength > nLength)
                            {
                                nLength = tempLength;
                            }
                        }
                    }
                }
                else if (PosFormat == 2)
                {
                    ChainContextPosFormat2 ccpf2 = GetChainContextPosFormat2();
                    for (uint iChainPosClassSet = 0; iChainPosClassSet < ccpf2.ChainPosClassSetCount; iChainPosClassSet++)
                    {
                        ChainContextPosFormat2.ChainPosClassSet cpcs = ccpf2.GetChainPosClassSetTable(iChainPosClassSet);
                        for (uint iChainPosClassRule = 0; iChainPosClassRule < cpcs.ChainPosClassRuleCount; iChainPosClassRule++)
                        {
                            ChainContextPosFormat2.ChainPosClassRule cpcr = cpcs.GetChainPosClassRuleTable(iChainPosClassRule);
                            uint tempLength = (uint)(cpcr.InputGlyphCount + cpcr.LookaheadGlyphCount);
                            if (tempLength > nLength)
                            {
                                nLength = tempLength;
                            }
                        }
                    }
                }
                else if (PosFormat == 3)
                {
                    ChainContextPosFormat3 ccpf3 = GetChainContextPosFormat3();
                    uint tempLength = (uint)(ccpf3.InputGlyphCount + ccpf3.LookaheadGlyphCount);
                    if (tempLength > nLength)
                    {
                        nLength = tempLength;
                    }
                }

                return nLength;
            }



            // nested classes

            public class ChainContextPosFormat1
            {
                public ChainContextPosFormat1(uint offset, MBOBuffer bufTable)
                {
                    m_offsetChainContextPos = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    PosFormat              = 0,
                    CoverageOffset         = 2,
                    ChainPosRuleSetCount   = 4,
                    ChainPosRuleSetOffsets = 6
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.ChainPosRuleSetOffsets + (uint)ChainPosRuleSetCount*2;
                }


                // nested classes

                public class ChainPosRuleSet
                {
                    public ChainPosRuleSet(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetChainPosRuleSet = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        ChainPosRuleCount   = 0,
                        ChainPosRuleOffsets = 2
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.ChainPosRuleOffsets + (uint)ChainPosRuleCount*2;
                    }

                    public ushort ChainPosRuleCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetChainPosRuleSet + (uint)FieldOffsets.ChainPosRuleCount);}
                    }

                    public ushort GetChainPosRuleOffset(uint i)
                    {
                        uint offset = m_offsetChainPosRuleSet + (uint)FieldOffsets.ChainPosRuleOffsets + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public ChainPosRule GetChainPosRuleTable(uint i)
                    {
                        ChainPosRule pr = null;

                        if (i < ChainPosRuleCount)
                        {
                            uint offset = m_offsetChainPosRuleSet + GetChainPosRuleOffset(i);
                            pr = new ChainPosRule(offset, m_bufTable);
                        }

                        return pr;
                    }

                    protected uint m_offsetChainPosRuleSet;
                    protected MBOBuffer m_bufTable;
                }

                public class ChainPosRule
                {
                    public ChainPosRule(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetChainPosRule = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        BacktrackGlyphCount = 0,
                        BacktrackGlyphIDs   = 2
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.BacktrackGlyphIDs + (uint)BacktrackGlyphCount*2
                            + 2 + (uint)(InputGlyphCount-1)*2 
                            + 2 + (uint)LookaheadGlyphCount*2
                            + 2 + (uint)PosCount*4;
                    }

                    public ushort BacktrackGlyphCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphCount);}                    
                    }

                    public ushort GetBacktrackGlyphID(uint i)
                    {
                        uint offset = m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphIDs + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public ushort InputGlyphCount
                    {
                        get
                        {
                            uint offset = m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                + (uint)BacktrackGlyphCount*2;
                            return m_bufTable.GetUshort(offset);
                        }
                    }

                    public ushort GetInputGlyphID(uint i)
                    {
                        uint offset = m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                            + (uint)BacktrackGlyphCount*2 + 2 + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public ushort LookaheadGlyphCount
                    {
                        get
                        {
                            uint offset = m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2;
                            return m_bufTable.GetUshort(offset);
                        }
                    }

                    public ushort GetLookaheadGlyphID(uint i)
                    {
                        uint offset = m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public ushort PosCount
                    {
                        get
                        {
                            uint offset = m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                                + (uint)LookaheadGlyphCount*2;
                            return m_bufTable.GetUshort(offset);
                        }
                    }

                    public PosLookupRecord GetPosLookupRecord(uint i)
                    {
                        PosLookupRecord plr = null;

                        if (i < PosCount)
                        {
                            uint offset = m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                                + (uint)LookaheadGlyphCount*2 + 2 + i*4;
                            plr = new PosLookupRecord(offset, m_bufTable);
                        }

                        return plr;
                    }

                    protected uint m_offsetChainPosRule;
                    protected MBOBuffer m_bufTable;
                }



                // accessors

                public ushort PosFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.PosFormat);}
                }

                public ushort CoverageOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.CoverageOffset);}
                }

                public ushort ChainPosRuleSetCount
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.ChainPosRuleSetCount);}
                }

                public ushort GetChainPosRuleSetOffset(uint i)
                {
                    return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.ChainPosRuleSetOffsets + i*2);
                }

                public ChainPosRuleSet GetChainPosRuleSetTable(uint i)
                {
                    ChainPosRuleSet cprs = null;

                    if (i < ChainPosRuleSetCount)
                    {
                        cprs = new ChainPosRuleSet(m_offsetChainContextPos + GetChainPosRuleSetOffset(i), m_bufTable);
                    }

                    return cprs;
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetChainContextPos + CoverageOffset, m_bufTable);
                }

                protected uint m_offsetChainContextPos;
                protected MBOBuffer m_bufTable;
            }

            public class ChainContextPosFormat2
            {
                public ChainContextPosFormat2(uint offset, MBOBuffer bufTable)
                {
                    m_offsetChainContextPos = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    PosFormat               = 0,
                    CoverageOffset          = 2,
                    BacktrackClassDefOffset = 4,
                    InputClassDefOffset     = 6,
                    LookaheadClassDefOffset = 8,
                    ChainPosClassSetCount   = 10,
                    ChainPosClassSetOffsets = 12
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.ChainPosClassSetOffsets + (uint)ChainPosClassSetCount*2;
                }


                // nested classes

                public class ChainPosClassSet
                {
                    public ChainPosClassSet(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetChainPosClassSet = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        ChainPosClassRuleCount   = 0,
                        ChainPosClassRuleOffsets = 2
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.ChainPosClassRuleOffsets + (uint)ChainPosClassRuleCount*2;
                    }

                    public ushort ChainPosClassRuleCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetChainPosClassSet + (uint)FieldOffsets.ChainPosClassRuleCount);}
                    }

                    public ushort GetChainPosClassRuleOffset(uint i)
                    {
                        uint offset = m_offsetChainPosClassSet + (uint)FieldOffsets.ChainPosClassRuleOffsets + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public ChainPosClassRule GetChainPosClassRuleTable(uint i)
                    {
                        ChainPosClassRule cpcr = null;

                        if (i < ChainPosClassRuleCount)
                        {
                            uint offset = m_offsetChainPosClassSet + GetChainPosClassRuleOffset(i);
                            cpcr = new ChainPosClassRule(offset, m_bufTable);
                        }

                        return cpcr;
                    }

                    protected uint m_offsetChainPosClassSet;
                    protected MBOBuffer m_bufTable;
                }

                public class ChainPosClassRule
                {
                    public ChainPosClassRule(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetChainPosClassRule = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        BacktrackGlyphCount = 0,
                        BacktrackClasses    = 2
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.BacktrackClasses + (uint)BacktrackGlyphCount*2
                            + 2 + (uint)(InputGlyphCount-1)*2 
                            + 2 + (uint)LookaheadGlyphCount*2 
                            + 2 + (uint)PosCount*4;
                    }

                    public ushort BacktrackGlyphCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackGlyphCount);}                    
                    }

                    public ushort GetBacktrackClass(uint i)
                    {
                        uint offset = m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackClasses + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public ushort InputGlyphCount
                    {
                        get
                        {
                            uint offset = m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackClasses + (uint)BacktrackGlyphCount*2;
                            return m_bufTable.GetUshort(offset);
                        }
                    }

                    public ushort GetInputClass(uint i)
                    {
                        uint offset = m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackClasses 
                            + (uint)BacktrackGlyphCount*2 + 2 + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public ushort LookaheadGlyphCount
                    {
                        get
                        {
                            uint offset = m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackClasses 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2;
                            return m_bufTable.GetUshort(offset);
                        }
                    }

                    public ushort GetLookaheadClass(uint i)
                    {
                        uint offset = m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackClasses 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public ushort PosCount
                    {
                        get
                        {
                            uint offset = m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackClasses 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                                + (uint)LookaheadGlyphCount*2;
                            return m_bufTable.GetUshort(offset);
                        }
                    }

                    public PosLookupRecord GetPosLookupRecord(uint i)
                    {
                        PosLookupRecord plr = null;

                        if (i < PosCount)
                        {
                            uint offset = m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackClasses 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                                + (uint)LookaheadGlyphCount*2 + 2 + i*4;
                            plr = new PosLookupRecord(offset, m_bufTable);
                        }

                        return plr;
                    }

                    protected uint m_offsetChainPosClassRule;
                    protected MBOBuffer m_bufTable;
                }

                // accessors

                public ushort PosFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.PosFormat);}
                }

                public ushort CoverageOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.CoverageOffset);}
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetChainContextPos + CoverageOffset, m_bufTable);
                }

                public ushort BacktrackClassDefOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.BacktrackClassDefOffset);}
                }

                public OTL.ClassDefTable GetBacktrackClassDefTable()
                {
                    uint offset = m_offsetChainContextPos + BacktrackClassDefOffset;
                    return new OTL.ClassDefTable(offset, m_bufTable);
                }

                public ushort InputClassDefOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.InputClassDefOffset);}
                }

                public OTL.ClassDefTable GetInputClassDefTable()
                {
                    uint offset = m_offsetChainContextPos + InputClassDefOffset;
                    return new OTL.ClassDefTable(offset, m_bufTable);
                }

                public ushort LookaheadClassDefOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.LookaheadClassDefOffset);}
                }

                public OTL.ClassDefTable GetLookaheadClassDefTable()
                {
                    uint offset = m_offsetChainContextPos + LookaheadClassDefOffset;
                    return new OTL.ClassDefTable(offset, m_bufTable);
                }

                public ushort ChainPosClassSetCount
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.ChainPosClassSetCount);}
                }

                public ushort GetChainPosClassSetOffset(uint i)
                {
                    return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.ChainPosClassSetOffsets + i*2);
                }

                public ChainPosClassSet GetChainPosClassSetTable(uint i)
                {
                    ChainPosClassSet cpcs = null;

                    if (i < ChainPosClassSetCount)
                    {
                        uint offset = GetChainPosClassSetOffset(i);
                        if (offset != 0)
                        {
                            offset += m_offsetChainContextPos;
                            cpcs = new ChainPosClassSet(offset, m_bufTable);
                        }
                    }
                    return cpcs;
                }

                protected uint m_offsetChainContextPos;
                protected MBOBuffer m_bufTable;
            }

            public class ChainContextPosFormat3
            {
                public ChainContextPosFormat3(uint offset, MBOBuffer bufTable)
                {
                    m_offsetChainContextPos = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    PosFormat                = 0,
                    BacktrackGlyphCount      = 2,
                    BacktrackCoverageOffsets = 4
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.BacktrackCoverageOffsets + (uint)BacktrackGlyphCount*2
                        + 2 + (uint)InputGlyphCount*2
                        + 2 + (uint)LookaheadGlyphCount*2
                        + 2 + (uint)PosCount*4;
                }


                // accessors

                public ushort PosFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.PosFormat);}
                }

                public ushort BacktrackGlyphCount
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.BacktrackGlyphCount);}
                }

                public ushort GetBacktrackCoverageOffset(uint i)
                {
                    uint offset = m_offsetChainContextPos + (uint)FieldOffsets.BacktrackCoverageOffsets + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public OTL.CoverageTable GetBacktrackCoverageTable(uint i)
                {
                    OTL.CoverageTable ct = null;
                    if (i < BacktrackGlyphCount)
                    {
                        uint offset = m_offsetChainContextPos + GetBacktrackCoverageOffset(i);
                        ct = new OTL.CoverageTable(offset, m_bufTable);
                    }
                    return ct;
                }

                public ushort InputGlyphCount
                {
                    get
                    {
                        uint offset = m_offsetChainContextPos + (uint)FieldOffsets.BacktrackCoverageOffsets 
                            + (uint)BacktrackGlyphCount*2;
                        return m_bufTable.GetUshort(offset);
                    }
                }

                public ushort GetInputCoverageOffset(uint i)
                {
                    uint offset = m_offsetChainContextPos + (uint)FieldOffsets.BacktrackCoverageOffsets 
                        + (uint)BacktrackGlyphCount*2 + 2 + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public OTL.CoverageTable GetInputCoverageTable(uint i)
                {
                    OTL.CoverageTable ct = null;
                    if (i < InputGlyphCount)
                    {
                        uint offset = m_offsetChainContextPos + GetInputCoverageOffset(i);
                        ct = new OTL.CoverageTable(offset, m_bufTable);
                    }
                    return ct;
                }

                public ushort LookaheadGlyphCount
                {
                    get
                    {
                        uint offset = m_offsetChainContextPos + (uint)FieldOffsets.BacktrackCoverageOffsets 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2;
                        return m_bufTable.GetUshort(offset);
                    }
                }

                public ushort GetLookaheadCoverageOffset(uint i)
                {
                    uint offset = m_offsetChainContextPos + (uint)FieldOffsets.BacktrackCoverageOffsets 
                        + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2 + 2 + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public OTL.CoverageTable GetLookaheadCoverageTable(uint i)
                {
                    OTL.CoverageTable ct = null;
                    if (i < LookaheadGlyphCount)
                    {
                        uint offset = m_offsetChainContextPos + GetLookaheadCoverageOffset(i);
                        ct = new OTL.CoverageTable(offset, m_bufTable);
                    }
                    return ct;
                }

                public ushort PosCount
                {
                    get
                    {
                        uint offset = m_offsetChainContextPos + (uint)FieldOffsets.BacktrackCoverageOffsets 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2 + 2 
                            + (uint)LookaheadGlyphCount*2;
                        return m_bufTable.GetUshort(offset);
                    }
                }

                public PosLookupRecord GetPosLookupRecord(uint i)
                {
                    PosLookupRecord plr = null;

                    if (i < PosCount)
                    {
                        uint offset = m_offsetChainContextPos + (uint)FieldOffsets.BacktrackCoverageOffsets 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2 + 2 
                            + (uint)LookaheadGlyphCount*2 + 2 + i*4;
                        plr = new PosLookupRecord(offset, m_bufTable);
                    }

                    return plr;
                }


                protected uint m_offsetChainContextPos;
                protected MBOBuffer m_bufTable;
            }

            public class PosLookupRecord
            {
                public PosLookupRecord(uint offset, MBOBuffer bufTable)
                {
                    m_offsetPosLookupRecord = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SequenceIndex = 0,
                    LookupListIndex = 2
                }

                public ushort SequenceIndex
                {
                    get {return m_bufTable.GetUshort(m_offsetPosLookupRecord + (uint)FieldOffsets.SequenceIndex);}
                }

                public ushort LookupListIndex
                {
                    get {return m_bufTable.GetUshort(m_offsetPosLookupRecord + (uint)FieldOffsets.LookupListIndex);}
                }

                protected uint m_offsetPosLookupRecord;
                protected MBOBuffer m_bufTable;
            }


            // accessors

            public ushort PosFormat
            {
                get {return m_bufTable.GetUshort(m_offsetChainContextPos + (uint)FieldOffsets.PosFormat);}
            }

            public ChainContextPosFormat1 GetChainContextPosFormat1()
            {
                if (PosFormat != 1)
                {
                    throw new InvalidOperationException("format not 1");
                }
                return new ChainContextPosFormat1(m_offsetChainContextPos, m_bufTable);
            }

            public ChainContextPosFormat2 GetChainContextPosFormat2()
            {
                if (PosFormat != 2)
                {
                    throw new InvalidOperationException("format not 2");
                }
                return new ChainContextPosFormat2(m_offsetChainContextPos, m_bufTable);
            }

            public ChainContextPosFormat3 GetChainContextPosFormat3()
            {
                if (PosFormat != 3)
                {
                    throw new InvalidOperationException("format not 3");
                }
                return new ChainContextPosFormat3(m_offsetChainContextPos, m_bufTable);
            }

            protected uint m_offsetChainContextPos;
        }

        // Lookup Type 9: Extension Positioning Subtable
        
        public class ExtensionPos : OTL.SubTable
        {
            public ExtensionPos(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetExtensionPos = offset;
            }

            public enum FieldOffsets
            {
                PosFormat           = 0,
                ExtensionLookupType = 2,
                ExtensionOffset     = 4
            }

            // public methods

            public uint CalcLength()
            {
                return 8;
            }

            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                uint nLength = 0;

                OTL.SubTable st = null;
                switch (ExtensionLookupType)
                {
                    case 1: st = new Table_GPOS.SinglePos      (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 2: st = new Table_GPOS.PairPos        (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 3: st = new Table_GPOS.CursivePos     (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 4: st = new Table_GPOS.MarkBasePos    (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 5: st = new Table_GPOS.MarkLigPos     (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 6: st = new Table_GPOS.MarkMarkPos    (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 7: st = new Table_GPOS.ContextPos     (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 8: st = new Table_GPOS.ChainContextPos(m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                }
                if (st != null)
                {
                    nLength = st.GetMaxContextLength();
                }

                return nLength;
            }

            // accessors

            public ushort PosFormat
            {
                get {return m_bufTable.GetUshort(m_offsetExtensionPos + (uint)FieldOffsets.PosFormat);}
            }

            public ushort ExtensionLookupType
            {
                get {return m_bufTable.GetUshort(m_offsetExtensionPos + (uint)FieldOffsets.ExtensionLookupType);}
            }

            public uint ExtensionOffset
            {
                get {return m_bufTable.GetUint(m_offsetExtensionPos + (uint)FieldOffsets.ExtensionOffset);}
            }



            protected uint m_offsetExtensionPos;
        }

        /************************
         * accessors
         */

        public OTFixed Version
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.Version);}
        }

        public ushort ScriptListOffset
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.ScriptListOffset);}
        }

        public ushort FeatureListOffset
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.FeatureListOffset);}
        }

        public ushort LookupListOffset
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.LookupListOffset);}
        }



        public OTL.ScriptListTable GetScriptListTable()
        {
            return new OTL.ScriptListTable(ScriptListOffset, m_bufTable);
        }

        public OTL.FeatureListTable GetFeatureListTable()
        {
            return new OTL.FeatureListTable(FeatureListOffset, m_bufTable);
        }

        public OTL.LookupListTable GetLookupListTable()
        {
            return new OTL.LookupListTable(LookupListOffset, m_bufTable, m_tag);
        }



        // GetMaxContext is used by OS/2 validation

        public ushort GetMaxContext()
        {
            // return the max length of a target glyph context for any feature in the font
            ushort usMaxLength = 0;

            OTL.FeatureListTable flt = GetFeatureListTable();
            if (flt != null)
            {
                for (uint iFeature=0; iFeature<flt.FeatureCount; iFeature++)
                {
                    OTL.FeatureListTable.FeatureRecord fr = flt.GetFeatureRecord(iFeature);
                    OTL.FeatureTable ft = flt.GetFeatureTable(fr);
                    for (uint iLookup = 0; iLookup < ft.LookupCount; iLookup++)
                    {
                        ushort LookupIndex = ft.GetLookupListIndex(iLookup);
                        ushort length = GetMaxContextFromLookup(LookupIndex);
                        if (length > usMaxLength)
                        {
                            usMaxLength = length;
                        }
                    }
                }
            }

            return usMaxLength;
        }

        protected ushort GetMaxContextFromLookup(uint iLookup)
        {
            ushort length = 0;

            OTL.LookupListTable llt = GetLookupListTable();
            if (llt != null)
            {                
                OTL.LookupTable lt = llt.GetLookupTable(iLookup);
                for (uint iSubTable = 0; iSubTable < lt.SubTableCount; iSubTable++)
                {
                    OTL.SubTable st = lt.GetSubTable(iSubTable);
                    ushort tempLength = (ushort)st.GetMaxContextLength();
                    if (length < tempLength)
                    {
                        length = tempLength;
                    }
                    
                }
                
            }

            return length;
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new GPOS_cache();
            }

            return m_cache;
        }
        
        public class GPOS_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        


    }
}
