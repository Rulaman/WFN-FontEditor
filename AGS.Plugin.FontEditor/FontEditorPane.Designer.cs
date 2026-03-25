namespace AGS.Plugin.FontEditor
{
	partial class FontEditorPane
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if ( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Komponenten-Designer generierter Code

		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.FlowCharacterPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.GroupBox = new System.Windows.Forms.GroupBox();
            this.TxtGlyphRange = new System.Windows.Forms.TextBox();
            this.BtnPagePrev = new System.Windows.Forms.Button();
            this.BtnPageNext = new System.Windows.Forms.Button();
            this.TxtPageNumber = new System.Windows.Forms.TextBox();
            this.LblPageTotal = new System.Windows.Forms.Label();
            this.ZoomDrawingArea = new System.Windows.Forms.TrackBar();
			this.LblZoom = new System.Windows.Forms.Label();
			this.PanelSize = new System.Windows.Forms.Panel();
			this.LblWidth = new System.Windows.Forms.Label();
			this.numWidth = new System.Windows.Forms.NumericUpDown();
			this.LblHeight = new System.Windows.Forms.Label();
			this.numHeight = new System.Windows.Forms.NumericUpDown();
			this.BtnClear = new System.Windows.Forms.Button();
			this.BtnFill = new System.Windows.Forms.Button();
			this.BtnShiftUp = new System.Windows.Forms.Button();
			this.BtnShiftLeft = new System.Windows.Forms.Button();
			this.BtnShiftRight = new System.Windows.Forms.Button();
			this.BtnShiftDown = new System.Windows.Forms.Button();
			this.ChkGrid = new System.Windows.Forms.CheckBox();
			this.BtnInvert = new System.Windows.Forms.Button();
			this.PanelMouseColor = new System.Windows.Forms.Panel();
			this.LblRight = new System.Windows.Forms.Label();
			this.LblLeft = new System.Windows.Forms.Label();
			this.PictSwap = new System.Windows.Forms.PictureBox();
			this.PanelRightMouse = new System.Windows.Forms.Panel();
			this.PanelLeftMouse = new System.Windows.Forms.Panel();
			this.DrawingArea = new System.Windows.Forms.PictureBox();
			this.BtnSwapHorizontally = new System.Windows.Forms.Button();
			this.BtnSwapVertically = new System.Windows.Forms.Button();
			this.BtnOutline = new System.Windows.Forms.Button();
			this.BtnOutlineFont = new System.Windows.Forms.Button();
			this.BtnRenderText = new System.Windows.Forms.Button();
			this.PictRenderText = new System.Windows.Forms.PictureBox();
			this.BtnSetText = new System.Windows.Forms.Button();
			this.LblCharacter = new System.Windows.Forms.Label();
			this.BtnPrevious = new System.Windows.Forms.Button();
			this.BtnNext = new System.Windows.Forms.Button();
			this.ChkGridFix = new System.Windows.Forms.CheckBox();
			this.BtnAllHeight = new System.Windows.Forms.Button();
			this.BtnAllWidth = new System.Windows.Forms.Button();
            this.BtnAllBlankClear = new System.Windows.Forms.Button();
            this.TxtCharacter = new System.Windows.Forms.TextBox();
            this.TxtGlyph = new System.Windows.Forms.TextBox();
            this.GrpAllCharacters = new System.Windows.Forms.GroupBox();
			this.GrpOneCharacter = new System.Windows.Forms.GroupBox();
			this.ChkOneAllCharacters = new System.Windows.Forms.CheckBox();
			this.GroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ZoomDrawingArea)).BeginInit();
			this.PanelSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
			this.PanelMouseColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PictSwap)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.DrawingArea)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PictRenderText)).BeginInit();
			this.GrpAllCharacters.SuspendLayout();
			this.GrpOneCharacter.SuspendLayout();
			this.SuspendLayout();
			// 
			// FlowCharacterPanel
			// 
			this.FlowCharacterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.FlowCharacterPanel.AutoScroll = true;
			this.FlowCharacterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
			this.FlowCharacterPanel.Location = new System.Drawing.Point(6, 28);
			this.FlowCharacterPanel.Name = "FlowCharacterPanel";
			this.FlowCharacterPanel.Size = new System.Drawing.Size(307, 433);
			this.FlowCharacterPanel.TabIndex = 4;
			this.FlowCharacterPanel.Click += new System.EventHandler(this.FlowCharacterPanel_Click);
			// 
			// GroupBox
			// 
			this.GroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.GroupBox.Controls.Add(this.FlowCharacterPanel);
            this.GroupBox.Controls.Add(this.TxtGlyphRange);
            this.GroupBox.Location = new System.Drawing.Point(6, 6);
			this.GroupBox.Name = "GroupBox";
			this.GroupBox.Size = new System.Drawing.Size(320, 467);
			this.GroupBox.TabIndex = 5;
			this.GroupBox.TabStop = false;
			this.GroupBox.Text = "Selected font settings";
            // 
            // TxtGlyphRange
            // 
            this.TxtGlyphRange.Location = new System.Drawing.Point(264, 0);
            this.TxtGlyphRange.Name = "TxtGlyphRange";
            this.TxtGlyphRange.Size = new System.Drawing.Size(50, 20);
            this.TxtGlyphRange.TabIndex = 19;
            this.TxtGlyphRange.MaxLength = 5;
            this.TxtGlyphRange.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtGlyphRange_KeyPress);
            this.TxtGlyphRange.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtGlyphRange_KeyPress_Filter);
            this.TxtGlyphRange.Leave += new System.EventHandler(this.TxtGlyphRange_Leave);
            // 
            // ZoomDrawingArea
            // 
            this.ZoomDrawingArea.LargeChange = 1;
			this.ZoomDrawingArea.Location = new System.Drawing.Point(335, 138);
			this.ZoomDrawingArea.Maximum = 40;
			this.ZoomDrawingArea.Minimum = 2;
			this.ZoomDrawingArea.Name = "ZoomDrawingArea";
			this.ZoomDrawingArea.Size = new System.Drawing.Size(322, 45);
			this.ZoomDrawingArea.TabIndex = 9;
			this.ZoomDrawingArea.Value = 15;
			this.ZoomDrawingArea.ValueChanged += new System.EventHandler(this.ZoomDrawingArea_ValueChanged);
			// 
			// LblZoom
			// 
			this.LblZoom.AutoSize = true;
			this.LblZoom.Location = new System.Drawing.Point(457, 122);
			this.LblZoom.Name = "LblZoom";
			this.LblZoom.Size = new System.Drawing.Size(18, 13);
			this.LblZoom.TabIndex = 10;
			this.LblZoom.Text = "x1";
			// 
			// PanelSize
			// 
			this.PanelSize.Controls.Add(this.LblWidth);
			this.PanelSize.Controls.Add(this.numWidth);
			this.PanelSize.Controls.Add(this.LblHeight);
			this.PanelSize.Controls.Add(this.numHeight);
			this.PanelSize.Location = new System.Drawing.Point(335, 77);
			this.PanelSize.Name = "PanelSize";
			this.PanelSize.Size = new System.Drawing.Size(115, 45);
			this.PanelSize.TabIndex = 11;
			// 
			// LblWidth
			// 
			this.LblWidth.AutoSize = true;
			this.LblWidth.Location = new System.Drawing.Point(6, 0);
			this.LblWidth.Name = "LblWidth";
			this.LblWidth.Size = new System.Drawing.Size(38, 13);
			this.LblWidth.TabIndex = 1;
			this.LblWidth.Text = "Width:";
			// 
			// numWidth
			// 
			this.numWidth.Location = new System.Drawing.Point(6, 16);
			this.numWidth.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
			this.numWidth.Name = "numWidth";
			this.numWidth.Size = new System.Drawing.Size(43, 20);
			this.numWidth.TabIndex = 0;
			this.numWidth.ValueChanged += new System.EventHandler(this.numWidth_ValueChanged);
			// 
			// LblHeight
			// 
			this.LblHeight.AutoSize = true;
			this.LblHeight.Location = new System.Drawing.Point(62, 0);
			this.LblHeight.Name = "LblHeight";
			this.LblHeight.Size = new System.Drawing.Size(41, 13);
			this.LblHeight.TabIndex = 1;
			this.LblHeight.Text = "Height:";
			// 
			// numHeight
			// 
			this.numHeight.Location = new System.Drawing.Point(62, 16);
			this.numHeight.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
			this.numHeight.Name = "numHeight";
			this.numHeight.Size = new System.Drawing.Size(43, 20);
			this.numHeight.TabIndex = 0;
			this.numHeight.ValueChanged += new System.EventHandler(this.numHeight_ValueChanged);
			// 
			// BtnClear
			// 
			this.BtnClear.Location = new System.Drawing.Point(12, 80);
			this.BtnClear.Name = "BtnClear";
			this.BtnClear.Size = new System.Drawing.Size(75, 23);
			this.BtnClear.TabIndex = 12;
			this.BtnClear.Text = "Clear";
			this.BtnClear.UseVisualStyleBackColor = true;
			this.BtnClear.Click += new System.EventHandler(this.BtnClear_Click);
			// 
			// BtnFill
			// 
			this.BtnFill.Location = new System.Drawing.Point(12, 109);
			this.BtnFill.Name = "BtnFill";
			this.BtnFill.Size = new System.Drawing.Size(75, 23);
			this.BtnFill.TabIndex = 12;
			this.BtnFill.Text = "Fill";
			this.BtnFill.UseVisualStyleBackColor = true;
			this.BtnFill.Click += new System.EventHandler(this.BtnFill_Click);
			// 
			// BtnShiftUp
			// 
			this.BtnShiftUp.Location = new System.Drawing.Point(33, 19);
			this.BtnShiftUp.Name = "BtnShiftUp";
			this.BtnShiftUp.Size = new System.Drawing.Size(23, 23);
			this.BtnShiftUp.TabIndex = 13;
			this.BtnShiftUp.Text = "U";
			this.BtnShiftUp.UseVisualStyleBackColor = true;
			this.BtnShiftUp.Click += new System.EventHandler(this.BtnShiftUp_Click);
			// 
			// BtnShiftLeft
			// 
			this.BtnShiftLeft.Location = new System.Drawing.Point(8, 44);
			this.BtnShiftLeft.Name = "BtnShiftLeft";
			this.BtnShiftLeft.Size = new System.Drawing.Size(23, 23);
			this.BtnShiftLeft.TabIndex = 13;
			this.BtnShiftLeft.Text = "L";
			this.BtnShiftLeft.UseVisualStyleBackColor = true;
			this.BtnShiftLeft.Click += new System.EventHandler(this.BtnShiftLeft_Click);
			// 
			// BtnShiftRight
			// 
			this.BtnShiftRight.Location = new System.Drawing.Point(58, 44);
			this.BtnShiftRight.Name = "BtnShiftRight";
			this.BtnShiftRight.Size = new System.Drawing.Size(23, 23);
			this.BtnShiftRight.TabIndex = 13;
			this.BtnShiftRight.Text = "R";
			this.BtnShiftRight.UseVisualStyleBackColor = true;
			this.BtnShiftRight.Click += new System.EventHandler(this.BtnShiftRight_Click);
			// 
			// BtnShiftDown
			// 
			this.BtnShiftDown.Location = new System.Drawing.Point(33, 44);
			this.BtnShiftDown.Name = "BtnShiftDown";
			this.BtnShiftDown.Size = new System.Drawing.Size(23, 23);
			this.BtnShiftDown.TabIndex = 13;
			this.BtnShiftDown.Text = "D";
			this.BtnShiftDown.UseVisualStyleBackColor = true;
			this.BtnShiftDown.Click += new System.EventHandler(this.BtnShiftDown_Click);
			// 
			// ChkGrid
			// 
			this.ChkGrid.AutoSize = true;
			this.ChkGrid.Location = new System.Drawing.Point(444, 13);
			this.ChkGrid.Name = "ChkGrid";
			this.ChkGrid.Size = new System.Drawing.Size(75, 17);
			this.ChkGrid.TabIndex = 14;
			this.ChkGrid.Text = "Show Grid";
			this.ChkGrid.UseVisualStyleBackColor = true;
			this.ChkGrid.CheckedChanged += new System.EventHandler(this.ChkGrid_CheckedChanged);
			// 
			// BtnInvert
			// 
			this.BtnInvert.Location = new System.Drawing.Point(93, 19);
			this.BtnInvert.Name = "BtnInvert";
			this.BtnInvert.Size = new System.Drawing.Size(75, 23);
			this.BtnInvert.TabIndex = 15;
			this.BtnInvert.Text = "Invert Image";
			this.BtnInvert.UseVisualStyleBackColor = true;
			this.BtnInvert.Click += new System.EventHandler(this.BtnInvert_Click);
			// 
			// PanelMouseColor
			// 
			this.PanelMouseColor.Controls.Add(this.LblRight);
			this.PanelMouseColor.Controls.Add(this.LblLeft);
			this.PanelMouseColor.Controls.Add(this.PictSwap);
			this.PanelMouseColor.Controls.Add(this.PanelRightMouse);
			this.PanelMouseColor.Controls.Add(this.PanelLeftMouse);
			this.PanelMouseColor.Location = new System.Drawing.Point(335, 9);
			this.PanelMouseColor.Name = "PanelMouseColor";
			this.PanelMouseColor.Size = new System.Drawing.Size(103, 65);
			this.PanelMouseColor.TabIndex = 16;
			// 
			// LblRight
			// 
			this.LblRight.AutoSize = true;
			this.LblRight.Location = new System.Drawing.Point(71, 46);
			this.LblRight.Name = "LblRight";
			this.LblRight.Size = new System.Drawing.Size(27, 13);
			this.LblRight.TabIndex = 17;
			this.LblRight.Text = "right";
			// 
			// LblLeft
			// 
			this.LblLeft.AutoSize = true;
			this.LblLeft.Location = new System.Drawing.Point(44, 3);
			this.LblLeft.Name = "LblLeft";
			this.LblLeft.Size = new System.Drawing.Size(21, 13);
			this.LblLeft.TabIndex = 17;
			this.LblLeft.Text = "left";
			// 
			// PictSwap
			// 
			this.PictSwap.Image = global::AGS.Plugin.FontEditor.Properties.Resources.Swap;
			this.PictSwap.Location = new System.Drawing.Point(3, 34);
			this.PictSwap.Name = "PictSwap";
			this.PictSwap.Size = new System.Drawing.Size(16, 14);
			this.PictSwap.TabIndex = 17;
			this.PictSwap.TabStop = false;
			this.PictSwap.Click += new System.EventHandler(this.PictSwap_Click);
			// 
			// PanelRightMouse
			// 
			this.PanelRightMouse.BackColor = System.Drawing.Color.Black;
			this.PanelRightMouse.Location = new System.Drawing.Point(25, 34);
			this.PanelRightMouse.Name = "PanelRightMouse";
			this.PanelRightMouse.Size = new System.Drawing.Size(40, 25);
			this.PanelRightMouse.TabIndex = 18;
			// 
			// PanelLeftMouse
			// 
			this.PanelLeftMouse.BackColor = System.Drawing.Color.White;
			this.PanelLeftMouse.Location = new System.Drawing.Point(3, 3);
			this.PanelLeftMouse.Name = "PanelLeftMouse";
			this.PanelLeftMouse.Size = new System.Drawing.Size(40, 25);
			this.PanelLeftMouse.TabIndex = 17;
			// 
			// DrawingArea
			// 
			this.DrawingArea.Location = new System.Drawing.Point(335, 183);
			this.DrawingArea.Name = "DrawingArea";
			this.DrawingArea.Size = new System.Drawing.Size(322, 290);
			this.DrawingArea.TabIndex = 7;
			this.DrawingArea.TabStop = false;
			this.DrawingArea.Click += new System.EventHandler(this.DrawingArea_Click);
			this.DrawingArea.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawingArea_Paint);
			this.DrawingArea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DrawingArea_MouseDown);
			this.DrawingArea.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DrawingArea_MouseMove);
			this.DrawingArea.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DrawingArea_MouseUp);
			// 
			// BtnSwapHorizontally
			// 
			this.BtnSwapHorizontally.Location = new System.Drawing.Point(93, 80);
			this.BtnSwapHorizontally.Name = "BtnSwapHorizontally";
			this.BtnSwapHorizontally.Size = new System.Drawing.Size(75, 23);
			this.BtnSwapHorizontally.TabIndex = 17;
			this.BtnSwapHorizontally.Text = "Swap H";
			this.BtnSwapHorizontally.UseVisualStyleBackColor = true;
			this.BtnSwapHorizontally.Click += new System.EventHandler(this.BtnSwapHorizontally_Click);
			// 
			// BtnSwapVertically
			// 
			this.BtnSwapVertically.Location = new System.Drawing.Point(93, 109);
			this.BtnSwapVertically.Name = "BtnSwapVertically";
			this.BtnSwapVertically.Size = new System.Drawing.Size(75, 23);
			this.BtnSwapVertically.TabIndex = 17;
			this.BtnSwapVertically.Text = "Swap V";
			this.BtnSwapVertically.UseVisualStyleBackColor = true;
			this.BtnSwapVertically.Click += new System.EventHandler(this.BtnSwapVertically_Click);
			// 
			// BtnOutline
			// 
			this.BtnOutline.Location = new System.Drawing.Point(93, 44);
			this.BtnOutline.Name = "BtnOutline";
			this.BtnOutline.Size = new System.Drawing.Size(75, 23);
			this.BtnOutline.TabIndex = 18;
			this.BtnOutline.Text = "Outline";
			this.BtnOutline.UseVisualStyleBackColor = true;
			this.BtnOutline.Click += new System.EventHandler(this.BtnOutline_Click);
			// 
			// BtnOutlineFont
			// 
			this.BtnOutlineFont.Location = new System.Drawing.Point(6, 19);
			this.BtnOutlineFont.Name = "BtnOutlineFont";
			this.BtnOutlineFont.Size = new System.Drawing.Size(75, 23);
			this.BtnOutlineFont.TabIndex = 18;
			this.BtnOutlineFont.Text = "Outline Font";
			this.BtnOutlineFont.UseVisualStyleBackColor = true;
			this.BtnOutlineFont.Click += new System.EventHandler(this.BtnOutlineFont_Click);
			// 
			// BtnRenderText
			// 
			this.BtnRenderText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.BtnRenderText.Location = new System.Drawing.Point(245, 479);
			this.BtnRenderText.Name = "BtnRenderText";
			this.BtnRenderText.Size = new System.Drawing.Size(75, 23);
			this.BtnRenderText.TabIndex = 20;
			this.BtnRenderText.Text = "Render Text";
			this.BtnRenderText.UseVisualStyleBackColor = true;
			this.BtnRenderText.Click += new System.EventHandler(this.BtnRenderText_Click);
            // 
            // BtnPagePrev
            // 
            this.BtnPagePrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnPagePrev.Location = new System.Drawing.Point(73, 479);
            this.BtnPagePrev.Name = "BtnPagePrev";
            this.BtnPagePrev.Size = new System.Drawing.Size(55, 23);
            this.BtnPagePrev.TabIndex = 21;
            this.BtnPagePrev.Text = "<< Page";
            this.BtnPagePrev.UseVisualStyleBackColor = true;
            this.BtnPagePrev.Click += new System.EventHandler(this.BtnPagePrev_Click);
            // 
            // BtnPageNext
            // 
            this.BtnPageNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnPageNext.Location = new System.Drawing.Point(190, 479);
            this.BtnPageNext.Name = "BtnPageNext";
            this.BtnPageNext.Size = new System.Drawing.Size(55, 23);
            this.BtnPageNext.TabIndex = 22;
            this.BtnPageNext.Text = "Page >>";
            this.BtnPageNext.UseVisualStyleBackColor = true;
            this.BtnPageNext.Click += new System.EventHandler(this.BtnPageNext_Click);
            // 
            // TxtPageNumber
            // 
            this.TxtPageNumber.Anchor = ((System.Windows.Forms.AnchorStyles)
                ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TxtPageNumber.Location = new System.Drawing.Point(130, 480);
            this.TxtPageNumber.Name = "TxtPageNumber";
            this.TxtPageNumber.Size = new System.Drawing.Size(30, 20);
            this.TxtPageNumber.TabIndex = 23;
            this.TxtPageNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TxtPageNumber.Text = "1";
            this.TxtPageNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtPageNumber_KeyDown);
            // 
            // LblPageTotal
            // 
            this.LblPageTotal.Anchor = ((System.Windows.Forms.AnchorStyles)
                ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LblPageTotal.Location = new System.Drawing.Point(160, 483);
            this.LblPageTotal.Name = "LblPageTotal";
            this.LblPageTotal.Size = new System.Drawing.Size(40, 15);
            this.LblPageTotal.TabIndex = 24;
            this.LblPageTotal.Text = "/ 1";
            // 
            // PictRenderText
            // 
            this.PictRenderText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.PictRenderText.Location = new System.Drawing.Point(330, 479);
			this.PictRenderText.Name = "PictRenderText";
			this.PictRenderText.Size = new System.Drawing.Size(833, 28);
			this.PictRenderText.TabIndex = 21;
			this.PictRenderText.TabStop = false;
			// 
			// BtnSetText
			// 
			this.BtnSetText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.BtnSetText.Location = new System.Drawing.Point(12, 479);
			this.BtnSetText.Name = "BtnSetText";
			this.BtnSetText.Size = new System.Drawing.Size(61, 23);
			this.BtnSetText.TabIndex = 20;
			this.BtnSetText.Text = "Set Text";
			this.BtnSetText.UseVisualStyleBackColor = true;
			this.BtnSetText.Click += new System.EventHandler(this.BtnSetText_Click);
			// 
			// LblCharacter
			// 
			this.LblCharacter.AutoSize = true;
			this.LblCharacter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LblCharacter.Location = new System.Drawing.Point(457, 100);
			this.LblCharacter.Name = "LblCharacter";
			this.LblCharacter.Size = new System.Drawing.Size(60, 16);
			this.LblCharacter.TabIndex = 22;
			this.LblCharacter.Text = "<ccode>";
			// 
			// BtnPrevious
			// 
			this.BtnPrevious.Location = new System.Drawing.Point(526, 22);
			this.BtnPrevious.Name = "BtnPrevious";
			this.BtnPrevious.Size = new System.Drawing.Size(75, 23);
			this.BtnPrevious.TabIndex = 23;
			this.BtnPrevious.Text = "Previous";
			this.BtnPrevious.UseVisualStyleBackColor = true;
			this.BtnPrevious.Click += new System.EventHandler(this.BtnPrevious_Click);
			// 
			// BtnNext
			// 
			this.BtnNext.Location = new System.Drawing.Point(526, 47);
			this.BtnNext.Name = "BtnNext";
			this.BtnNext.Size = new System.Drawing.Size(75, 23);
			this.BtnNext.TabIndex = 23;
			this.BtnNext.Text = "Next";
			this.BtnNext.UseVisualStyleBackColor = true;
			this.BtnNext.Click += new System.EventHandler(this.BtnNext_Click);
			// 
			// ChkGridFix
			// 
			this.ChkGridFix.AutoSize = true;
			this.ChkGridFix.Location = new System.Drawing.Point(444, 34);
			this.ChkGridFix.Name = "ChkGridFix";
			this.ChkGridFix.Size = new System.Drawing.Size(58, 17);
			this.ChkGridFix.TabIndex = 14;
			this.ChkGridFix.Text = "Grid fix";
			this.ChkGridFix.UseVisualStyleBackColor = true;
			this.ChkGridFix.CheckedChanged += new System.EventHandler(this.ChkGridFix_CheckedChanged);
			// 
			// BtnAllHeight
			// 
			this.BtnAllHeight.Location = new System.Drawing.Point(87, 19);
			this.BtnAllHeight.Name = "BtnAllHeight";
			this.BtnAllHeight.Size = new System.Drawing.Size(75, 37);
			this.BtnAllHeight.TabIndex = 23;
			this.BtnAllHeight.Text = "All height to current";
			this.BtnAllHeight.UseVisualStyleBackColor = true;
			this.BtnAllHeight.Click += new System.EventHandler(this.BtnAllHeight_Click);
			// 
			// BtnAllWidth
			// 
			this.BtnAllWidth.Location = new System.Drawing.Point(87, 62);
			this.BtnAllWidth.Name = "BtnAllWidth";
			this.BtnAllWidth.Size = new System.Drawing.Size(75, 37);
			this.BtnAllWidth.TabIndex = 23;
			this.BtnAllWidth.Text = "All width to current";
			this.BtnAllWidth.UseVisualStyleBackColor = true;
			this.BtnAllWidth.Click += new System.EventHandler(this.BtnAllWidth_Click);
            // 
            // BtnAllBlankClear
            // 
            this.BtnAllBlankClear.Location = new System.Drawing.Point(6, 105);
            this.BtnAllBlankClear.Name = "BtnAllBlankClear";
            this.BtnAllBlankClear.Size = new System.Drawing.Size(156, 23);
            this.BtnAllBlankClear.TabIndex = 23;
            this.BtnAllBlankClear.Text = "Make all blanks placeholdes";
            this.BtnAllBlankClear.UseVisualStyleBackColor = true;
            this.BtnAllBlankClear.Click += new System.EventHandler(this.BtnAllBlankClear_Click);
            // 
            // TxtGlyph
            // 
            this.TxtGlyph.Location = new System.Drawing.Point(460, 54);
            this.TxtGlyph.Name = "TxtGlyph";
            this.TxtGlyph.Size = new System.Drawing.Size(53, 20);
            this.TxtGlyph.TabIndex = 24;
            this.TxtGlyph.MaxLength = 1;
            this.TxtGlyph.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtGlyph_KeyPress);
            // 
            // TxtCharacter
            // 
            this.TxtCharacter.Location = new System.Drawing.Point(460, 77);
			this.TxtCharacter.Name = "TxtCharacter";
			this.TxtCharacter.Size = new System.Drawing.Size(53, 20);
			this.TxtCharacter.TabIndex = 24;
			this.TxtCharacter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtCharacter_KeyPress);
            this.TxtCharacter.Leave += new System.EventHandler(this.TxtCharacter_Leave);
            this.TxtGlyph.Leave += new System.EventHandler(this.TxtGlyph_Leave);
            // 
            // GrpAllCharacters
            // 
            this.GrpAllCharacters.Controls.Add(this.BtnOutlineFont);
			this.GrpAllCharacters.Controls.Add(this.BtnAllHeight);
			this.GrpAllCharacters.Controls.Add(this.BtnAllWidth);
            this.GrpAllCharacters.Controls.Add(this.BtnAllBlankClear);
            this.GrpAllCharacters.Location = new System.Drawing.Point(838, 3);
			this.GrpAllCharacters.Name = "GrpAllCharacters";
			this.GrpAllCharacters.Size = new System.Drawing.Size(169, 137);
			this.GrpAllCharacters.TabIndex = 25;
			this.GrpAllCharacters.TabStop = false;
			this.GrpAllCharacters.Text = "All characters at current page";
			// 
			// GrpOneCharacter
			// 
			this.GrpOneCharacter.Controls.Add(this.ChkOneAllCharacters);
			this.GrpOneCharacter.Controls.Add(this.BtnInvert);
			this.GrpOneCharacter.Controls.Add(this.BtnClear);
			this.GrpOneCharacter.Controls.Add(this.BtnFill);
			this.GrpOneCharacter.Controls.Add(this.BtnSwapHorizontally);
			this.GrpOneCharacter.Controls.Add(this.BtnSwapVertically);
			this.GrpOneCharacter.Controls.Add(this.BtnOutline);
			this.GrpOneCharacter.Controls.Add(this.BtnShiftRight);
			this.GrpOneCharacter.Controls.Add(this.BtnShiftUp);
			this.GrpOneCharacter.Controls.Add(this.BtnShiftDown);
			this.GrpOneCharacter.Controls.Add(this.BtnShiftLeft);
			this.GrpOneCharacter.Location = new System.Drawing.Point(607, 3);
			this.GrpOneCharacter.Name = "GrpOneCharacter";
			this.GrpOneCharacter.Size = new System.Drawing.Size(225, 137);
			this.GrpOneCharacter.TabIndex = 26;
			this.GrpOneCharacter.TabStop = false;
			this.GrpOneCharacter.Text = "One Character";
			// 
			// ChkOneAllCharacters
			// 
			this.ChkOneAllCharacters.AutoSize = true;
			this.ChkOneAllCharacters.Location = new System.Drawing.Point(93, -1);
			this.ChkOneAllCharacters.Name = "ChkOneAllCharacters";
			this.ChkOneAllCharacters.Size = new System.Drawing.Size(126, 17);
			this.ChkOneAllCharacters.TabIndex = 0;
			this.ChkOneAllCharacters.Text = "Use on all characters";
			this.ChkOneAllCharacters.UseVisualStyleBackColor = true;
			this.ChkOneAllCharacters.CheckedChanged += new System.EventHandler(this.ChkOneAllCharacters_CheckedChanged);
			// 
			// FontEditorPane
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.GrpOneCharacter);
			this.Controls.Add(this.GrpAllCharacters);
			this.Controls.Add(this.TxtCharacter);
            this.Controls.Add(this.TxtGlyph);
            this.Controls.Add(this.BtnNext);
			this.Controls.Add(this.BtnPrevious);
            this.Controls.Add(this.LblCharacter);
			this.Controls.Add(this.PictRenderText);
            this.Controls.Add(this.BtnPagePrev);
            this.Controls.Add(this.BtnPageNext);
            this.Controls.Add(this.TxtPageNumber);
            this.Controls.Add(this.LblPageTotal);
            this.Controls.Add(this.BtnSetText);
			this.Controls.Add(this.BtnRenderText);
			this.Controls.Add(this.PanelMouseColor);
			this.Controls.Add(this.ChkGridFix);
			this.Controls.Add(this.ChkGrid);
			this.Controls.Add(this.PanelSize);
			this.Controls.Add(this.LblZoom);
			this.Controls.Add(this.ZoomDrawingArea);
			this.Controls.Add(this.DrawingArea);
			this.Controls.Add(this.GroupBox);
			this.Name = "FontEditorPane";
			this.Size = new System.Drawing.Size(1013, 510);
			this.GroupBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ZoomDrawingArea)).EndInit();
			this.PanelSize.ResumeLayout(false);
			this.PanelSize.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
			this.PanelMouseColor.ResumeLayout(false);
			this.PanelMouseColor.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.PictSwap)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.DrawingArea)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PictRenderText)).EndInit();
			this.GrpAllCharacters.ResumeLayout(false);
			this.GrpOneCharacter.ResumeLayout(false);
			this.GrpOneCharacter.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel FlowCharacterPanel;
		private System.Windows.Forms.GroupBox GroupBox;
		private System.Windows.Forms.PictureBox DrawingArea;
		private System.Windows.Forms.TrackBar ZoomDrawingArea;
		private System.Windows.Forms.Label LblZoom;
		private System.Windows.Forms.Panel PanelSize;
		private System.Windows.Forms.Label LblWidth;
		private System.Windows.Forms.NumericUpDown numWidth;
		private System.Windows.Forms.Label LblHeight;
		private System.Windows.Forms.NumericUpDown numHeight;
		private System.Windows.Forms.Button BtnClear;
		private System.Windows.Forms.Button BtnFill;
		private System.Windows.Forms.Button BtnShiftUp;
		private System.Windows.Forms.Button BtnShiftLeft;
		private System.Windows.Forms.Button BtnShiftRight;
		private System.Windows.Forms.Button BtnShiftDown;
		private System.Windows.Forms.CheckBox ChkGrid;
		private System.Windows.Forms.Button BtnInvert;
		private System.Windows.Forms.Panel PanelMouseColor;
		private System.Windows.Forms.Panel PanelRightMouse;
		private System.Windows.Forms.Panel PanelLeftMouse;
		private System.Windows.Forms.PictureBox PictSwap;
		private System.Windows.Forms.Label LblRight;
		private System.Windows.Forms.Label LblLeft;
		private System.Windows.Forms.Button BtnSwapHorizontally;
		private System.Windows.Forms.Button BtnSwapVertically;
		private System.Windows.Forms.Button BtnOutline;
		private System.Windows.Forms.Button BtnOutlineFont;
        private System.Windows.Forms.TextBox TxtGlyphRange;
        private System.Windows.Forms.Button BtnPagePrev;
		private System.Windows.Forms.Button BtnPageNext;
		private System.Windows.Forms.Button BtnRenderText;
		private System.Windows.Forms.PictureBox PictRenderText;
		private System.Windows.Forms.Button BtnSetText;
		private System.Windows.Forms.Label LblCharacter;
		private System.Windows.Forms.Button BtnPrevious;
		private System.Windows.Forms.Button BtnNext;
        private System.Windows.Forms.TextBox TxtPageNumber;
        private System.Windows.Forms.Label LblPageTotal;
        private System.Windows.Forms.CheckBox ChkGridFix;
		private System.Windows.Forms.Button BtnAllHeight;
		private System.Windows.Forms.Button BtnAllWidth;
		private System.Windows.Forms.Button BtnAllBlankClear;
        private System.Windows.Forms.TextBox TxtCharacter;
        private System.Windows.Forms.TextBox TxtGlyph;
        private System.Windows.Forms.GroupBox GrpAllCharacters;
		private System.Windows.Forms.GroupBox GrpOneCharacter;
		private System.Windows.Forms.CheckBox ChkOneAllCharacters;
	}
}
