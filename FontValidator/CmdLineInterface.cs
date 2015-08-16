using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

using OTFontFile;
using NS_ValCommon;
using OTFontFileVal;

namespace FontValidator
{
    public class CmdLineInterface : Driver.DriverCallbacks
    {
        string []             m_sFiles;
        ReportFileDestination m_ReportFileDestination;
        bool                  m_bOpenReportFiles;
        string                m_sReportFixedDir;
        ValidatorParameters   m_vp;
        OTFileVal             m_curOTFileVal;
        List<string>          m_reportFiles = new List<string>();
        List<string>          m_captions = new List<string>();
        bool m_verbose;

        static void ErrOut( string s ) 
        {
            Console.WriteLine( s );
        }

        static void StdOut( string s )  {
            Console.WriteLine( s );
        }

        static public void CopyXslFile(string sReportFile)
        {
            // Note that this will not work in development because it depends 
            // upon the xsl file existing in the same location as the 
            // executing assembly, which is only true of the deployment package.
            //
            // During development, though, the file FontVal/fval.xsl can be 
            // copied as needed to 
            //     FontVal/bin/Debug or
            //     FontVal/bin/Release
            // and then the source file is found.
            // build the src filename
            string sAssemblyLocation = 
                System.Reflection.Assembly.GetExecutingAssembly().Location;
            FileInfo fi = new FileInfo(sAssemblyLocation);
            string sSrcDir  = fi.DirectoryName;
            string sSrcFile = sSrcDir + Path.DirectorySeparatorChar + "fval.xsl";

            // build the dest filename
            fi = new FileInfo(sReportFile);
            string sDestDir  = fi.DirectoryName;
            string sDestFile = sDestDir + Path.DirectorySeparatorChar + "fval.xsl";

            // copy the file
            try
            {
                File.Copy(sSrcFile, sDestFile, true);
                fi = new FileInfo(sDestFile);
                fi.Attributes = fi.Attributes & ~FileAttributes.ReadOnly;
            }
            catch (Exception)
            {
            }
        }

        // ================================================================
        // Callbacks for Driver.DriverCallbacks interface
        // ================================================================
        public void OnException( Exception e )
        {
            ErrOut( "Error: " + e.Message );
            DeleteTempFiles();
        }
        public void OnReportsReady()
        {
            StdOut( "Reports are ready!" );
        }
        public void OnBeginRasterTest( string label )
        {
            if (m_verbose == true && m_vp.IsTestingRaster())
                StdOut( "Begin Raster Test: " + label );
        }

        public void OnBeginTableTest( DirectoryEntry de )
        {
            string name = ( string )de.tag;
            if (m_verbose == true && m_vp.IsTestingTable(name))
                StdOut( "Table Test: " + name );
        }
        public void OnTestProgress( object oParam )
        {
            string s = ( string )oParam;
            if (s == null) {
                s = "";
            }
            if (m_verbose == true)
                StdOut( "Progress: " + s );
        }

        public void OnCloseReportFile( string sReportFile )
        {
            StdOut( "Complete: " + sReportFile );
            // copy the xsl file to the same directory as the report
            // 
            // This has to be done for each file because the if we are
            // putting the report on the font's directory, there may
            // be a different directory for each font.
            CopyXslFile( sReportFile );
        }

        public void OnOpenReportFile( string sReportFile, string fpath )
        {
            m_captions.Add( fpath );
            m_reportFiles.Add( sReportFile );
        }

        public void OnCancel()
        {
            DeleteTempFiles();
        }

        public void OnOTFileValChange( OTFileVal fontFile )
        {
            m_curOTFileVal = fontFile;
        }

        public string GetReportFileName( string sFontFile )
        {
            string sReportFile = null;
            switch ( m_ReportFileDestination )
            {
                case ReportFileDestination.TempFiles:
                    string sTemp = Path.GetTempFileName();
                    sReportFile = sTemp + ".report.xml";
                    File.Move(sTemp, sReportFile);
                    break;
                case ReportFileDestination.FixedDir:
                    sReportFile = m_sReportFixedDir + Path.DirectorySeparatorChar +
                        Path.GetFileName(sFontFile) + ".report.xml";
                    break;
                case ReportFileDestination.SameDirAsFont:
                    sReportFile = sFontFile + ".report.xml";
                    break;
            }
            return sReportFile;
        }

        public void OnBeginFontTest( string fname, int nth, int nFonts )
        {
            string label = fname + " (file " + (nth+1) + " of " + nFonts + ")";
            StdOut( label );
        }

        public void DeleteTempFiles()
        {
            if ( m_ReportFileDestination == ReportFileDestination.TempFiles )
            {
                for ( int i = 0; i < m_reportFiles.Count; i++ ) {
                    File.Delete( m_reportFiles[i] );
                }
            }
        }

