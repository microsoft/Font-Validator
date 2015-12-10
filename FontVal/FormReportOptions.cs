using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Win32APIs;
using OTFontFileVal;

namespace FontVal
{
    /// <summary>
    /// Summary description for FormReportOptions.
    /// </summary>
    public class FormReportOptions : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textFixedDir;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox checkBoxOpenReport;
        private System.Windows.Forms.RadioButton radioButtonTempFiles;
        private System.Windows.Forms.RadioButton radioButtonFixedDir;
        private System.Windows.Forms.RadioButton radioButtonSameDir;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private ReportFileDestination m_ReportFileDestination;
        private bool m_bOpenReportFile;
        private string m_sFixedDir;

        public FormReportOptions()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormReportOptions));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.textFixedDir = new System.Windows.Forms.TextBox();
			this.radioButtonSameDir = new System.Windows.Forms.RadioButton();
			this.radioButtonFixedDir = new System.Windows.Forms.RadioButton();
			this.radioButtonTempFiles = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.checkBoxOpenReport = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnBrowse,
																					this.textFixedDir,
																					this.radioButtonSameDir,
																					this.radioButtonFixedDir,
																					this.radioButtonTempFiles});
			this.groupBox1.Location = new System.Drawing.Point(24, 24);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(392, 192);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Default report file location";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(304, 104);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.TabIndex = 4;
			this.btnBrowse.Text = "Browse...";
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// textFixedDir
			// 
			this.textFixedDir.Location = new System.Drawing.Point(40, 104);
			this.textFixedDir.Name = "textFixedDir";
			this.textFixedDir.Size = new System.Drawing.Size(248, 20);
			this.textFixedDir.TabIndex = 3;
			this.textFixedDir.Text = "";
			this.textFixedDir.TextChanged += new System.EventHandler(this.textFixedDir_TextChanged);
			// 
			// radioButtonSameDir
			// 
			this.radioButtonSameDir.Location = new System.Drawing.Point(24, 144);
			this.radioButtonSameDir.Name = "radioButtonSameDir";
			this.radioButtonSameDir.Size = new System.Drawing.Size(200, 32);
			this.radioButtonSameDir.TabIndex = 2;
			this.radioButtonSameDir.Text = "in the same directory as the font file";
			this.radioButtonSameDir.CheckedChanged += new System.EventHandler(this.radioButtonSameDir_CheckedChanged);
			// 
			// radioButtonFixedDir
			// 
			this.radioButtonFixedDir.Location = new System.Drawing.Point(24, 72);
			this.radioButtonFixedDir.Name = "radioButtonFixedDir";
			this.radioButtonFixedDir.Size = new System.Drawing.Size(240, 24);
			this.radioButtonFixedDir.TabIndex = 1;
			this.radioButtonFixedDir.Text = "in the specified directory";
			this.radioButtonFixedDir.CheckedChanged += new System.EventHandler(this.radioButtonFixedDir_CheckedChanged);
			// 
			// radioButtonTempFiles
			// 
			this.radioButtonTempFiles.Location = new System.Drawing.Point(24, 32);
			this.radioButtonTempFiles.Name = "radioButtonTempFiles";
			this.radioButtonTempFiles.Size = new System.Drawing.Size(352, 16);
			this.radioButtonTempFiles.TabIndex = 0;
			this.radioButtonTempFiles.Text = "Temporary files in Windows Temp directory";
			this.radioButtonTempFiles.CheckedChanged += new System.EventHandler(this.radioButtonTempFiles_CheckedChanged);
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(112, 280);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(256, 280);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			// 
			// checkBoxOpenReport
			// 
			this.checkBoxOpenReport.Location = new System.Drawing.Point(40, 240);
			this.checkBoxOpenReport.Name = "checkBoxOpenReport";
			this.checkBoxOpenReport.Size = new System.Drawing.Size(352, 24);
			this.checkBoxOpenReport.TabIndex = 3;
			this.checkBoxOpenReport.Text = "Open report file after completion of validation";
			this.checkBoxOpenReport.CheckedChanged += new System.EventHandler(this.checkBoxOpenReport_CheckedChanged);
			// 
			// FormReportOptions
			// 
			this.AcceptButton = this.btnOK;
			if ( Type.GetType("Mono.Runtime") == null ) this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(440, 318);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.checkBoxOpenReport,
																		  this.btnCancel,
																		  this.btnOK,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReportOptions";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Report Options";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

        private void radioButtonTempFiles_CheckedChanged(object sender, System.EventArgs e)
        {
            if (radioButtonTempFiles.Checked)
            {
                m_ReportFileDestination = ReportFileDestination.TempFiles;
                checkBoxOpenReport.CheckState = CheckState.Checked;
                m_bOpenReportFile = checkBoxOpenReport.Checked;
                checkBoxOpenReport.Enabled = false;
            }
            else
            {
                checkBoxOpenReport.Enabled = true;
            }
        }

        private void radioButtonFixedDir_CheckedChanged(object sender, System.EventArgs e)
        {
            if (radioButtonFixedDir.Checked)
            {
                m_ReportFileDestination = ReportFileDestination.FixedDir;
            }
        }

        private void radioButtonSameDir_CheckedChanged(object sender, System.EventArgs e)
        {
            if (radioButtonSameDir.Checked)
            {
                m_ReportFileDestination = ReportFileDestination.SameDirAsFont;
            }
        }

        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            try
            {
                String sDir = SH.BrowseForFolder();
                if (sDir != null)
                {
                    m_sFixedDir = sDir;
                    textFixedDir.Text = sDir;
                }
            } catch (Exception what) {
                // Does not happen if SH.BrowseForFolder works.
                // "/tmp" is friendlier than do nothing.
                m_sFixedDir = "/tmp";
                textFixedDir.Text = "/tmp";
            }
        }

        private void checkBoxOpenReport_CheckedChanged(object sender, System.EventArgs e)
        {
            m_bOpenReportFile = checkBoxOpenReport.Checked;
        }

        private void textFixedDir_TextChanged(object sender, System.EventArgs e)
        {
            m_sFixedDir = textFixedDir.Text;
        }

        public ReportFileDestination ReportFileDestination
        {
            get {return m_ReportFileDestination;}
            set
            {
                m_ReportFileDestination = value;
                switch(m_ReportFileDestination)
                {
                    case ReportFileDestination.TempFiles:
                        radioButtonTempFiles.Checked = true;
                        break;
                    case ReportFileDestination.FixedDir:
                        radioButtonFixedDir.Checked = true;
                        break;
                    case ReportFileDestination.SameDirAsFont:
                        radioButtonSameDir.Checked = true;
                        break;
                }
            }
        }

        public bool OpenReportFile
        {
            get {return m_bOpenReportFile;}
            set
            {
                m_bOpenReportFile = value;
                checkBoxOpenReport.Checked = m_bOpenReportFile;
            }
        }

        public string FixedDir
        {
            get {return m_sFixedDir;}
            set
            {
                m_sFixedDir = value;
                textFixedDir.Text = m_sFixedDir;
            }
        }
    }
}
