using System;
using System.Collections;
using System.Diagnostics;

namespace NS_Glyph
{
    public class Flags
    {
        // members 
        protected Type typeEnum;
        protected Hashtable vals;
    
        // indexer
        virtual public bool this [object flag]
        {
            get 
            {
                if (!Enum.IsDefined(typeEnum,flag))
                {
                    throw new ExceptionGlyph("Flags","indexer(get)",null);
                }
                return ((bool)this.vals[(int)flag]);
            }
            set
            {
                if (!Enum.IsDefined(typeEnum,flag))
                {
                    throw new ExceptionGlyph("Flags","indexer(set)",null);
                }
                this.vals[(int)flag]=value;
            }
        }


        // constructors
        private Flags()
        {
        }
        public Flags(Type typeEnum)
        {
            this.vals=new Hashtable();
            this.typeEnum=typeEnum;
            this.SetAll(false);
        }
        // methods
        public void SetAll(bool val)
        {
            System.Array flags=Enum.GetValues(this.typeEnum);
            foreach (int flag in flags)
            {
                this.vals[flag]=val;
            }    
        }    
        public void Clear()
        {
            this.vals.Clear();
        }

        public bool AreAll(bool val)
        {
            System.Array flags=Enum.GetValues(this.typeEnum);
            foreach (int flag in flags)
            {
                if ((bool)(this.vals[flag])!=val)
                    return false;
            }    
            return true;
        }
    }
}
