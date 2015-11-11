using System;
using System.Diagnostics;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_EBDT.
    /// </summary>
    public class val_EBDT : Table_EBDT, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_EBDT(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }




        public class bigGlyphMetrics_val : bigGlyphMetrics
        {
            public static bigGlyphMetrics_val CreateFromBigGlyphMetrics(bigGlyphMetrics bgm)
            {
                bigGlyphMetrics_val bgm_val = new bigGlyphMetrics_val();
                bgm_val.height       = bgm.height;
                bgm_val.width        = bgm.width;
                bgm_val.horiBearingX = bgm.horiBearingX;
                bgm_val.horiBearingY = bgm.horiBearingY;
                bgm_val.horiAdvance  = bgm.horiAdvance;
                bgm_val.vertBearingX = bgm.vertBearingX;
                bgm_val.vertBearingY = bgm.vertBearingY;
                bgm_val.vertAdvance  = bgm.vertAdvance;

                return bgm_val;
            }

            public bool Validate(Validator v, string sIdentity, OTTable tableOwner)
            {
                bool bOk = true;

                // ???
                // ??? are there any values that are invalid ???
                // ???

                return bOk;
            }
        }

        public class smallGlyphMetrics_val : smallGlyphMetrics
        {
            public static smallGlyphMetrics_val CreateFromSmallGlyphMetrics(smallGlyphMetrics sgm)
            {
                smallGlyphMetrics_val sgm_val = new smallGlyphMetrics_val();

                sgm_val.height   = sgm.height;
                sgm_val.width    = sgm.width;
                sgm_val.BearingX = sgm.BearingX;
                sgm_val.BearingY = sgm.BearingY;
                sgm_val.Advance  = sgm.Advance;

                return sgm_val;
            }

            public bool Validate(Validator v, string sIdentity, OTTable tableOwner)
            {
                bool bOk = true;

                // ???
                // ??? are there any values that are invalid ???
                // ???

                return bOk;
            }
        }

        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            m_nCachedMaxpNumGlyphs = fontOwner.GetMaxpNumGlyphs();


            if (v.PerformTest(T.EBDT_version))
            {
                if (version.GetUint() == 0x00020000)
                {
                    v.Pass(T.EBDT_version, P.EBDT_P_version, m_tag);
                }
                else
                {
                    v.Error(T.EBDT_version, E.EBDT_E_version, m_tag, "version = 0x" + version.GetUint().ToString("x8") + ", unable to continue validation");
                    return false;
                }
            }

            if (v.PerformTest(T.EBDT_TableDependency))
            {
                Table_EBLC EBLCTable = (Table_EBLC)fontOwner.GetTable("EBLC");
                if (EBLCTable != null)
                {
                    v.Pass(T.EBDT_TableDependency, P.EBDT_P_TableDependency, m_tag);
                }
                else
                {
                    v.Error(T.EBDT_TableDependency, E.EBDT_E_TableDependency, m_tag);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.EBDT_GlyphImageData))
            {
                bool bGlyphImageDataOk = true;

                Table_EBLC EBLCTable = (Table_EBLC)fontOwner.GetTable("EBLC");
                
                // for each bitmap size
                for (uint i=0; i<EBLCTable.numSizes; i++)
                {
                    Table_EBLC.bitmapSizeTable bst = EBLCTable.GetBitmapSizeTable(i);
                    string sSize = "bitmapsize[" + i + "], ppemX=" + bst.ppemX + ", ppemY=" + bst.ppemY;

                    Table_EBLC.indexSubTableArray[] ista = EBLCTable.GetIndexSubTableArray(bst);

                    if (ista != null)
                    {
                        for (uint j=0; j < bst.numberOfIndexSubTables; j++)
                        {
                            Table_EBLC.indexSubTable ist = null;
                            if (ista[j] != null)
                            {
                                 ist = bst.GetIndexSubTable(ista[j]);
                            }

                            if (ist != null)
                            {
                                string sID = sSize + ", indexSubTable[" + j + "](index fmt " + ist.header.indexFormat + 
                                    ", image fmt " + ist.header.imageFormat + ")";

                                switch(ist.header.imageFormat)
                                {
                                    case 1:
                                        if (!Validate_Format1(v, sID, ist))
                                        {
                                            bGlyphImageDataOk = false;
                                            bRet = false;
                                        }
                                        break;

                                    case 2:
                                        if (!Validate_Format2(v, sID, ist))
                                        {
                                            bGlyphImageDataOk = false;
                                            bRet = false;
                                        }
                                        break;

                                    case 3:
                                        if (!Validate_Format3(v, sID, ist))
                                        {
                                            bGlyphImageDataOk = false;
                                            bRet = false;
                                        }
                                        break;

                                    case 4:
                                        if (!Validate_Format4(v, sID, ist))
                                        {
                                            bGlyphImageDataOk = false;
                                            bRet = false;
                                        }
                                        break;

                                    case 5:
                                        if (!Validate_Format5(v, sID, ist))
                                        {
                                            bGlyphImageDataOk = false;
                                            bRet = false;
                                        }
                                        break;

                                    case 6:
                                        if (!Validate_Format6(v, sID, ist))
                                        {
                                            bGlyphImageDataOk = false;
                                            bRet = false;
                                        }
                                        break;

                                    case 7:
                                        if (!Validate_Format7(v, sID, ist))
                                        {
                                            bGlyphImageDataOk = false;
                                            bRet = false;
                                        }
                                        break;

                                    case 8:
                                        if (!Validate_Format8(v, sID, ist))
                                        {
                                            bGlyphImageDataOk = false;
                                            bRet = false;
                                        }
                                        break;

                                    case 9:
                                        if (!Validate_Format9(v, sID, ist))
                                        {
                                            bGlyphImageDataOk = false;
                                            bRet = false;
                                        }
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }

                if (bGlyphImageDataOk)
                {
                    v.Pass(T.EBDT_GlyphImageData, P.EBDT_P_GlyphImageData, m_tag);
                }
            }

            return bRet;
        }


        public bool Validate_Format1(Validator v, string sIdentity, Table_EBLC.indexSubTable ist)
        {
            bool bOk = true;

            Table_EBLC.indexSubTableArray ista = ist.GetIndexSubTableArray();

            for (ushort idGlyph=ista.firstGlyphIndex; idGlyph <= ista.lastGlyphIndex; idGlyph++)
            {
                // validate small metrics
                smallGlyphMetrics sgm = GetSmallMetrics(ist, idGlyph, ista.firstGlyphIndex);
                if (sgm != null)
                {
                    smallGlyphMetrics_val sgm_val = smallGlyphMetrics_val.CreateFromSmallGlyphMetrics(sgm);
                    if (!sgm_val.Validate(v, sIdentity + ", idGlyph=" + idGlyph, this))
                    {
                        bOk = false;
                    }

                    // validate image data
                    // - this is just bitmap data, any values should be valid
                }
            }

            return bOk;
        }

        public bool Validate_Format2(Validator v, string sIdentity, Table_EBLC.indexSubTable ist)
        {
            bool bOk = true;

            Table_EBLC.indexSubTableArray ista = ist.GetIndexSubTableArray();

            for (ushort idGlyph=ista.firstGlyphIndex; idGlyph <= ista.lastGlyphIndex; idGlyph++)
            {
                // validate small metrics
                smallGlyphMetrics sgm = GetSmallMetrics(ist, idGlyph, ista.firstGlyphIndex);
                if (sgm != null)
                {
                    smallGlyphMetrics_val sgm_val = smallGlyphMetrics_val.CreateFromSmallGlyphMetrics(sgm);
                    if (!sgm_val.Validate(v, sIdentity + ", idGlyph=" + idGlyph, this))
                    {
                        bOk = false;
                    }

                    // validate image data
                    // - this is just bitmap data, any values should be valid
                }
            }

            return bOk;
        }

        public bool Validate_Format3(Validator v, string sIdentity, Table_EBLC.indexSubTable ist)
        {
            bool bOk = true;

            // this format is obsolete!
            // - however the image format number is actually stored in the EBLC table
            // - and so any errors should get reported there

            return bOk;
        }

        public bool Validate_Format4(Validator v, string sIdentity, Table_EBLC.indexSubTable ist)
        {
            bool bOk = true;

            // this format is not supported!
            // - however the image format number is actually stored in the EBLC table
            // - and so any errors should get reported there

            return bOk;
        }

        public bool Validate_Format5(Validator v, string sIdentity, Table_EBLC.indexSubTable ist)
        {
            bool bOk = true;

            Table_EBLC.indexSubTableArray ista = ist.GetIndexSubTableArray();

            for (ushort idGlyph=ista.firstGlyphIndex; idGlyph <= ista.lastGlyphIndex; idGlyph++)
            {
                // no metrics in format 5

                // validate image data
                // - this is just bitmap data, any values should be valid
            }

            return bOk;
        }

        public bool Validate_Format6(Validator v, string sIdentity, Table_EBLC.indexSubTable ist)
        {
            bool bOk = true;

            Table_EBLC.indexSubTableArray ista = ist.GetIndexSubTableArray();

            for (ushort idGlyph=ista.firstGlyphIndex; idGlyph <= ista.lastGlyphIndex; idGlyph++)
            {
                // validate big metrics
                bigGlyphMetrics bgm = GetBigMetrics(ist, idGlyph, ista.firstGlyphIndex);
                if (bgm != null)
                {
                    bigGlyphMetrics_val bgm_val = bigGlyphMetrics_val.CreateFromBigGlyphMetrics(bgm);
                    if (!bgm_val.Validate(v, sIdentity + ", idGlyph=" + idGlyph, this))
                    {
                        bOk = false;
                    }

                    // validate image data
                    // - this is just bitmap data, any values should be valid
                }
            }

            return bOk;
        }

        public bool Validate_Format7(Validator v, string sIdentity, Table_EBLC.indexSubTable ist)
        {
            bool bOk = true;

            Table_EBLC.indexSubTableArray ista = ist.GetIndexSubTableArray();

            for (ushort idGlyph=ista.firstGlyphIndex; idGlyph <= ista.lastGlyphIndex; idGlyph++)
            {
                // validate big metrics
                bigGlyphMetrics bgm = GetBigMetrics(ist, idGlyph, ista.firstGlyphIndex);
                if (bgm != null)
                {
                    bigGlyphMetrics_val bgm_val = bigGlyphMetrics_val.CreateFromBigGlyphMetrics(bgm);
                    if (!bgm_val.Validate(v, sIdentity + ", idGlyph=" + idGlyph, this))
                    {
                        bOk = false;
                    }

                    // validate image data
                    // - this is just bitmap data, any values should be valid
                }
            }

            return bOk;
        }

        public bool Validate_Format8(Validator v, string sIdentity, Table_EBLC.indexSubTable ist)
        {
            bool bOk = true;

            Table_EBLC.indexSubTableArray ista = ist.GetIndexSubTableArray();

            for (ushort idGlyph=ista.firstGlyphIndex; idGlyph <= ista.lastGlyphIndex; idGlyph++)
            {
                // validate small metrics
                smallGlyphMetrics sgm = GetSmallMetrics(ist, idGlyph, ista.firstGlyphIndex);
                if (sgm != null)
                {
                    smallGlyphMetrics_val sgm_val = smallGlyphMetrics_val.CreateFromSmallGlyphMetrics(sgm);
                    if (!sgm_val.Validate(v, sIdentity + ", idGlyph=" + idGlyph, this))
                    {
                        bOk = false;
                    }

                    ushort numComponents = this.GetNumComponents(ist, idGlyph, ista.firstGlyphIndex);

                    // validate component array
                    for (uint i=0; i<numComponents; i++)
                    {
                        ebdtComponent component = GetComponent(ist, idGlyph, ista.firstGlyphIndex, i);
                        Debug.Assert(component!= null);


                        // validate the ebdtComponent

                        // verify that the component's glyph code is less than maxp numGlyphs
                        if (component.glyphCode >= m_nCachedMaxpNumGlyphs)
                        {
                            string sDetails = sIdentity + ", idGlyph=" + idGlyph + 
                                ", component[" + i + "].glyphCode=" + component.glyphCode +
                                ", maxp.numGlyphs = " + m_nCachedMaxpNumGlyphs;
                            v.Error(T.EBDT_GlyphImageData, E.EBDT_E_GlyphImageData, m_tag, sDetails);
                            bOk = false;
                        }

                        // verify that the component's glyph code isn't 0, which should be reserved as the empty glyph
                        // (technically, someone could use the empty glyph as a component, but it's more likely to be an error)
                        if (component.glyphCode == 0)
                        {
                            string sDetails = sIdentity + ", idGlyph=" + idGlyph + 
                                ", component[" + i + "].glyphCode=" + component.glyphCode;
                            v.Error(T.EBDT_GlyphImageData, E.EBDT_E_GlyphImageData, m_tag, sDetails);
                            bOk = false;
                        }

                        // verify that the component's glyph code isn't the glyph code of its parent
                        if (component.glyphCode == idGlyph)
                        {
                            string sDetails = sIdentity + ", idGlyph=" + idGlyph + 
                                ", component[" + i + "].glyphCode=" + component.glyphCode + " (glyph can't use itself as a component)";
                            v.Error(T.EBDT_GlyphImageData, E.EBDT_E_GlyphImageData, m_tag, sDetails);
                            bOk = false;
                        }
                    }
                }
            }

            return bOk;
        }

        public bool Validate_Format9(Validator v, string sIdentity, Table_EBLC.indexSubTable ist)
        {
            bool bOk = true;

            Table_EBLC.indexSubTableArray ista = ist.GetIndexSubTableArray();

            for (ushort idGlyph=ista.firstGlyphIndex; idGlyph <= ista.lastGlyphIndex; idGlyph++)
            {
                // validate big metrics
                bigGlyphMetrics bgm = GetBigMetrics(ist, idGlyph, ista.firstGlyphIndex);
                if (bgm != null)
                {
                    bigGlyphMetrics_val bgm_val = bigGlyphMetrics_val.CreateFromBigGlyphMetrics(bgm);
                    if (!bgm_val.Validate(v, sIdentity + ", idGlyph=" + idGlyph, this))
                    {
                        bOk = false;
                    }

                    ushort numComponents = this.GetNumComponents(ist, idGlyph, ista.firstGlyphIndex);

                    // validate component array
                    for (uint i=0; i<numComponents; i++)
                    {
                        ebdtComponent component = GetComponent(ist, idGlyph, ista.firstGlyphIndex, i);
                        Debug.Assert(component!= null);


                        // validate the ebdtComponent

                        // verify that the component's glyph code is less than maxp numGlyphs
                        if (component.glyphCode >= m_nCachedMaxpNumGlyphs)
                        {
                            string sDetails = sIdentity + ", idGlyph=" + idGlyph + 
                                ", component[" + i + "].glyphCode=" + component.glyphCode +
                                ", maxp.numGlyphs = " + m_nCachedMaxpNumGlyphs;
                            v.Error(T.EBDT_GlyphImageData, E.EBDT_E_GlyphImageData, m_tag, sDetails);
                            bOk = false;
                        }

                        // verify that the component's glyph code isn't 0, which should be reserved as the empty glyph
                        // (technically, someone could use the empty glyph as a component, but it's more likely to be an error)
                        if (component.glyphCode == 0)
                        {
                            string sDetails = sIdentity + ", idGlyph=" + idGlyph + 
                                ", component[" + i + "].glyphCode=" + component.glyphCode;
                            v.Error(T.EBDT_GlyphImageData, E.EBDT_E_GlyphImageData, m_tag, sDetails);
                            bOk = false;
                        }

                        // verify that the component's glyph code isn't the glyph code of its parent
                        if (component.glyphCode == idGlyph)
                        {
                            string sDetails = sIdentity + ", idGlyph=" + idGlyph + 
                                ", component[" + i + "].glyphCode=" + component.glyphCode + " (glyph can't use itself as a component)";
                            v.Error(T.EBDT_GlyphImageData, E.EBDT_E_GlyphImageData, m_tag, sDetails);
                            bOk = false;
                        }
                    }
                }
            }

            return bOk;
        }

        public ushort m_nCachedMaxpNumGlyphs;

    }
}
