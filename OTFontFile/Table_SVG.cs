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

using System.IO;
using System.IO.Compression;

namespace OTFontFile
{
    public class Table_SVG : OTTable
    {
        public Table_SVG(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        public enum FieldOffsets
        {
            version               = 0, // USHORT
            offsetToSVGDocIndex   = 2, // ULONG
            reserved              = 6  // ULONG
        }

        public enum DocHeaderType
        {
            unknown,
            plain,
            UTF8,
            UTF16BE,
            UTF16LE,
            UTF32BE,
            UTF32LE,
            gzipped,
        }

        public class SVGDocumentIndexEntry
        {
            public SVGDocumentIndexEntry(uint offset, MBOBuffer bufTable)
            {
                m_offsetIndex = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                startGlyphID = 0, // USHORT
                endGlyphID   = 2, // USHORT
                svgDocOffset = 4, // ULONG
                svgDocLength = 8  // ULONG
            }

            // accessors
            public ushort startGlyphID
            {
                get { return m_bufTable.GetUshort(m_offsetIndex + (uint)FieldOffsets.startGlyphID); }
            }

            public ushort endGlyphID
            {
                get { return m_bufTable.GetUshort(m_offsetIndex + (uint)FieldOffsets.endGlyphID); }
            }

            public uint svgDocOffset
            {
                get { return m_bufTable.GetUint(m_offsetIndex + (uint)FieldOffsets.svgDocOffset); }
            }

            public uint svgDocLength
            {
                get { return m_bufTable.GetUint(m_offsetIndex + (uint)FieldOffsets.svgDocLength); }
            }

            uint m_offsetIndex;
            MBOBuffer m_bufTable;
        }

        public SVGDocumentIndexEntry GetDocIndexEntry(uint i)
        {
            SVGDocumentIndexEntry entry = null;

            if ( i < numEntries )
            {
                uint offset = this.offsetToSVGDocIndex
                    + 2
                    + 12 * i ;
                entry = new SVGDocumentIndexEntry( offset, m_bufTable );
            }
            return entry;
        }

        public byte[] GetDoc(uint i)
        {
            return GetDoc(i, true);
        }

        public byte[] GetDoc(uint i, bool autodecompress)
        {
            SVGDocumentIndexEntry entry = this.GetDocIndexEntry(i);
            uint length = entry.svgDocLength;
            byte [] buf = new byte[length];
            uint offset = this.offsetToSVGDocIndex + entry.svgDocOffset;
            System.Buffer.BlockCopy(m_bufTable.GetBuffer(), (int)offset, buf, 0, (int)length);

            if ( autodecompress && buf[0] == 0x1F && buf[1] == 0x8B )
            {
                byte[] decompressed = null;
                using (MemoryStream output = new MemoryStream())
                {
                    using (MemoryStream input = new MemoryStream(buf))
                    {
                        using (GZipStream zip = new GZipStream(input, CompressionMode.Decompress))
                        {
                            byte[] buffer = new byte[16 * 1024];
                            int byteRead;
                            while ( (byteRead = zip.Read( buffer, 0, buffer.Length )) > 0 )
                            {
                                output.Write( buffer, 0, byteRead );
                            }
                        }
                    }
                    decompressed = output.ToArray();
                }
                return decompressed;
            }
            else
                return buf;
        }

        public static DocHeaderType DetectType(byte[] buf)
        {
            if ( buf[0] == 0x1F && buf[1] == 0x8B )
                return DocHeaderType.gzipped;

            if ( buf[0] == 0xEF && buf[1] == 0xBB && buf[2] == 0xBF )
                return DocHeaderType.UTF8;
            if ( buf[0] == 0xFE && buf[1] == 0xFF )
                return DocHeaderType.UTF16BE;
            if ( buf[0] == 0xFF && buf[1] == 0xFE )
                return DocHeaderType.UTF16LE;
            if ( buf[0] == 0x00 && buf[1] == 0x00 && buf[2] == 0xFE && buf[3] == 0xFF )
                return DocHeaderType.UTF32BE;
            if ( buf[0] == 0xFF && buf[1] == 0xFE && buf[2] == 0x00 && buf[3] == 0x00 )
                return DocHeaderType.UTF32BE;

            if ( buf[0] == 0x3C && buf[1] == 0x3F && buf[2] == 0x78 && buf[3] == 0x6D )
                return DocHeaderType.plain;

            return DocHeaderType.unknown;
        }
        public DocHeaderType GetDocType(uint i)
        {
            SVGDocumentIndexEntry entry = this.GetDocIndexEntry(i);
            byte [] buf = new byte[4];
            uint offset = this.offsetToSVGDocIndex /* should be 10 */ + entry.svgDocOffset;
            System.Buffer.BlockCopy(m_bufTable.GetBuffer(), (int)offset, buf, 0, 4);
            return DetectType(buf);
        }

        // accessors
        public ushort version
        {
            get { return m_bufTable.GetUshort((uint)FieldOffsets.version); }
        }

        public uint offsetToSVGDocIndex
        {
            get { return m_bufTable.GetUint((uint)FieldOffsets.offsetToSVGDocIndex); }
        }

        public uint reserved
        {
            get { return m_bufTable.GetUint((uint)FieldOffsets.reserved); }
        }

        public ushort numEntries
        {
            get { return m_bufTable.GetUshort(this.offsetToSVGDocIndex); }
        }
    }
}
