using System;
using System.IO;

namespace OTFontFile.Rasterizer
{
    public class RasterInterf
    {
        public delegate void RastTestErrorDelegate (string sStringName, string sDetails);

        public delegate void UpdateProgressDelegate (string s);

        static public RasterInterf getInstance()
        {
            throw new NotImplementedException("UnImplemented OTFontFile.Rasterizer:getInstance");
        }

        public bool RastTest (int resX, int resY, int[] arrPointSizes,
                             float stretchX, float stretchY,
                             float rotation, float skew,
                             float[,] matrix,
                             bool unknown1, bool unknown2, bool unknown3, uint unknown4,
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
            throw new NotImplementedException("UnImplemented OTFontFile.Rasterizer:CalcDevMetrics");
        }

        public ushort RasterNewSfnt (FileStream fontFileStream, uint faceIndex)
        {
            throw new NotImplementedException("UnImplemented OTFontFile.Rasterizer:RasterNewSfnt");
        }

        public void CancelRastTest ()
        {
            throw new NotImplementedException("UnImplemented OTFontFile.Rasterizer:CancelRastTest");
        }

        public void CancelCalcDevMetrics ()
        {
            throw new NotImplementedException("UnImplemented OTFontFile.Rasterizer:CancelCalcDevMetrics");
        }

        public int GetRastErrorCount ()
        {
            throw new NotImplementedException("UnImplemented OTFontFile.Rasterizer:GetRastErrorCount");
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
            public byte[] yPels;
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
    }
}
