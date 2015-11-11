using System;




namespace OTFontFile
{
    /// <summary>
    /// Summary description for Table_cvt.
    /// </summary>
    public class Table_cvt : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_cvt(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }

        /************************
         * public methods
         */
        
        
        public short GetValue(uint i)
        {
            return m_bufTable.GetShort(i*2);
        }



        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new cvt_cache();
            }

            return m_cache;
        }
        
        public class cvt_cache : DataCache
        {
            public override OTTable GenerateTable()
            {
                // not yet implemented!
                return null;
            }
        }
        


    }
}
