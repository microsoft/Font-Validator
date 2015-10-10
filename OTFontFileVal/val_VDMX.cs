using System;
using System.Diagnostics;

using OTFontFile;
using OTFontFile.Rasterizer;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_VDMX.
    /// </summary>
    public class val_VDMX : Table_VDMX, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_VDMX(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
        * private methods
        */

        private static int tolperc = 10;

        bool TestTolerance(int ycalc, int ytable,int dif)
        {
            if (ytable < ycalc - (1 + (dif * tolperc - 1) / 100) || ytable > ycalc + (1 + (dif * tolperc - 1) / 100))
            {
                return false;
            }
            return true;
        }

        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.VDMX_Version))
            {
                if (version == 0 || version == 1)
                {
                    v.Pass(T.VDMX_Version, P.VDMX_P_Version, m_tag, version.ToString());
                }
                else
                {
                    v.Error(T.VDMX_Version, E.VDMX_E_Version, m_tag, version.ToString());
                    bRet = false;
                }
            }

            if (v.PerformTest(T.VDMX_Offsets))
            {
                bool bOffsetsOk = true;
                
                ushort minPossibleOffset = (ushort)((ushort)FieldOffsets.ratRange + numRatios * 4 + numRatios*2);
                ushort maxPossibleOffset = (ushort)GetLength();

                for (uint i=0; i<numRatios; i++)
                {
                    ushort offset = GetVdmxGroupOffset(i);
                    if (offset < minPossibleOffset || offset > maxPossibleOffset)
                    {
                        v.Error(T.VDMX_Offsets, E.VDMX_E_InvalidOffset, m_tag, "#" + i + " offset = " + offset);
                        bOffsetsOk = false;
                        bRet = false;
                    }
                }

                if (bOffsetsOk)
                {
                    v.Pass(T.VDMX_Offsets, P.VDMX_P_Offsets, m_tag);
                }
            }

            if (v.PerformTest(T.VDMX_GroupsInTable))
            {
                bool bGroupsOk = true;

                for (uint i=0; i<numRatios; i++)
                {
                    Vdmx vdmx = GetVdmxGroup(i);
                    uint EndOffset = (uint)GetVdmxGroupOffset(i) + 4 + (uint)vdmx.recs*6;

                    if (EndOffset > GetLength())
                    {
                        v.Error(T.VDMX_GroupsInTable, E.VDMX_E_GroupsInTable, m_tag, "group# " + i);
                        bGroupsOk = false;
                        bRet = false;
                    }
                }

                if (bGroupsOk)
                {
                    v.Pass(T.VDMX_GroupsInTable, P.VDMX_P_GroupsInTable, m_tag);
                }
            }

            if (v.PerformTest(T.VDMX_CompareToCalcData))
            {
                bool bDataOk = true;
                bool needtol = false;

                RasterInterf.DevMetricsData dmd = null;

                try
                {
                    Version ver = fontOwner.GetFile().GetRasterizer().FTVersion;

                    if ( ver.CompareTo(new Version(2,6,1)) < 0 )
                        v.Warning(T.VDMX_CompareToCalcData, W.VDMX_W_Need_Newer_FreeType, m_tag,
                                  "Using FreeType Version " + ver + " may not get correct results for VDMX");

                    dmd = fontOwner.GetCalculatedDevMetrics();
                }
                catch (InvalidOperationException e)
                {
                    // JJF Figure out what to do. Changed to warning
                    v.Warning(T.VDMX_CompareToCalcData, W._TEST_W_ErrorInAnotherTable, m_tag, e.Message);
                }
                catch(Exception e)
                {
                    v.ApplicationError(T.VDMX_CompareToCalcData, E._Table_E_Exception, m_tag, e.Message);
                    bRet = false;
                }

                if (dmd != null)
                {
                    for (uint iRatio=0; iRatio<numRatios; iRatio++)
                    {
                        Ratios ratio = GetRatioRange(iRatio);
                        Vdmx group = GetVdmxGroup(iRatio);

                        for (uint iEntry=0; iEntry<group.recs; iEntry++)
                        {
                            Vdmx.vTable vTableEntry = group.GetEntry(iEntry);

                            if (vTableEntry.yPelHeight <= 255)
                            {
                                if (vTableEntry.yPelHeight == dmd.vdmxData.groups[iRatio].entry[vTableEntry.yPelHeight - group.startsz].yPelHeight)
                                {
                                    
                                    if (vTableEntry.yMin != dmd.vdmxData.groups[iRatio].entry[iEntry].yMin
                                        || vTableEntry.yMax != dmd.vdmxData.groups[iRatio].entry[iEntry].yMax){

                                        int dif = dmd.vdmxData.groups[iRatio].entry[iEntry].yMax - dmd.vdmxData.groups[iRatio].entry[iEntry].yMin;

                                        if (!TestTolerance(dmd.vdmxData.groups[iRatio].entry[iEntry].yMin, vTableEntry.yMin, dif) ||
                                         !TestTolerance(dmd.vdmxData.groups[iRatio].entry[iEntry].yMax, vTableEntry.yMax, dif))
                                        {

                                            String sDetails = "group[" + iRatio + "], entry[" + iEntry + "], yPelHeight = " + vTableEntry.yPelHeight +
                                                ", yMin,yMax = " + vTableEntry.yMin + "," + vTableEntry.yMax +
                                                ", calculated yMin,yMax = " + dmd.vdmxData.groups[iRatio].entry[iEntry].yMin + "," +
                                                dmd.vdmxData.groups[iRatio].entry[iEntry].yMax;
                                            v.Error(T.VDMX_CompareToCalcData, E.VDMX_E_CalcData, m_tag, sDetails);

                                            bDataOk = false;

                                        }
                                        else
                                        {
                                            needtol = true;
                                        }
                                    }
                                    /*
                                    else
                                    {
                                        String s = "group[" + iRatio + "], yPelHeight = " + vTableEntry.yPelHeight + ", entry OK";
                                        v.DebugMsg(s, m_tag);
                                    }
                                    */
                                }
                                else
                                {
                                    Debug.Assert(false);
                                }
                            }
                            else
                            {
                                String sDetails = "group[" + iRatio + "], entry[" + iEntry + "], yPelHeight = " + vTableEntry.yPelHeight;
                                v.Error(T.VDMX_CompareToCalcData, E.VDMX_E_yPelHeight_illegal, m_tag, sDetails);
                                bDataOk = false;
                            }
                        }
                    }

                    if (bDataOk)
                    {
                        if (needtol)
                        {
                            String sDetails = "The differences were smaller than the tolerance, so they may well be valid if the VDMX was hand-tuned";
                            v.Warning(T.VDMX_CompareToCalcData, W.VDMX_W_CalcData, m_tag, sDetails);
                        }
                        else
                        {
                            v.Pass(T.VDMX_CompareToCalcData, P.VDMX_P_CalcData, m_tag);
                        }
                    }
                    else
                    {
                        //v.Error(T.VDMX_CompareToCalcData, E.VDMX_E_CalcData, m_tag);
                        bRet = false;
                    }
                }
                else
                {
                    // Rasterization could not occur for various reasons.
                    string s = "Unable to get calculated device metrics.";
                    v.Error(T.VDMX_CompareToCalcData, E.VDMX_E_CalcData, m_tag, s);
                    bDataOk = false;
                    bRet = false;
                }
            }

            return bRet;
        }



    }
}
