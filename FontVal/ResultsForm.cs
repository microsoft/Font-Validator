using System;
using System.IO;
using System.Windows.Forms;
// mshtml and AxSHDocVw.AxWebBrowser are very windows-specific,
// yet windows is the default platform; so we assume
// we have mshtml and AxSHDocVw.AxWebBrowser unless we know
// they are missing for specific non-windows compilers.
#if !__MonoCS__
using mshtml;
#endif

using OTFontFile;
using NS_ValCommon;

namespace FontVal
{
    /// <summary>
    /// Summary description for ResultsForm.
    /// </summary>
    public class ResultsForm : System.Windows.Forms.Form
    {
#if !__MonoCS__
        private AxSHDocVw.AxWebBrowser axWebBrowser1;
#endif
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public ResultsForm()
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

            if (m_bDeleteOnClose)
            {
                File.Delete(m_sFilename);
            }
        }

        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ResultsForm));
#if !__MonoCS__
			this.axWebBrowser1 = new AxSHDocVw.AxWebBrowser();
			((System.ComponentModel.ISupportInitialize)(this.axWebBrowser1)).BeginInit();
			this.SuspendLayout();
			// 
			// axWebBrowser1
			// 
			this.axWebBrowser1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.axWebBrowser1.Enabled = true;
			this.axWebBrowser1.Location = new System.Drawing.Point(8, 8);
			this.axWebBrowser1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWebBrowser1.OcxState")));
			this.axWebBrowser1.Size = new System.Drawing.Size(352, 272);
			this.axWebBrowser1.TabIndex = 1;
			// 
			// ResultsForm
			// 
			if ( Type.GetType("Mono.Runtime") == null ) this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(368, 286);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.axWebBrowser1});
			this.Name = "ResultsForm";
			this.Text = "ResultsForm";
			this.Load += new System.EventHandler(this.ResultsForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.axWebBrowser1)).EndInit();
			this.ResumeLayout(false);
#endif

		}
		#endregion

        public void ShowFile(string sFilename, string sCaption, bool bDeleteOnClose)
        {
#if !__MonoCS__
            object nullObj = null;
            axWebBrowser1.Navigate(sFilename, ref nullObj, ref nullObj, ref nullObj, ref nullObj);
            m_sFilename = sFilename;
            m_bDeleteOnClose = bDeleteOnClose;
#else
            //Do not set DeleteOnClose if it does not work
            MessageBox.Show(this, "XML viewing not Implemented on non-windows. Please open \""
                            + sFilename + "\" manually.");
#endif
            if (sCaption != null)
            {
                Text = sCaption;
            }
        }

        public void PrintFile()
        {
#if !__MonoCS__
            IHTMLDocument2 doc = (IHTMLDocument2)axWebBrowser1.Document;
            doc.execCommand("PRINT", true, null);
#endif
        }

        public void CopyToClipboard()
        {
#if !__MonoCS__
            IHTMLDocument2 doc = (IHTMLDocument2)axWebBrowser1.Document;
            doc.execCommand("Copy", false, null);
#endif
        }

        public void SelectAll()
        {
#if !__MonoCS__
            IHTMLDocument2 doc = (IHTMLDocument2)axWebBrowser1.Document;
            doc.execCommand("SelectAll", false, null);
#endif
        }

        public void FindText(String sText)
        {
#if !__MonoCS__
            IHTMLDocument2 doc = (IHTMLDocument2)axWebBrowser1.Document;
            IHTMLBodyElement bod = (IHTMLBodyElement)doc.body;
            IHTMLTxtRange txt = bod.createTextRange();
            txt.findText(sText, 0x7fffffff, 0);
#endif
        }

        public void SaveReportAs(string sFilename)
        {
            try
            {
                File.Copy(m_sFilename, sFilename, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(this, e.ToString());
            }
        }

        public string GetFilename()
        {
            return m_sFilename;
        }

        string m_sFilename;
        bool m_bDeleteOnClose;

        private void ResultsForm_Load(object sender, System.EventArgs e)
        {
            // hack the size of the webbrowser control if it is bigger than the form
            // which happens courtesy of the current .Net framework when running the program 
            // on a display that is greater than 96 dpi
            
#if !__MonoCS__
            if (axWebBrowser1.Right > Width || axWebBrowser1.Bottom > Height)
            {
                this.axWebBrowser1.Anchor = System.Windows.Forms.AnchorStyles.Top| System.Windows.Forms.AnchorStyles.Left;
                axWebBrowser1.Height = ClientRectangle.Height-axWebBrowser1.Top*2;
                axWebBrowser1.Width  = ClientRectangle.Width-axWebBrowser1.Left*2;
                this.axWebBrowser1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                    | System.Windows.Forms.AnchorStyles.Left) 
                    | System.Windows.Forms.AnchorStyles.Right);
            }
#endif
        }
    }
}
