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
using OTFontFile;
using System.IO; //FileStream
using System.Xml;

namespace Compat
{
    public class SVGInfo
    {
        private static int verbose;

        static int Main( string[] args )
        {
            verbose = 0;

            if (args.Length == 0) {
                Console.WriteLine("SVGInfo [-v] [-v] [-v] fontfile");
                return 0;
            }

            OTFile f = new OTFile();
            Table_SVG tSVG = null;
            string filename = null;
            verbose = 0;

            for ( int i = 0; i < args.Length; i++ ) {
                if ( "-v" == args[i] )
                    verbose++;
                else
                    filename = args[i];
            }

            if ( !f.open(filename) )
            {
                    Console.WriteLine("Error: Cannot open {0} as font file", filename);
                    return 0;
            }

            for (uint i = 0; i < f.GetNumFonts() ; i++)
            {
                OTFont fn = f.GetFont(i);
                tSVG = (Table_SVG) fn.GetTable("SVG ");

                Console.WriteLine("{0} SVG table: {1}", filename,
                                  ( tSVG == null ) ? "Absent" : "Present" );
                if (tSVG == null)
                    continue;

                Console.WriteLine("version={0}, reserved={1}, offsetToSVGDocIndex={2}, numEntries={3}",
                                  tSVG.version,
                                  tSVG.reserved,
                                  tSVG.offsetToSVGDocIndex,
                                  tSVG.numEntries);

                Console.WriteLine("start\tend\tOffset\tLength");
                Console.WriteLine("=====\t===\t======\t======");
                for (uint j = 0; j < tSVG.numEntries ; j++)
                {
                    var index = tSVG.GetDocIndexEntry(j);
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}",
                                      index.startGlyphID,
                                      index.endGlyphID,
                                      index.svgDocOffset,
                                      index.svgDocLength);
                }

                if ( verbose > 0 )
                {
                    XmlDocument doc = new XmlDocument();
                    if ( verbose < 2 )
                        doc.XmlResolver = null; //suppress network fetch
                    for (uint j = 0; j < tSVG.numEntries ; j++)
                    {
                        var svgdoc = tSVG.GetDoc(j);

                        Console.WriteLine("=={0}==", j);
                        using(MemoryStream ms = new MemoryStream(svgdoc))
                        {
                            doc.Load(ms);
                        }
                        using(XmlTextWriter writer = new XmlTextWriter(Console.Out))
                        {
                            writer.Formatting = Formatting.Indented;
                            writer.Indentation = 4;
                            doc.Save(writer);
                            writer.Flush();
                        }
                        Console.WriteLine(""); //Newline
                    }
                }
            }
            return 0;
        }
    }
}
