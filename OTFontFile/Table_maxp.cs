using System;


namespace OTFontFile
{
    public class Table_maxp : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_maxp(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            TableVersionNumber    = 0, // TableVersionNumber available in version 0x00010000 AND 0x00005000
            NumGlyphs             = 4, // numglyphs available in version 0x00010000 AND 0x00005000
            maxPoints             = 6, // fields from here down only in version 0x00010000
            maxContours           = 8,
            maxCompositePoints    = 10,
            maxCompositeContours  = 12,
            maxZones              = 14,
            maxTwilightPoints     = 16,
            maxStorage            = 18,
            maxFunctionDefs       = 20,
            maxInstructionDefs    = 22,
            maxStackElements      = 24,
            maxSizeOfInstructions = 26,
            maxComponentElements  = 28,
            maxComponentDepth     = 30
        }



        /************************
         * accessors
         */
        
        public OTFixed TableVersionNumber
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.TableVersionNumber);}
        }

        public ushort NumGlyphs
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.NumGlyphs);}
        }

        public ushort maxPoints
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxPoints);
            }
        }

        public ushort maxContours
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxContours);
            }
        }

        public ushort maxCompositePoints
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxCompositePoints);
            }
        }

        public ushort maxCompositeContours
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxCompositeContours);
            }
        }

        public ushort maxZones
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxZones);
            }
        }

        public ushort maxTwilightPoints
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxTwilightPoints);
            }
        }

        public ushort maxStorage
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxStorage);
            }
        }

        public ushort maxFunctionDefs
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxFunctionDefs);
            }
        }

        public ushort maxInstructionDefs
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxInstructionDefs);
            }
        }

        public ushort maxStackElements
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxStackElements);
            }
        }

        public ushort maxSizeOfInstructions
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxSizeOfInstructions);
            }
        }

        public ushort maxComponentElements
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxComponentElements);
            }
        }

        public ushort maxComponentDepth
        {
            get
            {
                if (TableVersionNumber.GetUint() != 0x00010000)
                {
                    throw new System.InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsets.maxComponentDepth);
            }
        }



        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new maxp_cache(this);
            }

            return m_cache;
        }
        
        public class maxp_cache : DataCache
        {
            // the cached data
            protected OTFixed m_TableVersionNumber;   
            protected ushort m_NumGlyphs;             
            protected ushort m_maxPoints;             
            protected ushort m_maxContours;           
            protected ushort m_maxCompositePoints;    
            protected ushort m_maxCompositeContours;  
            protected ushort m_maxZones;              
            protected ushort m_maxTwilightPoints;     
            protected ushort m_maxStorage;            
            protected ushort m_maxFunctionDefs;       
            protected ushort m_maxInstructionDefs;    
            protected ushort m_maxStackElements;      
            protected ushort m_maxSizeOfInstructions; 
            protected ushort m_maxComponentElements;  
            protected ushort m_maxComponentDepth;        

            // constructor
            public maxp_cache(Table_maxp OwnerTable)
            {
                // copy the data from the owner table's MBOBuffer
                // and store it in the cache variables
                m_TableVersionNumber    = OwnerTable.TableVersionNumber;
                m_NumGlyphs                = OwnerTable.NumGlyphs;
                if( m_TableVersionNumber.GetUint() == 0x00010000 )
                {
                    m_maxPoints                = OwnerTable.maxPoints;
                    m_maxContours            = OwnerTable.maxContours;
                    m_maxCompositePoints    = OwnerTable.maxCompositePoints;
                    m_maxCompositeContours    = OwnerTable.maxCompositeContours;
                    m_maxZones                = OwnerTable.maxZones;
                    m_maxTwilightPoints        = OwnerTable.maxTwilightPoints;
                    m_maxStorage            = OwnerTable.maxStorage;
                    m_maxFunctionDefs        = OwnerTable.maxFunctionDefs;
                    m_maxInstructionDefs    = OwnerTable.maxInstructionDefs;
                    m_maxStackElements        = OwnerTable.maxStackElements;
                    m_maxSizeOfInstructions    = OwnerTable.maxSizeOfInstructions;
                    m_maxComponentElements    = OwnerTable.maxComponentElements;
                    m_maxComponentDepth        = OwnerTable.maxComponentDepth;                    
                }
            }


            // accessors for the cached data

            public OTFixed TableVersionNumber
            {
                get {return m_TableVersionNumber;}
                set
                {
                    m_TableVersionNumber = value;
                    m_bDirty = true;
                }
            }

            public ushort NumGlyphs
            {
                get {return m_NumGlyphs;}
                set
                {
                    m_NumGlyphs = value;
                    m_bDirty = true;
                }
            }

            public ushort maxPoints
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxPoints;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxPoints = value;
                    m_bDirty = true;
                }
            }

            public ushort maxContours
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxContours;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxContours = value;
                    m_bDirty = true;
                }
            }

            public ushort maxCompositePoints
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxCompositePoints;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxCompositePoints = value;
                    m_bDirty = true;
                }
            }

            public ushort maxCompositeContours
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxCompositeContours;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxCompositeContours = value;
                    m_bDirty = true;
                }
            }

            public ushort maxZones
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxZones;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxZones = value;
                    m_bDirty = true;
                }
            }

            public ushort maxTwilightPoints
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxTwilightPoints;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxTwilightPoints = value;
                    m_bDirty = true;
                }
            }

            public ushort maxStorage
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxStorage;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxStorage = value;
                    m_bDirty = true;
                }
            }

            public ushort maxFunctionDefs
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxFunctionDefs;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxFunctionDefs = value;
                    m_bDirty = true;
                }
            }

            public ushort maxInstructionDefs
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxInstructionDefs;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxInstructionDefs = value;
                    m_bDirty = true;
                }
            }

            public ushort maxStackElements
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxStackElements;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxStackElements = value;
                    m_bDirty = true;
                }
            }

            public ushort maxSizeOfInstructions
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxSizeOfInstructions;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxSizeOfInstructions = value;
                    m_bDirty = true;
                }
            }

            public ushort maxComponentElements
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxComponentElements;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxComponentElements = value;
                    m_bDirty = true;
                }
            }

            public ushort maxComponentDepth
            {                
                get
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_maxComponentDepth;
                }
                set
                {
                    if( m_TableVersionNumber.GetUint() != 0x00010000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    m_maxComponentDepth = value;
                    m_bDirty = true;
                }
            }

            // generate a new table from the cached data

            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf;
                
                if( m_TableVersionNumber.GetUint() == 0x00000000 )
                {
                    newbuf = new MBOBuffer(6);

                    newbuf.SetFixed (m_TableVersionNumber,    (uint)Table_maxp.FieldOffsets.TableVersionNumber);
                    newbuf.SetUshort (m_NumGlyphs,            (uint)Table_maxp.FieldOffsets.NumGlyphs);
                }
                else
                {
                    newbuf = new MBOBuffer(32);

                    newbuf.SetFixed (m_TableVersionNumber,        (uint)Table_maxp.FieldOffsets.TableVersionNumber);
                    newbuf.SetUshort (m_NumGlyphs,                (uint)Table_maxp.FieldOffsets.NumGlyphs);
                    newbuf.SetUshort (m_maxPoints,                (uint)Table_maxp.FieldOffsets.maxPoints);
                    newbuf.SetUshort (m_maxContours,            (uint)Table_maxp.FieldOffsets.maxContours);
                    newbuf.SetUshort (m_maxCompositePoints,        (uint)Table_maxp.FieldOffsets.maxCompositePoints);
                    newbuf.SetUshort (m_maxCompositeContours,    (uint)Table_maxp.FieldOffsets.maxCompositeContours);
                    newbuf.SetUshort (m_maxZones,                (uint)Table_maxp.FieldOffsets.maxZones);
                    newbuf.SetUshort (m_maxTwilightPoints,        (uint)Table_maxp.FieldOffsets.maxTwilightPoints);
                    newbuf.SetUshort (m_maxStorage,                (uint)Table_maxp.FieldOffsets.maxStorage);
                    newbuf.SetUshort (m_maxFunctionDefs,        (uint)Table_maxp.FieldOffsets.maxFunctionDefs);
                    newbuf.SetUshort (m_maxInstructionDefs,        (uint)Table_maxp.FieldOffsets.maxInstructionDefs);
                    newbuf.SetUshort (m_maxStackElements,        (uint)Table_maxp.FieldOffsets.maxStackElements);
                    newbuf.SetUshort (m_maxSizeOfInstructions,    (uint)Table_maxp.FieldOffsets.maxSizeOfInstructions);
                    newbuf.SetUshort (m_maxComponentElements,    (uint)Table_maxp.FieldOffsets.maxComponentElements);
                    newbuf.SetUshort (m_maxComponentDepth,        (uint)Table_maxp.FieldOffsets.maxComponentDepth);
                }
                
                // put the buffer into a Table_maxp object and return it
                Table_maxp maxpTable = new Table_maxp("maxp", newbuf);

                return maxpTable;            
            }
        }
    }
}
