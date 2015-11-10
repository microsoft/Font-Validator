using System;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for TableManager.
    /// </summary>
    public class TableManager
    {
        /************************
         * constructors
         */
        
        
        public TableManager(OTFile file)
        {
            m_file = file;
            CachedTables = new System.Collections.ArrayList();
        }


        /************************
         * public methods
         */
        
        
        public OTTable GetTable(OTFont fontOwner, DirectoryEntry de)
        {
            // first try getting it from the table cache
            OTTable table = GetTableFromCache(de);

            if (table == null)
            {
                if (   de.length != 0 
                    && de.offset != 0 
                    && de.offset < m_file.GetFileLength()
                    && de.offset + de.length <= m_file.GetFileLength())
                {
                    // read the table from the file
                    MBOBuffer buf = m_file.ReadPaddedBuffer(de.offset, de.length);

                    if (buf != null)
                    {
                        // put the buffer into a table object
                        table = CreateTableObject(de.tag, buf);

                        // add the table to the cache
                        CachedTables.Add(table);
                    }
                }
            }

            return table;
        }

        public string GetUnaliasedTableName(OTTag tag)
        {
            if (tag == null) return "";

            string sName = tag;
            if (sName == "bloc" || sName == "CBLC" )
            {
                sName = "EBLC";
            }
            else if (sName == "bdat" || sName == "CBDT" )
            {
                sName = "EBDT";
            }
            
            return sName;
        }

        static public string [] GetKnownOTTableTypes()
        {
            string [] sTables =
                {
                    "BASE",
                    "CBDT",
                    "CBLC",
                    "CFF ",
                    "cmap",
                    "cvt ",
                    "DSIG",
                    "EBDT",
                    "EBLC",
                    "EBSC",
                    "fpgm",
                    "gasp",
                    "GDEF",
                    "glyf",
                    "GPOS",
                    "GSUB",
                    "hdmx",
                    "head",
                    "hhea",
                    "hmtx",
                    "JSTF",
                    "kern",
                    "loca",
                    "LTSH",
                    "maxp",
                    "name",
                    "OS/2",
                    "PCLT",
                    "post",
                    "prep",
                    "VDMX",
                    "vhea",
                    "vmtx",
                    "VORG"
                };

            return sTables;
        }

        static public bool IsKnownOTTableType(OTTag tag)
        {
            bool bFound = false;

            string [] sTables = GetKnownOTTableTypes();
            for (uint i=0; i<sTables.Length; i++)
            {
                if (sTables[i] == (string)tag)
                {
                    bFound = true;
                    break;
                }
            }

            return bFound;
        }

        public virtual OTTable CreateTableObject(OTTag tag, MBOBuffer buf)
        {
            OTTable table = null;

            string sName = GetUnaliasedTableName(tag);

            switch (sName)
            {
                case "BASE": table = new Table_BASE(tag, buf); break;
                case "CFF ": table = new Table_CFF(tag, buf); break;
                case "cmap": table = new Table_cmap(tag, buf); break;
                case "cvt ": table = new Table_cvt(tag, buf); break;
                case "DSIG": table = new Table_DSIG(tag, buf); break;
                case "EBDT": table = new Table_EBDT(tag, buf); break;
                case "EBLC": table = new Table_EBLC(tag, buf); break;
                case "EBSC": table = new Table_EBSC(tag, buf); break;
                case "fpgm": table = new Table_fpgm(tag, buf); break;
                case "gasp": table = new Table_gasp(tag, buf); break;
                case "GDEF": table = new Table_GDEF(tag, buf); break;
                case "glyf": table = new Table_glyf(tag, buf); break;
                case "GPOS": table = new Table_GPOS(tag, buf); break;
                case "GSUB": table = new Table_GSUB(tag, buf); break;
                case "hdmx": table = new Table_hdmx(tag, buf); break;
                case "head": table = new Table_head(tag, buf); break;
                case "hhea": table = new Table_hhea(tag, buf); break;
                case "hmtx": table = new Table_hmtx(tag, buf); break;
                case "JSTF": table = new Table_JSTF(tag, buf); break;
                case "kern": table = new Table_kern(tag, buf); break;
                case "loca": table = new Table_loca(tag, buf); break;
                case "LTSH": table = new Table_LTSH(tag, buf); break;
                case "maxp": table = new Table_maxp(tag, buf); break;
                case "name": table = new Table_name(tag, buf); break;
                case "OS/2": table = new Table_OS2(tag, buf); break;
                case "PCLT": table = new Table_PCLT(tag, buf); break;
                case "post": table = new Table_post(tag, buf); break;
                case "prep": table = new Table_prep(tag, buf); break;
                case "VDMX": table = new Table_VDMX(tag, buf); break;
                case "vhea": table = new Table_vhea(tag, buf); break;
                case "vmtx": table = new Table_vmtx(tag, buf); break;
                case "VORG": table = new Table_VORG(tag, buf); break;
                //case "Zapf": table = new Table_Zapf(tag, buf); break;
                default: table = new Table__Unknown(tag, buf); break;
            }

            return table;
        }

        /************************
         * protected methods
         */

        protected OTTable GetTableFromCache(DirectoryEntry de)
        {
            OTTable ot = null;

            for (int i=0; i<CachedTables.Count; i++)
            {
                OTTable temp = (OTTable)CachedTables[i];
                if (temp.MatchFileOffsetLength(de.offset, de.length))
                {
                    ot = temp;
                    break;
                }
            }

            return ot;
        }

        /************************
         * member data
         */
        
        OTFile m_file;

        System.Collections.ArrayList CachedTables;
    }
}
