using System;
using System.Diagnostics;
using System.IO;

using Microsoft.Win32.SafeHandles;

namespace OTFontFile
{
    /// <summary>
    /// A font read in from a disk file. May be a single OTF or a TT collection.
    /// </summary>
    public class OTFile
    {
        /***************
         * constructors
         */


        /// <summary>Construct empty object except for <c>TableManager</c>
        /// </summary>
        public OTFile()
        {
            m_TableManager = new TableManager(this);

            m_FontFileType = FontFileType.INVALID;
            m_nFonts = 0;
            m_ttch = null;
        }

        /*****************
        * private methods
        */

        private bool TestFileType()
        {
            m_FontFileType = Identify();

            switch (m_FontFileType)
            {
                case FontFileType.INVALID:
                    close();
                    return false;

                case FontFileType.SINGLE:
                    m_ttch = null;
                    m_nFonts = 1;
                    return true;

                case FontFileType.COLLECTION:
                    m_ttch = TTCHeader.ReadTTCHeader(this);
                    if (m_ttch != null)
                    {
                        m_nFonts = m_ttch.DirectoryCount;
                    }
                    else
                    {
                        m_nFonts = 0;
                    }
                    return true;
                default:
                    close();
                    return false;
            }

        }

        /*****************
         * public methods
         */


        /// <summary>Associate file at path <c>sFilename</c> with object.
        /// </summary>
        public bool open(string sFilename)
        {
            // NOTE: creating a filestream can throw exceptions
            // Should they be handled here, or let the caller worry about it?
            m_fs = new FileStream(sFilename, FileMode.Open, FileAccess.Read, FileShare.Read);

            return TestFileType();

        }

        public bool open(SafeFileHandle handle)
        {
            m_fs = new FileStream(handle,FileAccess.Read);

            return TestFileType();
        }

        /// <summary>Close filestream and set type to file type to 
        /// invalid.</summary>
        public void close()
        {
            if (m_fs != null)
            {
                m_fs.Close();
                m_fs = null;
            }
            m_TableManager = new TableManager(this); // JJF: Why?
            m_FontFileType = FontFileType.INVALID;
            m_ttch = null;
            m_nFonts = 0;
        }

        /// <summary>Accessor for filestream length.</summary>
        public long GetFileLength()
        {
            return m_fs.Length;
        }

        /// <summary>Accessor for <c>m_TableManager</c></summary>
        public TableManager GetTableManager()
        {
            return m_TableManager;
        }
        

        /// <summary>Based on first few bytes of file, decided whether
        /// this file is a collection, that is, the file starts with "ttcf".
        /// </summary>
        public bool IsCollection()
        {
            return (Identify() == FontFileType.COLLECTION);
        }
        
        /// <summary>Accessor for <c>m_nFonts</c></summary>
        public uint GetNumFonts()
        {
            return m_nFonts;
        }


        /// <summary>Read and parse font, either a single font or a single
        /// font from a collection, using the <c>OTFont</c> class.
        /// </summary>
        public OTFont GetFont(uint i)
        {
            OTFont f = null;

            Debug.Assert(i < m_nFonts);
            Debug.Assert(m_FontFileType != FontFileType.INVALID);
            if (i > 0)
            {
                Debug.Assert(m_FontFileType == FontFileType.COLLECTION);
            }

            if (i < m_nFonts)
            {
                if (m_FontFileType == FontFileType.SINGLE)
                {
                    f = OTFont.ReadFont(this, 0, 0);
                }
                else if (m_FontFileType == FontFileType.COLLECTION)
                {
                    Debug.Assert(m_ttch != null);
                    if (m_ttch != null)
                    {
                        uint nDirOffset = (uint)m_ttch.DirectoryOffsets[(int)i];

                        f = OTFont.ReadFont(this, i, nDirOffset);
                    }
                }
            }

            return f;
        }

        /// <summary>Check magic bytes at beginning of file</summary>
        public static bool IsValidSfntVersion(uint sfnt)
        {
            bool bRet = false;

            OTTag tag = sfnt;

            if (tag == 0x00010000 || // from MS OpenType spec
                (string)tag == "OTTO" ||   // from MS OpenType spec
                (string)tag == "true" ||   // from Apple TrueType Reference
                (string)tag == "typ1")     // from Apple TrueType Reference
            {
                bRet = true;
            }

            return bRet;
        }

