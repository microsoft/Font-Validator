using System;

namespace NS_Glyph
{
    public class DefsGV
    {
        public enum TypeGV
        {
            Invalid=-1, 

            /*
             *        ALL TESTS
             */
            
            /*
             *        validate: source
             */
            ValidateTypeGlyphSource=0,
            ValidateSimpSource,
            ValidateCompSource,
            

            /*
             *        validate: SIMPLE
             */

            ValidateSimpBBox,
            ValidateSimpContWrap,
            ValidateSimpContDupl,
            ValidateSimpContInters,        
            ValidateSimpContMisor,
            ValidateSimpKnotDupl,
            ValidateSimpContDegen,

            /*
             *        validate: COMPOSITE
             */            
            ValidateCompBind,
            ValidateCompBBox,
            ValidateCompComponentDupl,
            ValidateCompComponentInters,
        }

        /*
         *        ORDER OF PERFORMING TESTS
         */
        internal static TypeGV[] orderTestSimp=
            {
                TypeGV.ValidateTypeGlyphSource,
                TypeGV.ValidateSimpSource,
                TypeGV.ValidateSimpBBox,
                TypeGV.ValidateSimpKnotDupl,
                TypeGV.ValidateSimpContDegen,
                TypeGV.ValidateSimpContWrap,
                TypeGV.ValidateSimpContDupl,
                TypeGV.ValidateSimpContInters,        
                TypeGV.ValidateSimpContMisor,
            };

        internal static TypeGV[] orderTestComp=
            {
                TypeGV.ValidateTypeGlyphSource,
                TypeGV.ValidateCompSource,
                TypeGV.ValidateCompBind,
                TypeGV.ValidateCompBBox,
                TypeGV.ValidateCompComponentDupl,
                TypeGV.ValidateCompComponentInters,
            };
        
        /*
         *        PRE-REQUIRED TESTS: jagged array
         */
        private static TypeGV[][] testsPreRequired;
 
        public static TypeGV[] GetTestsPreRequired(TypeGV typeGV)
        {
            if ((int)typeGV<0)
                return null;
            if (DefsGV.testsPreRequired==null)
            {
                DefsGV.InitTestsPreRequired();
            }
            return DefsGV.testsPreRequired[(int)typeGV];
        }

        public static TypeGV[] GetTestsDependent(TypeGV typeGV)
        {
            if ((int)typeGV<0)
                return null;
            if (DefsGV.testsPreRequired==null)
            {
                DefsGV.InitTestsPreRequired();
            }
            int numTest=DefsGV.testsPreRequired.GetLength(0);
            bool[] isDependent=new bool[numTest]; 
            int cntDependent=0;
            for (int iTest=0; iTest<numTest; iTest++)
            {
                isDependent[iTest]=false;
                TypeGV[] typesPre=DefsGV.testsPreRequired[iTest];
                if (typesPre!=null)
                {
                    foreach (TypeGV typePre in typesPre)
                    {
                        if (typePre==typeGV)
                        {
                            isDependent[iTest]=true;
                            cntDependent++;
                            break;
                        }
                    }
                }
            }
            TypeGV[] testsDependent=new TypeGV[cntDependent];
            cntDependent=0;
            for (int iTest=0; iTest<numTest; iTest++)
            {
                if (isDependent[iTest])
                {
                    testsDependent[cntDependent++]=(DefsGV.TypeGV)iTest;
                }
            }
            return testsDependent;
        }

        /*
         *        PROPERTIES
         */
        static public int NumTest
        {
            get 
            {
                int numTest=0;
                foreach (TypeGV typeGV in Enum.GetValues(typeof(DefsGV.TypeGV)))
                {
                    if ((int)typeGV>=0)
                        numTest++;
                }
                return numTest;
            }
        }

        /*
         *        CONSTRUCTOR
         */
        public DefsGV()
        {
            DefsGV.InitTestsPreRequired();
        }

