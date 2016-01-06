using System;
using System.Diagnostics;

using OTFontFile;
using WinVerifyTrustAPI;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_DSIG.
    /// </summary>
    public class val_DSIG : Table_DSIG, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_DSIG(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.DSIG_Formats))
            {
                bool bFormatsOk = true;
                for (uint i=0; i<usNumSigs; i++)
                {
                    SigFormatOffset sfo;
                    try
                    {
                    sfo = GetSigFormatOffset(i);
                    if (sfo.ulFormat != 1)
                    {
                        v.Error(T.DSIG_Formats, E.DSIG_E_Formats, m_tag, "block " + i + ", format = " + sfo.ulFormat);
                        bFormatsOk = false;
                        bRet = false;
                    }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        v.Error(T.DSIG_Formats, E.DSIG_E_Formats, m_tag, "block " + i + ", " + e);
                        bFormatsOk = false;
                        bRet = false;
                        break; // No point continuing
                    }
                }
                if (bFormatsOk)
                {
                    v.Pass(T.DSIG_Formats, P.DSIG_P_Formats, m_tag);
                }
            }

            if (v.PerformTest(T.DSIG_Reserved))
            {
                bool bReservedOk = true;
                for (uint i=0; i<usNumSigs; i++)
                {
                    SignatureBlock sb;
                    try
                    {
                    sb = GetSignatureBlock(i);
                    if (sb.usReserved1 != 0 || sb.usReserved2 != 0)
                    {
                        v.Error(T.DSIG_Reserved, E.DSIG_E_Reserved, m_tag, "block " + i);
                        bReservedOk = false;
                        bRet = false;
                    }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        v.Error(T.DSIG_Reserved, E.DSIG_E_Reserved, m_tag, "block " + i);
                        bReservedOk = false;
                        bRet = false;
                        break; // No point continuing
                    }
                }
                if (bReservedOk)
                {
                    v.Pass(T.DSIG_Reserved, P.DSIG_P_Reserved, m_tag);
                }
            }

            if (v.PerformTest(T.DSIG_VerifySignature))
            {
                OTFile file = fontOwner.GetFile();
                System.IO.FileStream fs = file.GetFileStream();
                String sFilename = fs.Name;
                try {
                WinVerifyTrustWrapper wvt = new WinVerifyTrustWrapper();
                if (wvt.WinVerifyTrustFile(sFilename))
                {
                    if (wvt.usNumSigs > 0)
                    {
                        if (wvt.Warn_TTCv1 || wvt.Warn_DSIG_in_memFonts)
                            v.Warning(T.DSIG_VerifySignature, W.DSIG_W_VerifySignature_Generic, m_tag,
                                   ( wvt.Warn_TTCv1 ? "TTC with v1 header;":"")
                                   + (wvt.Warn_DSIG_in_memFonts ? "DSIG tables detected in member fonts;":"") );
                        v.Pass(T.DSIG_VerifySignature, P.DSIG_P_VerifySignature, m_tag,
                               wvt.Signer);
                    }
                    else
                        v.Pass(T.DSIG_VerifySignature, P.DSIG_P_VerifySignature, m_tag);
                }
                else
                {
                    v.Error(T.DSIG_VerifySignature, E.DSIG_E_VerifySignature, m_tag);
                    bRet = false;
                }
                }
                catch (Exception e)
                {
                    v.Error(T.DSIG_VerifySignature, E.DSIG_E_VerifySignature, m_tag, e.Message);
                    bRet = false;
                }
            }

            return bRet;
        }

    }
}
