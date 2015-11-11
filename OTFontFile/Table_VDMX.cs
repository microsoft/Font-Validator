using System;
using System.Collections;
using System.Diagnostics;



namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_VDMX.
    /// </summary>
    public class Table_VDMX : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_VDMX(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            version   = 0,
            numRecs   = 2,
            numRatios = 4,
            ratRange  = 6
        }


        /************************
         * classes
         */

        public class Ratios
        {
            public Ratios(ushort offset, MBOBuffer bufTable)
            {
                m_offsetRatios = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                bCharSet    = 0,
                xRatio      = 1,
                yStartRatio = 2,
                yEndRatio   = 3
            }

            public byte bCharSet
            {
                get {return m_bufTable.GetByte(m_offsetRatios + (uint)FieldOffsets.bCharSet);}
            }

            public byte xRatio
            {
                get {return m_bufTable.GetByte(m_offsetRatios + (uint)FieldOffsets.xRatio);}
            }

            public byte yStartRatio
            {
                get {return m_bufTable.GetByte(m_offsetRatios + (uint)FieldOffsets.yStartRatio);}
            }

            public byte yEndRatio
            {
                get {return m_bufTable.GetByte(m_offsetRatios + (uint)FieldOffsets.yEndRatio);}
            }

            ushort m_offsetRatios;
            MBOBuffer m_bufTable;
        }

        public class Vdmx
        {
            public Vdmx(ushort offset, MBOBuffer bufTable)
            {
                m_offsetVdmx = offset;
                m_bufTable = bufTable;
            }

            public enum FieldOffsets
            {
                recs    = 0,
                startsz = 2,
                endsz   = 3,
                vTable  = 4
            }

            public ushort recs
            {
                get {return m_bufTable.GetUshort(m_offsetVdmx + (uint)FieldOffsets.recs);}
            }

            public byte   startsz
            {
                get {return m_bufTable.GetByte(m_offsetVdmx + (uint)FieldOffsets.startsz);}
            }

            public byte   endsz
            {
                get {return m_bufTable.GetByte(m_offsetVdmx + (uint)FieldOffsets.endsz);}
            }

            public vTable GetEntry(uint i)
            {
                if (i >= recs)
                {
                    throw new ArgumentOutOfRangeException();
                }

                uint offset = m_offsetVdmx + (uint)FieldOffsets.vTable + i*6;
                return new vTable((ushort)offset, m_bufTable);
            }

            public class vTable
            {
                public vTable(ushort offset, MBOBuffer bufTable)
                {
                    m_offsetvTable = offset;
                    m_bufTable = bufTable;
                }

                public enum FieldOffsets
                {
                    yPelHeight = 0,
                    yMax       = 2,
                    yMin       = 4
                }

                public ushort yPelHeight
                {
                    get {return m_bufTable.GetUshort(m_offsetvTable + (uint)FieldOffsets.yPelHeight);}
                }

                public short  yMax
                {
                    get {return m_bufTable.GetShort(m_offsetvTable + (uint)FieldOffsets.yMax);}
                }

                public short  yMin
                {
                    get {return m_bufTable.GetShort(m_offsetvTable + (uint)FieldOffsets.yMin);}
                }

                ushort m_offsetvTable;
                MBOBuffer m_bufTable;
            }

            ushort m_offsetVdmx;
            MBOBuffer m_bufTable;
        }


        /************************
         * accessors
         */

        public ushort version
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.version);}
        }

        public ushort numRecs
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.numRecs);}
        }

        public ushort numRatios
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.numRatios);}
        }

        public Ratios GetRatioRange(uint i)
        {
            if (i >= numRatios)
            {
                throw new ArgumentOutOfRangeException();
            }

            uint offset = (uint)FieldOffsets.ratRange + i*4;

            return new Ratios((ushort)offset, m_bufTable);
        }

        public ushort GetVdmxGroupOffset(uint i)
        {
            ushort startOfOffsets = (ushort)((ushort)FieldOffsets.ratRange + numRatios * 4);
            return m_bufTable.GetUshort(startOfOffsets + i*2);
        }

        public Vdmx GetVdmxGroup(uint i)
        {
            if (i >= numRatios)
            {
                throw new ArgumentOutOfRangeException();
            }

            uint VdmxOffset = GetVdmxGroupOffset(i);

            return new Vdmx((ushort)VdmxOffset, m_bufTable);
        }



        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new VDMX_cache( this );
            }

            return m_cache;
        }
        
        public class VDMX_cache : DataCache
        {
            // the cached data
            protected ushort m_version;
            protected ushort m_numRecs;
            protected ushort m_numRatios;
            // No need to store group offsets because we can determine these when we save the cache
            protected ArrayList m_ratRange; // RatioCache[]
            protected ArrayList m_groups; // VDMXGroupCache[]

            // constructor
            public VDMX_cache(Table_VDMX OwnerTable)
            {

                m_version = OwnerTable.version;
                m_numRecs = OwnerTable.numRecs;
                m_numRatios = OwnerTable.numRatios;

                m_ratRange = new ArrayList( m_numRatios );
                m_groups = new ArrayList( m_numRecs );

                // Used to detrmine which vdmx goes with which ratio
                ushort[] VDMXOffsets = new ushort[m_numRecs]; 

                
                VDMXOffsets[0] = (ushort)(Table_VDMX.FieldOffsets.ratRange + (m_numRatios * 4) + (m_numRatios * 2));

                // Fill in the VDMX groups
                for( ushort i = 0; i < m_numRecs; i++ )
                {

                    VDMXGroupCache vgc = new VDMXGroupCache();
                    vgc.recs = OwnerTable.GetVdmxGroup( i ).recs;
                    vgc.startsz = OwnerTable.GetVdmxGroup( i ).startsz;
                    vgc.endsz = OwnerTable.GetVdmxGroup( i ).endsz;

                    for( ushort ii = 0; ii < vgc.recs; ii++ )
                    {
                        Vdmx.vTable vt = OwnerTable.GetVdmxGroup( i ).GetEntry( ii );
                        vgc.addVTableRecordCache( ii, vt.yPelHeight, vt.yMax, vt. yMin ); 
                    }

                    m_groups.Add( vgc );

                    if( i < (m_numRecs - 1))
                    {
                        VDMXOffsets[i+1] = (ushort)(VDMXOffsets[i] + 4 + ( vgc.recs * 6 ));
                    }

                }

                // Fill in the ratios
                for( ushort i = 0; i < m_numRatios; i++ )
                {
                    RatioCache rc = new RatioCache();
                    rc.bCharSet = OwnerTable.GetRatioRange( i ).bCharSet;
                    rc.xRatio = OwnerTable.GetRatioRange( i ).xRatio;
                    rc.yStartRatio = OwnerTable.GetRatioRange( i ).yStartRatio;
                    rc.yEndRatio = OwnerTable.GetRatioRange( i ).yEndRatio;

                    // Go through each of the offsets I saved above and match with this ratios offset
                    // When we find the right one save this group to this ratio
                    // later when we save the buffer we will redetermine these offsets
                    for( ushort ii = 0; ii < m_numRatios; ii++ )
                    {
                        if( VDMXOffsets[ii] == OwnerTable.GetVdmxGroupOffset( i ))
                        {
                            rc.VDMXGroupThisRatio = ii;
                            break;
                        }
                    }

                    m_ratRange.Add( rc );    
                }
            }

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

            // This can be set by adding or removing VDMXGroups
            public ushort numRecs
            {
                get
                {
                    return m_numRecs;
                }
                
            }

            // This can be set by adding or removing Ratios
            public ushort numRatios
            {
                get
                {
                    return m_numRatios;
                }
            }
            
            public RatioCache getRatioRecord( ushort nIndex )
            {
                RatioCache rc = null;

                if( nIndex >= m_numRatios )
                {
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the number of Ratio Records." );
                }
                else
                {
                    rc = (RatioCache)((RatioCache)m_ratRange[nIndex]).Clone();
                }
                
                return rc;
            }

            public bool setRatioRecord( ushort nIndex, byte bCharSet, byte xRatio, byte yStartRatio, byte yEndRatio, ushort VDMXGroupThisRatio )
            {
                bool bResult = true;
                
                if( nIndex >= m_numRatios )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the number of Ratio Records." );
                }
                else if( VDMXGroupThisRatio >= m_numRecs )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "VDMXGroupThisRatio is greater than the number of VDMX Groups." );
                }
                else
                {
                    ((RatioCache)m_ratRange[nIndex]).bCharSet = bCharSet;
                    ((RatioCache)m_ratRange[nIndex]).xRatio = xRatio;
                    ((RatioCache)m_ratRange[nIndex]).yStartRatio = yStartRatio;
                    ((RatioCache)m_ratRange[nIndex]).yEndRatio = yEndRatio;
                    ((RatioCache)m_ratRange[nIndex]).VDMXGroupThisRatio = VDMXGroupThisRatio;
                    m_bDirty = true;
                }            

                return bResult;
            }

            // NOTE: I am not sure if we should allow the nIndex since this can be determined by spec.
            public bool addRatioRecord( ushort nIndex, byte bCharSet, byte xRatio, byte yStartRatio, byte yEndRatio, ushort VDMXGroupThisRatio )
            {
                bool bResult = true;
                
                if( nIndex > m_numRatios )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the number of Ratio Records + 1." );
                }
                else if( VDMXGroupThisRatio >= m_numRecs )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "VDMXGroupThisRatio is greater than the number of VDMX Groups." );
                }
                else
                {
                    RatioCache rc = new RatioCache( bCharSet, xRatio, yStartRatio, yEndRatio, VDMXGroupThisRatio );

                    m_ratRange.Insert( nIndex, rc );
                    m_numRatios++;
                    m_bDirty = true;
                }            

                return bResult;

            }

            public bool removeRatioRecord( ushort nIndex )
            {
                bool bResult = true;

                if( nIndex >= m_numRatios )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the number of Ratio Records." );
                }
                else
                {
                    m_ratRange.RemoveAt( nIndex );
                    m_numRatios--;
                    m_bDirty = true;
                }

                return bResult;
            }
            
            public VDMXGroupCache getVDMXGroup( ushort nIndex )
            {
                VDMXGroupCache vgc = null;

                
                if( nIndex >= m_numRecs )
                {
                
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the number of VDMX Groups." );
                }
                else
                {
                    vgc = (VDMXGroupCache)((VDMXGroupCache)m_groups[nIndex]).Clone();
                }

                return vgc;
            }
            
            public bool setVDMXGroup( ushort nIndex, VDMXGroupCache vgc )
            {
                bool bResult = true;
                
                if( nIndex >= m_numRecs )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the number of VDMX Groups." );
                }
                else
                {
                    m_groups[nIndex] = vgc.Clone();
                    m_bDirty = true;
                }

                return bResult;
            }

            public bool addVDMXGroup( ushort nIndex, VDMXGroupCache vgc )
            {
                bool bResult = true;
                
                if( nIndex > m_numRecs )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the number of VDMX Groups + 1." );
                }
                else
                {
                    m_groups.Insert( nIndex, vgc.Clone());
                    m_numRecs++;
                    m_bDirty = true;

                    // Go fix up all of the ratio records
                    for( int i = 0; i < m_numRatios; i++ )
                    {
                        if( ((RatioCache)m_ratRange[i]).VDMXGroupThisRatio >= nIndex )
                        {
                            ((RatioCache)m_ratRange[i]).VDMXGroupThisRatio++;
                        }
                    }
                }

                return bResult;
            }

            public bool removeVDMXGroup( ushort nIndex, VDMXGroupCache vgc )
            {
                bool bResult = true;
                
                if( nIndex >= m_numRecs )
                {
                    bResult = false;
                    throw new ArgumentOutOfRangeException( "nIndex is greater than the number of VDMX Groups." );
                }
                else
                {
                    for( int i = 0; i < m_numRatios; i++ )
                    {
                        if( ((RatioCache)m_ratRange[i]).VDMXGroupThisRatio == nIndex )
                        {
                            bResult = false;
                            throw new ArgumentException( "VDMX Group can not be removed because a Ratio Record is using it" );
                        }
                    }

                    m_groups.RemoveAt( nIndex );
                    m_numRecs--;
                    m_bDirty = true;
                    
                    // Go fix up all of the ratio records
                    for( int i = 0; i < m_numRatios; i++ )
                    {
                        if( ((RatioCache)m_ratRange[i]).VDMXGroupThisRatio > nIndex )
                        {
                            ((RatioCache)m_ratRange[i]).VDMXGroupThisRatio--;
                        }
                    }
                }

                return bResult;
            }

            public override OTTable GenerateTable()
            {
                ushort iSizeOfVDMXGroups = 0;
                // Used to detrmine which vdmx goes with which ratio
                ushort[] VDMXOffsets = new ushort[m_numRecs]; 

                for( ushort i = 0; i < m_numRecs; i++ )
                {
                    iSizeOfVDMXGroups += (ushort)(4 + (((VDMXGroupCache)m_groups[i]).recs * 6));
                }

                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer( (uint)(6 + (m_numRatios * 4) + (2* m_numRatios) + iSizeOfVDMXGroups));

                // populate the buffer                
                newbuf.SetUshort( m_version,                (uint)Table_VDMX.FieldOffsets.version );
                newbuf.SetUshort( m_numRecs,                (uint)Table_VDMX.FieldOffsets.numRecs );
                newbuf.SetUshort( m_numRatios,                (uint)Table_VDMX.FieldOffsets.numRatios );

                // populate buffer with Ratio Records
                for( ushort i = 0; i < m_numRatios; i++ )
                {
                    newbuf.SetByte( ((RatioCache)m_ratRange[i]).bCharSet,        (uint)(Table_VDMX.FieldOffsets.ratRange + (i * 4)));
                    newbuf.SetByte( ((RatioCache)m_ratRange[i]).xRatio,            (uint)(Table_VDMX.FieldOffsets.ratRange + (i * 4) + 1));
                    newbuf.SetByte( ((RatioCache)m_ratRange[i]).yStartRatio,    (uint)(Table_VDMX.FieldOffsets.ratRange + (i * 4) + 2));
                    newbuf.SetByte( ((RatioCache)m_ratRange[i]).yEndRatio,        (uint)(Table_VDMX.FieldOffsets.ratRange + (i * 4) + 3));
                }

                // Set up the strting offset for the VDMX Groups
                ushort iOffset = (ushort)(Table_VDMX.FieldOffsets.ratRange + (m_numRatios * 4) + (m_numRatios * 2));

                // NOTE: we may want to check these VDMX Groups to see if they are orphans and remove them if they are?
                // populate buffer with VDMX Groups
                for( ushort i = 0; i < m_numRecs; i++ )
                {
                    // Save the offset for this group
                    VDMXOffsets[i] = iOffset;

                    newbuf.SetUshort( ((VDMXGroupCache)m_groups[i]).recs, iOffset );
                    iOffset += 2;
                    newbuf.SetByte( ((VDMXGroupCache)m_groups[i]).startsz, iOffset );
                    iOffset += 1;
                    newbuf.SetByte( ((VDMXGroupCache)m_groups[i]).endsz, iOffset );
                    iOffset += 1;
                    
                    for( ushort ii = 0; ii < ((VDMXGroupCache)m_groups[i]).recs; ii++ )
                    {
                        newbuf.SetUshort( ((VDMXGroupCache)m_groups[i]).getVTableRecordCache( ii ).yPelHeight, iOffset );
                        iOffset += 2;
                        newbuf.SetShort( ((VDMXGroupCache)m_groups[i]).getVTableRecordCache( ii ).yMax, iOffset );
                        iOffset += 2;
                        newbuf.SetShort( ((VDMXGroupCache)m_groups[i]).getVTableRecordCache( ii ).yMin, iOffset );
                        iOffset += 2;
                    }
                    
                }

                // populate buffer with Ratio Records
                for( ushort i = 0; i < m_numRatios; i++ )
                {
                    newbuf.SetByte( ((RatioCache)m_ratRange[i]).bCharSet,        (uint)(Table_VDMX.FieldOffsets.ratRange + (i * 4)));
                    newbuf.SetByte( ((RatioCache)m_ratRange[i]).xRatio,            (uint)(Table_VDMX.FieldOffsets.ratRange + (i * 4) + 1));
                    newbuf.SetByte( ((RatioCache)m_ratRange[i]).yStartRatio,    (uint)(Table_VDMX.FieldOffsets.ratRange + (i * 4) + 2));
                    newbuf.SetByte( ((RatioCache)m_ratRange[i]).yEndRatio,        (uint)(Table_VDMX.FieldOffsets.ratRange + (i * 4) + 3));

                    // Write out the offsets for the VDMX Groups to the buffer
                    newbuf.SetUshort( VDMXOffsets[((RatioCache)m_ratRange[i]).VDMXGroupThisRatio], (uint)(Table_VDMX.FieldOffsets.ratRange + (m_numRatios * 4) + (i * 2)));
                }

                // put the buffer into a Table_VDMX object and return it
                Table_VDMX VDMXTable = new Table_VDMX("VDMX", newbuf);

                return VDMXTable;
            }

            public class RatioCache : ICloneable
            {
                private byte m_bCharSet;
                private byte m_xRatio;
                private byte m_yStartRatio;
                private byte m_yEndRatio;
                // This represents the actual group number. Later we will use this to determine offset
                private ushort m_VDMXGroupThisRatio;

                public RatioCache()
                {

                }

                public RatioCache( byte bCharSet, byte xRatio, byte yStartRatio, byte yEndRatio, ushort VDMXGroupThisRatio )
                {
                    m_bCharSet = bCharSet;
                    m_xRatio = xRatio;
                    m_yStartRatio = yStartRatio;
                    m_yEndRatio = yEndRatio;
                    m_VDMXGroupThisRatio = VDMXGroupThisRatio;
                }

                public byte bCharSet
                {
                    get
                    {
                        return m_bCharSet;
                    }
                    set
                    {
                        m_bCharSet = value;                        
                    }
                }    
        
                public byte xRatio
                {
                    get
                    {
                        return m_xRatio;
                    }
                    set
                    {
                        m_xRatio = value;                        
                    }
                }    
        
                public byte yStartRatio
                {
                    get
                    {
                        return m_yStartRatio;
                    }
                    set
                    {
                        m_yStartRatio = value;                        
                    }
                }        

                public byte yEndRatio
                {
                    get
                    {
                        return m_yEndRatio;
                    }
                    set
                    {
                        m_yEndRatio = value;                        
                    }
                }    
    
                public ushort VDMXGroupThisRatio
                {
                    get
                    {
                        return m_VDMXGroupThisRatio;
                    }
                    set
                    {
                        m_VDMXGroupThisRatio = value;                        
                    }
                }
        
                public object Clone()
                {
                    return new RatioCache( bCharSet, xRatio, yStartRatio, yEndRatio, VDMXGroupThisRatio );
                }
            }

            public class VDMXGroupCache : ICloneable
            {
                private ushort m_recs;
                private byte m_startsz;
                private byte m_endsz;
                private ArrayList m_entry; // VTableRecordCache[]


                public VDMXGroupCache()
                {
                    m_entry = new ArrayList();
                }

                public ushort recs
                {
                    get
                    {
                        return m_recs;
                    }
                    set
                    {
                        m_recs = value;                        
                    }
                }

                public byte startsz
                {
                    get
                    {
                        return m_startsz;
                    }
                    set
                    {
                        m_startsz = value;                        
                    }
                }
            
                public byte endsz
                {
                    get
                    {
                        return m_endsz;
                    }
                    set
                    {
                        m_endsz = value;                        
                    }
                }

                public vTableRecordCache getVTableRecordCache( ushort nIndex )
                {
                    if( nIndex >= m_recs )
                    {
                        throw new ArgumentOutOfRangeException( "nIndex is greater than the number of records." );
                    }

                    return (vTableRecordCache)m_entry[nIndex];
                }

                public bool setVTableRecordCache( ushort nIndex,  vTableRecordCache vtrc )
                {
                    bool bResult = true;

                    if( nIndex >= m_recs )
                    {
                        bResult = false;
                        throw new ArgumentOutOfRangeException( "nIndex is greater than the number of records." );
                    }

                    m_entry[nIndex] = vtrc;                    

                    return bResult;
                }

                // NOTE: Maybe we should't ask for nIndex since this table is supposed to be sorted
                // we could sort ourselves?
                public bool addVTableRecordCache( ushort nIndex,  vTableRecordCache vtrc )
                {
                    bool bResult = true;

                    if( nIndex > m_recs )
                    {
                        bResult = false;
                        throw new ArgumentOutOfRangeException( "nIndex is greater than the number of records + 1." );
                    }

                    m_entry.Insert( nIndex, vtrc );                    

                    return bResult;
                }

                public bool addVTableRecordCache( ushort nIndex, ushort yPelHeight, short yMax, short yMin )
                {
                    bool bResult = true;

                    if( nIndex > m_recs )
                    {
                        bResult = false;
                        throw new ArgumentOutOfRangeException( "nIndex is greater than the number of records + 1." );
                    }

                    vTableRecordCache vtrc = new vTableRecordCache( yPelHeight, yMax, yMin );

                    bResult = addVTableRecordCache( nIndex, vtrc );                    

                    return bResult;
                }


                public bool removeVTableRecordCache( ushort nIndex )
                {
                    bool bResult = true;

                    if( nIndex >= m_recs )
                    {
                        bResult = false;
                        throw new ArgumentOutOfRangeException( "nIndex is greater than the number of records." );
                    }

                    m_entry.RemoveAt( nIndex );                    

                    return bResult;
                }

                public object Clone()
                {
                    VDMXGroupCache vgc = new VDMXGroupCache();
                    vgc.recs =  recs;
                    vgc.startsz = startsz;
                    vgc.endsz = endsz;
                    vgc.m_entry = new ArrayList( recs );

                    for( int i = 0; i < recs; i++ )
                    {
                        vgc.m_entry.Add( ((vTableRecordCache)m_entry[i]).Clone());            
                    }

                    return vgc;
                }
            
            }
            
    
            public class vTableRecordCache : ICloneable
            {
                private ushort m_yPelHeight;
                private short m_yMax;
                private short m_yMin;

                public vTableRecordCache( ushort yPelHeight, short yMax, short yMin )
                {
                    m_yPelHeight = yPelHeight;
                    m_yMax = yMax;
                    m_yMin = yMin;
                }

                public ushort yPelHeight
                {
                    get
                    {
                        return m_yPelHeight;
                    }
                    set
                    {
                        m_yPelHeight = value;                        
                    }
                }

                public short yMax
                {
                    get
                    {
                        return m_yMax;
                    }
                    set
                    {
                        m_yMax = value;                        
                    }
                }
            
                public short yMin
                {
                    get
                    {
                        return m_yMin;
                    }
                    set
                    {
                        m_yMin = value;                        
                    }
                }

                public object Clone()
                {
                    return new vTableRecordCache( yPelHeight, yMax, yMin );                    
                }
            }
        }

    }
}
