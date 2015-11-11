using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_fpgm.
    /// </summary>
    public class val_fpgm : Table_fpgm, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_fpgm(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            v.Info(I.fpgm_I_NotValidated, m_tag);
            
            return bRet;
        }
    }
}
