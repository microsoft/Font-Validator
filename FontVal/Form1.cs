using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using OTFontFile;
using OTFontFileVal;


namespace FontVal
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button ClearAllTestsBtn;
        private System.Windows.Forms.Button SetAllTestsBtn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem10;
        private System.Windows.Forms.MenuItem menuItem15;
        private System.Windows.Forms.MenuItem menuItem18;
        private System.Windows.Forms.ToolBar toolBar1;
        private System.Windows.Forms.MenuItem menuItem20;
        private System.Windows.Forms.MenuItem menuItem32;
        private System.Windows.Forms.MenuItem menuItem33;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBoxFont;
        private System.Windows.Forms.Button btnAddFont;
        private System.Windows.Forms.Button btnRemoveFont;
        private System.Windows.Forms.CheckedListBox checkedListBoxTests;
        private System.Windows.Forms.ToolBarButton tbBtnNewProj;
        private System.Windows.Forms.ToolBarButton tbBtnOpenProj;
        private System.Windows.Forms.ToolBarButton tbBtnSaveProj;
        private System.Windows.Forms.ToolBarButton sep1;
        private System.Windows.Forms.ToolBarButton tbBtnOpenReport;
        private System.Windows.Forms.ToolBarButton tbBtnPrintReport;
        private System.Windows.Forms.ToolBarButton sep2;
        private System.Windows.Forms.ToolBarButton tbBtnValidate;
        private System.Windows.Forms.MenuItem menuItemFileExit;
        private System.Windows.Forms.MenuItem menuItemNewProj;
        private System.Windows.Forms.MenuItem menuItemOpenProj;
        private System.Windows.Forms.MenuItem menuItemOpenReport;
        private System.Windows.Forms.MenuItem menuItemCloseReport;
        private System.Windows.Forms.MenuItem menuItemSaveProj;
        private System.Windows.Forms.MenuItem menuItemSaveProjAs;
        private System.Windows.Forms.MenuItem menuItemSaveReportAs;
        private System.Windows.Forms.MenuItem menuItemPrint;
        private System.Windows.Forms.MenuItem menuItemRecentProj;
        private System.Windows.Forms.MenuItem menuItemRecentReports;
        private System.Windows.Forms.MenuItem menuItemEditCopy;
        private System.Windows.Forms.MenuItem menuItemEditSelectAll;
        private System.Windows.Forms.MenuItem menuItemHelpHelp;
        private System.Windows.Forms.MenuItem menuItemHelpAbout;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItemVal;
        private System.Windows.Forms.MenuItem menuItemValRun;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.MenuItem menuItemMRUProject1;
        private System.Windows.Forms.MenuItem menuItemMRUProject2;
        private System.Windows.Forms.MenuItem menuItemMRUProject3;
        private System.Windows.Forms.MenuItem menuItemMRUProject4;

        project m_project;
        Validator m_Validator;
        Progress m_formProgress;
        RastTestTransform m_RastTestTransform;
        PersistedData m_PersistedData;

        private System.Windows.Forms.MenuItem menuItemMRUReport1;
        private System.Windows.Forms.MenuItem menuItemMRUReport2;
        private System.Windows.Forms.MenuItem menuItemMRUReport3;
        private System.Windows.Forms.MenuItem menuItemMRUReport4;
        private System.Windows.Forms.MenuItem menuItemWinCascade;
        private System.Windows.Forms.MenuItem menuItemWinTileHorz;
        private System.Windows.Forms.MenuItem menuItemWinTileVert;
        private System.Windows.Forms.MenuItem menuItemWinCloseAll;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItemValAddFont;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxRast;
        private System.Windows.Forms.Button btnTransform;
        private System.Windows.Forms.TextBox textBoxPointSizes;
        private System.Windows.Forms.Label labelPointsizes;
        private System.Windows.Forms.TextBox textBoxXRes;
        private System.Windows.Forms.TextBox textBoxYRes;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuReportOptions;
        private System.Windows.Forms.CheckBox checkBoxBW;
        private System.Windows.Forms.CheckBox checkBoxGray;
        private System.Windows.Forms.CheckBox checkBoxClearType;
        private System.Windows.Forms.CheckBox checkBoxCTCompWidth;
        private System.Windows.Forms.CheckBox checkBoxCTVert;
        private System.Windows.Forms.CheckBox checkBoxCTBGR;
        private System.Windows.Forms.CheckBox checkBoxCTFractWidth;
        private System.Windows.Forms.GroupBox groupBoxCTFlags;
        private System.Windows.Forms.GroupBox groupBoxResolution;
        private System.Windows.Forms.MenuItem menuItemValRemoveFont;

        public Form1()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            m_Validator = new Validator();

            string [] sTableTypes = TableManager.GetKnownOTTableTypes();
            for (int i=0; i<sTableTypes.Length; i++)
            {
                checkedListBoxTests.Items.Add(sTableTypes[i], System.Windows.Forms.CheckState.Checked);
            }

            groupBoxCTFlags.Enabled = checkBoxClearType.Checked;

            m_project = new project();
            m_RastTestTransform = new RastTestTransform();

            m_PersistedData = new PersistedData();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.ClearAllTestsBtn = new System.Windows.Forms.Button();
			this.SetAllTestsBtn = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.btnRemoveFont = new System.Windows.Forms.Button();
			this.btnAddFont = new System.Windows.Forms.Button();
			this.listBoxFont = new System.Windows.Forms.ListBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.checkedListBoxTests = new System.Windows.Forms.CheckedListBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.groupBoxResolution = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxYRes = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxXRes = new System.Windows.Forms.TextBox();
			this.groupBoxCTFlags = new System.Windows.Forms.GroupBox();
			this.checkBoxCTFractWidth = new System.Windows.Forms.CheckBox();
			this.checkBoxCTBGR = new System.Windows.Forms.CheckBox();
			this.checkBoxCTVert = new System.Windows.Forms.CheckBox();
			this.checkBoxCTCompWidth = new System.Windows.Forms.CheckBox();
			this.checkBoxClearType = new System.Windows.Forms.CheckBox();
			this.checkBoxGray = new System.Windows.Forms.CheckBox();
			this.checkBoxBW = new System.Windows.Forms.CheckBox();
			this.labelPointsizes = new System.Windows.Forms.Label();
			this.textBoxPointSizes = new System.Windows.Forms.TextBox();
			this.checkBoxRast = new System.Windows.Forms.CheckBox();
			this.btnTransform = new System.Windows.Forms.Button();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemNewProj = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItemOpenProj = new System.Windows.Forms.MenuItem();
			this.menuItemOpenReport = new System.Windows.Forms.MenuItem();
			this.menuItemCloseReport = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItemSaveProj = new System.Windows.Forms.MenuItem();
			this.menuItemSaveProjAs = new System.Windows.Forms.MenuItem();
			this.menuItemSaveReportAs = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.menuItemPrint = new System.Windows.Forms.MenuItem();
			this.menuItem15 = new System.Windows.Forms.MenuItem();
			this.menuItemRecentProj = new System.Windows.Forms.MenuItem();
			this.menuItemMRUProject1 = new System.Windows.Forms.MenuItem();
			this.menuItemMRUProject2 = new System.Windows.Forms.MenuItem();
			this.menuItemMRUProject3 = new System.Windows.Forms.MenuItem();
			this.menuItemMRUProject4 = new System.Windows.Forms.MenuItem();
			this.menuItemRecentReports = new System.Windows.Forms.MenuItem();
			this.menuItemMRUReport1 = new System.Windows.Forms.MenuItem();
			this.menuItemMRUReport2 = new System.Windows.Forms.MenuItem();
			this.menuItemMRUReport3 = new System.Windows.Forms.MenuItem();
			this.menuItemMRUReport4 = new System.Windows.Forms.MenuItem();
			this.menuItem18 = new System.Windows.Forms.MenuItem();
			this.menuItemFileExit = new System.Windows.Forms.MenuItem();
			this.menuItem20 = new System.Windows.Forms.MenuItem();
			this.menuItemEditCopy = new System.Windows.Forms.MenuItem();
			this.menuItemEditSelectAll = new System.Windows.Forms.MenuItem();
			this.menuItemVal = new System.Windows.Forms.MenuItem();
			this.menuItemValAddFont = new System.Windows.Forms.MenuItem();
			this.menuItemValRemoveFont = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuReportOptions = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItemValRun = new System.Windows.Forms.MenuItem();
			this.menuItem32 = new System.Windows.Forms.MenuItem();
			this.menuItemWinCascade = new System.Windows.Forms.MenuItem();
			this.menuItemWinTileHorz = new System.Windows.Forms.MenuItem();
			this.menuItemWinTileVert = new System.Windows.Forms.MenuItem();
			this.menuItemWinCloseAll = new System.Windows.Forms.MenuItem();
			this.menuItem33 = new System.Windows.Forms.MenuItem();
			this.menuItemHelpHelp = new System.Windows.Forms.MenuItem();
			this.menuItemHelpAbout = new System.Windows.Forms.MenuItem();
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.tbBtnNewProj = new System.Windows.Forms.ToolBarButton();
			this.tbBtnOpenProj = new System.Windows.Forms.ToolBarButton();
			this.tbBtnSaveProj = new System.Windows.Forms.ToolBarButton();
			this.sep1 = new System.Windows.Forms.ToolBarButton();
			this.tbBtnOpenReport = new System.Windows.Forms.ToolBarButton();
			this.tbBtnPrintReport = new System.Windows.Forms.ToolBarButton();
			this.sep2 = new System.Windows.Forms.ToolBarButton();
			this.tbBtnValidate = new System.Windows.Forms.ToolBarButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.groupBoxResolution.SuspendLayout();
			this.groupBoxCTFlags.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ClearAllTestsBtn
			// 
			this.ClearAllTestsBtn.Location = new System.Drawing.Point(112, 9);
			this.ClearAllTestsBtn.Name = "ClearAllTestsBtn";
			this.ClearAllTestsBtn.Size = new System.Drawing.Size(80, 24);
			this.ClearAllTestsBtn.TabIndex = 4;
			this.ClearAllTestsBtn.Text = "Clear All";
			this.ClearAllTestsBtn.Click += new System.EventHandler(this.ClearAllTestsBtn_Click);
			// 
			// SetAllTestsBtn
			// 
			this.SetAllTestsBtn.Location = new System.Drawing.Point(8, 9);
			this.SetAllTestsBtn.Name = "SetAllTestsBtn";
			this.SetAllTestsBtn.Size = new System.Drawing.Size(80, 24);
			this.SetAllTestsBtn.TabIndex = 3;
			this.SetAllTestsBtn.Text = "Set All";
			this.SetAllTestsBtn.Click += new System.EventHandler(this.SetAllTestsBtn_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(0, 9);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(208, 492);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.btnRemoveFont);
			this.tabPage1.Controls.Add(this.btnAddFont);
			this.tabPage1.Controls.Add(this.listBoxFont);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(200, 362);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Font Files";
			// 
			// btnRemoveFont
			// 
			this.btnRemoveFont.Location = new System.Drawing.Point(112, 8);
			this.btnRemoveFont.Name = "btnRemoveFont";
			this.btnRemoveFont.Size = new System.Drawing.Size(80, 24);
			this.btnRemoveFont.TabIndex = 5;
			this.btnRemoveFont.Text = "Remove";
			this.btnRemoveFont.Click += new System.EventHandler(this.btnRemoveFont_Click);
			// 
			// btnAddFont
			// 
			this.btnAddFont.Location = new System.Drawing.Point(8, 8);
			this.btnAddFont.Name = "btnAddFont";
			this.btnAddFont.Size = new System.Drawing.Size(80, 24);
			this.btnAddFont.TabIndex = 4;
			this.btnAddFont.Text = "Add...";
			this.btnAddFont.Click += new System.EventHandler(this.btnAddFont_Click);
			// 
			// listBoxFont
			// 
			this.listBoxFont.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxFont.HorizontalScrollbar = true;
			this.listBoxFont.Location = new System.Drawing.Point(9, 45);
			this.listBoxFont.Name = "listBoxFont";
			this.listBoxFont.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxFont.Size = new System.Drawing.Size(183, 290);
			this.listBoxFont.TabIndex = 3;
			this.listBoxFont.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxFont_KeyDown);
			this.listBoxFont.SelectedIndexChanged += new System.EventHandler(this.listBoxFont_SelectedIndexChanged);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.checkedListBoxTests);
			this.tabPage2.Controls.Add(this.SetAllTestsBtn);
			this.tabPage2.Controls.Add(this.ClearAllTestsBtn);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(200, 348);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Table Tests";
			// 
			// checkedListBoxTests
			// 
			this.checkedListBoxTests.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.checkedListBoxTests.CheckOnClick = true;
			this.checkedListBoxTests.Location = new System.Drawing.Point(9, 45);
			this.checkedListBoxTests.Name = "checkedListBoxTests";
			this.checkedListBoxTests.Size = new System.Drawing.Size(183, 287);
			this.checkedListBoxTests.TabIndex = 5;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.groupBoxResolution);
			this.tabPage3.Controls.Add(this.groupBoxCTFlags);
			this.tabPage3.Controls.Add(this.checkBoxClearType);
			this.tabPage3.Controls.Add(this.checkBoxGray);
			this.tabPage3.Controls.Add(this.checkBoxBW);
			this.tabPage3.Controls.Add(this.labelPointsizes);
			this.tabPage3.Controls.Add(this.textBoxPointSizes);
			this.tabPage3.Controls.Add(this.checkBoxRast);
			this.tabPage3.Controls.Add(this.btnTransform);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(200, 466);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Rasterization";
			// 
			// groupBoxResolution
			// 
			this.groupBoxResolution.Controls.Add(this.label2);
			this.groupBoxResolution.Controls.Add(this.textBoxYRes);
			this.groupBoxResolution.Controls.Add(this.label3);
			this.groupBoxResolution.Controls.Add(this.textBoxXRes);
			this.groupBoxResolution.Location = new System.Drawing.Point(8, 240);
			this.groupBoxResolution.Name = "groupBoxResolution";
			this.groupBoxResolution.Size = new System.Drawing.Size(184, 48);
			this.groupBoxResolution.TabIndex = 20;
			this.groupBoxResolution.TabStop = false;
			this.groupBoxResolution.Text = "Resolution";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(24, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "X";
			// 
			// textBoxYRes
			// 
			this.textBoxYRes.Location = new System.Drawing.Point(128, 16);
			this.textBoxYRes.Name = "textBoxYRes";
			this.textBoxYRes.Size = new System.Drawing.Size(48, 20);
			this.textBoxYRes.TabIndex = 8;
			this.textBoxYRes.Text = "96";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(96, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(24, 23);
			this.label3.TabIndex = 2;
			this.label3.Text = "Y";
			// 
			// textBoxXRes
			// 
			this.textBoxXRes.Location = new System.Drawing.Point(40, 16);
			this.textBoxXRes.Name = "textBoxXRes";
			this.textBoxXRes.Size = new System.Drawing.Size(48, 20);
			this.textBoxXRes.TabIndex = 7;
			this.textBoxXRes.Text = "96";
			// 
			// groupBoxCTFlags
			// 
			this.groupBoxCTFlags.Controls.Add(this.checkBoxCTFractWidth);
			this.groupBoxCTFlags.Controls.Add(this.checkBoxCTBGR);
			this.groupBoxCTFlags.Controls.Add(this.checkBoxCTVert);
			this.groupBoxCTFlags.Controls.Add(this.checkBoxCTCompWidth);
			this.groupBoxCTFlags.Location = new System.Drawing.Point(40, 120);
			this.groupBoxCTFlags.Name = "groupBoxCTFlags";
			this.groupBoxCTFlags.Size = new System.Drawing.Size(152, 112);
			this.groupBoxCTFlags.TabIndex = 19;
			this.groupBoxCTFlags.TabStop = false;
			this.groupBoxCTFlags.Text = "ClearType flags";
			// 
			// checkBoxCTFractWidth
			// 
			this.checkBoxCTFractWidth.Enabled = false;
			this.checkBoxCTFractWidth.Location = new System.Drawing.Point(16, 88);
			this.checkBoxCTFractWidth.Name = "checkBoxCTFractWidth";
			this.checkBoxCTFractWidth.Size = new System.Drawing.Size(120, 16);
			this.checkBoxCTFractWidth.TabIndex = 3;
			this.checkBoxCTFractWidth.Text = "Fractional Width";
			// 
			// checkBoxCTBGR
			// 
			this.checkBoxCTBGR.Checked = true;
			this.checkBoxCTBGR.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCTBGR.Location = new System.Drawing.Point(16, 64);
			this.checkBoxCTBGR.Name = "checkBoxCTBGR";
			this.checkBoxCTBGR.Size = new System.Drawing.Size(120, 16);
			this.checkBoxCTBGR.TabIndex = 2;
			this.checkBoxCTBGR.Text = "BGR";
			// 
			// checkBoxCTVert
			// 
			this.checkBoxCTVert.Checked = true;
			this.checkBoxCTVert.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCTVert.Location = new System.Drawing.Point(16, 40);
			this.checkBoxCTVert.Name = "checkBoxCTVert";
			this.checkBoxCTVert.Size = new System.Drawing.Size(120, 16);
			this.checkBoxCTVert.TabIndex = 1;
			this.checkBoxCTVert.Text = "Vertical";
			// 
			// checkBoxCTCompWidth
			// 
			this.checkBoxCTCompWidth.Checked = true;
			this.checkBoxCTCompWidth.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCTCompWidth.Location = new System.Drawing.Point(16, 16);
			this.checkBoxCTCompWidth.Name = "checkBoxCTCompWidth";
			this.checkBoxCTCompWidth.Size = new System.Drawing.Size(120, 16);
			this.checkBoxCTCompWidth.TabIndex = 0;
			this.checkBoxCTCompWidth.Text = "Compatible Width";
			// 
			// checkBoxClearType
			// 
			this.checkBoxClearType.Checked = true;
			this.checkBoxClearType.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxClearType.Location = new System.Drawing.Point(24, 96);
			this.checkBoxClearType.Name = "checkBoxClearType";
			this.checkBoxClearType.Size = new System.Drawing.Size(104, 16);
			this.checkBoxClearType.TabIndex = 18;
			this.checkBoxClearType.Text = "ClearType";
			this.checkBoxClearType.CheckedChanged += new System.EventHandler(this.checkBoxClearType_CheckedChanged);
			// 
			// checkBoxGray
			// 
			this.checkBoxGray.Checked = true;
			this.checkBoxGray.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxGray.Location = new System.Drawing.Point(24, 72);
			this.checkBoxGray.Name = "checkBoxGray";
			this.checkBoxGray.Size = new System.Drawing.Size(104, 16);
			this.checkBoxGray.TabIndex = 17;
			this.checkBoxGray.Text = "Grayscale";
			// 
			// checkBoxBW
			// 
			this.checkBoxBW.Checked = true;
			this.checkBoxBW.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxBW.Location = new System.Drawing.Point(24, 48);
			this.checkBoxBW.Name = "checkBoxBW";
			this.checkBoxBW.Size = new System.Drawing.Size(104, 16);
			this.checkBoxBW.TabIndex = 16;
			this.checkBoxBW.Text = "BW";
			// 
			// labelPointsizes
			// 
			this.labelPointsizes.Location = new System.Drawing.Point(16, 296);
			this.labelPointsizes.Name = "labelPointsizes";
			this.labelPointsizes.Size = new System.Drawing.Size(72, 16);
			this.labelPointsizes.TabIndex = 15;
			this.labelPointsizes.Text = "Point size(s)";
			// 
			// textBoxPointSizes
			// 
			this.textBoxPointSizes.Location = new System.Drawing.Point(16, 320);
			this.textBoxPointSizes.Name = "textBoxPointSizes";
			this.textBoxPointSizes.Size = new System.Drawing.Size(168, 20);
			this.textBoxPointSizes.TabIndex = 14;
			this.textBoxPointSizes.Text = "4-72, 80, 88, 96, 102, 110, 118, 126";
			// 
			// checkBoxRast
			// 
			this.checkBoxRast.Checked = true;
			this.checkBoxRast.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxRast.Location = new System.Drawing.Point(8, 8);
			this.checkBoxRast.Name = "checkBoxRast";
			this.checkBoxRast.Size = new System.Drawing.Size(176, 40);
			this.checkBoxRast.TabIndex = 6;
			this.checkBoxRast.Text = "Test rasterization of TrueType outlines";
			this.checkBoxRast.CheckedChanged += new System.EventHandler(this.checkBoxRast_CheckedChanged);
			// 
			// btnTransform
			// 
			this.btnTransform.Location = new System.Drawing.Point(64, 352);
			this.btnTransform.Name = "btnTransform";
			this.btnTransform.Size = new System.Drawing.Size(80, 23);
			this.btnTransform.TabIndex = 1;
			this.btnTransform.Text = "Transform...";
			this.btnTransform.Click += new System.EventHandler(this.btnTransform_Click);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem20,
																					  this.menuItemVal,
																					  this.menuItem32,
																					  this.menuItem33});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItemNewProj,
																					  this.menuItem3,
																					  this.menuItemCloseReport,
																					  this.menuItem2,
																					  this.menuItemSaveProj,
																					  this.menuItemSaveProjAs,
																					  this.menuItemSaveReportAs,
																					  this.menuItem10,
																					  this.menuItemPrint,
																					  this.menuItem15,
																					  this.menuItemRecentProj,
																					  this.menuItemRecentReports,
																					  this.menuItem18,
																					  this.menuItemFileExit});
			this.menuItem1.Text = "&File";
			// 
			// menuItemNewProj
			// 
			this.menuItemNewProj.Index = 0;
			this.menuItemNewProj.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.menuItemNewProj.Text = "&New Project";
			this.menuItemNewProj.Click += new System.EventHandler(this.menuItemNewProj_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItemOpenProj,
																					  this.menuItemOpenReport});
			this.menuItem3.Text = "Open";
			// 
			// menuItemOpenProj
			// 
			this.menuItemOpenProj.Index = 0;
			this.menuItemOpenProj.Text = "&Project...";
			this.menuItemOpenProj.Click += new System.EventHandler(this.menuItemOpenProj_Click);
			// 
			// menuItemOpenReport
			// 
			this.menuItemOpenReport.Index = 1;
			this.menuItemOpenReport.Text = "&Report...";
			this.menuItemOpenReport.Click += new System.EventHandler(this.menuItemOpenReport_Click);
			// 
			// menuItemCloseReport
			// 
			this.menuItemCloseReport.Index = 2;
			this.menuItemCloseReport.Text = "&Close Report";
			this.menuItemCloseReport.Click += new System.EventHandler(this.menuItemCloseReport_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 3;
			this.menuItem2.Text = "-";
			// 
			// menuItemSaveProj
			// 
			this.menuItemSaveProj.Index = 4;
			this.menuItemSaveProj.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.menuItemSaveProj.Text = "&Save Project";
			this.menuItemSaveProj.Click += new System.EventHandler(this.menuItemSaveProj_Click);
			// 
			// menuItemSaveProjAs
			// 
			this.menuItemSaveProjAs.Index = 5;
			this.menuItemSaveProjAs.Text = "Save Project &As ...";
			this.menuItemSaveProjAs.Click += new System.EventHandler(this.menuItemSaveProjAs_Click);
			// 
			// menuItemSaveReportAs
			// 
			this.menuItemSaveReportAs.Index = 6;
			this.menuItemSaveReportAs.Text = "Save Report &As ...";
			this.menuItemSaveReportAs.Click += new System.EventHandler(this.menuItemSaveReportAs_Click);
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 7;
			this.menuItem10.Text = "-";
			// 
			// menuItemPrint
			// 
			this.menuItemPrint.Index = 8;
			this.menuItemPrint.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.menuItemPrint.Text = "&Print...";
			this.menuItemPrint.Click += new System.EventHandler(this.menuItemPrint_Click);
			// 
			// menuItem15
			// 
			this.menuItem15.Index = 9;
			this.menuItem15.Text = "-";
			// 
			// menuItemRecentProj
			// 
			this.menuItemRecentProj.Index = 10;
			this.menuItemRecentProj.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							   this.menuItemMRUProject1,
																							   this.menuItemMRUProject2,
																							   this.menuItemMRUProject3,
																							   this.menuItemMRUProject4});
			this.menuItemRecentProj.Text = "Recent Pro&jects";
			// 
			// menuItemMRUProject1
			// 
			this.menuItemMRUProject1.Index = 0;
			this.menuItemMRUProject1.Text = "";
			this.menuItemMRUProject1.Click += new System.EventHandler(this.menuItemMRUProject1_Click);
			// 
			// menuItemMRUProject2
			// 
			this.menuItemMRUProject2.Index = 1;
			this.menuItemMRUProject2.Text = "";
			this.menuItemMRUProject2.Click += new System.EventHandler(this.menuItemMRUProject2_Click);
			// 
			// menuItemMRUProject3
			// 
			this.menuItemMRUProject3.Index = 2;
			this.menuItemMRUProject3.Text = "";
			this.menuItemMRUProject3.Click += new System.EventHandler(this.menuItemMRUProject3_Click);
			// 
			// menuItemMRUProject4
			// 
			this.menuItemMRUProject4.Index = 3;
			this.menuItemMRUProject4.Text = "";
			this.menuItemMRUProject4.Click += new System.EventHandler(this.menuItemMRUProject4_Click);
			// 
			// menuItemRecentReports
			// 
			this.menuItemRecentReports.Index = 11;
			this.menuItemRecentReports.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								  this.menuItemMRUReport1,
																								  this.menuItemMRUReport2,
																								  this.menuItemMRUReport3,
																								  this.menuItemMRUReport4});
			this.menuItemRecentReports.Text = "Recen&t Reports";
			// 
			// menuItemMRUReport1
			// 
			this.menuItemMRUReport1.Index = 0;
			this.menuItemMRUReport1.Text = "r1";
			this.menuItemMRUReport1.Click += new System.EventHandler(this.menuItemMRUReport1_Click);
			// 
			// menuItemMRUReport2
			// 
			this.menuItemMRUReport2.Index = 1;
			this.menuItemMRUReport2.Text = "r2";
			this.menuItemMRUReport2.Click += new System.EventHandler(this.menuItemMRUReport2_Click);
			// 
			// menuItemMRUReport3
			// 
			this.menuItemMRUReport3.Index = 2;
			this.menuItemMRUReport3.Text = "r3";
			this.menuItemMRUReport3.Click += new System.EventHandler(this.menuItemMRUReport3_Click);
			// 
			// menuItemMRUReport4
			// 
			this.menuItemMRUReport4.Index = 3;
			this.menuItemMRUReport4.Text = "r4";
			this.menuItemMRUReport4.Click += new System.EventHandler(this.menuItemMRUReport4_Click);
			// 
			// menuItem18
			// 
			this.menuItem18.Index = 12;
			this.menuItem18.Text = "-";
			// 
			// menuItemFileExit
			// 
			this.menuItemFileExit.Index = 13;
			this.menuItemFileExit.Text = "E&xit";
			this.menuItemFileExit.Click += new System.EventHandler(this.menuItemFileExit_Click);
			// 
			// menuItem20
			// 
			this.menuItem20.Index = 1;
			this.menuItem20.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuItemEditCopy,
																					   this.menuItemEditSelectAll});
			this.menuItem20.Text = "&Edit";
			// 
			// menuItemEditCopy
			// 
			this.menuItemEditCopy.Index = 0;
			this.menuItemEditCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.menuItemEditCopy.Text = "&Copy";
			this.menuItemEditCopy.Click += new System.EventHandler(this.menuItemEditCopy_Click);
			// 
			// menuItemEditSelectAll
			// 
			this.menuItemEditSelectAll.Index = 1;
			this.menuItemEditSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.menuItemEditSelectAll.Text = "SelectA&ll";
			this.menuItemEditSelectAll.Click += new System.EventHandler(this.menuItemEditSelectAll_Click);
			// 
			// menuItemVal
			// 
			this.menuItemVal.Index = 2;
			this.menuItemVal.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menuItemValAddFont,
																						this.menuItemValRemoveFont,
																						this.menuItem6,
																						this.menuReportOptions,
																						this.menuItem5,
																						this.menuItemValRun});
			this.menuItemVal.Text = "&Validation";
			// 
			// menuItemValAddFont
			// 
			this.menuItemValAddFont.Index = 0;
			this.menuItemValAddFont.Text = "&Add Font...";
			this.menuItemValAddFont.Click += new System.EventHandler(this.menuItemValAddFont_Click);
			// 
			// menuItemValRemoveFont
			// 
			this.menuItemValRemoveFont.Index = 1;
			this.menuItemValRemoveFont.Text = "&Remove Font";
			this.menuItemValRemoveFont.Click += new System.EventHandler(this.menuItemValRemoveFont_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 2;
			this.menuItem6.Text = "-";
			// 
			// menuReportOptions
			// 
			this.menuReportOptions.Index = 3;
			this.menuReportOptions.Text = "Report &Options...";
			this.menuReportOptions.Click += new System.EventHandler(this.menuReportOptions_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 4;
			this.menuItem5.Text = "-";
			// 
			// menuItemValRun
			// 
			this.menuItemValRun.Index = 5;
			this.menuItemValRun.Text = "&Start";
			this.menuItemValRun.Click += new System.EventHandler(this.menuItemValRun_Click);
			// 
			// menuItem32
			// 
			this.menuItem32.Index = 3;
			this.menuItem32.MdiList = true;
			this.menuItem32.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuItemWinCascade,
																					   this.menuItemWinTileHorz,
																					   this.menuItemWinTileVert,
																					   this.menuItemWinCloseAll});
			this.menuItem32.Text = "&Window";
			// 
			// menuItemWinCascade
			// 
			this.menuItemWinCascade.Index = 0;
			this.menuItemWinCascade.Text = "&Cascade";
			this.menuItemWinCascade.Click += new System.EventHandler(this.menuItemWinCascade_Click);
			// 
			// menuItemWinTileHorz
			// 
			this.menuItemWinTileHorz.Index = 1;
			this.menuItemWinTileHorz.Text = "Tile &Horizontally";
			this.menuItemWinTileHorz.Click += new System.EventHandler(this.menuItemWinTileHorz_Click);
			// 
			// menuItemWinTileVert
			// 
			this.menuItemWinTileVert.Index = 2;
			this.menuItemWinTileVert.Text = "Tile &Vertically";
			this.menuItemWinTileVert.Click += new System.EventHandler(this.menuItemWinTileVert_Click);
			// 
			// menuItemWinCloseAll
			// 
			this.menuItemWinCloseAll.Index = 3;
			this.menuItemWinCloseAll.Text = "Close &All";
			this.menuItemWinCloseAll.Click += new System.EventHandler(this.menuItemWinCloseAll_Click);
			// 
			// menuItem33
			// 
			this.menuItem33.Index = 4;
			this.menuItem33.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuItemHelpHelp,
																					   this.menuItemHelpAbout});
			this.menuItem33.Text = "&Help";
			// 
			// menuItemHelpHelp
			// 
			this.menuItemHelpHelp.Index = 0;
			this.menuItemHelpHelp.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.menuItemHelpHelp.Text = "Font Validator &Help...";
			this.menuItemHelpHelp.Click += new System.EventHandler(this.menuItemHelpHelp_Click);
			// 
			// menuItemHelpAbout
			// 
			this.menuItemHelpAbout.Index = 1;
			this.menuItemHelpAbout.Text = "&About Font Validator...";
			this.menuItemHelpAbout.Click += new System.EventHandler(this.menuItemHelpAbout_Click);
			// 
			// toolBar1
			// 
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						this.tbBtnNewProj,
																						this.tbBtnOpenProj,
																						this.tbBtnSaveProj,
																						this.sep1,
																						this.tbBtnOpenReport,
																						this.tbBtnPrintReport,
																						this.sep2,
																						this.tbBtnValidate});
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(632, 28);
			this.toolBar1.TabIndex = 11;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// tbBtnNewProj
			// 
			this.tbBtnNewProj.ImageIndex = 0;
			this.tbBtnNewProj.ToolTipText = "New Project";
			// 
			// tbBtnOpenProj
			// 
			this.tbBtnOpenProj.ImageIndex = 1;
			this.tbBtnOpenProj.ToolTipText = "Open Project";
			// 
			// tbBtnSaveProj
			// 
			this.tbBtnSaveProj.ImageIndex = 2;
			this.tbBtnSaveProj.ToolTipText = "Save Project";
			// 
			// sep1
			// 
			this.sep1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbBtnOpenReport
			// 
			this.tbBtnOpenReport.ImageIndex = 3;
			this.tbBtnOpenReport.ToolTipText = "Open Report";
			// 
			// tbBtnPrintReport
			// 
			this.tbBtnPrintReport.ImageIndex = 4;
			this.tbBtnPrintReport.ToolTipText = "Print Report";
			// 
			// sep2
			// 
			this.sep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbBtnValidate
			// 
			this.tbBtnValidate.ImageIndex = 5;
			this.tbBtnValidate.ToolTipText = "Start Validation";
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tabControl1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 28);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(208, 501);
			this.panel1.TabIndex = 13;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(208, 28);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(8, 501);
			this.splitter1.TabIndex = 15;
			this.splitter1.TabStop = false;
			// 
			// Form1
			// 
			this.AllowDrop = true;
			if ( Type.GetType("Mono.Runtime") == null ) this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(632, 529);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolBar1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "Font Validator";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.groupBoxResolution.ResumeLayout(false);
			this.groupBoxCTFlags.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

        private void btnAddFont_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "OpenType Font files (*.ttf;*.ttc;*.otf)|*.ttf;*.ttc;*.otf|All files (*.*)|*.*" ;
            openFileDialog1.FilterIndex = 1 ;
            openFileDialog1.RestoreDirectory = true ;
            openFileDialog1.Multiselect = true;

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                for (int i=0; i<openFileDialog1.FileNames.Length; i++)
                {
                    listBoxFont.Items.Add(openFileDialog1.FileNames[i]);
                }
            }
        
        }

        private void btnRemoveFont_Click(object sender, System.EventArgs e)
        {
            while (listBoxFont.SelectedIndices.Count > 0)
            {
                listBoxFont.Items.RemoveAt(listBoxFont.SelectedIndices[listBoxFont.SelectedIndices.Count-1]);
            }
        }

        private void SetAllTestsBtn_Click(object sender, System.EventArgs e)
        {
            for (int i=0; i<checkedListBoxTests.Items.Count; i++)
            {
                checkedListBoxTests.SetItemChecked(i, true);
            }
        }

        private void ClearAllTestsBtn_Click(object sender, System.EventArgs e)
        {
            for (int i=0; i<checkedListBoxTests.Items.Count; i++)
            {
                checkedListBoxTests.SetItemChecked(i, false);
            }
        }

        void OnNewProject()
        {
            m_project = new project();
            ProjectToForm(m_project);
        }

        void OnOpenProject()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Font Validator Project files (*.fvp)|*.fvp|All files (*.*)|*.*";
            dlg.FilterIndex = 1 ;
            dlg.RestoreDirectory = true ;

            if(dlg.ShowDialog() == DialogResult.OK)
            {
                OpenProject(dlg.FileName);
            }
        }

        void OpenProject(String sFile)
        {
            m_project.LoadFromXmlFile(sFile);
            ProjectToForm(m_project);

            AddMRUProject(sFile);
        }

        void OnSaveProject()
        {
            if (m_project.GetFilename() == null)
            {
                OnSaveProjectAs();
            }
            else
            {
                // write the project file
                FormToProject(m_project);
                m_project.SaveToXmlFile();
            }
        }

        void OnSaveProjectAs()
        {
            // get the filename from the user
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Font Validator Project files (*.fvp)|*.fvp";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true ;
 
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                // write the project file
                FormToProject(m_project);
                m_project.SetFilename(dlg.FileName);
                m_project.SaveToXmlFile();

                AddMRUProject(dlg.FileName);
            }
        }

        void OnOpenReport()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Font Validator Report files (*.xml)|*.xml|All files (*.*)|*.*";
            dlg.FilterIndex = 1 ;
            dlg.RestoreDirectory = true ;
            dlg.Multiselect = true;

            if(dlg.ShowDialog() == DialogResult.OK)
            {
                for (int i=0; i<dlg.FileNames.Length; i++)
                {
                    FileInfo fi = new FileInfo(dlg.FileNames[i]);
                    OpenReport(fi.FullName, fi.Name, false);
                }
            }
        }

        public delegate void OpenReportDelegate(string sFile, string sCaption, bool bDeleteOnClose);
        public void OpenReport(String sFile, string sCaption, bool bDeleteOnClose)
        {
            ResultsForm rf = new ResultsForm();
            rf.MdiParent = this;
            rf.Text = sFile;
            rf.Show();
            rf.ShowFile(sFile, sCaption, bDeleteOnClose);
            LayoutMdi(MdiLayout.Cascade); // this is necessary because the mshtml control was messing up the default window placement
            Invalidate(true); // this is necessary to clean up some paint bugs in the current .Net framework

            if (m_PersistedData.m_ReportFileDestination != ReportFileDestination.TempFiles)
            {
                AddMRUReport(sFile);
            }
            /*
            WebBrowse wb = new WebBrowse( sFile );
            wb.MdiParent = this;
            wb.Show();
            */
        }

        void OnPrintReport()
        {
            ResultsForm rf = (ResultsForm)ActiveMdiChild;
            if (rf != null)
            {
                rf.PrintFile();
            }
            else
            {
                MessageBox.Show(this, "nothing to print!");
            }
        }

        void BeginValidation()
        {
            if (listBoxFont.Items.Count == 0)
            {
                MessageBox.Show(this, "No fonts to validate!\r\nYou must first add one or more fonts to the font file list.");
            }
            else
            {
                // disable the ui during validation

                EnableUI(false);


                try
                {
                    // tell the validator which tests to perform

                    for (int i=0; i<checkedListBoxTests.Items.Count; i++)
                    {
                        string sTable = checkedListBoxTests.Items[i].ToString();
                        bool bPerform = checkedListBoxTests.GetItemChecked(i);
                        m_Validator.SetTablePerformTest(sTable, bPerform);
                    }

                    if (checkBoxRast.Checked)
                    {
                        m_Validator.SetRastPerformTest(checkBoxBW.Checked, checkBoxGray.Checked, checkBoxClearType.Checked,
                            checkBoxCTCompWidth.Checked, checkBoxCTVert.Checked, checkBoxCTBGR.Checked, checkBoxCTFractWidth.Checked);
                    }
                    else
                    {
                        m_Validator.SetRastPerformTest(false, false, false, false, false, false, false);
                    }
                    int x = Int32.Parse(textBoxXRes.Text);
                    int y = Int32.Parse(textBoxYRes.Text);
                    int [] pointsizes = ParseRastPointSizes();
                    
                    m_Validator.SetRastTestParams(x, y, pointsizes, m_RastTestTransform);


                    // put up the progress dialog which will manage the validation

                    string [] sFiles = new string[listBoxFont.Items.Count];
                    for (int i=0; i<listBoxFont.Items.Count; i++)
                    {
                        sFiles[i] = listBoxFont.Items[i].ToString();
                    }
                    m_formProgress = new Progress(this, m_Validator, sFiles, 
                        m_PersistedData.m_ReportFileDestination,
                        m_PersistedData.m_bOpenReportFile,
                        m_PersistedData.m_sReportFixedDir);
                    m_formProgress.TopLevel = false;
                    m_formProgress.Parent = this;
                    m_formProgress.BringToFront();
                    // centering isn't working with the current .Net framework, so force it to be centered
                    //m_formProgress.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                    m_formProgress.SetBounds(Width/2 - m_formProgress.Width/2, Height/2 - m_formProgress.Height/2,
                        m_formProgress.Width, m_formProgress.Height, BoundsSpecified.Location);

                    m_formProgress.Show();
                }
                catch (Exception e)
                {
                    MessageBox.Show(this, e.Message, "Error");
                    EnableUI(true);
                }
            }
        }

        public int[] ParseRastPointSizes()
        {
            ArrayList arrPointSizes = new ArrayList();
            string s = textBoxPointSizes.Text;
            Int32 n1;
            Int32 n2;
            while (s.Length != 0)
            {
                if (Char.IsDigit(s, 0))
                {
                    // extract the number
                    n1=0;
                    while (s.Length != 0)
                    {
                        n1 = n1*10 + (s[0]-'0');
                        s = s.Substring(1, s.Length-1);
                        if (s.Length == 0)
                        {
                            break;
                        }
                        else if (!Char.IsDigit(s,0))
                        {
                            break;
                        }
                    }

                    // check if this is a range of numbers
                    n2 = -1;
                    while (s.Length != 0)
                    {
                        if (s[0] == '-')
                        {
                            // seek to the number
                            while (s.Length != 0)
                            {
                                s = s.Substring(1, s.Length-1);
                                if (s.Length == 0)
                                {
                                    break;
                                }
                                else if (Char.IsDigit(s,0))
                                {
                                    break;
                                }
                            }
                            // extract the number
                            n2=0;
                            while (s.Length != 0)
                            {
                                n2 = n2*10 + (s[0]-'0');
                                s = s.Substring(1, s.Length-1);
                                if (s.Length == 0)
                                {
                                    break;
                                }
                                else if (!Char.IsDigit(s,0))
                                {
                                    break;
                                }
                            }
                            break;
                        }
                        else if (!Char.IsDigit(s, 0))
                        {
                            s = s.Substring(1, s.Length-1);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (n2 == -1)
                    {
                        // add the point size
                        arrPointSizes.Add(n1);
                    }
                    else
                    {
                        // add each point in the range
                        for (Int32 i=n1; i<=n2; i++)
                        {
                            arrPointSizes.Add(i);
                        }
                    }
                }
                else
                {
                    s = s.Substring(1, s.Length-1);
                }
            }

            int [] sizearray = new int[arrPointSizes.Count];
            for (int i=0; i<arrPointSizes.Count; i++)
            {
                sizearray[i] = (Int32)arrPointSizes[i];
            }

            return sizearray;
        }

        public delegate void UpdateBoolDelegate( bool val );
        public delegate void UpdateIthBoolDelegate( int i, bool val );
        private void UpdateIthMenuItem( int i, bool val )
        {
            mainMenu1.MenuItems[i].Enabled = val;
        }
        private void UpdateIthButton( int i, bool val )
        {
            toolBar1.Buttons[i].Enabled = val;
        }
        private void UpdateTabControl( bool val )
        {
            tabControl1.Enabled = val;
        }


        public void EnableUI(bool bEnabled)
        {
            UpdateIthBoolDelegate d1 = new UpdateIthBoolDelegate( UpdateIthMenuItem );
            for (int i=0; i<mainMenu1.MenuItems.Count; i++)
            {
                //JJF mainMenu1.MenuItems[i].Enabled = bEnabled;
                //JJF mainMenu1.MenuItems[i].Invoke( d1, new object[]{ i, bEnabled } );
                tabControl1.Invoke( d1, new object[]{ i, bEnabled } );
            }

            UpdateIthBoolDelegate d2 = new UpdateIthBoolDelegate( UpdateIthButton );
            for (int i=0; i<toolBar1.Buttons.Count; i++)
            {
                //JJF toolBar1.Buttons[i].Enabled = bEnabled;
                //JJF toolBar1.Buttons[i].Invoke( d2, new object[]{ i, bEnabled } );
                tabControl1.Invoke( d2, new object[]{ i, bEnabled } );
            }

            //JJF tabControl1.Enabled = bEnabled;
            UpdateBoolDelegate d3 = new UpdateBoolDelegate( UpdateTabControl );
            tabControl1.Invoke( d3, new object[]{ bEnabled } );
        }

        /**************************************************************************
         * 
         * button handling
         * 
         **************************************************************************/

        private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
        {
            if (e.Button == tbBtnNewProj)
            {
                OnNewProject();
            }
            else if (e.Button == tbBtnOpenProj)
            {
                OnOpenProject();
            }
            else if (e.Button == tbBtnSaveProj)
            {
                OnSaveProject();
            }
            else if (e.Button == tbBtnOpenReport)
            {
                OnOpenReport();
            }
            else if (e.Button == tbBtnPrintReport)
            {
                OnPrintReport();
            }
            else if (e.Button == tbBtnValidate)
            {
                BeginValidation();
            }
        }


        /**************************************************************************
         * 
         * menu handlers
         * 
         **************************************************************************/


        /*********************
         * file menu
         */

        private void menuItemNewProj_Click(object sender, System.EventArgs e)
        {
            OnNewProject();
        }

        private void menuItemOpenProj_Click(object sender, System.EventArgs e)
        {
            OnOpenProject();
        }

        private void menuItemOpenReport_Click(object sender, System.EventArgs e)
        {
            OnOpenReport();
        }

        private void menuItemCloseReport_Click(object sender, System.EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                ActiveMdiChild.Close();
            }
            else
            {
                MessageBox.Show(this, "No report to close!");
            }
        }

        private void menuItemSaveProj_Click(object sender, System.EventArgs e)
        {
            OnSaveProject();
        }

        private void menuItemSaveProjAs_Click(object sender, System.EventArgs e)
        {
            OnSaveProjectAs();
        }

        private void menuItemSaveReportAs_Click(object sender, System.EventArgs e)
        {
            ResultsForm rf = (ResultsForm)ActiveMdiChild;
            if (rf != null)
            {
                // get the filename from the user
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "report files (*.xml)|*.xml";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true ;
                dlg.FileName = rf.GetFilename();
 
                if(dlg.ShowDialog() == DialogResult.OK)
                {
                    rf.SaveReportAs(dlg.FileName);
                    Progress.CopyXslFile(dlg.FileName);
                    AddMRUReport(dlg.FileName);
                }
            }
            else
            {
                MessageBox.Show(this, "no report to save!");
            }
        }

        private void menuItemPrint_Click(object sender, System.EventArgs e)
        {
            OnPrintReport();
        }

        private void menuItemFileExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        /*********************
         * edit menu
         */

        private void menuItemEditCopy_Click(object sender, System.EventArgs e)
        {
            ResultsForm rf = (ResultsForm)ActiveMdiChild;
            if (rf != null)
            {
                rf.CopyToClipboard();
            }
            else
            {
                MessageBox.Show(this, "nothing to copy!");
            }
        }

        private void menuItemEditSelectAll_Click(object sender, System.EventArgs e)
        {
            if (this.ActiveControl == listBoxFont)
            {
                for (int i=0; i<listBoxFont.Items.Count; i++)
                {
                    listBoxFont.SetSelected(i, true);
                }
            }
            else
            {
                ResultsForm rf = (ResultsForm)ActiveMdiChild;
                if (rf != null)
                {
                    rf.SelectAll();
                }
                else
                {
                    MessageBox.Show(this, "nothing to select!");
                }
            }
        }

        /*********************
         * validation menu
         */

        private void menuItemValRun_Click(object sender, System.EventArgs e)
        {
            BeginValidation();
        }

        /*********************
         * window menu
         */

        /*********************
         * help menu
         */

        private void menuItemHelpHelp_Click(object sender, System.EventArgs e)
        {
            Help.ShowHelp(this, "fontvalidatorhelp.chm");
        }

        private void menuItemHelpAbout_Click(object sender, System.EventArgs e)
        {
            FormAbout aboutForm = new FormAbout();
            aboutForm.ShowDialog(this);
        }


        /*********************
         * project support
         */

        private String GetComputerName()
        {
            String sComputerName;
            try
            {
                sComputerName = SystemInformation.ComputerName;
            }
            catch
            {
                sComputerName = "unavailable";
            }

            return sComputerName;
        }

        private void FormToProject(project prj)
        {
            // set the network name for this computer
            prj.SetComputerName(GetComputerName());

            // set files to use
            string [] sFiles = new string[listBoxFont.Items.Count];
            for (int i=0; i<listBoxFont.Items.Count; i++)
            {
                sFiles[i] = listBoxFont.Items[i].ToString();
            }
            prj.SetFilesToTest(sFiles);

            // set which tests to perform
            for (int i=0; i<checkedListBoxTests.Items.Count; i++)
            {
                string sTable = checkedListBoxTests.Items[i].ToString();
                bool bPerform = checkedListBoxTests.GetItemChecked(i);
                prj.SetTableTest(sTable, bPerform);
            }
        }

        private void ProjectToForm(project prj)
        {
            // compare the project's computername with the network name for this computer
            if (prj.GetComputerName() != GetComputerName())
            {
                String sMsg = "Warning!  This project was created on a different computer named" +
                    "'" + prj.GetComputerName() + "'.  Some files may not be accessible.";
                MessageBox.Show(this, sMsg);
            }

            // load the files
            listBoxFont.Items.Clear();
            string [] sFiles = prj.GetFilesToTest();
            if (sFiles != null)
            {
                for (int i=0; i<sFiles.Length; i++)
                {
                    listBoxFont.Items.Add(sFiles[i]);
                }
            }

            // load the tests
            for (int i=0; i<checkedListBoxTests.Items.Count; i++)
            {
                string sTable = checkedListBoxTests.Items[i].ToString();
                bool bPerform = prj.GetTableTest(sTable);
                checkedListBoxTests.SetItemChecked(i, bPerform);
            }
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            // load PersistedData

            FileStream fs = null;
            try
            {
                // Create an XmlSerializer for the PersistedData type.
                XmlSerializer ser = new XmlSerializer(typeof(PersistedData));
                FileInfo fi = new FileInfo(Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + @"FontVal.Data");
                // If the config file exists, open it.
                if(fi.Exists)
                {
                    fs = fi.OpenRead();
                    m_PersistedData = (PersistedData)ser.Deserialize(fs);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // If the FileStream is open, close it.
                if (fs != null) fs.Close();
            }

            UpdateMRUMenus();

            // hack the size of the listbox if it is bigger than the tabcontrol
            // which happens courtesy of the current .Net framework when running the program 
            // on a display that is greater than 96 dpi
            
            if (listBoxFont.Right > tabControl1.Width || listBoxFont.Bottom > tabControl1.Height)
            {
                listBoxFont.SetBounds(listBoxFont.Location.X, listBoxFont.Location.Y,
                    tabControl1.Width-listBoxFont.Left*3, tabControl1.Height-listBoxFont.Top-listBoxFont.Left*3, BoundsSpecified.Size);
            }

            // show help if this is the first time the program is being run
            if (m_PersistedData.m_bFirstTime)
            {
                Help.ShowHelp(this, "fontvalidatorhelp.chm", HelpNavigator.Topic, "usingvalidator.htm");
                m_PersistedData.m_bFirstTime = false;
            }
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            // save application settings
            StreamWriter sw = null;
            try
            {
                // Create an XmlSerializer for the PersistedData type.
                XmlSerializer ser = new XmlSerializer(typeof(PersistedData));
                sw = new StreamWriter(Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + @"FontVal.Data", false);
                // Serialize the instance of the PersistedData class
                ser.Serialize(sw, m_PersistedData);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
            finally
            {
                // If the StreamWriter is open, close it.
                if (sw != null) sw.Close();
            }
        }

        private void AddMRUProject(String sProject)
        {
            int iDeletePos = m_PersistedData.m_sMRUProjects.Length-1;
            for (int i=0; i<m_PersistedData.m_sMRUProjects.Length; i++)
            {
                if (m_PersistedData.m_sMRUProjects[i] == sProject)
                {
                    iDeletePos = i;
                    break;
                }
            }

            for (int i=iDeletePos; i>0; i--)
            {
                m_PersistedData.m_sMRUProjects[i] = m_PersistedData.m_sMRUProjects[i-1];
            }

            m_PersistedData.m_sMRUProjects[0] = sProject;
            UpdateMRUMenus();
        }

        private void AddMRUReport(String sReport)
        {
            int iDeletePos = m_PersistedData.m_sMRUReports.Length-1;
            for (int i=0; i<m_PersistedData.m_sMRUReports.Length; i++)
            {
                if (m_PersistedData.m_sMRUReports[i] == sReport)
                {
                    iDeletePos = i;
                    break;
                }
            }

            for (int i=iDeletePos; i>0; i--)
            {
                m_PersistedData.m_sMRUReports[i] = m_PersistedData.m_sMRUReports[i-1];
            }

            m_PersistedData.m_sMRUReports[0] = sReport;
            UpdateMRUMenus();
        }

        private void UpdateMRUMenus()
        {
            menuItemMRUProject1.Text = m_PersistedData.m_sMRUProjects[0];
            menuItemMRUProject2.Text = m_PersistedData.m_sMRUProjects[1];
            menuItemMRUProject3.Text = m_PersistedData.m_sMRUProjects[2];
            menuItemMRUProject4.Text = m_PersistedData.m_sMRUProjects[3];
            
            bool bProjects = false;
            for (int i=0; i<4; i++)
            {
                if (m_PersistedData.m_sMRUProjects[i] != null)
                {
                    if (m_PersistedData.m_sMRUProjects[i].Length > 0)
                    {
                        bProjects = true;
                    }
                }
            }

            menuItemRecentProj.Enabled = bProjects;



            menuItemMRUReport1.Text = m_PersistedData.m_sMRUReports[0];
            menuItemMRUReport2.Text = m_PersistedData.m_sMRUReports[1];
            menuItemMRUReport3.Text = m_PersistedData.m_sMRUReports[2];
            menuItemMRUReport4.Text = m_PersistedData.m_sMRUReports[3];

            bool bReports = false;
            for (int i=0; i<4; i++)
            {
                if (m_PersistedData.m_sMRUReports[i] != null)
                {
                    if (m_PersistedData.m_sMRUReports[i].Length > 0)
                    {
                        bReports = true;
                    }
                }
            }

            menuItemRecentReports.Enabled = bReports;
        }


        private void menuItemMRUProject1_Click(object sender, System.EventArgs e)
        {
            if (m_PersistedData.m_sMRUProjects[0] != "")
            {
                OpenProject(m_PersistedData.m_sMRUProjects[0]);
            }
        }

        private void menuItemMRUProject2_Click(object sender, System.EventArgs e)
        {
            if (m_PersistedData.m_sMRUProjects[1] != "")
            {
                OpenProject(m_PersistedData.m_sMRUProjects[1]);
            }
        }

        private void menuItemMRUProject3_Click(object sender, System.EventArgs e)
        {
            if (m_PersistedData.m_sMRUProjects[2] != "")
            {
                OpenProject(m_PersistedData.m_sMRUProjects[2]);
            }
        }

        private void menuItemMRUProject4_Click(object sender, System.EventArgs e)
        {
            if (m_PersistedData.m_sMRUProjects[3] != "")
            {
                OpenProject(m_PersistedData.m_sMRUProjects[3]);
            }
        }

        private void menuItemMRUReport1_Click(object sender, System.EventArgs e)
        {
            if (m_PersistedData.m_sMRUReports[0] != "")
            {
                FileInfo fi = new FileInfo(m_PersistedData.m_sMRUReports[0]);
                OpenReport(fi.FullName, fi.Name, false);
            }
        }

        private void menuItemMRUReport2_Click(object sender, System.EventArgs e)
        {
            if (m_PersistedData.m_sMRUReports[1] != "")
            {
                FileInfo fi = new FileInfo(m_PersistedData.m_sMRUReports[1]);
                OpenReport(fi.FullName, fi.Name, false);
            }
        }

        private void menuItemMRUReport3_Click(object sender, System.EventArgs e)
        {
            if (m_PersistedData.m_sMRUReports[2] != "")
            {
                FileInfo fi = new FileInfo(m_PersistedData.m_sMRUReports[2]);
                OpenReport(fi.FullName, fi.Name, false);
            }
        }

        private void menuItemMRUReport4_Click(object sender, System.EventArgs e)
        {
            if (m_PersistedData.m_sMRUReports[3] != "")
            {
                FileInfo fi = new FileInfo(m_PersistedData.m_sMRUReports[3]);
                OpenReport(fi.FullName, fi.Name, false);
            }
        }

        private void menuItemWinCascade_Click(object sender, System.EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void menuItemWinTileHorz_Click(object sender, System.EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void menuItemWinTileVert_Click(object sender, System.EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void menuItemWinCloseAll_Click(object sender, System.EventArgs e)
        {
            while (MdiChildren.Length != 0)
            {
                MdiChildren[MdiChildren.Length-1].Close();
            }
        }

        private void menuItemValAddFont_Click(object sender, System.EventArgs e)
        {
            if (tabControl1.SelectedIndex != 0)
            {
                tabControl1.SelectedIndex = 0;
            }
            btnAddFont_Click(sender, e);
        }

        private void menuItemValRemoveFont_Click(object sender, System.EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                btnRemoveFont_Click(sender, e);
            }
            else
            {
                MessageBox.Show(this, 
                    "select the 'Fonts Files' tab before attempting to remove a font from the list",
                    "Warning: font not removed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void listBoxFont_SelectedIndexChanged(object sender, System.EventArgs e)
        {
        
        }

        private void checkBoxRast_CheckedChanged(object sender, System.EventArgs e)
        {
            bool bChecked = checkBoxRast.Checked;
            checkBoxBW.Enabled = bChecked;
            checkBoxGray.Enabled = bChecked;
            checkBoxClearType.Enabled = bChecked;
            if (bChecked)
            {
                groupBoxCTFlags.Enabled = checkBoxClearType.Checked;
            }
            else
            {
                groupBoxCTFlags.Enabled = bChecked;
            }

            groupBoxResolution.Enabled = bChecked;
            labelPointsizes.Enabled = bChecked;
            textBoxPointSizes.Enabled = bChecked;
            btnTransform.Enabled = bChecked;
        }


        private void checkBoxClearType_CheckedChanged(object sender, System.EventArgs e)
        {
            groupBoxCTFlags.Enabled = checkBoxClearType.Checked;
        }

        private void btnTransform_Click(object sender, System.EventArgs e)
        {
            FormTransform form = new FormTransform(m_RastTestTransform);
            form.ShowDialog(this);
        }

        private void menuReportOptions_Click(object sender, System.EventArgs e)
        {
            FormReportOptions fro = new FormReportOptions();
            fro.OpenReportFile        = m_PersistedData.m_bOpenReportFile;
            fro.ReportFileDestination = m_PersistedData.m_ReportFileDestination;
            fro.FixedDir              = m_PersistedData.m_sReportFixedDir;
            DialogResult dr = fro.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                m_PersistedData.m_bOpenReportFile       = fro.OpenReportFile;
                m_PersistedData.m_ReportFileDestination = fro.ReportFileDestination;
                m_PersistedData.m_sReportFixedDir       = fro.FixedDir;
            }
        }

        private void listBoxFont_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                btnRemoveFont_Click(null, null);
            }
        }

        private void Form1_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string [] arrFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int i=0; i<arrFiles.Length; i++)
            {
                listBoxFont.Items.Add(arrFiles[i]);
            }
            // show the font pane if necessary
            if (tabControl1.SelectedIndex != 0)
            {
                tabControl1.SelectedIndex = 0;
            }
        }

    }

    public class PersistedData
    {
        public PersistedData()
        {
            m_sMRUProjects = new String[4];
            m_sMRUReports  = new String[4];
            m_ReportFileDestination = ReportFileDestination.TempFiles;
            m_bOpenReportFile = true;
            m_sReportFixedDir = "";
            m_bFirstTime = true;
        }

        public String [] m_sMRUProjects;
        public String [] m_sMRUReports;
        public ReportFileDestination m_ReportFileDestination;
        public bool m_bOpenReportFile;
        public string m_sReportFixedDir;
        public bool m_bFirstTime;
    }

}
