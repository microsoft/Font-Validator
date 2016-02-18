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
using OTFontFileVal;

namespace Compat
{
    public class CFFInfo
    {
        private static int verbose;

        static int Main( string[] args )
        {
            verbose = 0;

            if (args.Length == 0) {
                Console.WriteLine("CFFInfo [-v] [-v] [-v] fontfile");
                return 0;
            }

            OTFile f = new OTFile();
            Table_CFF tCFF = null;
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

            if ( f.GetNumFonts() != 1 )
                Console.WriteLine("{0} contains {1} member fonts", filename, f.GetNumFonts() );
            for (uint iFont = 0; iFont < f.GetNumFonts() ; iFont++)
            {
                OTFont fn = f.GetFont(iFont);
                tCFF = (Table_CFF) fn.GetTable("CFF ");

                Console.WriteLine("{0} CFF table: {1}", filename,
                                  ( tCFF == null ) ? "Absent" : "Present" );
                if (tCFF == null)
                    continue;

                /* header */

                /* INDEX's */
                Console.WriteLine("Name:\tcount={0}\tsize={1}",
                                  tCFF.Name.count, tCFF.Name.size);
                Console.WriteLine("TopDICT:\tcount={0}\tsize={1}",
                                  tCFF.TopDICT.count, tCFF.TopDICT.size);
                Console.WriteLine("String:\tcount={0}\tsize={1}",
                                  tCFF.String.count, tCFF.String.size);
                Console.WriteLine("GlobalSubr:\tcount={0}\tsize={1}",
                                  tCFF.GlobalSubr.count, tCFF.GlobalSubr.size);
                /* INDEX success */

                var overlap = new DataOverlapDetector();
                overlap.CheckForNoOverlap(0, tCFF.hdrSize);
                overlap.CheckForNoOverlap(tCFF.Name.begin, tCFF.Name.size);
                overlap.CheckForNoOverlap(tCFF.TopDICT.begin, tCFF.TopDICT.size);
                overlap.CheckForNoOverlap(tCFF.String.begin, tCFF.String.size);
                overlap.CheckForNoOverlap(tCFF.GlobalSubr.begin, tCFF.GlobalSubr.size);

                if ( verbose > 1 )
                {
                    Console.WriteLine("Region-hdr        :\t{0}\t{1}", 0, tCFF.hdrSize);
                    Console.WriteLine("Region-Name       :\t{0}\t{1}", tCFF.Name.begin, tCFF.Name.size);
                    Console.WriteLine("Region-TopDICT    :\t{0}\t{1}", tCFF.TopDICT.begin, tCFF.TopDICT.size);
                    Console.WriteLine("Region-String     :\t{0}\t{1}", tCFF.String.begin, tCFF.String.size);
                    Console.WriteLine("Region-GlobalSubr :\t{0}\t{1}", tCFF.GlobalSubr.begin, tCFF.GlobalSubr.size);
                }
                for(uint i = 0; i< tCFF.Name.count; i++)
                    Console.WriteLine("Name: " + tCFF.Name.GetString(i));

                try{
                    for(uint i = 0; i< tCFF.String.count; i++)
                    {
                        var a = tCFF.String.GetString(i);
                        if ( verbose > 0 )
                            Console.WriteLine("String #{0}: {1}", i, a);
                    }
                }
                catch (DecoderFallbackException)
                {
                    for(uint i = 0; i< tCFF.String.count; i++)
                    {
                        var a = tCFF.String.GetUTFString(i);
                        if ( verbose > 0 )
                            Console.WriteLine("String #{0}: {1}", i, a);
                    }
                }

                for(uint iDICT = 0; iDICT < tCFF.TopDICT.count; iDICT++)
                {
                    var curTopDICT = tCFF.GetTopDICT(iDICT);

                    Console.WriteLine("FullName in TopDICT: " + curTopDICT.FullName);

                    overlap.CheckForNoOverlap((uint)curTopDICT.offsetPrivate, (uint)curTopDICT.sizePrivate);

                    if ( verbose > 1 )
                        Console.WriteLine("Region-Private    :\t{0}\t{1}", curTopDICT.offsetPrivate, curTopDICT.sizePrivate);

                    Console.WriteLine("Offset to Charset:\t"  + curTopDICT.offsetCharset);
                    Console.WriteLine("Offset to Encoding:\t" + curTopDICT.offsetEncoding);

                    Table_CFF.DICTData topPrivateDict = null;
                    try
                    {
                        topPrivateDict = tCFF.GetPrivate(curTopDICT);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine("Broken Private Dict:"  + e.Message);
                    }

                    if (topPrivateDict != null && topPrivateDict.Subrs != 0)
                    {
                        var topPrivSubrs = tCFF.GetINDEX(curTopDICT.offsetPrivate + topPrivateDict.Subrs);

                        overlap.CheckForNoOverlap(topPrivSubrs.begin, topPrivSubrs.size);

                        if ( verbose > 1 )
                            Console.WriteLine("Region-PrivSubrs  :\t{0}\t{1}", topPrivSubrs.begin, topPrivSubrs.size);
                    }

                    var CharStrings = tCFF.GetINDEX(curTopDICT.offsetCharStrings);
                    Console.WriteLine("CharStrings count: " + CharStrings.count);

                    overlap.CheckForNoOverlap(CharStrings.begin, CharStrings.size);

                    if ( verbose > 1 )
                        Console.WriteLine("Region-CharStrings:\t{0}\t{1}", CharStrings.begin, CharStrings.size);

                    if (curTopDICT.ROS != null)
                    {
                        Console.WriteLine("CID ROS: " + curTopDICT.ROS);
                        Console.WriteLine("Offset to CID FDSelect:\t" + curTopDICT.offsetFDSelect);
                    }

                    if (curTopDICT.offsetFDArray > 0)
                    {
                        var FDArray = tCFF.GetINDEX(curTopDICT.offsetFDArray);

                        overlap.CheckForNoOverlap(FDArray.begin, FDArray.size);

                        if ( verbose > 1 )
                            Console.WriteLine("Region-FDArray    :\t{0}\t{1}", FDArray.begin, FDArray.size);

                        for(uint i = 0; i< FDArray.count; i++)
                        {
                            var FDict = tCFF.GetDICT(FDArray.GetData(i));
                            Console.WriteLine("CID FontDict #{0}: {1}", i, FDict.FontName);

                            overlap.CheckForNoOverlap((uint)FDict.offsetPrivate, (uint)FDict.sizePrivate);

                            if ( verbose > 1 )
                                Console.WriteLine("Region-CID FontDict #{2}    :\t{0}\t{1}", FDict.offsetPrivate, FDict.sizePrivate, i);

                            Table_CFF.DICTData FDictPrivate = null;
                            try
                            {
                                FDictPrivate = tCFF.GetPrivate(FDict);
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Console.WriteLine("Broken CID FD Private Dict #{0}:{1}", i, e.Message);
                            }

                            if (FDictPrivate != null && FDictPrivate.Subrs != 0)
                            {
                                var PrivSubrs = tCFF.GetINDEX(FDict.offsetPrivate + FDictPrivate.Subrs);

                                overlap.CheckForNoOverlap(PrivSubrs.begin, PrivSubrs.size);
                                if ( verbose > 1 )
                                    Console.WriteLine("Region-CID PrivSubrs #{2}    :\t{0}\t{1}", PrivSubrs.begin, PrivSubrs.size, i);
                            }
                        }
                    }
                }
                Console.WriteLine("Tested region: {0} of {1}",
                                  overlap.Occupied, tCFF.GetLength());

                if ( overlap.ends != tCFF.GetLength() )
                {
                }
            }
            return 0;
        }
    }
}
