using System;
using System.Diagnostics;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_hmtx.
    /// </summary>
    public class val_hmtx : Table_hmtx, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_hmtx(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }



        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            Table_hhea hheaTable = (Table_hhea)fontOwner.GetTable("hhea");
            if (hheaTable == null)
            {
                v.Error(T.T_NULL, E._TEST_E_TableMissing, m_tag, "Unable to test this table, hhea table is invalid or missing");
                return false;
            }

            Table_maxp maxpTable = (Table_maxp)fontOwner.GetTable("maxp");
            if (maxpTable == null)
            {
                v.Error(T.T_NULL, E._TEST_E_TableMissing, m_tag, "Unable to test this table, maxp table is invalid or missing");
                return false;
            }
            
            if (v.PerformTest(T.hmtx_TableSize))
            {
                uint nhm = GetNumberOfHMetrics(fontOwner);
                uint nlsb = GetNumLeftSideBearingEntries(fontOwner);
                uint CalcTableLength = nhm*4 + nlsb*2;

                if (CalcTableLength == GetLength())
                {
                    v.Pass(T.hmtx_TableSize, P.hmtx_P_TableSize, m_tag);
                }
                else
                {
                    v.Error(T.hmtx_TableSize, E.hmtx_E_TableSize, m_tag);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.hmtx_CheckMetrics))
            {
                bool bMetricsOk = true;

                for (uint iGlyph=0; iGlyph<fontOwner.GetMaxpNumGlyphs(); iGlyph++)
                {
                    longHorMetric hm = this.GetOrMakeHMetric(iGlyph, fontOwner);

                    if (hm != null)
                    {
                        if (hm.lsb > hm.advanceWidth)
                        {
                            v.Warning(T.hmtx_CheckMetrics, W.hmtx_W_CheckMetrics_lsb_gt_adv, m_tag, "glyph# " + iGlyph);
                            bMetricsOk = false;
                        }
                    }
                    else
                    {
                        // unable to fetch this horizontal metric
                        // (probably bad hheaTable.numberOfHMetrics or bad table length)
                        bMetricsOk = false;
                    }
                }

                if (bMetricsOk)
                {
                    v.Pass(T.hmtx_CheckMetrics, P.hmtx_P_CheckMetrics, m_tag);
                }
            }

            return bRet;
        }


    }
}
