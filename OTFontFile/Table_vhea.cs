using System;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_vhea.
    /// </summary>
    public class Table_vhea : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_vhea(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            version              = 0,
            vertTypoAscender     = 4,
            vertTypoDescender    = 6,
            vertTypoLineGap      = 8,
            advanceHeightMax     = 10,
            minTopSideBearing    = 12,
            minBottomSideBearing = 14,
            yMaxExtent           = 16,
            caretSlopeRise       = 18,
            caretSlopeRun        = 20,
            caretOffset          = 22,
            reserved1            = 24,
            reserved2            = 26,
            reserved3            = 28,
            reserved4            = 30,
            metricDataFormat     = 32,
            numOfLongVerMetrics  = 34
        }


        /************************
         * accessors
         */
        
        
        public OTFixed version
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.version);}
        }

        public short vertTypoAscender
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.vertTypoAscender);}
        }

        public short vertTypoDescender
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.vertTypoDescender);}
        }

        public short vertTypoLineGap
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.vertTypoLineGap);}
        }

        public short advanceHeightMax
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.advanceHeightMax);}
        }

        public short minTopSideBearing
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.minTopSideBearing);}
        }

        public short minBottomSideBearing
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.minBottomSideBearing);}
        }

        public short yMaxExtent
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.yMaxExtent);}
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

        public ushort numOfLongVerMetrics
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.numOfLongVerMetrics);}
        }



        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new vhea_cache(this);
            }

            return m_cache;
        }
        
        public class vhea_cache : DataCache
        {
            // the cached data
            protected OTFixed m_version;
            protected short m_vertTypoAscender;
            protected short m_vertTypoDescender;
            protected short m_vertTypoLineGap;
            protected short m_advanceHeightMax;
            protected short m_minTopSideBearing;
            protected short m_minBottomSideBearing;
            protected short m_yMaxExtent;
            protected short m_caretSlopeRise;
            protected short m_caretSlopeRun;
            protected short m_caretOffset;
            protected short m_reserved1;
            protected short m_reserved2;
            protected short m_reserved3;
            protected short m_reserved4;
            protected short m_metricDataFormat;
            protected ushort m_numOfLongVerMetrics;

            // constructor
            public vhea_cache(Table_vhea OwnerTable)
            {
                // copy the data from the owner table's MBOBuffer
                // and store it in the cache variables
                m_version                = OwnerTable.version;
                m_vertTypoAscender        = OwnerTable.vertTypoAscender;
                m_vertTypoDescender        = OwnerTable.vertTypoDescender;
                m_vertTypoLineGap        = OwnerTable.vertTypoLineGap;
                m_advanceHeightMax        = OwnerTable.advanceHeightMax;
                m_minTopSideBearing        = OwnerTable.minTopSideBearing;
                m_minBottomSideBearing    = OwnerTable.minBottomSideBearing;
                m_yMaxExtent            = OwnerTable.yMaxExtent;
                m_caretSlopeRise        = OwnerTable.caretSlopeRise;
                m_caretSlopeRun            = OwnerTable.caretSlopeRun;
                m_caretOffset            = OwnerTable.caretOffset;
                m_reserved1             = OwnerTable.reserved1;
                m_reserved2                = OwnerTable.reserved2;
                m_reserved3                = OwnerTable.reserved3;
                m_reserved4                = OwnerTable.reserved4;
                m_metricDataFormat        = OwnerTable.metricDataFormat;
                m_numOfLongVerMetrics    = OwnerTable.numOfLongVerMetrics;
            }

            // accessors for the cached data

            public OTFixed version
            {
                get {return m_version;}
                set
                {
                    m_version = value;
                    m_bDirty = true;
                }
            }

            public short vertTypoAscender
            {
                get {return m_vertTypoAscender;}
                set
            {
                    m_vertTypoAscender = value;
                    m_bDirty = true;
                }
            }

            public short vertTypoDescender
            {
                get {return m_vertTypoDescender;}
                set
                {
                    m_vertTypoDescender = value;
                    m_bDirty = true;
                }
            }

            public short vertTypoLineGap
            {
                get {return m_vertTypoLineGap;}
                set
                {
                    m_vertTypoLineGap = value;
                    m_bDirty = true;
                }
            }

            public short advanceHeightMax
            {
                get {return m_advanceHeightMax;}
                set
                {
                    m_advanceHeightMax = value;
                    m_bDirty = true;
                }
            }

            public short minTopSideBearing
            {
                get {return m_minTopSideBearing;}
                set
                {
                    m_minTopSideBearing = value;
                    m_bDirty = true;
                }
            }

            public short minBottomSideBearing
            {
                get {return m_minBottomSideBearing;}
                set
                {
                    m_minBottomSideBearing = value;
                    m_bDirty = true;
                }
            }

            public short yMaxExtent
            {
                get {return m_yMaxExtent;}
                set
                {
                    m_yMaxExtent = value;
                    m_bDirty = true;
                }
            }

            public short caretSlopeRise
            {
                get {return m_caretSlopeRise;}
                set
                {
                    m_caretSlopeRise = value;
                    m_bDirty = true;
                }
            }

            public short caretSlopeRun
            {
                get {return m_caretSlopeRun;}
                set
                {
                    m_caretSlopeRun = value;
                    m_bDirty = true;
                }
            }

            public short caretOffset
            {
                get {return m_caretOffset;}
                set
                {
                    m_caretOffset = value;
                    m_bDirty = true;
                }
            }

            public short reserved1
            {
                get {return m_reserved1;}
                set
                {
                    m_reserved1 = value;
                    m_bDirty = true;
                }
            }

            public short reserved2
            {
                get {return m_reserved2;}
                set
                {
                    m_reserved2 = value;
                    m_bDirty = true;
                }
            }

            public short reserved3
            {
                get {return m_reserved3;}
                set
                {
                    m_reserved3 = value;
                    m_bDirty = true;
                }
            }

            public short reserved4
            {
                get {return m_reserved4;}
                set
                {
                    m_reserved4 = value;
                    m_bDirty = true;
                }
            }

            public short metricDataFormat
            {
                get {return m_metricDataFormat;}
                set
                {
                    m_metricDataFormat = value;
                    m_bDirty = true;
                }
            }

            public ushort numOfLongVerMetrics
            {
                get {return m_numOfLongVerMetrics;}
                set
                {
                    m_numOfLongVerMetrics = value;
                    m_bDirty = true;
                }
            }
        
            // generate a new table from the cached data
            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer(36);

                // populate the buffer                
                newbuf.SetFixed (m_version,                    (uint)Table_vhea.FieldOffsets.version);
                newbuf.SetShort (m_vertTypoAscender,        (uint)Table_vhea.FieldOffsets.vertTypoAscender);
                newbuf.SetShort (m_vertTypoDescender,        (uint)Table_vhea.FieldOffsets.vertTypoDescender);
                newbuf.SetShort (m_vertTypoLineGap,            (uint)Table_vhea.FieldOffsets.vertTypoLineGap);
                newbuf.SetShort (m_advanceHeightMax,        (uint)Table_vhea.FieldOffsets.advanceHeightMax);
                newbuf.SetShort (m_minTopSideBearing,        (uint)Table_vhea.FieldOffsets.minTopSideBearing);
                newbuf.SetShort (m_minBottomSideBearing,    (uint)Table_vhea.FieldOffsets.minBottomSideBearing);
                newbuf.SetShort (m_yMaxExtent,                (uint)Table_vhea.FieldOffsets.yMaxExtent);
                newbuf.SetShort (m_caretSlopeRise,            (uint)Table_vhea.FieldOffsets.caretSlopeRise);
                newbuf.SetShort (m_caretSlopeRun,            (uint)Table_vhea.FieldOffsets.caretSlopeRun);
                newbuf.SetShort (m_caretOffset,                (uint)Table_vhea.FieldOffsets.caretOffset);
                newbuf.SetShort (m_reserved1,               (uint)Table_vhea.FieldOffsets.reserved1);
                newbuf.SetShort (m_reserved2,                (uint)Table_vhea.FieldOffsets.reserved2);
                newbuf.SetShort (m_reserved3,                (uint)Table_vhea.FieldOffsets.reserved3);
                newbuf.SetShort (m_reserved4,                (uint)Table_vhea.FieldOffsets.reserved4);
                newbuf.SetShort (m_metricDataFormat,        (uint)Table_vhea.FieldOffsets.metricDataFormat);
                newbuf.SetUshort (m_numOfLongVerMetrics,    (uint)Table_vhea.FieldOffsets.numOfLongVerMetrics);


                // put the buffer into a Table_vhea object and return it
                Table_vhea vheaTable = new Table_vhea("vhea", newbuf);

                return vheaTable;
            }
        }
    }
}
