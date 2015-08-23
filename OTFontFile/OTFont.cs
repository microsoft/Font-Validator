using System;
using System.Diagnostics;
using System.IO;
using System.Collections;



namespace OTFontFile
{
    // The OTFont class encapsulates the Offset table and its directory entries for a font.

    public class OTFont
    {
        /***************
         * constructors
         */
        
        public OTFont()
        {
            // this constructor is for creating fonts in memory
            // as opposed to reading a font from a file

            m_File = null;
            MemBasedTables = new ArrayList();
            m_OutlineType = OutlineType.OUTLINE_INVALID;
            m_OffsetTable = new OffsetTable(new OTFixed(1,0), 0);
        }
        
        public OTFont(OTFile f)
        {
            m_File = f;
            m_FontFileNumber = 0;
            m_OffsetTable = null;
            m_maxpNumGlyphs = 0;
            m_arrUnicodeToGlyph_3_0 = null;
            m_arrUnicodeToGlyph_3_1 = null;
            m_arrUnicodeToGlyph_3_10 = null;
            m_bContainsMsSymbolEncodedCmap = false;
        }

        
        public OTFont(OTFile f, uint FontFileNumber, OffsetTable ot, OutlineType outlineType)
        {
            m_File = f;
            m_FontFileNumber = FontFileNumber;
            m_OffsetTable = ot;
            m_OutlineType = outlineType;
            m_maxpNumGlyphs = 0;
            m_arrUnicodeToGlyph_3_0 = null;
            m_arrUnicodeToGlyph_3_1 = null;
            m_arrUnicodeToGlyph_3_10 = null;
            m_bContainsMsSymbolEncodedCmap = false;
        }

        
        /************************
         * public static methods
         */
        
        
        /// <summary>Return a new OTFont by reading a single font from an 
        /// OTFile starting at a given file position. 
        /// <p/>
        /// The type of the font is set by determining whether there is
        /// a 'glyf' table or a 'CFF ' table.
        /// </summary>
        public static OTFont ReadFont(OTFile file, uint FontFileNumber, uint filepos)
        {
            OTFont f = null;

            OffsetTable ot = ReadOffsetTable(file, filepos);

            if (ot != null)
            {
                if (ot.numTables == ot.DirectoryEntries.Count)
                {
                    OutlineType olt = OutlineType.OUTLINE_INVALID;
                    for (int i = 0; i<ot.DirectoryEntries.Count; i++)
                    {
                        DirectoryEntry temp = (DirectoryEntry)ot.DirectoryEntries[i];
                        string sTable = (string)temp.tag;
                        if (sTable == "CFF ")
                        {
                            olt = OutlineType.OUTLINE_POSTSCRIPT;
                            break;
                        }
                        else if (sTable == "glyf")
                        {
                            olt = OutlineType.OUTLINE_TRUETYPE;
                            break;
                        }
                    }

                    f = new OTFont(file, FontFileNumber, ot, olt);
                }
            }
            return f;
        }

        
        /*****************
         * public methods
         */
        
        
        /// 
        /// <summary>Calculate the check sum for the font as the
        /// sum of the checksums of the offset table, directory entries, and 
        /// each table.
        /// </summary>
        public uint CalcChecksum()
        {
            uint sum = 0;

            sum += m_OffsetTable.CalcOffsetTableChecksum();
            sum += m_OffsetTable.CalcDirectoryEntriesChecksum();
            
            if (m_OffsetTable != null)
            {
                for (int i = 0; i<m_OffsetTable.DirectoryEntries.Count; i++)
                {
                    DirectoryEntry de = (DirectoryEntry)m_OffsetTable.DirectoryEntries[i];

                    OTTable table = GetTable(de);

                    uint calcChecksum = 0;
                    if (table != null)
                    {
                        calcChecksum = table.CalcChecksum();
                    }
                    sum += calcChecksum;
                }
            }

            return sum;
        }

        /// <summary>Return the ith table in the directory entries,
        /// or null if there is no such entry.
        /// </summary>
        public OTTable GetTable(ushort i)
        {
            OTTable table = null;

            DirectoryEntry de = GetDirectoryEntry(i);

            if (de != null)
            {
                table = GetTable(de);
            }

            return table;
        }

