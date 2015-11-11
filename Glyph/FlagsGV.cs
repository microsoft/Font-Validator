using System;
using System.Diagnostics;

namespace NS_Glyph
{
    public class FlagsGV : Flags
    {
        /*
         *        CONSTRUCTORS
         */

        public FlagsGV(): base(typeof(DefsGV.TypeGV))
        {
            this.SetAll(false);
        }

        // indexer
        override public bool this [object flag]
        {
            get 
            {
                if (!Enum.IsDefined(typeof(DefsGV.TypeGV),flag))
                {
                    throw new ExceptionGlyph("FlagsGV","indexer(get)",null);
                }
                return ((bool)this.vals[(int)flag]);
            }
            set
            {
                if (!Enum.IsDefined(typeof(DefsGV.TypeGV),flag))
                {
                    throw new ExceptionGlyph("FlagsGV","indexer(set)",null);
                }
                DefsGV.TypeGV typeGV=(DefsGV.TypeGV)flag;
                if (value==true)
                {
                    this.vals[(int)typeGV]=true;
                    DefsGV.TypeGV[] typesPreRequired=DefsGV.GetTestsPreRequired(typeGV);
                    if (typesPreRequired!=null)
                    {
                        foreach (DefsGV.TypeGV typePreRequired in typesPreRequired)
                            this.vals[(int)typePreRequired]=true;
                    }
                }
                else
                {
                    this.vals[(int)typeGV]=false;
                    DefsGV.TypeGV[] typesDependent=DefsGV.GetTestsDependent(typeGV);
                    if (typesDependent!=null)
                    {
                        foreach (DefsGV.TypeGV typeDependent in typesDependent)
                            this.vals[(int)typeDependent]=false;
                    }
                }
            }
        }

    }
}