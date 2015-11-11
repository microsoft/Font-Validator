using System;
using System.Diagnostics;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_head.
    /// </summary>
    public class Table_head : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_head(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            TableVersionNumber = 0,
            fontRevision       = 4,
            checkSumAdjustment = 8,
            magicNumber        = 12,
            flags              = 16,
            unitsPerEm         = 18,
            created            = 20,
            modified           = 28,
            xMin               = 36,
            yMin               = 38,
            xMax               = 40,
            yMax               = 42,
            macStyle           = 44,
            lowestRecPPEM      = 46,
            fontDirectionHint  = 48,
            indexToLocFormat   = 50,
            glyphDataFormat    = 52,
        }



        // head table checksum requires leaving out the checkSumAdjustment field
        public override uint CalcChecksum()
        {
            return m_bufTable.CalcChecksum() - checkSumAdjustment;
        }


        /************************
         * utility functions
         */

        public DateTime GetCreatedDateTime()
        {
            DateTime epoch = new DateTime(1904, 1, 1);
            return epoch.AddSeconds(created);
        }

        public DateTime GetModifiedDateTime()
        {
            DateTime epoch = new DateTime(1904, 1, 1);
            return epoch.AddSeconds(modified);
        }

        public long DateTimeToSecondsSince1904(DateTime dt)
        {
            DateTime epoch = new DateTime(1904, 1, 1);
            TimeSpan ts = dt.Subtract(epoch);
            return (long)ts.TotalSeconds;
        }

        /************************
         * property accessors
         */

        public OTFixed TableVersionNumber
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.TableVersionNumber);}
        }

        public OTFixed fontRevision
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.fontRevision);}
        }

        public uint checkSumAdjustment
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.checkSumAdjustment);}
        }

        public uint magicNumber
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.magicNumber);}
        }

        public ushort flags
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.flags);}
        }

        public ushort unitsPerEm
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.unitsPerEm);}
        }

        public long created
        {
            get {return m_bufTable.GetLong((uint)FieldOffsets.created);}
        }

        public long modified
        {
            get {return m_bufTable.GetLong((uint)FieldOffsets.modified);}
        }

        public short xMin
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.xMin);}
        }

        public short yMin
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.yMin);}
        }

        public short xMax
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.xMax);}
        }

        public short yMax
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.yMax);}
        }

        public ushort macStyle
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.macStyle);}
        }

        public ushort lowestRecPPEM
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.lowestRecPPEM);}
        }

        public short fontDirectionHint
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.fontDirectionHint);}
        }

        public short indexToLocFormat
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.indexToLocFormat);}
        }

        public short glyphDataFormat
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.glyphDataFormat);}
        }



        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new head_cache(this);
            }

            return m_cache;
        }
        
        public class head_cache : DataCache
        {
            // the cached data

            protected OTFixed m_TableVersion;
            protected OTFixed m_fontRevision;
            protected uint m_checkSumAdjustment;
            protected uint m_magicNumber;
            protected ushort m_flags;
            protected ushort m_unitsPerEm;
            protected long m_created;
            protected long m_modified;
            protected short m_xMin;
            protected short m_yMin;
            protected short m_xMax;
            protected short m_yMax;
            protected ushort m_macStyle;
            protected ushort m_lowestRecPPEM;
            protected short m_fontDirectionHint;
            protected short m_indexToLocFormat;
            protected short m_glyphDataFormat;


            // constructor

            public head_cache(Table_head OwnerTable)
            {
                // copy the data from the owner table's MBOBuffer
                // and store it in the cache variables
                m_TableVersion       = OwnerTable.TableVersionNumber;
                m_fontRevision       = OwnerTable.fontRevision;
                m_checkSumAdjustment = OwnerTable.checkSumAdjustment;
                m_magicNumber        = OwnerTable.magicNumber;
                m_flags              = OwnerTable.flags;
                m_unitsPerEm         = OwnerTable.unitsPerEm;
                m_created            = OwnerTable.created;
                m_modified           = OwnerTable.modified;
                m_xMin               = OwnerTable.xMin;
                m_yMin               = OwnerTable.yMin;
                m_xMax               = OwnerTable.xMax;
                m_yMax               = OwnerTable.yMax;
                m_macStyle           = OwnerTable.macStyle;
                m_lowestRecPPEM      = OwnerTable.lowestRecPPEM;
                m_fontDirectionHint  = OwnerTable.fontDirectionHint;
                m_indexToLocFormat   = OwnerTable.indexToLocFormat;
                m_glyphDataFormat    = OwnerTable.glyphDataFormat;
            }


            // accessors for the cached data

            public OTFixed TableVersion
            {
                get {return m_TableVersion;}
                set
                {
                    if (m_TableVersion != value)
                    {
                        m_TableVersion = value;
                        m_bDirty = true;
                    }
                }
            }

            public OTFixed fontRevision
            {
                get {return m_fontRevision;}
                set
                {
                    if (m_fontRevision != value)
                    {
                        m_fontRevision = value;
                        m_bDirty = true;
                    }
                }
            }

            public uint checkSumAdjustment
            {
                get {return m_checkSumAdjustment;}
                set
                {
                    m_checkSumAdjustment = value;
                    m_bDirty = true;
                }
            }

            public uint magicNumber
            {
                get {return m_magicNumber;}
                set
                {
                    m_magicNumber = value;
                    m_bDirty = true;
                }
            }

            public ushort flags
            {
                get {return m_flags;}
                set
                {
                    m_flags = value;
                    m_bDirty = true;
                }
            }

            public ushort unitsPerEm
            {
                get {return m_unitsPerEm;}
                set
                {
                    m_unitsPerEm = value;
                    m_bDirty = true;
                }
            }

            public long created
            {
                get {return m_created;}
                set
                {
                    m_created = value;
                    m_bDirty = true;
                }
            }

            public long modified
            {
                get {return m_modified;}
                set
                {
                    m_modified = value;
                    m_bDirty = true;
                }
            }

            public short xMin
            {
                get {return m_xMin;}
                set
                {
                    m_xMin = value;
                    m_bDirty = true;
                }
            }

            public short yMin
            {
                get {return m_yMin;}
                set
                {
                    m_yMin = value;
                    m_bDirty = true;
                }
            }

            public short xMax
            {
                get {return m_xMax;}
                set
                {
                    m_xMax = value;
                    m_bDirty = true;
                }
            }

            public short yMax
            {
                get {return m_yMax;}
                set
                {
                    m_yMax = value;
                    m_bDirty = true;
                }
            }

            public ushort macStyle
            {
                get {return m_macStyle;}
                set
                {
                    m_macStyle = value;
                    m_bDirty = true;
                }
            }

            public ushort lowestRecPPEM
            {
                get {return m_lowestRecPPEM;}
                set
                {
                    m_lowestRecPPEM = value;
                    m_bDirty = true;
                }
            }

            public short fontDirectionHint
            {
                get {return m_fontDirectionHint;}
                set
                {
                    m_fontDirectionHint = value;
                    m_bDirty = true;
                }
            }

            public short indexToLocFormat
            {
                get {return m_indexToLocFormat;}
                set
                {
                    m_indexToLocFormat = value;
                    m_bDirty = true;
                }
            }

            public short glyphDataFormat
            {
                get {return m_glyphDataFormat;}
                set
                {
                    m_glyphDataFormat = value;
                    m_bDirty = true;
                }
            }


            // generate a new table from the cached data

            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table

                MBOBuffer newbuf = new MBOBuffer(54);


                // populate the buffer
                
                newbuf.SetFixed (m_TableVersion,       (uint)Table_head.FieldOffsets.TableVersionNumber);
                newbuf.SetFixed (m_fontRevision,       (uint)Table_head.FieldOffsets.fontRevision);
                newbuf.SetUint  (m_checkSumAdjustment, (uint)Table_head.FieldOffsets.checkSumAdjustment);
                newbuf.SetUint  (m_magicNumber,        (uint)Table_head.FieldOffsets.magicNumber);
                newbuf.SetUshort(m_flags,              (uint)Table_head.FieldOffsets.flags);
                newbuf.SetUshort(m_unitsPerEm,         (uint)Table_head.FieldOffsets.unitsPerEm);
                newbuf.SetLong  (m_created,            (uint)Table_head.FieldOffsets.created);
                newbuf.SetLong  (m_modified,           (uint)Table_head.FieldOffsets.modified);
                newbuf.SetShort (m_xMin,               (uint)Table_head.FieldOffsets.xMin);
                newbuf.SetShort (m_yMin,               (uint)Table_head.FieldOffsets.yMin);
                newbuf.SetShort (m_xMax,               (uint)Table_head.FieldOffsets.xMax);
                newbuf.SetShort (m_yMax,               (uint)Table_head.FieldOffsets.yMax);
                newbuf.SetUshort(m_macStyle,           (uint)Table_head.FieldOffsets.macStyle);
                newbuf.SetUshort(m_lowestRecPPEM,      (uint)Table_head.FieldOffsets.lowestRecPPEM);
                newbuf.SetShort (m_fontDirectionHint,  (uint)Table_head.FieldOffsets.fontDirectionHint);
                newbuf.SetShort (m_indexToLocFormat,   (uint)Table_head.FieldOffsets.indexToLocFormat);
                newbuf.SetShort (m_glyphDataFormat,    (uint)Table_head.FieldOffsets.glyphDataFormat);


                // put the buffer into a Table_head object and return it

                Table_head headTable = new Table_head("head", newbuf);

                return headTable;
            }
        }
        

    }
}
