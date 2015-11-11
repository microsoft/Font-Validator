using System;
using System.Diagnostics;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_vmtx.
    /// </summary>
    public class val_vmtx : Table_vmtx, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_vmtx(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }



        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            Table_maxp maxpTable = (Table_maxp)fontOwner.GetTable("maxp");
            if (maxpTable == null)
            {
                v.Error(T.T_NULL, E._TEST_E_TableMissing, m_tag, "Unable to test this table, maxp table is invalid or missing");
                return false;
            }
            
            if (v.PerformTest(T.vmtx_TableLength))
            {
                uint CalcTableLength = GetNumOfLongVerMetrics(fontOwner)*4 
                    + (fontOwner.GetMaxpNumGlyphs() - GetNumOfLongVerMetrics(fontOwner))*2;
                if (CalcTableLength == GetLength())
                {
                    v.Pass(T.vmtx_TableLength, P.vmtx_P_TableLength, m_tag);
                }
                else
                {
                    v.Error(T.vmtx_TableLength, E.vmtx_E_TableLength, m_tag);
                    bRet = false;
                }
            }

            return bRet;
        }
    }
}
