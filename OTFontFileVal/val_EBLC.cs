using System;
using System.Diagnostics;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_EBLC.
    /// </summary>
    public class val_EBLC : Table_EBLC, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_EBLC(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;
            bool bNumSizesOkay = true;

            Table_EBDT EBDTTable = (Table_EBDT)fontOwner.GetTable("EBDT");

            
            if (v.PerformTest(T.EBLC_version))
            {
                if (version.GetUint() == 0x00020000 || version.GetUint() == 0x00030000)
                {
                    v.Pass(T.EBLC_version, P.EBLC_P_version, m_tag);
                }
                else
                {
                    v.Error(T.EBLC_version, E.EBLC_E_version, m_tag, "version = 0x" + version.GetUint().ToString("x8") + ", unable to continue validation");
                    return false;
                }
            }
            // TODO: check tag and warn EBLC v3, CBLC v2, bloc

            if (v.PerformTest(T.EBLC_numSizes))
            {
                if (numSizes < (m_bufTable.GetLength() / 48))
                {
                    v.Pass(T.EBLC_numSizes, P.EBLC_P_numSizes, m_tag);
                }
                else
                {
                    v.Error(T.EBLC_numSizes, E.EBLC_E_numSizes, m_tag, "0x" + numSizes.ToString("x8"));
                    bNumSizesOkay = false;
                    bRet = false;
                }
            }

            if (v.PerformTest(T.EBLC_TableDependency))
            {
                if (EBDTTable != null)
                {
                    v.Pass(T.EBLC_TableDependency, P.EBLC_P_TableDependency, m_tag);
                }
                else
                {
                    v.Error(T.EBLC_TableDependency, E.EBLC_E_TableDependency, m_tag);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.EBLC_indexSubTableArrayOffset) && bNumSizesOkay == true)
            {
                bool bOffsetsOk = true;
                bool bIndicesOk = true;

                uint SmallestPossibleOffset = 8 + numSizes*48; // sizeof header + sizeof bitmapSizeTAbles
                for (uint i=0; i<numSizes; i++)
                {
                    bitmapSizeTable bst = GetBitmapSizeTable(i);

                    // start must be less than or equal to end
                    if ( bst.startGlyphIndex > bst.endGlyphIndex)
                    {
                        string s = "index = " + i + ", start index = " + bst.startGlyphIndex + ", stop index = " + bst.endGlyphIndex;
                        v.Error(T.EBLC_SizeTableIndexOrder, E.EBLC_E_BitmapSizeTableIndexOrder, m_tag, s);
                        bIndicesOk = false;
                        bRet = false;
                    }

                    if (   bst.indexSubTableArrayOffset < SmallestPossibleOffset
                        || bst.indexSubTableArrayOffset + bst.indexTablesSize > GetLength())
                    {
                        string s = "index = " + i + ", indexSubTableArrayOffset = " + bst.indexSubTableArrayOffset 
                            + ", indexTablesSize = " + bst.indexTablesSize;
                        v.Error(T.EBLC_indexSubTableArrayOffset, E.EBLC_E_indexSubTableArrayOffset, m_tag, s);
                        bOffsetsOk = false;
                        bRet = false;
                    }
                    else
                    {
                        indexSubTableArray[] ista = GetIndexSubTableArray(bst);

                        for (uint j=0; j < bst.numberOfIndexSubTables; j++)
                        {
                            // first must be less than or equal to last
                            if ( ista[j].firstGlyphIndex > ista[j].lastGlyphIndex)
                            {
                            }

                            // subtable indices must be within size table range
                            if (   ista[j].firstGlyphIndex < bst.startGlyphIndex
                                || ista[j].lastGlyphIndex > bst.endGlyphIndex)
                            {
                            }
                        }
                    }
                }

                if (bOffsetsOk)
                {
                    v.Pass(T.EBLC_indexSubTableArrayOffset, P.EBLC_P_indexSubTableArrayOffset, m_tag);
                }

                if (bIndicesOk)
                {
                    v.Pass(T.EBLC_SizeTableIndexOrder, P.EBLC_P_BitmapSizeTableIndexOrder, m_tag);
                }
            }

            if (v.PerformTest(T.EBLC_bitDepth) && bNumSizesOkay == true)
            {
                bool bBitDepthOk = true;

                for (uint i=0; i<numSizes; i++)
                {
                    bitmapSizeTable bst = GetBitmapSizeTable(i);
                    if (bst.bitDepth != 1 && bst.bitDepth != 2 && bst.bitDepth != 4 && bst.bitDepth != 8
                        && !(version.GetUint() == 0x00030000 && bst.bitDepth == 32) )
                    {
                        string s = "index = " + i + ", bitDepth = " + bst.bitDepth;
                        v.Error(T.EBLC_bitDepth, E.EBLC_E_bitDepth, m_tag, s);
                        bBitDepthOk = false;
                        bRet = false;
                    }
                }

                if (bBitDepthOk)
                {
                    v.Pass(T.EBLC_bitDepth, P.EBLC_P_bitDepth, m_tag);
                }
            }

            if (v.PerformTest(T.EBLC_indexSubTables) && bNumSizesOkay == true)
            {

                for (uint i=0; i<numSizes; i++)
                {
                    bitmapSizeTable bst = GetBitmapSizeTable(i);
                    string sSize = "bitmapsize[" + i + "], ppemX=" + bst.ppemX + ", ppemY=" + bst.ppemY;

                    indexSubTableArray[] ista = GetIndexSubTableArray(bst);

                    for (uint j=0; j < bst.numberOfIndexSubTables; j++)
                    {
                        indexSubTable ist = bst.GetIndexSubTable(ista[j]);
                        string sID = sSize + ", indexSubTable[" + j + "](fmt " + ist.header.indexFormat + ")";

                        if (!Validate_indexSubTable(v, ist, ista[j], sID, fontOwner))
                        {
                            bRet = false;
                        }
                    }
                }
            }

            return bRet;
        }

        bool Validate_indexSubTable(Validator v, indexSubTable ist, indexSubTableArray ista, string sID, OTFontVal fontOwner)
        {
            bool bOk = true;

            if (!Validate_indexSubHeader(v, ist.header, sID, fontOwner))
            {
                bOk = false;
            }

            switch (ist.header.indexFormat)
            {
                case 1:
                    if (!Validate_indexSubTable_format1(v, (indexSubTable1)ist, ista, sID, fontOwner))
                    {
                        bOk = false;
                    }
                    break;

                case 2:
                    if (!Validate_indexSubTable_format2(v, (indexSubTable2)ist, ista, sID, fontOwner))
                    {
                        bOk = false;
                    }
                    break;

                case 3:
                    if (!Validate_indexSubTable_format3(v, (indexSubTable3)ist, ista, sID, fontOwner))
                    {
                        bOk = false;
                    }
                    break;

                case 4:
                    if (!Validate_indexSubTable_format4(v, (indexSubTable4)ist, ista, sID, fontOwner))
                    {
                        bOk = false;
                    }
                    break;

                case 5:
                    if (!Validate_indexSubTable_format5(v, (indexSubTable5)ist, ista, sID, fontOwner))
                    {
                        bOk = false;
                    }
                    break;

                default:
                    Debug.Assert(false, "illegal index format", "format = " + ist.header.indexFormat);
                    break;
            }

            if (bOk)
            {
                v.Pass(T.EBLC_indexSubTables, P.EBLC_P_indexSubTables, m_tag, sID);
            }

            return bOk;
        }
        
        bool Validate_indexSubHeader(Validator v, indexSubHeader iSH, string sID, OTFontVal fontOwner)
        {
            bool bOk = true;

            Table_EBDT EBDTTable = (Table_EBDT)fontOwner.GetTable("EBDT");

            if (iSH.indexFormat < 1 || iSH.indexFormat > 5)
            {
                string sDetails = "invalid indexFormat: " + sID + ", indexFormat = " + iSH.indexFormat;
                v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                bOk = false;
            }

            if ( (iSH.imageFormat < 1 || iSH.imageFormat > 9 || iSH.imageFormat == 3)
                && !(version.GetUint() == 0x00030000 &&
                     (iSH.imageFormat == 17 || iSH.imageFormat == 18 || iSH.imageFormat == 19)
                     ) )
            {
                string sDetails = "invalid imageFormat: " + sID + ", imageFormat = " + iSH.imageFormat;
                v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                bOk = false;
            }

            bool bInconsistentFormats = false;
            if (iSH.indexFormat == 1 || iSH.indexFormat == 3 || iSH.indexFormat == 4)
            {
                if (iSH.imageFormat == 5)
                {
                    bInconsistentFormats = true;
                }
            }
            else
            {
                if (iSH.imageFormat != 5)
                {
                    bInconsistentFormats = true;
                }
            }
            if (bInconsistentFormats)
            {
                string sDetails = "inconsistent formats: " + sID + ", indexFormat = " + iSH.indexFormat + ", imageFormat = " + iSH.imageFormat;
                v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                bOk = false;
            }

            if (EBDTTable != null)
            {
                if (iSH.imageDataOffset > EBDTTable.GetLength())
                {
                    string sDetails = "invalid imageDataOffset: " + sID + ", imageDataOffset = " + iSH.imageDataOffset;
                    v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                    bOk = false;
                }
            }

            return bOk;
        }

        bool Validate_indexSubTable_format1(Validator v, indexSubTable1 ist1, indexSubTableArray ista, string sID, OTFontVal fontOwner)
        {
            bool bOk = true;

            Table_EBDT EBDTTable = (Table_EBDT)fontOwner.GetTable("EBDT");

            // header has already been validated

            // validate the array of offsets
            uint nEBDTLength = EBDTTable.GetLength();
            int nArrSize = ista.lastGlyphIndex - ista.firstGlyphIndex + 1;
            for (ushort i=0; i<nArrSize; i++)
            {
                uint offsetImage = ist1.GetOffset(i) + ist1.header.imageDataOffset;
                if (offsetImage > nEBDTLength)
                {
                    string sDetails = "invalid offset: " + sID + ", offset[" + i + "] = " + offsetImage
                        + ", EBDT length = " + EBDTTable.GetLength();
                    v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                    bOk = false;
                }
            }

            return bOk;
        }


        bool Validate_indexSubTable_format2(Validator v, indexSubTable2 ist2, indexSubTableArray ista, string sID, OTFontVal fontOwner)
        {
            bool bOk = true;

            Table_EBDT EBDTTable = (Table_EBDT)fontOwner.GetTable("EBDT");

            // header has already been validated


            // validate the image size

            uint nGlyphsInRange = (uint)ista.lastGlyphIndex - ista.firstGlyphIndex + 1;
            if (nGlyphsInRange*ist2.imageSize > EBDTTable.GetLength())
            {
                string sDetails = "images extend past end of EBDT table: " + sID + ", imageSize = " + ist2.imageSize 
                    + ", EBDT length = " + EBDTTable.GetLength();
                v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                bOk = false;
            }

            // validate the bigGlyphMetrics
            val_EBDT.bigGlyphMetrics_val bgm = val_EBDT.bigGlyphMetrics_val.CreateFromBigGlyphMetrics(ist2.bigMetrics);
            bgm.Validate(v, sID, this);

            return bOk;
        }


        bool Validate_indexSubTable_format3(Validator v, indexSubTable3 ist3, indexSubTableArray ista, string sID, OTFontVal fontOwner)
        {
            bool bOk = true;

            Table_EBDT EBDTTable = (Table_EBDT)fontOwner.GetTable("EBDT");

            // header has already been validated


            // validate the array of offsets

            int nArrSize = ista.lastGlyphIndex - ista.firstGlyphIndex + 1;
            uint nEBDTLength = EBDTTable.GetLength();
            for (ushort i=0; i<nArrSize; i++)
            {
                uint offsetImage = ist3.GetOffset(i) + ist3.header.imageDataOffset;
                if (offsetImage > nEBDTLength)
                {
                    string sDetails = "invalid offset: " + sID + ", offset[" + i + "] = " + offsetImage +
                        ", EBDT length = " + nEBDTLength;
                    v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                    bOk = false;
                }
            }

            return bOk;
        }


        bool Validate_indexSubTable_format4(Validator v, indexSubTable4 ist4, indexSubTableArray ista, string sID, OTFontVal fontOwner)
        {
            bool bOk = true;

            Table_EBDT EBDTTable = (Table_EBDT)fontOwner.GetTable("EBDT");

            // header has already been validated


            // validate numGlyphs

            uint nGlyphsInRange = (uint)ista.lastGlyphIndex - ista.firstGlyphIndex + 1;
            if (ist4.numGlyphs > nGlyphsInRange)
            {
                string sDetails = "numGlyphs is invalid: " + sID + 
                        ", numGlyphs = " + ist4.numGlyphs + 
                        ", indexSubTableArray.firstGlyphIndex = " + ista.firstGlyphIndex +
                        ", indexSubTableArray.lastGlyphIndex = " + ista.lastGlyphIndex;
                v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                bOk = false;
            }


            // validate code offset pairs
            uint nEBDTLength = EBDTTable.GetLength();
            for (ushort idGlyph=0; idGlyph<ist4.numGlyphs; idGlyph++)
            {
                indexSubTable4.codeOffsetPair cop = ist4.GetCodeOffsetPair(idGlyph);

                // glyph code
                if (cop.glyphCode < ista.firstGlyphIndex || cop.glyphCode > ista.lastGlyphIndex)
                {
                    string sDetails = "glyph code is invalid: " + sID + 
                        ", codeOffsetPair[" + idGlyph + "]" + 
                        ", indexSubTableArray.firstGlyphIndex = " + ista.firstGlyphIndex +
                        ", indexSubTableArray.lastGlyphIndex = " + ista.lastGlyphIndex;
                    v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                    bOk = false;
                }

                // offset
                if (cop.offset > nEBDTLength)
                {
                    string sDetails = "invalid offset: " + sID + 
                        ", codeOffsetPair[" + idGlyph + "].offset = " + cop.offset +
                        ", EBDT length = " + nEBDTLength;
                    v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                    bOk = false;
                }
            }
            
            return bOk;
        }


        bool Validate_indexSubTable_format5(Validator v, indexSubTable5 ist5, indexSubTableArray ista, string sID, OTFontVal fontOwner)
        {
            bool bOk = true;

            Table_EBDT EBDTTable = (Table_EBDT)fontOwner.GetTable("EBDT");

            // header has already been validated


            // validate the image size

            if (ist5.numGlyphs*ist5.imageSize > EBDTTable.GetLength())
            {
                string sDetails = "images extend past end of EBDT table: " + sID + ", imageSize = " + ist5.imageSize 
                    + ", EBDT length = " + EBDTTable.GetLength();
                v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                bOk = false;
            }


            // validate the bigGlyphMetrics

            val_EBDT.bigGlyphMetrics_val bgm = val_EBDT.bigGlyphMetrics_val.CreateFromBigGlyphMetrics(ist5.bigMetrics);
            bgm.Validate(v, sID, this);


            // validate numGlyphs

            uint nGlyphsInRange = (uint)ista.lastGlyphIndex - ista.firstGlyphIndex + 1;
            if (ist5.numGlyphs > nGlyphsInRange)
            {
                string sDetails = "numGlyphs is invalid: " + sID + 
                    ", numGlyphs = " + ist5.numGlyphs + 
                    ", indexSubTableArray.firstGlyphIndex = " + ista.firstGlyphIndex +
                    ", indexSubTableArray.lastGlyphIndex = " + ista.lastGlyphIndex;
                v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                bOk = false;
            }

            
            // validate glyphCodeArray

            for (ushort i=0; i<ist5.numGlyphs; i++)
            {
                ushort idGlyph = ist5.GetGlyphCode(i);
                if (idGlyph < ista.firstGlyphIndex || idGlyph > ista.lastGlyphIndex)
                {
                    string sDetails = "invalid glyph id: " + sID + 
                        ", glyphCodeArray[" + i + "] = " + idGlyph +
                        ", indexSubTableArray.firstGlyphIndex = " + ista.firstGlyphIndex +
                        ", indexSubTableArray.lastGlyphIndex = " + ista.lastGlyphIndex;
                    v.Error(T.EBLC_indexSubTables, E.EBLC_E_indexSubTables, m_tag, sDetails);
                    bOk = false;
                }
            }

            return bOk;
        }
    }
}
