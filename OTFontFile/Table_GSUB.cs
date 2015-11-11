using System;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_GSUB.
    /// </summary>
    public class Table_GSUB : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_GSUB(OTTag tag, MBOBuffer buf) : base(tag, buf)
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


        // Lookup Type 1: Single Substitution Subtable

        public class SingleSubst : OTL.SubTable
        {
            public SingleSubst(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetSingleSubst = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                SubstFormat = 0
            }

            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                return 1;
            }

            // nested classes

            public class SingleSubstFormat1
            {
                public SingleSubstFormat1(uint offset, MBOBuffer bufTable)
                {
                    m_offsetSingleSubst = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SubstFormat    = 0,
                    CoverageOffset = 2,
                    DeltaGlyphID   = 4
                }

                public uint CalcLength()
                {
                    return 6;
                }

                public ushort SubstFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetSingleSubst + (uint)FieldOffsets.SubstFormat);}
                }

                public ushort CoverageOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetSingleSubst + (uint)FieldOffsets.CoverageOffset);}
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetSingleSubst + CoverageOffset, m_bufTable);
                }

                public ushort DeltaGlyphID
                {
                    get {return m_bufTable.GetUshort(m_offsetSingleSubst + (uint)FieldOffsets.DeltaGlyphID);}
                }

                protected uint m_offsetSingleSubst;
                protected MBOBuffer m_bufTable;
            }

            public class SingleSubstFormat2
            {
                public SingleSubstFormat2(uint offset, MBOBuffer bufTable)
                {
                    m_offsetSingleSubst = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SubstFormat        = 0, 
                    CoverageOffset     = 2,
                    GlyphCount         = 4,
                    SubstituteGlyphIDs = 6
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.SubstituteGlyphIDs + (uint)GlyphCount*2;
                }

                public ushort SubstFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetSingleSubst + (uint)FieldOffsets.SubstFormat);}
                }

                public ushort CoverageOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetSingleSubst + (uint)FieldOffsets.CoverageOffset);}
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetSingleSubst + CoverageOffset, m_bufTable);
                }

                public ushort GlyphCount
                {
                    get {return m_bufTable.GetUshort(m_offsetSingleSubst + (uint)FieldOffsets.GlyphCount);}
                }

                public ushort GetSubstituteGlyphID(uint i)
                {
                    uint offset = m_offsetSingleSubst + (uint)FieldOffsets.SubstituteGlyphIDs + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                protected uint m_offsetSingleSubst;
                protected MBOBuffer m_bufTable;
            }

            public ushort SubstFormat
            {
                get {return m_bufTable.GetUshort(m_offsetSingleSubst + (uint)FieldOffsets.SubstFormat);}
            }

            public SingleSubstFormat1 GetSingleSubstFormat1()
            {
                if (SubstFormat != 1)
                {
                    throw new System.InvalidOperationException();
                }

                return new SingleSubstFormat1(m_offsetSingleSubst, m_bufTable);
            }

            public SingleSubstFormat2 GetSingleSubstFormat2()
            {
                if (SubstFormat != 2)
                {
                    throw new System.InvalidOperationException();
                }

                return new SingleSubstFormat2(m_offsetSingleSubst, m_bufTable);
            }


            protected uint m_offsetSingleSubst;
        }

        // Lookup Type 2: Multiple Substitution Subtable

        public class MultipleSubst : OTL.SubTable
        {
            public MultipleSubst(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetMultipleSubst = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                SubstFormat     = 0,
                CoverageOffset  = 2,
                SequenceCount   = 4,
                SequenceOffsets = 6
            }

            // public methods

            public uint CalcLength()
            {
                return (uint)FieldOffsets.SequenceOffsets + (uint)SequenceCount*2;
            }

            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                return 1;
            }

            // accessors

            public ushort SubstFormat
            {
                get {return m_bufTable.GetUshort(m_offsetMultipleSubst + (uint)FieldOffsets.SubstFormat);}
            }

            public ushort CoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetMultipleSubst + (uint)FieldOffsets.CoverageOffset);}
            }

            public OTL.CoverageTable GetCoverageTable()
            {
                return new OTL.CoverageTable(m_offsetMultipleSubst + CoverageOffset, m_bufTable);
            }

            public ushort SequenceCount
            {
                get {return m_bufTable.GetUshort(m_offsetMultipleSubst + (uint)FieldOffsets.SequenceCount);}
            }

            public ushort GetSequenceOffset(uint i)
            {
                uint offset = m_offsetMultipleSubst + (uint)FieldOffsets.SequenceOffsets + i*2;
                return m_bufTable.GetUshort(offset);
            }

            public class SequenceTable
            {
                public SequenceTable(uint offset, MBOBuffer bufTable)
                {
                    m_offsetSequence = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    GlyphCount         = 0,
                    SubstituteGlyphIDs = 2
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.SubstituteGlyphIDs + (uint)GlyphCount*2;
                }

                public ushort GlyphCount
                {
                    get {return m_bufTable.GetUshort(m_offsetSequence + (uint)FieldOffsets.GlyphCount);}
                }

                public ushort GetSubstituteGlyphID(uint i)
                {
                    uint offset = m_offsetSequence + (uint)FieldOffsets.SubstituteGlyphIDs + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                protected uint m_offsetSequence;
                protected MBOBuffer m_bufTable;
            }

            public SequenceTable GetSequenceTable(uint i)
            {
                SequenceTable st = null;

                if (i < SequenceCount)
                {
                    st = new SequenceTable(m_offsetMultipleSubst + GetSequenceOffset(i), m_bufTable);
                }

                return st;
            }


            protected uint m_offsetMultipleSubst;
        }

        // Lookup Type 3: Alternate Substitution Subtable

        public class AlternateSubst : OTL.SubTable
        {
            public AlternateSubst(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetAlternateSubst = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                SubstFormat         = 0,
                CoverageOffset      = 2,
                AlternateSetCount   = 4,
                AlternateSetOffsets = 6
            }

            // public methods

            public uint CalcLength()
            {
                return (uint)FieldOffsets.AlternateSetOffsets + (uint)AlternateSetCount*2;
            }

            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                return 1;
            }

            // accessors

            public ushort SubstFormat
            {
                get {return m_bufTable.GetUshort(m_offsetAlternateSubst + (uint)FieldOffsets.SubstFormat);}
            }

            public ushort CoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetAlternateSubst + (uint)FieldOffsets.CoverageOffset);}
            }

            public OTL.CoverageTable GetCoverageTable()
            {
                return new OTL.CoverageTable(m_offsetAlternateSubst + CoverageOffset, m_bufTable);
            }

            public ushort AlternateSetCount
            {
                get {return m_bufTable.GetUshort(m_offsetAlternateSubst + (uint)FieldOffsets.AlternateSetCount);}
            }

            public ushort GetAlternateSetOffset(uint i)
            {
                uint offset = m_offsetAlternateSubst + (uint)FieldOffsets.AlternateSetOffsets + i*2;
                return m_bufTable.GetUshort(offset);
            }

            public class AlternateSet
            {
                public AlternateSet(uint offset, MBOBuffer bufTable)
                {
                    m_offsetAlternateSet = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    GlyphCount        = 0,
                    AlternateGlyphIDs = 2
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.AlternateGlyphIDs + (uint)GlyphCount*2;
                }

                public ushort GlyphCount
                {
                    get {return m_bufTable.GetUshort(m_offsetAlternateSet + (uint)FieldOffsets.GlyphCount);}
                }

                public ushort GetAlternateGlyphID(uint i)
                {
                    uint offset = m_offsetAlternateSet + (uint)FieldOffsets.AlternateGlyphIDs + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                protected uint m_offsetAlternateSet;
                protected MBOBuffer m_bufTable;
            }

            public AlternateSet GetAlternateSetTable(uint i)
            {
                AlternateSet aset = null;

                if (i < AlternateSetCount)
                {
                    aset = new AlternateSet(m_offsetAlternateSubst + GetAlternateSetOffset(i), m_bufTable);
                }

                return aset;
            }

            protected uint m_offsetAlternateSubst;
        }

        // Lookup Type 4: Ligature Substitution Subtable

        public class LigatureSubst : OTL.SubTable
        {
            public LigatureSubst(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetLigatureSubst = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                SubstFormat        = 0,
                CoverageOffset     = 2,
                LigSetCount        = 4,
                LigatureSetOffsets = 6
            }

            // public methods

            public uint CalcLength()
            {
                return (uint)FieldOffsets.LigatureSetOffsets + (uint)LigSetCount*2;
            }

            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                uint nLength = 0;

                for (uint iLigatureSet=0; iLigatureSet<LigSetCount; iLigatureSet++)
                {
                    LigatureSubst.LigatureSet ls = GetLigatureSetTable(iLigatureSet);
                    for (uint iLigature=0; iLigature< ls.LigatureCount; iLigature++)
                    {
                        LigatureSubst.LigatureSet.Ligature lig = ls.GetLigatureTable(iLigature);
                        if (lig.CompCount > nLength)
                        {
                            nLength = lig.CompCount;
                        }
                    }
                }

                return nLength;
            }

            // accessors

            public ushort SubstFormat
            {
                get {return m_bufTable.GetUshort(m_offsetLigatureSubst + (uint)FieldOffsets.SubstFormat);}
            }

            public ushort CoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetLigatureSubst + (uint)FieldOffsets.CoverageOffset);}
            }

            public OTL.CoverageTable GetCoverageTable()
            {
                return new OTL.CoverageTable(m_offsetLigatureSubst + CoverageOffset, m_bufTable);
            }

            public ushort LigSetCount
            {
                get {return m_bufTable.GetUshort(m_offsetLigatureSubst + (uint)FieldOffsets.LigSetCount);}
            }

            public ushort GetLigatureSetOffset(uint i)
            {
                uint offset = m_offsetLigatureSubst + (uint)FieldOffsets.LigatureSetOffsets + i*2;
                return m_bufTable.GetUshort(offset);
            }

            public class LigatureSet
            {
                public LigatureSet(uint offset, MBOBuffer bufTable)
                {
                    m_offsetLigatureSet = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    LigatureCount   = 0,
                    LigatureOffsets = 2
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.LigatureOffsets + (uint)LigatureCount*2;
                }

                public ushort LigatureCount
                {
                    get {return m_bufTable.GetUshort(m_offsetLigatureSet + (uint)FieldOffsets.LigatureCount);}
                }

                public ushort GetLigatureOffset(uint i)
                {
                    uint offset = m_offsetLigatureSet + (uint)FieldOffsets.LigatureOffsets + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public class Ligature
                {
                    public Ligature(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetLigature = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        LigGlyph          = 0,
                        CompCount         = 2,
                        ComponentGlyphIDs = 4
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.ComponentGlyphIDs + (uint)(CompCount-1)*2;
                    }

                    public ushort LigGlyph
                    {
                        get {return m_bufTable.GetUshort(m_offsetLigature + (uint)FieldOffsets.LigGlyph);}
                    }

                    public ushort CompCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetLigature + (uint)FieldOffsets.CompCount);}
                    }

                    public ushort GetComponentGlyphID(uint i)
                    {
                        uint offset = m_offsetLigature + (uint)FieldOffsets.ComponentGlyphIDs + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    protected uint m_offsetLigature;
                    protected MBOBuffer m_bufTable;
                }

                public Ligature GetLigatureTable(uint i)
                {
                    Ligature l = null;

                    if (i < LigatureCount)
                    {
                        l = new Ligature(m_offsetLigatureSet + GetLigatureOffset(i), m_bufTable);
                    }

                    return l;
                }

                protected uint m_offsetLigatureSet;
                protected MBOBuffer m_bufTable;
            }

            public LigatureSet GetLigatureSetTable(uint i)
            {
                LigatureSet ls = null;

                if (i < LigSetCount)
                {
                    ls = new LigatureSet(m_offsetLigatureSubst + GetLigatureSetOffset(i), m_bufTable);
                }

                return ls;
            }

            protected uint m_offsetLigatureSubst;
        }

        // Lookup Type 5: Contextual Substitution Subtable

        public class ContextSubst : OTL.SubTable
        {
            public ContextSubst(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetContextSubst = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                SubstFormat = 0
            }


            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                uint nLength = 0;

                if (SubstFormat == 1)
                {
                    ContextSubstFormat1 csf1 = GetContextSubstFormat1();
                    for (uint iSubRuleSet = 0; iSubRuleSet < csf1.SubRuleSetCount; iSubRuleSet++)
                    {
                        ContextSubstFormat1.SubRuleSet srs = csf1.GetSubRuleSetTable(iSubRuleSet);
                        if (srs != null)
                        {
                            for (uint iSubRule = 0; iSubRule < srs.SubRuleCount; iSubRule++)
                            {
                                ContextSubstFormat1.SubRuleSet.SubRule sr = srs.GetSubRuleTable(iSubRule);
                                if (sr.GlyphCount > nLength)
                                {
                                    nLength = sr.GlyphCount;
                                }
                            }
                        }
                    }
                }
                else if (SubstFormat == 2)
                {
                    ContextSubstFormat2 csf2 = GetContextSubstFormat2();
                    for (uint iSubClassSet = 0; iSubClassSet < csf2.SubClassSetCount; iSubClassSet ++)
                    {
                        ContextSubstFormat2.SubClassSet scs = csf2.GetSubClassSetTable(iSubClassSet);
                        if (scs != null)
                        {
                            for (uint iSubClassRule = 0; iSubClassRule < scs.SubClassRuleCount; iSubClassRule++)
                            {
                                ContextSubstFormat2.SubClassSet.SubClassRule scr = scs.GetSubClassRuleTable(iSubClassRule);
                                if (scr.GlyphCount > nLength)
                                {
                                    nLength = scr.GlyphCount;
                                }
                            }
                        }
                    }
                }
                else if (SubstFormat == 3)
                {
                    ContextSubstFormat3 csf3 = GetContextSubstFormat3();
                    if (csf3.GlyphCount > nLength)
                    {
                        nLength = csf3.GlyphCount;
                    }
                }

                return nLength;
            }


            // nested classes

            public class SubstLookupRecord
            {
                public SubstLookupRecord(uint offset, MBOBuffer bufTable)
                {
                    m_offsetSubstLookupRecord = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SequenceIndex = 0,
                    LookupListIndex = 2
                }

                public ushort SequenceIndex
                {
                    get {return m_bufTable.GetUshort(m_offsetSubstLookupRecord + (uint)FieldOffsets.SequenceIndex);}
                }

                public ushort LookupListIndex
                {
                    get {return m_bufTable.GetUshort(m_offsetSubstLookupRecord + (uint)FieldOffsets.LookupListIndex);}
                }

                protected uint m_offsetSubstLookupRecord;
                protected MBOBuffer m_bufTable;
            }

            public class ContextSubstFormat1
            {
                public ContextSubstFormat1(uint offset, MBOBuffer bufTable)
                {
                    m_offsetContextSubst = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SubstFormat       = 0,
                    CoverageOffset    = 2,
                    SubRuleSetCount   = 4,
                    SubRuleSetOffsets = 6
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.SubRuleSetOffsets + (uint)SubRuleSetCount*2;
                }

                public ushort SubstFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.SubstFormat);}
                }

                public ushort CoverageOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.CoverageOffset);}
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetContextSubst + CoverageOffset, m_bufTable);
                }

                public ushort SubRuleSetCount
                {
                    get {return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.SubRuleSetCount);}
                }

                public ushort GetSubRuleSetOffset(uint i)
                {
                    return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.SubRuleSetOffsets + i*2);
                }

                public SubRuleSet GetSubRuleSetTable(uint i)
                {
                    return new SubRuleSet(m_offsetContextSubst + GetSubRuleSetOffset(i), m_bufTable);
                }

                public class SubRuleSet
                {
                    public SubRuleSet(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetSubRuleSet = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        SubRuleCount   = 0,
                        SubRuleOffsets = 2
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.SubRuleOffsets + (uint)SubRuleCount*2;
                    }

                    public ushort SubRuleCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetSubRuleSet + (uint)FieldOffsets.SubRuleCount);}
                    }

                    public ushort GetSubRuleOffset(uint i)
                    {
                        uint offset = m_offsetSubRuleSet + (uint)FieldOffsets.SubRuleOffsets + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public class SubRule
                    {
                        public SubRule(uint offset, MBOBuffer bufTable)
                        {
                            m_offsetSubRule = offset;
                            m_bufTable = bufTable;
                        }

                        public enum FieldOffsets
                        {
                            GlyphCount    = 0,
                            SubstCount    = 2,
                            InputGlyphIds = 4
                        }

                        public uint CalcLength()
                        {
                            return (uint)FieldOffsets.InputGlyphIds + (uint)(GlyphCount-1)*2;
                        }

                        public ushort GlyphCount
                        {
                            get {return m_bufTable.GetUshort(m_offsetSubRule + (uint)FieldOffsets.GlyphCount);}
                        }

                        public ushort SubstCount
                        {
                            get {return m_bufTable.GetUshort(m_offsetSubRule + (uint)FieldOffsets.SubstCount);}
                        }

                        public ushort GetInputGlyphID(uint i)
                        {
                            uint offset = m_offsetSubRule + (uint)FieldOffsets.InputGlyphIds + i*2;
                            return m_bufTable.GetUshort(offset);
                        }

                        public SubstLookupRecord GetSubstLookupRecord(uint i)
                        {
                            SubstLookupRecord slr = null;

                            if (i < SubstCount)
                            {
                                uint offset = m_offsetSubRule + (uint)FieldOffsets.InputGlyphIds + (uint)(GlyphCount-1)*2 + i*4;
                                slr = new SubstLookupRecord(offset, m_bufTable);
                            }

                            return slr;
                        }

                        protected uint m_offsetSubRule;
                        protected MBOBuffer m_bufTable;
                    }

                    public SubRule GetSubRuleTable(uint i)
                    {
                        SubRule sr = null;

                        if (i < SubRuleCount)
                        {
                            sr = new SubRule(m_offsetSubRuleSet + GetSubRuleOffset(i), m_bufTable);
                        }

                        return sr;
                    }

                    protected uint m_offsetSubRuleSet;
                    protected MBOBuffer m_bufTable;
                }


                protected uint m_offsetContextSubst;
                protected MBOBuffer m_bufTable;
            }

            public class ContextSubstFormat2
            {
                public ContextSubstFormat2(uint offset, MBOBuffer bufTable)
                {
                    m_offsetContextSubst = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SubstFormat        = 0,
                    CoverageOffset     = 2,
                    ClassDefOffset     = 4,
                    SubClassSetCount   = 6,
                    SubClassSetOffsets = 8
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.SubClassSetOffsets + (uint)SubClassSetCount*2;
                }

                public ushort SubstFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.SubstFormat);}
                }

                public ushort CoverageOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.CoverageOffset);}
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetContextSubst + CoverageOffset, m_bufTable);
                }

                public ushort ClassDefOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.ClassDefOffset);}
                }

                public OTL.ClassDefTable GetClassDefTable()
                {
                    return new OTL.ClassDefTable(m_offsetContextSubst + ClassDefOffset, m_bufTable);
                }

                public ushort SubClassSetCount
                {
                    get {return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.SubClassSetCount);}
                }

                public ushort GetSubClassSetOffset(uint i)
                {
                    uint offset = m_offsetContextSubst + (uint)FieldOffsets.SubClassSetOffsets + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public SubClassSet GetSubClassSetTable(uint i)
                {
                    SubClassSet scs = null;
                    if (GetSubClassSetOffset(i) != 0)
                    {
                        scs =  new SubClassSet(m_offsetContextSubst + GetSubClassSetOffset(i), m_bufTable);
                    }
                    return scs;
                }

                public class SubClassSet
                {
                    public SubClassSet(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetSubClassSet = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        SubClassRuleCount   = 0,
                        SubClassRuleOffsets = 2
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.SubClassRuleOffsets + (uint)SubClassRuleCount*2;
                    }

                    public ushort SubClassRuleCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetSubClassSet + (uint)FieldOffsets.SubClassRuleCount);}
                    }

                    public ushort GetSubClassRuleOffset(uint i)
                    {
                        uint offset = m_offsetSubClassSet + (uint)FieldOffsets.SubClassRuleOffsets + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public class SubClassRule
                    {
                        public SubClassRule(uint offset, MBOBuffer bufTable)
                        {
                            m_offsetSubClassRule = offset;
                            m_bufTable = bufTable;
                        }

                        public enum FieldOffsets
                        {
                            GlyphCount = 0,
                            SubstCount = 2,
                            ClassArray = 4
                        }

                        public uint CalcLength()
                        {
                            return (uint)FieldOffsets.ClassArray + (uint)(GlyphCount-1)*2 + (uint)SubstCount*4;
                        }

                        public ushort GlyphCount
                        {
                            get {return m_bufTable.GetUshort(m_offsetSubClassRule + (uint)FieldOffsets.GlyphCount);}
                        }

                        public ushort SubstCount
                        {
                            get {return m_bufTable.GetUshort(m_offsetSubClassRule + (uint)FieldOffsets.SubstCount);}
                        }

                        public ushort GetClass(uint i)
                        {
                            uint offset = m_offsetSubClassRule + (uint)FieldOffsets.ClassArray + i*2;
                            return m_bufTable.GetUshort(offset);
                        }

                        public SubstLookupRecord GetSubstLookupRecord(uint i)
                        {
                            SubstLookupRecord slr = null;

                            if (i < SubstCount)
                            {
                                uint offset = m_offsetSubClassRule + (uint)FieldOffsets.ClassArray + (uint)(GlyphCount-1)*2 + i*4;
                                slr = new SubstLookupRecord(offset, m_bufTable);
                            }

                            return slr;
                        }

                        protected uint m_offsetSubClassRule;
                        protected MBOBuffer m_bufTable;
                    }

                    public SubClassRule GetSubClassRuleTable(uint i)
                    {
                        SubClassRule scr = null;

                        if (i < SubClassRuleCount)
                        {
                            scr = new SubClassRule(m_offsetSubClassSet + GetSubClassRuleOffset(i), m_bufTable);
                        }

                        return scr;
                    }

                    protected uint m_offsetSubClassSet;
                    protected MBOBuffer m_bufTable;
                }

                protected uint m_offsetContextSubst;
                protected MBOBuffer m_bufTable;
            }

            public class ContextSubstFormat3
            {
                public ContextSubstFormat3(uint offset, MBOBuffer bufTable)
                {
                    m_offsetContextSubst = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SubstFormat     = 0,
                    GlyphCount      = 2,
                    SubstCount      = 4,
                    CoverageOffsets = 6
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.CoverageOffsets + (uint)GlyphCount*2 + (uint)SubstCount*4;
                }

                public ushort SubstFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.SubstFormat);}
                }

                public ushort GlyphCount
                {
                    get {return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.GlyphCount);}
                }

                public ushort SubstCount
                {
                    get {return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.SubstCount);}
                }

                public ushort GetCoverageOffset(uint i)
                {
                    uint offset = m_offsetContextSubst + (uint)FieldOffsets.CoverageOffsets + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public OTL.CoverageTable GetCoverageTable(uint i)
                {
                    return new OTL.CoverageTable(m_offsetContextSubst + GetCoverageOffset(i), m_bufTable);
                }

                public SubstLookupRecord GetSubstLookupRecord(uint i)
                {
                    SubstLookupRecord slr = null;

                    if (i < SubstCount)
                    {
                        uint offset = m_offsetContextSubst + (uint)FieldOffsets.CoverageOffsets + (uint)GlyphCount*2 + i*4;
                        slr = new SubstLookupRecord(offset, m_bufTable);
                    }

                    return slr;
                }

                protected uint m_offsetContextSubst;
                protected MBOBuffer m_bufTable;
            }


            public ushort SubstFormat
            {
                get {return m_bufTable.GetUshort(m_offsetContextSubst + (uint)FieldOffsets.SubstFormat);}
            }

            public ContextSubstFormat1 GetContextSubstFormat1()
            {
                if (SubstFormat != 1)
                {
                    throw new System.InvalidOperationException();
                }

                return new ContextSubstFormat1(m_offsetContextSubst, m_bufTable);
            }

            public ContextSubstFormat2 GetContextSubstFormat2()
            {
                if (SubstFormat != 2)
                {
                    throw new System.InvalidOperationException();
                }

                return new ContextSubstFormat2(m_offsetContextSubst, m_bufTable);
            }

            public ContextSubstFormat3 GetContextSubstFormat3()
            {
                if (SubstFormat != 3)
                {
                    throw new System.InvalidOperationException();
                }

                return new ContextSubstFormat3(m_offsetContextSubst, m_bufTable);
            }

            protected uint m_offsetContextSubst;
        }

        // Lookup Type 6: Chaining Contextual Substitution Subtable

        public class ChainContextSubst : OTL.SubTable
        {
            public ChainContextSubst(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetChainContextSubst = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                SubstFormat = 0
            }


            // this is needed for calculating OS2.usMaxContext
            public override uint GetMaxContextLength()
            {
                uint nLength = 0;

                if (SubstFormat == 1)
                {
                    ChainContextSubstFormat1 ccsf1 = GetChainContextSubstFormat1();
                    for (uint iChainSubRuleSet = 0; iChainSubRuleSet < ccsf1.ChainSubRuleSetCount; iChainSubRuleSet++)
                    {
                        ChainContextSubstFormat1.ChainSubRuleSet csrs = ccsf1.GetChainSubRuleSetTable(iChainSubRuleSet);
                        if (csrs != null)
                        {
                            for (uint iChainSubRule = 0; iChainSubRule < csrs.ChainSubRuleCount; iChainSubRule++)
                            {
                                ChainContextSubstFormat1.ChainSubRuleSet.ChainSubRule csr = csrs.GetChainSubRuleTable(iChainSubRule);
                                uint tempLength = (uint)(csr.InputGlyphCount + csr.LookaheadGlyphCount);
                                if (tempLength > nLength)
                                {
                                    nLength = tempLength;
                                }
                            }
                        }
                    }
                }
                else if (SubstFormat == 2)
                {
                    ChainContextSubstFormat2 ccsf2 = GetChainContextSubstFormat2();
                    for (uint iChainSubClassSet = 0; iChainSubClassSet < ccsf2.ChainSubClassSetCount; iChainSubClassSet++)
                    {
                        ChainContextSubstFormat2.ChainSubClassSet cscs = ccsf2.GetChainSubClassSetTable(iChainSubClassSet);
                        if (cscs != null)
                        {
                            for (uint iChainSubClassRule = 0; iChainSubClassRule < cscs.ChainSubClassRuleCount; iChainSubClassRule++)
                            {
                                ChainContextSubstFormat2.ChainSubClassSet.ChainSubClassRule cscr = cscs.GetChainSubClassRuleTable(iChainSubClassRule);
                                uint tempLength = (uint)(cscr.InputGlyphCount + cscr.LookaheadGlyphCount);
                                if (tempLength > nLength)
                                {
                                    nLength = tempLength;
                                }
                            }
                        }
                    }
                }
                else if (SubstFormat == 3)
                {
                    ChainContextSubstFormat3 ccsf3 = GetChainContextSubstFormat3();
                    uint tempLength = (uint)(ccsf3.InputGlyphCount + ccsf3.LookaheadGlyphCount);
                    if (tempLength > nLength)
                    {
                        nLength = tempLength;
                    }
                }

                return nLength;
            }
            
            
            // nested classes

            public class SubstLookupRecord
            {
                public SubstLookupRecord(uint offset, MBOBuffer bufTable)
                {
                    m_offsetSubstLookupRecord = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SequenceIndex = 0,
                    LookupListIndex = 2
                }

                public ushort SequenceIndex
                {
                    get {return m_bufTable.GetUshort(m_offsetSubstLookupRecord + (uint)FieldOffsets.SequenceIndex);}
                }

                public ushort LookupListIndex
                {
                    get {return m_bufTable.GetUshort(m_offsetSubstLookupRecord + (uint)FieldOffsets.LookupListIndex);}
                }

                protected uint m_offsetSubstLookupRecord;
                protected MBOBuffer m_bufTable;
            }

            public class ChainContextSubstFormat1
            {
                public ChainContextSubstFormat1(uint offset, MBOBuffer bufTable)
                {
                    m_offsetChainContextSubst = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SubstFormat            = 0,
                    CoverageOffset         = 2,
                    ChainSubRuleSetCount   = 4,
                    ChainSubRuleSetOffsets = 6
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.ChainSubRuleSetOffsets + (uint)ChainSubRuleSetCount*2;
                }

                public ushort SubstFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.SubstFormat);}
                }

                public ushort CoverageOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.CoverageOffset);}
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetChainContextSubst + CoverageOffset, m_bufTable);
                }

                public ushort ChainSubRuleSetCount
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.ChainSubRuleSetCount);}
                }

                public ushort GetChainSubRuleSetOffset(uint i)
                {
                    uint offset = m_offsetChainContextSubst + (uint)FieldOffsets.ChainSubRuleSetOffsets + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public ChainSubRuleSet GetChainSubRuleSetTable(uint i)
                {
                    return new ChainSubRuleSet(m_offsetChainContextSubst + GetChainSubRuleSetOffset(i), m_bufTable);
                }

                public class ChainSubRuleSet
                {
                    public ChainSubRuleSet(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetChainSubRuleSet = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        ChainSubRuleCount   = 0,
                        ChainSubRuleOffsets = 2
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.ChainSubRuleOffsets + (uint)ChainSubRuleCount*2;
                    }

                    public ushort ChainSubRuleCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetChainSubRuleSet + (uint)FieldOffsets.ChainSubRuleCount);}
                    }

                    public ushort GetChainSubRuleOffset(uint i)
                    {
                        uint offset = m_offsetChainSubRuleSet + (uint)FieldOffsets.ChainSubRuleOffsets + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public class ChainSubRule
                    {
                        public ChainSubRule(uint offset, MBOBuffer bufTable)
                        {
                            m_offsetChainSubRule = offset;
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
                                + 2 + (uint)SubstCount*4;
                        }

                        public ushort BacktrackGlyphCount
                        {
                            get {return m_bufTable.GetUshort(m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphCount);}
                        }

                        public ushort GetBacktrackGlyphID(uint i)
                        {
                            uint offset = m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphIDs + i*2;
                            return m_bufTable.GetUshort(offset);
                        }

                        public ushort InputGlyphCount
                        {
                            get
                            {
                                uint offset = m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphIDs + (uint)BacktrackGlyphCount*2;
                                return m_bufTable.GetUshort(offset);
                            }
                        }

                        public ushort GetInputGlyphID(uint i)
                        {
                            uint offset = m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                + (uint)BacktrackGlyphCount*2 + 2 + i*2;
                            return m_bufTable.GetUshort(offset);
                        }

                        public ushort LookaheadGlyphCount
                        {
                            get
                            {
                                uint offset = m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                    + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2;
                                return m_bufTable.GetUshort(offset);
                            }
                        }

                        public ushort GetLookaheadGlyphID(uint i)
                        {
                            uint offset = m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 + i*2;
                            return m_bufTable.GetUshort(offset);
                        }

                        public ushort SubstCount
                        {
                            get
                            {
                                uint offset = m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                    + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                                    + (uint)LookaheadGlyphCount*2;
                                return m_bufTable.GetUshort(offset);
                            }
                        }

                        public SubstLookupRecord GetSubstLookupRecord(uint i)
                        {
                            SubstLookupRecord slr = null;

                            if (i < SubstCount)
                            {
                                uint offset = m_offsetChainSubRule + (uint)FieldOffsets.BacktrackGlyphIDs 
                                    + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                                    + (uint)LookaheadGlyphCount*2 + 2 + i*4;
                                slr = new SubstLookupRecord(offset, m_bufTable);
                            }

                            return slr;
                        }


                        protected uint m_offsetChainSubRule;
                        protected MBOBuffer m_bufTable;
                    }

                    public ChainSubRule GetChainSubRuleTable(uint i)
                    {
                        ChainSubRule csr = null;

                        if (i < ChainSubRuleCount)
                        {
                            csr = new ChainSubRule(m_offsetChainSubRuleSet + GetChainSubRuleOffset(i), m_bufTable);
                        }

                        return csr;
                    }

                    protected uint m_offsetChainSubRuleSet;
                    protected MBOBuffer m_bufTable;
                }


                protected uint m_offsetChainContextSubst;
                protected MBOBuffer m_bufTable;
            }

            public class ChainContextSubstFormat2
            {
                public ChainContextSubstFormat2(uint offset, MBOBuffer bufTable)
                {
                    m_offsetChainContextSubst = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SubstFormat             = 0,
                    CoverageOffset          = 2,
                    BacktrackClassDefOffset = 4,
                    InputClassDefOffset     = 6,
                    LookaheadClassDefOffset = 8,
                    ChainSubClassSetCount   = 10,
                    ChainSubClassSetOffsets = 12
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.ChainSubClassSetOffsets + (uint)ChainSubClassSetCount*2;
                }

                public ushort SubstFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.SubstFormat);}
                }

                public ushort CoverageOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.CoverageOffset);}
                }

                public OTL.CoverageTable GetCoverageTable()
                {
                    return new OTL.CoverageTable(m_offsetChainContextSubst + CoverageOffset, m_bufTable);
                }

                public ushort BacktrackClassDefOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackClassDefOffset);}
                }

                public OTL.ClassDefTable GetBacktrackClassDefTable()
                {
                    return new OTL.ClassDefTable(m_offsetChainContextSubst + BacktrackClassDefOffset, m_bufTable);
                }

                public ushort InputClassDefOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.InputClassDefOffset);}
                }

                public OTL.ClassDefTable GetInputClassDefTable()
                {
                    return new OTL.ClassDefTable(m_offsetChainContextSubst + InputClassDefOffset, m_bufTable);
                }

                public ushort LookaheadClassDefOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.LookaheadClassDefOffset);}
                }

                public OTL.ClassDefTable GetLookaheadClassDefTable()
                {
                    return new OTL.ClassDefTable(m_offsetChainContextSubst + LookaheadClassDefOffset, m_bufTable);
                }

                public ushort ChainSubClassSetCount
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.ChainSubClassSetCount);}
                }

                public ushort GetChainSubClassSetOffset(uint i)
                {
                    return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.ChainSubClassSetOffsets + i*2);
                }

                public ChainSubClassSet GetChainSubClassSetTable(uint i)
                {
                    ChainSubClassSet cscs = null;
                    if (GetChainSubClassSetOffset(i) != 0)
                    {
                        cscs = new ChainSubClassSet(m_offsetChainContextSubst + GetChainSubClassSetOffset(i), m_bufTable);
                    }
                    return cscs;
                }

                public class ChainSubClassSet
                {
                    public ChainSubClassSet(uint offset, MBOBuffer bufTable)
                    {
                        m_offsetChainSubClassSet = offset;
                        m_bufTable = bufTable;
                    }

                    public enum FieldOffsets
                    {
                        ChainSubClassRuleCount   = 0,
                        ChainSubClassRuleOffsets = 2
                    }

                    public uint CalcLength()
                    {
                        return (uint)FieldOffsets.ChainSubClassRuleOffsets + (uint)ChainSubClassRuleCount*2;
                    }

                    public ushort ChainSubClassRuleCount
                    {
                        get {return m_bufTable.GetUshort(m_offsetChainSubClassSet + (uint)FieldOffsets.ChainSubClassRuleCount);}
                    }

                    public ushort GetChainSubClassRuleOffset(uint i)
                    {
                        uint offset = m_offsetChainSubClassSet + (uint)FieldOffsets.ChainSubClassRuleOffsets + i*2;
                        return m_bufTable.GetUshort(offset);
                    }

                    public class ChainSubClassRule
                    {
                        public ChainSubClassRule(uint offset, MBOBuffer bufTable)
                        {
                            m_offsetChainSubClassRule = offset;
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
                                + 2 + (uint)SubstCount*4;
                        }

                        public ushort BacktrackGlyphCount
                        {
                            get {return m_bufTable.GetUshort(m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackGlyphCount);}
                        }

                        public ushort GetBacktrackClass(uint i)
                        {
                            uint offset = m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackClasses + i*2;
                            return m_bufTable.GetUshort(offset);
                        }

                        public ushort InputGlyphCount
                        {
                            get
                            {
                                uint offset = m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackClasses 
                                    + (uint)BacktrackGlyphCount*2;
                                return m_bufTable.GetUshort(offset);
                            }
                        }

                        public ushort GetInputClass(uint i)
                        {
                            uint offset = m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackClasses 
                                + (uint)BacktrackGlyphCount*2 + 2 + i*2;
                            return m_bufTable.GetUshort(offset);
                        }

                        public ushort LookaheadGlyphCount
                        {
                            get
                            {
                                uint offset = m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackClasses 
                                    + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2;
                                return m_bufTable.GetUshort(offset);
                            }
                        }

                        public ushort GetLookaheadClass(uint i)
                        {
                            uint offset = m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackClasses 
                                + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 + i*2;
                            return m_bufTable.GetUshort(offset);
                        }

                        public ushort SubstCount
                        {
                            get
                            {
                                uint offset = m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackClasses 
                                    + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                                    + (uint)LookaheadGlyphCount*2;
                                return m_bufTable.GetUshort(offset);
                            }
                        }

                        public SubstLookupRecord GetSubstLookupRecord(uint i)
                        {
                            SubstLookupRecord slr = null;

                            if (i < SubstCount)
                            {
                                uint offset = m_offsetChainSubClassRule + (uint)FieldOffsets.BacktrackClasses 
                                    + (uint)BacktrackGlyphCount*2 + 2 + (uint)(InputGlyphCount-1)*2 + 2 
                                    + (uint)LookaheadGlyphCount*2 + 2 + i*4;
                                slr = new SubstLookupRecord(offset, m_bufTable);
                            }

                            return slr;
                        }


                        protected uint m_offsetChainSubClassRule;
                        protected MBOBuffer m_bufTable;
                    }

                    public ChainSubClassRule GetChainSubClassRuleTable(uint i)
                    {
                        ChainSubClassRule cscr = null;

                        if (i < ChainSubClassRuleCount)
                        {
                            cscr = new ChainSubClassRule(m_offsetChainSubClassSet + GetChainSubClassRuleOffset(i), m_bufTable);
                        }

                        return cscr;
                    }

                    protected uint m_offsetChainSubClassSet;
                    protected MBOBuffer m_bufTable;
                }

                protected uint m_offsetChainContextSubst;
                protected MBOBuffer m_bufTable;
            }

            public class ChainContextSubstFormat3
            {
                public ChainContextSubstFormat3(uint offset, MBOBuffer bufTable)
                {
                    m_offsetChainContextSubst = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    SubstFormat              = 0,
                    BacktrackGlyphCount      = 2,
                    BacktrackCoverageOffsets = 4
                }

                public uint CalcLength()
                {
                    return (uint)FieldOffsets.BacktrackCoverageOffsets + (uint)BacktrackGlyphCount*2
                        + 2 + (uint)InputGlyphCount*2
                        + 2 + (uint)LookaheadGlyphCount*2 
                        + 2 + (uint)SubstCount*4;
                }

                public ushort SubstFormat
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.SubstFormat);}
                }

                public ushort BacktrackGlyphCount
                {
                    get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackGlyphCount);}
                }

                public ushort GetBacktrackCoverageOffset(uint i)
                {
                    uint offset = m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackCoverageOffsets + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public OTL.CoverageTable GetBacktrackCoverageTable(uint i)
                {
                    uint offset = m_offsetChainContextSubst + (uint)GetBacktrackCoverageOffset(i);
                    return new OTL.CoverageTable(offset, m_bufTable);
                }

                public ushort InputGlyphCount
                {
                    get
                    {
                        uint offset = m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackCoverageOffsets 
                            + (uint)BacktrackGlyphCount*2;
                        return m_bufTable.GetUshort(offset);
                    }
                }

                public ushort GetInputCoverageOffset(uint i)
                {
                    uint offset = m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackCoverageOffsets 
                        + (uint)BacktrackGlyphCount*2 + 2 + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public OTL.CoverageTable GetInputCoverageTable(uint i)
                {
                    uint offset = m_offsetChainContextSubst + (uint)GetInputCoverageOffset(i);
                    return new OTL.CoverageTable(offset, m_bufTable);
                }

                public ushort LookaheadGlyphCount
                {
                    get
                    {
                        uint offset = m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackCoverageOffsets 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2;
                        return m_bufTable.GetUshort(offset);
                    }
                }

                public ushort GetLookaheadCoverageOffset(uint i)
                {
                    uint offset = m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackCoverageOffsets 
                        + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2 + 2 + i*2;
                    return m_bufTable.GetUshort(offset);
                }

                public OTL.CoverageTable GetLookaheadCoverageTable(uint i)
                {
                    uint offset = m_offsetChainContextSubst + (uint)GetLookaheadCoverageOffset(i);
                    return new OTL.CoverageTable(offset, m_bufTable);
                }

                public ushort SubstCount
                {
                    get
                    {
                        uint offset = m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackCoverageOffsets 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2 + 2 
                            + (uint)LookaheadGlyphCount*2;
                        return m_bufTable.GetUshort(offset);
                    }
                }

                public SubstLookupRecord GetSubstLookupRecord(uint i)
                {
                    SubstLookupRecord slr = null;

                    if (i < SubstCount)
                    {
                        uint offset = m_offsetChainContextSubst + (uint)FieldOffsets.BacktrackCoverageOffsets 
                            + (uint)BacktrackGlyphCount*2 + 2 + (uint)InputGlyphCount*2 + 2 
                            + (uint)LookaheadGlyphCount*2 + 2 + i*4;
                        slr = new SubstLookupRecord(offset, m_bufTable);
                    }

                    return slr;
                }

                protected uint m_offsetChainContextSubst;
                protected MBOBuffer m_bufTable;
            }

            public ushort SubstFormat
            {
                get {return m_bufTable.GetUshort(m_offsetChainContextSubst + (uint)FieldOffsets.SubstFormat);}
            }

            public ChainContextSubstFormat1 GetChainContextSubstFormat1()
            {
                if (SubstFormat != 1)
                {
                    throw new System.InvalidOperationException();
                }

                return new ChainContextSubstFormat1(m_offsetChainContextSubst, m_bufTable);
            }

            public ChainContextSubstFormat2 GetChainContextSubstFormat2()
            {
                if (SubstFormat != 2)
                {
                    throw new System.InvalidOperationException();
                }

                return new ChainContextSubstFormat2(m_offsetChainContextSubst, m_bufTable);
            }

            public ChainContextSubstFormat3 GetChainContextSubstFormat3()
            {
                if (SubstFormat != 3)
                {
                    throw new System.InvalidOperationException();
                }

                return new ChainContextSubstFormat3(m_offsetChainContextSubst, m_bufTable);
            }

            protected uint m_offsetChainContextSubst;
        }

        // Lookup Type 7: Extension Substitution Subtable

        public class ExtensionSubst : OTL.SubTable
        {
            public ExtensionSubst(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetExtensionSubst = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                SubstFormat         = 0,
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
                    case 1: st = new Table_GSUB.SingleSubst      (m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;
                    case 2: st = new Table_GSUB.MultipleSubst    (m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;
                    case 3: st = new Table_GSUB.AlternateSubst   (m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;
                    case 4: st = new Table_GSUB.LigatureSubst    (m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;
                    case 5: st = new Table_GSUB.ContextSubst     (m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;
                    case 6: st = new Table_GSUB.ChainContextSubst(m_offsetExtensionSubst + ExtensionOffset, m_bufTable); break;

                }
                if (st != null)
                {
                    nLength = st.GetMaxContextLength();
                }

                return nLength;
            }
            
            
            // accessors

            public ushort SubstFormat
            {
                get {return m_bufTable.GetUshort(m_offsetExtensionSubst + (uint)FieldOffsets.SubstFormat);}
            }

            public ushort ExtensionLookupType
            {
                get {return m_bufTable.GetUshort(m_offsetExtensionSubst + (uint)FieldOffsets.ExtensionLookupType);}
            }

            public uint ExtensionOffset
            {
                get {return m_bufTable.GetUint(m_offsetExtensionSubst + (uint)FieldOffsets.ExtensionOffset);}
            }

            protected uint m_offsetExtensionSubst;
        }


        // Lookup Type 8: Reverse Chaining Contextual Single Substitution Subtable

        public class ReverseChainSubst : OTL.SubTable
        {
            public ReverseChainSubst(uint offset, MBOBuffer bufTable) : base (offset, bufTable)
            {
                m_offsetReverseChainSubst = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                SubstFormat                = 0,
                CoverageOffset             = 2,
                BacktrackGlyphCount        = 4,
                BacktrackCoverageOffsets   = 6
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

                uint tempLength = (uint)(this.BacktrackGlyphCount + this.LookaheadGlyphCount);
                if (tempLength > nLength)
                {
                    nLength = tempLength;
                }

                return nLength;
            }
            
            
            // accessors

            public ushort SubstFormat
            {
                get {return m_bufTable.GetUshort(m_offsetReverseChainSubst + (uint)FieldOffsets.SubstFormat);}
            }

            public ushort CoverageOffset
            {
                get {return m_bufTable.GetUshort(m_offsetReverseChainSubst + (uint)FieldOffsets.CoverageOffset);}
            }

            public OTL.CoverageTable GetCoverageTable()
            {
                return new OTL.CoverageTable(m_offsetReverseChainSubst + CoverageOffset, m_bufTable);
            }

            public ushort BacktrackGlyphCount
            {
                get {return m_bufTable.GetUshort(m_offsetReverseChainSubst + (uint)FieldOffsets.BacktrackGlyphCount);}
            }

            public ushort GetBacktrackCoverageOffset(uint i)
            {
                uint offset = m_offsetReverseChainSubst + (uint)FieldOffsets.BacktrackCoverageOffsets + i*2;
                return m_bufTable.GetUshort(offset);
            }

            public OTL.CoverageTable GetBacktrackCoverageTable(uint i)
            {
                uint offset = m_offsetReverseChainSubst + (uint)GetBacktrackCoverageOffset(i);
                return new OTL.CoverageTable(offset, m_bufTable);
            }

            public ushort LookaheadGlyphCount
            {
                get
                {
                    uint offset = m_offsetReverseChainSubst + (uint)FieldOffsets.BacktrackCoverageOffsets + (uint)BacktrackGlyphCount*2;
                    return m_bufTable.GetUshort(offset);
                }
            }

            public ushort GetLookaheadCoverageOffset(uint i)
            {
                uint offset = m_offsetReverseChainSubst + (uint)FieldOffsets.BacktrackCoverageOffsets + (uint)BacktrackGlyphCount*2
                     + 2 + i*2;
                return m_bufTable.GetUshort(offset);
            }

            public OTL.CoverageTable GetLookaheadCoverageTable(uint i)
            {
                uint offset = m_offsetReverseChainSubst + (uint)GetLookaheadCoverageOffset(i);
                return new OTL.CoverageTable(offset, m_bufTable);
            }

            public ushort SubstituteGlyphCount
            {
                get
                {
                    uint offset = m_offsetReverseChainSubst + (uint)FieldOffsets.BacktrackCoverageOffsets + (uint)BacktrackGlyphCount*2
                        + 2 + (uint)LookaheadGlyphCount*2 ;
                    return m_bufTable.GetUshort(offset);
                }
            }

            public ushort GetSubstituteGlyphID(uint i)
            {
                uint offset = m_offsetReverseChainSubst + (uint)FieldOffsets.BacktrackCoverageOffsets + (uint)BacktrackGlyphCount*2
                    + 2 + (uint)LookaheadGlyphCount*2 + 2 + i*2;
                return m_bufTable.GetUshort(offset);
            }

            protected uint m_offsetReverseChainSubst;
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
                m_cache = new GSUB_cache();
            }

            return m_cache;
        }
        
        public class GSUB_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        

    }
}
