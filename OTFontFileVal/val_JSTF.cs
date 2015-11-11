 using System;

using OTFontFile;
using OTFontFile.OTL;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_JSTF.
    /// </summary>
    public class val_JSTF : Table_JSTF, ITableValidate
    {
        public val_JSTF(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
            m_DataOverlapDetector = new DataOverlapDetector();
        }


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            
            // check the version
            if (v.PerformTest(T.JSTF_Version))
            {
                if (Version.GetUint() == 0x00010000)
                {
                    v.Pass(T.JSTF_Version, P.JSTF_P_Version, m_tag);
                }
                else
                {
                    v.Error(T.JSTF_Version, E.JSTF_E_Version, m_tag, "0x" + Version.GetUint().ToString("x8"));
                    bRet = false;
                }
            }

            // check the JstfScriptRecord array length
            if (v.PerformTest(T.JSTF_JstfScriptRecord_length))
            {
                if ((uint)FieldOffsets.JstfScriptRecords + JstfScriptCount*6 > m_bufTable.GetLength())
                {
                    v.Error(T.JSTF_JstfScriptRecord_length, E.JSTF_E_Array_pastEOT, m_tag, "JstfScriptRecord array");
                    bRet = false;
                }
            }

            // check that the JstfScriptRecord array is in alphabetical order
            if (v.PerformTest(T.JSTF_JstfScriptRecord_order))
            {
                if (JstfScriptCount > 1)
                {
                    for (uint i=0; i<JstfScriptCount-1; i++)
                    {
                        JstfScriptRecord jsrCurr = GetJstfScriptRecord_val(i);
                        JstfScriptRecord jsrNext = GetJstfScriptRecord_val(i+1);

                        if (jsrCurr.JstfScriptTag >= jsrNext.JstfScriptTag)
                        {
                            v.Error(T.JSTF_JstfScriptRecord_order, E.JSTF_E_Array_order, m_tag, "JstfScriptRecord array");
                            bRet = false;
                            break;
                        }
                    }
                }
            }

            // check each JstfScriptRecord
            if (v.PerformTest(T.JSTF_JstfScriptRecords))
            {
                for (uint i=0; i<JstfScriptCount; i++)
                {
                    JstfScriptRecord_val jsr = GetJstfScriptRecord_val(i);

                    // check the tag
                    if (!jsr.JstfScriptTag.IsValid())
                    {
                        v.Error(T.JSTF_JstfScriptRecords, E.JSTF_E_tag, m_tag, "JstfScriptRecord[" + i + "]");
                        bRet = false;
                    }

                    // check the offset
                    if (jsr.JstfScriptOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.JSTF_JstfScriptRecords, E.JSTF_E_Offset_PastEOT, m_tag, "JstfScriptRecord[" + i + "]");
                        bRet = false;
                    }

                    // validate the JstfScript table
                    JstfScript_val js = jsr.GetJstfScriptTable_val();
                    js.Validate(v, "JstfScriptRecord[" + i + "]", this);
                }
            }

            return bRet;
        }


        /************************
         * nested classes
         */

        public class JstfScriptRecord_val : JstfScriptRecord
        {
            public JstfScriptRecord_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public JstfScript_val GetJstfScriptTable_val()
            {
                return new JstfScript_val(JstfScriptOffset, m_bufTable);
            }
        }

        public class JstfScript_val : JstfScript, I_OTLValidate
        {
            public JstfScript_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                // check for data overlap
                bRet &= ((val_JSTF)table).ValidateNoOverlap(m_offsetJstfScript, CalcLength(), v, sIdentity, table.GetTag());

                // check the ExtenderGlyph offset
                if (ExtenderGlyphOffset != 0)
                {
                    if (m_offsetJstfScript + ExtenderGlyphOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", ExtenderGlyph");
                        bRet = false;
                    }
                }

                // check the ExtenderGlyph table
                if (ExtenderGlyphOffset != 0)
                {
                    ExtenderGlyph_val eg = GetExtenderGlyphTable_val();
                    bRet &= eg.Validate(v, sIdentity + ", ExtenderGlyph", table);
                }

                // check the DefJstfLangSys offset
                if (DefJstfLangSysOffset != 0)
                {
                    if (m_offsetJstfScript + DefJstfLangSysOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", DefJstfLangSys");
                        bRet = false;
                    }
                }

                // check the DefJstfLangSys table
                if (DefJstfLangSysOffset != 0)
                {
                    JstfLangSys_val jls = GetDefJstfLangSysTable_val();
                    bRet &= jls.Validate(v, sIdentity + ", DefJstfLangSys", table);
                }

                // check the JstfLangSysRecord array length
                if (m_offsetJstfScript + (uint)FieldOffsets.JstfLangSysRecords + JstfLangSysCount*6 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.JSTF_E_Array_pastEOT, table.m_tag, sIdentity + ", JstfLangSysRecord array");
                    bRet = false;
                }

                // check the JstfLangSysRecord array order
                if (JstfLangSysCount > 1)
                {
                    for (uint i=0; i<JstfLangSysCount-1; i++)
                    {
                        JstfLangSysRecord jlsrCurr = GetJstfLangSysRecord(i);
                        JstfLangSysRecord jlsrNext = GetJstfLangSysRecord(i);

                        if (jlsrCurr.JstfLangSysTag >= jlsrNext.JstfLangSysTag)
                        {
                            v.Error(T.T_NULL, E.JSTF_E_Array_order, table.m_tag, sIdentity + ", JstfLangSysRecord[" + i + "]");
                            bRet = false;
                            break;
                        }
                    }
                }

                // check each JstfLangSysRecord
                for (uint i=0; i<JstfLangSysCount; i++)
                {
                    JstfLangSysRecord jlsr = GetJstfLangSysRecord(i);

                    // check the tag
                    if (!jlsr.JstfLangSysTag.IsValid())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_tag, table.m_tag, sIdentity + ", JstfLangSysRecord[" + i + "]");
                        bRet = false;
                    }

                    // check the JstfLangSys offset
                    if (m_offsetJstfScript + jlsr.JstfLangSysOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", JstfLangSysRecord[" + i + "]");
                        bRet = false;
                    }

                    // check the JstfLangSys table
                    JstfLangSys_val jls = GetJstfLangSysTable_val(jlsr);
                    bRet &= jls.Validate(v, sIdentity + ", JstfLangSysRecord[" + i + "], JstfLangSys", table);
                }

                return bRet;
            }
            
            public ExtenderGlyph_val GetExtenderGlyphTable_val()
            {
                ExtenderGlyph_val eg = null;

                if (ExtenderGlyphOffset != 0)
                {
                    uint offset = m_offsetJstfScript + (uint)ExtenderGlyphOffset;
                    eg = new ExtenderGlyph_val((ushort)offset, m_bufTable);
                }

                return eg;
            }

            public JstfLangSys_val GetDefJstfLangSysTable_val()
            {
                JstfLangSys_val djls = null;

                if (DefJstfLangSysOffset != 0)
                {
                    uint offset = m_offsetJstfScript + (uint)DefJstfLangSysOffset;
                    djls = new JstfLangSys_val((ushort)offset, m_bufTable);
                }

                return djls;
            }

            public JstfLangSys_val GetJstfLangSysTable_val(JstfLangSysRecord jlsr)
            {
                uint offset = m_offsetJstfScript + (uint)jlsr.JstfLangSysOffset;
                return new JstfLangSys_val((ushort)offset, m_bufTable);
            }
        }

        public class ExtenderGlyph_val : ExtenderGlyph, I_OTLValidate
        {
            public ExtenderGlyph_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                // check for data overlap
                bRet &= ((val_JSTF)table).ValidateNoOverlap(m_offsetExtenderGlyph, CalcLength(), v, sIdentity, table.GetTag());

                // check the ExtenderGlyph array length
                if (m_offsetExtenderGlyph + (uint)FieldOffsets.ExtenderGlyphs + GlyphCount*2 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.JSTF_E_Array_pastEOT, table.m_tag, sIdentity + ", ExtenderGlyph array");
                    bRet = false;
                }

                // check that the ExtenderGlyph array is in increasing numerical order
                if (GlyphCount > 1)
                {
                    for (uint i=0; i<GlyphCount-1; i++)
                    {
                        if (GetExtenderGlyph(i) >= GetExtenderGlyph(i+1))
                        {
                            v.Error(T.T_NULL, E.JSTF_E_Array_order, table.m_tag, sIdentity + ", ExtenderGlyph array");
                            bRet = false;
                            break;
                        }
                    }
                }


                return bRet;
            }
        }

        public class JstfLangSys_val : JstfLangSys, I_OTLValidate
        {
            public JstfLangSys_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                // check for data overlap
                bRet &= ((val_JSTF)table).ValidateNoOverlap(m_offsetJstfLangSys, CalcLength(), v, sIdentity, table.GetTag());

                // check the JstfPriority array length
                if (m_offsetJstfLangSys + (uint)FieldOffsets.JstfPriorityOffsets + JstfPriorityCount*2 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.JSTF_E_Array_pastEOT, table.m_tag, sIdentity + ", JstfPriority array");
                    bRet = false;
                }

                // check each JstfPriority offset
                for (uint i=0; i<JstfPriorityCount; i++)
                {
                    if (m_offsetJstfLangSys + GetJstfPriorityOffset(i) > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", JstfPriority[" + i + "]");
                        bRet = false;
                    }
                }

                // check each JstfPriority table
                for (uint i=0; i<JstfPriorityCount; i++)
                {
                    JstfPriority_val jp = GetJstfPriorityTable_val(i);
                    bRet &= jp.Validate(v, sIdentity + ", JstfPriority[" + i + "]", table);
                }

                return bRet;
            }
            
            public JstfPriority_val GetJstfPriorityTable_val(uint i)
            {
                JstfPriority_val jp = null;

                if (i < JstfPriorityCount)
                {
                    uint offset = m_offsetJstfLangSys + (uint)GetJstfPriorityOffset(i);
                    jp = new JstfPriority_val((ushort)offset, m_bufTable);
                }

                return jp;
            }
        }

        public class JstfPriority_val : JstfPriority
        {
            public JstfPriority_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                // check for data overlap
                bRet &= ((val_JSTF)table).ValidateNoOverlap(m_offsetJstfPriority, CalcLength(), v, sIdentity, table.GetTag());

                // check the ShrinkageEnableGSUB offset
                if (ShrinkageEnableGSUBOffset != 0)
                {
                    if (m_offsetJstfPriority + ShrinkageEnableGSUBOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", ShrinkageEnableGSUB");
                        bRet = false;
                    }
                }

                // check the ShrinkageEnableGSUB table
                if (ShrinkageEnableGSUBOffset != 0)
                {
                    JstfGSUBModList_val jgml = GetShrinkageEnableGSUBTable_val();
                    bRet &= jgml.Validate(v, sIdentity + ", ShrinkageEnableGSUB", table);
                }

                // check the ShrinkageDisableGSUB offset
                if (ShrinkageDisableGSUBOffset != 0)
                {
                    if (m_offsetJstfPriority + ShrinkageDisableGSUBOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", ShrinkageDisableGSUB");
                        bRet = false;
                    }
                }

                // check the ShrinkageDisableGSUB table
                if (ShrinkageDisableGSUBOffset != 0)
                {
                    JstfGSUBModList_val jgml = GetShrinkageDisableGSUBTable_val();
                    bRet &= jgml.Validate(v, sIdentity + ", ShrinkageDisableGSUB", table);
                }

                // check the ShrinkageEnableGPOS offset
                if (ShrinkageEnableGPOSOffset != 0)
                {
                    if (m_offsetJstfPriority + ShrinkageEnableGPOSOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", ShrinkageEnableGPOS");
                        bRet = false;
                    }
                }

                // check the ShrinkageEnableGPOS table
                if (ShrinkageEnableGPOSOffset != 0)
                {
                    JstfGPOSModList_val jgml = GetShrinkageEnableGPOSTable_val();
                    bRet &= jgml.Validate(v, sIdentity + ", ShrinkageEnableGPOS", table);
                }

                // check the ShrinkageDisableGPOS offset
                if (ShrinkageDisableGPOSOffset != 0)
                {
                    if (m_offsetJstfPriority + ShrinkageDisableGPOSOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", ShrinkageDisableGPOS");
                        bRet = false;
                    }
                }

                // check the ShrinkageDisableGPOS table
                if (ShrinkageDisableGPOSOffset != 0)
                {
                    JstfGPOSModList_val jgml = GetShrinkageDisableGPOSTable_val();
                    bRet &= jgml.Validate(v, sIdentity + ", ShrinkageDisableGPOS", table);
                }

                // check the ShrinkageJstfMax offset
                if (ShrinkageJstfMaxOffset != 0)
                {
                    if (m_offsetJstfPriority + ShrinkageJstfMaxOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", ShrinkageJstfMax");
                        bRet = false;
                    }
                }

                // check the ShrinkageJstfMax table
                if (ShrinkageJstfMaxOffset != 0)
                {
                    JstfMax_val jm = GetShrinkageJstfMaxTable_val();
                    bRet &= jm.Validate(v, sIdentity + ", ShrinkageJstfMax", table);
                }

                // check the ExtensionEnableGSUB offset
                if (ExtensionEnableGSUBOffset != 0)
                {
                    if (m_offsetJstfPriority + ExtensionEnableGSUBOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", ExtensionEnableGSUB");
                        bRet = false;
                    }
                }

                // check the ExtensionEnableGSUB table
                if (ExtensionEnableGSUBOffset != 0)
                {
                    JstfGSUBModList_val jgml = GetExtensionEnableGSUBTable_val();
                    bRet &= jgml.Validate(v, sIdentity + ", ExtensionEnableGSUB", table);
                }

                // check the ExtensionDisableGSUB offset
                if (ExtensionDisableGSUBOffset != 0)
                {
                    if (m_offsetJstfPriority + ExtensionDisableGSUBOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", ExtensionDisableGSUB");
                        bRet = false;
                    }
                }

                // check the ExtensionDisableGSUB table
                if (ExtensionDisableGSUBOffset != 0)
                {
                    JstfGSUBModList_val jgml = GetExtensionDisableGSUBTable_val();
                    bRet &= jgml.Validate(v, sIdentity + ", ExtensionDisableGSUB", table);
                }

                // check the ExtensionEnableGPOS offset
                if (ExtensionEnableGPOSOffset != 0)
                {
                    if (m_offsetJstfPriority + ExtensionEnableGPOSOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", ExtensionEnableGPOS");
                        bRet = false;
                    }
                }

                // check the ExtensionEnableGPOS table
                if (ExtensionEnableGPOSOffset != 0)
                {
                    JstfGPOSModList_val jgml = GetExtensionEnableGPOSTable_val();
                    bRet &= jgml.Validate(v, sIdentity + ", ExtensionEnableGPOS", table);
                }

                // check the ExtensionDisableGPOS offset
                if (ExtensionDisableGPOSOffset != 0)
                {
                    if (m_offsetJstfPriority + ExtensionDisableGPOSOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", ExtensionDisableGPOS");
                        bRet = false;
                    }
                }

                // check the ExtensionDisableGPOS table
                if (ExtensionDisableGPOSOffset != 0)
                {
                    JstfGPOSModList_val jgml = GetExtensionDisableGPOSTable_val();
                    bRet &= jgml.Validate(v, sIdentity + ", ExtensionDisableGPOS", table);
                }

                // check the ExtensionJstfMax offset
                if (ExtensionJstfMaxOffset != 0)
                {
                    if (m_offsetJstfPriority + ExtensionJstfMaxOffset > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", ExtensionJstfMax");
                        bRet = false;
                    }
                }

                // check the ExtensionJstfMax table
                if (ExtensionJstfMaxOffset != 0)
                {
                    JstfMax_val jm = GetExtensionJstfMaxTable_val();
                    bRet &= jm.Validate(v, sIdentity + ", ExtensionJstfMax", table);
                }

                return bRet;
            }
            
            public JstfGSUBModList_val GetShrinkageEnableGSUBTable_val()
            {
                JstfGSUBModList_val jgml = null;

                if (ShrinkageEnableGSUBOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ShrinkageEnableGSUBOffset;
                    jgml = new JstfGSUBModList_val((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public JstfGSUBModList_val GetShrinkageDisableGSUBTable_val()
            {
                JstfGSUBModList_val jgml = null;

                if (ShrinkageDisableGSUBOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ShrinkageDisableGSUBOffset;
                    jgml = new JstfGSUBModList_val((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public JstfGPOSModList_val GetShrinkageEnableGPOSTable_val()
            {
                JstfGPOSModList_val jgml = null;

                if (ShrinkageEnableGPOSOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ShrinkageEnableGPOSOffset;
                    jgml = new JstfGPOSModList_val((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public JstfGPOSModList_val GetShrinkageDisableGPOSTable_val()
            {
                JstfGPOSModList_val jgml = null;

                if (ShrinkageDisableGPOSOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ShrinkageDisableGPOSOffset;
                    jgml = new JstfGPOSModList_val((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public JstfMax_val GetShrinkageJstfMaxTable_val()
            {
                JstfMax_val jm = null;

                if (ShrinkageJstfMaxOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ShrinkageJstfMaxOffset;
                    jm = new JstfMax_val((ushort)offset, m_bufTable);
                }

                return jm;
            }

            public JstfGSUBModList_val GetExtensionEnableGSUBTable_val()
            {
                JstfGSUBModList_val jgml = null;

                if (ExtensionEnableGSUBOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ExtensionEnableGSUBOffset;
                    jgml = new JstfGSUBModList_val((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public JstfGSUBModList_val GetExtensionDisableGSUBTable_val()
            {
                JstfGSUBModList_val jgml = null;

                if (ExtensionDisableGSUBOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ExtensionDisableGSUBOffset;
                    jgml = new JstfGSUBModList_val((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public JstfGPOSModList_val GetExtensionEnableGPOSTable_val()
            {
                JstfGPOSModList_val jgml = null;

                if (ExtensionEnableGPOSOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ExtensionEnableGPOSOffset;
                    jgml = new JstfGPOSModList_val((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public JstfGPOSModList_val GetExtensionDisableGPOSTable_val()
            {
                JstfGPOSModList_val jgml = null;

                if (ExtensionDisableGPOSOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ExtensionDisableGPOSOffset;
                    jgml = new JstfGPOSModList_val((ushort)offset, m_bufTable);
                }

                return jgml;
            }

            public JstfMax_val GetExtensionJstfMaxTable_val()
            {
                JstfMax_val jm = null;

                if (ExtensionJstfMaxOffset != 0)
                {
                    uint offset = m_offsetJstfPriority + (uint)ExtensionJstfMaxOffset;
                    jm = new JstfMax_val((ushort)offset, m_bufTable);
                }

                return jm;
            }
        }

        public class JstfGSUBModList_val : JstfGSUBModList, I_OTLValidate
        {
            public JstfGSUBModList_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                // check for data overlap
                bRet &= ((val_JSTF)table).ValidateNoOverlap(m_offsetJstfGSUBModList, CalcLength(), v, sIdentity, table.GetTag());

                // check GSUBLookupIndex array size
                if (m_offsetJstfGSUBModList + (uint)FieldOffsets.GSUBLookupIndex + LookupCount*2 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.JSTF_E_Array_pastEOT, table.m_tag, sIdentity + ", GSUBLookupIndex");
                    bRet = false;
                }

                // check that GSUBLookupIndex is in increasing numerical order
                if (LookupCount > 1)
                {
                    for (uint i=0; i<LookupCount-1; i++)
                    {
                        if (GetGSUBLookupIndex(i) >= GetGSUBLookupIndex(i+1))
                        {
                            v.Error(T.T_NULL, E.JSTF_E_Array_order, table.m_tag, sIdentity + ", GSUBLookupIndex");
                            bRet = false;
                            break;
                        }
                    }
                }

                return bRet;
            }
        }

        public class JstfGPOSModList_val : JstfGPOSModList, I_OTLValidate
        {
            public JstfGPOSModList_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                // check for data overlap
                bRet &= ((val_JSTF)table).ValidateNoOverlap(m_offsetJstfGPOSModList, CalcLength(), v, sIdentity, table.GetTag());

                // check GPOSLookupIndex array size
                if (m_offsetJstfGPOSModList + (uint)FieldOffsets.GPOSLookupIndex + LookupCount*2 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.JSTF_E_Array_pastEOT, table.m_tag, sIdentity + ", GPOSLookupIndex");
                    bRet = false;
                }

                // check that GPOSLookupIndex is in increasing numerical order
                if (LookupCount > 1)
                {
                    for (uint i=0; i<LookupCount-1; i++)
                    {
                        if (GetGPOSLookupIndex(i) >= GetGPOSLookupIndex(i+1))
                        {
                            v.Error(T.T_NULL, E.JSTF_E_Array_order, table.m_tag, sIdentity + ", GPOSLookupIndex");
                            bRet = false;
                            break;
                        }
                    }
                }

                return bRet;
            }
            
        }

        public class JstfMax_val : JstfMax, I_OTLValidate
        {
            public JstfMax_val(ushort offset, MBOBuffer bufTable) : base(offset, bufTable)
            {
            }

            public bool Validate(Validator v, string sIdentity, OTTable table)
            {
                bool bRet = true;

                // check for data overlap
                bRet &= ((val_JSTF)table).ValidateNoOverlap(m_offsetJstfMax, CalcLength(), v, sIdentity, table.GetTag());

                // check the Lookup array length
                if (m_offsetJstfMax + (uint)FieldOffsets.LookupOffsets + LookupCount*2 > m_bufTable.GetLength())
                {
                    v.Error(T.T_NULL, E.JSTF_E_Array_pastEOT, table.m_tag, sIdentity + ", Lookup");
                    bRet = false;
                }

                // check each Lookup offset
                for (uint i=0; i<LookupCount; i++)
                {
                    if (m_offsetJstfMax + GetLookupOffset(i) > m_bufTable.GetLength())
                    {
                        v.Error(T.T_NULL, E.JSTF_E_Offset_PastEOT, table.m_tag, sIdentity + ", Lookup[" + i + "]");
                        bRet = false;
                    }
                }

                // check each Lookup table
                for (uint i=0; i<LookupCount; i++)
                {
                    LookupTable_val lt = GetLookupTable_val(i);
                    bRet &= lt.Validate(v, sIdentity + ", Lookup[" + i + "]", table);
                }

                return bRet;
            }
            
            public LookupTable_val GetLookupTable_val(uint i)
            {
                return new LookupTable_val((ushort)(m_offsetJstfMax + GetLookupOffset(i)), m_bufTable, "GPOS");
            }
        }
        
        public JstfScriptRecord_val GetJstfScriptRecord_val(uint i)
        {
            JstfScriptRecord_val jsr = null;

            if (i < JstfScriptCount)
            {
                uint offset = (uint)FieldOffsets.JstfScriptRecords + i*6;
                jsr = new JstfScriptRecord_val((ushort)offset, m_bufTable);
            }

            return jsr;
        }


        public bool ValidateNoOverlap(uint offset, uint length, Validator v, string sIdentity, OTTag tag)
        {
            bool bValid = m_DataOverlapDetector.CheckForNoOverlap(offset, length);

            if (!bValid)
            {
                // JJF What to do?
                v.Error(T.T_NULL,E._Table_E_DataOverlap, tag, sIdentity + ", offset = " + offset + ", length = " + length);
            }

            return bValid;
        }

        protected DataOverlapDetector m_DataOverlapDetector;
    }
}
