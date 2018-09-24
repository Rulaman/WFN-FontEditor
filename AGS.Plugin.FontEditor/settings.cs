using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AGS.Plugin.FontEditor
{
	public class Settings
	{
		public Color Color;
		public bool Grid;
		public bool GridFix;
		public string CustomText;
		
		private XmlDocument doc = new XmlDocument();
		private string Filename = @"WFN-FontEditor.xml";
		private string PartOne = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<?xml-stylesheet type=\"text/xsl\" href=\"xsl/installation.xsl\"?>\r\n<FontEditor><Color>-12566464</Color><Text>";
		private string PartTwo = "</Text><Grid>false</Grid><GridFix>false</GridFix></FontEditor>";
		private string PartComplete = "";

		private bool CreateFile(string filename)
		{
			doc.LoadXml((PartComplete));
			//Save the document to a file.
			doc.Save(filename);

			return true;
		}
		private bool LoadFile(string filename)
		{
			if ( ! System.IO.File.Exists(filename) )
			{
				string newValue = "";

				Settings.InputBox("Set a Text you wish to render.", "If you leave empty, the default text will be rendered.", ref newValue);

				if ( newValue != null && newValue != "" )
				{
					CustomText = newValue;
					PartComplete = PartOne + CustomText + PartTwo;
				}
				else
				{
					//PartComplete = PartOne + "Franz jagt im total verwahrlosten Taxi quer durch Bayern." + PartTwo;
					//PartComplete = PartOne + "Falsches Üben von Xylophonmusik quält jeden größeren Zwerg." + PartTwo;
					//PartComplete = PartOne + "El pingüino Wenceslao hizo kilómetros bajo exhaustiva lluvia y frío, añoraba a su querido cachorro." + PartTwo;
					PartComplete = PartOne + "The quick brown Fox jumped over the lazy Dog." + PartTwo;
				}

				CreateFile(filename);
			}

			doc.Load(filename);

			try { Grid = bool.Parse(((XmlNode)doc.SelectSingleNode("FontEditor/Grid")).InnerText); }
			catch { Grid = false; }

			try { GridFix = bool.Parse(((XmlNode)doc.SelectSingleNode("FontEditor/GridFix")).InnerText); }
			catch { GridFix = false; }

			try { CustomText = ((XmlNode)doc.SelectSingleNode("FontEditor/Text")).InnerText; }
			catch { CustomText = ""; }

			try { Color = Color.FromArgb(int.Parse(((XmlNode)doc.SelectSingleNode("FontEditor/Color")).InnerText)); }
			catch { Color = Color.Silver; }

			return true;
		}

		public bool Read()
		{
			//CreateFile(@"C:\testxml.xml");
			LoadFile(Filename);
			return true;
		}
		public bool Write()
		{
			if ( doc != null )
			{
				try { ((XmlNode)doc.SelectSingleNode("FontEditor/Grid")).InnerText = Grid.ToString(); }
				catch { ((XmlNode)doc.CreateElement("Grid")).InnerText = Grid.ToString(); }
				
				try { ((XmlNode)doc.SelectSingleNode("FontEditor/GridFix")).InnerText = GridFix.ToString(); }
				catch { ((XmlNode)doc.CreateElement("GridFix")).InnerText = GridFix.ToString(); }
				
				try { ((XmlNode)doc.SelectSingleNode("FontEditor/Text")).InnerText = CustomText;}
				catch { ((XmlNode)doc.CreateElement("Text")).InnerText = CustomText; }

				try { ((XmlNode)doc.SelectSingleNode("FontEditor/Color")).InnerText = Color.ToArgb().ToString(); }
				catch { ((XmlNode)doc.CreateElement("Color")).InnerText = Color.ToArgb().ToString(); }

				doc.Save(Filename);
			}
			return true;
		}

		public static DialogResult InputBox(string title, string promptText, ref string value)
		{
			Form form = new Form();
			Label label = new Label();
			TextBox textBox = new TextBox();
			Button buttonOk = new Button();
			Button buttonCancel = new Button();

			form.Text = title;
			label.Text = promptText;
			textBox.Text = value;

			buttonOk.Text = "OK";
			buttonCancel.Text = "Cancel";
			buttonOk.DialogResult = DialogResult.OK;
			buttonCancel.DialogResult = DialogResult.Cancel;

			label.SetBounds(9, 20, 372, 13);
			textBox.SetBounds(12, 36, 372, 20);
			buttonOk.SetBounds(228, 72, 75, 23);
			buttonCancel.SetBounds(309, 72, 75, 23);

			label.AutoSize = true;
			textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
			buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

			form.ClientSize = new Size(396, 107);
			form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
			form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.AcceptButton = buttonOk;
			form.CancelButton = buttonCancel;

			DialogResult dialogResult = form.ShowDialog();
			value = textBox.Text;
			return dialogResult;
		}
	}
}