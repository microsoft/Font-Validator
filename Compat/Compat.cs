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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using SharpFont;

namespace OTFontFile.Rasterizer
{
    public class RasterInterf
    {
        private static RasterInterf _Rasterizer;
        private static Library _lib;
        private static Face _face;
        private DevMetricsData m_DevMetricsData;
        private bool m_UserCancelledCalcDevMetrics = false;

        public delegate void RastTestErrorDelegate (string sStringName, string sDetails);

        public delegate void UpdateProgressDelegate (string s);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string path);

        private RasterInterf ()
        {
            PlatformID pid = Environment.OSVersion.Platform;
            if ( pid != PlatformID.Unix && pid != PlatformID.MacOSX )
            {
                string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                path = Path.Combine(path, IntPtr.Size == 8 ? "Win64" : "Win32");
                if (!SetDllDirectory(path))
                    throw new System.ComponentModel.Win32Exception();
            }
            _lib = new Library();
            Console.WriteLine("FreeType version: " + _lib.Version);
        }

        static public RasterInterf getInstance()
        {
            if (_Rasterizer == null)
            {
                _Rasterizer = new RasterInterf ();
            }
            if ( _face != null )
            {
                _face.Dispose();
                _face = null;
            }

            return _Rasterizer;
        }

        public bool RastTest (int resX, int resY, int[] arrPointSizes,
                             float stretchX, float stretchY,
                             float rotation, float skew,
                             float[,] matrix,
                             bool setBW, bool setGrayscale, bool setCleartype, uint CTFlags,
                             RastTestErrorDelegate pRastTestErrorDelegate,
                             UpdateProgressDelegate pUpdateProgressDelegate,
                             int numGlyphs)
        {
            throw new NotImplementedException("UnImplemented OTFontFile.Rasterizer:RastTest");
        }

        public DevMetricsData CalcDevMetrics (int Huge_calcHDMX, int Huge_calcLTSH, int Huge_calcVDMX,
                                              ushort numGlyphs,
                                              byte[] phdmxPointSizes, ushort maxHdmxPointSize,
                                              byte uchPixelHeightRangeStart, byte uchPixelHeightRangeEnd,
                                              ushort[] pVDMXxResolution, ushort[] pVDMXyResolution,
                                              ushort cVDMXResolutions, UpdateProgressDelegate pUpdateProgressDelegate)
        {
            if ( Huge_calcHDMX == 0 && Huge_calcLTSH == 0 && Huge_calcVDMX == 0 )
                return null;

            this.m_DevMetricsData = new DevMetricsData();

            if ( Huge_calcHDMX != 0 )
            {
                List<uint> requestedPixelSize = new List<uint>();
                for( ushort i = 0; i <= maxHdmxPointSize ; i++ ) {
                    if ( phdmxPointSizes[i] == 1 ) {
                        requestedPixelSize.Add((uint)i);
                    }
                }

                this.m_DevMetricsData.hdmxData = new HDMX();
                this.m_DevMetricsData.hdmxData.Records = new HDMX_DeviceRecord[requestedPixelSize.Count];

                for (int i = 0; i < requestedPixelSize.Count; i++) {
                    if ( m_UserCancelledCalcDevMetrics ) return null;
                    trySetPixelSizes(0, requestedPixelSize[i]);
                    this.m_DevMetricsData.hdmxData.Records[i] = new HDMX_DeviceRecord();
                    this.m_DevMetricsData.hdmxData.Records[i].Widths = new byte[_face.GlyphCount];
                    for (uint glyphIndex = 0; glyphIndex < _face.GlyphCount; glyphIndex++)
                    {
                        _face.LoadGlyph(glyphIndex, LoadFlags.Default|LoadFlags.ComputeMetrics, LoadTarget.Normal);
                        this.m_DevMetricsData.hdmxData.Records[i].Widths[glyphIndex] =  (byte) _face.Glyph.Advance.X.Round();
                    }
                }
            }

            if ( Huge_calcLTSH != 0 )
            {
                this.m_DevMetricsData.ltshData = new LTSH();
                this.m_DevMetricsData.ltshData.yPels = new byte[numGlyphs];

                for (uint i = 0; i < this.m_DevMetricsData.ltshData.yPels.Length; i++) {
                    this.m_DevMetricsData.ltshData.yPels[i] = 1;
                }
                int remaining = numGlyphs;
                for (uint j = 254; j > 0; j--) {
                    if ( remaining == 0 )
                        break;
                    if ( m_UserCancelledCalcDevMetrics ) return null;
                    trySetPixelSizes(0, j);
                    for (uint i = 0; i < this.m_DevMetricsData.ltshData.yPels.Length; i++) {
                        if ( this.m_DevMetricsData.ltshData.yPels[i] > 1 )
                            continue;
                        _face.LoadGlyph(i, LoadFlags.Default|LoadFlags.ComputeMetrics, LoadTarget.Normal);
                        int Advance_X = _face.Glyph.Advance.X.Round() ;
                        int LinearHorizontalAdvance = _face.Glyph.LinearHorizontalAdvance.Round() ;
                        if ( Advance_X == LinearHorizontalAdvance )
                            continue;
                        int difference = Advance_X - LinearHorizontalAdvance ;
                        if ( difference < 0 )
                            difference = - difference;
                        if ( ( j >= 50 ) && (difference * 50 <= LinearHorizontalAdvance) ) // compat "<="
                            continue;
                        // this is off-spec but happens to agree better...
                        difference = (_face.Glyph.Advance.X.Value << 10) - _face.Glyph.LinearHorizontalAdvance.Value;
                        if ( difference < 0 )
                            difference = - difference;
                        if ( ( j >= 50 ) && (difference * 50 <= _face.Glyph.LinearHorizontalAdvance.Value) ) // compat "<=="
                            continue;
                        // off-spec-ness ends.
                        this.m_DevMetricsData.ltshData.yPels[i] = (byte) ( j + 1 );
                        remaining--;
                    }
                }
            }

            if ( Huge_calcVDMX != 0 )
            {
                this.m_DevMetricsData.vdmxData = new VDMX();
                this.m_DevMetricsData.vdmxData.groups = new VDMX_Group[cVDMXResolutions];
                for ( int i = 0 ; i < cVDMXResolutions ; i++ )
                {
                    this.m_DevMetricsData.vdmxData.groups[i] = new VDMX_Group();
                    this.m_DevMetricsData.vdmxData.groups[i].entry = new VDMX_Group_vTable[uchPixelHeightRangeEnd
                                                                                           - uchPixelHeightRangeStart + 1];
                    for ( ushort j = uchPixelHeightRangeStart ; j <= uchPixelHeightRangeEnd ; j++ )
                    {
                        int k = j - uchPixelHeightRangeStart;
                        this.m_DevMetricsData.vdmxData.groups[i].entry[k] = new VDMX_Group_vTable() ;
                        this.m_DevMetricsData.vdmxData.groups[i].entry[k].yPelHeight = j ;

                        uint x_pixelSize = (uint) ( (pVDMXyResolution[i] == 0) ?
                                                    0 : (pVDMXxResolution[i] * j + pVDMXyResolution[i]/2 ) / pVDMXyResolution[i] );
                        if ( m_UserCancelledCalcDevMetrics ) return null;
                        trySetPixelSizes(x_pixelSize, j);
                        short yMax = 0;
                        short yMin = 0;
                        BBox box;
                        Glyph glyph;
                        for (uint ig = 0; ig < numGlyphs; ig++) {
                            _face.LoadGlyph(ig, LoadFlags.Default|LoadFlags.ComputeMetrics, LoadTarget.Normal);
                            glyph = _face.Glyph.GetGlyph();
                            box = glyph.GetCBox(GlyphBBoxMode.Truncate);
                            if (box.Top > yMax) yMax = (short) box.Top;
                            if (box.Bottom < yMin) yMin = (short) box.Bottom;
                            glyph.Dispose();
                        }
                        this.m_DevMetricsData.vdmxData.groups[i].entry[k].yMax = yMax ;
                        this.m_DevMetricsData.vdmxData.groups[i].entry[k].yMin = yMin ;
                    }
                }
            }

            return m_DevMetricsData;
        }

