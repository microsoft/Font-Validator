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
using System.IO;
using System.Xml;
using OTFontFile;

namespace OTFontFileVal
{
    public class val_SVG : Table_SVG, ITableValidate
    {
        public val_SVG(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;
            bool bContinue = true;

            if (v.PerformTest(T.SVG_version))
            {
                if ( version == 0 )
                    v.Pass(T.SVG_version, P.SVG_P_version, m_tag);
                else
                    v.Warning(T.SVG_version, W.SVG_W_version, m_tag,
                              "Version=" + version);
            }

            if (v.PerformTest(T.SVG_offsetToSVGDocIndex))
            {
                uint offsetToSVGDocIndex_version0 = 10;
                if ( offsetToSVGDocIndex == offsetToSVGDocIndex_version0 )
                {
                    v.Pass(T.SVG_offsetToSVGDocIndex, P.SVG_P_offsetToSVGDocIndex, m_tag);
                }
                else
                {
                    if ( offsetToSVGDocIndex < offsetToSVGDocIndex_version0 )
                    {
                         v.Error(T.SVG_offsetToSVGDocIndex, E.SVG_E_offsetToSVGDocIndex_Overlapping, m_tag,
                                 "offset=" + offsetToSVGDocIndex);
                         bContinue = false;
                         bRet = false;
                    }
                    else
                    {
                        v.Warning(T.SVG_offsetToSVGDocIndex, W.SVG_W_offsetToSVGDocIndex_NonContiguous, m_tag,
                                  "offset=" + offsetToSVGDocIndex);
                    }
                }
            }

            if (v.PerformTest(T.SVG_reserved))
            {
                if ( reserved == 0 )
                    v.Pass(T.SVG_reserved, P.SVG_P_reserved, m_tag);
                else
                {
                    v.Warning(T.SVG_reserved, W.SVG_W_reserved, m_tag,
                              "Reserved=" + reserved);
                }
            }

            if (v.PerformTest(T.SVG_numEntries))
            {
                if ( numEntries != 0 )
                    v.Pass(T.SVG_numEntries, P.SVG_P_numEntries, m_tag);
                else
                {
                    v.Warning(T.SVG_numEntries, W.SVG_W_numEntries, m_tag);
                    bContinue = false;
                }
            }

            if (bContinue && v.PerformTest(T.SVG_SVGDocIndex))
            {
                bool statusOK = true;

                int prev_startGlyphID = GetDocIndexEntry(0).startGlyphID - 1;
                int prev_endGlyphID = GetDocIndexEntry(0).startGlyphID - 1;
                uint prev_end = 2
                    + (uint) numEntries * 12;
                for (uint j = 0; j < numEntries ; j++)
                {
                    var docEntry = GetDocIndexEntry(j);

                    if (docEntry.endGlyphID < docEntry.startGlyphID)
                    {
                        v.Error(T.SVG_SVGDocIndex, E.SVG_E_SVGDocIndex, m_tag,
                                "Entry " + j + ": endGlyphID < startGlyphID");
                        statusOK = false;
                    }

                    if (docEntry.svgDocOffset == 0)
                    {
                        v.Error(T.SVG_SVGDocIndex, E.SVG_E_SVGDocIndex, m_tag,
                                "Entry " + j + ": svgDocOffset is zero");
                        statusOK = false;
                    }

                    if (docEntry.svgDocLength == 0)
                    {
                        v.Error(T.SVG_SVGDocIndex, E.SVG_E_SVGDocIndex, m_tag,
                                "Entry " + j + ": svgDocLength is zero");
                        statusOK = false;
                    }

                    if (docEntry.startGlyphID  <= prev_startGlyphID)
                    {
                        v.Error(T.SVG_SVGDocIndex, E.SVG_E_SVGDocIndex, m_tag,
                                "Entry " + j + ": startGlyphID not in increasing order");
                        statusOK = false;
                    }

                    if (docEntry.startGlyphID  <= prev_endGlyphID)
                    {
                        v.Error(T.SVG_SVGDocIndex, E.SVG_E_SVGDocIndex, m_tag,
                                "Entry " + j + ": startGlyphID overlaps previous endGlyphID");
                        statusOK = false;
                    }

                    if ( docEntry.svgDocOffset < prev_end )
                    {
                        v.Error(T.SVG_SVGDocIndex, E.SVG_E_SVGDocIndex, m_tag,
                                "Entry " + j + ": overlaps previous entry" + docEntry.svgDocOffset + "<" + prev_end);
                        statusOK = false;
                        bContinue = false;
                    }
                    else
                        if ( docEntry.svgDocOffset >  prev_end )
                        {
                            v.Warning(T.SVG_SVGDocIndex, W.SVG_W_SVGDocIndex, m_tag,
                                      "Entry " + j + ": non-contiguous from previous entry");
                            statusOK = false;
                        }

                    prev_startGlyphID = docEntry.startGlyphID;
                    prev_endGlyphID = docEntry.endGlyphID;
                    prev_end = docEntry.svgDocOffset + docEntry.svgDocLength;
                }

                if (statusOK)
                    v.Pass(T.SVG_SVGDocIndex, P.SVG_P_SVGDocIndex, m_tag);
            }

            if (bContinue && v.PerformTest(T.SVG_TryLoadSVG))
            {
                bool statusOK = true;

                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;

                for (uint j = 0; j < numEntries ; j++)
                {
                    var svgdoc = GetDoc(j);

                    using(MemoryStream ms = new MemoryStream(svgdoc))
                    {
                        try
                        {
                            doc.Load(ms);
                        }
                        catch (Exception e)
                        {
                            v.Error(T.SVG_TryLoadSVG, E.SVG_E_TryLoadSVG, m_tag,
                                    "document " + j + " Error:" + e.Message);
                            statusOK = false;
                        }
                    }
                }

                if (statusOK)
                    v.Pass(T.SVG_TryLoadSVG, P.SVG_P_TryLoadSVG, m_tag);
            }

            return bRet;
        }
    }
}
