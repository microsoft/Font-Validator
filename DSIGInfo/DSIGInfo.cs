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

using System;
using System.Text;
using OTFontFile;
using System.IO;
using System.Collections.Generic;

using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography; // Oid
using Mono.Security;

namespace Compat
{
    public class DSIGInfo
    {
        private static readonly Dictionary<string, string> trusted_roots =
            new Dictionary<string, string>
            {
                { "0563B8630D62D75ABBC8AB1E4BDFB5A899B24D43", "CN=DigiCert Assured ID Root CA, OU=www.digicert.com, O=DigiCert Inc, C=US" },
                { "18F7C1FCC3090203FD5BAA2F861A754976C8DD25", "OU=\"NO LIABILITY ACCEPTED, (c)97 VeriSign, Inc.\", OU=VeriSign Time Stamping Service Root, OU=\"VeriSign, Inc.\", O=VeriSign Trust Network" },
                { "245C97DF7514E7CF2DF8BE72AE957B9E04741E85", "OU=Copyright (c) 1997 Microsoft Corp., OU=Microsoft Time Stamping Service Root, OU=Microsoft Corporation, O=Microsoft Trust Network" },
                { "24A40A1F573643A67F0A4B0749F6A22BF28ABB6B", "OU=VeriSign Commercial Software Publishers CA, O=\"VeriSign, Inc.\", L=Internet" },
                { "3B1EFD3A66EA28B16697394703A72CA340A05BD5", "CN=Microsoft Root Certificate Authority 2010, O=Microsoft Corporation, L=Redmond, S=Washington, C=US" },
                { "4EB6D578499B1CCF5F581EAD56BE3D9B6744A5E5", "CN=VeriSign Class 3 Public Primary Certification Authority - G5, OU=\"(c) 2006 VeriSign, Inc. - For authorized use only\", OU=VeriSign Trust Network, O=\"VeriSign, Inc.\", C=US" },
                { "4F65566336DB6598581D584A596C87934D5F2AB4", "OU=Class 3 Public Primary Certification Authority, O=\"VeriSign, Inc.\", C=US" },
                { "627F8D7827656399D27D7F9044C9FEB3F33EFA9A", "E=premium-server@thawte.com, CN=Thawte Premium Server CA, OU=Certification Services Division, O=Thawte Consulting cc, L=Cape Town, S=Western Cape, C=ZA" },
                { "742C3192E607E424EB4549542BE1BBC53E6174E2", "OU=Class 3 Public Primary Certification Authority, O=\"VeriSign, Inc.\", C=US" },
                { "7F88CD7223F3C813818C994614A89C99FA3B5247", "CN=Microsoft Authenticode(tm) Root Authority, O=MSFT, C=US" },
                { "8F43288AD272F3103B6FB1428485EA3014C0BCFE", "CN=Microsoft Root Certificate Authority 2011, O=Microsoft Corporation, L=Redmond, S=Washington, C=US" },
                { "91C6D6EE3E8AC86384E548C299295C756C817B81", "CN=thawte Primary Root CA, OU=\"(c) 2006 thawte, Inc. - For authorized use only\", OU=Certification Services Division, O=\"thawte, Inc.\", C=US" },
                { "97817950D81C9670CC34D809CF794431367EF474", "CN=GTE CyberTrust Global Root, OU=\"GTE CyberTrust Solutions, Inc.\", O=GTE Corporation, C=US" },
                { "A43489159A520F0D93D032CCAF37E7FE20A8B419", "CN=Microsoft Root Authority, OU=Microsoft Corporation, OU=Copyright (c) 1997 Microsoft Corp." },
                { "BE36A4562FB2EE05DBB3D32323ADF445084ED656", "CN=Thawte Timestamping CA, OU=Thawte Certification, O=Thawte, L=Durbanville, S=Western Cape, C=ZA" },
                { "CDD4EEAE6000AC7F40C3802C171E30148030C072", "CN=Microsoft Root Certificate Authority, DC=microsoft, DC=com" },
                { "DE28F4A4FFE5B92FA3C503D1A349A7F9962A8212", "CN=GeoTrust Global CA, O=GeoTrust Inc., C=US" },
                { "E0AB059420725493056062023670F7CD2EFC6666", "E=premium-server@thawte.com, CN=Thawte Premium Server CA, OU=Certification Services Division, O=Thawte Consulting cc, L=Cape Town, S=Western Cape, C=ZA" },
                { "E12DFB4B41D7D9C32B30514BAC1D81D8385E2D46", "CN=UTN-USERFirst-Object, OU=http://www.usertrust.com, O=The USERTRUST Network, L=Salt Lake City, S=UT, C=US" },
            };
        static HashAlgorithm hash;
        private OTFile fontfile;
        private Table_DSIG tDSIG;
        static int verbose;
#if HAVE_MONO_X509
        private static Mono.Security.X509.X509CertificateCollection certs;
#endif

