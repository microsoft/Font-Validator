using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_post.
    /// </summary>
    public class val_post : Table_post, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_post(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        
        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.post_TableLength))
            {
                bool bLenOk = true;

                if (Version.GetUint() == 0x00010000 ||
                    Version.GetUint() == 0x00030000)
                {
                    if (GetLength() != 32)
                    {
                        v.Error(T.post_TableLength, E.post_E_TableLenNot32, m_tag);
                        bLenOk = false;
                        bRet = false;
                    }
                }
                if (Version.GetUint() == 0x00020000)
                {
                    if (GetLength() < 34)
                    {
                        v.Error(T.post_TableLength, E.post_E_InvalidTableLen, m_tag);
                        bLenOk = false;
                        bRet = false;
                    }
                }

                if (bLenOk)
                {
                    v.Pass(T.post_TableLength, P.post_P_TableLength, m_tag);
                }
            }

            if (v.PerformTest(T.post_Version))
            {
                uint ver = Version.GetUint();
                if (ver == 0x00025000)
                {
                    v.Warning(T.post_Version, W.post_W_Version_2_5, m_tag);
                }
                else if (ver == 0x00010000 || ver == 0x00020000 || ver == 0x00030000)
                {
                    v.Pass(T.post_Version, P.post_P_Version, m_tag);
                }
                else
                {
                    v.Error(T.post_Version, E.post_E_Version, m_tag, "0x"+ver.ToString("x8"));
                }
            }

            if (v.PerformTest(T.post_italicAngle))
            {
                bool bItalOk = true;

                uint ia = italicAngle.GetUint();
                double dItalicAngle = italicAngle.GetDouble();

                if (dItalicAngle < -30.0 || dItalicAngle > 360.0 || (dItalicAngle > 0.0 && dItalicAngle < 330.0 ))
                {
                    v.Warning(T.post_italicAngle, W.post_W_italicAngle_unlikely, m_tag, "0x"+ia.ToString("x8"));
                    bItalOk = false;
                }

                Table_head headTable = (Table_head)fontOwner.GetTable("head");
                if (headTable != null)
                {
                    if ((headTable.macStyle & 0x0002) != 0)
                    {
                        if (ia == 0)
                        {
                            v.Error(T.post_italicAngle, E.post_E_italicAngleZero_macStyle, m_tag);
                            bItalOk = false;
                            bRet = false;
                        }
                    }
                    else
                    {
                        if (ia != 0)
                        {
                            v.Error(T.post_italicAngle, E.post_E_italicAngleNonzero_macStyle, m_tag);
                            bItalOk = false;
                            bRet = false;
                        }
                    }
                }
                else
                {
                    v.Error(T.post_italicAngle, E._TEST_E_TableMissing, m_tag, "head table missing, can't compare italicAngle to head.macStyle");
                    bItalOk = false;
                }

                Table_hhea hheaTable = (Table_hhea)fontOwner.GetTable("hhea");
                if (hheaTable != null)
                {
                    if (ia == 0)
                    {
                        if (hheaTable.caretSlopeRun != 0)
                        {
                            v.Error(T.post_italicAngle, E.post_E_italicAngleZero_caretSlopeRun, m_tag);
                            bItalOk = false;
                            bRet = false;
                        }
                    }
                    else
                    {
                        if (hheaTable.caretSlopeRun == 0)
                        {
                            v.Error(T.post_italicAngle, E.post_E_italicAngleNonzero_caretSlopeRun, m_tag);
                            bItalOk = false;
                            bRet = false;
                        }
                        else
                        {
                            double dActualAngle = 90.0 + italicAngle.GetDouble();
                            double dhheaAngle = (Math.Atan2(hheaTable.caretSlopeRise, hheaTable.caretSlopeRun)) * (180.0 / Math.PI);
                            if (Math.Abs(dActualAngle-dhheaAngle) >= 1.0)
                            {
                                string sDetails = "italicAngle = 0x" + ia.ToString("x8") + 
                                    " (" + italicAngle.GetDouble() + " degrees)" +
                                    ",caretSlope Rise:Run = " + hheaTable.caretSlopeRise + ":" + hheaTable.caretSlopeRun +
                                    " (" + (dhheaAngle-90.0) + " degrees)";
                                v.Error(T.post_italicAngle, E.post_E_italicAngleNonzero_hheaAngle, m_tag, sDetails);
                                bItalOk = false;
                                bRet = false;
                            }
                        }
                    }
                }
                else
                {
                    v.Error(T.post_italicAngle, E._TEST_E_TableMissing, m_tag, "hhea table missing, can't compare italicAngle to hhea.caretSlopeRun");
                    bItalOk = false;
					bRet = false;
                }

                if (bItalOk)
                {
                    v.Pass(T.post_italicAngle, P.post_P_italicAngle, m_tag);
                }
            }

            if (v.PerformTest(T.post_underlinePosition))
            {
                Table_hhea hheaTable = (Table_hhea)fontOwner.GetTable("hhea");
                if (hheaTable != null)
                {
                    if (underlinePosition >= hheaTable.Descender)
                    {
                        v.Pass(T.post_underlinePosition, P.post_P_underlinePosition, m_tag);
                    }
                    else
                    {
                        v.Warning(T.post_underlinePosition, W.post_W_underlinePos_LT_descender, m_tag);
                        //bRet = false;
                    }
                }
                else
                {
                    v.Error(T.post_underlinePosition, E._TEST_E_TableMissing, m_tag, "hhea table missing, can't compare underlinePosition to hhea.Descender");
                }
            }

            if (v.PerformTest(T.post_underlineThickness))
            {
                Table_head headTable = (Table_head)fontOwner.GetTable("head");
                if (headTable != null)
                {
                    if (underlineThickness >= 0 && underlineThickness < headTable.unitsPerEm / 2 )
                    {
                        v.Pass(T.post_underlineThickness, P.post_P_underlineThickness, m_tag);
                    }
                    else
                    {
                        v.Warning(T.post_underlineThickness, W.post_W_underlineThickness, m_tag, underlineThickness.ToString());
                    }
                }
                else
                {
                    v.Error(T.post_underlineThickness, E._TEST_E_TableMissing, m_tag, "head table missing, can't compare underlineThickness to head.unitsPerEm");
					bRet = false;
                }
            }

            if (v.PerformTest(T.post_isFixedPitch))
            {
                bool bFixedPitchOk = true;

                Table_hmtx hmtxTable = (Table_hmtx)fontOwner.GetTable("hmtx");
                if (hmtxTable != null)
                {
                    uint numberOfHMetrics = 0;

                    Table_hhea hheaTable = (Table_hhea)fontOwner.GetTable("hhea");
                    if (hheaTable != null)
                    {
                        numberOfHMetrics = hheaTable.numberOfHMetrics;
                    }
                    else
                    {
                        v.Error(T.post_isFixedPitch, E._TEST_E_TableMissing, m_tag, "hhea table missing, can't compare isFixedPitch to horizontal metrics");
						bRet = false;
                    }

                    bool bHmtxMono = hmtxTable.IsMonospace(fontOwner);

                    if (isFixedPitch == 0 && bHmtxMono)
                    {
                        v.Error(T.post_isFixedPitch, E.post_E_isFixedPitchZero_hmtx, m_tag);
                        bFixedPitchOk = false;
                        bRet = false;
                    }
                    else if (isFixedPitch != 0 && !bHmtxMono)
                    {
                        v.Error(T.post_isFixedPitch, E.post_E_isFixedPitchNonzero_hmtx, m_tag);
                        bFixedPitchOk = false;
                        bRet = false;
                    }
                }
                else
                {
                    v.Error(T.post_isFixedPitch, E._TEST_E_TableMissing, m_tag, "hmtx table missing, can't compare isFixedPitch to horizontal metrics");
					bRet = false;
                }

                Table_OS2 OS2Table = (Table_OS2)fontOwner.GetTable("OS/2");
                if (OS2Table != null)
                {
                    if (OS2Table.panose_byte1 == 2) // PANOSE family kind == LatinText
                    {
                        if (isFixedPitch == 0 && OS2Table.panose_byte4 == 9) // PANOSE proportion == monospaced
                        {
                            v.Error(T.post_isFixedPitch, E.post_E_isFixedPitchZero_OS_2, m_tag);
                            bFixedPitchOk = false;
                            bRet = false;
                        }
                        else if (isFixedPitch != 0 && OS2Table.panose_byte4 != 9)
                        {
                            v.Error(T.post_isFixedPitch, E.post_E_isFixedPitchNonzero_OS_2, m_tag);
                            bFixedPitchOk = false;
                            bRet = false;
                        }
                    }
                }
                else
                {
                    v.Error(T.post_isFixedPitch, E._TEST_E_TableMissing,
                            m_tag, "OS/2 table is missing, can't compare isFixedPitch to OS/2 PANOSE");
					bRet = false;
                }

                if (bFixedPitchOk && hmtxTable != null && OS2Table != null)
                {
                    v.Pass(T.post_isFixedPitch, P.post_P_isFixedPitch, m_tag, "matches the hmtx and OS/2 tables");
                }
                else if (bFixedPitchOk && hmtxTable == null && OS2Table != null)
                {
                    v.Pass(T.post_isFixedPitch, P.post_P_isFixedPitch, m_tag, "matches the OS/2 table");
                }
                else if (bFixedPitchOk && hmtxTable != null && OS2Table == null)
                {
                    v.Pass(T.post_isFixedPitch, P.post_P_isFixedPitch, m_tag, "matches the hmtx table");
                }
            }

            if (v.PerformTest(T.post_v2_numberOfGlyphs))
            {
                if (Version.GetUint() == 0x00020000)
                {
                    Table_maxp maxpTable = (Table_maxp)fontOwner.GetTable("maxp");
                    if (maxpTable == null)
                    {
                        v.Error(T.post_v2_numberOfGlyphs, E._TEST_E_TableMissing, m_tag, "maxp table is missing");
						bRet = false;
                    }
                    else
                    {
                        if (numberOfGlyphs == fontOwner.GetMaxpNumGlyphs())
                        {
                            v.Pass(T.post_v2_numberOfGlyphs, P.post_P_v2_numberOfGlyphs, m_tag);
                        }
                        else
                        {
                            v.Error(T.post_v2_numberOfGlyphs, E.post_E_v2_numberOfGlyphs,
                                    m_tag, "numberOfGlyphs = " + numberOfGlyphs + 
                                    ", maxp.numGlyphs = " + fontOwner.GetMaxpNumGlyphs());
                            bRet = false;
                        }
                    }
                }
                else
                {
                    v.Info(T.post_v2_numberOfGlyphs, I.post_I_v2_numberOfGlyphs_notv2, m_tag);
                }
            }

            if (v.PerformTest(T.post_v2_glyphNameIndex))
            {
                if (Version.GetUint() == 0x00020000 && GetLength() >= 34)
                {
                    bool bIndexOk = true;

                    for (uint i=0; i<numberOfGlyphs; i++)
                    {
                        if ((uint)FieldOffsetsVer2.glyphNameIndex + i*2 + 2 <= m_bufTable.GetLength())
                        {
                            ushort index = GetGlyphNameIndex((ushort)i);
                            if (index-258 >= m_nameOffsets.Length)
                            {
                                string s = "glyphNameIndex[" + i + "] = " + 
                                    index + ", # names = " + m_nameOffsets.Length;
                                v.Error(T.post_v2_glyphNameIndex, E.post_E_glyphNameIndex_range, m_tag, s);
                                bIndexOk = false;
                                bRet = false;
                            }
                        }
                        else
                        {
                            v.Warning(T.post_v2_glyphNameIndex, 
                                      W._TEST_W_OtherErrorsInTable, m_tag, 
                                      "unable to validate any more glyph indexes, index  " + 
                                      i + " is past end of table");
                            bIndexOk = false;
                            break;
                        }
                    }

                    if (bIndexOk)
                    {
                        v.Pass(T.post_v2_glyphNameIndex, P.post_P_glyphNameIndex, m_tag);
                    }
                }
                else
                {
                    v.Info(T.post_v2_glyphNameIndex, I.post_I_v2_glyphNameIndex_notv2, m_tag);
                }
            }

            if (v.PerformTest(T.post_v2_names))
            {
                if (Version.GetUint() == 0x00020000)
                {

                    PostNames pn = new PostNames();

                    bool bNamesOk = true;

                    byte [] buf = new byte[2];

                    for (uint iGlyph=0; iGlyph<numberOfGlyphs; iGlyph++)
                    {
                        if ((uint)FieldOffsetsVer2.glyphNameIndex + iGlyph*2 + 2
                            <= m_bufTable.GetLength())
                        {
                            uint index = GetGlyphNameIndex((ushort)iGlyph);
                            if (index >= 258 && index-258 < m_nameOffsets.Length)
                            {
                                if (m_nameOffsets[index-258] >= GetLength())
                                {
                                    string s = "name index = " + index;
                                    v.Error(T.post_v2_names, 
                                            E.post_E_v2_NameOffsetInvalid, 
                                            m_tag, s);
                                    bNamesOk = false;
                                    bRet = false;
                                }
                                else
                                {
                                    uint length = 
                                        m_bufTable.GetByte(m_nameOffsets[index-258]);
                                    if (m_nameOffsets[index-258] + length < GetLength())
                                    {
                                        bNamesOk = CheckNames( v, fontOwner, pn, iGlyph, index );
                                    }
                                    else
                                    {
                                        string s = "name index = " + index;
                                        v.Error(T.post_v2_names, E.post_E_v2_NameLengthInvalid, m_tag, s);
                                        bNamesOk = false;
                                        bRet = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            v.Warning(T.post_v2_names, W._TEST_W_OtherErrorsInTable, m_tag, 
                                      "unable to validate any more names, index " + 
                                      iGlyph + " is past end of table");
                            bNamesOk = false;
                            break;
                        }
                    }

                    if (bNamesOk)
                    {
                        v.Pass(T.post_v2_names, P.post_P_names, m_tag);
                    }
                }
                else
                {
                    v.Info(T.post_v2_names, I.post_I_v2_names_notv2, m_tag);
                }
            }

            return bRet;
        }

        private bool CheckNames( Validator v,
                                 OTFontVal fontOwner,
                                 PostNames pn,
                                 uint iGlyph,
                                 uint index )
        {
            bool bNamesOk = true;
            string sName = GetNameString(index-258);
            System.Globalization.NumberStyles hexStyle = 
                System.Globalization.NumberStyles.HexNumber;
            if (Is_uniXXXX(sName))
            {
                char c = (char)Int16.Parse(sName.Substring(3, 4), hexStyle );
                if (fontOwner.FastMapUnicodeToGlyphID(c) != iGlyph)
                {
                    char cMapped = fontOwner.MapGlyphIDToUnicode(iGlyph,(char)0);
                    string s = "glyph = " + iGlyph +
                        ", char = U+" + 
                        ((uint)cMapped).ToString("X4")
                        + ", name = " + sName;
                    v.Info(T.post_v2_names, 
                           I.post_I_names_uni_unexpected, 
                           m_tag, s);
                    bNamesOk = false;
                }
            }
            else if (Is_uXXXXX(sName))
            {
                uint c = (uint)Int32.Parse(sName.Substring(1), hexStyle );
                if (c < 0xffff)
                {
                    if (fontOwner.FastMapUnicodeToGlyphID((char)c) != iGlyph)
                    {
                        char cMapped = fontOwner.MapGlyphIDToUnicode(iGlyph, 
                                                                     (char)0);
                        string s = "glyph = " + iGlyph + ", char = U+" +
                            ((uint)cMapped).ToString("X4") + ", name = " + sName;
                        v.Info(T.post_v2_names, I.post_I_names_uni_unexpected, 
                               m_tag, s);
                        bNamesOk = false;
                    }
                }
                else
                {
                    if (fontOwner.FastMapUnicode32ToGlyphID(c) != iGlyph)
                    {
                        uint cMapped = 
                            fontOwner.MapGlyphIDToUnicode32(iGlyph, 0);
                        string s = "glyph = " + iGlyph + ", char = U+" + 
                            ((uint)cMapped).ToString("X5") + ", name = " + sName;
                        v.Info(T.post_v2_names, I.post_I_names_uni_unexpected, 
                               m_tag, s);
                        bNamesOk = false;
                    }
                }
            }
            else
            {
                // check the Adobe Glyph Names
                
                char c = (char)fontOwner.MapGlyphIDToUnicode(iGlyph, (char)0);
                if (c != 0xffff)
                {
                    string sAdobeName = pn.GetAdobeGlyphName(c);
                    if (sAdobeName == null)
                    {
                        string s = "glyph = " + iGlyph + ", char = U+" + 
                            ((uint)c).ToString("X4") + ", name = " + sName;
                        v.Info(T.post_v2_names, I.post_I_names_noAdobe, m_tag,s);
                        bNamesOk = false;
                    }
                    else if (sName != sAdobeName)
                    {
                        string s = "glyph = " + iGlyph + ", char = U+" + 
                            ((uint)c).ToString("X4") + ", name = " + sName;
                        v.Info(T.post_v2_names, I.post_I_names_nomatch, m_tag,s);
                        bNamesOk = false;
                    }
                }
            }
            return bNamesOk;
        }


        private bool Is_uniXXXX(string s)
        {
            bool bRet = false;

            if (s.Length == 7)
            {
                if (s.Substring(0,3) == "uni")
                {
                    bRet = true;
                    for (int i=3; i<7; i++)
                    {
                        if (!IsCapitalHexDigit(s[i]))
                        {
                            bRet = false;
                            break;
                        }
                    }
                }
            }

            return bRet;
        }

        private bool Is_uXXXXX(string s)
        {
            bool bRet = false;

            // Adobe allows 4 to 6 digits in this format
            if (s.Length >=5 && s.Length <=7 && s[0] == 'u')
            {
                bRet = true;

                for (int i=1; i<s.Length; i++)
                {
                    if (!IsCapitalHexDigit(s[i]))
                    {
                        bRet = false;
                        break;
                    }
                }
            }

            return bRet;
        }

        private bool IsCapitalHexDigit(char c)
        {
            if ((c >='0' && c <= '9') || (c >='A' && c <= 'F'))
                return true;
            else
                return false;
        }
    }

    
}
