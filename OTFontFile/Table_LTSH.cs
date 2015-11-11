using System;
using System.Diagnostics;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_LTSH.
    /// </summary>
    public class Table_LTSH : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_LTSH(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            version   = 0,
            numGlyphs = 2,
            yPels     = 4
        }


        /************************
         * accessors
         */

        public ushort version
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.version);}
        }

        public ushort numGlyphs
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.numGlyphs);}
        }

        public byte GetYPel(uint iGlyph)
        {
            return m_bufTable.GetByte((uint)FieldOffsets.yPels + iGlyph);
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new LTSH_cache(this);
            }

            return m_cache;
        }

        public class LTSH_cache : DataCache
        {
            protected ushort m_version;
            protected ushort m_numGlyphs;
            protected byte[] m_yPels;

            // constructor
            public LTSH_cache(Table_LTSH OwnerTable)
            {
                m_version = OwnerTable.version;
                m_numGlyphs = OwnerTable.numGlyphs;

                m_yPels = new byte[m_numGlyphs];

                for( ushort i = 0; i < m_numGlyphs; i++ )
                {
                    m_yPels[i] = OwnerTable.GetYPel( i );
                }
            }

            public ushort version
            {
                get {return m_version;}
                set
                {
                    m_version = value;
                    m_bDirty = true;
                }
            }

            // NOTE: I am not sure we should allow this to be set here
            // this is already set when a yPel of a glyph is removed or added
            public ushort numGlyphs
            {
                get {return m_numGlyphs;}
                set
                {
                    m_numGlyphs = value;
                    m_bDirty = true;
                }
            }

            // Just as you would expect this returns a requested yPel
            public byte GetYPel( ushort nIndex )
            {    
                byte nYPel;

                if( nIndex < m_numGlyphs )
                {
                    nYPel = m_yPels[nIndex];
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Tried to access a yPel that did not exist.");                    
                }

                return nYPel;
            }

            // Sets a yPel
            public bool SetYPel( ushort nIndex, byte nYPel )
            {
                bool bResult = true;

                if( nIndex < m_numGlyphs )
                {
                    m_yPels[nIndex] = nYPel;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Tried to access a yPel that did not exist.");                    
                }

                return bResult;                
            }
            
            // NOTE: not sure if the application might need to add past numGlyphs + 1
            public bool addYPel( ushort nIndex, byte nYPel )
            {
                bool bResult = true;

                if( nIndex <= m_numGlyphs )
                {
                    // NOTE: Table maxp numGlyphs isn't being dynamically updated
                    m_numGlyphs++;

                    byte[] updatedYPels = new byte[m_numGlyphs];
                    for( ushort i = 0, n = 0; i < m_numGlyphs; i++, n++ )
                    {
                        if( i != nIndex )
                        {
                            updatedYPels[i]= m_yPels[n];
                        }
                        else
                        {
                            updatedYPels[i] = nYPel;
                            i++;
                        }
                    }

                    m_yPels = updatedYPels;
                    m_bDirty = true;
                }
                else
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException("Tried to add a yPel to a position greater than the number of Glyphs + 1.");
                    
                }

                return bResult;
            }

            // Pulls out a Vertical Matric and updates vhea if necessary
            public bool removeYPel( ushort nIndex )
            {
                bool bResult = true;

                if( nIndex < m_numGlyphs )
                {                    
                    // NOTE: Table maxp numGlyphs isn't being dynamically updated
                    m_numGlyphs--;

                    byte[] updatedYPels = new byte[m_numGlyphs];
                    for( ushort i = 0, n = 0; i < m_numGlyphs; i++, n++ )
                    {
                        if( n != nIndex )
                        {
                            updatedYPels[i] = m_yPels[n];
                        }
                        else
                        {
                            n++;
                        }
                    }

                    m_yPels = updatedYPels;
                    m_bDirty = true;
                }
                else
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException("Tried to remove a yPel that did not exist.");                    
                }

                return bResult;
            }

            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer((uint)(Table_LTSH.FieldOffsets.yPels + m_numGlyphs));

                newbuf.SetUshort( m_version,        (uint)Table_LTSH.FieldOffsets.version);
                newbuf.SetUshort( m_numGlyphs,        (uint)Table_LTSH.FieldOffsets.numGlyphs);
                
                // Fill the buffer with the yPels
                for( int i = 0; i < m_numGlyphs; i++ )
                {
                    newbuf.SetByte( m_yPels[i],        (uint)(Table_LTSH.FieldOffsets.yPels + i));
                }
                
                // put the buffer into a Table_maxp object and return it
                Table_LTSH LTSHTable = new Table_LTSH("LTSH", newbuf);

                return LTSHTable;        
            }
        }        

    }
}
