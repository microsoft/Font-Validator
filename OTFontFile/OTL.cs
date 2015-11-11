using System;
using System.Diagnostics;


namespace OTFontFile.OTL
{
    public class ScriptListTable
    {
        /************************
         * constructor
         */

        public ScriptListTable(ushort offset, MBOBuffer bufTable)
        {
            m_offsetScriptListTable = offset;
            m_bufTable = bufTable;
        }

        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            ScriptCount   = 0,
            ScriptRecords = 2
        }


        /************************
         * accessors
         */

        public ushort ScriptCount
        {
            get {return m_bufTable.GetUshort(m_offsetScriptListTable + (uint)FieldOffsets.ScriptCount);}
        }

        public class ScriptRecord
        {
            public OTTag ScriptTag;
            public ushort ScriptTableOffset;
        }

        public ScriptRecord GetScriptRecord(uint i)
        {
            ScriptRecord sr = null;

            if (i < ScriptCount)
            {
                uint offset = m_offsetScriptListTable + (uint)FieldOffsets.ScriptRecords + i*6;
                sr = new ScriptRecord();
                sr.ScriptTag = m_bufTable.GetTag(offset);
                sr.ScriptTableOffset = m_bufTable.GetUshort(offset+4);
            }

            return sr;
        }

        public virtual ScriptTable GetScriptTable(ScriptRecord sr)
        {
            return new ScriptTable((ushort)(m_offsetScriptListTable + sr.ScriptTableOffset), m_bufTable);
        }

        /************************
         * member data
         */

