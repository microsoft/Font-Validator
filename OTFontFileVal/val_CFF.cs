// Copyright (c) Hin-Tak Leung

// All rights reserved.

// MIT License

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the ""Software""), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;
using System.Text;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_CFF.
    /// </summary>
    public class val_CFF : Table_CFF, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_CFF(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */

        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            v.Info(T.T_NULL, I.CFF_I_Version, m_tag, major + "." + minor);

            if ( major == 1 && minor == 0 && hdrSize == 4 )
            {
                v.Pass(T.CFF_Header, P.CFF_P_Header, m_tag);
            }
            else
            {
                v.Error(T.CFF_Header, E.CFF_E_Header, m_tag,
                        "major=" + major + ", minor=" + minor + ", hdrSize=" + hdrSize + ", offSize=" + offSize);
            }

            try
            {
                v.Pass(T.CFF_MAININDEX_Enumeration, P.CFF_P_MAININDEX_Enumeration, m_tag,
                       "Name=" + Name.count + ", TopDICT=" + TopDICT.count + ", String=" + String.count + ", GlobalSubr=" + GlobalSubr.count);
            }
            catch (Exception e)
            {
                v.Error(T.CFF_MAININDEX_Enumeration, E.CFF_E_MAININDEX_Enumeration, m_tag,
                        e.Message);

                return false;
            }

            if ( Name.count == TopDICT.count && TopDICT.count == 1)
            {
                v.Pass(T.CFF_NameDICTSize, P.CFF_P_NameDICTSize, m_tag);
            }
            else
            {
                v.Error(T.CFF_NameDICTSize, E.CFF_E_NameDICTSize, m_tag,
                        "Name=" + Name.count + ", TopDICT=" + TopDICT.count);
            }

            if (Name.GetOffset(0) != 1)
                v.Warning( T.CFF_INDEXFirstOffset, W.CFF_W_INDEXFirstOffset, m_tag, "Name=" + Name.GetOffset(0) );
            if (TopDICT.GetOffset(0) != 1)
                v.Warning( T.CFF_INDEXFirstOffset, W.CFF_W_INDEXFirstOffset, m_tag, "TopDICT=" + TopDICT.GetOffset(0) );
            if (String.GetOffset(0) != 1)
                v.Warning( T.CFF_INDEXFirstOffset, W.CFF_W_INDEXFirstOffset, m_tag, "String=" + String.GetOffset(0) );
            if (GlobalSubr.GetOffset(0) != 1)
                v.Warning( T.CFF_INDEXFirstOffset, W.CFF_W_INDEXFirstOffset, m_tag, "GlobalSubr=" + GlobalSubr.GetOffset(0) );

            var overlap = new DataOverlapDetector();

            if ( !overlap.CheckForNoOverlap(0, hdrSize) )
                v.Error(T.CFF_StructOverlap, E.CFF_E_StructOverlap, m_tag, "hdrSize");
            if ( !overlap.CheckForNoOverlap(Name.begin, Name.size) )
                v.Error(T.CFF_StructOverlap, E.CFF_E_StructOverlap, m_tag, "Name");
            if ( !overlap.CheckForNoOverlap(TopDICT.begin, TopDICT.size) )
                v.Error(T.CFF_StructOverlap, E.CFF_E_StructOverlap, m_tag, "TopDICT");
            if ( !overlap.CheckForNoOverlap(String.begin, String.size) )
                v.Error(T.CFF_StructOverlap, E.CFF_E_StructOverlap, m_tag, "String");
            if ( !overlap.CheckForNoOverlap(GlobalSubr.begin, GlobalSubr.size) )
                v.Error(T.CFF_StructOverlap, E.CFF_E_StructOverlap, m_tag, "GlobalSubr");

            try
            {
                for(uint i = 0; i< Name.count; i++)
                {
                    var name = Name.GetString(i);
                }

                v.Pass(T.CFF_Non_ASCII_String_or_Name, P.CFF_P_Non_ASCII_String_or_Name, m_tag, "Name INDEX");
            }
            catch (DecoderFallbackException)
            {
                v.Error(T.CFF_Non_ASCII_String_or_Name, E.CFF_E_Non_ASCII_String_or_Name, m_tag, "Name INDEX");
            }

            try{
                for(uint i = 0; i< String.count; i++)
                {
                    var a = String.GetString(i);
                }

                v.Pass(T.CFF_Non_ASCII_String_or_Name, P.CFF_P_Non_ASCII_String_or_Name, m_tag, "String INDEX");
            }
            catch (DecoderFallbackException)
            {
                v.Error(T.CFF_Non_ASCII_String_or_Name, E.CFF_E_Non_ASCII_String_or_Name, m_tag, "String INDEX");
            }

            for(uint iDICT = 0; iDICT < TopDICT.count; iDICT++)
            {
                Table_CFF.DICTData curTopDICT = null;
                try
                {
                    curTopDICT = GetTopDICT(iDICT);

                    v.Pass(T.CFF_DICTUnwind, P.CFF_P_DICTUnwind, m_tag, curTopDICT.FullName);

                    if ( !overlap.CheckForNoOverlap((uint)curTopDICT.offsetPrivate, (uint)curTopDICT.sizePrivate) )
                        v.Error(T.CFF_StructOverlap, E.CFF_E_StructOverlap, m_tag, "TopDICTPriv");
                }
                catch (Exception e)
                {
                    v.Error(T.CFF_DICTUnwind, E.CFF_E_DICTUnwind, m_tag, "TopDICT: " + e.Message);
                }

                Table_CFF.DICTData topPrivateDict = null;
                try
                {
                    topPrivateDict = GetPrivate(curTopDICT);
                    v.Pass(T.CFF_PrivDICTUnwind, P.CFF_P_PrivDICTUnwind, m_tag, "topPrivateDict");
                }
                catch (Exception e)
                {
                    v.Error(T.CFF_PrivDICTUnwind, E.CFF_E_PrivDICTUnwind, m_tag, "topPrivateDict: " + e.Message);
                }

                if (topPrivateDict != null && topPrivateDict.Subrs != 0)
                {
                    try
                    {
                        Table_CFF.INDEXData topPrivSubrs = GetINDEX(curTopDICT.offsetPrivate + topPrivateDict.Subrs);
                        v.Pass(T.CFF_INDEXCount, P.CFF_P_INDEXCount, m_tag, "topPrivSubrs: " + topPrivSubrs.count);

                        if (topPrivSubrs.GetOffset(0) != 1)
                            v.Warning( T.CFF_INDEXFirstOffset, W.CFF_W_INDEXFirstOffset, m_tag, "topPrivSubrs=" + topPrivSubrs.GetOffset(0) );

                        if ( !overlap.CheckForNoOverlap(topPrivSubrs.begin, topPrivSubrs.size) )
                            v.Error(T.CFF_StructOverlap, E.CFF_E_StructOverlap, m_tag, "topPrivSubrs");
                    }
                    catch (Exception e)
                    {
                        v.Error(T.CFF_INDEXCount, E.CFF_E_INDEXCount, m_tag, "topPrivSubrs: " + e.Message);
                    }
                }

                Table_CFF.INDEXData CharStrings = null;
                try
                {
                    CharStrings = GetINDEX(curTopDICT.offsetCharStrings);
                    v.Pass(T.CFF_INDEXCount, P.CFF_P_INDEXCount, m_tag, "CharStrings: " + CharStrings.count);

                    if (CharStrings.GetOffset(0) != 1)
                        v.Warning( T.CFF_INDEXFirstOffset, W.CFF_W_INDEXFirstOffset, m_tag, "CharStrings=" + CharStrings.GetOffset(0) );

                    if ( !overlap.CheckForNoOverlap(CharStrings.begin, CharStrings.size) )
                        v.Error(T.CFF_StructOverlap, E.CFF_E_StructOverlap, m_tag, "CharStrings");
                }
                catch (Exception e)
                {
                    v.Error(T.CFF_INDEXCount, E.CFF_E_INDEXCount, m_tag, "CharStrings: " + e.Message);
                }

                if ( CharStrings.count == fontOwner.GetMaxpNumGlyphs() )
                {
                    v.Pass(T.CFF_CharStringsCount, P.CFF_P_CharStringsCount, m_tag);
                }
                else
                {
                    v.Error(T.CFF_CharStringsCount, E.CFF_E_CharStringsCount, m_tag,
                            CharStrings.count.ToString() + "!=" + fontOwner.GetMaxpNumGlyphs());
                }

                if (curTopDICT.ROS != null)
                {
                    v.Info(T.CFF_CIDROS, I.CFF_I_CIDROS, m_tag, curTopDICT.ROS);
                }

                if (curTopDICT.offsetFDArray > 0)
                {
                    Table_CFF.INDEXData FDArray = null;
                    try {
                        FDArray = GetINDEX(curTopDICT.offsetFDArray);
                        v.Pass(T.CFF_INDEXCount, P.CFF_P_INDEXCount, m_tag, "FDArray: " + FDArray.count);

                        if (FDArray.GetOffset(0) != 1)
                            v.Warning( T.CFF_INDEXFirstOffset, W.CFF_W_INDEXFirstOffset, m_tag, "FDArray=" + FDArray.GetOffset(0) );

                        if ( !overlap.CheckForNoOverlap(FDArray.begin, FDArray.size) )
                            v.Error(T.CFF_StructOverlap, E.CFF_E_StructOverlap, m_tag, "FDArray");
                    }
                    catch (Exception e)
                    {
                        v.Error(T.CFF_INDEXCount, E.CFF_E_INDEXCount, m_tag, "FDArray: " + e.Message);
                    }

                    for(uint i = 0; i< FDArray.count; i++)
                    {
                        Table_CFF.DICTData FDict = null;
                        try
                        {
                            FDict = GetDICT(FDArray.GetData(i));

                            v.Pass(T.CFF_DICTUnwind, P.CFF_P_DICTUnwind, m_tag, "CID FontDict #" + i + ": " +  FDict.FontName);

                            if ( !overlap.CheckForNoOverlap((uint)FDict.offsetPrivate, (uint)FDict.sizePrivate) )
                                v.Error(T.CFF_StructOverlap, E.CFF_E_StructOverlap, m_tag, "FDictPriv#" + i);
                        }
                        catch (Exception e)
                        {
                            v.Error(T.CFF_DICTUnwind, E.CFF_E_DICTUnwind, m_tag, "CID FontDict #" + i + ": " + e.Message);
                        }

                        Table_CFF.DICTData FDictPrivate = null;
                        try
                        {
                            FDictPrivate = GetPrivate(FDict);
                            v.Pass(T.CFF_PrivDICTUnwind, P.CFF_P_PrivDICTUnwind, m_tag,  "CID FontDict Private DICT#" + i);
                        }
                        catch (Exception e)
                        {
                            v.Error(T.CFF_PrivDICTUnwind, E.CFF_E_PrivDICTUnwind, m_tag, "CID FontDict Private DICT#" + i + ": " + e.Message);
                        }

                        if (FDictPrivate != null && FDictPrivate.Subrs != 0)
                        {
                            Table_CFF.INDEXData PrivSubrs = null;
                            try
                            {
                                PrivSubrs = GetINDEX(FDict.offsetPrivate + FDictPrivate.Subrs);
                                v.Pass(T.CFF_INDEXCount, P.CFF_P_INDEXCount, m_tag, "PrivSubrs#" + i + ": " + PrivSubrs.count);

                                if (PrivSubrs.GetOffset(0) != 1)
                                    v.Warning( T.CFF_INDEXFirstOffset, W.CFF_W_INDEXFirstOffset, m_tag,
                                               "PrivSubrs#" + i + "=" + PrivSubrs.GetOffset(0) );

                                if ( !overlap.CheckForNoOverlap(PrivSubrs.begin, PrivSubrs.size) )
                                    v.Error(T.CFF_StructOverlap, E.CFF_E_StructOverlap, m_tag, "PrivSubrs#" + i);
    ;
                            }
                            catch (Exception e)
                            {
                                v.Error(T.CFF_INDEXCount, E.CFF_E_INDEXCount, m_tag, "PrivSubrs#" + i + ": " + e.Message);
                            }
                        }
                    }
                }
                string result = "Tested Region: " + overlap.Occupied + " of " + GetLength() + ", not testing" +
                    ( curTopDICT.offsetCharset == 0 ? "" : " Charset: " + curTopDICT.offsetCharset) +
                    ( curTopDICT.offsetEncoding == 0 ? "" : " Encoding: " + curTopDICT.offsetEncoding) +
                    ( curTopDICT.offsetFDSelect == 0 ? "" : " FDSelect: " + curTopDICT.offsetFDSelect ) ;
                v.Info(T.CFF_TestCoverage, I.CFF_I_NotValidated, m_tag, result);
            }

            return bRet;
        }
    }
}
