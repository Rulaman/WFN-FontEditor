using System;
using System.Drawing;
using System.Windows.Forms;
using AGS.Plugin.FontEditor;

namespace WFN_FontEditor
{
	public partial class MainWindow: Form
	{
		private FolderBrowserDialog fbd = new FolderBrowserDialog();

		public MainWindow()
		{
			InitializeComponent();
			int iCaptionHeight = SystemInformation.CaptionHeight;
			Size Border = SystemInformation.Border3DSize;
		}

		private void BtnOpen_Click(object sender, EventArgs e)
		{
			if ( DialogResult.OK == fbd.ShowDialog() )
			{
				BuildFontList();
			}
		}

		private void BuildFontList()
		{
			TabControl.Controls.Clear();
			FontListBox.Items.Clear();

			string[] wfnfiles = System.IO.Directory.GetFiles(fbd.SelectedPath, "*.wfn");
			foreach ( string item in wfnfiles )
			{
				FontListBox.Items.Add(new FontPane(fbd.SelectedPath, System.IO.Path.GetFileName(item), System.IO.Path.GetFileName(item)));
			}

			string[] scifiles = System.IO.Directory.GetFiles(fbd.SelectedPath, "FONT.*");
			foreach ( string item in scifiles )
			{
				FontListBox.Items.Add(new FontPane(fbd.SelectedPath, System.IO.Path.GetFileName(item), System.IO.Path.GetFileName(item)));
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
		private void BtnConvert_Click(object sender, EventArgs e)
		{
			FontPane pane = (FontPane)FontListBox.SelectedItem;

			string oldname = System.IO.Path.Combine(pane.Filepath, pane.Filename);

			if ( pane.Filename.Contains("AGSFNT") )
			{
				/* to SCI */
				CFontInfo oldfont = new CWFNFontInfo();
				CFontInfo newfont = new CSCIFontInfo();
				

				int number = int.Parse(pane.Filename.Replace("AGSFNT", "").Replace(".WFN", ""));
				string newname = System.IO.Path.Combine(pane.Filepath, "FONT." + number.ToString("000"));

				oldfont.Read(oldname);

				newfont.FontPath = pane.Filepath;
				newfont.FontName = pane.Filename;
				newfont.NumberOfCharacters = oldfont.NumberOfCharacters;
				newfont.TextHeight = UInt16.Parse(TxtTextHeight.Text);
				newfont.Character = new CCharInfo[newfont.NumberOfCharacters];

				for ( int cnt = 0; cnt < newfont.NumberOfCharacters; cnt++ )
				{
					newfont.Character[cnt] = oldfont.Character[cnt];
				}

				System.IO.FileStream fs = System.IO.File.Create(newname);
				fs.Close();
				newfont.Write(newname);
			}
			else if ( pane.Filename.Contains("FONT") )
			{
				/* to WFN */
				CFontInfo oldfont = new CSCIFontInfo();
				CFontInfo newfont = new CWFNFontInfo();

				int number = int.Parse(pane.Filename.Replace("FONT.", ""));
				string newname = System.IO.Path.Combine(pane.Filepath, "AGSFNT" + number.ToString() + ".WFN");

				oldfont.Read(oldname);

				newfont.FontPath = pane.Filepath;
				newfont.FontName = pane.Filename;
				newfont.NumberOfCharacters = oldfont.NumberOfCharacters;
				newfont.Character = new CCharInfo[newfont.NumberOfCharacters];

				for ( int cnt = 0; cnt < newfont.NumberOfCharacters; cnt++ )
				{
					newfont.Character[cnt] = oldfont.Character[cnt];
				}

				System.IO.FileStream fs = System.IO.File.Create(newname);
				fs.Close();
				newfont.Write(newname);
			}

			BuildFontList();
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

			if ( !bFound && pane != null )
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

		private void TxtTextHeight_TextChanged(object sender, EventArgs e)
		{
			UInt16 num=0;
			UInt16.TryParse(TxtTextHeight.Text, out num);

			TxtTextHeight.Text = num.ToString();
		}

	}
}
