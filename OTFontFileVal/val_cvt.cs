using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_cvt.
    /// </summary>
    public class val_cvt : Table_cvt, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_cvt(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.cvt_length))
            {
                if ((m_bufTable.GetLength() & 1) != 0)
                {
                    v.Error(T.cvt_length, E.cvt_E_length_odd, m_tag);
                    bRet = false;
                }
                else
                {
                    v.Pass(T.cvt_length, P.cvt_P_length_even, m_tag);
                }
            }
            
            return bRet;
        }

    }
}
