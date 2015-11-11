using System;
using System.Diagnostics;

using OTFontFile;
using Win32APIs;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_OS2
    /// </summary>
    public class val_OS2 : Table_OS2, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_OS2(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * public methods
         */
        
        
        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.OS_2_Version))
            {
                if (version == 0 || version == 1 || version == 2)
                {
                    v.Warning(T.OS_2_Version, W.OS_2_W_Version_old, m_tag, version.ToString());
                }
                else if (version == 3)
                {
                    v.Pass(T.OS_2_Version, P.OS_2_P_Version, m_tag, version.ToString());
                }
                else
                {
                    v.Error(T.OS_2_Version, E.OS_2_E_Version, m_tag, version.ToString());
                    bRet = false;
                }
            }

            if (v.PerformTest(T.OS_2_TableLength))
            {
                uint len = GetLength();
                if ((version == 0 && len == 78)
                    || (version == 1 && len == 86)
                    || (version == 2 && len == 96)
                    || (version == 3 && len == 96))
                {
                    v.Pass(T.OS_2_TableLength, P.OS_2_P_TableLength, m_tag);
                }
                else
                {
                    v.Error(T.OS_2_TableLength, E.OS_2_E_TableLength, m_tag);
                    bRet = false;
                }
            }

            if (!bRet)
            {
                v.Warning(T.T_NULL, W._TEST_W_OtherErrorsInTable, m_tag, "OS/2 table appears to be corrupt.  No further tests will be performed.");
                return bRet;
            }

            if (v.PerformTest(T.OS_2_xAvgCharWidth))
            {
                if (fontOwner.GetTable("maxp") == null)
                {
                    v.Warning(T.OS_2_xAvgCharWidth, W._TEST_W_ErrorInAnotherTable, m_tag, "maxp table inaccessible, can't check Avg Char Width");
                }
                else
                {
                    if (fontOwner.ContainsTrueTypeOutlines())
                    {
                        Table_hmtx hmtxTable = (Table_hmtx)fontOwner.GetTable("hmtx");
                        val_loca locaTable = (val_loca)fontOwner.GetTable("loca");
                        val_maxp maxpTable = (val_maxp)fontOwner.GetTable("maxp");
                        if (hmtxTable == null)
                        {
                            v.Error(T.OS_2_xAvgCharWidth, E._TEST_E_TableMissing, m_tag, "hmtx");
                            bRet = false;
                        }
                        else if (locaTable == null)
                        {
                            v.Error(T.OS_2_xAvgCharWidth, E._TEST_E_TableMissing, m_tag, "loca");
                            bRet = false;
                        }
                        else if (maxpTable == null)
                        {
                            v.Error(T.OS_2_xAvgCharWidth, E._TEST_E_TableMissing, m_tag, "maxp");
                            bRet = false;
                        }
                        else
                        {

                            if (   version == 3
                                || fontOwner.ContainsMsSymbolEncodedCmap())
                            {
                                int nTotalWidth = 0;
                                int nTotalGlyphs = 0;
                                for (uint iGlyph=0; iGlyph<fontOwner.GetMaxpNumGlyphs(); iGlyph++)
                                {
                                    Table_hmtx.longHorMetric hm = hmtxTable.GetOrMakeHMetric(iGlyph, fontOwner);
                                    if (hm != null)
                                    {
                                        nTotalWidth += hm.advanceWidth;
                                        // only average non-zero width glyphs
                                        if (hm.advanceWidth > 0)
                                        {
                                            nTotalGlyphs ++;
                                        }
                                    }
                                }

                                short CalcAvgWidth = 0;
                                if (nTotalGlyphs > 0)
                                    CalcAvgWidth = (short)(nTotalWidth / nTotalGlyphs);

                                if (xAvgCharWidth == CalcAvgWidth)
                                {
                                    v.Pass(T.OS_2_xAvgCharWidth, P.OS_2_P_xAvgCharWidth, m_tag);
                                }
                                else
                                {
                                    string s = "actual = " + xAvgCharWidth + ", calc = " + CalcAvgWidth;
                                    v.Error(T.OS_2_xAvgCharWidth, E.OS_2_E_xAvgCharWidth, m_tag, s);
                                    bRet = false;
                                }
                            }
                            else
                            {
                                // do weighed average
                                ushort [] weightLC = { 
                                                       64, 14, 27, 35, 100, 20, 14, 42, 63,  3,  6, 35, 20,
                                                       56, 56, 17,  4,  49, 56, 71, 31, 10,  18, 3, 18, 2
                                                     };
                                ushort weightSpace = 166;

                                try
                                {

                                    uint iSpaceGlyph = fontOwner.FastMapUnicodeToGlyphID(' ');
                                    if (iSpaceGlyph >= fontOwner.GetMaxpNumGlyphs())
                                    {
                                        throw new ApplicationException("error in cmap table");
                                    }
                                    Table_hmtx.longHorMetric hmSpace = hmtxTable.GetOrMakeHMetric(iSpaceGlyph, fontOwner);

                                    if (hmSpace != null)
                                    {

                                        int nTotalWeight = hmSpace.advanceWidth * weightSpace;
                                        int nLowerCase = 0;
                                        for (char c = 'a'; c <= 'z'; c++)
                                        {
                                            uint iGlyph = fontOwner.FastMapUnicodeToGlyphID(c);
                                            if (iGlyph > 0
                                                && iGlyph < fontOwner.GetMaxpNumGlyphs())
                                            {
                                                nLowerCase ++;

                                                Table_hmtx.longHorMetric hm = hmtxTable.GetOrMakeHMetric(iGlyph, fontOwner);
                                                nTotalWeight += hm.advanceWidth * weightLC[c-'a'];
                                            }
                                        }

                                        short CalcAvgWidth = 0;
                                        
                                        if (nLowerCase == 26)
                                        {
                                            CalcAvgWidth = (short)(nTotalWeight / 1000);
                                        }
                                        else
                                        {
                                            int nTotalWidth = 0;
                                            int nTotalGlyphs = 0;
                                            for (uint iGlyph=0; iGlyph<fontOwner.GetMaxpNumGlyphs(); iGlyph++)
                                            {
                                                Table_hmtx.longHorMetric hm = hmtxTable.GetOrMakeHMetric(iGlyph, fontOwner);
                                                nTotalWidth += hm.advanceWidth;
                                                nTotalGlyphs ++;
                                            }

                                            CalcAvgWidth = (short)(nTotalWidth / nTotalGlyphs);
                                        }

                                        if (xAvgCharWidth == CalcAvgWidth)
                                        {
                                            v.Pass(T.OS_2_xAvgCharWidth, P.OS_2_P_xAvgCharWidth, m_tag);
                                        }
                                        else
                                        {
                                            string s = "actual = " + xAvgCharWidth + ", calc = " + CalcAvgWidth;
                                            v.Error(T.OS_2_xAvgCharWidth, E.OS_2_E_xAvgCharWidth, m_tag, s);
                                            bRet = false;
                                        }
                                    }
                                    else
                                    {
                                        v.Warning(T.OS_2_xAvgCharWidth, W.OS_2_W_hmtx_invalid, m_tag, "unable to parse hmtx table");
                                    }
                                }
                                catch (ApplicationException e)
                                {
                                    v.Warning(T.OS_2_xAvgCharWidth, W._TEST_W_ErrorInAnotherTable, m_tag, "xAvgCharWidth cannot be validated due to " + e.Message);
                                }
                            }
                        }

                    }
                    else
                    {
                        // not a TT outline font - should embedded bitmap only font be handled here?
                        v.Info(T.OS_2_xAvgCharWidth, I._TEST_I_NotForCFF, m_tag, "test = OS/2_xAvgCharWidth");
                    }

                }
            }

            if (v.PerformTest(T.OS_2_WeightClass))
            {
                if (usWeightClass >= 100 &&
                    usWeightClass <= 900 &&
                    usWeightClass% 100 == 0)
                {
                    bool bWeightClassOk = true;

                    // compare to the PANOSE weight value
                    if (panose_byte1 == 2 || panose_byte1 == 3 || panose_byte1 == 4) // latin text, hand writing, or decorative
                    {
                        if (panose_byte3 > 1)
                        {
                            // convert PANOSE weight value from a range of [2..11] to a range of [100..900] (reasonably close anyway, given integer math)
                            int nConvertedPANOSE = (panose_byte3-2)*89+100;
                            int nDifference = Math.Abs(usWeightClass - nConvertedPANOSE);
                            if (nDifference < 200)
                            {
                                v.Pass(T.OS_2_WeightClass, P.OS_2_P_WeightClass_PANOSE, m_tag);
                            }
                            else
                            {
                                v.Warning(T.OS_2_WeightClass, W.OS_2_W_WeightClass_PANOSE, m_tag, "usWeightClass = " + usWeightClass + ", PANOSE weight = " + panose_byte3);
                                bWeightClassOk = false;
                            }
                        }
                    }

                    if (bWeightClassOk)
                    {
                        v.Pass(T.OS_2_WeightClass, P.OS_2_P_WeightClass, m_tag, usWeightClass.ToString());
                    }
                }
                else
                {
                    v.Error(T.OS_2_WeightClass, E.OS_2_E_WeightClass, m_tag, usWeightClass.ToString());
                    bRet = false;
                }
            }

            if (v.PerformTest(T.OS_2_WidthClass))
            {
                if (usWidthClass >=1 && usWidthClass <= 9)
                {
                    v.Pass(T.OS_2_WidthClass, P.OS_2_P_WidthClass, m_tag, usWidthClass.ToString());
                }
                else
                {
                    v.Error(T.OS_2_WidthClass, E.OS_2_E_WidthClass, m_tag, usWidthClass.ToString());
                    bRet = false;
                }
            }

            if (v.PerformTest(T.OS_2_fsType))
            {
                bool bPass = true;

                if ((fsType & 0xfcf1) != 0)
                {
                    v.Error(T.OS_2_fsType, E.OS_2_E_fsTypeReserved, m_tag, "0x"+fsType.ToString("x4"));
                    bRet = false;
                    bPass = false;
                }
                else
                {
                    int nExclusiveBits = 0;
                    if ((fsType & 0x0002) !=0) //Restricted License Embedding
                    {
                        nExclusiveBits++;
                    }
                    if ((fsType & 0x0004) != 0) // Preview and Print embedding
                    {
                        nExclusiveBits++;
                    }
                    if ((fsType & 0x0008) != 0) // Editable embedding
                    {
                        nExclusiveBits++;
                    }

                    if (nExclusiveBits > 1)
                    {
                        string sDetails = "0x"+fsType.ToString("x4");
                        if ((fsType & 0x0002) !=0)
                        {
                            sDetails += ", Restricted License Embedding";
                        }
                        if ((fsType & 0x0004) != 0)
                        {
                            sDetails += ", Preview and Print embedding";
                        }
                        if ((fsType & 0x0008) != 0)
                        {
                            sDetails += ", Editable embedding";
                        }

                        v.Error(T.OS_2_fsType, E.OS_2_E_fsTypeExclusiveBits, m_tag, sDetails);
                        bRet = false;
                        bPass = false;
                    }
                }

                if (bPass)
                {
                    string sDetails = "0x"+fsType.ToString("x4");
                    if (fsType == 0)
                    {
                        sDetails += ", Installable Embedding";
                    }
                    if ((fsType & 0x0002) !=0)
                    {
                        sDetails += ", Restricted License Embedding";
                    }
                    if ((fsType & 0x0004) != 0)
                    {
                        sDetails += ", Preview and Print embedding";
                    }
                    if ((fsType & 0x0008) != 0)
                    {
                        sDetails += ", Editable embedding";
                    }
                    if ((fsType & 0x0100) != 0)
                    {
                        sDetails += ", No subsetting";
                    }
                    if ((fsType & 0x0200) != 0)
                    {
                        sDetails += ", Bitmap embedding only";
                    }

                    v.Pass(T.OS_2_fsType, P.OS_2_P_fsType, m_tag, sDetails);
                }
            }

            if (v.PerformTest(T.OS_2_SubscriptSuperscript))
            {
                Table_head headTable = (Table_head)fontOwner.GetTable("head");
                if (headTable != null)
                {
                    ushort unitsPerEm = headTable.unitsPerEm;
                    ushort SmallestScript = (ushort)(unitsPerEm / 10);
                    bool bNoWarnOrErr = true;


                    if (ySubscriptXSize < SmallestScript ||
                        ySubscriptXSize > unitsPerEm )
                    {
                        v.Warning(T.OS_2_SubscriptSuperscript, W.OS_2_W_ySubscriptXSize_unlikely, m_tag, ySubscriptXSize.ToString());
                        bNoWarnOrErr = false;
                    }
                    if (ySubscriptYSize < SmallestScript ||
                        ySubscriptYSize > unitsPerEm )
                    {
                        v.Warning(T.OS_2_SubscriptSuperscript, W.OS_2_W_ySubscriptYSize_unlikely, m_tag, ySubscriptYSize.ToString());
                        bNoWarnOrErr = false;
                    }
                    if (ySubscriptYOffset < 0 )
                    {
                        v.Warning(T.OS_2_SubscriptSuperscript, W.OS_2_W_ySubscriptYOffset_LTZero, m_tag, ySubscriptYOffset.ToString());
                        bNoWarnOrErr = false;
                    }
                    if (ySuperscriptXSize < SmallestScript ||
                        ySuperscriptXSize > unitsPerEm )
                    {
                        v.Warning(T.OS_2_SubscriptSuperscript, W.OS_2_W_ySuperscriptXSize_unlikely, m_tag, ySuperscriptXSize.ToString());
                        bNoWarnOrErr = false;
                    }
                    if (ySuperscriptYSize < SmallestScript ||
                        ySuperscriptYSize > unitsPerEm )
                    {
                        v.Warning(T.OS_2_SubscriptSuperscript, W.OS_2_W_ySuperscriptYSize_unlikely, m_tag, ySuperscriptYSize.ToString());
                        bNoWarnOrErr = false;
                    }
                    if (ySuperscriptYOffset < 0 )
                    {
                        v.Warning(T.OS_2_SubscriptSuperscript, W.OS_2_W_ySuperscriptYOffset_unlikely, m_tag, ySuperscriptYOffset.ToString());
                        bNoWarnOrErr = false;
                    }

                    if (bNoWarnOrErr)
                    {
                        v.Pass(T.OS_2_SubscriptSuperscript, P.OS_2_P_SuperscriptSubscript, m_tag);
                    }
                }
                else
                {
                    v.Error(T.OS_2_SubscriptSuperscript, E._TEST_E_TableMissing, m_tag, "head");
                    bRet = false;
                }
            }

            if (v.PerformTest(T.OS_2_Strikeout))
            {
                Table_head headTable = (Table_head)fontOwner.GetTable("head");
                if (headTable != null)
                {
                    ushort unitsPerEm = headTable.unitsPerEm;
                    bool bNoWarnings = true;

                    if (yStrikeoutSize < 0 || yStrikeoutSize > unitsPerEm / 2 )
                    {
                        v.Warning(T.OS_2_Strikeout, W.OS_2_W_yStrikeoutSize_unlikely, m_tag, yStrikeoutSize.ToString());
                        bNoWarnings = false;
                    }
                    if (yStrikeoutPosition <= 0 )
                    {
                        v.Warning(T.OS_2_Strikeout, W.OS_2_W_yStrikeoutPosition_unlikely, m_tag, yStrikeoutPosition.ToString());
                        bNoWarnings = false;
                    }

                    if (bNoWarnings)
                    {
                        v.Pass(T.OS_2_Strikeout, P.OS_2_P_Strikeout, m_tag);
                    }
                }
                else
                {
                    v.Error(T.OS_2_Strikeout, E._TEST_E_TableMissing, m_tag, "head");
                    bRet = false;
                }
            }

            if (v.PerformTest(T.OS_2_FamilyClass))
            {
                bool bIDsOk = true;
                byte ClassID = (byte)(sFamilyClass >> 8);
                byte SubclassID = (byte)(sFamilyClass);

                if (ClassID == 6 || ClassID == 11 || ClassID == 13 || ClassID == 14 )
                {
                    v.Error(T.OS_2_FamilyClass, E.OS_2_E_sFamilyClass_classID_reserved, m_tag, ClassID.ToString());
                    bIDsOk = false;
                    bRet = false;
                }
                if (ClassID > 14 )
                {
                    v.Error(T.OS_2_FamilyClass, E.OS_2_E_sFamilyClass_ClassID_undefined, m_tag, ClassID.ToString());
                    bIDsOk = false;
                    bRet = false;
                }
                if (SubclassID > 15 )
                {
                    v.Error(T.OS_2_FamilyClass, E.OS_2_E_sFamilyClass_subclassID_undefined, m_tag, ClassID.ToString());
                    bIDsOk = false;
                    bRet = false;
                }
                if (bIDsOk)
                {
                    v.Pass(T.OS_2_FamilyClass, P.OS_2_P_sFamilyClass, m_tag);
                }
            }

            if (v.PerformTest(T.OS_2_Panose))
            {
                bool bPanoseOk = true;

                if (panose_byte1 < 5) // panose kind valid, but not latin symbol
                {
                    if (fontOwner.ContainsSymbolsOnly())
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_FamilyTypeNotSymbol, m_tag, "PANOSE byte 1 = " + panose_byte1.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }
                }
                else if (panose_byte1 == 5) // panose family kind == latin symbol
                {
                    if (!fontOwner.ContainsSymbolsOnly())
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_FamilyTypeSymbol, m_tag, "PANOSE byte 1 = " + panose_byte1.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }
                }
                else if ( panose_byte1 > 5 ) // family kind is invalid
                {
                    v.Error(T.OS_2_Panose, E.OS_2_E_Panose_bFamilyType, m_tag, panose_byte1.ToString());
                    bPanoseOk = false;
                    bRet = false;
                }




                if (panose_byte1 == 2) // family kind == latin text
                {
                    if ( panose_byte2 > 15 )
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_bSerifStyle, m_tag, panose_byte2.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }
                }

                if ( panose_byte3 > 11 ) // byte3 always means weight
                {
                    v.Error(T.OS_2_Panose, E.OS_2_E_Panose_bWeight, m_tag, panose_byte3.ToString());
                    bPanoseOk = false;
                    bRet = false;
                }

                if (panose_byte1 == 5) // panose family kind == latin symbol
                {
                    if (panose_byte3 != 1) // weight must be 1 for symbols
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_SymbolWeight, m_tag, panose_byte3.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }

                    if (panose_byte5 != 1) // aspect ratio & contrast must be 1 for symbols
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_SymbolAspectRatio, m_tag, panose_byte5.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }
                }


                if (panose_byte1 == 2) // family kind == latin text
                {
                    // the following tests are only valid when family kind is latin text

                    if ( panose_byte4 > 9 )
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_bProportion, m_tag, panose_byte4.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }

                    if ( panose_byte5 > 9 )
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_bContrast, m_tag, panose_byte5.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }

                    if ( panose_byte6 > 10 )
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_bStrokeVariation, m_tag, panose_byte6.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }

                    if ( panose_byte7 > 11 )
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_bArmStyle, m_tag, panose_byte7.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }

                    if ( panose_byte8 > 15 )
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_bLetterform, m_tag, panose_byte8.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }

                    if ( panose_byte9 > 13 )
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_bMidline, m_tag, panose_byte9.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }

                    if ( panose_byte10 > 7 )
                    {
                        v.Error(T.OS_2_Panose, E.OS_2_E_Panose_bXHeight, m_tag, panose_byte10.ToString());
                        bPanoseOk = false;
                        bRet = false;
                    }
                }

                if (panose_byte1  == 0 && 
                    panose_byte2  == 0 &&
                    panose_byte3  == 0 &&
                    panose_byte4  == 0 &&
                    panose_byte5  == 0 &&
                    panose_byte6  == 0 &&
                    panose_byte7  == 0 &&
                    panose_byte8  == 0 &&
                    panose_byte9  == 0 &&
                    panose_byte10 == 0 )
                {
                    v.Warning(T.OS_2_Panose, W.OS_2_W_Panose_undefined, m_tag);
                    bPanoseOk = false;
                }

                if (bPanoseOk)
                {
                    v.Pass(T.OS_2_Panose, P.OS_2_P_Panose, m_tag);
                }
            }

            if (v.PerformTest(T.OS_2_UnicodeRanges))
            {
                bRet &= CheckUnicodeRanges(v, fontOwner);
            }

            if (v.PerformTest(T.OS_2_fsSelection))
            {
                bool bSelOk = true;

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

                // reserved bits
                if ( (fsSelection & 0xFF80 ) != 0 )
                {
                    // we need to look for Win 3.1 font pages
                    // Fonts with these
                    const ushort HEBREW_FONT_PAGE = 0xB100;
                    const ushort SIMP_ARABIC_FONT_PAGE = 0xB200;
                    const ushort TRAD_ARABIC_FONT_PAGE = 0xB300;
                    const ushort OEM_ARABIC_FONT_PAGE = 0xB400;
                    const ushort SIMP_FARSI_FONT_PAGE = 0xBA00;
                    const ushort TRAD_FARSI_FONT_PAGE = 0xBB00;
                    const ushort THAI_FONT_PAGE = 0xDE00;

                    String sDetails = "Bit(s) ";
                    bool bFoundFirstBadBit = false;

                    if (version == 0)
                    {
                        switch (fsSelection & 0xFF00)
                        {
                            case HEBREW_FONT_PAGE:
                                sDetails = "Hebrew Windows 3.1 font page";
                                break;
                            case SIMP_ARABIC_FONT_PAGE:
                                sDetails = "Simplified Arabic Windows 3.1 font page";
                                break;
                            case TRAD_ARABIC_FONT_PAGE:
                                sDetails = "Traditional Arabic Windows 3.1 font page";
                                break;
                            case OEM_ARABIC_FONT_PAGE:
                                sDetails = "OEM Arabic Windows 3.1 font page";
                                break;
                            case SIMP_FARSI_FONT_PAGE:
                                sDetails = "Simplified Farsi Windows 3.1 font page";
                                break;
                            case TRAD_FARSI_FONT_PAGE:
                                sDetails = "Traditional Farsi Windows 3.1 font page";
                                break;
                            case THAI_FONT_PAGE:
                                sDetails = "Thai Windows 3.1 font page";
                                break;
                            default:
                                for (int i=0; i<16; i++)
                                {
                                    int nBitValue = 1<<i;
                                    if ((nBitValue & 0xFF80) != 0)
                                    {
                                        if ((fsSelection & nBitValue) != 0)
                                        {
                                            if (bFoundFirstBadBit)
                                            {
                                                sDetails += ", " + i;
                                            }
                                            else
                                            {
                                                sDetails += i.ToString();
                                                bFoundFirstBadBit = true;
                                            }
                                        }
                                    }
                                }
                                break;
                            
                        }
                    }
                    else
                    {
                        for (int i=0; i<16; i++)
                        {
                            int nBitValue = 1<<i;
                            if ((nBitValue & 0xFF80) != 0)
                            {
                                if ((fsSelection & nBitValue) != 0)
                                {
                                    if (bFoundFirstBadBit)
                                    {
                                        sDetails += ", " + i;
                                    }
                                    else
                                    {
                                        sDetails += i.ToString();
                                        bFoundFirstBadBit = true;
                                    }
                                }
                            }
                        }
                    }

                    v.Error(T.OS_2_fsSelection, E.OS_2_E_fsSelection_undefbits, m_tag, sDetails);
                    bSelOk = false;
                    bRet = false;
                }

                // compare to head.macStyle italic and bold bits
                Table_head headTable = (Table_head)fontOwner.GetTable("head");
                if (headTable != null)
                {
                    bool bItalic = ((fsSelection & 0x01) != 0 );
                    bool bBold   = ((fsSelection & 0x20) != 0 );

                    bool bMacBold = ((headTable.macStyle & 0x0001) != 0);
                    bool bMacItal = ((headTable.macStyle & 0x0002) != 0);

                    if (bItalic != bMacItal)
                    {
                        string sDetails = null;
                        if (bItalic) sDetails = "fsSelection italic bit is set, but macstyle italic bit is clear";
                        else sDetails =  "fsSelection italic bit is clear, but macstyle italic bit is set";

                        v.Error(T.OS_2_fsSelection, E.OS_2_E_fsSelection_macStyle_italic, m_tag, sDetails);
                        bSelOk = false;
                        bRet = false;
                    }

                    if (bBold != bMacBold)
                    {
                        string sDetails = null;
                        if (bBold) sDetails = "fsSelection bold bit is set, but macstyle bold bit is clear";
                        else sDetails =  "fsSelection bold bit is clear, but macstyle bold bit is set";

                        v.Error(T.OS_2_fsSelection, E.OS_2_E_fsSelection_macStyle_bold, m_tag, sDetails);
                        bSelOk = false;
                        bRet = false;
                    }
                }

                if ((fsSelection & 0x01) != 0 ) // italic bit
                {
                    // compare to name subfamily
                    if (sStyleLower != null)
                    {
                        if (sStyleLower.IndexOf("italic") == -1 && sStyleLower.IndexOf("oblique") == -1)
                        {
                            v.Error(T.OS_2_fsSelection, E.OS_2_E_fsSelection_subfamily, m_tag, "fsSelection italic bit is set, but subfamily is " + sStyle);
                            bSelOk = false;
                            bRet = false;
                        }
                    }
                }

                if ((fsSelection & 0x20) != 0 ) // bold bit
                {
                    // compare to name subfamily
                    if (sStyleLower != null)
                    {
                        if (sStyleLower.IndexOf("bold") == -1)
                        {
                            v.Error(T.OS_2_fsSelection, E.OS_2_E_fsSelection_subfamily, m_tag, "fsSelection bold bit is set, but subfamily is " + sStyle);
                            bSelOk = false;
                            bRet = false;
                        }
                    }

                    if (usWeightClass <= 500)
                    {
                        v.Warning(T.OS_2_fsSelection, W.OS_2_W_fsSelection_weight, m_tag, "usWeightClass = " + usWeightClass);
                        bSelOk = false;
                    }
                }

                if ((fsSelection & 0x40) != 0 ) // regular bit
                {
                    if ((fsSelection & 0x20) != 0 )
                    {
                        v.Error(T.OS_2_fsSelection, E.OS_2_E_reg_bold, m_tag);
                        bSelOk = false;
                        bRet = false;
                    }
                    if ((fsSelection & 0x01) != 0) 
                    {
                        v.Error(T.OS_2_fsSelection, E.OS_2_E_reg_ital, m_tag);
                        bSelOk = false;
                        bRet = false;
                    }

                    if (sStyleLower != null)
                    {
                        // compare to name subfamily
                        if (sStyleLower.CompareTo("regular") != 0)
                        {
                            v.Error(T.OS_2_fsSelection, E.OS_2_E_fsSelection_subfamily, m_tag, "fsSelection regular bit is set, but subfamily is " + sStyle);
                            bSelOk = false;
                            bRet = false;
                        }
                    }                    
                }

                if (bSelOk)
                {
                    v.Pass(T.OS_2_fsSelection, P.OS_2_P_fsSelection, m_tag);
                }
            }

            if (v.PerformTest(T.OS_2_CharIndexes))
            {
                Table_cmap cmapTable = (Table_cmap)fontOwner.GetTable("cmap");

                if (cmapTable != null)
                {
                    ushort charFirst = 0xffff;
                    ushort charLast  = 0;

                    Table_cmap.EncodingTableEntry eteUni = cmapTable.GetEncodingTableEntry(3,1);
                    if (eteUni == null)
                    {
                        // presumably a symbol font
                        eteUni = cmapTable.GetEncodingTableEntry(3,0);
                    }

                    if (eteUni != null)
                    {
                        Table_cmap.Subtable st = cmapTable.GetSubtable(eteUni);
                        if (st != null)
                        {
                            bool bCmapOk = true;
                            byte[] charbuf = new byte[2];
                            // find the first char
                            for (ushort c=0; c<0xffff; c++)
                            {
                                charbuf[0] = (byte)c;
                                charbuf[1] = (byte)(c>>8);
                                uint glyphID;
                                try
                                {
                                    glyphID = st.MapCharToGlyph(charbuf, 0);
                                }
                                catch
                                {
                                    bCmapOk = false;
                                    break;
                                }
                                if (glyphID != 0)
                                {
                                    charFirst = c;
                                    break;
                                }
                            }
                            // find the last char (start at fffd: fffe and ffff aren't legal characters)
                            if (bCmapOk)
                            {
                                for (ushort c=0xfffd; c>0; c--)
                                {
                                    charbuf[0] = (byte)c;
                                    charbuf[1] = (byte)(c>>8);
                                    uint glyphID;
                                    try
                                    {
                                        glyphID = st.MapCharToGlyph(charbuf, 0);
                                    }
                                    catch
                                    {
                                        bCmapOk = false;
                                        break;
                                    }

                                    if (glyphID != 0)
                                    {
                                        charLast = c;
                                        break;
                                    }
                                }
                            }

                            if (!bCmapOk)
                            {
                                v.Warning(T.OS_2_CharIndexes, W._TEST_W_ErrorInAnotherTable, m_tag, "usFirstCharIndex and usLastCharIndex cannot be validated due to errors in the cmap table");
                            }
                            else if (usFirstCharIndex == charFirst && usLastCharIndex == charLast)
                            {
                                v.Pass(T.OS_2_CharIndexes, P.OS_2_P_CharIndexes, m_tag, "first = 0x" + usFirstCharIndex.ToString("x4") + ", last = 0x" + usLastCharIndex.ToString("x4"));
                            }
                            else
                            {
                                if (usFirstCharIndex != charFirst)
                                {
                                    String sDetails = "actual = 0x" + usFirstCharIndex.ToString("x4") + ", calculated = 0x" + charFirst.ToString("x4");
                                    v.Error(T.OS_2_CharIndexes, E.OS_2_E_usFirstCharIndex, m_tag, sDetails);
                                    bRet = false;
                                }
                        
                                if (usLastCharIndex != charLast)
                                {
                                    String sDetails = "actual = 0x" + usLastCharIndex.ToString("x4") + ", calculated = 0x" + charLast.ToString("x4");
                                    v.Error(T.OS_2_CharIndexes, E.OS_2_E_usLastCharIndex, m_tag, sDetails);
                                    bRet = false;
                                }
                            }
                        }
                        else
                        {
                            v.Warning(T.OS_2_CharIndexes, W._TEST_W_ErrorInAnotherTable, m_tag, "usFirstCharIndex and usLastCharIndex cannot be validated due to errors in the cmap table");
                        }
                    }

                }
                else
                {
                    v.Error(T.OS_2_CharIndexes, E._TEST_E_TableMissing, m_tag, "cmap");
                    bRet = false;
                }
            }

            if (v.PerformTest(T.OS_2_TypoMetrics))
            {
                bool bOk = true;

                if (sTypoAscender <= 0)
                {
                    v.Error(T.OS_2_TypoMetrics, E.OS_2_E_sTypoAscender_notpositive, m_tag, sTypoAscender.ToString());
                    bOk = false;
                    bRet = false;
                }

                if (sTypoDescender > 0)
                {
                    v.Error(T.OS_2_TypoMetrics, E.OS_2_E_sTypoDescender_positive, m_tag, sTypoDescender.ToString());
                    bOk = false;
                    bRet = false;
                }

                Table_head headTable = (Table_head)fontOwner.GetTable("head");
                if (headTable != null)
                {
                    if (sTypoAscender - sTypoDescender > headTable.unitsPerEm)
                    {
                        string sDetails = "sTypoAscender = " + sTypoAscender + ", sTypoDescender = " + sTypoDescender;
                        v.Warning(T.OS_2_TypoMetrics, W.OS_2_W_sTypoAscenderDescender_difference, m_tag, sDetails);
                        bOk = false;
                    }
                }
                else
                {
                    v.Error(T.OS_2_TypoMetrics, E._TEST_E_TableMissing, m_tag, "head");
                    bRet = false;
                }

                if (bOk)
                {
                    string sDetails = "sTypoAscender = " + sTypoAscender + ", sTypoDescender = " + sTypoDescender;
                    v.Pass(T.OS_2_TypoMetrics, P.OS_2_P_sTypoAscenderDescender, m_tag, sDetails);
                }
            }

            if (v.PerformTest(T.OS_2_CodePageRanges))
            {
                if (version >= 1)
                {
                    bRet &= CheckCodePageRanges(v, fontOwner);
                }
            }

            if (v.PerformTest(T.OS_2_sxHeight))
            {
                if (version >= 2)
                {
                    if (sxHeight == 0)
                    {
                        if (fontOwner.FastMapUnicodeToGlyphID((char)0x0078) != 0)
                        {
                            v.Error(T.OS_2_sxHeight, E.OS_2_E_sxHeight, m_tag);
                            bRet = false;
                        }
                        else
                        {
                            v.Pass(T.OS_2_sxHeight, P.OS_2_P_sxHeight_zero, m_tag, sxHeight.ToString());
                        }
                    }
                    else
                    {
                        v.Pass(T.OS_2_sxHeight, P.OS_2_P_sxHeight_nonzero, m_tag, sxHeight.ToString());
                    }
                }
            }

            if (v.PerformTest(T.OS_2_sCapHeight))
            {
                if (version >= 2)
                {
                    if (sCapHeight == 0)
                    {
                        if (fontOwner.FastMapUnicodeToGlyphID((char)0x0048) != 0)
                        {
                            v.Error(T.OS_2_sCapHeight, E.OS_2_E_sCapHeight, m_tag);
                            bRet = false;
                        }
                        else
                        {
                            v.Pass(T.OS_2_sCapHeight, P.OS_2_P_sCapHeight_zero, m_tag, sCapHeight.ToString());
                        }
                    }
                    else
                    {
                        v.Pass(T.OS_2_sCapHeight, P.OS_2_P_sCapHeight_nonzero, m_tag, sCapHeight.ToString());
                    }
                }
            }

            if (v.PerformTest(T.OS_2_usDefaultChar))
            {
                if (version >= 2)
                {
                    if (usDefaultChar == 0)
                    {
                        v.Pass(T.OS_2_usDefaultChar, P.OS_2_P_usDefaultChar_zero, m_tag);
                    }
                    else if (fontOwner.FastMapUnicodeToGlyphID((char)usDefaultChar) != 0)
                    {
                        v.Pass(T.OS_2_usDefaultChar, P.OS_2_P_usDefaultChar_nonzero, m_tag, "0x" + usDefaultChar.ToString("x4"));
                    }
                    else
                    {
                        v.Error(T.OS_2_usDefaultChar, E.OS_2_E_usDefaultChar_notmapped, m_tag, "0x" + usDefaultChar.ToString("x4"));
                        bRet = false;
                    }
                }
            }

            if (v.PerformTest(T.OS_2_usBreakChar))
            {
                if (version >= 2)
                {
                    if (fontOwner.FastMapUnicodeToGlyphID((char)usBreakChar) != 0)
                    {
                        v.Pass(T.OS_2_usBreakChar, P.OS_2_P_usBreakChar_mapped, m_tag);
                    }
                    else
                    {
                        v.Error(T.OS_2_usBreakChar, E.OS_2_E_usBreakChar_notmapped, m_tag, "0x" + usBreakChar.ToString("x4"));
                        bRet = false;
                    }
                }
            }

            if (v.PerformTest(T.OS_2_usMaxContext))
            {
                if (version >= 2)
                {
                    ushort GPOSMaxContext = 0;
                    Table_GPOS GPOSTable = (Table_GPOS)fontOwner.GetTable("GPOS");
                    if (GPOSTable != null)
                    {
                        GPOSMaxContext = GPOSTable.GetMaxContext();
                    }

                    ushort GSUBMaxContext = 0;
                    Table_GSUB GSUBTable = (Table_GSUB)fontOwner.GetTable("GSUB");
                    if (GSUBTable != null)
                    {
                        GSUBMaxContext = GSUBTable.GetMaxContext();
                    }

                    ushort CalcMaxContext = Math.Max(GPOSMaxContext, GSUBMaxContext);

                    if (usMaxContext == CalcMaxContext)
                    {
                        v.Pass(T.OS_2_usMaxContext, P.OS_2_P_usMaxContext, m_tag, usMaxContext.ToString());
                    }
                    else
                    {
                        v.Error(T.OS_2_usMaxContext, E.OS_2_E_usMaxContext, m_tag, "calc = " + CalcMaxContext + ", actual = " + usMaxContext);
                        bRet = false;
                    }
                }
            }

            return bRet;
        }

        public class UnicodeRanges
        {
            public UnicodeRanges()
            {
                m_arrRanges = MakeRanges();
            }

            Range [] m_arrRanges;


            public class Range
            {
                public Range(uint i, uint low, uint high, string sName)
                {
                    index = i;
                    m_nLow = low;
                    m_nHigh = high;
                    sRangeName = sName;
                    count = 0;
                }

                public uint index;
                public uint m_nLow;
                public uint m_nHigh;
                public string sRangeName;
                public uint count;
            }

            Range [] MakeRanges()
            {
                uint i=0;
                Range [] arrRanges =
                    {
                        // unicode ranges from the Unicode 4.0 spec, sorted by ascending ranges
                        new Range(i++, 0x0000, 0x007f, "Basic Latin"),
                        new Range(i++, 0x0080, 0x00ff, "Latin-1 Supplement"),
                        new Range(i++, 0x0100, 0x017f, "Latin Extended-A"),
                        new Range(i++, 0x0180, 0x024f, "Latin Extended-B"),
                        new Range(i++, 0x0250, 0x02af, "IPA Extensions"),
                        new Range(i++, 0x02b0, 0x02ff, "Spacing Modifier Letters"),
                        new Range(i++, 0x0300, 0x036f, "Combining Diacritical Marks"),
                        new Range(i++, 0x0370, 0x03ff, "Greek and Coptic"),
                        new Range(i++, 0x0400, 0x04ff, "Cyrillic"),
                        new Range(i++, 0x0500, 0x052f, "Cyrillic Supplementary"),
                        new Range(i++, 0x0530, 0x058f, "Armenian"),
                        new Range(i++, 0x0590, 0x05ff, "Hebrew"),
                        new Range(i++, 0x0600, 0x06ff, "Arabic"),
                        new Range(i++, 0x0700, 0x074f, "Syriac"),
                        new Range(i++, 0x0780, 0x07bf, "Thaana"),
                        new Range(i++, 0x0900, 0x097f, "Devanagari"),
                        new Range(i++, 0x0980, 0x09ff, "Bengali"),
                        new Range(i++, 0x0a00, 0x0a7f, "Gurmukhi"),
                        new Range(i++, 0x0a80, 0x0aff, "Gujarati"),
                        new Range(i++, 0x0b00, 0x0b7f, "Oriya"),
                        new Range(i++, 0x0b80, 0x0bff, "Tamil"),
                        new Range(i++, 0x0c00, 0x0c7f, "Telugu"),
                        new Range(i++, 0x0c80, 0x0cff, "Kannada"),
                        new Range(i++, 0x0d00, 0x0d7f, "Malayalam"),
                        new Range(i++, 0x0d80, 0x0dff, "Sinhala"),
                        new Range(i++, 0x0e00, 0x0e7f, "Thai"),
                        new Range(i++, 0x0e80, 0x0eff, "Lao"),
                        new Range(i++, 0x0f00, 0x0fff, "Tibetan"),
                        new Range(i++, 0x1000, 0x109f, "Myanmar"),
                        new Range(i++, 0x10a0, 0x10ff, "Georgian"),
                        new Range(i++, 0x1100, 0x11ff, "Hangul Jamo"),
                        new Range(i++, 0x1200, 0x137f, "Ethiopic"),
                        new Range(i++, 0x13a0, 0x13ff, "Cherokee"),
                        new Range(i++, 0x1400, 0x167f, "Unified Canadian Aboriginal Syllabic"),
                        new Range(i++, 0x1680, 0x169f, "Ogham"),
                        new Range(i++, 0x16a0, 0x16ff, "Runic"),
                        new Range(i++, 0x1700, 0x171f, "Tagalog"),
                        new Range(i++, 0x1720, 0x173f, "Hanunoo"),
                        new Range(i++, 0x1740, 0x175f, "Buhid"),
                        new Range(i++, 0x1760, 0x177f, "Tagbanwa"),
                        new Range(i++, 0x1780, 0x17ff, "Khmer"),
                        new Range(i++, 0x1800, 0x18af, "Mongolian"),
                        new Range(i++, 0x1900, 0x194f, "Limbu"),
                        new Range(i++, 0x1950, 0x197f, "Tai Le"),
                        new Range(i++, 0x19e0, 0x19ff, "Khmer Symbols"),
                        new Range(i++, 0x1d00, 0x1d7f, "Phonetic Extensions"),
                        new Range(i++, 0x1e00, 0x1eff, "Latin Extended Additional"),
                        new Range(i++, 0x1f00, 0x1fff, "Greek Extended"),
                        new Range(i++, 0x2000, 0x206f, "General Punctuation"),
                        new Range(i++, 0x2070, 0x209f, "Superscripts and Subscripts"),
                        new Range(i++, 0x20a0, 0x20cf, "Currency Symbols"),
                        new Range(i++, 0x20d0, 0x20ff, "Combining Marks for Symbols"),
                        new Range(i++, 0x2100, 0x214f, "Letterlike Symbols"),
                        new Range(i++, 0x2150, 0x218f, "Number Forms"),
                        new Range(i++, 0x2190, 0x21ff, "Arrows"),
                        new Range(i++, 0x2200, 0x22ff, "Mathematical Operators"),
                        new Range(i++, 0x2300, 0x23ff, "Miscellaneous Technical"),
                        new Range(i++, 0x2400, 0x243f, "Control Pictures"),
                        new Range(i++, 0x2440, 0x245f, "Optical Character Recognition"),
                        new Range(i++, 0x2460, 0x24ff, "Enclosed Alphanumerics"),
                        new Range(i++, 0x2500, 0x257f, "Box Drawing"),
                        new Range(i++, 0x2580, 0x259f, "Block Elements"),
                        new Range(i++, 0x25a0, 0x25ff, "Geometric Shapes"),
                        new Range(i++, 0x2600, 0x26ff, "Miscellaneous Symbols"),
                        new Range(i++, 0x2700, 0x27bf, "Dingbats"),
                        new Range(i++, 0x27c0, 0x27ef, "Miscellaneous Mathematical Symbols-A"),
                        new Range(i++, 0x27f0, 0x27ff, "Supplemental Arrows-A"),
                        new Range(i++, 0x2800, 0x28ff, "Braille Patterns"),
                        new Range(i++, 0x2900, 0x297f, "Supplemental Arrows-B"),
                        new Range(i++, 0x2980, 0x29ff, "Miscellaneous Mathematical Symbols-B"),
                        new Range(i++, 0x2a00, 0x2aff, "Supplemental Mathematical Operators"),
                        new Range(i++, 0x2b00, 0x2bff, "Miscellaneous Symbols and Arrows"),
                        new Range(i++, 0x2e80, 0x2eff, "CJK Radicals Supplement"),
                        new Range(i++, 0x2f00, 0x2fdf, "Kangxi Radicals"),
                        new Range(i++, 0x2ff0, 0x2fff, "Ideographic Description Characters"),
                        new Range(i++, 0x3000, 0x303f, "CJK Symbols and Punctuation"),
                        new Range(i++, 0x3040, 0x309f, "Hiragana"),
                        new Range(i++, 0x30a0, 0x30ff, "Katakana"),
                        new Range(i++, 0x3100, 0x312f, "Bopomofo"),
                        new Range(i++, 0x3130, 0x318f, "Hangul Compatibility Jamo"),
                        new Range(i++, 0x3190, 0x319f, "Kanbun"),
                        new Range(i++, 0x31a0, 0x31bf, "Bopomofo Extended"),
                        new Range(i++, 0x31f0, 0x31ff, "Katakana Phonetic Extensions"),
                        new Range(i++, 0x3200, 0x32ff, "Enclosed CJK Letters and Months"),
                        new Range(i++, 0x3300, 0x33ff, "CJK Compatibility"),
                        new Range(i++, 0x3400, 0x4dbf, "CJK Unified Ideographs Extension A"),
                        new Range(i++, 0x4dc0, 0x4dff, "Yijing Hexagram Symbols"),
                        new Range(i++, 0x4e00, 0x9fff, "CJK Unified Ideographs"),
                        new Range(i++, 0xa000, 0xa48f, "Yi Syllables"),
                        new Range(i++, 0xa490, 0xa4cf, "Yi Radicals"),
                        new Range(i++, 0xac00, 0xd7af, "Hangul Syllables"),
                        new Range(i++, 0xd800, 0xd87f, "High Surrogates"),
                        new Range(i++, 0xd880, 0xdbff, "High Private Use Surrogates"),
                        new Range(i++, 0xdc00, 0xdfff, "Low Surrogates"),
                        new Range(i++, 0xe000, 0xf8ff, "Private Use Area"),
                        new Range(i++, 0xf900, 0xfaff, "CJK Compatibility Ideographs"),
                        new Range(i++, 0xfb00, 0xfb4f, "Alphabetic Presentation Forms"),
                        new Range(i++, 0xfb50, 0xfdff, "Arabic Presentation Forms-A"),
                        new Range(i++, 0xfe00, 0xfe0f, "Variation Selectors"),
                        new Range(i++, 0xfe20, 0xfe2f, "Combining Half Marks"),
                        new Range(i++, 0xfe30, 0xfe4f, "CJK Compatibility Forms"),
                        new Range(i++, 0xfe50, 0xfe6f, "Small Form Variants"),
                        new Range(i++, 0xfe70, 0xfeff, "Arabic Presentation Forms-B"),
                        new Range(i++, 0xff00, 0xffef, "Halfwidth and Fullwidth Forms"),
                        new Range(i++, 0xfff0, 0xffff, "Specials"),
                        new Range(i++, 0x10000, 0x1007f, "Linear B Syllabary"),
                        new Range(i++, 0x10080, 0x100ff, "Linear B Ideograms"),
                        new Range(i++, 0x10100, 0x1013f, "Agean Numbers"),
                        new Range(i++, 0x10300, 0x1032f, "Old Italic"),
                        new Range(i++, 0x10330, 0x1034f, "Gothic"),
                        new Range(i++, 0x10380, 0x1039f, "Ugaritic"),
                        new Range(i++, 0x10400, 0x1044f, "Deseret"),
                        new Range(i++, 0x10450, 0x1047f, "Shavian"),
                        new Range(i++, 0x10480, 0x104af, "Osmanya"),
                        new Range(i++, 0x10800, 0x1083f, "Cypriot Syllabary"),
                        new Range(i++, 0x1d000, 0x1d0ff, "Byzantine Musical Symbols"),
                        new Range(i++, 0x1d100, 0x1d1ff, "Musical Symbols"),
                        new Range(i++, 0x1d300, 0x1d35f, "Tai Xuan Jing Symbols"),
                        new Range(i++, 0x1d400, 0x1d7ff, "Mathematical Alphanumeric Symbols"),
                        new Range(i++, 0x20000, 0x2a6df, "CJK Unified Ideographs Extension B"),
                        new Range(i++, 0x2f800, 0x2fa1f, "CJK Compatibility Ideographs Supplement"),
                        new Range(i++, 0xe0000, 0xe007f, "Tags"),
                        new Range(i++, 0xe0100, 0xe01ef, "Variation Selectors Supplement"),
                        new Range(i++, 0xf0000, 0xffffd, "Supplementary Private Use Area-A"),
                        new Range(i++, 0x100000, 0x10ffff, "Supplementary Private Use Area-B")
                    };

                return arrRanges;
            }


            public Range GetRange(uint charcode)
            {
                Range urReturn = null;

                uint iFirst = 0;
                uint iLast = (uint)m_arrRanges.Length-1;
                Range urFirst = m_arrRanges[iFirst];
                Range urLast = m_arrRanges[iLast];

                if (charcode >= urFirst.m_nLow && charcode <= urLast.m_nHigh)
                {
                    while (iFirst <= iLast)
                    {
                        if (iFirst+1 == iLast)
                        {
                            urFirst = m_arrRanges[iFirst];
                            urLast = m_arrRanges[iLast];
                            if (charcode >= urFirst.m_nLow && charcode <= urFirst.m_nHigh)
                                urReturn = urFirst;
                            else if (charcode >= urLast.m_nLow && charcode <= urLast.m_nHigh)
                            {
                                urReturn = urLast;
                            }
                            break;
                        }
                        else
                        {
                            uint iMiddle = (iFirst + iLast)/2;
                            Range urMiddle = m_arrRanges[iMiddle];
                    
                            if (charcode < urMiddle.m_nLow)
                            {
                                iLast = iMiddle-1;
                            }
                            else if (charcode > urMiddle.m_nHigh)
                            {
                                iFirst = iMiddle+1;
                            }
                            else
                            {
                                urReturn = urMiddle;
                                break;
                            }
                        }
                    }
                }

                return urReturn;
            }

            public uint GetNumRanges()
            {
                return (uint)m_arrRanges.Length;
            }

        }

        private bool CheckUnicodeRanges(Validator v, OTFont fontOwner)
        {
            bool bRet = true;
            bool bOk = true;

            if (version == 0)
            {
                if (ulUnicodeRange1 != 0)
                {
                    v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "Range[0] was undefined. All bits must be 0.");
                    bOk = false;
                }
                if (ulUnicodeRange2 != 0)
                {
                    v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "Range[1] was undefined. All bits must be 0.");
                    bOk = false;
                }
                if (ulUnicodeRange3 != 0)
                {
                    v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "Range[2] was undefined. All bits must be 0.");
                    bOk = false;
                }
                if (ulUnicodeRange4 != 0)
                {
                    v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "Range[3] was undefined. All bits must be 0.");
                    bOk = false;
                }

                if (bOk)
                {
                    v.Pass(P.OS_2_P_UnicodeRanges, m_tag);
                }
                else
                {
                    bRet = false;
                }

                return bRet;
            }

            // count the number of entries in each unicode range in the cmap subtable
            

            UnicodeRanges ur = new UnicodeRanges();
            for (uint c = 0; c < 0xffff; c++)
            {
                // check if c is mapped to a glyph
                uint iGlyph = fontOwner.FastMapUnicodeToGlyphID((char)c);
                if (iGlyph != 0)
                {
                    UnicodeRanges.Range r = ur.GetRange(c);
                    if (r != null)
                    {
                        r.count++;
                    }
                }
            }


            uint nCharsInRange;


            // bit 0
            uint BASIC_LATIN_LOW                     = 0x0020; 
            nCharsInRange = ur.GetRange(BASIC_LATIN_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00000001, nCharsInRange, "Basic Latin");

            // bit 1
            uint LATIN_1_SUPPLEMENT_LOW                = 0x00A0; 
            nCharsInRange = ur.GetRange(LATIN_1_SUPPLEMENT_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00000002, nCharsInRange, "Latin-1 Supplement");

            // bit 2
            uint LATIN_EXTENDED_A_LOW                = 0x0100; 
            nCharsInRange = ur.GetRange(LATIN_EXTENDED_A_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00000004, nCharsInRange, "Latin Extended-A");

            // bit 3
            uint LATIN_EXTENDED_B_LOW                = 0x0180; 
            nCharsInRange = ur.GetRange(LATIN_EXTENDED_B_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00000008, nCharsInRange, "Latin Extended-B");

            // bit 4
            uint IPA_EXTENSIONS_LOW                    = 0x0250; 
            nCharsInRange = ur.GetRange(IPA_EXTENSIONS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00000010, nCharsInRange, "IPA Extensions");

            // bit 5
            uint SPACING_MODIFIER_LETTERS_LOW        = 0x02B0; 
            nCharsInRange = ur.GetRange(SPACING_MODIFIER_LETTERS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00000020, nCharsInRange, "Spacing Modifier Letters");

            // bit 6
            uint COMBINING_DIACRITICAL_MARKS_LOW     = 0x0300; 
            nCharsInRange = ur.GetRange(COMBINING_DIACRITICAL_MARKS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00000040, nCharsInRange, "Combining Diacritical Marks");

            // bit 7
            uint GREEK_LOW                             = 0x0370; 
            nCharsInRange = ur.GetRange(GREEK_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00000080, nCharsInRange, "Greek");
            
            // bit 8 reserved
            if ((ulUnicodeRange1 & 0x00000100) != 0)
            {
                bOk = false;
                v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "bit #8");
            }

            // bit 9
            uint CYRILLIC_LOW                        = 0x0400; 
            uint CYRILLIC_SUPPLEMENTARY_LOW         = 0x0500;
            nCharsInRange = ur.GetRange(CYRILLIC_LOW).count
                          + ur.GetRange(CYRILLIC_SUPPLEMENTARY_LOW).count;
            bOk &= VerifyUnicodeRanges(v, ulUnicodeRange1, 0x00000200, nCharsInRange, "Cyrillic, Cyrillic Supplementary");
            
            // bit 10
            uint ARMENIAN_LOW                        = 0x0530; 
            nCharsInRange = ur.GetRange(ARMENIAN_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00000400, nCharsInRange, "Armenian");
            
            // bit 11
            uint HEBREW_LOW                            = 0x0590; 
            nCharsInRange = ur.GetRange(HEBREW_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00000800, nCharsInRange, "Hebrew");
            
            if (version > 1)
            {
                // bit 12 reserved
                if ((ulUnicodeRange1 & 0x00001000) != 0)
                {
                    bOk = false;
                    v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "bit #12");
                }
            }

            // bit 13
            uint ARABIC_LOW                            = 0x0600; 
            nCharsInRange = ur.GetRange(ARABIC_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00002000, nCharsInRange, "Arabic");
            
            if (version > 1)
            {
                // bit 14 reserved
                if ((ulUnicodeRange1 & 0x00004000) != 0)
                {
                    bOk = false;
                    v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "bit #14");
                }
            }

            // bit 15
            uint DEVANAGARI_LOW                        = 0x0900; 
            nCharsInRange = ur.GetRange(DEVANAGARI_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00008000, nCharsInRange, "Devanagari");
            
            // bit 16
            uint BENGALI_LOW                         = 0x0980; 
            nCharsInRange = ur.GetRange(BENGALI_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00010000, nCharsInRange, "Bengali");
            
            // bit 17
            uint GURMUKHI_LOW                        = 0x0A00; 
            nCharsInRange = ur.GetRange(GURMUKHI_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00020000, nCharsInRange, "Gurmukhi");
            
            // bit 18
            uint GUJARATI_LOW                        = 0x0A80; 
            nCharsInRange = ur.GetRange(GUJARATI_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00040000, nCharsInRange, "Gujarati");
            
            // bit 19
            uint ORIYA_LOW                            = 0x0B00; 
            nCharsInRange = ur.GetRange(ORIYA_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00080000, nCharsInRange, "Oriya");
            
            // bit 20
            uint TAMIL_LOW                            = 0x0B80; 
            nCharsInRange = ur.GetRange(TAMIL_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00100000, nCharsInRange, "Tamil");
            
            // bit 21
            uint TELUGU_LOW                            = 0x0C00; 
            nCharsInRange = ur.GetRange(TELUGU_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00200000, nCharsInRange, "Telugu");
            
            // bit 22
            uint KANNADA_LOW                         = 0x0C80; 
            nCharsInRange = ur.GetRange(KANNADA_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00400000, nCharsInRange, "Kannada");
            
            // bit 23
            uint MALAYALAM_LOW                        = 0x0D00; 
            nCharsInRange = ur.GetRange(MALAYALAM_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x00800000, nCharsInRange, "Malayalam");
            
            // bit 24
            uint THAI_LOW                            = 0x0E00; 
            nCharsInRange = ur.GetRange(THAI_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x01000000, nCharsInRange, "Thai");
            
            // bit 25
            uint LAO_LOW                             = 0x0E80; 
            nCharsInRange = ur.GetRange(LAO_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x02000000, nCharsInRange, "Lao");
            
            // bit 26
            uint GEORGIAN_LOW                        = 0x10A0; 
            nCharsInRange = ur.GetRange(GEORGIAN_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x04000000, nCharsInRange, "Georgian");
            
            if (version > 1)
            {
                // bit 27 reserved
                if ((ulUnicodeRange1 & 0x08000000) != 0)
                {
                    bOk = false;
                    v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "bit #27");
                }
            }
            
            // bit 28
            uint HANGUL_JAMO_LOW                     = 0x1100; 
            nCharsInRange = ur.GetRange(HANGUL_JAMO_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x10000000, nCharsInRange, "Hangul Jamo");
            
            // bit 29
            uint LATIN_EXTENDED_ADDITIONAL_LOW        = 0x1E00; 
            nCharsInRange = ur.GetRange(LATIN_EXTENDED_ADDITIONAL_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x20000000, nCharsInRange, "Latin Extended Additional");
            
            // bit 30
            uint GREEK_EXTENDED_LOW                    = 0x1F00; 
            nCharsInRange = ur.GetRange(GREEK_EXTENDED_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x40000000, nCharsInRange, "Greek Extended");
            
            // bit 31
            uint GENERAL_PUNCTUATION_LOW             = 0x2000; 
            nCharsInRange = ur.GetRange(GENERAL_PUNCTUATION_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange1, 0x80000000, nCharsInRange, "General Punctuation");


            // bit 32
            uint SUPER_SUB_SCRIPTS_LOW                = 0x2070; 
            nCharsInRange = ur.GetRange(SUPER_SUB_SCRIPTS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00000001, nCharsInRange, "Superscripts and Subscripts");
            
            // bit 33
            uint CURRENCY_SYMBOLS_LOW                = 0x20A0; 
            nCharsInRange = ur.GetRange(CURRENCY_SYMBOLS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00000002, nCharsInRange, "Currency Symbols");
            
            // bit 34
            uint SYMBOL_COMBINING_MARKS_LOW            = 0x20D0; 
            nCharsInRange = ur.GetRange(SYMBOL_COMBINING_MARKS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00000004, nCharsInRange, "Combining Diacritical marks for symbols");
            
            // bit 35
            uint LETTERLIKE_SYMBOLS_LOW                = 0x2100; 
            nCharsInRange = ur.GetRange(LETTERLIKE_SYMBOLS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00000008, nCharsInRange, "Letterlike Symbols");
            
            // bit 36
            uint NUMBER_FORMS_LOW                    = 0x2150; 
            nCharsInRange = ur.GetRange(NUMBER_FORMS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00000010, nCharsInRange, "Number Forms");
            
            // bit 37
            uint ARROWS_LOW                            = 0x2190; 
            uint SUPPLEMENTAL_ARROWS_A_LOW            = 0x27F0;
            uint SUPPLEMENTAL_ARROWS_B_LOW            = 0x2900;
            nCharsInRange = ur.GetRange(ARROWS_LOW).count
                          + ur.GetRange(SUPPLEMENTAL_ARROWS_A_LOW).count
                          + ur.GetRange(SUPPLEMENTAL_ARROWS_B_LOW).count; 
            bOk &= VerifyUnicodeRanges(v, ulUnicodeRange2, 0x00000020, nCharsInRange, "Arrows, Supplementary Arrows A, Supplementary Arrows B");

            // bit 38
            uint MATH_OPERATORS_LOW                    = 0x2200; 
            uint SUPPLEMENTAL_MATH_OPERATORS_LOW    = 0x2A00;
            uint MISC_MATH_SYMBOLS_A_LOW            = 0x27C0;
            uint MISC_MATH_SYMBOLS_B_LOW            = 0x2980;
            nCharsInRange = ur.GetRange(MATH_OPERATORS_LOW).count
                          + ur.GetRange(SUPPLEMENTAL_MATH_OPERATORS_LOW).count
                          + ur.GetRange(MISC_MATH_SYMBOLS_A_LOW).count
                          + ur.GetRange(MISC_MATH_SYMBOLS_B_LOW).count;
            bOk &= VerifyUnicodeRanges(v, ulUnicodeRange2, 0x00000040, nCharsInRange, "Mathematical Operators, Supplemental Mathematical Operators, Mathematical Symbols A, Mathematical Symbols B");
            
            // bit 39
            uint MISC_TECHNICAL_LOW                    = 0x2300; 
            nCharsInRange = ur.GetRange(MISC_TECHNICAL_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00000080, nCharsInRange, "Miscellaneous Technical");
            
            // bit 40
            uint CONTROL_PICTURES_LOW                = 0x2400; 
            nCharsInRange = ur.GetRange(CONTROL_PICTURES_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00000100, nCharsInRange, "Control Pictures");
            
            // bit 41
            uint OCR_LOW                             = 0x2440; 
            nCharsInRange = ur.GetRange(OCR_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00000200, nCharsInRange, "Optical Character Recognition");
            
            // bit 42
            uint ENCLOSED_ALPHANUMERICS_LOW            = 0x2460; 
            nCharsInRange = ur.GetRange(ENCLOSED_ALPHANUMERICS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00000400, nCharsInRange, "Enclosed Alphanumerics");
            
            // bit 43
            uint BOX_DRAWING_LOW                     = 0x2500; 
            nCharsInRange = ur.GetRange(BOX_DRAWING_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00000800, nCharsInRange, "Box Drawing");
            
            // bit 44
            uint BLOCK_ELEMENTS_LOW                    = 0x2580; 
            nCharsInRange = ur.GetRange(BLOCK_ELEMENTS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00001000, nCharsInRange, "Block Elements");
            
            // bit 45
            uint GEOMETRIC_SHAPES_LOW                = 0x25A0; 
            nCharsInRange = ur.GetRange(GEOMETRIC_SHAPES_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00002000, nCharsInRange, "Geometric Shapes");
            
            // bit 46
            uint MISC_SYMBOLS_LOW                    = 0x2600; 
            nCharsInRange = ur.GetRange(MISC_SYMBOLS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00004000, nCharsInRange, "Miscellaneous Symbols");
            
            // bit 47
            uint DINGBATS_LOW                        = 0x2700; 
            nCharsInRange = ur.GetRange(DINGBATS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00008000, nCharsInRange, "Dingbats");
            
            // bit 48
            uint CJK_SYMBOLS_PUNCTUATION_LOW         = 0x3000; 
            nCharsInRange = ur.GetRange(CJK_SYMBOLS_PUNCTUATION_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00010000, nCharsInRange, "CJK Symbols and Punctuation");
            
            // bit 49
            uint HIRAGANA_LOW                        = 0x3040; 
            nCharsInRange = ur.GetRange(HIRAGANA_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00020000, nCharsInRange, "Hiragana");
            
            // bit 50
            uint KATAKANA_LOW                        = 0x30A0; 
            nCharsInRange = ur.GetRange(KATAKANA_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00040000, nCharsInRange, "Katakana");
            
            // bit 51
            uint BOPOMOFO_LOW                        = 0x3100; 
            uint BOPOMOFO_EXTENDED_LOW                = 0x31A0;
            nCharsInRange = ur.GetRange(BOPOMOFO_LOW).count
                          + ur.GetRange(BOPOMOFO_EXTENDED_LOW).count; 
            bOk &= VerifyUnicodeRanges(v, ulUnicodeRange2, 0x00080000, nCharsInRange, "Bopomofo, Bopomofo Extended");
            
            // bit 52
            uint HANGUL_COMPAT_JAMO_LOW                = 0x3130; 
            nCharsInRange = ur.GetRange(HANGUL_COMPAT_JAMO_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00100000, nCharsInRange, "Hangul Compatibility Jamo");
            
            // bit 53 !!! OT Spec 1.3 says bit 53 is CJK Misc !!!
            //        !!! Since there's no unicode range name !!!
            //        !!! that maps nicely to CJK Misc,       !!!
            //        !!! I naturally just ignore it.         !!!
            /*
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00200000, nCharsInRange, "CJK Miscellaneous");
            */
            
            // bit 54
            uint ENCLOSED_CJK_LETTERS_MONTHS_LOW     = 0x3200; 
            nCharsInRange = ur.GetRange(ENCLOSED_CJK_LETTERS_MONTHS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00400000, nCharsInRange, "Enclosed CJK Letters and Months");
            
            // bit 55
            uint CJK_COMPATIBILITY_LOW                = 0x3300; 
            nCharsInRange = ur.GetRange(CJK_COMPATIBILITY_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x00800000, nCharsInRange, "CJK Compatibility");
            
            // bit 56
            uint HANGUL_LOW                            = 0xAC00; 
            nCharsInRange = ur.GetRange(HANGUL_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x01000000, nCharsInRange, "Hangul");
            
            // bit 57 surrogates
            if (version > 1)
            {
                bool bSurrogatesCmapPresent = false;
                Table_cmap cmapTable = (Table_cmap)fontOwner.GetTable("cmap");
                if (cmapTable != null)
                {
                    Table_cmap.EncodingTableEntry eteUniSurrogates = cmapTable.GetEncodingTableEntry(3,10);
                    if (eteUniSurrogates != null)
                        bSurrogatesCmapPresent = true;
                }

                if ((ulUnicodeRange2 & 0x02000000) == 0)
                {
                    if (bSurrogatesCmapPresent)
                    {
                        v.Error(T.T_NULL, E.OS_2_E_SurrogatesBitClear, m_tag);
                        bOk = false;
                    }
                }
                else
                {
                    if (!bSurrogatesCmapPresent)
                    {
                        v.Error(T.T_NULL, E.OS_2_E_SurrogatesBitSet, m_tag);
                        bOk = false;
                    }
                }
            }

            // bit 58 reserved
            if ((ulUnicodeRange2 & 0x04000000) != 0)
            {
                bOk = false;
                v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "bit #58");
            }

            // bit 59
            uint CJK_UNIFIED_IDEOGRAPHS_LOW            = 0x4E00; 
            uint CJK_RADICALS_SUPPLEMENT_LOW        = 0x2E80;
            uint KANGXI_RADICALS_LOW                = 0x2F00;
            uint IDEOGRAPHIC_DESCRIPTION_CHARS_LOW    = 0x2FF0;
            uint CJK_UNIFIED_IDEOGRAPHS_EXT_A_LOW   = 0x3400;
            uint CJK_UNIFIED_IDEOGRAPHS_EXT_B_LOW   = 0x20000;
            uint KANBUN_LOW                         = 0x3190;
            nCharsInRange = ur.GetRange(CJK_UNIFIED_IDEOGRAPHS_LOW).count
                          + ur.GetRange(CJK_RADICALS_SUPPLEMENT_LOW).count
                          + ur.GetRange(KANGXI_RADICALS_LOW).count
                          + ur.GetRange(IDEOGRAPHIC_DESCRIPTION_CHARS_LOW).count
                          + ur.GetRange(CJK_UNIFIED_IDEOGRAPHS_EXT_A_LOW).count
                          + ur.GetRange(CJK_UNIFIED_IDEOGRAPHS_EXT_B_LOW).count
                          + ur.GetRange(KANBUN_LOW).count; 
            bOk &= VerifyUnicodeRanges(v, ulUnicodeRange2, 0x08000000, nCharsInRange, "CJK Unified Ideographs, CJK Radicals Supplement, Kangxi Radicals, Ideographic Description Chars, CJK Unified Ideographs Extended A, CJK Unified Ideographs Extended B, Kanbun");
            
            // bit 60
            uint PRIVATE_USE_AREA_LOW                = 0xE000; 
            if (!fontOwner.ContainsMsSymbolEncodedCmap())
            {
                nCharsInRange = ur.GetRange(PRIVATE_USE_AREA_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x10000000, nCharsInRange, "Private Use Area");
            }
            
            // bit 61
            uint CJK_COMPATIBILITY_IDEOGRAPHS_LOW    = 0xF900; 
            uint CJK_COMPATIBILITY_IDEO_SUPP_LOW    = 0x2F800;
            nCharsInRange = ur.GetRange(CJK_COMPATIBILITY_IDEOGRAPHS_LOW).count
                          + ur.GetRange(CJK_COMPATIBILITY_IDEO_SUPP_LOW).count; 
            bOk &= VerifyUnicodeRanges(v, ulUnicodeRange2, 0x20000000, nCharsInRange, "CJK Compatibility Ideographs, CJK Compatibility Ideographs Supplement");
            
            // bit 62
            uint ALPHABETIC_PRESENTATION_FORMS_LOW    = 0xFB00; 
            nCharsInRange = ur.GetRange(ALPHABETIC_PRESENTATION_FORMS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x40000000, nCharsInRange, "Alphabetic Presentation Forms");
            
            // bit 63
            uint ARABIC_PRESENTATION_FORMS_A_LOW     = 0xFB50; 
            nCharsInRange = ur.GetRange(ARABIC_PRESENTATION_FORMS_A_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange2, 0x80000000, nCharsInRange, "Arabic Presentation Forms-A");
            

            
            // bit 64
            uint COMBINING_HALF_MARKS_LOW            = 0xFE20; 
            nCharsInRange = ur.GetRange(COMBINING_HALF_MARKS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000001, nCharsInRange, "Combining Half Marks");
            
            // bit 65
            uint CJK_COMPATIBILITY_FORMS_LOW         = 0xFE30; 
            nCharsInRange = ur.GetRange(CJK_COMPATIBILITY_FORMS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000002, nCharsInRange, "CJK Compatibility Forms");
            
            // bit 66
            uint SMALL_FORM_VARIANTS_LOW             = 0xFE50; 
            nCharsInRange = ur.GetRange(SMALL_FORM_VARIANTS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000004, nCharsInRange, "Small Form Variants");
            
            // bit 67
            uint ARABIC_PRESENTATION_FORMS_B_LOW     = 0xFE70; 
            nCharsInRange = ur.GetRange(ARABIC_PRESENTATION_FORMS_B_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000008, nCharsInRange, "Arabic Presentation Forms-B");
            
            // bit 68
            uint HALFWIDTH_FULLWIDTH_FORMS_LOW        = 0xFF00; 
            nCharsInRange = ur.GetRange(HALFWIDTH_FULLWIDTH_FORMS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000010, nCharsInRange, "Halfwidth and Fullwidth Forms");
            
            // bit 69
            uint SPECIALS_LOW                        = 0xFFF0; 
            nCharsInRange = ur.GetRange(SPECIALS_LOW).count;
            bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000020, nCharsInRange, "Specials");

            if (version < 2)
            {
                for (int bitpos = 6; bitpos < 32; bitpos++)
                {
                    if ((ulUnicodeRange3 & (1<<bitpos)) != 0) { bOk = false; v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "bit #" + (64+bitpos)); }
                }

                for (int bitpos = 0; bitpos < 32; bitpos++)
                {
                    if ((ulUnicodeRange4 & (1<<bitpos)) != 0) { bOk = false; v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "bit #" + (96+bitpos)); }
                }
            }
            else
            {
                // bit 70
                uint TIBETAN_LOW                        = 0x0F00; 
                nCharsInRange = ur.GetRange(TIBETAN_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000040, nCharsInRange, "Tibetan");
                
                // bit 71
                uint SYRIAC_LOW                            = 0x0700; 
                nCharsInRange = ur.GetRange(SYRIAC_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000080, nCharsInRange, "Syriac");
                
                // bit 72
                uint THAANA_LOW                            = 0x0780; 
                nCharsInRange = ur.GetRange(THAANA_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000100, nCharsInRange, "Thaana");
                
                // bit 73
                uint SINHALA_LOW                        = 0x0D80; 
                nCharsInRange = ur.GetRange(SINHALA_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000200, nCharsInRange, "Sinhala");
                
                // bit 74
                uint MYANMAR_LOW                        = 0x1000; 
                nCharsInRange = ur.GetRange(MYANMAR_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000400, nCharsInRange, "Myanmar");
                
                // bit 75
                uint ETHIOPIC_LOW                        = 0x1200; 
                nCharsInRange = ur.GetRange(ETHIOPIC_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00000800, nCharsInRange, "Ethiopic");
                
                // bit 76
                uint CHEROKEE_LOW                        = 0x13A0; 
                nCharsInRange = ur.GetRange(CHEROKEE_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00001000, nCharsInRange, "Cherokee");
                
                // bit 77
                uint UNIFIED_CANADIAN_AB_SYL_LOW        = 0x1400; 
                nCharsInRange = ur.GetRange(UNIFIED_CANADIAN_AB_SYL_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00002000, nCharsInRange, "Unified Canadian Syllabics");
                
                // bit 78
                uint OGHAM_LOW                            = 0x1680; 
                nCharsInRange = ur.GetRange(OGHAM_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00004000, nCharsInRange, "Ogham");
                
                // bit 79
                uint RUNIC_LOW                            = 0x16A0; 
                nCharsInRange = ur.GetRange(RUNIC_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00008000, nCharsInRange, "Runic");
                
                // bit 80
                uint KHMER_LOW                            = 0x1780; 
                nCharsInRange = ur.GetRange(KHMER_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00010000, nCharsInRange, "Khmer");
                
                // bit 81
                uint MONGOLIAN_LOW                        = 0x1800; 
                nCharsInRange = ur.GetRange(MONGOLIAN_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00020000, nCharsInRange, "Mongolian");
                
                // bit 82
                uint BRAILLE_PATTERNS_LOW                = 0x2800; 
                nCharsInRange = ur.GetRange(BRAILLE_PATTERNS_LOW).count;
                bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00040000, nCharsInRange, "Braille");
                
                // bit 83
                uint YI_LOW                                = 0xA000; 
                uint YI_RADICALS_LOW                    = 0xA490;
                nCharsInRange = ur.GetRange(YI_LOW).count 
                              + ur.GetRange(YI_RADICALS_LOW).count;
                bOk &= VerifyUnicodeRanges(v, ulUnicodeRange3, 0x00080000, nCharsInRange, "Yi, Yi Radicals");
                

                if (version < 3)
                {
                    for (int bitpos = 20; bitpos < 32; bitpos++)
                    {
                        if ((ulUnicodeRange3 & (1<<bitpos)) != 0) { bOk = false; v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "bit #" + (64+bitpos)); }
                    }

                    for (int bitpos = 0; bitpos < 32; bitpos++)
                    {
                        if ((ulUnicodeRange4 & (1<<bitpos)) != 0) { bOk = false; v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "bit #" + (96+bitpos)); }
                    }
                }
                else
                {
                    // bit 84
                    uint TAGALOG_LOW                    = 0x1700;
                    uint HANUNOO_LOW                    = 0x1720;
                    uint BUHID_LOW                      = 0x1740;
                    uint TAGBANWA_LOW                   = 0x1760;
                    nCharsInRange = ur.GetRange(TAGALOG_LOW).count
                                  + ur.GetRange(HANUNOO_LOW).count
                                  + ur.GetRange(BUHID_LOW).count
                                  + ur.GetRange(TAGBANWA_LOW).count;
                    bOk &= VerifyUnicodeRanges(v, ulUnicodeRange3, 0x00100000, nCharsInRange, "Tagalog, Hanunoo, Buhid, Tagbanwa");

                    // bit 85
                    uint OLD_ITALIC_LOW                 = 0x10300;
                    nCharsInRange = ur.GetRange(OLD_ITALIC_LOW).count;
                    bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00200000, nCharsInRange, "Old Italic");

                    // bit 86
                    uint GOTHIC_LOW                     = 0x10330;
                    nCharsInRange = ur.GetRange(GOTHIC_LOW).count;
                    bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00400000, nCharsInRange, "Gothic");

                    // bit 87
                    uint DESERET_LOW                    = 0x10400;
                    nCharsInRange = ur.GetRange(DESERET_LOW).count;
                    bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x00800000, nCharsInRange, "Deseret");

                    // bit 88
                    uint BYZANTINE_MUSICAL_SYMBOLS_LOW  = 0x1D000;
                    uint MUSICAL_SYMBOLS_LOW            = 0x1D100;
                    nCharsInRange = ur.GetRange(BYZANTINE_MUSICAL_SYMBOLS_LOW).count
                                  + ur.GetRange(MUSICAL_SYMBOLS_LOW).count;
                    bOk &= VerifyUnicodeRanges(v, ulUnicodeRange3, 0x01000000, nCharsInRange, "Byzantine Musical Symbols, Musical Symbols");

                    // bit 89
                    uint MATHEMATICAL_ALPHANUMERIC_LOW  = 0x1D400;
                    nCharsInRange = ur.GetRange(MATHEMATICAL_ALPHANUMERIC_LOW).count;
                    bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x02000000, nCharsInRange, "Mathematical Alphanumeric Symbols");

                    // bit 90
                    uint PRIVATE_USE_15_LOW             = 0xFFF80;
                    uint PRIVATE_USE_16_LOW             = 0x10FF80;
                    nCharsInRange = ur.GetRange(PRIVATE_USE_15_LOW).count
                                  + ur.GetRange(PRIVATE_USE_16_LOW).count;
                    bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x04000000, nCharsInRange, "Private Use (Plane 15), Private Use (Plane 16)");

                    // bit 91
                    uint VARIATION_SELECTORS_LOW         = 0xE0100;
                    nCharsInRange = ur.GetRange(VARIATION_SELECTORS_LOW).count;
                    bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x08000000, nCharsInRange, "Variation Selectors");

                    // bit 92
                    uint TAGS_LOW                       = 0xE0000;
                    nCharsInRange = ur.GetRange(TAGS_LOW).count;
                    bOk &= VerifyUnicodeRange(v, ulUnicodeRange3, 0x10000000, nCharsInRange, "Tags");

                    for (int bitpos = 29; bitpos < 32; bitpos++)
                    {
                        if ((ulUnicodeRange3 & (1<<bitpos)) != 0) { bOk = false; v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "bit #" + (64+bitpos)); }
                    }

                    for (int bitpos = 0; bitpos < 32; bitpos++)
                    {
                        if ((ulUnicodeRange4 & (1<<bitpos)) != 0) { bOk = false; v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_Unicode, m_tag, "bit #" + (96+bitpos)); }
                    }
                }
            }

            if (bOk)
            {
                v.Pass(P.OS_2_P_UnicodeRanges, m_tag);
            }
            else
            {
                bRet = false;
            }

            return bRet;
        }

        bool VerifyUnicodeRange( Validator  v,
            uint        ulUnicodeRange,
            uint        ulUnicodeRangeBit,
            uint        nCharsInRange,
            string      sRangeName)
        {
            bool bRet = true;
            if(nCharsInRange != 0)
            {
                if((ulUnicodeRange & ulUnicodeRangeBit) == 0)
                {
                    v.Info(T.T_NULL, I.OS_2_I_RangeBitNotSet, m_tag, "'" + sRangeName + "', " + nCharsInRange + " characters are present");
                }
            }
            else
            {
                if((ulUnicodeRange & ulUnicodeRangeBit) != 0)
                {
                    v.Error(T.T_NULL, E.OS_2_E_RangeBitSet, m_tag, "No '" + sRangeName + "' characters are present");
                    bRet = false;
                }
            }

            return bRet;
        }

        bool VerifyUnicodeRanges( Validator  v,
            uint        ulUnicodeRange,
            uint        ulUnicodeRangeBit,
            uint        nCharsInRange,
            string      sRangeNames)
        {
            bool bRet = true;
            if(nCharsInRange != 0)
            {
                if((ulUnicodeRange & ulUnicodeRangeBit) == 0)
                {
                    v.Info(T.T_NULL, I.OS_2_I_RangeBitNotSet, m_tag, nCharsInRange + " characters are present in the ranges: " + sRangeNames);
                }
            }
            else
            {
                if((ulUnicodeRange & ulUnicodeRangeBit) != 0)
                {
                    v.Error(T.T_NULL, E.OS_2_E_RangeBitSet, m_tag, "No characters are present in the ranges: " + sRangeNames);
                    bRet = false;
                }
            }

            return bRet;
        }

        private bool CheckCodePageRanges(Validator v, OTFont fontOwner)
        {
            bool bRet = true;

            bRet &= CheckCodePageBit(0, 1252, "Latin 1",    v, fontOwner);
            bRet &= CheckCodePageBit(1, 1250, "Latin 2",    v, fontOwner);
            bRet &= CheckCodePageBit(2, 1251, "Cyrillic",   v, fontOwner);
            bRet &= CheckCodePageBit(3, 1253, "Greek",      v, fontOwner);
            bRet &= CheckCodePageBit(4, 1254, "Turkish",    v, fontOwner);
            bRet &= CheckCodePageBit(5, 1255, "Hebrew",     v, fontOwner);
            bRet &= CheckCodePageBit(6, 1256, "Arabic",     v, fontOwner);
            bRet &= CheckCodePageBit(7, 1257, "Baltic",     v, fontOwner);
            bRet &= CheckCodePageBit(8, 1258, "Vietnamese", v, fontOwner);

            // bits 9-15 are reserved
            bRet &= CheckCodePageBit( 9, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(10, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(11, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(12, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(13, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(14, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(15, 0, null, v, fontOwner);


            bRet &= CheckCodePageBit(16,  874, "Thai",                v, fontOwner);
            bRet &= CheckCodePageBit(17,  932, "Japanese Shift-JIS",  v, fontOwner);
            bRet &= CheckCodePageBit(18,  936, "Chinese simplified",  v, fontOwner);
            bRet &= CheckCodePageBit(19,  949, "Korean Wansung",      v, fontOwner);
            bRet &= CheckCodePageBit(20,  950, "Chinese traditional", v, fontOwner);
            bRet &= CheckCodePageBit(21, 1361, "Korean Johab",        v, fontOwner);

            // bits 22-28 are reserved
            bRet &= CheckCodePageBit(22, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(23, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(24, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(25, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(26, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(27, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(28, 0, null, v, fontOwner);


            bRet &= CheckCodePageBit(29, 10000, "Mac character set", v, fontOwner);
            //bRet &= CheckCodePageBit(30, ???? , "OEM character set", v, fontOwner); // no code page specified
            bRet &= CheckCodePageBit(31, 0, "Symbol character set", v, fontOwner);


            // bits 32-47 are reserved
            bRet &= CheckCodePageBit(32, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(33, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(34, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(35, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(36, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(37, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(38, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(39, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(40, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(41, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(42, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(43, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(44, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(45, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(46, 0, null, v, fontOwner);
            bRet &= CheckCodePageBit(47, 0, null, v, fontOwner);


            bRet &= CheckCodePageBit(48, 869, "IBM Greek",              v, fontOwner);
            bRet &= CheckCodePageBit(49, 866, "MS-DOS Russian",         v, fontOwner);
            bRet &= CheckCodePageBit(50, 865, "MS-DOS Nordic",          v, fontOwner);
            bRet &= CheckCodePageBit(51, 864, "Arabic",                 v, fontOwner);
            bRet &= CheckCodePageBit(52, 863, "MS-DOS Canadian French", v, fontOwner);
            bRet &= CheckCodePageBit(53, 862, "Hebrew",                 v, fontOwner);
            bRet &= CheckCodePageBit(54, 861, "MS-DOS Icelandic",       v, fontOwner);
            bRet &= CheckCodePageBit(55, 860, "MS-DOS Portuguese",      v, fontOwner);
            bRet &= CheckCodePageBit(56, 857, "IBM Turkish",            v, fontOwner);
            bRet &= CheckCodePageBit(57, 855, "IBM Cyrillic",           v, fontOwner);
            bRet &= CheckCodePageBit(58, 852, "Latin 2",                v, fontOwner);
            bRet &= CheckCodePageBit(59, 775, "MS-DOS Baltic",          v, fontOwner);
            bRet &= CheckCodePageBit(60, 737, "Greek",                  v, fontOwner);
            bRet &= CheckCodePageBit(61, 708, "Arabic; ASMO 708",       v, fontOwner);
            bRet &= CheckCodePageBit(62, 850, "WE/Latin 1",             v, fontOwner);
            bRet &= CheckCodePageBit(63, 437, "US",                     v, fontOwner);

            if (bRet == true)
            {
                v.Pass(P.OS_2_P_CodePageRanges, m_tag);
            }

            return bRet;
        }

        private bool CheckCodePageBit(int nBit, uint CodePage, string sName, Validator v, OTFont fontOwner)
        {
            bool bRet = true;

            int nTotalChars = 0;
            int nMissingChars = 0;

            // decode the bit number
            bool bBitSet;
            if (nBit < 32)
            {
                bBitSet = ((ulCodePageRange1 & (1<<nBit)) != 0);
            }
            else
            {
                bBitSet = ((ulCodePageRange2 & (1<<(nBit-32))) != 0);
            }


            if (nBit == 31)
            {
                // symbol character set
                Table_cmap cmapTable = (Table_cmap)fontOwner.GetTable("cmap");
                if (cmapTable != null)
                {
                    if (cmapTable.GetEncodingTableEntry(3,0) != null)
                    {
                        if (!bBitSet)
                        {
                            v.Error(T.T_NULL, E.OS_2_E_SymbolBitClear, m_tag);
                            bRet = false;
                        }
                    }
                    else
                    {
                        if (bBitSet)
                        {
                            int nPresentChars = 0;
                            for (ushort c=0xf000; c<= 0xf0ff; c++)
                            {
                                if (fontOwner.FastMapUnicodeToGlyphID((char)c) != 0)
                                {
                                    nPresentChars++;
                                }
                            }
                    
                            if (nPresentChars == 0)
                            {
                                v.Error(T.T_NULL, E.OS_2_E_SymbolBitSet, m_tag);
                                bRet = false;
                            }
                        }
                    }
                }
                else
                {
                    v.Error(T.T_NULL, E._TEST_E_TableMissing, m_tag, "cmap");
                    bRet = false;
                }
            }
            else if (CodePage == 0)
            {
                // reserved field, bit should not be set
                if (bBitSet)
                {
                    v.Error(T.T_NULL, E.OS_2_E_ReservedBitSet_CodePage, m_tag, "bit #" + nBit);
                    bRet = false;
                }
            }
            else
            {
                try
                {
                    if (MultiByte.IsCodePageInstalled(CodePage))
                    {
                        ushort [] arrMissingChars = new ushort[10];

                        uint nMaxCharSize = MultiByte.GetCodePageMaxCharSize(CodePage);
                        if (nMaxCharSize == 1)
                        {
                            for (ushort c = 0; c<256; c++)
                            {
                                // check for special case: MultiByteToWideChar maps char 0xca in CP1255 to U05BA, but CP1255 spec says its not defined
                                if (CodePage != 1255 || c != 0xca)
                                {
                                    CheckCodePageGlyph(CodePage, (char)c, ref nTotalChars, ref nMissingChars, arrMissingChars, fontOwner);
                                }
                            }
                        }
                        else if (nMaxCharSize == 2)
                        {
                            bool [] LeadByteMap = new bool[256];
                            for (int i=0; i<256; i++)
                            {
                                LeadByteMap[i] = MultiByte.IsCodePageLeadByte(CodePage, (byte)i);
                            }

                            for (ushort c = 0; c<256; c++)
                            {
                                if (LeadByteMap[c] == false)
                                {
                                    CheckCodePageGlyph(CodePage, (char)c, ref nTotalChars, ref nMissingChars, arrMissingChars, fontOwner);
                                }
                            }

                            for (uint leadbyte = 0; leadbyte<256; leadbyte++)
                            {
                                if (LeadByteMap[leadbyte] == true)
                                {
                                    for (uint secondbyte = 0; secondbyte<256; secondbyte++)
                                    {
                                        char c = (char)((leadbyte << 8) + secondbyte);
                                        CheckCodePageGlyph(CodePage, c, ref nTotalChars, ref nMissingChars, arrMissingChars, fontOwner);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.Assert(false);
                        }

                        if (bBitSet)
                        {
                            if (nMissingChars != 0)
                            {
                                string sDetails = "bit #" + nBit + ", " + sName;
                                int n = 0;
                                if (nMissingChars <=10)
                                {
                                    sDetails += " (missing chars:";
                                    n = nMissingChars;
                                }
                                else
                                {
                                    sDetails += " (" + nMissingChars + " missing, first ten missing chars are:";
                                    n = 10;
                                }
                                for (int i=0; i<n; i++)
                                {
                                    sDetails += " U" + arrMissingChars[i].ToString("X4");
                                }
                                sDetails += ")";
                                v.Warning(T.T_NULL, W.OS_2_W_CodePageRangeBitSet, m_tag, sDetails);
                            }
                        }
                        else
                        {
                            if (nMissingChars == 0)
                            {
                                v.Warning(T.T_NULL, W.OS_2_W_CodePageRangeBitClear, m_tag, "bit #" + nBit + ", " + sName);
                            }
                        }
                    }
                    else
                    {
                        v.ApplicationError(T.T_NULL, E.OS_2_A_CodePageNotInstalled, m_tag, "CodePage " + CodePage + " is not installed on the system");
                    }
                }
                catch (Exception e)
                {
                    v.ApplicationError(T.T_NULL, E.OS_2_A_CodePageNotInstalled, m_tag, "CodePage " + CodePage + " throws an exception:" + e.Message);
                }
            }

            return bRet;
        }

        private void CheckCodePageGlyph(uint CodePage, char c, ref int nTotalChars, ref int nMissingChars, ushort [] arrMissingChars, OTFont fontOwner)
        {
            int unicode = MultiByte.MultiByteCharToUnicodeChar(CodePage, c);
            if (unicode != -1)
            {
                if (    !Char.IsControl((char)unicode) && 
                    (unicode < 0xe000 || unicode > 0xf8ff) // outside private use area
                    )
                {
                    nTotalChars++;
                    if (fontOwner.FastMapUnicodeToGlyphID((char)unicode) == 0)
                    {
                        nMissingChars++;

                        if (nMissingChars <= 10)
                        {
                            arrMissingChars[nMissingChars-1] = (ushort)unicode;
                        }
                    }
                }
            }
        }


    }
}
