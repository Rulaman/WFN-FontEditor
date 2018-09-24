using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace AGS.Plugin.FontEditor
{
	public class CCharInfo
	{
		public UInt16		Width;
		public UInt16		Height;
		public byte[]		ByteLines;

		public UInt16		WidthOriginal;
		public UInt16		HeightOriginal;
		public byte[]		ByteLinesOriginal;

		public Image		UnscaledImage;
		public Int32		Index;

		public List<byte[]>	UndoRedoList = new List<byte[]>();
		public Int32		UndoRedoPosition = 0;

		public void UndoRedoListAdd(byte[] bytearray)
		{
			if ( UndoRedoList.Count == 0 )
			{
				UndoRedoList.Add(bytearray);
				//UndoRedoPosition++;
			}
			else if ( ArraysEqual(UndoRedoList[UndoRedoPosition], bytearray) )
			{
			}
			else
			{
				UndoRedoList.Add(bytearray);
				UndoRedoPosition++;
			}
		}
		public void UndoRedoListClear()
		{
			UndoRedoList.Clear();
		}
		public void UndoRedoListTidyUp()
		{
			while ( UndoRedoList.Count-1 > UndoRedoPosition )
			{
				UndoRedoList.RemoveAt(UndoRedoList.Count - 1);
			}
		}

		public bool RedoPossible
		{
			get { return (UndoRedoList.Count - 1 > UndoRedoPosition); }
		}
		public bool UndoPossible
		{
			get { return (UndoRedoPosition > 0); }
		}

		public void Undo()
		{
			if ( UndoRedoPosition > 0 )
			{
				UndoRedoPosition--;
				ByteLines = UndoRedoList[UndoRedoPosition];
			}
		}
		public void Redo()
		{
			if ( UndoRedoPosition < (UndoRedoList.Count) )
			{
				UndoRedoPosition++;
				ByteLines = UndoRedoList[UndoRedoPosition];
			}
		}

		private static bool ArraysEqual(byte[] a1, byte[] a2)
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
	}
	public class CWFNFontInfo
	{
		public string		FontPath;
		public string		FontName;
		public string		WFNName;
		public CCharInfo[]	Character;
		public Int16		NumberOfCharacters;

		private byte[] StringToByteArray(string str)
		{
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
			return enc.GetBytes(str);
		}
		private string ByteArrayToString(byte[] arr)
		{
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
			return enc.GetString(arr);
		}

		public void Read(System.IO.BinaryReader binaryReader)
		{
			byte[] name = binaryReader.ReadBytes(15);
			WFNName = ByteArrayToString(name);
			UInt16 offset = binaryReader.ReadUInt16();
			
			binaryReader.BaseStream.Position = offset;

			NumberOfCharacters = (Int16)((binaryReader.BaseStream.Length - offset) / 2);

			UInt16[] positionArray = new UInt16[NumberOfCharacters];
			Character = new CCharInfo[NumberOfCharacters];

			for ( int counter = 0; counter < NumberOfCharacters; counter++ )
			{
				positionArray[counter] = binaryReader.ReadUInt16();
			}

			for ( int counter = 0; counter < NumberOfCharacters; counter++ )
			{
				binaryReader.BaseStream.Position = positionArray[counter];

				Character[counter] = new CCharInfo();

				Character[counter].Index = counter;
				Character[counter].Width = binaryReader.ReadUInt16();
				Character[counter].Height = binaryReader.ReadUInt16();

				Character[counter].WidthOriginal = Character[counter].Width;
				Character[counter].HeightOriginal = Character[counter].Height;

				if ( counter == NumberOfCharacters - 1 )
				{
					Character[NumberOfCharacters - 1].ByteLines = binaryReader.ReadBytes(offset - positionArray[counter] - 4);
					Character[NumberOfCharacters - 1].ByteLinesOriginal = new byte[Character[NumberOfCharacters - 1].ByteLines.Length];
					Array.Copy(Character[NumberOfCharacters - 1].ByteLines, Character[NumberOfCharacters - 1].ByteLinesOriginal, Character[NumberOfCharacters - 1].ByteLines.Length);
				}
				else
				{
					Character[counter].ByteLines = binaryReader.ReadBytes(positionArray[counter + 1] - positionArray[counter] - 4);
					Character[counter].ByteLinesOriginal = new byte[Character[counter].ByteLines.Length];
					Array.Copy(Character[counter].ByteLines, Character[counter].ByteLinesOriginal, Character[counter].ByteLines.Length);
				}
			}
		}
		public void Write(System.IO.BinaryWriter binaryWriter)
		{
			Int64 PositionOfChars = 0;
			binaryWriter.Write(StringToByteArray(WFNName));
			binaryWriter.Write((UInt16)1); // Dummy, because offset is not yet known


			UInt16[] positionArray = new UInt16[Character.Length];

			foreach ( CCharInfo item in Character)
			{
				positionArray[item.Index] = (UInt16)binaryWriter.BaseStream.Position;

				binaryWriter.Write(item.Width);
				binaryWriter.Write(item.Height);
				binaryWriter.Write(item.ByteLines);

				item.WidthOriginal = item.Width;
				item.HeightOriginal = item.Height;
				item.ByteLinesOriginal = new byte[item.ByteLines.Length];
				Array.Copy(item.ByteLines, item.ByteLinesOriginal, item.ByteLines.Length);
			}

			PositionOfChars = binaryWriter.BaseStream.Position;

			for ( int counter = 0; counter < Character.Length; counter++ )
			{
				binaryWriter.Write(positionArray[counter]);
			}

			binaryWriter.BaseStream.Position = 15;
			binaryWriter.Write((UInt16)PositionOfChars);
		}
	}

	public static class CFontUtils
	{
		public static void RecreateCharacter(CCharInfo character, UInt16 newwidth, UInt16 newheight)
		{
			Bitmap oldbitmap;
			Bitmap tmpbitmap = new Bitmap(newwidth, newheight);
			Bitmap newbitmap = new Bitmap(newwidth, newheight, PixelFormat.Format1bppIndexed);

			CreateBitmap(character, out oldbitmap);

			Graphics g = Graphics.FromImage(tmpbitmap);
			g.DrawImage(oldbitmap, 0, 0, new Rectangle(0, 0, Math.Min(character.Width, newwidth), Math.Min(character.Height, newheight)), GraphicsUnit.Pixel);

			if ( (character.Width - newwidth) < 1 )
			{
				/* Picture goes wider */
				g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(character.Width, 0, newwidth, newheight));
			}

			if ( (character.Height - newheight) < 1 )
			{
				/* Picture goes higher */
				g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, character.Height, newwidth, newheight));
			}

			g.Dispose();

			newbitmap = Indexed.Image.CopyToBpp(tmpbitmap, 1);

			character.Height = newheight;
			character.Width = newwidth;
			SaveByteLinesFromPicture(character, newbitmap);
		}
		public static void CreateBitmap(CCharInfo character, out Bitmap bmp)
		{
			bmp = new System.Drawing.Bitmap(character.Width, character.Height, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
			IntPtr ptr = bmpData.Scan0;
			int len = bmpData.Width;
			Int32 bytesPerLine = 0;
			Int32 startPos = character.ByteLines.Length / bmpData.Height;

			if ( bmpData.Width <= 8 )
			{
				bytesPerLine = 1;
			}
			else if ( bmpData.Width <= 16 )
			{
				bytesPerLine = 2;
			}
			else if ( bmpData.Width <= 32 )
			{
				bytesPerLine = 3;
			}

			if ( character.ByteLines.Length == 0 )
			{
			}
			else
			{
				for ( int heightcounter = 0; heightcounter < bmp.Height; heightcounter++ )
				{
					System.Runtime.InteropServices.Marshal.Copy(character.ByteLines, startPos * heightcounter, ptr, bytesPerLine);
					ptr = (IntPtr)((int)ptr + bmpData.Stride);
				}
			}

			bmp.UnlockBits(bmpData);
		}
		public static void SaveByteLinesFromPicture(CCharInfo character, Bitmap bmp)
		{
			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
			IntPtr ptr = bmpData.Scan0;
			int len = bmpData.Width;
			Int32 bytesPerLine = 0;
			Int32 startPos = character.ByteLines.Length / bmpData.Height;

			if ( bmpData.Width <= 8 )
			{
				bytesPerLine = 1;
			}
			else if ( bmpData.Width <= 16 )
			{
				bytesPerLine = 2;
			}
			else if ( bmpData.Width <= 32 )
			{
				bytesPerLine = 3;
			}

			character.ByteLines = new byte[bmp.Height * bytesPerLine];
			for ( int heightcounter = 0; heightcounter < bmp.Height; heightcounter++ )
			{
				System.Runtime.InteropServices.Marshal.Copy(ptr, character.ByteLines, bytesPerLine * heightcounter, bytesPerLine);
				ptr = (IntPtr)((int)ptr + bmpData.Stride);
			}

			bmp.UnlockBits(bmpData);
		}
		public static void SaveOneFont(CWFNFontInfo font)
		{
			string filename = System.IO.Path.GetFileName(font.FontPath);
			string filepath = System.IO.Path.GetDirectoryName(font.FontPath);

			if ( !System.IO.File.Exists(font.FontPath + ".bak") ) // agsfnt1.wfn.bak
			{
				System.IO.File.Copy(font.FontPath, font.FontPath + ".bak");
			}

			System.IO.FileStream fs = System.IO.File.Open(font.FontPath, System.IO.FileMode.Create);
			System.IO.BinaryWriter binaryWriter = new System.IO.BinaryWriter(fs);

			font.Write(binaryWriter);

			binaryWriter.Close();
			fs.Close();
		}
		public static void ScaleBitmap(Bitmap bitmap, out Bitmap newbmp, Int32 scale)
		{
			newbmp = new Bitmap(bitmap, bitmap.Width * scale, bitmap.Height * scale);
			Graphics g = Graphics.FromImage(newbmp);
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
			g.DrawImage(bitmap, 0, 0, newbmp.Width, newbmp.Height);
		}
	}
}
