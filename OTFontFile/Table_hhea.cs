using System;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_hhea.
    /// </summary>
    public class Table_hhea : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_hhea(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            TableVersionNumber  = 0,
            Ascender            = 4,
            Descender           = 6,
            LineGap             = 8,
            advanceWidthMax     = 10,
            minLeftSideBearing  = 12,
            minRightSideBearing = 14,
            xMaxExtent          = 16,
            caretSlopeRise      = 18,
            caretSlopeRun       = 20,
            caretOffset         = 22,
            reserved1           = 24,
            reserved2           = 26,
            reserved3           = 28,
            reserved4           = 30,
            metricDataFormat    = 32,
            numberOfHMetrics    = 34
        }



        /************************
         * property accessors
         */

        public OTFixed TableVersionNumber
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.TableVersionNumber);}
        }

        public short Ascender
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.Ascender);}
        }

        public short Descender
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.Descender);}
        }

        public short LineGap
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.LineGap);}
        }

        public ushort advanceWidthMax
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.advanceWidthMax);}
        }

        public short minLeftSideBearing
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.minLeftSideBearing);}
        }

        public short minRightSideBearing
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.minRightSideBearing);}
        }

        public short xMaxExtent
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.xMaxExtent);}
        }

        public short caretSlopeRise
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.caretSlopeRise);}
        }

        public short caretSlopeRun
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.caretSlopeRun);}
        }

        public short caretOffset
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.caretOffset);}
        }

        public short reserved1
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.reserved1);}
        }

        public short reserved2
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.reserved2);}
        }

        public short reserved3
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.reserved3);}
        }

        public short reserved4
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.reserved4);}
        }

        public short metricDataFormat
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.metricDataFormat);}
        }

        public ushort numberOfHMetrics
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.numberOfHMetrics);}
        }



        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new hhea_cache(this);
            }

            return m_cache;
        }
        
        public class hhea_cache : DataCache
        {
            protected OTFixed m_TableVersionNumber;
            protected short m_Ascender;
            protected short m_Descender;
            protected short m_LineGap;
            protected ushort m_advanceWidthMax;
            protected short m_minLeftSideBearing;
            protected short m_minRightSideBearing;
            protected short m_xMaxExtent;
            protected short m_caretSlopeRise;
            protected short m_caretSlopeRun;
            protected short m_caretOffset;
            protected short m_reserved1;
            protected short m_reserved2;
            protected short m_reserved3;
            protected short m_reserved4;
            protected short m_metricDataFormat;
            protected ushort m_numberOfHMetrics;

            // constructor
            public hhea_cache(Table_hhea OwnerTable)
            {
                // copy the data from the owner table's MBOBuffer
                // and store it in the cache variables
                m_TableVersionNumber    = OwnerTable.TableVersionNumber;
                m_Ascender                = OwnerTable.Ascender;
                m_Descender                = OwnerTable.Descender;
                m_LineGap                = OwnerTable.LineGap;
                m_advanceWidthMax        = OwnerTable.advanceWidthMax;
                m_minLeftSideBearing    = OwnerTable.minLeftSideBearing;
                m_minRightSideBearing    = OwnerTable.minRightSideBearing;
                m_xMaxExtent            = OwnerTable.xMaxExtent;
                m_caretSlopeRise        = OwnerTable.caretSlopeRise;
                m_caretSlopeRun            = OwnerTable.caretSlopeRun;
                m_caretOffset            = OwnerTable.caretOffset;
                m_reserved1             = OwnerTable.reserved1;
                m_reserved2                = OwnerTable.reserved2;
                m_reserved3                = OwnerTable.reserved3;
                m_reserved4                = OwnerTable.reserved4;
                m_metricDataFormat        = OwnerTable.metricDataFormat;
                m_numberOfHMetrics        = OwnerTable.numberOfHMetrics;
            }

            // accessors for the cached data

            public OTFixed TableVersionNumber
            {
                get {return m_TableVersionNumber;}
                set
                {
                    if (m_TableVersionNumber != value)
                    {
                        m_TableVersionNumber = value;
                        m_bDirty = true;
                    }
                }
            }

            public short Ascender
            {
                get {return m_Ascender;}
                set
                {
                    if (m_Ascender != value)
                    {
                        m_Ascender = value;
                        m_bDirty = true;
                    }
                }
            }


            public short Descender
            {
                get {return m_Descender;}
                set
                {
                    if (m_Descender != value)
                    {
                        m_Descender = value;
                        m_bDirty = true;
                    }
                }
            }

            public short LineGap
            {
                get {return m_LineGap;}
                set
                {
                    if (m_LineGap != value)
                    {
                        m_LineGap = value;
                        m_bDirty = true;
                    }
                }
            }

            public ushort advanceWidthMax
            {
                get {return m_advanceWidthMax;}
                set
                {
                    if (m_advanceWidthMax != value)
                    {
                        m_advanceWidthMax = value;
                        m_bDirty = true;
                    }
                }
            }

            public short minLeftSideBearing
            {
                get {return m_minLeftSideBearing;}
                set
                {
                    if (m_minLeftSideBearing != value)
                    {
                        m_minLeftSideBearing = value;
                        m_bDirty = true;
                    }
                }
            }

            public short minRightSideBearing
            {
                get {return m_minRightSideBearing;}
                set
                {
                    if (m_minRightSideBearing != value)
                    {
                        m_minRightSideBearing = value;
                        m_bDirty = true;
                    }
                }
            }

            public short xMaxExtent
            {
                get {return m_xMaxExtent;}
                set
                {
                    if (m_xMaxExtent != value)
                    {
                        m_xMaxExtent = value;
                        m_bDirty = true;
                    }
                }
            }

            public short caretSlopeRise
            {
                get {return m_caretSlopeRise;}
                set
                {
                    if (m_caretSlopeRise != value)
                    {
                        m_caretSlopeRise = value;
                        m_bDirty = true;
                    }
                }
            }

            public short caretSlopeRun
            {
                get {return m_caretSlopeRun;}
                set
                {
                    if (m_caretSlopeRun != value)
                    {
                        m_caretSlopeRun = value;
                        m_bDirty = true;
                    }
                }
            }

            public short caretOffset
            {
                get {return m_caretOffset;}
                set
                {
                    if (m_caretOffset != value)
                    {
                        m_caretOffset = value;
                        m_bDirty = true;
                    }
                }
            }

            public short reserved1
            {
                get {return m_reserved1;}
                set
                {
                    if (m_reserved1 != value)
                    {
                        m_reserved1 = value;
                        m_bDirty = true;
                    }
                }
            }

            public short reserved2
            {
                get {return m_reserved2;}
                set
                {
                    if (m_reserved2 != value)
                    {
                        m_reserved2 = value;
                        m_bDirty = true;
                    }
                }
            }

            public short reserved3
            {
                get {return m_reserved3;}
                set
                {
                    if (m_reserved3 != value)
                    {
                        m_reserved3 = value;
                        m_bDirty = true;
                    }
                }
            }

            public short reserved4
            {
                get {return m_reserved4;}
                set
                {
                    if (m_reserved4 != value)
                    {
                        m_reserved4 = value;
                        m_bDirty = true;
                    }
                }
            }

            public short metricDataFormat
            {
                get {return m_metricDataFormat;}
                set
                {
                    if (m_metricDataFormat != value)
                    {
                        m_metricDataFormat = value;
                        m_bDirty = true;
                    }
                }
            }

            public ushort numberOfHMetrics
            {
                get {return m_numberOfHMetrics;}
                set
                {
                    if (m_numberOfHMetrics != value)
                    {
                        m_numberOfHMetrics = value;
                        m_bDirty = true;
                    }
                }
            }
            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer(36);

                // populate the buffer                
                newbuf.SetFixed( m_TableVersionNumber,        (uint)Table_hhea.FieldOffsets.TableVersionNumber );
                newbuf.SetShort( m_Ascender,                (uint)Table_hhea.FieldOffsets.Ascender );
                newbuf.SetShort( m_Descender,                (uint)Table_hhea.FieldOffsets.Descender );
                newbuf.SetShort( m_LineGap,                    (uint)Table_hhea.FieldOffsets.LineGap );
                newbuf.SetUshort( m_advanceWidthMax,        (uint)Table_hhea.FieldOffsets.advanceWidthMax );
                newbuf.SetShort( m_minLeftSideBearing,        (uint)Table_hhea.FieldOffsets.minLeftSideBearing );
                newbuf.SetShort( m_minRightSideBearing,        (uint)Table_hhea.FieldOffsets.minRightSideBearing );
                newbuf.SetShort( m_xMaxExtent,                (uint)Table_hhea.FieldOffsets.xMaxExtent );
                newbuf.SetShort( m_caretSlopeRise,            (uint)Table_hhea.FieldOffsets.caretSlopeRise );
                newbuf.SetShort( m_caretSlopeRun,            (uint)Table_hhea.FieldOffsets.caretSlopeRun );
                newbuf.SetShort( m_caretOffset,                (uint)Table_hhea.FieldOffsets.caretOffset );
                newbuf.SetShort( m_reserved1,               (uint)Table_hhea.FieldOffsets.reserved1 );
                newbuf.SetShort( m_reserved2,                (uint)Table_hhea.FieldOffsets.reserved2 );
                newbuf.SetShort( m_reserved3,                (uint)Table_hhea.FieldOffsets.reserved3 );
                newbuf.SetShort( m_reserved4,                (uint)Table_hhea.FieldOffsets.reserved4 );
                newbuf.SetShort( m_metricDataFormat,        (uint)Table_hhea.FieldOffsets.metricDataFormat );
                newbuf.SetUshort( m_numberOfHMetrics,        (uint)Table_hhea.FieldOffsets.numberOfHMetrics );


                // put the buffer into a Table_vhea object and return it
                Table_hhea hheaTable = new Table_hhea("hhea", newbuf);

                return hheaTable;
            }
        }
        

    }
}
