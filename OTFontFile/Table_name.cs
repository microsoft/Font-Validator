using System;
using System.Diagnostics;
using System.Text;
using System.Collections;
//using Win32APIs;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_name.
    /// </summary>
    public class Table_name : OTTable
    {
        /************************
         * constructors
         */
        
        public Table_name(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            FormatSelector    = 0,
            NumberNameRecords = 2,
            OffsetToStrings   = 4,
            NameRecords       = 6
        }


        /************************
         * name record class
         */

        public class NameRecord
        {
            public NameRecord(ushort offset, MBOBuffer bufTable)
            {
                m_offsetNameRecord = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                PlatformID   = 0,
                EncodingID   = 2,
                LanguageID   = 4,
                NameID       = 6,
                StringLength = 8,
                StringOffset = 10
            }

            public ushort PlatformID
            {
                get {return m_bufTable.GetUshort(m_offsetNameRecord + (uint)FieldOffsets.PlatformID);}
            }

            public ushort EncodingID
            {
                get {return m_bufTable.GetUshort(m_offsetNameRecord + (uint)FieldOffsets.EncodingID);}
            }

            public ushort LanguageID
            {
                get {return m_bufTable.GetUshort(m_offsetNameRecord + (uint)FieldOffsets.LanguageID);}
            }

            public ushort NameID
            {
                get {return m_bufTable.GetUshort(m_offsetNameRecord + (uint)FieldOffsets.NameID);}
            }

            public ushort StringLength
            {
                get {return m_bufTable.GetUshort(m_offsetNameRecord + (uint)FieldOffsets.StringLength);}
            }

            public ushort StringOffset
            {
                get {return m_bufTable.GetUshort(m_offsetNameRecord + (uint)FieldOffsets.StringOffset);}
            }


            ushort m_offsetNameRecord;
            MBOBuffer m_bufTable;
        }


        /************************
         * utility methods
         */

        static public int MacEncIdToCodePage(ushort MacEncodingID)
        {
            /*
                Q187858 INFO: Macintosh Code Pages Supported Under Windows NT
                
                10000 (MAC - Roman)
                10001 (MAC - Japanese)
                10002 (MAC - Traditional Chinese Big5)
                10003 (MAC - Korean)
                10004 (MAC - Arabic)
                10005 (MAC - Hebrew)
                10006 (MAC - Greek I)
                10007 (MAC - Cyrillic)
                10008 (MAC - Simplified Chinese GB 2312)
                10010 (MAC - Romania)
                10017 (MAC - Ukraine)
                10029 (MAC - Latin II)
                10079 (MAC - Icelandic)
                10081 (MAC - Turkish)
                10082 (MAC - Croatia) 
            */
            // NOTE: code pages 10010 through 10082
            // don't seem to map to Encoding IDs in the OT spec

            int MacCodePage = -1;


            switch (MacEncodingID)
            {
                case 0: // Roman
                    MacCodePage = 10000;
                    break;
                case 1: // Japanese
                    MacCodePage = 10001;
                    break;
                case 2: // Chinese (Traditional)
                    MacCodePage = 10002;
                    break;
                case 3: // Korean
                    MacCodePage = 10003;
                    break;
                case 4: // Arabic
                    MacCodePage = 10004;
                    break;
                case 5: // Hebrew
                    MacCodePage = 10005;
                    break;
                case 6: // Greek
                    MacCodePage = 10006;
                    break;
                case 7: // Russian
                    MacCodePage = 10007;
                    break;

                case 25: // Chinese (Simplified)
                    MacCodePage = 10008;
                    break;

                default:
                    Debug.Assert(false, "unsupported text encoding");
                    break;
            }

            return MacCodePage;

        }

        static public int MSEncIdToCodePage(ushort MSEncID)
        {
            int nCodePage = -1;

            switch(MSEncID)
            {
                case 2: // ShiftJIS
                    nCodePage = 932;
                    break;
                case 3: // PRC
                    nCodePage = 936;
                    break;
                case 4: // Big5
                    nCodePage = 950;
                    break;
                case 5: // Wansung
                    nCodePage = 949;
                    break;
                case 6: // Johab
                    nCodePage = 1361;
                    break;
            }

            return nCodePage;
        }

        static protected string GetUnicodeStrFromCodePageBuf(byte [] buf, int codepage)
        {
            Encoding enc = Encoding.GetEncoding(codepage);
            Decoder dec = enc.GetDecoder();
            int nChars = dec.GetCharCount(buf, 0, buf.Length);
            char [] destbuf = new Char[nChars];
            dec.GetChars(buf, 0, buf.Length, destbuf, 0);
            String s = new String(destbuf);
            return s;
        }

        static protected byte[] GetCodePageBufFromUnicodeStr( string sNameString, int nCodepage )
        {
            byte[] bString;

            Encoding enc = Encoding.GetEncoding( nCodepage );
            bString = enc.GetBytes( sNameString );
            
            return bString;
        }

        static protected string DecodeString(ushort PlatID, ushort EncID, byte [] EncodedStringBuf)
        {
            string s = null;

            if (PlatID == 0) // unicode
            {
                System.Text.UnicodeEncoding ue = new System.Text.UnicodeEncoding(true, false);
                s = ue.GetString(EncodedStringBuf);
            }
            else if (PlatID == 1) // Mac
            {
                int nMacCodePage = MacEncIdToCodePage(EncID);
                if (nMacCodePage != -1)
                {
                    if ( Type.GetType("Mono.Runtime") != null )
                    {
                        // Mono.Runtime don't currently support
                        // 10001 to 10008.
                        switch ( nMacCodePage )
                        {
                            // Close-enough substitutes for names:

                            case 10001:             // Japanese
                                nMacCodePage = 932; // ShiftJIS
                                break;

                            case 10002:             // Chinese (Traditional)
                                nMacCodePage = 950; // Big5
                                break;

                            case 10003:             // Korean
                                nMacCodePage = 949; //
                                break;

                            case 10004:             // mac-arabic
                                nMacCodePage = 1256;
                                break;

                            case 10005:             // mac-hebrew
                                nMacCodePage = 1255;
                                break;

                            case 10006:             // mac-greek
                                nMacCodePage = 1253;
                                break;

                            case 10007:             // mac-cyrillic
                                nMacCodePage = 1251;
                                break;

                            case 10008:             // Chinese (Simplified)
                                nMacCodePage = 936; // PRC
                                break;

                            default:
                                break;
                        }
                    }
                    s = GetUnicodeStrFromCodePageBuf(EncodedStringBuf, nMacCodePage);
                }
            }
            else if (PlatID == 3) // MS
            {
                if (EncID == 0 || // symbol - strings identified as symbol encoded strings 
                                  // aren't symbol encoded, they're unicode encoded!!!
                    EncID == 1 || // unicode
                    EncID == 10 ) // unicode with surrogate support for UCS-4
                {
                    System.Text.UnicodeEncoding ue = new System.Text.UnicodeEncoding(true, false);
                    s = ue.GetString(EncodedStringBuf);
                }
                else if (EncID >= 2 && EncID <= 6)
                {
                    int nCodePage = MSEncIdToCodePage(EncID);
                    s = GetUnicodeStrFromCodePageBuf(EncodedStringBuf, nCodePage);
                }
                else
                {
                    //Debug.Assert(false, "unsupported text encoding");
                }
            }

            return s;
        }

        static protected byte[] EncodeString(string s, ushort PlatID, ushort EncID)
        {
            byte[] buf = null;

            if(PlatID == 0) // unicode
            {
                System.Text.UnicodeEncoding ue = new System.Text.UnicodeEncoding( true, false );
                buf = ue.GetBytes( s );
            }
            else if (PlatID == 1 ) // Mac
            {
                int nCodePage = MacEncIdToCodePage(EncID);
                if( nCodePage != -1 )
                {
                    buf = GetCodePageBufFromUnicodeStr(s, nCodePage);
                }
            }
            else if (PlatID == 3 ) // MS
            {
                if (EncID == 0 || // symbol - strings identified as symbol encoded strings 
                                  // aren't symbol encoded, they're unicode encoded!!!
                    EncID == 1 || // unicode
                    EncID == 10 ) // unicode with surrogate support for UCS-4
                {
                    System.Text.UnicodeEncoding ue = new System.Text.UnicodeEncoding( true, false );
                    buf = ue.GetBytes(s);
                }
                else if (EncID >= 2 || EncID <= 6)
                {
                    int nCodePage = MSEncIdToCodePage(EncID);
                    if( nCodePage != -1 )
                    {
                        buf = GetCodePageBufFromUnicodeStr(s, nCodePage);
                    }
                }
                else
                {
                    //Debug.Assert(false, "unsupported text encoding");
                }
            }

            return buf;
        }

        public string GetString(ushort PlatID, ushort EncID, ushort LangID, ushort NameID)
        {
            // !!! NOTE: a value of 0xffff for PlatID, EncID, or LangID is used !!!
            // !!! as a wildcard and will match any value found in the table    !!!

            string s = null;

            for (uint i=0; i<NumberNameRecords; i++) 
            {
                NameRecord nr = GetNameRecord(i);
                if (nr != null)
                {
                    if ((PlatID == 0xffff || nr.PlatformID == PlatID) &&
                        (EncID  == 0xffff || nr.EncodingID == EncID ) &&
                        (LangID == 0xffff || nr.LanguageID == LangID) &&
                        nr.NameID == NameID)
                    {
                        byte [] buf = GetEncodedString(nr);
                        if (buf != null)
                        {
                            s = DecodeString(nr.PlatformID, nr.EncodingID, buf);
                        }

                        break;
                    }
                }
            }

            return s;
        }

        public string GetNameString()
        {
            string sName = null;
            try
            {
                sName = GetString(3, 0xffff, 0x0409, 4);  // MS, any encoding, english, name
                if (sName == null)
                {
                    sName = GetString(3, 0xffff, 0xffff, 4); // MS, any encoding, any language, name
                }
                if (sName == null)
                {
                    sName = GetString(1, 0, 0, 4); // mac, roman, english, name
                }

                // validate surrogate content
                for (int i = 0; i < sName.Length - 1; i++)
                {
                    if (   (   (sName[i] >= 0xd800 && sName[i] <= 0xdbff)
                            && !(sName[i+1] >= 0xdc00 && sName[i+1] <= 0xdfff))
                        || (   !(sName[i] >= 0xd800 && sName[i] <= 0xdbff)
                            && (sName[i+1] >= 0xdc00 && sName[i+1] <= 0xdfff)))
                    {
                        sName = null;
                        break;
                    }
                }
            }
            catch
            {
            }

            return sName;
        }

        public string GetVersionString()
        {
            string sVersion = null;

            try
            {
                sVersion = GetString(3, 0xffff, 0x0409, 5);  // MS, any encoding, english, version
                if (sVersion == null)
                {
                    sVersion = GetString(3, 0xffff, 0xffff, 5); // MS, any encoding, any language, version
                }
                if (sVersion == null)
                {
                    sVersion = GetString(1, 0, 0, 5); // mac, roman, english, version
                }

                // validate surrogate content
                for (int i = 0; i < sVersion.Length - 1; i++)
                {
                    if (   (   (sVersion[i] >= 0xd800 && sVersion[i] <= 0xdbff)
                        && !(sVersion[i+1] >= 0xdc00 && sVersion[i+1] <= 0xdfff))
                        || (   !(sVersion[i] >= 0xd800 && sVersion[i] <= 0xdbff)
                        && (sVersion[i+1] >= 0xdc00 && sVersion[i+1] <= 0xdfff)))
                    {
                        sVersion = null;
                        break;
                    }
                }
            }
            catch
            {
            }

            return sVersion;
        }

        public string GetStyleString()
        {
            string sStyle = null;

            try
            {
                sStyle = GetString(3, 0xffff, 0x0409, 2);  // MS, any encoding, english, subfamily (style)
                if (sStyle == null)
                {
                    sStyle = GetString(3, 0xffff, 0xffff, 2); // MS, any encoding, any language, subfamily (style)
                }
                if (sStyle == null)
                {
                    sStyle = GetString(1, 0, 0, 2); // mac, roman, english, subfamily (style)
                }
            }
            catch
            {
            }

            return sStyle;
        }

        /************************
         * property accessors
         */

        public ushort FormatSelector
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.FormatSelector);}
        }

        public ushort NumberNameRecords
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.NumberNameRecords);}
        }

        public ushort OffsetToStrings
        {
            get    {return m_bufTable.GetUshort((uint)FieldOffsets.OffsetToStrings);}
        }

        public NameRecord GetNameRecord(uint i)
        {
            NameRecord nr = null;

            if (i < NumberNameRecords)
            {
                uint offset = (uint)FieldOffsets.NameRecords + i*12;
                if (offset + 12 < m_bufTable.GetLength())
                {
                    nr = new NameRecord((ushort)offset, m_bufTable);
                }
            }

            return nr;
        }

        public byte[] GetEncodedString(NameRecord nr)
        {
            byte[] buf = null;
            int offset = OffsetToStrings + nr.StringOffset;
            if (offset + nr.StringLength - 1 <= m_bufTable.GetLength())
            {
                buf = new byte[nr.StringLength];
                System.Buffer.BlockCopy(m_bufTable.GetBuffer(), offset, buf, 0, nr.StringLength);
            }
            return buf;
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new name_cache( this );
            }

            return m_cache;
        }
        
        public class name_cache : DataCache
        {
            // the cached data
            protected ushort m_format;
            // No need to store string offsets because we can determine these when we save the cache
            protected ArrayList m_nameRecords; // NameRecordCache[]

            // constructor
            public name_cache(Table_name OwnerTable)
            {
            
                m_format = OwnerTable.FormatSelector;
                
                m_nameRecords = new ArrayList( OwnerTable.NumberNameRecords );

                for( ushort i = 0; i < OwnerTable.NumberNameRecords; i++ )
                {
                    NameRecord nr = OwnerTable.GetNameRecord( i );                    
                    string sNameString = OwnerTable.GetString( nr.PlatformID, nr.EncodingID, nr.LanguageID, nr.NameID );                    
                    addNameRecord(nr.PlatformID, nr.EncodingID, nr.LanguageID, nr.NameID, sNameString);
                }
            }


            public ushort format
            {
                get{ return m_format; }
                set
                {
                    m_format = value;
                    m_bDirty = true;    
                }
            }

            public ushort count
            {
                get{ return (ushort)m_nameRecords.Count; }
            }

            public NameRecordCache getNameRecord( ushort nIndex )
            {
                NameRecordCache nrc = null;

                if( nIndex >= m_nameRecords.Count )
                {                    
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the amount of name records." );
                }
                else
                {
                    nrc = (NameRecordCache)((NameRecordCache)m_nameRecords[nIndex]).Clone();
                }

                return nrc;
            }

            public NameRecordCache getNameRecord(ushort platformID, ushort encodingID, ushort languageID, ushort nameID )
            {
                NameRecordCache nrc = null;

                for (int i=0; i<m_nameRecords.Count; i++)
                {
                    NameRecordCache nrcTemp = (NameRecordCache)m_nameRecords[i];
                    if (nrcTemp.platformID == platformID && 
                        nrcTemp.encodingID == encodingID && 
                        nrcTemp.languageID == languageID && 
                        nrcTemp.nameID == nameID)
                    {
                        nrc = (NameRecordCache)nrcTemp.Clone();
                        break;
                    }
                }

                return nrc;
            }

            public int GetNameRecordIndex(ushort platformID, ushort encodingID, ushort languageID, ushort nameID)
            {
                int nIndex = -1; // return -1 if not found

                for (int i=0; i<m_nameRecords.Count; i++)
                {
                    NameRecordCache nrcTemp = (NameRecordCache)m_nameRecords[i];
                    if (nrcTemp.platformID == platformID && 
                        nrcTemp.encodingID == encodingID && 
                        nrcTemp.languageID == languageID && 
                        nrcTemp.nameID == nameID)
                    {
                        nIndex = i;
                        break;
                    }
                }

                return nIndex;
            }

            protected int GetInsertionPosition(ushort platformID, ushort encodingID, ushort languageID, ushort nameID)
            {
                int nIndex = 0;

                if (GetNameRecordIndex(platformID, encodingID, languageID, nameID) != -1)
                {
                    // can't insert because the specified IDs are already in the list
                    nIndex = -1;
                }
                else
                {
                    if (m_nameRecords.Count != 0)
                    {
                        // check for insertion before the first string
                        NameRecordCache nrcTemp = (NameRecordCache)m_nameRecords[0];
                        int nCompare = nrcTemp.CompareNameRecordToIDs(platformID, encodingID, languageID, nameID);

                        if (nCompare == 1)
                        {
                            nIndex = 0;
                        }
                        else
                        {
                            // check for insertion between two other strings
                            for (int i=0; i<m_nameRecords.Count-1; i++)
                            {
                                NameRecordCache nrcCurr = (NameRecordCache)m_nameRecords[i];
                                NameRecordCache nrcNext = (NameRecordCache)m_nameRecords[i+1];
                                // check if specified IDs are greater than the current object and less than the next object
                                int nCmp1 = nrcCurr.CompareNameRecordToIDs(platformID, encodingID, languageID, nameID);
                                int nCmp2 = nrcNext.CompareNameRecordToIDs(platformID, encodingID, languageID, nameID);
                                if (nCmp1 == -1 && nCmp2 == 1)
                                {
                                    nIndex = i+1;
                                    break;
                                }
                            }
                            // if not found yet then insertion position is at end of list
                            if (nIndex == 0)
                            {
                                nIndex = m_nameRecords.Count;
                            }
                        }
                    }
                }

                return nIndex;
            }

            public void UpdateNameRecord(ushort platformID, ushort encodingID, ushort languageID, ushort nameID, string sNameString)
            {
                int nIndex = GetNameRecordIndex(platformID, encodingID, languageID, nameID);


                if (nIndex != -1)
                {
                    setNameRecord((ushort)nIndex, platformID, encodingID, languageID, nameID, sNameString);
                }
                else
                {
                    throw new ApplicationException("Name Record not found");
                }
            }

            protected bool setNameRecord( ushort nIndex, ushort platformID, ushort encodingID, ushort languageID, ushort nameID, string sNameString )
            {
                bool bResult = true;

                if( nIndex >= m_nameRecords.Count )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the amount of name records." );
                }
                else
                {
                    m_nameRecords[nIndex] = new NameRecordCache( platformID, encodingID, languageID, nameID, sNameString );
                    m_bDirty = true;
                }

                return bResult;
            }

            public void addNameRecord(ushort platformID, ushort encodingID, ushort languageID, ushort nameID, string sNameString)
            {
                int nIndex = GetInsertionPosition(platformID, encodingID, languageID, nameID);

                if (nIndex != -1)
                {
                    addNameRecord((ushort)nIndex, platformID, encodingID, languageID, nameID, sNameString);
                }
                else
                {
                    throw new ApplicationException("string already exists in the table");
                }
            }


            protected bool addNameRecord( ushort nIndex, ushort platformID, ushort encodingID, ushort languageID, ushort nameID, string sNameString )
            {
                bool bResult = true;

                if( nIndex > m_nameRecords.Count )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the amount of name records + 1." );
                }
                else
                {                    
                    m_nameRecords.Insert( nIndex, new NameRecordCache( platformID, encodingID, languageID, nameID, sNameString ));
                    m_bDirty = true;                            
                }

                return bResult;
            }

            // removes the corresponding name string

            public void removeNameRecord(ushort platformID, ushort encodingID, ushort languageID, ushort nameID)
            {
                int nIndex = GetNameRecordIndex(platformID, encodingID, languageID, nameID);
                if (nIndex != -1)
                {
                    removeNameRecord((ushort)nIndex);
                }
                else
                {
                    throw new ApplicationException("NameRecord not found");
                }
            }

            public bool removeNameRecord( ushort nIndex )
            {
                bool bResult = true;

                if( nIndex >= m_nameRecords.Count )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the amount of name records." );
                }
                else
                {                    
                    m_nameRecords.RemoveAt( nIndex );
                    m_bDirty = true;                    
                }

                return bResult;
            }

            public override OTTable GenerateTable()
            {
                ArrayList bytesNameString = new ArrayList();
                ushort nLengthOfStrings = 0;
                ushort nStartOfStringStorage = (ushort)(6 + (m_nameRecords.Count * 12));

                for( ushort i = 0; i < m_nameRecords.Count; i++ )
                {
                    NameRecordCache nrc = (NameRecordCache)m_nameRecords[i];
                    byte[] byteString = EncodeString(nrc.sNameString, nrc.platformID, nrc.encodingID);
                    bytesNameString.Add( byteString );
                    nLengthOfStrings += (ushort)byteString.Length;
                }

                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer( (uint)(Table_name.FieldOffsets.NameRecords + (m_nameRecords.Count * 12) + nLengthOfStrings));

                // populate the buffer                
                newbuf.SetUshort( m_format,                        (uint)Table_name.FieldOffsets.FormatSelector );
                newbuf.SetUshort( (ushort)m_nameRecords.Count,    (uint)Table_name.FieldOffsets.NumberNameRecords );
                newbuf.SetUshort( nStartOfStringStorage,        (uint)Table_name.FieldOffsets.OffsetToStrings );

                ushort nOffset = 0;
                // Write the NameRecords and Strings
                for( ushort i = 0; i < m_nameRecords.Count; i++ )
                {    
                    byte[] bString = (byte[])bytesNameString[i];
                    
                    newbuf.SetUshort( ((NameRecordCache)m_nameRecords[i]).platformID,    (uint)(Table_name.FieldOffsets.NameRecords + (i * 12)));
                    newbuf.SetUshort( ((NameRecordCache)m_nameRecords[i]).encodingID,    (uint)(Table_name.FieldOffsets.NameRecords + (i * 12) + 2));
                    newbuf.SetUshort( ((NameRecordCache)m_nameRecords[i]).languageID,    (uint)(Table_name.FieldOffsets.NameRecords + (i * 12) + 4));
                    newbuf.SetUshort( ((NameRecordCache)m_nameRecords[i]).nameID,        (uint)(Table_name.FieldOffsets.NameRecords + (i * 12) + 6));
                    newbuf.SetUshort( (ushort)bString.Length,                            (uint)(Table_name.FieldOffsets.NameRecords + (i * 12) + 8));
                    newbuf.SetUshort( nOffset,                                            (uint)(Table_name.FieldOffsets.NameRecords + (i * 12) + 10));
                    
                    //Write the string to the buffer
                    for( int ii = 0; ii < bString.Length; ii++ )
                    {
                        newbuf.SetByte( bString[ii], (uint)(nStartOfStringStorage + nOffset + ii));
                    }

                    nOffset += (ushort)bString.Length;
                }

                // put the buffer into a Table_name object and return it
                Table_name nameTable = new Table_name("name", newbuf);
            
                return nameTable;
            }

            public class NameRecordCache : ICloneable
            {
                protected ushort m_platformID;
                protected ushort m_encodingID;
                protected ushort m_languageID;
                protected ushort m_nameID;
                // Instead of the offset we will store the actual string 
                protected string m_sNameString;

                public NameRecordCache( ushort nPlatformID, ushort nEncodingID, ushort nLanguageID, ushort nNameID, string NameString )
                {
                    platformID = nPlatformID;
                    encodingID = nEncodingID;
                    languageID = nLanguageID;
                    nameID = nNameID;
                    sNameString = NameString;
                }

                public ushort platformID
                {
                    get{ return m_platformID; }
                    set{ m_platformID = value; }
                }

                public ushort encodingID
                {
                    get{ return m_encodingID; }
                    set{ m_encodingID = value; }
                }

                public ushort languageID
                {
                    get{ return m_languageID; }
                    set{ m_languageID = value; }
                }

                public ushort nameID
                {
                    get{ return m_nameID; }
                    set{ m_nameID = value; }
                }

                public string sNameString
                {
                    get{ return m_sNameString; }
                    set{ m_sNameString = value; }
                }

                public object Clone()
                {
                    return new NameRecordCache( platformID, encodingID, languageID, nameID, sNameString );
                }

                public int CompareNameRecordToIDs( ushort nPlatformID, ushort nEncodingID, ushort nLanguageID, ushort nNameID)
                {
                    // return
                    // -1 if this object is less than the specified IDs,
                    // 0 if this object is equal to the specified IDs,
                    // 1 if this object is greater than the specified IDs

                    int nResult = 0;

                    if (m_platformID < nPlatformID)
                    {
                        nResult = -1;
                    }
                    else if (m_platformID > nPlatformID)
                    {
                        nResult = 1;
                    }
                    else
                    {
                        if (m_encodingID < nEncodingID)
                        {
                            nResult = -1;
                        }
                        else if (m_encodingID > nEncodingID)
                        {
                            nResult = 1;
                        }
                        else
                        {
                            if (m_languageID < nLanguageID)
                            {
                                nResult = -1;
                            }
                            else if (m_languageID > nLanguageID)
                            {
                                nResult = 1;
                            }
                            else
                            {
                                if (m_nameID < nNameID)
                                {
                                    nResult = -1;
                                }
                                else if (m_nameID > nNameID)
                                {
                                    nResult = 1;
                                }
                                else
                                {
                                    // they are equal!
                                }
                            }
                        }
                    }

                    return nResult;
                }

            }
        }
    }
}
