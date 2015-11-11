using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_CFF.
    /// </summary>
    public class val_CFF : Table_CFF, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_CFF(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */

        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            v.Info(T.T_NULL, I.CFF_I_Version, m_tag, major + "." + minor);
            v.Info(I.CFF_I_NotValidated, m_tag);
            
            return bRet;
        }

        
    }
}
