using System;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_JSTF.
    /// </summary>
    public class Table_JSTF : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_JSTF(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            Version           = 0,
            JstfScriptCount   = 4,
            JstfScriptRecords = 6
        }



        /************************
         * classes
         */

        public class JstfScriptRecord
        {
            public JstfScriptRecord(ushort offset, MBOBuffer bufTable)
            {
                m_offsetJstfScriptRecord = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                JstfScriptTag = 0,
                JstfScriptOffset = 4
            }

            public OTTag JstfScriptTag
            {
                get {return m_bufTable.GetTag(m_offsetJstfScriptRecord + (uint)FieldOffsets.JstfScriptTag);}
            }

            public ushort JstfScriptOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfScriptRecord + (uint)FieldOffsets.JstfScriptOffset);}
            }

            public JstfScript GetJstfScriptTable()
            {
                return new JstfScript(JstfScriptOffset, m_bufTable);
            }

            protected ushort m_offsetJstfScriptRecord;
            protected MBOBuffer m_bufTable;
        }

        public class JstfScript
        {
            public JstfScript(ushort offset, MBOBuffer bufTable)
            {
                m_offsetJstfScript = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                ExtenderGlyphOffset  = 0,
                DefJstfLangSysOffset = 2,
                JstfLangSysCount     = 4,
                JstfLangSysRecords   = 6
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.JstfLangSysRecords + (uint)JstfLangSysCount*6;
            }

            public ushort ExtenderGlyphOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfScript + (uint)FieldOffsets.ExtenderGlyphOffset);}
            }

            public ExtenderGlyph GetExtenderGlyphTable()
            {
                ExtenderGlyph eg = null;

                if (ExtenderGlyphOffset != 0)
                {
                    uint offset = m_offsetJstfScript + (uint)ExtenderGlyphOffset;
                    eg = new ExtenderGlyph((ushort)offset, m_bufTable);
                }

                return eg;
            }

            public ushort DefJstfLangSysOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfScript + (uint)FieldOffsets.DefJstfLangSysOffset);}
            }

            public JstfLangSys GetDefJstfLangSysTable()
            {
                JstfLangSys djls = null;

                if (DefJstfLangSysOffset != 0)
                {
                    uint offset = m_offsetJstfScript + (uint)DefJstfLangSysOffset;
                    djls = new JstfLangSys((ushort)offset, m_bufTable);
                }

                return djls;
            }

            public ushort JstfLangSysCount
            {
                get {return m_bufTable.GetUshort(m_offsetJstfScript + (uint)FieldOffsets.JstfLangSysCount);}
            }

            public class JstfLangSysRecord
            {
                public JstfLangSysRecord(ushort offset, MBOBuffer bufTable)
                {
                    m_offsetJstfLangSysRecord = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    JstfLangSysTag    = 0,
                    JstfLangSysOffset = 4
                }

                public OTTag JstfLangSysTag
                {
                    get {return m_bufTable.GetTag(m_offsetJstfLangSysRecord + (uint)FieldOffsets.JstfLangSysTag);}
                }

                public ushort JstfLangSysOffset
                {
                    get {return m_bufTable.GetUshort(m_offsetJstfLangSysRecord + (uint)FieldOffsets.JstfLangSysOffset);}
                }

                protected ushort m_offsetJstfLangSysRecord;
                protected MBOBuffer m_bufTable;
            }

            public JstfLangSysRecord GetJstfLangSysRecord(uint i)
            {
                JstfLangSysRecord jlsr = null;

                if (i < JstfLangSysCount)
                {
                    uint offset = m_offsetJstfScript + (uint)FieldOffsets.JstfLangSysRecords + i*6;
                    jlsr = new JstfLangSysRecord((ushort)offset, m_bufTable);
                }

                return jlsr;
            }

            public JstfLangSys GetJstfLangSysTable(JstfLangSysRecord jlsr)
            {
                uint offset = m_offsetJstfScript + (uint)jlsr.JstfLangSysOffset;
                return new JstfLangSys((ushort)offset, m_bufTable);
            }

            protected ushort m_offsetJstfScript;
            protected MBOBuffer m_bufTable;
        }

        public class ExtenderGlyph
        {
            public ExtenderGlyph(ushort offset, MBOBuffer bufTable)
            {
                m_offsetExtenderGlyph = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                GlyphCount = 0,
                ExtenderGlyphs = 2
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.ExtenderGlyphs + (uint)GlyphCount*2;
            }

            public ushort GlyphCount
            {
                get {return m_bufTable.GetUshort(m_offsetExtenderGlyph + (uint)FieldOffsets.GlyphCount);}
            }

            public ushort GetExtenderGlyph(uint i)
            {
                if (i >= GlyphCount)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return m_bufTable.GetUshort(m_offsetExtenderGlyph + (uint)FieldOffsets.ExtenderGlyphs + i*2);
            }

            protected ushort m_offsetExtenderGlyph;
            protected MBOBuffer m_bufTable;
        }

        public class JstfLangSys
        {
            public JstfLangSys(ushort offset, MBOBuffer bufTable)
            {
                m_offsetJstfLangSys = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                JstfPriorityCount   = 0,
                JstfPriorityOffsets = 2
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.JstfPriorityOffsets + (uint)JstfPriorityCount*2;
            }

            public ushort JstfPriorityCount
            {
                get {return m_bufTable.GetUshort(m_offsetJstfLangSys + (uint)FieldOffsets.JstfPriorityCount);}
            }

            public ushort GetJstfPriorityOffset(uint i)
            {
                if (i >= JstfPriorityCount)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return m_bufTable.GetUshort(m_offsetJstfLangSys + (uint)FieldOffsets.JstfPriorityOffsets + i*2);
            }

            public JstfPriority GetJstfPriorityTable(uint i)
            {
                JstfPriority jp = null;

                if (i < JstfPriorityCount)
                {
                    uint offset = m_offsetJstfLangSys + (uint)GetJstfPriorityOffset(i);
                    jp = new JstfPriority((ushort)offset, m_bufTable);
                }

                return jp;
            }

            protected ushort m_offsetJstfLangSys;
            protected MBOBuffer m_bufTable;
        }

        public class JstfPriority
        {
            public JstfPriority(ushort offset, MBOBuffer bufTable)
            {
                m_offsetJstfPriority = offset;
                m_bufTable = bufTable;
            }

            public uint CalcLength()
            {
                return 20;
            }

            public enum FieldOffsets
            {
                ShrinkageEnableGSUBOffset  = 0,
                ShrinkageDisableGSUBOffset = 2,
                ShrinkageEnableGPOSOffset  = 4,
                ShrinkageDisableGPOSOffset = 6,
                ShrinkageJstfMaxOffset     = 8,
                ExtensionEnableGSUBOffset  = 10,
                ExtensionDisableGSUBOffset = 12,
                ExtensionEnableGPOSOffset  = 14,
                ExtensionDisableGPOSOffset = 16,
                ExtensionJstfMaxOffset     = 18
            }

            public ushort ShrinkageEnableGSUBOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfPriority + (uint)FieldOffsets.ShrinkageEnableGSUBOffset);}
            }

            public JstfGSUBModList GetShrinkageEnableGSUBTable()
            {
                JstfGSUBModList jgml = null;

                if (ShrinkageEnableGSUBOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ShrinkageEnableGSUBOffset;
                    jgml = new JstfGSUBModList((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public ushort ShrinkageDisableGSUBOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfPriority + (uint)FieldOffsets.ShrinkageDisableGSUBOffset);}
            }

            public JstfGSUBModList GetShrinkageDisableGSUBTable()
            {
                JstfGSUBModList jgml = null;

                if (ShrinkageDisableGSUBOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ShrinkageDisableGSUBOffset;
                    jgml = new JstfGSUBModList((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public ushort ShrinkageEnableGPOSOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfPriority + (uint)FieldOffsets.ShrinkageEnableGPOSOffset);}
            }

            public JstfGPOSModList GetShrinkageEnableGPOSTable()
            {
                JstfGPOSModList jgml = null;

                if (ShrinkageEnableGPOSOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ShrinkageEnableGPOSOffset;
                    jgml = new JstfGPOSModList((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public ushort ShrinkageDisableGPOSOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfPriority + (uint)FieldOffsets.ShrinkageDisableGPOSOffset);}
            }

            public JstfGPOSModList GetShrinkageDisableGPOSTable()
            {
                JstfGPOSModList jgml = null;

                if (ShrinkageDisableGPOSOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ShrinkageDisableGPOSOffset;
                    jgml = new JstfGPOSModList((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public ushort ShrinkageJstfMaxOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfPriority + (uint)FieldOffsets.ShrinkageJstfMaxOffset);}
            }

            public JstfMax GetShrinkageJstfMaxTable()
            {
                JstfMax jm = null;

                if (ShrinkageJstfMaxOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ShrinkageJstfMaxOffset;
                    jm = new JstfMax((ushort)offset, m_bufTable);
                }

                return jm;
            }

            public ushort ExtensionEnableGSUBOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfPriority + (uint)FieldOffsets.ExtensionEnableGSUBOffset);}
            }

            public JstfGSUBModList GetExtensionEnableGSUBTable()
            {
                JstfGSUBModList jgml = null;

                if (ExtensionEnableGSUBOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ExtensionEnableGSUBOffset;
                    jgml = new JstfGSUBModList((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public ushort ExtensionDisableGSUBOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfPriority + (uint)FieldOffsets.ExtensionDisableGSUBOffset);}
            }

            public JstfGSUBModList GetExtensionDisableGSUBTable()
            {
                JstfGSUBModList jgml = null;

                if (ExtensionDisableGSUBOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ExtensionDisableGSUBOffset;
                    jgml = new JstfGSUBModList((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public ushort ExtensionEnableGPOSOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfPriority + (uint)FieldOffsets.ExtensionEnableGPOSOffset);}
            }

            public JstfGPOSModList GetExtensionEnableGPOSTable()
            {
                JstfGPOSModList jgml = null;

                if (ExtensionEnableGPOSOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ExtensionEnableGPOSOffset;
                    jgml = new JstfGPOSModList((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public ushort ExtensionDisableGPOSOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfPriority + (uint)FieldOffsets.ExtensionDisableGPOSOffset);}
            }

            public JstfGPOSModList GetExtensionDisableGPOSTable()
            {
                JstfGPOSModList jgml = null;

                if (ExtensionDisableGPOSOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ExtensionDisableGPOSOffset;
                    jgml = new JstfGPOSModList((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public ushort ExtensionJstfMaxOffset
            {
                get {return m_bufTable.GetUshort(m_offsetJstfPriority + (uint)FieldOffsets.ExtensionJstfMaxOffset);}
            }

            public JstfMax GetExtensionJstfMaxTable()
            {
                JstfMax jm = null;

                if (ExtensionJstfMaxOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ExtensionJstfMaxOffset;
                    jm = new JstfMax((ushort)offset, m_bufTable);
                }

                return jm;
            }

            protected ushort m_offsetJstfPriority;
            protected MBOBuffer m_bufTable;
        }

        public class JstfGSUBModList
        {
            public JstfGSUBModList(ushort offset, MBOBuffer bufTable)
            {
                m_offsetJstfGSUBModList = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                LookupCount     = 0,
                GSUBLookupIndex = 2
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.GSUBLookupIndex + (uint)LookupCount*2;
            }

            public ushort LookupCount
            {
                get {return m_bufTable.GetUshort(m_offsetJstfGSUBModList + (uint)FieldOffsets.LookupCount);}
            }

            public ushort GetGSUBLookupIndex(uint i)
            {
                if (i >= LookupCount)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return m_bufTable.GetUshort(m_offsetJstfGSUBModList + (uint)FieldOffsets.GSUBLookupIndex + i*2);
            }

            protected ushort m_offsetJstfGSUBModList;
            protected MBOBuffer m_bufTable;
        }

        public class JstfGPOSModList
        {
            public JstfGPOSModList(ushort offset, MBOBuffer bufTable)
            {
                m_offsetJstfGPOSModList = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                LookupCount     = 0,
                GPOSLookupIndex = 2
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.GPOSLookupIndex + (uint)LookupCount*2;
            }

            public ushort LookupCount
            {
                get {return m_bufTable.GetUshort(m_offsetJstfGPOSModList + (uint)FieldOffsets.LookupCount);}
            }

            public ushort GetGPOSLookupIndex(uint i)
            {
                if (i >= LookupCount)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return m_bufTable.GetUshort(m_offsetJstfGPOSModList + (uint)FieldOffsets.GPOSLookupIndex + i*2);
            }

            protected ushort m_offsetJstfGPOSModList;
            protected MBOBuffer m_bufTable;
        }

        public class JstfMax
        {
            public JstfMax(ushort offset, MBOBuffer bufTable)
            {
                m_offsetJstfMax = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                LookupCount   = 0,
                LookupOffsets = 2
            }

            public uint CalcLength()
            {
                return (uint)FieldOffsets.LookupOffsets + (uint)LookupCount*2;
            }

            public ushort LookupCount
            {
                get {return m_bufTable.GetUshort(m_offsetJstfMax + (uint)FieldOffsets.LookupCount);}
            }

            public ushort GetLookupOffset(uint i)
            {
                if (i >= LookupCount)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return m_bufTable.GetUshort(m_offsetJstfMax + (uint)FieldOffsets.LookupOffsets + i*2);
            }

            public OTL.LookupTable GetLookupTable(uint i)
            {
                return new OTL.LookupTable((ushort)(m_offsetJstfMax + GetLookupOffset(i)), m_bufTable, "GPOS");
            }

            protected ushort m_offsetJstfMax;
            protected MBOBuffer m_bufTable;
        }
        
        
        /************************
         * accessors
         */
        
        public OTFixed Version
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.Version);}
        }

        public ushort JstfScriptCount
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.JstfScriptCount);}
        }

        public JstfScriptRecord GetJstfScriptRecord(uint i)
        {
            JstfScriptRecord jsr = null;

            if (i < JstfScriptCount)
            {
                uint offset = (uint)FieldOffsets.JstfScriptRecords + i*6;
                jsr = new JstfScriptRecord((ushort)offset, m_bufTable);
            }

            return jsr;
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new JSTF_cache();
            }

            return m_cache;
        }
        
        public class JSTF_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        

    }
}
