using System;
using System.IO;
using System.Collections;
using System.Diagnostics;




namespace OTFontFile
{
    abstract public class OTTable
    {
        /************************
         * constructors
         */
        
        
        /// <summary>Construct abstract base class tag and buffer</summary>
        public OTTable(OTTag tag, MBOBuffer buf)
        {
            m_tag = new OTTag(tag);
            m_bufTable = buf;
        }


        /************************
         * public methods
         */
        

        /// <summary>Calculate checksum for all except for 'head'</summary>
        public virtual uint CalcChecksum()
        {
            // NOTE: this method gets overridden by the head table class

            return m_bufTable.CalcChecksum();
        }

        /// <summary>Accessor of <c>m_bufTable</c></summary>
        public MBOBuffer GetBuffer()
        {
            return m_bufTable;
        }

        /// <summary>Return <c>true</c> iff <c>offset</c> is at the same
        /// <c>m_bufTable.GetFilePos()</c>, and <c>length</c> is equal to 
        /// <c>m_bufTable.GetLength()</c>.
        /// </summary>
        public bool MatchFileOffsetLength(uint offset, uint length)
        {
            bool bRet = true;

            if (m_bufTable == null)
            {
                bRet = false;
            }
            else
            {
                if (offset != (uint)m_bufTable.GetFilePos())
                {
                    bRet = false;
                }
                if (length != m_bufTable.GetLength())
                {
                    bRet = false;
                }
            }

            return bRet;
        }

        /// <summary>Return <c>true</c> iff <c>de</c> is for a 
        /// <c>DirectoryEntry</c> for a table equal to this one in
        /// tag, checksum, file offset and length.
        /// </summary>
        public bool MatchDirectoryEntry(DirectoryEntry de)
        {
            bool bRet = true;

            if (de.tag != m_tag)
            {
                bRet = false;
            }

            if (de.checkSum != CalcChecksum())
            {
                bRet = false;
            }

            if (!MatchFileOffsetLength(de.offset, de.length))
            {
                bRet = false;
            }

            return bRet;
        }

        /// <summary>Return length of <c>m_bufTable</c> or 0, if none.</summary>
        public uint GetLength()
        {
            uint nLength = 0;

            if (m_bufTable != null)
            {
                nLength = m_bufTable.GetLength();
            }

            return nLength;
        }

        /// <summary>Accessor for <c>m_tag</c></summary>
        public OTTag GetTag()
        {
            return m_tag;
        }


        /************************
         * nested class for 
         * holding cached data
         */

        public abstract class DataCache
        {
            public DataCache()
            {
                m_bDirty = false;
            }

            public bool IsDirty()
            {
                return m_bDirty;
            }

            public abstract OTTable GenerateTable();
            protected bool m_bDirty;
        }

        /// <summary>
        /// derived classes should override this method to 
        /// create their cache on demand
        /// </summary>
        public virtual DataCache GetCache()
        {
            return m_cache;
        }


        /************************
         * member data
         */

        public MBOBuffer m_bufTable;
        public OTTag m_tag;
        protected DataCache m_cache;
    }
}
