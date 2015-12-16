using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using System.IO;

using OTFontFile;
using OTFontFileVal;
using NS_ValCommon;

namespace FontVal
{

    /// <summary>
    /// Summary description for Progress.
    /// </summary>
    public class Progress : System.Windows.Forms.Form, Driver.DriverCallbacks
    {
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelFontname;
        private System.Windows.Forms.Label labelCancelling;
        private System.Windows.Forms.Label labelTestProgress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelTestname;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public Progress( Form1 formParent, 
                         Validator v, 
                         string [] sFilenames, 
                         ReportFileDestination rfd, 
                         bool bOpenReportFiles, 
                         string sReportFixedDir )
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            m_Validator = v;
            m_sFiles = sFilenames;
            m_formParent = formParent;
            m_bValidationInProgress = false;
            m_ReportFileDestination = rfd;
            m_bOpenReportFiles = bOpenReportFiles;
            m_sReportFixedDir = sReportFixedDir;
        }


        //
        // Update label text in same thread in which widget was created.
        //
        private delegate void 
            UpdateLabelTextDelegate( System.Windows.Forms.Label label,
                                     string text );

        private void UpdateLabelTextWorker( System.Windows.Forms.Label label,
                                            string text )
        {
            label.Text = text;
        }
        private void UpdateLabelText( System.Windows.Forms.Label label,
                                      string text )
        {
            label.Invoke( new UpdateLabelTextDelegate(UpdateLabelTextWorker), 
                          new object[]{label,text} );
        }

        //
        // Update label visibility in same thread in which widget was created.
        //

        private delegate void 
            UpdateLabelBoolDelegate( System.Windows.Forms.Label label, 
                                     bool val );

        private void UpdateLabelVisibleWorker( System.Windows.Forms.Label label,
                                               bool bValue )
        {
            label.Visible = bValue;
        }
        private void UpdateLabelVisible( System.Windows.Forms.Label label,
                                         bool bValue )
        {
            label.Invoke( new UpdateLabelBoolDelegate(UpdateLabelVisibleWorker), 
                          new object[]{label,bValue} );
        }

        //
        // Close main form in same thread in which widget was created.
        //
        private delegate void UpdateVoidDelegate();

        private void CloseOurFormWorker() {
            Close();
        }
        private void CloseOurForm()
        {
            m_formParent.Invoke( new UpdateVoidDelegate( CloseOurFormWorker ), 
                                 new object[]{} );
        }


        protected override void OnLoad(EventArgs e)
        {
            // call the base class
            base.OnLoad(e);

            // clear the cancel flag, then start validating in the worker thread
            m_Validator.CancelFlag = false;
            StartWorkerThread();
        }

        public void StartWorkerThread()
        {
            m_threadWorker = new Thread(new ThreadStart(Worker));
            m_threadWorker.Start();
        }

        private void OpenReportFiles( List<string> arrCaptions,
                                      List<string> arrReportFiles )
        {
            if ( m_bOpenReportFiles ) {
                for ( int i = 0; i < arrReportFiles.Count; i++ ) {
                    string sFile = arrReportFiles[i];
                    string sCaption = arrCaptions[i];
                    bool bDeleteOnClose = 
                        (m_ReportFileDestination == 
                         ReportFileDestination.TempFiles);
                    Form1.OpenReportDelegate ord = 
                        new Form1.OpenReportDelegate(m_formParent.OpenReport);
                    m_formParent.Invoke( ord,
                                         new Object[] {sFile, 
                                                       sCaption, 
                                                       bDeleteOnClose} );
                }
            }
        }

