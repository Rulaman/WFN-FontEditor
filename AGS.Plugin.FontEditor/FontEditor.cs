using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AGS.Types;

namespace AGS.Plugin.FontEditor
{
	public partial class FontEditor: EditorContentPanel
	{
		private bool			bInEdit				= false;
		private Point			EditPoint;
		private const Int32		MaxWidth			= 32;
		private const Int32		MaxHeight			= 32;

		private IAGSEditor		_editor;
		private PictureBox[]	CharacterPictures;
		private Bitmap			Selected;
		private UInt32			Index;
		private CFontInfo		SelectedFont;
		public List<CFontInfo>	FontList			= new List<CFontInfo>();
		private bool			ClickedOnCharacter	= false;

		private void InitPictures()
		{
			CharacterPictures = new PictureBox[] 
			{ 
				pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10,
				pictureBox11, pictureBox12, pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18, pictureBox19, pictureBox20,
				pictureBox21, pictureBox22, pictureBox23, pictureBox24, pictureBox25, pictureBox26, pictureBox27, pictureBox28, pictureBox29, pictureBox30,
				pictureBox31, pictureBox32, pictureBox33, pictureBox34, pictureBox35, pictureBox36, pictureBox37, pictureBox38, pictureBox39, pictureBox40,
				pictureBox41, pictureBox42, pictureBox43, pictureBox44, pictureBox45, pictureBox46, pictureBox47, pictureBox48, pictureBox49, pictureBox50,
				pictureBox51, pictureBox52, pictureBox53, pictureBox54, pictureBox55, pictureBox56, pictureBox57, pictureBox58, pictureBox59, pictureBox60,
				pictureBox61, pictureBox62, pictureBox63, pictureBox64, pictureBox65, pictureBox66, pictureBox67, pictureBox68, pictureBox69, pictureBox70,
				pictureBox71, pictureBox72, pictureBox73, pictureBox74, pictureBox75, pictureBox76, pictureBox77, pictureBox78, pictureBox79, pictureBox80,
				pictureBox81, pictureBox82, pictureBox83, pictureBox84, pictureBox85, pictureBox86, pictureBox87, pictureBox88, pictureBox89, pictureBox90,
				pictureBox91, pictureBox92, pictureBox93, pictureBox94, pictureBox95, pictureBox96, pictureBox97, pictureBox98, pictureBox99, pictureBox100,
				pictureBox101, pictureBox102, pictureBox103, pictureBox104, pictureBox105, pictureBox106, pictureBox107, pictureBox108, pictureBox109, pictureBox110,
				pictureBox111, pictureBox112, pictureBox113, pictureBox114, pictureBox115, pictureBox116, pictureBox117, pictureBox118, pictureBox119, pictureBox120,
				pictureBox121, pictureBox122, pictureBox123, pictureBox124, pictureBox125, pictureBox126, pictureBox127, pictureBox128
			};
		}
		public FontEditor(IAGSEditor editor)
		{
			InitializeComponent();
			InitPictures();
			_editor = editor;
		}
		public FontEditor()
		{
			InitializeComponent();
			InitPictures();
		}

		public bool AddFontToList(string filepath, string filename, string fontname)
		{
			if ( System.IO.File.Exists(System.IO.Path.Combine(filepath, filename)) )
			{
				CFontInfo info = new CFontInfo();

				info.FontPath = System.IO.Path.Combine(filepath, filename);
				info.FontName = fontname;

				TreeNode node = new TreeNode();

				node.Tag = info;
				node.Name = fontname;
				node.Text = fontname;

				FontView.Nodes.Add(node);
				FontList.Add(info);

				System.IO.FileStream file = null;
				System.IO.BinaryReader binaryReader = null;

				try
				{
					file = System.IO.File.Open(info.FontPath, System.IO.FileMode.Open);
					binaryReader = new System.IO.BinaryReader(file);

					info.Read(binaryReader);
				}
				catch
				{
				}
				finally
				{
					if ( null != binaryReader )
					{
						binaryReader.Close();
					}
					if ( null != file )
					{
						file.Close();
					}
				}
				return true;
			}

			return false;
		}
		public void PaintCharacter(Bitmap bitmap, Graphics graphics)
		{
			if ( null != bitmap )
			{
				bool bCreated = false;

				if ( graphics == null )
				{
					graphics = PictSelectedCharacter.CreateGraphics();
					bCreated = true;
				}

				PictSelectedCharacter.Width = bitmap.Width * 10;
				PictSelectedCharacter.Height = bitmap.Height * 10;

				graphics.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(0, 0, PictSelectedCharacter.Width, PictSelectedCharacter.Height));

				for ( int counter = 0; counter < bitmap.Width; counter++ )
				{
					for ( int innercounter = 0; innercounter < bitmap.Height; innercounter++ )
					{
						Color pixelColor = bitmap.GetPixel(counter, innercounter);

						Brush solidBrush = new SolidBrush(pixelColor);
						graphics.FillRectangle(solidBrush, new Rectangle(counter * 10, innercounter * 10, 10, 10));
					}
				}

				if ( bCreated )
				{
					graphics.Dispose();
				}
			}
		}

