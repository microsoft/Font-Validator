using NS_ValCommon;
using NS_GMath;


namespace NS_Glyph
{
    // class which implements the interface has its members which store all
    // necessary data

    public interface I_IOGlyphs
    {
        // should be called after constructor before any other operation
        bool Initialize(object source, 
            DIAction dia);

        /*
         *    On failure: indexGlyph=GConsts.TypeGlyph.Undef 
         */
        void ReadTypeGlyph(int indexGlyph, 
            out GConsts.TypeGlyph typeGlyph,
            DIAction dia);
        /*
         *    On failure: bbox=null 
         */
        void ReadGGB(int indexGlyph, 
            out BoxD bbox, 
            DIAction dia);
        /*
         *    On failure: outl=null 
         */
        void ReadGGO(int indexGlyph, 
            out Outline outl, 
            DIAction dia);
        /*
         *    On failure: comp=null 
         */
        void ReadGGC(int indexGlyph, 
            out Composite comp, 
            DIAction dia);
        bool ReadNumGlyph(out int numGlyph, 
            DIAction dia);
        // TODO: rewrite
        void ReadFontMetrics(out int descendMin, 
            out int ascendMax,    
            out int lsbMin, 
            out int rsbMax, 
            DIAction dia);
        void Clear();

    }
    // add any other interfaces
}
