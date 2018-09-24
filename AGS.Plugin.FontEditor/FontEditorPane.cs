using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

using AGS.Types;

namespace AGS.Plugin.FontEditor
{
	public partial class FontEditorPane: EditorContentPanel
	{
		private List<PictureBox> CharacterPictureList = new List<PictureBox>();
		private Int32 Index;
		private bool bInEdit = false;
		private Point EditPoint;
		private Int32 Scalefactor = 2;
		private CFontInfo FontInfo;
		private const Int32 MaxWidth = 32;
		private const Int32 MaxHeight = 32;
		private bool ClickedOnCharacter = false;
		private bool FontModifiedSaved = false;
		private bool ShowGrid = false;
		private Point Grid = new Point(-1, -1);
		private Pen GridPen = new Pen(Color.FromArgb(255, 64, 64, 64)); // darker gray

		public event EventHandler OnFontModified;

		private enum SizeMode
		{
			ChangeNothing,
			ChangeWidth,
			ChangeHeight,
			ChangeBoth,
		}

		static bool ArraysEqual(byte[] a1, byte[] a2)
		{
			if ( a1 == a2 )
				return true;

			if ( a1 == null || a2 == null )
				return false;

			if ( a1.Length != a2.Length )
				return false;

			for ( int i = 0; i < a1.Length; i++ )
			{
				if ( a1[i] != a2[i] )
					return false;
			}
			return true;
		}

		private void CheckChange()
		{
			MyEventArgs me = new MyEventArgs();
			EventHandler DoChange = OnFontModified;

			foreach ( CCharInfo item in FontInfo.Character )
			{
				if ( !ArraysEqual(item.ByteLines, item.ByteLinesOriginal)
					|| (item.Width != item.WidthOriginal) 
					|| (item.Height != item.HeightOriginal) )
				{
					me.Modified = true;
					break;
				}
			}

			if ( FontModifiedSaved != me.Modified )
			{
				FontModifiedSaved = me.Modified;
			
				if ( null != DoChange )
				{
					DoChange(this, me);
				}
			}
		}

		internal static string Chr(int p_intByte)
		{
			if ( (p_intByte < 0) || (p_intByte > 255) )
			{
				throw new ArgumentOutOfRangeException("p_intByte", p_intByte, "Must be between 0 and 255.");
			}
			byte[] bytBuffer = new byte[] { (byte)p_intByte };
			return Encoding.GetEncoding(1252).GetString(bytBuffer);
		}

