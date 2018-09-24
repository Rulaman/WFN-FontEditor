using System;
using AGS.Types;

namespace AGS.Plugin.FontEditor
{
	public class MyEventArgs: EventArgs
	{
		public bool Modified;
	}

	public class FontPane
	{
		/* needed by plugin */
		public ContentDocument Document;
		public FontEditorPane Pane;
		public AGS.Types.Font Font;

		/* needed by standalone */
		public string Filepath;
		public string Filename;
		public string Fontname;

		public FontPane(ContentDocument document, FontEditorPane pane, AGS.Types.Font font)
		{
			Document = document;
			Pane = pane;
			Font = font;
		}
		public FontPane(string path, string filename, string fontname)
		{
			Filepath = path;
			Filename = filename;
			Fontname = fontname;
		}

		public override string ToString()
		{
			return Fontname;
		}
	}
}
