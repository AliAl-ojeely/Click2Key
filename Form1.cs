using ReaLTaiizor.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Click2Key
{
    public partial class frmMain : CrownForm
    {
        private ToolTip infoToolTip;

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private bool isLightTheme = false;
        private bool isArabic = false;

        // Standalone ShortcutData used by ShortcutsRepository
        private List<ShortcutData> shortcutsRegistry = new List<ShortcutData>();
        private NotifyIcon appTrayIcon;

        // Context menu for the system tray icon
        private ContextMenuStrip trayContextMenu;
        private ToolStripMenuItem miOpen;
        private ToolStripMenuItem miExit;
        private ToolStripMenuItem miToggleTray;

        private const uint KEYEVENTF_KEYUP = 0x0002;

        // Performance caches
        private Bitmap cachedGradient;
        private Size lastClientSize;
        private int lastFlowPanelWidth = -1;

        public frmMain()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            ApplyGradientBackground();

            ClickLogger.LoadStats();
            SetupToolTips();

            // -----------------------------------------------------------------
            // System tray icon – initially hidden
            // -----------------------------------------------------------------
            appTrayIcon = new NotifyIcon
            {
                Icon = this.Icon,
                Text = "Click2Key - Running in background",
                Visible = false               // not visible until user clicks "System Tray"
            };
            appTrayIcon.MouseDoubleClick += notifyIcon1_MouseDoubleClick;

            // Build the context menu for the tray icon
            trayContextMenu = new ContextMenuStrip();

            miOpen = new ToolStripMenuItem("Open");
            miOpen.Click += (s, args) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                appTrayIcon.Visible = false;          // hide tray when window is open
                UpdateTrayToggleText();
            };

            miToggleTray = new ToolStripMenuItem("Turn Off System Tray");
            miToggleTray.Click += (s, args) =>
            {
                appTrayIcon.Visible = !appTrayIcon.Visible;
                UpdateTrayToggleText();

                // If the user turns the tray OFF while the window is hidden, show the window
                if (!appTrayIcon.Visible && !this.Visible)
                {
                    this.Show();
                    this.ShowInTaskbar = true;
                }
            };

            miExit = new ToolStripMenuItem("Exit");
            miExit.Click += (s, args) =>
            {
                this.Close();   // now triggers normal form closing -> app exits
            };

            trayContextMenu.Items.Add(miOpen);
            trayContextMenu.Items.Add(miToggleTray);
            trayContextMenu.Items.Add(new ToolStripSeparator());
            trayContextMenu.Items.Add(miExit);

            appTrayIcon.ContextMenuStrip = trayContextMenu;

            // 4‑column layout handler
            flpShortcuts.Resize += flpShortcuts_Resize;

            // Delay ComboBox: prevent free‑text input
            cmbDelay.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // ------------------------------------------------------------
        // Helper to keep the "Turn Off/On System Tray" text correct
        // ------------------------------------------------------------
        private void UpdateTrayToggleText()
        {
            if (isArabic)
                miToggleTray.Text = appTrayIcon.Visible ? "إيقاف أيقونة النظام" : "تشغيل أيقونة النظام";
            else
                miToggleTray.Text = appTrayIcon.Visible ? "Turn Off System Tray" : "Turn On System Tray";
        }

        // ==========================================
        // Engine methods
        // ==========================================
        private void SetupToolTips()
        {
            infoToolTip = new ToolTip
            {
                IsBalloon = true,
                ToolTipIcon = ToolTipIcon.Info,
                ToolTipTitle = "Shortcut Info"
            };
        }

        private void SimulateKeystroke(byte[] modifiers, byte mainKey)
        {
            if (modifiers != null)
            {
                foreach (byte mod in modifiers)
                    keybd_event(mod, 0, 0, 0);
            }

            keybd_event(mainKey, 0, 0, 0);
            keybd_event(mainKey, 0, KEYEVENTF_KEYUP, 0);

            if (modifiers != null)
            {
                for (int i = modifiers.Length - 1; i >= 0; i--)
                    keybd_event(modifiers[i], 0, KEYEVENTF_KEYUP, 0);
            }
        }

        private async System.Threading.Tasks.Task ExecuteShortcutAsync(
            Button targetButton, Label counterLabel, string logKey,
            byte[] modifiers, byte mainKey)
        {
            targetButton.Enabled = false;
            string originalText = targetButton.Text;

            // Read delay directly from the combo box (no extra event needed)
            int delaySeconds = 0;
            if (cmbDelay.SelectedItem != null)
                int.TryParse(cmbDelay.SelectedItem.ToString(), out delaySeconds);

            for (int i = delaySeconds; i > 0; i--)
            {
                targetButton.Text = isArabic ? $"انتظر {i}ث..." : $"Wait {i}s...";
                await System.Threading.Tasks.Task.Delay(1000);
            }

            SimulateKeystroke(modifiers, mainKey);

            ClickLogger.LogClick(logKey);
            if (counterLabel != null)
                counterLabel.Text = ClickLogger.GetCount(logKey).ToString();

            targetButton.Text = isArabic ? "تم ✓" : "DONE ✓";
            await System.Threading.Tasks.Task.Delay(1500);

            targetButton.Text = originalText;
            targetButton.Enabled = true;
        }

        // ==========================================
        // Form & UI Events
        // ==========================================
        private void frmMain_Load(object sender, EventArgs e)
        {
            cmbDelay.Items.Clear();
            for (int i = 0; i <= 5; i++)
                cmbDelay.Items.Add(i.ToString());
            cmbDelay.SelectedIndex = 0;

            this.SuspendLayout();
            shortcutsRegistry = ShortcutsRepository.GetAll();   // uses the external ShortcutData
            RenderShortcuts();
            this.ResumeLayout();

            UpdateLanguage();
        }

        private void flpShortcuts_Resize(object sender, EventArgs e)
        {
            const int columns = 4;
            const int spacing = 6;
            if (flpShortcuts.Controls.Count == 0) return;

            int availableWidth = flpShortcuts.ClientSize.Width - spacing * (columns + 1);
            if (availableWidth == lastFlowPanelWidth) return;
            lastFlowPanelWidth = availableWidth;

            int childWidth = availableWidth / columns;
            if (childWidth < 20) childWidth = 20;

            foreach (ShortcutControl ctrl in flpShortcuts.Controls)
                ctrl.Width = childWidth;
        }

        // ==========================================
        // Language & Theme
        // ==========================================
        private void UpdateLanguage()
        {
            if (isArabic)
            {
                mtbtnLang.Text = "EN/AR";
                mtbtnTheme.Text = "المظهر";
                mtrBtnLogFile.Text = "سجل العمليات";
                mtbtnDeveloperInfo.Text = "معلومات المطور";
                bigLabel1.Text = "ارتباط بينك و بين لوحة المفاتيح";
                bigLabel1.RightToLeft = RightToLeft.Yes;
                lablTimer.Text = "مؤقت";
                btnSystemTray.Text = "اظهار في شريط النظام";
                if (infoToolTip != null) infoToolTip.ToolTipTitle = "معلومات الاختصار";

                // Context menu translations
                miOpen.Text = "فتح";
                miExit.Text = "خروج";
                UpdateTrayToggleText();   // sets correct toggle text
            }
            else
            {
                mtbtnLang.Text = "AR/EN";
                mtbtnTheme.Text = "THEME";
                mtrBtnLogFile.Text = "LOG FILE";
                mtbtnDeveloperInfo.Text = "DEVELOPER INFO";
                bigLabel1.Text = "Connection Between You and Keyboard";
                bigLabel1.RightToLeft = RightToLeft.No;
                lablTimer.Text = "Timer";
                btnSystemTray.Text = "System Tray";
                if (infoToolTip != null) infoToolTip.ToolTipTitle = "Shortcut Info";

                miOpen.Text = "Open";
                miExit.Text = "Exit";
                UpdateTrayToggleText();
            }

            foreach (ShortcutControl ctrl in flpShortcuts.Controls)
                ctrl.ApplyLocalization(isArabic);
        }

        private void mtbtnLang_Click(object sender, EventArgs e)
        {
            isArabic = !isArabic;
            UpdateLanguage();
        }

        private void ApplyGradientBackground()
        {
            if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0) return;

            if (cachedGradient != null && this.ClientSize == lastClientSize) return;
            lastClientSize = this.ClientSize;
            cachedGradient?.Dispose();

            cachedGradient = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
            using (Graphics g = Graphics.FromImage(cachedGradient))
            {
                Color startColor = isLightTheme ? Color.White : Color.FromArgb(10, 25, 55);
                Color endColor = isLightTheme ? Color.FromArgb(150, 180, 195) : Color.FromArgb(30, 50, 50);

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    this.ClientRectangle, startColor, endColor, LinearGradientMode.ForwardDiagonal))
                {
                    g.FillRectangle(brush, this.ClientRectangle);
                }
            }

            this.BackgroundImage = cachedGradient;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            Color textColor = isLightTheme ? Color.Black : Color.White;
            Color separatorColor = isLightTheme ? Color.FromArgb(180, 180, 180) : Color.White;

            UpdateThemeColors(this, textColor, separatorColor);

            foreach (ShortcutControl ctrl in flpShortcuts.Controls)
                ctrl.ApplyTheme(textColor);
        }

        private void UpdateThemeColors(Control parent, Color textColor, Color separatorColor)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Label && !(c is LinkLabel))
                {
                    c.ForeColor = textColor;
                    c.BackColor = Color.Transparent;
                }

                if (c.Tag != null && c.Tag.ToString() == "Separator")
                {
                    c.BackColor = separatorColor;
                }

                if (c.HasChildren)
                    UpdateThemeColors(c, textColor, separatorColor);
            }
        }

        private void mtbtnTheme_Click(object sender, EventArgs e)
        {
            isLightTheme = !isLightTheme;
            ApplyGradientBackground();
        }

        private void mtrBtnLogFile_Click(object sender, EventArgs e)
        {
            ClickLogger.OpenLogFile();
        }

        // ==========================================
        // Dynamic shortcut rendering
        // ==========================================
        private void RenderShortcuts()
        {
            flpShortcuts.SuspendLayout();
            flpShortcuts.Controls.Clear();

            foreach (var data in shortcutsRegistry)
            {
                ShortcutControl shortcutItem = new ShortcutControl();

                shortcutItem.Setup(
                    data.EnglishTitle,
                    data.ArabicTitle,
                    data.LogKey,
                    data.Modifiers,
                    data.MainKey,
                    data.InfoMessageEn,
                    data.InfoMessageAr,
                    infoToolTip
                );

                shortcutItem.ApplyLocalization(isArabic);
                Color textColor = isLightTheme ? Color.Black : Color.White;
                shortcutItem.ApplyTheme(textColor);

                shortcutItem.OnExecuteRequested += ShortcutItem_OnExecuteRequested;

                flpShortcuts.Controls.Add(shortcutItem);
            }

            flpShortcuts.ResumeLayout();
            flpShortcuts_Resize(this, EventArgs.Empty);
        }

        private async void ShortcutItem_OnExecuteRequested(object sender, ShortcutExecutionArgs e)
        {
            ShortcutControl clickedShortcut = e.ControlInstance;
            Button executeBtn = (Button)clickedShortcut.Controls.Find("btnExecute", true).FirstOrDefault();

            if (executeBtn != null)
            {
                await ExecuteShortcutAsync(
                    executeBtn,
                    e.CounterLabel,
                    clickedShortcut.LogKey,
                    clickedShortcut.Modifiers,
                    clickedShortcut.MainKey
                );
            }
        }

        // ==========================================
        // System tray & close button
        // ==========================================

        // Double‑click on the tray icon restores the window and hides the icon
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            appTrayIcon.Visible = false;
            UpdateTrayToggleText();
        }

        // "System Tray" button on the form: enable the tray icon and hide the window
        private void btnSystemTray_Click(object sender, EventArgs e)
        {
            appTrayIcon.Visible = true;         // now the icon appears
            this.Hide();
            this.ShowInTaskbar = false;

            string balloonTitle = isArabic ? "تطبيق Click2Key" : "Click2Key";
            string balloonText = isArabic
                ? "التطبيق يعمل الآن في الخلفية."
                : "Application is running in the background.";

            appTrayIcon.ShowBalloonTip(2000, balloonTitle, balloonText, ToolTipIcon.Info);
            UpdateTrayToggleText();
        }

        // No longer intercept close: the X button exits the app
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);   // allows normal closing -> application exit
        }

        private void mtbtnDeveloperInfo_Click(object sender, EventArgs e)
        {
            frmAbout aboutForm = new frmAbout(isLightTheme, isArabic);
            aboutForm.ShowDialog();
        }
    }
}