		public FontEditorPane()
		{
			InitializeComponent();
			LblZoom.Text = "x" + ZoomDrawingArea.Value;

			numWidth.Maximum = MaxWidth;
			numHeight.Maximum = MaxHeight;
		}
		public FontEditorPane(string filepath, string filename, string fontname)
		{
			InitializeComponent();
			LblZoom.Text = "x" + ZoomDrawingArea.Value;

			numWidth.Maximum = MaxWidth;
			numHeight.Maximum = MaxHeight;

			if ( System.IO.File.Exists(System.IO.Path.Combine(filepath, filename)) )
			{
				if ( System.IO.Path.GetExtension(filename).ToLower() == ".wfn" )
				{
					FontInfo = new CWFNFontInfo();
				}
				else if ( System.IO.Path.GetFileNameWithoutExtension(filename).ToLower() == "font" )
				{
					FontInfo = new CSCIFontInfo();
				}

				FontInfo.FontPath = System.IO.Path.Combine(filepath, filename);
				FontInfo.FontName = fontname;

				System.IO.FileStream file = null;
				System.IO.BinaryReader binaryReader = null;

				try
				{
					file = System.IO.File.Open(FontInfo.FontPath, System.IO.FileMode.Open);
					binaryReader = new System.IO.BinaryReader(file);
					FontInfo.Read(binaryReader);
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

				/* ++ Testcode for 256-character font */
				//for ( int i = 0; i < 128; i++ )
				//{
				//    FontInfo.Character[i + 128] = new CCharInfo();
				//    FontInfo.Character[i + 128].Height = FontInfo.Character[i].Height;
				//    FontInfo.Character[i + 128].HeightOriginal = FontInfo.Character[i].HeightOriginal;
				//    FontInfo.Character[i + 128].Width = FontInfo.Character[i].Width;
				//    FontInfo.Character[i + 128].WidthOriginal = FontInfo.Character[i].WidthOriginal;
				//    FontInfo.Character[i + 128].Index = FontInfo.Character[i].Index+128;

				//    FontInfo.Character[i + 128].ByteLines = new byte[FontInfo.Character[i].ByteLines.Length];
				//    FontInfo.Character[i + 128].ByteLinesOriginal = new byte[FontInfo.Character[i].ByteLinesOriginal.Length];

				//    Array.Copy(FontInfo.Character[i].ByteLines, FontInfo.Character[i + 128].ByteLines, FontInfo.Character[i + 128].ByteLines.Length);
				//    Array.Copy(FontInfo.Character[i].ByteLinesOriginal, FontInfo.Character[i + 128].ByteLinesOriginal, FontInfo.Character[i].ByteLinesOriginal.Length);
				//}
				/* -- Testcode for 256-character font */

				foreach ( CCharInfo item in FontInfo.Character )
				{
					Bitmap bitmap = null; 
					PictureBox pict = new PictureBox();
					CharacterPictureList.Add(pict);

					pict.Tag = item;
					pict.Width = item.Width * Scalefactor;
					pict.Height = item.Height * Scalefactor;
					pict.Click += new EventHandler(Character_Click);
					pict.ContextMenu = new ContextMenu();

					ToolTip tt = new ToolTip();
					tt.SetToolTip(pict, Chr(item.Index));

					pict.ContextMenu.Tag = pict;
					pict.ContextMenu.MenuItems.Add("Undo").Click += new EventHandler(MenuUndoClicked);
					pict.ContextMenu.MenuItems.Add("Redo").Click += new EventHandler(MenuRedoClicked);
					pict.ContextMenu.MenuItems.Add("Copy").Click += new EventHandler(MenuCopyClicked);
					pict.ContextMenu.MenuItems.Add("Paste").Click += new EventHandler(MenuPasteClicked);
					pict.ContextMenu.MenuItems[0].Enabled = false;
					pict.ContextMenu.MenuItems[1].Enabled = false;

					CFontUtils.CreateBitmap(item, out bitmap);

					item.UnscaledImage = bitmap;
					item.UndoRedoListAdd(item.ByteLines);

					Bitmap outbmp;
					CFontUtils.ScaleBitmap(bitmap, out outbmp, Scalefactor);
					pict.Image = outbmp;

					FlowCharacterPanel.Controls.Add(pict);
				}
			}
		}

		private bool CorrectImage(Bitmap original, out Bitmap corrected)
		{

			BitmapData bmpData = original.LockBits(new Rectangle(0, 0, original.Width, original.Height), ImageLockMode.ReadWrite, original.PixelFormat);

			if ( bmpData.Stride < 0 )
			{
				corrected = new Bitmap(original.Width, original.Height, original.PixelFormat);
				BitmapData cbmpdata = corrected.LockBits(new Rectangle(0, 0, corrected.Width, corrected.Height), ImageLockMode.ReadWrite, corrected.PixelFormat);

				IntPtr ptroriginal = bmpData.Scan0;
				IntPtr ptrorrected = cbmpdata.Scan0;
				byte[] array = new byte[Math.Abs(bmpData.Stride)];

				for ( int cnt = 0; cnt < original.Height; cnt++ )
				{
					System.Runtime.InteropServices.Marshal.Copy(ptroriginal, array, 0, Math.Abs(bmpData.Stride));
					System.Runtime.InteropServices.Marshal.Copy(array, 0, ptrorrected, cbmpdata.Stride);

					ptroriginal = (IntPtr)((int)ptroriginal + bmpData.Stride);
					ptrorrected = (IntPtr)((int)ptrorrected + cbmpdata.Stride);
				}

				original.UnlockBits(bmpData);
				corrected.UnlockBits(cbmpdata);

				return true;
			}
			else
			{
				original.UnlockBits(bmpData);
				corrected = null;
				return false;
			}
		}
		private void OutlineCharacter(Int32 index)
		{
			CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[index]).Tag));
			Bitmap Selected;

