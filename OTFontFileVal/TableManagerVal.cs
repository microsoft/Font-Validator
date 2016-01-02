using System;

using OTFontFile;


namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for TableManager.
    /// </summary>
    public class TableManagerVal : TableManager
    {
        /************************
         * constructors
         */
        
        
        public TableManagerVal(OTFile file) : base(file)
        {

        }


        /************************
         * public methods
         */

        public override OTTable CreateTableObject(OTTag tag, MBOBuffer buf)
        {
            OTTable table = null;

            string sName = GetUnaliasedTableName(tag);

            switch (sName)
            {
                case "BASE": table = new val_BASE(tag, buf); break;
                case "CFF ": table = new val_CFF(tag, buf); break;
                case "cmap": table = new val_cmap(tag, buf); break;
                case "cvt ": table = new val_cvt(tag, buf); break;
                case "DSIG": table = new val_DSIG(tag, buf); break;
                case "EBDT": table = new val_EBDT(tag, buf); break;
                case "EBLC": table = new val_EBLC(tag, buf); break;
                case "EBSC": table = new val_EBSC(tag, buf); break;
                case "fpgm": table = new val_fpgm(tag, buf); break;
                case "gasp": table = new val_gasp(tag, buf); break;
                case "GDEF": table = new val_GDEF(tag, buf); break;
                case "glyf": table = new val_glyf(tag, buf); break;
                case "GPOS": table = new val_GPOS(tag, buf); break;
                case "GSUB": table = new val_GSUB(tag, buf); break;
                case "hdmx": table = new val_hdmx(tag, buf); break;
                case "head": table = new val_head(tag, buf); break;
                case "hhea": table = new val_hhea(tag, buf); break;
                case "hmtx": table = new val_hmtx(tag, buf); break;
                case "JSTF": table = new val_JSTF(tag, buf); break;
                case "kern": table = new val_kern(tag, buf); break;
                case "loca": table = new val_loca(tag, buf); break;
                case "LTSH": table = new val_LTSH(tag, buf); break;
                case "maxp": table = new val_maxp(tag, buf); break;
                case "name": table = new val_name(tag, buf); break;
                case "OS/2": table = new val_OS2(tag, buf); break;
                case "PCLT": table = new val_PCLT(tag, buf); break;
                case "post": table = new val_post(tag, buf); break;
                case "prep": table = new val_prep(tag, buf); break;
                case "SVG ": table = new val_SVG(tag, buf); break;
                case "VDMX": table = new val_VDMX(tag, buf); break;
                case "vhea": table = new val_vhea(tag, buf); break;
                case "vmtx": table = new val_vmtx(tag, buf); break;
                case "VORG": table = new val_VORG(tag, buf); break;
                //case "Zapf": table = new val_Zapf(tag, buf); break;

                default: table = new val__Unknown(tag, buf); break;
            }

            return table;
        }
        
        
        /************************
         * protected methods
         */

    }
}
