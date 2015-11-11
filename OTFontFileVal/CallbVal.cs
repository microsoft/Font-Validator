//Simple callback class for the Driver class
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OTFontFile;

namespace OTFontFileVal
{
    public class CallbVal : Driver.DriverCallbacks{

	    public void OnBeginFontTest( String label, int nthFont, int nFonts ){

	    }
	    public void OnBeginTableTest( DirectoryEntry de ){

	    }
	    public void OnBeginRasterTest( String label ){

	    }
	    public void OnTestProgress( Object obj ){

	    }
	    public String GetReportFileName( String fname ){
		    String ret = fname + ".report.xml";
		    return ret;
	    }
	    public void OnOpenReportFile( String sReportFile, String fpath ){

	    }
	    public void OnCloseReportFile( String sReportFile ){
            Driver.CopyXslFile(sReportFile);
	    }
	    public void OnReportsReady(){

	    }
	    public void OnCancel(){

	    }
	    public void OnException( Exception e ){
            if (e.GetType() == typeof(FileNotFoundException))
            {
                FileNotFoundException fileex = (FileNotFoundException) e;
                Console.WriteLine("ERROR: Validation library incomplete. Aborting.");
                Console.WriteLine("Missing file: " + "\"" + fileex.FileName + "\"");
            }
	    }
	    public void OnOTFileValChange( OTFileVal fontFile ){

	    }

    }
}