        /// <summary>Return the first table with tag == <c>tag</c>
        /// or null if there is no such table.
        /// </summary>
        public OTTable GetTable(OTTag tag)
        {
            OTTable table = null;

            // find the directory entry in this font that matches the tag
            DirectoryEntry de = GetDirectoryEntry(tag);

            if (de != null)
            {
                table = GetTable(de);
            }

            return table;
        }

        /// <summary>Return the ith directory entry,
        /// or null if there is no such entry.
        /// </summary>
        public DirectoryEntry GetDirectoryEntry(ushort i)
        {
            DirectoryEntry de = null;

            if (m_OffsetTable != null)
            {
                if (i<m_OffsetTable.numTables)
                {
                    de = (DirectoryEntry)m_OffsetTable.DirectoryEntries[i];
                }
            }

            return de;
        }

        /// <summary>Return the first directory entry with 
        /// tag == <c>tag</c>
        /// or null if there is no such entry.
        /// </summary>
        public DirectoryEntry GetDirectoryEntry(OTTag tag)
        {
            Debug.Assert(m_OffsetTable != null);

            DirectoryEntry de = null;

            if (m_OffsetTable != null)
            {
                for (int i = 0; i<m_OffsetTable.DirectoryEntries.Count; i++)
                {
                    DirectoryEntry temp = (DirectoryEntry)m_OffsetTable.DirectoryEntries[i];
                    if (temp.tag == tag)
                    {
                        de = temp;
                        break;
                    }
                }
            }

            return de;
        }

        /// <summary>Accessor for <c>m_file</c></summary>
        public OTFile GetFile()
        {
            return m_File;
        }

        /// <summary>Accessor for <c>m_FontFileNumber</c></summary>
        public uint GetFontIndexInFile()
        {
            return m_FontFileNumber;
        }

        /// <summary>Accessor for <c>m_OffstTable.numTables</c>, or 0
        /// if no offset table yet.
        /// </summary>
        public ushort GetNumTables()
        {
            ushort nTables = 0;
            if (m_OffsetTable != null)
            {
                nTables = m_OffsetTable.numTables;
            }
            return nTables;
        }

        /// <summary>Accessor for <c>m_OffstTable</c></summary>
        public OffsetTable GetOffsetTable()
        {
            return m_OffsetTable;
        }

        /// <summary>Get font name from "name" table, or <c>null</c> if
        /// no "name" table yet.</summary>
        public string GetFontName()
        {
            string sName = null;

            Table_name nameTable = (Table_name)GetTable("name");
            if (nameTable != null)
            {
                sName = nameTable.GetNameString();
            }

            return sName;
        }

        /// <summary>Get font version from "name" table, or <c>null</c> if
        /// no "name" table yet.</summary>
        public string GetFontVersion()
        {
            string sVersion = null;

            Table_name nameTable = (Table_name)GetTable("name");
            if (nameTable != null)
            {
                sVersion = nameTable.GetVersionString();
            }

            return sVersion;
        }

        /// <summary>Get modified date from "name" table, or <c>1/1/1904</c> if
        /// no "name" table yet.</summary>
        public DateTime GetFontModifiedDate()
        {
            // default to Jan 1, 1904 if unavailable
            DateTime dt = new DateTime(1904, 1, 1);

            try
            {
                Table_head headTable = (Table_head)GetTable("head");
                if (headTable != null)
                {
                    dt = headTable.GetModifiedDateTime();
                }
            }
            catch
            {
            }

            return dt;
        }

