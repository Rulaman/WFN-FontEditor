using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
//using System.Data;
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
		private Int32 Scalefactor = 3;
		private CFontInfo FontInfo;
		private const Int32 MaxWidth = 32;
		private const Int32 MaxHeight = 32;
		private bool ClickedOnCharacter = false;

		private void SetIndex(PictureBox picturebox)
		{
			Int32 index = 0;
			foreach ( PictureBox item in CharacterPictureList )
			{
				if ( picturebox == item )
				{
					break;
				}
				index++;
			}

			Index = index;
		}

		public FontEditorPane()
		{
			InitializeComponent();
		}
		public FontEditorPane(string filepath, string filename, string fontname)
		{
			InitializeComponent();

			if ( System.IO.File.Exists(System.IO.Path.Combine(filepath, filename)) )
			{
				FontInfo = new CFontInfo();

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

				foreach ( CCharInfo item in FontInfo.Character )
				{
					Bitmap bitmap = null; 
					PictureBox pict = new PictureBox();
					CharacterPictureList.Add(pict);

					pict.Tag = item;
					pict.Width = item.Width * Scalefactor;
					pict.Height = item.Height * Scalefactor;
					pict.Click += new EventHandler(Character_Click);
					
					CFontUtils.CreateBitmap(item, out bitmap);

					item.UnscaledImage = bitmap;

					Bitmap outbmp;
					CFontUtils.ScaleBitmap(bitmap, out outbmp, Scalefactor);
					pict.Image = outbmp;

					FlowCharacterPanel.Controls.Add(pict);
				}
			}
		}

		void Character_Click(object sender, EventArgs e)
		{
			PictureBox picture = (PictureBox)sender;
			CCharInfo characterinfo = (CCharInfo)(picture.Tag);
			Bitmap bitmap = (Bitmap)characterinfo.UnscaledImage;

			Index = characterinfo.Index;

			Bitmap scaledbitmap;
			CFontUtils.ScaleBitmap(bitmap, out scaledbitmap, ZoomDrawingArea.Value);
			DrawingArea.Size = scaledbitmap.Size;
			DrawingArea.Image = scaledbitmap;

			ClickedOnCharacter = true;
			numWidth.Value = characterinfo.Width;
			numHeight.Value = characterinfo.Height;
			ClickedOnCharacter = false;
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

		private void DrawingArea_Click(object sender, EventArgs e)
		{
			MouseEventArgs mouse = (MouseEventArgs)e;
			Graphics graphics = ((PictureBox)sender).CreateGraphics();
			Int32 zoom = ZoomDrawingArea.Value;
			Bitmap Selected;
			Color col = Color.Gray;

			if ( null != (Selected = (Bitmap)(((CCharInfo)(CharacterPictureList[Index].Tag)).UnscaledImage)) )
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
							col = Color.White;
						}
						break;
					case MouseButtons.Right:
						{
							col = Color.Black;
						}
						break;
					case MouseButtons.Middle:
						{
							//col = Selected.GetPixel(mouse.X / zoom, mouse.Y / zoom) == Color.White ? Color.Black : Color.White;
							col = ((line >> (mouse.X / zoom)) > 0) ? Color.Black : Color.White;
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
		}
		private void DrawingArea_MouseDown(object sender, MouseEventArgs e)
		{
			bInEdit = true;
		}
		private void DrawingArea_MouseMove(object sender, MouseEventArgs e)
		{
			if ( bInEdit )
			{
				Int32 zoom = ZoomDrawingArea.Value;

				if ( ((e.X / zoom) - EditPoint.X != 0) || ((e.Y / zoom) - EditPoint.Y != 0) )
				{
					EditPoint.X = e.X / zoom;
					EditPoint.Y = e.Y / zoom;

					DrawingArea_Click(sender, e);
				}
			}
		}
		private void DrawingArea_MouseUp(object sender, MouseEventArgs e)
		{
			bInEdit = false;
		}

		public void Save()
		{
			CFontUtils.SaveOneFont(FontInfo);
		}
		private void AdjustSize()
		{
			/* Now adjust the image size. */
			if ( !ClickedOnCharacter )
			{
				CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index]).Tag));
				CFontUtils.RecreateCharacter(character, (UInt16)numWidth.Value, (UInt16)numHeight.Value);
				Bitmap bitmap = null;

				CFontUtils.CreateBitmap(character, out bitmap);
				character.UnscaledImage = bitmap;

				Bitmap outbmp;
				CFontUtils.ScaleBitmap(bitmap, out outbmp, Scalefactor);
				CharacterPictureList[Index].Size = outbmp.Size;
				CharacterPictureList[Index].Image = outbmp;

				Bitmap drawingbitmap;
				CFontUtils.ScaleBitmap(bitmap, out drawingbitmap, ZoomDrawingArea.Value);
				DrawingArea.Size = drawingbitmap.Size;
				DrawingArea.Image = drawingbitmap;
			}
		}
		
		private void numWidth_ValueChanged(object sender, EventArgs e)
		{
			if ( numWidth.Value > MaxWidth )
			{
				numWidth.Value = MaxWidth;
			}

			AdjustSize();
		}
		private void numHeight_ValueChanged(object sender, EventArgs e)
		{
			if ( numHeight.Value > MaxHeight )
			{
				numHeight.Value = MaxHeight;
			}

			AdjustSize();
		}
	}
}
