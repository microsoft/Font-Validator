using System;

using OTFontFile;
using OTFontFile.OTL;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_GDEF.
    /// </summary>
    public class val_GDEF : Table_GDEF, ITableValidate
    {
        /************************
         * constructors
         */
                
        public val_GDEF(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
            m_DataOverlapDetector = new DataOverlapDetector();
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.GDEF_Version))
            {
                if (Version.GetUint() == 0x00010000 || Version.GetUint() == 0x00010002 )
                {
                    v.Pass(T.GDEF_Version, P.GDEF_P_Version, m_tag, "0x"+ Version.GetUint().ToString("x8"));
                }
                else
                {
                    v.Error(T.GDEF_Version, E.GDEF_E_Version, m_tag, "0x" + Version.GetUint().ToString("x8"));
                }
            }

            if (v.PerformTest(T.GDEF_HeaderOffsets))
            {
                bool bOffsetsOk = true;

                if (GlyphClassDefOffset != 0)
                {
                    if (GlyphClassDefOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.GDEF_HeaderOffsets, E.GDEF_E_HeaderOffset, m_tag, "GlyphClassDef");
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }

                if (AttachListOffset != 0)
                {
                    if (AttachListOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.GDEF_HeaderOffsets, E.GDEF_E_HeaderOffset, m_tag, "AttachList");
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }

                if (LigCaretListOffset != 0)
                {
                    if (LigCaretListOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.GDEF_HeaderOffsets, E.GDEF_E_HeaderOffset, m_tag, "LigCaretList");
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }

                if (MarkAttachClassDefOffset != 0)
                {
                    if (MarkAttachClassDefOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.GDEF_HeaderOffsets, E.GDEF_E_HeaderOffset, m_tag, "MarkAttachClassDef");
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }

                if (MarkGlyphSetsDefOffset != 0)
                {
                    if (MarkGlyphSetsDefOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.GDEF_HeaderOffsets, E.GDEF_E_HeaderOffset, m_tag, "MarkGlyphSetsDef");
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }

                if (bOffsetsOk == true)
                {
                    v.Pass(T.GDEF_HeaderOffsets, P.GDEF_P_HeaderOffsets, m_tag);
                }
            }

            if (v.PerformTest(T.GDEF_Subtables))
            {
                if (GlyphClassDefOffset != 0)
                {
                    ClassDefTable_val cdt = GetGlyphClassDefTable_val();
                    cdt.Validate(v, "GlyphClassDef", this);
                }

                if (AttachListOffset != 0)
                {
                    AttachListTable_val alt = GetAttachListTable_val();
                    alt.Validate(v, "AttachList", this);
                }

                if (LigCaretListOffset != 0)
                {
                    LigCaretListTable_val lclt = GetLigCaretListTable_val();
                    lclt.Validate(v, "LigCaretList", this);
                }

                if (MarkAttachClassDefOffset != 0)
                {
                    ClassDefTable_val macdt = GetMarkAttachClassDefTable_val();
                    macdt.Validate(v, "MarkAttachClassDef", this);
                }

                if (MarkGlyphSetsDefOffset != 0)
                {
                    MarkGlyphSetsDefTable_val mgsdt = GetMarkGlyphSetsDefTable_val();
                    mgsdt.Validate(v, "MarkGlyphSetsDef", this);
                }
            }

            return bRet;
        }

        /************************
         * nested classes
         */

        public class AttachListTable_val : AttachListTable, I_OTLValidate
        {
            public AttachListTable_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_GDEF)table).ValidateNoOverlap(m_offsetAttachListTable, CalcLength(), v, sIdentity, table.GetTag());

                if (CoverageOffset == 0)
                {
                    v.Error(T.T_NULL, E.GDEF_E_AttachListTable_CoverageOffset_0, table.m_tag, sIdentity);
                    bRet = false;
                }
                else if (CoverageOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GDEF_E_AttachListTable_CoverageOffset_invalid, table.m_tag, sIdentity);
                    bRet = false;
                }
                else
                {
                    v.Pass(T.T_NULL, P.GDEF_P_AttachListTable_CoverageOffset, table.m_tag, sIdentity);
                    CoverageTable_val ct = GetCoverageTable_val();
                    ct.Validate(v, sIdentity + ", Coverage", table);
                }



                if (m_offsetAttachListTable + (uint)FieldOffsets.AttachPointOffsets + GlyphCount*2 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GDEF_E_AttachListTable_AttachPointArray_PastEOT, table.m_tag, sIdentity);
                    bRet = false;
                }

                bool bOffsetsOk = true;
                for (uint i=0; i<GlyphCount; i++)
                {
                    if (m_offsetAttachListTable + GetAttachPointOffset(i) > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GDEF_E_AttachListTable_AttachPointOffset, table.m_tag, sIdentity + ", index = " + i);
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }
                if (bOffsetsOk)
                {
                    v.Pass(T.T_NULL, P.GDEF_P_AttachListTable_AttachPointArray, table.m_tag, sIdentity);
                }

                return bRet;
            }


            public CoverageTable_val GetCoverageTable_val()
            {
                ushort offset = (ushort)(m_offsetAttachListTable + CoverageOffset);
                return new CoverageTable_val(offset, m_bufTable);
            }
        }

        public class LigCaretListTable_val : LigCaretListTable, I_OTLValidate
        {
            public LigCaretListTable_val(ushort offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_GDEF)table).ValidateNoOverlap(m_offsetLigCaretListTable, CalcLength(), v, sIdentity, table.GetTag());

                if (CoverageOffset == 0)
                {
                    v.Error(T.T_NULL, E.GDEF_E_LigCaretListTable_CoverageOffset_0, table.m_tag, sIdentity);
                    bRet = false;
                }
                else if (CoverageOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GDEF_E_LigCaretListTable_CoverageOffset_invalid, table.m_tag, sIdentity);
                    bRet = false;
                }
                else
                {
                    v.Pass(T.T_NULL, P.GDEF_P_LigCaretListTable_CoverageOffset, table.m_tag, sIdentity);
                    CoverageTable_val ct = GetCoverageTable_val();
                    ct.Validate(v, sIdentity + ", Coverage", table);
                }



                if (m_offsetLigCaretListTable + (uint)FieldOffsets.LigGlyphOffsets + LigGlyphCount*2 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GDEF_E_LigCaretListTable_LigGlyphArray_PastEOT, table.m_tag, sIdentity);
                    bRet = false;
                }

                bool bOffsetsOk = true;
                for (uint i=0; i<LigGlyphCount; i++)
                {
                    if (m_offsetLigCaretListTable + GetLigGlyphOffset(i) > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GDEF_E_LigCaretListTable_LigGlyphOffset, table.m_tag, sIdentity + ", index = " + i);
                        bOffsetsOk = false;
                        bRet = false;
                    }
                    else
                    {
                        LigGlyphTable_val lgt = GetLigGlyphTable_val(i);
                        lgt.Validate(v, sIdentity + ", LigGlyph[" + i + "]", table);
                    }
                }
                if (bOffsetsOk)
                {
                    v.Pass(T.T_NULL, P.GDEF_P_LigCaretListTable_LigGlyphArray, table.m_tag, sIdentity);
                }


                return bRet;
            }


            public CoverageTable_val GetCoverageTable_val()
            {
                ushort offset = (ushort)(m_offsetLigCaretListTable + CoverageOffset);
                return new CoverageTable_val(offset, m_bufTable);
            }

            public LigGlyphTable_val GetLigGlyphTable_val(uint i)
            {
                ushort offset = GetLigGlyphOffset(i);                 
                return new LigGlyphTable_val((ushort)(m_offsetLigCaretListTable + offset), m_bufTable);
            }
        }

        public class LigGlyphTable_val : LigGlyphTable
        {
            public LigGlyphTable_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_GDEF)table).ValidateNoOverlap(m_offsetLigGlyphTable, CalcLength(), v, sIdentity, table.GetTag());

                if (m_offsetLigGlyphTable + (uint)FieldOffsets.CaretValueOffsets + CaretCount*2 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GDEF_E_LigGlyphTable_CaretValueArray_PastEOT, table.m_tag, sIdentity);
                    bRet = false;
                }

                bool bOffsetsOk = true;
                for (uint i=0; i<CaretCount; i++)
                {
                    if (m_offsetLigGlyphTable + GetCaretValueOffset(i) > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GDEF_E_LigGlyphTable_CaretValueOffset, table.m_tag, sIdentity);
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }
                if (bOffsetsOk)
                {
                    v.Pass(T.T_NULL, P.GDEF_P_LigGlyphTable_CaretValueArray, table.m_tag, sIdentity);
                }

                for (uint i=0; i<CaretCount; i++)
                {
                    CaretValueTable cvt = GetCaretValueTable(i);
                    if (cvt.CaretValueFormat < 1 || cvt.CaretValueFormat > 3)
                    {
                        v.Error(T.T_NULL, E.GDEF_E_CaretValueTable_Format, table.m_tag, sIdentity + ", CaretValue[" + i + "], format = " + cvt.CaretValueFormat);
                        bRet = false;
                    }
                }

                return bRet;
            }
        }

        public class MarkGlyphSetsDefTable_val : MarkGlyphSetsDefTable, I_OTLValidate
        {
            public MarkGlyphSetsDefTable_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                bRet &= ((val_GDEF)table).ValidateNoOverlap(m_offsetMarkGlyphSetsDefTable, CalcLength(), v, sIdentity, table.GetTag());

                if (MarkSetTableFormat != 1)
                {
                    v.Error(T.T_NULL, E.GDEF_E_MarkSetTableFormat, table.m_tag,
                            sIdentity + ": MarkSetTableFormat=" + MarkSetTableFormat.ToString());
                    bRet = false;
                }

                if (m_offsetMarkGlyphSetsDefTable + CalcLength() > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GDEF_E_MarkGlyphSetsDefTable_PastEOT, table.m_tag, sIdentity);
                    bRet = false;
                }

                v.Info(T.T_NULL, I.GDEF_I_MarkSetCount, table.m_tag, sIdentity + ": MarkSetCount=" + MarkSetCount);
                // TODO: check Coverage [MarkSetCount] array?

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GDEF_P_MarkGlyphSetsDefTable, table.m_tag, sIdentity);
                }

                return bRet;
            }
        }

        public ClassDefTable_val GetGlyphClassDefTable_val()
        {
            ClassDefTable_val cdt = null;

            if (GlyphClassDefOffset != 0)
            {
                cdt = new ClassDefTable_val(GlyphClassDefOffset, m_bufTable);
            }

            return cdt;
        }

        public AttachListTable_val GetAttachListTable_val()
        {
            AttachListTable_val alt = null;

            if (AttachListOffset != 0)
            {
                alt = new AttachListTable_val(AttachListOffset, m_bufTable);
            }

            return alt;
        }

        public LigCaretListTable_val GetLigCaretListTable_val()
        {
            LigCaretListTable_val lclt = null;

            if (LigCaretListOffset != 0)
            {
                lclt = new LigCaretListTable_val(LigCaretListOffset, m_bufTable);
            }

            return lclt;
        }

        public ClassDefTable_val GetMarkAttachClassDefTable_val()
        {
            ClassDefTable_val cdt = null;

            if (MarkAttachClassDefOffset != 0)
            {
                cdt = new ClassDefTable_val(MarkAttachClassDefOffset, m_bufTable);
            }

            return cdt;
        }

        public MarkGlyphSetsDefTable_val GetMarkGlyphSetsDefTable_val()
        {
            MarkGlyphSetsDefTable_val mgsdt = null;

            if (MarkGlyphSetsDefOffset != 0)
            {
                mgsdt = new MarkGlyphSetsDefTable_val(MarkGlyphSetsDefOffset, m_bufTable);
            }

            return mgsdt;
        }

        public bool ValidateNoOverlap(uint offset, uint length, Validator v, string sIdentity, OTTag tag)
        {
            bool bValid = m_DataOverlapDetector.CheckForNoOverlap(offset, length);

            if (!bValid)
            {
                // JJF. Figure out what to do
                v.Error(T.T_NULL, E._Table_E_DataOverlap, tag, sIdentity + ", offset = " + offset + ", length = " + length);
            }

            return bValid;
        }

        protected DataOverlapDetector m_DataOverlapDetector;
    }
}
