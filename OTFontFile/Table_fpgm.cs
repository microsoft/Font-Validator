using System;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_fpgm.
    /// </summary>
    public class Table_fpgm : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_fpgm(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
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
                m_cache = new fpgm_cache();
            }

            return m_cache;
        }
        
        public class fpgm_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        

    }
}
