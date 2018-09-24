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
				FontListBox.Items.Clear();

				foreach ( string item in files )
				{
					FontListBox.Items.Add(new FontPane(fbd.SelectedPath, System.IO.Path.GetFileName(item), System.IO.Path.GetFileName(item)));
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

		private void FontListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool bFound = false;
			ListBox listbox = (ListBox)sender;

			FontPane pane = (FontPane)listbox.SelectedItem;

			foreach ( TabPage tabpage in TabControl.Controls )
			{
				if ( tabpage.Tag == pane )
				{
					bFound = true;
					break;
				}

			}

			if ( !bFound )
			{
				FontEditorPane fep = new FontEditorPane(pane.Filepath, pane.Filename, pane.Fontname);
				fep.OnFontModified += new EventHandler(fep_OnFontModified);
				TabPage tp = new TabPage();
				tp.Text = System.IO.Path.GetFileName(pane.Fontname);
				tp.Tag = pane;
				tp.Controls.Add(fep);
				fep.Dock = DockStyle.Fill;
				TabControl.Controls.Add(tp);
				fep.Tag = tp;
			}
		}

		void fep_OnFontModified(object sender, System.EventArgs e)
		{
			TabPage tp = (TabPage)((FontEditorPane)sender).Tag;
			MyEventArgs me = (MyEventArgs)e;

			if ( tp.Text.Contains("*") && me.Modified == false )
			{
				tp.Text = tp.Text.Replace("*", "");
			}
			else if ( !tp.Text.Contains("*") && me.Modified == true )
			{
				tp.Text += "*";
			}
		}
	}
}
