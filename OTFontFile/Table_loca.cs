using System;
using System.Diagnostics;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_loca.
    /// </summary>
    public class Table_loca : OTTable
    {
        protected const int ValueInvalid = -1; 

        /*
         *        MEMBERS
         */
        private short    m_format;            // from head
        private int        m_lengthGlyf;            // from glyf
        private int        m_numGlyph;            // from maxp
        /*
         *        METHODS: ACCESS
         */
        protected int LengthGlyf(OTFont fontOwner)
        {
            if (this.m_lengthGlyf==Table_loca.ValueInvalid)
            {
                DirectoryEntry deGlyf=fontOwner.GetDirectoryEntry("glyf");
                if (deGlyf != null)
                {
                    this.m_lengthGlyf=(int)deGlyf.length;
                }
            }
            return this.m_lengthGlyf;
        }

        protected int Format(OTFont fontOwner)
        {
            // sets format without validating it
            if (this.m_format==Table_loca.ValueInvalid)
            {
                Table_head headTable = (Table_head)fontOwner.GetTable("head");
                if (headTable!=null)
                {
                    this.m_format=headTable.indexToLocFormat;
                }
            }
            return this.m_format;
        }

        protected int NumGlyph(OTFont fontOwner)
        {
            if (this.m_numGlyph==Table_loca.ValueInvalid)
            {
                Table_maxp tableMaxp = (Table_maxp)fontOwner.GetTable("maxp");
                if (tableMaxp != null)
                {
                    this.m_numGlyph = tableMaxp.NumGlyphs;
                }
            }
            return this.m_numGlyph;
        }
        
        protected int SizeEntry(OTFont fontOwner)
        {
            this.Format(fontOwner);
            if ((this.m_format!=0)&&(this.m_format!=1))
            {
                return Table_loca.ValueInvalid;
            }
            return (this.m_format==0)?2:4;
        }

        public int NumEntry(OTFont fontOwner) 
        {
            int sizeEntry=this.SizeEntry(fontOwner);
            if (sizeEntry==Table_loca.ValueInvalid)
            {
                return Table_loca.ValueInvalid;
            }
            else
            {
                return (int)(this.m_bufTable.GetLength()/sizeEntry);
            }
        }




        /*
         *        CONSTRUCTORS
         */

        public Table_loca(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
            this.m_format=Table_loca.ValueInvalid;
            this.m_lengthGlyf=Table_loca.ValueInvalid;;
            this.m_numGlyph=Table_loca.ValueInvalid;
        }


        /*
         *        METHODS PROTECTED: GLYF ACCESS
         */

        protected bool GetGlyfOffset(int indexGlyph, out int offsGlyf, OTFont fontOwner)
        {
            offsGlyf=Table_loca.ValueInvalid;
            int numEntry=this.NumEntry(fontOwner);
            if (numEntry==Table_loca.ValueInvalid)
                return false;
            if ((indexGlyph<0)||(indexGlyph>=numEntry))
            {
                //Debug.Assert(false, "Table_loca: GetGlyfOffset");
                return false;
            }

            int sizeEntry=this.SizeEntry(fontOwner);
            if (sizeEntry==Table_loca.ValueInvalid)
                return false;
            int pozLoca=sizeEntry*indexGlyph;
            if (pozLoca+sizeEntry>this.GetLength())
            {
                //Offset Exceeds Table Length
                return false;
            }
            if (sizeEntry==2)
            {
                offsGlyf = 2*m_bufTable.GetUshort((uint)pozLoca);
            }
            else
            {
                offsGlyf = (int)(m_bufTable.GetUint((uint)pozLoca));
            }
            return true;
        }

        /*
         *        METHODS PUBLIC: GLYF ACCESS
         */        
        public bool GetEntryGlyf(int indexGlyph, 
            out int offsStart, out int length, OTFont fontOwner)
        {
            offsStart=Table_loca.ValueInvalid;
            length=Table_loca.ValueInvalid;

            int offsGlyfCur, offsGlyfNext; 
            if  ((!this.GetGlyfOffset(indexGlyph,out offsGlyfCur, fontOwner))||
                (!this.GetGlyfOffset(indexGlyph+1,out offsGlyfNext, fontOwner)))
            {
                return false; // the error is already reported
            }

            int lengthGlyf=this.LengthGlyf(fontOwner);
            if (lengthGlyf==Table_loca.ValueInvalid)
            {
                return false;
            }

            if ((offsGlyfCur<0)||(offsGlyfCur>=lengthGlyf))
            {
                // Offset Within Glyf Range
                return false;
            }
            if ((offsGlyfNext<0)||(offsGlyfNext>=lengthGlyf))
            {
                int numEntry=this.NumEntry(fontOwner);
                if ((indexGlyph!=numEntry-2)||(offsGlyfNext!=lengthGlyf))
                {
                    // Offset Within Glyf Range
                    return false;
                }
            } 

            int lengthGlyfCur=offsGlyfNext-offsGlyfCur;
            if (lengthGlyfCur<0)
            {
                // offsets increasing error
                return false;
            }
            offsStart=offsGlyfCur;
            length=lengthGlyfCur;
            return true;
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new loca_cache();
            }

            return m_cache;
        }
        
        public class loca_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        

    }
}
        
