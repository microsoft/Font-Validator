// Copyright (c) Hin-Tak Leung

// All rights reserved.

// MIT License

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the ""Software""), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// mcs /nostdlib /platform:AnyCPU /reference:/usr/lib/mono/2.0/System.dll /reference:/usr/lib/mono/2.0/mscorlib.dll -lib:/usr/lib/mono/2.0 -r:System.Security   DSIGInfo.cs ../mcs-class-Mono.Security/ASN1.cs ../OTFontFile/* ../mcs-class-Mono.Security/ASN1Convert.cs

using System;
using System.Text;
using OTFontFile;
using System.Collections.Generic;
using System.Linq;

using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography; // Oid
using Mono.Security;

namespace Compat
{
    public class DSIGInfo
    {
        static HashAlgorithm hash;

        static int Main( string[] args )
        {
            if (args.Length == 0) {
                Console.WriteLine("DSIGInfo [-v] [-v] [-v] fontfile");
                return 0;
            }

            OTFile f = new OTFile();
            Table_DSIG tDSIG = null;
            int verbose = 0;
            string filename = null;

            for ( int i = 0; i < args.Length; i++ ) {
                if ( "-v" == args[i] )
                    verbose++;
                else
                    filename = args[i];
            }

            if ( !f.open(filename) )
            {
                    Console.WriteLine("Error: Cannot open {0} as font file", filename);
                    return 0;
            }

            TTCHeader ttc = null;
            if ( f.IsCollection() )
            {
                ttc = f.GetTTCHeader();
                if ( f.GetTableManager().GetUnaliasedTableName(ttc.DsigTag) == "DSIG" )
                {
                MBOBuffer buf = f.ReadPaddedBuffer(ttc.DsigOffset, ttc.DsigLength);
                tDSIG = (Table_DSIG) f.GetTableManager().CreateTableObject(ttc.DsigTag, buf);
                }
                for (uint i = 0; i < f.GetNumFonts() ; i++)
                {
                    OTFont fn = f.GetFont(i);
                    Table_DSIG memDSIG = (Table_DSIG) fn.GetTable("DSIG");
                    if (memDSIG != null)
                    {
                        Console.WriteLine("Warning: DSIG in member font");
                        break;
                    }
                }
            }
            else
            {
                OTFont fn = f.GetFont(0);
                tDSIG = (Table_DSIG) fn.GetTable("DSIG");
            }

            Console.WriteLine("{0} DSIG table: {1}", filename,
                              ( tDSIG == null ) ? "Absent" : "Present" );
            if (tDSIG == null)
                return 0;
            if ( f.IsCollection() && ttc.version != 0x00020000 )
                Console.WriteLine("Warning: TTC has DSIG but header version is 0x{0}, != 0x00020000", ttc.version.ToString("X8"));

            if ( tDSIG.usNumSigs != 1 )
            Console.WriteLine("NumSigs = {0}", tDSIG.usNumSigs);
            for ( uint v = 0; v < tDSIG.usNumSigs ; v++ )
            {
                Table_DSIG.SignatureBlock sgb;
                try {
                    sgb = tDSIG.GetSignatureBlock(v);
                } catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine("Error: Out of Range SignatureBlock {0}", v);
                    break;
                }

                SignedCms cms = new SignedCms();
                cms.Decode(sgb.bSignature);

                if ( cms.SignerInfos.Count > 1 )
                Console.WriteLine( "#SignerInfos: {0}", cms.SignerInfos.Count );
                foreach ( var si in cms.SignerInfos )
                {
                    Console.WriteLine(si.Certificate);
                }
                Console.WriteLine( "#Certificates: {0}", cms.Certificates.Count );
                foreach ( var x509 in cms.Certificates )
                {
                if ( verbose > 0 )
                {
                    Console.WriteLine(x509);
                }
                else
                {
                    Console.WriteLine(x509.Subject);
                }
                };

                ASN1 spc = new ASN1(cms.ContentInfo.Content);

                ASN1 playload_oid = null;
                ASN1 oid = null;
                ASN1 digest = null;
                ASN1 obsolete = null;
                if ( Type.GetType("Mono.Runtime") == null )
                {
                    // DotNet is much saner!
                    playload_oid = spc[0][0];
                    obsolete = spc[0][1][0];
                    oid = spc[1][0][0];
                    digest = spc[1][1];
                }
                else
                {
                    playload_oid = spc[0];
                    obsolete = spc[1][0];
                    oid = spc[2][0][0];
                    digest = spc[2][1];
                }
                string algo = ASN1Convert.ToOid (oid);
                string algoname = (new Oid(algo)).FriendlyName;
                Console.WriteLine("Digest Algorithm: {0}", algoname);
                byte[] Value = digest.Value;
                StringBuilder hexLine = new StringBuilder ();
                for (int i = 0; i < Value.Length; i++) {
                    hexLine.AppendFormat ("{0} ", Value [i].ToString ("X2"));
                }
                hexLine.AppendFormat (Environment.NewLine);
                Console.WriteLine("{0} Digest: {1}", algoname, hexLine);

                switch ( algoname )
                {
                    case "md5":
                        hash = HashAlgorithm.Create ("MD5");
                        break;
                    case "sha1":
                        hash = HashAlgorithm.Create ("SHA1");
                        break;
                    default:
                        throw new NotImplementedException("Unknown HashAlgorithm: " + algoname );
                }

                byte[] cdigest;
                if ( f.IsCollection() )
                {
                    cdigest = get_TTC_digest( f );

                }
                else
                {
                    cdigest = get_TTF_digest( f );
                }
                hexLine = new StringBuilder ();
                for (int i = 0; i < cdigest.Length; i++) {
                    hexLine.AppendFormat ("{0} ", cdigest [i].ToString ("X2"));
                }
                hexLine.AppendFormat (Environment.NewLine);
                Console.WriteLine("Calculated Digest: {0}", hexLine);
            }

