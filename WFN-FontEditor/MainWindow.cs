using System;
using System.Drawing;
using System.Windows.Forms;
using AGS.Plugin.FontEditor;

namespace WFN_FontEditor
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            SetupTabClosingUI();

            this.StartPosition = FormStartPosition.CenterScreen;

            // Make window wider without breaking layout
            this.Width += 60;   // increase by 60 pixels

            // Optional: prevent shrinking too small
            this.MinimumSize = new Size(this.Width, this.Height);

            this.Shown += (s, e) => BtnNew.Focus();
        }
        private void BtnNew_Click(object sender, EventArgs e)
        {
            int unicodeRange = 65536;

            // Generate unique untitled name
            int counter = 0;
            string baseName;
            bool exists;

            do
            {
                baseName = $"AGSFNT{counter}.WFN";
                exists = false;

                foreach (TabPage tab in TabControl.TabPages)
                {
                    if (tab.Text.Replace("*", "") == baseName)
                    {
                        exists = true;
                        break;
                    }
                }

                counter++;

            } while (exists);

            CWFNFontInfo newFont = new CWFNFontInfo();
            newFont.FontPath = "";
            newFont.FontName = baseName;
            newFont.NumberOfCharacters = unicodeRange;
            newFont.Character = new CCharInfo[unicodeRange];

            for (int i = 0; i < unicodeRange; i++)
            {
                CCharInfo ch = new CCharInfo();
                ch.Index = i;
                ch.Width = 0;
                ch.Height = 0;
                ch.ByteLines = new byte[0];
                newFont.Character[i] = ch;
            }

            FontEditorPane fep = new FontEditorPane("", "", baseName);
            fep.LoadFontFromMemory(newFont);

            TabPage tp = new TabPage();
            tp.Text = baseName + "*";
            tp.Tag = null;
            tp.Controls.Add(fep);
            fep.Dock = DockStyle.Fill;
            fep.Tag = tp;

            TabControl.TabPages.Add(tp);
            TabControl.SelectedTab = tp;
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Open Font";
                dialog.Filter =
                    "WFN and SCI Fonts (*.wfn; FONT.*)|*.wfn;FONT.*|WFN Font (*.wfn)|*.wfn|SCI Font (FONT.*)|FONT.*|All Files (*.*)|*.*";
                dialog.Multiselect = true;
                dialog.RestoreDirectory = true;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                foreach (string fullPath in dialog.FileNames)
                    OpenFontFileInTab(fullPath);
            }
        }

        private void OpenFontFileInTab(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                return;

            string dir = System.IO.Path.GetDirectoryName(fullPath);
            string file = System.IO.Path.GetFileName(fullPath);

            if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(file))
                return;

            // If already open -> focus existing tab
            foreach (TabPage existing in TabControl.TabPages)
            {
                if (existing.Tag is string tagPath &&
                    string.Equals(tagPath, fullPath, StringComparison.OrdinalIgnoreCase))
                {
                    TabControl.SelectedTab = existing;
                    return;
                }
            }

            FontEditorPane fep = new FontEditorPane(dir, file, file);
            fep.OnFontModified += new EventHandler(fep_OnFontModified);

            TabPage tp = new TabPage();
            tp.Text = file;
            tp.Tag = fullPath;
            tp.Controls.Add(fep);
            fep.Dock = DockStyle.Fill;
            fep.Tag = tp;

            TabControl.TabPages.Add(tp);
            TabControl.SelectedTab = tp;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedTab == null)
                return;

            FontEditorPane activePane = TabControl.SelectedTab.Controls.Count > 0
                ? TabControl.SelectedTab.Controls[0] as FontEditorPane
                : null;

            if (activePane == null)
                return;

            // If no file path yet → redirect to Save As
            if (!(TabControl.SelectedTab.Tag is string fullPath) || string.IsNullOrEmpty(fullPath))
            {
                BtnSaveAs_Click(sender, e);
                return;
            }

            try
            {
                activePane.CurrentFontInfo.Write(fullPath);
                TabControl.SelectedTab.Text = System.IO.Path.GetFileName(fullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to save font:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnSaveAs_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedTab == null)
                return;

            FontEditorPane activePane = TabControl.SelectedTab.Controls.Count > 0
                ? TabControl.SelectedTab.Controls[0] as FontEditorPane
                : null;

            if (activePane == null)
                return;

            CFontInfo font = activePane.CurrentFontInfo;
            if (font == null)
                return;

            bool isWfn = font is CWFNFontInfo;
            bool isSci = font is CSCIFontInfo;

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                if (isWfn)
                {
                    dialog.Filter = "WFN Font Files (*.wfn)|*.wfn|All Files (*.*)|*.*";
                    dialog.DefaultExt = "wfn";
                }
                else if (isSci)
                {
                    dialog.Filter = "SCI Font Files (FONT.xxx)|FONT.*|All Files (*.*)|*.*";
                    dialog.DefaultExt = "";
                }
                else
                {
                    dialog.Filter = "All Files (*.*)|*.*";
                }

                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;

                // 🔥 THIS FIXES THE EMPTY FILENAME
                dialog.FileName = TabControl.SelectedTab.Text.Replace("*", "");

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    font.Write(dialog.FileName);

                    TabControl.SelectedTab.Tag = dialog.FileName;
                    TabControl.SelectedTab.Text = System.IO.Path.GetFileName(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Failed to save font:\n\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // Keep your existing modified-marker logic (*)
        void fep_OnFontModified(object sender, System.EventArgs e)
        {
            TabPage tp = (TabPage)((FontEditorPane)sender).Tag;
            MyEventArgs me = (MyEventArgs)e;

            if (tp.Text.Contains("*") && me.Modified == false)
            {
                tp.Text = tp.Text.Replace("*", "");
            }
            else if (!tp.Text.Contains("*") && me.Modified == true)
            {
                tp.Text += "*";
            }
        }

        // ----------------------------
        // Closable tabs (X + middle click)
        // ----------------------------
        private void SetupTabClosingUI()
        {
            TabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            TabControl.Padding = new Point(18, 4);
            TabControl.DrawItem += TabControl_DrawItem;
            TabControl.MouseDown += TabControl_MouseDown;
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = TabControl.TabPages[e.Index];
            Rectangle tabRect = TabControl.GetTabRect(e.Index);

            // Background
            e.Graphics.FillRectangle(SystemBrushes.Control, tabRect);

            // Text
            Rectangle textRect = new Rectangle(tabRect.X + 2, tabRect.Y + 4, tabRect.Width - 18, tabRect.Height - 4);
            TextRenderer.DrawText(e.Graphics, page.Text, Font, textRect, SystemColors.ControlText, TextFormatFlags.Left);

            // X button
            Rectangle closeRect = GetCloseRect(tabRect);
            TextRenderer.DrawText(e.Graphics, "x", Font, closeRect, SystemColors.ControlText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            // Border highlight on selected
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                ControlPaint.DrawBorder(e.Graphics, tabRect, SystemColors.Highlight, ButtonBorderStyle.Solid);
            }
        }

        private void TabControl_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < TabControl.TabPages.Count; i++)
            {
                Rectangle tabRect = TabControl.GetTabRect(i);
                if (!tabRect.Contains(e.Location))
                    continue;

                // Middle click closes anywhere on the tab
                if (e.Button == MouseButtons.Middle)
                {
                    CloseTabAt(i);
                    return;
                }

                // Left click on X closes
                if (e.Button == MouseButtons.Left)
                {
                    Rectangle closeRect = GetCloseRect(tabRect);
                    if (closeRect.Contains(e.Location))
                    {
                        CloseTabAt(i);
                        return;
                    }
                }

                return;
            }
        }

        private Rectangle GetCloseRect(Rectangle tabRect)
        {
            // Small "x" area on the right side of tab
            int size = 12;
            return new Rectangle(
                tabRect.Right - size - 4,
                tabRect.Top + (tabRect.Height - size) / 2,
                size,
                size);
        }

        private void CloseTabAt(int index)
        {
            if (index < 0 || index >= TabControl.TabPages.Count)
                return;

            TabPage tp = TabControl.TabPages[index];

            // If modified -> ask
            bool modified = tp.Text.Contains("*");
            if (modified)
            {
                var res = MessageBox.Show(
                    "This font has unsaved changes.\n\nClose anyway?",
                    "Unsaved changes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (res != DialogResult.Yes)
                    return;
            }

            TabControl.TabPages.RemoveAt(index);
            tp.Dispose();
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedTab == null)
                return;

            FontEditorPane activePane = TabControl.SelectedTab.Controls.Count > 0
                ? TabControl.SelectedTab.Controls[0] as FontEditorPane
                : null;

            if (activePane == null)
                return;

            CFontInfo oldfont = activePane.CurrentFontInfo;
            if (oldfont == null)
                return;

            // fullPath may be null for "New" / unsaved tabs
            string fullPath = TabControl.SelectedTab.Tag as string;

            // Choose output folder:
            // - if we have a source path -> same folder
            // - else -> ask user for a folder
            string paneFilepath = null;

            if (!string.IsNullOrEmpty(fullPath))
            {
                paneFilepath = System.IO.Path.GetDirectoryName(fullPath);
            }
            else
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select a folder to save the converted font";

                    if (folderDialog.ShowDialog() != DialogResult.OK)
                        return;

                    paneFilepath = folderDialog.SelectedPath;
                }
            }

            // Decide conversion direction from IN-MEMORY type (works for unsaved tabs too)
            bool isWfn = oldfont is CWFNFontInfo;
            bool isSci = oldfont is CSCIFontInfo;

            // Fallback (shouldn't normally happen): try extension if we have a path
            if (!isWfn && !isSci && !string.IsNullOrEmpty(fullPath))
            {
                string ext = System.IO.Path.GetExtension(fullPath).ToLower();
                isWfn = (ext == ".wfn");
                isSci = !isWfn;
            }

            if (isWfn)
            {
                // WFN -> SCI
                DialogResult result = MessageBox.Show(
                    "You're about to convert this WFN font into a SCI font. This will create a new SCI font (FONT.xxx) and open it in a new tab.\n\nContinue?",
                    "Warning",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.OK)
                    return;

                // Find next free FONT.xxx
                int number = -1;
                for (int i = 0; i <= 999; i++)
                {
                    string candidate = System.IO.Path.Combine(paneFilepath, "FONT." + i.ToString("000"));
                    if (!System.IO.File.Exists(candidate))
                    {
                        number = i;
                        break;
                    }
                }

                if (number == -1)
                {
                    MessageBox.Show(
                        "This folder already contains the maximum number of SCI fonts (FONT.000–FONT.999).\n\nConversion aborted.",
                        "Limit Reached",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string newname = System.IO.Path.Combine(paneFilepath, "FONT." + number.ToString("000"));

                int glyphLimit = 256;

                int glyphCount = (oldfont.Character != null)
                    ? oldfont.Character.Length
                    : oldfont.NumberOfCharacters;

                if (glyphCount > glyphLimit)
                {
                    DialogResult limitResult = MessageBox.Show(
                        $"Warning!\n\nSCI fonts are limited to 256 glyphs (0–255).\n\nThe current WFN font glyphs range is {glyphCount}.\n\nAll glyphs beyond index 256 will NOT be saved.\n\nContinue?",
                        "SCI Glyph Limit",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);

                    if (limitResult != DialogResult.OK)
                        return;
                }

                int finalCount = Math.Min(glyphCount, glyphLimit);

                CFontInfo newfont = new CSCIFontInfo();
                newfont.FontPath = paneFilepath;
                newfont.FontName = "FONT." + number.ToString("000");
                newfont.NumberOfCharacters = finalCount;
                newfont.Character = new CCharInfo[finalCount];

                for (int i = 0; i < finalCount; i++)
                {
                    CCharInfo src = (oldfont.Character != null && i < oldfont.Character.Length)
                        ? oldfont.Character[i]
                        : null;

                    CCharInfo dst = new CCharInfo();
                    dst.Index = i;

                    if (src != null)
                    {
                        dst.Width = src.Width;
                        dst.Height = src.Height;

                        if (src.ByteLines != null)
                        {
                            dst.ByteLines = new byte[src.ByteLines.Length];
                            Array.Copy(src.ByteLines, dst.ByteLines, src.ByteLines.Length);
                        }
                        else
                        {
                            dst.ByteLines = new byte[0];
                        }
                    }
                    else
                    {
                        dst.Width = 0;
                        dst.Height = 0;
                        dst.ByteLines = new byte[0];
                    }

                    newfont.Character[i] = dst;
                }

                using (var fs = System.IO.File.Create(newname)) { }
                newfont.Write(newname);
                OpenFontFileInTab(newname);
            }
            else
            {
                // SCI -> WFN
                DialogResult result = MessageBox.Show(
                    "You're about to convert this SCI font into a WFN font. This will create a new WFN font (AGSFNTx.WFN) and open it in a new tab.\n\nContinue?",
                    "Warning",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.OK)
                    return;

                int number = -1;
                for (int i = 0; i <= 999; i++)
                {
                    string candidate = System.IO.Path.Combine(paneFilepath, "AGSFNT" + i.ToString() + ".WFN");
                    if (!System.IO.File.Exists(candidate))
                    {
                        number = i;
                        break;
                    }
                }

                if (number == -1)
                {
                    MessageBox.Show(
                        "This folder already contains the maximum number of WFN fonts (AGSFNT0–AGSFNT999).\n\nConversion aborted.",
                        "Limit Reached",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string newname = System.IO.Path.Combine(paneFilepath, "AGSFNT" + number.ToString() + ".WFN");

                int finalCount = (oldfont.Character != null)
                    ? oldfont.Character.Length
                    : oldfont.NumberOfCharacters;

                CFontInfo newfont = new CWFNFontInfo();
                newfont.FontPath = paneFilepath;
                newfont.FontName = "AGSFNT" + number.ToString() + ".WFN";
                newfont.NumberOfCharacters = finalCount;
                newfont.Character = new CCharInfo[finalCount];

                for (int i = 0; i < finalCount; i++)
                {
                    CCharInfo src = (oldfont.Character != null && i < oldfont.Character.Length)
                        ? oldfont.Character[i]
                        : null;

                    CCharInfo dst = new CCharInfo();
                    dst.Index = i;

                    if (src != null)
                    {
                        dst.Width = src.Width;
                        dst.Height = src.Height;

                        if (src.ByteLines != null)
                        {
                            dst.ByteLines = new byte[src.ByteLines.Length];
                            Array.Copy(src.ByteLines, dst.ByteLines, src.ByteLines.Length);
                        }
                        else
                        {
                            dst.ByteLines = new byte[0];
                        }
                    }
                    else
                    {
                        dst.Width = 0;
                        dst.Height = 0;
                        dst.ByteLines = new byte[0];
                    }

                    newfont.Character[i] = dst;
                }

                using (var fs = System.IO.File.Create(newname)) { }
                newfont.Write(newname);
                OpenFontFileInTab(newname);
            }
        }
        private void BtnAbout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "WFN-FontEditor (256 ASCII characters) by Rulaman:\nhttps://github.com/Rulaman/WFN-FontEditor\n\nImproved version with full Unicode support by PacketLauncher (Gal Shemesh):\nhttps://github.com/PacketLauncher/WFN-FontEditor",
                "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            foreach (TabPage tab in TabControl.TabPages)
            {
                if (tab.Text.Contains("*"))
                {
                    DialogResult result = MessageBox.Show(
                        "There are unsaved fonts open.\n\nClose anyway?",
                        "Unsaved Changes",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }

                    break; // Only prompt once
                }
            }
        }

    }
}