        /// <summary>Add a new table to an non-file based table. Table cannot
        /// already exist in the font.
        /// </summary>
        public void AddTable(OTTable t)
        {
            if (m_File != null)
            {
                throw new ApplicationException("attempted to add a table to a file-based OTFont object");
            }

            if (GetDirectoryEntry(t.m_tag) != null)
            {
                throw new ArgumentException("the specified table already exists in the font");
            }

            // build a new directory entry
            DirectoryEntry de = new DirectoryEntry();
            de.tag = new OTTag(t.m_tag.GetBytes());
            de.checkSum = t.CalcChecksum();
            de.offset = 0;  // this value won't get fixed up until the font is written
            de.length = t.GetLength();

            // add the directory entry

            m_OffsetTable.DirectoryEntries.Add(de);    
            m_OffsetTable.numTables++;
            

            // add the table to the list of tables in memory

            MemBasedTables.Add(t);


            // special handling for certain tables

            string sTable = (string)t.m_tag;
            if (sTable == "CFF ")
            {
                m_OutlineType = OutlineType.OUTLINE_POSTSCRIPT;
            }
            else if (sTable == "glyf")
            {
                m_OutlineType = OutlineType.OUTLINE_TRUETYPE;
            }
        }

        /// <summary>Rmove a table to an non-file based table. Throws
        /// exception if table is not in font.
        /// </summary>
        public void RemoveTable(OTTag tag)
        {
            if (m_File != null)
            {
                throw new ApplicationException("attempted to remove a table from a file-based OTFont object");
            }

            if (GetDirectoryEntry(tag) == null)
            {
                throw new ArgumentException("the specified table doesn't doesn't exist in the font");
            }


            // remove the directory entry

            for (int i=0; i<m_OffsetTable.DirectoryEntries.Count; i++)
            {
                DirectoryEntry de = (DirectoryEntry)m_OffsetTable.DirectoryEntries[i];
                if (tag == de.tag)
                {
                    m_OffsetTable.DirectoryEntries.RemoveAt(i);
                    m_OffsetTable.numTables--;
                    break;
                }
            }
            

            // remove the table from the list of tables in memory

            for (int i=0; i<MemBasedTables.Count; i++)
            {
                OTTable t = (OTTable)MemBasedTables[i];
                if (tag == t.m_tag)
                {
                    MemBasedTables.RemoveAt(i);
                    break;
                }
            }

            string sTable = (string)tag;
            if (sTable == "CFF ")
            {
                m_OutlineType = OutlineType.OUTLINE_INVALID;
            }
            else if (sTable == "glyf")
            {
                m_OutlineType = OutlineType.OUTLINE_INVALID;
            }
        }

        /*******************************
         * public methods
         * that cache information
         * from various tables
         */

        public bool ContainsPostScriptOutlines()
        {
            if (m_OutlineType == OutlineType.OUTLINE_POSTSCRIPT)
                return true;
            else
                return false;
        }

        public bool ContainsTrueTypeOutlines()
        {
            if (m_OutlineType == OutlineType.OUTLINE_TRUETYPE)
                return true;
            else
                return false;
        }

        public ushort GetMaxpNumGlyphs()
        {
            // this routine caches the maxp.numGlyphs value for better performance
            if (m_maxpNumGlyphs == 0)
            {
                Table_maxp maxpTable = (Table_maxp)GetTable("maxp");

                if (maxpTable != null)
                {
                    m_maxpNumGlyphs = maxpTable.NumGlyphs;
                }
                else
                {
                    throw new InvalidOperationException("maxp table missing or invalid");
                }
            }

            return m_maxpNumGlyphs;
        }

        public uint FastMapUnicodeToGlyphID(char c)
        {
            // this routine caches the windows unicode cmap for better performance
            // but uses 256K of heap

            uint glyphID = 0xffffffff;

            if (m_arrUnicodeToGlyph_3_1 == null)
            {
                m_arrUnicodeToGlyph_3_1 = new uint[0x00010000];
                for (uint i=0; i<=0xffff; i++)
                {
                    m_arrUnicodeToGlyph_3_1[i] = 0;
                }

                Table_cmap cmapTable = (Table_cmap)GetTable("cmap");
                if (cmapTable != null)
                {
                    Table_cmap.EncodingTableEntry eteUni = cmapTable.GetEncodingTableEntry(3,1);
                    if (eteUni != null)
                    {
                        Table_cmap.Subtable st = cmapTable.GetSubtable(eteUni);

                        if (st != null)
                        {

                            byte [] buf = new byte[2];
                            for (ushort i=0; i<0xffff; i++)
                            {
                                buf[0] = (byte)i;
                                buf[1] = (byte)(i>>8);

                                uint g = st.MapCharToGlyph(buf, 0);
                                if (g!=0)
                                {
                                    // BEWARE: Some bad fonts have the glyph ID greater than # of glyphs
                                    // and we're not throwing it out here!
                                    m_arrUnicodeToGlyph_3_1[i] = g;
                                }
                            }
                        }
                    }
                }
            }

            Debug.Assert(m_arrUnicodeToGlyph_3_1 != null);
            if (m_arrUnicodeToGlyph_3_1 != null)
            {
                glyphID = m_arrUnicodeToGlyph_3_1[c];
            }

            return glyphID;
        }


