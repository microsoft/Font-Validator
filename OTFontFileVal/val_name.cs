using System;
using System.Diagnostics;

using OTFontFile;
using Win32APIs;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_name.
    /// </summary>
    public class val_name : Table_name, ITableValidate
    {
        /************************
         * constructors
         */
        
        public val_name(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            if (v.PerformTest(T.name_FormatSelector))
            {
                if (FormatSelector == 0)
                {
                    v.Pass(T.name_FormatSelector, P.name_P_FormatSelector, m_tag);
                }
                else
                {
                    v.Error(T.name_FormatSelector, E.name_E_FormatSelector, m_tag, FormatSelector.ToString());
                    bRet = false;
                }
            }

            if (v.PerformTest(T.name_StringsWithinTable))
            {
                bool bStringsWithinTable = true;

                uint tableLength = GetLength();
                for (uint i=0; i<NumberNameRecords; i++)
                {
                    NameRecord nr = GetNameRecord(i);
                    if (nr != null)
                    {
                        if (nr.StringOffset + nr.StringLength > tableLength)
                        {
                            v.Error(T.name_StringsWithinTable, E.name_E_StringsWithinTable, m_tag, "string# " + i);
                            bStringsWithinTable = false;
                            bRet = false;
                        }
                    }
                    else
                    {
                        v.Warning(T.name_StringsWithinTable, W._TEST_W_OtherErrorsInTable, m_tag);
                        bStringsWithinTable = false;
                        break;
                    }
                }
                if (bStringsWithinTable)
                {
                    v.Pass(T.name_StringsWithinTable, P.name_P_StringsWithinTable, m_tag);
                }
            }

            if (v.PerformTest(T.name_NameRecordsSorted))
            {
                bool bSortedOrder = true;

                if (NumberNameRecords > 1)
                {
                    NameRecord CurrNR = GetNameRecord(0);
                    NameRecord NextNR = null;
                    for (uint i=0; i<NumberNameRecords-1; i++)
                    {
                        NextNR = GetNameRecord(i+1);

                        if (CurrNR == null || NextNR == null)
                        {
                            bSortedOrder = false;
                            break;
                        }

                        if (CurrNR.PlatformID > NextNR.PlatformID)
                        {
                            bSortedOrder = false;
                            break;
                        }
                        else if (CurrNR.PlatformID == NextNR.PlatformID)
                        {
                            if (CurrNR.EncodingID > NextNR.EncodingID)
                            {
                                bSortedOrder = false;
                                break;
                            }
                            else if (CurrNR.EncodingID == NextNR.EncodingID)
                            {
                                if (CurrNR.LanguageID > NextNR.LanguageID)
                                {
                                    bSortedOrder = false;
                                    break;
                                }
                                else if (CurrNR.LanguageID == NextNR.LanguageID)
                                {
                                    if (CurrNR.NameID > NextNR.NameID)
                                    {
                                        bSortedOrder = false;
                                        break;
                                    }
                                }
                            }
                        }

                        CurrNR = NextNR;
                    }
                }

                if (bSortedOrder)
                {
                    v.Pass(T.name_NameRecordsSorted, P.name_P_NameRecordsSorted, m_tag);
                }
                else
                {
                    v.Error(T.name_NameRecordsSorted, E.name_E_NameRecordsSorted, m_tag);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.name_ReservedNameIDs))
            {
                bool bReservedOk = true;

                for (uint i=0; i<NumberNameRecords; i++)
                {
                    NameRecord nr = GetNameRecord(i);

                    if (nr != null)
                    {
                        if (nr.NameID >= 21 && nr.NameID <= 255)
                        {
                            string s = "platID = " + nr.PlatformID 
                                + ", encID = " + nr.EncodingID
                                + ", langID = " + nr.LanguageID
                                + ", nameID = " + nr.NameID;
                            v.Error(T.name_ReservedNameIDs, E.name_E_ReservedNameID, m_tag, s);
                            bReservedOk = false;
                            break;
                        }
                    }
                    else
                    {
                        v.Warning(T.name_ReservedNameIDs, W._TEST_W_OtherErrorsInTable, m_tag);
                        bReservedOk = false;
                        break;
                    }
                }

                if (bReservedOk)
                {
                    v.Pass(T.name_ReservedNameIDs, P.name_P_ReservedNameID, m_tag);
                }
            }

            if (v.PerformTest(T.name_BothPlatforms))
            {
                bool bMac = false;
                bool bMS = false;


                for (uint i=0; i<NumberNameRecords; i++)
                {
                    NameRecord nr = GetNameRecord(i);
                    if (nr != null)
                    {

                        if (nr.PlatformID == 1)
                        {
                            bMac = true;
                        }
                        else if (nr.PlatformID == 3)
                        {
                            bMS = true;
                        }
                    }
                }

                if (bMac && bMS)
                {
                    v.Pass(T.name_BothPlatforms, P.name_P_BothPlatforms, m_tag);
                }
                else if (!bMac)
                {
                    v.Error(T.name_BothPlatforms, E.name_E_NoMacPlatform, m_tag);
					bRet = false;
                }
                else if (!bMS)
                {
                    v.Error(T.name_BothPlatforms, E.name_E_NoMSPlatform, m_tag);
					bRet = false;
                }
            }

            if (v.PerformTest(T.name_VersionString))
            {
                bool bFound = false;
                string sVersion = "";

                for (uint i=0; i<NumberNameRecords; i++)
                {
                    NameRecord nr = GetNameRecord(i);
                    if (nr != null)
                    {
                        if (nr.PlatformID    == 3       // ms
                            && (nr.EncodingID == 1 /* unicode */ || nr.EncodingID == 0 /*symbol*/)
                            && nr.NameID     == 5)      // version string
                        {
                            bFound = true;

                            bool bVerStringValid = false;
                            byte [] buf = GetEncodedString(nr);

							string s = "";

                            if (buf != null)
                            {
                                sVersion = "";
                                for (int j=0; j<buf.Length/2; j++)
                                {
                                    char c = (char)(ushort)(buf[j*2]<<8 | buf[j*2+1]);
                                    sVersion += c;
                                }

                                if (sVersion.Length >= 11
                                    && sVersion.StartsWith("Version ")
                                    && Char.IsDigit(sVersion, 8))
                                {
                                    int j = 9;
                            
                                    // advance past the digits in the major number
                                    while (j < sVersion.Length)
                                    {
                                        if (Char.IsDigit(sVersion, j))
                                        {
                                            j++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    // if major number is followed by a period
                                    if (sVersion[j] == '.')
                                    {
                                        // advance past the period
                                        j++;

                                        // check for a digit
                                        if (Char.IsDigit(sVersion, j))
                                        {
                                            bVerStringValid = true;
                                        }
                                    }
                                }
                            }
                        
							s = "platID = " + nr.PlatformID 
								+ ", encID = " + nr.EncodingID
								+ ", langID = " + nr.LanguageID
								+ ", \"" + sVersion + "\"";

							if (bVerStringValid)
							{
								v.Pass(T.name_VersionString, P.name_P_VersionStringFormat, m_tag, s);
							}
							else
                            {
                                v.Error(T.name_VersionString, E.name_E_VersionStringFormat, m_tag, s);
                                bRet = false;
                            }

                            // compare to mac version string if present
                            string sMacVer = GetString(1, 0, 0xffff, 5);
                            if (sMacVer != null)
                            {
                                if (sVersion.CompareTo(sMacVer) != 0)
                                {
                                    v.Warning(T.name_VersionString, W.name_W_VersionMismatch_MS_MAC, m_tag);
                                }
                            }

							// compare to 3,10 version string if present
							string s310Ver = GetString(3, 10, nr.LanguageID, 5);
							if (s310Ver != null)
							{
								if (sVersion.CompareTo(s310Ver) != 0)
								{
									string s310 = "platID = 3, encID = 10, langID = " + nr.LanguageID
										+ ", \"" + s310Ver + "\"";
									v.Warning(T.name_VersionString, W.name_W_VersionMismatch_3_1_3_10, m_tag, s + " / " + s310);
								}
							}
                        }
                    }
                }

                if (!bFound)
                {
                    v.Error(T.name_VersionString, E.name_E_VersionStringNotFound, m_tag);
                    bRet = false;
                }
            }

            if (v.PerformTest(T.name_PlatformSpecificEncoding))
            {
                bool bIDsOk = true;

                for (uint i=0; i<NumberNameRecords; i++)
                {
                    NameRecord nr = GetNameRecord(i);
                    if (nr != null)
                    {
                        if (nr.PlatformID == 0) // unicode
                        {
                            if (nr.EncodingID > 3)
                            {
                                string s = "platID = " + nr.PlatformID 
                                    + ", encID = " + nr.EncodingID
                                    + ", langID = " + nr.LanguageID
                                    + ", nameID = " + nr.NameID;
                                v.Error(T.name_PlatformSpecificEncoding, E.name_E_PlatformSpecificEncoding, m_tag, s);
                                bIDsOk = false;
                                bRet = false;
                            }
                        }
                        else if (nr.PlatformID == 1) // mac
                        {
                            if (nr.EncodingID > 32)
                            {
                                string s = "platID = " + nr.PlatformID 
                                    + ", encID = " + nr.EncodingID
                                    + ", langID = " + nr.LanguageID
                                    + ", nameID = " + nr.NameID;
                                v.Error(T.name_PlatformSpecificEncoding, E.name_E_PlatformSpecificEncoding, m_tag, s);
                                bIDsOk = false;
                                bRet = false;
                            }
                        }
                        else if (nr.PlatformID == 2) // iso
                        {
                            if (nr.EncodingID > 2)
                            {
                                string s = "platID = " + nr.PlatformID 
                                    + ", encID = " + nr.EncodingID
                                    + ", langID = " + nr.LanguageID
                                    + ", nameID = " + nr.NameID;
                                v.Error(T.name_PlatformSpecificEncoding, E.name_E_PlatformSpecificEncoding, m_tag, s);
                                bIDsOk = false;
                                bRet = false;
                            }
                        }
                        else if (nr.PlatformID == 3) // MS
                        {
                            if (nr.EncodingID > 10)
                            {
                                string s = "platID = " + nr.PlatformID 
                                    + ", encID = " + nr.EncodingID
                                    + ", langID = " + nr.LanguageID
                                    + ", nameID = " + nr.NameID;
                                v.Error(T.name_PlatformSpecificEncoding, E.name_E_PlatformSpecificEncoding, m_tag, s);
                                bIDsOk = false;
                                bRet = false;
                            }
                        }
                        /*
                        else if (nr.PlatformID == 4) // Custom
                        {
                        }
                        */

                    }
                    else
                    {
                        v.Warning(T.name_PlatformSpecificEncoding, W._TEST_W_OtherErrorsInTable, m_tag);
                        bIDsOk = false;
                        break;
                    }
                }

                if (bIDsOk)
                {
                    v.Pass(T.name_PlatformSpecificEncoding, P.name_P_PlatformSpecificEncoding, m_tag);
                }
            }

            if (v.PerformTest(T.name_MSLanguageIDs))
            {
                bool bFound = false;
                bool bIDsOk = true;

                ushort [] MSLangIDs = // taken from Q224804
                    {
                        0x0401, 0x0402, 0x0403, 0x0404, 0x0405, 0x0406, 0x0407, 0x0408, 0x0409,
                        0x040a, 0x040b, 0x040c, 0x040D, 0x040e, 0x040F, 0x0410, 0x0411, 0x0412,
                        0x0413, 0x0414, 0x0415, 0x0416, 0x0417, 0x0418, 0x0419, 0x041a, 0x041b,
                        0x041c, 0x041D, 0x041E, 0x041f, 0x0420, 0x0421, 0x0422, 0x0423, 0x0424,
                        0x0425, 0x0426, 0x0427, 0x0429, 0x042a, 0x042b, 0x042c, 0x042D, 0x042e,
                        0x042f, 0x0430, 0x0431, 0x0432, 0x0433, 0x0434, 0x0435, 0x0436, 0x0437,
                        0x0438, 0x0439, 0x043b, 0x043d, 0x043e, 0x043f, 0x0441, 0x0443, 0x0444,
                        0x0445, 0x0446, 0x0447, 0x0448, 0x0449, 0x044a, 0x044b, 0x044c, 0x044d,
                        0x044e, 0x044f, 0x0457, 0x0459, 0x0461, 0x0801, 0x0804, 0x0807, 0x0809,
                        0x080a, 0x080c, 0x0810, 0x0812, 0x0813, 0x0814, 0x0816, 0x0818, 0x0819,
                        0x081a, 0x081d, 0x0820, 0x0827, 0x082c, 0x083e, 0x0843, 0x0860, 0x0861,
                        0x0C01, 0x0C04, 0x0c07, 0x0c09, 0x0c0a, 0x0c0c, 0x0c1a, 0x1001, 0x1004,
                        0x1007, 0x1009, 0x100a, 0x100c, 0x1401, 0x1404, 0x1407, 0x1409, 0x140a,
                        0x140c, 0x1801, 0x1809, 0x180a, 0x180c, 0x1C01, 0x1c09, 0x1c0a, 0x2001,
                        0x2009, 0x200a, 0x2401, 0x2409, 0x240a, 0x2801, 0x2809, 0x280a, 0x2C01,
                        0x2c09, 0x2c0a, 0x3001, 0x3009, 0x300a, 0x3401, 0x3409, 0x340a, 0x3801,
                        0x380a, 0x3C01, 0x3c0a, 0x4001, 0x400a, 0x440a, 0x480a, 0x4c0a, 0x500a
                    };

                for (uint i=0; i<NumberNameRecords; i++)
                {
                    NameRecord nr = GetNameRecord(i);
                    if (nr != null)
                    {
                        if (nr.PlatformID == 3 && nr.EncodingID == 1)
                        {
                            bFound = true;

                            bool bValidID = false;
                            for (uint j=0; j<MSLangIDs.Length; j++)
                            {
                                if (nr.LanguageID == MSLangIDs[j])
                                {
                                    bValidID = true;
                                    break;
                                }
                            }

                            if (!bValidID)
                            {
                                string s = "platID = " + nr.PlatformID 
                                    + ", encID = " + nr.EncodingID
                                    + ", langID = 0x" + nr.LanguageID.ToString("x4")
                                    + ", nameID = " + nr.NameID;
                                v.Error(T.name_MSLanguageIDs, E.name_E_MSLanguageID, m_tag, s);
                                bIDsOk = false;
                                bRet = false;
                            }
                        }
                    }
                }

                if (bFound && bIDsOk)
                {
                    v.Pass(T.name_MSLanguageIDs, P.name_P_MSLanguageID, m_tag);
                }
            }

            if (v.PerformTest(T.name_unicode_length))
            {
                bool bLengthsOk = true;

                for (uint i=0; i<NumberNameRecords; i++)
                {
                    NameRecord nr = GetNameRecord(i);
                    if (nr != null)
                    {
                        if (nr.PlatformID == 0 || nr.PlatformID == 2 || // unicode or iso platform
                            (nr.PlatformID == 3 && nr.EncodingID == 1)) // microsoft platform, unicode encoding
                        {
                            if ((nr.StringLength & 1) == 1)
                            {
                                string s = "platID = " + nr.PlatformID 
                                    + ", encID = " + nr.EncodingID
                                    + ", langID = " + nr.LanguageID
                                    + ", nameID = " + nr.NameID
                                    + ", length = " + nr.StringLength;
                                v.Error(T.name_unicode_length, E.name_E_UnicodeLength, m_tag, s);
                                bLengthsOk = false;
                                bRet = false;
                            }
                        }
                    }
                    else
                    {
                        v.Warning(T.name_unicode_length, W._TEST_W_OtherErrorsInTable, m_tag);
                        bLengthsOk = false;
                        break;
                    }
                }

                if (bLengthsOk)
                {
                    v.Pass(T.name_unicode_length, P.name_P_UnicodeLength, m_tag);
                }
            }

            if (v.PerformTest(T.name_Postscript))
            {
                bool bPostscriptOk = true;

                string sPostscriptMac = GetString(1, 0, 0, 6);
                if (sPostscriptMac != null)
                {
                    if (sPostscriptMac.Length > 63)
                    {
                        v.Error(T.name_Postscript, E.name_E_Postscript_length, m_tag, "name string (1, 0, 0, 6) is " + sPostscriptMac.Length + " characters long");
                        bRet = false;
                        bPostscriptOk = false;
                    }

                    for (int i=0; i<sPostscriptMac.Length; i++)
                    {
                        char c = sPostscriptMac[i];
                        if (c < 33 || c > 126 || c=='[' || c==']' || c=='(' || c==')' || c=='{' || c=='}' || c=='<' || c=='>' || c=='/' || c=='%')
                        {
                            v.Error(T.name_Postscript, E.name_E_Postscript_chars, m_tag, "name string (1, 0, 0, 6) contains an illegal character at index " + i);
                            bRet = false;
                            bPostscriptOk = false;
                        }
                    }
                }

                ushort nEncoding = 1;
                if (fontOwner.ContainsMsSymbolEncodedCmap())
                {
                    nEncoding = 0;
                }

                string sPostscriptMS = GetString(3, nEncoding, 0x409, 6); // ms
                if (sPostscriptMS != null)
                {
                    if (sPostscriptMS.Length > 63)
                    {
                        v.Error(T.name_Postscript, E.name_E_Postscript_length, m_tag, "name string (3, " + nEncoding + ", 0x409, 6) is " + sPostscriptMS.Length + " characters long");
                        bRet = false;
                        bPostscriptOk = false;
                    }

                    for (int i=0; i<sPostscriptMS.Length; i++)
                    {
                        char c = sPostscriptMS[i];
                        if (c < 33 || c > 126 || c=='[' || c==']' || c=='(' || c==')' || c=='{' || c=='}' || c=='<' || c=='>' || c=='/' || c=='%')
                        {
                            v.Error(T.name_Postscript, E.name_E_Postscript_chars, m_tag, "name string (3, " + nEncoding + ", 0x409, 6) contains an illegal character at index " + i);
                            bRet = false;
                            bPostscriptOk = false;
                        }
                    }
                }

                if (sPostscriptMac==null && sPostscriptMS!=null)
                {
                    v.Error(T.name_Postscript, E.name_E_Postscript_missing, m_tag, "Mac Postscript string is missing, but MS Postscript string is present");
                    bRet = false;
                    bPostscriptOk = false;
                }
                else if (sPostscriptMac!=null && sPostscriptMS==null)
                {
                    v.Error(T.name_Postscript, E.name_E_Postscript_missing, m_tag, "MS Postscript string is missing, but Mac Postscript string is present");
                    bRet = false;
                    bPostscriptOk = false;
                }
                if (sPostscriptMac!=null && sPostscriptMS!=null)
                {
                    if (sPostscriptMac != sPostscriptMS)
                    {
                        v.Error(T.name_Postscript, E.name_E_Postscript_unequal, m_tag, "mac postscript = " + sPostscriptMac + ", MS postscript = " + sPostscriptMS );
                        bRet = false;
                        bPostscriptOk = false;
                    }
                }


                if (sPostscriptMac!=null && sPostscriptMS!=null && bPostscriptOk)
                {
                    v.Pass(T.name_Postscript, P.name_P_Postscript, m_tag);
                }
            }

            if (v.PerformTest(T.name_Subfamily))
            {
                string sStyle = GetStyleString();
                if (sStyle != null)
                {
                    Table_OS2 OS2Table = (Table_OS2)fontOwner.GetTable("OS/2");
                    if (OS2Table != null)
                    {
                        bool bStyleOk = true;
                        string sStyleDetails = "";
                        string s = sStyle.ToLower();

                        bool bItalic = ((OS2Table.fsSelection & 0x01) != 0 );
                        bool bBold   = ((OS2Table.fsSelection & 0x20) != 0 );

                        if (bItalic)                            
                        {
                            if (s.IndexOf("italic") == -1 && s.IndexOf("oblique") == -1)
                            {
                                bStyleOk = false;
                                sStyleDetails = "OS/2.fsSelection italic bit is set, but subfamily string = '" + sStyle + "'";
                            }
                        }
                        else
                        {
                            if (s.IndexOf("italic") != -1 || s.IndexOf("oblique") != -1)
                            {
                                bStyleOk = false;
                                sStyleDetails = "OS/2.fsSelection italic bit is clear, but subfamily string = '" + sStyle + "'";
                            }
                        }

                        if (bBold)                            
                        {
                            if (s.IndexOf("bold") == -1)
                            {
                                bStyleOk = false;
                                sStyleDetails = "OS/2.fsSelection bold bit is set, but subfamily string = '" + sStyle + "'";
                            }
                        }
                        else
                        {
                            if (s.IndexOf("bold") != -1)
                            {
                                bStyleOk = false;
                                sStyleDetails = "OS/2.fsSelection bold bit is clear, but subfamily string = '" + sStyle + "'";
                            }
                        }

                        if (bStyleOk)
                        {
                            v.Pass(T.name_Subfamily, P.name_P_subfamily, m_tag);
                        }
                        else
                        {
                            v.Warning(T.name_Subfamily, W.name_W_subfamily_style, m_tag, sStyleDetails);
                        }
                    }
                }
            }


            if (v.PerformTest(T.name_NoFormat14))
            {
                bool bStringOK = true;
                for (uint i=0; i<NumberNameRecords; i++)
                {
                    NameRecord nr = GetNameRecord(i);
                    if (nr != null &&
                        nr.NameID == 19 &&
                        nr.PlatformID == 0 &&
                        nr.EncodingID == 5 )
                    {
                        string sDetails = 
                            "name string(" + nr.PlatformID 
                            + ", " + nr.EncodingID
                            + ", 0x" + nr.LanguageID.ToString("x4")
                            + ", " + nr.NameID + 
                            ", offset=0x" + 
                            nr.StringOffset.ToString("x4") + ")";
                        v.Error( T.name_NoFormat14, 
                                 E.name_E_NoFormat14,
                                 m_tag,
                                 sDetails );
						bRet = false;
                        bStringOK = false;
                    }
                }
                if ( bStringOK )
                {
                    string sDetails = "PlatformID=0, EncodingID=5 is for " +
                        "Variation Sequences (Format 14)";
                    v.Pass(T.name_NoFormat14, P.name_P_NoFormat14, m_tag, 
                           sDetails );
                }

            }

            if (v.PerformTest(T.name_SampleString))
            {
                Table_cmap cmapTable = (Table_cmap)fontOwner.GetTable("cmap");
                if (cmapTable != null)
                {
                    for (uint i=0; i<NumberNameRecords; i++)
                    {
                        NameRecord nr = GetNameRecord(i);
                        if (nr != null)
                        {
                            if (nr.NameID == 19)
                            {
                                if ( nr.PlatformID == 0 &&
                                     nr.EncodingID == 5 ) 
                                {
                                    // Unicode platform encoding ID 5 can be 
                                    // used for encodings in the 'cmap' table 
                                    // but not for strings in the 'name' table. 
                                    // It has already been flagged as an error,
                                    // so we will just skip it here.
                                    break;
                                }
                                Table_cmap.Subtable CmapSubTable = cmapTable.GetSubtable(nr.PlatformID, nr.EncodingID);
                                if (CmapSubTable != null)
                                {
                                    bool bStringOk = true;

                                    byte[] strbuf = GetEncodedString(nr);
                            
                                    for (uint j=0; j<strbuf.Length;)
                                    {

                                        if (CmapSubTable.MapCharToGlyph(strbuf, j, true) == 0)
                                        {
                                            string sDetails = "name string(" + nr.PlatformID 
                                                + ", " + nr.EncodingID
                                                + ", 0x" + nr.LanguageID.ToString("x4")
                                                + ", " + nr.NameID
                                                + "), character at index " + j + " is not mapped";
                                            v.Error(T.name_SampleString, E.name_E_sample, m_tag, sDetails);
                                            bStringOk = false;
                                            bRet = false;
                                            break;
                                        }

                                        j += CmapSubTable.BytesInChar(strbuf, j);
                                    }

                                    if (bStringOk)
                                    {
                                        string sDetails = "name string(" + nr.PlatformID 
                                            + ", " + nr.EncodingID
                                            + ", 0x" + nr.LanguageID.ToString("x4")
                                            + ", " + nr.NameID
                                            + ")";
                                        v.Pass(T.name_SampleString, P.name_P_sample, m_tag, sDetails);
                                    }
                                }
                            }
                        }
                    }
                }
            }

			if (v.PerformTest(T.name_PreferredFamily))
			{
				bool bFound = false;

				for (uint i=0; i<NumberNameRecords; i++)
				{
					NameRecord nr = GetNameRecord(i);
					if (nr != null)
					{
						if (nr.NameID == 16)
						{
							string sPrefFam = this.GetString(nr.PlatformID, nr.EncodingID, nr.LanguageID, 16);

							string s = "platID = " + nr.PlatformID 
									+ ", encID = " + nr.EncodingID
									+ ", langID = " + nr.LanguageID
									+ ", nameID = " + nr.NameID
									+ ", \"" + sPrefFam + "\"";
							v.Info(T.name_PreferredFamily, I.name_I_Preferred_family_present, m_tag, s);

							bFound = true;
						}
					}
				}

				if (!bFound)
				{
					v.Info(T.name_PreferredFamily, I.name_I_Preferred_family_not_present, m_tag);
				}
			}

			if (v.PerformTest(T.name_PreferredSubfamily))
			{
				bool bFound = false;

				for (uint i=0; i<NumberNameRecords; i++)
				{
					NameRecord nr = GetNameRecord(i);
					if (nr != null)
					{
						if (nr.NameID == 17)
						{
							string sPrefSubfam = this.GetString(nr.PlatformID, nr.EncodingID, nr.LanguageID, 17);

							string s = "platID = " + nr.PlatformID 
								+ ", encID = " + nr.EncodingID
								+ ", langID = " + nr.LanguageID
								+ ", nameID = " + nr.NameID
								+ ", \"" + sPrefSubfam + "\"";
							v.Info(T.name_PreferredSubfamily, I.name_I_Preferred_subfamily_present, m_tag, s);

							bFound = true;
						}
					}
				}

				if (!bFound)
				{
					v.Info(T.name_PreferredSubfamily, I.name_I_Preferred_subfamily_not_present, m_tag);
				}
			}

            if (v.PerformTest(T.name_CopyrightConsistent))
            {
                bool bCopyrightOk = true;

                // get mac roman english Copyright string if present
                string sMac = GetString(1, 0, 0, 0);

                // get windows 3,0 english Copyright string if present
                string sWin3_0 = GetString(3, 0, 1033, 0);

                // get windows 3,1 english Copyright string if present
                string sWin3_1 = GetString(3, 1, 1033, 0);

                // get windows 3,10 english Copyright string if present
                string sWin3_10 = GetString(3, 10, 1033, 0);

                // compare strings

                if (sMac != null)
                {
                    if (sWin3_0 != null)
                    {
                        if (sWin3_0.CompareTo(sMac) != 0)
                        {
                            string sDetails = "(1,0,0,0)='" + sMac + "', (3,0,1033,0)='" + sWin3_0 + "'";
                            v.Warning(T.name_CopyrightConsistent, W.name_W_CopyrightInconsistent, m_tag, sDetails);
                            bCopyrightOk = false;
                        }
                    }

                    if (sWin3_1 != null)
                    {
                        if (sWin3_1.CompareTo(sMac) != 0)
                        {
                            string sDetails = "(1,0,0,0)='" + sMac + "', (3,1,1033,0)='" + sWin3_1 + "'";
                            v.Warning(T.name_CopyrightConsistent, W.name_W_CopyrightInconsistent, m_tag, sDetails);
                            bCopyrightOk = false;
                        }
                    }

                    if (sWin3_10 != null)
                    {
                        if (sWin3_10.CompareTo(sMac) != 0)
                        {
                            string sDetails = "(1,0,0,0)='" + sMac + "', (3,10,1033,0)='" + sWin3_10 + "'";
                            v.Warning(T.name_CopyrightConsistent, W.name_W_CopyrightInconsistent, m_tag, sDetails);
                            bCopyrightOk = false;
                        }
                    }
                }

                if (sWin3_0 != null)
                {
                    if (sWin3_1 != null)
                    {
                        if (sWin3_1.CompareTo(sWin3_0) != 0)
                        {
                            string sDetails = "(3,0,1033,0)='" + sWin3_0 + "', (3,1,1033,0)='" + sWin3_1 + "'";
                            v.Warning(T.name_CopyrightConsistent, W.name_W_CopyrightInconsistent, m_tag, sDetails);
                            bCopyrightOk = false;
                        }
                    }

                    if (sWin3_10 != null)
                    {
                        if (sWin3_10.CompareTo(sWin3_0) != 0)
                        {
                            string sDetails = "(3,0,1033,0)='" + sWin3_0 + "', (3,10,1033,0)='" + sWin3_10 + "'";
                            v.Warning(T.name_CopyrightConsistent, W.name_W_CopyrightInconsistent, m_tag, sDetails);
                            bCopyrightOk = false;
                        }
                    }
                }

                if (sWin3_1 != null)
                {
                    if (sWin3_10 != null)
                    {
                        if (sWin3_10.CompareTo(sWin3_1) != 0)
                        {
                            string sDetails = "(3,1,1033,0)='" + sWin3_1 + "', (3,10,1033,0)='" + sWin3_10 + "'";
                            v.Warning(T.name_CopyrightConsistent, W.name_W_CopyrightInconsistent, m_tag, sDetails);
                            bCopyrightOk = false;
                        }
                    }
                }

                if (bCopyrightOk)
                {
                    v.Pass(T.name_CopyrightConsistent, P.name_P_CopyrightConsistent, m_tag);
                }
                else
                {
                    //bRet = false;
                }


            }

            if (v.PerformTest(T.name_TrademarkConsistent))
            {
                bool bTrademarkOk = true;

                // get mac roman english Trademark string if present
                string sMac = GetString(1, 0, 0, 7);

                // get windows 3,0 english Trademark string if present
                string sWin3_0 = GetString(3, 0, 1033, 7);

                // get windows 3,1 english Trademark string if present
                string sWin3_1 = GetString(3, 1, 1033, 7);

                // get windows 3,10 english Trademark string if present
                string sWin3_10 = GetString(3, 10, 1033, 7);

                // compare strings

                if (sMac != null)
                {
                    if (sWin3_0 != null)
                    {
                        if (sWin3_0.CompareTo(sMac) != 0)
                        {
                            string sDetails = "(1,0,0,7)='" + sMac + "', (3,0,1033,7)='" + sWin3_0 + "'";
                            v.Warning(T.name_TrademarkConsistent, W.name_W_TrademarkInconsistent, m_tag, sDetails);
                            bTrademarkOk = false;
                        }
                    }

                    if (sWin3_1 != null)
                    {
                        if (sWin3_1.CompareTo(sMac) != 0)
                        {
                            string sDetails = "(1,0,0,7)='" + sMac + "', (3,1,1033,7)='" + sWin3_1 + "'";
                            v.Warning(T.name_TrademarkConsistent, W.name_W_TrademarkInconsistent, m_tag, sDetails);
                            bTrademarkOk = false;
                        }
                    }

                    if (sWin3_10 != null)
                    {
                        if (sWin3_10.CompareTo(sMac) != 0)
                        {
                            string sDetails = "(1,0,0,7)='" + sMac + "', (3,10,1033,7)='" + sWin3_10 + "'";
                            v.Warning(T.name_TrademarkConsistent, W.name_W_TrademarkInconsistent, m_tag, sDetails);
                            bTrademarkOk = false;
                        }
                    }
                }

                if (sWin3_0 != null)
                {
                    if (sWin3_1 != null)
                    {
                        if (sWin3_1.CompareTo(sWin3_0) != 0)
                        {
                            string sDetails = "(3,0,1033,7)='" + sWin3_0 + "', (3,1,1033,7)='" + sWin3_1 + "'";
                            v.Warning(T.name_TrademarkConsistent, W.name_W_TrademarkInconsistent, m_tag, sDetails);
                            bTrademarkOk = false;
                        }
                    }

                    if (sWin3_10 != null)
                    {
                        if (sWin3_10.CompareTo(sWin3_0) != 0)
                        {
                            string sDetails = "(3,0,1033,7)='" + sWin3_0 + "', (3,10,1033,7)='" + sWin3_10 + "'";
                            v.Warning(T.name_TrademarkConsistent, W.name_W_TrademarkInconsistent, m_tag, sDetails);
                            bTrademarkOk = false;
                        }
                    }
                }

                if (sWin3_1 != null)
                {
                    if (sWin3_10 != null)
                    {
                        if (sWin3_10.CompareTo(sWin3_1) != 0)
                        {
                            string sDetails = "(3,1,1033,7)='" + sWin3_1 + "', (3,10,1033,7)='" + sWin3_10 + "'";
                            v.Warning(T.name_TrademarkConsistent, W.name_W_TrademarkInconsistent, m_tag, sDetails);
                            bTrademarkOk = false;
                        }
                    }
                }

                if (bTrademarkOk)
                {
                    v.Pass(T.name_TrademarkConsistent, P.name_P_TrademarkConsistent, m_tag);
                }
                else
                {
                    //bRet = false;
                }
            }

            if (v.PerformTest(T.name_DescriptionConsistent))
            {
                bool bDescriptionOk = true;

                // get mac roman english Description string if present
                string sMac = GetString(1, 0, 0, 10);

                // get windows 3,0 english Description string if present
                string sWin3_0 = GetString(3, 0, 1033, 10);

                // get windows 3,1 english Description string if present
                string sWin3_1 = GetString(3, 1, 1033, 10);

                // get windows 3,10 english Description string if present
                string sWin3_10 = GetString(3, 10, 1033, 10);

                // compare strings

                if (sMac != null)
                {
                    if (sWin3_0 != null)
                    {
                        if (sWin3_0.CompareTo(sMac) != 0)
                        {
                            string sDetails = "(1,0,0,10)='" + sMac + "', (3,0,1033,10)='" + sWin3_0 + "'";
                            v.Warning(T.name_DescriptionConsistent, W.name_W_DescriptionInconsistent, m_tag, sDetails);
                            bDescriptionOk = false;
                        }
                    }

                    if (sWin3_1 != null)
                    {
                        if (sWin3_1.CompareTo(sMac) != 0)
                        {
                            string sDetails = "(1,0,0,10)='" + sMac + "', (3,1,1033,10)='" + sWin3_1 + "'";
                            v.Warning(T.name_DescriptionConsistent, W.name_W_DescriptionInconsistent, m_tag, sDetails);
                            bDescriptionOk = false;
                        }
                    }

                    if (sWin3_10 != null)
                    {
                        if (sWin3_10.CompareTo(sMac) != 0)
                        {
                            string sDetails = "(1,0,0,10)='" + sMac + "', (3,10,1033,10)='" + sWin3_10 + "'";
                            v.Warning(T.name_DescriptionConsistent, W.name_W_DescriptionInconsistent, m_tag, sDetails);
                            bDescriptionOk = false;
                        }
                    }
                }

                if (sWin3_0 != null)
                {
                    if (sWin3_1 != null)
                    {
                        if (sWin3_1.CompareTo(sWin3_0) != 0)
                        {
                            string sDetails = "(3,0,1033,10)='" + sWin3_0 + "', (3,1,1033,10)='" + sWin3_1 + "'";
                            v.Warning(T.name_DescriptionConsistent, W.name_W_DescriptionInconsistent, m_tag, sDetails);
                            bDescriptionOk = false;
                        }
                    }

                    if (sWin3_10 != null)
                    {
                        if (sWin3_10.CompareTo(sWin3_0) != 0)
                        {
                            string sDetails = "(3,0,1033,10)='" + sWin3_0 + "', (3,10,1033,10)='" + sWin3_10 + "'";
                            v.Warning(T.name_DescriptionConsistent, W.name_W_DescriptionInconsistent, m_tag, sDetails);
                            bDescriptionOk = false;
                        }
                    }
                }

                if (sWin3_1 != null)
                {
                    if (sWin3_10 != null)
                    {
                        if (sWin3_10.CompareTo(sWin3_1) != 0)
                        {
                            string sDetails = "(3,1,1033,10)='" + sWin3_1 + "', (3,10,1033,10)='" + sWin3_10 + "'";
                            v.Warning(T.name_DescriptionConsistent, W.name_W_DescriptionInconsistent, m_tag, sDetails);
                            bDescriptionOk = false;
                        }
                    }
                }

                if (bDescriptionOk)
                {
                    v.Pass(T.name_DescriptionConsistent, P.name_P_DescriptionConsistent, m_tag);
                }
                else
                {
                    //bRet = false;
                }

            }


            return bRet;
        }


    }
}