			if ( null != (Selected = (Bitmap)(character.UnscaledImage)) )
			{
				BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
				IntPtr ptrbegin = bmpData.Scan0;
				IntPtr ptrwrite = bmpData.Scan0;
				byte[] linearray = new byte[bmpData.Stride];
				UInt32 line;
				UInt32 temp2;

				bool[,] bitarray = new bool[bmpData.Stride * 8, bmpData.Height];
				bool[,] bitarraynew = new bool[bmpData.Stride * 8, bmpData.Height];

				for ( int ycnt = 0; ycnt < Selected.Height; ycnt++ )
				{
					System.Runtime.InteropServices.Marshal.Copy(ptrbegin, linearray, 0, bmpData.Stride);
					Array.Reverse(linearray);
					line = BitConverter.ToUInt32(linearray, 0);

					for ( int cnti = 0; cnti < MaxWidth; cnti++ )
					{
						temp2 = line >> (MaxWidth - (cnti + 1));

						if ( (temp2 & 1) > 0 )
						{
							bitarray[cnti, ycnt] = true;
						}
					}

					ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
				}

				for ( int cnt = 0; cnt < Selected.Height; cnt++ )
				{
					for ( int innercnt = 0; innercnt < MaxWidth; innercnt++ )
					{
						if ( bitarray[innercnt, cnt] )
						{
							/* set four pixel in the corner, if they don't set */
							if ( ((innercnt - 1) >= 0) && ((cnt - 1) >= 0) ) { bitarraynew[innercnt - 1, cnt - 1] = bitarray[innercnt - 1, cnt - 1] == true ? false : true; }
							if ( ((innercnt - 1) >= 0) && ((cnt + 1) < bmpData.Height) ) { bitarraynew[innercnt - 1, cnt + 1] = bitarray[innercnt - 1, cnt + 1] == true ? false : true; }
							if ( ((innercnt + 1) < (bmpData.Stride * 8)) && ((cnt - 1) >= 0) ) { bitarraynew[innercnt + 1, cnt - 1] = bitarray[innercnt + 1, cnt - 1] == true ? false : true; }
							if ( ((innercnt + 1) < (bmpData.Stride * 8)) && ((cnt + 1) < bmpData.Height) ) { bitarraynew[innercnt + 1, cnt + 1] = bitarray[innercnt + 1, cnt + 1] == true ? false : true; }

							/* set the pixel left and right */
							if ( ((innercnt - 1) >= 0) ) { bitarraynew[innercnt - 1, cnt] = bitarray[innercnt - 1, cnt] == true ? false : true; }
							if ( ((innercnt + 1) < (bmpData.Stride * 8)) ) { bitarraynew[innercnt + 1, cnt] = bitarray[innercnt + 1, cnt] == true ? false : true; }

							/* set the pixel upper and lower */
							if (  ((cnt - 1) >= 0) ) { bitarraynew[innercnt, cnt - 1] = bitarray[innercnt, cnt - 1] == true ? false : true; }
							if ( ((cnt + 1) < bmpData.Height) ) { bitarraynew[innercnt, cnt + 1] = bitarray[innercnt, cnt + 1] == true ? false : true; }
						}
					}
				}

				for ( int ycnt2 = 0; ycnt2 < Selected.Height; ycnt2++ )
				{
					line = 0;

					for ( int cnti = 0; cnti < MaxWidth; cnti++ )
					{
						if ( bitarraynew[cnti, ycnt2] )
						{
							line |= (UInt32)(0x80000000 >> (cnti));
						}
					}

					linearray = BitConverter.GetBytes(line);
					Array.Reverse(linearray);
					System.Runtime.InteropServices.Marshal.Copy(linearray, 0, ptrwrite, bmpData.Stride);

					ptrwrite = (IntPtr)(((int)ptrwrite + bmpData.Stride));
				}

				Selected.UnlockBits(bmpData);

				CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index].Tag), Selected);
				((CCharInfo)(CharacterPictureList[character.Index].Tag)).UnscaledImage = Selected;
			}

			character.UndoRedoListTidyUp();
			character.UndoRedoListAdd(character.ByteLines);

			CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
			CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

			CreateAndShow(character, SizeMode.ChangeNothing);
			CheckChange();
		}

		void MenuUndoClicked(object sender, EventArgs e)
		{
			MenuItem menu = (MenuItem)sender;

			if ( menu != null )
			{
				PictureBox picture = (PictureBox)menu.Parent.Tag;
				CCharInfo character = (CCharInfo)(picture.Tag);
				character.Undo();
				menu.Enabled = character.UndoPossible;

				Bitmap bitmap = null;
				CFontUtils.CreateBitmap(character, out bitmap);

				character.UnscaledImage = bitmap;
				Bitmap outbmp;
				CFontUtils.ScaleBitmap(bitmap, out outbmp, Scalefactor);
				picture.Image = outbmp;

				CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
				CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

				CreateAndShow(character, SizeMode.ChangeBoth);
				CheckChange();
			}
		}
		void MenuRedoClicked(object sender, EventArgs e)
		{
			MenuItem menu = (MenuItem)sender;

			if ( menu != null )
			{
				PictureBox picture = (PictureBox)menu.Parent.Tag;
				CCharInfo character = (CCharInfo)(picture.Tag);
				character.Redo();
				menu.Enabled = character.RedoPossible;

				Bitmap bitmap = null;
				CFontUtils.CreateBitmap(character, out bitmap);

				character.UnscaledImage = bitmap;
				Bitmap outbmp;
				CFontUtils.ScaleBitmap(bitmap, out outbmp, Scalefactor);
				picture.Image = outbmp;

				CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
				CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

				CreateAndShow(character, SizeMode.ChangeBoth);
				CheckChange();
			}
		}
		void MenuCopyClicked(object sender, EventArgs e)
		{
			MenuItem menu = (MenuItem)sender;

			if ( menu != null )
			{
				PictureBox picture = (PictureBox)menu.Parent.Tag;
				CCharInfo charinfo = (CCharInfo)(picture.Tag);
				Clipboard.SetData(DataFormats.Dib, charinfo.UnscaledImage);

				CheckChange();
			}
		}
		void MenuPasteClicked(object sender, EventArgs e)
		{
			MenuItem menu = (MenuItem)sender;

			if ( menu != null )
			{
				PictureBox picture = (PictureBox)menu.Parent.Tag;
				CCharInfo characterinfo = (CCharInfo)(picture.Tag);
				object t = characterinfo.UnscaledImage.Tag;
				Bitmap bitmap = (Bitmap)Clipboard.GetImage();
				
				Bitmap untested = Indexed.Image.CopyToBpp(bitmap, 1);
				Bitmap corrected = null;
				if ( CorrectImage(untested, out corrected) )
				{
					characterinfo.UnscaledImage = corrected;
				}
				else
				{
					characterinfo.UnscaledImage = untested;
				}

				CFontUtils.SaveByteLinesFromPicture(characterinfo, (Bitmap)characterinfo.UnscaledImage);
				characterinfo.UnscaledImage.Tag = t;
				bitmap = (Bitmap)characterinfo.UnscaledImage;
				Index = characterinfo.Index;

				Bitmap scaledbitmap;
				CFontUtils.ScaleBitmap(bitmap, out scaledbitmap, ZoomDrawingArea.Value);
				DrawingArea.Size = scaledbitmap.Size;
				DrawingArea.Image = scaledbitmap;
				characterinfo.Width = (UInt16)characterinfo.UnscaledImage.Width;
				characterinfo.Height = (UInt16)characterinfo.UnscaledImage.Height;

				ClickedOnCharacter = true;
				numWidth.Value = characterinfo.Width;
				numHeight.Value = characterinfo.Height;
				ClickedOnCharacter = false;

				Bitmap outbmp;
				CFontUtils.ScaleBitmap(bitmap, out outbmp, Scalefactor);
				CharacterPictureList[characterinfo.Index].Image = outbmp;
				CharacterPictureList[characterinfo.Index].Size = outbmp.Size;

				characterinfo.UndoRedoListTidyUp();
				characterinfo.UndoRedoListAdd(characterinfo.ByteLines);

				CharacterPictureList[characterinfo.Index].ContextMenu.MenuItems[0].Enabled = characterinfo.UndoPossible;
				CharacterPictureList[characterinfo.Index].ContextMenu.MenuItems[1].Enabled = characterinfo.RedoPossible;

				CheckChange();
			}
		}

		void Character_Click(object sender, EventArgs e)
		{
			MouseEventArgs mouse = (MouseEventArgs)e;
			PictureBox picture = (PictureBox)sender;
			CCharInfo characterinfo = (CCharInfo)(picture.Tag);
			ContextMenu menu = (ContextMenu)picture.ContextMenu;

			switch ( mouse.Button )
			{
			case MouseButtons.Left:
				{
					Bitmap bitmap = (Bitmap)characterinfo.UnscaledImage;

					Index = characterinfo.Index;

					ClickedOnCharacter = true;
					numWidth.Value = characterinfo.Width;
					numHeight.Value = characterinfo.Height;
					ClickedOnCharacter = false;

					Bitmap scaledbitmap;
					CFontUtils.ScaleBitmap(bitmap, out scaledbitmap, ZoomDrawingArea.Value);
					DrawingArea.Size = scaledbitmap.Size;
#if XY
					DrawingArea.Image = scaledbitmap;
#else				
					Graphics graphics = DrawingArea.CreateGraphics();
					graphics.DrawImage(scaledbitmap, 0, 0);
					
					if ( ShowGrid )
					{
					    Int32 zoom = ZoomDrawingArea.Value;

					    for ( int xcnt = 0; xcnt < characterinfo.Width; xcnt++ )
					    {
					        for ( int ycnt = 0; ycnt < characterinfo.Height; ycnt++ )
					        {
					            graphics.DrawRectangle(GridPen, xcnt * zoom, ycnt * zoom, zoom, zoom);
					        }
					    }
					}
					
					graphics.Dispose();
#endif
					
					

					PaintOnDrawingArea(DrawingArea, null);
				}
				break;
			case MouseButtons.Right:
				{
					menu.MenuItems[0].Enabled = characterinfo.UndoPossible;
					menu.MenuItems[1].Enabled = characterinfo.RedoPossible;
				}
				break;
			};
		}

		private void ZoomDrawingArea_ValueChanged(object sender, EventArgs e)
		{
			CCharInfo characterinfo = (CCharInfo)(CharacterPictureList[Index].Tag);

			Bitmap scaledbitmap;
			CFontUtils.ScaleBitmap((Bitmap)characterinfo.UnscaledImage, out scaledbitmap, ZoomDrawingArea.Value);
			DrawingArea.Size = scaledbitmap.Size;
			DrawingArea.Image = scaledbitmap;

			LblZoom.Text = "x" + ZoomDrawingArea.Value;
		}

		private void PaintOnDrawingArea(object sender, EventArgs e)
		{
			MouseEventArgs mouse = e as MouseEventArgs;


			Graphics graphics = ((PictureBox)sender).CreateGraphics();
			Int32 zoom = ZoomDrawingArea.Value;
			Bitmap Selected;
			Color col = Color.Gray;

			if ( null != (Selected = (Bitmap)(((CCharInfo)(CharacterPictureList[Index].Tag)).UnscaledImage)) )
			{
				if ( null != mouse && mouse.Button != MouseButtons.None )
				{
					if ( ((mouse.X / zoom) <= Selected.Width) || ((mouse.Y / zoom) <= Selected.Height) )
					{
						SolidBrush brush = new SolidBrush(Color.Gray);
						BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
						IntPtr ptr = bmpData.Scan0;
						ptr = (IntPtr)((int)ptr + bmpData.Stride * (mouse.Y / zoom));
						byte[] b = new byte[bmpData.Stride];
						System.Runtime.InteropServices.Marshal.Copy(ptr, b, 0, bmpData.Stride);
						Array.Reverse(b);
						UInt32 line = BitConverter.ToUInt32(b, 0);

						switch ( mouse.Button )
						{
						case MouseButtons.Left:
							{
								col = PanelLeftMouse.BackColor;
							}
							break;
						case MouseButtons.Right:
							{
								col = PanelRightMouse.BackColor;
							}
							break;
						case MouseButtons.Middle:
							{
								col = ((line >> (mouse.X / zoom)) > 0) ? PanelLeftMouse.BackColor : PanelRightMouse.BackColor;
							}
							break;
						};

						if ( Color.Black == col )
						{
							brush = new SolidBrush(Color.Black);
							line &= ~(UInt32)(0x80000000 >> (mouse.X / zoom));
						}
						else
						{
							brush = new SolidBrush(Color.White);
							line |= (UInt32)(0x80000000 >> (mouse.X / zoom));
						}

						b = BitConverter.GetBytes(line);
						Array.Reverse(b);
						((CCharInfo)(CharacterPictureList[Index].Tag)).UnscaledImage = Selected;

						System.Runtime.InteropServices.Marshal.Copy(b, 0, ptr, bmpData.Stride);
						Selected.UnlockBits(bmpData);
						graphics.FillRectangle(brush, new Rectangle((mouse.X / zoom) * zoom, (mouse.Y / zoom) * zoom, zoom, zoom));
						brush.Dispose();
					}

					CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[Index].Tag), Selected);
					((CCharInfo)(CharacterPictureList[Index].Tag)).UnscaledImage = Selected;

					Bitmap outbmp;
					CFontUtils.ScaleBitmap(Selected, out outbmp, Scalefactor);
					CharacterPictureList[Index].Image = outbmp;
				}

				if ( ShowGrid )
				{
					for ( int xcnt = 0; xcnt < Selected.Width; xcnt++ )
					{
						for ( int ycnt = 0; ycnt < Selected.Height; ycnt++)
						{
							graphics.DrawRectangle(GridPen, xcnt * zoom, ycnt * zoom, zoom, zoom);
						}
					}
				}
			}

			graphics.Dispose();
		}

		private void DrawingArea_Click(object sender, EventArgs e)
		{
			PaintOnDrawingArea(sender, e);
			CheckChange();
		}
		private void DrawingArea_MouseDown(object sender, MouseEventArgs e)
		{
			bInEdit = true;
		}
		private void DrawingArea_MouseMove(object sender, MouseEventArgs e)
		{
			Int32 zoom = ZoomDrawingArea.Value;

			if ( bInEdit )
			{
				if ( ((e.X / zoom) - EditPoint.X != 0) || ((e.Y / zoom) - EditPoint.Y != 0) )
				{
					EditPoint.X = e.X / zoom;
					EditPoint.Y = e.Y / zoom;

					PaintOnDrawingArea(sender, e);
				}
			}

			//if ( (Grid.X != (e.X / zoom)) || (Grid.Y != (e.Y / zoom)) )
			//{
			//    Grid.X = (e.X / zoom);
			//    Grid.Y = (e.Y / zoom);

			//    if ( ((e.X / zoom) - EditPoint.X != 0) || ((e.Y / zoom) - EditPoint.Y != 0) )
			//    {
			//        /* show the rectangle */
			//        //DrawingArea.Invalidate();
			//        //PaintOnDrawingArea(sender, e);
			//        Graphics graphics = ((PictureBox)sender).CreateGraphics();
			//        graphics.DrawRectangle(pen, (e.X / zoom) * zoom, (e.Y / zoom) * zoom, zoom, zoom);
			//		  graphics.Dispose();
			//    }
			//}
		}
		private void DrawingArea_MouseUp(object sender, MouseEventArgs e)
		{
			bInEdit = false;
			CheckChange();

			CCharInfo characterinfo = (CCharInfo)(CharacterPictureList[Index].Tag);
			characterinfo.UndoRedoListTidyUp();
			characterinfo.UndoRedoListAdd(characterinfo.ByteLines);

			CharacterPictureList[characterinfo.Index].ContextMenu.MenuItems[0].Enabled = characterinfo.UndoPossible;
			CharacterPictureList[characterinfo.Index].ContextMenu.MenuItems[1].Enabled = characterinfo.RedoPossible;
		}
		private void DrawingArea_Paint(object sender, PaintEventArgs e)
		{
			Bitmap Selected;

			if ( null != (Selected = (Bitmap)(((CCharInfo)(CharacterPictureList[Index].Tag)).UnscaledImage)) )
			{
				Bitmap scaledbitmap;
				CFontUtils.ScaleBitmap(Selected, out scaledbitmap, ZoomDrawingArea.Value);

				Graphics graphics = e.Graphics;
				graphics.DrawImage(scaledbitmap, 0, 0);

				if ( ShowGrid )
				{
					Int32 zoom = ZoomDrawingArea.Value;

					for ( int xcnt = 0; xcnt < ((CCharInfo)(CharacterPictureList[Index].Tag)).Width; xcnt++ )
					{
						for ( int ycnt = 0; ycnt < ((CCharInfo)(CharacterPictureList[Index].Tag)).Height; ycnt++ )
						{
							graphics.DrawRectangle(GridPen, xcnt * zoom, ycnt * zoom, zoom, zoom);
						}
					}
				}

				//graphics.Dispose();
			}
		}

		public void Save()
		{
			CFontUtils.SaveOneFont(FontInfo);
			CheckChange();
		}
		private void AdjustSize()
		{
			/* Now adjust the image size. */
			if ( !ClickedOnCharacter )
			{
				CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index]).Tag));
				CreateAndShow(character, SizeMode.ChangeBoth);
			}
			CheckChange();
		}

		private void CreateAndShow(CCharInfo character, SizeMode sizeMode)
		{
			switch ( sizeMode )
			{
			case SizeMode.ChangeNothing:
				{
					CFontUtils.RecreateCharacter(character, character.Width, character.Height);
				}
				break;
			case SizeMode.ChangeBoth:
				{
					CFontUtils.RecreateCharacter(character, (UInt16)numWidth.Value, (UInt16)numHeight.Value);
				}
				break;
			case SizeMode.ChangeHeight:
				{
					CFontUtils.RecreateCharacter(character, character.Width, (UInt16)numHeight.Value);
				}
				break;
			case SizeMode.ChangeWidth:
				{
					CFontUtils.RecreateCharacter(character, (UInt16)numWidth.Value, character.Height);
				}
				break;
			};

			Bitmap bitmap = null;

			CFontUtils.CreateBitmap(character, out bitmap);
			character.UnscaledImage = bitmap;

			Bitmap outbmp;
			CFontUtils.ScaleBitmap(bitmap, out outbmp, Scalefactor);
			CharacterPictureList[character.Index].Size = outbmp.Size;
			CharacterPictureList[character.Index].Image = outbmp;

			Bitmap drawingbitmap;
			CFontUtils.ScaleBitmap(bitmap, out drawingbitmap, ZoomDrawingArea.Value);
			DrawingArea.Size = drawingbitmap.Size;
			DrawingArea.Image = drawingbitmap;
		}
		
		private void numWidth_ValueChanged(object sender, EventArgs e)
		{
			if ( numWidth.Value > MaxWidth )
			{
				numWidth.Value = MaxWidth;
			}
			if ( numWidth.Value < 1 )
			{
				numWidth.Value = 1;
			}

			AdjustSize();
		}
		private void numHeight_ValueChanged(object sender, EventArgs e)
		{
			if ( numHeight.Value > MaxHeight )
			{
				numHeight.Value = MaxHeight;
			}
			if ( numHeight.Value < 1 )
			{
				numHeight.Value = 1;
			}

			AdjustSize();
		}

		private void BtnClear_Click(object sender, EventArgs e)
		{
			CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index]).Tag));

			for ( int bytelinecounter = 0; bytelinecounter < character.ByteLines.Length; bytelinecounter++ )
			{
				character.ByteLines[bytelinecounter] = 0x00;
			}

			character.UndoRedoListTidyUp();
			character.UndoRedoListAdd(character.ByteLines);

			CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
			CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

			CreateAndShow(character, SizeMode.ChangeNothing);
			CheckChange();
		}
		private void BtnFill_Click(object sender, EventArgs e)
		{
			CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index]).Tag));

			for ( int bytelinecounter = 0; bytelinecounter < character.ByteLines.Length; bytelinecounter++)
			{
				character.ByteLines[bytelinecounter] = 0xFF;
			}

			character.UndoRedoListTidyUp();
			character.UndoRedoListAdd(character.ByteLines);

			CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
			CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

			CreateAndShow(character, SizeMode.ChangeNothing);
			CheckChange();
		}

		private void BtnShiftUp_Click(object sender, EventArgs e)
		{
			CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index]).Tag));
			Bitmap Selected;

			if ( null != (Selected = (Bitmap)(character.UnscaledImage)) )
			{
				BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
				IntPtr ptrbegin = bmpData.Scan0;
				IntPtr ptrrest = bmpData.Scan0;
				ptrrest = (IntPtr)((int)ptrrest + bmpData.Stride);
				byte[] shiftarray = new byte[bmpData.Stride * Selected.Height];

				System.Runtime.InteropServices.Marshal.Copy(ptrrest, shiftarray, 0, bmpData.Stride * (Selected.Height - 1));
				System.Runtime.InteropServices.Marshal.Copy(ptrbegin, shiftarray, bmpData.Stride * (Selected.Height - 1), bmpData.Stride);

				ptrbegin = bmpData.Scan0;
				System.Runtime.InteropServices.Marshal.Copy(shiftarray, 0, ptrbegin, bmpData.Stride * Selected.Height);
				Selected.UnlockBits(bmpData);

				CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index].Tag), Selected);
				((CCharInfo)(CharacterPictureList[character.Index].Tag)).UnscaledImage = Selected;
			}

			character.UndoRedoListTidyUp();
			character.UndoRedoListAdd(character.ByteLines);

			CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
			CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

			CreateAndShow(character, SizeMode.ChangeNothing);
			CheckChange();
		}
		private void BtnShiftDown_Click(object sender, EventArgs e)
		{
			CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index]).Tag));
			Bitmap Selected;

			if ( null != (Selected = (Bitmap)(character.UnscaledImage)) )
			{
				BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
				IntPtr ptrbegin = bmpData.Scan0;
				IntPtr ptrrest = bmpData.Scan0;
				ptrrest = (IntPtr)((int)ptrrest + bmpData.Stride * (Selected.Height - 1));
				byte[] shiftarray = new byte[bmpData.Stride * Selected.Height];

				System.Runtime.InteropServices.Marshal.Copy(ptrbegin, shiftarray, bmpData.Stride, bmpData.Stride * (Selected.Height - 1));
				System.Runtime.InteropServices.Marshal.Copy(ptrrest, shiftarray, 0, bmpData.Stride);

				ptrbegin = bmpData.Scan0;
				System.Runtime.InteropServices.Marshal.Copy(shiftarray, 0, ptrbegin, bmpData.Stride * Selected.Height);
				Selected.UnlockBits(bmpData);

				CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index].Tag), Selected);
				((CCharInfo)(CharacterPictureList[character.Index].Tag)).UnscaledImage = Selected;
			}

			character.UndoRedoListTidyUp();
			character.UndoRedoListAdd(character.ByteLines);

			CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
			CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

			CreateAndShow(character, SizeMode.ChangeNothing);
			CheckChange();
		}
		private void BtnShiftLeft_Click(object sender, EventArgs e)
		{
			CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index]).Tag));
			Bitmap Selected;

			if ( null != (Selected = (Bitmap)(character.UnscaledImage)) )
			{
				BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
				IntPtr ptrbegin = bmpData.Scan0;
				IntPtr ptrwrite = bmpData.Scan0;
				byte[] linearray = new byte[bmpData.Stride];
				ptrbegin = bmpData.Scan0;
				UInt32 line;
				UInt32 overflow;

				for ( int cnt=0; cnt < Selected.Height; cnt++ )
				{
					System.Runtime.InteropServices.Marshal.Copy(ptrbegin, linearray, 0, bmpData.Stride);
					Array.Reverse(linearray);
					line = BitConverter.ToUInt32(linearray, 0);

					overflow = (line & 0x80000000) >> (Selected.Width-1);
					line <<= 1;
					line += overflow;

					linearray = BitConverter.GetBytes(line);
					Array.Reverse(linearray);
					System.Runtime.InteropServices.Marshal.Copy(linearray, 0, ptrwrite, bmpData.Stride);

					ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
					ptrwrite = (IntPtr)(((int)ptrwrite + bmpData.Stride));
				}

				Selected.UnlockBits(bmpData);

				CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index].Tag), Selected);
				((CCharInfo)(CharacterPictureList[character.Index].Tag)).UnscaledImage = Selected;
			}

			character.UndoRedoListTidyUp();
			character.UndoRedoListAdd(character.ByteLines);

			CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
			CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

			CreateAndShow(character, SizeMode.ChangeNothing);
			CheckChange();
		}
		private void BtnShiftRight_Click(object sender, EventArgs e)
		{
			CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index]).Tag));
			Bitmap Selected;

			if ( null != (Selected = (Bitmap)(character.UnscaledImage)) )
			{
				BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
				IntPtr ptrbegin = bmpData.Scan0;
				IntPtr ptrwrite = bmpData.Scan0;
				byte[] linearray = new byte[bmpData.Stride];
				ptrbegin = bmpData.Scan0;
				UInt32 line;
				UInt32 underflow;

				for ( int cnt=0; cnt < Selected.Height; cnt++ )
				{
					System.Runtime.InteropServices.Marshal.Copy(ptrbegin, linearray, 0, bmpData.Stride);
					Array.Reverse(linearray);
					line = BitConverter.ToUInt32(linearray, 0);

					underflow = line << (Selected.Width - 1);
					line >>= 1;
					line += underflow;

					linearray = BitConverter.GetBytes(line);
					Array.Reverse(linearray);
					System.Runtime.InteropServices.Marshal.Copy(linearray, 0, ptrwrite, bmpData.Stride);

					ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
					ptrwrite = (IntPtr)(((int)ptrwrite + bmpData.Stride));
				}

				Selected.UnlockBits(bmpData);

				CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index].Tag), Selected);
				((CCharInfo)(CharacterPictureList[character.Index].Tag)).UnscaledImage = Selected;
			}

			character.UndoRedoListTidyUp();
			character.UndoRedoListAdd(character.ByteLines);

			CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
			CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

			CreateAndShow(character, SizeMode.ChangeNothing);
			CheckChange();
		}

		//private void BtnSetAllHeight_Click(object sender, EventArgs e)
		//{
		//    foreach ( PictureBox item in CharacterPictureList )
		//    {
		//        if ( !ClickedOnCharacter )
		//        {
		//            CCharInfo character = ((CCharInfo)(item.Tag));
		//            CreateAndShow(character, SizeMode.ChangeHeight);
		//        }
		//    }

		//    CheckChange();
		//}

		private void ChkGrid_CheckedChanged(object sender, EventArgs e)
		{
			ShowGrid = ChkGrid.Checked;
			Character_Click(CharacterPictureList[Index], new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
		}

		private void BtnInvert_Click(object sender, EventArgs e)
		{
			CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index]).Tag));
			Bitmap Selected;

			if ( null != (Selected = (Bitmap)(character.UnscaledImage)) )
			{
				BitmapData	bmpData		= Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
				IntPtr		ptrbegin	= bmpData.Scan0;
				IntPtr		ptrwrite	= bmpData.Scan0;
				byte[]		linearray	= new byte[bmpData.Stride];
				UInt32		line;

				for ( int cnt = 0; cnt < Selected.Height; cnt++ )
				{
					System.Runtime.InteropServices.Marshal.Copy(ptrbegin, linearray, 0, bmpData.Stride);
					Array.Reverse(linearray);
					line = BitConverter.ToUInt32(linearray, 0);
					line = ~line;

					linearray = BitConverter.GetBytes(line);
					Array.Reverse(linearray);
					System.Runtime.InteropServices.Marshal.Copy(linearray, 0, ptrwrite, bmpData.Stride);

					ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
					ptrwrite = (IntPtr)(((int)ptrwrite + bmpData.Stride));
				}

				Selected.UnlockBits(bmpData);

				CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index].Tag), Selected);
				((CCharInfo)(CharacterPictureList[character.Index].Tag)).UnscaledImage = Selected;
			}

			ClickedOnCharacter = true;
			numWidth.Value = character.Width;
			numHeight.Value = character.Height;
			ClickedOnCharacter = false;

			character.UndoRedoListTidyUp();
			character.UndoRedoListAdd(character.ByteLines);

			CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
			CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

			CreateAndShow(character, SizeMode.ChangeNothing);
			CheckChange();
		}

		private void PictSwap_Click(object sender, EventArgs e)
		{
			Color temp = PanelRightMouse.BackColor;
			PanelRightMouse.BackColor = PanelLeftMouse.BackColor;
			PanelLeftMouse.BackColor = temp;
		}

		private void BtnSwapHorizontally_Click(object sender, EventArgs e)
		{
			CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index]).Tag));
			Bitmap Selected;

			if ( null != (Selected = (Bitmap)(character.UnscaledImage)) )
			{
				BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
				IntPtr ptrbegin = bmpData.Scan0;
				byte[][] linearray = new byte[Selected.Height][];

				for ( int cnt = 0; cnt < Selected.Height; cnt++ )
				{
					linearray[cnt] = new byte[bmpData.Stride];
					System.Runtime.InteropServices.Marshal.Copy(ptrbegin, linearray[cnt], 0, bmpData.Stride);
					ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
				}

				ptrbegin = bmpData.Scan0;

				for ( int cnt2 = 0; cnt2 < Selected.Height; cnt2++ )
				{
					System.Runtime.InteropServices.Marshal.Copy(linearray[Selected.Height - 1 - cnt2], 0, ptrbegin, bmpData.Stride);

					ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
				}

				Selected.UnlockBits(bmpData);

				CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index].Tag), Selected);
				((CCharInfo)(CharacterPictureList[character.Index].Tag)).UnscaledImage = Selected;
			}

			character.UndoRedoListTidyUp();
			character.UndoRedoListAdd(character.ByteLines);

			CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
			CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

			CreateAndShow(character, SizeMode.ChangeNothing);
			CheckChange();
		}
		private void BtnSwapVertically_Click(object sender, EventArgs e)
		{
			CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index]).Tag));
			Bitmap Selected;

			if ( null != (Selected = (Bitmap)(character.UnscaledImage)) )
			{
				BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
				IntPtr ptrbegin = bmpData.Scan0;
				IntPtr ptrwrite = bmpData.Scan0;
				byte[] linearray = new byte[bmpData.Stride];
				UInt32 line;

				for ( int cnt = 0; cnt < Selected.Height; cnt++ )
				{
					System.Runtime.InteropServices.Marshal.Copy(ptrbegin, linearray, 0, bmpData.Stride);
					Array.Reverse(linearray);
					line = BitConverter.ToUInt32(linearray, 0);

					UInt32 temp = 0;
					UInt32 temp2 = 0;
					//UInt32 temp3 = 0;

					for(int cnti=0;cnti<32;cnti++)
					{
						temp2 = line >> (32-(cnti+1));
						//temp3 = (UInt32)(1 << (cnti+1));

						temp |= (temp2&1) << cnti;
					}
					
					line = temp << (32-Selected.Width);

					linearray = BitConverter.GetBytes(line);
					Array.Reverse(linearray);
					System.Runtime.InteropServices.Marshal.Copy(linearray, 0, ptrwrite, bmpData.Stride);

					ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
					ptrwrite = (IntPtr)(((int)ptrwrite + bmpData.Stride));
				}

				Selected.UnlockBits(bmpData);

				CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index].Tag), Selected);
				((CCharInfo)(CharacterPictureList[character.Index].Tag)).UnscaledImage = Selected;
			}

			character.UndoRedoListTidyUp();
			character.UndoRedoListAdd(character.ByteLines);

			CharacterPictureList[character.Index].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
			CharacterPictureList[character.Index].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

			CreateAndShow(character, SizeMode.ChangeNothing);
			CheckChange();
		}

		private void BtnOutline_Click(object sender, EventArgs e)
		{
			OutlineCharacter(Index);
		}

		
		private void BtnOutlineFont_Click(object sender, EventArgs e)
		{
			for ( Int32 counter = 0; counter < CharacterPictureList.Count; counter++ )
			{
				OutlineCharacter(counter);
			}
		}
	}
}
