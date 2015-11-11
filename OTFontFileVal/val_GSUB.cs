using System;

using OTFontFile;
using OTFontFile.OTL;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_GSUB.
    /// </summary>
    public class val_GSUB : Table_GSUB, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_GSUB(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
            m_DataOverlapDetector = new DataOverlapDetector();
        }

        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;


            if (v.PerformTest(T.GSUB_Version))
            {
                if (Version.GetUint() == 0x00010000)
                {
                    v.Pass(T.GSUB_Version, P.GSUB_P_Version, m_tag);
                }
                else
                {
                    v.Error(T.GSUB_Version, E.GSUB_E_Version, m_tag, "0x" + Version.GetUint().ToString("x8"));
                }
            }

            bool bScriptListOffsetOK = true;
            bool bFeatureListOffsetOK = true;
            bool bLookupListOffsetOK = true;


            if (v.PerformTest(T.GSUB_HeaderOffsets))
            {
                if (ScriptListOffset == 0)
                {
                    v.Error(T.GSUB_HeaderOffsets, E.GSUB_E_HeaderOffset_0, m_tag, "ScriptListOffset");
                    bScriptListOffsetOK = false;
                    bRet = false;
                }
                else
                {
                    if (ScriptListOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.GSUB_HeaderOffsets, E.GSUB_E_HeaderOffset, m_tag, "ScriptListOffset");
                        bScriptListOffsetOK = false;
                        bRet = false;
                    }
                }

                if (FeatureListOffset == 0)
                {
                    v.Error(T.GSUB_HeaderOffsets, E.GSUB_E_HeaderOffset_0, m_tag, "FeatureListOffset");
                    bFeatureListOffsetOK = false;
                    bRet = false;
                }
                else
                {
                    if (FeatureListOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.GSUB_HeaderOffsets, E.GSUB_E_HeaderOffset, m_tag, "FeatureListOffset");
                        bFeatureListOffsetOK = false;
                        bRet = false;
                    }
                }

                if (LookupListOffset == 0)
                {
                    v.Error(T.GSUB_HeaderOffsets, E.GSUB_E_HeaderOffset_0, m_tag, "LookupListOffset");
                    bLookupListOffsetOK = false;
                    bRet = false;
                }
                else
                {
                    if (LookupListOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.GSUB_HeaderOffsets, E.GSUB_E_HeaderOffset, m_tag, "LookupListOffset");
                        bLookupListOffsetOK = false;
                        bRet = false;
                    }
                }


                if (bScriptListOffsetOK == true && bFeatureListOffsetOK == true && bLookupListOffsetOK == true)
                {
                    v.Pass(T.GSUB_HeaderOffsets, P.GSUB_P_HeaderOffsets, m_tag);
                }
            }

            if (v.PerformTest(T.GSUB_Subtables))
            {
                if (bScriptListOffsetOK)
                {
                    ScriptListTable_val slt = GetScriptListTable_val();
                    if (slt != null)
                    {
                        bRet &= slt.Validate(v, "ScriptList", this);
                    }
                }

                if (bFeatureListOffsetOK)
                {
                    FeatureListTable_val flt = GetFeatureListTable_val();
                    if (flt != null)
                    {
                        bRet &= flt.Validate(v, "FeatureList", this);
                    }
                }

                if (bLookupListOffsetOK)
                {
                    LookupListTable_val llt = GetLookupListTable_val();
                    if (llt != null)
                    {
                        bRet &= llt.Validate(v, "LookupList", this);
                    }
                }
            }

            return bRet;
        }

        
        /************************
         * classes
         */


        // Lookup Type 1: Single Substitution Subtable

        public class SingleSubst_val : SingleSubst, I_OTLValidate
        {
            public SingleSubst_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(SingleSubst, fmt " + SubstFormat + ")";


                if (SubstFormat == 1)
                {
                    SingleSubstFormat1_val f1 = GetSingleSubstFormat1_val();
                    bRet &= f1.Validate(v, sIdentity, table);
                }
                else if (SubstFormat == 2)
                {
                    SingleSubstFormat2_val f2 = GetSingleSubstFormat2_val();
                    bRet &= f2.Validate(v, sIdentity, table);
                }
                else
                {
                    v.Error(T.T_NULL, E.GSUB_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GSUB_P_SingleSubst, table.m_tag, sIdentity);
                }


                return bRet;
            }


            // nested classes

            public class SingleSubstFormat1_val : SingleSubstFormat1
            {
                public SingleSubstFormat1_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetSingleSubst, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetSingleSubst + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                    return bRet;
                }
                
                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetSingleSubst + CoverageOffset, m_bufTable);
                }

            }

            public class SingleSubstFormat2_val : SingleSubstFormat2
            {
                public SingleSubstFormat2_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetSingleSubst, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetSingleSubst + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                    // check the Substitute array length
                    if (m_offsetSingleSubst + (uint)FieldOffsets.SubstituteGlyphIDs + GlyphCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Substitute array");
                        bRet = false;
                    }

                    return bRet;
                }
                
                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetSingleSubst + CoverageOffset, m_bufTable);
                }

            }

            public SingleSubstFormat1_val GetSingleSubstFormat1_val()
            {
                if (SubstFormat != 1)
                {
                    throw new System.InvalidOperationException();
                }

                return new SingleSubstFormat1_val(m_offsetSingleSubst, m_bufTable);
            }

            public SingleSubstFormat2_val GetSingleSubstFormat2_val()
            {
                if (SubstFormat != 2)
                {
                    throw new System.InvalidOperationException();
                }

                return new SingleSubstFormat2_val(m_offsetSingleSubst, m_bufTable);
            }

        }

        // Lookup Type 2: Multiple Substitution Subtable

        public class MultipleSubst_val : MultipleSubst, I_OTLValidate
        {
            public MultipleSubst_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(MultipleSubst, fmt " + SubstFormat + ")";

                if (SubstFormat == 1)
                {
                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetMultipleSubst, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetMultipleSubst + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                    // check the sequence array length
                    if (m_offsetMultipleSubst + (uint)FieldOffsets.SequenceOffsets + SequenceCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Sequence array");
                        bRet = false;
                    }

                    // check each sequence offset
                    for (uint i=0; i<SequenceCount; i++)
                    {
                        if (m_offsetMultipleSubst + GetSequenceOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Sequence[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each sequence table
                    for (uint i=0; i<SequenceCount; i++)
                    {
                        SequenceTable_val st = GetSequenceTable_val(i);
                        bRet &= st.Validate(v, sIdentity + ", Sequence[" + i + "]", table);
                    }
                }
                else
                {
                    v.Error(T.T_NULL, E.GSUB_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GSUB_P_MultipleSubst, table.m_tag, sIdentity);
                }

                return bRet;
            }


            public CoverageTable_val GetCoverageTable_val()
            {
                return new CoverageTable_val(m_offsetMultipleSubst + CoverageOffset, m_bufTable);
            }

            public class SequenceTable_val : SequenceTable
            {
                public SequenceTable_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetSequence, CalcLength(), v, sIdentity, table.GetTag());

                    // check the substitute array length
                    if (m_offsetSequence + (uint)FieldOffsets.SubstituteGlyphIDs + GlyphCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Substitute array");
                        bRet = false;
                    }

                    return bRet;
                }
            }

            public SequenceTable_val GetSequenceTable_val(uint i)
            {
                SequenceTable_val st = null;

                if (i < SequenceCount)
                {
                    st = new SequenceTable_val(m_offsetMultipleSubst + GetSequenceOffset(i), m_bufTable);
                }

                return st;
            }
        }

        // Lookup Type 3: Alternate Substitution Subtable

        public class AlternateSubst_val : AlternateSubst, I_OTLValidate
        {
            public AlternateSubst_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;


                sIdentity += "(AlternateSubst, fmt " + SubstFormat + ")";

                if (SubstFormat == 1)
                {
                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetAlternateSubst, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetAlternateSubst + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                    // check the AlternateSet array length
                    if (m_offsetAlternateSubst + (uint)FieldOffsets.AlternateSetOffsets + AlternateSetCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", AlternateSet array");
                        bRet = false;
                    }

                    // check each AlternateSet offset
                    for (uint i=0; i<AlternateSetCount; i++)
                    {
                        if (m_offsetAlternateSubst + GetAlternateSetOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", AlternateSet[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each AlternateSet table
                    for (uint i=0; i<AlternateSetCount; i++)
                    {
                        AlternateSet_val altset = GetAlternateSetTable_val(i);
                        bRet &= altset.Validate(v, sIdentity + ", AlternateSet[" + i + "]", table);
                    }
                }
                else
                {
                    v.Error(T.T_NULL, E.GSUB_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GSUB_P_AlternateSubst, table.m_tag, sIdentity);
                }

                return bRet;
            }


            public CoverageTable_val GetCoverageTable_val()
            {
                return new CoverageTable_val(m_offsetAlternateSubst + CoverageOffset, m_bufTable);
            }

            public class AlternateSet_val : AlternateSet, I_OTLValidate
            {
                public AlternateSet_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetAlternateSet, CalcLength(), v, sIdentity, table.GetTag());

                    // check the alternate array length
                    if (m_offsetAlternateSet + (uint)FieldOffsets.AlternateGlyphIDs + GlyphCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Alternate array");
                        bRet = false;
                    }

                    return bRet;
                }
            }

            public AlternateSet_val GetAlternateSetTable_val(uint i)
            {
                AlternateSet_val aset = null;

                if (i < AlternateSetCount)
                {
                    aset = new AlternateSet_val(m_offsetAlternateSubst + GetAlternateSetOffset(i), m_bufTable);
                }

                return aset;
            }
        }

        // Lookup Type 4: Ligature Substitution Subtable

        public class LigatureSubst_val : LigatureSubst, I_OTLValidate
        {
            public LigatureSubst_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;


                sIdentity += "(LigatureSubst, fmt " + SubstFormat + ")";

                if (SubstFormat == 1)
                {
                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetLigatureSubst, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetLigatureSubst + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                    // check the LigatureSet array length
                    if (m_offsetLigatureSubst + (uint)FieldOffsets.LigatureSetOffsets + LigSetCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", LigatureSet array");
                        bRet = false;
                    }

                    // check each LigatureSet offset
                    for (uint i=0; i<LigSetCount; i++)
                    {
                        if (m_offsetLigatureSubst + GetLigatureSetOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", LigatureSet[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each LigatureSet table
                    for (uint i=0; i<LigSetCount; i++)
                    {
                        LigatureSet_val ls = GetLigatureSetTable_val(i);
                        bRet &= ls.Validate(v, sIdentity + ", LigatureSet[" + i + "]", table);
                    }
                }
                else
                {
                    v.Error(T.T_NULL, E.GSUB_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GSUB_P_LigatureSubst, table.m_tag, sIdentity);
                }

                return bRet;
            }


            public CoverageTable_val GetCoverageTable_val()
            {
                return new CoverageTable_val(m_offsetLigatureSubst + CoverageOffset, m_bufTable);
            }

            public class LigatureSet_val : LigatureSet, I_OTLValidate
            {
                public LigatureSet_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetLigatureSet, CalcLength(), v, sIdentity, table.GetTag());

                    // check the Ligature array length
                    if (m_offsetLigatureSet + (uint)FieldOffsets.LigatureOffsets + LigatureCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Ligature array");
                        bRet = false;
                    }

                    // check each Ligature offset
                    for (uint i=0; i<LigatureCount; i++)
                    {
                        if (m_offsetLigatureSet + GetLigatureOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Ligature[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each Ligature table
                    for (uint i=0; i<LigatureCount; i++)
                    {
                        Ligature_val lt = GetLigatureTable_val(i);
                        bRet &= lt.Validate(v, sIdentity + ", Ligature[" + i + "]", table);
                    }

                    return bRet;
                }
                
                public class Ligature_val : Ligature, I_OTLValidate
                {
                    public Ligature_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetLigature, CalcLength(), v, sIdentity, table.GetTag());

                        // check the component array length
                        if (m_offsetLigature + (uint)FieldOffsets.ComponentGlyphIDs + (CompCount-1)*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Component array");
                            bRet = false;
                        }

                        return bRet;
                    }
                
                }

                public Ligature_val GetLigatureTable_val(uint i)
                {
                    Ligature_val l = null;

                    if (i < LigatureCount)
                    {
                        l = new Ligature_val(m_offsetLigatureSet + GetLigatureOffset(i), m_bufTable);
                    }

                    return l;
                }
            }

            public LigatureSet_val GetLigatureSetTable_val(uint i)
            {
                LigatureSet_val ls = null;

                if (i < LigSetCount)
                {
                    ls = new LigatureSet_val(m_offsetLigatureSubst + GetLigatureSetOffset(i), m_bufTable);
                }

                return ls;
            }
        }

        // Lookup Type 5: Contextual Substitution Subtable

        public class ContextSubst_val : ContextSubst, I_OTLValidate
        {
            public ContextSubst_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(ContextSubst, fmt " + SubstFormat + ")";


                if (SubstFormat == 1)
                {
                    ContextSubstFormat1_val f1 = GetContextSubstFormat1_val();
                    bRet &= f1.Validate(v, sIdentity, table);
                }
                else if (SubstFormat == 2)
                {
                    ContextSubstFormat2_val f2 = GetContextSubstFormat2_val();
                    bRet &= f2.Validate(v, sIdentity, table);
                }
                else if (SubstFormat == 3)
                {
                    ContextSubstFormat3_val f3 = GetContextSubstFormat3_val();
                    bRet &= f3.Validate(v, sIdentity, table);
                }
                else
                {
                    v.Error(T.T_NULL, E.GSUB_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GSUB_P_ContextSubst, table.m_tag, sIdentity);
                }

                return bRet;
            }


            public class ContextSubstFormat1_val : ContextSubstFormat1, I_OTLValidate
            {
                public ContextSubstFormat1_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetContextSubst, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetContextSubst + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                    // check the SubRuleSet array length
                    if (m_offsetContextSubst + (uint)FieldOffsets.SubRuleSetOffsets + SubRuleSetCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", SubRuleSet array");
                        bRet = false;
                    }

                    // check each SubRuleSet offset
                    for (uint i=0; i<SubRuleSetCount; i++)
                    {
                        if (m_offsetContextSubst + GetSubRuleSetOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", SubRuleSet[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each SubRuleSet table
                    for (uint i=0; i<SubRuleSetCount; i++)
                    {
                        SubRuleSet_val srs = GetSubRuleSetTable_val(i);
                        bRet &= srs.Validate(v, sIdentity + ", SubRuleSet[" + i + "]", table);
                    }

                    return bRet;
                }
                
                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetContextSubst + CoverageOffset, m_bufTable);
                }

                public SubRuleSet_val GetSubRuleSetTable_val(uint i)
                {
                    return new SubRuleSet_val(m_offsetContextSubst + GetSubRuleSetOffset(i), m_bufTable);
                }

                public class SubRuleSet_val : SubRuleSet, I_OTLValidate
                {
                    public SubRuleSet_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetSubRuleSet, CalcLength(), v, sIdentity, table.GetTag());

                        // check the SubRule array length
                        if (m_offsetSubRuleSet + (uint)FieldOffsets.SubRuleOffsets + SubRuleCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", SubRule array");
                            bRet = false;
                        }

                        // check each SubRule offset
                        for (uint i=0; i<SubRuleCount; i++)
                        {
                            if (m_offsetSubRuleSet + GetSubRuleOffset(i) > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", SubRule[" + i + "]");
                                bRet = false;
                            }
                        }

                        // check each SubRule table
                        for (uint i=0; i<SubRuleCount; i++)
                        {
                            SubRule_val sr = GetSubRuleTable_val(i);
                            bRet &= sr.Validate(v, sIdentity + ", SubRule[" + i + "]", table);
                        }

                        return bRet;
                    }
                    
                    public class SubRule_val : SubRule, I_OTLValidate
                    {
                        public SubRule_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                        {
                        }

                        public bool Validate(Validator v, string sIdentity, OTTable table)
                        {
                            bool bRet = true;
                            
                            // check for data overlap
                            bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetSubRule, CalcLength(), v, sIdentity, table.GetTag());

                            // check the Input array length
                            if (m_offsetSubRule + (uint)FieldOffsets.InputGlyphIds + (GlyphCount-1)*2 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Input array");
                                bRet = false;
                            }

                            // check the SubstLookupRecord array length
                            uint offset = m_offsetSubRule + (uint)FieldOffsets.InputGlyphIds + (uint)(GlyphCount-1)*2;
                            if (offset + SubstCount*4 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", SubstLookupRecord array");
                                bRet = false;
                            }

                            // check each SubstLookupRecord
                            for (uint i=0; i<SubstCount; i++)
                            {
                                SubstLookupRecord slr = GetSubstLookupRecord(i);
                                if (slr.SequenceIndex > GlyphCount)
                                {
                                    string sValues = ", slr.SequenceIndex = " + slr.SequenceIndex + ", slr.LookupListIndex = " + slr.LookupListIndex + ", GlyphCount = " + GlyphCount;
                                    v.Error(T.T_NULL, E.GSUB_E_SubstLookupRecord_SequenceIndex, table.m_tag, sIdentity + ", SubstLookupRecord[" + i + "]" + sValues);
                                    bRet = false;
                                }
                            }


                            return bRet;
                        }
                    }

                    public SubRule_val GetSubRuleTable_val(uint i)
                    {
                        SubRule_val sr = null;

                        if (i < SubRuleCount)
                        {
                            sr = new SubRule_val(m_offsetSubRuleSet + GetSubRuleOffset(i), m_bufTable);
                        }

                        return sr;
                    }
                }
            }

            public class ContextSubstFormat2_val : ContextSubstFormat2, I_OTLValidate
            {
                public ContextSubstFormat2_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetContextSubst, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetContextSubst + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage", table);

                    // check the ClassDef offset
                    if (m_offsetContextSubst + ClassDefOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", ClassDef offset");
                        bRet = false;
                    }

                    // check the ClassDef table
                    ClassDefTable_val cdt = GetClassDefTable_val();
                    bRet &= cdt.Validate(v, sIdentity + ", ClassDef", table);

                    // check the SubClassSet array length
                    if (m_offsetContextSubst + (uint)FieldOffsets.SubClassSetOffsets + SubClassSetCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", SubClassSet array");
                        bRet = false;
                    }

                    // check each SubClassSet offset
                    for (uint i=0; i<SubClassSetCount; i++)
                    {
                        if (m_offsetContextSubst + GetSubClassSetOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", SubClassSet[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each SubClassSet table
                    for (uint i=0; i<SubClassSetCount; i++)
                    {
                        if (GetSubClassSetOffset(i) != 0)
                        {
                            SubClassSet_val scs = GetSubClassSetTable_val(i);
                            if (scs != null)
                            {
                                bRet &= scs.Validate(v, sIdentity + ", SubClassSet[" + i + "]", table);
                            }
                        }
                    }

                    return bRet;
                }
                
                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetContextSubst + CoverageOffset, m_bufTable);
                }

                public ClassDefTable_val GetClassDefTable_val()
                {
                    return new ClassDefTable_val(m_offsetContextSubst + ClassDefOffset, m_bufTable);
                }

                public SubClassSet_val GetSubClassSetTable_val(uint i)
                {
                    SubClassSet_val scs = null;
                    if (GetSubClassSetOffset(i) != 0)
                    {
                        scs =  new SubClassSet_val(m_offsetContextSubst + GetSubClassSetOffset(i), m_bufTable);
                    }
                    return scs;
                }

                public class SubClassSet_val : SubClassSet, I_OTLValidate
                {
                    public SubClassSet_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetSubClassSet, CalcLength(), v, sIdentity, table.GetTag());

                        // check the SubClassRule array length
                        if (m_offsetSubClassSet + (uint)FieldOffsets.SubClassRuleOffsets + SubClassRuleCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", SubClassRule array");
                            bRet = false;
                        }

                        // check each SubClassRule offset
                        for (uint i=0; i<SubClassRuleCount; i++)
                        {
                            if (m_offsetSubClassSet + GetSubClassRuleOffset(i) > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", SubClassRule[" + i + "]");
                                bRet = false;
                            }
                        }

                        // check each SubClassRule table
                        for (uint i=0; i<SubClassRuleCount; i++)
                        {
                            SubClassRule_val scr = GetSubClassRuleTable_val(i);
                            bRet &= scr.Validate(v, sIdentity + ", SubClassRule[" + i + "]", table);
                        }

                        return bRet;
                    }
                    
                    public class SubClassRule_val : SubClassRule, I_OTLValidate
                    {
                        public SubClassRule_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                        {
                        }


                        public bool Validate(Validator v, string sIdentity, OTTable table)
                        {
                            bool bRet = true;

                            // check for data overlap
                            bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetSubClassRule, CalcLength(), v, sIdentity, table.GetTag());

                            // check the Class array length
                            if (m_offsetSubClassRule + (uint)FieldOffsets.ClassArray + (GlyphCount-1)*2 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Class array");
                                bRet = false;
                            }

                            // check the SubstLookupRecord array length
                            uint offset = m_offsetSubClassRule + (uint)FieldOffsets.ClassArray + (uint)(GlyphCount-1)*2;
                            if (offset + SubstCount*4 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", SubstLookupRecord array");
                                bRet = false;
                            }

                            // check each SubstLookupRecord
                            for (uint i=0; i<SubstCount; i++)
                            {
                                SubstLookupRecord slr = GetSubstLookupRecord(i);
                                if (slr.SequenceIndex > GlyphCount)
                                {
                                    string sValues = ", slr.SequenceIndex = " + slr.SequenceIndex + ", slr.LookupListIndex = " + slr.LookupListIndex + ", GlyphCount = " + GlyphCount;
                                    v.Error(T.T_NULL, E.GSUB_E_SubstLookupRecord_SequenceIndex, table.m_tag, sIdentity + ", SubstLookupRecord[" + i + "]" + sValues);
                                    bRet = false;
                                }
                            }

                            return bRet;
                        }
                    }

                    public SubClassRule_val GetSubClassRuleTable_val(uint i)
                    {
                        SubClassRule_val scr = null;

                        if (i < SubClassRuleCount)
                        {
                            scr = new SubClassRule_val(m_offsetSubClassSet + GetSubClassRuleOffset(i), m_bufTable);
                        }

                        return scr;
                    }
                }
            }

            public class ContextSubstFormat3_val : ContextSubstFormat3, I_OTLValidate
            {
                public ContextSubstFormat3_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetContextSubst, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage array length
                    if (m_offsetContextSubst + (uint)FieldOffsets.CoverageOffsets + GlyphCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", coverage array");
                        bRet = false;
                    }

                    // check each coverage offset
                    for (uint i=0; i<GlyphCount; i++)
                    {
                        if (m_offsetContextSubst + GetCoverageOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each coverage table
                    for (uint i=0; i<GlyphCount; i++)
                    {
                        CoverageTable_val ct = GetCoverageTable_val(i);
                        bRet &= ct.Validate(v, sIdentity + ", Coverage[" + i + "]", table);
                    }

                    // check the SubstLookupRecord array length
                    uint offset = m_offsetContextSubst + (uint)FieldOffsets.CoverageOffsets + (uint)GlyphCount*2;
                    if (offset + SubstCount*4 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", SubstLookupRecord array");
                        bRet = false;
                    }

                    // check each SubstLookupRecord
                    for (uint i=0; i<SubstCount; i++)
                    {
                        SubstLookupRecord slr = GetSubstLookupRecord(i);
                        if (slr.SequenceIndex > GlyphCount)
                        {
                            string sValues = ", slr.SequenceIndex = " + slr.SequenceIndex + ", slr.LookupListIndex = " + slr.LookupListIndex + ", GlyphCount = " + GlyphCount;
                            v.Error(T.T_NULL, E.GSUB_E_SubstLookupRecord_SequenceIndex, table.m_tag, sIdentity + ", SubstLookupRecord[" + i + "]" + sValues);
                            bRet = false;
                        }
                    }

                    return bRet;
                }
                
                public CoverageTable_val GetCoverageTable_val(uint i)
                {
                    return new CoverageTable_val(m_offsetContextSubst + GetCoverageOffset(i), m_bufTable);
                }
            }


            public ContextSubstFormat1_val GetContextSubstFormat1_val()
            {
                if (SubstFormat != 1)
                {
                    throw new System.InvalidOperationException();
                }

                return new ContextSubstFormat1_val(m_offsetContextSubst, m_bufTable);
            }

            public ContextSubstFormat2_val GetContextSubstFormat2_val()
            {
                if (SubstFormat != 2)
                {
                    throw new System.InvalidOperationException();
                }

                return new ContextSubstFormat2_val(m_offsetContextSubst, m_bufTable);
            }

            public ContextSubstFormat3_val GetContextSubstFormat3_val()
            {
                if (SubstFormat != 3)
                {
                    throw new System.InvalidOperationException();
                }

                return new ContextSubstFormat3_val(m_offsetContextSubst, m_bufTable);
            }
        }

        // Lookup Type 6: Chaining Contextual Substitution Subtable

        public class ChainContextSubst_val : ChainContextSubst, I_OTLValidate
        {
            public ChainContextSubst_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(ChainContextSubst, fmt " + SubstFormat + ")";


                if (SubstFormat == 1)
                {
                    ChainContextSubstFormat1_val f1 = GetChainContextSubstFormat1_val();
                    bRet &= f1.Validate(v, sIdentity, table);
                }
                else if (SubstFormat == 2)
                {
                    ChainContextSubstFormat2_val f2 = GetChainContextSubstFormat2_val();
                    bRet &= f2.Validate(v, sIdentity, table);
                }
                else if (SubstFormat == 3)
                {
                    ChainContextSubstFormat3_val f3 = GetChainContextSubstFormat3_val();
                    bRet &= f3.Validate(v, sIdentity, table);
                }
                else
                {
                    v.Error(T.T_NULL, E.GSUB_E_SubtableFormat, table.m_tag, sIdentity);
                    bRet = false;
                }

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GSUB_P_ChainContextSubst, table.m_tag, sIdentity);
                }


                return bRet;
            }


            // nested classes

            public class ChainContextSubstFormat1_val : ChainContextSubstFormat1, I_OTLValidate
            {
                public ChainContextSubstFormat1_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetChainContextSubst, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetChainContextSubst + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage table", table);

                    // check the ChainSubRuleSet array length
                    if (m_offsetChainContextSubst + (uint)FieldOffsets.ChainSubRuleSetOffsets + ChainSubRuleSetCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", ChainSubRuleSet array");
                        bRet = false;
                    }

                    // check each ChainSubRuleSet offset
                    for (uint i=0; i<ChainSubRuleSetCount; i++)
                    {
                        if (m_offsetChainContextSubst + GetChainSubRuleSetOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", ChainSubRuleSet[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each ChainSubRuleSet table
                    for (uint i=0; i<ChainSubRuleSetCount; i++)
                    {
                        ChainSubRuleSet_val csrs = GetChainSubRuleSetTable_val(i);
                        bRet &= csrs.Validate(v, sIdentity + ", ChainSubRuleSet[" + i + "]", table);
                    }

                    return bRet;
                }
                
                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetChainContextSubst + CoverageOffset, m_bufTable);
                }

                public ChainSubRuleSet_val GetChainSubRuleSetTable_val(uint i)
                {
                    return new ChainSubRuleSet_val(m_offsetChainContextSubst + GetChainSubRuleSetOffset(i), m_bufTable);
                }

                public class ChainSubRuleSet_val : ChainSubRuleSet, I_OTLValidate
                {
                    public ChainSubRuleSet_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetChainSubRuleSet, CalcLength(), v, sIdentity, table.GetTag());

                        // check the ChainSubRule array length
                        if (m_offsetChainSubRuleSet + (uint)FieldOffsets.ChainSubRuleOffsets + ChainSubRuleCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", ChainSubRule array");
                            bRet = false;
                        }

                        // check each ChainSubRule offset
                        for (uint i=0; i<ChainSubRuleCount; i++)
                        {
                            if (m_offsetChainSubRuleSet + GetChainSubRuleOffset(i) > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", ChainSubRule[" + i + "]");
                                bRet = false;
                            }
                        }

                        // check each ChainSubRule table
                        for (uint i=0; i<ChainSubRuleCount; i++)
                        {
                            ChainSubRule_val csr = GetChainSubRuleTable_val(i);
                            bRet &= csr.Validate(v, sIdentity + ", ChainSubRule[" + i + "]", table);
                        }

                        return bRet;
                    }
                    
                    public class ChainSubRule_val : ChainSubRule, I_OTLValidate
                    {
                        public ChainSubRule_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
                        {
                        }

                        public bool Validate(Validator v, string sIdentity, OTTable table)
                        {
                            bool bRet = true;

                            // check for data overlap
                            bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetChainSubRule, CalcLength(), v, sIdentity, table.GetTag());

                            // check the backtrack array length
                            uint offset = m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphIDs;
                            if (offset + BacktrackGlyphCount*2 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Backtrack array");
                                bRet = false;
                            }

                            // check the input array length
                            offset = m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                + (uint)BacktrackGlyphCount*2 + 2;
                            if (offset + (InputGlyphCount-1)*2 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Input array");
                                bRet = false;
                            }

                            // check the Lookahead array length
                            offset = m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2;
                            if (offset + LookaheadGlyphCount*2 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Lookahead array");
                                bRet = false;
                            }

                            // check the SubstLookupRecord array length
                            offset = m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                                + (uint)LookaheadGlyphCount*2 + 2;
                            if (offset + SubstCount*4 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", SubstLookupRecord array");
                                bRet = false;
                            }

                            // check each SubstLookupRecord
                            for (uint i=0; i<SubstCount; i++)
                            {
                                SubstLookupRecord slr = GetSubstLookupRecord(i);
                                if (slr.SequenceIndex > InputGlyphCount)
                                {
                                    string sValues = ", slr.SequenceIndex = " + slr.SequenceIndex + ", slr.LookupListIndex = " + slr.LookupListIndex + ", InputGlyphCount = " + InputGlyphCount;
                                    v.Error(T.T_NULL, E.GSUB_E_SubstLookupRecord_SequenceIndex, table.m_tag, sIdentity + ", SubstLookupRecord[" + i + "]" + sValues);
                                    bRet = false;
                                }
                            }

                            return bRet;
                        }
                    
                    }

                    public ChainSubRule_val GetChainSubRuleTable_val(uint i)
                    {
                        ChainSubRule_val csr = null;

                        if (i < ChainSubRuleCount)
                        {
                            csr = new ChainSubRule_val(m_offsetChainSubRuleSet + GetChainSubRuleOffset(i), m_bufTable);
                        }

                        return csr;
                    }
                }
            }

            public class ChainContextSubstFormat2_val : ChainContextSubstFormat2, I_OTLValidate
            {
                public ChainContextSubstFormat2_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetChainContextSubst, CalcLength(), v, sIdentity, table.GetTag());

                    // check the coverage offset
                    if (m_offsetChainContextSubst + CoverageOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                        bRet = false;
                    }

                    // check the coverage table
                    CoverageTable_val ct = GetCoverageTable_val();
                    bRet &= ct.Validate(v, sIdentity + ", Coverage table", table);

                    // check the BacktrackClassDef offset
                    if (m_offsetChainContextSubst + BacktrackClassDefOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", BacktrackClassDef offset");
                        bRet = false;
                    }

                    // check the BacktrackClassDef table
                    ClassDefTable_val bcd = GetBacktrackClassDefTable_val();
                    bRet &= bcd.Validate(v, sIdentity + ", BacktrackClassDef", table);

                    // check the InputClassDef offset
                    if (m_offsetChainContextSubst + InputClassDefOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", InputClassDef offset");
                        bRet = false;
                    }

                    // check the InputClassDef table
                    ClassDefTable_val icd = GetInputClassDefTable_val();
                    bRet &= icd.Validate(v, sIdentity + ", InputClassDef", table);

                    // check the LookaheadClassDef offset
                    if (m_offsetChainContextSubst + LookaheadClassDefOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", LookaheadClassDef offset");
                        bRet = false;
                    }

                    // check the LookaheadClassDef table
                    ClassDefTable_val lcd = GetLookaheadClassDefTable_val();
                    bRet &= lcd.Validate(v, sIdentity + ", LookaheadClassDef", table);

                    // check the ChainSubClassSet array length
                    if (m_offsetChainContextSubst + (uint)FieldOffsets.ChainSubClassSetOffsets + ChainSubClassSetCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", ChainSubClassSet array");
                        bRet = false;
                    }

                    // check each ChainSubClassSet offset
                    for (uint i=0; i<ChainSubClassSetCount; i++)
                    {
                        if (GetChainSubClassSetOffset(i) != 0)
                        {
                            if (m_offsetChainContextSubst + GetChainSubClassSetOffset(i) > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", ChainSubClassSet[" + i + "]");
                                bRet = false;
                            }
                        }
                    }

                    // check each ChainSubClassSet table
                    for (uint i=0; i<ChainSubClassSetCount; i++)
                    {
                        if (GetChainSubClassSetOffset(i) != 0)
                        {
                            ChainSubClassSet_val cscs = GetChainSubClassSetTable_val(i);
                            bRet &= cscs.Validate(v, sIdentity + ", ChainSubClassSet[" + i + "]", table);
                        }
                    }

                    return bRet;
                }
                
                public CoverageTable_val GetCoverageTable_val()
                {
                    return new CoverageTable_val(m_offsetChainContextSubst + CoverageOffset, m_bufTable);
                }

                public ClassDefTable_val GetBacktrackClassDefTable_val()
                {
                    return new ClassDefTable_val(m_offsetChainContextSubst + BacktrackClassDefOffset, m_bufTable);
                }

                public ClassDefTable_val GetInputClassDefTable_val()
                {
                    return new ClassDefTable_val(m_offsetChainContextSubst + InputClassDefOffset, m_bufTable);
                }

                public ClassDefTable_val GetLookaheadClassDefTable_val()
                {
                    return new ClassDefTable_val(m_offsetChainContextSubst + LookaheadClassDefOffset, m_bufTable);
                }

                public ChainSubClassSet_val GetChainSubClassSetTable_val(uint i)
                {
                    ChainSubClassSet_val cscs = null;
                    if (GetChainSubClassSetOffset(i) != 0)
                    {
                        cscs = new ChainSubClassSet_val(m_offsetChainContextSubst + GetChainSubClassSetOffset(i), m_bufTable);
                    }
                    return cscs;
                }

                public class ChainSubClassSet_val : ChainSubClassSet, I_OTLValidate
                {
                    public ChainSubClassSet_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                    {
                    }

                    public bool Validate(Validator v, string sIdentity, OTTable table)
                    {
                        bool bRet = true;

                        // check for data overlap
                        bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetChainSubClassSet, CalcLength(), v, sIdentity, table.GetTag());

                        // check the ChainSubClassRule array length
                        if (m_offsetChainSubClassSet + (uint)FieldOffsets.ChainSubClassRuleOffsets + ChainSubClassRuleCount*2 > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", ChainSubClassRule array");
                            bRet = false;
                        }

                        // check each ChainSubClassRule offset
                        for (uint i=0; i<ChainSubClassRuleCount; i++)
                        {
                            if (m_offsetChainSubClassSet + GetChainSubClassRuleOffset(i) > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", ChainSubClassRule[" + i + "]");
                                bRet = false;
                            }
                        }

                        // check each ChainSubClassRule table
                        for (uint i=0; i<ChainSubClassRuleCount; i++)
                        {
                            ChainSubClassRule_val cscr = GetChainSubClassRuleTable_val(i);
                            bRet &= cscr.Validate(v, sIdentity + ", ChainSubClassRule[" + i + "]", table);
                        }

                        return bRet;
                    }
                    
                    public class ChainSubClassRule_val : ChainSubClassRule, I_OTLValidate
                    {
                        public ChainSubClassRule_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                        {
                        }

                        public bool Validate(Validator v, string sIdentity, OTTable table)
                        {
                            bool bRet = true;

                            // check for data overlap
                            bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetChainSubClassRule, CalcLength(), v, sIdentity, table.GetTag());

                            // check the backtrack array length
                            uint offset = m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackClasses;
                            if (offset + BacktrackGlyphCount*2 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Backtrack array");
                                bRet = false;
                            }

                            // check the input array length
                            offset = m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackClasses 
                                + (uint)BacktrackGlyphCount*2 + 2;
                            if (offset + (InputGlyphCount-1)*2 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Input array");
                                bRet = false;
                            }

                            // check the Lookahead array length
                            offset = m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackClasses 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2;
                            if (offset + LookaheadGlyphCount*2 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Lookahead array");
                                bRet = false;
                            }

                            // check the SubstLookupRecord array length
                            offset = m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackClasses 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                                + (uint)LookaheadGlyphCount*2 + 2;
                            if (offset + SubstCount*4 > m_bufTable.GetLength())
                            {
                                v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", SubstLookupRecord array");
                                bRet = false;
                            }

                            // check each SubstLookupRecord
                            for (uint i=0; i<SubstCount; i++)
                            {
                                SubstLookupRecord slr = GetSubstLookupRecord(i);
                                if (slr.SequenceIndex > InputGlyphCount)
                                {
                                    string sValues = ", slr.SequenceIndex = " + slr.SequenceIndex + ", slr.LookupListIndex = " + slr.LookupListIndex + ", InputGlyphCount = " + InputGlyphCount;
                                    v.Error(T.T_NULL, E.GSUB_E_SubstLookupRecord_SequenceIndex, table.m_tag, sIdentity + ", SubstLookupRecord[" + i + "]" + sValues);
                                    bRet = false;
                                }
                            }
                            return bRet;
                        }
                    
                    }

                    public ChainSubClassRule_val GetChainSubClassRuleTable_val(uint i)
                    {
                        ChainSubClassRule_val cscr = null;

                        if (i < ChainSubClassRuleCount)
                        {
                            cscr = new ChainSubClassRule_val(m_offsetChainSubClassSet + GetChainSubClassRuleOffset(i), m_bufTable);
                        }

                        return cscr;
                    }
                }
            }

            public class ChainContextSubstFormat3_val : ChainContextSubstFormat3, I_OTLValidate
            {
                public ChainContextSubstFormat3_val(uint offset, MBOBuffer bufTable) : base(offset, bufTable)
                {
                }

                public bool Validate(Validator v, string sIdentity, OTTable table)
                {
                    bool bRet = true;

                    // check for data overlap
                    bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetChainContextSubst, CalcLength(), v, sIdentity, table.GetTag());

                    // check the BacktrackCoverage array length
                    uint offset = m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackCoverageOffsets;
                    if (offset + BacktrackGlyphCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", BacktrackCoverage array");
                        bRet = false;
                    }

                    // check each BacktrackCoverage offset
                    for (uint i=0; i<BacktrackGlyphCount; i++)
                    {
                        if (m_offsetChainContextSubst + GetBacktrackCoverageOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", BacktrackCoverage[" + i + "]");
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
                    offset = m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackCoverageOffsets 
                        + (uint)BacktrackGlyphCount*2 + 2;
                    if (offset + InputGlyphCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", InputCoverage array");
                        bRet = false;
                    }

                    // check each InputCoverage offset
                    for (uint i=0; i<InputGlyphCount; i++)
                    {
                        if (m_offsetChainContextSubst + GetInputCoverageOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", InputCoverage[" + i + "]");
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
                    offset = m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackCoverageOffsets 
                        + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2 + 2;
                    if (offset + LookaheadGlyphCount*2 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", LookaheadCoverage array");
                        bRet = false;
                    }

                    // check each LookaheadCoverage offset
                    for (uint i=0; i<LookaheadGlyphCount; i++)
                    {
                        if (m_offsetChainContextSubst + GetLookaheadCoverageOffset(i) > m_bufTable.GetLength())
                        {
                            v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", LookaheadCoverage[" + i + "]");
                            bRet = false;
                        }
                    }

                    // check each LookaheadCoverage table
                    for (uint i=0; i<LookaheadGlyphCount; i++)
                    {
                        CoverageTable_val ct = GetLookaheadCoverageTable_val(i);
                        ct.Validate(v, sIdentity + ", LookaheadCoverage[" + i + "]", table);
                    }

                    // check the SubstLookupRecord array length
                    offset = m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackCoverageOffsets 
                        + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2 + 2 
                        + (uint)LookaheadGlyphCount*2 + 2;
                    if (offset + SubstCount*4 > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", SubstLookupRecord array");
                        bRet = false;
                    }

                    // check each SubstLookupRecord
                    for (uint i=0; i<SubstCount; i++)
                    {
                        SubstLookupRecord slr = GetSubstLookupRecord(i);
                        if (slr.SequenceIndex > InputGlyphCount)
                        {
                            string sValues = ", slr.SequenceIndex = " + slr.SequenceIndex + ", slr.LookupListIndex = " + slr.LookupListIndex + ", InputGlyphCount = " + InputGlyphCount;
                            v.Error(T.T_NULL, E.GSUB_E_SubstLookupRecord_SequenceIndex, table.m_tag, sIdentity + ", SubstLookupRecord[" + i + "]" + sValues);
                            bRet = false;
                        }
                    }

                    return bRet;
                }
                
                public CoverageTable_val GetBacktrackCoverageTable_val(uint i)
                {
                    uint offset = m_offsetChainContextSubst + (uint)GetBacktrackCoverageOffset(i);
                    return new CoverageTable_val(offset, m_bufTable);
                }

                public CoverageTable_val GetInputCoverageTable_val(uint i)
                {
                    uint offset = m_offsetChainContextSubst + (uint)GetInputCoverageOffset(i);
                    return new CoverageTable_val(offset, m_bufTable);
                }

                public CoverageTable_val GetLookaheadCoverageTable_val(uint i)
                {
                    uint offset = m_offsetChainContextSubst + (uint)GetLookaheadCoverageOffset(i);
                    return new CoverageTable_val(offset, m_bufTable);
                }
            }

            public ChainContextSubstFormat1_val GetChainContextSubstFormat1_val()
            {
                if (SubstFormat != 1)
                {
                    throw new System.InvalidOperationException();
                }

                return new ChainContextSubstFormat1_val(m_offsetChainContextSubst, m_bufTable);
            }

            public ChainContextSubstFormat2_val GetChainContextSubstFormat2_val()
            {
                if (SubstFormat != 2)
                {
                    throw new System.InvalidOperationException();
                }

                return new ChainContextSubstFormat2_val(m_offsetChainContextSubst, m_bufTable);
            }

            public ChainContextSubstFormat3_val GetChainContextSubstFormat3_val()
            {
                if (SubstFormat != 3)
                {
                    throw new System.InvalidOperationException();
                }

                return new ChainContextSubstFormat3_val(m_offsetChainContextSubst, m_bufTable);
            }
        }

        // Lookup Type 7: Extension Substitution Subtable

        public class ExtensionSubst_val : ExtensionSubst, I_OTLValidate
        {
            public ExtensionSubst_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(ExtensionSubst)";

                // check for data overlap
                bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetExtensionSubst, CalcLength(), v, sIdentity, table.GetTag());

                // check the SubstFormat
                if (SubstFormat != 1)
                {
                    v.Error(T.T_NULL, E.GSUB_E_SubtableFormat, table.m_tag, sIdentity + ", SubstFormat = " + SubstFormat);
                    bRet = false;
                }

                // check the ExtensionLookupType
                if (ExtensionLookupType >= 7)
                {
                    v.Error(T.T_NULL, E.GSUB_E_ExtensionLookupType, table.m_tag, sIdentity + ", ExtensionLookupType = " + ExtensionLookupType);
                    bRet = false;
                }

                // check the ExtensionOffset
                if (m_offsetExtensionSubst + ExtensionOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", ExtensionOffset");
                    bRet = false;
                }

                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GSUB_P_ExtensionSubst, table.m_tag, sIdentity);
                }

                // validate the subtable
                SubTable st = null;
                switch (ExtensionLookupType)
                {
                    case 1: st = new val_GSUB.SingleSubst_val      (m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;
                    case 2: st = new val_GSUB.MultipleSubst_val    (m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;
                    case 3: st = new val_GSUB.AlternateSubst_val   (m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;
                    case 4: st = new val_GSUB.LigatureSubst_val    (m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;
                    case 5: st = new val_GSUB.ContextSubst_val     (m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;
                    case 6: st = new val_GSUB.ChainContextSubst_val(m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;

                }
                if (st != null)
                {
                    I_OTLValidate iv = (I_OTLValidate)st;
                    iv.Validate(v, sIdentity, table);
                }

                return bRet;
            }
        }


        // Lookup Type 8: Reverse Chaining Contextual Single Substitution Subtable

        public class ReverseChainSubst_val : ReverseChainSubst, I_OTLValidate
        {
            public ReverseChainSubst_val(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                sIdentity += "(ReverseChainSubst)";

                // check for data overlap
                bRet &= ((val_GSUB)table).ValidateNoOverlap(m_offsetReverseChainSubst, CalcLength(), v, sIdentity, table.GetTag());

                // check the SubstFormat
                if (SubstFormat != 1)
                {
                    v.Error(T.T_NULL, E.GSUB_E_SubtableFormat, table.m_tag, sIdentity + ", SubstFormat = " + SubstFormat);
                    bRet = false;
                }

                // check the coverage offset
                if (m_offsetReverseChainSubst + CoverageOffset > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", Coverage offset");
                    bRet = false;
                }

                // check the coverage table
                CoverageTable_val ct = GetCoverageTable_val();
                bRet &= ct.Validate(v, sIdentity + ", Coverage table", table);

                // check the BacktrackCoverage array length
                uint offset = m_offsetReverseChainSubst + (uint)FieldOffsets.BacktrackCoverageOffsets;
                if (offset + BacktrackGlyphCount*2 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", BacktrackCoverage array");
                    bRet = false;
                }

                // check each BacktrackCoverage offset
                for (uint i=0; i<BacktrackGlyphCount; i++)
                {
                    if (m_offsetReverseChainSubst + GetBacktrackCoverageOffset(i) > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", BacktrackCoverage[" + i + "]");
                        bRet = false;
                    }
                }

                // check each BacktrackCoverage table
                for (uint i=0; i<BacktrackGlyphCount; i++)
                {
                    CoverageTable_val bct = GetBacktrackCoverageTable_val(i);
                    bRet &= bct.Validate(v, sIdentity + ", BacktrackCoverage[" + i + "]", table);
                }

                // check the LookaheadCoverage array length
                offset = m_offsetReverseChainSubst + (uint)FieldOffsets.BacktrackCoverageOffsets + (uint)BacktrackGlyphCount*2
                    + 2;
                if (offset + LookaheadGlyphCount*2 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", LookaheadCoverage array");
                    bRet = false;
                }

                // check each LookaheadCoverage offset
                for (uint i=0; i<LookaheadGlyphCount; i++)
                {
                    if (m_offsetReverseChainSubst + GetLookaheadCoverageOffset(i) > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.GSUB_E_Offset_PastEOT, table.m_tag, sIdentity + ", LookaheadCoverage[" + i + "]");
                        bRet = false;
                    }
                }

                // check each LookaheadCoverage table
                for (uint i=0; i<LookaheadGlyphCount; i++)
                {
                    CoverageTable_val lct = GetLookaheadCoverageTable_val(i);
                    bRet &= lct.Validate(v, sIdentity + ", LookaheadCoverage[" + i + "]", table);
                }


                // check the SubstituteGlyphIDs array length
                offset = m_offsetReverseChainSubst + (uint)FieldOffsets.BacktrackCoverageOffsets + (uint)BacktrackGlyphCount*2
                    + 2 + (uint)LookaheadGlyphCount*2 + 2;
                if (offset + SubstituteGlyphCount*2 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.GSUB_E_Array_pastEOT, table.m_tag, sIdentity + ", Substitue Glyph ID array");
                    bRet = false;
                }


                if (bRet)
                {
                    v.Pass(T.T_NULL, P.GSUB_P_ReverseChainContextSubst, table.m_tag, sIdentity);
                }



                return bRet;
            }


            public CoverageTable_val GetCoverageTable_val()
            {
                return new CoverageTable_val(m_offsetReverseChainSubst + CoverageOffset, m_bufTable);
            }

            public CoverageTable_val GetBacktrackCoverageTable_val(uint i)
            {
                uint offset = m_offsetReverseChainSubst + (uint)GetBacktrackCoverageOffset(i);
                return new CoverageTable_val(offset, m_bufTable);
            }

            public CoverageTable_val GetLookaheadCoverageTable_val(uint i)
            {
                uint offset = m_offsetReverseChainSubst + (uint)GetLookaheadCoverageOffset(i);
                return new CoverageTable_val(offset, m_bufTable);
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
                // JJF: Figure out what to do here
                v.Error(T.T_NULL,E._Table_E_DataOverlap, tag, sIdentity + ", offset = " + offset + ", length = " + length);
            }

            return bValid;
        }

        protected DataOverlapDetector m_DataOverlapDetector;
    }
}
