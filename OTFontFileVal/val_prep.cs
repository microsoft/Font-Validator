using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_prep.
    /// </summary>
    public class val_prep : Table_prep, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_prep(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            v.Info(I.prep_I_NotValidated, m_tag);
            
            return bRet;
        }
    }
}
