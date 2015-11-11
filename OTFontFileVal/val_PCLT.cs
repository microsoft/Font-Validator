using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_PCLT.
    /// </summary>
    public class val_PCLT : Table_PCLT, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_PCLT(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.PCLT_TableLength))
            {
                if (GetLength() == 54)
                {
                    v.Pass(T.PCLT_TableLength, P.PCLT_P_TableLength, m_tag);
                }
                else
                {
                    v.Error(T.PCLT_TableLength, E.PCLT_E_TableLength, m_tag, GetLength().ToString());
                    bRet = false;
                }
            }

            if (v.PerformTest(T.PCLT_Version))
            {
                if (Version.GetUint() == 0x00010000)
                {
                    v.Pass(T.PCLT_Version, P.PCLT_P_Version, m_tag);
                }
                else
                {
                    v.Error(T.PCLT_Version, E.PCLT_E_Version, m_tag, "0x"+Version.GetUint().ToString("x8"));
                    bRet = false;
                }
            }

            if (v.PerformTest(T.PCLT_Pitch))
            {
                Table_hmtx hmtxTable = (Table_hmtx)fontOwner.GetTable("hmtx");
                Table_maxp maxpTable = (Table_maxp)fontOwner.GetTable("maxp");
                if (hmtxTable == null)
                {
                    v.Error(T.PCLT_Pitch, E._TEST_E_TableMissing, m_tag, "hmtx");
                    bRet = false;
                }
                else if (maxpTable == null)
                {
                    v.Error(T.PCLT_Pitch, E._TEST_E_TableMissing, m_tag, "maxp");
                    bRet = false;
                }
                else
                {
                    uint iSpaceGlyph = fontOwner.FastMapUnicodeToGlyphID(' ');

                    if (iSpaceGlyph < fontOwner.GetMaxpNumGlyphs())
                    {
                        Table_hmtx.longHorMetric hmSpace = hmtxTable.GetOrMakeHMetric(iSpaceGlyph, fontOwner);
                        if (hmSpace != null)
                        {
                            if (Pitch == hmSpace.advanceWidth)
                            {
                                v.Pass(T.PCLT_Pitch, P.PCLT_P_Pitch, m_tag);
                            }
                            else
                            {
                                string s = "actual = " + Pitch + ", expected = " + hmSpace.advanceWidth;
                                v.Error(T.PCLT_Pitch, E.PCLT_E_Pitch, m_tag, s);
                                bRet = false;
                            }
                        }
                    }
                    else
                    {
                        // JJF Figure out what to do
                        v.Warning(T.PCLT_Pitch, W._TEST_W_ErrorInAnotherTable, m_tag, "can't validate Pitch field, error getting the space glyph");
                        bRet = false;
                    }
                }
            }

            if (v.PerformTest(T.PCLT_Style))
            {
                ushort Posture   = (ushort)(Style      & 0x0003);
                ushort Width     = (ushort)((Style>>2) & 0x0007);
                ushort Structure = (ushort)((Style>>5) & 0x001f);
                ushort Reserved  = (ushort)(Style>>10);

                bool bBitsOk = true;

                if (Posture == 3)
                {
                    v.Error(T.PCLT_Style, E.PCLT_E_Style_Posture, m_tag, "0x"+Style.ToString("x4"));
                    bBitsOk = false;
                    bRet = false;
                }
                if (Width == 5)
                {
                    v.Error(T.PCLT_Style, E.PCLT_E_Style_Width, m_tag, "0x"+Style.ToString("x4"));
                    bBitsOk = false;
                    bRet = false;
                }
                if (Structure > 17)
                {
                    v.Error(T.PCLT_Style, E.PCLT_E_Style_Structure, m_tag, "0x"+Style.ToString("x4"));
                    bBitsOk = false;
                    bRet = false;
                }
                if (Reserved != 0)
                {
                    v.Error(T.PCLT_Style, E.PCLT_E_Style_Reserved, m_tag, "0x"+Style.ToString("x4"));
                    bBitsOk = false;
                    bRet = false;
                }

                if (bBitsOk)
                {
                    v.Pass(T.PCLT_Style, P.PCLT_P_Style, m_tag);
                }

            }

            if (v.PerformTest(T.PCLT_StrokeWeight))
            {
                if (StrokeWeight >= -7 && StrokeWeight <= 7)
                {
                    v.Pass(T.PCLT_StrokeWeight, P.PCLT_P_StrokeWeight, m_tag, StrokeWeight.ToString());
                }
                else
                {
                    v.Error(T.PCLT_StrokeWeight, E.PCLT_E_StrokeWeight, m_tag, StrokeWeight.ToString());
                    bRet = false;
                }
            }

            if (v.PerformTest(T.PCLT_WidthType))
            {
                if (WidthType >= -5 && WidthType <= 5)
                {
                    v.Pass(T.PCLT_WidthType, P.PCLT_P_WidthType, m_tag, WidthType.ToString());
                }
                else
                {
                    v.Error(T.PCLT_WidthType, E.PCLT_E_WidthType, m_tag, WidthType.ToString());
                    bRet = false;
                }
            }

            if (v.PerformTest(T.PCLT_SerifStyle))
            {
                uint bot6 = (uint)SerifStyle & 0x3f;
                uint top2 = (uint)SerifStyle>>6;

                bool bBitsOk = true;

                if (bot6 > 12)
                {
                    v.Error(T.PCLT_SerifStyle, E.PCLT_E_Bottom6, m_tag, "0x"+SerifStyle.ToString("x2"));
                    bBitsOk = false;
                    bRet = false;
                }                
                if (top2 == 0 || top2 == 3)
                {
                    v.Error(T.PCLT_SerifStyle, E.PCLT_E_Top2, m_tag);
                    bBitsOk = false;
                    bRet = false;
                }

                if (bBitsOk)
                {
                    v.Pass(T.PCLT_SerifStyle, P.PCLT_P_SerifStyle, m_tag);
                }
            }

            if (v.PerformTest(T.PCLT_Reserved))
            {
                if (Reserved == 0)
                {
                    v.Pass(T.PCLT_Reserved, P.PCLT_P_Reserved, m_tag);
                }
                else
                {
                    v.Error(T.PCLT_Reserved, E.PCLT_E_Reserved, m_tag, Reserved.ToString());
                    bRet = false;
                }
            }

            return bRet;
        }


    }
}
