using System;
using System.Drawing;
using System.Windows.Forms;

namespace WFN_FontEditor
{
	public partial class MainWindow: Form
	{
		public MainWindow()
		{
			InitializeComponent();
			LocalFontEditor.Location = new Point(0, 0);

			int iCaptionHeight = SystemInformation.CaptionHeight;
			Size Border = SystemInformation.Border3DSize;

			this.Size = new Size(LocalFontEditor.Size.Width + Border.Width*4, LocalFontEditor.Size.Height + Border.Height*4 + iCaptionHeight);
			this.MinimumSize = this.Size;
			this.MaximumSize = this.Size;
		}

		private void BtnOpen_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();

			if ( DialogResult.OK == fbd.ShowDialog() )
			{
				string[] files = System.IO.Directory.GetFiles(fbd.SelectedPath, "*.wfn");

				LocalFontEditor.FontList.Clear();
				LocalFontEditor.FontView.Nodes.Clear();

				foreach ( string item in files )
				{
					LocalFontEditor.AddFontToList(fbd.SelectedPath, System.IO.Path.GetFileName(item), System.IO.Path.GetFileName(item));
				}
			}
		}
	}
}
