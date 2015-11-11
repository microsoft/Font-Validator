using System;
using System.Diagnostics;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_head.
    /// </summary>
    public class val_head : Table_head, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_head(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;


            if (v.PerformTest(T.head_TableLength))
            {
                if (m_bufTable.GetLength() == 54)
                {
                    v.Pass(T.head_TableLength, P.head_P_TableLength, m_tag);
                }
                else
                {
                    v.Error(T.head_TableLength, E.head_E_TableLength, m_tag, m_bufTable.GetLength().ToString());
                    bRet = false;
                }
            }

            if (v.PerformTest(T.head_TableVersion))
            {
                if (TableVersionNumber.GetUint() == 0x00010000)
                {
                    v.Pass(T.head_TableVersion, P.head_P_TableVersion, m_tag);
                }
                else
                {
                    v.Error(T.head_TableVersion, E.head_E_TableVersion, m_tag, "0x"+TableVersionNumber.GetUint().ToString("x8"));
                    bRet = false;
                }
            }

            if (v.PerformTest(T.head_fontRevision))
            {
                string sVersion = fontOwner.GetFontVersion();
                if (sVersion != null)
                {
                    if (sVersion.Length >= 11
                        && sVersion.StartsWith("Version ")
                        && Char.IsDigit(sVersion, 8))
                    {
                        string sVersionNum = sVersion.Substring(8);
                        bool bFoundDecPt = false;
                        int nLastDigitPos = 0;
                        for (int i=0; i<sVersionNum.Length; i++)
                        {
                            if (Char.IsDigit(sVersionNum, i))
                            {
                                nLastDigitPos = i;
                            }
                            else if (sVersionNum[i] == '.')
                            {
                                if (!bFoundDecPt)
                                {
                                    bFoundDecPt = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        double fVersion = Double.Parse(sVersionNum.Substring(0, nLastDigitPos+1));
                        double fRevision = fontRevision.GetDouble();

                        if (Math.Round(fVersion, 3) == Math.Round(fRevision, 3))
                        {
                            v.Pass(T.head_fontRevision, P.head_P_fontRevision, m_tag, fRevision.ToString("f3"));
                        }
                        else
                        {
                            string s = "revision: " + fRevision.ToString("f3") + ", version: " + sVersionNum;
                            v.Warning(T.head_fontRevision, W.head_W_fontRevision, m_tag, s);
                        }
                    }
                }
            }
            
            if (v.PerformTest(T.head_ChecksumAdjustment))
            {
                if (checkSumAdjustment == 0xb1b0afba - fontOwner.CalcChecksum())
                {
                    v.Pass(T.head_ChecksumAdjustment, P.head_P_FontChecksum, m_tag, "0x"+checkSumAdjustment.ToString("x8"));
                }
                else
                {
                    v.Error(T.head_ChecksumAdjustment, E.head_E_FontChecksum, m_tag, "0x"+checkSumAdjustment.ToString("x8"));
                    bRet = false;
                }
            }
            
            if (v.PerformTest(T.head_MagicNumber))
            {
                if (magicNumber == 0x5f0f3cf5)
                {
                    v.Pass(T.head_MagicNumber, P.head_P_MagicNumber, m_tag);
                }
                else
                {
                    v.Error(T.head_MagicNumber, E.head_E_MagicNumber, m_tag, "0x"+magicNumber.ToString("x8"));
                    bRet = false;
                }
            }
            
            if (v.PerformTest(T.head_FlagTests))
            {
                ushort val = flags;
                // bit 0 indicates baseline for font at y=0
                
                // bit 1 indicates left sidebearing point at x=0

                // bit 2 indicates instructions may depend on point size

                // bit 3 indicates forcing ppem to integer values instead of fractional values

                // bit 4 indicates non-linear scaling - if set allows presence of LTSH and/or hdmx tables
                Table_hdmx hdmxTable = (Table_hdmx)fontOwner.GetTable("hdmx");
                Table_LTSH LTSHTable = (Table_LTSH)fontOwner.GetTable("LTSH");
                if ((val & 0x0010) == 0)
                {
                    if (hdmxTable == null)
                    {
                        v.Pass(T.head_FlagTests, P.head_P_flags_bit4_0_hdmx, m_tag);
                    }
                    else
                    {
                        v.Error(T.head_FlagTests, E.head_E_flags_bit4_0_hdmx, m_tag);
                    }

                    if (LTSHTable == null)
                    {
                        v.Pass(T.head_FlagTests, P.head_P_flags_bit4_0_LTSH, m_tag);
                    }
                    else
                    {
                        v.Error(T.head_FlagTests, E.head_E_flags_bit4_0_LTSH, m_tag);
                    }
                }
                else
                {
                    if (hdmxTable != null)
                    {
                        v.Pass(T.head_FlagTests, P.head_P_flags_bit4_1_hdmx, m_tag);
                    }
                    else
                    {
                        v.Warning(T.head_FlagTests, W.head_W_flags_bit4_1_hdmx, m_tag);
                    }

                    if (LTSHTable != null)
                    {
                        v.Pass(T.head_FlagTests, P.head_P_flags_bit4_1_LTSH, m_tag);
                    }
                    else
                    {
                        v.Warning(T.head_FlagTests, W.head_W_flags_bit4_1_LTSH, m_tag);
                    }
                }

                // bits 5 - 10 are used by apple and ignored by windows

                // bit 11 indicates font data is 'lossless' as a result of having been compressed/decompressed

                // bit 12 indicates a converted font

                // bit 13 indicates optimized for cleartype

                // bit 14 reserved
                if ((val & 0x4000) == 0)
                {
                    v.Pass(T.head_FlagTests, P.head_P_flags_bit14, m_tag);
                }
                else
                {
                    v.Error(T.head_FlagTests, E.head_E_flags_bit14, m_tag, val.ToString());
                    bRet = false;
                }
                    
                // bit 15 reserved
                if ((val & 0x8000) == 0)
                {
                    v.Pass(T.head_FlagTests, P.head_P_flags_bit15, m_tag);
                }
                else
                {
                    v.Error(T.head_FlagTests, E.head_E_flags_bit15, m_tag, val.ToString());
                    bRet = false;
                }
            }
            
            if (v.PerformTest(T.head_UnitsPerEmValues))
            {
                ushort val = unitsPerEm;

                bool bInRange = false;

                if (val < 16) // opentype spec says min value is 16
                {
                    v.Error(T.head_UnitsPerEmValues, E.head_E_unitsPerEm_LT16, m_tag, val.ToString());
                    bRet = false;
                }
                else if (val < 64) // apple spec says min value is 64
                {
                    v.Warning(T.head_UnitsPerEmValues, W.head_W_unitsPerEM_LT64, m_tag, val.ToString());
                }
                else if (val > 16384)
                {
                    v.Error(T.head_UnitsPerEmValues, E.head_E_unitsPerEM_GT16384, m_tag, val.ToString());
                    bRet = false;
                }
                else
                {
                    bInRange = true;
                }

                // apple spec says unitsPerEm must be power of two
                // but don't check if it's a CFF font
                if (!fontOwner.ContainsPostScriptOutlines())
                {
                    bool bPowerOfTwo = false;
                    for (int i=0; i<16; i++)
                    {
                        ushort nPow2 = (ushort)(1<<i);
                        if (val == nPow2)
                        {
                            bPowerOfTwo = true;
                            break;
                        }
                    }
                    if (bPowerOfTwo)
                    {
                        if (bInRange)
                        {
                            v.Pass(T.head_UnitsPerEmValues, P.head_P_unitsPerEm, m_tag, val.ToString());
                        }
                    }
                    else
                    {
                        v.Warning(T.head_UnitsPerEmValues, W.head_W_unitsPerEm_Pow2, m_tag, val.ToString());
                    }
                }
            }
            
            if (v.PerformTest(T.head_Dates))
            {
                DateTime dtBeforeTrueType = new DateTime(1985, 1, 1);

                if ((created >> 32) == 0)
                {
                    DateTime dtCreated = this.GetCreatedDateTime();
                    if (created == 0)
                    {
                        v.Warning(T.head_Dates, W.head_W_created_0, m_tag);
                    }
                    else if (dtCreated < dtBeforeTrueType || dtCreated > DateTime.Now)
                    {
                        string sDetails = "created = " + created + " (" + dtCreated.ToString("f", null) + ")";
                        v.Warning(T.head_Dates, W.head_W_created_unlikely, m_tag, sDetails);
                    }
                    else
                    {
                        v.Pass(T.head_Dates, P.head_P_created_0, m_tag);
                    }
                }
                else
                {
                    string sDetails = "created = 0x" + created.ToString("x16");
                    v.Error(T.head_Dates, E.head_E_created_invalid, m_tag, sDetails);
                    bRet = false;
                }

                if ((modified >> 32) == 0)
                {
                    DateTime dtModified = this.GetModifiedDateTime();
                    if (modified == 0)
                    {
                        v.Warning(T.head_Dates, W.head_W_modified_0, m_tag);
                    }
                    else if (dtModified < dtBeforeTrueType || dtModified > DateTime.Now)
                    {
                        string sDetails = "modified = " + modified + " (" + dtModified.ToString("f", null) + ")";
                        v.Warning(T.head_Dates, W.head_W_modified_unlikely, m_tag, sDetails);
                    }
                    else
                    {
                        v.Pass(T.head_Dates, P.head_P_modified_0, m_tag);
                    }
                }
                else
                {
                    string sDetails = "modified = 0x" + modified.ToString("x16");
                    v.Error(T.head_Dates, E.head_E_modified_invalid, m_tag, sDetails);
                    bRet = false;
                }
            }
            
            if (v.PerformTest(T.head_MinMaxValues))
            {
                if (fontOwner.ContainsTrueTypeOutlines())
                {
                    if (xMin > xMax)
                    {
                        v.Error(T.head_MinMaxValues, E.head_E_xMin_GT_xMax, m_tag);
                        bRet = false;
                    }

                    if (yMin > yMax)
                    {
                        v.Error(T.head_MinMaxValues, E.head_E_yMin_GT_yMax, m_tag);
                        bRet = false;
                    }

                    short xMinExpected = 32767;
                    short xMaxExpected = -32768;
                    short yMinExpected = 32767;
                    short yMaxExpected = -32768;

                    Table_glyf glyfTable = (Table_glyf)fontOwner.GetTable("glyf");
                    if (glyfTable != null)
                    {
                        Table_maxp maxp = (Table_maxp)fontOwner.GetTable("maxp");
                        if (maxp != null)
                        {
                            for (uint i=0; i<fontOwner.GetMaxpNumGlyphs(); i++)
                            {
                                Table_glyf.header h = glyfTable.GetGlyphHeader(i, fontOwner);
                                if (h != null)
                                {
                                    if (xMinExpected > h.xMin) xMinExpected = h.xMin;
                                    if (xMaxExpected < h.xMax) xMaxExpected = h.xMax;
                                    if (yMinExpected > h.yMin) yMinExpected = h.yMin;
                                    if (yMaxExpected < h.yMax) yMaxExpected = h.yMax;
                                }
                            }

                            if (xMin == xMinExpected)
                            {
                                String s = "xMin = " + xMin;
                                v.Pass(T.head_MinMaxValues, P.head_P_xMin_glyf, m_tag, s);
                            }
                            else
                            {
                                string s = "actual: " + xMin + ", expected: " + xMinExpected;
                                v.Error(T.head_MinMaxValues, E.head_E_xMin_glyf, m_tag, s);
                                bRet = false;
                            }

                            if (yMin == yMinExpected)
                            {
                                String s = "yMin = " + yMin;
                                v.Pass(T.head_MinMaxValues, P.head_P_yMin_glyf, m_tag, s);
                            }
                            else
                            {
                                string s = "actual: " + yMin + ", expected: " + yMinExpected;
                                v.Error(T.head_MinMaxValues, E.head_E_yMin_glyf, m_tag, s);
                                bRet = false;
                            }

                            if (xMax == xMaxExpected)
                            {
                                String s = "xMax = " + xMax;
                                v.Pass(T.head_MinMaxValues, P.head_P_xMax_glyf, m_tag, s);
                            }
                            else
                            {
                                string s = "actual: " + xMax + ", expected: " + xMaxExpected;
                                v.Error(T.head_MinMaxValues, E.head_E_xMax_glyf, m_tag, s);
                                bRet = false;
                            }

                            if (yMax == yMaxExpected)
                            {
                                String s = "yMax = " + yMax;
                                v.Pass(T.head_MinMaxValues, P.head_P_yMax_glyf, m_tag, s);
                            }
                            else
                            {
                                string s = "actual: " + yMax + ", expected: " + yMaxExpected;
                                v.Error(T.head_MinMaxValues, E.head_E_yMax_glyf, m_tag, s);
                                bRet = false;
                            }
                        }
                        else
                        {
                            v.Error(T.head_MinMaxValues, E._TEST_E_TableMissing, m_tag, "maxp");
                        }
                    }
                    else
                    {
                        v.Error(T.head_MinMaxValues, E._TEST_E_TableMissing, m_tag, "glyf");
                    }
                }
                else
                {
                    v.Info(T.head_MinMaxValues, I._TEST_I_NotForCFF, m_tag, "test = head_MinMaxValues");
                }
            }
            
            if (v.PerformTest(T.head_MacStyleBits))
            {
                bool bMacBold = ((macStyle & 0x0001) != 0);
                bool bMacItal = ((macStyle & 0x0002) != 0);

                // subfamily (style) string

                Table_name nameTable = (Table_name)fontOwner.GetTable("name");
                string sStyle = null;
                string sStyleLower = null;
                if (nameTable != null)
                {
                    sStyle = nameTable.GetStyleString();
                    if (sStyle != null)
                    {
                        sStyleLower = sStyle.ToLower();
                    }
                }
                if (sStyleLower != null)
                {
                    if (bMacBold && sStyleLower.IndexOf("bold") == -1)
                    {
                        v.Error(T.head_MacStyleBits, E.head_E_macStyleBold_subfamily, m_tag, "macStyle bold bit is set, but subfamily is " + sStyle);
                        bRet = false;
                    }
                    else if (!bMacBold && sStyleLower.IndexOf("bold") != -1)
                    {
                        v.Error(T.head_MacStyleBits, E.head_E_macStyleBold_subfamily, m_tag, "macStyle bold bit is clear, but subfamily is " + sStyle);
                        bRet = false;
                    }
                    else
                    {
                        v.Pass(T.head_MacStyleBits, P.head_P_macStyleBold_subfamily, m_tag);
                    }

                    if (bMacItal && sStyleLower.IndexOf("italic") == -1 && sStyleLower.IndexOf("oblique") == -1)
                    {
                        v.Error(T.head_MacStyleBits, E.head_E_macStyleItal_subfamily, m_tag, "macStyle italic bit is set, but subfamily is " + sStyle);
                        bRet = false;
                    }
                    else if (!bMacItal && (sStyleLower.IndexOf("italic") != -1 || sStyleLower.IndexOf("oblique") != -1))
                    {
                        v.Error(T.head_MacStyleBits, E.head_E_macStyleItal_subfamily, m_tag, "macStyle italic bit is clear, but subfamily is " + sStyle);
                        bRet = false;
                    }
                    else
                    {
                        v.Pass(T.head_MacStyleBits, P.head_P_macStyleItal_subfamily, m_tag);
                    }
                }

                Table_OS2 OS2Table = (Table_OS2)fontOwner.GetTable("OS/2");
                if (OS2Table != null)
                {
                    // fsSelection

                    bool bOS2Bold = ((OS2Table.fsSelection & 0x0020) != 0);
                    bool bOS2Ital = ((OS2Table.fsSelection & 0x0001) != 0);

                    if (bMacBold == bOS2Bold)
                    {
                        v.Pass(T.head_MacStyleBits, P.head_P_macStyleBold_OS2, m_tag);
                    }
                    else if (bMacBold)
                    {
                        v.Error(T.head_MacStyleBits, E.head_E_macStyleBold1_OS2, m_tag);
                        bRet = false;
                    }
                    else
                    {
                        v.Error(T.head_MacStyleBits, E.head_E_macStyleBold0_OS2, m_tag);
                        bRet = false;
                    }

                    if (bMacItal == bOS2Ital)
                    {
                        v.Pass(T.head_MacStyleBits, P.head_P_macStyleItal_OS2, m_tag);
                    }
                    else if (bMacItal)
                    {
                        v.Error(T.head_MacStyleBits, E.head_E_macStyleItal1_OS2, m_tag);
                        bRet = false;
                    }
                    else
                    {
                        v.Error(T.head_MacStyleBits, E.head_E_macStyleItal0_OS2, m_tag);
                        bRet = false;
                    }
                }

                Table_post postTable = (Table_post)fontOwner.GetTable("post");

                if (postTable != null)
                {
                    bool bPostItal = (postTable.italicAngle.GetUint() != 0);


                    if (bMacItal == bPostItal)
                    {
                        v.Pass(T.head_MacStyleBits, P.head_P_macStyleItal_post, m_tag);
                    }
                    else if (bMacItal)
                    {
                        v.Error(T.head_MacStyleBits, E.head_E_macStyleItal1_post, m_tag);
                        bRet = false;
                    }
                    else
                    {
                        v.Error(T.head_MacStyleBits, E.head_E_macStyleItal0_post, m_tag);
                        bRet = false;
                    }
                }
            }
            
            if (v.PerformTest(T.head_LowestRecSize))
            {
                if (lowestRecPPEM == 0)
                {
                    v.Error(T.head_LowestRecSize, E.head_E_lowestRecPPEM_zero, m_tag);
                    bRet = false;
                }
                else if (lowestRecPPEM >= 1 && lowestRecPPEM <= 6)
                {
                    v.Warning(T.head_LowestRecSize, W.head_W_lowestRecPPEM_small, m_tag, "lowestRecPPEM = " + lowestRecPPEM.ToString());
                }
                else if (lowestRecPPEM >= 36)
                {
                    v.Warning(T.head_LowestRecSize, W.head_W_lowestRecPPEM_large, m_tag, "lowestRecPPEM = " + lowestRecPPEM.ToString());
                }
                else
                {
                    v.Pass(T.head_LowestRecSize, P.head_P_lowestRecPPEM, m_tag);
                }
            }
            
            if (v.PerformTest(T.head_FontDirectionHint))
            {
                if (fontDirectionHint >= -2 && fontDirectionHint <= 2)
                {
                    v.Pass(T.head_FontDirectionHint, P.head_P_fontDirectionHint, m_tag, fontDirectionHint.ToString());
                }
                else
                {
                    v.Error(T.head_FontDirectionHint, E.head_E_fontDirectionHint, m_tag, fontDirectionHint.ToString());
                    bRet = false;
                }
            }
            
            if (v.PerformTest(T.head_IndexToLocFormat))
            {
                if (fontOwner.ContainsPostScriptOutlines())
                {
                    v.Pass(T.head_IndexToLocFormat, P.head_P_indexToLocFormat_ignore, m_tag, "indexToLocFormat = " + indexToLocFormat);
                }
                else
                {
                    if (indexToLocFormat == 0 || indexToLocFormat == 1)
                    {
                        v.Pass(T.head_IndexToLocFormat, P.head_P_indexToLocFormat_range, m_tag, indexToLocFormat.ToString());
                    }
                    else
                    {
                        v.Error(T.head_IndexToLocFormat, E.head_E_indexToLocFormat_range, m_tag, indexToLocFormat.ToString());
                        bRet = false;
                    }

                    if (indexToLocFormat == 0 || indexToLocFormat == 1)
                    {
                        Table_loca locaTable = (Table_loca)fontOwner.GetTable("loca");
                        if (locaTable != null)
                        {
                            Table_maxp maxpTable = (Table_maxp)fontOwner.GetTable("maxp");
                            if (maxpTable != null)
                            {
                                uint locaTableElementSize = 0;
                                if (indexToLocFormat == 0)
                                    locaTableElementSize = 2;
                                else if (indexToLocFormat == 1)
                                    locaTableElementSize = 4;

                                uint CalcLocaLength = (uint)(maxpTable.NumGlyphs + 1) * locaTableElementSize;
                                if (CalcLocaLength == locaTable.GetLength())
                                {
                                    v.Pass(T.head_IndexToLocFormat, P.head_P_indexToLocFormat_match, m_tag, indexToLocFormat.ToString());
                                }
                                else
                                {
                                    v.Error(T.head_IndexToLocFormat, E.head_E_indexToLocFormat_match, m_tag, indexToLocFormat.ToString());
                                    bRet = false;
                                }
                            }
                            else
                            {
                                v.Error(T.head_IndexToLocFormat, E._TEST_E_TableMissing, m_tag, "maxp");
                            }
                        }
                        else
                        {
                            v.Error(T.head_IndexToLocFormat, E._TEST_E_TableMissing, m_tag, "loca");
                        }
                    }
                }
            }
            
            if (v.PerformTest(T.head_GlyphDataFormat))
            {
                if (glyphDataFormat == 0)
                {
                    v.Pass(T.head_GlyphDataFormat, P.head_P_glyphDataFormat, m_tag);
                }
                else
                {
                    v.Error(T.head_GlyphDataFormat, E.head_E_glyphDataFormat, m_tag);
                    bRet = false;
                }
            }

            return bRet;
        }



    }
}