        public uint FastMapUnicode32ToGlyphID(uint c)
        {
            // this routine caches the windows 3,10 cmap for better performance
            // but might use tons of heap

            uint glyphID = 0xffffffff;

            if (m_arrUnicodeToGlyph_3_10 == null)
            {
                Table_cmap cmapTable = (Table_cmap)GetTable("cmap");
                if (cmapTable != null)
                {
                    Table_cmap.Format12 subtable = (Table_cmap.Format12)cmapTable.GetSubtable(3,10);

                    // Apple Color Emoji does not have a 3.10 charmap
                    if (subtable == null)
                        return glyphID;

                    Table_cmap.Format12.Group g = subtable.GetGroup(subtable.nGroups-1);
                    uint nArraySize = g.endCharCode + 1;


                    m_arrUnicodeToGlyph_3_10 = new uint[nArraySize];
                    for (uint i=0; i<nArraySize; i++)
                    {
                        m_arrUnicodeToGlyph_3_10[i] = 0;
                    }

                    for (uint nGroup = 0; nGroup<subtable.nGroups; nGroup++)
                    {
                        g = subtable.GetGroup(nGroup);

                        for (uint i=0; i<=g.endCharCode-g.startCharCode; i++)
                        {
                            m_arrUnicodeToGlyph_3_10[g.startCharCode + i] = g.startGlyphID + i;
                        }
                    }
                }
            }

            Debug.Assert(m_arrUnicodeToGlyph_3_10 != null);
            if (m_arrUnicodeToGlyph_3_10 != null)
            {
                glyphID = m_arrUnicodeToGlyph_3_10[c];
            }

            return glyphID;
        }

        public char MapGlyphIDToUnicode(uint nGlyphID, char Start)
        {
            char unicode = (char)0xffff; // return this (illegal) char on failure

            for (char c=Start; c<0xffff; c++)
            {
                uint g = FastMapUnicodeToGlyphID(c);
                if (g == nGlyphID)
                {
                    unicode = c;
                    break;
                }
            }

            return unicode;
        }

        public uint MapGlyphIDToUnicode32(uint nGlyphID, uint Start)
        {
            uint uni32 = 0xffffffff; // return this (illegal) char on failure

            FastMapUnicode32ToGlyphID(0); // force the map to be calculated

            // Apple Color Emoji does not have a 3.10 charmap
            if (m_arrUnicodeToGlyph_3_10 == null)
                return uni32;

            for (uint c=Start; c<m_arrUnicodeToGlyph_3_10.Length; c++)
            {
                uint g = FastMapUnicode32ToGlyphID(c);
                if (g == nGlyphID)
                {
                    uni32 = c;
                    break;
                }
            }

            return uni32;
        }

        bool bCheckedForSymbolCmap = false;
        public bool ContainsMsSymbolEncodedCmap()
        {
            if (!bCheckedForSymbolCmap)
            {
                Table_cmap cmapTable = (Table_cmap)GetTable("cmap");
                if (cmapTable != null)
                {
                    Table_cmap.EncodingTableEntry eteUni = cmapTable.GetEncodingTableEntry(3,0);
                    if (eteUni != null)
                    {
                        m_bContainsMsSymbolEncodedCmap = true;
                    }
                }
                bCheckedForSymbolCmap = true;
            }

            return m_bContainsMsSymbolEncodedCmap;
        }

