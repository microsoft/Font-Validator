using System;

namespace NS_IDraw
{    
    public class InfoDrawable
    {
        // members
        I_Drawable    drawable;
        DrawParam    dp;

        // properties
        public I_Drawable Drawable
        {
            get { return this.drawable; }
        }
        public DrawParam DP
        { 
            get { return this.dp; }
        }

        // constructors
        private InfoDrawable()
        {
            this.drawable=null;
            this.dp=null;
        }

        public InfoDrawable(I_Drawable drawable, DrawParam dp)
            : this()
        {
            if (drawable!=null)
            {
                this.drawable=drawable;
                this.dp=dp;
            }
        }

        public void ClearRelease()
        {
            this.drawable=null;
            if (this.dp!=null)
            {
                this.dp.ClearRelease();
            }
            this.dp=null;
        }
    }
}