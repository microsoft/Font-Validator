using System;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_BASE.
    /// </summary>
    public class Table_BASE : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_BASE(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            Version         = 0,
            HorizAxisOffset = 4,
            VertAxisOffset  = 6
        }


        /************************
         * classes
         */

        public class AxisTable
        {
            public AxisTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetAxisTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                BaseTagListOffset    = 0,
                BaseScriptListOffset = 2
            }

            public uint CalcLength()
            {
                return 4;
            }

            public ushort BaseTagListOffset
            {
                get {return m_bufTable.GetUshort(m_offsetAxisTable + (uint)FieldOffsets.BaseTagListOffset);}
            }

            public ushort BaseScriptListOffset
            {
                get {return m_bufTable.GetUshort(m_offsetAxisTable + (uint)FieldOffsets.BaseScriptListOffset);}
            }

            public BaseTagListTable GetBaseTagListTable()
            {
                BaseTagListTable btlt = null;
                
                if (BaseTagListOffset != 0)
                {
                    ushort offset = (ushort)(m_offsetAxisTable + BaseTagListOffset);
                    btlt = new BaseTagListTable(offset, m_bufTable);
                }

                return btlt;
            }

            public BaseScriptListTable GetBaseScriptListTable()
            {
                BaseScriptListTable bslt = null;
                
                if (BaseScriptListOffset != 0)
                {
                    ushort offset = (ushort)(m_offsetAxisTable + BaseScriptListOffset);
                    bslt = new BaseScriptListTable(offset, m_bufTable);
                }

                return bslt;
            }

            protected ushort m_offsetAxisTable;
            protected MBOBuffer m_bufTable;
        }

        public class BaseTagListTable
        {
            public BaseTagListTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetBaseTagListTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                BaseTagCount     = 0,
                BaselineTagArray = 2
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.BaselineTagArray + (uint)BaseTagCount*4;
            }

            public ushort BaseTagCount
            {
                get {return m_bufTable.GetUshort(m_offsetBaseTagListTable + (uint)FieldOffsets.BaseTagCount);}
            }

            public OTTag GetBaselineTag(uint i)
            {
                OTTag tag = null;
                if (i < BaseTagCount)
                {
                    tag = m_bufTable.GetTag(m_offsetBaseTagListTable + (uint)FieldOffsets.BaselineTagArray + i*4);
                }

                return tag;
            }

            protected ushort m_offsetBaseTagListTable;
            protected MBOBuffer m_bufTable;
        }

        public class BaseScriptListTable
        {
            public BaseScriptListTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetBaseScriptListTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                BaseScriptCount       = 0,
                BaseScriptRecordArray = 2
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.BaseScriptRecordArray + (uint)BaseScriptCount*6;
            }

            public ushort BaseScriptCount
            {
                get {return m_bufTable.GetUshort(m_offsetBaseScriptListTable + (uint)FieldOffsets.BaseScriptCount);}
            }

            public BaseScriptRecord GetBaseScriptRecord(uint i)
            {
                BaseScriptRecord bsr = null;

                if (i < BaseScriptCount)
                {
                    uint sizeofRecord = 6;
                    uint offsetRecord = m_offsetBaseScriptListTable + (uint)FieldOffsets.BaseScriptRecordArray + i*sizeofRecord;
                    bsr = new BaseScriptRecord((ushort)offsetRecord, m_bufTable);
                }

                return bsr;
            }

            public BaseScriptTable GetBaseScriptTable(BaseScriptRecord bsr)
            {
                BaseScriptTable bst = null;
                if (bsr != null)
                {
                    ushort offset = (ushort)(m_offsetBaseScriptListTable + bsr.BaseScriptOffset);
                    bst = new BaseScriptTable(offset, m_bufTable);
                }
                return bst;
            }

            protected ushort m_offsetBaseScriptListTable;
            protected MBOBuffer m_bufTable;
        }

        public class BaseScriptRecord
        {
            public BaseScriptRecord(ushort offset, MBOBuffer bufTable)
            {
                m_offsetBaseScriptRecord = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                BaseScriptTag    = 0,
                BaseScriptOffset = 4
            }

            public OTTag  BaseScriptTag
            {
                get {return m_bufTable.GetTag(m_offsetBaseScriptRecord + (uint)FieldOffsets.BaseScriptTag);}
            }

            public ushort BaseScriptOffset
            {
                get {return m_bufTable.GetUshort(m_offsetBaseScriptRecord + (uint)FieldOffsets.BaseScriptOffset);}
            }

            protected ushort m_offsetBaseScriptRecord;
            protected MBOBuffer m_bufTable;
        }

        public class BaseScriptTable
        {
            public BaseScriptTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetBaseScriptTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                BaseValuesOffset    = 0,
                DefaultMinMaxOffset = 2,
                BaseLangSysCount    = 4,
                BaseLangSysRecords  = 6
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.BaseLangSysRecords + (uint)BaseLangSysCount*6;
            }

            public ushort BaseValuesOffset
            {
                get {return m_bufTable.GetUshort(m_offsetBaseScriptTable + (uint)FieldOffsets.BaseValuesOffset);}
            }

            public ushort DefaultMinMaxOffset
            {
                get {return m_bufTable.GetUshort(m_offsetBaseScriptTable + (uint)FieldOffsets.DefaultMinMaxOffset);}
            }

            public ushort BaseLangSysCount
            {
                get {return m_bufTable.GetUshort(m_offsetBaseScriptTable + (uint)FieldOffsets.BaseLangSysCount);}
            }

            public BaseLangSysRecord GetBaseLangSysRecord(uint i)
            {
                BaseLangSysRecord blsr = null;

                if (i < BaseLangSysCount)
                {
                    ushort sizeofBaseLangSysRecord = 6;
                    ushort offset = (ushort)(m_offsetBaseScriptTable + (uint)FieldOffsets.BaseLangSysRecords + sizeofBaseLangSysRecord*i);
                    blsr = new BaseLangSysRecord(offset, m_bufTable);
                }

                return blsr;
            }

            public BaseValuesTable GetBaseValuesTable()
            {
                BaseValuesTable bvt = null;

                if (BaseValuesOffset != 0)
                {
                    bvt = new BaseValuesTable((ushort)(m_offsetBaseScriptTable + BaseValuesOffset), m_bufTable);
                }

                return bvt;
            }

            public MinMaxTable GetDefaultMinMaxTable()
            {
                MinMaxTable mmt = null;
                
                if (DefaultMinMaxOffset != 0)
                {
                    mmt = new MinMaxTable((ushort)(m_offsetBaseScriptTable + DefaultMinMaxOffset), m_bufTable);
                }

                return mmt;
            }

            public MinMaxTable GetMinMaxTable(BaseLangSysRecord blsr)
            {
                MinMaxTable mmt = null;

                if (blsr != null)
                {
                    ushort offset = (ushort)(m_offsetBaseScriptTable + blsr.MinMaxOffset);
                    mmt = new MinMaxTable(offset, m_bufTable);
                }

                return mmt;
            }

            protected ushort m_offsetBaseScriptTable;
            protected MBOBuffer m_bufTable;
        }

        public class BaseLangSysRecord
        {
            public BaseLangSysRecord(ushort offset, MBOBuffer bufTable)
            {
                m_offsetBaseLangSysRecord = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                BaseLangSysTag = 0,
                MinMaxOffset   = 4
            }

            public OTTag BaseLangSysTag
            {
                get {return m_bufTable.GetTag(m_offsetBaseLangSysRecord + (uint)FieldOffsets.BaseLangSysTag);}
            }

            public ushort MinMaxOffset
            {
                get {return m_bufTable.GetUshort(m_offsetBaseLangSysRecord + (uint)FieldOffsets.MinMaxOffset);}
            }

            public MinMaxTable GetMinMaxTable()
            {
                MinMaxTable mmt = new MinMaxTable(MinMaxOffset, m_bufTable);
                return mmt;
            }

            protected ushort m_offsetBaseLangSysRecord;
            protected MBOBuffer m_bufTable;
        }

        public class BaseValuesTable
        {
            public BaseValuesTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetBaseValuesTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                DefaultIndex     = 0,
                BaseCoordCount   = 2,
                BaseCoordOffsets = 4
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.BaseCoordOffsets + (uint)BaseCoordCount*2;
            }

            public ushort DefaultIndex
            {
                get {return m_bufTable.GetUshort(m_offsetBaseValuesTable + (uint)FieldOffsets.DefaultIndex);}
            }

            public ushort BaseCoordCount
            {
                get {return m_bufTable.GetUshort(m_offsetBaseValuesTable + (uint)FieldOffsets.BaseCoordCount);}
            }

            public ushort GetBaseCoordOffset(uint i)
            {
                uint offset = m_offsetBaseValuesTable + (uint)FieldOffsets.BaseCoordOffsets + (uint)i*2;
                return m_bufTable.GetUshort(offset);
            }

            public BaseCoordTable GetBaseCoordTable(uint i)
            {
                BaseCoordTable bct = null;

                if (i < BaseCoordCount)
                {
                    ushort offset = (ushort)(m_offsetBaseValuesTable + GetBaseCoordOffset(i));
                    bct = new BaseCoordTable(offset, m_bufTable);
                }

                return bct;
            }

            protected ushort m_offsetBaseValuesTable;
            protected MBOBuffer m_bufTable;
        }

        public class MinMaxTable
        {
            public MinMaxTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetMinMaxTable = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                MinCoordOffset    = 0,
                MaxCoordOffset    = 2,
                FeatMinMaxCount   = 4,
                FeatMinMaxRecords = 6
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.FeatMinMaxRecords + (uint)FeatMinMaxCount*8;
            }

            public ushort MinCoordOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMinMaxTable + (uint)FieldOffsets.MinCoordOffset);}
            }

            public ushort MaxCoordOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMinMaxTable + (uint)FieldOffsets.MaxCoordOffset);}
            }

            public ushort FeatMinMaxCount
            {
                get {return m_bufTable.GetUshort(m_offsetMinMaxTable + (uint)FieldOffsets.FeatMinMaxCount);}
            }

            public FeatMinMaxRecord GetFeatMinMaxRecord(uint i)
            {
                FeatMinMaxRecord fmmr = null;

                if (i < FeatMinMaxCount)
                {
                    uint sizeofFeatMinMaxRecord = 8;
                    ushort offset = (ushort)(m_offsetMinMaxTable + i*sizeofFeatMinMaxRecord);
                    if (offset + sizeofFeatMinMaxRecord <= m_bufTable.GetLength())
                    {
                        fmmr = new FeatMinMaxRecord(offset, m_bufTable);
                    }
                }

                return fmmr;
            }

            public BaseCoordTable GetMinCoordTable()
            {
                BaseCoordTable bct = null;

                if (MinCoordOffset != 0)
                {
                    ushort offset = (ushort)(m_offsetMinMaxTable + MinCoordOffset);
                    bct = new BaseCoordTable(offset, m_bufTable);
                }

                return bct;
            }

            public BaseCoordTable GetMaxCoordTable()
            {
                BaseCoordTable bct = null;

                if (MaxCoordOffset != 0)
                {
                    ushort offset = (ushort)(m_offsetMinMaxTable + MaxCoordOffset);
                    bct = new BaseCoordTable(offset, m_bufTable);
                }

                return bct;
            }

            public BaseCoordTable GetFeatMinCoordTable(FeatMinMaxRecord fmmr)
            {
                BaseCoordTable bct = null;

                if (fmmr != null)
                {
                    if (fmmr.MinCoordOffset != 0)
                    {
                        ushort offset = (ushort)(m_offsetMinMaxTable + fmmr.MinCoordOffset);
                        bct = new BaseCoordTable(offset, m_bufTable);
                    }
                }

                return bct;
            }

            public BaseCoordTable GetFeatMaxCoordTable(FeatMinMaxRecord fmmr)
            {
                BaseCoordTable bct = null;

                if (fmmr != null)
                {
                    if (fmmr.MaxCoordOffset != 0)
                    {
                        ushort offset = (ushort)(m_offsetMinMaxTable + fmmr.MaxCoordOffset);
                        bct = new BaseCoordTable(offset, m_bufTable);
                    }
                }

                return bct;
            }

            protected ushort m_offsetMinMaxTable;
            protected MBOBuffer m_bufTable;
        }

        public class FeatMinMaxRecord
        {
            public FeatMinMaxRecord(ushort offset, MBOBuffer bufTable)
            {
                m_offsetFeatMinMaxRecord = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                FeatureTableTag = 0,
                MinCoordOffset  = 4,
                MaxCoordOffset  = 6
            }

            public OTTag FeatureTableTag
            {
                get {return m_bufTable.GetTag(m_offsetFeatMinMaxRecord + (uint)FieldOffsets.FeatureTableTag);}
            }

            public ushort MinCoordOffset
            {
                get {return m_bufTable.GetUshort(m_offsetFeatMinMaxRecord + (uint)FieldOffsets.MinCoordOffset);}
            }

            public ushort MaxCoordOffset
            {
                get {return m_bufTable.GetUshort(m_offsetFeatMinMaxRecord + (uint)FieldOffsets.MaxCoordOffset);}
            }

            protected ushort m_offsetFeatMinMaxRecord;
            protected MBOBuffer m_bufTable;
        }

        public class BaseCoordTable
        {
            public BaseCoordTable(ushort offset, MBOBuffer bufTable)
            {
                m_offsetBaseCoordTable = offset;
                m_bufTable = bufTable;
            }

            // format 1, 2, and 3

            public enum FieldOffsets
            {
                BaseCoordFormat = 0,
                Coordinate      = 2
            }


            public ushort BaseCoordFormat
            {
                get {return m_bufTable.GetUshort(m_offsetBaseCoordTable + (uint)FieldOffsets.BaseCoordFormat);}
            }

            public ushort Coordinate
            {
                get {return m_bufTable.GetUshort(m_offsetBaseCoordTable + (uint)FieldOffsets.Coordinate);}
            }

            
            // format 2 only!

            public enum FieldOffsets2
            {
                ReferenceGlyph = 4,
                BaseCoordPoint = 6
            }
            public ushort ReferenceGlyph
            {
                get
                {
                    if (BaseCoordFormat != 2)
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_bufTable.GetUshort(m_offsetBaseCoordTable + (uint)FieldOffsets2.ReferenceGlyph);
                }
            }

            public ushort BaseCoordPoint
            {
                get
                {
                    if (BaseCoordFormat != 2)
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_bufTable.GetUshort(m_offsetBaseCoordTable + (uint)FieldOffsets2.BaseCoordPoint);
                }
            }


            // format 3 only!

            public enum FieldOffsets3
            {
                DeviceTableOffset = 4
            }

            public uint CalcLength()
            {
                uint nLen = 0xffffffff;

                if (BaseCoordFormat == 1)
                {
                    nLen = 4;
                }
                else if (BaseCoordFormat == 2)
                {
                    nLen = 8;
                }
                else if (BaseCoordFormat == 3)
                {
                    nLen = 6;
                }

                return nLen;
            }

            public ushort DeviceTableOffset
            {
                get
                {
                    if (BaseCoordFormat != 3)
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_bufTable.GetUshort(m_offsetBaseCoordTable + (uint)FieldOffsets3.DeviceTableOffset);
                }
            }

            public OTL.DeviceTable GetDeviceTable()
            {
                if (BaseCoordFormat != 3)
                {
                    throw new System.InvalidOperationException();
                }
                return new OTL.DeviceTable(DeviceTableOffset, m_bufTable);
            }



            protected ushort m_offsetBaseCoordTable;
            protected MBOBuffer m_bufTable;
        }


        
        /************************
         * accessors
         */
        
        public OTFixed Version
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.Version);}
        }

        public ushort HorizAxisOffset
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.HorizAxisOffset);}
        }

        public ushort VertAxisOffset
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.VertAxisOffset);}
        }

        public AxisTable GetHorizAxisTable()
        {
            AxisTable at = null;

            if (HorizAxisOffset != 0)
            {
                at = new AxisTable(HorizAxisOffset, m_bufTable);
            }

            return at;
        }

        public AxisTable GetVertAxisTable()
        {
            AxisTable at = null;

            if (VertAxisOffset != 0)
            {
                at = new AxisTable(VertAxisOffset, m_bufTable);
            }

            return at;
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new BASE_cache();
            }

            return m_cache;
        }
        
        public class BASE_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        

    }
}
