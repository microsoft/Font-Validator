using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_EBSC.
    /// </summary>
    public class val_EBSC : Table_EBSC, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_EBSC(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.EBSC_version))
            {
                if (version.GetUint() == 0x00020000)
                {
                    v.Pass(T.EBSC_version, P.EBSC_P_version, m_tag);
                }
                else
                {
                    v.Error(T.EBSC_version, E.EBSC_E_version, m_tag, "version = 0x" + version.GetUint().ToString("x8") + ", unable to continue validation");
                    return false;
                }
            }

            if (v.PerformTest(T.EBSC_TableLength))
            {
                uint CalcLength = 8 + (numSizes * 28);
                if (CalcLength == GetLength())
                {
                    v.Pass(T.EBSC_TableLength, P.EBSC_P_TableLength, m_tag);
                }
                else
                {
                    string s = "actual length = " + GetLength() + ", calculated = " + CalcLength;
                    v.Error(T.EBSC_TableLength, E.EBSC_E_TableLength, m_tag, s);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.EBSC_TableDependency))
            {
                Table_EBLC EBLCTable = (Table_EBLC)fontOwner.GetTable("EBLC");
                Table_EBDT EBDTTable = (Table_EBDT)fontOwner.GetTable("EBDT");
                if (   EBLCTable != null
                    && EBDTTable != null)
                {
                    v.Pass(T.EBSC_TableDependency, P.EBSC_P_TableDependency, m_tag);
                }
                else
                {
                    string s = "Missing: ";
                    if (EBLCTable == null)
                    {
                        s = s + "EBLC";
                    }
                    if (EBDTTable == null)
                    {
                        if (EBLCTable == null)
                            s = s + ", EBDT";
                        else
                            s = s + "EBDT";
                    }

                    v.Error(T.EBSC_TableDependency, E.EBSC_E_TableDependency, m_tag, s);
                    bRet = false;
                }
            }

            if (v.PerformTest("EBSC_StrikeSizes"))
            {
                Table_EBLC EBLCTable = (Table_EBLC)fontOwner.GetTable("EBLC");
                if (EBLCTable != null)
                {
                    string s = "Missing strike: ";
                    bool bPass = true;

                    for (uint i = 0; i < numSizes; i++)
                    {
                        bitmapScaleTable bitmapScale = GetBitmapScaleTable(i);

                        if (bitmapScale != null)
                        {
                            bool bFound = false;

                            for (uint n = 0; n < EBLCTable.numSizes; n++)
                            {
                                Table_EBLC.bitmapSizeTable bitmapSize = EBLCTable.GetBitmapSizeTable(n);
                                if (bitmapSize != null)
                                {
                                    if (   bitmapScale.substitutePpemX == bitmapSize.ppemX
                                        && bitmapScale.substitutePpemY == bitmapSize.ppemY)
                                    {
                                        bFound = true;
                                        break;
                                    }
                                }

                            }

                            if (!bFound)
                            {
                                string size = "(PpemX:" + bitmapScale.substitutePpemX + ", PpemY:" + bitmapScale.substitutePpemY + ") ";
                                s = s + size;
                                bPass = false;
                            }
                        }
                    }

                    if (bPass)
                    {
                        v.Pass(T.EBSC_StrikeSizes, P.EBSC_P_StrikeSize, m_tag);
                    }
                    else
                    {
                        v.Error(T.EBSC_StrikeSizes, E.EBSC_E_StrikeSize, m_tag, s);
                    }
                }
                else
                {
                    v.Error(T.EBSC_StrikeSizes, E.EBSC_E_StrikeSizeNoEBLC, m_tag);
                }
            }


            return bRet;
        }


    }
}
