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

using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography; // Oid
using Mono.Security;

namespace Compat
{
    public class DSIGInfo
    {
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
                    Console.WriteLine(x509.IssuerName.Name);
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
                Console.WriteLine("Digest Algorithm: {0}", (new Oid(algo)).FriendlyName);
                byte[] Value = digest.Value;
                StringBuilder hexLine = new StringBuilder ();
                for (int i = 0; i < Value.Length; i++) {
                    hexLine.AppendFormat ("{0} ", Value [i].ToString ("X2"));
                }
                hexLine.AppendFormat (Environment.NewLine);
                Console.WriteLine("{0} Digest: {1}", (new Oid(algo)).FriendlyName, hexLine);
            }
            return 0;
        }
    }
}