        public ushort RasterNewSfnt (FileStream fontFileStream, uint faceIndex)
        {
            _face = _lib.NewFace(fontFileStream.Name, (int)faceIndex);
            m_UserCancelledCalcDevMetrics = false;

            return 1; //Not used by caller
        }

        public void CancelRastTest ()
        {
        }

        public void CancelCalcDevMetrics ()
        {
            m_UserCancelledCalcDevMetrics = true;
        }

        public int GetRastErrorCount ()
        {
            return 0;
        }

        public class DevMetricsData
        {
            public HDMX hdmxData;
            public LTSH ltshData;
            public VDMX vdmxData;
        }

        // These structures largely have their OTSPEC meanings,
        // except there is no need to store array lengths seperately
        // as .NET arrays know their own lengths.

        public class HDMX
        {
            public HDMX_DeviceRecord[] Records;
        }

        public class HDMX_DeviceRecord
        {
            public byte[] Widths;
        }

        public class LTSH
        {
            public byte[] yPels; // byte[numGlyphs] 1 default
        }

        public class VDMX
        {
            public VDMX_Group[] groups;
        }

        public class VDMX_Group
        {
            public VDMX_Group_vTable[] entry;
        }

        public class VDMX_Group_vTable
        {
            public ushort yPelHeight;
            public short yMax;
            public short yMin;
        }

        private void trySetPixelSizes(uint x_requestedPixelSize, uint y_requestPixelSize)
        {
            try{
                _face.SetPixelSizes(x_requestedPixelSize, y_requestPixelSize);
            } catch (FreeTypeException e) {
                Console.WriteLine("SetPixelSizes caught " + e + " at size " + x_requestedPixelSize + ", " + y_requestPixelSize);
                if (e.Error == Error.InvalidPixelSize)
                    throw new ArgumentException("FreeType invalid pixel size error: Likely setting unsupported size "
                                                + x_requestedPixelSize + ", " + y_requestPixelSize + " for fixed-size font.");
                else
                    throw;
            }
        }
    }
}
