using System.Collections.Generic;

namespace Compat.Win32APIs.DBCSLeadBytes
{
    public class dbcs10001
    {
        private static readonly List<byte> dbcs =
            new List<byte>
            {
                0x81,
                0x82,
                0x83,
                0x84,
                0x85,
                0x86,
                0x87,
                0x88,
                0x89,
                0x8A,
                0x8B,
                0x8C,
                0x8D,
                0x8E,
                0x8F,
                0x90,
                0x91,
                0x92,
                0x93,
                0x94,
                0x95,
                0x96,
                0x97,
                0x98,
                0x99,
                0x9A,
                0x9B,
                0x9C,
                0x9D,
                0x9E,
                0x9F,
                0xE0,
                0xE1,
                0xE2,
                0xE3,
                0xE4,
                0xE5,
                0xE6,
                0xE7,
                0xE8,
                0xE9,
                0xEA,
                0xEB,
                0xEC,
                0xED,
                0xEE,
                0xEF,
                0xF0,
                0xF1,
                0xF2,
                0xF3,
                0xF4,
                0xF5,
                0xF6,
                0xF7,
                0xF8,
                0xF9,
                0xFA,
                0xFB,
                0xFC,
            };

        public bool this[byte c] {
            get {
                return dbcs.Contains(c);
            }
        }
    }
}
