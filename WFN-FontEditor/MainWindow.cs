using System;
using System.Drawing;
using System.Windows.Forms;
using AGS.Plugin.FontEditor;

namespace WFN_FontEditor
{
	public partial class MainWindow: Form
	{
		public MainWindow()
		{
			InitializeComponent();
			int iCaptionHeight = SystemInformation.CaptionHeight;
			Size Border = SystemInformation.Border3DSize;
		}

		private void BtnOpen_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();

			if ( DialogResult.OK == fbd.ShowDialog() )
			{
				string[] files = System.IO.Directory.GetFiles(fbd.SelectedPath, "*.wfn");
				TabControl.Controls.Clear();

				foreach ( string item in files )
				{
					FontEditorPane fep = new FontEditorPane(fbd.SelectedPath, System.IO.Path.GetFileName(item), System.IO.Path.GetFileName(item));
					TabPage tp = new TabPage();
					tp.Text = System.IO.Path.GetFileName(item);
					tp.Controls.Add(fep);
					TabControl.Controls.Add(tp);
				}
			}
		}
		private void BtnSave_Click(object sender, EventArgs e)
		{
			foreach ( object item in TabControl.Controls )
			{
				TabPage tp = item as TabPage;
				FontEditorPane fep = tp.Controls[0] as FontEditorPane;
				fep.Save();
			}
		}
	}
}
