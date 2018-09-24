namespace WFN_FontEditor
{
	partial class MainWindow
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

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.BtnOpen = new System.Windows.Forms.Button();
			this.TabControl = new System.Windows.Forms.TabControl();
			this.BtnSave = new System.Windows.Forms.Button();
			this.FontListBox = new System.Windows.Forms.ListBox();
			this.BtnConvert = new System.Windows.Forms.Button();
			this.TxtTextHeight = new System.Windows.Forms.TextBox();
			this.LblTextHeight = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// BtnOpen
			// 
			this.BtnOpen.Location = new System.Drawing.Point(12, 12);
			this.BtnOpen.Name = "BtnOpen";
			this.BtnOpen.Size = new System.Drawing.Size(113, 23);
			this.BtnOpen.TabIndex = 1;
			this.BtnOpen.Text = "Open Directory";
			this.BtnOpen.UseVisualStyleBackColor = true;
			this.BtnOpen.Click += new System.EventHandler(this.BtnOpen_Click);
			// 
			// TabControl
			// 
			this.TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TabControl.Location = new System.Drawing.Point(160, 41);
			this.TabControl.Name = "TabControl";
			this.TabControl.SelectedIndex = 0;
			this.TabControl.Size = new System.Drawing.Size(809, 527);
			this.TabControl.TabIndex = 2;
			// 
			// BtnSave
			// 
			this.BtnSave.Location = new System.Drawing.Point(131, 12);
			this.BtnSave.Name = "BtnSave";
			this.BtnSave.Size = new System.Drawing.Size(113, 23);
			this.BtnSave.TabIndex = 1;
			this.BtnSave.Text = "Save";
			this.BtnSave.UseVisualStyleBackColor = true;
			this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
			// 
			// FontListBox
			// 
			this.FontListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.FontListBox.FormattingEnabled = true;
			this.FontListBox.Location = new System.Drawing.Point(12, 48);
			this.FontListBox.Name = "FontListBox";
			this.FontListBox.Size = new System.Drawing.Size(142, 511);
			this.FontListBox.TabIndex = 3;
			this.FontListBox.SelectedIndexChanged += new System.EventHandler(this.FontListBox_SelectedIndexChanged);
			// 
			// BtnConvert
			// 
			this.BtnConvert.Location = new System.Drawing.Point(250, 12);
			this.BtnConvert.Name = "BtnConvert";
			this.BtnConvert.Size = new System.Drawing.Size(113, 23);
			this.BtnConvert.TabIndex = 1;
			this.BtnConvert.Text = "Convert selected";
			this.BtnConvert.UseVisualStyleBackColor = true;
			this.BtnConvert.Click += new System.EventHandler(this.BtnConvert_Click);
			// 
			// TxtTextHeight
			// 
			this.TxtTextHeight.Location = new System.Drawing.Point(434, 14);
			this.TxtTextHeight.MaxLength = 3;
			this.TxtTextHeight.Name = "TxtTextHeight";
			this.TxtTextHeight.Size = new System.Drawing.Size(43, 20);
			this.TxtTextHeight.TabIndex = 4;
			this.TxtTextHeight.Text = "9";
			this.TxtTextHeight.TextChanged += new System.EventHandler(this.TxtTextHeight_TextChanged);
			// 
			// LblTextHeight
			// 
			this.LblTextHeight.AutoSize = true;
			this.LblTextHeight.Location = new System.Drawing.Point(369, 17);
			this.LblTextHeight.Name = "LblTextHeight";
			this.LblTextHeight.Size = new System.Drawing.Size(59, 13);
			this.LblTextHeight.TabIndex = 5;
			this.LblTextHeight.Text = "text height:";
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(981, 580);
			this.Controls.Add(this.LblTextHeight);
			this.Controls.Add(this.TxtTextHeight);
			this.Controls.Add(this.FontListBox);
			this.Controls.Add(this.TabControl);
			this.Controls.Add(this.BtnConvert);
			this.Controls.Add(this.BtnSave);
			this.Controls.Add(this.BtnOpen);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainWindow";
			this.Text = "WFN-FontEditor";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button BtnOpen;
		private System.Windows.Forms.TabControl TabControl;
		private System.Windows.Forms.Button BtnSave;
		private System.Windows.Forms.ListBox FontListBox;
		private System.Windows.Forms.Button BtnConvert;
		private System.Windows.Forms.TextBox TxtTextHeight;
		private System.Windows.Forms.Label LblTextHeight;
	}
}

