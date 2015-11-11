using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;


namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_cmap.
    /// </summary>
    public class Table_cmap : OTTable
    {
        /************************
         * constructors
         */
        
        
        /// <summary>Just pass data to base class</summary>
        public Table_cmap(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            TableVersionNumber     = 0,
            NumberOfEncodingTables = 2,
            EncodingTableEntries   = 4
        }



        /************************
         * property accessors
         */


        /// <summary>Get ushort at 0 bytes offset</summary>
        public ushort TableVersionNumber
        {
            get {
                return m_bufTable.GetUshort((uint)
                                            FieldOffsets.TableVersionNumber);
            }
        }

        /// <summary>Get ushort at 2 bytes offset</summary>
        public ushort NumberOfEncodingTables
        {
            get {
                return 
                    m_bufTable.GetUshort((uint)
                                         FieldOffsets.NumberOfEncodingTables);
            }
        }


        /// <summary>Get <c>ith</c> encoding table, or null if <c>i</c> out
        /// of range.</summary>
        public EncodingTableEntry GetEncodingTableEntry(uint i)
        {
            uint SIZEOF_ENCODINGTABLEENTRY = 8;

            EncodingTableEntry ete = null;
            if ( i < NumberOfEncodingTables && 
                 (uint)FieldOffsets.EncodingTableEntries + 
                 (i+1)*SIZEOF_ENCODINGTABLEENTRY <= m_bufTable.GetLength())
            {
                ete = new EncodingTableEntry();
                ete.platformID = 
                    m_bufTable.GetUshort((uint)
                                         FieldOffsets.EncodingTableEntries + 
                                         i*SIZEOF_ENCODINGTABLEENTRY);
                ete.encodingID = 
                    m_bufTable.GetUshort((uint)
                                         FieldOffsets.EncodingTableEntries + 
                                         i*SIZEOF_ENCODINGTABLEENTRY + 2);
                ete.offset = 
                    m_bufTable.GetUint((uint)
                                       FieldOffsets.EncodingTableEntries + 
                                       i*SIZEOF_ENCODINGTABLEENTRY + 4);
            }

            return ete;
        }

        /// <summary>Get first encoding table matching
        /// <c>platformID</c> and <c>encodingID</c>, or null if no
        /// such encoding table.
        /// </summary>
        public EncodingTableEntry GetEncodingTableEntry( ushort platformID,
                                                         ushort encodingID )
        {
            EncodingTableEntry ete = null;

            for (uint i=0; i<NumberOfEncodingTables; i++)
            {
                EncodingTableEntry eteTemp = GetEncodingTableEntry(i);
                if (eteTemp != null)
                {
                    if (eteTemp.platformID == platformID && 
                        eteTemp.encodingID == encodingID)
                    {
                        ete = eteTemp;
                        break;
                    }
                }
            }

            return ete;
        }

        /// <summary>Get new subtable in this format by 
        /// <c>EncodingTableEntry</c>.
        /// This method is virtual so that
        /// the corresponding validator class can override this and
        /// return an object of its subclass of the subtable.
        /// </summary>
        virtual public Subtable GetSubtable(EncodingTableEntry ete)
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
                case 0:  st = new Format0 (ete, m_bufTable); break;
                case 2:  st = new Format2 (ete, m_bufTable); break;
                case 4:  st = new Format4 (ete, m_bufTable); break;
                case 6:  st = new Format6 (ete, m_bufTable); break;
                case 8:  st = new Format8 (ete, m_bufTable); break;
                case 10: st = new Format10(ete, m_bufTable); break;
                case 12: st = new Format12(ete, m_bufTable); break;
                case 14: st = new Format14(ete, m_bufTable); break;
            }

            return st;
        }
        
        /// <summary>GetSubtable by <c>platformID</c> and 
        /// <c>encodingID</c>, but calls the virtual overloaded function.
        /// </summary>
        public Subtable GetSubtable(ushort platformID, ushort encodingID)
        {
            Subtable st = null;

            try
            {
                EncodingTableEntry ete = GetEncodingTableEntry( platformID, 
                                                                encodingID);
                if (ete != null)
                {
                    st = GetSubtable(ete);
                }
            }
            catch
            {
            }

            return st;
        }


        /************************
         * supporting classes
         */

        public class EncodingTableEntry
        {
            public ushort platformID;
            public ushort encodingID;
            public uint   offset;
        }

        public abstract class Subtable
        {
            /// <summary>Abstract base class stores <c>platformID</c>,
            /// <c>encodingID</c>, <c>offset</c> and MBO byte array for whole
            /// cmap table.
            /// </summary>
            public Subtable(EncodingTableEntry ete, MBOBuffer bufTable)
            {
                m_ete = ete;
                m_bufTable = bufTable;
            }

            /// <summary>Return the first ushort of subtable</summary>
            public ushort format
            {
                get { return m_bufTable.GetUshort( m_ete.offset ); } 
                // the format field is the first field for all subtables
            }

            /// <summary>Return glyphID of (possibly) multi-byte code point
            /// in Intel byte order. Calls abstract overloaded version.
            /// </summary>
            public uint MapCharToGlyph(byte[] chars, uint charindex) 
            {
                // the 'byte[] chars' parameter is in Intel byte order for 
                // multi-byte chars
                return MapCharToGlyph(chars, charindex, false);
            }

            /// <summary>Abstract. Return glyphID of (possibly) multi-byte 
            /// code point.
            /// <c>bMBO</c> is true if buffer is in Motorola byte order, 
            /// false for Intel.
            /// </summary>
            abstract public uint MapCharToGlyph( byte[] chars, 
                                                 uint charindex, 
                                                 bool bMBO );


            abstract public uint BytesInChar(byte[] chars, uint charindex);

            /// <summary>Abstract property.</summary>
            abstract public uint length
            {
                get;
            }

            /// <summary>Abstract property.</summary>
            abstract public uint language
            {
                get;
            }

            /// <summary>Abstract method.</summary>
            abstract public uint[] GetMap();

            /// <summary>PlatformID, encodingID, and offset in 
            /// <c>m_bufTable</c>.
            /// </summary>
            public EncodingTableEntry m_ete;

            /// <summary>In practice, the entire cmap table buffer.</summary>
            public MBOBuffer m_bufTable;
        }

        public class Format0 : Subtable
        {
            public Format0( EncodingTableEntry ete, 
                            MBOBuffer bufTable )
                : base(ete, bufTable)
            {
            }

            public override uint MapCharToGlyph( byte[] chars, 
                                                 uint charindex, 
                                                 bool bMBO )
            {
                return GetGlyphID(chars[charindex]);
            }

            public override uint BytesInChar( byte[] chars, uint charindex )
            {
                return 1;
            }

            public enum FieldOffsets
            {
                format       = 0,
                length       = 2,
                language     = 4,
                glyphIDArray = 6
            }

            public override uint length
            {
                get {
                    return m_bufTable.GetUshort( m_ete.offset + 
                                                 (uint)FieldOffsets.length);
                }
            }

            public override uint language
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.language);
                }
            }

            public byte GetGlyphID(uint n)
            {
                return m_bufTable.GetByte(m_ete.offset + 
                                          (uint)FieldOffsets.glyphIDArray + n);
            }

            public override uint[] GetMap()
            {
                uint[] map = new uint[256];
                for (uint i=0; i<256; i++)
                {
                    map[i] = GetGlyphID(i);
                }
                return map;
            }

        }

        public class Format2 : Subtable
        {
            public Format2( EncodingTableEntry ete, MBOBuffer bufTable )
                : base(ete, bufTable)
            {
            }

            public enum FieldOffsets
            {
                format        = 0,
                length        = 2,
                language      = 4,
                subHeaderKeys = 6,
                subHeaders    = 518
                //glyphIndexArray = varies since subHeaders is variable length
            }

            public override uint length
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.length);
                }
            }

            public override uint language
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.language);
                }
            }

            public override uint MapCharToGlyph( byte[] chars,
                                                 uint charindex,
                                                 bool bMBO )
            {
                byte byte1 = chars[charindex];
                byte byte2 = chars[charindex+1];

                ushort nGlyph = 0;
                ushort SubHeaderIndex = (ushort)(GetSubHeaderKey(byte1) / 8);
                SubHeader sh = GetSubHeader(SubHeaderIndex);
                
                if (sh != null)
                {
                    if (SubHeaderIndex == 0) // one byte char
                    {
                        if ( byte1 >= sh.firstCode && 
                             byte1 < sh.firstCode + sh.entryCount )
                        {
                            ushort SIZEOF_SHORT = 2;
                            ushort offset = (ushort)
                                (sh.GetFirstCodeGlyphOffset() + 
                                 (byte1 - sh.firstCode) * SIZEOF_SHORT);
                            nGlyph = m_bufTable.GetUshort(offset);
                        }

                    }
                    else // two byte char
                    {
                        if (byte2 >= sh.firstCode && 
                            byte2 < sh.firstCode + sh.entryCount)
                        {
                            ushort SIZEOF_SHORT = 2;
                            ushort offset = (ushort)
                                (sh.GetFirstCodeGlyphOffset() + 
                                 (byte2 - sh.firstCode) * SIZEOF_SHORT);
                            nGlyph = m_bufTable.GetUshort(offset);
                            if (nGlyph != 0)
                            {
                                nGlyph += (ushort)sh.idDelta;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Assert(false, "error retrieving subheader");
                }

                return nGlyph;
            }

            public override uint BytesInChar(byte[] chars, uint charindex)
            {
                uint nBytes=1;

                if (GetSubHeaderKey(chars[charindex]) / 8 == 0)
                    nBytes = 1;
                else
                    nBytes = 2;

                return nBytes;
            }

            public ushort GetSubHeaderKey(byte n)
            {
                return 
                    m_bufTable.GetUshort( m_ete.offset + 
                                          (uint)FieldOffsets.subHeaderKeys + 
                                          (uint)n*2);
            }

            public SubHeader GetSubHeader(ushort n)
            {
                uint SIZEOF_SUBHEADER = 8;

                uint SubHeaderOffset = m_ete.offset + 
                    (uint)FieldOffsets.subHeaders + n*SIZEOF_SUBHEADER;

                SubHeader sh = null;
                
                if (SubHeaderOffset < m_bufTable.GetLength())
                {
                    sh = new SubHeader(SubHeaderOffset, m_bufTable);
                }


                return sh;
            }

            public class SubHeader
            {
                public SubHeader(uint offsetSubheader, MBOBuffer bufTable)
                {
                    firstCode     = bufTable.GetUshort(offsetSubheader);
                    entryCount    = bufTable.GetUshort(offsetSubheader + 2);
                    idDelta       = bufTable.GetShort (offsetSubheader + 4);
                    idRangeOffset = bufTable.GetUshort(offsetSubheader + 6);

                    m_offsetSubHeader = offsetSubheader;
                }

                public ushort firstCode;
                public ushort entryCount;
                public short  idDelta;
                public ushort idRangeOffset;

                public uint GetFirstCodeGlyphOffset()
                {
                    // return the offset of idRangeOffset plus the 
                    // value of idRangeOffset
                    return m_offsetSubHeader + 6 + idRangeOffset;
                }

                private uint m_offsetSubHeader;
            }

            public override uint[] GetMap()
            {
                return null; // TODO: implement
            }

        }

        public class Format4 : Subtable
        {
            public Format4(EncodingTableEntry ete, MBOBuffer bufTable) : 
                base(ete, bufTable)
            {
            }

            public enum FieldOffsets
            {
                format        = 0,
                length        = 2,
                language      = 4,
                segCountX2    = 6,
                searchRange   = 8,
                entrySelector = 10,
                rangeShift    = 12,
                endCode       = 14 
                // endCode is a variable length array, 
                // so remaining offsets must be calculated at runtime
            }

            public override uint length
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.length);
                }
            }
            
            public override uint language
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.language);
                }
            }

            public ushort segCountX2
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.segCountX2);
                }
            }

            public ushort searchRange
            {
                get { 
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.searchRange);
                }
            }

            public ushort entrySelector
            {
                get {
                    return
                        m_bufTable.GetUshort(m_ete.offset +
                                             (uint)FieldOffsets.entrySelector);
                }
            }

            public ushort rangeShift
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.rangeShift);
                }
            }

            public override uint MapCharToGlyph( byte[] chars,
                                                 uint charindex,
                                                 bool bMBO )
            {
                char c;
                if (bMBO)
                {
                    c = (char)
                        ((chars[charindex]<<8) + chars[charindex+1]); // MBO
                }
                else
                {
                    c = (char)
                        (chars[charindex] + (chars[charindex+1]<<8)); // IBO
                }

                return MapCharToGlyph( c);
            }

            public uint MapCharToGlyph(char c)
            {
                ushort nGlyph = 0;

                // find the index of the first segment endcode greater than 
                // the character
                uint i=0;
                ushort usEndCode = GetEndCode(i);
                while (usEndCode < c && usEndCode != 0xffff)
                {
                    i++;
                    usEndCode = GetEndCode(i);
                }

                // check if the starting code for that segment is <= to the 
                // character
                
                ushort usStartCode = GetStartCode(i);
                if (usStartCode <= c)
                {
                    // map it

                    ushort idRangeOffset = GetIdRangeOffset(i);
                    short idDelta = GetIdDelta(i);

                    if (idRangeOffset == 0)
                    {
                        nGlyph = (ushort)(c + idDelta);
                    }
                    else
                    {
                        uint AddressOfIdRangeOffset = (uint)
                            FieldOffsets.endCode + segCountX2*3u + 2 + i*2;
                        uint obscureIndex = (uint)
                            (idRangeOffset + (c-usStartCode)*2 + 
                             AddressOfIdRangeOffset);

                        // make sure we are not going to access outside of 
                        // table
                        if (m_ete.offset + obscureIndex < 
                            m_bufTable.GetLength())
                        {
                            nGlyph = m_bufTable.GetUshort(m_ete.offset +
                                                          obscureIndex);
                        }
                        else
                        {
                            nGlyph = 0;
                        }

                        if (nGlyph !=0 )
                        {
                            nGlyph = (ushort)(nGlyph + idDelta);
                        }

                    }
                }
                

                return nGlyph;
            }

            public override uint BytesInChar(byte[] chars, uint charindex)
            {
                return 2;
            }

            public ushort GetEndCode(uint n)
            {
                ushort endCode = 0;
                
                int segCount = segCountX2 / 2;
                
                // Some invalid fonts do not have terminating 0xFFFF entry,
                // so we will emulate it here. This is just a fix for our own 
                // code; some other code may break on that anyway.
                Debug.Assert(n<=segCount );
                
                if (n == segCount)
                {
                    return 0xFFFF;
                }

                // check for buffer overrun
                if (m_ete.offset + (uint)FieldOffsets.endCode + n * 2 < 
                    m_bufTable.GetLength())
                {
                    endCode = m_bufTable.GetUshort(m_ete.offset + 
                                                   (uint)FieldOffsets.endCode +
                                                   n * 2);
                }

                return endCode;
            }

            public ushort reservedPad
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.endCode + 
                                                segCountX2);
                }
            }

            public ushort GetStartCode(uint n)
            {
                ushort startCode = 0;

                ushort segCount = (ushort)(segCountX2 / 2);

                // Some invalid fonts do not have terminating 0xFFFF entry,
                // so we will emulate it here. This is just a fix for our 
                // own code; some other code may break on that anyway.
                Debug.Assert(n<=segCount);
                
                if (n == segCount)
                {
                    return 0xFFFF;
                }

                // check for buffer overrun
                if (m_ete.offset + 
                    (uint)FieldOffsets.endCode + segCountX2 + 2 + n*2 
                    < m_bufTable.GetLength())
                {
                    // startCode offset = endCode offset + sizeof_endcode + 
                    // sizeof_pad + index * sizeof_ushort
                    startCode = 
                        m_bufTable.GetUshort( m_ete.offset + 
                                              (uint)FieldOffsets.endCode +
                                              segCountX2 + 2 + n*2 );
                }

                return startCode;
            }

            public short GetIdDelta(uint n)
            {
                short idDelta = 0;
                ushort segCount = (ushort)(segCountX2 / 2);

                // Some invalid fonts do not have terminating 0xFFFF entry,
                // so we will emulate it here. 
                // This is just a fix for our own code;
                // some other code may break on that anyway.
                Debug.Assert(n<=segCount);
                
                if (n == segCount)
                {
                    return 1;
                }

                if ( m_ete.offset +
                     (uint)FieldOffsets.endCode + segCountX2 + 2 + segCountX2 +
                     n*2 < m_bufTable.GetLength())
                {
                    // idDelta offset = endCode offset + sizeof_EndCode + 
                    // sizeof_pad + sizeof_StartCode + index * sizeof_ushort
                    idDelta = m_bufTable.GetShort(m_ete.offset + 
                                                  (uint)FieldOffsets.endCode + 
                                                  segCountX2 + 2 + segCountX2 +
                                                  n*2);
                }

                return idDelta;
            }

            public ushort GetIdRangeOffset(uint n)
            {
                ushort idRangeOffset = 0;
                ushort segCount = (ushort)(segCountX2 / 2);

                // Some invalid fonts do not have terminating 0xFFFF entry,
                // so we will emulate it here.
                // This is just a fix for our own code;
                // some other code may break on that anyway.
                Debug.Assert(n<=segCount);
                
                if (n == segCount)
                {
                    return 0;
                }

                if ( m_ete.offset + (uint)FieldOffsets.endCode + 
                     segCountX2*3u + 2 + n*2 
                     < m_bufTable.GetLength())
                {
                    // idRangeOffset offset = endCode offset + 
                    // sizeof_EndCode + sizeof_pad + sizeof_StartCode + 
                    // sizeof_idDelta + index * sizeof_ushort
                    idRangeOffset = 
                        m_bufTable.GetUshort(m_ete.offset + 
                                             (uint)FieldOffsets.endCode + 
                                             segCountX2*3u + 2 + n*2);
                }
                return idRangeOffset;
            }

            public ushort GetGlyphId(uint n)
            {
                ushort glyphId = 0;

                if (m_ete.offset + (uint)FieldOffsets.endCode + segCountX2*4u +
                    2 + n*2 < m_bufTable.GetLength())
                {
                    // glyphId offset = endCode offset + 
                    // sizeof_theFourArrays + sizeof_pad + 
                    // index * sizeof_ushort
                    glyphId = m_bufTable.GetUshort(m_ete.offset + 
                                                   (uint)FieldOffsets.endCode +
                                                   segCountX2*4u + 2 + n*2);
                }

                return glyphId;
            }


            public override uint[] GetMap()
            {
                uint [] map = new uint[65536];

                int segCount = segCountX2/2;
                for (uint i=0; i<segCount; i++)
                {
                    ushort usStartCode = GetStartCode(i);
                    ushort usEndCode = GetEndCode(i);
                    ushort idRangeOffset = GetIdRangeOffset(i);
                    short idDelta = GetIdDelta(i);

                    if (idRangeOffset == 0)
                    {
                        for (uint c=usStartCode; c<= usEndCode; c++)
                        {
                            map[c] = (ushort)(c + idDelta);
                        }
                    }
                    else
                    {
                        for (uint c=usStartCode; c<= usEndCode; c++)
                        {
                            uint AddressOfIdRangeOffset = 
                                (uint)FieldOffsets.endCode + segCountX2*3u + 
                                2 + i*2;
                            uint obscureIndex = (uint)
                                (idRangeOffset + (c-usStartCode)*2 + 
                                 AddressOfIdRangeOffset);
                            ushort nGlyph = m_bufTable.GetUshort(m_ete.offset +
                                                                 obscureIndex);
                            if (nGlyph !=0 )
                            {
                                nGlyph = (ushort)(nGlyph + idDelta);
                            }
                            map[c] = nGlyph;
                        }
                    }
                }

                return map;
            }
        }

        public class Format6 : Subtable
        {
            public Format6( EncodingTableEntry ete, MBOBuffer bufTable ) 
                : base(ete, bufTable)
            {
            }

            public enum FieldOffsets
            {
                format        = 0,
                length        = 2,
                language      = 4,
                firstCode     = 6,
                entryCount    = 8,
                glyphIDArray  = 10,
            }

            public override uint length
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset +
                                                (uint)FieldOffsets.length);
                }
            }

            public override uint language
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset +
                                                (uint)FieldOffsets.language);
                }
            }

            public ushort firstCode
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.firstCode);
                }
            }

            public ushort entryCount
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.entryCount);
                }
            }

            public override uint BytesInChar(byte[] chars, uint charindex)
            {
                return 2;
            }

            public override uint MapCharToGlyph( byte[] chars,
                                                 uint charindex,
                                                 bool bMBO )
            {
                char c;
                if (bMBO)
                {
                    c = (char)
                        ((chars[charindex]<<8) + chars[charindex+1]); // MBO
                }
                else
                {
                    c = (char)
                        (chars[charindex] + (chars[charindex+1]<<8)); // IBO
                }

                return MapCharToGlyph( c);
            }

            public uint MapCharToGlyph(char c)
            {
                ushort iGlyph = 0;

                if ((uint)c >= firstCode && (uint)c < firstCode + entryCount)
                {
                    uint i = (uint)c - firstCode;
                    iGlyph = 
                        m_bufTable.GetUshort(m_ete.offset + 
                                             (uint)FieldOffsets.glyphIDArray +
                                             i*2);
                }

                return iGlyph;
            }


            public override uint[] GetMap()
            {
                uint [] map = new uint[65536];
                
                for (uint i = 0; i < entryCount; i++)
                {
                    uint c = firstCode + i;
                    ushort iGlyph = 
                        m_bufTable.GetUshort(m_ete.offset +
                                             (uint)FieldOffsets.glyphIDArray +
                                             i*2);
                    map[c] = iGlyph;
                }

                return map;
            }
        }

        public class Format8 : Subtable
        {
            public Format8(EncodingTableEntry ete,
                           MBOBuffer bufTable) : base(ete, bufTable)
            {
            }

            public enum FieldOffsets
            {
                format        = 0,
                reserved      = 2,
                length        = 4,
                language      = 8,
                is32          = 12,
                nGroups       = 8204,
                firstGroup    = 8208
            }

            public ushort reserved
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset +
                                                (uint)FieldOffsets.reserved);
                }
            }

            public override uint length
            {
                get {
                    return m_bufTable.GetUint(m_ete.offset +
                                              (uint)FieldOffsets.length);
                }
            }

            public override uint language
            {
                get {
                    return m_bufTable.GetUint(m_ete.offset +
                                              (uint)FieldOffsets.language);
                }
            }

            public bool is32(ushort i)
            {
                byte b = m_bufTable.GetByte(m_ete.offset + 
                                            (uint)FieldOffsets.is32 +
                                            (uint)i/8);
                return (b & (1 << (i%8))) != 0;
            }

            public uint nGroups
            {
                get {
                    return m_bufTable.GetUint(m_ete.offset +
                                              (uint)FieldOffsets.nGroups);
                }
            }


            public Group GetGroup(uint i)
            {
                if (i >= nGroups)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return new
                    Group(m_ete.offset + (uint)FieldOffsets.firstGroup + i*12,
                          m_bufTable);
            }

            public class Group
            {
                public Group(uint offset, MBOBuffer bufTable)
                {
                    m_offsetGroup = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    startCharCode = 0,
                    endCharCode   = 4,
                    startGlyphID  = 8
                }

                public uint startCharCode
                {
                    get {
                        return 
                            m_bufTable.GetUint(m_offsetGroup + (uint)
                                               FieldOffsets.startCharCode);
                    }
                }

                public uint endCharCode
                {
                    get {
                        return m_bufTable.GetUint(m_offsetGroup + (uint)
                                                  FieldOffsets.endCharCode);
                    }
                }

                public uint startGlyphID
                {
                    get {
                        return 
                            m_bufTable.GetUint(m_offsetGroup + 
                                               (uint)FieldOffsets.startGlyphID);
                    }
                }

                uint m_offsetGroup;
                MBOBuffer m_bufTable;
            }

            public override uint MapCharToGlyph( byte[] chars,
                                                 uint charindex,
                                                 bool bMBO )
            {
                uint char32 = 0;

                if (BytesInChar(chars, charindex) == 2)
                {
                    if (bMBO)
                    {
                        char32 = (char)
                            ((chars[charindex]<<8) + chars[charindex+1]); // MBO
                    }
                    else
                    {
                        char32 = (char)
                            (chars[charindex] + (chars[charindex+1]<<8)); // IBO
                    }
                }
                else
                {
                    char c1;
                    char c2;

                    if (bMBO)
                    {
                        c1 = (char)
                            ((chars[charindex]<<8) + chars[charindex+1]); // MBO
                        c2 = (char)
                            ((chars[charindex+2]<<8) + chars[charindex+1]);
                        // MBO
                    }
                    else
                    {
                        c1 = (char)
                            (chars[charindex] + (chars[charindex+1]<<8)); // IBO
                        c2 = (char)
                            (chars[charindex+2] + (chars[charindex+3]<<8)); 
                        // IBO
                    }

                    char32 = (uint)c1 + ((uint)c2<<16);
                }
                
                return MapCharToGlyph( char32);
            }

            public uint MapCharToGlyph(uint char32)
            {
                uint nGlyph = 0;

                for (uint i=0; i<nGroups; i++)
                {
                    Group g = GetGroup(i);
                    if (char32 >= g.startCharCode && char32 <= g.endCharCode)
                    {
                        nGlyph = g.startGlyphID + (char32 - g.startCharCode);
                        break;
                    }
                }

                return nGlyph;
            }

            public override uint BytesInChar(byte[] chars, uint charindex)
            {
                uint nBytes = 2;

                // oh no, in which byte order is the character stored?
                //char c = (char)((chars[charindex]<<8) + chars[charindex+1]);
                // MBO
                char c = (char)
                    (chars[charindex] + (chars[charindex+1]<<8)); // IBO

                if (is32(c))
                {
                    nBytes = 4;
                }

                return nBytes;
            }


            public override uint[] GetMap()
            {
                return null; // TODO: implement
            }
        }

        public class Format10 : Subtable
        {
            public Format10(EncodingTableEntry ete, MBOBuffer bufTable)
                : base(ete, bufTable)
            {
            }

            public enum FieldOffsets
            {
                format        = 0,
                reserved      = 2,
                length        = 4,
                language      = 8,
                startCharCode = 12,
                numChars      = 16,
                glyphs        = 20
            }

            public ushort reserved
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset + 
                                                (uint)FieldOffsets.reserved);
                }
            }

            public override uint length
            {
                get {
                    return m_bufTable.GetUint(m_ete.offset +
                                              (uint)FieldOffsets.length);
                }
            }

            public override uint language
            {
                get {
                    return m_bufTable.GetUint(m_ete.offset +
                                              (uint)FieldOffsets.language);
                }
            }

            public uint startCharCode
            {
                get { 
                    return 
                        m_bufTable.GetUint(m_ete.offset + 
                                           (uint)FieldOffsets.startCharCode);
                }
            }

            public uint numChars
            {
                get {
                    return m_bufTable.GetUint(m_ete.offset + 
                                              (uint)FieldOffsets.numChars);
                }
            }

            public ushort GetGlyph(uint i)
            {
                if (i >= numChars)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return m_bufTable.GetUshort(m_ete.offset + 
                                            (uint)FieldOffsets.glyphs + i*2);
            }


            public override uint MapCharToGlyph( byte[] chars,
                                                 uint charindex, 
                                                 bool bMBO)
            {
                uint char32 = 0;

                if (bMBO)
                {
                    char32 = 
                        ((uint)chars[charindex]<<24) + 
                        ((uint)chars[charindex+1]<<16) + 
                        ((uint)chars[charindex+2]<<8) + 
                        (uint)chars[charindex+3]; // MBO
                }
                else
                {
                    char32 = 
                        (uint)chars[charindex] + 
                        ((uint)chars[charindex+1]<<8) + 
                        ((uint)chars[charindex+2]<<16) +
                        ((uint)chars[charindex+3]<<24); // IBO
                }
                return MapCharToGlyph(char32);
            }

            public uint MapCharToGlyph(uint char32)
            {
                uint nGlyph = 0;

                if (char32 >= startCharCode && 
                    char32 <= (startCharCode+numChars))
                {
                    nGlyph = GetGlyph(char32 - startCharCode);
                }

                return nGlyph;
            }

            public override uint BytesInChar(byte[] chars, uint charindex)
            {
                return 4;
            }

            public override uint[] GetMap()
            {
                return null; // TODO: implement
            }
        }

        public class Format12 : Subtable
        {
            public Format12(EncodingTableEntry ete, MBOBuffer bufTable) 
                : base(ete, bufTable)
            {
            }

            public enum FieldOffsets
            {
                format        = 0,
                reserved      = 2,
                length        = 4,
                language      = 8,
                nGroups       = 12,
                firstGroup    = 16
            }

            public ushort reserved
            {
                get {
                    return m_bufTable.GetUshort(m_ete.offset +
                                                (uint)FieldOffsets.reserved);
                }
            }

            public override uint length
            {
                get {
                    return m_bufTable.GetUint(m_ete.offset +
                                              (uint)FieldOffsets.length);
                }
            }

            public override uint language
            {
                get {
                    return m_bufTable.GetUint(m_ete.offset +
                                              (uint)FieldOffsets.language);
                }
            }

            public override uint MapCharToGlyph( byte[] chars,
                                                 uint charindex, 
                                                 bool bMBO )
            {
                uint char32 = 0;

                if (BytesInChar(chars, charindex) == 2)
                {
                    if (bMBO)
                    {
                        char32 = (char)
                            ((chars[charindex]<<8) + chars[charindex+1]); // MBO
                    }
                    else
                    {
                        char32 = (char)
                            (chars[charindex] + (chars[charindex+1]<<8)); // IBO
                    }
                }
                else
                {
                    char h;
                    char l;

                    if (bMBO)
                    {
                        h = (char)
                            ((chars[charindex]<<8) + chars[charindex+1]); // MBO
                        l = (char)
                            ((chars[charindex+2]<<8) + chars[charindex+1]);
                        // MBO
                    }
                    else
                    {
                        h = (char)
                            (chars[charindex] + (chars[charindex+1]<<8)); // IBO
                        l = (char)
                            (chars[charindex+2] + (chars[charindex+3]<<8)); 
                        // IBO
                    }

                    char32 = SurrogatePairToUnicodeScalar(h, l);

                }

                return MapCharToGlyph(char32);
            }

            public uint MapCharToGlyph(uint char32)
            {
                uint nGlyph = 0;

                for (uint i=0; i<nGroups; i++)
                {
                    Group g = GetGroup(i);
                    if (char32 >= g.startCharCode && char32 <= g.endCharCode)
                    {
                        nGlyph = g.startGlyphID + (char32 - g.startCharCode);
                        break;
                    }
                }

                return nGlyph;
            }

            public override uint BytesInChar(byte[] chars, uint charindex)
            {
                uint nBytes = 2;

                // check if this is a UCS-4 character

                // oh no, in which byte order is the character stored?
                //char c1 = (char)((chars[charindex]<<8) + chars[charindex+1]);
                // MBO
                char c1 = (char)
                    (chars[charindex] + (chars[charindex+1]<<8)); // IBO
                if (IsHighSurrogate(c1) && chars.Length > charindex+3)
                {
                    //char c2 = (char)((chars[charindex+2]<<8) + 
                    //          chars[charindex+1]); // MBO
                    char c2 = (char)
                        (chars[charindex+2] + (chars[charindex+3]<<8)); // IBO
                    if (IsLowSurrogate(c2))
                    {
                        nBytes = 4;
                    }
                }

                return nBytes;
            }

            public uint SurrogatePairToUnicodeScalar(uint SurrogateHigh,
                                                     uint SurrogateLow)
            {
                return ((uint)SurrogateHigh - 0xd800) * 0x0400 + 
                    ((uint)SurrogateLow - 0xdc00) + 0x10000;
            }

            public bool IsHighSurrogate(char c)
            {
                return (c >= 0xd800 && c <= 0xdbff);
            }

            public bool IsLowSurrogate(char c)
            {
                return (c >= 0xdc00 && c <= 0xdfff);
            }

            public uint nGroups
            {
                get {
                    return m_bufTable.GetUint(m_ete.offset + 
                                              (uint)FieldOffsets.nGroups);
                }
            }

            public Group GetGroup(uint i)
            {
                if (i >= nGroups)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return new Group(m_ete.offset + (uint)FieldOffsets.firstGroup +
                                 i*12, 
                                 m_bufTable);
            }

            public class Group
            {
                public Group(uint offset, MBOBuffer bufTable)
                {
                    m_offsetGroup = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    startCharCode = 0,
                    endCharCode   = 4,
                    startGlyphID  = 8
                }

                public uint startCharCode
                {
                    get {
                        return m_bufTable.GetUint(m_offsetGroup + (uint)
                                                  FieldOffsets.startCharCode);
                    }
                }

                public uint endCharCode
                {
                    get {
                        return m_bufTable.GetUint(m_offsetGroup + (uint)
                                                  FieldOffsets.endCharCode);
                    }
                }

                public uint startGlyphID
                {
                    get {
                        return m_bufTable.GetUint(m_offsetGroup + (uint)
                                                  FieldOffsets.startGlyphID);
                    }
                }

                uint m_offsetGroup;
                MBOBuffer m_bufTable;
            }


            public override uint[] GetMap()
            {
                Group g = GetGroup(nGroups-1);
                uint nArraySize = g.endCharCode + 1;

                uint [] map = new uint[(int)nArraySize];

                for (uint nGroup = 0; nGroup<nGroups; nGroup++)
                {
                    g = GetGroup(nGroup);

                    for (uint i=0; i<=g.endCharCode-g.startCharCode; i++)
                    {
                        map[g.startCharCode + i] = g.startGlyphID + i;
                    }
                }

                return map;
            }
        }


        public class Format14 : Subtable
        {

            public enum FieldOffsets
            {
                format                = 0, // USHORT
                length                = 2, // ULONG
                numVarSelectorRecords = 6, // ULONG
                firstSelectorRecord   = 10
            }

            public class UnicodeValueRange
            {
                public uint    startUnicodeValue;  // UINT24
                public byte    additionalCount;    // BYTE

                public const uint BYTES_PER_RECORD = 4;

                // Offset is absolute within MBOBuffer
                public void Populate( MBOBuffer b, uint offset )
                {
                    startUnicodeValue = b.GetUint24( offset );
                    additionalCount = b.GetByte( offset + 3 );
                }

            }

            public class DefaultUVSTable
            {
                public uint                     numUnicodeValueRanges; // ULONG
                public List<UnicodeValueRange>  ranges;

                // Offset is relative to start of subtable
                public void Populate( MBOBuffer b, 
                                      uint subtableOffset,
                                      uint offset )
                {
                    uint o = subtableOffset + offset;
                    numUnicodeValueRanges = b.GetUint( o );
                    ranges = new List<UnicodeValueRange>();
                    o += 4;
                    for ( uint i = 0; i < numUnicodeValueRanges; i++ ) {
                        UnicodeValueRange r = new UnicodeValueRange();
                        r.Populate( b, o );
                        ranges.Add( r );
                        o += UnicodeValueRange.BYTES_PER_RECORD;
                    }
                }

                // Offset relative to subtable start
                // Return value is relative to subtable start
                static public uint MaxOffset( MBOBuffer b, 
                                              uint subtableOffset,
                                              uint offset )
                {
                    
                    uint nRec = b.GetUint( subtableOffset + offset );
                    return 
                        offset + 4 + 
                        ( nRec * UnicodeValueRange.BYTES_PER_RECORD )
                        - 1;
                }
            }

            public class UVSMapping
            {
                public uint    unicodeValue;  // UINT24
                public ushort  glyphID;       // USHORT

                public const uint BYTES_PER_RECORD = 5;

                // Offset is absolute within MBOBuffer
                public void Populate( MBOBuffer b, uint offset )
                {
                    unicodeValue = b.GetUint24( offset );
                    glyphID = b.GetUshort( offset + 3 );
                }

            }

            public class NonDefaultUVSTable
            {
                public uint     numUVSMappings;  // ULONG
                public List<UVSMapping> mappings;

                public void Populate( MBOBuffer b, 
                                      uint subtableOffset,
                                      uint offset )
                {
                    uint o = subtableOffset + offset;
                    mappings = new List<UVSMapping>();
                    numUVSMappings = b.GetUint( o );
                    o += 4;
                    for ( uint i = 0; i < numUVSMappings; i++ ) {
                        UVSMapping r = new UVSMapping();
                        r.Populate( b, o );
                        mappings.Add( r );
                        o += UVSMapping.BYTES_PER_RECORD;
                    }
                }

                // Offset relative to subtable start
                // Return value is relative to subtable start
                static public uint MaxOffset( MBOBuffer b, 
                                              uint subtableOffset,
                                              uint offset )
                {
                    uint nRec = b.GetUint( subtableOffset + offset );
                    return 
                        offset + 4 + ( nRec * UVSMapping.BYTES_PER_RECORD ) - 1;
                }

            }


            public class VarSelectorRecord 
            {
                private uint    m_subtableOffset;
                public  uint    varSelector;         // UINT24
                public  uint    defaultUVSOffset;    // ULONG
                public  uint    nonDefaultUVSOffset; // ULONG

                private MBOBuffer m_buf;
                public VarSelectorRecord( MBOBuffer buf,
                                          uint      subtableOffset ) {
                    m_buf = buf;
                    m_subtableOffset = subtableOffset;
                }

                public const uint BYTES_PER_RECORD = 11;

                // Offset is relative to subtable start
                public void Populate( uint offset )
                {
                    uint off = m_subtableOffset + offset;
                    varSelector = m_buf.GetUint24( off );
                    defaultUVSOffset = m_buf.GetUint( off + 3 );
                    nonDefaultUVSOffset = m_buf.GetUint( off + 7 );
                }

                // Returned value is relative to start of subtable
                public uint GetDefaultUVSMaxOffset()
                {
                    uint off = 0;
                    if ( defaultUVSOffset > 0 ) {
                        off = DefaultUVSTable.MaxOffset( m_buf, 
                                                         m_subtableOffset,
                                                         defaultUVSOffset );
                    }
                    return off;
                }

                // Returned value is relative to start of subtable
                public uint GetNonDefaultUVSMaxOffset()
                {
                    uint off = 0;
                    if ( nonDefaultUVSOffset > 0 ) {
                        off = NonDefaultUVSTable.MaxOffset(m_buf, 
                                                           m_subtableOffset,
                                                           nonDefaultUVSOffset);
                    }
                    return off;
                }

                public NonDefaultUVSTable GetNonDefaultUVSTable()
                {
                    NonDefaultUVSTable tbl = null;
                    if ( nonDefaultUVSOffset > 0 ) {
                        tbl = new NonDefaultUVSTable();
                        tbl.Populate( m_buf, 
                                      m_subtableOffset, 
                                      nonDefaultUVSOffset );
                    }
                    return tbl;
                }

                public DefaultUVSTable GetDefaultUVSTable()
                {
                    DefaultUVSTable tbl = null;
                    if ( defaultUVSOffset > 0 ) {
                        tbl = new DefaultUVSTable();
                        tbl.Populate( m_buf, 
                                      m_subtableOffset,
                                      defaultUVSOffset );
                    }
                    return tbl;
                }

            }

            public Format14( EncodingTableEntry ete, MBOBuffer bufTable ) : 
                base(ete, bufTable)
            {
            }

            public override uint length
            {
                get {
                    return m_bufTable.GetUint( m_ete.offset + 
                                               (uint)FieldOffsets.length);
                }
            }
            
            public override uint language
            {
                get {
                    return 0;
                }
            }

            public uint NumVarSelectorRecs
            {
                get {
                    return
                        m_bufTable.GetUint( m_ete.offset + 
                                            (uint)FieldOffsets.
                                            numVarSelectorRecords);
                }
            }
            
            // This is from the beginning of the subtable
            private uint IthSelectorRecordOffset( uint i )
            {
                // Already should have been checked
                Debug.Assert( i < NumVarSelectorRecs );
                return (uint)
                    FieldOffsets.firstSelectorRecord
                    + ( i * VarSelectorRecord.BYTES_PER_RECORD );
            }

            public VarSelectorRecord GetIthSelectorRecord( uint i )
            {
                if ( i >= NumVarSelectorRecs ) {
                    return null;
                }
                VarSelectorRecord v = new VarSelectorRecord( m_bufTable,
                                                             m_ete.offset );
                v.Populate( IthSelectorRecordOffset(i) );
                return v;
            }

            // 1. All of the calls to this method in val_cmap are provably
            //    from other subtables.
            // 2. Same for OTFont.
            // 3. Same for I_IOGLYPHSFILE.
            // 4. Same for val_name.
            // 5. Same for val_os2.
            public override uint MapCharToGlyph( byte[] chars, 
                                                 uint charindex, 
                                                 bool bMBO )
            {
                String msg = "Format 14 ByteInChar() should not be called";
                throw new ApplicationException( msg );
            }

            /// <summary>This routine is only called for typesetting
            /// the sample string (<c>NameID==19</c>) from the 'name' table.
            /// <p/>
            /// The OpenType Spec states:
            /// "Unicode platform encoding ID 5 can be used for encodings 
            /// in the 'cmap' table but not for strings in the 'name' table."
            /// <p/>
            /// In other words, it is illegal to have a name table entry
            /// with <c>PlatformID==0, EncodingID==5</c>.
            /// <p/>
            /// 
            /// </summary>
            public override uint BytesInChar(byte[] chars, uint charindex)
            {
                String msg = "Format 14 ByteInChar() should not be called";
                throw new ApplicationException( msg );
            }

            /// I am pretty sure this never gets called, but there is a call
            /// in the cmap_cache that I need to track down.
            public override uint[] GetMap()
            {
                return new uint[0];
            }

        }



        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new cmap_cache(this);
            }

            return m_cache;
        }
        
        /*
         * This class does not appear to be used by FontValidator.
         * 'head', 'name' and 'post' caches are referenced in various apps.
         */
        public class cmap_cache : DataCache
        {
            // the cached data

            public class CachedSubtable
            {
                public CachedSubtable(ushort platID, ushort encID)
                {
                    m_platID = platID;
                    m_encID = encID;
                }

                public ushort m_platID;
                public ushort m_encID;

                public uint[] m_CharToGlyphMap;
            }

            public class SubtableArray : ArrayList
            {
                public CachedSubtable GetSubtable(ushort platID, ushort encID)
                {
                    CachedSubtable st = null;
                    for (int i=0; i<Count; i++)
                    {
                        CachedSubtable temp = (CachedSubtable)this[i];
                        if (temp.m_platID == platID &&
                            temp.m_encID  == encID)
                        {
                            st = temp;
                            break;
                        }
                    }
                    return st;
                }

                public int GetSubtableIndex(ushort platID, ushort encID)
                {
                    int iRet = -1;
                    for (int i=0; i<Count; i++)
                    {
                        CachedSubtable st = (CachedSubtable)this[i];
                        if (st.m_platID == platID &&
                            st.m_encID  == encID)
                        {
                            iRet = i;
                            break;
                        }
                    }
                    return iRet;
                }
            }

            SubtableArray m_arrSubtables;
            CachedSubtable m_DefaultSubtable;


            // constructor

            public cmap_cache(Table_cmap OwnerTable)
            {
                m_arrSubtables = new SubtableArray();

                for ( uint iSubtable=0; 
                      iSubtable < OwnerTable.NumberOfEncodingTables; 
                      iSubtable++ ) {
                    EncodingTableEntry ete = 
                        OwnerTable.GetEncodingTableEntry(iSubtable);
                    Subtable st = OwnerTable.GetSubtable(ete);

                    CachedSubtable cst = new 
                        CachedSubtable(ete.platformID, ete.encodingID);
                    cst.m_CharToGlyphMap = st.GetMap();

                    m_arrSubtables.Add(cst);
                    
                    if (cst.m_platID == 3 && cst.m_encID == 10)
                    {
                        m_DefaultSubtable = cst;
                    }
                    
                    if ( cst.m_platID == 3 && cst.m_encID == 1 && 
                         m_DefaultSubtable == null)
                    {
                        m_DefaultSubtable = cst;
                    }
                }
            }

            // accessors for the cached data

            public ushort MapCharToGlyph( ushort platID,
                                          ushort encID, 
                                          BigUn charcode )
            {
                ushort glyphID = 0xffff;

                CachedSubtable st = m_arrSubtables.GetSubtable(platID, encID);
                if (st != null)
                {
                    glyphID = (ushort)st.m_CharToGlyphMap[(uint)charcode];
                }
                else
                {
                    throw new ApplicationException("subtable not found");
                }

                return glyphID;
            }

            public ushort MapCharToGlyph(uint charcode)
            {
                ushort glyphID = 0xffff;

                if (m_DefaultSubtable != null && 
                    charcode < m_DefaultSubtable.m_CharToGlyphMap.Length)
                {
                    glyphID = (ushort)
                        m_DefaultSubtable.m_CharToGlyphMap[charcode];
                }
                else
                {
                    throw new ApplicationException("Subtable not found");
                }

                return glyphID;
            }

            public void AddSubtable(ushort platID, ushort encID)
            {
                CachedSubtable st = m_arrSubtables.GetSubtable(platID, encID);
                if (st != null)
                {
                    throw new 
                        ApplicationException( "attempted to add duplicate " + 
                                              "subtable");
                }

                m_arrSubtables.Add(new CachedSubtable(platID, encID));

                m_bDirty = true;
            }

            public void RemoveSubtable(ushort platID, ushort encID)
            {
                int index = m_arrSubtables.GetSubtableIndex(platID, encID);

                if (index == -1)
                {
                    throw new ApplicationException( "attempted to remove " + 
                                                    "nonexistant subtable");
                }

                m_arrSubtables.RemoveAt(index);

                m_bDirty = true;
            }

            public void AddChar( ushort platID, 
                                 ushort encID, 
                                 BigUn charcode, 
                                 ushort glyphID )
            {
                CachedSubtable st = m_arrSubtables.GetSubtable(platID, encID);
                if (st != null)
                {
                    if ((uint)charcode >= (uint)st.m_CharToGlyphMap.Length)
                    {
                        uint[] newmap = new uint[(uint)charcode+1];
                        System.Buffer.BlockCopy( st.m_CharToGlyphMap, 0, 
                                                 newmap, 0, 
                                                 st.m_CharToGlyphMap.Length*4);
                        st.m_CharToGlyphMap = newmap;
                    }

                    st.m_CharToGlyphMap[(uint)charcode] = glyphID;
                }
                else
                {
                    throw new ApplicationException("subtable not found");
                }

                m_bDirty = true;
            }

            public void RemoveChar(ushort platID, ushort encID, BigUn charcode)
            {
                CachedSubtable st = m_arrSubtables.GetSubtable(platID, encID);
                if (st != null)
                {
                    st.m_CharToGlyphMap[(uint)charcode] = 0;
                }
                else
                {
                    throw new ApplicationException("subtable not found");
                }

                m_bDirty = true;
            }

            // generate a new table from the cached data
            public override OTTable GenerateTable()
            {
                // generate the subtables
                ArrayList arrSubtableBuffers = new ArrayList();
                for (int i=0; i<m_arrSubtables.Count; i++)
                {
                    byte [] buf = null;
                    CachedSubtable st = (CachedSubtable)m_arrSubtables[i];

                    if (st.m_platID == 3 && st.m_encID == 10)
                    {
                        buf = GenerateFormat12Subtable(st.m_CharToGlyphMap);
                    }
                    else if (st.m_platID == 1 && st.m_encID == 0)
                    {
                        buf = GenerateFormat0Subtable(st.m_CharToGlyphMap, 0);
                    }
                    else
                    {
                        buf = GenerateFormat4Subtable(st.m_CharToGlyphMap);
                    }
                    arrSubtableBuffers.Add(buf);
                }


                // calculate the number of bytes required for the cmap table

                uint nBytes = 4; // version and number of subtables
                // encoding table entries
                nBytes += 8 * (uint)m_arrSubtables.Count; 
                for (int i=0; i<m_arrSubtables.Count; i++)
                {
                    byte[] buf = (byte[])arrSubtableBuffers[i];
                    nBytes += (uint)buf.Length;
                }

                // create a buffer for the new cmap table

                MBOBuffer newbuf = new MBOBuffer(nBytes);


                // populate the buffer
                
                // version and number of encoding tables
                newbuf.SetUshort(0, (uint)
                                 Table_cmap.FieldOffsets.TableVersionNumber);
                newbuf.SetUshort((ushort)m_arrSubtables.Count, (uint)
                                 Table_cmap.FieldOffsets.NumberOfEncodingTables
                                 );

                // encoding table entries
                uint SubtableOffset = (uint)(4 + 8*m_arrSubtables.Count);
                for (int i=0; i < m_arrSubtables.Count; i++)
                {
                    CachedSubtable st = (CachedSubtable)m_arrSubtables[i];
                    uint eteOffset = 
                        (uint)Table_cmap.FieldOffsets.EncodingTableEntries + 
                        (uint)i*8;
                    newbuf.SetUshort(st.m_platID, eteOffset);
                    newbuf.SetUshort(st.m_encID,  eteOffset+2);
                    newbuf.SetUint(SubtableOffset, eteOffset+4);
                    byte[] buf = (byte[])arrSubtableBuffers[i];
                    SubtableOffset += (uint)buf.Length;
                }

                // subtables
                byte[] TableBuf = newbuf.GetBuffer();
                SubtableOffset = (uint)(4 + 8*m_arrSubtables.Count);
                for (int i=0; i<m_arrSubtables.Count; i++)
                {
                    byte[] SubtableBuf = (byte[])arrSubtableBuffers[i];
                    System.Buffer.BlockCopy(SubtableBuf, 0, TableBuf, 
                                            (int)SubtableOffset, 
                                            SubtableBuf.Length);
                    SubtableOffset += (uint)SubtableBuf.Length;
                }

                // put the buffer into a Table_cmap object and return it

                Table_cmap cmapTable = new Table_cmap("cmap", newbuf);

                return cmapTable;
            }

            protected byte[] GenerateFormat0Subtable( uint [] map, 
                                                      ushort language )
            {
                // allocate the buffer
                MBOBuffer buf = new MBOBuffer(262);

                // set the data

                buf.SetUshort(0, (uint)Format0.FieldOffsets.format);
                buf.SetUshort(262, (uint)Format0.FieldOffsets.length);
                buf.SetUshort(language, (uint)Format0.FieldOffsets.language);
                for (int i=0; i<256; i++)
                {
                    buf.SetByte((byte)map[i], 
                                (uint)Format0.FieldOffsets.glyphIDArray + 
                                (uint)i);
                }

                // return the buffer

                return buf.GetBuffer();
            }

            protected class segment4
            {
                public ushort startCode, endCode;
                public short  idDelta;
                public ushort idRangeOffset;
                public ushort [] glyphIdArray;
            }

            protected byte[] GenerateFormat4Subtable(uint [] map)
            {
                bool bFound = false;
                uint CurChar = 0;
                ArrayList arrSegments = new ArrayList();
                segment4 seg = new segment4();

                while (CurChar < map.Length)
                {
                    // find the startCode char in this segment
                    bFound = false;
                    for (uint i=CurChar; i<map.Length; i++)
                    {
                        if (map[i] != 0)
                        {
                            seg.startCode = (ushort)i;
                            bFound = true;
                            break;
                        }
                    }
                    if (bFound)
                    {
                        // count number of chars available for delta 
                        // representation
                        short delta = (short)
                            (map[seg.startCode] - seg.startCode);
                        uint nDelta = 1;
                        for (int i=seg.startCode+1; i<map.Length; i++)
                        {
                            if ((ushort)(i+delta) == (ushort)(map[i]))
                            {
                                nDelta++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        
                        // count number of chars available for rangeoffset 
                        // to glyph id representation
                        uint nRangeGlyph=1;
                        if (nDelta == 1)
                        {
                            for (int i=seg.startCode+1; i<map.Length; i++)
                            {
                                if (map[i] == 0)
                                {
                                    // don't keep the zero if it's followed 
                                    // by another zero
                                    if (i+1 < map.Length)
                                    {
                                        if (map[i+1] == 0)
                                        {
                                            break;
                                        }
                                    }
                                    // don't keep the zero if it's followed by
                                    // a delta segment
                                    if (i+3 < map.Length)
                                    {
                                        if ((ushort)(map[i+1]+1) == 
                                            (ushort)(map[i+2]) &&
                                            (ushort)(map[i+2]+1) == 
                                            (ushort)(map[i+3]))
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else if (i+2 < map.Length)
                                {
                                    // stop if this is the start of a two 
                                    // glyph delta segment
                                    if ((ushort)(map[i  ]+1) == 
                                        (ushort)(map[i+1]) &&
                                        (ushort)(map[i+1]+1) == 
                                        (ushort)(map[i+2]))
                                    {
                                        break;
                                    }
                                }
                                else if (i+1 < map.Length)
                                {
                                    // stop if we're at the end of the map
                                    if ((ushort)(map[i]+1) == 
                                        (ushort)(map[i+1]))
                                    {
                                        break;
                                    }
                                }

                                nRangeGlyph++;
                            }
                        }

                        // choose delta or rangeOffset representation
                        if (nDelta >= nRangeGlyph)
                        {
                            // set the endCode character for this segment
                            CurChar = seg.startCode;
                            if (CurChar < 65535)
                            {
                                seg.endCode = (ushort)
                                    (seg.startCode + nDelta - 1);
                                CurChar = seg.endCode;
                            }
                            else if (CurChar == 65535)
                            {
                                seg.endCode = 65535;
                            }
                            else
                            {
                                Debug.Assert(false);
                            }
                            Debug.Assert(seg.startCode <= seg.endCode);

                            // calculate the id delta
                            seg.idDelta = 
                                (short)(map[seg.startCode] - seg.startCode);
                    
                            // add the segment to the array
                            arrSegments.Add(seg);
                            Debug.Assert(arrSegments.Count < 65536);
                            CurChar = (uint)(seg.endCode + 1);
                            seg = new segment4();
                        }
                        else
                        {
                            // set the endCode character for this segment
                            CurChar = seg.startCode;
                            if (CurChar < 65535)
                            {
                                seg.endCode = 
                                    (ushort)(seg.startCode + nRangeGlyph - 1);
                                CurChar = seg.endCode;

                                Debug.Assert(seg.startCode != seg.endCode);
                            }
                            else if (CurChar == 65535)
                            {
                                seg.endCode = 65535;
                            }
                            else
                            {
                                Debug.Assert(false);
                            }

                            // store each character's glyph value in the 
                            // glyphID array

                            int nCount = seg.endCode-seg.startCode+1;
                            seg.glyphIdArray = new ushort[nCount];
                            for (ushort i=0; i<nCount; i++)
                            {
                                seg.glyphIdArray[i] = 
                                    (ushort)map[seg.startCode+i];
                            }

                            // add the segment to the array
                            arrSegments.Add(seg);
                            Debug.Assert(arrSegments.Count < 65536);
                            CurChar = (uint)(seg.endCode + 1);
                            seg = new segment4();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                // add a final segment if needed
                bool bNeedFinal = true;
                if (arrSegments.Count != 0)
                {
                    segment4 lastseg = (segment4)
                        arrSegments[arrSegments.Count-1];
                    if (lastseg.endCode == 0xffff)
                    {
                        bNeedFinal = false;
                    }
                }
                if (bNeedFinal)
                {
                    segment4 segFinal = new segment4();
                    segFinal.startCode = 0xffff;
                    segFinal.endCode = 0xffff;
                    segFinal.idDelta = 1;
                    arrSegments.Add(segFinal);
                }

                // allocate a buffer for the subtable

                ushort nSize = (ushort)(16 + 8 * arrSegments.Count);
                for (int i=0; i<arrSegments.Count; i++)
                {
                    segment4 s = (segment4)arrSegments[(int)i];
                    if (s.glyphIdArray != null)
                    {
                        nSize += (ushort)(s.glyphIdArray.Length*2);
                    }
                }
                MBOBuffer buf = new MBOBuffer(nSize);

                // set the data

                ushort segCountX2 = (ushort)(arrSegments.Count*2);
                ushort searchRange = (ushort)
                    (2 * util.MaxPower2LE((ushort)(arrSegments.Count)));
                ushort entrySelector = util.Log2((ushort)(searchRange/2));
                ushort rangeShift = (ushort)(segCountX2 - searchRange);

                buf.SetUshort(4,             
                              (uint)Format4.FieldOffsets.format);
                buf.SetUshort(nSize,
                              (uint)Format4.FieldOffsets.length);
                buf.SetUshort(0, 
                              (uint)Format4.FieldOffsets.language);
                buf.SetUshort(segCountX2, 
                              (uint)Format4.FieldOffsets.segCountX2);
                buf.SetUshort(searchRange, 
                              (uint)Format4.FieldOffsets.searchRange);
                buf.SetUshort(entrySelector, 
                              (uint)Format4.FieldOffsets.entrySelector);
                buf.SetUshort(rangeShift,
                              (uint)Format4.FieldOffsets.rangeShift);

                uint endOffset = (uint)Format4.FieldOffsets.endCode;
                uint startOffset = (uint)Format4.FieldOffsets.endCode + 
                    2*(uint)arrSegments.Count + 2;
                uint idDeltaOffset = startOffset + 2*(uint)arrSegments.Count;
                uint idRangeOffset = idDeltaOffset + 2*(uint)arrSegments.Count;

                int nGlyphIdCount = 0;
                for (uint i=0; i<arrSegments.Count; i++)
                {
                    segment4 s = (segment4)arrSegments[(int)i];

                    if (s.glyphIdArray != null)
                    {
                        s.idRangeOffset = (ushort)
                            ((arrSegments.Count - i + nGlyphIdCount)*2);
                        nGlyphIdCount += s.glyphIdArray.Length;
                    }

                    buf.SetUshort(s.endCode, endOffset + i*2);
                    buf.SetUshort(s.startCode, startOffset + i*2);
                    buf.SetShort(s.idDelta, idDeltaOffset + i*2);
                    buf.SetUshort(s.idRangeOffset, idRangeOffset + i*2);

                }
                uint glyphIdPos = idRangeOffset + 2*(uint)arrSegments.Count;
                for (uint i=0; i<arrSegments.Count; i++)
                {
                    segment4 s = (segment4)arrSegments[(int)i];
                    if (s.glyphIdArray != null)
                    {
                        for (uint j=0; j<s.glyphIdArray.Length; j++)
                        {
                            buf.SetUshort(s.glyphIdArray[j], glyphIdPos);
                            glyphIdPos += 2;
                        }
                    }
                }

                
                return buf.GetBuffer();
            }


            protected class segment12
            {
                public uint startCharCode, endCharCode, startGlyphID;
            }

            protected byte[] GenerateFormat12Subtable(uint [] map)
            {
                bool bFound = false;
                uint CurChar = 0;
                ArrayList arrSegments = new ArrayList();
                segment12 seg = new segment12();

                while (CurChar < map.Length)
                {
                    // find the startCode char in this segment
                    bFound = false;
                    for (uint i=CurChar; i<map.Length; i++)
                    {
                        if (map[i] != 0)
                        {
                            seg.startCharCode = i;
                            bFound = true;
                            break;
                        }
                    }
                    if (bFound)
                    {
                        // find the endCode character in this segment
                        CurChar = seg.startCharCode;
                        if (CurChar < map.Length-2)
                        {
                            while (CurChar < map.Length - 1)
                            {
                                if (map[CurChar]+1 == map[CurChar+1] && 
                                    CurChar != map.Length-2)
                                {
                                    CurChar++;
                                }
                                else
                                {
                                    seg.endCharCode = CurChar;
                                    break;
                                }
                            }
                        }
                        else if (CurChar == map.Length - 1)
                        {
                            seg.endCharCode = CurChar;
                        }
                        else
                        {
                            Debug.Assert(false);
                        }
                        Debug.Assert(seg.startCharCode <= seg.endCharCode);

                        // store the start glyph ID for this segment
                        seg.startGlyphID = map[seg.startCharCode];
                    
                        // add the segment to the array
                        arrSegments.Add(seg);
                        Debug.Assert(arrSegments.Count < map.Length, 
                                     "more segments than characters");
                        CurChar = (uint)(seg.endCharCode + 1);
                        seg = new segment12();
                    }
                    else
                    {
                        break;
                    }
                }

                // allocate a buffer for the subtable

                uint nSize = (uint)(16 + 12 * arrSegments.Count);
                MBOBuffer buf = new MBOBuffer(nSize);

                // set the data
                buf.SetUshort(12,  (uint)Format12.FieldOffsets.format);
                buf.SetUshort(0,   (uint)Format12.FieldOffsets.reserved);
                buf.SetUint(nSize, (uint)Format12.FieldOffsets.length);
                buf.SetUint(0,     (uint)Format12.FieldOffsets.language);
                buf.SetUint((uint)arrSegments.Count, 
                            (uint)Format12.FieldOffsets.nGroups);

                for (int i=0; i<arrSegments.Count; i++)
                {
                    seg = (segment12)arrSegments[i];
                    buf.SetUint(seg.startCharCode, (uint)
                                (Format12.FieldOffsets.firstGroup + i*12));
                    buf.SetUint(seg.endCharCode,   (uint)
                                (Format12.FieldOffsets.firstGroup + i*12 + 4));
                    buf.SetUint(seg.startGlyphID,  (uint)
                                (Format12.FieldOffsets.firstGroup + i*12 + 8));
                }

                return buf.GetBuffer();
            }
        }

        

    }
}
