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
			this.DrawingArea = new System.Windows.Forms.PictureBox();
			this.ZoomDrawingArea = new System.Windows.Forms.TrackBar();
			this.LblZoom = new System.Windows.Forms.Label();
			this.PanelSize = new System.Windows.Forms.Panel();
			this.LblWidth = new System.Windows.Forms.Label();
			this.numWidth = new System.Windows.Forms.NumericUpDown();
			this.LblHeight = new System.Windows.Forms.Label();
			this.numHeight = new System.Windows.Forms.NumericUpDown();
			this.GroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.DrawingArea)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ZoomDrawingArea)).BeginInit();
			this.PanelSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
			this.SuspendLayout();
			// 
			// FlowCharacterPanel
			// 
			this.FlowCharacterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.FlowCharacterPanel.BackColor = System.Drawing.Color.Black;
			this.FlowCharacterPanel.Location = new System.Drawing.Point(6, 19);
			this.FlowCharacterPanel.Name = "FlowCharacterPanel";
			this.FlowCharacterPanel.Size = new System.Drawing.Size(307, 442);
			this.FlowCharacterPanel.TabIndex = 4;
			// 
			// GroupBox
			// 
			this.GroupBox.Controls.Add(this.FlowCharacterPanel);
			this.GroupBox.Location = new System.Drawing.Point(6, 6);
			this.GroupBox.Name = "GroupBox";
			this.GroupBox.Size = new System.Drawing.Size(320, 467);
			this.GroupBox.TabIndex = 5;
			this.GroupBox.TabStop = false;
			this.GroupBox.Text = "Selected font settings";
			// 
			// LblInfo
			// 
			this.LblInfo.AutoSize = true;
			this.LblInfo.Location = new System.Drawing.Point(332, 15);
			this.LblInfo.Name = "LblInfo";
			this.LblInfo.Size = new System.Drawing.Size(295, 52);
			this.LblInfo.TabIndex = 6;
			this.LblInfo.Text = "left click = white pixel\r\nright click = black pixel\r\n(middle click = invert pixel" +
				")\r\nmove with pressed mouse; change the pixel under the mouse";
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
			this.DrawingArea.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DrawingArea_MouseUp);
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
			this.ZoomDrawingArea.Value = 2;
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
			// FontEditorPane
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.PanelSize);
			this.Controls.Add(this.LblZoom);
			this.Controls.Add(this.ZoomDrawingArea);
			this.Controls.Add(this.DrawingArea);
			this.Controls.Add(this.LblInfo);
			this.Controls.Add(this.GroupBox);
			this.Name = "FontEditorPane";
			this.Size = new System.Drawing.Size(975, 586);
			this.GroupBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.DrawingArea)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ZoomDrawingArea)).EndInit();
			this.PanelSize.ResumeLayout(false);
			this.PanelSize.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
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
	}
}