        // method that will be called when the worker thread is started
        public void Worker()
        {
            m_bValidationInProgress = true;

            OTFontFileVal.Driver driver = new OTFontFileVal.Driver( this );
            driver.RunValidation( m_Validator, m_sFiles );
            m_bValidationInProgress = false;

            // after all work is done, enable the UI and close this dialog
            m_formParent.EnableUI(true);
            //JJF Close();
            CloseOurForm();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                ValidationDieDieDie();

                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelFontname = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelTestname = new System.Windows.Forms.Label();
            this.labelCancelling = new System.Windows.Forms.Label();
            this.labelTestProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(155, 168);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "current font:";
            // 
            // labelFontname
            // 
            this.labelFontname.Location = new System.Drawing.Point(104, 16);
            this.labelFontname.Name = "labelFontname";
            this.labelFontname.Size = new System.Drawing.Size(264, 48);
            this.labelFontname.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "current test:";
            // 
            // labelTestname
            // 
            this.labelTestname.Location = new System.Drawing.Point(104, 72);
            this.labelTestname.Name = "labelTestname";
            this.labelTestname.Size = new System.Drawing.Size(264, 24);
            this.labelTestname.TabIndex = 4;
            // 
            // labelCancelling
            // 
            this.labelCancelling.Location = new System.Drawing.Point(156, 144);
            this.labelCancelling.Name = "labelCancelling";
            this.labelCancelling.Size = new System.Drawing.Size(72, 16);
            this.labelCancelling.TabIndex = 5;
            this.labelCancelling.Text = "Cancelling!";
            this.labelCancelling.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelCancelling.Visible = false;
            // 
            // labelTestProgress
            // 
            this.labelTestProgress.Location = new System.Drawing.Point(104, 104);
            this.labelTestProgress.Name = "labelTestProgress";
            this.labelTestProgress.Size = new System.Drawing.Size(264, 40);
            this.labelTestProgress.TabIndex = 7;
            this.labelTestProgress.Visible = false;
            // 
            // Progress
            // 
            if ( Type.GetType("Mono.Runtime") == null ) this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(384, 214);
            this.ControlBox = false;
            this.Controls.Add(this.labelTestProgress);
            this.Controls.Add(this.labelCancelling);
            this.Controls.Add(this.labelTestname);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelFontname);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Progress";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Progress";
            this.TopMost = true;
            this.ResumeLayout(false);

        }
        #endregion

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            labelTestProgress.Visible = false;
            labelCancelling.Visible = true;
            btnCancel.Enabled = false;

            // set the validator's cancel flag
            m_Validator.CancelFlag = true;

            // tell the font file as well
            // FIX: Race Condition?
            if (m_curOTFileVal != null)
            {
                m_curOTFileVal.CancelValidation();
            }
        }

        protected void ValidationDieDieDie()
        {
            if (m_bValidationInProgress)
            {
                btnCancel_Click(null, null);
                Thread.Sleep(1000); 
                // wait for a second to see if the validator can cancel in that 
                // time

                // still running? Well, hey, if a second isn't enough time, 
                // then off with your head.
                if (m_threadWorker != null)
                {
                    if (m_threadWorker.IsAlive)
                    {
                        m_threadWorker.Abort();
                    }
                }
            }
        }

        public delegate void UpdateTestProgressDelegate(string s);

        protected void UpdateTestProgressWorker(string s)
        {
            labelTestProgress.Visible = true;
            if (s != null)
            {
                labelTestProgress.Text = s;
            }
            else
            {
                labelTestProgress.Text = "";
            }
        }

        public void DeleteTemporaryFiles( List<string> arrReportFiles )
        {
            if ( m_ReportFileDestination == ReportFileDestination.TempFiles )
            {
                for ( int i = 0; i < arrReportFiles.Count; i++ ) {
                    File.Delete( arrReportFiles[i] );
                }
            }
        }

        // ================================================================
        // Callbacks for Driver.DriverCallbacks interface
        // ================================================================
        

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

        public void OnTestProgress( object oParam )
        {
            UpdateTestProgressDelegate utpd = new 
                UpdateTestProgressDelegate(UpdateTestProgressWorker);
            object [] obj = new object[1];
            obj[0] = oParam;
            BeginInvoke(utpd, obj);
        }

        public void OnException( Exception e )
        {
            MessageBox.Show( this, e.Message, "Error", 
                             System.Windows.Forms.MessageBoxButtons.OK, 
                             System.Windows.Forms.MessageBoxIcon.Error);
            DeleteTemporaryFiles( m_reportFiles );
        }

        public void OnCloseReportFile( string sReportFile )
        {
            // copy the xsl file to the same directory as the report
            // 
            // This has to be done for each file because the if we are
            // putting the report on the font's directory, there may
            // be a different directory for each font.
            Driver.CopyXslFile( sReportFile );
        }


        public void OnOpenReportFile( string sReportFile, string fpath )
        {
            m_reportFiles.Add( sReportFile );
            FileInfo fi = new FileInfo(fpath);
            m_captions.Add( fi.Name );
         }

        public void OnReportsReady()
        {
            OpenReportFiles( m_captions, m_reportFiles );
        }

        public void OnBeginRasterTest( string label )
        {
            UpdateLabelText( labelTestname, "Rasterization: " + label );
            UpdateLabelText( labelTestProgress, "" );
            UpdateLabelVisible( labelTestProgress, false );
        }

        public void OnBeginTableTest( DirectoryEntry de )
        {
            UpdateLabelText( labelTestname, "" + de.tag );
            UpdateLabelText( labelTestProgress, "" );
            UpdateLabelVisible( labelTestProgress, false );
        }

        public void OnBeginFontTest( string fname, int nth, int nFonts )
        {
            string label = fname + " (file " + (nth+1) + " of " + nFonts + ")";
            UpdateLabelText( labelFontname, label );
        }

        public void OnCancel()
        {
            DeleteTemporaryFiles( m_reportFiles );
        }

        public void OnOTFileValChange( OTFileVal fontFile )
        {
            m_curOTFileVal = fontFile;
        }

        List<string>          m_captions = new List<string>();
        List<string>          m_reportFiles = new List<string>();
        Validator             m_Validator;
        string []             m_sFiles;
        OTFileVal             m_curOTFileVal;
        Form1                 m_formParent;
        bool                  m_bValidationInProgress;
        Thread                m_threadWorker;
        ReportFileDestination m_ReportFileDestination;
        bool                  m_bOpenReportFiles;
        string                m_sReportFixedDir;
    }
}
