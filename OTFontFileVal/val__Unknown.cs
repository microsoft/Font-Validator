using System;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val__Unknown.
    /// </summary>
    public class val__Unknown : Table__Unknown, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val__Unknown(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            string sDetails = "";

            string [] Apple_Tables = 
                {
                    "acnt", "avar", "bdat", "bhed", "bloc", "bsln", "cmap", "cvar", "cvt ",
                    "EBSC", "fdsc", "feat", "fmtx", "fpgm", "gasp", "glyf", "gvar", "hdmx",
                    "head", "hhea", "hmtx", "hsty", "just", "kern", "lcar", "loca", "maxp",
                    "mort", "morx", "name", "opbd", "OS/2", "post", "prep", "prop", "trak",
                    "vhea", "vmtx", "Zapf"
                };

            string [] VOLT_only_Tables = 
                {
                    "TSIV"
                };

            string [] VOLT_VTT_shared_Tables = 
                {
                    "TSIS", "TSIP", "TSID"
                };

            string [] VTT_only_Tables =
                {
                    "TSI0", "TSI1", "TSI2", "TSI3", "TSI4", "TSI5" ,"TSIJ", "TSIB"
                };

            bool bIdentified = false;

            for (int i=0; i<Apple_Tables.Length; i++)
            {
                if (Apple_Tables[i] == (string)m_tag)
                {
                    sDetails = "This table type is defined in the Apple TrueType spec.";
                    bIdentified = true;
                }
            }

            if (!bIdentified)
            {
                for (int i=0; i<VOLT_only_Tables.Length; i++)
                {
                    if (VOLT_only_Tables[i] == (string)m_tag)
                    {
                        sDetails = "This table type is used by the VOLT tool.";
                        bIdentified = true;
                    }
                }
            }

            if (!bIdentified)
            {
                for (int i=0; i<VOLT_VTT_shared_Tables.Length; i++)
                {
                    if (VOLT_VTT_shared_Tables[i] == (string)m_tag)
                    {
                        sDetails = "This table type is used by the VOLT tool and the VTT tool.";
                        bIdentified = true;
                    }
                }
            }

            if (!bIdentified)
            {
                for (int i=0; i<VTT_only_Tables.Length; i++)
                {
                    if (VTT_only_Tables[i] == (string)m_tag)
                    {
                        sDetails = "This table type is used by the VTT tool.";
                        bIdentified = true;
                    }
                }
            }

            v.Info(T.T_NULL, I._Table_I_Non_OT_Table, m_tag, sDetails);

            // Since there is no way to do any validation on unknown tables
            // always return true

            return true;
        }
    }
}