        public bool ContainsSymbolsOnly()
        {
            // check for a 3,0 cmap, if found then it's a symbol only font
            bool bSymbolOnly = ContainsMsSymbolEncodedCmap();

            // if it didn't contain a 3,0 cmap, then check for unicode symbols in the font
            if (!bSymbolOnly)
            {
                int nUnicodeSymbols = 0;

                for (ushort c=0xf000; c<= 0xf0ff; c++)
                {
                    uint nGlyphID = 0;
                    try
                    {
                        nGlyphID = FastMapUnicodeToGlyphID((char)c);
                    }
                    catch
                    {
                    }

                    if (nGlyphID != 0)
                    {
                        nUnicodeSymbols++;
                    }
                }
                
                if (GetTable("maxp") != null)
                {
                    if (nUnicodeSymbols == GetMaxpNumGlyphs())
                    {
                        bSymbolOnly = true;
                    }
                }
            }

            return bSymbolOnly;
        }

        public bool ContainsLatinOnly()
        {
            bool bLatinOnly = true;

            // check for a 3,0 cmap, if found then it's a symbol only font
            if (ContainsMsSymbolEncodedCmap()) bLatinOnly = false;

            if (bLatinOnly)
            {
                // unicode ranges Basic Latin, Latin-1 Supplement, Latin Extended-A and LatinExtended-B range from 0 to 0x024f
                FastMapUnicodeToGlyphID(' '); // force the cmap to be cached

                for (uint i=0x0250; i<0xffff; i++)
                {
                    if (m_arrUnicodeToGlyph_3_1[i] != 0)
                    {
                        bLatinOnly = false;
                        break;
                    }
                }
            }

            return bLatinOnly;
        }


        /******************
         * protected methods
         */
        
        
        protected static OffsetTable ReadOffsetTable(OTFile file, uint filepos)
        {
            // read the Offset Table from the file

            const int SIZEOF_OFFSETTABLE = 12;

            OffsetTable ot = null;


            // read the offset table

            MBOBuffer buf = file.ReadPaddedBuffer(filepos, SIZEOF_OFFSETTABLE);

            if (buf != null)
            {
                if (OTFile.IsValidSfntVersion(buf.GetUint(0)))
                {
                    ot = new OffsetTable(buf);
                }
            }


            // now read the directory entries

            if (ot != null)
            {
                const int SIZEOF_DIRECTORYENTRY = 16;


                for (int i=0; i<ot.numTables; i++)
                {
                    uint dirFilePos = (uint)(filepos+SIZEOF_OFFSETTABLE+i*SIZEOF_DIRECTORYENTRY);
                    MBOBuffer DirEntBuf = file.ReadPaddedBuffer(dirFilePos, SIZEOF_DIRECTORYENTRY);

                    if (DirEntBuf != null)
                    {
                        DirectoryEntry de = new DirectoryEntry(DirEntBuf);
                        ot.DirectoryEntries.Add(de);                        
                    }
                    else
                    {
                        Debug.Assert(false);
                        break;
                    }
                }
            }

            return ot;
        }


        protected OTTable GetTable(DirectoryEntry de)
        {
            OTTable table = null;

            if (m_File != null)
            {
                // get the table from OTFile's table manager
                table = m_File.GetTableManager().GetTable(this, de);
            }
            else
            {
                // get the table from the list in memory
                for (int i=0; i<MemBasedTables.Count; i++)
                {
                    OTTable t = (OTTable)MemBasedTables[i];
                    if (de.tag == t.m_tag)
                    {
                        table = t;
                        break;
                    }
                }
            }

            return table;
        }


        /**************
         * member data
         */

        public enum OutlineType
        {
            OUTLINE_INVALID,
            OUTLINE_TRUETYPE,
            OUTLINE_POSTSCRIPT
        }
        
        
        protected OTFile m_File;
        protected uint m_FontFileNumber; // 0 if ttf, else number of font in ttc, starting at 0
        protected OffsetTable m_OffsetTable;
        protected OutlineType m_OutlineType;
        protected ushort m_maxpNumGlyphs;
        protected uint [] m_arrUnicodeToGlyph_3_0;
        protected uint [] m_arrUnicodeToGlyph_3_1;
        protected uint [] m_arrUnicodeToGlyph_3_10;
        protected bool m_bContainsMsSymbolEncodedCmap;
        protected ArrayList MemBasedTables; // this list is used when building a font in memory
    }
}
