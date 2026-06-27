using ReaLTaiizor.Forms;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
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

        private const string AppEventName = "Click2Key_RestoreEvent";
        private EventWaitHandle restoreEvent;
        private System.Windows.Forms.Timer eventCheckTimer;

        // True when the user selected "Exit" from the tray menu → allow actual closing
        private bool _exitRequested = false;

        public frmMain()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;   // fallback centering

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
            miOpen.Click += (s, args) => RestoreFromTray();

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
            miExit.Click += (s, args) =>
            {
                _exitRequested = true;
                this.Close();
            };

            trayContextMenu.Items.Add(miOpen);
            trayContextMenu.Items.Add(miToggleTray);
            trayContextMenu.Items.Add(new ToolStripSeparator());
            trayContextMenu.Items.Add(miExit);
            appTrayIcon.ContextMenuStrip = trayContextMenu;

            // Set up the restore‑event listener (another instance asks us to restore)
            restoreEvent = new EventWaitHandle(false, EventResetMode.AutoReset, AppEventName);
            eventCheckTimer = new System.Windows.Forms.Timer { Interval = 100 };
            eventCheckTimer.Tick += (s, e) =>
            {
                if (restoreEvent.WaitOne(0))
                {
                    this.Invoke((Action)(() => RestoreFromTray()));
                }
            };
            eventCheckTimer.Start();

            // ------------------------------------------------------------
            // 🔥 WIRE WPF EVENTS DIRECTLY HERE – BEFORE FORM IS SHOWN
            // ------------------------------------------------------------
            wpfShortcutCanvas1.OnExecuteRequested += WpfCanvas_OnExecuteRequested;
            wpfShortcutCanvas1.OnLangToggleRequested += (s, args) => ToggleLanguage();
            wpfShortcutCanvas1.OnThemeToggleRequested += (s, args) => ToggleTheme();
            wpfShortcutCanvas1.OnLogFileRequested += (s, args) => ClickLogger.OpenLogFile();
            wpfShortcutCanvas1.OnSystemTrayRequested += (s, args) => MinimizeToTray();
            wpfShortcutCanvas1.OnDevInfoRequested += (s, args) => ShowDeveloperInfo();
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
        // Form Load – only data loading (events already wired in constructor)
        // ------------------------------------------------------------
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Restore window state (saved position & size)
            AppSettings.LoadWindowState(this);
            if (this.WindowState == FormWindowState.Normal && this.ClientSize.Width == 0)
                AppSettings.LoadWindowState(this);   // force default if settings missing

            var wpfCanvas = wpfShortcutCanvas1;       // direct reference, no cast needed
            if (wpfCanvas == null) return;

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

            wpfCanvas.ApplyFavorites();
            wpfCanvas.SetTheme(isLightTheme);
            wpfCanvas.SetLanguage(isArabic);
        }

        // ------------------------------------------------------------
        // WPF event handlers
        // ------------------------------------------------------------
        private async void WpfCanvas_OnExecuteRequested(object sender, ShortcutNode node)
        {
            var wpfCanvas = wpfShortcutCanvas1;
            if (wpfCanvas == null) return;

            SimulateKeystroke(node.Modifiers, node.MainKey);
            ClickLogger.LogClick(node.LogKey);

            node.ExecutionCount = ClickLogger.GetCount(node.LogKey);
            wpfCanvas.ShortcutList.Items.Refresh();
        }

        private void ToggleLanguage()
        {
            isArabic = !isArabic;
            UpdateTrayToggleText();
            wpfShortcutCanvas1?.SetLanguage(isArabic);
        }

        private void ToggleTheme()
        {
            isLightTheme = !isLightTheme;
            wpfShortcutCanvas1?.SetTheme(isLightTheme);
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
        private void RestoreFromTray()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            appTrayIcon.Visible = false;
            this.Activate();
            UpdateTrayToggleText();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RestoreFromTray();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_exitRequested)
            {
                // Real exit – clean up and allow closing
                eventCheckTimer?.Stop();
                restoreEvent?.Dispose();
                base.OnFormClosing(e);
                return;
            }

            // Normal close (X button, Alt+F4) → hide to tray
            e.Cancel = true;
            this.Hide();
            this.ShowInTaskbar = false;
            appTrayIcon.Visible = true;
            UpdateTrayToggleText();
        }

        // ------------------------------------------------------------
        // Optional gradient background
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