        /*
         *        METHODS
         */
        private static void InitTestsPreRequired()
        {
            DefsGV.testsPreRequired=new TypeGV[Enum.GetValues(typeof(TypeGV)).Length][];

            // simple
            
            DefsGV.testsPreRequired[(int)TypeGV.ValidateSimpSource]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource
                };
            DefsGV.testsPreRequired[(int)TypeGV.ValidateSimpBBox]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
                    TypeGV.ValidateSimpSource
                };    
            DefsGV.testsPreRequired[(int)TypeGV.ValidateSimpContWrap]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
                    TypeGV.ValidateSimpSource
                };
            DefsGV.testsPreRequired[(int)TypeGV.ValidateSimpContDupl]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
                    TypeGV.ValidateSimpSource
                };
            DefsGV.testsPreRequired[(int)TypeGV.ValidateSimpContInters]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
                    TypeGV.ValidateSimpSource,
                    TypeGV.ValidateSimpContWrap,
                    TypeGV.ValidateSimpContDupl,
            };
            DefsGV.testsPreRequired[(int)TypeGV.ValidateSimpContMisor]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
                    TypeGV.ValidateSimpSource,
                    TypeGV.ValidateSimpContWrap,
                    TypeGV.ValidateSimpContDupl,
                    TypeGV.ValidateSimpContInters,
            };
            DefsGV.testsPreRequired[(int)TypeGV.ValidateSimpKnotDupl]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
                    TypeGV.ValidateSimpSource,
            };
            DefsGV.testsPreRequired[(int)TypeGV.ValidateSimpContDegen]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
                    TypeGV.ValidateSimpSource, // ???
                //TypeGV.SourceCompGGC,
                //TypeGV.ValidateCompBind
            };
        
            // composite
            DefsGV.testsPreRequired[(int)TypeGV.ValidateCompSource]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
            };
            DefsGV.testsPreRequired[(int)TypeGV.ValidateCompBind]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
                    TypeGV.ValidateSimpSource,
                    TypeGV.ValidateCompSource,
            };
            DefsGV.testsPreRequired[(int)TypeGV.ValidateCompBBox]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
                    TypeGV.ValidateSimpSource,
                    TypeGV.ValidateCompSource,
                    TypeGV.ValidateCompBind,
            };
            DefsGV.testsPreRequired[(int)TypeGV.ValidateCompComponentDupl]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
                    TypeGV.ValidateSimpSource,
                    TypeGV.ValidateCompSource,
                    TypeGV.ValidateCompBind,        
            };
            DefsGV.testsPreRequired[(int)TypeGV.ValidateCompComponentInters]=new TypeGV[]
                {
                    TypeGV.ValidateTypeGlyphSource,
                    TypeGV.ValidateSimpSource,
                    TypeGV.ValidateCompSource,
                    TypeGV.ValidateCompBind,
                    TypeGV.ValidateCompComponentDupl,
            };
        }
        public static bool IsSimp(TypeGV typeGV)
        {
            if (typeGV==DefsGV.TypeGV.ValidateTypeGlyphSource)
                return true;
            return DefsGV.IsPureSimp(typeGV);
        }
        public static bool IsComp(TypeGV typeGV)
        {
            if (typeGV==DefsGV.TypeGV.ValidateTypeGlyphSource)
                return true;
            return DefsGV.IsPureComp(typeGV);
        }
        public static bool IsPureSimp(TypeGV typeGV)
        {
            string strTypeGM=Enum.GetName(typeof(DefsGV.TypeGV),typeGV);
            return strTypeGM.StartsWith("ValidateSimp");
        }
        public static bool IsPureComp(TypeGV typeGV)
        {
            string strTypeGM=Enum.GetName(typeof(DefsGV.TypeGV),typeGV);
            return strTypeGM.StartsWith("ValidateComp");
        }
        public static bool IsTest(TypeGV typeGV)
        {
            return (typeGV!=DefsGV.TypeGV.Invalid);
        }

        public static bool IsSourceValidator(TypeGV typeGV)
        {
            return ((typeGV==TypeGV.ValidateTypeGlyphSource)||
                (typeGV==TypeGV.ValidateSimpSource)||
                (typeGV==TypeGV.ValidateCompSource));
        }

        public static bool IsBindingValidator(TypeGV typeGV)
        {
            return (typeGV==TypeGV.ValidateCompBind);
        }

        public static bool IsContentValidator(TypeGV typeGV)
        {
            return (DefsGV.IsTest(typeGV)&&
                (!DefsGV.IsSourceValidator(typeGV))&&
                (!DefsGV.IsBindingValidator(typeGV)));
        }

        public static bool IsModifier(TypeGV typeGV)
        {
            return (DefsGM.From(typeGV)!=DefsGM.TypeGM.Invalid);
        }
        public static bool IsPureValidator(TypeGV typeGV)
        {
            return (!DefsGV.IsModifier(typeGV));
        }
        public static TypeGV From(DefsGM.TypeGM typeGM)
        {
            string strTypeGM=Enum.GetName(typeof(DefsGM.TypeGM),typeGM);
            object obj;
            try
            {
                obj=Enum.Parse(typeof(DefsGV.TypeGV),strTypeGM);
            }
            catch
            {
                return DefsGV.TypeGV.Invalid;
            }
            return (DefsGV.TypeGV)obj;
        }

    }
}