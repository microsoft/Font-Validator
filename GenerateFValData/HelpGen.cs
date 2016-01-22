using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace helpgen {

    public class HelpItemXMLFile {
        string m_xmlPath;
        string m_xsdPath;
        string m_nameSpace;
        bool   m_validationError = false;

        public HelpItemXMLFile( string xmlPath, 
                                string xsdPath,
                                string nameSpace ) {
            m_xmlPath = xmlPath;
            m_xsdPath = xsdPath;
            m_nameSpace = nameSpace;
        }

        public XmlDocument GetXMLDoc( String xmlPath )
        {
            XmlDocument document = new XmlDocument();
            document.Load( xmlPath );
            return document;
        }

        public XPathNavigator ValidateXMLDoc( XmlDocument doc,
                                              String xsdPath )
        {
            XPathNavigator nav = doc.CreateNavigator();
            doc.Schemas.Add( null, xsdPath );
            ValidationEventHandler validation = 
                new ValidationEventHandler( SchemaValidationHandler );
            // G.CO( "Will validate document" );
            doc.Validate( validation );
            if ( m_validationError ) {
                return null;
            }
            // G.CO( "Validated document" );
            return nav;
        }

        void SchemaValidationHandler( object sender, 
                                      ValidationEventArgs e )
        {
            switch (e.Severity) {
            case XmlSeverityType.Error:
                Console.WriteLine("Schema Validation Error: {0}", e.Message);
                m_validationError = true;
                break;
            case XmlSeverityType.Warning:
                Console.WriteLine("Schema Validation Warning: {0}", e.Message);
                break;
            default:
                Console.WriteLine("Schema Validation something: {0}", 
                                  e.Message);
                m_validationError = true;
                break;
            }
        }

        public List<HelpItem> ReadHelpItems()
        {
            List<HelpItem> items = new List<HelpItem>();
            XmlDocument xmlDoc = GetXMLDoc( m_xmlPath );
            XPathNavigator nav = ValidateXMLDoc( xmlDoc, m_xsdPath );
            if ( null == nav ) {
                return items;
            }
            // G.CO( "Validated XML" );
            //Instantiate an XmlNamespaceManager object. 
            XmlNamespaceManager nsmgr = new 
                XmlNamespaceManager( xmlDoc.NameTable );
            nsmgr.AddNamespace( "foo", m_nameSpace );

            string q = "foo:helpitems/*";
            XPathNodeIterator it = nav.Select( q, nsmgr );

            while ( it.MoveNext() ) {
                XPathNavigator node = it.Current;
                HelpItem hi = new HelpItem();
                hi.atom = XG.StrAttrReq( node, "atom" );
                hi.helpID = XG.StrAttrReq( node, "helpID" );
                hi.linkTitle = XG.StrAttrReq( node, "linkTitle" );
                hi.specLink = XG.StrAttrReq( node, "specLink" );
                hi.shortDesc = XG.StrAttrReq( node, "shortDesc" );
                hi.tableName = XG.StrAttrReq( node, "tableName" );
                hi.fwLinkID = XG.IntAttrReq( node, "fwLinkID" );
                XPathNavigator pNode = 
                    XG.SingletonNodeReq(node,"foo:problem",nsmgr);
                hi.problem = pNode.Value;
                items.Add( hi );
            }
            return items;
        }


    }

    public class HelpGen {

        static string s_valstringsTemplate = 
@"  <data name=""##ATOM##"" xml:space=""preserve"">
    <value>##HELPID##: ##SHORTDESC##</value>
  </data>
";

        public static void WriteFile( String path, String contents ) {
            TextWriter tw = new StreamWriter( path );
            tw.Write( contents );
            tw.Close();
        }

        static void Usage() {
            String cname = ( new HelpGen() ).GetType().Name.ToString();
            G.CO( "Usage: " + cname + " [options and args]" );
            G.CO( "" );

            //Order of appearance in code
            G.CO( "Options:" );
            G.CO( "-xml               <xmlPath>" );
            G.CO( "-xsd               <xsdPath>" );
            G.CO( "-template          <templatePath>" );
            G.CO( "-contents-template <contentsTemplatePath>" );
            G.CO( "-outdir            <helpDir>" );
            G.CO( "-tmpdir            <tmpDir>" );
            G.CO( "-inputdir          <inputDir>" );
            G.CO( "-gendir            <genDir>" );
            G.CO( "-verbose" );
            G.CO( "-u/-usage/-h/-help/--help" );
            G.CO( "" );
            G.CO( "All the options are optional." );
        }

        static string DoReplacements( string template, HelpItem hi )
        {
            string s = template;
            s = Regex.Replace( s, "##ATOM##", hi.atom );
            s = Regex.Replace( s, "##HELPID##", hi.helpID );
            s = Regex.Replace( s, "##SHORTDESC##", 
                               HttpUtility.HtmlEncode( hi.shortDesc ) );
            s = Regex.Replace( s, "##LINKTITLE##", hi.linkTitle );
            s = Regex.Replace( s, "##PROBLEM##", hi.problem );
            s = Regex.Replace( s, "##FWLINKID##", ""+hi.fwLinkID );
            s = Regex.Replace( s, "##TABLENAME##", ""+hi.tableName );
            return s;
        }


        static void WriteAtomsPreamble( TextWriter s ) 
        {
            s.WriteLine( "using System;" );
            s.WriteLine( "using System.Diagnostics;" );
            s.WriteLine( "using OTFontFile;" );
            s.WriteLine( "namespace OTFontFileVal {" );
        }

        static void WriteAtomsPostamble( TextWriter s )
        {
            s.WriteLine( "}" );
        }

        static int AppendTextFileToStream( TextWriter outs,
                                           string path )
        {
            outs.Write( TextFile.AsString( path ) );
            return 0;
        }

        static List<HelpItem> GetHelpItemsFromXMLFile( string xmlPath,
                                                       string xsdPath )
        {
            HelpItemXMLFile hx = new HelpItemXMLFile( xmlPath, xsdPath, 
                                                      "urn:helpgen" );
            return hx.ReadHelpItems();
        }

        static void FixBadAtomNames( List<HelpItem> items )
        {
            foreach ( HelpItem hi in items ) {
                string sbefore = hi.atom;
                hi.atom = hi.atom.Replace( '/', '_' );
                if ( sbefore != hi.atom ) {
                    G.CO( "Warning: Atom '"+sbefore+"' fixed" );
                }
            }

        }

        static int CheckForDuplicateKeys( List<HelpItem> items )
        {
            // Check for duplicate items
            List<string> helpIDs = new List<string>();
            List<string> atoms = new List<string>();
            int ndup = 0;
            foreach ( HelpItem hi in items ) {
                if ( helpIDs.Contains( hi.helpID ) ) {
                    G.CO( "Error: Duplicate entries for helpID \""+hi.helpID+
                          "\"" );
                    ndup++;
                }
                helpIDs.Add( hi.helpID );
                if ( atoms.Contains( hi.atom ) ) {
                    G.CO( "Error: Duplicate entries for atom \""+hi.atom+
                          "\"" );
                    ndup++;
                }
                atoms.Add( hi.atom );                
            }            
            return ndup;
        }

        static string HelpFileName( string helpDir, string atom )
        {
            return helpDir + atom + ".htm";
        }
                                    

        static void WriteHelpProject( List<HelpItem> items, 
                                      string inputDir,
                                      string helpprojTemp )
        {
            TextWriter s = new StreamWriter( helpprojTemp );
            AppendTextFileToStream( s, inputDir+"help-proj-preamble.txt" );
            foreach ( HelpItem hi in items ) {
                try
                {
                    if ( 'P' != hi.helpID[0] &&
                         "ERROR" != hi.problem.Substring(0,5) ) {
                        s.WriteLine( HelpFileName( "", hi.helpID ) );
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("<problem/> description must not be empty.");
                    throw;
                }
            }
            AppendTextFileToStream( s, inputDir+"help-proj-postamble.txt" );
            s.Close();
        }



        static void WriteHelpTableOfContents( List<HelpItem> items, 
                                              string contentsTemplatePath, 
                                              string inputDir,
                                              string tocTempOut )
        {
            string contentsTemplate = TextFile.AsString( contentsTemplatePath );
            TextWriter s = new StreamWriter( tocTempOut );
            AppendTextFileToStream( s, inputDir+"toc-preamble.txt" );
            AppendTextFileToStream( s, inputDir+"toc-error-preamble.txt" );
            foreach ( HelpItem hi in items ) {
                if ( ( 'E' == hi.helpID[0] ||
                       'A' == hi.helpID[0] ) && 
                     "ERROR" != hi.problem.Substring(0,5) ) {
                    string cs = DoReplacements( contentsTemplate, hi );
                    s.Write( cs );
                }
            }
            AppendTextFileToStream( s, inputDir+"toc-error-postamble.txt" );
            AppendTextFileToStream( s, inputDir+"toc-info-preamble.txt" );
            foreach ( HelpItem hi in items ) {
                if ( 'I' == hi.helpID[0] && 
                     "ERROR" != hi.problem.Substring(0,5) ) {
                    string cs = DoReplacements( contentsTemplate, hi );
                    s.Write( cs );
                }
            }
            AppendTextFileToStream( s, inputDir+"toc-info-postamble.txt" );
            AppendTextFileToStream( s, inputDir+"toc-warning-preamble.txt" );
            foreach ( HelpItem hi in items ) {
                if ( 'W' == hi.helpID[0] && 
                     "ERROR" != hi.problem.Substring(0,5) ) {
                    string cs = DoReplacements( contentsTemplate, hi );
                    s.Write( cs );
                }
            }
            AppendTextFileToStream( s, inputDir+"toc-warning-postamble.txt" );
            AppendTextFileToStream( s, inputDir+"toc-postamble.txt" );
            s.Close();
        }


        static void WriteHelpFiles( List<HelpItem> items, 
                                    string templatePath, 
                                    string tmpDir,
                                    string helpDir )
        {
            string template = TextFile.AsString( templatePath );

            foreach ( HelpItem hi in items ) {
                if ( 'P' != hi.helpID[0] &&
                     "ERROR" != hi.problem.Substring(0,5) ) {
                    string s = DoReplacements( template, hi );
                    string fname = HelpFileName( helpDir, hi.helpID );
                    WriteFile( fname, s );
                }
            }
        }

        static void WriteValStrings( List<HelpItem> items, 
                                     string inputDir,
                                     string valstringsTemp )
        {
            TextWriter vsw = new StreamWriter( valstringsTemp );
            AppendTextFileToStream( vsw, inputDir+"valstrings-preamble.txt" );

            foreach ( HelpItem hi in items ) {
                string vs = DoReplacements( s_valstringsTemplate, hi );
                vsw.Write( vs );
            }
            AppendTextFileToStream( vsw, inputDir+"valstrings-postamble.txt" );
            vsw.Close();
        }

        static void AddAtomSubset( TextWriter stm,
                                   List<HelpItem> items,
                                   char startChar )
        {
            foreach ( HelpItem hi in items ) {
                if ( startChar == hi.helpID[0] ) {
                    stm.WriteLine( "        " + hi.atom + "," );
                }
            }
        }

        static void WriteAtomsFile( List<HelpItem> items,
                                    string atomsTemp )
        {
            TextWriter esw = new StreamWriter( atomsTemp );
            WriteAtomsPreamble( esw );

            esw.WriteLine( "    public enum W {" ); // Warnings
            AddAtomSubset( esw, items, 'W' );
            esw.WriteLine( "    }" );

            esw.WriteLine( "    public enum E {" ); // Errors
            AddAtomSubset( esw, items, 'A' );
            AddAtomSubset( esw, items, 'E' );
            esw.WriteLine( "    }" );

            esw.WriteLine( "    public enum I {" ); // Info
            AddAtomSubset( esw, items, 'I' );
            esw.WriteLine( "    }" );

            esw.WriteLine( "    public enum P {" ); // Pass
            AddAtomSubset( esw, items, 'P' );
            esw.WriteLine( "    }" );

            esw.WriteLine( "    public enum T {" ); // Test Names
            esw.WriteLine( "        " + "T_NULL" + "," );
            for ( int i = 0; i < ValTests.s_testNames.Length; i++ ) {
                string s = ValTests.s_testNames[i];
                esw.WriteLine( "        " + s + "," );
            }
            esw.WriteLine( "    }" );

            WriteAtomsPostamble( esw );
            esw.Close();
        }

        static void CopyIfDifferent( string src, string dst, bool verbose )
        {
            if ( !File.Exists( dst ) ||
                 !G.FileCompare( src, dst ) ) {
                G.CO( "Copying " + src + " to " + dst );
                File.Copy( src, dst, true );
            } else {
                if ( verbose ) {
                    G.CO( "SAME: Files "+src+" and "+dst );
                }
            }
        }


        static void CopyConstantHelpFiles( string inputDir, 
                                           string helpDir,
                                           bool verbose )
        {
            string [] copyFiles = {
                "Index.hhk",
                "interpretinglog.htm",
                "introtofv.htm",
                "legacy.htm",
                "resources.htm",
                "tooloverview.htm",
                "usingvalidator.htm",
                "startbutton.jpg",
            };
            foreach ( string f in copyFiles ) {
                CopyIfDifferent( inputDir+f, helpDir+f, verbose );
            }
        }

        // Keep all the paths in one place, and provide method to give them 
        // default values if they were not specified already.
        class HelpGenPaths {
            public string inputDir = null;
            public string helpDir = null;
            public string genDir = null;
            public string tmpDir = null;
            public string xmlPath = null; 
            public string xsdPath = null;
            public string templatePath = null;
            public string contentsTemplatePath = null;

            // Fill in all null fields, that is, those that have not 
            // yet been specified, with reasonable values.
            public void FillInDefaults( bool verbose )
            {

                inputDir = (null==inputDir)? "../.." : inputDir;
                genDir = (null==genDir)    ? "../../../OTFontFileVal" : genDir;
                helpDir = (null==helpDir)  ? "../../../NewHelp" : helpDir;
                tmpDir = (null==tmpDir)    ? "." : tmpDir;
                
                inputDir = G.ForceEndingSlash( inputDir );
                genDir   = G.ForceEndingSlash( genDir );
                helpDir  = G.ForceEndingSlash( helpDir );
                tmpDir   = G.ForceEndingSlash( tmpDir );

                if ( verbose ) {
                    G.CO( "Input Dir:          \"" + inputDir + "\"" );
                    G.CO( "Generated File Dir: \"" + genDir + "\"" );
                    G.CO( "Help Dir:           \"" + helpDir + "\"" );
                    G.CO( "Temp Dir:           \"" + tmpDir + "\"" );
                }
                
                if ( null == xmlPath ) {
                    xmlPath = inputDir + "OurData.xml";
                }
                if ( null == xsdPath ) {
                    xsdPath = inputDir + "helpitems.xsd";
                }
                if ( null == templatePath ) {
                    templatePath = inputDir + "template.txt";
                }
                if ( null == contentsTemplatePath ) {
                    contentsTemplatePath = inputDir + "template-contents.txt";
                }
            }
        }

        static int DoWork( HelpGenPaths ps, bool verbose )
        {
            List<HelpItem> items = GetHelpItemsFromXMLFile( ps.xmlPath,
                                                            ps.xsdPath );
            items.Sort(delegate(HelpItem x, HelpItem y)
                       {
                           return x.helpID.CompareTo(y.helpID);
                       });
            if ( null == items ) {
                return 1;
            }

            FixBadAtomNames( items );
            if ( CheckForDuplicateKeys( items ) > 0 ) {
                return 1;
            }

            string helpProjectTemp = ps.tmpDir + "help-proj.txt";
            WriteHelpProject( items, ps.inputDir, helpProjectTemp );
            CopyIfDifferent( helpProjectTemp, 
                             ps.helpDir + "FontValidatorHelp.hhp",
                             verbose );

            string helpTOCTemp = ps.tmpDir + "help-toc.txt";
            WriteHelpTableOfContents( items, 
                                      ps.contentsTemplatePath, 
                                      ps.inputDir, 
                                      helpTOCTemp );
            CopyIfDifferent( helpTOCTemp, 
                             ps.helpDir+"Table of Contents.hhc",
                             verbose );

            CopyConstantHelpFiles( ps.inputDir, ps.helpDir, verbose );
            WriteHelpFiles( items, ps.templatePath, ps.tmpDir, ps.helpDir );

            string valstringsTemp = ps.tmpDir + "OTFontFileVal.ValStrings.noresx";
            WriteValStrings( items, ps.inputDir, valstringsTemp );
            CopyIfDifferent( valstringsTemp, ps.genDir + "OTFontFileVal.ValStrings.resx",
                             verbose );

            string atomsTemp = ps.tmpDir + "atoms.notcs";
            WriteAtomsFile( items, atomsTemp );
            CopyIfDifferent( atomsTemp, ps.genDir + "atoms.cs", verbose );

            return 0;
        }

        static int Main( string [] args ) {

            HelpGenPaths ps = new HelpGenPaths();

            bool err = false;
            bool usage = false;
            int iarg = 0;
            bool verbose = false;

            for ( iarg = 0; iarg < args.Length; iarg++ ) {
                if ( G.StrMatch( "-xml", args[iarg] ) ) {
                    iarg++;
                    ps.xmlPath = args[iarg];
                }
                else if ( G.StrMatch( "-xsd", args[iarg] ) ) {
                    iarg++;
                    ps.xsdPath = args[iarg];
                }
                else if ( G.StrMatch( "-template", args[iarg] ) ) {
                    iarg++;
                    ps.templatePath = args[iarg];
                }
                else if ( G.StrMatch( "-contents-template", args[iarg] ) ) {
                    iarg++;
                    ps.contentsTemplatePath = args[iarg];
                }
                else if ( G.StrMatch( "-outdir", args[iarg] ) ) {
                    iarg++;
                    ps.helpDir = args[iarg];
                }
                else if ( G.StrMatch( "-tmpdir", args[iarg] ) ) {
                    iarg++;
                    ps.tmpDir = args[iarg];
                }
                else if ( G.StrMatch( "-inputdir", args[iarg] ) ) {
                    iarg++;
                    ps.inputDir = args[iarg];
                }
                else if ( G.StrMatch( "-gendir", args[iarg] ) ) {
                    iarg++;
                    ps.genDir = args[iarg];
                }
                else if ( G.StrMatch( "-verbose", args[iarg] ) ) {
                    verbose = true;
                }
                else if ( G.StrMatch( "-u", args[iarg] ) ||
                          G.StrMatch( "-usage", args[iarg] ) ||
                          G.StrMatch( "-h", args[iarg] ) ||
                          G.StrMatch( "-help", args[iarg] ) ||
                          G.StrMatch( "--help", args[iarg] ) ) {
                    usage = true;
                }
                else {
                    G.CO( "Unknown arg: " + args[iarg] );
                    err = true;
                }
            }

            if ( usage || err ) {
                Usage();
                return !err? 0 : 1;
            }

            ps.FillInDefaults( verbose );
            return DoWork( ps, verbose );
        }
    } 
    
}