            return 0;
        }

        static byte[] get_TTC_digest( OTFile f )
        {
                        throw new NotImplementedException( "TTC digest" );
        }

        static byte[] get_TTF_digest( OTFile f )
        {
            OTFont fn = f.GetFont(0);
            Table_DSIG tDSIG = (Table_DSIG) fn.GetTable("DSIG");
            DirectoryEntry deDSIG = null;
            // sort table by offset
            Dictionary<uint, int> offsetlookup = new Dictionary<uint, int>();
            for (ushort i=0; i<fn.GetNumTables(); i++) {
                DirectoryEntry de = fn.GetDirectoryEntry(i);
                offsetlookup.Add(de.offset, i);
                if ((string) de.tag == "DSIG" )
                    deDSIG = de;
            }
            var list = offsetlookup.Keys.ToList();
            list.Sort();

            // New offset table
            var old_ot = fn.GetOffsetTable();
            OffsetTable ot = new OffsetTable (old_ot.sfntVersion, (ushort) ( old_ot.numTables - 1) );

            for (ushort i=0; i<fn.GetNumTables(); i++) {
                DirectoryEntry oldde = fn.GetDirectoryEntry(i);
                if ( (string) oldde.tag != "DSIG" )
                {
                    DirectoryEntry de = new DirectoryEntry(oldde);
                    de.offset -=16; // one less entry
                    if ( de.offset > deDSIG.offset )
                        de.offset -= tDSIG.GetBuffer().GetPaddedLength();
                    ot.DirectoryEntries.Add(de);
                }
            }
            hash.TransformBlock (ot.m_buf.GetBuffer(), 0, (int)ot.m_buf.GetLength(), ot.m_buf.GetBuffer(), 0);

            for (int i=0; i< ot.DirectoryEntries.Count ; i++)
            {
                DirectoryEntry de = (DirectoryEntry)ot.DirectoryEntries[i];
                hash.TransformBlock (de.m_buf.GetBuffer(), 0, (int)de.m_buf.GetLength(), de.m_buf.GetBuffer(), 0);
            }

            Table_head headTable = (Table_head) fn.GetTable("head");

            // calculate the checksum
            uint sum = 0;
            sum += ot.CalcOffsetTableChecksum();
            sum += ot.CalcDirectoryEntriesChecksum();
            foreach (var key in list) {
                OTTable table = fn.GetTable((ushort)offsetlookup[key]);

                if ( (string) table.GetTag() != "DSIG")
                    sum += table.CalcChecksum();
            }

            Table_head.head_cache headCache = (Table_head.head_cache)headTable.GetCache();

            // set the checkSumAdujustment field
            headCache.checkSumAdjustment = 0xb1b0afba - sum;
            Table_head newHead = (Table_head)headCache.GenerateTable();

            foreach (var key in list) {
                OTTable table = fn.GetTable((ushort)offsetlookup[key]);

                if ( (string) table.GetTag() == "head" )
                    table = newHead;

                if ( (string) table.GetTag() != "DSIG" ) {
                    hash.TransformBlock (table.m_bufTable.GetBuffer(), 0, (int)table.GetBuffer().GetPaddedLength(),
                                         table.m_bufTable.GetBuffer(), 0);
                }
            }

            byte[] usFlag = {0, 1};
            hash.TransformFinalBlock(usFlag, 0,2);
            return hash.Hash;
        }
    }
}
