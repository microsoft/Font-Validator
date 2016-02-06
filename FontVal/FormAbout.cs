using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace FontVal
{
    /// <summary>
    /// Summary description for FormAbout.
    /// </summary>
    public class FormAbout : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel2;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public FormAbout()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            // Display the assembly information in a Label.
            //Label label1 = new Label();
            AssemblyName assemName = Assembly.GetExecutingAssembly().GetName();
            label1.Text += " " + assemName.Version.Major + "." + assemName.Version.Minor
                + "." + assemName.Version.Build + "." + assemName.Version.Revision;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormAbout));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.linkLabel2 = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(272, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "Release";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(272, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Microsoft\u00AE Font Validator";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 136);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(272, 23);
			this.label4.TabIndex = 4;
			this.label4.Text = "Provided by Microsoft\'s typography group";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// linkLabel2
			// 
			this.linkLabel2.Location = new System.Drawing.Point(24, 160);
			this.linkLabel2.Name = "linkLabel2";
			this.linkLabel2.Size = new System.Drawing.Size(272, 23);
			this.linkLabel2.TabIndex = 5;
			this.linkLabel2.TabStop = true;
			this.linkLabel2.Text = "http://www.microsoft.com/typography/";
			this.linkLabel2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
			// 
			// FormAbout
			// 
			if ( Type.GetType("Mono.Runtime") == null ) this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(320, 222);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.linkLabel2,
																		  this.label4,
																		  this.label2,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAbout";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About";
			this.ResumeLayout(false);

		}
		#endregion

        private void linkLabel2_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel2.Text);
        }

    }
}
