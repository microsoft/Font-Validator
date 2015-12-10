using System;


namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_Zapf.
    /// </summary>
    public class Table_Zapf : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_Zapf(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * nested classes
         */

        
        public class GlyphInfo
        {
            public GlyphInfo(uint offset, MBOBuffer bufTable)
            {
                m_offsetGlyphInfo = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                groupOffset    = 0,
                featOffset     = 4,
                n16BitUnicodes = 8,
                unicodes       = 10
            }

            // GlyphInfo accessors
            
            public uint groupOffset
            {
                get {return m_bufTable.GetUint(m_offsetGlyphInfo + (uint)FieldOffsets.groupOffset);}
            }

            public uint featOffset
            {
                get {return m_bufTable.GetUint(m_offsetGlyphInfo + (uint)FieldOffsets.featOffset);}
            }

            public ushort n16BitUnicodes
            {
                get {return m_bufTable.GetUshort(m_offsetGlyphInfo + (uint)FieldOffsets.n16BitUnicodes);}
            }

            public ushort GetUnicodeCodePoint(uint i)
            {
                if (i >= n16BitUnicodes)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return m_bufTable.GetUshort(m_offsetGlyphInfo + (uint)FieldOffsets.unicodes + i*2);
            }

            public ushort nNames
            {
                get {return m_bufTable.GetUshort(m_offsetGlyphInfo + (uint)FieldOffsets.unicodes + (uint)n16BitUnicodes*2);}
            }

            public KindName GetKindName(uint i)
            {
                if (i >= nNames)
                {
                    throw new ArgumentOutOfRangeException();
                }

                // find the offset
                uint offset = m_offsetGlyphInfo + (uint)FieldOffsets.unicodes + (uint)n16BitUnicodes*2 + 2;
                for (int j=0; j<i; j++)
                {
                    KindName knTemp = new KindName(offset, m_bufTable);
                    offset += knTemp.GetLength();
                }

                return new KindName(offset, m_bufTable);
            }


            // member data
            uint m_offsetGlyphInfo;
            MBOBuffer m_bufTable;
        }

        public class KindName
        {
            public KindName(uint offset, MBOBuffer bufTable)
            {
                m_offsetKindName = offset;
                m_bufTable = bufTable;
            }

            public byte Type
            {
                get {return m_bufTable.GetByte(m_offsetKindName);}
            }

            public byte PascalStringLength
            {
                get {return m_bufTable.GetByte(m_offsetKindName+1);}
            }

            public string GetString()
            {
                if (Type >= 64)
                {
                    throw new InvalidOperationException();
                }

                string s = "";
                for (uint i=0; i<PascalStringLength; i++)
                {
                    s = s + (char)m_bufTable.GetByte(m_offsetKindName+2+i);
                }
                return s;
            }

            public ushort Get2byteValue()
            {
                if (Type < 64 || Type > 127)
                {
                    throw new InvalidOperationException();
                }
                return m_bufTable.GetUshort(m_offsetKindName+1);
            }

            public uint GetLength()
            {
                if (Type < 64)
                {
                    return 2+(uint)PascalStringLength;
                }
                else if (Type < 128)
                {
                    return 3;
                }
                else
                {
                    throw new InvalidOperationException("illegal KindName.Type");
                }
            }

            // member data
            uint m_offsetKindName;
            MBOBuffer m_bufTable;
        }

        public class NamedGroup
        {
            public NamedGroup(uint offset, MBOBuffer bufTable)
            {
                m_offsetNamedGroup = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                nameIndex = 0,
                nGlyphs   = 2,
                glyphs    = 4
            }

            public uint GetLength()
            {
                return (uint)FieldOffsets.glyphs + (uint)nGlyphs*2;
            }

            // accessors
            public ushort nameIndex
            {
                get {return m_bufTable.GetUshort(m_offsetNamedGroup + (uint)FieldOffsets.nameIndex);}
            }

            public ushort nGlyphs
            {
                get {return m_bufTable.GetUshort(m_offsetNamedGroup + (uint)FieldOffsets.nGlyphs);}
            }

            public ushort GetGlyphIndex(uint i)
            {
                if (i >= nGlyphs)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return m_bufTable.GetUshort(m_offsetNamedGroup + (uint)FieldOffsets.glyphs + i*2);
            }

            // member data
            uint m_offsetNamedGroup;
            MBOBuffer m_bufTable;
        }

        public class GroupInfoGroup
        {
            public GroupInfoGroup(uint offset, MBOBuffer bufTable)
            {
                m_offsetGroupInfoGroup = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                nGroups      = 0,
                padding      = 2,
                groupOffsets = 4
            }

            public ushort nGroups
            {
                get {return m_bufTable.GetUshort(m_offsetGroupInfoGroup + (uint)FieldOffsets.nGroups);}
            }

            public ushort ActualNumberOfGroups
            {
                get {return (ushort)(nGroups & 0x3fff);}
            }

            public ushort padding
            {
                get {return m_bufTable.GetUshort(m_offsetGroupInfoGroup + (uint)FieldOffsets.padding);}
            }

            public uint GetGroupOffset(uint i)
            {
                if (i >= ActualNumberOfGroups)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return m_bufTable.GetUint(m_offsetGroupInfoGroup + (uint)FieldOffsets.groupOffsets + i*4);
            }

            // member data
            uint m_offsetGroupInfoGroup;
            MBOBuffer m_bufTable;
        }

        public class GroupInfo
        {
            public GroupInfo(uint offset, MBOBuffer bufTable)
            {
                m_offsetGroupInfo = offset;
                m_bufTable = bufTable;
            }

            public ushort nGroups
            {
                get {return m_bufTable.GetUshort(m_offsetGroupInfo);}
            }

            public ushort ActualNumberOfGroups
            {
                get {return (ushort)(nGroups & 0x3fff);}
            }

            public bool PrecededBy16BitFlag()
            {
                return ((nGroups & 0x8000) == 0x8000);
            }

            public ushort Get16BitFlag(uint i)
            {
                if (!PrecededBy16BitFlag())
                {
                    throw new InvalidOperationException("invalid attempt to fetch 16 Bit Flag");
                }

                // calculate the offset of the 16 bit flag
                uint offset = m_offsetGroupInfo + 2;
                for (uint j=0; j<i; j++)
                {
                    offset += 2;
                    NamedGroup ngTemp = new NamedGroup(offset, m_bufTable);
                    offset += ngTemp.GetLength();
                }

                return m_bufTable.GetUshort(offset);
            }

            public NamedGroup GetGroup(uint i)
            {
                if (i >= ActualNumberOfGroups)
                {
                    throw new ArgumentOutOfRangeException();
                }

                // calculate the offset of the NamedGroup
                uint offset = m_offsetGroupInfo + 2;
                for (uint j=0; j<i; j++)
                {
                    if (PrecededBy16BitFlag())
                    {
                        offset += 2;
                    }
                    NamedGroup ngTemp = new NamedGroup(offset, m_bufTable);
                    offset += ngTemp.GetLength();
                }
                if (PrecededBy16BitFlag())
                {
                    offset += 2;
                }

                return new NamedGroup(offset, m_bufTable);
            }

            // member data
            uint m_offsetGroupInfo;
            MBOBuffer m_bufTable;
        }

        public class FeatureInfo
        {
            public FeatureInfo(uint offset, MBOBuffer bufTable)
            {
                m_offsetFeatureInfo = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                context            = 0,
                nAATFeatures       = 2,
                sfntFontRunFeature = 4
            }

            // member data
            uint m_offsetFeatureInfo;
            MBOBuffer m_bufTable;
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            version   = 0,
            extraInfo = 4,
            offsets   = 8
        }


        /************************
         * property accessors
         */

        public OTFixed version
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.version);}
        }

        public uint extraInfo
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.extraInfo);}
        }

        public uint GetGlyphInfoOffset(uint iGlyph, OTFont fontOwner)
        {
            if (iGlyph >= fontOwner.GetMaxpNumGlyphs())
            {
                throw new ArgumentOutOfRangeException("iGlyph");
            }
            return m_bufTable.GetUint((uint)FieldOffsets.offsets + iGlyph*4);
        }

        public GlyphInfo GetGlyphInfo(uint iGlyph, OTFont fontOwner)
        {
            uint offset = GetGlyphInfoOffset(iGlyph, fontOwner);
            return new GlyphInfo(offset, m_bufTable);
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new Zapf_cache();
            }

            return m_cache;
        }
        
        public class Zapf_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        

    }
}