        public uint GetNumPadBytesAfterTable(OTTable t)
        {
            long TableFilePos = t.GetBuffer().GetFilePos();
            if (TableFilePos == -1)
            {
                // the table's file position is initialized to -1 if the table was not read from a file
                throw new ApplicationException("invalid file position");
            }

            uint TableLength = t.GetBuffer().GetLength();
            long PadFilePos = TableFilePos + TableLength;

            long NextTablePos = m_fs.Length;

            // possible DSIG in TTC after all the tables
            if (IsCollection() && (string) m_ttch.DsigTag == "DSIG" && m_ttch.DsigOffset > 0 && m_ttch.DsigOffset < NextTablePos && (string) t.m_tag != "DSIG" )
                NextTablePos = m_ttch.DsigOffset;

            for (uint iFont=0; iFont<m_nFonts; iFont++)
            {
                if (IsCollection())
                {
                    uint offset = (uint)m_ttch.DirectoryOffsets[(int)iFont];
                    if (offset >= PadFilePos && offset < NextTablePos)
                    {
                        NextTablePos = offset;
                    }
                }

                OTFont f = GetFont(iFont);
                for (ushort iTable=0; iTable<f.GetNumTables(); iTable++)
                {
                    DirectoryEntry de = f.GetDirectoryEntry(iTable);
                    if (de.offset >= PadFilePos && de.offset < NextTablePos)
                    {
                        NextTablePos = de.offset;
                    }
                }
            }

            return (uint)(NextTablePos - PadFilePos);
        }
        
        
        /// <summary>Read part of filestream into MBOBuffer</summary>
        public MBOBuffer ReadPaddedBuffer(uint filepos, uint length)
        {
            // allocate a buffer to hold the table
            MBOBuffer buf = new MBOBuffer(filepos, length);

            // read the table
            m_fs.Seek(filepos, SeekOrigin.Begin);
            int nBytes = m_fs.Read(buf.GetBuffer(), 0, (int)length);
            if (nBytes != length)
            {
                // check for premature EOF
                if (m_fs.Position == m_fs.Length)
                {
                    // EOF
                }
                else
                {
                    // Read Error
                }

                buf = null;
            }

            return buf;
        }

        /// <summary>Random access to file stream</summary>
        public byte[] ReadBytes(long filepos, uint length)
        {
            byte[] buf = new byte[length];
            m_fs.Seek(filepos, SeekOrigin.Begin);
            m_fs.Read(buf, 0, (int)length);
            return buf;
        }

        /// <summary>Accessor for filestream field.</summary>
        public FileStream GetFileStream()
        {
            return m_fs;
        }

        /// <summary>Accessor for TTC header field.</summary>
        public TTCHeader GetTTCHeader()
        {
            return m_ttch;
        }


        // writing a font file

