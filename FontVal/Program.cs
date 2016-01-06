using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using OTFontFileVal;
using OTFontFile;
using System.IO;

namespace FontVal
{

    public class Program : Driver.DriverCallbacks
    {
        ReportFileDestination m_ReportFileDestination;
        string m_sReportFixedDir;
        List<string> m_reportFiles = new List<string>();
        List<string> m_captions = new List<string>();

        static void ErrOut(string s)
        {
            Console.WriteLine(s);
        }

        static void StdOut(string s)
        {
            Console.WriteLine(s);
        }

        // ================================================================
        // Callbacks for Driver.DriverCallbacks interface
        // ================================================================
        public void OnException(Exception e)
        {
            ErrOut("Error: " + e.Message);
            DeleteTempFiles();
        }
        public void OnReportsReady()
        {
            StdOut("Reports are ready!");
        }
        public void OnBeginRasterTest(string label)
        {
            StdOut("Begin Raster Test: " + label);
        }

        public void OnBeginTableTest(DirectoryEntry de)
        {
            StdOut("Table Test: " + (string)de.tag);
        }
        public void OnTestProgress(object oParam)
        {
            string s = (string)oParam;
            if (s == null)
            {
                s = "";
            }
            StdOut("Progress: " + s);
        }

        public void OnCloseReportFile(string sReportFile)
        {
            StdOut("Complete: " + sReportFile);
            // copy the xsl file to the same directory as the report
            // 
            // This has to be done for each file because the if we are
            // putting the report on the font's directory, there may
            // be a different directory for each font.
            Driver.CopyXslFile(sReportFile);
        }

        public void OnOpenReportFile(string sReportFile, string fpath)
        {
            m_captions.Add(fpath);
            m_reportFiles.Add(sReportFile);
        }

        public void OnCancel()
        {
            DeleteTempFiles();
        }

        public void OnOTFileValChange(OTFileVal fontFile)
        {
        }

        public string GetReportFileName(string sFontFile)
        {
            string sReportFile = null;
            switch (m_ReportFileDestination)
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

        public void OnBeginFontTest(string fname, int nth, int nFonts)
        {
            string label = fname + " (file " + (nth + 1) + " of " + nFonts + ")";
            StdOut(label);
        }

        public void DeleteTempFiles()
        {
            if (m_ReportFileDestination == ReportFileDestination.TempFiles)
            {
                for (int i = 0; i < m_reportFiles.Count; i++)
                {
                    File.Delete(m_reportFiles[i]);
                }
            }
        }

        static void Usage()
        {
            Console.WriteLine("Usage: FontValidator [options]");
            Console.WriteLine("Options");
            Console.WriteLine("<to be written>");
            Console.WriteLine("");
        }

        [DllImport("Kernel32.dll")]
        private static extern Boolean FreeConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                try {
                FreeConsole();
                } catch (Exception e) when ( e is EntryPointNotFoundException || e is DllNotFoundException ) {
                    // FreeConsole() is neither available nor relevant
                    // on non-windows.
                }
                Application.Run(new Form1());
                return ;
            }

            bool err = false;
            string reportDir = null;
            ReportFileDestination rfd = ReportFileDestination.TempFiles;
            List<string> sFileList = new List<string>();
            ValidatorParameters vp = new ValidatorParameters();

            int i,j;

            for (i = 0; i < args.Length; i++)
            {
                if ("-file" == args[i])
                {
                    j = i + 1;
                    if (j == args.Length)
                    {
                        ErrOut("Argument required for \"" + args[j-1] + "\"");
                        err = true;
                        break;
                    }

                    for (;j < args.Length; j++)
                    {
                        if (args[j][0] == '-' || args[j][0] == '+')
                        {
                            j--;
                            break;
                        }
                        sFileList.Add(args[j]);
                    }

                    if (j == i)
                    {
                        ErrOut("Argument required for \"" + args[i] + "\"");
                        err = true;
                        break;
                    }
                    i = j;

                }
                else if ("+table" == args[i])
                {

                    j = i + 1;
                    if (j == args.Length)
                    {
                        ErrOut("Argument required for \"" + args[j-1] + "\"");
                        err = true;
                        break;
                    }

                    for (; j < args.Length; j++)
                    {
                        if (args[j][0] == '-' || args[j][0] == '+')
                        {
                            j--;
                            break;
                        }
                        vp.AddTable(args[j]);
                    }

                    if (j == i)
                    {
                        ErrOut("Argument required for \"" + args[i] + "\"");
                        err = true;
                        break;
                    }

                    i = j;

                }
                else if ("-table" == args[i])
                {
                    j = i + 1;
                    if (j == args.Length)
                    {
                        ErrOut("Argument required for \"" + args[j - 1] + "\"");
                        err = true;
                        break;
                    }

                    for (; j < args.Length; j++)
                    {
                        if (args[j][0] == '-' || args[j][0] == '+')
                        {
                            j--;
                            break;
                        }
                        vp.RemoveTableFromList(args[j]);
                    }

                    if (j == i)
                    {
                        ErrOut("Argument required for \"" + args[i] + "\"");
                        err = true;
                        break;
                    }

                    i = j;

                }
                else if ("-all-tables" == args[i])
                {
                    vp.SetAllTables();
                }
                else if ("-only-tables" == args[i])
                {
                    vp.ClearTables();
                }
                else if ("-report-dir" == args[i])
                {
                    i++;
                    if (i < args.Length)
                    {
                        reportDir = args[i];
                        rfd = ReportFileDestination.FixedDir;
                    }
                    else
                    {
                        ErrOut("Argument required for \"" + args[i - 1] + "\"");
                        err = true;
                    }

                }
                else if ("-report-in-font-dir" == args[i])
                {
                    rfd = ReportFileDestination.SameDirAsFont;
                }
                else
                {
                    ErrOut("Unknown argument: \"" + args[i] + "\"");
                    err = true;
                }
            }
            if (err)
            {
                Usage();
                return ;
            }

            //Ready to run
            Validator v = new Validator();
            vp.SetupValidator(v);

            Program p = new Program();
            p.m_ReportFileDestination = rfd;
            p.m_sReportFixedDir = reportDir;

            Driver drv = new Driver(p);
            drv.RunValidation(v, sFileList.ToArray());
        }
    }
}
