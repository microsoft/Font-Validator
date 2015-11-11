using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

using Microsoft.Win32.SafeHandles;

using OTFontFile;
using NS_ValCommon;

namespace OTFontFileVal {

    // This enum is temporarily here. Later it will be in the serializable
    // parameters class that will be used to drive the engine and store state
    // across runs.
    public enum ReportFileDestination {
        TempFiles,
        FixedDir,
        SameDirAsFont
    }


    /// <summary>
    /// In order that the command line interface and the GUI app have the
    /// same behavior, this class has been factored out and should be used
    /// by both front ends.
    /// <p/>
    /// Behavior specific to a font end, for example, displaying messages
    /// in a GUI window, should be handled via callbacks through an interface
    /// passed in here.
    /// </summary>
    public class Driver {

        public static void CopyXslFile(string sReportFile)
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
            string sSrcDir = fi.DirectoryName;
            string sSrcFile = sSrcDir + Path.DirectorySeparatorChar + "fval.xsl";

            // build the dest filename
            fi = new FileInfo(sReportFile);
            string sDestDir = fi.DirectoryName;
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

        /// <summary>Callback routines that must be provided by any front
        /// end to the validator.
        /// </summary>
        public interface DriverCallbacks {
            void OnBeginFontTest( string label, int nthFont, int nFonts );
            void OnBeginTableTest( DirectoryEntry de );
            void OnBeginRasterTest( string label );
            void OnTestProgress( object obj );
            string GetReportFileName( string fpath );
            void OnOpenReportFile( string sReportFile, string fpath );
            void OnCloseReportFile( string sReportFile );
            void OnReportsReady();
            void OnCancel();
            void OnException( Exception e );
            void OnOTFileValChange( OTFileVal fontFile );
        }

        DriverCallbacks       m_callbacks;
        XmlTextWriter         m_xmlWriter;


        public Driver( DriverCallbacks cbs )
        {
            m_callbacks = cbs;
        }

        private OTTag MakeCodeFriendlyTag(OTTag tag)
        {
            OTTag friendlyTag = new OTTag(tag.GetBytes());
            byte[] tagbuf = friendlyTag.GetBytes();
            for (uint i=0; i<tagbuf.Length; i++)
            {
                // convert any high order characters to underlines
                if (tagbuf[i] > 127)
                {
                    tagbuf[i] = (byte)'_';
                }
                // convert any control characters to underlines
                if (tagbuf[i] < 32)
                {
                    tagbuf[i] = (byte)'_';
                }
                // convert any spaces to underlines
                if (tagbuf[i] == (byte)' ')
                {
                    tagbuf[i] = (byte)'_';
                }
                // convert any slashes to underlines
                if (tagbuf[i] == (byte)'/')
                {
                    tagbuf[i] = (byte)'_';
                }
            }
            
            return friendlyTag;
        }


        private void OnBeginTable( DirectoryEntry de )
        {
            m_xmlWriter.WriteStartElement("TableEntry");
            m_xmlWriter.WriteAttributeString("Tag", de.tag);
            m_xmlWriter.WriteAttributeString("Offset", 
                                             "0x"+de.offset.ToString("x8"));
            m_xmlWriter.WriteAttributeString("Length", 
                                             "0x"+de.length.ToString("x8"));
            m_xmlWriter.WriteAttributeString("Checksum", 
                                             "0x"+de.checkSum.ToString("x8"));
            m_xmlWriter.WriteAttributeString("CodeFriendlyTag", 
                                             MakeCodeFriendlyTag(de.tag));
            m_xmlWriter.WriteWhitespace("\r\n");
            
            m_callbacks.OnBeginTableTest( de );
        }

        private void OnFontParsed( OTFont f ) 
        {
            string sFontName = f.GetFontName();
            string sFontVersion = f.GetFontVersion();
            
            m_xmlWriter.WriteStartElement("FontInfo");
            if (sFontName != null) {
                m_xmlWriter.WriteAttributeString("FontName", 
                                                 sFontName);
            } else {
                m_xmlWriter.WriteAttributeString("FontName", 
                                                 "BAD FONT NAME");
            }
            
            if ( sFontVersion != null) {
                m_xmlWriter.WriteAttributeString("FontVersion", 
                                                 sFontVersion);
            } else {
                m_xmlWriter.WriteAttributeString("FontVersion", 
                                                 "BAD FONT VERSION");
            }
            
            string date = f.GetFontModifiedDate().ToShortDateString();
            m_xmlWriter.WriteAttributeString("FontDate", date );
            m_xmlWriter.WriteEndElement();
            m_xmlWriter.WriteWhitespace("\r\n");
        }

