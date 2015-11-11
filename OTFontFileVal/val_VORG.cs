using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_VORG.
    /// </summary>
    public class val_VORG : Table_VORG, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_VORG(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.VORG_CFF_Font))
            {
                if (fontOwner.ContainsPostScriptOutlines())
                {
                    v.Pass(T.VORG_CFF_Font, P.VORG_P_CFF_Font, m_tag);
                }
                else
                {
                    v.Error(T.VORG_CFF_Font, E.VORG_E_CFF_Font, m_tag);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.VORG_Version))
            {
                if (majorVersion == 1 && minorVersion == 0)
                {
                    v.Pass(T.VORG_Version, P.VORG_P_Version, m_tag);
                }
                else
                {
                    v.Error(T.VORG_Version, E.VORG_E_Version, m_tag);
                    bRet = false;
                }
            }

            return bRet;
        }

    }
}
