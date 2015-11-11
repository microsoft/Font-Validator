using System;

namespace NS_GMath
{
    public abstract class GConsts
    {
        public const int IND_UNDEFINED=-1;
        public const int IND_UNINITIALIZED=-2;
        public const int IND_DESTROYED=-3;
        public const int IND_ALL = -100;
        public const int IND_ANY = -200;

        public const int OFFS_INVALID = -1;
        public const int OFFS_UNINITIALIZED = -2;

        public const int POZ_INVALID = -1;

        /*
        public enum TypeStatusInit
        {
            InitFailure=-2, InitNotFound=-1, InitUninitialized=0, InitSuccess=1 
        }
        */

        public enum TypeGlyph
        {
            Undef=-2, Uninitialized=-1, Empty=0, Simple=1, Composite=2
        }

    }
}

