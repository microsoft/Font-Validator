using System;

using OTFontFile;
using OTFontFile.OTL;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_BASE.
    /// </summary>
    public class val_BASE : Table_BASE, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_BASE(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
            m_DataOverlapDetector = new DataOverlapDetector();
        }

        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.BASE_Version))
            {
                if (Version.GetUint() == 0x00010000)
                {
                    v.Pass(T.BASE_Version, P.BASE_P_Version, m_tag);
                }
                else
                {
                    v.Error(T.BASE_Version, E.BASE_E_Version, m_tag, "0x"+Version.GetUint().ToString("x8"));
                    bRet = false;
                }
            }

            if (v.PerformTest(T.BASE_HeaderOffsets))
            {
                if (HorizAxisOffset == 0)
                {
                    v.Pass(T.BASE_HeaderOffsets, P.BASE_P_HorizAxisOffset_null, m_tag);
                }
                else if (HorizAxisOffset < GetLength())
                {
                    v.Pass(T.BASE_HeaderOffsets, P.BASE_P_HorizAxisOffset_valid, m_tag);
                }
                else
                {
                    v.Error(T.BASE_HeaderOffsets, E.BASE_E_HorizAxisOffset_OutsideTable, m_tag);
                    bRet = false;
                }

                if (VertAxisOffset == 0)
                {
                    v.Pass(T.BASE_HeaderOffsets, P.BASE_P_VertAxisOffset_null, m_tag);
                }
                else if (VertAxisOffset < GetLength())
                {
                    v.Pass(T.BASE_HeaderOffsets, P.BASE_P_VertAxisOffset_valid, m_tag);
                }
                else
                {
                    v.Error(T.BASE_HeaderOffsets, E.BASE_E_VertAxisOffset_OutsideTable, m_tag);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.BASE_HorizAxisTable))
            {
                AxisTable_val at = GetHorizAxisTable_val();
                if (at != null)
                {
                    at.Validate(v, "H Axis", this);
                }
            }

            if (v.PerformTest(T.BASE_VertAxisTable))
            {
                AxisTable_val at = GetVertAxisTable_val();
                if (at != null)
                {
                    at.Validate(v, "V Axis", this);
                }
            }

            return bRet;
        }


        /************************
         * classes
         */

        public class AxisTable_val : AxisTable, I_OTLValidate
        {
            public AxisTable_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_BASE)table).ValidateNoOverlap(m_offsetAxisTable, CalcLength(), v, sIdentity, table.GetTag());


                if (BaseTagListOffset == 0)
                {
                    v.Pass(T.T_NULL, P.BASE_P_AxisTable_BaseTagListOffset_null, table.m_tag, sIdentity);
                }
                else if (BaseTagListOffset + m_offsetAxisTable > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.BASE_E_AxisTable_BaseTagListOffset, table.m_tag, sIdentity);
                    bRet = false;
                }
                else
                {
                    v.Pass(T.T_NULL, P.BASE_P_AxisTable_BaseTagListOffset_valid, table.m_tag, sIdentity);
                    BaseTagListTable_val btlt = GetBaseTagListTable_val();
                    btlt.Validate(v, sIdentity, table);
                }


                if (BaseScriptListOffset == 0)
                {
                    v.Error(T.T_NULL, E.BASE_E_AxisTable_BaseScriptListOffset_null, table.m_tag, sIdentity);
                    bRet = false;
                }
                else if (BaseScriptListOffset + m_offsetAxisTable > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.BASE_E_AxisTable_BaseScriptListOffset, table.m_tag, sIdentity);
                    bRet = false;
                }
                else
                {
                    v.Pass(T.T_NULL, P.BASE_P_AxisTable_BaseScriptListOffset_valid, table.m_tag, sIdentity);
                    BaseScriptListTable_val bslt = GetBaseScriptListTable_val();
                    bslt.Validate(v, sIdentity, table);
                }

                return bRet;
            }

            public BaseTagListTable_val GetBaseTagListTable_val()
            {
                BaseTagListTable_val btlt = null;
                
                if (BaseTagListOffset != 0)
                {
                    ushort offset = (ushort)(m_offsetAxisTable + BaseTagListOffset);
                    btlt = new BaseTagListTable_val(offset, m_bufTable);
                }

                return btlt;
            }

            public BaseScriptListTable_val GetBaseScriptListTable_val()
            {
                BaseScriptListTable_val bslt = null;
                
                if (BaseScriptListOffset != 0)
                {
                    ushort offset = (ushort)(m_offsetAxisTable + BaseScriptListOffset);
                    bslt = new BaseScriptListTable_val(offset, m_bufTable);
                }

                return bslt;
            }
        }

        public class BaseTagListTable_val : BaseTagListTable, I_OTLValidate
        {
            public BaseTagListTable_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_BASE)table).ValidateNoOverlap(m_offsetBaseTagListTable, CalcLength(), v, sIdentity, table.GetTag());

                bool bTagsValid = true;
                for (uint i=0; i<BaseTagCount; i++)
                {
                    OTTag BaselineTag = GetBaselineTag(i);
                    if (!BaselineTag.IsValid())
                    {
                        v.Error(T.T_NULL, E.BASE_E_BaseTagList_TagValid, table.m_tag, sIdentity + ", tag = 0x" + ((uint)BaselineTag).ToString("x8"));
                        bTagsValid = false;
                        bRet = false;
                    }
                }
                if (bTagsValid)
                {
                    v.Pass(T.T_NULL, P.BASE_P_BaseTagList_TagValid, table.m_tag, sIdentity);
                }

                bool bOrderOk = true;
                if (BaseTagCount > 1)
                {
                    for (uint i=0; i<BaseTagCount-1; i++)
                    {
                        OTTag ThisTag = GetBaselineTag(i);
                        OTTag NextTag = GetBaselineTag(i+1);
                        if (ThisTag >= NextTag)
                        {
                            v.Error(T.T_NULL, E.BASE_E_BaseTagList_TagOrder, table.m_tag, sIdentity);
                            bOrderOk = false;
                            bRet = false;
                            break;
                        }
                    }
                }
                if (bOrderOk)
                {
                    v.Pass(T.T_NULL, P.BASE_P_BaseTagList_TagOrder, table.m_tag, sIdentity);
                }

                return bRet;
            }
        }

        public class BaseScriptListTable_val : BaseScriptListTable, I_OTLValidate
        {
            public BaseScriptListTable_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_BASE)table).ValidateNoOverlap(m_offsetBaseScriptListTable, CalcLength(), v, sIdentity, table.GetTag());

                bool bOrderOk = true;
                if (BaseScriptCount > 1)
                {
                    for (uint i=0; i<BaseScriptCount-1; i++)
                    {
                        BaseScriptRecord ThisBsr = GetBaseScriptRecord(i);
                        BaseScriptRecord NextBsr = GetBaseScriptRecord(i+1);
                        if (ThisBsr.BaseScriptTag >= NextBsr.BaseScriptTag)
                        {
                            v.Error(T.T_NULL, E.BASE_E_BaseScriptList_Order, table.m_tag, sIdentity);
                            bOrderOk = false;
                            bRet = false;
                            break;
                        }
                    }
                }
                if (bOrderOk)
                {
                    v.Pass(T.T_NULL, P.BASE_P_BaseScriptList_Order, table.m_tag, sIdentity);
                }

                bool bOffsetsOk = true;
                for (uint i=0; i<BaseScriptCount; i++)
                {
                    BaseScriptRecord bsr = GetBaseScriptRecord(i);
                    if (bsr.BaseScriptOffset + m_offsetBaseScriptListTable > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.BASE_E_BaseScriptList_Offset, table.m_tag, sIdentity + ", index = "+i);
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }
                if (bOffsetsOk)
                {
                    v.Pass(T.T_NULL, P.BASE_P_BaseScriptList_Offset, table.m_tag, sIdentity);
                }

                for (uint i=0; i<BaseScriptCount; i++)
                {
                    BaseScriptRecord bsr = GetBaseScriptRecord(i);
                    BaseScriptTable_val bst = GetBaseScriptTable_val(bsr);
                    bst.Validate(v, sIdentity + ", BaseScriptRecord[" + i + "](" + bsr.BaseScriptTag + ")", table);
                }


                return bRet;
            }

            public BaseScriptTable_val GetBaseScriptTable_val(BaseScriptRecord bsr)
            {
                BaseScriptTable_val bst = null;
                if (bsr != null)
                {
                    ushort offset = (ushort)(m_offsetBaseScriptListTable + bsr.BaseScriptOffset);
                    bst = new BaseScriptTable_val(offset, m_bufTable);
                }
                return bst;
            }
        }

        public class BaseScriptTable_val : BaseScriptTable, I_OTLValidate
        {
            public BaseScriptTable_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_BASE)table).ValidateNoOverlap(m_offsetBaseScriptTable, CalcLength(), v, sIdentity, table.GetTag());

                // check the BaseValuesOffset
                if (BaseValuesOffset == 0)
                {
                    v.Pass(T.T_NULL, P.BASE_P_BaseValuesOffset_null, table.m_tag, sIdentity);
                }
                else if (BaseValuesOffset + m_offsetBaseScriptTable > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.BASE_E_BaseValuesOffset, table.m_tag, sIdentity);
                }
                else
                {
                    v.Pass(T.T_NULL, P.BASE_P_BaseValuesOffset, table.m_tag, sIdentity);
                }

                // check the DefaultMinMaxOffset
                if (DefaultMinMaxOffset == 0)
                {
                    v.Pass(T.T_NULL, P.BASE_P_DefaultMinMaxOffset_null, table.m_tag, sIdentity);
                }
                else if (DefaultMinMaxOffset + m_offsetBaseScriptTable > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.BASE_E_DefaultMinMaxOffset, table.m_tag, sIdentity);
                }
                else
                {
                    v.Pass(T.T_NULL, P.BASE_P_DefaultMinMaxOffset, table.m_tag, sIdentity);
                }

                // check the BaseLangSysRecord order
                bool bOrderOk = true;
                if (BaseLangSysCount > 1)
                {
                    for (uint i=0; i<BaseLangSysCount-1; i++)
                    {
                        BaseLangSysRecord ThisBlsr = GetBaseLangSysRecord(i);
                        BaseLangSysRecord NextBlsr = GetBaseLangSysRecord(i+1);
                        if (ThisBlsr.BaseLangSysTag >= NextBlsr.BaseLangSysTag)
                        {
                            v.Error(T.T_NULL, E.BASE_E_BaseLangSysRecord_order, table.m_tag, sIdentity);
                            bOrderOk = false;
                            bRet = false;
                        }
                    }
                }
                if (bOrderOk)
                {
                    v.Pass(T.T_NULL, P.BASE_P_BaseLangSysRecord_order, table.m_tag, sIdentity);
                }

                // check the BaseLangSysRecord MinMaxOffsets
                bool bOffsetsOk = true;
                for (uint i=0; i<BaseLangSysCount; i++)
                {
                    BaseLangSysRecord bslr = GetBaseLangSysRecord(i);
                    if (bslr.MinMaxOffset == 0)
                    {
                        v.Error(T.T_NULL, E.BASE_E_BaseLangSysRecord_offset0, table.m_tag, sIdentity + ", BaseLangSysRecord index = "+i);
                        bOffsetsOk = false;
                        bRet = false;
                    }
                    else if (bslr.MinMaxOffset + m_offsetBaseScriptTable > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.BASE_E_BaseLangSysRecord_offset, table.m_tag, sIdentity + ", BaseLangSysRecord index = "+i);
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }
                if (bOffsetsOk)
                {
                    v.Pass(T.T_NULL, P.BASE_P_BaseLangSysRecord_offsets, table.m_tag, sIdentity);
                }

                // check the BaseValuesTable
                if (BaseValuesOffset != 0)
                {
                    BaseValuesTable_val bvt = GetBaseValuesTable_val();
                    bvt.Validate(v, sIdentity, table);
                }

                // check the Default MinMaxTable
                if (DefaultMinMaxOffset != 0)
                {
                    MinMaxTable_val mmt = GetDefaultMinMaxTable_val();
                    mmt.Validate(v, sIdentity + ", default MinMax", table);
                }

                // check the BaseLangSysRecord MinMaxTables
                for (uint i=0; i<BaseLangSysCount; i++)
                {
                    BaseLangSysRecord blsr = GetBaseLangSysRecord(i);
                    MinMaxTable_val mmt = GetMinMaxTable_val(blsr);
                    mmt.Validate(v, sIdentity + ", BaseLangSysRecord[" + i + "]", table);
                }

                return bRet;
            }


            public BaseValuesTable_val GetBaseValuesTable_val()
            {
                BaseValuesTable_val bvt = null;

                if (BaseValuesOffset != 0)
                {
                    bvt = new BaseValuesTable_val((ushort)(m_offsetBaseScriptTable + BaseValuesOffset), m_bufTable);
                }

                return bvt;
            }

            public MinMaxTable_val GetDefaultMinMaxTable_val()
            {
                MinMaxTable_val mmt = null;
                
                if (DefaultMinMaxOffset != 0)
                {
                    mmt = new MinMaxTable_val((ushort)(m_offsetBaseScriptTable + DefaultMinMaxOffset), m_bufTable);
                }

                return mmt;
            }

            public MinMaxTable_val GetMinMaxTable_val(BaseLangSysRecord blsr)
            {
                MinMaxTable_val mmt = null;

                if (blsr != null)
                {
                    ushort offset = (ushort)(m_offsetBaseScriptTable + blsr.MinMaxOffset);
                    mmt = new MinMaxTable_val(offset, m_bufTable);
                }

                return mmt;
            }
        }

        public class BaseValuesTable_val : BaseValuesTable, I_OTLValidate
        {
            public BaseValuesTable_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_BASE)table).ValidateNoOverlap(m_offsetBaseValuesTable, CalcLength(), v, sIdentity, table.GetTag());

                // check BaseCoord offsets
                bool bOffsetsOk = true;
                for (uint i=0; i<BaseCoordCount; i++)
                {
                    ushort bco = GetBaseCoordOffset(i);
                    if (bco == 0)
                    {
                        v.Error(T.T_NULL, E.BASE_E_BaseValuesTable_BCO_0, table.m_tag, sIdentity + ", BaseCoordOffset index = " + i);
                        bOffsetsOk = false;
                        bRet = false;
                    }
                    else if (bco + m_offsetBaseValuesTable > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.BASE_E_BaseValuesTable_BCO_invalid, table.m_tag, sIdentity + ", BaseCoordOffset index = " + i);
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }
                if (bOffsetsOk)
                {
                    v.Pass(T.T_NULL, P.BASE_P_BaseValuesTable_BCO, table.m_tag, sIdentity);
                }

                // check the BaseCoord tables
                for (uint i=0; i<BaseCoordCount; i++)
                {
                    BaseCoordTable_val bct = GetBaseCoordTable_val(i);
                    bct.Validate(v, sIdentity, table);
                }

                return bRet;
            }

            public BaseCoordTable_val GetBaseCoordTable_val(uint i)
            {
                BaseCoordTable_val bct = null;

                if (i < BaseCoordCount)
                {
                    ushort offset = (ushort)(m_offsetBaseValuesTable + GetBaseCoordOffset(i));
                    bct = new BaseCoordTable_val(offset, m_bufTable);
                }

                return bct;
            }
        }

        public class MinMaxTable_val : MinMaxTable, I_OTLValidate
        {
            public MinMaxTable_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_BASE)table).ValidateNoOverlap(m_offsetMinMaxTable, CalcLength(), v, sIdentity, table.GetTag());

                // check the MinCoordOffset
                if (MinCoordOffset == 0)
                {
                    v.Pass(T.T_NULL, P.BASE_P_MinMaxTable_MinCO_0, table.m_tag, sIdentity);
                }
                else if (MinCoordOffset + m_offsetMinMaxTable > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.BASE_E_MinMaxTable_MinCO, table.m_tag, sIdentity);
                    bRet = false;
                }
                else
                {
                    v.Pass(T.T_NULL, P.BASE_P_MinMaxTable_MinCO, table.m_tag, sIdentity);
                }

                // check the MaxCoordOffset
                if (MaxCoordOffset == 0)
                {
                    v.Pass(T.T_NULL, P.BASE_P_MinMaxTable_MaxCO_0, table.m_tag, sIdentity);
                }
                else if (MaxCoordOffset + m_offsetMinMaxTable > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.BASE_E_MinMaxTable_MaxCO, table.m_tag, sIdentity);
                    bRet = false;
                }
                else
                {
                    v.Pass(T.T_NULL, P.BASE_P_MinMaxTable_MaxCO, table.m_tag, sIdentity);
                }

                // check the FeatMinMaxRecords
                bool bFeatMinMaxRecordsOk = true;
                for (uint i=0; i<FeatMinMaxCount; i++)
                {
                    FeatMinMaxRecord fmmr = GetFeatMinMaxRecord(i);

                    if (fmmr!=null)
                    {
                    
                        if (fmmr.MinCoordOffset + m_offsetMinMaxTable > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.BASE_E_FeatMinMaxRecords_MinCO_offset, table.m_tag, sIdentity + ", FeatMinMaxRecord[" + i + "]");
                            bFeatMinMaxRecordsOk = false;
                            bRet = false;
                        }

                        if (fmmr.MaxCoordOffset + m_offsetMinMaxTable > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.BASE_E_FeatMinMaxRecords_MaxCO_offset, table.m_tag, sIdentity + ", FeatMinMaxRecord[" + i + "]");
                            bFeatMinMaxRecordsOk = false;
                            bRet = false;
                        }
                    }
                    else
                    {
                        bFeatMinMaxRecordsOk = false;
                    }
                }
                if (bFeatMinMaxRecordsOk)
                {
                    v.Pass(T.T_NULL, P.BASE_P_FeatMinMaxRecords_offsets, table.m_tag, sIdentity);
                }

                // check the BaseCoord Tables
                BaseCoordTable_val bct = null;

                bct = GetMinCoordTable_val();
                if (bct != null)
                {
                    bct.Validate(v, sIdentity, table);
                }
                
                bct = GetMaxCoordTable_val();
                if (bct != null)
                {
                    bct.Validate(v, sIdentity, table);
                }
                for (uint i=0; i<FeatMinMaxCount; i++)
                {
                    FeatMinMaxRecord fmmr = GetFeatMinMaxRecord(i);

                    bct = GetFeatMinCoordTable_val(fmmr);
                    if (bct != null)
                    {
                        bct.Validate(v, sIdentity, table);
                    }

                    bct = GetFeatMaxCoordTable_val(fmmr);
                    if (bct != null)
                    {
                        bct.Validate(v, sIdentity, table);
                    }
                }

                return bRet;
            }

            public BaseCoordTable_val GetMinCoordTable_val()
            {
                BaseCoordTable_val bct = null;

                if (MinCoordOffset != 0)
                {
                    ushort offset = (ushort)(m_offsetMinMaxTable + MinCoordOffset);
                    if (offset + 4 < m_bufTable.GetLength())
                    {
                        bct = new BaseCoordTable_val(offset, m_bufTable);
                    }
                }

                return bct;
            }

            public BaseCoordTable_val GetMaxCoordTable_val()
            {
                BaseCoordTable_val bct = null;

                if (MaxCoordOffset != 0)
                {
                    ushort offset = (ushort)(m_offsetMinMaxTable + MaxCoordOffset);
                    if (offset + 4 < m_bufTable.GetLength())
                    {
                        bct = new BaseCoordTable_val(offset, m_bufTable);
                    }
                }

                return bct;
            }

            public BaseCoordTable_val GetFeatMinCoordTable_val(FeatMinMaxRecord fmmr)
            {
                BaseCoordTable_val bct = null;

                if (fmmr != null)
                {
                    if (fmmr.MinCoordOffset != 0)
                    {
                        ushort offset = (ushort)(m_offsetMinMaxTable + fmmr.MinCoordOffset);
                        if (offset + 4 < m_bufTable.GetLength())
                        {
                            bct = new BaseCoordTable_val(offset, m_bufTable);
                        }
                    }
                }

                return bct;
            }

            public BaseCoordTable_val GetFeatMaxCoordTable_val(FeatMinMaxRecord fmmr)
            {
                BaseCoordTable_val bct = null;

                if (fmmr != null)
                {
                    if (fmmr.MaxCoordOffset != 0)
                    {
                        ushort offset = (ushort)(m_offsetMinMaxTable + fmmr.MaxCoordOffset);
                        if (offset + 4 < m_bufTable.GetLength())
                        {
                            bct = new BaseCoordTable_val(offset, m_bufTable);
                        }
                    }
                }

                return bct;
            }
        }

        public class BaseCoordTable_val : BaseCoordTable, I_OTLValidate
        {
            public BaseCoordTable_val(ushort offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_BASE)table).ValidateNoOverlap(m_offsetBaseCoordTable, CalcLength(), v, sIdentity, table.GetTag());

                if (BaseCoordFormat == 1 || BaseCoordFormat == 2 || BaseCoordFormat == 3)
                {
                    v.Pass(T.T_NULL, P.BASE_P_BaseCoordTable_format, table.m_tag, sIdentity);
                }
                else
                {
                    v.Error(T.T_NULL, E.BASE_E_BaseCoordTable_format, table.m_tag, sIdentity + ", format = " + BaseCoordFormat);
                }

                return bRet;
            }

            public DeviceTable_val GetDeviceTable_val()
            {
                if (BaseCoordFormat != 3)
                {
                    throw new System.InvalidOperationException();
                }
                return new DeviceTable_val(DeviceTableOffset, m_bufTable);
            }
        }


        
        public AxisTable_val GetHorizAxisTable_val()
        {
            AxisTable_val at = null;

            if (HorizAxisOffset != 0)
            {
                at = new AxisTable_val(HorizAxisOffset, m_bufTable);
            }

            return at;
        }

        public AxisTable_val GetVertAxisTable_val()
        {
            AxisTable_val at = null;

            if (VertAxisOffset != 0)
            {
                at = new AxisTable_val(VertAxisOffset, m_bufTable);
            }

            return at;
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
