using ReaLTaiizor.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Click2Key
{
    public partial class frmMain : CrownForm
    {
        // ==========================================
        // WIN32 IMPORTS (SendInput for reliable simulation)
        // ==========================================
        // ==========================================
        // WIN32 IMPORTS (keybd_event for reliable simulation)
        // ==========================================
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool LockWorkStation();

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);
        private const uint MAPVK_VK_TO_VSC = 0;

        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        struct INPUT
        {
            public uint type;
            public MOUSEKEYBDHARDWAREUNION mkhi;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct MOUSEKEYBDHARDWAREUNION
        {
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT { /* not used */ }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT { /* not used */ }


        // ==========================================
        // FIELDS
        // ==========================================
        private bool isLightTheme = false;
        private bool isArabic = false;

        private NotifyIcon appTrayIcon;
        private ContextMenuStrip trayContextMenu;
        private ToolStripMenuItem miOpen, miExit, miToggleTray;

        private Bitmap cachedGradient;
        private Size lastClientSize;
        private bool lastRenderedTheme;

        private const string AppEventName = "Click2Key_RestoreEvent";
        private EventWaitHandle restoreEvent;
        private System.Windows.Forms.Timer eventCheckTimer;
        private bool _exitRequested = false;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // WS_EX_NOACTIVATE: Prevents the form from taking focus when clicked
                const int WS_EX_NOACTIVATE = 0x08000000;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }

        public frmMain()
        {
            InitializeComponent();

            

            // ==========================================
            // WINDOW PLACEMENT LOGIC
            // ==========================================
            // Load state BEFORE the form handle is drawn to prevent the top-left flicker
            bool loaded = AppSettings.LoadWindowState(this);

            if (!loaded || this.ClientSize.Width == 0)
            {
                // First run (or missing file): Natively center the form
                this.StartPosition = FormStartPosition.CenterScreen;

                // Calculate 80% of the primary screen
                Rectangle area = Screen.PrimaryScreen.WorkingArea;
                this.ClientSize = new Size((int)(area.Width * 0.8), (int)(area.Height * 0.8));
            }
            else
            {
                // Saved state found: Tell Windows to use the exact saved coordinates
                this.StartPosition = FormStartPosition.Manual;
            }

            // Save position/size when the user moves or resizes the window
            this.ResizeEnd += (s, e) => AppSettings.SaveWindowState(this);
            this.Move += (s, e) => AppSettings.SaveWindowState(this);

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

            // Tray context menu
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

            // Single‑instance restore listener
            restoreEvent = new EventWaitHandle(false, EventResetMode.AutoReset, AppEventName);
            eventCheckTimer = new System.Windows.Forms.Timer { Interval = 100 };
            eventCheckTimer.Tick += (s, e) =>
            {
                if (restoreEvent.WaitOne(0))
                    this.Invoke((Action)(() => RestoreFromTray()));
            };
            eventCheckTimer.Start();

            // Wire WPF events
            wpfShortcutCanvas1.OnExecuteRequested += WpfCanvas_OnExecuteRequested;
            wpfShortcutCanvas1.OnLangToggleRequested += (s, args) => ToggleLanguage();
            wpfShortcutCanvas1.OnThemeToggleRequested += (s, args) => ToggleTheme();
            wpfShortcutCanvas1.OnLogFileRequested += (s, args) => ClickLogger.OpenLogFile();
            wpfShortcutCanvas1.OnSystemTrayRequested += (s, args) => MinimizeToTray();
            wpfShortcutCanvas1.OnDevInfoRequested += (s, args) => ShowDeveloperInfo();
        }

        // ==========================================
        // SYSTEM TRAY HELPERS
        // ==========================================
        private void UpdateTrayToggleText()
        {
            if (isArabic)
                miToggleTray.Text = appTrayIcon.Visible ? "إيقاف أيقونة النظام" : "تشغيل أيقونة النظام";
            else
                miToggleTray.Text = appTrayIcon.Visible ? "Turn Off System Tray" : "Turn On System Tray";
        }

        // ==========================================
        // KEYBOARD SIMULATION (SendInput – reliable)
        // ==========================================
        // ==========================================
        // KEYBOARD SIMULATION (keybd_event)
        // ==========================================
        private void SimulateKeystroke(byte[] modifiers, byte mainKey)
        {
            // 1. Press Modifiers (Ctrl, Alt, Win, Shift)
            if (modifiers != null)
            {
                foreach (byte mod in modifiers)
                {
                    keybd_event(mod, (byte)MapVirtualKey(mod, MAPVK_VK_TO_VSC), GetKeyFlags(mod, true), UIntPtr.Zero);
                }
            }

            // 2. Press Main Key
            keybd_event(mainKey, (byte)MapVirtualKey(mainKey, MAPVK_VK_TO_VSC), GetKeyFlags(mainKey, true), UIntPtr.Zero);

            // 3. Release Main Key
            keybd_event(mainKey, (byte)MapVirtualKey(mainKey, MAPVK_VK_TO_VSC), GetKeyFlags(mainKey, false), UIntPtr.Zero);

            // 4. Release Modifiers (In reverse order)
            if (modifiers != null)
            {
                for (int i = modifiers.Length - 1; i >= 0; i--)
                {
                    byte mod = modifiers[i];
                    keybd_event(mod, (byte)MapVirtualKey(mod, MAPVK_VK_TO_VSC), GetKeyFlags(mod, false), UIntPtr.Zero);
                }
            }
        }

        private uint GetKeyFlags(byte vk, bool isKeyDown)
        {
            uint flags = isKeyDown ? KEYEVENTF_KEYDOWN : KEYEVENTF_KEYUP;

            // Standard extended keys (Win keys, Alt, Ctrl, Arrows, Home/End, etc.)
            bool isExtended = (vk == 0x5B || vk == 0x5C || vk == 0x12 || vk == 0x11 ||
                               vk == 0x25 || vk == 0x26 || vk == 0x27 || vk == 0x28 ||
                               vk == 0x21 || vk == 0x22 || vk == 0x23 || vk == 0x24 ||
                               vk == 0x2D || vk == 0x2E);

            if (isExtended)
            {
                flags |= KEYEVENTF_EXTENDEDKEY;
            }

            return flags;
        }

        // ==========================================
        // FORM LOAD – restore saved size/position
        // ==========================================
        private void frmMain_Load(object sender, EventArgs e)
        {
            var wpfCanvas = wpfShortcutCanvas1;
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

        // ==========================================
        // WPF EVENT HANDLERS
        // ==========================================
        private async void WpfCanvas_OnExecuteRequested(object sender, ShortcutNode node)
        {
            // Add a 150ms buffer to allow the physical mouse click to release
            await System.Threading.Tasks.Task.Delay(150);

            if (node.LogKey == "Shortcut_Win_L")
            {
                LockWorkStation();
            }
            else
            {
                SimulateKeystroke(node.Modifiers, node.MainKey);
            }

            ClickLogger.LogClick(node.LogKey);

            node.ExecutionCount = ClickLogger.GetCount(node.LogKey);
            wpfShortcutCanvas1?.ShortcutList.Items.Refresh();

            this.SendToBack();
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

        // ==========================================
        // TRAY RESTORE & CLOSE
        // ==========================================
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
            // Save final window position & size before shutting down
            AppSettings.SaveWindowState(this);

            // Clean up background listeners
            eventCheckTimer?.Stop();
            restoreEvent?.Dispose();

            // Hide and dispose of the tray icon so it doesn't leave a "ghost" icon 
            // in the Windows taskbar after the app closes
            if (appTrayIcon != null)
            {
                appTrayIcon.Visible = false;
                appTrayIcon.Dispose();
            }

            // Allow the form to close naturally
            base.OnFormClosing(e);
        }

        // ==========================================
        // GRADIENT BACKGROUND (unchanged)
        // ==========================================
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