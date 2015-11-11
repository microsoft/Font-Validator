using System;

using NS_IDraw;

namespace NS_Glyph
{
    public class DrawParamGErr : DrawParam
    {
        // members
        Glyph glyph;

        // properties
        internal Glyph G
        {
            get { return this.glyph; }
        }
        // constructors
        public DrawParamGErr(Glyph glyph)
            : base(null,0)
        {
            this.glyph=glyph;
        }
        // members
        override public void ClearRelease()
        {
            this.glyph=null;
        }
    }
}