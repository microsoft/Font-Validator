using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_kern.
    /// </summary>
    public class val_kern : Table_kern, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_kern(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.kern_TableVersion))
            {
                if (version == 0)
                {
                    v.Pass(T.kern_TableVersion, P.kern_P_TableVersion, m_tag);
                }
                else
                {
                    v.Error(T.kern_TableVersion, E.kern_E_TableVersion, m_tag, "version = " + version.ToString() + ", unrecognized version #, no further tests can be performed");
                    return false;
                }
            }



            if (v.PerformTest(T.kern_NumSubtables))
            {
                if (nTables != 0)
                {
                    v.Pass(T.kern_NumSubtables, P.kern_P_NumSubTables, m_tag, nTables.ToString());
                }
                else
                {
                    v.Error(T.kern_NumSubtables, E.kern_E_NumSubTables, m_tag);
                    bRet = false;
                }
            }


            if (v.PerformTest(T.kern_SubtableFormats))
            {
                bool bFormatsOk = true;

                for (uint i=0; i<nTables; i++)
                {
                    SubTableHeader sth = GetSubTableHeader(i);
                    if (sth != null)
                    {
                        if (sth.GetFormat() != 0 && sth.GetFormat() != 2)
                        {
                            v.Error(T.kern_SubtableFormats, E.kern_E_SubtableFormats, m_tag, "subtable #" + i + ", format " + sth.GetFormat());
                            bFormatsOk = false;
                            bRet = false;
                        }
                    }
                    else
                    {
                        v.Error(T.kern_SubtableFormats, E.kern_E_SubTableExtendsPastEOT, m_tag, "subtable #" + i );
                        bFormatsOk = false;
                        bRet = false;
                        break;
                    }
                }
                if (bFormatsOk)
                {
                    v.Pass(T.kern_SubtableFormats, P.kern_P_SubtableFormats, m_tag);
                }
            }

            if (!bRet)
            {
                v.Warning(T.kern_SubtableFormats, W._TEST_W_OtherErrorsInTable, m_tag, "kern table appears to be corrupt.  No further tests will be performed.");
                return bRet;
            }

            if (v.PerformTest(T.kern_SubtableLength))
            {
                bool bLengthsOk = true;

                for (uint i=0; i<nTables; i++)
                {
                    SubTable st = this.GetSubTable(i);
                    if (st.length != st.CalculatedLength())
                    {
                        v.Error(T.kern_SubtableLength, E.kern_E_SubtableLength, m_tag, "subtable #" + i + ", length = " + st.length + ", calculated length = " + st.CalculatedLength());
                        bLengthsOk = false;
                        bRet = false;
                    }
                }
                if (bLengthsOk)
                {
                    v.Pass(T.kern_SubtableLength, P.kern_P_SubtableLengths, m_tag);
                }
            }

            if (v.PerformTest(T.kern_CoverageReservedBits))
            {
                bool bReservedOk = true;

                for (uint i=0; i<nTables; i++)
                {
                    SubTable st = this.GetSubTable(i);
                    if ((st.coverage & 0xf0)  != 0)
                    {
                        bReservedOk = false;
                        v.Error(T.kern_CoverageReservedBits, E.kern_E_ReservedCoverageBits, m_tag, "subtable #" + i);
                        bRet = false;
                        break;
                    }
                }
                if (bReservedOk)
                {
                    v.Pass(T.kern_CoverageReservedBits, P.kern_P_ReservedCoverageBits, m_tag);
                }
            }

            if (v.PerformTest(T.kern_Format0_SearchFields))
            {
                bool bBinaryFieldsOk = true;

                for (uint i=0; i<nTables; i++)
                {
                    SubTable st = this.GetSubTable(i);
                    if (st.version == 0)
                    {
                        SubTableFormat0 stf0 = (SubTableFormat0)st;
                        ushort nPairs = stf0.nPairs;
                        ushort sizeofEntry = 6;

                        if (nPairs != 0)
                        {
                            ushort CalculatedSearchRange   = (ushort)(util.MaxPower2LE(nPairs) * sizeofEntry);
                            ushort CalculatedEntrySelector = util.Log2(util.MaxPower2LE(nPairs));
                            ushort CalculatedRangeShift    = (ushort)((nPairs - util.MaxPower2LE(nPairs))* sizeofEntry);

                            if (stf0.searchRange != CalculatedSearchRange)
                            {
                                string s =  "subtable #" + i 
                                    + ", calc = " + CalculatedSearchRange 
                                    + ", actual = " + stf0.searchRange;
                                v.Error(T.kern_Format0_SearchFields, E.kern_E_Format0_searchRange, m_tag, s);
                                bBinaryFieldsOk = false;
                                bRet = false;
                            }
                            if (stf0.entrySelector != CalculatedEntrySelector)
                            {
                                string s =  "subtable #" + i 
                                    + ", calc = " +  CalculatedEntrySelector
                                    + ", actual = " + stf0.entrySelector;
                                v.Error(T.kern_Format0_SearchFields, E.kern_E_Format0_entrySelector, m_tag, s);
                                bBinaryFieldsOk = false;
                                bRet = false;
                            }
                            if (stf0.rangeShift != CalculatedRangeShift)
                            {
                                string s =  "subtable #" + i 
                                    + ", calc = " +  CalculatedRangeShift
                                    + ", actual = " + stf0.rangeShift;
                                v.Error(T.kern_Format0_SearchFields, E.kern_E_Format0_rangeShift, m_tag, s);
                                bBinaryFieldsOk = false;
                                bRet = false;
                            }
                        }
                        else
                        {
                            // cannot validate fields since they are undefined when nPairs is zero
                            v.Warning(T.kern_Format0_SearchFields, W.kern_W_Format0_SearchFields, m_tag, "subtable #" + i + ", nPairs = " + stf0.nPairs);
                            bBinaryFieldsOk = false;
                        }
                    }
                }

                if (bBinaryFieldsOk)
                {
                    v.Pass(T.kern_Format0_SearchFields, P.kern_P_Format0_SearchFields, m_tag);
                }
            }

            if (v.PerformTest(T.kern_Format0_GlyphIDs))
            {
                for (uint i=0; i<nTables; i++)
                {
                    SubTable st = this.GetSubTable(i);
                    if (st.version == 0)
                    {
                        bool bGlyphIDsOk = true;

                        SubTableFormat0 stf0 = (SubTableFormat0)st;

                        for (int iPair=0; iPair<stf0.nPairs; iPair++)
                        {
                            ushort left=0, right=0;
                            short kernvalue=0;

                            stf0.GetKerningPairAndValue(iPair, ref left, ref right, ref kernvalue);

                            ushort numGlyphs = fontOwner.GetMaxpNumGlyphs();
                            if (left >= numGlyphs)
                            {
                                v.Error(T.kern_Format0_GlyphIDs, E.kern_E_Format0_GlyphIDs, m_tag, "kern pair[" + iPair + "]: left = " + left);
                                bGlyphIDsOk = false;
                                bRet = false;
                            }
                            if (right >= numGlyphs)
                            {
                                v.Error(T.kern_Format0_GlyphIDs, E.kern_E_Format0_GlyphIDs, m_tag, "kern pair[" + iPair + "]: right = " + right);
                                bGlyphIDsOk = false;
                                bRet = false;
                            }
                        }

                        if (bGlyphIDsOk)
                        {
                            v.Pass(T.kern_Format0_GlyphIDs, P.kern_P_Format0_GlyphIDs, m_tag);
                        }
                    }
                }

            }

            if (v.PerformTest(T.kern_Format0_Values))
            {
                for (uint i=0; i<nTables; i++)
                {
                    SubTable st = this.GetSubTable(i);
                    if (st.version == 0)
                    {
                        SubTableFormat0 stf0 = (SubTableFormat0)st;

                        Table_hmtx hmtxTable = (Table_hmtx)fontOwner.GetTable("hmtx");
                        if (hmtxTable != null)
                        {

                            bool bValuesOk = true;

                            for (int iPair=0; iPair<stf0.nPairs; iPair++)
                            {
                                ushort left=0, right=0;
                                short kernvalue=0;

                                stf0.GetKerningPairAndValue(iPair, ref left, ref right, ref kernvalue);
                                short absKernValue = Math.Abs(kernvalue);

                                Table_hmtx.longHorMetric lhmLeft = hmtxTable.GetOrMakeHMetric(left, fontOwner);
                                Table_hmtx.longHorMetric lhmRight = hmtxTable.GetOrMakeHMetric(right, fontOwner);
                            
                                if (absKernValue > lhmLeft.advanceWidth && absKernValue > lhmRight.advanceWidth)
                                {
                                    v.Error(T.kern_Format0_Values, E.kern_E_Format0_Values, m_tag, "kern pair[" + iPair + "]: left id = " + left + ", right id = " + right + ", value = " + kernvalue);
                                    bValuesOk=false;
                                    bRet = false;
                                }
                            }

                            if (bValuesOk)
                            {
                                v.Pass(T.kern_Format0_Values, P.kern_P_Format0_Values, m_tag);
                            }
                        }
                    }
                }

            }

            if (v.PerformTest("kern_Format0_IDsInCmap"))
            {
                for (uint i=0; i<nTables; i++)
                {
                    SubTable st = this.GetSubTable(i);
                    if (st.version == 0)
                    {
                        SubTableFormat0 stf0 = (SubTableFormat0)st;

                        Table_cmap cmapTable = (Table_cmap)fontOwner.GetTable("cmap");
                        if (cmapTable != null)
                        {
                            Table_cmap.Subtable cmapSubtable = cmapTable.GetSubtable(3,10);
                            if (cmapSubtable == null)
                            {
                                cmapSubtable = cmapTable.GetSubtable(3,1);
                            }
                            if (cmapSubtable == null)
                            {
                                Table_cmap.EncodingTableEntry ete = cmapTable .GetEncodingTableEntry(0);
                                cmapSubtable = cmapTable.GetSubtable(ete);
                            }

                            if (cmapSubtable != null)
                            {
                                uint [] map = cmapSubtable.GetMap();

                                bool bAllIDsInCmap = true;

                                for (int iPair=0; iPair<stf0.nPairs; iPair++)
                                {
                                    ushort left=0, right=0;
                                    short kernvalue=0;

                                    stf0.GetKerningPairAndValue(iPair, ref left, ref right, ref kernvalue);
                                    
                                    bool bFoundLeft = false;
                                    bool bFoundRight = false;

                                    for (int j=0; j<map.Length; j++)
                                    {
                                        if (left == map[j])
                                        {
                                            bFoundLeft = true;
                                            break;
                                        }
                                    }

                                    for (int j=0; j<map.Length; j++)
                                    {
                                        if (right == map[j])
                                        {
                                            bFoundRight = true;
                                            break;
                                        }
                                    }

                                    if (bFoundLeft==false)
                                    {
                                        v.Error(T.kern_Format0_IDsInCmap, E.kern_E_Format0_GlyphIdInCmap, m_tag, "kern pair[" + iPair + "]: left id = " + left);
                                        bAllIDsInCmap = false;
                                        bRet = false;
                                    }
                                    if (bFoundRight==false)
                                    {
                                        v.Error(T.kern_Format0_IDsInCmap, E.kern_E_Format0_GlyphIdInCmap, m_tag, "kern pair[" + iPair + "]: right id = " + right);
                                        bAllIDsInCmap = false;
                                        bRet = false;
                                    }
                                }

                                if (bAllIDsInCmap)
                                {
                                    v.Pass(T.kern_Format0_IDsInCmap, P.kern_P_Format0_GlyphIdInCmap, m_tag);
                                }
                            }

                        }
                    }
                }
            }

            return bRet;
        }


    }
}
