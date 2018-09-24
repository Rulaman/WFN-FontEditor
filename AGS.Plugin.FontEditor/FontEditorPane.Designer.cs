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
			this.LblInfo = new System.Windows.Forms.Label();
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
			this.BtnSetAllHeight = new System.Windows.Forms.Button();
			this.ChkGrid = new System.Windows.Forms.CheckBox();
			this.BtnInvert = new System.Windows.Forms.Button();
			this.PanelMouseColor = new System.Windows.Forms.Panel();
			this.PanelLeftMouse = new System.Windows.Forms.Panel();
			this.PanelRightMouse = new System.Windows.Forms.Panel();
			this.PictSwap = new System.Windows.Forms.PictureBox();
			this.DrawingArea = new System.Windows.Forms.PictureBox();
			this.LblLeft = new System.Windows.Forms.Label();
			this.LblRight = new System.Windows.Forms.Label();
			this.BtnSwapHorizontally = new System.Windows.Forms.Button();
			this.BtnSwapVertically = new System.Windows.Forms.Button();
			this.GroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ZoomDrawingArea)).BeginInit();
			this.PanelSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
			this.PanelMouseColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PictSwap)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.DrawingArea)).BeginInit();
			this.SuspendLayout();
			// 
			// FlowCharacterPanel
			// 
			this.FlowCharacterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.FlowCharacterPanel.AutoScroll = true;
			this.FlowCharacterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
			this.FlowCharacterPanel.Location = new System.Drawing.Point(6, 19);
			this.FlowCharacterPanel.Name = "FlowCharacterPanel";
			this.FlowCharacterPanel.Size = new System.Drawing.Size(307, 443);
			this.FlowCharacterPanel.TabIndex = 4;
			// 
			// GroupBox
			// 
			this.GroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.GroupBox.Controls.Add(this.FlowCharacterPanel);
			this.GroupBox.Location = new System.Drawing.Point(6, 6);
			this.GroupBox.Name = "GroupBox";
			this.GroupBox.Size = new System.Drawing.Size(320, 468);
			this.GroupBox.TabIndex = 5;
			this.GroupBox.TabStop = false;
			this.GroupBox.Text = "Selected font settings";
			// 
			// LblInfo
			// 
			this.LblInfo.AutoSize = true;
			this.LblInfo.Location = new System.Drawing.Point(663, 395);
			this.LblInfo.Name = "LblInfo";
			this.LblInfo.Size = new System.Drawing.Size(295, 52);
			this.LblInfo.TabIndex = 6;
			this.LblInfo.Text = "left click = white pixel\r\nright click = black pixel\r\n\r\nmove with pressed mouse; c" +
				"hange the pixel under the mouse";
			this.LblInfo.Visible = false;
			// 
			// ZoomDrawingArea
			// 
			this.ZoomDrawingArea.LargeChange = 1;
			this.ZoomDrawingArea.Location = new System.Drawing.Point(335, 135);
			this.ZoomDrawingArea.Maximum = 40;
			this.ZoomDrawingArea.Minimum = 2;
			this.ZoomDrawingArea.Name = "ZoomDrawingArea";
			this.ZoomDrawingArea.Size = new System.Drawing.Size(322, 42);
			this.ZoomDrawingArea.TabIndex = 9;
			this.ZoomDrawingArea.Value = 15;
			this.ZoomDrawingArea.ValueChanged += new System.EventHandler(this.ZoomDrawingArea_ValueChanged);
			// 
			// LblZoom
			// 
			this.LblZoom.AutoSize = true;
			this.LblZoom.Location = new System.Drawing.Point(657, 152);
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
			this.BtnClear.Location = new System.Drawing.Point(545, 70);
			this.BtnClear.Name = "BtnClear";
			this.BtnClear.Size = new System.Drawing.Size(75, 23);
			this.BtnClear.TabIndex = 12;
			this.BtnClear.Text = "Clear";
			this.BtnClear.UseVisualStyleBackColor = true;
			this.BtnClear.Click += new System.EventHandler(this.BtnClear_Click);
			// 
			// BtnFill
			// 
			this.BtnFill.Location = new System.Drawing.Point(545, 99);
			this.BtnFill.Name = "BtnFill";
			this.BtnFill.Size = new System.Drawing.Size(75, 23);
			this.BtnFill.TabIndex = 12;
			this.BtnFill.Text = "Fill";
			this.BtnFill.UseVisualStyleBackColor = true;
			this.BtnFill.Click += new System.EventHandler(this.BtnFill_Click);
			// 
			// BtnShiftUp
			// 
			this.BtnShiftUp.Location = new System.Drawing.Point(487, 70);
			this.BtnShiftUp.Name = "BtnShiftUp";
			this.BtnShiftUp.Size = new System.Drawing.Size(23, 23);
			this.BtnShiftUp.TabIndex = 13;
			this.BtnShiftUp.Text = "U";
			this.BtnShiftUp.UseVisualStyleBackColor = true;
			this.BtnShiftUp.Click += new System.EventHandler(this.BtnShiftUp_Click);
			// 
			// BtnShiftLeft
			// 
			this.BtnShiftLeft.Location = new System.Drawing.Point(458, 99);
			this.BtnShiftLeft.Name = "BtnShiftLeft";
			this.BtnShiftLeft.Size = new System.Drawing.Size(23, 23);
			this.BtnShiftLeft.TabIndex = 13;
			this.BtnShiftLeft.Text = "L";
			this.BtnShiftLeft.UseVisualStyleBackColor = true;
			this.BtnShiftLeft.Click += new System.EventHandler(this.BtnShiftLeft_Click);
			// 
			// BtnShiftRight
			// 
			this.BtnShiftRight.Location = new System.Drawing.Point(516, 99);
			this.BtnShiftRight.Name = "BtnShiftRight";
			this.BtnShiftRight.Size = new System.Drawing.Size(23, 23);
			this.BtnShiftRight.TabIndex = 13;
			this.BtnShiftRight.Text = "R";
			this.BtnShiftRight.UseVisualStyleBackColor = true;
			this.BtnShiftRight.Click += new System.EventHandler(this.BtnShiftRight_Click);
			// 
			// BtnShiftDown
			// 
			this.BtnShiftDown.Location = new System.Drawing.Point(487, 99);
			this.BtnShiftDown.Name = "BtnShiftDown";
			this.BtnShiftDown.Size = new System.Drawing.Size(23, 23);
			this.BtnShiftDown.TabIndex = 13;
			this.BtnShiftDown.Text = "D";
			this.BtnShiftDown.UseVisualStyleBackColor = true;
			this.BtnShiftDown.Click += new System.EventHandler(this.BtnShiftDown_Click);
			// 
			// BtnSetAllHeight
			// 
			this.BtnSetAllHeight.Location = new System.Drawing.Point(663, 450);
			this.BtnSetAllHeight.Name = "BtnSetAllHeight";
			this.BtnSetAllHeight.Size = new System.Drawing.Size(95, 23);
			this.BtnSetAllHeight.TabIndex = 12;
			this.BtnSetAllHeight.Text = "Set all height";
			this.BtnSetAllHeight.UseVisualStyleBackColor = true;
			this.BtnSetAllHeight.Visible = false;
			this.BtnSetAllHeight.Click += new System.EventHandler(this.BtnSetAllHeight_Click);
			// 
			// ChkGrid
			// 
			this.ChkGrid.AutoSize = true;
			this.ChkGrid.Location = new System.Drawing.Point(455, 13);
			this.ChkGrid.Name = "ChkGrid";
			this.ChkGrid.Size = new System.Drawing.Size(75, 17);
			this.ChkGrid.TabIndex = 14;
			this.ChkGrid.Text = "Show Grid";
			this.ChkGrid.UseVisualStyleBackColor = true;
			this.ChkGrid.CheckedChanged += new System.EventHandler(this.ChkGrid_CheckedChanged);
			// 
			// BtnInvert
			// 
			this.BtnInvert.Location = new System.Drawing.Point(545, 9);
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
			// PanelLeftMouse
			// 
			this.PanelLeftMouse.BackColor = System.Drawing.Color.White;
			this.PanelLeftMouse.Location = new System.Drawing.Point(3, 3);
			this.PanelLeftMouse.Name = "PanelLeftMouse";
			this.PanelLeftMouse.Size = new System.Drawing.Size(40, 25);
			this.PanelLeftMouse.TabIndex = 17;
			// 
			// PanelRightMouse
			// 
			this.PanelRightMouse.BackColor = System.Drawing.Color.Black;
			this.PanelRightMouse.Location = new System.Drawing.Point(25, 34);
			this.PanelRightMouse.Name = "PanelRightMouse";
			this.PanelRightMouse.Size = new System.Drawing.Size(40, 25);
			this.PanelRightMouse.TabIndex = 18;
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
			// DrawingArea
			// 
			this.DrawingArea.Location = new System.Drawing.Point(335, 183);
			this.DrawingArea.Name = "DrawingArea";
			this.DrawingArea.Size = new System.Drawing.Size(322, 290);
			this.DrawingArea.TabIndex = 7;
			this.DrawingArea.TabStop = false;
			this.DrawingArea.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DrawingArea_MouseMove);
			this.DrawingArea.Click += new System.EventHandler(this.DrawingArea_Click);
			this.DrawingArea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DrawingArea_MouseDown);
			this.DrawingArea.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawingArea_Paint);
			this.DrawingArea.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DrawingArea_MouseUp);
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
			// LblRight
			// 
			this.LblRight.AutoSize = true;
			this.LblRight.Location = new System.Drawing.Point(71, 46);
			this.LblRight.Name = "LblRight";
			this.LblRight.Size = new System.Drawing.Size(27, 13);
			this.LblRight.TabIndex = 17;
			this.LblRight.Text = "right";
			// 
			// BtnSwapHorizontally
			// 
			this.BtnSwapHorizontally.Location = new System.Drawing.Point(626, 70);
			this.BtnSwapHorizontally.Name = "BtnSwapHorizontally";
			this.BtnSwapHorizontally.Size = new System.Drawing.Size(75, 23);
			this.BtnSwapHorizontally.TabIndex = 17;
			this.BtnSwapHorizontally.Text = "Swap H";
			this.BtnSwapHorizontally.UseVisualStyleBackColor = true;
			this.BtnSwapHorizontally.Click += new System.EventHandler(this.BtnSwapHorizontally_Click);
			// 
			// BtnSwapVertically
			// 
			this.BtnSwapVertically.Location = new System.Drawing.Point(626, 99);
			this.BtnSwapVertically.Name = "BtnSwapVertically";
			this.BtnSwapVertically.Size = new System.Drawing.Size(75, 23);
			this.BtnSwapVertically.TabIndex = 17;
			this.BtnSwapVertically.Text = "Swap V";
			this.BtnSwapVertically.UseVisualStyleBackColor = true;
			this.BtnSwapVertically.Click += new System.EventHandler(this.BtnSwapVertically_Click);
			// 
			// FontEditorPane
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.BtnSwapVertically);
			this.Controls.Add(this.BtnSwapHorizontally);
			this.Controls.Add(this.PanelMouseColor);
			this.Controls.Add(this.BtnInvert);
			this.Controls.Add(this.ChkGrid);
			this.Controls.Add(this.BtnShiftRight);
			this.Controls.Add(this.BtnShiftDown);
			this.Controls.Add(this.BtnShiftLeft);
			this.Controls.Add(this.BtnShiftUp);
			this.Controls.Add(this.BtnFill);
			this.Controls.Add(this.BtnSetAllHeight);
			this.Controls.Add(this.BtnClear);
			this.Controls.Add(this.PanelSize);
			this.Controls.Add(this.LblZoom);
			this.Controls.Add(this.ZoomDrawingArea);
			this.Controls.Add(this.DrawingArea);
			this.Controls.Add(this.LblInfo);
			this.Controls.Add(this.GroupBox);
			this.Name = "FontEditorPane";
			this.Size = new System.Drawing.Size(721, 488);
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
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel FlowCharacterPanel;
		private System.Windows.Forms.GroupBox GroupBox;
		private System.Windows.Forms.Label LblInfo;
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
		private System.Windows.Forms.Button BtnSetAllHeight;
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
	}
}
