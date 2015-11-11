using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_hhea.
    /// </summary>
    public class val_hhea : Table_hhea, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_hhea(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }



        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.hhea_version))
            {
                if (TableVersionNumber.GetUint() == 0x00010000)
                {
                    v.Pass(T.hhea_version, P.hhea_P_version, m_tag);
                }
                else
                {
                    v.Error(T.hhea_version, E.hhea_E_version, m_tag, "0x"+TableVersionNumber.GetUint().ToString("x8"));
                    bRet = false;
                }
            }

            if (v.PerformTest(T.hhea_AscenderPositive))
            {
                if (Ascender <= 0)
                {
                    string s = "Ascender = " + Ascender;
                    v.Error(T.hhea_AscenderPositive, E.hhea_E_AscenderPositive, m_tag, s);
                    bRet = false;
                }
                else
                {
                    v.Pass(T.hhea_AscenderPositive, P.hhea_P_AscenderPositive, m_tag);
                }
            }

            if (v.PerformTest(T.hhea_DescenderNegative))
            {
                if (Descender >= 0)
                {
                    string s = "Descender = " + Descender;
                    v.Error(T.hhea_DescenderNegative, E.hhea_E_DescenderNegative, m_tag, s);
                    bRet = false;
                }
                else
                {
                    v.Pass(T.hhea_DescenderNegative, P.hhea_P_DescenderNegative, m_tag);
                }
            }

            Table_head headTable = (Table_head)fontOwner.GetTable("head");

            if (headTable != null)
            {
                if (v.PerformTest(T.hhea_Ascender_yMax))
                {
                    if (Ascender > headTable.yMax)
                    {
                        string s = "Ascender = " + Ascender + ", head.yMax = " + headTable.yMax;
                        v.Info(T.hhea_Ascender_yMax, I.hhea_I_Ascender_yMax, m_tag, s);
                        // bRet = false;
                    }
                    else
                    {
                        v.Pass(T.hhea_Ascender_yMax, P.hhea_P_Ascender_yMax, m_tag);
                    }
                }

                if (v.PerformTest(T.hhea_Descender_yMin))
                {
                    if (Descender < headTable.yMin)
                    {
                        string s = "Descender = " + Descender + ", head.yMin = " + headTable.yMin;
                        v.Info(T.hhea_Descender_yMin, I.hhea_I_Descender_yMin, m_tag, s);
                        // bRet = false;
                    }
                    else
                    {
                        v.Pass(T.hhea_Descender_yMin, P.hhea_P_Descender_yMin, m_tag);
                    }
                }
            }
            else
            {
                v.Error(T.hhea_Ascender_yMax, E._TEST_E_TableMissing, m_tag, "head");
				bRet = false;
            }

            if (v.PerformTest(T.hhea_LineGapPositive))
            {
                if (LineGap < 0)
                {
                    string s = "LineGap = " + LineGap;
                    v.Warning(T.hhea_LineGapPositive, W.hhea_W_LineGapPositive, m_tag, s);
                    //bRet = false;
                }
                else
                {
                    v.Pass(T.hhea_LineGapPositive, P.hhea_P_LineGapPositive, m_tag);
                }
            }

            Table_OS2 OS2Table = (Table_OS2)fontOwner.GetTable("OS/2");
            if (OS2Table != null)
            {

                if (v.PerformTest(T.hhea_Ascender_usWinAscent))
                {
                    if (OS2Table.usWinAscent != Ascender)
                    {
                        string s = "hhea.Ascender = " + Ascender + ", OS/2.usWinAscent = " + OS2Table.usWinAscent;
                        v.Warning(T.hhea_Ascender_usWinAscent, W.hhea_W_Ascender_usWinAscent, m_tag, s);
                    }
                    else
                    {
                        v.Pass(T.hhea_Ascender_usWinAscent, P.hhea_P_Ascender_usWinAscent, m_tag);
                    }
                }

                if (v.PerformTest(T.hhea_Descender_usWinDescent))
                {
                    if (OS2Table.usWinDescent != (Descender * -1))
                    {
                        string s = "hhea.Descender = " + Descender + ", OS/2.usWinDescent = " + OS2Table.usWinDescent;
                        v.Warning(T.hhea_Descender_usWinDescent, W.hhea_W_Descender_usWinDescent, m_tag, s);
                    }
                    else
                    {
                        v.Pass(T.hhea_Descender_usWinDescent, P.hhea_P_Descender_usWinDescent, m_tag);
                    }
                }

                // Microsoft is recommending that Ascender, Descender and LineGap be in line with OS2.winAscent, OS2.winDescent
                // and formulated value for LineGap to make sure font displays the same on Apple as it does no Windows. 
                if (v.PerformTest(T.hhea_LineGap_minGap))
                {
                    int sMinGap = (OS2Table.usWinAscent + OS2Table.usWinDescent) - (Ascender - Descender);
                    if (LineGap < sMinGap)
                    {
                        string s = "LineGap = " + LineGap + ", recommended = " + sMinGap;
                        v.Warning(T.hhea_LineGap_minGap, W.hhea_W_LineGap_minGap, m_tag, s);
                        //bRet = false;
                    }
                    else
                    {
                        v.Pass(T.hhea_LineGap_minGap, P.hhea_P_LineGap_minGap, m_tag);
                    }
                }
                else
                {
                    v.Error(T.hhea_LineGap_minGap, E._TEST_E_TableMissing, m_tag, "OS/2");
                }

            }
            else
            {
                v.Error(T.hhea_Ascender_usWinAscent, E._TEST_E_TableMissing, m_tag, "OS/2");
                v.Error(T.hhea_Descender_usWinDescent, E._TEST_E_TableMissing, m_tag, "OS/2");
                v.Error(T.hhea_LineGap_minGap, E._TEST_E_TableMissing, m_tag, "OS/2");
				bRet = false;
            }

            if (v.PerformTest(T.hhea_MinMax))
            {
                if (fontOwner.ContainsTrueTypeOutlines())
                {
                    Table_hmtx hmtxTable = (Table_hmtx)fontOwner.GetTable("hmtx");
                    Table_glyf glyfTable = (Table_glyf)fontOwner.GetTable("glyf");
                    Table_maxp maxpTable = (Table_maxp)fontOwner.GetTable("maxp");
                    if (hmtxTable == null)
                    {
                        v.Error(T.hhea_MinMax, E._TEST_E_TableMissing, m_tag, "hmtx");
						bRet = false;
                    }
                    else if (glyfTable == null)
                    {
                        v.Error(T.hhea_MinMax, E._TEST_E_TableMissing, m_tag, "glyf");
						bRet = false;
                    }
                    else if (maxpTable == null)
                    {
                        v.Error(T.hhea_MinMax, E._TEST_E_TableMissing, m_tag, "maxp");
						bRet = false;
                    }
                    else
                    {
                        ushort numGlyphs = fontOwner.GetMaxpNumGlyphs();

                        ushort awMax = 0;
                        short minLSB = 32767;
                        short minRSB = 32767;
                        short xmaxEx = -32768;

                        Table_hmtx.longHorMetric hm = null;

                        for (uint iGlyph = 0; iGlyph < numGlyphs; iGlyph++)
                        {
                            hm = hmtxTable.GetOrMakeHMetric(iGlyph, fontOwner);
                            if (hm == null)
                            {
                                break;
                            }

                            if (awMax  < hm.advanceWidth)
                            {
                                awMax  = hm.advanceWidth;

                                // We want to give the user feedback to know what glyph id
                                // has the bad width assigned
                                if (awMax > advanceWidthMax)
                                {
                                    string s = "glyph ID = " + iGlyph + ", advance width = " + awMax;
                                    v.Error(T.hhea_MinMax, E.hhea_E_advanceWidthMax, m_tag, s);
									bRet = false;
                                }
                            }

                            Table_glyf.header gh = glyfTable.GetGlyphHeader(iGlyph, fontOwner);
                            // calculate for all non-zero contours...this includes composites
                            if (gh != null
                                && gh.numberOfContours != 0)
                            {
                                if (minLSB > hm.lsb) minLSB = hm.lsb;

                                short rsb = (short)(hm.advanceWidth - hm.lsb - (gh.xMax - gh.xMin));
                                if (minRSB > rsb) minRSB = rsb;

                                short extent = (short)(hm.lsb + (gh.xMax - gh.xMin));
                                if (xmaxEx < extent) xmaxEx = extent;
                            }
                        }


                        if (hm != null)
                        {
                            if (advanceWidthMax == awMax)
                            {
                                v.Pass(T.hhea_MinMax, P.hhea_P_advanceWidthMax, m_tag);
                            }
                            else
                            {
                                string s = "actual = " + advanceWidthMax + ", calc = " + awMax;
                                v.Error(T.hhea_MinMax, E.hhea_E_advanceWidthMax, m_tag, s);
                                bRet = false;
                            }

                            if (minLeftSideBearing == minLSB)
                            {
                                v.Pass(T.hhea_MinMax, P.hhea_P_minLeftSideBearing, m_tag);
                            }
                            else
                            {
                                string s = "actual = " + minLeftSideBearing + ", calc = " + minLSB;
                                v.Error(T.hhea_MinMax, E.hhea_E_minLeftSideBearing, m_tag, s);
                                bRet = false;
                            }

                            if (minRightSideBearing == minRSB)
                            {
                                v.Pass(T.hhea_MinMax, P.hhea_P_minRightSideBearing, m_tag);
                            }
                            else
                            {
                                string s = "actual = " + minRightSideBearing + ", calc = " + minRSB;
                                v.Error(T.hhea_MinMax, E.hhea_E_minRightSideBearing, m_tag, s);
                                bRet = false;
                            }

                            if (xMaxExtent == xmaxEx)
                            {
                                v.Pass(T.hhea_MinMax, P.hhea_P_xMaxExtent, m_tag);
                            }
                            else
                            {
                                string s = "actual = " + xMaxExtent + ", calc = " + xmaxEx;
                                v.Error(T.hhea_MinMax, E.hhea_E_xMaxExtent, m_tag, s);
                                bRet = false;
                            }
                        }
                        else
                        {
                            v.Warning(T.hhea_MinMax, W.hhea_W_hmtx_invalid, m_tag, "unable to parse hmtx table");
                        }
                    }
                }
                else
                {
                    v.Info(T.hhea_MinMax, I._TEST_I_NotForCFF, m_tag, "test = hhea_MinMax");
                }
            }

            if (v.PerformTest(T.hhea_reserved))
            {
                if (reserved1 != 0)
                {
                    v.Error(T.hhea_reserved, E.hhea_E_reserved1, m_tag, reserved1.ToString());
                    bRet = false;
                }
                else if (reserved2 != 0)
                {
                    v.Error(T.hhea_reserved, E.hhea_E_reserved2, m_tag, reserved2.ToString());
                    bRet = false;
                }
                else if (reserved3 != 0)
                {
                    v.Error(T.hhea_reserved, E.hhea_E_reserved3, m_tag, reserved3.ToString());
                    bRet = false;
                }
                else if (reserved4 != 0)
                {
                    v.Error(T.hhea_reserved, E.hhea_E_reserved4, m_tag, reserved4.ToString());
                    bRet = false;
                }
                else
                {
                    v.Pass(T.hhea_reserved, P.hhea_P_reserved, m_tag);
                }
            }

            if (v.PerformTest(T.hhea_metricDataFormat))
            {
                if (metricDataFormat == 0)
                {
                    v.Pass(T.hhea_metricDataFormat, P.hhea_P_metricDataFormat, m_tag);
                }
                else
                {
                    v.Error(T.hhea_metricDataFormat, E.hhea_E_metricDataFormat, m_tag, metricDataFormat.ToString());
                    bRet = false;
                }
            }

            if (v.PerformTest(T.hhea_numberOfHMetrics))
            {
                Table_hmtx hmtxTable = (Table_hmtx)fontOwner.GetTable("hmtx");
                Table_maxp maxpTable = (Table_maxp)fontOwner.GetTable("maxp");
                if (hmtxTable == null)
                {
                    v.Error(T.hhea_numberOfHMetrics, E._TEST_E_TableMissing, m_tag, "hmtx");
					bRet = false;
                }
                else if (maxpTable == null)
                {
                    v.Error(T.hhea_numberOfHMetrics, E._TEST_E_TableMissing, m_tag, "maxp");
					bRet = false;
                }
                else
                {
                    ushort numGlyphs = fontOwner.GetMaxpNumGlyphs();
                    if (numberOfHMetrics*4 + (numGlyphs-numberOfHMetrics)*2 == hmtxTable.GetLength())
                    {
                        v.Pass(T.hhea_numberOfHMetrics, P.hhea_P_numberOfHMetrics, m_tag);
                    }
                    else
                    {
                        v.Error(T.hhea_numberOfHMetrics, E.hhea_E_numberOfHMetrics, m_tag);
                        bRet = false;
                    }
                }

            }

            if (v.PerformTest(T.hhea_caretSlope))
            {
                bool bSlopeOk = true;

                Table_post postTable = (Table_post)fontOwner.GetTable("post");

                if (postTable != null)
                {
                    uint ia = postTable.italicAngle.GetUint();
                    double dItalicAngle = postTable.italicAngle.GetDouble();

                    if (ia == 0)
                    {
                        if (caretSlopeRun != 0)
                        {
                            v.Error(T.hhea_caretSlope, E.hhea_E_caretSlopeRunNonZero_italicAngle, m_tag);
                            bSlopeOk = false;
                            bRet = false;
                        }
                    }
                    else
                    {
                        if (caretSlopeRun == 0)
                        {
                            v.Error(T.hhea_caretSlope, E.hhea_E_caretSlopeRunZero_italicAngle, m_tag);
                            bSlopeOk = false;
                            bRet = false;
                        }
                        else
                        {
                            double dActualAngle = 90.0 + postTable.italicAngle.GetDouble();
                            double dhheaAngle = (Math.Atan2(caretSlopeRise, caretSlopeRun)) * (180.0 / Math.PI);
                            if (Math.Abs(dActualAngle-dhheaAngle) >= 1.0)
                            {
                                string sDetails = "caretSlope Rise:Run = " + caretSlopeRise + ":" + caretSlopeRun +
                                    " (" + (dhheaAngle-90.0) + " degrees)" +
                                    ", post.italicAngle = 0x" + ia.ToString("x8") +
                                    " (" + postTable.italicAngle.GetDouble() + " degrees)";
                                v.Error(T.hhea_caretSlope, E.hhea_E_caretSlopeAngle_italicAngle, m_tag, sDetails);
                                bSlopeOk = false;
                                bRet = false;
                            }
                        }
                    }
                }
                else
                {
                    v.Error(T.hhea_caretSlope, E._TEST_E_TableMissing, m_tag, "post table missing, can't compare caret slope to italicAngle");
                    bSlopeOk = false;
					bRet = false;
                }


                if (bSlopeOk)
                {
                    v.Pass(T.hhea_caretSlope, P.hhea_P_caretSlopeAngle_italicAngle, m_tag);
                }
            }

            return bRet;
        }



    }
}
