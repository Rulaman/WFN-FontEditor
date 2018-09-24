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
			this.LocalFontEditor = new AGS.Plugin.FontEditor.FontEditor();
			this.BtnOpen = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// LocalFontEditor
			// 
			this.LocalFontEditor.BackColor = System.Drawing.Color.Gainsboro;
			this.LocalFontEditor.Location = new System.Drawing.Point(0, 0);
			this.LocalFontEditor.Name = "LocalFontEditor";
			this.LocalFontEditor.Size = new System.Drawing.Size(478, 379);
			this.LocalFontEditor.TabIndex = 0;
			// 
			// BtnOpen
			// 
			this.BtnOpen.Location = new System.Drawing.Point(12, 6);
			this.BtnOpen.Name = "BtnOpen";
			this.BtnOpen.Size = new System.Drawing.Size(113, 23);
			this.BtnOpen.TabIndex = 1;
			this.BtnOpen.Text = "Open Directory";
			this.BtnOpen.UseVisualStyleBackColor = true;
			this.BtnOpen.Click += new System.EventHandler(this.BtnOpen_Click);
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(506, 408);
			this.Controls.Add(this.BtnOpen);
			this.Controls.Add(this.LocalFontEditor);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainWindow";
			this.Text = "WFN-FontEditor";
			this.ResumeLayout(false);

		}

		#endregion

		private AGS.Plugin.FontEditor.FontEditor LocalFontEditor;
		private System.Windows.Forms.Button BtnOpen;
	}
}