        private void OnFontBegin( uint fontIndex )
        {
            m_xmlWriter.WriteStartElement("Font");
            m_xmlWriter.WriteAttributeString("FontIndex", fontIndex.ToString());
            m_xmlWriter.WriteWhitespace("\r\n");
        }

        private void OnEndElement()
        {
            m_xmlWriter.WriteEndElement();
            m_xmlWriter.WriteWhitespace("\r\n");
        }

        private void OnRasterTestBegin( string element, 
                                        string label )
        {
            m_xmlWriter.WriteStartElement( element );
            m_xmlWriter.WriteWhitespace("\r\n");
            m_callbacks.OnBeginRasterTest( label );
        }

        public void OnValidateEvent(Validator.EventTypes e, object oParam)
        {
            switch ( e ) {

            case Validator.EventTypes.FileBegin:
            case Validator.EventTypes.FileEnd:
                break;

            case Validator.EventTypes.FontBegin:
                OnFontBegin( (uint)oParam );
                break;

            case Validator.EventTypes.FontParsed:
                OnFontParsed( (OTFont)oParam );
                break;

            case Validator.EventTypes.TableBegin:
                OnBeginTable( ( DirectoryEntry )oParam );
                break;

            case Validator.EventTypes.RastTestBegin_BW:
                OnRasterTestBegin( "RasterizationTest_BW", "Black and White" );

                break;
            case Validator.EventTypes.RastTestBegin_Grayscale:
                OnRasterTestBegin( "RasterizationTest_Grayscale", "Grayscale" );
                break;
            case Validator.EventTypes.RastTestBegin_ClearType:
                OnRasterTestBegin( "RasterizationTest_ClearType", "ClearType" );
                break;

            case Validator.EventTypes.TableProgress:
            case Validator.EventTypes.RastTestProgress_BW:
            case Validator.EventTypes.RastTestProgress_Grayscale:
            case Validator.EventTypes.RastTestProgress_ClearType:
                m_callbacks.OnTestProgress( oParam );
                break;

            case Validator.EventTypes.RastTestEnd_ClearType:
            case Validator.EventTypes.RastTestEnd_Grayscale:
            case Validator.EventTypes.RastTestEnd_BW:
            case Validator.EventTypes.TableEnd:
            case Validator.EventTypes.FontEnd:
                OnEndElement();
                break;

            default:
                Debug.Assert(false);
                break;
            }
        }

        private string ValInfoTypeToString( ValidationInfo.ValInfoType vt )
        {
            switch( vt ) {
            case ValidationInfo.ValInfoType.AppError: return "A";
            case ValidationInfo.ValInfoType.Error:    return "E";
            case ValidationInfo.ValInfoType.Warning:  return "W";
            case ValidationInfo.ValInfoType.Pass:     return "P";
            case ValidationInfo.ValInfoType.Info:     return "I";
            default: return "?";
            }
        }

        public void ValidatorCallback(ValInfoBasic viBasic)
        {
            ValidationInfo vi=new ValidationInfo(viBasic);

            string sCode    = (vi.GetErrorID()!=null)? vi.GetErrorID() : "?????";
            string sType    = ValInfoTypeToString( vi.GetValInfoType() );
            string sTable   = (vi.GetOTTag()!=null)? "'"+vi.GetOTTag()+"'" : "";
            string sMsg     = vi.GetString();
            string sDetails = vi.GetDetails();

            m_xmlWriter.WriteStartElement("Report");
            // I do not know why this try-catch is here.
            try {
                m_xmlWriter.WriteAttributeString("ErrorType", sType);
                m_xmlWriter.WriteAttributeString("ErrorCode", sCode);
                m_xmlWriter.WriteAttributeString("Message", sMsg);
                if (sDetails != null) {
                    m_xmlWriter.WriteAttributeString("Details", sDetails);
                }
                if (viBasic.TestName != null) {
                    m_xmlWriter.WriteAttributeString("TestName", 
                                                     viBasic.TestName);
                }
            }
            catch(Exception) {
                Debug.Assert(false);
            }
            m_xmlWriter.WriteEndElement();
            m_xmlWriter.WriteWhitespace("\r\n");
        }


