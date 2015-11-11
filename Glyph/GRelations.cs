using System;
using System.Collections;

namespace NS_Glyph
{
    class GRelations
    {
        private Hashtable dependComp;
        
        public GRelations()
        {
            this.dependComp=new Hashtable();
        }

        internal void ClearReset()
        {
            foreach (DictionaryEntry entry in this.dependComp)
            {
                ArrayList arr=entry.Value as ArrayList;
                if (arr!=null)
                {
                    arr.Clear();
                }
            }
            this.dependComp.Clear();

        }

        internal void AddCompDep(int indGlyph, int indComponent)
        {
            if (this.dependComp[indComponent]==null)
            {
                this.dependComp[indComponent]=new ArrayList();
            }
            ArrayList arr=this.dependComp[indComponent] as ArrayList;
            if (!arr.Contains(indGlyph))
            {
                arr.Add(indGlyph);
            }
        }

        internal ArrayList GetCompDep(int indComponent)
        {
            return (this.dependComp[indComponent] as ArrayList);
        }
    }


}