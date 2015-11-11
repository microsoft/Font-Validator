using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using OTFontFileVal;

namespace FontVal
{
    /// <summary>
    /// Summary description for FormTransform.
    /// </summary>
    public class FormTransform : System.Windows.Forms.Form
    {
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxR1C1;
        private System.Windows.Forms.TextBox textBoxR1C2;
        private System.Windows.Forms.TextBox textBoxR1C3;
        private System.Windows.Forms.TextBox textBoxR2C1;
        private System.Windows.Forms.TextBox textBoxR2C2;
        private System.Windows.Forms.TextBox textBoxR2C3;
        private System.Windows.Forms.TextBox textBoxR3C1;
        private System.Windows.Forms.TextBox textBoxR3C2;
        private System.Windows.Forms.TextBox textBoxR3C3;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox textBoxStretchX;
        private System.Windows.Forms.TextBox textBoxStretchY;
        private System.Windows.Forms.TextBox textBoxRotation;
        private System.Windows.Forms.TextBox textBoxSkew;
        private System.Windows.Forms.ErrorProvider errorProvider1;

        private RastTestTransform m_RastTestTransform;

        public FormTransform(RastTestTransform rtt)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            m_RastTestTransform = rtt;

            textBoxStretchX.Text = rtt.stretchX.ToString("0.000");
            textBoxStretchY.Text = rtt.stretchY.ToString("0.000");
            textBoxRotation.Text = rtt.rotation.ToString("0.000");
            textBoxSkew.Text     = rtt.skew.ToString("0.000");

            textBoxR1C1.Text = rtt.matrix[0,0].ToString("0.000");
            textBoxR1C2.Text = rtt.matrix[0,1].ToString("0.000");
            textBoxR1C3.Text = rtt.matrix[0,2].ToString("0.000");

            textBoxR2C1.Text = rtt.matrix[1,0].ToString("0.000");
            textBoxR2C2.Text = rtt.matrix[1,1].ToString("0.000");
            textBoxR2C3.Text = rtt.matrix[1,2].ToString("0.000");