        // Internal Analysis Results
        private bool HaveDSIG;
        private byte[] signed_hash;
        private byte[] calc_hash;
        private string signer;
        private int signer_count;
        private string algoname;

        // Analysis Results to Caller
        public bool Warn_TTCv1;
        public bool Warn_DSIG_in_memFonts;
        public bool Warn_MalformedSIG;
        public int  usNumSigs;

        public bool IsOK
        {
            get {
                if ( !HaveDSIG )
                    return true;
                if ( usNumSigs == 0 )
                    return true;

                // Does not happen - can't handle yet
                if ( usNumSigs > 1 )
                    return false;

                // usNumSigs == 1
                for (int i  = 0 ; i < signed_hash.Length ; i++ )
                    if (signed_hash[i] != calc_hash[i])
                        return false;
                return true;
            }
        }

        public string Signer
        {
            get {
                return signer;
            }
        }

        public string AlgoName
        {
            get {
                return algoname.ToUpper();
            }
        }

        public string SignedHash
        {
            get {
                string result = "";
                for (int i = 0; i < signed_hash.Length; i++) {
                    result += signed_hash[i].ToString ("X2");
                }
                return result;
            }
        }

        public string CalcHash
        {
            get {
                string result = "";
                for (int i = 0; i < calc_hash.Length; i++) {
                    result += calc_hash[i].ToString ("X2");
                }
                return result;
            }
        }
        // Caller Result ends

        public DSIGInfo( OTFile f )
        {
            fontfile = f;
        }

        public DSIGInfo( string filename )
        {
            fontfile = new OTFile();
            tDSIG = null;

            Warn_TTCv1 = false;
            Warn_DSIG_in_memFonts = false;
            Warn_MalformedSIG = false;
            usNumSigs = 0;

            if ( !fontfile.open(filename) )
            {
                        throw new IOException("Cannot open file " + filename );
            }

            TTCHeader ttc = null;
            if ( fontfile.IsCollection() )
            {
                ttc = fontfile.GetTTCHeader();
                if ( fontfile.GetTableManager().GetUnaliasedTableName(ttc.DsigTag) == "DSIG" )
                {
                MBOBuffer buf = fontfile.ReadPaddedBuffer(ttc.DsigOffset, ttc.DsigLength);
                tDSIG = (Table_DSIG) fontfile.GetTableManager().CreateTableObject(ttc.DsigTag, buf);
                }
                for (uint i = 0; i < fontfile.GetNumFonts() ; i++)
                {
                    OTFont fn = fontfile.GetFont(i);
                    Table_DSIG memDSIG = (Table_DSIG) fn.GetTable("DSIG");
                    if (memDSIG != null)
                    {
                        Warn_DSIG_in_memFonts = true;
                        break;
                    }
                }
            }
            else
            {
                OTFont fn = fontfile.GetFont(0);
                tDSIG = (Table_DSIG) fn.GetTable("DSIG");
            }

            HaveDSIG = ( ( tDSIG == null ) ? false : true );

            // Officially we should only warn if HaveDSIG true
            if ( fontfile.IsCollection() && ttc.version != 0x00020000 )
                Warn_TTCv1 = true;

            if ( HaveDSIG )
                FurtherWork();
        }

