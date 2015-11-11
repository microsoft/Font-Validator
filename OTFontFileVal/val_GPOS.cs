using System;
using System.Diagnostics;

using OTFontFile;
using OTFontFile.OTL;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_GPOS.
    /// </summary>
    public class val_GPOS : Table_GPOS, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_GPOS(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
            m_DataOverlapDetector = new DataOverlapDetector();
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.GPOS_Version))
            {
                if (Version.GetUint() == 0x00010000)
                {
                    v.Pass(T.GPOS_Version, P.GPOS_P_Version, m_tag);
                }
                else
                {
                    v.Error(T.GPOS_Version, E.GPOS_E_Version, m_tag, "0x" + Version.GetUint().ToString("x8"));
                    bRet = false;
                }
            }


            if (v.PerformTest(T.GPOS_HeaderOffsets))
            {
                bool bOffsetsOk = true;

                if (ScriptListOffset == 0)
                {
                    v.Error(T.GPOS_HeaderOffsets, E.GPOS_E_HeaderOffset_0, m_tag, "ScriptListOffset");
                    bOffsetsOk = false;
                    bRet = false;
                }
                else
                {
                    if (ScriptListOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.GPOS_HeaderOffsets, E.GPOS_E_HeaderOffset, m_tag, "ScriptListOffset");
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }

                if (FeatureListOffset == 0)
                {
                    v.Error(T.GPOS_HeaderOffsets, E.GPOS_E_HeaderOffset_0, m_tag, "FeatureListOffset");
                    bOffsetsOk = false;
                    bRet = false;
                }
                else
                {
                    if (FeatureListOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.GPOS_HeaderOffsets, E.GPOS_E_HeaderOffset, m_tag, "FeatureListOffset");
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }

                if (LookupListOffset == 0)
                {
                    v.Error(T.GPOS_HeaderOffsets, E.GPOS_E_HeaderOffset_0, m_tag, "LookupListOffset");
                    bOffsetsOk = false;
                    bRet = false;
                }
                else
                {
                    if (LookupListOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.GPOS_HeaderOffsets, E.GPOS_E_HeaderOffset, m_tag, "LookupListOffset");
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }


                if (bOffsetsOk == true)
                {
                    v.Pass(T.GPOS_HeaderOffsets, P.GPOS_P_HeaderOffsets, m_tag);
                }
            }

            if (v.PerformTest(T.GPOS_Subtables))
            {
                ScriptListTable_val slt = GetScriptListTable_val();
                bRet &= slt.Validate(v, "ScriptList", this);

                FeatureListTable_val flt = GetFeatureListTable_val();
                bRet &= flt.Validate(v, "FeatureList", this);

                LookupListTable_val llt = GetLookupListTable_val();
                bRet &= llt.Validate(v, "LookupList", this);
            }


            return bRet;
        }

        
        /************************
         * classes
         */

        public class ValueRecord_val : ValueRecord, I_OTLValidate
        {
            public ValueRecord_val(uint offset, MBOBuffer bufTable, uint offsetPosTable, ushort ValueFormat)
                : base (offset, bufTable, offsetPosTable, ValueFormat)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                // check the offsets

                if (XPlaDevicePresent)
                {
                    if (XPlaDeviceOffset != 0)
                    {
                        if (m_offsetPosTable + XPlaDeviceOffset > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", XPlaDevice offset");
                            bRet = false;
                        }
                    }
                }


                if (YPlaDevicePresent)
                {
                    if (YPlaDeviceOffset != 0)
                    {
                        if (m_offsetPosTable + YPlaDeviceOffset > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", YPlaDevice offset");
                            bRet = false;
                        }
                    }
                }

                if (XAdvDevicePresent)
                {
                    if (XAdvDeviceOffset != 0)
                    {
                        if (m_offsetPosTable + XAdvDeviceOffset > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", XAdvDevice offset");
                            bRet = false;
                        }
                    }
                }

                if (YAdvDevicePresent)
                {
                    if (YAdvDeviceOffset != 0)
                    {
                        if (m_offsetPosTable + YAdvDeviceOffset > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", YAdvDevice offset");
                            bRet = false;
                        }
                    }
                }

                // validate each device table
                DeviceTable_val dt;

                dt = GetXPlaDeviceTable_val();
                if (dt != null)
                {
                    bRet &= dt.Validate(v, sIdentity + ", XPlaDeviceTable", table);
                }

                dt = GetYPlaDeviceTable_val();
                if (dt != null)
                {
                    bRet &= dt.Validate(v, sIdentity + ", YPlaDeviceTable", table);
                }

                dt = GetXAdvDeviceTable_val();
                if (dt != null)
                {
                    bRet &= dt.Validate(v, sIdentity + ", XAdvDeviceTable", table);
                }

                dt = GetYAdvDeviceTable_val();
                if (dt != null)
                {
                    bRet &= dt.Validate(v, sIdentity+ ", YAdvDeviceTable", table);
                }


                // way too many ValueRecords to justify this pass message
                //if (bRet)
                //{
                //    v.Pass(T.T_NULL, P.GPOS_P_ValueRecord, table.m_tag, sIdentity);
                //}

                return bRet;
            }



            public DeviceTable_val GetXPlaDeviceTable_val()
            {
                DeviceTable_val dt = null;

                if (XPlaDevicePresent)
                {
                    if (XPlaDeviceOffset != 0)
                    {
                        dt = new DeviceTable_val(m_offsetPosTable + XPlaDeviceOffset, m_bufTable);
                    }
                }

                return dt;
            }

            public DeviceTable_val GetYPlaDeviceTable_val()
            {
                DeviceTable_val dt = null;

                if (YPlaDevicePresent)
                {
                    if (YPlaDeviceOffset != 0)
                    {
                        dt = new DeviceTable_val(m_offsetPosTable + YPlaDeviceOffset, m_bufTable);
                    }
                }

                return dt;
            }

            public DeviceTable_val GetXAdvDeviceTable_val()
            {
                DeviceTable_val dt = null;

                if (XAdvDevicePresent)
                {
                    if (XAdvDeviceOffset != 0)
                    {
                        dt = new DeviceTable_val(m_offsetPosTable + XAdvDeviceOffset, m_bufTable);
                    }
                }

                return dt;
            }

            public DeviceTable_val GetYAdvDeviceTable_val()
            {
                DeviceTable_val dt = null;

                if (YAdvDevicePresent)
                {
                    if (YAdvDeviceOffset != 0)
                    {
                        dt = new DeviceTable_val(m_offsetPosTable + YAdvDeviceOffset, m_bufTable);
                    }
                }

                return dt;
            }



        }

        public class AnchorTable_val : AnchorTable, I_OTLValidate
        {
            public AnchorTable_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                if (AnchorFormat == 1)
                {
                    AnchorFormat1_val af1 = GetAnchorFormat1_val();
                    bRet &= af1.Validate(v, sIdentity, table);
                }
                else if (AnchorFormat == 2)
                {
                    AnchorFormat2_val af2 = GetAnchorFormat2_val();
                    bRet &= af2.Validate(v, sIdentity, table);
                }
                else if (AnchorFormat == 3)
                {
                    AnchorFormat3_val af3 = GetAnchorFormat3_val();
                    bRet &= af3.Validate(v, sIdentity, table);
                }
                else
                {
                    v.Error(T.T_NULL, E.GPOS_E_AnchorTable_format, table.m_tag, sIdentity + ", AnchorFormat = " + AnchorFormat);
                    bRet = false;
                }
                
                // way too many anchor tables to justify this pass message
                //if (bRet)
                //{
                //    v.Pass(T.T_NULL, P.GPOS_P_AnchorTable, table.m_tag, sIdentity);
                //}

                return bRet;
            }

            // nested classes
            public class AnchorFormat1_val : AnchorFormat1, I_OTLValidate
            {
                public AnchorFormat1_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetAnchorTable, CalcLength(), v, sIdentity, table.GetTag());

                    return bRet;
                }
                
            }

            public class AnchorFormat2_val : AnchorFormat2, I_OTLValidate
            {
                public AnchorFormat2_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetAnchorTable, CalcLength(), v, sIdentity, table.GetTag());

                    return bRet;
                }
            }

            public class AnchorFormat3_val : AnchorFormat3, I_OTLValidate
            {
                public AnchorFormat3_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetAnchorTable, CalcLength(), v, sIdentity, table.GetTag());

                    // check that the offsets are within the table

                    if (XDeviceTableOffset != 0)
                    {
                        if (m_offsetAnchorTable + XDeviceTableOffset > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", XDeviceTable");
                            bRet = false;
                        }
                    }

                    if (YDeviceTableOffset != 0)
                    {
                        if (m_offsetAnchorTable + YDeviceTableOffset > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", YDeviceTable");
                            bRet = false;
                        }
                    }

                    // validate the device tables

                    if (XDeviceTableOffset != 0)
                    {
                        DeviceTable_val xdt = GetXDeviceTable_val();
                        xdt.Validate(v, sIdentity + ", XDeviceTable", table);
                    }

                    if (YDeviceTableOffset != 0)
                    {
                        DeviceTable_val ydt = GetYDeviceTable_val();
                        ydt.Validate(v, sIdentity + ", YDeviceTable", table);
                    }

                    return bRet;
                }


                public DeviceTable_val GetXDeviceTable_val()
                {
                    DeviceTable_val dt = null;
                    if (XDeviceTableOffset != 0)
                    {
                        dt = new DeviceTable_val(m_offsetAnchorTable + XDeviceTableOffset, m_bufTable);
                    }
                    return dt;
                }

                public DeviceTable_val GetYDeviceTable_val()
                {
                    DeviceTable_val dt = null;
                    if (YDeviceTableOffset != 0)
                    {
                        dt = new DeviceTable_val(m_offsetAnchorTable + YDeviceTableOffset, m_bufTable);
                    }
                    return dt;
                }
            }
            


            public AnchorFormat1_val GetAnchorFormat1_val()
            {
                if (AnchorFormat != 1)
                {
                    throw new InvalidOperationException("format not 1");
                }
                return new AnchorFormat1_val(m_offsetAnchorTable, m_bufTable);
            }

            public AnchorFormat2_val GetAnchorFormat2_val()
            {
                if (AnchorFormat != 2)
                {
                    throw new InvalidOperationException("format not 2");
                }
                return new AnchorFormat2_val(m_offsetAnchorTable, m_bufTable);
            }

            public AnchorFormat3_val GetAnchorFormat3_val()
            {
                if (AnchorFormat != 3)
                {
                    throw new InvalidOperationException("format not 3");
                }
                return new AnchorFormat3_val(m_offsetAnchorTable, m_bufTable);
            }
        }

        public class MarkArray_val : MarkArray, I_OTLValidate
        {
            public MarkArray_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetMarkArray, CalcLength(), v, sIdentity, table.GetTag());

                // check that the Mark Record array doesn't extend past end of table
                if (m_offsetMarkArray + (uint)FieldOffsets.MarkRecordArray + MarkCount*4 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", MarkRecord");
                    bRet = false;
                }

                // check each MarkRecord
                for (uint i=0; i<MarkCount; i++)
                {
                    MarkRecord mr = GetMarkRecord(i);
                    if (m_offsetMarkArray + mr.MarkAnchorOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", MarkRecord[" + i + "]");
                        bRet = false;
                    }
                    AnchorTable_val at = new AnchorTable_val(m_offsetMarkArray + mr.MarkAnchorOffset, m_bufTable);
                    bRet &= at.Validate(v, sIdentity + ", MarkRecord[" + i + "], AnchorTable", table);
                }

                // way too many MarkArry tables to justify this pass message
                //if (bRet)
                //{
                //    v.Pass(T.T_NULL, P.GPOS_P_MarkArrayTable, table.m_tag, sIdentity);
                //}

                return bRet;
            }

        }



        // Lookup Type 1: single adjustment positioning subtable

        public class SinglePos_val : SinglePos, I_OTLValidate
        {
            public SinglePos_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(SinglePos, fmt " + PosFormat + ")";


                if (PosFormat == 1)
                {
                    SinglePosFormat1_val f1 = GetSinglePosFormat1_val();
                    f1.Validate(v, sIdentity, table);
                }
                else if (PosFormat == 2)
                {
                    SinglePosFormat2_val f2 = GetSinglePosFormat2_val();
                    f2.Validate(v, sIdentity, table);
                }
                else
                {
                    v.Error(T.T_NULL, E.GPOS_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }

                return bRet;
            }


            // nested classes

            public class SinglePosFormat1_val : SinglePosFormat1, I_OTLValidate
            {
                public SinglePosFormat1_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetSinglePos, CalcLength(), v, sIdentity, table.GetTag());

                    // the PosFormat field was validated before this object was constructed

                    // coverage offset
                    if (m_offsetSinglePos + Coverage > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the ValueFormat reserved bits
                    if ((ValueFormat & 0xff00) != 0)
                    {
                        v.Error(T.T_NULL, E.GPOS_E_ValueFormatReservedBits, table.m_tag, sIdentity);
                        bRet = false;
                    }

                    // validate the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                    // validate the ValueRecord
                    ValueRecord_val vr = Value_val;
                    bRet &= vr.Validate(v, sIdentity + ", ValueRecord", table);


                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_SinglePos, table.m_tag, sIdentity);
                    }

                    return bRet;
                }
                

                CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetSinglePos + Coverage, m_bufTable);
                }

                public ValueRecord_val Value_val
                {
                    get
                    {
                        uint offset = m_offsetSinglePos + (uint)FieldOffsets.Value;
                        return new ValueRecord_val(offset, m_bufTable, m_offsetSinglePos, ValueFormat);
                    }
                }
            }

            public class SinglePosFormat2_val : SinglePosFormat2, I_OTLValidate
            {
                public SinglePosFormat2_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetSinglePos, CalcLength(), v, sIdentity, table.GetTag());

                    // the PosFormat field was validated before this object was constructed

                    // coverage offset
                    if (m_offsetSinglePos + Coverage > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the ValueFormat reserved bits
                    if ((ValueFormat & 0xff00) != 0)
                    {
                        v.Error(T.T_NULL, E.GPOS_E_ValueFormatReservedBits, table.m_tag, sIdentity);
                        bRet = false;
                    }

                    // check that the ValueRecord array doesn't extend past end of table
                    uint SizeOfValueRecord = ValueRecord.SizeOfValueRecord(ValueFormat);
                    if (m_offsetSinglePos + (uint)FieldOffsets.ValueRecordArray + ValueCount*SizeOfValueRecord > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_ValueArray_pastEOT, table.m_tag, sIdentity);
                        bRet = false;
                    }

                    // validate the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                    // validate the ValueRecords
                    for (uint i=0; i<ValueCount; i++)
                    {
                        ValueRecord_val vr = GetValue_val(i);
                        bRet &= vr.Validate(v, sIdentity + ", ValueRecord[" + i + "]", table);
                    }

                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_SinglePos, table.m_tag, sIdentity);
                    }

                    return bRet;
                }
                
                public ValueRecord_val GetValue_val(uint i)
                {
                    ValueRecord_val vr = null;
                    if (i < ValueCount)
                    {
                        uint sizeofValueRecord = ValueRecord.SizeOfValueRecord(ValueFormat);
                        uint offset = m_offsetSinglePos + (uint)FieldOffsets.ValueRecordArray + i*sizeofValueRecord;
                        vr = new ValueRecord_val(offset, m_bufTable, m_offsetSinglePos, ValueFormat);
                    }
                    return vr;
                }

                CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetSinglePos + Coverage, m_bufTable);
                }
            }

            public SinglePosFormat1_val GetSinglePosFormat1_val()
            {
                if (PosFormat != 1)
                {
                    throw new System.InvalidOperationException("this object is not format 1");
                }

                return new SinglePosFormat1_val(m_offsetSinglePos, m_bufTable);
            }

            public SinglePosFormat2_val GetSinglePosFormat2_val()
            {
                if (PosFormat != 2)
                {
                    throw new System.InvalidOperationException("this object is not format 2");
                }

                return new SinglePosFormat2_val(m_offsetSinglePos, m_bufTable);
            }
        }


        // Lookup Type 2: Pair Adjustment Positioning Subtable
        
        public class PairPos_val : PairPos, I_OTLValidate
        {
            public PairPos_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(PairPos, fmt " + PosFormat + ")";


                if (PosFormat == 1)
                {
                    PairPosFormat1_val f1 = GetPairPosFormat1_val();
                    f1.Validate(v, sIdentity, table);
                }
                else if (PosFormat == 2)
                {
                    PairPosFormat2_val f2 = GetPairPosFormat2_val();
                    f2.Validate(v, sIdentity, table);
                }
                else
                {
                    v.Error(T.T_NULL, E.GPOS_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }

                return bRet;
            }


            // nested classes

            public class PairPosFormat1_val : PairPosFormat1, I_OTLValidate
            {
                public PairPosFormat1_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }


                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetPairPos, CalcLength(), v, sIdentity, table.GetTag());

                    // the PosFormat field was validated before this object was constructed

                    // coverage offset
                    if (m_offsetPairPos + Coverage > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the ValueFormat1 reserved bits
                    if ((ValueFormat1 & 0xff00) != 0)
                    {
                        v.Error(T.T_NULL, E.GPOS_E_ValueFormatReservedBits, table.m_tag, sIdentity + ", ValueFormat1");
                        bRet = false;
                    }

                    // check the ValueFormat2 reserved bits
                    if ((ValueFormat2 & 0xff00) != 0)
                    {
                        v.Error(T.T_NULL, E.GPOS_E_ValueFormatReservedBits, table.m_tag, sIdentity + ", ValueFormat2");
                        bRet = false;
                    }

                    // check that the PairSet array doesn't extend past the end of the table
                    if (m_offsetPairPos + (uint)FieldOffsets.PairSetOffsets + PairSetCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PairSet");
                        bRet = false;
                    }

                    // check that each PairSet offset is within the table
                    for (uint i=0; i<PairSetCount; i++)
                    {
                        if (m_offsetPairPos + GetPairSetOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", PairSet[" + i + "]");
                            bRet = false;
                        }
                    }

                    // validate the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                    // validate the PairSet tables
                    for (uint i=0; i<PairSetCount; i++)
                    {
                        PairSetTable_val pst = GetPairSetTable_val(i);
                        bRet &= pst.Validate(v, sIdentity + ", PairSet[" + i + "]", table);
                    }

                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_PairPos, table.m_tag, sIdentity);
                    }

                    return bRet;
                }


                public class PairSetTable_val : PairSetTable, I_OTLValidate
                {
                    public PairSetTable_val(uint offset, MBOBuffer bufTable, uint offsetPosTable, ushort ValueFormat1, ushort ValueFormat2)
                        : base(offset, bufTable, offsetPosTable, ValueFormat1, ValueFormat2)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;
                    
                        // check for data overlap
                        bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetPairSetTable, CalcLength(), v, sIdentity, table.GetTag());

                        // check that PairValueRecord array doesn't extend past end of table
                        uint sizeofPairValueRecord = 2 + ValueRecord.SizeOfValueRecord(m_ValueFormat1) + ValueRecord.SizeOfValueRecord(m_ValueFormat2);
                        if (m_offsetPairSetTable + (uint)FieldOffsets.PairValueRecordArray + PairValueCount*sizeofPairValueRecord > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PairValueRecord");
                            bRet = false;
                        }

                        // check the PairValue record order
                        if (PairValueCount > 1)
                        {
                            for (uint i=0; i<PairValueCount-1; i++)
                            {
                                PairValueRecord pvrThis = GetPairValueRecord(i);
                                PairValueRecord pvrNext = GetPairValueRecord(i+1);

                                if (pvrThis.SecondGlyph >= pvrNext.SecondGlyph)
                                {
                                    v.Error(T.T_NULL, E.GPOS_E_Array_order, table.m_tag, sIdentity + ", PairValueRecord");
                                    bRet = false;
                                }
                            }
                        }

                        // validate the PairValueRecords
                        for (uint i=0; i<PairValueCount; i++)
                        {

                            PairValueRecord_val pvr = GetPairValueRecord_val(i);
                            pvr.Validate(v, sIdentity + ", PairValueRecord[" + i + "]", table);
                        }

                        if (bRet)
                        {
                            v.Pass(T.T_NULL, P.GPOS_P_PairSet, table.m_tag, sIdentity);
                        }

                        return bRet;
                    }

                    public PairValueRecord_val GetPairValueRecord_val(uint i)
                    {
                        PairValueRecord_val pvr = null;

                        if (i < PairValueCount)
                        {
                            uint sizeofPairValueRecord = 2 + ValueRecord.SizeOfValueRecord(m_ValueFormat1) + ValueRecord.SizeOfValueRecord(m_ValueFormat2);
                            uint offset = m_offsetPairSetTable + (uint)FieldOffsets.PairValueRecordArray + i*sizeofPairValueRecord;
                            pvr = new PairValueRecord_val(offset, m_bufTable, m_offsetPairSetTable, m_ValueFormat1, m_ValueFormat2);
                        }

                        return pvr;
                    }                    
                }

                public class PairValueRecord_val : PairValueRecord, I_OTLValidate
                {
                    public PairValueRecord_val(uint offset, MBOBuffer bufTable, uint offsetPairSetTable, ushort ValueFormat1, ushort ValueFormat2)
                        : base(offset, bufTable, offsetPairSetTable, ValueFormat1, ValueFormat2)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;


                        // validate Value1
                        bRet &= Value1_val.Validate(v, sIdentity + ", Value1", table);

                        // Validate Value2
                        bRet &= Value2_val.Validate(v, sIdentity + ", Value2", table);

                        return bRet;
                    }


                    public ValueRecord_val Value1_val
                    {
                        get
                        {
                            uint offset = m_offsetPairValueRecord + (uint)FieldOffsets.Value1;
                            return new ValueRecord_val(offset, m_bufTable, m_offsetPairSetTable, m_ValueFormat1);
                        }
                    }

                    public ValueRecord_val Value2_val
                    {
                        get
                        {
                            uint offset = m_offsetPairValueRecord + (uint)FieldOffsets.Value1 + ValueRecord.SizeOfValueRecord(m_ValueFormat1);
                            return new ValueRecord_val(offset, m_bufTable, m_offsetPairSetTable, m_ValueFormat2);
                        }
                    }
                }



                public PairSetTable_val GetPairSetTable_val(uint i)
                {
                    if (PosFormat != 1)
                    {
                        throw new System.InvalidOperationException();
                    }

                    PairSetTable_val pst = null;

                    if (i < PairSetCount)
                    {
                        uint offset = m_offsetPairPos + GetPairSetOffset(i);
                        return new PairSetTable_val(offset, m_bufTable, m_offsetPairPos, ValueFormat1, ValueFormat2);
                    }

                    return pst;
                }

                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetPairPos + Coverage, m_bufTable);
                }
            }

            public class PairPosFormat2_val : PairPosFormat2, I_OTLValidate
            {
                public PairPosFormat2_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;


                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetPairPos, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetPairPos + Coverage > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the ValueFormat1 reserved bits
                    if ((ValueFormat1 & 0xff00) != 0)
                    {
                        v.Error(T.T_NULL, E.GPOS_E_ValueFormatReservedBits, table.m_tag, sIdentity + ", ValueFormat1");
                        bRet = false;
                    }

                    // check the ValueFormat2 reserved bits
                    if ((ValueFormat2 & 0xff00) != 0)
                    {
                        v.Error(T.T_NULL, E.GPOS_E_ValueFormatReservedBits, table.m_tag, sIdentity + ", ValueFormat2");
                        bRet = false;
                    }

                    // check the ClassDef1 offset
                    if (m_offsetPairPos + ClassDef1Offset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", ClassDef1");
                        bRet = false;
                    }

                    // check the ClassDef2 offset
                    if (m_offsetPairPos + ClassDef2Offset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", ClassDef2");
                        bRet = false;
                    }

                    // check that the Class1Record array doesn't extend past the end of the table
                    uint sizeofClass2 = ValueRecord.SizeOfValueRecord(ValueFormat1) + ValueRecord.SizeOfValueRecord(ValueFormat2);
                    if (m_offsetPairPos + (uint)FieldOffsets.Class1Records + Class1Count*Class2Count*sizeofClass2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", Class1Record");
                        bRet = false;
                    }

                    // validate the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                    // validate the ClassDef tables
                    ClassDefTable_val cdt1 = GetClassDef1Table_val();
                    bRet &= cdt1.Validate(v, sIdentity + ", ClassDef1", table);

                    ClassDefTable_val cdt2 = GetClassDef2Table_val();
                    bRet &= cdt2.Validate(v, sIdentity + ", ClassDef2", table);

                    // validate the Class1 records
                    for (uint i=0; i<Class1Count; i++)
                    {
                        Class1Record_val c1r = GetClass1Record_val(i);
                        c1r.Validate(v, sIdentity + ", Class1Record[" + i + "]", table);
                    }


                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_PairPos, table.m_tag, sIdentity);
                    }

                    return bRet;
                }


                // nested classes

                public class Class1Record_val : Class1Record, I_OTLValidate
                {
                    public Class1Record_val(uint offset, MBOBuffer bufTable, ushort Class2Count, uint offsetPosTable, ushort ValueFormat1, ushort ValueFormat2)
                        : base(offset, bufTable, Class2Count, offsetPosTable, ValueFormat1, ValueFormat2)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        for (uint i=0; i<m_Class2Count; i++)
                        {
                            Class2Record_val c2r = GetClass2Record_val(i);
                            bRet &= c2r.Validate(v, sIdentity + ", Class2Record[" + i + "]", table);
                        }

                        // way too many Class1Records to justify this pass message
                        //if (bRet)
                        //{
                        //    v.Pass(T.T_NULL, P.GPOS_P_Class1Record, table.m_tag, sIdentity);
                        //}

                        return bRet;
                    }

                    public Class2Record_val GetClass2Record_val(uint i)
                    {
                        Class2Record_val c2r = null;

                        if (i < m_Class2Count)
                        {
                            ushort sizeofClass2Record = (ushort)(ValueRecord.SizeOfValueRecord(m_ValueFormat1) + ValueRecord.SizeOfValueRecord(m_ValueFormat2));
                            uint offset = m_offsetClass1Record + i*sizeofClass2Record;
                            c2r = new Class2Record_val(offset, m_bufTable, m_offsetPosTable, m_ValueFormat1, m_ValueFormat2);
                        }

                        return c2r;
                    }

                }

                public class Class2Record_val : Class2Record, I_OTLValidate
                {
                    public Class2Record_val(uint offset, MBOBuffer bufTable, uint offsetPosTable, ushort ValueFormat1, ushort ValueFormat2)
                        : base(offset, bufTable, offsetPosTable, ValueFormat1, ValueFormat2)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        bRet &= Value1_val.Validate(v, sIdentity + ", Value1", table);
                        bRet &= Value2_val.Validate(v, sIdentity + ", Value2", table);

                        // way too many Class2Records to justify this pass message
                        //if (bRet)
                        //{
                        //    v.Pass(T.T_NULL, P.GPOS_P_Class2Record, table.m_tag, sIdentity);
                        //}

                        return bRet;
                    }

                    // accessors

                    public ValueRecord_val Value1_val
                    {
                        get
                        {
                            uint offset = m_offsetClass2Record + (uint)m_offsetValue1;
                            return new ValueRecord_val(offset, m_bufTable, m_offsetPosTable, m_ValueFormat1);
                        }
                    }

                    public ValueRecord_val Value2_val
                    {
                        get
                        {
                            uint offset = m_offsetClass2Record + (uint)m_offsetValue2;
                            return new ValueRecord_val(offset, m_bufTable, m_offsetPosTable, m_ValueFormat2);
                        }
                    }
                }


                public Class1Record_val GetClass1Record_val(uint i)
                {
                    if (PosFormat != 2)
                    {
                        throw new System.InvalidOperationException();
                    }

                    Class1Record_val c1r = null;

                    if (i < Class1Count)
                    {
                        ushort sizeofClass1Record = (ushort)( Class2Count * ValueRecord.SizeOfValueRecord(ValueFormat2));
                        uint offset = m_offsetPairPos + (uint)FieldOffsets.Class1Records + i*sizeofClass1Record;
                        c1r = new Class1Record_val(offset, m_bufTable, Class2Count, m_offsetPairPos, ValueFormat1, ValueFormat2);
                    }

                    return c1r;
                }

                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetPairPos + Coverage, m_bufTable);
                }

                public ClassDefTable_val GetClassDef1Table_val()
                {
                    return new ClassDefTable_val(m_offsetPairPos + ClassDef1Offset, m_bufTable);
                }

                public ClassDefTable_val GetClassDef2Table_val()
                {
                    return new ClassDefTable_val(m_offsetPairPos + ClassDef2Offset, m_bufTable);
                }
            }

            
            
            public PairPosFormat1_val GetPairPosFormat1_val()
            {
                if (PosFormat != 1)
                {
                    throw new InvalidOperationException("this object is not format 1");
                }

                return new PairPosFormat1_val(m_offsetPairPos, m_bufTable);
            }

            public PairPosFormat2_val GetPairPosFormat2_val()
            {
                if (PosFormat != 2)
                {
                    throw new InvalidOperationException("this object is not format 2");
                }

                return new PairPosFormat2_val(m_offsetPairPos, m_bufTable);
            }
        }


        // Lookup Type 3: Cursive Attachment Positioning Subtable
        
        public class CursivePos_val : CursivePos, I_OTLValidate
        {
            public CursivePos_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(CursivePos)";

                // check for data overlap
                bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetCursivePos, CalcLength(), v, sIdentity, table.GetTag());

                // check the PosFormat field
                if (PosFormat != 1)
                {
                    v.Error(T.T_NULL, E.GPOS_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }
                
                // check the coverage offset
                if (m_offsetCursivePos + Coverage > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                    bRet = false;
                }

                // check the coverage table
                CoverageTable_val ct = GetCoverageTable_val();
                bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                // check that the EntryExitRecord array doesn't extend past end of table
                if (m_offsetCursivePos + (uint)FieldOffsets.EntryExitRecordArray + EntryExitCount*4 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", EntryExit");
                    bRet = false;
                }

                // check each EntryExitRecord
                for (uint i=0; i<EntryExitCount; i++)
                {
                    EntryExitRecord_val eer = GetEntryExitRecord_val(i);
                    bRet &= eer.Validate(v, sIdentity + ", EntryExit[" + i + "]", table);
                }

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GPOS_P_CursivePos, table.m_tag, sIdentity);
                }

                return bRet;
            }


            // nested classes

            public class EntryExitRecord_val : EntryExitRecord, I_OTLValidate
            {
                public EntryExitRecord_val(uint offset, MBOBuffer bufTable, uint offsetCursivePos)
                    : base(offset, bufTable, offsetCursivePos)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    if (EntryAnchorOffset != 0)
                    {
                        if (m_offsetCursivePos + EntryAnchorOffset > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", EntryAnchorOffset");
                            bRet = false;
                        }
                        AnchorTable_val at = GetEntryAnchorTable_val();
                        at.Validate(v, sIdentity + ", EntryAnchor", table);
                    }

                    if (ExitAnchorOffset != 0)
                    {
                        if (m_offsetCursivePos + ExitAnchorOffset > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", ExitAnchorOffset");
                            bRet = false;
                        }
                        AnchorTable_val at = GetExitAnchorTable_val();
                        at.Validate(v, sIdentity + ", ExitAnchor", table);
                    }

                    // way too many EntryExit records to justify this pass message
                    //if (bRet)
                    //{
                    //    v.Pass(T.T_NULL, P.GPOS_P_EntryExit, table.m_tag, sIdentity);
                    //}

                    return bRet;
                }


                public AnchorTable_val GetEntryAnchorTable_val()
                {
                    AnchorTable_val at = null;

                    if (EntryAnchorOffset != 0)
                    {
                        uint offset = m_offsetCursivePos + EntryAnchorOffset;
                        at = new AnchorTable_val(offset, m_bufTable);
                    }

                    return at;
                }

                public AnchorTable_val GetExitAnchorTable_val()
                {
                    AnchorTable_val at = null;

                    if (ExitAnchorOffset != 0)
                    {
                        uint offset = m_offsetCursivePos + ExitAnchorOffset;
                        at = new AnchorTable_val(offset, m_bufTable);
                    }

                    return at;
                }
            }
            
            

            public EntryExitRecord_val GetEntryExitRecord_val(uint i)
            {
                EntryExitRecord_val eer = null;

                if (i < EntryExitCount)
                {
                    ushort sizeofEntryExitRecord = 4;
                    uint offset = m_offsetCursivePos + (uint)FieldOffsets.EntryExitRecordArray + i*sizeofEntryExitRecord;
                    eer = new EntryExitRecord_val(offset, m_bufTable, m_offsetCursivePos);
                }

                return eer;
            }

            public CoverageTable_val GetCoverageTable_val()
            {
                return new CoverageTable_val(m_offsetCursivePos + Coverage, m_bufTable);
            }
        }

        
        // Lookup Type 4: MarkToBase Attachment Positioning Subtable
        
        public class MarkBasePos_val : MarkBasePos, I_OTLValidate
        {
            public MarkBasePos_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(MarkBasePos)";

                // check for data overlap
                bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetMarkBasePos, CalcLength(), v, sIdentity, table.GetTag());

                // check the PosFormat field
                if (PosFormat != 1)
                {
                    v.Error(T.T_NULL, E.GPOS_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }
                
                // check the MarkCoverage offset
                if (m_offsetMarkBasePos + MarkCoverageOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", MarkCoverage offset");
                    bRet = false;
                }

                // check the MarkCoverage table
                CoverageTable_val mct = GetMarkCoverageTable_val();
                bRet &= mct.Validate(v, sIdentity + ", MarkCoverage", table);

                // check the BaseCoverage offset
                if (m_offsetMarkBasePos + BaseCoverageOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", BaseCoverage offset");
                    bRet = false;
                }

                // check the BaseCoverage table
                CoverageTable_val bct = GetBaseCoverageTable_val();
                bRet &= bct.Validate(v, sIdentity + ", BaseCoverage", table);

                // check the MarkArray offset
                if (m_offsetMarkBasePos + MarkArrayOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", MarkArray offset");
                    bRet = false;
                }

                // check the MarkArray table
                MarkArray_val ma = GetMarkArrayTable_val();
                bRet &= ma.Validate(v, sIdentity + ", MarkArray", table);

                // check the BaseArray offset
                if (m_offsetMarkBasePos + BaseArrayOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", BaseArray offset");
                    bRet = false;
                }

                // validate the BaseArray table
                BaseArrayTable_val bat = GetBaseArrayTable_val();
                bRet &= bat.Validate(v, sIdentity + ", BaseArray", table);


                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GPOS_P_MarkBasePos, table.m_tag, sIdentity);
                }

                return bRet;
            }


            // nested classes

            public class BaseArrayTable_val : BaseArrayTable, I_OTLValidate
            {
                public BaseArrayTable_val(uint offset, MBOBuffer bufTable, ushort ClassCount)
                    : base(offset, bufTable, ClassCount)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetBaseArrayTable, CalcLength(), v, sIdentity, table.GetTag());

                    // check that the BaseRecord array doesn't extend past end of table
                    ushort sizeofBaseRecord = (ushort)(2 * m_ClassCount);
                    if (m_offsetBaseArrayTable + (uint)FieldOffsets.BaseRecordArray + BaseCount*sizeofBaseRecord > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", BaseRecord");
                        bRet = false;
                    }

                    // check each BaseRecord
                    for (uint i=0; i<BaseCount; i++)
                    {
                        BaseRecord_val br = GetBaseRecord_val(i);
                        bRet &= br.Validate(v, sIdentity + ", BaseRecord[" + i + "]", table);
                    }

                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_BaseArrayTable, table.m_tag, sIdentity);
                    }

                    return bRet;
                }
                
                public BaseRecord_val GetBaseRecord_val(uint i)
                {
                    BaseRecord_val br = null;

                    if (i < BaseCount)
                    {
                        ushort sizeofBaseRecord = (ushort)(2 * m_ClassCount);
                        uint offset = m_offsetBaseArrayTable + (uint)FieldOffsets.BaseRecordArray + i*sizeofBaseRecord;
                        br = new BaseRecord_val(offset, m_bufTable, m_offsetBaseArrayTable, m_ClassCount);
                    }

                    return br;
                }
            }

            public class BaseRecord_val : BaseRecord, I_OTLValidate
            {
                public BaseRecord_val(uint offset, MBOBuffer bufTable, uint offsetBaseArrayTable, ushort ClassCount)
                    : base(offset, bufTable, offsetBaseArrayTable, ClassCount)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check that the BaseAnchor array doesn't extend past the end of the table
                    if (m_offsetBaseRecord + m_ClassCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", BaseAnchor");
                        bRet = false;
                    }

                    // check that the each BaseAnchor offset points within the table
                    for (uint i=0; i<m_ClassCount; i++)
                    {
                        if (m_offsetBaseArrayTable + GetBaseAnchorOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", BaseAnchor[" + i + "]");
                            bRet = false;
                        }
                    }

                    // validate each BaseAnchor table
                    for (uint i=0; i<m_ClassCount; i++)
                    {
                        AnchorTable_val at = GetBaseAnchorTable_val(i);
                        bRet &= at.Validate(v, sIdentity + ", BaseAnchor[" + i + "]", table);
                    }

                    // way too many base records to justify this pass message
                    //if (bRet)
                    //{
                    //    v.Pass(T.T_NULL, P.GPOS_P_BaseRecord, table.m_tag, sIdentity);
                    //}

                    return bRet;
                }
                
                public AnchorTable_val GetBaseAnchorTable_val(uint i)
                {
                    return new AnchorTable_val(m_offsetBaseArrayTable + GetBaseAnchorOffset(i), m_bufTable);
                }
            }

            // accessors

            public CoverageTable_val GetMarkCoverageTable_val()
            {
                uint offset = m_offsetMarkBasePos + MarkCoverageOffset;
                return new CoverageTable_val(offset, m_bufTable);
            }

            public CoverageTable_val GetBaseCoverageTable_val()
            {
                uint offset = m_offsetMarkBasePos + BaseCoverageOffset;
                return new CoverageTable_val(offset, m_bufTable);
            }

            public MarkArray_val GetMarkArrayTable_val()
            {
                uint offset = m_offsetMarkBasePos + MarkArrayOffset;
                return new MarkArray_val(offset, m_bufTable);
            }

            public BaseArrayTable_val GetBaseArrayTable_val()
            {
                uint offset = m_offsetMarkBasePos + BaseArrayOffset;
                return new BaseArrayTable_val(offset, m_bufTable, ClassCount);
            }
        }


        // Lookup Type 5: MarkToLigature Attachment Positioning Subtable
        
        public class MarkLigPos_val : MarkLigPos, I_OTLValidate
        {
            public MarkLigPos_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(MarkLigPos)";

                // check for data overlap
                bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetMarkLigPos, CalcLength(), v, sIdentity, table.GetTag());

                // check the PosFormat field
                if (PosFormat != 1)
                {
                    v.Error(T.T_NULL, E.GPOS_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }
                
                // check the MarkCoverage offset
                if (m_offsetMarkLigPos + MarkCoverageOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", MarkCoverage offset");
                    bRet = false;
                }

                // check the MarkCoverage table
                CoverageTable_val mct = GetMarkCoverageTable_val();
                bRet &= mct.Validate(v, sIdentity + ", MarkCoverage", table);

                // check the LigatureCoverage offset
                if (m_offsetMarkLigPos + LigatureCoverageOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", LigatureCoverage offset");
                    bRet = false;
                }

                // check the LigatureCoverage table
                CoverageTable_val lct = GetLigatureCoverageTable_val();
                bRet &= lct.Validate(v, sIdentity + ", LigatureCoverage", table);

                // check the MarkArray offset
                if (m_offsetMarkLigPos + MarkArrayOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", MarkArray offset");
                    bRet = false;
                }

                // check the MarkArray table
                MarkArray_val ma = GetMarkArrayTable_val();
                bRet &= ma.Validate(v, sIdentity + ", MarkArray", table);

                // check the LigatureArray offset
                if (m_offsetMarkLigPos + LigatureArrayOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", LigatureArray offset");
                    bRet = false;
                }

                // validate the LigatureArray table
                LigatureArray_val la = GetLigatureArrayTable_val();
                bRet &= la.Validate(v, sIdentity + ", LigatureArray", table);

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GPOS_P_MarkLigPos, table.m_tag, sIdentity);
                }


                return bRet;
            }


            // nested classes

            public class LigatureArray_val : LigatureArray, I_OTLValidate
            {
                public LigatureArray_val(uint offset, MBOBuffer bufTable, ushort ClassCount)
                    : base(offset, bufTable, ClassCount)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetLigatureArray, CalcLength(), v, sIdentity, table.GetTag());

                    // check that the LigatureAttach array doesn't extend past the end of the table
                    if (m_offsetLigatureArray + (uint)FieldOffsets.LigatureAttachOffsets + LigatureCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", LigatureAttach");
                        bRet = false;
                    }

                    // check each LigatureAttach offset
                    for (uint i=0; i<LigatureCount; i++)
                    {
                        if (m_offsetLigatureArray + GetLigatureAttachOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", LigatureAttach[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each LigatureAttach table
                    for (uint i=0; i<LigatureCount; i++)
                    {
                        LigatureAttach_val la = GetLigatureAttachTable_val(i);
                        bRet &= la.Validate(v, sIdentity + ", LigatureAttach[" + i + "]", table);
                    }

                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_LigatureArrayTable, table.m_tag, sIdentity);
                    }

                    return bRet;
                }


                public LigatureAttach_val GetLigatureAttachTable_val(uint i)
                {
                    LigatureAttach_val la = null;

                    if (i < LigatureCount)
                    {
                        uint offset = m_offsetLigatureArray + GetLigatureAttachOffset(i);
                        la = new LigatureAttach_val(offset, m_bufTable, m_ClassCount);
                    }

                    return la;
                }
            }

            public class LigatureAttach_val : LigatureAttach, I_OTLValidate
            {
                public LigatureAttach_val(uint offset, MBOBuffer bufTable, ushort ClassCount)
                    : base(offset, bufTable, ClassCount)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetLigatureAttach, CalcLength(), v, sIdentity, table.GetTag());

                    // check that the ComponentRecord array doesn't extend past the end of the table
                    ushort sizeofComponentRecord = (ushort)(2 * m_ClassCount);
                    if (m_offsetLigatureAttach + (uint)FieldOffsets.ComponentRecordArray + ComponentCount*sizeofComponentRecord > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", ComponentRecord");
                        bRet = false;
                    }

                    // validate each ComponentRecord
                    for (uint i=0; i<ComponentCount; i++)
                    {
                        ComponentRecord_val cr = GetComponentRecord_val(i);
                        bRet &= cr.Validate(v, sIdentity + ", ComponentRecord[" + i + "]", table);
                    }

                    // way too many LigatureAttach tables to justify this pass message
                    //if (bRet)
                    //{
                    //    v.Pass(T.T_NULL, P.GPOS_P_LigatureAttachTable, table.m_tag, sIdentity);
                    //}

                    return bRet;
                }
                
                public ComponentRecord_val GetComponentRecord_val(uint i)
                {
                    ComponentRecord_val cr = null;

                    if (i < ComponentCount)
                    {
                        ushort sizeofComponentRecord = (ushort)(2 * m_ClassCount);
                        uint offset = m_offsetLigatureAttach + (uint)FieldOffsets.ComponentRecordArray + i*sizeofComponentRecord;
                        cr = new ComponentRecord_val(offset, m_bufTable, m_offsetLigatureAttach, m_ClassCount);
                    }

                    return cr;
                }
            }

            public class ComponentRecord_val : ComponentRecord, I_OTLValidate
            {
                public ComponentRecord_val(uint offset, MBOBuffer bufTable, uint offsetLigatureAttach, ushort ClassCount)
                    : base(offset, bufTable, offsetLigatureAttach, ClassCount)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check the offsets
                    for (uint i=0; i<m_ClassCount; i++)
                    {
                        uint offset = GetLigatureAnchorOffset(i);
                        if (offset != 0)
                        {
                            if (m_offsetLigatureAttach + offset > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", LigatureAnchor[" + i + "]");
                                bRet = false;
                            }
                        }
                    }

                    // check each anchor table
                    for (uint i=0; i<m_ClassCount; i++)
                    {
                        if (GetLigatureAnchorOffset(i) != 0)
                        {
                            AnchorTable_val at = GetLigatureAnchorTable_val(i);
                            bRet &= at.Validate(v, sIdentity + ", LigatureAnchor[" + i + "]", table);
                        }
                    }

                    // way too many component records to justify this pass message
                    //if (bRet)
                    //{
                    //    v.Pass(T.T_NULL, P.GPOS_P_ComponentRecord, table.m_tag, sIdentity);
                    //}

                    return bRet;
                }
                
                public AnchorTable_val GetLigatureAnchorTable_val(uint i)
                {
                    AnchorTable_val at = null;
                    uint offset = GetLigatureAnchorOffset(i);
                    if (offset != 0)
                    {
                        at = new AnchorTable_val(m_offsetLigatureAttach + offset, m_bufTable);
                    }
                    return at;
                }
            }

            public CoverageTable_val GetMarkCoverageTable_val()
            {
                uint offset = m_offsetMarkLigPos + MarkCoverageOffset;
                return new CoverageTable_val(offset, m_bufTable);
            }

            public CoverageTable_val GetLigatureCoverageTable_val()
            {
                uint offset = m_offsetMarkLigPos + LigatureCoverageOffset;
                return new CoverageTable_val(offset, m_bufTable);
            }

            public MarkArray_val GetMarkArrayTable_val()
            {
                uint offset = m_offsetMarkLigPos + MarkArrayOffset;
                return new MarkArray_val(offset, m_bufTable);
            }

            public LigatureArray_val GetLigatureArrayTable_val()
            {
                uint offset = m_offsetMarkLigPos + LigatureArrayOffset;
                return new LigatureArray_val(offset, m_bufTable, ClassCount);
            }
        }


        // Lookup Type 6: MarkToMark Attachment Positioning Subtable
        
        public class MarkMarkPos_val : MarkMarkPos, I_OTLValidate
        {
            public MarkMarkPos_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(MarkMarkPos)";

                // check for data overlap
                bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetMarkMarkPos, CalcLength(), v, sIdentity, table.GetTag());

                // check the PosFormat field
                if (PosFormat != 1)
                {
                    v.Error(T.T_NULL, E.GPOS_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }
                
                // check the Mark1Coverage offset
                if (m_offsetMarkMarkPos + Mark1CoverageOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", Mark1Coverage offset");
                    bRet = false;
                }

                // check the Mark1Coverage table
                CoverageTable_val m1ct = GetMark1CoverageTable_val();
                bRet &= m1ct.Validate(v, sIdentity + ", Mark1Coverage", table);

                // check the Mark2Coverage offset
                if (m_offsetMarkMarkPos + Mark2CoverageOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", Mark2Coverage offset");
                    bRet = false;
                }

                // check the Mark2Coverage table
                CoverageTable_val m2ct = GetMark2CoverageTable_val();
                bRet &= m2ct.Validate(v, sIdentity + ", Mark2Coverage", table);

                // check the Mark1Array offset
                if (m_offsetMarkMarkPos + Mark1ArrayOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", Mark1Array offset");
                    bRet = false;
                }

                // check the Mark1Array table
                MarkArray_val m1a = GetMark1ArrayTable_val();
                bRet &= m1a.Validate(v, sIdentity + ", Mark1Array", table);

                // check the Mark2Array offset
                if (m_offsetMarkMarkPos + Mark2ArrayOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", Mark1Array offset");
                    bRet = false;
                }

                // check the Mark2Array table
                Mark2Array_val m2a = GetMark2ArrayTable_val();
                bRet &= m2a.Validate(v, sIdentity + ", Mark2Array", table);

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GPOS_P_MarkMarkPos, table.m_tag, sIdentity);
                }

                return bRet;
            }

            
            // nested classes

            public class Mark2Array_val : Mark2Array, I_OTLValidate
            {
                public Mark2Array_val(uint offset, MBOBuffer bufTable, ushort ClassCount)
                    : base(offset, bufTable, ClassCount)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetMark2Array, CalcLength(), v, sIdentity, table.GetTag());

                    // check that the Mark2Record array doesn't extend past the end of the table
                    ushort sizeofMark2Record = (ushort)(2 * m_ClassCount);
                    if (m_offsetMark2Array + (uint)FieldOffsets.Mark2RecordArray + Mark2Count*sizeofMark2Record > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", Mark2Record");
                        bRet = false;
                    }

                    // validate each Mark2Record
                    for (uint i=0; i<Mark2Count; i++)
                    {
                        Mark2Record_val m2r = GetMark2Record_val(i);
                        bRet &= m2r.Validate(v, sIdentity + ", Mark2Record[" + i + "]", table);
                    }

                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_Mark2ArrayTable, table.m_tag, sIdentity);
                    }

                    return bRet;
                }
                
                public Mark2Record_val GetMark2Record_val(uint i)
                {
                    Mark2Record_val m2r = null;

                    if (i < Mark2Count)
                    {
                        ushort sizeofMark2Record = (ushort)(2 * m_ClassCount);
                        uint offset = m_offsetMark2Array + (uint)FieldOffsets.Mark2RecordArray + i*sizeofMark2Record;
                        m2r = new Mark2Record_val(offset, m_bufTable, m_offsetMark2Array, m_ClassCount);
                    }

                    return m2r;
                }
            }

            public class Mark2Record_val : Mark2Record, I_OTLValidate
            {
                public Mark2Record_val(uint offset, MBOBuffer bufTable, uint offsetMark2Array, ushort ClassCount)
                    : base (offset, bufTable, offsetMark2Array, ClassCount)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check that the Mark2Anchor array doesn't extend past the end of the table
                    if (m_offsetMark2Record + m_ClassCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", Mark2Anchor");
                        bRet = false;
                    }

                    // check each Mark2Anchor array offset
                    for (uint i=0; i<m_ClassCount; i++)
                    {
                        if (m_offsetMark2Array + GetMark2AnchorOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", Mark2Anchor[" + i + "]");
                            bRet = false;
                        }
                    }

                    // validate each Anchor table
                    for (uint i=0; i<m_ClassCount; i++)
                    {
                        AnchorTable_val m2at = GetMark2AnchorTable_val(i);
                        bRet &= m2at.Validate(v, sIdentity + ", Mark2Anchor[" + i + "]", table);
                    }

                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_Mark2Record, table.m_tag, sIdentity);
                    }

                    return bRet;
                }
                
                public AnchorTable_val GetMark2AnchorTable_val(uint i)
                {
                    return new AnchorTable_val(m_offsetMark2Array + GetMark2AnchorOffset(i), m_bufTable);
                }
            }


            public CoverageTable_val GetMark1CoverageTable_val()
            {
                return new CoverageTable_val(m_offsetMarkMarkPos + Mark1CoverageOffset, m_bufTable);
            }

            public CoverageTable_val GetMark2CoverageTable_val()
            {
                return new CoverageTable_val(m_offsetMarkMarkPos + Mark2CoverageOffset, m_bufTable);
            }

            public MarkArray_val GetMark1ArrayTable_val()
            {
                return new MarkArray_val(m_offsetMarkMarkPos + Mark1ArrayOffset, m_bufTable);
            }

            public Mark2Array_val GetMark2ArrayTable_val()
            {
                return new Mark2Array_val(m_offsetMarkMarkPos + Mark2ArrayOffset, m_bufTable, ClassCount);
            }
        }


        // Lookup Type 7: Contextual Positioning Subtable
        
        public class ContextPos_val : ContextPos, I_OTLValidate
        {
            public ContextPos_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(ContextPos, fmt " + PosFormat + ")";


                if (PosFormat == 1)
                {
                    ContextPosFormat1_val f1 = GetContextPosFormat1_val();
                    f1.Validate(v, sIdentity, table);
                }
                else if (PosFormat == 2)
                {
                    ContextPosFormat2_val f2 = GetContextPosFormat2_val();
                    f2.Validate(v, sIdentity, table);
                }
                else if (PosFormat == 3)
                {
                    ContextPosFormat3_val f3 = GetContextPosFormat3_val();
                    f3.Validate(v, sIdentity, table);
                }
                else
                {
                    v.Error(T.T_NULL, E.GPOS_E_SubtableFormat, table.m_tag, sIdentity + ", PosFormat = " + PosFormat);
                    bRet = false;
                }


                return bRet;
            }

            
            // nested classes

            public class ContextPosFormat1_val : ContextPosFormat1, I_OTLValidate
            {
                public ContextPosFormat1_val(uint offset, MBOBuffer bufTable)
                    : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetContextPos, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetContextPos + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", coverage", table);

                    // check the PosRuleSet array length
                    if (m_offsetContextPos + (uint)FieldOffsets.PosRuleSetOffsets + PosRuleSetCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PosRuleSet array");
                        bRet = false;
                    }

                    // check each PosRuleSet offset
                    for (uint i=0; i<PosRuleSetCount; i++)
                    {
                        if (m_offsetContextPos + GetPosRuleSetOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", PosRuleSet[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each PosRuleSet table
                    for (uint i=0; i<PosRuleSetCount; i++)
                    {
                        PosRuleSet_val prs = GetPosRuleSet_val(i);
                        bRet &= prs.Validate(v, sIdentity + ", PosRuleSet[" + i + "]", table);
                    }

                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_ContextPos, table.m_tag, sIdentity);
                    }

                    return bRet;
                }

                // nested classes

                public class PosRuleSet_val : PosRuleSet, I_OTLValidate
                {
                    public PosRuleSet_val(uint offset, MBOBuffer bufTable)
                        : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetPosRuleSet, CalcLength(), v, sIdentity, table.GetTag());

                        // check the PosRule array length
                        if (m_offsetPosRuleSet + (uint)FieldOffsets.PosRuleOffsets + PosRuleCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PosRule array");
                            bRet = false;
                        }

                        // check the PosRule array offsets
                        for (uint i=0; i< PosRuleCount; i++)
                        {
                            if (m_offsetPosRuleSet + GetPosRuleOffset(i) > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", PosRule[" + i + "]");
                                bRet = false;
                            }
                        }

                        // check each PosRule table
                        for (uint i=0; i< PosRuleCount; i++)
                        {
                            PosRule_val pr = GetPosRuleTable_val(i);
                            bRet &= pr.Validate(v, sIdentity + ", PosRule[" + i + "]", table);
                        }

                        if (bRet)
                        {
                            v.Pass(T.T_NULL, P.GPOS_P_PosRuleSet, table.m_tag, sIdentity);
                        }

                        return bRet;
                    }

                    public PosRule_val GetPosRuleTable_val(uint i)
                    {
                        PosRule_val pr = null;

                        if (i < PosRuleCount)
                        {
                            uint offset = m_offsetPosRuleSet + GetPosRuleOffset(i);
                            pr = new PosRule_val(offset, m_bufTable);
                        }

                        return pr;
                    }
                }

                public class PosRule_val : PosRule, I_OTLValidate
                {
                    public PosRule_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetPosRule, CalcLength(), v, sIdentity, table.GetTag());

                        // check the Input array length
                        if (m_offsetPosRule + (uint)FieldOffsets.InputGlyphIDs + (GlyphCount-1)*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", Input");
                            bRet = false;
                        }

                        // check the PosLookupRecord array length
                        uint offsetPosLookupRecord = (uint)FieldOffsets.InputGlyphIDs + ((uint)GlyphCount-1)*2;
                        if (m_offsetPosRule + offsetPosLookupRecord +  PosCount*4 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PosLookupRecord array");
                            bRet = false;
                        }

                        // validate each PosLookupRecord
                        for (uint i=0; i<PosCount; i++)
                        {
                            // check the SequenceIndex
                            PosLookupRecord plr = GetPosLookupRecord(i);
                            if (plr.SequenceIndex > GlyphCount)
                            {
                                v.Error(T.T_NULL, E.GPOS_E_PosLookupRecord_SequenceIndex, table.m_tag, sIdentity + ", PosLookupRecord[" + i + "]");
                                bRet = false;
                            }
                        }                        

                        if (bRet)
                        {
                            v.Pass(T.T_NULL, P.GPOS_P_PosRule, table.m_tag, sIdentity);
                        }

                        return bRet;
                    }
                }


                public PosRuleSet_val GetPosRuleSet_val(uint i)
                {

                    PosRuleSet_val prs = null;

                    if (i < PosRuleSetCount)
                    {
                        uint offset = m_offsetContextPos + GetPosRuleSetOffset(i);
                        prs = new PosRuleSet_val(offset, m_bufTable);
                    }

                    return prs;
                }

                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetContextPos + CoverageOffset, m_bufTable);
                }
            }

            public class ContextPosFormat2_val : ContextPosFormat2, I_OTLValidate
            {
                public ContextPosFormat2_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetContextPos, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetContextPos + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", coverage", table);

                    // check the ClassDef offset
                    if (m_offsetContextPos + ClassDefOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", ClassDef offset");
                        bRet = false;
                    }

                    // check the ClassDef table
                    ClassDefTable_val cdt = GetClassDefTable_val();
                    bRet &= cdt.Validate(v, sIdentity + ", ClassDef", table);

                    // Check the PosClassSet array length
                    if (m_offsetContextPos + (uint)FieldOffsets.PosClassSetOffsets + PosClassSetCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PosClassSet array");
                        bRet = false;
                    }

                    // Check the PosClassSet offsets
                    for (uint i=0; i<PosClassSetCount; i++)
                    {
                        if (m_offsetContextPos + GetPosClassSetOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", PosClassSet[" + i + "]");
                            bRet = false;
                        }
                    }

                    // Check each PosClassSet table
                    for (uint i=0; i<PosClassSetCount; i++)
                    {
                        PosClassSet_val pcs = GetPosClassSetTable_val(i);
                        bRet &= pcs.Validate(v, sIdentity + ", PosClassSet[" + i + "]", table);
                    }

                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_ContextPos, table.m_tag, sIdentity);
                    }

                    return bRet;
                }

                // nested classes

                public class PosClassSet_val : PosClassSet, I_OTLValidate
                {
                    public PosClassSet_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetPosClassSet, CalcLength(), v, sIdentity, table.GetTag());

                        // check the PosClassRule array length
                        if (m_offsetPosClassSet + (uint)FieldOffsets.PosClassRuleOffsets + PosClassRuleCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PosClassRule array");
                            bRet = false;
                        }

                        // check each PosClassRule offset
                        for (uint i=0; i<PosClassRuleCount; i++)
                        {
                            if (m_offsetPosClassSet + GetPosClassRuleOffset(i) > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", PosClassRule[" + i + "]");
                                bRet = false;
                            }
                        }

                        // check each PosClassRule table
                        for (uint i=0; i<PosClassRuleCount; i++)
                        {
                            PosClassRule_val pcr = GetPosClassRuleTable_val(i);
                            pcr.Validate(v, sIdentity + ", PosClassRule[" + i + "]", table);
                        }

                        if (bRet)
                        {
                            v.Pass(T.T_NULL, P.GPOS_P_PosClassSet, table.m_tag, sIdentity);
                        }

                        return bRet;
                    }

                    public PosClassRule_val GetPosClassRuleTable_val(uint i)
                    {
                        PosClassRule_val pcr = null;

                        if (i < PosClassRuleCount)
                        {
                            uint offset = m_offsetPosClassSet + GetPosClassRuleOffset(i);
                            pcr = new PosClassRule_val(offset, m_bufTable);
                        }

                        return pcr;
                    }
                }

                public class PosClassRule_val : PosClassRule, I_OTLValidate
                {
                    public PosClassRule_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetPosClassRule, CalcLength(), v, sIdentity, table.GetTag());

                        // check the Class array length
                        if (m_offsetPosClassRule + (uint)FieldOffsets.Classes + (GlyphCount-1)*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", Class array");
                            bRet = false;
                        }

                        // check the PosLookupRecord array length
                        uint offsetPosLookupRecord = (uint)FieldOffsets.Classes + ((uint)GlyphCount-1)*2;
                        if (m_offsetPosClassRule + offsetPosLookupRecord +  PosCount*4 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PosLookupRecord array");
                            bRet = false;
                        }

                        // validate each PosLookupRecord
                        for (uint i=0; i<PosCount; i++)
                        {
                            // check the SequenceIndex
                            PosLookupRecord plr = GetPosLookupRecord(i);
                            if (plr.SequenceIndex > GlyphCount)
                            {
                                v.Error(T.T_NULL, E.GPOS_E_PosLookupRecord_SequenceIndex, table.m_tag, sIdentity + ", PosLookupRecord[" + i + "]");
                                bRet = false;
                            }
                        }                        

                        if (bRet)
                        {
                            v.Pass(T.T_NULL, P.GPOS_P_PosRule, table.m_tag, sIdentity);
                        }

                        return bRet;
                    }

                }

                public ClassDefTable_val GetClassDefTable_val()
                {
                    return new ClassDefTable_val(m_offsetContextPos + ClassDefOffset, m_bufTable);
                }

                public PosClassSet_val GetPosClassSetTable_val(uint i)
                {

                    PosClassSet_val pcs = null;

                    if (i < PosClassSetCount)
                    {
                        pcs = new PosClassSet_val(m_offsetContextPos + GetPosClassSetOffset(i), m_bufTable);
                    }

                    return pcs;
                }

                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetContextPos + CoverageOffset, m_bufTable);
                }
            }

            public class ContextPosFormat3_val : ContextPosFormat3, I_OTLValidate
            {
                public ContextPosFormat3_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetContextPos, CalcLength(), v, sIdentity, table.GetTag());

                    // check the Coverage array length
                    if (m_offsetContextPos + (uint)FieldOffsets.CoverageOffsets + GlyphCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", Coverage array");
                        bRet = false;
                    }

                    // check each coverage offset
                    for (uint i=0; i<GlyphCount; i++)
                    {
                        if (m_offsetContextPos + GetCoverageOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each coverage table
                    for (uint i=0; i<GlyphCount; i++)
                    {
                        CoverageTable_val ct = GetCoverageTable_val(i);
                        bRet &= ct.Validate(v, sIdentity + ", Coverage[" + i + "]", table);
                    }

                    // check the PosLookupRecord array length
                    uint offsetPosLookupRecords = (uint)FieldOffsets.CoverageOffsets + (uint)GlyphCount*2;
                    if (m_offsetContextPos + offsetPosLookupRecords + PosCount*4 > m_bufTable.GetLength())
                    {                        
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PosLookupRecord array");
                        bRet = false;
                    }

                    // check each PosLookupRecord
                    for (uint i=0; i<PosCount; i++)
                    {
                        // check the SequenceIndex
                        PosLookupRecord plr = GetPosLookupRecord(i);
                        if (plr.SequenceIndex > GlyphCount)
                        {
                            v.Error(T.T_NULL, E.GPOS_E_PosLookupRecord_SequenceIndex, table.m_tag, sIdentity + ", PosLookupRecord[" + i + "]");
                            bRet = false;
                        }
                    }
                    
                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_ContextPos, table.m_tag, sIdentity);
                    }

                    return bRet;
                }

                // accessors

                public CoverageTable_val GetCoverageTable_val(uint i)
                {
                    return new CoverageTable_val(m_offsetContextPos + GetCoverageOffset(i), m_bufTable);
                }

            }


            public ContextPosFormat1_val GetContextPosFormat1_val()
            {
                if (PosFormat != 1)
                {
                    throw new InvalidOperationException("format not 1");
                }

                return new ContextPosFormat1_val(m_offsetContextPos, m_bufTable);
            }

            public ContextPosFormat2_val GetContextPosFormat2_val()
            {
                if (PosFormat != 2)
                {
                    throw new InvalidOperationException("format not 2");
                }

                return new ContextPosFormat2_val(m_offsetContextPos, m_bufTable);
            }

            public ContextPosFormat3_val GetContextPosFormat3_val()
            {
                if (PosFormat != 3)
                {
                    throw new InvalidOperationException("format not 3");
                }

                return new ContextPosFormat3_val(m_offsetContextPos, m_bufTable);
            }

        }

        // Lookup Type 8: Chaining Contextual Positioning Subtable
        
        public class ChainContextPos_val : ChainContextPos, I_OTLValidate
        {
            public ChainContextPos_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(ChainContextPos, fmt " + PosFormat + ")";


                if (PosFormat == 1)
                {
                    ChainContextPosFormat1_val f1 = GetChainContextPosFormat1_val();
                    f1.Validate(v, sIdentity, table);
                }
                else if (PosFormat == 2)
                {
                    ChainContextPosFormat2_val f2 = GetChainContextPosFormat2_val();
                    f2.Validate(v, sIdentity, table);
                }
                else if (PosFormat == 3)
                {
                    ChainContextPosFormat3_val f3 = GetChainContextPosFormat3_val();
                    f3.Validate(v, sIdentity, table);
                }
                else
                {
                    v.Error(T.T_NULL, E.GPOS_E_SubtableFormat, table.m_tag, sIdentity + ", PosFormat = " + PosFormat);
                    bRet = false;
                }

                return bRet;
            }

            
            // nested classes

            public class ChainContextPosFormat1_val : ChainContextPosFormat1, I_OTLValidate
            {
                public ChainContextPosFormat1_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetChainContextPos, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetChainContextPos + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", coverage", table);


                    // check the ChainPosRuleSet array length
                    if (m_offsetChainContextPos + (uint)FieldOffsets.ChainPosRuleSetOffsets + ChainPosRuleSetCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", ChainPosRuleSet array");
                        bRet = false;
                    }

                    // check each ChainPosRuleSet offset
                    for (uint i=0; i<ChainPosRuleSetCount; i++)
                    {
                        if (m_offsetChainContextPos + GetChainPosRuleSetOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", ChainPosRuleSet[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each ChainPosRuleSet table
                    for (uint i=0; i<ChainPosRuleSetCount; i++)
                    {
                        ChainPosRuleSet_val cprs = GetChainPosRuleSetTable_val(i);
                        bRet &= cprs.Validate(v, sIdentity + ", ChainPosRuleSet[" + i + "]", table);
                    }

                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_ChainContextPos, table.m_tag, sIdentity);
                    }

                    return bRet;
                }

                // nested classes

                public class ChainPosRuleSet_val : ChainPosRuleSet, I_OTLValidate
                {
                    public ChainPosRuleSet_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetChainPosRuleSet, CalcLength(), v, sIdentity, table.GetTag());

                        // check the ChainPosRule array length
                        if (m_offsetChainPosRuleSet + (uint)FieldOffsets.ChainPosRuleOffsets + ChainPosRuleCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", ChainPosRule array");
                            bRet = false;
                        }

                        // check the ChainPosRule offsets
                        for (uint i=0; i<ChainPosRuleCount; i++)
                        {
                            if (m_offsetChainPosRuleSet + GetChainPosRuleOffset(i) > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", ChainPosRule[" + i + "]");
                                bRet = false;
                            }
                        }

                        // check the ChainPosRule tables
                        for (uint i=0; i<ChainPosRuleCount; i++)
                        {
                            ChainPosRule_val cpr = GetChainPosRuleTable_val(i);
                            bRet &= cpr.Validate(v, sIdentity + ", ChainPosRule[" + i + "]", table);
                        }

                        if (bRet)
                        {
                            v.Pass(T.T_NULL, P.GPOS_P_ChainPosRuleSet, table.m_tag, sIdentity);
                        }

                        return bRet;
                    }

                    public ChainPosRule_val GetChainPosRuleTable_val(uint i)
                    {
                        ChainPosRule_val pr = null;

                        if (i < ChainPosRuleCount)
                        {
                            uint offset = m_offsetChainPosRuleSet + GetChainPosRuleOffset(i);
                            pr = new ChainPosRule_val(offset, m_bufTable);
                        }

                        return pr;
                    }
                }

                public class ChainPosRule_val : ChainPosRule, I_OTLValidate
                {
                    public ChainPosRule_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetChainPosRule, CalcLength(), v, sIdentity, table.GetTag());

                        // check the Backtrack array length
                        uint offset = m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphIDs;
                        if (offset + BacktrackGlyphCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", Backtrack array");
                            bRet = false;
                        }

                        // check the Input array length
                        offset = m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphIDs
                            + (uint)BacktrackGlyphCount*2 + 2;
                        if (offset + InputGlyphCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", Input array");
                            bRet = false;
                        }

                        // check the LookAhead array length
                        offset = m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2;
                        if (offset + LookaheadGlyphCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", LookAhead array");
                            bRet = false;
                        }

                        // check the PosLookupRecord length
                        offset = m_offsetChainPosRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                            + (uint)LookaheadGlyphCount*2 + 2;
                        if (offset + PosCount*4 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PosLookupRecord array");
                            bRet = false;
                        }

                        if (bRet)
                        {
                            v.Pass(T.T_NULL, P.GPOS_P_ChainPosRule, table.m_tag, sIdentity);
                        }

                        return bRet;
                    }

                }



                public ChainPosRuleSet_val GetChainPosRuleSetTable_val(uint i)
                {
                    ChainPosRuleSet_val cprs = null;

                    if (i < ChainPosRuleSetCount)
                    {
                        cprs = new ChainPosRuleSet_val(m_offsetChainContextPos + GetChainPosRuleSetOffset(i), m_bufTable);
                    }

                    return cprs;
                }

                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetChainContextPos + CoverageOffset, m_bufTable);
                }
            }

            public class ChainContextPosFormat2_val : ChainContextPosFormat2, I_OTLValidate
            {
                public ChainContextPosFormat2_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetChainContextPos, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetChainContextPos + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", coverage", table);

                    // check the BacktrackClassDef offset
                    if (m_offsetChainContextPos + BacktrackClassDefOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", BacktrackClassDef offset");
                        bRet = false;
                    }

                    // check the BacktrackClassDef table
                    ClassDefTable_val bcd = GetBacktrackClassDefTable_val();
                    bcd.Validate(v, sIdentity + ", BacktrackClassDef", table);

                    // check the InputClassDef offset
                    if (m_offsetChainContextPos + InputClassDefOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", InputClassDef offset");
                        bRet = false;
                    }

                    // check the InputClassDef table
                    ClassDefTable_val icd = GetInputClassDefTable_val();
                    icd.Validate(v, sIdentity + ", InputClassDef", table);

                    // check the LookaheadClassDef offset
                    if (m_offsetChainContextPos + LookaheadClassDefOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", LookaheadClassDef offset");
                        bRet = false;
                    }

                    // check the LookaheadClassDef table
                    ClassDefTable_val lcd = GetLookaheadClassDefTable_val();
                    lcd.Validate(v, sIdentity + ", LookaheadClassDef", table);

                    // check the ChainPosClassSet array length
                    if (m_offsetChainContextPos + (uint)FieldOffsets.ChainPosClassSetOffsets + ChainPosClassSetCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", ChainPosClassSet array");
                        bRet = false;
                    }

                    // check the ChainPosClassSet offsets
                    for (uint i=0; i<ChainPosClassSetCount; i++)
                    {
                        if (m_offsetChainContextPos + GetChainPosClassSetOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", ChainPosClassSet[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check the ChainPosClassSet tables
                    for (uint i=0; i<ChainPosClassSetCount; i++)
                    {
                        if (GetChainPosClassSetOffset(i) != 0)
                        {
                            ChainPosClassSet_val cpcs = GetChainPosClassSetTable_val(i);
                            bRet &= cpcs.Validate(v, sIdentity + ", ChainPosClassSet[" + i + "]", table);
                        }
                    }


                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_ChainContextPos, table.m_tag, sIdentity);
                    }

                    return bRet;
                }

                // nested classes

                public class ChainPosClassSet_val : ChainPosClassSet, I_OTLValidate
                {
                    public ChainPosClassSet_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetChainPosClassSet, CalcLength(), v, sIdentity, table.GetTag());

                        // check the ChainPosClassRule array length
                        if (m_offsetChainPosClassSet + (uint)FieldOffsets.ChainPosClassRuleOffsets + ChainPosClassRuleCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", ChainPosClassRule");
                            bRet = false;
                        }

                        // check each ChainPosClassRule offset
                        for (uint i=0; i<ChainPosClassRuleCount; i++)
                        {
                            if (m_offsetChainPosClassSet + GetChainPosClassRuleOffset(i) > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", ChainPosClassRule[" + i + "]");
                                bRet = false;
                            }
                        }

                        // check each ChainPosClassRule table
                        for (uint i=0; i<ChainPosClassRuleCount; i++)
                        {
                            ChainPosClassRule_val cpcr = GetChainPosClassRuleTable_val(i);
                            bRet &= cpcr.Validate(v, sIdentity + ", ChainPosClassRule[" + i + "]", table);
                        }

                        if (bRet)
                        {
                            v.Pass(T.T_NULL, P.GPOS_P_ChainPosClassSet, table.m_tag, sIdentity);
                        }


                        return bRet;
                    }

                    public ChainPosClassRule_val GetChainPosClassRuleTable_val(uint i)
                    {
                        ChainPosClassRule_val cpcr = null;

                        if (i < ChainPosClassRuleCount)
                        {
                            uint offset = m_offsetChainPosClassSet + GetChainPosClassRuleOffset(i);
                            cpcr = new ChainPosClassRule_val(offset, m_bufTable);
                        }

                        return cpcr;
                    }
                }

                public class ChainPosClassRule_val : ChainPosClassRule, I_OTLValidate
                {
                    public ChainPosClassRule_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetChainPosClassRule, CalcLength(), v, sIdentity, table.GetTag());

                        // check the Backtrack array length
                        uint offset = m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackClasses;
                        if (offset + BacktrackGlyphCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", Backtrack");
                            bRet = false;
                        }

                        // check the Input array length
                        offset = m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackClasses 
                            + (uint)BacktrackGlyphCount*2 + 2;
                        if (offset + InputGlyphCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", Input");
                            bRet = false;
                        }

                        // check the Lookahead array length
                        offset = m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackClasses 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2;
                        if (offset + LookaheadGlyphCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", Lookahead");
                            bRet = false;
                        }

                        // check the PosLookupRecord array length
                        offset = m_offsetChainPosClassRule + (uint)FieldOffsets.BacktrackClasses 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                            + (uint)LookaheadGlyphCount*2 + 2;
                        if (offset + PosCount*4 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PosLookupRecord");
                            bRet = false;
                        }

                        if (bRet)
                        {
                            v.Pass(T.T_NULL, P.GPOS_P_ChainPosClassRule, table.m_tag, sIdentity);
                        }

                        return bRet;
                    }
                }

                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetChainContextPos + CoverageOffset, m_bufTable);
                }

                public ClassDefTable_val GetBacktrackClassDefTable_val()
                {
                    uint offset = m_offsetChainContextPos + BacktrackClassDefOffset;
                    return new ClassDefTable_val(offset, m_bufTable);
                }

                public ClassDefTable_val GetInputClassDefTable_val()
                {
                    uint offset = m_offsetChainContextPos + InputClassDefOffset;
                    return new ClassDefTable_val(offset, m_bufTable);
                }

                public ClassDefTable_val GetLookaheadClassDefTable_val()
                {
                    uint offset = m_offsetChainContextPos + LookaheadClassDefOffset;
                    return new ClassDefTable_val(offset, m_bufTable);
                }

                public ChainPosClassSet_val GetChainPosClassSetTable_val(uint i)
                {
                    ChainPosClassSet_val cpcs = null;

                    if (i < ChainPosClassSetCount)
                    {
                        uint offset = GetChainPosClassSetOffset(i);
                        if (offset != 0)
                        {
                            offset += m_offsetChainContextPos;
                            cpcs = new ChainPosClassSet_val(offset, m_bufTable);
                        }
                    }
                    return cpcs;
                }
            }

            public class ChainContextPosFormat3_val : ChainContextPosFormat3, I_OTLValidate
            {
                public ChainContextPosFormat3_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetChainContextPos, CalcLength(), v, sIdentity, table.GetTag());

                    // check the BacktrackCoverage array length
                    uint offset = m_offsetChainContextPos + (uint)FieldOffsets.BacktrackCoverageOffsets;
                    if (offset + BacktrackGlyphCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", BacktrackCoverage");
                        bRet = false;
                    }

                    // check the BacktrackCoverage offsets
                    for (uint i=0; i<BacktrackGlyphCount; i++)
                    {
                        if (m_offsetChainContextPos + GetBacktrackCoverageOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", BacktrackCoverage[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each BacktrackCoverage table
                    for (uint i=0; i<BacktrackGlyphCount; i++)
                    {
                        CoverageTable_val ct = GetBacktrackCoverageTable_val(i);
                        ct.Validate(v, sIdentity + ", BacktrackCoverage[" + i + "]", table);
                    }

                    // check the InputCoverage array length
                    offset = m_offsetChainContextPos + (uint)FieldOffsets.BacktrackCoverageOffsets 
                        + (uint)BacktrackGlyphCount*2 + 2;
                    if (offset + InputGlyphCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", InputCoverage");
                        bRet = false;
                    }

                    // check the InputCoverage offsets
                    for (uint i=0; i<InputGlyphCount; i++)
                    {
                        if (m_offsetChainContextPos + GetInputCoverageOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", InputCoverage[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each InputCoverage table
                    for (uint i=0; i<InputGlyphCount; i++)
                    {
                        CoverageTable_val ct = GetInputCoverageTable_val(i);
                        ct.Validate(v, sIdentity + ", InputCoverage[" + i + "]", table);
                    }

                    // check the LookaheadCoverage array length
                    offset = m_offsetChainContextPos + (uint)FieldOffsets.BacktrackCoverageOffsets 
                        + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2 + 2;
                    if (offset + LookaheadGlyphCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", LookaheadCoverage");
                        bRet = false;
                    }

                    // check the LookaheadCoverage offsets
                    for (uint i=0; i<LookaheadGlyphCount; i++)
                    {
                        if (m_offsetChainContextPos + GetLookaheadCoverageOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", LookaheadCoverage[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each LookaheadCoverage table
                    for (uint i=0; i<LookaheadGlyphCount; i++)
                    {
                        CoverageTable_val ct = GetLookaheadCoverageTable_val(i);
                        ct.Validate(v, sIdentity + ", LookaheadCoverage[" + i + "]", table);
                    }

                    // check the PosLookupRecord array length
                    offset = m_offsetChainContextPos + (uint)FieldOffsets.BacktrackCoverageOffsets 
                        + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2 + 2 
                        + (uint)LookaheadGlyphCount*2 + 2;
                    if (offset + PosCount*4 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GPOS_E_Array_pastEOT, table.m_tag, sIdentity + ", PosLookupRecord");
                        bRet = false;
                    }

                    if (bRet)
                    {
                        v.Pass(T.T_NULL, P.GPOS_P_ChainContextPos, table.m_tag, sIdentity);
                    }

                    return bRet;
                }


                public CoverageTable_val GetBacktrackCoverageTable_val(uint i)
                {
                    CoverageTable_val ct = null;
                    if (i < BacktrackGlyphCount)
                    {
                        uint offset = m_offsetChainContextPos + GetBacktrackCoverageOffset(i);
                        ct = new CoverageTable_val(offset, m_bufTable);
                    }
                    return ct;
                }

                public CoverageTable_val GetInputCoverageTable_val(uint i)
                {
                    CoverageTable_val ct = null;
                    if (i < InputGlyphCount)
                    {
                        uint offset = m_offsetChainContextPos + GetInputCoverageOffset(i);
                        ct = new CoverageTable_val(offset, m_bufTable);
                    }
                    return ct;
                }

                public CoverageTable_val GetLookaheadCoverageTable_val(uint i)
                {
                    CoverageTable_val ct = null;
                    if (i < LookaheadGlyphCount)
                    {
                        uint offset = m_offsetChainContextPos + GetLookaheadCoverageOffset(i);
                        ct = new CoverageTable_val(offset, m_bufTable);
                    }
                    return ct;
                }

            }

            public ChainContextPosFormat1_val GetChainContextPosFormat1_val()
            {
                if (PosFormat != 1)
                {
                    throw new InvalidOperationException("format not 1");
                }
                return new ChainContextPosFormat1_val(m_offsetChainContextPos, m_bufTable);
            }

            public ChainContextPosFormat2_val GetChainContextPosFormat2_val()
            {
                if (PosFormat != 2)
                {
                    throw new InvalidOperationException("format not 2");
                }
                return new ChainContextPosFormat2_val(m_offsetChainContextPos, m_bufTable);
            }

            public ChainContextPosFormat3_val GetChainContextPosFormat3_val()
            {
                if (PosFormat != 3)
                {
                    throw new InvalidOperationException("format not 3");
                }
                return new ChainContextPosFormat3_val(m_offsetChainContextPos, m_bufTable);
            }
        }

        // Lookup Type 9: Extension Positioning Subtable
        
        public class ExtensionPos_val : ExtensionPos, I_OTLValidate
        {
            public ExtensionPos_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(ExtensionPos)";

                // check for data overlap
                bRet &= ((val_GPOS)table).ValidateNoOverlap(m_offsetExtensionPos, CalcLength(), v, sIdentity, table.GetTag());

                // check the PosFormat
                if (PosFormat != 1)
                {
                    v.Error(T.T_NULL, E.GPOS_E_SubtableFormat, table.m_tag, sIdentity + ", PosFormat = " + PosFormat);
                    bRet = false;
                }

                // check the ExtensionLookupType
                if (ExtensionLookupType >= 9)
                {
                    v.Error(T.T_NULL, E.GPOS_E_ExtensionLookupType, table.m_tag, sIdentity + ", ExtensionLookupType = " + ExtensionLookupType);
                    bRet = false;
                }

                // check the ExtensionOffset
                if (m_offsetExtensionPos + ExtensionOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GPOS_E_Offset_PastEOT, table.m_tag, sIdentity + ", ExtensionOffset");
                    bRet = false;
                }

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GPOS_P_ExtensionPos, table.m_tag, sIdentity);
                }

                // validate the subtable
                SubTable st = null;
                switch (ExtensionLookupType)
                {
                    case 1: st = new val_GPOS.SinglePos_val      (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 2: st = new val_GPOS.PairPos_val        (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 3: st = new val_GPOS.CursivePos_val     (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 4: st = new val_GPOS.MarkBasePos_val    (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 5: st = new val_GPOS.MarkLigPos_val     (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 6: st = new val_GPOS.MarkMarkPos_val    (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 7: st = new val_GPOS.ContextPos_val     (m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                    case 8: st = new val_GPOS.ChainContextPos_val(m_offsetExtensionPos + ExtensionOffset, m_bufTable); break;
                }
                if (st != null)
                {
                    I_OTLValidate iv = (I_OTLValidate)st;
                    iv.Validate(v, sIdentity, table);
                }

                return bRet;
            }

        }

        public ScriptListTable_val GetScriptListTable_val()
        {
            return new ScriptListTable_val(ScriptListOffset, m_bufTable);
        }

        public FeatureListTable_val GetFeatureListTable_val()
        {
            return new FeatureListTable_val(FeatureListOffset, m_bufTable);
        }

        public LookupListTable_val GetLookupListTable_val()
        {
            return new LookupListTable_val(LookupListOffset, m_bufTable, m_tag);
        }



        public bool ValidateNoOverlap(uint offset, uint length, Validator v, string sIdentity, OTTag tag)
        {
            bool bValid = m_DataOverlapDetector.CheckForNoOverlap(offset, length);

            if (!bValid)
            {
                v.Error(T.T_NULL, E._Table_E_DataOverlap, tag, sIdentity + ", offset = " + offset + ", length = " + length);
            }

            return bValid;
        }

        protected DataOverlapDetector m_DataOverlapDetector;
    }
}