        public int DoIt( )
        {

            Validator v = new Validator();
            m_vp.SetupValidator( v );
            OTFontFileVal.Driver driver = new OTFontFileVal.Driver( this );
            return driver.RunValidation( v, m_sFiles );
        }

        public CmdLineInterface( ValidatorParameters vp, 
                                 string [] sFilenames, 
                                 ReportFileDestination rfd, 
                                 bool bOpenReportFiles, 
                                 string sReportFixedDir,
                                 bool verbose)
        {
            m_vp = vp;
            m_sFiles = sFilenames;
            m_ReportFileDestination = rfd;
            m_bOpenReportFiles = bOpenReportFiles;
            m_sReportFixedDir = sReportFixedDir;
            m_verbose = verbose;
        }

        static void Usage()
        {
            Console.WriteLine( "Usage: FontValidator [options]" );
            Console.WriteLine( "" );
            Console.WriteLine( "Options:" );
            Console.WriteLine( "-file          <fontfile>      (multiple allowed)" );
            Console.WriteLine( "+table         <table-include> (multible allowed)" );
            Console.WriteLine( "-table         <table-skip>    (multiple allowed)" );
            Console.WriteLine( "-all-tables" );
            Console.WriteLine( "-only-tables" );
            Console.WriteLine( "-quiet" );
            Console.WriteLine( "+raster-tests" );
            Console.WriteLine( "-report-dir    <reportDir>" );
            Console.WriteLine( "-report-in-font-dir" );

            Console.WriteLine( "" );
            Console.WriteLine( "Valid table names (note the space after \"CFF \" and \"cvt \"):" );

            string [] allTables = TableManager.GetKnownOTTableTypes();
            Console.Write(allTables[0]);
            for ( int k = 1; k < allTables.Length; k++ )
                Console.Write(",{0}", allTables[k]);
            Console.WriteLine( "" );
            Console.WriteLine( "" );

            Console.WriteLine( "Example:" );
            Console.WriteLine( "  FontValidator -file arial.ttf -file times.ttf -table 'OS/2' -table DSIG -report-dir /tmp");
        }

        static int Main( string[] args )
        {
            bool err = false;
            bool verbose = true;
            string reportDir = null;
            ReportFileDestination rfd = ReportFileDestination.TempFiles;
            List<string> sFileList = new List<string>();
            ValidatorParameters vp = new ValidatorParameters();
            
            if (args.Length == 0) {
                Usage();
                return 0;
            }

            for ( int i = 0; i < args.Length; i++ ) {
                if ( "-file" == args[i] ) {
                    i++;
                    if ( i < args.Length ) {
                        sFileList.Add( args[i] );
                    } else {
                        ErrOut( "Argument required for \"" + args[i-1] + "\"" );
                        err = true;
                    }
                }
                else if ( "+table" == args[i] ) {
                    i++;
                    if ( i < args.Length ) {
                        vp.AddTable( args[i] );
                    } else {
                        ErrOut( "Argument required for \"" + args[i-1] + "\"" );
                        err = true;
                    }
                }
                else if ( "-table" == args[i] ) {
                    i++;
                    if ( i < args.Length ) {
                        int n = vp.RemoveTableFromList( args[i] );
                        if ( 0 == n ) {
                            ErrOut( "Table \"" + args[i] + "\" not found" );
                            err = true;
                        }
                    } else {
                        ErrOut( "Argument required for \"" + args[i-1] + "\"" );
                        err = true;
                    }
                }
                else if ( "-all-tables" == args[i] ) {
                    vp.SetAllTables();
                }
                else if ( "-only-tables" == args[i] ) {
                    vp.ClearTables();
                }
                else if ( "-quiet" == args[i] ) {
                    verbose = false;
                }
                else if ( "+raster-tests" == args[i] ) {
                    vp.SetRasterTesting();
                }
                else if ( "-report-dir" == args[i] ) {
                    i++;
                    if ( i < args.Length ) {
                        reportDir = args[i];
                        rfd = ReportFileDestination.FixedDir;
                    } else {
                        ErrOut( "Argument required for \"" + args[i-1] + "\"" );
                        err = true;
                    }
                    
                }
                else if ( "-report-in-font-dir" == args[i] ) {
                    rfd = ReportFileDestination.SameDirAsFont;
                }
                else {
                    ErrOut( "Unknown argument: \"" + args[i] + "\"" );
                    err = true;
                }
            }
            if ( err ) {
                Usage();
                return 1;
            }

            CmdLineInterface cmd = new 
                CmdLineInterface( vp, sFileList.ToArray(), rfd, false, 
                                  reportDir , verbose);
            return cmd.DoIt();
        }
    }
}