        void FurtherWork()
        {
            usNumSigs = tDSIG.usNumSigs;
            if ( tDSIG.usNumSigs > 1 )
                throw new NotImplementedException("usNumSigs=" + tDSIG.usNumSigs + " > 1" );
            for ( uint v = 0; v < tDSIG.usNumSigs ; v++ )
            {
                Table_DSIG.SignatureBlock sgb;
                try {
                    sgb = tDSIG.GetSignatureBlock(v);
                } catch (IndexOutOfRangeException e)
                {
                    Warn_MalformedSIG = true;
                    break;
                }

                SignedCms cms = new SignedCms();
                cms.Decode(sgb.bSignature);

                signer_count = cms.SignerInfos.Count;
                if ( signer_count > 1 )
                    throw new NotImplementedException("SignerInfos.Count=" + signer_count + " > 1" );
                foreach ( var si in cms.SignerInfos )
                {
                    signer = si.Certificate.Subject;
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
                algoname = (new Oid(algo)).FriendlyName;
                signed_hash = digest.Value;

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

                if ( fontfile.IsCollection() )
                {
                    calc_hash = get_TTC_digest();
                }
                else
                {
                    calc_hash = get_TTF_digest();
                }
            }
        }

        static int Main( string[] args )
        {
            if (args.Length == 0) {
                Console.WriteLine("DSIGInfo [-v] [-v] [-v] fontfile");
                return 0;
            }

            OTFile f = new OTFile();
            Table_DSIG tDSIG = null;
            string filename = null;
            verbose = 0;

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
                    if ( Type.GetType("Mono.Runtime") == null )
                        foreach ( var ua in si.UnsignedAttributes )
                        {
                            foreach ( var asnd in ua.Values )
                            {
                                try
                                {
                                    ASN1 vv = new ASN1(asnd.RawData);
                                    ASN1 t = new ASN1(vv[3][1][1].Value);
                                    Console.WriteLine("Decoded Signing Time: {0}", ASN1Convert.ToDateTime (t) );
                                }
                                catch (Exception e)
                                {/* Nothing to do */ }
                            }
                        }
                }
                Console.WriteLine( "#Certificates: {0}", cms.Certificates.Count );
#if HAVE_MONO_X509
                certs =  new Mono.Security.X509.X509CertificateCollection ();
                //Mono.Security.X509.X509Chain signerChain = new Mono.Security.X509.X509Chain ();
#endif
                foreach ( var x509 in cms.Certificates )
                {
#if HAVE_MONO_X509
                    certs.Add(new Mono.Security.X509.X509Certificate(x509.RawData));
#endif
                if ( verbose > 0 )
                {
                    Console.WriteLine(x509);
                }
                else
                {
                    Console.WriteLine(x509.Subject);
                }
                };
#if HAVE_MONO_X509
                Mono.Security.X509.X509Certificate x = new Mono.Security.X509.X509Certificate(cms.SignerInfos[0].Certificate.RawData);
                Mono.Security.X509.X509Certificate parent = x;
                while (x != null) { // Self-signed is fine - the font bundled CA is self-signed.
                    parent = x; // last valid
                    x = FindCertificateParent (x);
                    if (x != null && x.Equals(parent))
                        break;
                }
#endif
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
                StringBuilder hexLine_sig = new StringBuilder ();
                for (int i = 0; i < Value.Length; i++) {
                    hexLine_sig.AppendFormat ("{0} ", Value [i].ToString ("X2"));
                }
                hexLine_sig.AppendFormat (Environment.NewLine);

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
                StringBuilder hexLine = new StringBuilder ();
                for (int i = 0; i < cdigest.Length; i++) {
                    hexLine.AppendFormat ("{0} ", cdigest [i].ToString ("X2"));
                }
                hexLine.AppendFormat (Environment.NewLine);
                Console.WriteLine("{0} Signed Digest: {1}", algoname.ToUpper(), hexLine_sig);
                Console.WriteLine("Calculated Digest: {0}", hexLine);
                string root_thumb = "";
#if HAVE_MONO_X509
                root_thumb =
                    (new System.Security.Cryptography.X509Certificates.X509Certificate2(parent.RawData)).Thumbprint;
                Console.WriteLine("ChainEnd Name: {0}", parent.SubjectName);
#endif
                Console.WriteLine("ChainEnd: {0}", root_thumb);
                bool trusted = false;
                try
                {
                    string root_id = trusted_roots[root_thumb];
                    Console.WriteLine("RootID: {0}", root_id);
                    trusted = true;
                }
                catch (KeyNotFoundException e)
                {}
                Console.WriteLine("Trusted: {0}", trusted);
            }

