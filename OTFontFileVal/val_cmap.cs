using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_cmap.
    /// </summary>
    public class val_cmap : Table_cmap, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        const uint HIGHEST_FORMAT = 14;

        public val_cmap(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * public methods
         */
        
        public override Subtable GetSubtable(EncodingTableEntry ete)
        {
            Subtable st = null;

            // identify the format of the table
            ushort format = 0xffff;
            
            try
            {
                format = m_bufTable.GetUshort(ete.offset);
            }
            catch
            {
            }

            switch(format)
            {
                case 0:  st = new Format0_val (ete, m_bufTable); break;
                case 2:  st = new Format2_val (ete, m_bufTable); break;
                case 4:  st = new Format4_val (ete, m_bufTable); break;
                case 6:  st = new Format6_val (ete, m_bufTable); break;
                case 8:  st = new Format8_val (ete, m_bufTable); break;
                case 10: st = new Format10_val(ete, m_bufTable); break;
                case 12: st = new Format12_val(ete, m_bufTable); break;
                case 14: st = new Format14_val(ete, m_bufTable); break;
            }

            return st;
        }


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.cmap_Version))
            {
                if (TableVersionNumber == 0)
                {
                    v.Pass(T.cmap_Version, P.cmap_P_version, m_tag);
                }
                else
                {
                    v.Error(T.cmap_Version, E.cmap_E_version, m_tag,
                            TableVersionNumber.ToString());
                    bRet = false;
                }
            }

            // Check that the number of encoding records seems to 
            // agree with the numTables field, and that each offset
            // is greater than 0 and less than the length of the cmap 
            // table.

            bool bOffsetsOk = true;
            if (v.PerformTest(T.cmap_SubtableOffset))
            {
                
                for (uint i=0; i<NumberOfEncodingTables; i++)
                {
                    EncodingTableEntry ete = GetEncodingTableEntry(i);
                    if (ete != null)
                    {
                        if (ete.offset == 0)
                        {
                            v.Error(T.cmap_SubtableOffset, 
                                    E.cmap_E_SubtableOffset_zero, m_tag, 
                                    "encoding table entry index = " + 
                                    i.ToString() + ", platformID = " + 
                                    ete.platformID + ", encodingID = " +
                                    ete.encodingID);
                            bRet = false;
                            bOffsetsOk = false;
                        }

                        if (ete.offset > m_bufTable.GetLength())
                        {
                            v.Error(T.cmap_SubtableOffset,
                                    E.cmap_E_SubtableOffset_eot, m_tag, 
                                    "encoding table entry index = " +
                                    i.ToString() + ", platformID = " +
                                    ete.platformID +
                                    ", encodingID = " + ete.encodingID);
                            bRet = false;
                            bOffsetsOk = false;
                        }
                    }
                    else
                    {
                        v.Warning(T.cmap_SubtableOffset, 
                                  W._TEST_W_OtherErrorsInTable, m_tag, 
                                  "cmap table appears to be corrupt. " + 
                                  " No further tests will be performed.");
                        return bRet; 
                    }
                }
                
                if (bOffsetsOk)
                {
                    v.Pass(T.cmap_SubtableOffset, P.cmap_P_SubtableOffset,
                           m_tag);
                }
            }

            // Check that each subtable can be retrieved from the font
            // and that its offset + length lies within the cmap.

            bool bLengthsOk = true;
            if (v.PerformTest(T.cmap_SubtableLength))
            {
                
                for (uint i=0; i<NumberOfEncodingTables; i++)
                {
                    EncodingTableEntry ete = GetEncodingTableEntry(i);
                    Subtable st = GetSubtable(ete);

                    if (st != null)
                    {
                        if (st.length == 0)
                        {
                            v.Error(T.cmap_SubtableLength,
                                    E.cmap_E_SubtableLength_zero, m_tag, 
                                    i.ToString());
                            bRet = false;
                            bLengthsOk = false;
                        }

                        if (ete.offset + st.length > m_bufTable.GetLength())
                        {
                            v.Error(T.cmap_SubtableLength,
                                    E.cmap_E_SubtableLength_eot, m_tag, 
                                    i.ToString());
                            bRet = false;
                            bLengthsOk = false;
                        }
                    }
                    else
                    {
                        string sDetails = "PlatID = " + ete.platformID +
                            ", EncID = " + ete.encodingID;
                        v.Warning(T.cmap_SubtableLength, 
                                  W._TEST_W_OtherErrorsInTable, m_tag,
                                  "unable to validate the length of subtable - "
                                  + sDetails);
                        bLengthsOk = false;
                    }
                }
                
                if (bLengthsOk)
                {
                    v.Pass(T.cmap_SubtableLength, P.cmap_P_SubtableLength,
                           m_tag);
                }
            }

            // Assuming previous tests ok, check that sort order is by
            // 1. increasing platformID
            // 2. increasing encodingID
            // 3. increasing language

            if (v.PerformTest(T.cmap_SubtableSortOrder))
            {
                if (bOffsetsOk && bLengthsOk)
                {
                    bool bOrderOk = true;

                    if (NumberOfEncodingTables > 1)
                    {
                        EncodingTableEntry etePrev = GetEncodingTableEntry(0);

                        for (uint i=1; i<NumberOfEncodingTables; i++)
                        {
                            EncodingTableEntry ete = GetEncodingTableEntry(i);

                            if (etePrev.platformID == ete.platformID)
                            {
                                if (etePrev.encodingID == ete.encodingID)
                                {
                                    Subtable stPrev = GetSubtable(etePrev);
                                    Subtable st = GetSubtable(ete);

                                    if (stPrev.language > st.language)
                                    {
                                        bOrderOk = false;
                                    }
                                }
                                else if (etePrev.encodingID > ete.encodingID)
                                {
                                    bOrderOk = false;
                                }
                            }
                            else if (etePrev.platformID > ete.platformID)
                            {
                                bOrderOk = false;
                            }

                            etePrev = ete;
                        }
                    }

                    if (bOrderOk)
                    {
                        v.Pass(T.cmap_SubtableSortOrder,
                               P.cmap_P_SubtableSortOrder, m_tag);
                    }
                    else
                    {
                        v.Error(T.cmap_SubtableSortOrder,
                                E.cmap_E_SubtableSortOrder, m_tag);
                        bRet = false;
                    }
                }
                else
                {
                    v.Warning(T.cmap_SubtableSortOrder,
                              W._TEST_W_OtherErrorsInTable, m_tag,
                              "unable to validate sort order");
                }
            }

            // Check that no encoding record has the same 
            //      (platformID,encodingID,language)
            // as the previous one.

            if (v.PerformTest(T.cmap_DuplicateSubtables))
            {
                if (bOffsetsOk && bLengthsOk)
                {
                    bool bNoDuplicates = true;

                    if (NumberOfEncodingTables > 1)
                    {
                        EncodingTableEntry etePrev = GetEncodingTableEntry(0);

                        for (uint i=1; i<NumberOfEncodingTables; i++)
                        {
                            EncodingTableEntry ete = GetEncodingTableEntry(i);

                            if (etePrev.platformID == ete.platformID)
                            {
                                if (etePrev.encodingID == ete.encodingID)
                                {
                                    Subtable stPrev = GetSubtable(etePrev);
                                    Subtable st = GetSubtable(ete);

                                    if (stPrev.language == st.language)
                                    {
                                        bNoDuplicates = false;
                                    }
                                }
                            }

                            etePrev = ete;
                        }
                    }

                    if (bNoDuplicates)
                    {
                        v.Pass(T.cmap_DuplicateSubtables,
                               P.cmap_P_DuplicateSubtables, m_tag);
                    }
                    else
                    {
                        v.Error(T.cmap_DuplicateSubtables,
                                E.cmap_E_DuplicateSubtables, m_tag);
                        bRet = false;
                    }
                }
                else
                {
                    string unable = "unable to validate that there are " + 
                        "no duplicate subtables";
                    v.Warning(T.cmap_DuplicateSubtables,
                              W._TEST_W_OtherErrorsInTable, m_tag,
                              unable );
                }
            }

            // Check that no subtable overlaps and subtable following it,
            // in other words, check for the beginning of one subtable 
            // falling within the bounds of a subtable that follows it.

            if (v.PerformTest(T.cmap_SubtableOverlap))
            {
                if (bOffsetsOk && bLengthsOk)
                {
                    bool bNoOverlap = true;

                    if (NumberOfEncodingTables > 1)
                    {
                        for (uint i=0; i<NumberOfEncodingTables; i++)
                        {
                            EncodingTableEntry ete1 = GetEncodingTableEntry(i);
                            Subtable st1 = GetSubtable(ete1);

                            for (uint j=i+1; j<NumberOfEncodingTables; j++)
                            {
                                EncodingTableEntry ete2 = 
                                    GetEncodingTableEntry(j);
                                Subtable st2 = GetSubtable(ete2);

                                if ( (ete1.offset > ete2.offset && 
                                      ete1.offset < ete2.offset + st2.length) ||
                                    (ete2.offset > ete1.offset &&
                                     ete2.offset < ete1.offset + st1.length) )
                                {
                                    v.Error(T.cmap_SubtableOverlap, 
                                            E.cmap_E_SubtableOverlap, m_tag,
                                            i.ToString() + " " + j.ToString());
                                    bRet = false;
                                    bNoOverlap = true;
                                }
                            }
                        }
                    }

                    if (bNoOverlap)
                    {
                        v.Pass(T.cmap_SubtableOverlap, 
                               P.cmap_P_SubtableOverlap, m_tag);
                    }
                }
                else
                {
                    v.Warning(T.cmap_SubtableOverlap, 
                              W._TEST_W_OtherErrorsInTable, m_tag,
                              "unable to validate that no subtables overlap");
                }
            }

            // Check that all subtable formats (First USHORT in the subtable)
            // is an even number between 0 and 14, inclusive.

            if (v.PerformTest(T.cmap_ValidFormat))
            {
                bool bAllFormatsValid = true;
                for (uint i=0; i<NumberOfEncodingTables; i++)
                {
                    EncodingTableEntry ete = GetEncodingTableEntry(i);
                    if (ete.offset < m_bufTable.GetLength()-1)
                    {
                        // the format of the subtable is the first uint16
                        ushort format = m_bufTable.GetUshort(ete.offset);

                        if (format > HIGHEST_FORMAT || ((format&1)==1))
                        {
                            string sDetails = "PlatID = " + ete.platformID +
                                ", EncID = " + ete.encodingID + ", Fmt = " + 
                                format;
                            v.Error(T.cmap_ValidFormat, 
                                    E.cmap_E_SubtableValidFormat, m_tag, 
                                    sDetails);
                            bRet = false;
                            bAllFormatsValid = false;
                        }
                    }
                    else
                    {
                        string sDetails = "PlatID = " + ete.platformID + 
                            ", EncID = " + ete.encodingID;
                        string unable = 
                            "unable to validate format number for subtable - ";
                        v.Warning(T.cmap_ValidFormat, 
                                  W._TEST_W_OtherErrorsInTable, m_tag, 
                                  unable + sDetails);
                        bAllFormatsValid = false;
                    }
                }

                if (bAllFormatsValid)
                {
                    v.Pass(T.cmap_ValidFormat, 
                           P.cmap_P_SubtableValidFormat, m_tag);
                }
            }

            // Assuming maxp is present, get the number of glyphs from maxp, 
            // and for each subtable, run its validate routine.

            if (v.PerformTest(T.cmap_SubtableInternalFormat))
            {
                if (fontOwner.GetTable("maxp") == null)
                {
                    string cant = 
                        "can't check subtable internal formats - " + 
                        "maxp table inaccessible";
                    v.Warning(T.cmap_SubtableInternalFormat, 
                              W._TEST_W_ErrorInAnotherTable, m_tag,
                              cant );
                }
                else
                {
                    for (uint i=0; i<NumberOfEncodingTables; i++)
                    {
                        bool bInternalFormatOk = true;
                        EncodingTableEntry ete = GetEncodingTableEntry(i);
                        Subtable st = GetSubtable(ete);
                        if (st != null)
                        {
                            ushort numGlyphs = fontOwner.GetMaxpNumGlyphs();
                            string sIdentity = "PlatID = " + ete.platformID +
                                ", EncID = " + ete.encodingID + ", Fmt = " + 
                                st.format;
                            
                            ISubtableValidate valSubtable =
                                (ISubtableValidate)st;
                            bInternalFormatOk = valSubtable.Validate(v, this,
                                                                     numGlyphs,
                                                                     sIdentity);
                            bRet &= bInternalFormatOk;
                            if (bInternalFormatOk)
                            {
                                v.Pass(T.cmap_SubtableInternalFormat,
                                       P.cmap_P_InternalFormat, m_tag, 
                                       sIdentity);
                            }
                        }
                        else
                        {
                            string sDetails = "PlatID = " + ete.platformID +
                                ", EncID = " + ete.encodingID;
                            v.Warning(T.cmap_SubtableInternalFormat,
                                      W._TEST_W_OtherErrorsInTable, m_tag, 
                                      "unable to validate internal format " +
                                      "for subtable - "
                                      + sDetails);
                        }
                    }
                }
            }

            // Pass if there is at least one subtable with platformID == 1 and
            // one subtable with platformID == 3.
            // Warn if either is missing.

            if (v.PerformTest(T.cmap_AppleMSSupport))
            {
                bool bFoundApple = false;
                bool bFoundMS    = false;

                for (uint i=0; i<NumberOfEncodingTables; i++)
                {
                    EncodingTableEntry ete = GetEncodingTableEntry(i);
                    if (ete.platformID == 1)
                    {
                        bFoundApple = true;
                    }
                    else if (ete.platformID == 3)
                    {
                        bFoundMS = true;
                    }
                }

                if (bFoundApple && bFoundMS)
                {
                    v.Pass(T.cmap_AppleMSSupport, P.cmap_P_AppleMSSupport,
                           m_tag);
                }
                else if (!bFoundApple)
                {
                    v.Warning(T.cmap_AppleMSSupport, W.cmap_W_AppleMSSupport_A,
                              m_tag);
                }
                else if (!bFoundMS)
                {
                    v.Warning(T.cmap_AppleMSSupport, W.cmap_W_AppleMSSupport_M,
                              m_tag);
                }
            }

            // Find encoding table with platformID==1, encodingID==0 
            // i.e., Macintosh. Make sure that the Apple Logo, code point 240,
            // is mapped to glyphID 0, a legal requirement for Microsoft fonts
            // developed for use on Apple platforms.

            if (v.PerformTest(T.cmap_AppleLogo))
            {
                EncodingTableEntry ete = GetEncodingTableEntry(1,0);
                if (ete != null)
                {
                    Subtable st = GetSubtable(ete);
                    if (st != null)
                    {
                        byte[] charbuf = new byte[2];
                        charbuf[0] = 240;
                        charbuf[1] = 0;
                        uint glyph = st.MapCharToGlyph(charbuf, 0);
                        if (glyph == 0)
                        {
                            v.Pass(T.cmap_AppleLogo, P.cmap_P_AppleLogo, m_tag);
                        }
                        else
                        {
                            v.Warning(T.cmap_AppleLogo, W.cmap_W_AppleLogo,
                                      m_tag);
                        }
                    }
                    else
                    {
                        string sDetails = "PlatID = " + ete.platformID +
                            ", EncID = " + ete.encodingID;
                        string unable = 
                            "unable to validate apple logo for subtable - ";
                        v.Warning(T.cmap_AppleLogo, 
                                  W._TEST_W_OtherErrorsInTable, m_tag,
                                  unable + sDetails);
                    }
                }
                else
                {
                    v.Warning(T.cmap_AppleLogo, W.cmap_W_AppleLogo_NoMap, 
                              m_tag);
                }
            }

            // Euro symbol.
            // Unless there is a Microsoft Symbol encoding cmap (3,0):
            // * Make sure that any apple cmap(1,0) contains a glyph U+00db
            // * Make sure that any Microsoft Unicode cmap (3,1) contains
            //   a glyph U+20AC.

            if (v.PerformTest(T.cmap_EuroGlyph))
            {
                EncodingTableEntry eteSym = GetEncodingTableEntry(3,0);
                if (eteSym == null)
                {
                    EncodingTableEntry eteMac = GetEncodingTableEntry(1,0);
                    if (eteMac != null)
                    {
                        Subtable st = GetSubtable(eteMac);
                        if (st != null)
                        {
                            byte[] charbuf = new byte[2];
                            charbuf[0] = 0xdb;
                            charbuf[1] = 0;
                            uint glyph = st.MapCharToGlyph(charbuf, 0);
                            if (glyph != 0)
                            {
                                v.Pass(T.cmap_EuroGlyph,
                                       P.cmap_P_EuroGlyph_Mac, m_tag);
                            }
                            else
                            {
                                v.Warning(T.cmap_EuroGlyph,
                                          W.cmap_W_EuroGlyph_Mac, m_tag);
                            }
                        }
                        else
                        {
                            string sDetails = "PlatID = " + eteMac.platformID +
                                ", EncID = " + eteMac.encodingID;
                            string unable = 
                                "unable to validate if euro glyph " + 
                                "is present for subtable - ";
                            v.Warning(T.cmap_EuroGlyph,
                                      W._TEST_W_OtherErrorsInTable, m_tag,
                                      unable + sDetails);
                        }
                    }

                    EncodingTableEntry eteUni = GetEncodingTableEntry(3,1);
                    if (eteUni != null)
                    {
                        Subtable st = GetSubtable(eteUni);
                        if (st != null)
                        {
                            byte[] charbuf = new byte[2];
                            charbuf[0] = 0xac;
                            charbuf[1] = 0x20;
                            uint glyph = st.MapCharToGlyph(charbuf, 0);
                            if (glyph != 0)
                            {
                                v.Pass(T.cmap_EuroGlyph,
                                       P.cmap_P_EuroGlyph_Uni, m_tag);
                            }
                            else
                            {
                                v.Warning(T.cmap_EuroGlyph,
                                          W.cmap_W_EuroGlyph_Uni, m_tag);
                            }
                        }
                        else
                        {
                            string sDetails = "PlatID = " + eteMac.platformID +
                                ", EncID = " + eteMac.encodingID;
                            string unable = "unable to validate if euro glyph" +
                                " is present for subtable - ";
                            v.Warning(T.cmap_EuroGlyph,
                                      W._TEST_W_OtherErrorsInTable, m_tag, 
                                      unable + sDetails);
                        }
                    }
                }
            }

            // Make sure that no glyphs, other than "fi" and "fl" ligatures,
            // are mapped from the "private use area", that is, 
            // 0xE000 - 0xF8FF. The above ligatures may only be a the 
            // prescribed places. 
            //
            // Checks that 0xf001 and 0xfb01 map to the same place, and
            // Checks that 0xf002 and 0xfb02 map to the same place.
            //
            // Only warn if a problem.

            if (v.PerformTest(T.cmap_PrivateUse))
            {
                EncodingTableEntry eteUni = GetEncodingTableEntry(3,1);
                if (eteUni != null)
                {
                    Subtable st = GetSubtable(eteUni);
                    if (st != null)
                    {
                        bool bFoundGlyph = false;
                        bool bFoundLigChar = false;
                        byte[] charbuf = new byte[2];

                        for (char c='\xe000'; c<'\xf8ff'; c++)
                        {
                            uint glyph = fontOwner.FastMapUnicodeToGlyphID(c);
                            if (glyph != 0)
                            {
                                if (c == '\xf001')
                                {
                                    // check to see if this is the 'fi' ligature
                                    uint glyph2 = fontOwner.
                                        FastMapUnicodeToGlyphID('\xfb01');
                                    if (glyph == glyph2)
                                    {
                                        bFoundLigChar = true;
                                    }
                                    else
                                    {
                                        bFoundGlyph = true;
                                        break;
                                    }
                                }
                                else if (c == '\xf002')
                                {
                                    // check to see if this is the 'fl' ligature
                                    uint glyph2 = fontOwner.
                                        FastMapUnicodeToGlyphID('\xfb02');
                                    if (glyph == glyph2)
                                    {
                                        bFoundLigChar = true;
                                    }
                                    else
                                    {
                                        bFoundGlyph = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    bFoundGlyph = true;
                                    break;
                                }
                            }
                        }
                        if (bFoundGlyph)
                        {
                            v.Warning(T.cmap_PrivateUse, 
                                      W.cmap_W_UnicodePrivateUse, m_tag);
                        }
                        else if (bFoundLigChar)
                        {
                            v.Pass(T.cmap_PrivateUse, 
                                   P.cmap_P_UnicodePrivateUse_lig, m_tag);
                        }
                        else
                        {
                            v.Pass(T.cmap_PrivateUse, 
                                   P.cmap_P_UnicodePrivateUse, m_tag);
                        }
                    }
                    else
                    {
                        string sDetails = "PlatID = " + eteUni.platformID + 
                            ", EncID = " + eteUni.encodingID;
                        v.Warning(T.cmap_PrivateUse,
                                  W._TEST_W_OtherErrorsInTable, m_tag, 
                                  "unable to validate Private Use Area " + 
                                  "for subtable - "
                                  + sDetails);
                    }
                }
            }

            // Check that subtable language field is zero if not mac platform

            if (v.PerformTest(T.cmap_NonMacSubtableLanguage))
            {
                bool bOk = true;
                for (uint i=0; i<NumberOfEncodingTables; i++)
                {
                    EncodingTableEntry ete = GetEncodingTableEntry(i);
                    Subtable st = GetSubtable(ete);
                    if (st != null)
                    {
                        if (ete.platformID != 1) // not mac
                        {
                            if (st.language != 0)
                            {
                                string sDetails = "PlatID = " +
                                    ete.platformID + ", EncID = " +
                                    ete.encodingID +
                                    ", Language = " + st.language;
                                v.Error(T.cmap_SubtableLanguage, 
                                        E.cmap_E_NonMacSubtableLanguage,
                                        m_tag, sDetails);
                                bOk = false;
                            }
                        }
                    }
                }
                if (bOk)
                {
                    v.Pass(T.cmap_SubtableLanguage,
                           P.cmap_P_NonMacSubtableLanguage, m_tag);
                }
            }

            return bRet;
        }



        interface ISubtableValidate
        {
            bool Validate(Validator v, OTTable table, ushort numGlyphs, 
                          String sIdentity);
        }


        public class Format0_val : Format0, ISubtableValidate
        {
            public Format0_val(EncodingTableEntry ete, MBOBuffer bufTable)
                : base(ete, bufTable)
            {
            }

            public bool Validate(Validator v, OTTable table, ushort numGlyphs,
                                 String sIdentity)
            {
                bool bRet = true;

                // check the length

                if (length != 262)
                {
                    v.Error(T.T_NULL, E.cmap_E_f0_length, table.m_tag, 
                            sIdentity);
                    bRet = false;
                }

                // check the mapping
                for (char c = (char)0; c<256; c++)
                {
                    ushort nGlyphID = GetGlyphID(c);
                    if (nGlyphID >= numGlyphs)
                    {
                        v.Error(T.T_NULL, E.cmap_E_Mapping, table.m_tag,
                                sIdentity + ", char = 0x" +
                                ((uint)c).ToString("X4") + ", glyphID = " + 
                                nGlyphID);
                        bRet = false;
                    }
                }


                return bRet;
            }
        }

        public class Format2_val : Format2, ISubtableValidate
        {
            public Format2_val(EncodingTableEntry ete, MBOBuffer bufTable)
                : base(ete, bufTable)
            {
            }

            public bool Validate(Validator v, OTTable table, ushort numGlyphs,
                                 String sIdentity)
            {
                bool bRet = true;

                
                // check the mapping

                byte [] charbuf = new byte[2];
                for (uint i = 0; i<=65535; i++)
                {
                    char c = (char)i;
                    charbuf[0] = (byte)c;
                    charbuf[1] = (byte)(c>>8);
                    uint nGlyphID = MapCharToGlyph(charbuf, 0);
                    if (nGlyphID >= numGlyphs)
                    {
                        v.Error(T.T_NULL, E.cmap_E_Mapping, table.m_tag, 
                                sIdentity + ", char = 0x" + 
                                ((uint)c).ToString("X4") + ", glyphID = " +
                                nGlyphID);
                        bRet = false;
                    }
                }


                return bRet;
            }
        }

        public class Format4_val : Format4, ISubtableValidate
        {
            public Format4_val(EncodingTableEntry ete, MBOBuffer bufTable) 
                : base(ete, bufTable)
            {
            }

            public bool Validate(Validator v, OTTable table, ushort numGlyphs,
                                 String sIdentity)
            {
                bool bRet = true;

                // check that segCountX2 is even
                if ((segCountX2 & 1) == 1)
                {
                    v.Error(T.T_NULL, E.cmap_E_f4_segCountX2, table.m_tag, 
                            sIdentity + ", segCountX2 = " + segCountX2);
                    bRet = false;
                }

                // check searchRange
                ushort segCount = (ushort)(segCountX2 / 2);
                ushort CalculatedSearchRange  = 
                    (ushort)(2 * util.MaxPower2LE(segCount));
                if (searchRange != CalculatedSearchRange)
                {
                    v.Error(T.T_NULL, E.cmap_E_f4_searchRange, table.m_tag, 
                            sIdentity + ", searchRange = " + searchRange +
                            ", calc = " + CalculatedSearchRange);
                    bRet = false;
                }

                // check entrySelector
                ushort CalculatedEntrySelector = 
                    util.Log2((ushort)(CalculatedSearchRange/2));
                if (entrySelector != CalculatedEntrySelector)
                {
                    v.Error(T.T_NULL, E.cmap_E_f4_entrySelector, table.m_tag, 
                            sIdentity + ", entrySelector = " + entrySelector +
                            ", calc = " + CalculatedEntrySelector);
                    bRet = false;
                }

                // check rangeShift
                ushort CalculatedRangeShift    = 
                    (ushort)(2 * segCount - CalculatedSearchRange);
                if (rangeShift != CalculatedRangeShift)
                {
                    v.Error(T.T_NULL, E.cmap_E_f4_rangeShift, table.m_tag,
                            sIdentity + ", rangeShift = " + rangeShift + 
                            ", calc = " + CalculatedRangeShift);
                    bRet = false;
                }

                // check that final endCode value is 0xffff
                if (GetEndCode((uint)segCount-1) != 0xffff)
                {
                    v.Error(T.T_NULL, E.cmap_E_f4_FinalEndCode, table.m_tag, 
                            sIdentity + ", endCode[" + (segCount-1) + "] = " +
                            GetEndCode((uint)segCount-1));
                    bRet = false;
                }

                // check that end character codes are in ascending order
                if (segCount > 1)
                {
                    for (uint i=0; i<segCount-1; i++)
                    {
                        if (GetEndCode(i) >= GetEndCode(i+1))
                        {
                            v.Error(T.T_NULL, E.cmap_E_f4_EndCodeOrder, 
                                    table.m_tag, sIdentity);
                            bRet = false;
                            break;
                        }
                    }
                }

                // check that the reservedPad is zero
                if (   m_ete.offset + (uint)FieldOffsets.endCode + segCountX2 
                       < m_bufTable.GetLength()
                       && reservedPad != 0)
                {
                    v.Error(T.T_NULL, E.cmap_E_f4_reservedPad, table.m_tag,
                            sIdentity);
                    bRet = false;
                }

                // check that each start character code is not greater than 
                // the corresponding end character code
                for (uint i=0; i<segCount; i++)
                {
                    if (GetStartCode(i) > GetEndCode(i))
                    {
                        String sDetails = ", startCode[" + i + "] = " +
                            GetStartCode(i) + " , endCode[" + i + "] = " +
                            GetEndCode(i);
                        v.Error(T.T_NULL, E.cmap_E_f4_StartCode_GT_EndCode, 
                                table.m_tag, sIdentity + sDetails);
                        bRet = false;
                    }
                }

                // check that each delta value (where idRangeOffset == 0) 
                // added to the start code is not negative
                for (uint i=0; i<segCount; i++)
                {
                    if (GetIdRangeOffset(i) == 0)
                    {
                        if (GetIdDelta(i) + GetStartCode(i) < 0)
                        {
                            String sDetails = ", idDelta[" + i + "] = " +
                                GetIdDelta(i) + ", startCode[" + i + "] = " + 
                                GetStartCode(i);
                            v.Error(T.T_NULL, E.cmap_E_f4_idDeltaNeg,
                                    table.m_tag, sIdentity + sDetails);
                            bRet = false;
                        }
                    }
                }

                // check range offsets
                for (uint i=0; i<segCount; i++)
                {
                    ushort idRangeOffset = GetIdRangeOffset(i);
                    ushort endCode = GetEndCode(i);
                    ushort startCode = GetStartCode(i);
                    if (idRangeOffset != 0)
                    {
                        uint AddressOfIdRangeOffset =
                            (uint)FieldOffsets.endCode + segCountX2*3u + 2 + 
                            i*2;
                        uint obscureIndex = (uint)
                            (idRangeOffset + (endCode - startCode)*2 + 
                             AddressOfIdRangeOffset);
                        if (obscureIndex > length)
                        {
                            v.Error(T.T_NULL, E.cmap_E_f4_idRangeOffset,
                                    table.m_tag, sIdentity + 
                                    ", idRangeOffset[" + i + "] = " + 
                                    idRangeOffset);
                            bRet = false;
                        }
                    }
                }

                // check that glyph ids are less than numGlyphs
                for (uint i=0; i<segCount; i++)
                {
                    int nGlyphMappingErrors = 0;
                    // Report the first 10 errors and 
                    // a count of errors after that.
                    int nMaxMappingErrors = 10; 
                    ushort idRangeOffset = GetIdRangeOffset(i);
                    ushort endCode = GetEndCode(i);
                    ushort startCode = GetStartCode(i);
                    short idDelta = GetIdDelta(i);
                    ushort nGlyph;

                    for (uint j=startCode; j<= endCode; j++)
                    {
                        ushort c = (ushort)j;
                        nGlyph = 0;
                        if (idRangeOffset == 0)
                        {
                            nGlyph = (ushort)(c + idDelta);
                        }
                        else
                        {
                            uint AddressOfIdRangeOffset = (uint)
                                FieldOffsets.endCode + segCountX2*3u + 2 + i*2;
                            uint obscureIndex = (uint)
                                (idRangeOffset + (c-startCode)*2 + 
                                 AddressOfIdRangeOffset);
                            // throw out invalid indexes for this test 
                            // (they should have already been reported)
                            if (obscureIndex < length && 
                                (m_ete.offset + obscureIndex < 
                                 m_bufTable.GetLength()))
                            {
                                nGlyph = 
                                    m_bufTable.GetUshort(m_ete.offset + 
                                                         obscureIndex);
                            }
                            if (nGlyph !=0 )
                            {
                                nGlyph = (ushort)(nGlyph + idDelta);
                            }
                        }

                        if (nGlyph > numGlyphs)
                        {
                            // it is possible that an entire segment is bad.
                            // That causes a hang like behavior and a 
                            // bloated error file
                            // that cannot be opened because it is so huge. 
                            if (nGlyphMappingErrors < nMaxMappingErrors)
                            {
                                v.Error(T.T_NULL, E.cmap_E_Mapping, 
                                        table.m_tag, 
                                        sIdentity + ", char = 0x" +
                                        c.ToString("X4") + ", glyphID = " + 
                                        nGlyph);
                            }
                            nGlyphMappingErrors++;
                        }
                    }

                    if (nGlyphMappingErrors >= nMaxMappingErrors)
                    {
                        string sDetails = sIdentity + ", segment = " + i + 
                            " has " + nGlyphMappingErrors + 
                            " glyphs mapped out of range";    
                        v.Error(T.T_NULL, E.cmap_E_Mapping, table.m_tag, 
                                sDetails);
                        bRet = false;
                    }
                }

                return bRet;
            }
        }

        public class Format6_val : Format6, ISubtableValidate
        {
            public Format6_val(EncodingTableEntry ete, MBOBuffer bufTable)
                : base(ete, bufTable)
            {
            }
            public bool Validate(Validator v, OTTable table, ushort numGlyphs,
                                 String sIdentity)
            {
                bool bRet = true;


                // check the mapping

                byte [] charbuf = new byte[2];
                for (uint i = 0; i<=65535; i++)
                {
                    char c = (char)i;
                    charbuf[0] = (byte)c;
                    charbuf[1] = (byte)(c>>8);
                    uint nGlyphID = MapCharToGlyph(charbuf, 0);
                    if (nGlyphID >= numGlyphs)
                    {
                        v.Error(T.T_NULL, E.cmap_E_Mapping, table.m_tag, 
                                sIdentity + ", char = 0x" + 
                                ((uint)c).ToString("X4") + ", glyphID = " + 
                                nGlyphID);
                        bRet = false;
                    }
                }


                return bRet;
            }
        }

        public class Format8_val : Format8, ISubtableValidate
        {
            public Format8_val(EncodingTableEntry ete, MBOBuffer bufTable) :
                base(ete, bufTable)
            {
            }
            public bool Validate(Validator v, OTTable table, ushort numGlyphs,
                                 String sIdentity)
            {
                bool bRet = true;


                // check the mapping

                byte [] charbuf = new byte[2];
                for (uint i = 0; i<=65535; i++)
                {
                    char c = (char)i;
                    charbuf[0] = (byte)c;
                    charbuf[1] = (byte)(c>>8);
                    uint nGlyphID = MapCharToGlyph(charbuf, 0);
                    if (nGlyphID >= numGlyphs)
                    {
                        v.Error(T.T_NULL, E.cmap_E_Mapping, table.m_tag, 
                                sIdentity + ", char = 0x" + 
                                ((uint)c).ToString("X4") + ", glyphID = " + 
                                nGlyphID);
                        bRet = false;
                    }
                }


                return bRet;
            }
        }

        public class Format10_val : Format10, ISubtableValidate
        {
            public Format10_val(EncodingTableEntry ete, MBOBuffer bufTable) :
                base(ete, bufTable)
            {
            }

            
            public bool Validate(Validator v, OTTable table, ushort numGlyphs,
                                 String sIdentity)
            {
                bool bRet = true;


                // check the mapping

                byte [] charbuf = new byte[2];
                for (uint i = 0; i<=65535; i++)
                {
                    char c = (char)i;
                    charbuf[0] = (byte)c;
                    charbuf[1] = (byte)(c>>8);
                    uint nGlyphID = MapCharToGlyph(charbuf, 0);
                    if (nGlyphID >= numGlyphs)
                    {
                        v.Error(T.T_NULL, E.cmap_E_Mapping, table.m_tag,
                                sIdentity + ", char = 0x" + 
                                ((uint)c).ToString("X4") + ", glyphID = " + 
                                nGlyphID);
                        bRet = false;
                    }
                }


                return bRet;
            }
        }

        public class Format12_val : Format12, ISubtableValidate
        {
            public Format12_val(EncodingTableEntry ete, MBOBuffer bufTable) 
                : base(ete, bufTable)
            {
            }


            public bool Validate(Validator v, OTTable table, ushort numGlyphs,
                                 String sIdentity)
            {
                bool bRet = true;

                // warn if not encoding id 10
                if (m_ete.encodingID != 10)
                {
                    v.Warning(T.T_NULL, W.cmap_W_f12_EncID, table.m_tag, 
                              sIdentity);
                }

                // check that groups are sorted by ascending startCharCodes
                if (nGroups > 1)
                {
                    for (uint i=0; i<nGroups-1; i++)
                    {
                        Group gCurr = GetGroup(i);
                        Group gNext = GetGroup(i+1);

                        if (gCurr.startCharCode >= gNext.startCharCode)
                        {
                            v.Error(T.T_NULL, E.cmap_E_f12_SortOrder, 
                                    table.m_tag, sIdentity);
                            bRet = false;
                            break;
                        }
                    }
                }

                // check each start code is less than or equal to the end code
                for (uint i=0; i<nGroups; i++)
                {
                    Group g = GetGroup(i);
                    if (g.startCharCode > g.endCharCode)
                    {
                        String sDetails = ", group[" + i +
                            "], startCharCode = 0x" +
                            g.startCharCode.ToString("X8") + 
                            ", endCharCode = 0x" + g.endCharCode.ToString("X8");
                        v.Error(T.T_NULL, E.cmap_E_f12_StartCode_GT_EndCode, 
                                table.m_tag, sIdentity + sDetails);
                        bRet = false;
                    }
                }

                // check the mapping
                for (uint i=0; i<nGroups; i++)
                {
                    Group g = GetGroup(i);
                    uint c = g.endCharCode;
                    uint nGlyphID = g.startGlyphID + (c - g.startCharCode);
                    if (nGlyphID >= numGlyphs)
                    {
                        v.Error(T.T_NULL, E.cmap_E_Mapping, table.m_tag, 
                                sIdentity + ", char = 0x" + 
                                ((uint)c).ToString("X4") + ", glyphID = " + 
                                nGlyphID);
                        bRet = false;
                    }
                }

                
                // if encoding id 10, then check that the greatest character
                // is not greater than 0x10ffff
                if (m_ete.encodingID == 10)
                {
                    if (nGroups > 0)
                    {
                        Group lastgroup = GetGroup(nGroups-1);
                        if (lastgroup.endCharCode > 0x10ffff)
                        {
                            v.Error(T.T_NULL, E.cmap_E_f12_EndCode_GT_10FFFF, 
                                    table.m_tag, sIdentity + 
                                    ", last endCharCode = 0x" + 
                                    lastgroup.endCharCode.ToString("X8"));
                            bRet = false;
                        }
                    }
                }

                return bRet;
            }
        }

        public class Format14_val : Format14, ISubtableValidate

        {
            public Format14_val( EncodingTableEntry ete, MBOBuffer bufTable ) 
                : base(ete, bufTable)
            {
            }


            static String s_NoteIndex1 = 
                " (Note that the first Variation Selector Record is being " +
                " called 'Record #1', not 'Record #0')";

            Validator m_v;
            OTTable   m_table;
            String    m_sIdentity;
            ushort    m_numGlyphs;

            // FIX: This should be shared by many formats!
            private String SDetails() {
                return "PlatID = " + m_ete.platformID + 
                    ", EncID = " + m_ete.encodingID;
            }

            private void VWarn( W warn, String desc )
            {
                m_v.Warning( T.T_NULL, warn, m_table.m_tag, 
                             m_sIdentity+", "+desc );
            }
            

            private void VError( E err, String desc )
            {
                m_v.Error( T.T_NULL, err, m_table.m_tag, 
                           m_sIdentity+", "+desc );
            }
            
            private bool IsLegalVarSelector( uint varSelector )
            {
                if ( varSelector >= 0xFE00 && varSelector <= 0xFE0F ) {
                    return true;
                }
                if ( varSelector >= 0xE0100 && varSelector <= 0xE01EF ) {
                    return true;
                }
                // Fix. See if there are other legal ranges
                return false;
            }

            private bool IsLegalUnicodeCodePoint( uint cp )
            {
                // FIX
                if ( cp >= 0x10FFFF ) {
                    // Error
                    return false;
                }
                return true;
            }

            private bool CheckLegalValues( List<VarSelectorRecord> recs )
            {
                bool bRet = true;
                uint prevVarSelector = 0;
                
                for ( int i = 0; i < recs.Count; i++ ) {
                    VarSelectorRecord r = recs[i];
                    if ( !IsLegalVarSelector( r.varSelector ) ) {
                        VError( E.cmap_E_f14_VarSelector,
                                "Variation Selector Record #" + (i+1) +
                                ", illegal Variation Selector is 0x"+
                                r.varSelector.ToString("X") + s_NoteIndex1 );
                        bRet = false;
                    }
                    if ( r.varSelector < prevVarSelector ) {
                        bRet = false;
                        VError( E.cmap_E_f14_VarSelector_Order,
                                "Variation Selector Record #" + (i+1) +
                                " contains Variation Selector 0x" + 
                                r.varSelector.ToString("X") + 
                                ", which is smaller than the " +
                                " Variation Selector 0x" +
                                prevVarSelector.ToString("X") +
                                " in Record #" + i + s_NoteIndex1 );
                    }
                    if ( r.varSelector == prevVarSelector ) {
                        bRet = false;
                        VError( E.cmap_E_f14_VarSelector_Duplicate,
                                "Variation Selector Records #" + i +
                                " and #" + (i+1) + " are both for Variation " +
                                "Selector 0x" + 
                                r.varSelector.ToString("X") + s_NoteIndex1 );
                    }
                    prevVarSelector = r.varSelector;
                }

                for ( int i = 0; i < recs.Count; i++ ) {
                    VarSelectorRecord r = recs[i];
                    if ( r.defaultUVSOffset > 0 ) {
                        DefaultUVSTable map = r.GetDefaultUVSTable();
                        uint lastCodePoint = 0;
                        for ( int k = 0; k < map.ranges.Count; k++ ) {
                            UnicodeValueRange m = map.ranges[k];
                            uint from = m.startUnicodeValue;
                            uint to = from + m.additionalCount;
                            if ( !IsLegalUnicodeCodePoint( from ) ) {
                                bRet = false;
                                VError( E.cmap_E_f14_CodePoint,
                                        "Variation Selector Record #" + (i+1) +
                                        ", Unicode Range #" + (k+1) + 
                                        ", 0x" + from.ToString("X") +
                                        s_NoteIndex1 );
                            }
                            if ( !IsLegalUnicodeCodePoint( to ) ) {
                                bRet = false;
                                VError( E.cmap_E_f14_CodePoint,
                                        "Variation Selector Record #" + (i+1) +
                                        ", Unicode Range #" + (k+1) + 
                                        ", 0x" + to.ToString("X") + 
                                        s_NoteIndex1 );
                            }
                            if ( k > 0 && from <= lastCodePoint ) {
                                bRet = false;
                                VError( E.cmap_E_f14_DefaultUVS_Sort,
                                        "Variation Selector Record #" + (i+1) +
                                        ", Unicode Range #" + (k+1) + 
                                        ", Range starts with Unicode 0x" + 
                                        from.ToString("X") + 
                                        ", but the previous range ends with" +
                                        " 0x" + lastCodePoint.ToString("X") +
                                        ", which is the same or larger" +
                                        s_NoteIndex1 );
                            }
                            lastCodePoint = to;
                            if ( to > 0xFFFFFF ) {
                                bRet = false;
                                VError( E.cmap_E_f14_LastCode_GT_FFFFFF,
                                        "Variation Selector Record #" + (i+1) +
                                        ", Unicode Range #" + (k+1) + 
                                        ", end with Unicode 0x" +
                                        to.ToString("X") +
                                        ", which is larger than 0XFFFFFF" +
                                        s_NoteIndex1 );
                                    
                            }
                        }
                    }

                    if ( r.nonDefaultUVSOffset > 0 ) {
                        NonDefaultUVSTable map = r.GetNonDefaultUVSTable();
                        uint lastCodePoint = 0;
                        for ( int k = 0; k < map.mappings.Count; k++ ) {
                            UVSMapping m = map.mappings[k];
                            if ( !IsLegalUnicodeCodePoint( m.unicodeValue ) ) {
                                bRet = false;
                                VError( E.cmap_E_f14_CodePoint,
                                        "Variation Selector Record #" + (i+1) +
                                        ", UVS Mapping #" + (k+1) + ": " +
                                        "Unicode code point 0x" +
                                        m.unicodeValue.ToString("X") +
                                        s_NoteIndex1 );
                            }
                            if ( k > 0 && m.unicodeValue <= lastCodePoint ) {
                                bRet = false;
                                VError( E.cmap_E_f14_NonDefaultUVS_Sort,
                                        "Variation Selector Record #" + (i+1) +
                                        ", UVS Mapping #" + (k+1) + ": " +
                                        "Range starts with Unicode 0x" + 
                                        m.unicodeValue.ToString("X") +
                                        ", but the previous code point was " + 
                                        "Unicode 0x" + 
                                        lastCodePoint.ToString("X") +
                                        ", which is the same or larger" +
                                        s_NoteIndex1 );
                            }
                            lastCodePoint = m.unicodeValue;
                            if ( m.glyphID >= m_numGlyphs ) {
                                bRet = false;
                                VError( E.cmap_E_f14_GlyphID_Range,
                                        "Variation Selector Record #" + (i+1) +
                                        ", UVS Mapping #" + (k+1) + ": " +
                                        "(Unicode 0x" + 
                                        m.unicodeValue.ToString("X") + 
                                        " mapped to glyphID " + 
                                        m.glyphID.ToString() +
                                        "), but the number of glyphs " + 
                                        "in the font is " +
                                        m_numGlyphs + s_NoteIndex1 );
                            }
                        }
                    }
                }
                return bRet;
            }

            /// <summary>
            /// While checking lengths, accumulate set of variation selector
            /// records and add them to <c>recs</c>
            /// </summary>
            private bool CheckLengths( uint dataStart,
                                       List<VarSelectorRecord>recs )
            {
                bool bLengthsOk = true;
                for ( uint i=0; i < NumVarSelectorRecs; i++ ) 
                {
                    VarSelectorRecord r = GetIthSelectorRecord( i );
                    if ( r.defaultUVSOffset > 0 ) {
                        bool stop = false;
                        if ( r.defaultUVSOffset < dataStart ) {
                            bLengthsOk = false;
                            VError( E.cmap_E_f14_DefaultUVS_TooSmall,
                                    "Variation Selector Record #" + (i+1) +
                                    " Default UVS Table Offset = " + 
                                    r.defaultUVSOffset + 
                                    s_NoteIndex1 );
                            stop = true;
                        }
                        else if ( r.defaultUVSOffset >= length ) {
                            bLengthsOk = false;
                            VError( E.cmap_E_f14_DefaultUVS_EOT,
                                    "Variation Selector Record #" + (i+1) +
                                    " Default UVS Table Offset = " + 
                                    r.defaultUVSOffset +
                                    s_NoteIndex1 );
                            stop = true;
                        }
                        uint maxDefault = r.GetDefaultUVSMaxOffset();
                        if ( !stop && maxDefault >= length ) {
                            bLengthsOk = false;
                            VError( E.cmap_E_f14_DefaultUVS_EOT,
                                    "Variation Selector Record #" + (i+1) +
                                    " Default UVS Table End Offset = " + 
                                    maxDefault +
                                    s_NoteIndex1 );
                            stop = true;
                        }
                    }

                    if ( r.nonDefaultUVSOffset > 0 ) {
                        bool stop = false;
                        if ( r.nonDefaultUVSOffset < dataStart ) {
                            bLengthsOk = false;
                            VError( E.cmap_E_f14_NonDefaultUVS_TooSmall,
                                    "Variation Selector Record #" + (i+1) +
                                    " Non-Default UVS Table Offset = " +
                                    r.nonDefaultUVSOffset +
                                    s_NoteIndex1 );
                            stop = true;
                        }
                        else if ( r.nonDefaultUVSOffset >= length ) {
                            bLengthsOk = false;
                            VError( E.cmap_E_f14_NonDefaultUVS_EOT,
                                    "Variation Selector Record #" + (i+1) +
                                    " Non-Default UVS Table Offset = " + 
                                    r.nonDefaultUVSOffset +
                                    s_NoteIndex1 );
                            stop = true;
                        }
                        uint maxNonDefault = r.GetNonDefaultUVSMaxOffset();
                        if ( !stop && maxNonDefault >= length ) {
                            bLengthsOk = false;
                            VError( E.cmap_E_f14_NonDefaultUVS_EOT,
                                    "Variation Selector Record #" + (i+1) +
                                    " Non-Default UVS Table End Offset = " + 
                                    maxNonDefault +
                                    s_NoteIndex1 );
                            stop = true;
                        }
                    }
                    recs.Add( r );
                }
                return bLengthsOk;
            }



            // Used only in CheckOverlaps. Can it be declared there somehow?
            struct Region { 
                public uint start; 
                public uint end; 
                public int  recIndex; 
                public bool isDefault;
                public Region( uint s, uint e, int i, bool isD ) {
                    start = s; end = e; recIndex = i; isDefault = isD;
                }
            }

            private bool CheckOverlaps( uint dataStart,
                                        List<VarSelectorRecord>recs )
            {
                bool isOk = true;
                List<Region> regions = new List<Region>();
                
                for ( int i = 0; i < recs.Count; i++ ) {
                    VarSelectorRecord r = recs[i];
                    if ( r.defaultUVSOffset > 0 ) {
                        regions.Add( new Region( r.defaultUVSOffset,
                                                 r.GetDefaultUVSMaxOffset(),
                                                 i, true ) );
                    }
                    if ( r.nonDefaultUVSOffset > 0 ) {
                        regions.Add( new Region( r.nonDefaultUVSOffset,
                                                 r.GetNonDefaultUVSMaxOffset(),
                                                 i, false ) );
                    }
                }
     
                // Sort the list by region start. Slick!
                regions.Sort( 
                              delegate(Region r1, Region r2) { 
                                  return r1.start.CompareTo( r2.start ); 
                              } );


                // Check for packing and overlaps
                uint lastEnd = dataStart - 1;
                for ( int i = 0; i < regions.Count; i++ ) {
                    Region r = regions[i];
                    uint expected = lastEnd + 1;
                    if ( i > 0 && r.start < expected ) {
                        Region p = regions[i-1];
                        VError( E.cmap_E_f14_OverlappingMaps,
                                "Tables for Variation Selector Records #" + 
                                (r.recIndex + 1) + "(" + 
                                (r.isDefault? "default" : "non-default") + 
                                ")" +
                                " and " +
                                (p.recIndex + 1) + "(" + 
                                (p.isDefault? "default" : "non-default") + 
                                ")" +
                                " are overlapping. " + s_NoteIndex1 );
                        isOk = false;
                    }
                    else if ( r.start > expected ) {
                        VWarn( W.cmap_W_f14_NonPacked, 
                               "There is unused space before the " +
                               (r.isDefault? "default" : "non-default") +
                               " table of record #" + (r.recIndex+1) + 
                               s_NoteIndex1 );
                    }
                    lastEnd = r.end;
                }
                return isOk;
            }

            public bool Validate( Validator v, 
                                  OTTable table, 
                                  ushort numGlyphs,
                                  String sIdentity )
            {

                bool bRet = true;

                m_v = v;
                m_table = table;
                m_sIdentity = sIdentity;
                m_numGlyphs = numGlyphs;

                List<VarSelectorRecord> recs = new List<VarSelectorRecord>();

                // Find the offset of the start of the Default and
                // Non-Default UVS tables to make sure that
                // everything the offsets point to is within the table
                // area.
                const uint f14HeaderLength = 10; // USHORT, ULONG, ULONG
                const uint varSelRecLength = 11; // UINT24, ULONG, ULONG
                uint dataStart = f14HeaderLength + 
                    ( NumVarSelectorRecs * varSelRecLength );
                        
                // Fill in recs while we are at it.
                bool bLengthsOk = CheckLengths( dataStart, recs );
                bRet = bLengthsOk;

                if ( !bLengthsOk ) {
                    string unable = "Unable to perform more tests";
                    // Is the 2nd arg correct?
                    v.Warning( T.T_NULL, W._TEST_W_OtherErrorsInTable, 
                               m_table.m_tag, unable + SDetails() );
                    goto Finish;
                } 
                
                // We now know that all the tables are within the data area,
                // that is, from dataStart to the end of the subtable. Now
                // we about overlapping and non-packed tables.
                bool bOverlapsOk = CheckOverlaps( dataStart, recs );
                if ( !bOverlapsOk ) {
                    bRet = false;
                    string unable = "Unable to perform more tests";
                    // Is the 2nd arg correct?
                    v.Warning( T.T_NULL, W._TEST_W_OtherErrorsInTable, 
                               m_table.m_tag, unable + SDetails() );
                    goto Finish;
                }

                // Check that all values in offset tables are legal
                bRet = CheckLegalValues( recs );

                Finish:
                return bRet;
            }
        }

    }
}
