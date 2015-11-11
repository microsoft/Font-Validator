using System;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_CFF.
    /// </summary>
    public class Table_CFF : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_CFF(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * field offset values
         */
        
        public enum FieldOffsets
        {
            major = 0,
            minor = 1,
            hdrSize = 2,
            offSize = 3
        }


        /************************
         * accessors
         */

        public byte major
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.major);}
        }

        public byte minor
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.minor);}
        }
        
        public byte hdrSize
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.hdrSize);}
        }
        
        public byte offSize
        {
            get {return m_bufTable.GetByte((uint)FieldOffsets.offSize);}
        }



        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new CFF_cache();
            }

            return m_cache;
        }
        
        public class CFF_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        


    }
}