            return 0;
        }

#if HAVE_MONO_X509
        private static Mono.Security.X509.X509Certificate FindCertificateParent (Mono.Security.X509.X509Certificate child)
        {
            foreach (Mono.Security.X509.X509Certificate potentialParent in certs) {
                if (IsParent (child, potentialParent))
                    return potentialParent;
            }
            return null;
        }

        private static bool IsParent (Mono.Security.X509.X509Certificate child, Mono.Security.X509.X509Certificate parent)
        {
            if (child.IssuerName != parent.SubjectName)
                return false;

            if (child.VerifySignature (parent.RSA) || child.VerifySignature (parent.DSA)) {
                return true;
            }

            return true;
        }
#endif

        public class TTCHBuffer
        {
            private byte[] buf;
            private int offset;

            public TTCHBuffer (uint size)
            {
                buf = new byte[size];
                offset = 0;
            }

            public void FillUint32MBO(uint n)
            {
                buf[offset] = (byte)(n >> 24);
                buf[offset+1] = (byte)(n >> 16);
                buf[offset+2] = (byte)(n >> 8);
                buf[offset+3] = (byte)n;

                offset +=4;
                return;
            }

            public byte[] buffer
            {
                get {
                    return buf;
                }
            }
        }

        public byte[] get_TTC_digest()
        {
            return get_TTC_digest(this.fontfile);
        }

