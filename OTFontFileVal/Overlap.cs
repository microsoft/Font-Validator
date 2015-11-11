using System;
using System.Collections;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for Overlap.
    /// </summary>
    public class DataOverlapDetector
    {
        public DataOverlapDetector()
        {
            m_listFootprints = new ArrayList();
        }

        public bool CheckForNoOverlap(uint offset, uint length)
        {
            bool bNoOverlap = true;

            // create a new footprint object
            Footprint footprint = new Footprint(offset, length);

            // compare the current footprint against each stored footprint

            for (int i=0; i<m_listFootprints.Count; i++)
            {
                if (footprint.Overlaps((Footprint)m_listFootprints[i]))
                {
                    bNoOverlap = false;
                    break;
                }
            }

            // add the current footprint to the list of stored footprints
            m_listFootprints.Add(footprint);

            return bNoOverlap;
        }
        
        private class Footprint
        {
            public Footprint(uint offset, uint length)
            {
                first = offset;
                last = offset + length - 1;
                this.length = length;
            }

            public bool Overlaps(Footprint f)
            {
                bool bOverlap = false;

                // if either footprint is zero length, then there is no overlap
                // or if both footprints are identical, then assume they are caused by the same foot and therefore no overlap
                if ((length != 0 && f.length != 0) &&
                    (first != f.first || length != f.length) )
                {
                    if ( (first >= f.first && first <= f.last) || (f.first >= first && f.first <= last) )
                    {
                        bOverlap = true;
                    }
                }

                return bOverlap;
            }

            uint first;
            uint last;
            uint length;
        }

        ArrayList m_listFootprints;
    }

}
