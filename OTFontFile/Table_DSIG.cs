using System;
using System.Diagnostics;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_DSIG.
    /// </summary>
    public class Table_DSIG : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_DSIG(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            ulVersion = 0,
            usNumSigs = 4,
            usFlag    = 6,
        }


        /************************
         * public methods
         */
        
        

        /************************
         * internal classes
         */

        public class SigFormatOffset
        {
            public uint ulFormat;
            public uint ulLength;
            public uint ulOffset;
        }

        public class SignatureBlock
        {
            public ushort usReserved1;
            public ushort usReserved2;
            public uint cbSignature;
            public byte[] bSignature;
        }
        
        /************************
         * accessors
         */
        
        
        public uint ulVersion
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.ulVersion);}
        }

        public ushort usNumSigs
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.usNumSigs);}
        }

        public ushort usFlag
        {
            get {return m_bufTable.GetUshort((uint)FieldOffsets.usFlag);}
        }

        public SigFormatOffset GetSigFormatOffset(uint i)
        {
            SigFormatOffset sfo = null;

            if (i < usNumSigs)
            {
                sfo = new SigFormatOffset();
                uint offset = 8 + i * 12;
                sfo.ulFormat = m_bufTable.GetUint(offset);
                sfo.ulLength = m_bufTable.GetUint(offset + 4);
                sfo.ulOffset = m_bufTable.GetUint(offset + 8);
            }

            return sfo;
        }
        
        public SignatureBlock GetSignatureBlock(uint i)
        {
            SignatureBlock sb = null;

            if (i < usNumSigs)
            {
                SigFormatOffset sfo = GetSigFormatOffset(i);

                sb = new SignatureBlock();
                sb.usReserved1 = m_bufTable.GetUshort(sfo.ulOffset);
                sb.usReserved2 = m_bufTable.GetUshort(sfo.ulOffset + 2);
                sb.cbSignature = m_bufTable.GetUint(sfo.ulOffset + 4);
                sb.bSignature  = new byte[sb.cbSignature];
                System.Buffer.BlockCopy(m_bufTable.GetBuffer(), (int)sfo.ulOffset + 8, sb.bSignature, 0, (int)sb.cbSignature);
            }

            return sb;
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new DSIG_cache();
            }

            return m_cache;
        }
        
        public class DSIG_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        

    }
}
