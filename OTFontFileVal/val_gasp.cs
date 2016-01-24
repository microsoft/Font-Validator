using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_gasp.
    /// </summary>
    public class val_gasp : Table_gasp, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_gasp(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }



        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.gasp_Version))
            {
                if (version == 0 || version == 1)
                {
                    v.Pass(T.gasp_Version, P.gasp_P_Version, m_tag, "version = " + version.ToString());
                }
                else
                {
                    v.Error(T.gasp_Version, E.gasp_E_Version, m_tag, "version = " + version.ToString() + ", unable to continue validation");
                    return false;
                }
            }

            if (v.PerformTest(T.gasp_rangeGaspBehavior))
            {
                bool bFlagsOk = true;
                string first_error = null;
                for (uint i=0; i<numRanges; i++)
                {
                    GaspRange gr = GetGaspRange(i);
                    if (gr != null)
                    {
                        if (   (version == 0 && gr.rangeGaspBehavior > 0x3)
                            || (version == 1 && gr.rangeGaspBehavior > 0xf))
                        {
                            bFlagsOk = false;
                            first_error = "version=" + version + ", range #" + i + ", rangeGaspBehavior=0x"
                                + gr.rangeGaspBehavior.ToString("X4");
                            break;
                        }
                    }
                }
                if (bFlagsOk)
                {
                    v.Pass(T.gasp_rangeGaspBehavior, P.gasp_P_rangeGaspBehavior, m_tag);
                }
                else
                {
                    v.Error(T.gasp_rangeGaspBehavior, E.gasp_E_rangeGaspBehavior, m_tag, first_error);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.gasp_SortOrder))
            {
                bool bSortOk = true;
                if (numRanges > 1)
                {
                    GaspRange grCurr = GetGaspRange(0);
                    GaspRange grNext = null;
                    for (uint i=1; i<numRanges; i++)
                    {
                        grNext = GetGaspRange(i);
                        if (grCurr.rangeMaxPPEM >= grNext.rangeMaxPPEM)
                        {
                            bSortOk = false;
                            break;
                        }
                        grCurr = grNext;
                    }
                }
                if (bSortOk)
                {
                    v.Pass(T.gasp_SortOrder, P.gasp_P_SortOrder, m_tag);
                }
                else
                {
                    v.Error(T.gasp_SortOrder, E.gasp_E_SortOrder, m_tag);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.gasp_Sentinel))
            {
                GaspRange gr = GetGaspRange((uint)numRanges-1);
                if (gr.rangeMaxPPEM == 0xFFFF)
                {
                    v.Pass(T.gasp_Sentinel, P.gasp_P_Sentinel, m_tag);
                }
                else
                {
                    v.Error(T.gasp_Sentinel, E.gasp_E_Sentinel, m_tag);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.gasp_AdjRangeIdenticalFlags))
            {
                bool bNoAdjIdent = true;

                if (numRanges > 1)
                {
                    for (uint i=0; i<numRanges-1; i++)
                    {
                        GaspRange grCurr = GetGaspRange(i);
                        GaspRange grNext = GetGaspRange(i+1);
                        if (grCurr.rangeGaspBehavior == grNext.rangeGaspBehavior)
                        {
                            string sDetails = "rangeGaspBehavior[" + i + "] = " + grCurr.rangeGaspBehavior + ", rangeGaspBehavior[" + (i+1) + "] = " + grNext.rangeGaspBehavior;
                            v.Warning(T.gasp_AdjRangeIdenticalFlags, W.gasp_W_AdjRangeIdenticalFlags, m_tag, sDetails);
                            bNoAdjIdent = false;
                        }
                    }
                }

                if (bNoAdjIdent)
                {
                    v.Pass(T.gasp_AdjRangeIdenticalFlags, P.gasp_P_AdjRangeIdenticalFlags, m_tag);
                }
            }

            return bRet;
        }



    }
}
