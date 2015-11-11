
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;


namespace OTFontFile
{
    public class util
    {
        public static ushort MaxPower2LE(ushort n)
        {
            // returns max power of 2 <= n

            if (n == 0)
            {
                throw new ArithmeticException();
            }

            ushort pow2 = 1;
            n >>= 1;
            while (n != 0)
            {
                n >>= 1;
                pow2 <<= 1;
            }

            return pow2;
        }

        public static ushort Log2(ushort n)
        {
            // returns the integer component of log2 of n
            // fractional component is lost, but not needed by font spec

            if (n == 0)
            {
                throw new ArithmeticException();
            }

            ushort log2 = 0;
            n >>= 1;

            while (n != 0)
            {
                n >>= 1;
                log2++;
            }

            return log2;
        }

    }

    // The purpose of this class is to allow an ArrayList with a deep copy clone method
    // Any object that is part of this ArryList needs to support ICloneable
    public class DeepCloneArrayList : ArrayList
    {
        public override object Clone()
        {        
            DeepCloneArrayList clonedList = new DeepCloneArrayList();

            for( int i = 0; i < this.Count; i++ )
            {
                clonedList.Add( ((ICloneable)this[i]).Clone() );

            }

            return clonedList;
        }

    }


}
