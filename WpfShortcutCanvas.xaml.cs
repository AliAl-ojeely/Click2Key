using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Click2Key
{
    public partial class WpfShortcutCanvas : UserControl
    {
        public ObservableCollection<ShortcutNode> Shortcuts { get; set; } = new ObservableCollection<ShortcutNode>();

        public event EventHandler<ShortcutNode> OnExecuteRequested;
        public event EventHandler OnLangToggleRequested;
        public event EventHandler OnThemeToggleRequested;
        public event EventHandler OnLogFileRequested;
        public event EventHandler OnSystemTrayRequested;
        public event EventHandler OnDevInfoRequested;

        public WpfShortcutCanvas()
        {
            InitializeComponent();
            ShortcutList.ItemsSource = Shortcuts;
        }

        // ==========================================
        // DYNAMIC UI UPDATES
        // ==========================================
        public void SetLanguage(bool isArabic)
        {
            // Header texts
            txtMainTitle.Text = isArabic ? "ارتباط بينك و بين لوحة المفاتيح" : "Connection Between You and Keyboard";
            txtMainTitle.FlowDirection = isArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            btnLang.Content = isArabic ? "EN/AR" : "AR/EN";
            btnTheme.Content = isArabic ? "المظهر" : "THEME";
            btnLogFile.Content = isArabic ? "سجل العمليات" : "LOG FILE";
            btnSystemTray.Content = isArabic ? "شريط النظام" : "SYSTEM TRAY";
            btnDevInfo.Content = isArabic ? "معلومات المطور" : "DEVELOPER INFO";
            txtTimerLabel.Text = isArabic ? "مؤقت:" : "Timer:";

            foreach (var node in Shortcuts)
                node.ApplyLanguage(isArabic);

            ShortcutList.Items.Refresh();
        }

        public void SetTheme(bool isLight)
        {
            var bc = new BrushConverter();
            if (isLight)
            {
                this.Resources["AppBackground"] = (SolidColorBrush)bc.ConvertFrom("#F4F4F5");
                this.Resources["CardBackground"] = (SolidColorBrush)bc.ConvertFrom("#FFFFFF");
                this.Resources["PrimaryText"] = (SolidColorBrush)bc.ConvertFrom("#18181B");
                this.Resources["SecondaryText"] = (SolidColorBrush)bc.ConvertFrom("#71717A");
                this.Resources["BorderColor"] = (SolidColorBrush)bc.ConvertFrom("#E4E4E7");
                this.Resources["NavButtonBg"] = (SolidColorBrush)bc.ConvertFrom("#E4E4E7");
                this.Resources["NavButtonHover"] = (SolidColorBrush)bc.ConvertFrom("#D4D4D8");
                this.Resources["ComboBg"] = (SolidColorBrush)bc.ConvertFrom("#FFFFFF");
                this.Resources["ComboFg"] = (SolidColorBrush)bc.ConvertFrom("#18181B");
            }
            else
            {
                this.Resources["AppBackground"] = (SolidColorBrush)bc.ConvertFrom("#09090B");
                this.Resources["CardBackground"] = (SolidColorBrush)bc.ConvertFrom("#18181B");
                this.Resources["PrimaryText"] = (SolidColorBrush)bc.ConvertFrom("#F4F4F5");
                this.Resources["SecondaryText"] = (SolidColorBrush)bc.ConvertFrom("#A1A1AA");
                this.Resources["BorderColor"] = (SolidColorBrush)bc.ConvertFrom("#27272A");
                this.Resources["NavButtonBg"] = (SolidColorBrush)bc.ConvertFrom("#27272A");
                this.Resources["NavButtonHover"] = (SolidColorBrush)bc.ConvertFrom("#3F3F46");
                this.Resources["ComboBg"] = (SolidColorBrush)bc.ConvertFrom("#27272A");
                this.Resources["ComboFg"] = (SolidColorBrush)bc.ConvertFrom("#F4F4F5");
            }
        }

        // ==========================================
        // EVENTS
        // ==========================================
        private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn) || !(btn.DataContext is ShortcutNode node))
                return;

            // 1. Disable button & start countdown
            btn.IsEnabled = false;
            string originalContent = btn.Content.ToString();
            int delay = GetSelectedDelay();

            try
            {
                for (int i = delay; i > 0; i--)
                {
                    btn.Content = $"Wait {i}s…";
                    await Task.Delay(1000);
                }

                // 2. Execute the shortcut
                OnExecuteRequested?.Invoke(this, node);
            }
            finally
            {
                // 3. Restore button (UI thread not guaranteed, but Content set works on dispatcher)
                btn.Dispatcher.Invoke(() =>
                {
                    btn.Content = originalContent;
                    btn.IsEnabled = true;
                });
            }
        }

        public int GetSelectedDelay()
        {
            if (cmbTimerDelay.SelectedItem is ComboBoxItem item &&
                int.TryParse(item.Content.ToString(), out int delay))
                return delay;
            return 0;
        }

        private void BtnLang_Click(object sender, RoutedEventArgs e) => OnLangToggleRequested?.Invoke(this, EventArgs.Empty);
        private void BtnTheme_Click(object sender, RoutedEventArgs e) => OnThemeToggleRequested?.Invoke(this, EventArgs.Empty);
        private void BtnLogFile_Click(object sender, RoutedEventArgs e) => OnLogFileRequested?.Invoke(this, EventArgs.Empty);
        private void BtnSystemTray_Click(object sender, RoutedEventArgs e) => OnSystemTrayRequested?.Invoke(this, EventArgs.Empty);
        private void BtnDevInfo_Click(object sender, RoutedEventArgs e) => OnDevInfoRequested?.Invoke(this, EventArgs.Empty);
    }

    // ==========================================
    // DATA MODEL
    // ==========================================
    public class ShortcutNode
    {
        public string LogKey { get; set; }
        public byte MainKey { get; set; }
        public byte[] Modifiers { get; set; }

        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string InfoEn { get; set; }
        public string InfoAr { get; set; }

        public int ExecutionCount { get; set; }

        // UI‑bound properties
        public string DisplayTitle { get; private set; }
        public string DisplayInfo { get; private set; }
        public string ExecuteButtonText { get; private set; }

        public void ApplyLanguage(bool isArabic)
        {
            DisplayTitle = isArabic ? TitleAr : TitleEn;
            DisplayInfo = isArabic ? InfoAr : InfoEn;
            ExecuteButtonText = isArabic ? "تنفيذ" : "EXECUTE";
        }
    }
}