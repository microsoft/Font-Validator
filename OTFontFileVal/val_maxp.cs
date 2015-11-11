using System;

using OTFontFile;

namespace OTFontFileVal
{
    public class val_maxp : Table_maxp, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_maxp(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.maxp_TableVersion))
            {
                uint val = TableVersionNumber.GetUint();

                Table_CFF  CFFTable  = (Table_CFF)  fontOwner.GetTable("CFF ");
                Table_glyf glyfTable = (Table_glyf) fontOwner.GetTable("glyf");

                if (val == 0x00005000)
                {
                    if (CFFTable != null && glyfTable == null)
                    {
                        v.Pass(T.maxp_TableVersion, P.maxp_P_VERSION_0_5, m_tag);
                    }
                    else if (CFFTable == null)
                    {
                        v.Error(T.maxp_TableVersion, E.maxp_E_VERSION_0_5_NOCFF, m_tag);
                        bRet = false;
                    }
                    else if (glyfTable != null)
                    {
                        v.Error(T.maxp_TableVersion, E.maxp_E_VERSION_0_5_glyf, m_tag);
                        bRet = false;
                    }
                }
                else if (val == 0x00010000)
                {
                    if (CFFTable == null && glyfTable != null)
                    {
                        v.Pass(T.maxp_TableVersion, P.maxp_P_VERSION_1_0, m_tag);
                    }
                    else if (glyfTable == null)
                    {
                        v.Error(T.maxp_TableVersion, E.maxp_E_VERSION_1_0_NOglyf, m_tag);
                        bRet = false;
                    }
                    else if (CFFTable != null)
                    {
                        v.Error(T.maxp_TableVersion, E.maxp_E_VERSION_1_0_CFF, m_tag);
                        bRet = false;
                    }
                }
                else
                {
                    v.Error(T.maxp_TableVersion, E.maxp_E_VERSION_INVALID, m_tag, "0x"+val.ToString("x8"));
                    bRet = false;
                }
            }

            if (v.PerformTest(T.maxp_TableLength))
            {
                uint val = TableVersionNumber.GetUint();

                if (val == 0x00005000)
                {
                    if (m_bufTable.GetLength() == 6)
                    {
                        v.Pass(T.maxp_TableLength, P.maxp_P_LENGTH_0_5, m_tag);
                    }
                    else
                    {
                        v.Error(T.maxp_TableLength, E.maxp_E_LENGTH_0_5, m_tag, m_bufTable.GetLength().ToString());
                        bRet = false;
                    }
                }
                else if (val == 0x00010000)
                {
                    if (m_bufTable.GetLength() == 32)
                    {
                        v.Pass(T.maxp_TableLength, P.maxp_P_LENGTH_1_0, m_tag);
                    }
                    else
                    {
                        v.Error(T.maxp_TableLength, E.maxp_E_LENGTH_1_0, m_tag, m_bufTable.GetLength().ToString());
                        bRet = false;
                    }
                }
            }

            if (v.PerformTest(T.maxp_NumGlyphsMatchLoca))
            {
                if (TableVersionNumber.GetUint() == 0x00010000)
                {
                    Table_loca locaTable = (Table_loca)fontOwner.GetTable("loca");
                    if (locaTable != null)
                    {
                        // locaTable.NumEntry returns (-1) on failure
                        if (locaTable.NumEntry(fontOwner) == NumGlyphs+1)
                        {
                            v.Pass(T.maxp_NumGlyphsMatchLoca, P.maxp_P_NumGlyphsMatchLoca, m_tag, "numGlyphs = " + NumGlyphs);
                        }
                        else
                        {
                            v.Error(T.maxp_NumGlyphsMatchLoca, E.maxp_E_NumGlyphsMatchLoca, m_tag, "numGlyphs = " + NumGlyphs);
                            bRet = false;
                        }
                    }
                    else
                    {
                        v.Error(T.maxp_NumGlyphsMatchLoca, E._TEST_E_TableMissing, m_tag, "loca");
						bRet = false;
                    }
                }
                else
                {
                    v.Info(T.maxp_NumGlyphsMatchLoca, I._TEST_I_TableVersion, m_tag, "test = maxp_NumGlyphsMatchLoca");
                }
            }

            if (v.PerformTest(T.maxp_GlyphStats))
            {
                if (TableVersionNumber.GetUint() == 0x00010000)
                {
                    Table_glyf glyfTable = (Table_glyf) fontOwner.GetTable("glyf");

                    if (glyfTable == null)
                    {
                        v.Error(T.maxp_GlyphStats, E._TEST_E_TableMissing, m_tag, "glyf");
						bRet = false;
                    }
                    else
                    {

                        bool bGlyphStatsOk = true;

                        if (ComputeMaxpStats(glyfTable, fontOwner))
                        {

                            if (maxPoints != maxPointsCalc)
                            {
                                String sDetails = "maxPoints = " + maxPoints + ", calculated = " + maxPointsCalc;
                                v.Error(T.maxp_GlyphStats, E.maxp_E_Calculation, m_tag, sDetails);
                                bRet = false;
                                bGlyphStatsOk = false;
                            }

                            if (maxContours != maxContoursCalc)
                            {
                                String sDetails = "maxContours = " + maxContours + ", calculated = " + maxContoursCalc;
                                v.Error(T.maxp_GlyphStats, E.maxp_E_Calculation, m_tag, sDetails);
                                bRet = false;
                                bGlyphStatsOk = false;
                            }

                            if (maxCompositePoints != maxCompositePointsCalc)
                            {
                                String sDetails = "maxCompositePoints = " + maxCompositePoints + ", calculated = " + maxCompositePointsCalc;
                                v.Error(T.maxp_GlyphStats, E.maxp_E_Calculation, m_tag, sDetails);
                                bRet = false;
                                bGlyphStatsOk = false;
                            }

                            if (maxCompositeContours != maxCompositeContoursCalc)
                            {
                                String sDetails = "maxCompositeContours = " + maxCompositeContours + ", calculated = " + maxCompositeContoursCalc;
                                v.Error(T.maxp_GlyphStats, E.maxp_E_Calculation, m_tag, sDetails);
                                bRet = false;
                                bGlyphStatsOk = false;
                            }

                            // Bug 2168.
                            // Case 1: Same as max size from glyf table. Info.
                            // Case 2: Same as max size from glyf table + 
                            //         size(fpgm) + size(prep). Info message.
                            // Case 3: Smaller than max size from glyf table. 
                            //         Error
                            // Case 4: Neither 1 nor 2. Warning.
                            DirectoryEntry dePrep = 
                                fontOwner.GetDirectoryEntry("prep" );
                            uint prepLength = 0;
                            if ( null != dePrep ) {
                                prepLength = dePrep.length;
                            }
                            DirectoryEntry deFpgm = 
                                fontOwner.GetDirectoryEntry("fpgm" );
                            uint fpgmLength = 0;
                            if ( null != deFpgm ) {
                                fpgmLength = deFpgm.length;
                            }

                            if ( maxSizeOfInstructions == 
                                 maxSizeOfInstructionsCalc ) {
                                // Case 1:
                                String sDetails = "maxSizeOfInstructions=" +
                                    maxSizeOfInstructions + ", computed " +
                                    "from the glyf table";
                                v.Info( T.maxp_GlyphStats,
                                        I.maxp_I_Calculation_Method1,
                                        m_tag, sDetails);
                            }
                            else if ( maxSizeOfInstructions ==  
                                      ( maxSizeOfInstructionsCalc + 
                                        prepLength + fpgmLength ) ) {
                                // Case 2:
                                String sDetails = 
                                    "maxp maxSizeOfInstructions is " + 
                                    maxSizeOfInstructions + ", which is " +
                                    "glyf maxSizeOfInstructions (" + 
                                    maxSizeOfInstructionsCalc + 
                                    ") + prep size (" + prepLength + 
                                    ") + fpgm size (" + fpgmLength + ")";
                                v.Info( T.maxp_GlyphStats, 
                                        I.maxp_I_Calculation_Method2, 
                                        m_tag, sDetails);
                            }
                            else if ( maxSizeOfInstructions < 
                                      maxSizeOfInstructionsCalc ) {
                                // Case 3
                                String sDetails = 
                                    "maxp maxSizeOfInstructions is " + 
                                    maxSizeOfInstructions + 
                                    ", which is smaller than the " + 
                                    "size of instuctions (" + 
                                    maxSizeOfInstructionsCalc + ") found" +
                                    " for some glyph in the glyf table.";
                                v.Error( T.maxp_GlyphStats, 
                                         E.maxp_E_Calculation, m_tag, 
                                         sDetails);
                                bRet = false;
                            }
                            else {
                                // Case 4
                                String sDetails = 
                                    "glyf maxSizeOfInstructions=" + 
                                    maxSizeOfInstructionsCalc + 
                                    ", prep size=" + prepLength + 
                                    ", fpgm size=" + fpgmLength + 
                                    ", whereas maxp maxSizeOfInstruction " +
                                    "is " + maxSizeOfInstructions;
                                v.Warning(T.maxp_GlyphStats, 
                                          W.maxp_W_Calculation_Unclear, 
                                          m_tag, 
                                          sDetails);
                                bGlyphStatsOk = false;
                            }

                            if (maxComponentElements != maxComponentElementsCalc)
                            {
                                String sDetails = "maxComponentElements = " + maxComponentElements + ", calculated = " + maxComponentElementsCalc;
                                v.Error(T.maxp_GlyphStats, E.maxp_E_Calculation, m_tag, sDetails);
                                bRet = false;
                                bGlyphStatsOk = false;
                            }

                            if (maxComponentDepth != maxComponentDepthCalc)
                            {
                                String sDetails = "maxComponentDepth = " + maxComponentDepth + ", calculated = " + maxComponentDepthCalc;
                                v.Error(T.maxp_GlyphStats, E.maxp_E_Calculation, m_tag, sDetails);
                                bRet = false;
                                bGlyphStatsOk = false;
                            }

                            if (bGlyphStatsOk)
                            {
                                v.Pass(T.maxp_GlyphStats, P.maxp_P_Calculation, m_tag);
                            }
                        }
                        else
                        {
                            v.Warning(T.maxp_GlyphStats, W._TEST_W_ErrorInAnotherTable, m_tag, "Errors in the glyf table are preventing validation of maxPoints, maxContours, maxCompositePoints, maxCompositeContours, maxSizeofInstructions, maxComponentElements, and maxComponentDepth");
                        }
                    }
                }
                else
                {
                    v.Info(T.maxp_GlyphStats, I._TEST_I_TableVersion, m_tag, "test = maxp_GlyphStats");
                }
            }

            return bRet;
        }

        private bool ComputeMaxpStats(Table_glyf glyfTable, OTFont fontOwner)
        {
            bool bRet = true;

            try
            {
                maxPointsCalc             = 0;
                maxContoursCalc           = 0;
                maxCompositePointsCalc    = 0;
                maxCompositeContoursCalc  = 0;
                maxSizeOfInstructionsCalc = 0;
                maxComponentElementsCalc  = 0;
                maxComponentDepthCalc     = 0;

                for (uint iGlyph = 0; iGlyph<NumGlyphs; iGlyph++)
                {
                    Table_glyf.header glyfHeader = glyfTable.GetGlyphHeader(iGlyph, fontOwner);
                    if (glyfHeader != null)
                    {
                        if (glyfHeader.numberOfContours >= 0)
                        {
                            Table_glyf.SimpleGlyph sg = glyfHeader.GetSimpleGlyph();
                            maxPointsCalc = (ushort)Math.Max(maxPointsCalc, sg.GetEndPtOfContour((uint)glyfHeader.numberOfContours-1)+1);
                            maxContoursCalc = (ushort)Math.Max(maxContoursCalc, glyfHeader.numberOfContours);
                            maxSizeOfInstructionsCalc = Math.Max(maxSizeOfInstructionsCalc, sg.instructionLength);
                        }
                        else
                        {
                            ushort nPointsTemp = 0;
                            short nContoursTemp = 0;
                            ushort nInstructionsTemp = 0;
                            ushort nCompElementsTemp = 0;
                            ushort nCompDepthTemp = 0;

                            Table_glyf.CompositeGlyph cg = glyfHeader.GetCompositeGlyph();
                            GetCompositeGlyphStats(cg, ref nContoursTemp, ref nPointsTemp, ref nInstructionsTemp, ref nCompElementsTemp, ref nCompDepthTemp, fontOwner, glyfTable);

                            maxCompositePointsCalc    = Math.Max(maxCompositePointsCalc, nPointsTemp);
                            maxCompositeContoursCalc  = Math.Max(maxCompositeContoursCalc, nContoursTemp);
                            maxSizeOfInstructionsCalc = Math.Max(maxSizeOfInstructionsCalc, nInstructionsTemp);
                            maxComponentElementsCalc  = Math.Max(maxComponentElementsCalc, nCompElementsTemp);
                            maxComponentDepthCalc     = Math.Max(maxComponentDepthCalc, nCompDepthTemp);
                        }
                    }
                }

            }
            catch
            {
                maxPointsCalc             = 0;
                maxContoursCalc           = 0;
                maxCompositePointsCalc    = 0;
                maxCompositeContoursCalc  = 0;
                maxSizeOfInstructionsCalc = 0;
                maxComponentElementsCalc  = 0;
                maxComponentDepthCalc     = 0;

                bRet = false;
            }

            return bRet;
        }

        private void GetCompositeGlyphStats(Table_glyf.CompositeGlyph cg, ref short nContours, ref ushort nPoints, ref ushort nInstructions, ref ushort nComponentElements, ref ushort nComponentDepth, OTFont fontOwner, Table_glyf glyfTable)
        {
            ushort nPointsTemp = 0;
            short nContoursTemp = 0;
            ushort nInstructionsTemp = 0;
            short TotalContours = 0;
            ushort TotalPoints = 0;
            ushort maxInstructions = 0;

            nComponentDepth++;

            int nCompositeGlyphs = 0;

            while (cg != null)
            {
                nCompositeGlyphs++;
                nInstructions = Math.Max(nInstructions, cg.GetNumInstructions());

                Table_glyf.header glyfHeader = glyfTable.GetGlyphHeader(cg.glyphIndex, fontOwner);
                if (glyfHeader != null)
                {
                    if (glyfHeader.numberOfContours >= 0)
                    {
                        nContoursTemp = glyfHeader.numberOfContours;
                        Table_glyf.SimpleGlyph sg = glyfHeader.GetSimpleGlyph();
                        nPointsTemp = (ushort)(sg.GetEndPtOfContour((uint)glyfHeader.numberOfContours-1)+1);
                        nInstructionsTemp = sg.instructionLength;
                    }
                    else
                    {
                        Table_glyf.CompositeGlyph EmbeddedCG = glyfHeader.GetCompositeGlyph();
                        GetCompositeGlyphStats(EmbeddedCG, ref nContoursTemp, ref nPointsTemp, ref nInstructionsTemp, ref nComponentElements, ref nComponentDepth, fontOwner, glyfTable);
                    }

                    TotalContours += nContoursTemp;
                    TotalPoints += nPointsTemp;
                    maxInstructions = Math.Max(maxInstructions, nInstructionsTemp);
                }

                cg = cg.GetNextCompositeGlyph();
            }

            nComponentElements = (ushort)Math.Max(nComponentElements, nCompositeGlyphs);
            nContours = TotalContours;
            nPoints = TotalPoints;
            nInstructions = maxInstructions;
        }

        /************************
         * member data
         */

        // variables to hold values that were calculated from the glyf table
        ushort maxPointsCalc;
        ushort maxContoursCalc;
        ushort maxCompositePointsCalc;
        short maxCompositeContoursCalc;
        ushort maxSizeOfInstructionsCalc;
        ushort maxComponentElementsCalc;
        ushort maxComponentDepthCalc;

    }
}
