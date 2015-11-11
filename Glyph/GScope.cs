using System;
using NS_GMath;

namespace NS_Glyph
{
    public abstract class GScope
    {
        // constructors
        private GScope() 
        {
        }

        // enum
        public enum TypeGElem { _GGB_=0, _GGO_, _GGC_, _GGI_, _GB_ };

        public enum TypeGScope    {_SEARCH_ANY_=-2, _UNDEF_=-1, 
                             _GGB_=0, _GGO_, _GGC_, _GGI_, _GB_, 
                             _GG_, _G_};

        /*
         *        PROPERTIES
         */
        internal static int NumGelem
        {
            get { return Enum.GetValues(typeof(GScope.TypeGElem)).Length; }
        }

        // UNDEF_" is never combined with other strings
        public static string StrGScope(params TypeGScope[] scopes)
        {
            string strAux="";
            string strRes="";
            foreach (TypeGScope scope in scopes)
            {
                strAux+=TypeToStr(scope);
            }
            // delete duplicates
            foreach (string gelem in Enum.GetNames(typeof(GScope.TypeGElem)))
            {
                if (strAux.IndexOf(gelem)!=GConsts.POZ_INVALID)
                    strRes+=gelem;
            }
            return strRes;
        }

        private static string TypeToStr(TypeGScope type)
        {
            string strRes;
            switch (type)
            {
                case TypeGScope._UNDEF_        :
                case TypeGScope._SEARCH_ANY_: strRes=""; break;
                case TypeGScope._G_            : strRes="_GGB__GGO__GGC__GGI__GB_"; break;
                case TypeGScope._GG_        : strRes="_GGB__GGO__GGC__GGI_"; break;
                default                    : strRes=Enum.GetName(typeof(TypeGScope),type); break;
            }
            return strRes;
        }

        public static bool GElemEmpty(string str)
        {
            foreach (string gelem in Enum.GetNames(typeof(GScope.TypeGElem)))
            {
                if (str.IndexOf(gelem)!=GConsts.POZ_INVALID)
                    return false;
            }
            return true;
        }

        public static bool GScUndef(string str)
        {
            return GElemEmpty(str);
        }
            
        public static bool GElemIncluded(string strA, string strB)
        {
            if (GElemEmpty(strA)||GElemEmpty(strB))
                return false;
            foreach (string gelem in Enum.GetNames(typeof(GScope.TypeGElem)))
            {
                if ((strA.IndexOf(gelem)!=GConsts.POZ_INVALID)&&
                    (strB.IndexOf(gelem)==GConsts.POZ_INVALID))
                    return false;
            }
            return true;
        }

        public static bool GScInterfer(GScope.TypeGScope scope, string str)
        {
            switch (scope)
            {
                case TypeGScope._SEARCH_ANY_:
                    return true;
                case TypeGScope._UNDEF_:
                    return GScUndef(str);
                default:
                    return GElemInterfer(StrGScope(scope),str);
            }
        }

        public static bool GElemInterfer(string strA, string strB)
        {
            if (GElemEmpty(strA)||GElemEmpty(strB))
                return false;
            foreach (string gelem in Enum.GetNames(typeof(GScope.TypeGElem)))
            {
                if ((strA.IndexOf(gelem)!=GConsts.POZ_INVALID)&&
                    (strB.IndexOf(gelem)!=GConsts.POZ_INVALID))
                    return true;
            }
            return false;
        }
    }
}