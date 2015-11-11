using System;
using System.Diagnostics;

using OTFontFile;

using NS_ValCommon;
using NS_Glyph;
using NS_GMath;


namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_glyf.
    /// </summary>
    public class val_glyf : Table_glyf, ITableValidate
    {
        public val_glyf(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        public bool Validate(Validator validator, OTFontVal fontOwner)
        {
            bool bRet = true;
            if (!validator.PerformTest(T.glyf_ValidateAll))
            {
                return true;
            }

            this.m_diaValidate=validator.DIA;
            this.m_cnts=new int[this.m_namesInfoCnt.Length];
            for (int iCnt=0; iCnt<this.m_cnts.Length; iCnt++)
            {
                this.m_cnts[iCnt]=0;
            }

            I_IOGlyphsFile i_IOGlyphs=new I_IOGlyphsFile();
            if (!i_IOGlyphs.Initialize(fontOwner,validator))
            {
                return false; // the error is already reported
            }

            DIAction diaFilter=
                DIActionBuilder.DIA(this,"DIAFunc_Filter");
            FManager fm=new FManager(i_IOGlyphs, null, null);
            int numGlyph=fm.FNumGlyph;
            int indGlyph;
            for (indGlyph=0; indGlyph<numGlyph; indGlyph++)
            {
                try
                {
                    validator.OnTableProgress("Validating glyph with index "+indGlyph+" (out of "+numGlyph+" glyphs)");
                    Glyph glyph=fm.GGet(indGlyph);
                    glyph.GValidate();
                    bRet &= fm.GErrGetInformed(indGlyph,diaFilter);
                    fm.ClearManagementStructs();
                }
                catch
                {
                    validator.Error(T.T_NULL, E.glyf_E_ExceptionUnhandeled, (OTTag)"glyf",
                        "Glyph index "+indGlyph);
                }
                if (validator.CancelFlag)
                    break;
            }
            i_IOGlyphs.Clear();
            fm.ClearDestroy();    
            fm=null;
                    
/*            
            I_ProgressUpdater i_ProgressUpdater=new ValidationCancel(validator);

            FManager fm=new FManager(i_IOGlyphs, null, null);
            DIAction diaFilter=
                DIActionBuilder.DIA(this,"DIAFunc_Filter");

            fm.GErrActionAdd(diaFilter,
                FManager.TypeActionOnErr.onAdd);
            
            
            fm.FValidate(GConsts.IND_UNINITIALIZED,
                GConsts.IND_UNINITIALIZED);

            i_IOGlyphs.Clear();
            i_ProgressUpdater.Clear();
            fm.ClearDestroy();    
            fm=null;
*/
            for (int iCnt=0; iCnt<this.m_cnts.Length; iCnt++)
            {
                if (this.m_cnts[iCnt]>0)
                {
                    bool isGErr=this.m_namesInfoCnt[iCnt].StartsWith("GERR_");
                    string nameFileErr=isGErr? GErrConsts.FILE_RES_GERR_STRINGS: GErrConsts.FILE_RES_OTFFERR_STRINGS;
                    string nameAsmFileErr=isGErr? GErrConsts.ASM_RES_GERR_STRINGS: GErrConsts.ASM_RES_OTFFERR_STRINGS;
                    string strDetails="Number of glyphs with the warning = "+this.m_cnts[iCnt];
                    if (validator.CancelFlag)
                        strDetails+=" (Validation cancelled)";
                    ValInfoBasic info=new ValInfoBasic(
                        ValInfoBasic.ValInfoType.Warning,
                        this.m_namesInfoCnt[iCnt],
                        strDetails,
                        nameFileErr,
                        nameAsmFileErr,
                        "glyf",
                        null);
                    validator.DIA(info);
                }
            }

            this.m_cnts=null;
            return bRet;
        }



        /*===============================================================
         * 
         *            I_IOGlyphsFont:    IMPLEMENTATION & AUXILLIARY FUNCTIONS
         * 
         *==============================================================*/

        private DIAction m_diaValidate;
        private string[] m_namesInfoCnt=
            {
                "glyf_W_CompositeReservedBit",
                "GERR_CONT_DEGEN"
            };
        private int[] m_cnts;

        private void DIAFunc_Filter(ValInfoBasic info)
        {
            if ((string)(info.TagPrincipal)=="loca")
                return;
            for (int iCnt=0; iCnt<this.m_cnts.Length; iCnt++)
            {
                if (info.Name==this.m_namesInfoCnt[iCnt])
                {
                    this.m_cnts[iCnt]++;
                    return;
                }
            }
            
            this.m_diaValidate(info);
        }




    }
}
