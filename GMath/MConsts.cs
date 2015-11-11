using System;

namespace NS_GMath
{
    public abstract class MConsts
    {
        public const double EPS_COMP=1.0e-10;
        public const double EPS_DEC=1.0e-6;
        public const double EPS_DEC_WEAK=1.0e-2;

        public const double Infinity=1.0e20;

        public enum TypeInnerOuter
        {
            Undef=-1, Outer=0, Inner=1 
        }
        public enum TypeParity
        {
            Undef=-1, Even=0, Odd=1 
        }
        public const int MAX_RAY_INTERS_TRIAL=7;
    }
}