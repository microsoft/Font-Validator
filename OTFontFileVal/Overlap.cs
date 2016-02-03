using System;
using System.Collections.Generic;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for Overlap.
    /// </summary>
    public class DataOverlapDetector
    {
        public DataOverlapDetector()
        {
            m_listFootprints = new List<Footprint>();
        }

        public bool CheckForNoOverlap(uint offset, uint length)
        {
            bool bNoOverlap = true;

            // create a new footprint object
            Footprint footprint = new Footprint(offset, length);

            // compare the current footprint against each stored footprint

            for (int i=0; i<m_listFootprints.Count; i++)
            {
                if (footprint.Overlaps(m_listFootprints[i]))
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

            public uint begins
            {
                get {
                    return first;
                }
            }
            public uint ends
            {
                get {
                    return (first + length);
                }
            }

            uint first;
            uint last;
            uint length;
        }

        public string Occupied
        {
            get {
                m_listFootprints.Sort(delegate(Footprint x, Footprint y)
                                      {
                                          if (x.begins == y.begins)
                                              return x.ends.CompareTo(y.ends);
                                          return x.begins.CompareTo(y.begins);
                                      });

                string result = m_listFootprints[0].begins.ToString();
                for (int i = 1; i < m_listFootprints.Count; i++)
                {
                    if (m_listFootprints[i].begins != m_listFootprints[i-1].ends )
                        result += "-" + m_listFootprints[i-1].ends + ";" + m_listFootprints[i].begins;
                }
                result += "-" + m_listFootprints[m_listFootprints.Count -1 ].ends;
                return result;
            }
        }

        public uint ends
        {
            get {
                return m_listFootprints[m_listFootprints.Count -1 ].ends;
            }
        }

        private List<Footprint> m_listFootprints;
    }

}
