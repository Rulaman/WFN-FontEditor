using AGS.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AGS.Plugin.FontEditor
{
    public partial class FontEditorPane : EditorContentPanel
    {
        Settings XmlSettings = new Settings();
        private List<PictureBox> CharacterPictureList = new List<PictureBox>();
        private PictureBox _selectedPreview = null;
        private readonly ToolTip _toolTip = new ToolTip();
        private bool _bulkUpdate = false;
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
        private bool GridFix = false;
        private Point Grid = new Point(-1, -1);
        private Pen GridPen = new Pen(Color.FromArgb(255, 64, 64, 64)); // darker gray
        private List<CFontInfo> EmbeddedFontList = new List<CFontInfo>();
        public event EventHandler OnFontModified;
        private const int MAX_VISIBLE_GLYPHS = 256; // UI safety cap
        private int PageStart = 0;         // index of first glyph shown
        private const int PageSize = 256;  // number of glyphs shown at once

        private enum SizeMode
        {
            ChangeNothing,
            ChangeWidth,
            ChangeHeight,
            ChangeBoth,
        }
        private enum EDirection
        {
            Next,
            Prev,
        }

        static bool ArraysEqual(byte[] a1, byte[] a2)
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

        private void CheckChange()
        {
            MyEventArgs me = new MyEventArgs();
            EventHandler DoChange = OnFontModified;

            foreach (CCharInfo item in FontInfo.Character)
            {
                if (!ArraysEqual(item.ByteLines, item.ByteLinesOriginal)
                    || (item.Width != item.WidthOriginal)
                    || (item.Height != item.HeightOriginal))
                {
                    me.Modified = true;
                    break;
                }
            }

            if (FontModifiedSaved != me.Modified)
            {
                FontModifiedSaved = me.Modified;

                if (null != DoChange)
                {
                    DoChange(this, me);
                }
            }
        }

        internal static string Chr(int codePoint)
        {
            if (codePoint < 0 || codePoint > 0xFFFF)
                throw new ArgumentOutOfRangeException(nameof(codePoint), codePoint, "Must be between 0 and 65535.");

            return char.ConvertFromUtf32(codePoint);
        }

        private void LoadInternalResources()
        {
            string[] sa = typeof(FontEditorPane).Assembly.GetManifestResourceNames();

            foreach (string s in sa)
            {
                if (s.EndsWith(".WFN"))
                {
                    // eigebettete Schrift gefunden
                    using (System.IO.Stream stream = typeof(FontEditorPane).Assembly.GetManifestResourceStream(s))
                    {
                        using (System.IO.BinaryReader br = new System.IO.BinaryReader(stream))
                        {
                            CWFNFontInfo fi = new CWFNFontInfo();
                            fi.Read(br);

                            string[] namearray = System.IO.Path.GetFileNameWithoutExtension(s).Split('.');
                            string name = namearray[namearray.Length - 1];

                            fi.FontName = "Internal" + name;
                            fi.FontPath = "InternalResource";
                            EmbeddedFontList.Add((CFontInfo)fi);
                        }
                    }
                }
            }
        }

        private void BaseConstructor()
        {
            /* ++ only for debugging */
            //string[] sa = typeof(FontEditorPane).Assembly.GetManifestResourceNames();

            //foreach (string s in sa)
            //{
            //	System.Diagnostics.Trace.WriteLine(s);
            //}
            /* -- only for debugging */

            // Font resource names
            //AGS.Plugin.FontEditor.EmbeddedResources.AGSFNT0.WFN
            //AGS.Plugin.FontEditor.EmbeddedResources.AGSFNT1.WFN
            //AGS.Plugin.FontEditor.EmbeddedResources.AGSFNT2.WFN

            XmlSettings.Read();
            InitializeComponent();

            typeof(PictureBox)
                .GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(DrawingArea, true, null);

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            LblZoom.Text = "x" + ZoomDrawingArea.Value;

            numWidth.Maximum = MaxWidth;
            numHeight.Maximum = MaxHeight;

            this.CreateControl();
            LoadInternalResources();
        }
        public CFontInfo CurrentFontInfo
        {
            get { return FontInfo; }
        }

        public FontEditorPane()
        {
            BaseConstructor();
        }
        public FontEditorPane(string filepath, string filename, string fontname)
        {
            BaseConstructor();

            if (System.IO.File.Exists(System.IO.Path.Combine(filepath, filename)))
            {
                if (System.IO.Path.GetExtension(filename).ToLower() == ".wfn")
                {
                    FontInfo = new CWFNFontInfo();
                }
                else if (System.IO.Path.GetFileNameWithoutExtension(filename).ToLower() == "font")
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
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(
                        "Failed to load font:\n\n" + ex.Message,
                        "Load Error",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);

                    FontInfo.Character = new CCharInfo[0];
                }
                finally
                {
                    if (null != binaryReader)
                    {
                        binaryReader.Close();
                    }
                    if (null != file)
                    {
                        file.Close();
                    }
                }
                PageStart = 0;
                RebuildPage();
                UpdateGlyphTextbox();
                TxtGlyphRange.Text = FontInfo.Character.Length.ToString();
                SetGridFix();
            }

            FlowCharacterPanel.BackColor = XmlSettings.Color;
            ChkGrid.Checked = XmlSettings.Grid;
            ChkGridFix.Checked = XmlSettings.GridFix;
            ShowCharacterCount();
        }

        private void ShowCharacterCount()
        {
            if (FontInfo != null && FontInfo.Character != null)
            {
                long sizeBytes = CalculateCurrentWFNSize();
                double sizeKB = sizeBytes / 1024.0;

                GroupBox.Text = $"Font size {sizeKB:0.00} KB / 64 KB";

                if (sizeBytes > 65535)
                {
                    GroupBox.ForeColor = Color.Red;
                    GroupBox.Text = $"Font size {sizeKB:0.00} KB / 64 KB (THIS WON'T SAVE!)";
                }
                else {
                    GroupBox.ForeColor = Color.Black;
                    GroupBox.Text = $"Font size {sizeKB:0.00} KB / 64 KB";
                }
            }
            else
            {
                GroupBox.Text = "Selected font settings";
                GroupBox.ForeColor = Color.Black;
            }
        }


        private void RebuildPage()
        {
            if (FontInfo == null || FontInfo.Character == null)
                return;

            FlowCharacterPanel.SuspendLayout();

            try
            {
                FlowCharacterPanel.Controls.Clear();
                CharacterPictureList.Clear();

                int end = Math.Min(PageStart + PageSize, FontInfo.Character.Length);

                for (int i = PageStart; i < end; i++)
                {
                    AddCharacterToList(FontInfo.Character[i]);
                }
            }
            finally
            {
                FlowCharacterPanel.ResumeLayout();
            }

            ShowCharacterCount();
            UpdatePageButtons();
            
            int localIndex = Index - PageStart;

            if (localIndex >= 0 && localIndex < CharacterPictureList.Count)
            {
                Character_Click(CharacterPictureList[localIndex],
                    new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            }
        }

        private void AddCharacterToList(CCharInfo item)
        {
            Bitmap bitmap = null;
            PictureBox pict = new PictureBox();
            CharacterPictureList.Add(pict);

            pict.Tag = item;
            pict.Width = item.Width * Scalefactor;
            pict.Height = item.Height * Scalefactor;
            pict.Click += new EventHandler(Character_Click);
            pict.ContextMenu = new ContextMenu();

            _toolTip.SetToolTip(pict, Chr(item.Index));

            pict.ContextMenu.Tag = pict;
            pict.ContextMenu.MenuItems.Add("Undo").Click += new EventHandler(MenuUndoClicked);
            pict.ContextMenu.MenuItems.Add("Redo").Click += new EventHandler(MenuRedoClicked);
            pict.ContextMenu.MenuItems.Add("Copy").Click += new EventHandler(MenuCopyClicked);
            pict.ContextMenu.MenuItems.Add("Paste").Click += new EventHandler(MenuPasteClicked);
            pict.ContextMenu.MenuItems.Add("-");
            pict.ContextMenu.MenuItems.Add("Remove glyph data").Click += new EventHandler(MenuRemoveGlyphDataClicked);
            pict.ContextMenu.MenuItems[0].Enabled = false;
            pict.ContextMenu.MenuItems[1].Enabled = false;

            // If glyph has no size yet, show placeholder
            if (item.Width == 0 || item.Height == 0)
            {
                // Small placeholder preview size
                int previewW = 8 * Scalefactor;
                int previewH = 9 * Scalefactor;

                Bitmap placeholder = new Bitmap(previewW, previewH);
                using (Graphics g = Graphics.FromImage(placeholder))
                {
                    // Fill placeholder background to the same tone of panel
                    g.Clear(XmlSettings.Color);

                    // White stroke border
                    using (Pen pen = new Pen(Color.White))
                    {
                        g.DrawRectangle(pen, 0, 0, previewW - 1, previewH - 1);
                    }
                }

                pict.Image = placeholder;
                pict.Width = previewW;
                pict.Height = previewH;

                item.UnscaledImage = null;
            }
            else
            {
                CFontUtils.CreateBitmap(item, out bitmap);

                item.UnscaledImage = bitmap;
                item.UndoRedoListAdd(item.ByteLines);

                Bitmap outbmp;
                CFontUtils.ScaleBitmap(bitmap, out outbmp, Scalefactor);
                pict.Image = outbmp;

                pict.Width = outbmp.Width;
                pict.Height = outbmp.Height;
            }
            
            pict.Paint += CharacterPreview_Paint;
            FlowCharacterPanel.Controls.Add(pict);
        }

        private bool CorrectImage(Bitmap original, out Bitmap corrected)
        {

            BitmapData bmpData = original.LockBits(new Rectangle(0, 0, original.Width, original.Height), ImageLockMode.ReadWrite, original.PixelFormat);

            if (bmpData.Stride < 0)
            {
                corrected = new Bitmap(original.Width, original.Height, original.PixelFormat);
                BitmapData cbmpdata = corrected.LockBits(new Rectangle(0, 0, corrected.Width, corrected.Height), ImageLockMode.ReadWrite, corrected.PixelFormat);

                IntPtr ptroriginal = bmpData.Scan0;
                IntPtr ptrorrected = cbmpdata.Scan0;
                byte[] array = new byte[Math.Abs(bmpData.Stride)];

                for (int cnt = 0; cnt < original.Height; cnt++)
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

        void MenuUndoClicked(object sender, EventArgs e)
        {
            MenuItem menu = (MenuItem)sender;

            if (menu != null)
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

                CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
                CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

                CreateAndShow(character, SizeMode.ChangeBoth);
                CheckChange();
            }
        }
        void MenuRedoClicked(object sender, EventArgs e)
        {
            MenuItem menu = (MenuItem)sender;

            if (menu != null)
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

                CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
                CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

                CreateAndShow(character, SizeMode.ChangeBoth);
                CheckChange();
            }
        }
        void MenuCopyClicked(object sender, EventArgs e)
        {
            MenuItem menu = (MenuItem)sender;

            if (menu != null)
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
            if (menu == null)
                return;

            PictureBox picture = (PictureBox)menu.Parent.Tag;
            if (picture == null)
                return;

            CCharInfo characterinfo = (CCharInfo)(picture.Tag);
            if (characterinfo == null)
                return;

            // Grab clipboard image safely
            Image clipImg = Clipboard.GetImage();
            if (clipImg == null)
                return;

            Bitmap clipBmp = clipImg as Bitmap;
            if (clipBmp == null)
                clipBmp = new Bitmap(clipImg); // convert Image -> Bitmap

            // Preserve old tag if existed (placeholders may have null UnscaledImage)
            object oldTag = null;
            if (characterinfo.UnscaledImage != null)
                oldTag = characterinfo.UnscaledImage.Tag;

            // Convert clipboard bitmap to 1bpp
            Bitmap untested = Indexed.Image.CopyToBpp(clipBmp, 1);

            // Fix upside-down / stride issues if needed
            Bitmap corrected = null;
            if (CorrectImage(untested, out corrected))
                characterinfo.UnscaledImage = corrected;
            else
                characterinfo.UnscaledImage = untested;

            // Restore tag if we had one
            if (oldTag != null)
                characterinfo.UnscaledImage.Tag = oldTag;

            // Update glyph metrics based on pasted bitmap
            characterinfo.Width = (UInt16)characterinfo.UnscaledImage.Width;
            characterinfo.Height = (UInt16)characterinfo.UnscaledImage.Height;

            // Save ByteLines from the pasted image (allocates ByteLines)
            CFontUtils.SaveByteLinesFromPicture(characterinfo, (Bitmap)characterinfo.UnscaledImage);

            // Update selection/UI
            Index = characterinfo.Index;
            UpdateGlyphTextbox();

            // Update drawing area
            Bitmap scaledbitmap;
            CFontUtils.ScaleBitmap((Bitmap)characterinfo.UnscaledImage, out scaledbitmap, ZoomDrawingArea.Value);
            if (scaledbitmap != null)
            {
                DrawingArea.Size = scaledbitmap.Size;
                DrawingArea.Image = scaledbitmap;
            }

            ClickedOnCharacter = true;
            numWidth.Value = characterinfo.Width;
            numHeight.Value = characterinfo.Height;
            ClickedOnCharacter = false;

            // Update preview cell
            int localIndex = characterinfo.Index - PageStart;
            if (localIndex >= 0 && localIndex < CharacterPictureList.Count)
            {
                Bitmap outbmp;
                CFontUtils.ScaleBitmap((Bitmap)characterinfo.UnscaledImage, out outbmp, Scalefactor);
                CharacterPictureList[localIndex].Image = outbmp;
                CharacterPictureList[localIndex].Size = outbmp.Size;
                CharacterPictureList[localIndex].Invalidate();
            }

            // Undo/redo state
            characterinfo.UndoRedoListTidyUp();
            characterinfo.UndoRedoListAdd(characterinfo.ByteLines);

            if (localIndex >= 0 && localIndex < CharacterPictureList.Count)
            {
                CharacterPictureList[localIndex].ContextMenu.MenuItems[0].Enabled = characterinfo.UndoPossible;
                CharacterPictureList[localIndex].ContextMenu.MenuItems[1].Enabled = characterinfo.RedoPossible;
            }

            CheckChange();
        }


        private void MenuRemoveGlyphDataClicked(object sender, EventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            if (menu == null)
                return;

            PictureBox pict = menu.Parent.Tag as PictureBox;
            if (pict == null)
                return;

            CCharInfo character = pict.Tag as CCharInfo;
            if (character == null)
                return;

            // 🔴 Convert glyph to placeholder (no size, no bitmap)
            character.Width = 0;
            character.Height = 0;
            character.WidthOriginal = 0;
            character.HeightOriginal = 0;
            character.ByteLines = null;
            character.ByteLinesOriginal = null;
            character.UnscaledImage = null;

            // 🔵 Recreate ONLY this preview box
            int previewW = 8 * Scalefactor;
            int previewH = 9 * Scalefactor;

            Bitmap placeholder = new Bitmap(previewW, previewH);
            using (Graphics g = Graphics.FromImage(placeholder))
            {
                g.Clear(XmlSettings.Color);

                using (Pen pen = new Pen(Color.White))
                {
                    g.DrawRectangle(pen, 0, 0, previewW - 1, previewH - 1);
                }
            }

            pict.Image = placeholder;
            pict.Width = previewW;
            pict.Height = previewH;
            pict.Invalidate();

            // ✅ If this glyph is currently selected → refresh drawing area
            if (character.Index == Index)
            {
                DisplayCurrentCharacter(character);
            }

            // 🔵 Make this glyph the active selection
            Index = character.Index;
            UpdateGlyphTextbox();

            // Visually select it (same behavior as left-click)
            Character_Click(pict, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

            CheckChange();
        }


        void PrevNext(EDirection direction)
        {
            switch (direction)
            {
                case EDirection.Next:
                    {
                        Index++;
                    }
                    break;
                case EDirection.Prev:
                    {
                        Index--;
                    }
                    break;
            }
            ;

            foreach (PictureBox item in CharacterPictureList)
            {
                CCharInfo characterinfo = (CCharInfo)(item.Tag);

                if (characterinfo.Index == Index)
                {
                    DisplayCurrentCharacter(characterinfo);
                }
            }
        }

        void Character_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouse = (MouseEventArgs)e;
            PictureBox picture = (PictureBox)sender;
            CCharInfo characterinfo = (CCharInfo)(picture.Tag);
            ContextMenu menu = (ContextMenu)picture.ContextMenu;

            // Remove highlight from previously selected preview
            if (_selectedPreview != null)
            {
                _selectedPreview.BackColor = FlowCharacterPanel.BackColor;
            }

            // Highlight current preview
            _selectedPreview = picture;
            _selectedPreview.BackColor = Color.FromArgb(80, 120, 200); // subtle blue

            switch (mouse.Button)
            {
                case MouseButtons.Left:
                    DisplayCurrentCharacter(characterinfo);
                    break;

                case MouseButtons.Right:
                    menu.MenuItems[0].Enabled = characterinfo.UndoPossible;
                    menu.MenuItems[1].Enabled = characterinfo.RedoPossible;
                    break;
            }
        }


        private void DisplayCurrentCharacter(CCharInfo characterinfo)
        {
            if (characterinfo == null)
                return;

            Index = characterinfo.Index;
            UpdateGlyphTextbox();

            LblCharacter.Text = Convert.ToString(Index) + "   " + "0x" + Convert.ToString(Index, 16) + "   " + Chr(Index);
            TxtCharacter.Text = Convert.ToString(Index);

            // Keep your numeric controls in sync, but do NOT force placeholder to become a real glyph.
            ClickedOnCharacter = true;

            // If placeholder, show default size in UI, but do not write it into the glyph.
            if (characterinfo.Width == 0 || characterinfo.Height == 0)
            {
                numWidth.Value = PLACEHOLDER_W;
                numHeight.Value = PLACEHOLDER_H;
            }
            else
            {
                numWidth.Value = characterinfo.Width;
                numHeight.Value = characterinfo.Height;
            }

            ClickedOnCharacter = false;

            // ✅ This is the key: just refresh the drawing area view (temp bitmap if placeholder).
            RefreshDrawingAreaDisplay(characterinfo);

            // Optional: grid overlay happens in Paint/Mouse logic; don't call PaintOnDrawingArea here.

            int localIndex = Index - PageStart;
            if (localIndex >= 0 && localIndex < CharacterPictureList.Count)
                SetSelectedPreview(CharacterPictureList[localIndex]);

            PaintOnDrawingArea(DrawingArea, null);
        }

        private void ZoomDrawingArea_ValueChanged(object sender, EventArgs e)
        {
            int localIndex = Index - PageStart;
            if (localIndex < 0 || localIndex >= CharacterPictureList.Count)
                return;

            CCharInfo characterinfo = (CCharInfo)CharacterPictureList[localIndex].Tag;

            RefreshDrawingAreaDisplay(characterinfo);

            LblZoom.Text = "x" + ZoomDrawingArea.Value;
        }

        private void PaintOnDrawingArea(object sender, EventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;

            Graphics graphics = ((PictureBox)sender).CreateGraphics();
            Int32 zoom = ZoomDrawingArea.Value;
            Color col = Color.Gray;

            int localIndex = Index - PageStart;

            // Guard against invalid page/index combination
            if (localIndex < 0 || localIndex >= CharacterPictureList.Count)
            {
                graphics.Dispose();
                return;
            }

            CCharInfo character =
                (CCharInfo)CharacterPictureList[localIndex].Tag;

            // 🔴 INITIALIZE EMPTY GLYPH IF NEEDED
            // Do NOT initialize placeholder glyphs unless the user is actually drawing
            Bitmap Selected = character.UnscaledImage as Bitmap;

            bool isUserDrawing = (mouse != null && mouse.Button != MouseButtons.None);

            if (isUserDrawing)
            {
                // If placeholder, materialize it NOW (user started drawing)
                if (character.Width == 0 || character.Height == 0 || Selected == null)
                {
                    character.Width = 8;
                    character.Height = 9;
                    character.WidthOriginal = 8;
                    character.HeightOriginal = 9;

                    int bytesPerLine = (character.Width - 1) / 8 + 1;
                    character.ByteLines = new byte[character.Height * bytesPerLine];

                    Bitmap newBmp;
                    CFontUtils.CreateBitmap(character, out newBmp);
                    character.UnscaledImage = newBmp;
                    Selected = newBmp;

                    // Update preview image immediately
                    Bitmap scaled;
                    CFontUtils.ScaleBitmap(newBmp, out scaled, Scalefactor);
                    CharacterPictureList[localIndex].Image = scaled;
                    CharacterPictureList[localIndex].Invalidate();
                }
            }

            // If still placeholder and not drawing: just exit (keep placeholder!)
            if (Selected == null)
            {
                graphics.Dispose();
                return;
            }

            if (mouse != null && mouse.Button != MouseButtons.None)
            {
                if (((mouse.X / zoom) < Selected.Width) &&
                    ((mouse.Y / zoom) < Selected.Height))
                {
                    SolidBrush brush = new SolidBrush(Color.Gray);

                    BitmapData bmpData = Selected.LockBits(
                        new Rectangle(0, 0, Selected.Width, Selected.Height),
                        ImageLockMode.ReadWrite,
                        Selected.PixelFormat);

                    IntPtr ptr = bmpData.Scan0;
                    ptr = (IntPtr)((int)ptr + bmpData.Stride * (mouse.Y / zoom));

                    byte[] b = new byte[bmpData.Stride];
                    System.Runtime.InteropServices.Marshal.Copy(ptr, b, 0, bmpData.Stride);
                    Array.Reverse(b);

                    UInt32 line = BitConverter.ToUInt32(b, 0);

                    switch (mouse.Button)
                    {
                        case MouseButtons.Left:
                            col = PanelLeftMouse.BackColor;
                            break;

                        case MouseButtons.Right:
                            col = PanelRightMouse.BackColor;
                            break;

                        case MouseButtons.Middle:
                            col = ((line >> (mouse.X / zoom)) > 0)
                                ? PanelLeftMouse.BackColor
                                : PanelRightMouse.BackColor;
                            break;
                    }

                    if (Color.Black == col)
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

                    System.Runtime.InteropServices.Marshal.Copy(b, 0, ptr, bmpData.Stride);
                    Selected.UnlockBits(bmpData);

                    graphics.FillRectangle(
                        brush,
                        new Rectangle(
                            (mouse.X / zoom) * zoom,
                            (mouse.Y / zoom) * zoom,
                            zoom,
                            zoom));

                    brush.Dispose();

                    CFontUtils.SaveByteLinesFromPicture(character, Selected);
                    character.UnscaledImage = Selected;

                    Bitmap outbmp;
                    CFontUtils.ScaleBitmap(Selected, out outbmp, Scalefactor);
                    CharacterPictureList[localIndex].Image = outbmp;
                    CharacterPictureList[localIndex].Invalidate();
                }
            }

            if (ShowGrid)
            {
                for (int xcnt = 0; xcnt < Selected.Width; xcnt++)
                {
                    for (int ycnt = 0; ycnt < Selected.Height; ycnt++)
                    {
                        graphics.DrawRectangle(
                            GridPen,
                            xcnt * zoom,
                            ycnt * zoom,
                            zoom,
                            zoom);
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

            if (bInEdit)
            {
                if (((e.X / zoom) - EditPoint.X != 0) || ((e.Y / zoom) - EditPoint.Y != 0))
                {
                    EditPoint.X = e.X / zoom;
                    EditPoint.Y = e.Y / zoom;

                    PaintOnDrawingArea(sender, e);
                }
            }
        }
        private void DrawingArea_MouseUp(object sender, MouseEventArgs e)
        {
            bInEdit = false;
            CheckChange();

            // If no glyphs exist, do nothing
            if (FontInfo == null || FontInfo.Character == null)
                return;

            if (FontInfo.Character.Length == 0)
                return;

            int localIndex = Index - PageStart;

            if (localIndex < 0 || localIndex >= CharacterPictureList.Count)
                return;

            CCharInfo characterinfo =
                (CCharInfo)(CharacterPictureList[localIndex].Tag);

            characterinfo.UndoRedoListTidyUp();
            characterinfo.UndoRedoListAdd(characterinfo.ByteLines);

            if (CharacterPictureList[localIndex].ContextMenu != null)
            {
                CharacterPictureList[localIndex]
                    .ContextMenu.MenuItems[0].Enabled = characterinfo.UndoPossible;

                CharacterPictureList[localIndex]
                    .ContextMenu.MenuItems[1].Enabled = characterinfo.RedoPossible;
            }
        }
        private void DrawingArea_Paint(object sender, PaintEventArgs e)
        {
            int localIndex = Index - PageStart;
            if (localIndex < 0 || localIndex >= CharacterPictureList.Count)
                return;

            CCharInfo character = (CCharInfo)CharacterPictureList[localIndex].Tag;

            Bitmap src = GetDrawingAreaBitmapForGlyph(character);

            Bitmap scaled;
            CFontUtils.ScaleBitmap(src, out scaled, ZoomDrawingArea.Value);
            if (scaled == null)
                return;

            e.Graphics.DrawImage(scaled, 0, 0);

            if (ShowGrid && character != null)
            {
                // For placeholders, grid should match the placeholder size (8x9),
                // not 0x0.
                int w = (character.Width > 0) ? character.Width : PLACEHOLDER_W;
                int h = (character.Height > 0) ? character.Height : PLACEHOLDER_H;

                Int32 zoom = ZoomDrawingArea.Value;
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        e.Graphics.DrawRectangle(GridPen, x * zoom, y * zoom, zoom, zoom);
                    }
                }
            }
        }

        private void SetGridFix()
        {
            FlowCharacterPanel.SuspendLayout();

            try
            {
                if (GridFix)
                {
                    int maxwidth = 0;

                    foreach (PictureBox item in CharacterPictureList)
                    {
                        maxwidth = Math.Max(item.Width, maxwidth);
                    }

                    foreach (PictureBox item in CharacterPictureList)
                    {
                        item.Margin = new Padding(3, 3, maxwidth + 3 - item.Width, 3);
                    }
                }
                else
                {
                    foreach (PictureBox item in CharacterPictureList)
                    {
                        item.Margin = new Padding(3, 3, 3, 3);
                    }
                }
            }
            finally
            {
                FlowCharacterPanel.ResumeLayout();
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
            if (!ClickedOnCharacter)
            {
                CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index - PageStart]).Tag));
                CreateAndShow(character, SizeMode.ChangeBoth);
            }

            SetGridFix();
            CheckChange();
        }

        private void CreateAndShow(CCharInfo character, SizeMode sizeMode)
        {
            switch (sizeMode)
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
            }
            ;

            Bitmap bitmap = null;

            CFontUtils.CreateBitmap(character, out bitmap);
            character.UnscaledImage = bitmap;

            Bitmap outbmp;
            CFontUtils.ScaleBitmap(bitmap, out outbmp, Scalefactor);
            CharacterPictureList[character.Index - PageStart].Size = outbmp.Size;
            CharacterPictureList[character.Index - PageStart].Image = outbmp;

            _toolTip.SetToolTip(CharacterPictureList[character.Index - PageStart], Chr(character.Index));

            Bitmap drawingbitmap;
            CFontUtils.ScaleBitmap(bitmap, out drawingbitmap, ZoomDrawingArea.Value);
            DrawingArea.Size = drawingbitmap.Size;
            DrawingArea.Image = drawingbitmap;
        }

        private void numWidth_ValueChanged(object sender, EventArgs e)
        {
            if (numWidth.Value > MaxWidth)
            {
                numWidth.Value = MaxWidth;
            }
            if (numWidth.Value < 1)
            {
                numWidth.Value = 1;
            }

            AdjustSize();
        }
        private void numHeight_ValueChanged(object sender, EventArgs e)
        {
            if (numHeight.Value > MaxHeight)
            {
                numHeight.Value = MaxHeight;
            }
            if (numHeight.Value < 1)
            {
                numHeight.Value = 1;
            }

            AdjustSize();
        }

        private void ChkGrid_CheckedChanged(object sender, EventArgs e)
        {
            ShowGrid = ChkGrid.Checked;
            Character_Click(CharacterPictureList[Index - PageStart], new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            XmlSettings.Grid = ShowGrid;
            XmlSettings.Write();
        }
        private void ChkGridFix_CheckedChanged(object sender, EventArgs e)
        {
            GridFix = ChkGridFix.Checked;

            SetGridFix();

            XmlSettings.GridFix = GridFix;
            XmlSettings.Write();
        }

        private Image RenderTextLine(string text)
        {
            Int32 xpos = 0;
            Int32 widthpreliminary = 0;
            Int32 heightpreliminary = 0;
            Image bmp;

            try
            {
                foreach (char c in text)
                {
                    widthpreliminary += FontInfo.Character[c].Width;
                    heightpreliminary = Math.Max(heightpreliminary, FontInfo.Character[c].Height);
                }

                bmp = new Bitmap(widthpreliminary + 4, heightpreliminary + 4);
                Graphics g = Graphics.FromImage(bmp);

                g.FillRectangle(new SolidBrush(Color.Gray), 0, 0, bmp.Width, bmp.Height);

                foreach (char c in text)
                {
                    g.DrawImageUnscaled(FontInfo.Character[c].UnscaledImage, xpos + 2, 2);
                    xpos += FontInfo.Character[c].Width;
                }

                g.Dispose();
            }
            catch
            {
                bmp = new Bitmap(600, 20);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawString("No rendering possible. Not enough characters in the font, or another problem!", new System.Drawing.Font("Arial", 12), new SolidBrush(Color.Black), 2, 2);
                g.Dispose();
            }

            return bmp;
        }

        private void FlowCharacterPanel_Click(object sender, EventArgs e)
        {
            MouseEventArgs events = (MouseEventArgs)e;

            switch (events.Button)
            {
                case MouseButtons.Right:
                    {
                        ColorDialog cd = new ColorDialog();

                        cd.Color = FlowCharacterPanel.BackColor;

                        if (cd.ShowDialog() == DialogResult.OK)
                        {
                            FlowCharacterPanel.BackColor = cd.Color;
                            RefreshPlaceholders();
                        }

                        XmlSettings.Color = cd.Color;
                        XmlSettings.Write();
                    }
                    break;
                default:
                    {
                    }
                    break;
            }
            ;
        }
        private void PictSwap_Click(object sender, EventArgs e)
        {
            Color temp = PanelRightMouse.BackColor;
            PanelRightMouse.BackColor = PanelLeftMouse.BackColor;
            PanelLeftMouse.BackColor = temp;
        }

        #region Character functions
        private void ShiftUpCharacter(CCharInfo character)
        {
            // Ignore placeholder glyphs
            if (character.Width == 0 ||
                character.Height == 0 ||
                character.UnscaledImage == null)
            {
                return;
            }

            //CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index - PageStart]).Tag));
            Bitmap Selected;

            if (null != (Selected = (Bitmap)(character.UnscaledImage)))
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

                CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag), Selected);
                ((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag)).UnscaledImage = Selected;
            }

            character.UndoRedoListTidyUp();
            character.UndoRedoListAdd(character.ByteLines);

            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

            CreateAndShow(character, SizeMode.ChangeNothing);
            CheckChange();
        }
        private void ShiftDownCharacter(CCharInfo character)
        {
            // Ignore placeholder glyphs
            if (character.Width == 0 ||
                character.Height == 0 ||
                character.UnscaledImage == null)
            {
                return;
            }

            //CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index - PageStart]).Tag));
            Bitmap Selected;

            if (null != (Selected = (Bitmap)(character.UnscaledImage)))
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

                CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag), Selected);
                ((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag)).UnscaledImage = Selected;
            }

            character.UndoRedoListTidyUp();
            character.UndoRedoListAdd(character.ByteLines);

            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

            CreateAndShow(character, SizeMode.ChangeNothing);
            CheckChange();
        }
        private void ShiftLeftCharacter(CCharInfo character)
        {
            // Ignore placeholder glyphs
            if (character.Width == 0 ||
                character.Height == 0 ||
                character.UnscaledImage == null)
            {
                return;
            }

            //CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index - PageStart]).Tag));
            Bitmap Selected;

            if (null != (Selected = (Bitmap)(character.UnscaledImage)))
            {
                BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
                IntPtr ptrbegin = bmpData.Scan0;
                IntPtr ptrwrite = bmpData.Scan0;
                byte[] linearray = new byte[bmpData.Stride];
                ptrbegin = bmpData.Scan0;
                UInt32 line;
                UInt32 overflow;

                for (int cnt = 0; cnt < Selected.Height; cnt++)
                {
                    System.Runtime.InteropServices.Marshal.Copy(ptrbegin, linearray, 0, bmpData.Stride);
                    Array.Reverse(linearray);
                    line = BitConverter.ToUInt32(linearray, 0);

                    overflow = (line & 0x80000000) >> (Selected.Width - 1);
                    line <<= 1;
                    line += overflow;

                    linearray = BitConverter.GetBytes(line);
                    Array.Reverse(linearray);
                    System.Runtime.InteropServices.Marshal.Copy(linearray, 0, ptrwrite, bmpData.Stride);

                    ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
                    ptrwrite = (IntPtr)(((int)ptrwrite + bmpData.Stride));
                }

                Selected.UnlockBits(bmpData);

                CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag), Selected);
                ((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag)).UnscaledImage = Selected;
            }

            character.UndoRedoListTidyUp();
            character.UndoRedoListAdd(character.ByteLines);

            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

            CreateAndShow(character, SizeMode.ChangeNothing);
            CheckChange();
        }
        private void ShiftRightCharacter(CCharInfo character)
        {
            // Ignore placeholder glyphs
            if (character.Width == 0 ||
                character.Height == 0 ||
                character.UnscaledImage == null)
            {
                return;
            }

            //CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index - PageStart]).Tag));
            Bitmap Selected;

            if (null != (Selected = (Bitmap)(character.UnscaledImage)))
            {
                BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
                IntPtr ptrbegin = bmpData.Scan0;
                IntPtr ptrwrite = bmpData.Scan0;
                byte[] linearray = new byte[bmpData.Stride];
                ptrbegin = bmpData.Scan0;
                UInt32 line;
                UInt32 underflow;

                for (int cnt = 0; cnt < Selected.Height; cnt++)
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

                CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag), Selected);
                ((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag)).UnscaledImage = Selected;
            }

            character.UndoRedoListTidyUp();
            character.UndoRedoListAdd(character.ByteLines);

            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

            CreateAndShow(character, SizeMode.ChangeNothing);
            CheckChange();
        }
        private void InvertCharacter(CCharInfo character)
        {
            if (character.UnscaledImage == null)
                return;

            //CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index - PageStart]).Tag));
            Bitmap Selected;

            if (null != (Selected = (Bitmap)(character.UnscaledImage)))
            {
                BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
                IntPtr ptrbegin = bmpData.Scan0;
                IntPtr ptrwrite = bmpData.Scan0;
                byte[] linearray = new byte[bmpData.Stride];
                UInt32 line;

                for (int cnt = 0; cnt < Selected.Height; cnt++)
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

                CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag), Selected);
                ((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag)).UnscaledImage = Selected;
            }

            ClickedOnCharacter = true;
            numWidth.Value = character.Width;
            numHeight.Value = character.Height;
            ClickedOnCharacter = false;

            character.UndoRedoListTidyUp();
            character.UndoRedoListAdd(character.ByteLines);

            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

            CreateAndShow(character, SizeMode.ChangeNothing);
            CheckChange();
        }
        private void SwapHorizontallyCharacter(CCharInfo character)
        {
            if (character.UnscaledImage == null)
                return;

            //CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index - PageStart]).Tag));
            Bitmap Selected;

            if (null != (Selected = (Bitmap)(character.UnscaledImage)))
            {
                BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
                IntPtr ptrbegin = bmpData.Scan0;
                byte[][] linearray = new byte[Selected.Height][];

                for (int cnt = 0; cnt < Selected.Height; cnt++)
                {
                    linearray[cnt] = new byte[bmpData.Stride];
                    System.Runtime.InteropServices.Marshal.Copy(ptrbegin, linearray[cnt], 0, bmpData.Stride);
                    ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
                }

                ptrbegin = bmpData.Scan0;

                for (int cnt2 = 0; cnt2 < Selected.Height; cnt2++)
                {
                    System.Runtime.InteropServices.Marshal.Copy(linearray[Selected.Height - 1 - cnt2], 0, ptrbegin, bmpData.Stride);

                    ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
                }

                Selected.UnlockBits(bmpData);

                CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag), Selected);
                ((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag)).UnscaledImage = Selected;
            }

            character.UndoRedoListTidyUp();
            character.UndoRedoListAdd(character.ByteLines);

            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

            CreateAndShow(character, SizeMode.ChangeNothing);
            CheckChange();
        }
        private void SwapVerticallyCharacter(CCharInfo character)
        {
            if (character.UnscaledImage == null)
                return;

            //CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index - PageStart]).Tag));
            Bitmap Selected;

            if (null != (Selected = (Bitmap)(character.UnscaledImage)))
            {
                BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
                IntPtr ptrbegin = bmpData.Scan0;
                IntPtr ptrwrite = bmpData.Scan0;
                byte[] linearray = new byte[bmpData.Stride];
                UInt32 line;

                for (int cnt = 0; cnt < Selected.Height; cnt++)
                {
                    System.Runtime.InteropServices.Marshal.Copy(ptrbegin, linearray, 0, bmpData.Stride);
                    Array.Reverse(linearray);
                    line = BitConverter.ToUInt32(linearray, 0);

                    UInt32 temp = 0;
                    UInt32 temp2 = 0;

                    for (int cnti = 0; cnti < 32; cnti++)
                    {
                        temp2 = line >> (32 - (cnti + 1));
                        temp |= (temp2 & 1) << cnti;
                    }

                    line = temp << (32 - Selected.Width);

                    linearray = BitConverter.GetBytes(line);
                    Array.Reverse(linearray);
                    System.Runtime.InteropServices.Marshal.Copy(linearray, 0, ptrwrite, bmpData.Stride);

                    ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
                    ptrwrite = (IntPtr)(((int)ptrwrite + bmpData.Stride));
                }

                Selected.UnlockBits(bmpData);

                CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag), Selected);
                ((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag)).UnscaledImage = Selected;
            }

            character.UndoRedoListTidyUp();
            character.UndoRedoListAdd(character.ByteLines);

            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

            CreateAndShow(character, SizeMode.ChangeNothing);
            CheckChange();
        }
        private void ClearCharacter(CCharInfo character)
        {
            // 🟢 If placeholder, materialize it first
            if (character.Width == 0 || character.Height == 0 || character.UnscaledImage == null)
            {
                character.Width = 8;
                character.Height = 9;
                character.WidthOriginal = 8;
                character.HeightOriginal = 9;

                int bytesPerLine = (character.Width - 1) / 8 + 1;
                character.ByteLines = new byte[character.Height * bytesPerLine];
            }

            // Ensure ByteLines exists
            if (character.ByteLines == null)
            {
                int bytesPerLine = (character.Width - 1) / 8 + 1;
                character.ByteLines = new byte[character.Height * bytesPerLine];
            }

            // Clear bitmap data (all black)
            for (int i = 0; i < character.ByteLines.Length; i++)
                character.ByteLines[i] = 0x00;

            character.UndoRedoListTidyUp();
            character.UndoRedoListAdd(character.ByteLines);

            CreateAndShow(character, SizeMode.ChangeNothing);
            CheckChange();
        }
        private void FillCharacter(CCharInfo character)
        {
            // If placeholder → materialize first
            if (character.Width == 0 || character.Height == 0 || character.UnscaledImage == null)
            {
                character.Width = 8;
                character.Height = 9;
                character.WidthOriginal = 8;
                character.HeightOriginal = 9;

                int bytesPerLine = (character.Width - 1) / 8 + 1;
                character.ByteLines = new byte[character.Height * bytesPerLine];

                Bitmap newBmp;
                CFontUtils.CreateBitmap(character, out newBmp);
                character.UnscaledImage = newBmp;
            }

            // Now safe to fill
            if (character.ByteLines == null)
            {
                int bytesPerLine = (character.Width - 1) / 8 + 1;
                character.ByteLines = new byte[character.Height * bytesPerLine];
            }

            for (int i = 0; i < character.ByteLines.Length; i++)
            {
                character.ByteLines[i] = 0xFF;
            }

            character.UndoRedoListTidyUp();
            character.UndoRedoListAdd(character.ByteLines);

            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

            CreateAndShow(character, SizeMode.ChangeNothing);
            CheckChange();
        }

        private void OutlineCharacter(CCharInfo character)
        {
            if (character.UnscaledImage == null)
                return;

            //CCharInfo character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index - PageStart]).Tag));
            Bitmap Selected;

            if (null != (Selected = (Bitmap)(character.UnscaledImage)))
            {
                BitmapData bmpData = Selected.LockBits(new Rectangle(0, 0, Selected.Width, Selected.Height), ImageLockMode.ReadWrite, Selected.PixelFormat);
                IntPtr ptrbegin = bmpData.Scan0;
                IntPtr ptrwrite = bmpData.Scan0;
                byte[] linearray = new byte[bmpData.Stride];
                UInt32 line;
                UInt32 temp2;

                bool[,] bitarray = new bool[bmpData.Stride * 8, bmpData.Height];
                bool[,] bitarraynew = new bool[bmpData.Stride * 8, bmpData.Height];

                for (int ycnt = 0; ycnt < Selected.Height; ycnt++)
                {
                    System.Runtime.InteropServices.Marshal.Copy(ptrbegin, linearray, 0, bmpData.Stride);
                    Array.Reverse(linearray);
                    line = BitConverter.ToUInt32(linearray, 0);

                    for (int cnti = 0; cnti < MaxWidth; cnti++)
                    {
                        temp2 = line >> (MaxWidth - (cnti + 1));

                        if ((temp2 & 1) > 0)
                        {
                            bitarray[cnti, ycnt] = true;
                        }
                    }

                    ptrbegin = (IntPtr)(((int)ptrbegin + bmpData.Stride));
                }

                for (int cnt = 0; cnt < Selected.Height; cnt++)
                {
                    for (int innercnt = 0; innercnt < MaxWidth; innercnt++)
                    {
                        if (bitarray[innercnt, cnt])
                        {
                            /* set four pixel in the corner, if they don't set */
                            if (((innercnt - 1) >= 0) && ((cnt - 1) >= 0)) { bitarraynew[innercnt - 1, cnt - 1] = bitarray[innercnt - 1, cnt - 1] == true ? false : true; }
                            if (((innercnt - 1) >= 0) && ((cnt + 1) < bmpData.Height)) { bitarraynew[innercnt - 1, cnt + 1] = bitarray[innercnt - 1, cnt + 1] == true ? false : true; }
                            if (((innercnt + 1) < (bmpData.Stride * 8)) && ((cnt - 1) >= 0)) { bitarraynew[innercnt + 1, cnt - 1] = bitarray[innercnt + 1, cnt - 1] == true ? false : true; }
                            if (((innercnt + 1) < (bmpData.Stride * 8)) && ((cnt + 1) < bmpData.Height)) { bitarraynew[innercnt + 1, cnt + 1] = bitarray[innercnt + 1, cnt + 1] == true ? false : true; }

                            /* set the pixel left and right */
                            if (((innercnt - 1) >= 0)) { bitarraynew[innercnt - 1, cnt] = bitarray[innercnt - 1, cnt] == true ? false : true; }
                            if (((innercnt + 1) < (bmpData.Stride * 8))) { bitarraynew[innercnt + 1, cnt] = bitarray[innercnt + 1, cnt] == true ? false : true; }

                            /* set the pixel upper and lower */
                            if (((cnt - 1) >= 0)) { bitarraynew[innercnt, cnt - 1] = bitarray[innercnt, cnt - 1] == true ? false : true; }
                            if (((cnt + 1) < bmpData.Height)) { bitarraynew[innercnt, cnt + 1] = bitarray[innercnt, cnt + 1] == true ? false : true; }


                            // only for testing double outline

                            ///* set four pixel in the corner, if they don't set */
                            //if (((innercnt - 2) >= 0) && ((cnt - 2) >= 0)) { bitarraynew[innercnt - 1, cnt - 2] = bitarray[innercnt - 2, cnt - 2] == true ? false : true; }
                            //if (((innercnt - 2) >= 0) && ((cnt + 2) < bmpData.Height)) { bitarraynew[innercnt - 2, cnt + 2] = bitarray[innercnt - 2, cnt + 2] == true ? false : true; }
                            //if (((innercnt + 2) < (bmpData.Stride * 8)) && ((cnt - 2) >= 0)) { bitarraynew[innercnt + 2, cnt - 2] = bitarray[innercnt + 2, cnt - 2] == true ? false : true; }
                            //if (((innercnt + 2) < (bmpData.Stride * 8)) && ((cnt + 2) < bmpData.Height)) { bitarraynew[innercnt + 2, cnt + 2] = bitarray[innercnt + 2, cnt + 1] == true ? false : true; }

                            ///* set the pixel left and right */
                            //if (((innercnt - 2) >= 0)) { bitarraynew[innercnt - 2, cnt] = bitarray[innercnt - 2, cnt] == true ? false : true; }
                            //if (((innercnt + 2) < (bmpData.Stride * 8))) { bitarraynew[innercnt + 2, cnt] = bitarray[innercnt + 2, cnt] == true ? false : true; }

                            ///* set the pixel upper and lower */
                            //if (((cnt - 2) >= 0)) { bitarraynew[innercnt, cnt - 2] = bitarray[innercnt, cnt - 2] == true ? false : true; }
                            //if (((cnt + 2) < bmpData.Height)) { bitarraynew[innercnt, cnt + 2] = bitarray[innercnt, cnt + 2] == true ? false : true; }
                        }
                    }
                }

                for (int ycnt2 = 0; ycnt2 < Selected.Height; ycnt2++)
                {
                    line = 0;

                    for (int cnti = 0; cnti < MaxWidth; cnti++)
                    {
                        if (bitarraynew[cnti, ycnt2])
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

                CFontUtils.SaveByteLinesFromPicture((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag), Selected);
                ((CCharInfo)(CharacterPictureList[character.Index - PageStart].Tag)).UnscaledImage = Selected;
            }

            character.UndoRedoListTidyUp();
            character.UndoRedoListAdd(character.ByteLines);

            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[0].Enabled = character.UndoPossible;
            CharacterPictureList[character.Index - PageStart].ContextMenu.MenuItems[1].Enabled = character.RedoPossible;

            CreateAndShow(character, SizeMode.ChangeNothing);
            CheckChange();
        }

        private void CharacterFunctionOneOrAll(Action<CCharInfo> function, bool onechar)
        {
            CCharInfo character;

            if (onechar)
            {
                foreach (PictureBox item in CharacterPictureList)
                {
                    character = ((CCharInfo)(item.Tag));
                    function(character);
                }
            }
            else
            {
                character = ((CCharInfo)(((PictureBox)CharacterPictureList[Index - PageStart]).Tag));
                function(character);
            }
        }
        #endregion Character functions

        private void BtnClear_Click(object sender, EventArgs e)
        {
            CharacterFunctionOneOrAll(ClearCharacter, ChkOneAllCharacters.Checked);
        }

        private void BtnFill_Click(object sender, EventArgs e)
        {
            CharacterFunctionOneOrAll(FillCharacter, ChkOneAllCharacters.Checked);
        }

        private void BtnShiftUp_Click(object sender, EventArgs e)
        {
            CharacterFunctionOneOrAll(ShiftUpCharacter, ChkOneAllCharacters.Checked);
        }

        private void BtnShiftDown_Click(object sender, EventArgs e)
        {
            CharacterFunctionOneOrAll(ShiftDownCharacter, ChkOneAllCharacters.Checked);
        }

        private void BtnShiftLeft_Click(object sender, EventArgs e)
        {
            CharacterFunctionOneOrAll(ShiftLeftCharacter, ChkOneAllCharacters.Checked);
        }

        private void BtnShiftRight_Click(object sender, EventArgs e)
        {
            CharacterFunctionOneOrAll(ShiftRightCharacter, ChkOneAllCharacters.Checked);
        }

        private void BtnInvert_Click(object sender, EventArgs e)
        {
            CharacterFunctionOneOrAll(InvertCharacter, ChkOneAllCharacters.Checked);
        }

        private void BtnSwapHorizontally_Click(object sender, EventArgs e)
        {
            CharacterFunctionOneOrAll(SwapHorizontallyCharacter, ChkOneAllCharacters.Checked);
        }

        private void BtnSwapVertically_Click(object sender, EventArgs e)
        {
            CharacterFunctionOneOrAll(SwapVerticallyCharacter, ChkOneAllCharacters.Checked);
        }

        private void BtnOutline_Click(object sender, EventArgs e)
        {
            CharacterFunctionOneOrAll(OutlineCharacter, ChkOneAllCharacters.Checked);
        }
        private void BtnOutlineFont_Click(object sender, EventArgs e)
        {
            CharacterFunctionOneOrAll(OutlineCharacter, true); // Outline the whole font
        }
        private void BtnRenderText_Click(object sender, EventArgs e)
        {
            PictRenderText.Image = RenderTextLine(XmlSettings.CustomText);
        }
        private void BtnSetText_Click(object sender, EventArgs e)
        {
            string newValue = "";

            Settings.InputBox("Set new Text you wish to render.", XmlSettings.CustomText, ref newValue);

            if (newValue != null && newValue != "")
            {
                XmlSettings.CustomText = newValue;
            }

            XmlSettings.Write();
        }
        private void BtnPagePrev_Click(object sender, EventArgs e)
        {
            if (FontInfo == null || FontInfo.Character == null)
                return;

            if (PageStart > 0)
            {
                PageStart -= PageSize;

                Index = PageStart;
                UpdateGlyphTextbox();

                RebuildPage();
                UpdateGlyphTextbox();

                int localIndex = Index - PageStart;

                if (localIndex >= 0 && localIndex < CharacterPictureList.Count)
                {
                    Character_Click(CharacterPictureList[localIndex],
                        new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                }
            }
        }
        private void BtnPageNext_Click(object sender, EventArgs e)
        {
            if (FontInfo == null || FontInfo.Character == null)
                return;

            if (PageStart + PageSize < FontInfo.Character.Length)
            {
                PageStart += PageSize;

                Index = PageStart;
                RebuildPage();
                UpdateGlyphTextbox();

                int localIndex = Index - PageStart;

                if (localIndex >= 0 && localIndex < CharacterPictureList.Count)
                {
                    Character_Click(CharacterPictureList[localIndex],
                        new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                }
            }
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (Index - PageStart > 0)
            {
                PrevNext(EDirection.Prev);
            }
        }
        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (Index - PageStart < (CharacterPictureList.Count - 1))
            {
                PrevNext(EDirection.Next);
            }
        }
        private async void BtnAllHeight_Click(object sender, EventArgs e)
        {
            _bulkUpdate = true;
            FlowCharacterPanel.SuspendLayout();
            try
            {
                foreach (PictureBox item in CharacterPictureList)
                {
                    if (!ClickedOnCharacter)
                    {
                        CCharInfo character = (CCharInfo)item.Tag;

                        if (character.UnscaledImage == null)
                            continue;

                        CreateAndShow(character, SizeMode.ChangeHeight);
                    }

                    await Task.Yield();
                }
            }
            finally
            {
                FlowCharacterPanel.ResumeLayout();
                _bulkUpdate = false;
            }

            CheckChange();
        }

        private async void BtnAllWidth_Click(object sender, EventArgs e)
        {
            _bulkUpdate = true;
            FlowCharacterPanel.SuspendLayout();
            try
            {
                foreach (PictureBox item in CharacterPictureList)
                {
                    if (!ClickedOnCharacter)
                    {
                        CCharInfo character = (CCharInfo)item.Tag;

                        if (character.UnscaledImage == null)
                            continue;

                        CreateAndShow(character, SizeMode.ChangeWidth);
                    }

                    await Task.Yield();
                }
            }
            finally
            {
                FlowCharacterPanel.ResumeLayout();
                _bulkUpdate = false;
            }

            CheckChange();
        }
        private async void BtnAllBlankClear_Click(object sender, EventArgs e)
        {
            if (FontInfo == null || FontInfo.Character == null)
                return;

            FlowCharacterPanel.SuspendLayout();

            try
            {
                foreach (PictureBox pict in CharacterPictureList)
                {
                    CCharInfo character = pict.Tag as CCharInfo;

                    if (character == null)
                        continue;

                    // Skip real placeholders
                    if (character.ByteLines == null || character.ByteLines.Length == 0)
                        continue;

                    // Check if bitmap is completely black
                    bool isAllBlack = true;

                    foreach (byte b in character.ByteLines)
                    {
                        if (b != 0x00)
                        {
                            isAllBlack = false;
                            break;
                        }
                    }

                    if (!isAllBlack)
                        continue;

                    // 🔴 Convert to placeholder
                    character.Width = 0;
                    character.Height = 0;
                    character.WidthOriginal = 0;
                    character.HeightOriginal = 0;

                    character.ByteLines = null;
                    character.ByteLinesOriginal = null;
                    character.UnscaledImage = null;

                    // If this glyph is currently selected, re-display it
                    if (character.Index == Index)
                    {
                        DisplayCurrentCharacter(character);
                    }

                    // Replace preview image with placeholder
                    int previewW = 8 * Scalefactor;
                    int previewH = 9 * Scalefactor;

                    Bitmap placeholder = new Bitmap(previewW, previewH);
                    using (Graphics g = Graphics.FromImage(placeholder))
                    {
                        g.Clear(XmlSettings.Color);

                        using (Pen pen = new Pen(Color.White))
                        {
                            g.DrawRectangle(pen, 0, 0, previewW - 1, previewH - 1);
                        }
                    }

                    pict.Image = placeholder;
                    pict.Width = previewW;
                    pict.Height = previewH;

                    await Task.Yield();
                }
            }
            finally
            {
                FlowCharacterPanel.ResumeLayout();
            }

            CheckChange();

        }


        private UInt32? ParseXmlNumber(string s)
        {
            UInt32? number;

            if (s.ToLower().StartsWith("0x"))
            {
                number = UInt32.Parse(s.Substring(2), System.Globalization.NumberStyles.HexNumber);
            }
            else if (s.ToLower().EndsWith("h"))
            {
                number = UInt32.Parse(s.TrimEnd('h'), System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                number = UInt32.Parse(s, System.Globalization.NumberStyles.Integer);
            }

            return number;
        }

        private void TxtCharacter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (FontInfo == null || FontInfo.Character == null)
                    return;

                UInt32? number = null;

                try
                {
                    number = ParseXmlNumber(TxtCharacter.Text);
                }
                catch
                {
                }

                if (number != null)
                {
                    int newIndex = (int)number;

                    // Clamp to valid range
                    if (newIndex < 0)
                        newIndex = 0;

                    if (newIndex >= FontInfo.Character.Length)
                        newIndex = FontInfo.Character.Length - 1;

                    int newPageStart = (newIndex / PageSize) * PageSize;

                    // Only rebuild page if page actually changes
                    if (newPageStart != PageStart)
                    {
                        PageStart = newPageStart;
                        RebuildPage();
                        UpdateGlyphTextbox();
                    }

                    Index = newIndex;
                    UpdateGlyphTextbox();

                    int localIndex = Index - PageStart;

                    if (localIndex >= 0 && localIndex < CharacterPictureList.Count)
                    {
                        CCharInfo characterinfo =
                            (CCharInfo)CharacterPictureList[localIndex].Tag;

                        DisplayCurrentCharacter(characterinfo);
                    }

                    // Always update textbox to real index
                    TxtCharacter.Text = Index.ToString();
                }

                e.Handled = true;
            }
        }


        private void ChkOneAllCharacters_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkOneAllCharacters.Checked)
            {
                GrpOneCharacter.BackColor = SystemColors.Highlight;
            }
            else
            {
                GrpOneCharacter.BackColor = SystemColors.Control;
            }
        }

        private void UpdatePageButtons()
        {
            if (FontInfo == null || FontInfo.Character == null)
            {
                BtnPagePrev.Enabled = false;
                BtnPageNext.Enabled = false;
                TxtPageNumber.Text = "0";
                LblPageTotal.Text = "/ 0";
                return;
            }

            int total = FontInfo.Character.Length;

            int totalPages = (total + PageSize - 1) / PageSize;
            if (totalPages == 0)
                totalPages = 1;

            int currentPage = (PageStart / PageSize) + 1;

            BtnPagePrev.Enabled = (PageStart > 0);
            BtnPageNext.Enabled = (PageStart + PageSize < total);

            TxtPageNumber.Text = currentPage.ToString();
            LblPageTotal.Text = $"/ {totalPages}";
        }

        private void TxtPageNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (FontInfo == null || FontInfo.Character == null)
                    return;

                int total = FontInfo.Character.Length;
                int totalPages = (int)Math.Ceiling((double)total / PageSize);

                if (totalPages == 0)
                    totalPages = 1;

                if (int.TryParse(TxtPageNumber.Text, out int page))
                {
                    // Only change page if valid
                    if (page >= 1 && page <= totalPages)
                    {
                        PageStart = (page - 1) * PageSize;
                        RebuildPage();
                        UpdateGlyphTextbox();
                    }
                }

                // Always restore correct page number in textbox
                int currentPage = (PageStart / PageSize) + 1;
                TxtPageNumber.Text = currentPage.ToString();

                e.SuppressKeyPress = true;
            }
        }

        private void TxtGlyphRange_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\r')
                return;

            if (FontInfo == null || FontInfo.Character == null)
                return;

            const int max = 65536;

            if (!int.TryParse(TxtGlyphRange.Text, out int requestedLength))
            {
                TxtGlyphRange.Text = FontInfo.Character.Length.ToString();
                return;
            }

            // ✅ Remember what user asked for BEFORE clamping
            bool userAskedForBelowOne = (requestedLength < 1);

            int newLength = requestedLength;

            // Enforce minimum of 1 glyph
            if (newLength < 1)
                newLength = 1;

            // Clamp maximum
            if (newLength > max)
                newLength = max;

            TxtGlyphRange.Text = newLength.ToString();

            int currentLength = FontInfo.Character.Length;

            // ✅ If user typed 0 (or negative) and we already are at length 1,
            // we STILL need to reset glyph 0 into a placeholder.
            if (newLength == currentLength)
            {
                if (userAskedForBelowOne && newLength == 1 && currentLength >= 1)
                {
                    DialogResult result = MessageBox.Show(
                    $"ALL GLYPHS ABOVE INDEX {newLength - 1} WILL BE PERMANENTLY REMOVED!\n\nThis action cannot be undone.\n\nContinue?",
                    "WARNING",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);

                    if (result != DialogResult.OK)
                    {
                        TxtGlyphRange.Text = currentLength.ToString();
                        return;
                    }

                    MakeGlyphPlaceholder(FontInfo.Character[0]);

                    // keep selection sane
                    Index = 0;
                    PageStart = 0;

                    RebuildPage();
                    UpdatePageButtons();
                    ShowCharacterCount();
                    UpdateGlyphTextbox();
                }

                e.Handled = true;
                return;
            }

            // 🔴 SHRINK
            if (newLength < currentLength)
            {
                DialogResult result = MessageBox.Show(
                    $"ALL GLYPHS ABOVE INDEX {newLength - 1} WILL BE PERMANENTLY REMOVED!\n\nThis action cannot be undone.\n\nContinue?",
                    "WARNING",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.OK)
                {
                    TxtGlyphRange.Text = currentLength.ToString();
                    return;
                }

                Array.Resize(ref FontInfo.Character, newLength);

                // If we shrunk to 1 (because user asked 0), force glyph 0 placeholder
                if (newLength == 1 && userAskedForBelowOne)
                    MakeGlyphPlaceholder(FontInfo.Character[0]);

                if (Index >= newLength)
                    Index = newLength - 1;
            }
            // 🟢 EXTEND
            else
            {
                Array.Resize(ref FontInfo.Character, newLength);

                for (int i = currentLength; i < newLength; i++)
                {
                    CCharInfo ch = new CCharInfo();
                    ch.Height = 0;
                    ch.HeightOriginal = 0;
                    ch.Width = 0;
                    ch.WidthOriginal = 0;
                    ch.Index = i;
                    ch.ByteLines = null;
                    ch.ByteLinesOriginal = null;
                    ch.UnscaledImage = null;
                    FontInfo.Character[i] = ch;
                }
            }

            PageStart = (Index / PageSize) * PageSize;
            RebuildPage();
            UpdatePageButtons();
            ShowCharacterCount();
            UpdateGlyphTextbox();

            TxtGlyphRange.Text = FontInfo.Character.Length.ToString();
            e.Handled = true;
        }

        // Put this helper anywhere inside FontEditorPane (same class)
        private void MakeGlyphPlaceholder(CCharInfo ch)
        {
            if (ch == null) return;

            ch.Width = 0;
            ch.Height = 0;
            ch.WidthOriginal = 0;
            ch.HeightOriginal = 0;

            ch.ByteLines = null;
            ch.ByteLinesOriginal = null;

            ch.UnscaledImage = null;

            // optional but recommended so undo/redo won’t reference old data
            ch.UndoRedoListClear();
            ch.UndoRedoPosition = 0;
        }


        private void UpdateGlyphTextbox()
        {
            if (Index >= 0 && Index <= 0xFFFF)
            {
                TxtGlyph.Text = char.ConvertFromUtf32(Index);
            }
            else
            {
                TxtGlyph.Text = "";
            }
        }

        private void TxtGlyph_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If typing a normal character (not Enter), replace existing character
            if (e.KeyChar != '\r' && !char.IsControl(e.KeyChar))
            {
                TxtGlyph.Text = e.KeyChar.ToString();
                TxtGlyph.SelectionStart = 1; // keep cursor at end
                e.Handled = true;            // stop default behavior
                return;
            }

            // From here down is your original Enter logic
            if (e.KeyChar != '\r')
                return;

            if (string.IsNullOrEmpty(TxtGlyph.Text))
                return;

            string input = TxtGlyph.Text;

            int codePoint = char.ConvertToUtf32(input, 0);

            if (codePoint < 0 || codePoint >= FontInfo.Character.Length)
            {
                // Out of range → ignore
                UpdateGlyphTextbox();
                return;
            }

            int newIndex = codePoint;
            int newPageStart = (newIndex / PageSize) * PageSize;

            // Only rebuild if page changes
            if (newPageStart != PageStart)
            {
                PageStart = newPageStart;
                RebuildPage();
            }

            Index = newIndex;

            int localIndex = Index - PageStart;

            if (localIndex >= 0 && localIndex < CharacterPictureList.Count)
            {
                CCharInfo characterinfo =
                    (CCharInfo)CharacterPictureList[localIndex].Tag;

                DisplayCurrentCharacter(characterinfo);
            }

            TxtCharacter.Text = Index.ToString();
            UpdateGlyphTextbox();

            e.Handled = true;
        }
        private void TxtGlyphRange_Leave(object sender, EventArgs e)
        {
            // Simulate pressing Enter
            TxtGlyphRange_KeyPress(TxtGlyphRange, new KeyPressEventArgs('\r'));
        }
        private void TxtCharacter_Leave(object sender, EventArgs e)
        {
            // Simulate pressing Enter
            TxtCharacter_KeyPress(TxtCharacter, new KeyPressEventArgs('\r'));
        }

        private void TxtGlyph_Leave(object sender, EventArgs e)
        {
            // Simulate pressing Enter
            TxtGlyph_KeyPress(TxtGlyph, new KeyPressEventArgs('\r'));
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            // Force focus away from textboxes
            this.ActiveControl = null;
        }

        private void TxtGlyphRange_KeyPress_Filter(object sender, KeyPressEventArgs e)
        {
            // Allow digits
            if (char.IsDigit(e.KeyChar))
                return;

            // Allow Backspace
            if (e.KeyChar == (char)Keys.Back)
                return;

            // Allow Enter (for your existing logic)
            if (e.KeyChar == '\r')
                return;

            // Block everything else
            e.Handled = true;
        }
        public void SaveAs(string filePath)
        {
            if (FontInfo == null)
                return;

            // 1️⃣ Perform size check BEFORE file creation
            if (FontInfo is CWFNFontInfo wfn)
            {
                if (wfn.EstimateSize() > 65535)
                {
                    MessageBox.Show(
                        "Font exceeds 64KB. The WFN format used by AGS does not support files larger than 65,535 bytes.",
                        "Save Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return; // 🔴 Exit BEFORE file creation
                }
            }

            // 2️⃣ Proceed with saving only if no size issue
            using (System.IO.FileStream file = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(file))
            {
                FontInfo.Write(writer);
            }

            // 3️⃣ Update the font path after successful save
            FontInfo.FontPath = filePath;
            FontInfo.FontName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        }

        private long CalculateCurrentWFNSize()
        {
            if (FontInfo == null || FontInfo.Character == null)
                return 0;

            long size = 17; // Header (15 bytes signature + 2 bytes offset pointer)

            foreach (var ch in FontInfo.Character)
            {
                if (ch == null)
                    continue;

                // 🔴 Skip placeholder glyphs
                if (ch.Height <= 0)
                    continue;

                size += 2; // Width
                size += 2; // Height

                int bytesPerLine = (ch.Width - 1) / 8 + 1;
                size += ch.Height * bytesPerLine;
            }

            // 🔵 IMPORTANT NOTE:
            // Offset table size does NOT count toward 64KB limit.
            // Only check that it STARTS before 65535.
            // So we do NOT add Character.Length * 2 here.

            return size;
        }


        private const int PLACEHOLDER_W = 8;
        private const int PLACEHOLDER_H = 9;

        // Temporary drawing-area image for placeholder glyphs.
        // IMPORTANT: This does NOT modify the glyph or store into UnscaledImage.
        private Bitmap GetDrawingAreaBitmapForGlyph(CCharInfo ch)
        {
            if (ch != null && ch.UnscaledImage is Bitmap realBmp)
                return realBmp;

            // Make a temp bitmap (black 1bpp) so Zoom/Draw don't crash and it's 8x9 (not 1x1, not 4x4).
            return new Bitmap(PLACEHOLDER_W, PLACEHOLDER_H, PixelFormat.Format1bppIndexed);
        }

        // Refresh drawing area display without materializing the glyph.
        private void RefreshDrawingAreaDisplay(CCharInfo characterinfo)
        {
            if (characterinfo == null)
                return;

            Bitmap src = GetDrawingAreaBitmapForGlyph(characterinfo);

            Bitmap scaled;
            CFontUtils.ScaleBitmap(src, out scaled, ZoomDrawingArea.Value);

            if (scaled != null)
            {
                DrawingArea.Size = scaled.Size;
                DrawingArea.Image = scaled;
                DrawingArea.Invalidate();
            }
        }

        private void RefreshPlaceholders()
        {
            foreach (PictureBox pict in CharacterPictureList)
            {
                CCharInfo item = pict.Tag as CCharInfo;

                if (item == null)
                    continue;

                // Placeholder = no bitmap data
                if (item.Width == 0 || item.Height == 0 || item.UnscaledImage == null)
                {
                    int previewW = 8 * Scalefactor;
                    int previewH = 9 * Scalefactor;

                    Bitmap placeholder = new Bitmap(previewW, previewH);
                    using (Graphics g = Graphics.FromImage(placeholder))
                    {
                        g.Clear(FlowCharacterPanel.BackColor);

                        using (Pen pen = new Pen(Color.White))
                        {
                            g.DrawRectangle(pen, 0, 0, previewW - 1, previewH - 1);
                        }
                    }

                    pict.Image = placeholder;
                    pict.Width = previewW;
                    pict.Height = previewH;
                }
            }
        }

        private void SetSelectedPreview(PictureBox pict)
        {
            if (_selectedPreview == pict)
                return;

            // repaint old + new only (fast)
            if (_selectedPreview != null)
                _selectedPreview.Invalidate();

            _selectedPreview = pict;

            if (_selectedPreview != null)
                _selectedPreview.Invalidate();
        }

        private void CharacterPreview_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb == null)
                return;

            if (pb == _selectedPreview)
            {
                using (Pen pen = new Pen(Color.DeepSkyBlue, 2))
                {
                    Rectangle r = pb.ClientRectangle;
                    r.Inflate(0, 0); // keep inside bounds
                    e.Graphics.DrawRectangle(pen, r);
                }
            }
        }
        public void LoadFontFromMemory(CFontInfo font)
        {
            this.FontInfo = font;

            if (FontInfo != null)
                TxtGlyphRange.Text = FontInfo.NumberOfCharacters.ToString();

            RebuildPage();
        }
    }
}