        static byte[] get_TTC_digest( OTFile f )
        {
            TTCHeader ttc = f.GetTTCHeader();

            uint back_shift = 12;
            //if ( ttc.version == 0x00020000 )
                back_shift = 0;

            TTCHBuffer ttcb = new TTCHBuffer ( 12 + ttc.DirectoryCount * 4 + 12 - back_shift );

            // write the TTC header
            ttcb.FillUint32MBO((uint) ttc.TTCTag);
            ttcb.FillUint32MBO(ttc.version);
            ttcb.FillUint32MBO(ttc.DirectoryCount);

            for (int i=0; i< f.GetNumFonts() ; i++)
            {
                ttcb.FillUint32MBO( (uint) ttc.DirectoryOffsets[i] - back_shift);
            }
            //if ( ttc.version == 0x00020000 )
            {
                ttcb.FillUint32MBO( 0 );
                ttcb.FillUint32MBO( 0 );
                ttcb.FillUint32MBO( 0 );
            }
            hash.TransformBlock ( ttcb.buffer, 0, ttcb.buffer.Length, ttcb.buffer, 0 );

            // build an array of offset tables
            OffsetTable[] otArr = new OffsetTable[f.GetNumFonts()];
            for (uint iFont=0; iFont<f.GetNumFonts(); iFont++)
            {
                otArr[iFont] = new OffsetTable(new OTFixed(1,0), f.GetFont(iFont).GetNumTables());

                for (ushort i=0; i<f.GetFont(iFont).GetNumTables(); i++) {
                    DirectoryEntry old = f.GetFont(iFont).GetDirectoryEntry(i);
                    DirectoryEntry de = new DirectoryEntry(old);
                    de.offset -= back_shift;
                    otArr[iFont].DirectoryEntries.Add(de);
                }
            }

            if ( (uint) ttc.DirectoryOffsets[(int)f.GetNumFonts()-1] < f.GetFont(0).GetDirectoryEntry(0).offset )
            {
                // assume directory-contiguous
                for (uint iFont=0; iFont<f.GetNumFonts(); iFont++)
                {
                    // write the offset table
                    hash.TransformBlock (otArr[iFont].m_buf.GetBuffer(), 0, (int)otArr[iFont].m_buf.GetLength(),
                                         otArr[iFont].m_buf.GetBuffer(), 0);

                    // write the directory entries
                    for (int i=0; i<f.GetFont(iFont).GetNumTables(); i++)
                    {
                        DirectoryEntry de = (DirectoryEntry)otArr[iFont].DirectoryEntries[i];
                        hash.TransformBlock (de.m_buf.GetBuffer(), 0, (int)de.m_buf.GetLength(),
                                             de.m_buf.GetBuffer(), 0 );
                    }
                }
            }

            // write out each font
            uint PrevPos = 0;
            for (uint iFont=0; iFont<f.GetNumFonts(); iFont++)
            {
                ushort numTables = f.GetFont(iFont).GetNumTables();

                if ( (uint) ttc.DirectoryOffsets[(int)f.GetNumFonts()-1] > f.GetFont(0).GetDirectoryEntry(0).offset )
                {
                    // assume font-contiguous
                    // write the offset table
                    hash.TransformBlock (otArr[iFont].m_buf.GetBuffer(), 0, (int)otArr[iFont].m_buf.GetLength(),
                                         otArr[iFont].m_buf.GetBuffer(), 0 );

                    // write the directory entries
                    for (int i=0; i<numTables; i++)
                    {
                        DirectoryEntry de = (DirectoryEntry)otArr[iFont].DirectoryEntries[i];
                        hash.TransformBlock (de.m_buf.GetBuffer(), 0, (int)de.m_buf.GetLength(),
                                             de.m_buf.GetBuffer(), 0 );
                    }
                }

                // write out each table unless a shared version has been written
                for (ushort i=0; i<numTables; i++)
                {
                    DirectoryEntry de = (DirectoryEntry)otArr[iFont].DirectoryEntries[i];
                    if (PrevPos < de.offset) //crude
                    {
                        OTTable table = f.GetFont(iFont).GetTable(de.tag);
                        hash.TransformBlock (table.m_bufTable.GetBuffer(), 0, (int)table.GetBuffer().GetPaddedLength(),
                                             table.m_bufTable.GetBuffer(), 0 );
                        PrevPos = de.offset;
                    }
                }
            }

            byte[] usFlag = {0, 1};
            hash.TransformFinalBlock(usFlag, 0,2);
            return hash.Hash;
        }

        public byte[] get_TTF_digest()
        {
            return get_TTF_digest(this.fontfile);
        }

        static byte[] get_TTF_digest( OTFile f )
        {
            OTFont fn = f.GetFont(0);
            Table_DSIG tDSIG = (Table_DSIG) fn.GetTable("DSIG");
            DirectoryEntry deDSIG = null;
            // sort table by offset
            Dictionary<uint, int> offsetlookup = new Dictionary<uint, int>();
            var list = new List<uint>();
            for (ushort i=0; i<fn.GetNumTables(); i++) {
                DirectoryEntry de = fn.GetDirectoryEntry(i);
                offsetlookup.Add(de.offset, i);
                list.Add( de.offset );
                if ((string) de.tag == "DSIG" )
                    deDSIG = de;
            }
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
