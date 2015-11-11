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

    /// <summary>Short-hand utility functions</summary>
    static public class G {
        // FIX! There is probably a clean varargs way to overload this.
        static public void CO( String a ) { Console.WriteLine( a ); }
        static public void CO( String a, String b ) { 
            Console.WriteLine( a, b ); 
        }
        static public void CO( String a, String b, String c ) { 
            Console.WriteLine( a, b, c ); 
        }

        static public bool StrMatch( String a, String b ) {
            return String.Equals( a, b, StringComparison.OrdinalIgnoreCase );
        }

        public static string ForceEndingSlash( string s )
        {
           char lastChar = s[ s.Length - 1 ];
            if ( '/' != lastChar &&
                 '\\' != lastChar ) {
                s += '/';
            }
            return s;
        }


        // From http://support.microsoft.com/kb/320348
        // This method accepts two strings the represent two files to 
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the 
        // files are not the same.
        public static bool FileCompare( string file1, string file2 )
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;
            
            // Determine if the same file was referenced two times.
            if ( file1 == file2 ) {
                // Return true to indicate that the files are the same.
                return true;
            }
            
            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);
            
            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length) {
                // Close the file
                fs1.Close();
                fs2.Close();
                    
                // Return false to indicate files are different
                return false;
            }
            
            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ( (file1byte == file2byte) && (file1byte != -1) );
            
            // Close the files.
            fs1.Close();
            fs2.Close();
            
            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }
    }

    static public class XG {
        public static String StrAttrReq( XPathNavigator node, String attr )
        {
            String s1 = node.GetAttribute( attr, "" );
            if ( String.Empty == s1 ) {
                throw new 
                    ApplicationException( "Required string attribute " + attr + 
                                          " not found" );
            }
            return s1;
        }


        public static int IntAttrReq( XPathNavigator node, String attr )
        {
            String s1 = node.GetAttribute( attr, "" );
            if ( String.Empty == s1 ) {
                throw new
                    ApplicationException( "Required int attribute " + attr + 
                                          " not found" );
            }
            return int.Parse( s1 );
        }

        public static uint UintAttrReq( XPathNavigator node, String attr )
        {
            String s1 = node.GetAttribute( attr, "" );
            if ( String.Empty == s1 ) {
                throw new
                    ApplicationException( "Required uint attribute " + attr + 
                                          " not found" );
            }
            return uint.Parse( s1 );
        }

        public static uint HexAttrReq( XPathNavigator node, String attr )
        {
            String s1 = node.GetAttribute( attr, "" );
            if ( String.Empty == s1 ) {
                throw new
                    ApplicationException( "Required hex attribute " + attr + 
                                          " not found" );
            }
            return ( uint )Convert.ToInt32( s1, 16 );
        }

        public static XPathNavigator GetXMLDoc( String path )
        {
            XPathDocument docNav = new XPathDocument( path );
            return docNav.CreateNavigator();
        }

        public static XPathNavigator SingletonNodeReq( XPathNavigator node,
                                                       String         name )
        {
            XPathNodeIterator iter = node.Select( name );
            if ( iter.Count != 1 ) {
                throw new 
                    ApplicationException( "Must be one " + name );
                }
            iter.MoveNext();
            return iter.Current;
        }

        public static XPathNavigator 
        SingletonNodeReq( XPathNavigator node,
                          String         name,
                          XmlNamespaceManager nsmgr )
        {
            XPathNodeIterator iter = node.Select( name, nsmgr );
            if ( iter.Count != 1 ) {
                throw new 
                    ApplicationException( "Must be one " + name );
                }
            iter.MoveNext();
            return iter.Current;
        }

    }

    public class TextFile {
        
        private string m_text;

        public TextFile( string path ) {
            using (StreamReader sr = new StreamReader( path )) {
                m_text = sr.ReadToEnd();
            }
        }

        public static void WriteFile( String path, String contents ) {
            TextWriter tw = new StreamWriter( path );
            tw.Write( contents );
            tw.Close();
        }

        public string Text { get { return m_text; } }

        public static string AsString( string path )
        {
            TextFile tf = new TextFile( path );
            return tf.Text;
        }
    }

    public class HelpItem {
        public string helpID;
        public string atom;
        public string linkTitle;
        public string specLink;
        public string shortDesc;
        public string tableName;
        public int    fwLinkID;
        public string problem;
        public int mark = 0; // For apps to use as they see fit.

        public HelpItem() {}

        public HelpItem( string aatom,
                         string ahelpID,
                         string alinkTitle,
                         string aspecLink,
                         string ashortDesc,
                         string atableName,
                         int    afwLinkID,
                         string aproblem ) 
        {
            atom = aatom;
            helpID = ahelpID;
            linkTitle = alinkTitle;
            specLink = aspecLink;
            shortDesc = ashortDesc;
            tableName = atableName;
            fwLinkID = afwLinkID;
            problem = aproblem;
        }

        private string Option( string s ) {
            return ( null == s )? "WARNING: field missing" : s;
        }

        private string Required( string fieldName, string s ) {
            if ( null == s ) {
                G.CO( helpID + ": Required field '" + fieldName + "' missing" );
                return "ERROR: Required field missing";
            }
            return s;
        }

        public void Write( TextWriter tw ) 
        {
            String name = "item";
            String sd = Required("shortDesc",shortDesc);
            String prob = Required("problem",problem);
            tw.WriteLine( "<" + name );
            tw.WriteLine( "    helpID=\"" +
                          Required("helpID",helpID) + "\"" );
            tw.WriteLine( "    atom=\"" +
                          Required("atom",atom) + "\"" );
            tw.WriteLine( "    linkTitle=\"" + Option(linkTitle) + "\""  );
            tw.WriteLine( "    specLink=\"" + Option(specLink) + "\"" );
            tw.WriteLine( "    shortDesc=\"" + 
                          HttpUtility.HtmlEncode( sd ) + "\"" );
            tw.WriteLine( "    fwLinkID=\"" + fwLinkID  + "\"" );
            tw.WriteLine( "    tableName=\"" + Option(tableName) + "\">" );
            if ( 'P' == helpID[0] ) {
                tw.WriteLine( "    <problem/>" );
            } else {
                tw.WriteLine( "    <problem>" +
                              HttpUtility.HtmlEncode( prob ) + 
                              "</problem>" );
            }
            tw.WriteLine( "</" + name + ">" );
            tw.WriteLine( "" );
        }

        public void Print( string name )
        {
            G.CO( "<" + name );
            G.CO( "  helpID=\"" + helpID + "\"" );
            G.CO( "  atom=" + atom );
            G.CO( "  linkTitle=" + linkTitle );
            G.CO( "  specLink=" + specLink );
            G.CO( "  shortDesc=\"" + shortDesc + "\"" );
            G.CO( "  fwLinkID=" + fwLinkID );
            G.CO( "  tableName=\"" + tableName + "\">" );
            G.CO( "    <problem>" + problem + "</problem>" );
            G.CO( "</" + name + ">" );
        }

    }


}