        protected ushort m_offsetScriptListTable;
        protected MBOBuffer m_bufTable;
    }

    public class ScriptTable
    {
        /************************
         * constructor
         */

        public ScriptTable(ushort offset, MBOBuffer bufTable)
        {
            m_offsetScriptTable = offset;
            m_bufTable = bufTable;
        }

        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            DefaultLangSysOffset = 0,
            LangSysCount         = 2,
            LangSysRecord        = 4
        }



        /************************
         * accessors
         */

        public ushort DefaultLangSysOffset
        {
            get {return m_bufTable.GetUshort(m_offsetScriptTable + (uint)FieldOffsets.DefaultLangSysOffset);}
        }

        public ushort LangSysCount
        {
            get {return m_bufTable.GetUshort(m_offsetScriptTable + (uint)FieldOffsets.LangSysCount);}
        }

        public class LangSysRecord
        {
            public OTTag LangSysTag;
            public ushort LangSysOffset;
        }

        public LangSysRecord GetLangSysRecord(uint i)
        {
            LangSysRecord lsr = null;

            if (i < LangSysCount)
            {
                uint offset = m_offsetScriptTable + (uint)FieldOffsets.LangSysRecord + i*6;
                lsr = new LangSysRecord();
                lsr.LangSysTag = m_bufTable.GetTag(offset);
                lsr.LangSysOffset = m_bufTable.GetUshort(offset+4);
            }

            return lsr;
        }

        public virtual LangSysTable GetDefaultLangSysTable()
        {
            LangSysTable lst = null;
            if (DefaultLangSysOffset != 0)
            {
                lst = new LangSysTable((ushort)(m_offsetScriptTable + DefaultLangSysOffset), m_bufTable);;
            }
            return lst;
        }

        public virtual LangSysTable GetLangSysTable(LangSysRecord lsr)
        {
            return new LangSysTable((ushort)(m_offsetScriptTable + lsr.LangSysOffset), m_bufTable);
        }

        /************************
         * member data
         */

        protected ushort m_offsetScriptTable;
        protected MBOBuffer m_bufTable;
    }

    public class LangSysTable
    {
        /************************
         * constructor
         */

        public LangSysTable(ushort offset, MBOBuffer bufTable)
        {
            m_offsetLangSysTable = offset;
            m_bufTable = bufTable;
        }

        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            LookupOrder       = 0,
            ReqFeatureIndex   = 2,
            FeatureCount      = 4,
            FeatureIndexArray = 6
        }


        /************************
         * accessors
         */

        public ushort LookupOrder
        {
            get {return m_bufTable.GetUshort(m_offsetLangSysTable + (uint)FieldOffsets.LookupOrder);}
        }

        public ushort ReqFeatureIndex
        {
            get {return m_bufTable.GetUshort(m_offsetLangSysTable + (uint)FieldOffsets.ReqFeatureIndex);}
        }

        public ushort FeatureCount
        {
            get {return m_bufTable.GetUshort(m_offsetLangSysTable + (uint)FieldOffsets.FeatureCount);}
        }

        public ushort GetFeatureIndex(uint i)
        {
            uint offset = m_offsetLangSysTable + (uint)FieldOffsets.FeatureIndexArray + i*2;
            return m_bufTable.GetUshort(offset);
        }

        /************************
         * member data
         */

        protected ushort m_offsetLangSysTable;
        protected MBOBuffer m_bufTable;
    }

    public class FeatureListTable
    {
        /************************
         * constructor
         */

        public FeatureListTable(ushort offset, MBOBuffer bufTable)
        {
            m_offsetFeatureListTable = offset;
            m_bufTable = bufTable;
        }

        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            FeatureCount       = 0,
            FeatureRecordArray = 2
        }



        /************************
         * accessors
         */

        public ushort FeatureCount
        {
            get {return m_bufTable.GetUshort(m_offsetFeatureListTable + (uint)FieldOffsets.FeatureCount);}
        }

        public class FeatureRecord
        {
            public OTTag FeatureTag;
            public ushort FeatureTableOffset;
        }

        public FeatureRecord GetFeatureRecord(uint i)
        {
            FeatureRecord fr = null;

            if (i < FeatureCount)
            {
                uint offset = m_offsetFeatureListTable + (uint)FieldOffsets.FeatureRecordArray + i*6;
                if (offset+6 <= m_bufTable.GetLength())
                {
                    fr = new FeatureRecord();
                    fr.FeatureTag = m_bufTable.GetTag(offset);
                    fr.FeatureTableOffset = m_bufTable.GetUshort(offset + 4);
                }
            }

            return fr;
        }

        public virtual FeatureTable GetFeatureTable(FeatureRecord fr)
        {
            return new FeatureTable((ushort)(m_offsetFeatureListTable + fr.FeatureTableOffset), m_bufTable);
        }


        /************************
         * member data
         */

        protected ushort m_offsetFeatureListTable;
        protected MBOBuffer m_bufTable;
    }

    public class FeatureTable
    {
        /************************
         * constructor
         */

        public FeatureTable(ushort offset, MBOBuffer bufTable)
        {
            m_offsetFeatureTable = offset;
            m_bufTable = bufTable;
        }

        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            FeatureParams        = 0,
            LookupCount          = 2,
            LookupListIndexArray = 4
        }



        /************************
         * accessors
         */

        public ushort FeatureParams
        {
            get {return m_bufTable.GetUshort(m_offsetFeatureTable + (uint)FieldOffsets.FeatureParams);}
        }

        public ushort LookupCount
        {
            get {return m_bufTable.GetUshort(m_offsetFeatureTable + (uint)FieldOffsets.LookupCount);}
        }

        public ushort GetLookupListIndex(uint i)
        {
            uint offset = m_offsetFeatureTable + (uint)FieldOffsets.LookupListIndexArray + i*2;
            return m_bufTable.GetUshort(offset);
        }

        /************************
         * member data
         */

        protected ushort m_offsetFeatureTable;
        protected MBOBuffer m_bufTable;
    }

    public class LookupListTable
    {
        /************************
         * constructor
         */

        public LookupListTable(ushort offset, MBOBuffer bufTable, OTTag tag)
        {
            m_offsetLookupListTable = offset;
            m_bufTable = bufTable;
            m_tag = tag;
        }

        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            LookupCount = 0,
            LookupArray = 2
        }



        /************************
         * accessors
         */

        public ushort LookupCount
        {
            get {return m_bufTable.GetUshort(m_offsetLookupListTable + (uint)FieldOffsets.LookupCount);}
        }

        public ushort GetLookupOffset(uint i)
        {
            return m_bufTable.GetUshort(m_offsetLookupListTable + (uint)FieldOffsets.LookupArray + i*2);
        }

        public virtual LookupTable GetLookupTable(uint i)
        {
            LookupTable lt = null;

            if (i < LookupCount)
            {
                ushort offset = (ushort)(m_offsetLookupListTable + GetLookupOffset(i));
                if (offset + 6 <= m_bufTable.GetLength()) // minimum lookuptable with zero entries is six bytes
                {
                    lt = new LookupTable(offset, m_bufTable, m_tag);
                }
            }

            return lt;
        }


        /************************
         * member data
         */

        protected ushort m_offsetLookupListTable;
        protected MBOBuffer m_bufTable;
        protected OTTag m_tag;
    }

    public class LookupTable
    {
        /************************
         * constructor
         */

        public LookupTable(ushort offset, MBOBuffer bufTable, OTTag tag)
        {
            m_offsetLookupTable = offset;
            m_bufTable = bufTable;
            m_tag = tag;
        }

        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            LookupType          = 0,
            LookupFlag          = 2,
            SubTableCount       = 4,
            SubTableOffsetArray = 6
        }

        
        /************************
         * accessors
         */

        public ushort LookupType
        {
            get {return m_bufTable.GetUshort(m_offsetLookupTable + (uint)FieldOffsets.LookupType);}
        }

        public ushort LookupFlag
        {
            get {return m_bufTable.GetUshort(m_offsetLookupTable + (uint)FieldOffsets.LookupFlag);}
        }

        public ushort SubTableCount
        {
            get {return m_bufTable.GetUshort(m_offsetLookupTable + (uint)FieldOffsets.SubTableCount);}
        }

        public ushort GetSubTableOffset(uint i)
        {
            if (i >= SubTableCount)
            {
                throw new ArgumentOutOfRangeException();
            }

            return m_bufTable.GetUshort(m_offsetLookupTable + (uint)FieldOffsets.SubTableOffsetArray + i*2);
        }

        public virtual SubTable GetSubTable(uint i)
        {
            if (i >= SubTableCount)
            {
                throw new ArgumentOutOfRangeException();
            }

            SubTable st = null;
            uint stOffset = m_offsetLookupTable + (uint)GetSubTableOffset(i);

            if ((string)m_tag == "GPOS")
            {
                switch (LookupType)
                {
                    case 1: st = new Table_GPOS.SinglePos      (stOffset, m_bufTable); break;
                    case 2: st = new Table_GPOS.PairPos        (stOffset, m_bufTable); break;
                    case 3: st = new Table_GPOS.CursivePos     (stOffset, m_bufTable); break;
                    case 4: st = new Table_GPOS.MarkBasePos    (stOffset, m_bufTable); break;
                    case 5: st = new Table_GPOS.MarkLigPos     (stOffset, m_bufTable); break;
                    case 6: st = new Table_GPOS.MarkMarkPos    (stOffset, m_bufTable); break;
                    case 7: st = new Table_GPOS.ContextPos     (stOffset, m_bufTable); break;
                    case 8: st = new Table_GPOS.ChainContextPos(stOffset, m_bufTable); break;
                    case 9: st = new Table_GPOS.ExtensionPos   (stOffset, m_bufTable); break;
                }
            }
            else if ((string)m_tag == "GSUB")
            {
                switch (LookupType)
                {
                    case 1: st = new Table_GSUB.SingleSubst      (stOffset, m_bufTable); break;
                    case 2: st = new Table_GSUB.MultipleSubst    (stOffset, m_bufTable); break;
                    case 3: st = new Table_GSUB.AlternateSubst   (stOffset, m_bufTable); break;
                    case 4: st = new Table_GSUB.LigatureSubst    (stOffset, m_bufTable); break;
                    case 5: st = new Table_GSUB.ContextSubst     (stOffset, m_bufTable); break;
                    case 6: st = new Table_GSUB.ChainContextSubst(stOffset, m_bufTable); break;
                    case 7: st = new Table_GSUB.ExtensionSubst   (stOffset, m_bufTable); break;
                    case 8: st = new Table_GSUB.ReverseChainSubst(stOffset, m_bufTable); break;
                }
            }
            else
            {
                throw new InvalidOperationException("unknown table type");
            }

            return st;
        }

        /************************
         * member data
         */

        protected ushort m_offsetLookupTable;
        protected MBOBuffer m_bufTable;
        protected OTTag m_tag;
    }

    public abstract class SubTable
    {
        public SubTable(uint offset, MBOBuffer bufTable)
        {
            m_offsetSubTable = offset;
            m_bufTable = bufTable;
        }

        // this is needed for calculating OS2.usMaxContext
        public abstract uint GetMaxContextLength();

        protected uint m_offsetSubTable;
        protected MBOBuffer m_bufTable;
    }

    public class CoverageTable
    {
        /************************
         * constructor
         */

        public CoverageTable(uint offset, MBOBuffer bufTable)
        {
            m_offsetCoverageTable = offset;
            m_bufTable = bufTable;
        }

        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            CoverageFormat = 0
        }
        public enum FieldOffsets1
        {
            GlyphCount = 2,
            GlyphArray = 4
        }
        public enum FieldOffsets2
        {
            RangeCount       = 2,
            RangeRecordArray = 4
        }


        /************************
         * accessors
         */
        

        public ushort CoverageFormat
        {
            get {return m_bufTable.GetUshort(m_offsetCoverageTable + (uint)FieldOffsets.CoverageFormat);}
        }

        // format 1 only!

        public ushort F1GlyphCount
        {
            get
            {
                if (CoverageFormat != 1)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort(m_offsetCoverageTable + (uint)FieldOffsets1.GlyphCount);
            }
        }

        public ushort F1GetGlyphID(uint iPos)
        {
            if (CoverageFormat != 1)
            {
                throw new System.InvalidOperationException();
            }
            uint offset = m_offsetCoverageTable + (uint)FieldOffsets1.GlyphArray + iPos*2;
            return m_bufTable.GetUshort(offset);
        }

        // format 2 only!

        public ushort F2RangeCount
        {
            get
            {
                if (CoverageFormat != 2)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort(m_offsetCoverageTable + (uint)FieldOffsets2.RangeCount);
            }
        }

        public RangeRecord F2GetRangeRecord(uint i)
        {
            if (CoverageFormat != 2)
            {
                throw new System.InvalidOperationException();
            }

            RangeRecord rr = null;

            if (i < F2RangeCount)
            {
                rr = new RangeRecord();
                uint rrOffset = m_offsetCoverageTable + (uint)FieldOffsets2.RangeRecordArray + i*6;
                rr.Start              = m_bufTable.GetUshort(rrOffset);
                rr.End                = m_bufTable.GetUshort(rrOffset + 2);
                rr.StartCoverageIndex = m_bufTable.GetUshort(rrOffset + 4);
            }

            return rr;
        }

        public class RangeRecord
        {
            public ushort Start;
            public ushort End;
            public ushort StartCoverageIndex;
        }


        // either format

        public CoverageResults GetGlyphCoverage(ushort nGlyph)
        {
            CoverageResults cr;
            cr.bCovered = false;
            cr.CoverageIndex = 0;

            if (CoverageFormat == 1)
            {
                ushort count = F1GlyphCount;
                for (ushort i=0; i<count; i++)
                {
                    if (nGlyph == F1GetGlyphID(i))
                    {
                        cr.bCovered = true;
                        cr.CoverageIndex = i;
                        break;
                    }
                }
            }
            else if (CoverageFormat == 2)
            {
                ushort FirstIndexOfRange = 0;
                for (uint i=0; i<F2RangeCount; i++)
                {
                    RangeRecord rr = F2GetRangeRecord(i);
                    if (nGlyph >= rr.Start && nGlyph <= rr.End)
                    {
                        cr.bCovered = true;
                        cr.CoverageIndex = (ushort)(FirstIndexOfRange + nGlyph - rr.Start);
                        break;
                    }
                    FirstIndexOfRange += (ushort)(rr.End-rr.Start+1);
                }
            }
            else
            {
                Debug.Assert(false);
            }

            return cr;
        }

        public struct CoverageResults
        {
            public bool bCovered;
            public ushort CoverageIndex;
        }

        /************************
         * member data
         */

        protected uint m_offsetCoverageTable;
        protected MBOBuffer m_bufTable;
    }

    public class ClassDefTable
    {
        /************************
         * constructor
         */

        public ClassDefTable(uint offset, MBOBuffer bufTable)
        {
            m_offsetClassDefTable = offset;
            m_bufTable = bufTable;
        }

        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            ClassFormat = 0
        }

        /************************
         * nested classes
         */

        protected class ClassDefFormat1
        {
            public ClassDefFormat1(uint offset, MBOBuffer bufTable)
            {
                m_offsetClassDefFormat1 = offset;
                m_bufTable = bufTable;
            }


            public enum FieldOffsets
            {
                ClassFormat     = 0,
                StartGlyph      = 2,
                GlyphCount      = 4,
                ClassValueArray = 6
            }

            public ushort ClassFormat
            {
                get {return m_bufTable.GetUshort(m_offsetClassDefFormat1 + (uint)FieldOffsets.ClassFormat);}
            }

            public ushort StartGlyph
            {
                get {return m_bufTable.GetUshort(m_offsetClassDefFormat1 + (uint)FieldOffsets.StartGlyph);}
            }

            public ushort GlyphCount
            {
                get {return m_bufTable.GetUshort(m_offsetClassDefFormat1 + (uint)FieldOffsets.GlyphCount);}
            }

            public ushort GetClassValue(uint i)
            {
                ushort val = 0;

                ushort start = StartGlyph;
                ushort count = GlyphCount;

                if (i >= start && i < start + count)
                {
                    uint index = i - start;
                    uint ArrayOffset = m_offsetClassDefFormat1 + (uint)FieldOffsets.ClassValueArray;
                    val = m_bufTable.GetUshort(ArrayOffset + index*2);
                }

                return val;
            }

            protected uint m_offsetClassDefFormat1;
            protected MBOBuffer m_bufTable;
        }

        protected class ClassDefFormat2
        {
            public ClassDefFormat2(uint offset, MBOBuffer bufTable)
            {
                m_offsetClassDefFormat2 = offset;
                m_bufTable = bufTable;
            }


            public enum FieldOffsets
            {
                ClassFormat           = 0,
                ClassRangeCount       = 2,
                ClassRangeRecordArray = 4
            }

            public ushort ClassFormat
            {
                get {return m_bufTable.GetUshort(m_offsetClassDefFormat2 + (uint)FieldOffsets.ClassFormat);}
            }

            public ushort ClassRangeCount
            {
                get {return m_bufTable.GetUshort(m_offsetClassDefFormat2 + (uint)FieldOffsets.ClassRangeCount);}
            }

            public ClassRangeRecord GetClassRangeRecord(uint i)
            {
                ClassRangeRecord crr = null;

                if (i < ClassRangeCount)
                {
                    crr = new ClassRangeRecord();
                    uint crrOffset = m_offsetClassDefFormat2 + (uint)FieldOffsets.ClassRangeRecordArray + i*6;
                    crr.Start = m_bufTable.GetUshort(crrOffset);
                    crr.End   = m_bufTable.GetUshort(crrOffset + 2);
                    crr.Class = m_bufTable.GetUshort(crrOffset + 4);
                }

                return crr;
            }

            public class ClassRangeRecord
            {
                public ushort Start;
                public ushort End;
                public ushort Class;
            }

            protected uint m_offsetClassDefFormat2;
            protected MBOBuffer m_bufTable;
        }


        /************************
         * accessors
         */
        

        public ushort ClassFormat
        {
            get {return m_bufTable.GetUshort(m_offsetClassDefTable + (uint)FieldOffsets.ClassFormat);}
        }

        ClassDefFormat1 GetClassDefFormat1()
        {
            return new ClassDefFormat1(m_offsetClassDefTable, m_bufTable);
        }

        ClassDefFormat2 GetClassDefFormat2()
        {
            return new ClassDefFormat2(m_offsetClassDefTable, m_bufTable);
        }

        // get class value for a given glyph for either format
        
        public ushort GetClassValue(ushort nGlyph)
        {
            ushort val = 0;

            if (ClassFormat == 1)
            {
                ClassDefFormat1 cdf1 = GetClassDefFormat1();
                val = cdf1.GetClassValue(nGlyph);
            }
            else if (ClassFormat == 2)
            {
                ClassDefFormat2 cdf2 = GetClassDefFormat2();
                for (uint i=0; i<cdf2.ClassRangeCount; i++)
                {
                    ClassDefFormat2.ClassRangeRecord crr = cdf2.GetClassRangeRecord(i);
                    if (nGlyph >= crr.Start && nGlyph <= crr.End)
                    {
                        val = crr.Class;
                        break;
                    }
                }
            }
            else
            {
                Debug.Assert(false);
            }

            return val;
        }

        /************************
         * member data
         */

        protected uint m_offsetClassDefTable;
        protected MBOBuffer m_bufTable;
    }

    public class DeviceTable
    {
        public DeviceTable(uint offset, MBOBuffer bufTable)
        {
            m_offsetDeviceTable = offset;
            m_bufTable = bufTable;
        }

        public enum FieldOffsets
        {
            StartSize       = 0,
            EndSize         = 2,
            DeltaFormat     = 4,
            DeltaValueArray = 6
        }

        public ushort StartSize
        {
            get {return m_bufTable.GetUshort(m_offsetDeviceTable + (uint)FieldOffsets.StartSize);}
        }

        public ushort EndSize
        {
            get {return m_bufTable.GetUshort(m_offsetDeviceTable + (uint)FieldOffsets.EndSize);}
        }

        public ushort DeltaFormat
        {
            get {return m_bufTable.GetUshort(m_offsetDeviceTable + (uint)FieldOffsets.DeltaFormat);}
        }

        public ushort GetDeltaValueUint(uint i)
        {
            uint offset = m_offsetDeviceTable + (uint)FieldOffsets.DeltaValueArray + i*2;
            return m_bufTable.GetUshort(offset);
        }

        public uint m_offsetDeviceTable;
        public MBOBuffer m_bufTable;
    }


}
