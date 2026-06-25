using ReaLTaiizor.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Click2Key
{
    public partial class frmMain : CrownForm
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private bool isLightTheme = false;
        private bool isArabic = false;

        private NotifyIcon appTrayIcon;
        private ContextMenuStrip trayContextMenu;
        private ToolStripMenuItem miOpen, miExit, miToggleTray;

        private const uint KEYEVENTF_KEYUP = 0x0002;

        // Optional cached gradient background for the WinForms shell
        private Bitmap cachedGradient;
        private Size lastClientSize;
        private bool lastRenderedTheme;

        public frmMain()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            ApplyGradientBackground();

            ClickLogger.LoadStats();

            // System tray icon – initially hidden
            appTrayIcon = new NotifyIcon
            {
                Icon = this.Icon,
                Text = "Click2Key - Running in background",
                Visible = false
            };
            appTrayIcon.MouseDoubleClick += notifyIcon1_MouseDoubleClick;

            // Build tray context menu
            trayContextMenu = new ContextMenuStrip();
            miOpen = new ToolStripMenuItem("Open");
            miOpen.Click += (s, args) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                appTrayIcon.Visible = false;
                UpdateTrayToggleText();
            };

            miToggleTray = new ToolStripMenuItem("Turn Off System Tray");
            miToggleTray.Click += (s, args) =>
            {
                appTrayIcon.Visible = !appTrayIcon.Visible;
                UpdateTrayToggleText();
                if (!appTrayIcon.Visible && !this.Visible)
                {
                    this.Show();
                    this.ShowInTaskbar = true;
                }
            };

            miExit = new ToolStripMenuItem("Exit");
            miExit.Click += (s, args) => this.Close();

            trayContextMenu.Items.Add(miOpen);
            trayContextMenu.Items.Add(miToggleTray);
            trayContextMenu.Items.Add(new ToolStripSeparator());
            trayContextMenu.Items.Add(miExit);
            appTrayIcon.ContextMenuStrip = trayContextMenu;
        }

        // ------------------------------------------------------------
        // System tray helpers
        // ------------------------------------------------------------
        private void UpdateTrayToggleText()
        {
            if (isArabic)
                miToggleTray.Text = appTrayIcon.Visible ? "إيقاف أيقونة النظام" : "تشغيل أيقونة النظام";
            else
                miToggleTray.Text = appTrayIcon.Visible ? "Turn Off System Tray" : "Turn On System Tray";
        }

        // ------------------------------------------------------------
        // Keyboard simulation
        // ------------------------------------------------------------
        private void SimulateKeystroke(byte[] modifiers, byte mainKey)
        {
            if (modifiers != null)
                foreach (byte mod in modifiers)
                    keybd_event(mod, 0, 0, 0);

            keybd_event(mainKey, 0, 0, 0);
            keybd_event(mainKey, 0, KEYEVENTF_KEYUP, 0);

            if (modifiers != null)
                for (int i = modifiers.Length - 1; i >= 0; i--)
                    keybd_event(modifiers[i], 0, KEYEVENTF_KEYUP, 0);
        }

        // ------------------------------------------------------------
        // Form Load – initialise WPF control
        // ------------------------------------------------------------
        private void frmMain_Load(object sender, EventArgs e)
        {
            var wpfCanvas = elementHost1.Child as WpfShortcutCanvas;
            if (wpfCanvas == null) return;

            // Wire WPF events
            wpfCanvas.OnExecuteRequested += WpfCanvas_OnExecuteRequested;
            wpfCanvas.OnLangToggleRequested += (s, args) => ToggleLanguage();
            wpfCanvas.OnThemeToggleRequested += (s, args) => ToggleTheme();
            wpfCanvas.OnLogFileRequested += (s, args) => ClickLogger.OpenLogFile();
            wpfCanvas.OnSystemTrayRequested += (s, args) => MinimizeToTray();
            wpfCanvas.OnDevInfoRequested += (s, args) => ShowDeveloperInfo();

            // Populate shortcuts
            var rawShortcuts = ShortcutsRepository.GetAll();
            foreach (var sc in rawShortcuts)
            {
                var node = new ShortcutNode
                {
                    LogKey = sc.LogKey,
                    MainKey = sc.MainKey,
                    Modifiers = sc.Modifiers,
                    TitleEn = sc.EnglishTitle,
                    TitleAr = sc.ArabicTitle,
                    InfoEn = sc.InfoMessageEn,
                    InfoAr = sc.InfoMessageAr,
                    ExecutionCount = ClickLogger.GetCount(sc.LogKey)
                };
                node.ApplyLanguage(isArabic);
                wpfCanvas.Shortcuts.Add(node);
            }

            // Apply initial theme & language
            wpfCanvas.SetTheme(isLightTheme);
            wpfCanvas.SetLanguage(isArabic);
        }

        // ------------------------------------------------------------
        // WPF event handlers
        // ------------------------------------------------------------
        private async void WpfCanvas_OnExecuteRequested(object sender, ShortcutNode node)
        {
            var wpfCanvas = elementHost1.Child as WpfShortcutCanvas;
            if (wpfCanvas == null) return;

            SimulateKeystroke(node.Modifiers, node.MainKey);
            ClickLogger.LogClick(node.LogKey);

            node.ExecutionCount = ClickLogger.GetCount(node.LogKey);
            wpfCanvas.ShortcutList.Items.Refresh();
        }

        // ------------------------------------------------------------
        // Stub methods for old WinForms buttons (keep the designer happy)
        // ------------------------------------------------------------
        private void mtbtnLang_Click(object sender, EventArgs e) => ToggleLanguage();
        private void mtbtnTheme_Click(object sender, EventArgs e) => ToggleTheme();
        private void mtrBtnLogFile_Click(object sender, EventArgs e) => ClickLogger.OpenLogFile();
        private void btnSystemTray_Click(object sender, EventArgs e) => MinimizeToTray();
        private void mtbtnDeveloperInfo_Click(object sender, EventArgs e) => ShowDeveloperInfo();

        // ------------------------------------------------------------
        // Core toggle logic
        // ------------------------------------------------------------
        private void ToggleLanguage()
        {
            isArabic = !isArabic;
            UpdateTrayToggleText();
            var wpfCanvas = elementHost1.Child as WpfShortcutCanvas;
            wpfCanvas?.SetLanguage(isArabic);
        }

        private void ToggleTheme()
        {
            isLightTheme = !isLightTheme;
            var wpfCanvas = elementHost1.Child as WpfShortcutCanvas;
            wpfCanvas?.SetTheme(isLightTheme);
            ApplyGradientBackground();
        }

        private void MinimizeToTray()
        {
            appTrayIcon.Visible = true;
            this.Hide();
            this.ShowInTaskbar = false;

            string balloonTitle = isArabic ? "تطبيق Click2Key" : "Click2Key";
            string balloonText = isArabic
                ? "التطبيق يعمل الآن في الخلفية."
                : "Application is running in the background.";
            appTrayIcon.ShowBalloonTip(2000, balloonTitle, balloonText, ToolTipIcon.Info);
            UpdateTrayToggleText();
        }

        private void ShowDeveloperInfo()
        {
            frmAbout aboutForm = new frmAbout(isLightTheme, isArabic);
            aboutForm.ShowDialog();
        }

        // ------------------------------------------------------------
        // Tray restore & close
        // ------------------------------------------------------------
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            appTrayIcon.Visible = false;
            UpdateTrayToggleText();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e); // X closes the app
        }

        // ------------------------------------------------------------
        // Optional gradient background for the WinForms shell
        // ------------------------------------------------------------
        private void ApplyGradientBackground()
        {
            if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0) return;
            if (cachedGradient != null && this.ClientSize == lastClientSize && isLightTheme == lastRenderedTheme) return;

            lastClientSize = this.ClientSize;
            lastRenderedTheme = isLightTheme;
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
        }
    }
}