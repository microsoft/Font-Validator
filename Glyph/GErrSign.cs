using System;
using System.Text;

using System.Diagnostics;

using NS_ValCommon;
using NS_GMath;

namespace NS_Glyph
{
    public abstract class GErrSign
    {

        static public void Sign(GErr gerr, DefsGV.TypeGV typeGV, params int[] indsGlyph)
        {
            if (gerr==null)
            {
                throw new ExceptionGlyph("GErrSign","Sign","NullArgument");
            }
            StringBuilder sb=new StringBuilder("");
            sb.Append(Enum.GetName(typeof(DefsGV.TypeGV),typeGV));
            sb.Append("_");
            foreach (int indGlyph in indsGlyph)
            {
                sb.Append(indGlyph+"_");
            }
            gerr.SignCreator=sb.ToString();
        }

        static public void Sign(GErrList gerrlist, DefsGV.TypeGV typeGV, params int[] indsGlyph)
        {
            if (gerrlist==null)
            {
                throw new ExceptionGlyph("GErrSign","Sign","Null argument");
            }
            foreach (GErr gerr in gerrlist)
            {
                Sign(gerr,typeGV,indsGlyph);
            }
        }

        static public bool DICFunc_IsSignedBy(ValInfoBasic info, DefsGV.TypeGV typeGV)
        {
            GErr gerr=info as GErr;
            if (gerr==null)
            {
                throw new ExceptionGlyph("GErrSign","DICFunc_IsSignedBy",null);
            }
            string strSign=gerr.SignCreator;
            if (strSign==null)
            {
                throw new ExceptionGlyph("GErrSign","DICFunc_IsSignedBy",null);
            }
            string strTypeGV=Enum.GetName(typeof(DefsGV.TypeGV),typeGV);
            return strSign.StartsWith(strTypeGV);
        }
        
        static public bool DICFunc_IsSignedBy(ValInfoBasic info, int indGlyph)
        {
            GErr gerr=info as GErr;
            if (gerr==null)
            {
                throw new ExceptionGlyph("GErrSign","DICFunc_IsSignedBy",null);
            }
            string strSign=gerr.SignCreator;
            if (strSign==null)
            {
                throw new ExceptionGlyph("GErrSign","DICFunc_IsSignedBy",null);
            }
            string strGlyph="_"+indGlyph+"_";
            return (strSign.IndexOf(strGlyph)!=GConsts.POZ_INVALID);
        }


        static public void Parse(GErr gerr, 
            out DefsGV.TypeGV typeGV, out int[] indsGlyph)
        {
            typeGV=DefsGV.TypeGV.Invalid;
            indsGlyph=null;
            string strSign=gerr.SignCreator;
            if (strSign==null)
            {
                throw new ExceptionGlyph("GErrSign","Parse",null);
            }
            string[] strs=strSign.Split('_');
            if (strs.Length<1)
            {
                throw new ExceptionGlyph("GErrSign","Parse",null);
            }
            typeGV=(DefsGV.TypeGV)Enum.Parse(typeof(DefsGV.TypeGV),strs[0]);
            if ((strs.Length)>1)
            {
                indsGlyph=new int[strs.Length-1];
            }
            for (int iInd=1; iInd<strs.Length; iInd++)
            {
                indsGlyph[iInd]=Int32.Parse(strs[iInd]);
            }
        }
        static public DefsGV.TypeGV GetTypeErrCreator(GErr gerr)
        {
            string strSign=gerr.SignCreator;
            foreach (string strTypeGV in Enum.GetNames(typeof(DefsGV.TypeGV)))
            {
                if (strSign.StartsWith(strTypeGV))
                {
                    return (DefsGV.TypeGV)(Enum.Parse(typeof(DefsGV.TypeGV),strTypeGV));
                }
            }
            throw new ExceptionGlyph("GErrSign","GetTypeErrCreator",null);
            //return DefsGV.TypeGV.Invalid;
        }

        static public bool GetIndGlyph(GErr gerr, out int indGlyph)
        {
            indGlyph=GConsts.IND_UNINITIALIZED;
            string sign=gerr.SignCreator;
            if (sign==null)
                return false;
            string[] strAux=sign.Split('_');
            if (strAux.Length<2)
                return false;
            indGlyph=Int32.Parse(strAux[1]);
            return true;
        }
    }
}




