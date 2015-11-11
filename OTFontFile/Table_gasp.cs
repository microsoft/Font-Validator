using System;
using System.Collections;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_gasp.
    /// </summary>
    public class Table_gasp : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_gasp(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            version   = 0,
            numRanges = 2,
            gaspRange = 4
        }


        /************************
         * GaspRange class
         */

        public class GaspRange
        {
            public ushort rangeMaxPPEM;
            public ushort rangeGaspBehavior;

            public enum flags
            {
                GASP_GRIDFIT = 0x0001,
                GASP_DOGRAY  = 0x0002,
				GASP_SYMETRIC_GRIDFIT = 0x0004, // ART team using, not documented YDir?
				GASP_SYMETRIC_SMOOTHING = 0x0008  // ART team using, ???
            }
        }

        ushort GetFlagsForPPEM(ushort ppem)
        {
            ushort flags = 0;

            for (uint i=0; i<numRanges; i++)
            {
                GaspRange gr = GetGaspRange(i);

                if (ppem <= gr.rangeMaxPPEM)
                {
                    flags = gr.rangeGaspBehavior;
                    break;
                }
            }

            return flags;
        }


        /************************
         * accessors
         */

        public ushort version
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.version);}
        }

        public ushort numRanges
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.numRanges);}
        }

        public GaspRange GetGaspRange(uint i)
        {
            GaspRange gr = null;

            if (i < numRanges && (uint)FieldOffsets.gaspRange + (i+1)*4 <= m_bufTable.GetLength())
            {
                gr = new GaspRange();
                gr.rangeMaxPPEM      = m_bufTable.GetUshort((uint)FieldOffsets.gaspRange + i*4);
                gr.rangeGaspBehavior = m_bufTable.GetUshort((uint)FieldOffsets.gaspRange + i*4 + 2);
            }

            return gr;
        }



        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new gasp_cache(this);
            }

            return m_cache;
        }
        
        public class gasp_cache : DataCache
        {
            protected ushort m_version;
            protected ushort m_numRanges;
            protected  ArrayList m_GaspRange; // GaspRange[]            

            // constructor
            public gasp_cache(Table_gasp OwnerTable)
            {
                // assign
                m_version = OwnerTable.version;
                m_numRanges = OwnerTable.numRanges;

                // Store all of the gaspRanges
                m_GaspRange = new ArrayList( m_numRanges );
                for( ushort i = 0; i < m_numRanges; i++ )
                {
                    m_GaspRange.Add( OwnerTable.GetGaspRange( i ));                    
                }

            }

            // accessors
            public ushort version
            {
                get
                {
                    return m_version;
                }
                set
                {
                    m_version = value;
                    m_bDirty = true;
                }

            }

            // NOTE: Not letting the user set the num of ranges manualy. If they want to
            // change tis they can do it by adding or removing GaspRanges
            public ushort numRanges
            {
                get
                {
                    return m_numRanges;
                }
            }

            public GaspRange GetGaspRange( ushort nIndex )
            {
                GaspRange gr = null;

                if( nIndex < m_numRanges )
                {
                    gr = new GaspRange();
                    gr.rangeMaxPPEM = ((GaspRange)m_GaspRange[nIndex]).rangeMaxPPEM;
                    gr.rangeGaspBehavior= ((GaspRange)m_GaspRange[nIndex]).rangeGaspBehavior;
                }

                return gr;                
            }

            public bool setGaspRange( ushort nIndex, ushort nRangeMaxPPEM, ushort nRangeGaspBehavior )
            {
                bool bResult = true;

                // Check if in range
                if( nIndex > m_numRanges )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Tried to set a gaspRange that doesn't exist." );
                }    
                else
                {
                    ((GaspRange)m_GaspRange[nIndex]).rangeMaxPPEM = nRangeMaxPPEM;
                    ((GaspRange)m_GaspRange[nIndex]).rangeGaspBehavior = nRangeGaspBehavior;
                    m_bDirty = true;
                }

                return bResult;
            }

            public bool addGaspRange( ushort nRangeMaxPPEM, ushort nRangeGaspBehavior )
            {
                
                GaspRange gr = new GaspRange();
                gr.rangeMaxPPEM = nRangeMaxPPEM;
                gr.rangeGaspBehavior = nRangeGaspBehavior;                

                return addGaspRange( gr );
            }

            public bool addGaspRange( GaspRange gaspRange )
            {
                bool bResult = true;

                // We need to sort these on nRangeMaxPPEM so we we go through each until we find the insertion point
                for( ushort i = 0; i < m_numRanges; i++ )
                {
                    // This GaspRange is already there so we can't add it only set it
                    if( gaspRange.rangeMaxPPEM == ((GaspRange)m_GaspRange[i]).rangeMaxPPEM )
                    {
                        bResult = false;
                        break;
                    }
                    else if( gaspRange.rangeMaxPPEM > ((GaspRange)m_GaspRange[i]).rangeMaxPPEM )
                    {
                        // This could be the spot to insert we just need to make sure that
                        // the next one isn't equal
                        if( i == (m_numRanges - 1) || gaspRange.rangeMaxPPEM < ((GaspRange)m_GaspRange[i + 1]).rangeMaxPPEM )
                        {
                            // We found the insertion point
                            m_GaspRange.Insert( i + 1, gaspRange );
                            m_numRanges++;
                            m_bDirty = true;
                            break;
                        }
                        else
                        {
                            bResult = false;
                            break;
                        }
                    }
                }        

                return bResult;
            }

            public bool removeGaspRange( ushort nIndex )
            {
                bool bResult = true;

                // Check if in range
                if( nIndex > m_numRanges )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "Tried to set a gaspRange that doesn't exist." );
                }
                else
                {
                    m_GaspRange.RemoveAt( nIndex );
                    m_numRanges--;
                    m_bDirty = true;
                }

                return bResult;
            }

            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer( 4 + ((uint)m_numRanges * 4));

                newbuf.SetUshort( m_version,        (uint)Table_gasp.FieldOffsets.version );
                newbuf.SetUshort( m_numRanges,        (uint)Table_gasp.FieldOffsets.numRanges );

                for( ushort i = 0; i < m_numRanges; i++ )
                {
                    newbuf.SetUshort( ((GaspRange)m_GaspRange[i]).rangeMaxPPEM,        (uint)(Table_gasp.FieldOffsets.gaspRange + (i * 4)));
                    newbuf.SetUshort(((GaspRange)m_GaspRange[i]).rangeGaspBehavior,    (uint)(Table_gasp.FieldOffsets.gaspRange + (i * 4) + 2));        
                }                

                // put the buffer into a Table_vhmtx object and return it
                Table_gasp gaspTable = new Table_gasp( "gasp", newbuf );
            
                return gaspTable;
            }
        }
        

    }
}