		private void FontView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			CFontInfo font = (CFontInfo)((TreeView)sender).SelectedNode.Tag;
			SelectedFont = font;

			for ( int counter = 0; counter < 128; counter++ )
			{
				Bitmap bitmap = null;

				CFontUtils.CreateBitmap(font.Character[counter], out bitmap);

				CharacterPictures[counter].Image = bitmap;
				PictSelectedCharacter.Image = bitmap;
				CharacterPictures[counter].Tag = font.Character[counter];
			}
		}

		private void PictSelectedCharacter_Click(object sender, EventArgs e)
		{
			if ( Selected != null )
			{
				MouseEventArgs mouse = (MouseEventArgs)e;
				Graphics graphics = PictSelectedCharacter.CreateGraphics();

				switch ( mouse.Button )
				{
				case MouseButtons.Left:
					{
						SolidBrush brush = new SolidBrush(Color.White);
						graphics.FillRectangle(brush, new Rectangle((mouse.X / 10) * 10, (mouse.Y / 10) * 10, 10, 10));

						if ( ((mouse.X / 10) <= Selected.Width) || ((mouse.Y / 10) <= Selected.Height) )
						{
							BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
							IntPtr ptr = bmpData.Scan0;

							ptr = (IntPtr)((int)ptr + bmpData.Stride * (mouse.Y / 10));
							byte[] b = new byte[bmpData.Stride];
							System.Runtime.InteropServices.Marshal.Copy(ptr, b, 0, bmpData.Stride);

							Array.Reverse(b);
							UInt32 line = BitConverter.ToUInt32(b, 0);
							line |= (UInt32)(0x80000000 >> (mouse.X / 10));
							b = BitConverter.GetBytes(line);
							Array.Reverse(b);

							System.Runtime.InteropServices.Marshal.Copy(b, 0, ptr, bmpData.Stride);
							Selected.UnlockBits(bmpData);
							CharacterPictures[Index].Image = Selected;
						}

						brush.Dispose();
					}
					break;
				case MouseButtons.Right:
					{
						SolidBrush brush = new SolidBrush(Color.Black);
						graphics.FillRectangle(brush, new Rectangle((mouse.X / 10) * 10, (mouse.Y / 10) * 10, 10, 10));

						if ( ((mouse.X / 10) <= Selected.Width) || ((mouse.Y / 10) <= Selected.Height) )
						{
							BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
							IntPtr ptr = bmpData.Scan0;

							ptr = (IntPtr)((int)ptr + bmpData.Stride * (mouse.Y / 10));
							byte[] b = new byte[bmpData.Stride];
							System.Runtime.InteropServices.Marshal.Copy(ptr, b, 0, bmpData.Stride);

							Array.Reverse(b);
							UInt32 line = BitConverter.ToUInt32(b, 0);
							line &= ~(UInt32)(0x80000000 >> (mouse.X / 10));
							b = BitConverter.GetBytes(line);
							Array.Reverse(b);

							System.Runtime.InteropServices.Marshal.Copy(b, 0, ptr, bmpData.Stride);
							Selected.UnlockBits(bmpData);
							CharacterPictures[Index].Image = Selected;
						}

						brush.Dispose();
					}
					break;
				case MouseButtons.Middle:
					{
						SolidBrush brush = new SolidBrush(Color.Black);
						Bitmap bitmap = (Bitmap)PictSelectedCharacter.Image;
						Color col = bitmap.GetPixel(mouse.X, mouse.Y);

						if ( ((mouse.X / 10) <= Selected.Width) || ((mouse.Y / 10) <= Selected.Height) )
						{
							BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
							IntPtr ptr = bmpData.Scan0;

							ptr = (IntPtr)((int)ptr + bmpData.Stride * (mouse.Y / 10));
							byte[] b = new byte[bmpData.Stride];
							System.Runtime.InteropServices.Marshal.Copy(ptr, b, 0, bmpData.Stride);

							Array.Reverse(b);
							UInt32 line = BitConverter.ToUInt32(b, 0);

							if ( col == Color.White )
							{
								brush = new SolidBrush(Color.Black);
								line &= ~(UInt32)(0x80000000 >> (mouse.X / 10));
							}
							else
							{
								brush = new SolidBrush(Color.White);
								line |= (UInt32)(0x80000000 >> (mouse.X / 10));
							}

							b = BitConverter.GetBytes(line);
							Array.Reverse(b);

							System.Runtime.InteropServices.Marshal.Copy(b, 0, ptr, bmpData.Stride);
							Selected.UnlockBits(bmpData);
							CharacterPictures[Index].Image = Selected;
						}

						graphics.FillRectangle(brush, new Rectangle((mouse.X / 10) * 10, (mouse.Y / 10) * 10, 10, 10));
						brush.Dispose();
					}
					break;
				};

				CFontUtils.SaveByteLinesFromPicture(SelectedFont.Character[Index], Selected);
			}
		}
		private void PictSelectedCharacter_MouseDown(object sender, MouseEventArgs e)
		{
			bInEdit = true;
		}
		private void PictSelectedCharacter_MouseMove(object sender, MouseEventArgs e)
		{
			if ( bInEdit )
			{
				if ( ((e.X/10) - EditPoint.X != 0) || ((e.Y/10) - EditPoint.Y != 0) )
				{
					EditPoint.X = e.X / 10;
					EditPoint.Y = e.Y / 10;

					PictSelectedCharacter_Click(sender, e);
				}
			}
		}
		private void PictSelectedCharacter_MouseUp(object sender, MouseEventArgs e)
		{
			bInEdit = false;
		}
		
		private void Character_Click(object sender, EventArgs e)
		{
			PictureBox pict = (PictureBox)sender;
			Bitmap bitmap = (Bitmap)pict.Image;
			
			if ( bitmap != null )
			{
				Selected = (Bitmap)pict.Image;

				/* This is for the first upper-left point in the Image. 
				 * So you can Draw over it in the other color from the start. */
				EditPoint.X = -1;
				EditPoint.Y = -1;

				uint i = 0;
				foreach ( PictureBox item in CharacterPictures )
				{
					if ( item == pict )
					{
						break;
					}
					i++;
				}
				Index = i;

				PaintCharacter(Selected, null);

				ClickedOnCharacter = true;
				numWidth.Value = ((CFontInfo)FontView.SelectedNode.Tag).Character[Index].Width;
				numHeight.Value = ((CFontInfo)FontView.SelectedNode.Tag).Character[Index].Height;
				ClickedOnCharacter = false;
			}
		}
		private void BtnSave_Click(object sender, EventArgs e)
		{
			if ( RadioSelected.Checked )
			{
				CFontUtils.SaveOneFont(SelectedFont);
			}
			else
			{
				foreach ( CFontInfo item in FontList )
				{
					CFontUtils.SaveOneFont(item);
				}
			}
		}

		private void numWidth_ValueChanged(object sender, EventArgs e)
		{
			if ( numWidth.Value > MaxWidth )
			{
				numWidth.Value = MaxWidth;
			}

			/* Now adjust the image size. */
			if ( !ClickedOnCharacter )
			{
				CFontUtils.RecreateCharacter(SelectedFont.Character[Index], (UInt16)numWidth.Value, (UInt16)numHeight.Value);
				Bitmap bitmap = null;

				CFontUtils.CreateBitmap(SelectedFont.Character[Index], out bitmap);
				CharacterPictures[Index].Image = bitmap;
				Selected = bitmap;
				PaintCharacter(bitmap, null);
			}
		}
		private void numHeight_ValueChanged(object sender, EventArgs e)
		{
			if ( numHeight.Value > MaxHeight )
			{
				numHeight.Value = MaxHeight;
			}

			/* Now adjust the image size. */
			if ( !ClickedOnCharacter )
			{
				CFontUtils.RecreateCharacter(SelectedFont.Character[Index], (UInt16)numWidth.Value, (UInt16)numHeight.Value);
				Bitmap bitmap = null;

				CFontUtils.CreateBitmap(SelectedFont.Character[Index], out bitmap);
				CharacterPictures[Index].Image = bitmap;
				Selected = bitmap;
				PaintCharacter(bitmap, null);
			}
		}

		private void PictSelectedCharacter_Paint(object sender, PaintEventArgs e)
		{
			if ( null != Selected )
			{
				PaintCharacter(Selected, e.Graphics);
			}
		}

		private void restoreOriginalCharacterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripItem menuItem1 = sender as ToolStripItem;
			if ( menuItem1 != null )
			{
				ContextMenuStrip owner = menuItem1.Owner as ContextMenuStrip;
				if ( owner != null )
				{
					Control sourceControl = owner.SourceControl;

					uint i = 0;
					foreach ( PictureBox item in CharacterPictures )
					{
						if ( item == sourceControl )
						{
							break;
						}
						i++;
					}
					
					SelectedFont.Character[i].ByteLines = new byte[SelectedFont.Character[i].ByteLinesOriginal.Length];
					SelectedFont.Character[i].Width = SelectedFont.Character[i].WidthOriginal;
					SelectedFont.Character[i].Height = SelectedFont.Character[i].HeightOriginal;

					Array.Copy(SelectedFont.Character[i].ByteLinesOriginal, SelectedFont.Character[i].ByteLines, SelectedFont.Character[i].ByteLinesOriginal.Length);

					Bitmap bitmap = null;
					CFontUtils.CreateBitmap(SelectedFont.Character[i], out bitmap);
					CharacterPictures[i].Image = bitmap;

					if ( i == Index )
					{
						ClickedOnCharacter = true;
						numWidth.Value = SelectedFont.Character[i].Width;
						numHeight.Value = SelectedFont.Character[i].Height;
						ClickedOnCharacter = false;

						Selected = bitmap;
						PaintCharacter(bitmap, null);
					}
				}
			}
		}
	}
}