        public void OpenXmlReportFile(string sReportFile, string sFontFile)
        {

            Stream stream;

            try
            {
                stream = new FileStream(sReportFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                Console.WriteLine("Could not open the report file. Not outputting report");
                stream = new MemoryStream(); //Just dumping into memory, later this is thrown away
            }

            m_xmlWriter = new XmlTextWriter(stream, 
                                                System.Text.Encoding.UTF8);
            m_xmlWriter.WriteStartDocument();
            m_xmlWriter.WriteWhitespace("\r\n");
            string type = "type=\"text/xsl\" href=\"fval.xsl\"";
            m_xmlWriter.WriteProcessingInstruction("xml-stylesheet", type );
            m_xmlWriter.WriteWhitespace("\r\n");
            m_xmlWriter.WriteStartElement("FontValidatorReport");
            string dateAndTime = System.DateTime.Now.ToString("f", null);
            m_xmlWriter.WriteAttributeString("RunDateTime", dateAndTime );
            m_xmlWriter.WriteAttributeString("MachineName", 
                                             SystemInformation.ComputerName);
            m_xmlWriter.WriteWhitespace("\r\n");

            FileInfo fi = new FileInfo(sFontFile);

            m_xmlWriter.WriteStartElement("FontFile");
            m_xmlWriter.WriteAttributeString("FileName", fi.Name);
            m_xmlWriter.WriteAttributeString("FileNameAndPath", sFontFile);
            m_xmlWriter.WriteWhitespace("\r\n");
        }

        public void CloseXmlReportFile()
        {
            m_xmlWriter.WriteEndElement(); // end FontFile
            m_xmlWriter.WriteWhitespace("\r\n");
            m_xmlWriter.WriteEndElement(); // end FontValidatorReport
            m_xmlWriter.WriteWhitespace("\r\n");
            m_xmlWriter.WriteEndDocument();
            m_xmlWriter.Close();
            m_xmlWriter = null;
        }

        //
        // The point of heavy use of callbacks is that we want to
        // do everything here so that the report comes out the same,
        // regardless of the program running the validator.
        // 
        // All questions of file names, temporary file cleanup, 
        // and decisions about what to show when should be handled
        // by the calling code.
        // 
        private int ValidateFont(Validator v, string fpath, SafeFileHandle hFile,int i,int n)
        {

            string sReportFile = "";
            OTFileVal fontFile;
            int ret = 0;

            // create a FontFile Object
            fontFile = new OTFileVal(v);
            m_callbacks.OnOTFileValChange(fontFile);

            // open the report file
            sReportFile = m_callbacks.GetReportFileName(fpath);
            OpenXmlReportFile(sReportFile, fpath);
            m_callbacks.OnOpenReportFile(sReportFile, fpath);

            // open the font file and validate it
            bool isvalid = (hFile == null) ? fontFile.open(fpath) : fontFile.open(hFile);

            //if the font file is invalid, we just do not go on with validation and output the results
            //this apparently fixed a lot of bugs (1231, 1429, 934, 1335)
            //Check if the sample font files are really invalid
            if (isvalid)
            {
                m_callbacks.OnBeginFontTest(fpath, i, n);
                if (!fontFile.Validate())
                {
                    ret = 1;
                }
                fontFile.close();
            }
            else
            {
                ret = 1;
            }

            fontFile = null;
            m_callbacks.OnOTFileValChange(fontFile);
            CloseXmlReportFile();
            m_callbacks.OnCloseReportFile(sReportFile);

            return ret;
        }

        //Validates a list of font files...meant to be used by the Font Validator UI and the command line
        public int RunValidation( Validator v, string [] fontList )
        {
            int i;
            int ret = 0;
            // setup notification for validation events
            v.SetOnValidateEvent(new
                                 Validator.OnValidateEvent(OnValidateEvent));
            // enable us to receive validation info messages
            DIAction vid = new DIAction(ValidatorCallback);
            v.SetValInfoDelegate(vid);

            try
            {
                for (i = 0; i < fontList.Length; i++)
                {
                    // check to see if the user canceled validation
                    if (v.CancelFlag)
                    {
                        m_callbacks.OnCancel();
                        return ret;
                    }

                    ret |= ValidateFont(v, fontList[i], null, i, fontList.Length);
                }
                m_callbacks.OnReportsReady();
            }
            catch (Exception e)
            {
                m_callbacks.OnException(e);
                return 1;
            }

            return ret;

        }

        //This version of RunValidation is to be used when the file is already opened by other process and you have the handle
        //i.e. , this is used when the validation is called from the ValInterface.dll
        //The file name is needed just for the path of the report file
        public int RunValidation(Validator v, string fpath,SafeFileHandle hFile)
        {
            int ret = 0;
            v.SetOnValidateEvent(new
                    Validator.OnValidateEvent(OnValidateEvent));
            // enable us to receive validation info messages
            DIAction vid = new DIAction(ValidatorCallback);
            v.SetValInfoDelegate(vid);

            try
            {
                // check to see if the user canceled validation
                if (v.CancelFlag)
                {
                    m_callbacks.OnCancel();
                    return ret;
                }

                ret |= ValidateFont(v, fpath, hFile, 0, 1);
                m_callbacks.OnReportsReady();
            }
            catch (Exception e)
            {
                m_callbacks.OnException(e);
                return 1;
            }

            return ret;

        }

    }

}