using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Click2Key
{
    public partial class WpfShortcutCanvas : UserControl
    {
        // ==========================================
        // DATA & COLLECTIONS
        // ==========================================
        public ObservableCollection<ShortcutNode> Shortcuts { get; set; } = new ObservableCollection<ShortcutNode>();
        private ObservableCollection<ShortcutNode> favoritesOrdered = new ObservableCollection<ShortcutNode>();

        private List<string> favoriteKeys = new List<string>();
        private bool showFavoritesOnly = false;

        private bool isArabic;

        // ==========================================
        // EVENTS
        // ==========================================
        public event EventHandler<ShortcutNode> OnExecuteRequested;
        public event EventHandler OnLangToggleRequested;
        public event EventHandler OnThemeToggleRequested;
        public event EventHandler OnLogFileRequested;
        public event EventHandler OnSystemTrayRequested;
        public event EventHandler OnDevInfoRequested;

        // ==========================================
        // CONSTRUCTOR
        // ==========================================
        public WpfShortcutCanvas()
        {
            InitializeComponent();

            // Load the ordered list of favorite keys
            favoriteKeys = FavoriteManager.LoadFavorites();
            ShortcutList.ItemsSource = Shortcuts;   // start with "All" view
        }

        // ==========================================
        // LANGUAGE & THEME (unchanged)
        // ==========================================
        public void SetLanguage(bool isArabic)
        {
            this.isArabic = isArabic;
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

            UpdateFavoritesButton();
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
        // FAVORITES SYSTEM – ordered, no lag
        // ==========================================

        // Called after the shortcuts list is populated
        public void ApplyFavorites()
        {
            // Build a lookup for quick node access
            var lookup = Shortcuts.ToDictionary(n => n.LogKey);

            // Mark favorites and build the ordered list
            favoritesOrdered.Clear();
            foreach (var key in favoriteKeys)
            {
                if (lookup.TryGetValue(key, out var node))
                {
                    node.IsFavorite = true;
                    favoritesOrdered.Add(node);   // preserves starring order
                }
            }

            // The rest are not favorites
            foreach (var node in Shortcuts)
                if (!favoriteKeys.Contains(node.LogKey))
                    node.IsFavorite = false;

            // Initial view is "All"
            ShortcutList.ItemsSource = Shortcuts;
            UpdateFavoritesButton();
        }

        // Star/unstar a shortcut
        private void ToggleFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ShortcutNode node)
            {
                node.IsFavorite = !node.IsFavorite;
                string key = node.LogKey;

                if (node.IsFavorite)
                {
                    // Add to the end of the starred order
                    favoriteKeys.Add(key);
                    favoritesOrdered.Add(node);
                }
                else
                {
                    favoriteKeys.Remove(key);
                    favoritesOrdered.Remove(node);
                }

                // Save the new order
                Task.Run(() => FavoriteManager.SaveFavorites(favoriteKeys));

                // If currently showing only favorites, we need to rebind to reflect the change
                if (showFavoritesOnly)
                    ShortcutList.ItemsSource = new ObservableCollection<ShortcutNode>(favoritesOrdered);
                // Otherwise we don't need to change the ItemsSource; the star icon updates via binding.
            }
        }

        // Toggle between "All" and "Favorites"
        private void BtnToggleFavorites_Click(object sender, RoutedEventArgs e)
        {
            showFavoritesOnly = !showFavoritesOnly;

            if (showFavoritesOnly)
                ShortcutList.ItemsSource = new ObservableCollection<ShortcutNode>(favoritesOrdered);
            else
                ShortcutList.ItemsSource = Shortcuts;

            UpdateFavoritesButton();
        }

        private void UpdateFavoritesButton()
        {
            if (showFavoritesOnly)
            {
                btnToggleFavorites.Content = "★";
                btnToggleFavorites.ToolTip = isArabic ? "إظهار الكل" : "Show All";
            }
            else
            {
                btnToggleFavorites.Content = "☆";
                btnToggleFavorites.ToolTip = isArabic ? "إظهار المفضلة فقط" : "Show Favorites Only";
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

        // Event forwarders
        private void BtnLang_Click(object sender, RoutedEventArgs e) => OnLangToggleRequested?.Invoke(this, EventArgs.Empty);
        private void BtnTheme_Click(object sender, RoutedEventArgs e) => OnThemeToggleRequested?.Invoke(this, EventArgs.Empty);
        private void BtnLogFile_Click(object sender, RoutedEventArgs e) => OnLogFileRequested?.Invoke(this, EventArgs.Empty);
        private void BtnSystemTray_Click(object sender, RoutedEventArgs e) => OnSystemTrayRequested?.Invoke(this, EventArgs.Empty);
        private void BtnDevInfo_Click(object sender, RoutedEventArgs e) => OnDevInfoRequested?.Invoke(this, EventArgs.Empty);
    }

    // ==========================================
    // DATA MODEL
    // ==========================================
    public class ShortcutNode : INotifyPropertyChanged
    {
        public string LogKey { get; set; }
        public byte MainKey { get; set; }
        public byte[] Modifiers { get; set; }

        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string InfoEn { get; set; }
        public string InfoAr { get; set; }

        private int _executionCount;
        public int ExecutionCount
        {
            get => _executionCount;
            set { _executionCount = value; OnPropertyChanged(nameof(ExecutionCount)); }
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set { _isFavorite = value; OnPropertyChanged(nameof(IsFavorite)); }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set { _isVisible = value; OnPropertyChanged(nameof(IsVisible)); }
        }

        // UI‑bound properties
        public string DisplayTitle { get; private set; }
        public string DisplayInfo { get; private set; }
        public string ExecuteButtonText { get; private set; }

        public void ApplyLanguage(bool isArabic)
        {
            DisplayTitle = isArabic ? TitleAr : TitleEn;
            DisplayInfo = isArabic ? InfoAr : InfoEn;
            ExecuteButtonText = isArabic ? "تنفيذ" : "EXECUTE";
            OnPropertyChanged(nameof(DisplayTitle));
            OnPropertyChanged(nameof(DisplayInfo));
            OnPropertyChanged(nameof(ExecuteButtonText));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }  
}