using System;
using System.Diagnostics;

using OTFontFile;
using OTFontFile.Rasterizer;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_LTSH.
    /// </summary>
    public class val_LTSH : Table_LTSH, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_LTSH(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.LTSH_version))
            {
                if (version == 0)
                {
                    v.Pass(T.LTSH_version, P.LTSH_P_version, m_tag);
                }
                else
                {
                    v.Error(T.LTSH_version, E.LTSH_E_version, m_tag, version.ToString());
                    bRet = false;
                }
            }

            Table_maxp maxpTable = (Table_maxp)fontOwner.GetTable("maxp");
            if (maxpTable == null)
            {
                v.Error(T.T_NULL, E._TEST_E_TableMissing, m_tag, "Unable to test this table, maxp table is invalid or missing");
                return false;
            }
            
            if (v.PerformTest(T.LTSH_numGlyphs))
            {
                if (numGlyphs == fontOwner.GetMaxpNumGlyphs())
                {
                    v.Pass(T.LTSH_numGlyphs, P.LTSH_P_numGlyphs, m_tag);
                }
                else
                {
                    string s = "LTSH.numGlyphs = " + numGlyphs + ", maxp.numGlyphs = " + fontOwner.GetMaxpNumGlyphs();
                    v.Error(T.LTSH_numGlyphs, E.LTSH_E_numGlyphs, m_tag, s);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.LTSH_TableLength))
            {
                uint CalcLength = (uint)FieldOffsets.yPels + numGlyphs;
                if (GetLength() == CalcLength)
                {
                    v.Pass(T.LTSH_TableLength, P.LTSH_P_TableLength, m_tag);
                }
                else
                {
                    string s = "calc length = " + CalcLength + ", actual length = " + GetLength();
                    v.Error(T.LTSH_TableLength, E.LTSH_E_TableLength, m_tag, s);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.LTSH_yPels))
            {
                bool bYPelsOk = true;
                RasterInterf.DevMetricsData dmd = null;
                try
                {
                    Version ver = fontOwner.GetFile().GetRasterizer().FTVersion;

                    if ( ver.CompareTo(new Version(2,6,1)) < 0 )
                        v.Warning(T.LTSH_yPels, W.LTSH_W_Need_Newer_FreeType, m_tag,
                                  "Using FreeType Version " + ver + " may not get correct results for LTSH");

                    dmd = fontOwner.GetCalculatedDevMetrics();
                }
                catch (Exception e)
                {
                    v.ApplicationError(T.LTSH_yPels, E._Table_E_Exception, m_tag, e.Message);
                    bRet = false;
                }

                if (dmd != null)
                {
                    for( uint iGlyphIndex = 0; iGlyphIndex < numGlyphs; iGlyphIndex++ )
                    {
                        if (iGlyphIndex >= fontOwner.GetMaxpNumGlyphs())
                        {
                            // JJF. Figure out what to do
                            v.Warning(T.LTSH_yPels, W._TEST_W_OtherErrorsInTable, m_tag, "can't test all yPel values, LTSH.numGlyphs does not equal maxp.numGlyphs");
                            bRet = false;
                            bYPelsOk = false;
                            break;
                        }

                        if( GetYPel(iGlyphIndex) != dmd.ltshData.yPels[iGlyphIndex] )
                        {
                            String sDetails = "glyph# = " + iGlyphIndex + ", value = " + GetYPel(iGlyphIndex) + ", calculated value = " + dmd.ltshData.yPels[iGlyphIndex];
                            v.Error(T.LTSH_yPels, E.LTSH_E_yPels, m_tag, sDetails);
                            bRet = false;
                            bYPelsOk = false;
                        }
                        /*
                        else
                        {
                            String sDetails = "glyph# = " + iGlyphIndex + ", value = " + GetYPel(iGlyphIndex);
                            v.DebugMsg("yPel value OK! " + sDetails, m_tag);
                        }
                        */

                        if (GetYPel(iGlyphIndex) == 0)
                        {
                            String sDetails = "glyph# = " + iGlyphIndex;
                            v.Warning(T.LTSH_yPels, W.LTSH_W_yPels_zero, m_tag, sDetails);
                        }
                    }

                    if (bYPelsOk)
                    {
                        v.Pass(T.LTSH_yPels, P.LTSH_P_yPels, m_tag);
                    }
                }
                else
                {
                    // if user didn't cancel, then check for error message
                    if (!v.CancelFlag)
                    {
                        String sDetails = null;
                        try
                        {
                            sDetails = fontOwner.GetDevMetricsDataError();
                        }
                        catch (Exception e)
                        {
                            v.ApplicationError(T.LTSH_yPels, E._Table_E_Exception, m_tag, e.Message);
                        }
                        Debug.Assert(sDetails != null);
                        v.Error(T.LTSH_yPels, E.LTSH_E_Rasterizer, m_tag, sDetails);
                        bRet = false;
                    }                    
                }
            }

            return bRet;
        }
    }
}
