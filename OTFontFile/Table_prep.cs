using System;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_prep.
    /// </summary>
    public class Table_prep : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_prep(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * accessors
         */
        
        
        public byte GetByte(uint i)
        {
            return m_bufTable.GetByte(i);
        }


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new prep_cache();
            }

            return m_cache;
        }
        
        public class prep_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        

    }
}
