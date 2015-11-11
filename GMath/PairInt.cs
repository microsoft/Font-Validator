using System;
using System.Diagnostics;
using System.Collections;

namespace NS_GMath
{
    public class PairInt
    { 
        /*
         *        MEMBERS
         */
        int itemA;
        int itemB;

        /*
         *        PROPERTIES
         */
        public int this [int ind]
        {
            get 
            {
                if ((ind!=0)&&(ind!=1))
                {
                    throw new ExceptionGMath("PairInt","indexer",null);
                }
                return (ind==0)? this.itemA: this.itemB;
            }
            set
            {
                if ((ind!=0)&&(ind!=1))
                {
                    throw new ExceptionGMath("PairInt","indexer",null);
                }
                if (ind==0)
                {
                    this.itemA=value;
                }
                else
                {
                    this.itemB=value;
                }
            }
        }

        /*
         *        CONSTRUCTORS 
         */
        public PairInt(int itemA, int itemB)
        {
            this.itemA=itemA;
            this.itemB=itemB;
        }
    }

    public class ListPairInt : IEnumerable
    {
        /*
         *        MEMBERS
         */
        ArrayList pairs;

        /*
         *        PROPERTIES
         */

        public int Count
        {
            get 
            {
                return (this.pairs.Count);
            }
        }
        
        /*
         *        CONSTRUCTORS
         */
    
        public ListPairInt()
        {
            this.pairs=new ArrayList();    
        }

        /*
         *        METHODS
         */
        public void Add(PairInt pair)
        {
            this.pairs.Add(pair);
        }
        public void Add(int itemA, int itemB)
        {
            PairInt pair=new PairInt(itemA,itemB);
            this.pairs.Add(pair);
        }
        public void Clear()
        {
            this.pairs.Clear();
        }
        public IEnumerator GetEnumerator()
        {
            return this.pairs.GetEnumerator();
        }
    }
}