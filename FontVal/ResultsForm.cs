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
using System.IO;
using System.Windows.Forms;
using System.Xml.Xsl;

using OTFontFile;
using NS_ValCommon;

namespace FontVal
{
    /// <summary>
    /// Summary description for ResultsForm.
    /// </summary>
    public class ResultsForm : Form
    {
        private WebBrowser axWebBrowser1;
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
            if ( Type.GetType("Mono.Runtime") != null )
                Environment.SetEnvironmentVariable("MONO_BROWSER_ENGINE", "webkit", EnvironmentVariableTarget.Process);
			this.axWebBrowser1 = new WebBrowser();
            if ( this.axWebBrowser1 != null )
            {
                this.axWebBrowser1.AllowWebBrowserDrop = false;
                this.SuspendLayout();
			// 
			// axWebBrowser1
			// 
                this.axWebBrowser1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                              | System.Windows.Forms.AnchorStyles.Left)
                                             | System.Windows.Forms.AnchorStyles.Right);
                this.axWebBrowser1.Location = new System.Drawing.Point(8, 8);
                this.axWebBrowser1.Size = new System.Drawing.Size(352, 272);
                this.axWebBrowser1.TabIndex = 1;
            }
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
			this.ResumeLayout(false);

		}
		#endregion

        public void ShowFile(string sFilename, string sCaption, bool bDeleteOnClose)
        {
            try
            {
                string sFileToShow = sFilename ;
                if (( Type.GetType("Mono.Runtime") != null )
                    || ( Environment.GetEnvironmentVariable("WINEPREFIX") != null ))
                {
                    string sHTMLFile = ( sFilename.EndsWith(".report.xml") ?
                                         sFilename.Replace(".report.xml", ".report.html") :
                                         sFilename );
                    if ( File.Exists( sHTMLFile ) )
                        sFileToShow = sHTMLFile;
                    else
                    {
                        sFileToShow = Path.GetTempFileName() + "." + Path.GetFileName(sHTMLFile);
                        string sXSL = Path.GetTempFileName() + ".fval.xsl";
                        File.WriteAllBytes(sXSL, Compat.Xsl.fval);
                        var xslTrans = new XslCompiledTransform();
                        xslTrans.Load(sXSL);
                        xslTrans.Transform(sFilename, sFileToShow);
                    }
                }
                axWebBrowser1.Navigate(sFileToShow, false); // false=No new window
                axWebBrowser1.AllowNavigation = false;
                m_sFilename = sFilename;
                m_bDeleteOnClose = bDeleteOnClose;
            }
            catch ( NullReferenceException )
            {
                this.Controls.Clear();
                MessageBox.Show(this, "XML viewing not fully implemented on non-windows. Please open \""
                                + sFilename + "\" manually.");
            }
            if (sCaption != null)
            {
                Text = sCaption;
            }
        }

        public void PrintFile()
        {
            if ( Type.GetType("Mono.Runtime") == null )
            {
                HtmlDocument doc = axWebBrowser1.Document;
                doc.ExecCommand("PRINT", true, null);
            }
        }

        public void CopyToClipboard()
        {
            if ( Type.GetType("Mono.Runtime") == null )
            {
                HtmlDocument doc = axWebBrowser1.Document;
                doc.ExecCommand("Copy", false, null);
            }
        }

        public void SelectAll()
        {
            if ( Type.GetType("Mono.Runtime") == null )
            {
                HtmlDocument doc = axWebBrowser1.Document;
                doc.ExecCommand("SelectAll", false, null);
            }
        }

        public void FindText(String sText)
        {
            // Not used.
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
            if ( this.axWebBrowser1 == null )
                return;

            // hack the size of the webbrowser control if it is bigger than the form
            // which happens courtesy of the current .Net framework when running the program 
            // on a display that is greater than 96 dpi
            if (axWebBrowser1.Right > Width || axWebBrowser1.Bottom > Height)
            {
                this.axWebBrowser1.Anchor = System.Windows.Forms.AnchorStyles.Top| System.Windows.Forms.AnchorStyles.Left;
                axWebBrowser1.Height = ClientRectangle.Height-axWebBrowser1.Top*2;
                axWebBrowser1.Width  = ClientRectangle.Width-axWebBrowser1.Left*2;
                this.axWebBrowser1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                    | System.Windows.Forms.AnchorStyles.Left) 
                    | System.Windows.Forms.AnchorStyles.Right);
            }
        }
    }
}
