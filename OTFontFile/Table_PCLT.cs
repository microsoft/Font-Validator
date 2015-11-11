using System;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_PCLT.
    /// </summary>
    public class Table_PCLT : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_PCLT(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            Version             = 0,
            FontNumber          = 4,
            Pitch               = 8,
            xHeight             = 10,
            Style               = 12,
            TypeFamily          = 14,
            CapHeight           = 16,
            SymbolSet           = 18,
            Typeface            = 20,
            CharacterComplement = 36,
            FileName            = 44,
            StrokeWeight        = 50,
            WidthType           = 51,
            SerifStyle          = 52,
            Reserved            = 53
        }


        public OTFixed Version 
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.Version);}
        }

        public uint FontNumber 
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.FontNumber);}
        }

        public ushort Pitch 
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.Pitch);}
        }

        public ushort xHeight 
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.xHeight);}
        }

        public ushort Style 
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.Style);}
        }

        public ushort TypeFamily 
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.TypeFamily);}
        }

        public ushort CapHeight 
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.CapHeight);}
        }

        public ushort SymbolSet 
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.SymbolSet);}
        }

        public byte[] Typeface
        {
            get
            {
                byte[] buf = new byte[16];
                System.Buffer.BlockCopy(m_bufTable.GetBuffer(), (int)FieldOffsets.Typeface, buf, 0, 16);
                return buf;
            }            
        }

        public string GetTypeface()
        {
            char[] charBuf = new char[16];

            System.Text.ASCIIEncoding ae = new System.Text.ASCIIEncoding();
            System.Text.Decoder d = ae.GetDecoder();

            d.GetChars(Typeface, 0, 16, charBuf, 0);

            return new string(charBuf);
        }

        public byte[] CharacterComplement
        {
            get
            {
                byte[] buf = new byte[8];
                System.Buffer.BlockCopy(m_bufTable.GetBuffer(), (int)FieldOffsets.CharacterComplement, buf, 0, 8);
                return buf;
            }            
        }

        public string GetCharacterComplement()
        {
            char [] charBuf = new char[8];

            System.Text.ASCIIEncoding ae = new System.Text.ASCIIEncoding();
            System.Text.Decoder d = ae.GetDecoder();

            d.GetChars(CharacterComplement, 0, 8, charBuf, 0);

            return new string(charBuf);
        }

        public byte[] FileName
        {
            get
            {
                byte[] buf = new byte[6];
                System.Buffer.BlockCopy(m_bufTable.GetBuffer(), (int)FieldOffsets.FileName, buf, 0, 6);
                return buf;
            }            
        }

        public string GetFileName()
        {
            char [] charBuf = new char[6];

            System.Text.ASCIIEncoding ae = new System.Text.ASCIIEncoding();
            System.Text.Decoder d = ae.GetDecoder();

            d.GetChars(FileName, 0, 6, charBuf, 0);

            return new string(charBuf);
        }

        public sbyte StrokeWeight 
        {
            get {return (sbyte)m_bufTable.GetByte((uint)FieldOffsets.StrokeWeight);}
        }

        public sbyte WidthType 
        {
            get {return (sbyte)m_bufTable.GetByte((uint)FieldOffsets.WidthType);}
        }

        public byte SerifStyle 
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.SerifStyle);}
        }

        public byte Reserved
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.Reserved);}
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new PCLT_cache(this);
            }

            return m_cache;
        }
        
        public class PCLT_cache : DataCache
        {
            protected OTFixed m_Version;            
            protected uint m_FontNumber;         
            protected ushort m_Pitch;              
            protected ushort m_xHeight;            
            protected ushort m_Style;              
            protected ushort m_TypeFamily;         
            protected ushort m_CapHeight;          
            protected ushort m_SymbolSet;          
            protected byte[] m_Typeface;           
            protected byte[] m_CharacterComplement;
            protected byte[] m_FileName;           
            protected sbyte m_StrokeWeight;       
            protected sbyte m_WidthType;          
            protected byte m_SerifStyle;         
            protected byte m_Reserved;           

            // constructor
            public PCLT_cache(Table_PCLT OwnerTable)
            {
                // copy the data from the owner table's MBOBuffer
                // and store it in the cache variables
                m_Version                = OwnerTable.Version;
                m_FontNumber            = OwnerTable.FontNumber;
                m_Pitch                    = OwnerTable.Pitch;
                m_xHeight                = OwnerTable.xHeight;
                m_Style                    = OwnerTable.Style;
                m_TypeFamily            = OwnerTable.TypeFamily;
                m_CapHeight                = OwnerTable.CapHeight;
                m_SymbolSet                = OwnerTable.SymbolSet;
                m_Typeface                = OwnerTable.Typeface;
                m_CharacterComplement    = OwnerTable.CharacterComplement;
                m_FileName                = OwnerTable.FileName;
                m_StrokeWeight            = OwnerTable.StrokeWeight;
                m_WidthType             = OwnerTable.WidthType;
                m_SerifStyle            = OwnerTable.SerifStyle;
                m_Reserved                = OwnerTable.Reserved;                
            }

            // accessors
            public OTFixed Version 
            {
                get{ return m_Version; }
                set
                {
                    m_Version = value;
                    m_bDirty = true;
                }
            }

            public uint FontNumber 
            {
                get{ return m_FontNumber; }
                set
                {
                    m_FontNumber = value;
                    m_bDirty = true;
                }
            }

            public ushort Pitch 
            {
                get{ return m_Pitch; }
                set
                {
                    m_Pitch = value;
                    m_bDirty = true;
                }
            }

            public ushort xHeight 
            {
                get{ return m_xHeight; }
                set
                {
                    m_xHeight = value;
                    m_bDirty = true;
                }
            }

            public ushort Style 
            {
                get{ return m_Style; }
                set
                {
                    m_Style = value;
                    m_bDirty = true;
                }
            }

            public ushort TypeFamily 
            {
                get{ return m_TypeFamily; }
                set
                {
                    m_TypeFamily = value;
                    m_bDirty = true;
                }
            }

            public ushort CapHeight 
            {
                get{ return m_CapHeight; }
                set
                {
                    m_CapHeight = value;
                    m_bDirty = true;
                }
            }

            public ushort SymbolSet 
            {
                get{ return m_SymbolSet; }
                set
                {
                    m_SymbolSet = value;
                    m_bDirty = true;
                }
            }

        
            public byte[] Typeface
            {
                get
                {
                    byte[] buf = new byte[16];
                    System.Buffer.BlockCopy( m_Typeface, 0, buf, 0, 16);
                    return buf;
                }
                set
                {
                    if( Typeface.Length != 16 )
                    {
                        throw new ArrayTypeMismatchException( "Typeface is should be a byte[16] array." );
                    }
                    else
                    {
                        System.Buffer.BlockCopy( value, 0, m_Typeface, 0, 16);
                        m_bDirty = true;
                    }
                }
            }

            public string getTypeface()
            {
                char [] charBuf = new char[16];

                System.Text.ASCIIEncoding ae = new System.Text.ASCIIEncoding();
                System.Text.Decoder d = ae.GetDecoder();

                d.GetChars( m_Typeface, 0, 6, charBuf, 0 );

                return new string( charBuf );
            }

            public bool setTypeface( string sTypeface )
            {
                bool bResult = true;

                System.Text.UnicodeEncoding ue = new System.Text.UnicodeEncoding();
            
                if( ue.GetByteCount( sTypeface ) != 6 )
                {
                    bResult = false;
                    throw new ArrayTypeMismatchException( "Typeface should have a length of 16 characters." );
                }

                m_Typeface = ue.GetBytes( sTypeface );

                return bResult;
            }

            public byte[] CharacterComplement
            {
                get
                {
                    byte[] buf = new byte[8];
                    System.Buffer.BlockCopy( m_CharacterComplement, 0, buf, 0, 8 );
                    return buf;
                }
                set
                {
                    if( CharacterComplement.Length != 8 )
                    {
                        throw new ArrayTypeMismatchException( "CharacterComplement should be a byte[8] array." );
                    }
                    else
                    {
                        System.Buffer.BlockCopy( value, 0, m_CharacterComplement, 0, 8 );
                        m_bDirty = true;
                    }
                }
            }

            public string getCharacterComplement()
            {
                char [] charBuf = new char[8];

                System.Text.ASCIIEncoding ae = new System.Text.ASCIIEncoding();
                System.Text.Decoder d = ae.GetDecoder();

                d.GetChars( m_CharacterComplement, 0, 8, charBuf, 0 );

                return new string( charBuf );
            }

            public bool setCharacterComplement( string sCharacterComplement )
            {
                bool bResult = true;

                System.Text.UnicodeEncoding ue = new System.Text.UnicodeEncoding();
            
                if( ue.GetByteCount( sCharacterComplement ) != 8 )
                {
                    bResult = false;
                    throw new ArrayTypeMismatchException( "CharacterComplement should have a length of 8 characters." );
                }

                m_CharacterComplement = ue.GetBytes( sCharacterComplement );

                return bResult;
            }

            
            public byte[] FileName
            {
                get
                {
                    byte[] buf = new byte[6];
                    System.Buffer.BlockCopy( m_FileName, 0, buf, 0, 6);
                    return buf;
                }
                set
                {
                    if( FileName.Length != 6 )
                    {
                        throw new ArrayTypeMismatchException( "FileName should be a byte[6] array." );
                    }
                    else
                    {
                        System.Buffer.BlockCopy( value, 0, m_FileName, 0, 6);
                        m_bDirty = true;
                    }
                }
            }

            public string getFileName()
            {
                char [] charBuf = new char[6];

                System.Text.ASCIIEncoding ae = new System.Text.ASCIIEncoding();
                System.Text.Decoder d = ae.GetDecoder();

                d.GetChars( m_FileName, 0, 6, charBuf, 0 );

                return new string( charBuf );
            }

            public bool setFileName( string sFileName )
            {
                bool bResult = true;

                System.Text.UnicodeEncoding ue = new System.Text.UnicodeEncoding();
            
                if( ue.GetByteCount( sFileName ) != 6 )
                {
                    bResult = false;
                    throw new ArrayTypeMismatchException( "FileName is should have a length of 6 characters." );
                }

                m_FileName = ue.GetBytes( sFileName );

                return bResult;
            }

            public sbyte StrokeWeight 
            {
                get{ return m_StrokeWeight; }
                set
                {
                    m_StrokeWeight = value;
                    m_bDirty = true;
                }
            }

            public sbyte WidthType 
            {
                get{ return m_WidthType; }
                set
                {
                    m_WidthType = value;
                    m_bDirty = true;
                }
            }

            public byte SerifStyle 
            {
                get{ return m_SerifStyle; }
                set
                {
                    m_SerifStyle = value;
                    m_bDirty = true;
                }
            }

            public byte Reserved 
            {
                get{ return m_Reserved; }
                set
                {
                    m_Reserved = value;
                    m_bDirty = true;
                }
            }

        

            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf = new MBOBuffer(54);

                // populate the buffer                
                newbuf.SetFixed( m_Version,                    (uint)Table_PCLT.FieldOffsets.Version );
                newbuf.SetUint( m_FontNumber,                (uint)Table_PCLT.FieldOffsets.FontNumber );
                newbuf.SetUshort( m_Pitch,                    (uint)Table_PCLT.FieldOffsets.Pitch );
                newbuf.SetUshort( m_xHeight,                (uint)Table_PCLT.FieldOffsets.xHeight );
                newbuf.SetUshort( m_Style,                    (uint)Table_PCLT.FieldOffsets.Style );
                newbuf.SetUshort( m_TypeFamily,                (uint)Table_PCLT.FieldOffsets.TypeFamily );
                newbuf.SetUshort( m_CapHeight,                (uint)Table_PCLT.FieldOffsets.CapHeight );
                newbuf.SetUshort( m_SymbolSet,                (uint)Table_PCLT.FieldOffsets.SymbolSet );
                for( uint i = 0; i < 16; i++ )
                {
                    newbuf.SetByte( m_Typeface[i],            (uint)Table_PCLT.FieldOffsets.Typeface + i );
                }
                
                for( uint i = 0; i < 8; i++ )
                {
                    newbuf.SetByte( m_CharacterComplement[i],(uint)Table_PCLT.FieldOffsets.CharacterComplement + i );
                }

                for( uint i = 0; i < 6; i++ )
                {
                    newbuf.SetByte( m_FileName[i],            (uint)Table_PCLT.FieldOffsets.FileName + i );
                }
                
                newbuf.SetShort( m_StrokeWeight,            (uint)Table_PCLT.FieldOffsets.StrokeWeight );
                newbuf.SetShort( m_WidthType,                (uint)Table_PCLT.FieldOffsets.WidthType );
                newbuf.SetByte( m_SerifStyle,                (uint)Table_PCLT.FieldOffsets.SerifStyle );
                newbuf.SetByte( m_Reserved,                    (uint)Table_PCLT.FieldOffsets.Reserved );
                


                // put the buffer into a Table_vhea object and return it
                Table_PCLT PCLTTable = new Table_PCLT("PCLT", newbuf);

                return PCLTTable;
            }
        }
        


    }
}
