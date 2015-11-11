using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_vhea.
    /// </summary>
    public class val_vhea : Table_vhea, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_vhea(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.vhea_version))
            {
                if (version.GetUint() == 0x00010000 || version.GetUint() == 0x00011000)
                {
                    v.Pass(T.vhea_version, P.vhea_P_version, m_tag, "0x" + version.GetUint().ToString("x8"));
                }
                else
                {
                    v.Error(T.vhea_version, E.vhea_E_version, m_tag, "0x" + version.GetUint().ToString("x8"));
                    bRet = false;
                }
            }

            if (v.PerformTest(T.vhea_ReservedFields))
            {
                if (reserved1 == 0 && reserved2 == 0 && reserved3 == 0 && reserved4 == 0)
                {
                    v.Pass(T.vhea_ReservedFields, P.vhea_P_ReservedFields, m_tag);
                }
                else
                {
                    v.Error(T.vhea_ReservedFields, E.vhea_E_ReservedFields, m_tag);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.vhea_metricDataFormat))
            {
                if (metricDataFormat == 0)
                {
                    v.Pass(T.vhea_metricDataFormat, P.vhea_P_metricDataFormat, m_tag);
                }
                else
                {
                    v.Error(T.vhea_metricDataFormat, E.vhea_E_metricDataFormat, m_tag, metricDataFormat.ToString());
                    bRet = false;
                }
            }

            return bRet;
        }




    }
}
