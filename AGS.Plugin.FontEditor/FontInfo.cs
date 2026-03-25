using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace AGS.Plugin.FontEditor
{
    public class CCharInfo
    {
        public UInt16 Width;
        public UInt16 Height;
        public byte[] ByteLines;

        public UInt16 WidthOriginal;
        public UInt16 HeightOriginal;
        public byte[] ByteLinesOriginal;

        public Image UnscaledImage;
        public Int32 Index;

        public List<byte[]> UndoRedoList = new List<byte[]>();
        public Int32 UndoRedoPosition = 0;

        public void UndoRedoListAdd(byte[] bytearray)
        {
            if (UndoRedoList.Count == 0)
            {
                UndoRedoList.Add(bytearray);
            }
            else if (ArraysEqual(UndoRedoList[UndoRedoPosition], bytearray))
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
            while (UndoRedoList.Count - 1 > UndoRedoPosition)
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
            if (UndoRedoPosition > 0)
            {
                UndoRedoPosition--;
                ByteLines = UndoRedoList[UndoRedoPosition];
            }
        }
        public void Redo()
        {
            if (UndoRedoPosition < (UndoRedoList.Count))
            {
                UndoRedoPosition++;
                ByteLines = UndoRedoList[UndoRedoPosition];
            }
        }

        private static bool ArraysEqual(byte[] a1, byte[] a2)
        {
            if (a1 == a2)
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                    return false;
            }
            return true;
        }

        public bool HasBitmapData()
        {
            if (ByteLines == null || ByteLines.Length == 0)
                return false;

            for (int i = 0; i < ByteLines.Length; i++)
            {
                if (ByteLines[i] != 0)
                    return true;
            }
            return false;
        }

        public bool IsEmptyGlyph()
        {
            return !HasBitmapData();
        }
    }

    public abstract class CFontInfo
    {
        public string FontPath;
        public string FontName;
        public string WFNName;
        public CCharInfo[] Character;
        public int NumberOfCharacters;
        public UInt16 TextHeight; // only SCI fonts

        protected byte[] StringToByteArray(string str)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetBytes(str);
        }
        protected string ByteArrayToString(byte[] arr)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(arr);
        }
        public abstract void Read(System.IO.BinaryReader binaryReader);
        public abstract void Write(System.IO.BinaryWriter binaryWriter);

        public void Read(string filename)
        {
            System.IO.FileStream file = System.IO.File.Open(filename, System.IO.FileMode.Open);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(file);

            Read(binaryReader);

            binaryReader.Close();
            file.Close();
        }
        public void Write(string filename)
        {
            // (Note: This method is not used by your Save flow, but Create is safer than Open)
            System.IO.FileStream file = System.IO.File.Open(filename, System.IO.FileMode.Create);
            System.IO.BinaryWriter binaryWriter = new System.IO.BinaryWriter(file);

            Write(binaryWriter);

            binaryWriter.Close();
            file.Close();
        }
    }

    public class CWFNFontInfo : CFontInfo
    {
        public override void Read(System.IO.BinaryReader binaryReader)
        {
            byte[] name = binaryReader.ReadBytes(15);
            WFNName = ByteArrayToString(name);

            UInt16 offset = binaryReader.ReadUInt16();

            // Offsets table starts at "offset"
            binaryReader.BaseStream.Position = offset;

            long bytesLeft = binaryReader.BaseStream.Length - offset;
            if (bytesLeft < 0) bytesLeft = 0;

            int numChars = (int)(bytesLeft / 2);
            NumberOfCharacters = numChars;

            UInt16[] positionArray = new UInt16[numChars];
            Character = new CCharInfo[numChars];

            for (int i = 0; i < numChars; i++)
                positionArray[i] = binaryReader.ReadUInt16();

            for (int i = 0; i < numChars; i++)
            {
                UInt16 pos = positionArray[i];

                // offset==0 means "missing/placeholder glyph"
                if (pos == 0)
                {
                    Character[i] = new CCharInfo
                    {
                        Index = i,
                        Width = 0,
                        Height = 0,
                        WidthOriginal = 0,
                        HeightOriginal = 0,
                        ByteLines = null,
                        ByteLinesOriginal = null
                    };
                    continue;
                }

                binaryReader.BaseStream.Position = pos;

                var ch = new CCharInfo();
                ch.Index = i;

                ch.Width = binaryReader.ReadUInt16();
                ch.Height = binaryReader.ReadUInt16();
                ch.WidthOriginal = ch.Width;
                ch.HeightOriginal = ch.Height;

                // If zero-size, treat as placeholder and skip data
                if (ch.Width == 0 || ch.Height == 0)
                {
                    ch.ByteLines = null;
                    ch.ByteLinesOriginal = null;
                    Character[i] = ch;
                    continue;
                }

                int bytesPerLine = (ch.Width - 1) / 8 + 1;
                int totalBytes = ch.Height * bytesPerLine;

                // Defensive: don't read beyond stream
                long remaining = binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
                if (remaining < totalBytes) totalBytes = (int)Math.Max(0, remaining);

                ch.ByteLines = binaryReader.ReadBytes(totalBytes);
                ch.ByteLinesOriginal = new byte[ch.ByteLines.Length];
                Array.Copy(ch.ByteLines, ch.ByteLinesOriginal, ch.ByteLines.Length);

                Character[i] = ch;
            }
        }

        private static void ThrowWfnTooLarge()
        {
            throw new Exception("Font exceeds 64KB. The WFN format used by AGS does not support files larger than 65,535 bytes.\n\nEither reduce the range or make glyphs smaller; refer to the red text above the font preview.");
        }

        public long EstimateSize()
        {
            if (Character == null)
                return 0;

            long estimatedSize = 17; // header (15 + 2)

            foreach (CCharInfo item in Character)
            {
                if (item == null)
                    continue;

                // Skip placeholder glyphs
                if (item.Height <= 0 || item.Width <= 0)
                    continue;

                estimatedSize += 2; // width
                estimatedSize += 2; // height

                int bytesPerLine = (item.Width - 1) / 8 + 1;
                estimatedSize += item.Height * bytesPerLine;
            }

            // ❌ DO NOT add offset table here

            return estimatedSize;
        }

        public override void Write(System.IO.BinaryWriter binaryWriter)
        {
            if (Character == null)
                throw new Exception("No font data to save (Character array is null).");

            if (string.IsNullOrEmpty(WFNName))
                WFNName = "WGT Font File  ";

            // Header
            binaryWriter.Write(StringToByteArray(WFNName));
            binaryWriter.Write((UInt16)1); // dummy; will patch later

            // 🔵 Find last non-placeholder glyph
            int lastUsedIndex = -1;

            for (int i = Character.Length - 1; i >= 0; i--)
            {
                CCharInfo ch = Character[i];

                if (ch != null && ch.Width > 0 && ch.Height > 0)
                {
                    lastUsedIndex = i;
                    break;
                }
            }

            // If everything is placeholder, still keep index 0
            if (lastUsedIndex < 0)
                lastUsedIndex = 0;

            // 🔵 Only allocate offset table up to last used glyph
            UInt16[] positionArray = new UInt16[lastUsedIndex + 1];

            // Write glyph data
            for (int i = 0; i <= lastUsedIndex; i++)
            {
                CCharInfo item = Character[i];

                // Placeholder
                if (item == null || item.Width == 0 || item.Height == 0)
                {
                    positionArray[i] = 0;
                    continue;
                }

                // Ensure ByteLines exists
                if (item.ByteLines == null)
                {
                    int bytesPerLine0 = (item.Width - 1) / 8 + 1;
                    item.ByteLines = new byte[item.Height * bytesPerLine0];
                }

                long pos = binaryWriter.BaseStream.Position;

                if (pos > UInt16.MaxValue)
                    throw new Exception("WFN glyph data exceeded 64KB. Reduce glyph count/size.");

                positionArray[i] = (UInt16)pos;

                binaryWriter.Write(item.Width);
                binaryWriter.Write(item.Height);

                int bytesPerLine = (item.Width - 1) / 8 + 1;
                int totalBytes = item.Height * bytesPerLine;

                if (item.ByteLines.Length != totalBytes)
                {
                    byte[] fixedBytes = new byte[totalBytes];
                    int copy = Math.Min(totalBytes, item.ByteLines.Length);
                    Array.Copy(item.ByteLines, fixedBytes, copy);
                    item.ByteLines = fixedBytes;
                }

                binaryWriter.Write(item.ByteLines);

                // Originals bookkeeping
                item.WidthOriginal = item.Width;
                item.HeightOriginal = item.Height;

                item.ByteLinesOriginal = new byte[item.ByteLines.Length];
                Array.Copy(item.ByteLines, item.ByteLinesOriginal, item.ByteLines.Length);
            }

            // Offset table position
            long offsetTablePos = binaryWriter.BaseStream.Position;

            if (offsetTablePos > UInt16.MaxValue)
                throw new Exception("WFN offset table pointer exceeded 64KB. Reduce glyph data size.");

            // Write offset table (ONLY up to lastUsedIndex)
            for (int i = 0; i < positionArray.Length; i++)
                binaryWriter.Write(positionArray[i]);

            // Patch header pointer
            binaryWriter.BaseStream.Position = 15;
            binaryWriter.Write((UInt16)offsetTablePos);
        }
    }

    public class CSCIFontInfo : CFontInfo
    {
        public override void Read(System.IO.BinaryReader binaryReader)
        {
            UInt16 resourcename = binaryReader.ReadUInt16();
            UInt16 alwayszero = binaryReader.ReadUInt16();
            NumberOfCharacters = binaryReader.ReadInt16();
            TextHeight = binaryReader.ReadUInt16();

            UInt16[] positionArray = new UInt16[NumberOfCharacters];
            Character = new CCharInfo[NumberOfCharacters];

            for (int counter = 0; counter < NumberOfCharacters; counter++)
            {
                positionArray[counter] = (UInt16)(binaryReader.ReadUInt16() + 2);
            }

            for (int counter = 0; counter < NumberOfCharacters; counter++)
            {
                binaryReader.BaseStream.Position = positionArray[counter];

                Character[counter] = new CCharInfo();

                Character[counter].Index = counter;
                Character[counter].Width = binaryReader.ReadByte();
                Character[counter].Height = binaryReader.ReadByte();

                Character[counter].WidthOriginal = Character[counter].Width;
                Character[counter].HeightOriginal = Character[counter].Height;

                if (counter == NumberOfCharacters - 1)
                {
                    Character[NumberOfCharacters - 1].ByteLines = binaryReader.ReadBytes((UInt16)(binaryReader.BaseStream.Length - (positionArray[counter] + 2)));
                    Character[NumberOfCharacters - 1].ByteLinesOriginal = new byte[Character[NumberOfCharacters - 1].ByteLines.Length];
                    Array.Copy(Character[NumberOfCharacters - 1].ByteLines, Character[NumberOfCharacters - 1].ByteLinesOriginal, Character[NumberOfCharacters - 1].ByteLines.Length);
                }
                else
                {
                    Character[counter].ByteLines = binaryReader.ReadBytes(positionArray[counter + 1] - (positionArray[counter] + 2));
                    Character[counter].ByteLinesOriginal = new byte[Character[counter].ByteLines.Length];
                    Array.Copy(Character[counter].ByteLines, Character[counter].ByteLinesOriginal, Character[counter].ByteLines.Length);
                }
            }
        }

        public override void Write(System.IO.BinaryWriter binaryWriter)
        {
            int glyphLimit = 256;

            if (Character != null && Character.Length > glyphLimit)
            {
                DialogResult result = MessageBox.Show(
                    $"Warning!\n\nSCI fonts are limited to 256 glyphs (0–255).\n\nThe current font contains {Character.Length} glyphs.\n\nAll glyphs beyond 256 will NOT be saved.\n\nContinue?",
                    "SCI Glyph Limit",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.OK)
                    return;

                Array.Resize(ref Character, glyphLimit);
                NumberOfCharacters = glyphLimit;
            }

            binaryWriter.Write((UInt16)0x87);
            binaryWriter.Write((UInt16)0x00);
            binaryWriter.Write((UInt16)Character.Length);

            if (TextHeight == 0)
            {
                binaryWriter.Write((UInt16)9); // assume some value
            }
            else
            {
                binaryWriter.Write((UInt16)TextHeight);
            }

            Int64 offset = binaryWriter.BaseStream.Position;

            foreach (CCharInfo item in Character)
            {
                binaryWriter.Write((UInt16)0);
            }

            UInt16[] positionArray = new UInt16[Character.Length];

            foreach (CCharInfo item in Character)
            {
                if (item == null)
                    continue;

                if (item.ByteLines == null)
                    item.ByteLines = new byte[0];

                positionArray[item.Index] = (UInt16)binaryWriter.BaseStream.Position;

                binaryWriter.Write((byte)item.Width);
                binaryWriter.Write((byte)item.Height);
                binaryWriter.Write(item.ByteLines);

                item.WidthOriginal = item.Width;
                item.HeightOriginal = item.Height;

                item.ByteLinesOriginal = new byte[item.ByteLines.Length];
                Array.Copy(item.ByteLines, item.ByteLinesOriginal, item.ByteLines.Length);
            }

            binaryWriter.BaseStream.Position = offset;

            for (int counter = 0; counter < Character.Length; counter++)
            {
                binaryWriter.Write((UInt16)(positionArray[counter] - 2));
            }
        }
    }

    public static class CFontUtils
    {
        public static void RecreateCharacter(CCharInfo character, UInt16 newwidth, UInt16 newheight)
        {
            // If glyph is empty, just initialize fresh bitmap
            if (character.Width == 0 || character.Height == 0 || character.UnscaledImage == null)
            {
                character.Width = newwidth;
                character.Height = newheight;

                Bitmap blank = new Bitmap(newwidth, newheight, PixelFormat.Format1bppIndexed);
                SaveByteLinesFromPicture(character, blank);

                character.UnscaledImage = blank;
                return;
            }

            Bitmap oldbitmap;
            Bitmap tmpbitmap = new Bitmap(newwidth, newheight);
            Bitmap newbitmap = new Bitmap(newwidth, newheight, PixelFormat.Format1bppIndexed);

            CreateBitmap(character, out oldbitmap);

            Graphics g = Graphics.FromImage(tmpbitmap);
            g.DrawImage(oldbitmap, 0, 0, new Rectangle(0, 0, Math.Min(character.Width, newwidth), Math.Min(character.Height, newheight)), GraphicsUnit.Pixel);

            if ((character.Width - newwidth) < 1)
            {
                g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(character.Width, 0, newwidth, newheight));
            }

            if ((character.Height - newheight) < 1)
            {
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
            if (character.Width == 0 || character.Height == 0)
            {
                bmp = null;
                return;
            }
            else
            {
                bmp = new System.Drawing.Bitmap(character.Width, character.Height, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                IntPtr ptr = bmpData.Scan0;

                Int32 bytesPerLine = (bmpData.Width - 1) / 8 + 1;
                Int32 startPos = 0;

                if (character.ByteLines != null && character.ByteLines.Length > 0)
                {
                    startPos = character.ByteLines.Length / bmpData.Height;

                    for (int heightcounter = 0; heightcounter < bmp.Height; heightcounter++)
                    {
                        if ((startPos * heightcounter) + bytesPerLine <= character.ByteLines.Length)
                        {
                            System.Runtime.InteropServices.Marshal.Copy(
                                character.ByteLines,
                                startPos * heightcounter,
                                ptr,
                                bytesPerLine);

                            ptr = (IntPtr)((int)ptr + bmpData.Stride);
                        }
                    }
                }

                bmp.UnlockBits(bmpData);
            }
        }

        public static void SaveByteLinesFromPicture(CCharInfo character, Bitmap bmp)
        {
            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            Int32 bytesPerLine = (bmpData.Width - 1) / 8 + 1;

            character.ByteLines = new byte[bmp.Height * bytesPerLine];

            for (int heightcounter = 0; heightcounter < bmp.Height; heightcounter++)
            {
                System.Runtime.InteropServices.Marshal.Copy(
                    ptr,
                    character.ByteLines,
                    bytesPerLine * heightcounter,
                    bytesPerLine);

                ptr = (IntPtr)((int)ptr + bmpData.Stride);
            }

            bmp.UnlockBits(bmpData);
        }

        public static void SaveOneFont(CFontInfo font)
        {
            if (font == null)
                return;

            if (font is CWFNFontInfo wfn)
            {
                if (wfn.Character == null)
                    throw new Exception("Cannot save: Character array is null.");

                foreach (CCharInfo item in wfn.Character)
                {
                    if (item == null)
                        throw new Exception("Cannot save: Character array contains null glyph entries.");
                }

                if (wfn.EstimateSize() > 65535)
                {
                    System.Windows.Forms.MessageBox.Show(
                        "Font exceeds 64KB. The WFN format used by AGS does not support files larger than 65,535 bytes.",
                        "Save Error",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);

                    return;
                }
            }

            using (var fs = System.IO.File.Open(font.FontPath, System.IO.FileMode.Create))
            using (var bw = new System.IO.BinaryWriter(fs))
            {
                font.Write(bw);
            }
        }



        public static void ScaleBitmap(Bitmap bitmap, out Bitmap newbmp, Int32 scale)
        {
            if (bitmap == null)
            {
                newbmp = null;
                return;
            }

            newbmp = new Bitmap(bitmap.Width * scale, bitmap.Height * scale);

            using (Graphics g = Graphics.FromImage(newbmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(bitmap, 0, 0, newbmp.Width, newbmp.Height);
            }
        }
    }
}