            textBoxR3C1.Text = rtt.matrix[2,0].ToString("0.000");
            textBoxR3C2.Text = rtt.matrix[2,1].ToString("0.000");
            textBoxR3C3.Text = rtt.matrix[2,2].ToString("0.000");
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormTransform));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxStretchX = new System.Windows.Forms.TextBox();
			this.textBoxStretchY = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxRotation = new System.Windows.Forms.TextBox();
			this.textBoxSkew = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textBoxR1C1 = new System.Windows.Forms.TextBox();
			this.textBoxR1C2 = new System.Windows.Forms.TextBox();
			this.textBoxR1C3 = new System.Windows.Forms.TextBox();
			this.textBoxR2C1 = new System.Windows.Forms.TextBox();
			this.textBoxR2C2 = new System.Windows.Forms.TextBox();
			this.textBoxR2C3 = new System.Windows.Forms.TextBox();
			this.textBoxR3C1 = new System.Windows.Forms.TextBox();
			this.textBoxR3C2 = new System.Windows.Forms.TextBox();
			this.textBoxR3C3 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.tabPage1,
																					  this.tabPage2});
			this.tabControl1.Location = new System.Drawing.Point(8, 8);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(328, 224);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				   this.textBoxSkew,
																				   this.textBoxRotation,
																				   this.label4,
																				   this.label3,
																				   this.groupBox1});
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(320, 198);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Settings";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				   this.groupBox2});
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(320, 198);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Matrix";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(64, 248);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(200, 248);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.textBoxStretchY,
																					this.textBoxStretchX,
																					this.label2,
																					this.label1});
			this.groupBox1.Location = new System.Drawing.Point(16, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(288, 72);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Stretch Factor";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(16, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "X:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(136, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(24, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Y:";
			// 
			// textBoxStretchX
			// 
			this.textBoxStretchX.Location = new System.Drawing.Point(48, 32);
			this.textBoxStretchX.Name = "textBoxStretchX";
			this.textBoxStretchX.Size = new System.Drawing.Size(64, 20);
			this.textBoxStretchX.TabIndex = 2;
			this.textBoxStretchX.Text = "";
			this.textBoxStretchX.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxStretchX_Validating);
			this.textBoxStretchX.Validated += new System.EventHandler(this.textBoxStretchX_Validated);
			// 
			// textBoxStretchY
			// 
			this.textBoxStretchY.Location = new System.Drawing.Point(168, 32);
			this.textBoxStretchY.Name = "textBoxStretchY";
			this.textBoxStretchY.Size = new System.Drawing.Size(64, 20);
			this.textBoxStretchY.TabIndex = 3;
			this.textBoxStretchY.Text = "";
			this.textBoxStretchY.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxStretchY_Validating);
			this.textBoxStretchY.Validated += new System.EventHandler(this.textBoxStretchY_Validated);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 104);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(224, 23);
			this.label3.TabIndex = 1;
			this.label3.Text = "Rotation in degrees (+ Counter Clockwise)";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 144);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(168, 23);
			this.label4.TabIndex = 2;
			this.label4.Text = "Skew in degrees (+ Clockwise)";
			// 
			// textBoxRotation
			// 
			this.textBoxRotation.Location = new System.Drawing.Point(248, 104);
			this.textBoxRotation.Name = "textBoxRotation";
			this.textBoxRotation.Size = new System.Drawing.Size(56, 20);
			this.textBoxRotation.TabIndex = 3;
			this.textBoxRotation.Text = "";
			this.textBoxRotation.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxRotation_Validating);
			this.textBoxRotation.Validated += new System.EventHandler(this.textBoxRotation_Validated);
			// 
			// textBoxSkew
			// 
			this.textBoxSkew.Location = new System.Drawing.Point(248, 144);
			this.textBoxSkew.Name = "textBoxSkew";
			this.textBoxSkew.Size = new System.Drawing.Size(56, 20);
			this.textBoxSkew.TabIndex = 4;
			this.textBoxSkew.Text = "";
			this.textBoxSkew.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxSkew_Validating);
			this.textBoxSkew.Validated += new System.EventHandler(this.textBoxSkew_Validated);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.label5,
																					this.textBoxR3C3,
																					this.textBoxR3C2,
																					this.textBoxR3C1,
																					this.textBoxR2C3,
																					this.textBoxR2C2,
																					this.textBoxR2C1,
																					this.textBoxR1C3,
																					this.textBoxR1C2,
																					this.textBoxR1C1});
			this.groupBox2.Location = new System.Drawing.Point(32, 16);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(256, 160);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Transform Matrix";
			// 
			// textBoxR1C1
			// 
			this.textBoxR1C1.Location = new System.Drawing.Point(16, 56);
			this.textBoxR1C1.Name = "textBoxR1C1";
			this.textBoxR1C1.Size = new System.Drawing.Size(64, 20);
			this.textBoxR1C1.TabIndex = 0;
			this.textBoxR1C1.Text = "";
			this.textBoxR1C1.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxR1C1_Validating);
			this.textBoxR1C1.Validated += new System.EventHandler(this.textBoxR1C1_Validated);
			// 
			// textBoxR1C2
			// 
			this.textBoxR1C2.Location = new System.Drawing.Point(96, 56);
			this.textBoxR1C2.Name = "textBoxR1C2";
			this.textBoxR1C2.Size = new System.Drawing.Size(64, 20);
			this.textBoxR1C2.TabIndex = 1;
			this.textBoxR1C2.Text = "";
			this.textBoxR1C2.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxR1C2_Validating);
			this.textBoxR1C2.Validated += new System.EventHandler(this.textBoxR1C2_Validated);
			// 
			// textBoxR1C3
			// 
			this.textBoxR1C3.Location = new System.Drawing.Point(176, 56);
			this.textBoxR1C3.Name = "textBoxR1C3";
			this.textBoxR1C3.Size = new System.Drawing.Size(64, 20);
			this.textBoxR1C3.TabIndex = 2;
			this.textBoxR1C3.Text = "";
			this.textBoxR1C3.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxR1C3_Validating);
			this.textBoxR1C3.Validated += new System.EventHandler(this.textBoxR1C3_Validated);
			// 
			// textBoxR2C1
			// 
			this.textBoxR2C1.Location = new System.Drawing.Point(16, 88);
			this.textBoxR2C1.Name = "textBoxR2C1";
			this.textBoxR2C1.Size = new System.Drawing.Size(64, 20);
			this.textBoxR2C1.TabIndex = 3;
			this.textBoxR2C1.Text = "";
			this.textBoxR2C1.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxR2C1_Validating);
			this.textBoxR2C1.Validated += new System.EventHandler(this.textBoxR2C1_Validated);
			// 
			// textBoxR2C2
			// 
			this.textBoxR2C2.Location = new System.Drawing.Point(96, 88);
			this.textBoxR2C2.Name = "textBoxR2C2";
			this.textBoxR2C2.Size = new System.Drawing.Size(64, 20);
			this.textBoxR2C2.TabIndex = 4;
			this.textBoxR2C2.Text = "";
			this.textBoxR2C2.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxR2C2_Validating);
			this.textBoxR2C2.Validated += new System.EventHandler(this.textBoxR2C2_Validated);
			// 
			// textBoxR2C3
			// 
			this.textBoxR2C3.Location = new System.Drawing.Point(176, 88);
			this.textBoxR2C3.Name = "textBoxR2C3";
			this.textBoxR2C3.Size = new System.Drawing.Size(64, 20);
			this.textBoxR2C3.TabIndex = 5;
			this.textBoxR2C3.Text = "";
			this.textBoxR2C3.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxR2C3_Validating);
			this.textBoxR2C3.Validated += new System.EventHandler(this.textBoxR2C3_Validated);
			// 
			// textBoxR3C1
			// 
			this.textBoxR3C1.Location = new System.Drawing.Point(16, 120);
			this.textBoxR3C1.Name = "textBoxR3C1";
			this.textBoxR3C1.Size = new System.Drawing.Size(64, 20);
			this.textBoxR3C1.TabIndex = 6;
			this.textBoxR3C1.Text = "";
			this.textBoxR3C1.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxR3C1_Validating);
			this.textBoxR3C1.Validated += new System.EventHandler(this.textBoxR3C1_Validated);
			// 
			// textBoxR3C2
			// 
			this.textBoxR3C2.Location = new System.Drawing.Point(96, 120);
			this.textBoxR3C2.Name = "textBoxR3C2";
			this.textBoxR3C2.Size = new System.Drawing.Size(64, 20);
			this.textBoxR3C2.TabIndex = 7;
			this.textBoxR3C2.Text = "";
			this.textBoxR3C2.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxR3C2_Validating);
			this.textBoxR3C2.Validated += new System.EventHandler(this.textBoxR3C2_Validated);
			// 
			// textBoxR3C3
			// 
			this.textBoxR3C3.Location = new System.Drawing.Point(176, 120);
			this.textBoxR3C3.Name = "textBoxR3C3";
			this.textBoxR3C3.Size = new System.Drawing.Size(64, 20);
			this.textBoxR3C3.TabIndex = 8;
			this.textBoxR3C3.Text = "";
			this.textBoxR3C3.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxR3C3_Validating);
			this.textBoxR3C3.Validated += new System.EventHandler(this.textBoxR3C3_Validated);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(224, 23);
			this.label5.TabIndex = 9;
			this.label5.Text = "Combined with values in the Settings tab.";
			// 
			// errorProvider1
			// 
			this.errorProvider1.DataMember = null;
			// 
			// FormTransform
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(344, 286);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnCancel,
																		  this.btnOK,
																		  this.tabControl1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTransform";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Transform Properties";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            m_RastTestTransform.stretchX = (float)Double.Parse(textBoxStretchX.Text);

            m_RastTestTransform.stretchY = (float)Double.Parse(textBoxStretchY.Text);
            m_RastTestTransform.rotation = (float)Double.Parse(textBoxRotation.Text);
            m_RastTestTransform.skew     = (float)Double.Parse(textBoxSkew.Text);

            m_RastTestTransform.matrix[0,0] = (float)Double.Parse(textBoxR1C1.Text);
            m_RastTestTransform.matrix[0,1] = (float)Double.Parse(textBoxR1C2.Text);
            m_RastTestTransform.matrix[0,2] = (float)Double.Parse(textBoxR1C3.Text);

            m_RastTestTransform.matrix[1,0] = (float)Double.Parse(textBoxR2C1.Text);
            m_RastTestTransform.matrix[1,1] = (float)Double.Parse(textBoxR2C2.Text);
            m_RastTestTransform.matrix[1,2] = (float)Double.Parse(textBoxR2C3.Text);

            m_RastTestTransform.matrix[2,0] = (float)Double.Parse(textBoxR3C1.Text);
            m_RastTestTransform.matrix[2,1] = (float)Double.Parse(textBoxR3C2.Text);
            m_RastTestTransform.matrix[2,2] = (float)Double.Parse(textBoxR3C3.Text);

            this.DialogResult = DialogResult.OK;
        }

        /**********************************************************************************
         * 
         * Validation for text boxes containing a float
         *
         */

        private void ValidateFloat(TextBox tb, object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tb.Text.Length == 0)
            {
                e.Cancel = true;
                this.errorProvider1.SetError(tb, "this value is required");
            }
            else
            {
                // make sure there is one or more digits in the string, no more than one decimal point, and no other characters
                int nDigits = 0;
                int nDecPts = 0;
                int nOthers = 0;
                for (int i=0; i<tb.Text.Length; i++)
                {
                    char c = tb.Text[i];
                    if (c>='0' && c<='9') nDigits++;
                    else if (c=='.') nDecPts++;
                    else nOthers++;
                }
                if (nDigits<1 || nDecPts>1 || nOthers!=0)
                {
                    e.Cancel = true;
                    errorProvider1.SetError(tb, "illegal data, enter a floating point number");
                }
            }
        }


        private void textBoxStretchX_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxStretchX, sender, e);
        }

        private void textBoxStretchX_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxStretchX, "");
        }

        private void textBoxStretchY_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxStretchY, sender, e);
        }

        private void textBoxStretchY_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxStretchY, "");
        }

        private void textBoxRotation_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxRotation, sender, e);
        }

        private void textBoxRotation_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxRotation, "");
        }

        private void textBoxSkew_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxSkew, sender, e);
        }

        private void textBoxSkew_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxSkew, "");
        }

        private void textBoxR1C1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxR1C1, sender, e);
        }

        private void textBoxR1C1_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxR1C1, "");
        }

        private void textBoxR1C2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxR1C2, sender, e);
        }

        private void textBoxR1C2_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxR1C2, "");
        }

        private void textBoxR1C3_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxR1C3, sender, e);
        }

        private void textBoxR1C3_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxR1C3, "");
        }

        private void textBoxR2C1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxR2C1, sender, e);
        }

        private void textBoxR2C1_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxR2C1, "");
        }

        private void textBoxR2C2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxR2C2, sender, e);
        }

        private void textBoxR2C2_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxR2C2, "");
        }

        private void textBoxR2C3_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxR2C3, sender, e);
        }

        private void textBoxR2C3_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxR2C3, "");
        }

        private void textBoxR3C1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxR3C1, sender, e);
        }

        private void textBoxR3C1_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxR3C1, "");
        }

        private void textBoxR3C2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxR3C2, sender, e);
        }

        private void textBoxR3C2_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxR3C2, "");
        }

        private void textBoxR3C3_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateFloat(textBoxR3C3, sender, e);
        }

        private void textBoxR3C3_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError(textBoxR3C3, "");
        }
    }
}
