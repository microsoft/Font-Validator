using System;
using System.Collections;

using System.Diagnostics;
using System.Text;
using System.IO;

using NS_GMath;

namespace NS_Glyph
{
    internal class GStore
    {
        // methods
        Hashtable glyphs;    // storage of Glyphs

        // constructors
        public GStore()
        {
            this.glyphs=new Hashtable();
        }
        // methods
        public bool GAdd(Glyph glyph)
        {
            if (glyph==null)
            {
                throw new ExceptionGlyph("GStore","GAdd","Null argument");
            }
            if (glyph.IndexGlyph==GConsts.IND_UNINITIALIZED)
            {
                throw new ExceptionGlyph("GStore","GAdd",null);
            }
            this.glyphs[glyph.IndexGlyph]=glyph;
            return true;
        }

        public Glyph GGet(int indGlyph)
        {
            return (this.glyphs[indGlyph] as Glyph);
        }

        public void GDeleteTry(int indGlyph)
        {
            // does NOT delete glyph if is is different from Init (& not Saved...) 
            Glyph glyph=this.GGet(indGlyph);
            if (glyph==null)
                return;
            if (!glyph.IsDiffFromSource)
            {
                this.glyphs.Remove(indGlyph);
            }
        }
    
        public void GDeleteForce(int indGlyph)
        {
            if (this.glyphs.ContainsKey(indGlyph))
            {
                this.glyphs.Remove(indGlyph);
            }
        }

        public bool GContains(int indGlyph)
        {
            return this.glyphs.ContainsKey(indGlyph);
        }

        public void ClearReset()
        {
            foreach (DictionaryEntry entry in this.glyphs)
            {
                Glyph glyph=entry.Value as Glyph;
                if (glyph!=null)
                {
                    glyph.ClearDestroy(); // was ClearRelease()
                }
            }
            this.glyphs.Clear();
        }

        public void ClearDestroy()
        {
            foreach (DictionaryEntry entry in this.glyphs)
            {
                Glyph glyph=entry.Value as Glyph;
                if (glyph!=null)
                {
                    glyph.ClearRelease();
                }
            }
            this.glyphs.Clear();
            this.glyphs=null;
        }

        /*
        public void DumpToFile()
        {
            FileInfo infoFile=new FileInfo("GSTORE_Content.txt");
            StreamWriter writer=infoFile.AppendText();
            writer.WriteLine("\r\n\r\n===========================");
            foreach (DictionaryEntry entry in this.glyphs)
            {
                Glyph glyph=entry.Value as Glyph;
                if (!glyph.GG.IsSimple)
                {
                    writer.WriteLine(glyph.IndexGlyph+":");
                    foreach (Component component in glyph.GG.Comp)
                    {
                        writer.WriteLine("    "+component.IndexGlyphComponent+" ");
                        if (component.IndexKnotGlyph!=GConsts.IND_UNINITIALIZED)
                            writer.Write(" K ");
                        else
                            writer.Write(" - ");
                        if (component.Shift!=null)
                            writer.Write(" S ");
                        else
                            writer.Write(" - ");
                        if (component.Transform!=null)
                            writer.Write(" T ");
                        else
                            writer.Write(" - ");
                        if (component.Shift!=null)
                            writer.Write(" ("+component.Shift.X+","+component.Shift.Y+")");
                        writer.WriteLine("");
                    }                
                }
            }
            writer.Close();
        }
        */
    }
}