        /// <summary>Write single font or collection to disk.</summary>
        static public bool WriteFile(string sFilename, OTFont[] fonts)
        {
            bool bRet = true;

            if (fonts.Length == 0)
            {
                // illegal - need at least one font
                throw new ApplicationException("zero length font array");
            }
            else
            {
                // open the font file for writing
                FileStream fs = new FileStream(sFilename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

                // write it
                if (fonts.Length == 1)
                {
                    bRet = WriteSfntFile(fs, fonts[0]);
                }
                else
                {
                    bRet = WriteTTCFile(fs, fonts);
                }

                // close the font file
                fs.Close();
                fs = null;
            }
            

            return bRet;
        }

        /// <summary>Write a single OTF to a disk file, with tables in
        /// proper order, checksums set, etc.
        /// </summary>
        public static bool WriteSfntFile(FileStream fs, OTFont font)
        {
            bool bRet = true;


            OTFixed sfntVersion = new OTFixed(1,0);
            ushort numTables = font.GetNumTables();
            OffsetTable ot = new OffsetTable(sfntVersion, numTables);

            
            // order tables in fastfont order

            string [] arrOrderedNames = null;
            string [] ttNames =
                {
                    "head", "hhea", "maxp", "OS/2", "hmtx", "LTSH", "VDMX", "hdmx", "cmap", "fpgm",
                    "prep", "cvt ", "loca", "glyf", "kern", "name", "post", "gasp", "PCLT"
                };
            string [] psNames = 
                {
                    "head", "hhea", "maxp", "OS/2", "name", "cmap", "post", "CFF "
                };

            if (font.ContainsTrueTypeOutlines())
            {
                arrOrderedNames = ttNames;
            }
            else if (font.ContainsPostScriptOutlines())
            {
                arrOrderedNames = psNames;
            }

            OTTable[] OrderedTables = new OTTable[numTables];
            for (ushort i=0; i<numTables; i++)
            {
                OrderedTables[i] = font.GetTable(i);
            }

            if (arrOrderedNames != null)
            {
                ushort curpos = 0;
                for (int iName=0; iName<arrOrderedNames.Length; iName++)
                {
                    for (ushort i=curpos; i<numTables; i++)
                    {
                        if (arrOrderedNames[iName] == (string)OrderedTables[i].m_tag)
                        {
                            OTTable temp = OrderedTables[curpos];
                            OrderedTables[curpos] = OrderedTables[i];
                            OrderedTables[i] = temp;
                            curpos++;
                            break;
                        }
                    }
                }
            }


            // update the modified date in the head table

            for (int i=0; i<OrderedTables.Length; i++)
            {
                if ((string)OrderedTables[i].m_tag == "head")
                {
                    // get the cache
                    Table_head headTable = (Table_head)OrderedTables[i];
                    Table_head.head_cache headCache = (Table_head.head_cache)headTable.GetCache();

                    // set the 'modified' field to the current date and time
                    DateTime dt = DateTime.Now;
                    headCache.modified = headTable.DateTimeToSecondsSince1904(dt);

                    // generate a new table and replace the head table in the list of ordered tables
                    Table_head newHead = (Table_head)headCache.GenerateTable();
                    OrderedTables[i] = newHead;
                    
                    break;
                }
            }


            // build a list of directory entries

            long TableFilePos = 12 + numTables*16;

            for (ushort i=0; i<numTables; i++)
            {
                OTTable table = OrderedTables[i];
                OTTag tag = table.m_tag;
                
                // build a new directory entry
                DirectoryEntry de = new DirectoryEntry();
                de.tag = new OTTag(tag.GetBytes());
                de.checkSum = table.CalcChecksum();
                de.offset = (uint)TableFilePos;
                de.length = table.GetLength();

                ot.DirectoryEntries.Add(de);                        

                TableFilePos += table.GetBuffer().GetPaddedLength();
            }


            // sort the directory entries

            if (numTables > 1)
            {
                for (int i=0; i<numTables-1; i++)
                {
                    for (int j=i+1; j<numTables; j++)
                    {
                        if (((DirectoryEntry)(ot.DirectoryEntries[i])).tag > ((DirectoryEntry)(ot.DirectoryEntries[j])).tag)
                        {
                            DirectoryEntry temp = (DirectoryEntry)ot.DirectoryEntries[i];
                            ot.DirectoryEntries[i] = (DirectoryEntry)ot.DirectoryEntries[j];
                            ot.DirectoryEntries[j] = temp;
                        }
                    }
                }
            }


            // update the font checksum in the head table

            for (int i=0; i<OrderedTables.Length; i++)
            {
                if ((string)OrderedTables[i].m_tag == "head")
                {
                    // calculate the checksum
                    uint sum = 0;
                    sum += ot.CalcOffsetTableChecksum();
                    sum += ot.CalcDirectoryEntriesChecksum();
                    for (int j=0; j<OrderedTables.Length; j++)
                    {
                        sum += OrderedTables[j].CalcChecksum();
                    }

                    // get the cache
                    Table_head headTable = (Table_head)OrderedTables[i];
                    Table_head.head_cache headCache = (Table_head.head_cache)headTable.GetCache();

                    // set the checkSumAdujustment field
                    headCache.checkSumAdjustment = 0xb1b0afba - sum;

                    // generate a new table and replace the head table in the list of ordered tables
                    Table_head newHead = (Table_head)headCache.GenerateTable();
                    OrderedTables[i] = newHead;

                    break;
                }
            }

            
            // write the offset table

            fs.Write(ot.m_buf.GetBuffer(), 0, (int)ot.m_buf.GetLength());

            
            // write the directory entries

            for (int i=0; i<numTables; i++)
            {
                DirectoryEntry de = (DirectoryEntry)ot.DirectoryEntries[i];
                fs.Write(de.m_buf.GetBuffer(), 0, (int)de.m_buf.GetLength());
            }


            // write the tables

            for (ushort i=0; i<numTables; i++)
            {
                OTTable table = OrderedTables[i];

                fs.Write(table.m_bufTable.GetBuffer(), 0, (int)table.GetBuffer().GetPaddedLength());
            }


            return bRet;
        }


        /// <summary>Write a TTC (TT Collection) to a disk file.</summary>
        public static bool WriteTTCFile(FileStream fs, OTFont[] fonts)
        {
            bool bRet = true;


            // build the TTC header

            OTTag TTCtag = (OTTag)"ttcf";
            uint version = 0x00020000;
            uint DirectoryCount = (uint)fonts.Length;
            uint [] TableDirectory = new uint[fonts.Length]; 
            uint ulDsigTag    = 0;
            uint ulDsigLength = 0;
            uint ulDsigOffset = 0;

            uint TTCHeaderLen = 12 + DirectoryCount * 4 + 12; // length of version 2.0 header


            // build an array of offset tables
            OffsetTable[] otArr = new OffsetTable[fonts.Length];
            for (int iFont=0; iFont<fonts.Length; iFont++)
            {
                otArr[iFont] = new OffsetTable(new OTFixed(1,0), fonts[iFont].GetNumTables());
            }

            // build an array of head tables that will contain the updated modified field and font checksum
            Table_head[] arrHeadTables = new Table_head[fonts.Length];
            for (int i=0; i<fonts.Length; i++)
            {
                // get the cache
                Table_head headTable = (Table_head)fonts[i].GetTable("head");
                Table_head.head_cache headCache = (Table_head.head_cache)headTable.GetCache();

                // set the 'modified' field to the current date
                DateTime dt = DateTime.Now;
                headCache.modified = headTable.DateTimeToSecondsSince1904(dt);

                // generate a new table and add it to the array
                Table_head newHead = (Table_head)headCache.GenerateTable();
                arrHeadTables[i] = newHead;
            }



            // build a list of directory entries for each font

            long FilePos = TTCHeaderLen;
            
            for (int iFont = 0; iFont<fonts.Length; iFont++)
            {
                ushort numTables = fonts[iFont].GetNumTables();
                TableDirectory[iFont] = (uint)FilePos;
                FilePos += 12 + numTables*16;

                uint PrevFilePos = 0;
                for (ushort i=0; i<numTables; i++)
                {
                    OTTable table = fonts[iFont].GetTable(i);
                    OTTag tag = table.m_tag;

                    if ((string)tag == "head")
                    {
                        table = arrHeadTables[iFont];
                    }

                    // check if this table is a duplicate of a table in a previous font

                    PrevFilePos = 0;
                    if (iFont > 0)
                    {
                        for (int iPrevFont=0; iPrevFont<iFont; iPrevFont++)
                        {
                            for (int iTable=0; iTable<fonts[iPrevFont].GetNumTables(); iTable++)
                            {
                                OTTable PrevTable = fonts[iPrevFont].GetTable(table.m_tag);
                                if (PrevTable != null)
                                {
                                    if (MBOBuffer.BinaryEqual(table.m_bufTable, PrevTable.m_bufTable))
                                    {
                                        // get the file position for the previous table
                                        for (int iDe=0; iDe<otArr[iPrevFont].DirectoryEntries.Count; iDe++)
                                        {
                                            DirectoryEntry dePrev = (DirectoryEntry)otArr[iPrevFont].DirectoryEntries[iDe];
                                            if (dePrev.tag == table.m_tag)
                                            {
                                                PrevFilePos = dePrev.offset;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }


                    // build a new directory entry

                    DirectoryEntry de = new DirectoryEntry();
                    de.tag = new OTTag(tag.GetBytes());
                    de.checkSum = table.CalcChecksum();
                    de.length = table.GetLength();
                    if (PrevFilePos != 0)
                    {
                        de.offset = (uint)PrevFilePos;
                    }
                    else
                    {
                        de.offset = (uint)FilePos;
                        FilePos += table.GetBuffer().GetPaddedLength();
                    }

                    otArr[iFont].DirectoryEntries.Add(de);
                }


                // sort the directory entries

                if (numTables > 1)
                {
                    for (int i=0; i<numTables-1; i++)
                    {
                        for (int j=i+1; j<numTables; j++)
                        {
                            if (((DirectoryEntry)otArr[iFont].DirectoryEntries[i]).tag > ((DirectoryEntry)otArr[iFont].DirectoryEntries[j]).tag)
                            {
                                DirectoryEntry temp = (DirectoryEntry)otArr[iFont].DirectoryEntries[i];
                                otArr[iFont].DirectoryEntries[i] = (DirectoryEntry)otArr[iFont].DirectoryEntries[j];
                                otArr[iFont].DirectoryEntries[j] = temp;
                            }
                        }
                    }
                }
            }

            
            // update each font's checksum in the head table

            for (int iFont=0; iFont<fonts.Length; iFont++)
            {
                ushort numTables = fonts[iFont].GetNumTables();

                // calculate the checksum
                uint sum = 0;
                sum += otArr[iFont].CalcOffsetTableChecksum();
                sum += otArr[iFont].CalcDirectoryEntriesChecksum();
                for (ushort i=0; i<numTables; i++)
                {
                    DirectoryEntry de = (DirectoryEntry)otArr[iFont].DirectoryEntries[i];
                    OTTable table = fonts[iFont].GetTable(de.tag);
                    if ((string)de.tag == "head")
                    {
                        table = arrHeadTables[iFont];
                    }
                    sum += table.CalcChecksum();
                }

                // get the cache
                Table_head headTable = arrHeadTables[iFont];
                Table_head.head_cache headCache = (Table_head.head_cache)headTable.GetCache();

                // set the checkSumAdujustment field
                headCache.checkSumAdjustment = 0xb1b0afba - sum;

                // generate a new table and replace the head table in the array of head tables
                Table_head newHead = (Table_head)headCache.GenerateTable();
                arrHeadTables[iFont] = newHead;



            }



            // write the TTC header

            WriteUint32MBO(fs, (uint)TTCtag);
            WriteUint32MBO(fs, version);
            WriteUint32MBO(fs, DirectoryCount);
            for (int i=0; i<fonts.Length; i++)
            {
                WriteUint32MBO(fs, TableDirectory[i]);
            }
            WriteUint32MBO(fs, ulDsigTag);
            WriteUint32MBO(fs, ulDsigLength);
            WriteUint32MBO(fs, ulDsigOffset);


            // write out each font

            for (int iFont=0; iFont<fonts.Length; iFont++)
            {
                ushort numTables = fonts[iFont].GetNumTables();

                // write the offset table

                fs.Write(otArr[iFont].m_buf.GetBuffer(), 0, (int)otArr[iFont].m_buf.GetLength());

            
                // write the directory entries

                for (int i=0; i<numTables; i++)
                {
                    DirectoryEntry de = (DirectoryEntry)otArr[iFont].DirectoryEntries[i];
                    fs.Write(de.m_buf.GetBuffer(), 0, (int)de.m_buf.GetLength());
                }


                // write out each table unless a shared version has been written

                for (ushort i=0; i<numTables; i++)
                {
                    DirectoryEntry de = (DirectoryEntry)otArr[iFont].DirectoryEntries[i];
                    if (fs.Position == de.offset)
                    {
                        OTTable table = fonts[iFont].GetTable(de.tag);
                        if ((string)table.m_tag == "head")
                        {
                            table = arrHeadTables[iFont];
                        }
                        fs.Write(table.m_bufTable.GetBuffer(), 0, (int)table.GetBuffer().GetPaddedLength());
                    }
                }

            }


            return bRet;
        }

        // write a 16 bit uint in motorola byte order 
        protected static void WriteUint16MBO(FileStream fs, ushort n)
        {
            byte [] buf = new Byte [2];
            buf[0] = (byte)(n >> 8);
            buf[1] = (byte)n;

            fs.Write(buf, 0, 2);
        }

        // write a 32 bit uint in motorola byte order 
        protected static void WriteUint32MBO(FileStream fs, uint n)
        {
            byte [] buf = new Byte [4];
            buf[0] = (byte)(n >> 24);
            buf[1] = (byte)(n >> 16);
            buf[2] = (byte)(n >> 8);
            buf[3] = (byte)n;

            fs.Write(buf, 0, 4);
        }


        /******************
         * protected methods
         */

        protected bool IsExtension(string sFilename, string sExtension)
        {
            bool bRet = false;

            int nExtPos = sFilename.LastIndexOf('.');
            if (nExtPos != -1)
            {
                string sExt = sFilename.Substring(nExtPos);
                if (string.Compare(sExt, sExtension, true) == 0)
                {
                    bRet = true;
                }
            }

            return bRet;
        }

        protected FontFileType Identify()
        {
            Debug.Assert(m_fs != null);

            FontFileType fft = FontFileType.INVALID;

            // read the first four bytes in the file
            MBOBuffer buf = ReadPaddedBuffer(0, 4);

            if (buf != null)
            {
                OTTag tag = new OTTag(buf.GetBuffer());

                if (IsValidSfntVersion(tag))
                {
                    fft = FontFileType.SINGLE;
                }
                else if ((string)tag == "ttcf")
                {
                    fft = FontFileType.COLLECTION;
                }
            }

            return fft;
        }

        
        protected enum FontFileType
        {
            INVALID,
            SINGLE,
            COLLECTION
        }

        
        /**************
         * member data
         */


        protected TableManager m_TableManager;        
        protected FontFileType m_FontFileType;

        protected FileStream m_fs;
        protected uint m_nFonts;
        protected TTCHeader m_ttch;
    }
}
