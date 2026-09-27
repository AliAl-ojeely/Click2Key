using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Click2Key
{
    // ============================================================
    // THEME PALETTE
    // ============================================================

    public sealed class ThemePalette : INotifyPropertyChanged
    {
        private static readonly ThemePalette instance =
            new ThemePalette();

        public static ThemePalette Instance
        {
            get { return instance; }
        }


        private Color appBackgroundColor;
        private Color cardBackgroundColor;

        private Color primaryTextColor;
        private Color secondaryTextColor;

        private Color borderColor;

        private Color navButtonBackgroundColor;
        private Color navButtonHoverColor;

        private Color comboBackgroundColor;
        private Color comboForegroundColor;


        public Color AppBackgroundColor
        {
            get { return appBackgroundColor; }
            private set
            {
                SetColor(
                    ref appBackgroundColor,
                    value);
            }
        }


        public Color CardBackgroundColor
        {
            get { return cardBackgroundColor; }
            private set
            {
                SetColor(
                    ref cardBackgroundColor,
                    value);
            }
        }


        public Color PrimaryTextColor
        {
            get { return primaryTextColor; }
            private set
            {
                SetColor(
                    ref primaryTextColor,
                    value);
            }
        }


        public Color SecondaryTextColor
        {
            get { return secondaryTextColor; }
            private set
            {
                SetColor(
                    ref secondaryTextColor,
                    value);
            }
        }


        public Color BorderColor
        {
            get { return borderColor; }
            private set
            {
                SetColor(
                    ref borderColor,
                    value);
            }
        }


        public Color NavButtonBackgroundColor
        {
            get { return navButtonBackgroundColor; }
            private set
            {
                SetColor(
                    ref navButtonBackgroundColor,
                    value);
            }
        }


        public Color NavButtonHoverColor
        {
            get { return navButtonHoverColor; }
            private set
            {
                SetColor(
                    ref navButtonHoverColor,
                    value);
            }
        }


        public Color ComboBackgroundColor
        {
            get { return comboBackgroundColor; }
            private set
            {
                SetColor(
                    ref comboBackgroundColor,
                    value);
            }
        }


        public Color ComboForegroundColor
        {
            get { return comboForegroundColor; }
            private set
            {
                SetColor(
                    ref comboForegroundColor,
                    value);
            }
        }


        private ThemePalette()
        {
            ApplyDarkTheme();
        }


        public void Apply(bool isLight)
        {
            if (isLight)
            {
                ApplyLightTheme();
            }
            else
            {
                ApplyDarkTheme();
            }
        }


        private void ApplyDarkTheme()
        {
            AppBackgroundColor =
                ParseColor("#09090B");

            CardBackgroundColor =
                ParseColor("#18181B");

            PrimaryTextColor =
                ParseColor("#F4F4F5");

            SecondaryTextColor =
                ParseColor("#A1A1AA");

            BorderColor =
                ParseColor("#27272A");

            NavButtonBackgroundColor =
                ParseColor("#27272A");

            NavButtonHoverColor =
                ParseColor("#3F3F46");

            ComboBackgroundColor =
                ParseColor("#27272A");

            ComboForegroundColor =
                ParseColor("#F4F4F5");
        }


        private void ApplyLightTheme()
        {
            AppBackgroundColor =
                ParseColor("#F4F4F5");

            CardBackgroundColor =
                ParseColor("#FFFFFF");

            PrimaryTextColor =
                ParseColor("#18181B");

            SecondaryTextColor =
                ParseColor("#71717A");

            BorderColor =
                ParseColor("#E4E4E7");

            NavButtonBackgroundColor =
                ParseColor("#E4E4E7");

            NavButtonHoverColor =
                ParseColor("#D4D4D8");

            ComboBackgroundColor =
                ParseColor("#FFFFFF");

            ComboForegroundColor =
                ParseColor("#18181B");
        }


        private static Color ParseColor(
            string value)
        {
            return
                (Color)ColorConverter.ConvertFromString(
                    value);
        }


        private void SetColor(
            ref Color field,
            Color value,
            [CallerMemberName]
            string propertyName = null)
        {
            if (field == value)
                return;

            field = value;

            OnPropertyChanged(
                propertyName);
        }


        public event PropertyChangedEventHandler
            PropertyChanged;


        private void OnPropertyChanged(
            string propertyName)
        {
            PropertyChangedEventHandler handler =
                PropertyChanged;

            if (handler != null)
            {
                handler(
                    this,
                    new PropertyChangedEventArgs(
                        propertyName));
            }
        }
    }


    // ============================================================
    // SORT MODES
    // ============================================================

    public enum ShortcutSortMode
    {
        DefaultOrder,

        NameAscending,
        NameDescending,

        MostUsed,
        LeastUsed,

        FavoritesFirst
    }


    // ============================================================
    // MAIN CONTROL
    // ============================================================

    public partial class WpfShortcutCanvas : UserControl
    {
        // ========================================================
        // MASTER COLLECTION
        // ========================================================

        public ObservableCollection<ShortcutNode> Shortcuts
        {
            get;
            private set;
        }


        // ========================================================
        // FAVORITES
        // ========================================================

        private List<string> favoriteKeys;

        private HashSet<string> favoriteKeySet;


        private readonly Dictionary<string, int>
            favoriteOrderLookup =
            new Dictionary<string, int>(
                StringComparer.Ordinal);


        private CancellationTokenSource
            favoriteSaveCancellation;


        // ========================================================
        // MASTER ORDER
        // ========================================================

        private readonly Dictionary<ShortcutNode, int>
            masterOrderLookup =
            new Dictionary<ShortcutNode, int>();


        // ========================================================
        // PROPERTY SUBSCRIPTIONS
        // ========================================================

        private readonly HashSet<ShortcutNode>
            subscribedNodes =
            new HashSet<ShortcutNode>();


        // ========================================================
        // COLLECTION VIEW
        // ========================================================

        private ListCollectionView shortcutView;

        private ShortcutNodeComparer shortcutComparer;


        // ========================================================
        // VIEW STATE
        // ========================================================

        private bool showFavoritesOnly =
            false;

        private bool isArabic =
            false;


        private ShortcutCategory selectedCategory =
            ShortcutCategory.None;


        private ShortcutSortMode selectedSort =
            ShortcutSortMode.DefaultOrder;


        private bool updatingFilterControls =
            false;


        // ========================================================
        // RESPONSIVE SHORTCUT GRID
        //
        // Large  = 4 columns
        // Medium = 2 columns
        // Small  = 1 column
        //
        // No 3-column state.
        // ========================================================

        private const double ShortcutColumnWidth =
            270.0;

        // Breakpoints based on the real UserControl width.
        private const double FourColumnBreakpoint =
            1180.0;

        private const double TwoColumnBreakpoint =
            650.0;

        private int currentShortcutColumns =
            0;


        // ========================================================
        // EVENTS
        // ========================================================

        public event EventHandler<ShortcutNode>
            OnExecuteRequested;


        public event EventHandler
            OnLangToggleRequested;


        public event EventHandler
            OnThemeToggleRequested;


        public event EventHandler
            OnLogFileRequested;


        public event EventHandler
            OnSystemTrayRequested;


        public event EventHandler
            OnDevInfoRequested;


        // ========================================================
        // CONSTRUCTOR
        // ========================================================

        public WpfShortcutCanvas()
        {
            // ----------------------------------------------------
            // MASTER COLLECTION
            // ----------------------------------------------------

            Shortcuts =
                new ObservableCollection<ShortcutNode>();


            // ----------------------------------------------------
            // CREATE XAML
            // ----------------------------------------------------

            InitializeComponent();


            // ----------------------------------------------------
            // FAVORITES
            // ----------------------------------------------------

            favoriteKeys =
                FavoriteManager.LoadFavorites();


            if (favoriteKeys == null)
            {
                favoriteKeys =
                    new List<string>();
            }


            favoriteKeySet =
                new HashSet<string>(
                    favoriteKeys,
                    StringComparer.Ordinal);


            RebuildFavoriteOrderLookup();


            // ----------------------------------------------------
            // WATCH COLLECTION
            // ----------------------------------------------------

            Shortcuts.CollectionChanged +=
                Shortcuts_CollectionChanged;


            // ----------------------------------------------------
            // COLLECTION VIEW
            // ----------------------------------------------------

            shortcutView =
                new ListCollectionView(
                    Shortcuts);


            shortcutView.Filter =
                FilterShortcut;


            shortcutComparer =
                new ShortcutNodeComparer(
                    this);


            shortcutView.CustomSort =
                shortcutComparer;


            ShortcutList.ItemsSource =
                shortcutView;


            // ----------------------------------------------------
            // FILTER CONTROLS
            // ----------------------------------------------------

            PopulateFilterControls();

            // ----------------------------------------------------
            // RESPONSIVE LAYOUT
            // ----------------------------------------------------

            // Listen to the REAL control width.
            SizeChanged -=
                WpfShortcutCanvas_SizeChanged;

            SizeChanged +=
                WpfShortcutCanvas_SizeChanged;


            // Keep this too in case scrollbar/layout changes
            // affect the available area.
            ShortcutScrollViewer.SizeChanged -=
                ShortcutScrollViewer_SizeChanged;

            ShortcutScrollViewer.SizeChanged +=
                ShortcutScrollViewer_SizeChanged;


            // Initial calculation after WPF completes layout.
            Loaded +=
                delegate
                {
                    UpdateShortcutColumns();
                };


            // ----------------------------------------------------
            // RESPONSIVE GRID
            //
            // Remove any existing XAML subscription first.
            // This makes the code safe even if SizeChanged=""
            // still exists in XAML.
            // ----------------------------------------------------

            ShortcutScrollViewer.SizeChanged -=
                ShortcutScrollViewer_SizeChanged;


            ShortcutScrollViewer.SizeChanged +=
                ShortcutScrollViewer_SizeChanged;


            Loaded +=
                delegate
                {
                    UpdateShortcutColumns();
                };
        }


        // ========================================================
        // COLLECTION CHANGED
        // ========================================================

        private void Shortcuts_CollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs e)
        {
            // ----------------------------------------------------
            // RESET
            // ----------------------------------------------------

            if (e.Action ==
                NotifyCollectionChangedAction.Reset)
            {
                foreach (ShortcutNode oldNode
                    in subscribedNodes.ToList())
                {
                    oldNode.PropertyChanged -=
                        ShortcutNode_PropertyChanged;
                }


                subscribedNodes.Clear();


                foreach (ShortcutNode node
                    in Shortcuts)
                {
                    SubscribeNode(
                        node);
                }


                RebuildMasterOrderLookup();


                RefreshShortcutView();


                return;
            }


            // ----------------------------------------------------
            // REMOVED
            // ----------------------------------------------------

            if (e.OldItems != null)
            {
                foreach (object item
                    in e.OldItems)
                {
                    ShortcutNode node =
                        item as ShortcutNode;


                    if (node == null)
                        continue;


                    UnsubscribeNode(
                        node);


                    masterOrderLookup.Remove(
                        node);
                }
            }


            // ----------------------------------------------------
            // ADDED
            // ----------------------------------------------------

            if (e.NewItems != null)
            {
                foreach (object item
                    in e.NewItems)
                {
                    ShortcutNode node =
                        item as ShortcutNode;


                    if (node == null)
                        continue;


                    SubscribeNode(
                        node);
                }
            }


            RebuildMasterOrderLookup();
        }


        // ========================================================
        // PROPERTY SUBSCRIPTIONS
        // ========================================================

        private void SubscribeNode(
            ShortcutNode node)
        {
            if (node == null)
                return;


            if (!subscribedNodes.Add(node))
                return;


            node.PropertyChanged +=
                ShortcutNode_PropertyChanged;
        }


        private void UnsubscribeNode(
            ShortcutNode node)
        {
            if (node == null)
                return;


            if (!subscribedNodes.Remove(node))
                return;


            node.PropertyChanged -=
                ShortcutNode_PropertyChanged;
        }


        private void ShortcutNode_PropertyChanged(
            object sender,
            PropertyChangedEventArgs e)
        {
            // Only re-sort automatically if the active
            // sorting mode depends on ExecutionCount.

            if (e.PropertyName ==
                nameof(ShortcutNode.ExecutionCount))
            {
                if (selectedSort ==
                        ShortcutSortMode.MostUsed ||
                    selectedSort ==
                        ShortcutSortMode.LeastUsed)
                {
                    RefreshShortcutView();
                }
            }
        }


        // ========================================================
        // MASTER ORDER CACHE
        // ========================================================

        private void RebuildMasterOrderLookup()
        {
            masterOrderLookup.Clear();


            for (int i = 0;
                 i < Shortcuts.Count;
                 i++)
            {
                masterOrderLookup[
                    Shortcuts[i]] = i;
            }
        }


        private int GetMasterIndex(
            ShortcutNode node)
        {
            if (node == null)
                return int.MaxValue;


            int index;


            if (masterOrderLookup.TryGetValue(
                node,
                out index))
            {
                return index;
            }


            return int.MaxValue;
        }


        // ========================================================
        // FAVORITE ORDER CACHE
        // ========================================================

        private void RebuildFavoriteOrderLookup()
        {
            favoriteOrderLookup.Clear();


            for (int i = 0;
                 i < favoriteKeys.Count;
                 i++)
            {
                string key =
                    favoriteKeys[i];


                if (string.IsNullOrWhiteSpace(
                    key))
                {
                    continue;
                }


                if (!favoriteOrderLookup.ContainsKey(
                    key))
                {
                    favoriteOrderLookup[
                        key] = i;
                }
            }
        }


        private int GetFavoriteOrder(
            ShortcutNode node)
        {
            if (node == null)
                return int.MaxValue;


            if (string.IsNullOrEmpty(
                node.LogKey))
            {
                return int.MaxValue;
            }


            int index;


            if (favoriteOrderLookup.TryGetValue(
                node.LogKey,
                out index))
            {
                return index;
            }


            return int.MaxValue;
        }


        // ========================================================
        // LANGUAGE
        // ========================================================

        public void SetLanguage(
            bool isArabic)
        {
            this.isArabic =
                isArabic;


            // ----------------------------------------------------
            // TITLE
            // ----------------------------------------------------

            txtMainTitle.Text =
                isArabic
                    ? "ارتباط بينك و بين لوحة المفاتيح"
                    : "Connection Between You and Keyboard";


            txtMainTitle.FlowDirection =
                isArabic
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight;


            txtMainTitle.TextAlignment =
                TextAlignment.Center;


            // ----------------------------------------------------
            // CONTROLS
            // ----------------------------------------------------

            btnLang.Content =
                isArabic
                    ? "EN/AR"
                    : "AR/EN";


            btnTheme.Content =
                isArabic
                    ? "المظهر"
                    : "THEME";


            btnLogFile.Content =
                isArabic
                    ? "سجل العمليات"
                    : "LOG FILE";


            btnSystemTray.Content =
                isArabic
                    ? "شريط النظام"
                    : "SYSTEM TRAY";


            btnDevInfo.Content =
                isArabic
                    ? "معلومات المطور"
                    : "DEVELOPER INFO";


            txtTimerLabel.Text =
                isArabic
                    ? "مؤقت:"
                    : "Timer:";


            txtCategoryLabel.Text =
                isArabic
                    ? "التصنيف:"
                    : "Category:";


            txtSortLabel.Text =
                isArabic
                    ? "الترتيب:"
                    : "Sort by:";


            // ----------------------------------------------------
            // SHORTCUT TEXT
            // ----------------------------------------------------

            foreach (ShortcutNode node
                in Shortcuts)
            {
                node.ApplyLanguage(
                    isArabic);
            }


            // ----------------------------------------------------
            // REBUILD COMBO TEXT
            // ----------------------------------------------------

            PopulateFilterControls();


            UpdateFavoritesButton();


            RefreshShortcutView();
        }


        // ========================================================
        // THEME
        // ========================================================

        public void SetTheme(
            bool isLight)
        {
            ThemePalette.Instance.Apply(
                isLight);
        }


        // ========================================================
        // COMBO CHOICE MODEL
        // ========================================================

        private sealed class Choice<T>
        {
            public T Value
            {
                get;
                private set;
            }


            public string Label
            {
                get;
                private set;
            }


            public Choice(
                T value,
                string label)
            {
                Value =
                    value;

                Label =
                    label;
            }


            public override string ToString()
            {
                return Label;
            }
        }


        // ========================================================
        // POPULATE FILTER CONTROLS
        // ========================================================

        private void PopulateFilterControls()
        {
            if (cmbCategory == null ||
                cmbSortBy == null)
            {
                return;
            }


            updatingFilterControls =
                true;


            // ----------------------------------------------------
            // CATEGORIES
            // ----------------------------------------------------

            List<Choice<ShortcutCategory>>
                categoryOptions =
                new List<Choice<ShortcutCategory>>
                {
                    new Choice<ShortcutCategory>(
                        ShortcutCategory.None,
                        isArabic
                            ? "جميع الاختصارات"
                            : "All Shortcuts"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.WindowsKey,
                        isArabic
                            ? "مفتاح Windows"
                            : "Windows Key"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.Ctrl,
                        "Ctrl"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.Alt,
                        "Alt"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.FunctionKeys,
                        isArabic
                            ? "مفاتيح الوظائف"
                            : "Function Keys"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.WindowManagement,
                        isArabic
                            ? "إدارة النوافذ"
                            : "Window Management"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.FileExplorer,
                        isArabic
                            ? "الملفات والمستكشف"
                            : "File & Explorer"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.TextEditing,
                        isArabic
                            ? "تحرير النصوص"
                            : "Text Editing"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.BrowserNavigation,
                        isArabic
                            ? "المتصفح والتنقل"
                            : "Browser & Navigation"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.ScreenshotsRecording,
                        isArabic
                            ? "لقطات الشاشة والتسجيل"
                            : "Screenshots & Recording"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.VirtualDesktops,
                        isArabic
                            ? "أسطح المكتب الافتراضية"
                            : "Virtual Desktops"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.TaskbarApps,
                        isArabic
                            ? "شريط المهام والتطبيقات"
                            : "Taskbar & Apps"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.SystemAccessibility,
                        isArabic
                            ? "النظام وإمكانية الوصول"
                            : "System & Accessibility"),


                    new Choice<ShortcutCategory>(
                        ShortcutCategory.Other,
                        isArabic
                            ? "أخرى"
                            : "Other")
                };


            // ----------------------------------------------------
            // SORT MODES
            // ----------------------------------------------------

            List<Choice<ShortcutSortMode>>
                sortOptions =
                new List<Choice<ShortcutSortMode>>
                {
                    new Choice<ShortcutSortMode>(
                        ShortcutSortMode.DefaultOrder,
                        isArabic
                            ? "الترتيب الافتراضي"
                            : "Default Order"),


                    new Choice<ShortcutSortMode>(
                        ShortcutSortMode.NameAscending,
                        isArabic
                            ? "الاسم: أ ← ي"
                            : "Name A → Z"),


                    new Choice<ShortcutSortMode>(
                        ShortcutSortMode.NameDescending,
                        isArabic
                            ? "الاسم: ي ← أ"
                            : "Name Z → A"),


                    new Choice<ShortcutSortMode>(
                        ShortcutSortMode.MostUsed,
                        isArabic
                            ? "الأكثر استخدامًا"
                            : "Most Used"),


                    new Choice<ShortcutSortMode>(
                        ShortcutSortMode.LeastUsed,
                        isArabic
                            ? "الأقل استخدامًا"
                            : "Least Used"),


                    new Choice<ShortcutSortMode>(
                        ShortcutSortMode.FavoritesFirst,
                        isArabic
                            ? "المفضلة أولًا"
                            : "Favorites First")
                };


            cmbCategory.ItemsSource =
                categoryOptions;


            cmbCategory.SelectedValuePath =
                "Value";


            cmbCategory.SelectedValue =
                selectedCategory;


            cmbSortBy.ItemsSource =
                sortOptions;


            cmbSortBy.SelectedValuePath =
                "Value";


            cmbSortBy.SelectedValue =
                selectedSort;


            updatingFilterControls =
                false;
        }


        // ========================================================
        // CATEGORY CHANGED
        // ========================================================

        private void CmbCategory_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (updatingFilterControls)
                return;


            Choice<ShortcutCategory> option =
                cmbCategory.SelectedItem
                as Choice<ShortcutCategory>;


            if (option == null)
                return;


            if (selectedCategory ==
                option.Value)
            {
                return;
            }


            selectedCategory =
                option.Value;


            RefreshShortcutView();
        }


        // ========================================================
        // SORT CHANGED
        // ========================================================

        private void CmbSortBy_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (updatingFilterControls)
                return;


            Choice<ShortcutSortMode> option =
                cmbSortBy.SelectedItem
                as Choice<ShortcutSortMode>;


            if (option == null)
                return;


            if (selectedSort ==
                option.Value)
            {
                return;
            }


            selectedSort =
                option.Value;


            RefreshShortcutView();
        }


        // ========================================================
        // FILTER
        // ========================================================

        private bool FilterShortcut(
            object item)
        {
            ShortcutNode node =
                item as ShortcutNode;


            if (node == null)
                return false;


            // Favorites only

            if (showFavoritesOnly &&
                !node.IsFavorite)
            {
                return false;
            }


            // Category

            if (selectedCategory !=
                ShortcutCategory.None)
            {
                if ((node.Categories &
                     selectedCategory) ==
                    ShortcutCategory.None)
                {
                    return false;
                }
            }


            return true;
        }


        // ========================================================
        // RESPONSIVE SHORTCUT LAYOUT
        // 4 -> 2 -> 1
        // ========================================================

        private void ShortcutScrollViewer_SizeChanged(
            object sender,
            SizeChangedEventArgs e)
        {
            UpdateShortcutColumns();
        }


        private void WpfShortcutCanvas_SizeChanged(
            object sender,
            SizeChangedEventArgs e)
        {
            UpdateShortcutColumns();
        }


        private void UpdateShortcutColumns()
        {
            if (ShortcutList == null)
                return;


            // ----------------------------------------------------
            // IMPORTANT:
            //
            // Do NOT use ScrollViewer.ViewportWidth here.
            //
            // In this application it can report a width based on
            // the measured child and cause the layout to start at
            // two columns even when the window is very wide.
            //
            // Use the real WpfShortcutCanvas width instead.
            // ----------------------------------------------------

            double availableWidth =
                ActualWidth;


            if (double.IsNaN(availableWidth) ||
                double.IsInfinity(availableWidth) ||
                availableWidth <= 0)
            {
                return;
            }


            int columns;


            // ----------------------------------------------------
            // EXPLICIT BREAKPOINTS
            //
            // Large  -> 4
            // Medium -> 2
            // Small  -> 1
            //
            // There is intentionally NO 3-column state.
            // ----------------------------------------------------

            if (availableWidth >=
                FourColumnBreakpoint)
            {
                columns = 4;
            }
            else if (availableWidth >=
                     TwoColumnBreakpoint)
            {
                columns = 2;
            }
            else
            {
                columns = 1;
            }


            // ----------------------------------------------------
            // TARGET WIDTH
            //
            // 4 x 270 = 1080
            // 2 x 270 =  540
            // 1 x 270 =  270
            // ----------------------------------------------------

            double targetWidth =
                columns *
                ShortcutColumnWidth;


            if (columns ==
                    currentShortcutColumns &&
                !double.IsNaN(
                    ShortcutList.Width) &&
                Math.Abs(
                    ShortcutList.Width -
                    targetWidth) < 0.5)
            {
                return;
            }


            currentShortcutColumns =
                columns;


            ShortcutList.Width =
                targetWidth;
        }


        // ========================================================
        // REFRESH VIEW
        // ========================================================

        private void RefreshShortcutView()
        {
            if (shortcutView == null)
                return;


            shortcutView.Refresh();


            // Filtering can show/hide the vertical scrollbar.
            // Let WPF finish that layout pass, then calculate
            // the exact number of columns again.

            Dispatcher.BeginInvoke(
                new Action(
                    UpdateShortcutColumns));
        }


        // ========================================================
        // APPLY FAVORITES
        // ========================================================

        public void ApplyFavorites()
        {
            foreach (ShortcutNode node
                in Shortcuts)
            {
                node.IsFavorite =
                    favoriteKeySet.Contains(
                        node.LogKey);
            }


            RebuildFavoriteOrderLookup();


            UpdateFavoritesButton();


            RefreshShortcutView();
        }


        // ========================================================
        // TOGGLE FAVORITE
        // ========================================================

        private void ToggleFavorite_Click(
            object sender,
            RoutedEventArgs e)
        {
            Button button =
                sender as Button;


            if (button == null)
                return;


            ShortcutNode node =
                button.DataContext
                as ShortcutNode;


            if (node == null)
                return;


            node.IsFavorite =
                !node.IsFavorite;


            string key =
                node.LogKey;


            if (node.IsFavorite)
            {
                if (favoriteKeySet.Add(
                    key))
                {
                    favoriteKeys.Add(
                        key);
                }
            }
            else
            {
                favoriteKeySet.Remove(
                    key);


                favoriteKeys.Remove(
                    key);
            }


            RebuildFavoriteOrderLookup();


            ScheduleFavoritesSave();


            RefreshShortcutView();
        }


        // ========================================================
        // DEBOUNCED FAVORITES SAVE
        // ========================================================

        private void ScheduleFavoritesSave()
        {
            if (favoriteSaveCancellation != null)
            {
                favoriteSaveCancellation.Cancel();

                favoriteSaveCancellation.Dispose();
            }


            favoriteSaveCancellation =
                new CancellationTokenSource();


            CancellationToken token =
                favoriteSaveCancellation.Token;


            List<string> snapshot =
                new List<string>(
                    favoriteKeys);


            Task.Run(
                async () =>
                {
                    try
                    {
                        await Task.Delay(
                            250,
                            token);


                        if (token.IsCancellationRequested)
                            return;


                        FavoriteManager.SaveFavorites(
                            snapshot);
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected when another favorite action
                        // happens before the save delay ends.
                    }
                });
        }


        // ========================================================
        // FAVORITES VIEW
        // ========================================================

        private void BtnToggleFavorites_Click(
            object sender,
            RoutedEventArgs e)
        {
            showFavoritesOnly =
                !showFavoritesOnly;


            UpdateFavoritesButton();


            RefreshShortcutView();
        }


        private void UpdateFavoritesButton()
        {
            if (showFavoritesOnly)
            {
                btnToggleFavorites.Content =
                    "★";


                btnToggleFavorites.ToolTip =
                    isArabic
                        ? "إظهار الكل"
                        : "Show All";
            }
            else
            {
                btnToggleFavorites.Content =
                    "☆";


                btnToggleFavorites.ToolTip =
                    isArabic
                        ? "إظهار المفضلة فقط"
                        : "Show Favorites Only";
            }
        }


        // ========================================================
        // EXECUTE
        // ========================================================

        private async void ExecuteButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Button button =
                sender as Button;


            if (button == null)
                return;


            ShortcutNode node =
                button.DataContext
                as ShortcutNode;


            if (node == null)
                return;


            button.IsEnabled =
                false;


            string originalContent =
                button.Content != null
                    ? button.Content.ToString()
                    : string.Empty;


            int delay =
                GetSelectedDelay();


            try
            {
                // Countdown

                for (int i = delay;
                     i > 0;
                     i--)
                {
                    button.Content =
                        isArabic
                            ? string.Format(
                                "انتظر {0}...",
                                i)
                            : string.Format(
                                "Wait {0}s…",
                                i);


                    await Task.Delay(
                        1000);
                }


                EventHandler<ShortcutNode> handler =
                    OnExecuteRequested;


                if (handler != null)
                {
                    handler(
                        this,
                        node);
                }
            }
            finally
            {
                button.Content =
                    originalContent;


                button.IsEnabled =
                    true;
            }
        }


        // ========================================================
        // TIMER
        // ========================================================

        public int GetSelectedDelay()
        {
            ComboBoxItem item =
                cmbTimerDelay.SelectedItem
                as ComboBoxItem;


            if (item == null)
                return 0;


            int delay;


            string value =
                item.Content != null
                    ? item.Content.ToString()
                    : "0";


            if (int.TryParse(
                value,
                out delay))
            {
                return delay;
            }


            return 0;
        }


        // ========================================================
        // EVENT FORWARDERS
        // ========================================================

        private void BtnLang_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventHandler handler =
                OnLangToggleRequested;


            if (handler != null)
            {
                handler(
                    this,
                    EventArgs.Empty);
            }
        }


        private void BtnTheme_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventHandler handler =
                OnThemeToggleRequested;


            if (handler != null)
            {
                handler(
                    this,
                    EventArgs.Empty);
            }
        }


        private void BtnLogFile_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventHandler handler =
                OnLogFileRequested;


            if (handler != null)
            {
                handler(
                    this,
                    EventArgs.Empty);
            }
        }


        private void BtnSystemTray_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventHandler handler =
                OnSystemTrayRequested;


            if (handler != null)
            {
                handler(
                    this,
                    EventArgs.Empty);
            }
        }


        private void BtnDevInfo_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventHandler handler =
                OnDevInfoRequested;


            if (handler != null)
            {
                handler(
                    this,
                    EventArgs.Empty);
            }
        }


        // ========================================================
        // SORT COMPARER
        // ========================================================

        private sealed class ShortcutNodeComparer
            : IComparer
        {
            private readonly WpfShortcutCanvas owner;


            public ShortcutNodeComparer(
                WpfShortcutCanvas owner)
            {
                this.owner =
                    owner;
            }


            public int Compare(
                object x,
                object y)
            {
                ShortcutNode left =
                    x as ShortcutNode;


                ShortcutNode right =
                    y as ShortcutNode;


                if (ReferenceEquals(
                    left,
                    right))
                {
                    return 0;
                }


                if (left == null)
                    return 1;


                if (right == null)
                    return -1;


                switch (owner.selectedSort)
                {
                    // ============================================
                    // NAME A -> Z
                    // ============================================

                    case ShortcutSortMode.NameAscending:
                        {
                            int result =
                                StringComparer
                                .CurrentCultureIgnoreCase
                                .Compare(
                                    left.SortName,
                                    right.SortName);


                            if (result != 0)
                                return result;


                            return CompareDefault(
                                left,
                                right);
                        }


                    // ============================================
                    // NAME Z -> A
                    // ============================================

                    case ShortcutSortMode.NameDescending:
                        {
                            int result =
                                StringComparer
                                .CurrentCultureIgnoreCase
                                .Compare(
                                    right.SortName,
                                    left.SortName);


                            if (result != 0)
                                return result;


                            return CompareDefault(
                                left,
                                right);
                        }


                    // ============================================
                    // MOST USED
                    // ============================================

                    case ShortcutSortMode.MostUsed:
                        {
                            int result =
                                right.ExecutionCount
                                .CompareTo(
                                    left.ExecutionCount);


                            if (result != 0)
                                return result;


                            return CompareDefault(
                                left,
                                right);
                        }


                    // ============================================
                    // LEAST USED
                    // ============================================

                    case ShortcutSortMode.LeastUsed:
                        {
                            int result =
                                left.ExecutionCount
                                .CompareTo(
                                    right.ExecutionCount);


                            if (result != 0)
                                return result;


                            return CompareDefault(
                                left,
                                right);
                        }


                    // ============================================
                    // FAVORITES FIRST
                    // ============================================

                    case ShortcutSortMode.FavoritesFirst:
                        {
                            if (left.IsFavorite &&
                                !right.IsFavorite)
                            {
                                return -1;
                            }


                            if (!left.IsFavorite &&
                                right.IsFavorite)
                            {
                                return 1;
                            }


                            if (left.IsFavorite &&
                                right.IsFavorite)
                            {
                                int leftFavoriteOrder =
                                    owner.GetFavoriteOrder(
                                        left);


                                int rightFavoriteOrder =
                                    owner.GetFavoriteOrder(
                                        right);


                                int result =
                                    leftFavoriteOrder
                                    .CompareTo(
                                        rightFavoriteOrder);


                                if (result != 0)
                                    return result;
                            }


                            return CompareDefault(
                                left,
                                right);
                        }


                    // ============================================
                    // DEFAULT
                    // ============================================

                    case ShortcutSortMode.DefaultOrder:

                    default:
                        {
                            return CompareDefault(
                                left,
                                right);
                        }
                }
            }


            private int CompareDefault(
                ShortcutNode left,
                ShortcutNode right)
            {
                // Favorites-only mode keeps the order in which
                // the user starred shortcuts.

                if (owner.showFavoritesOnly)
                {
                    int leftFavoriteOrder =
                        owner.GetFavoriteOrder(
                            left);


                    int rightFavoriteOrder =
                        owner.GetFavoriteOrder(
                            right);


                    int favoriteResult =
                        leftFavoriteOrder
                        .CompareTo(
                            rightFavoriteOrder);


                    if (favoriteResult != 0)
                    {
                        return favoriteResult;
                    }
                }


                int leftIndex =
                    owner.GetMasterIndex(
                        left);


                int rightIndex =
                    owner.GetMasterIndex(
                        right);


                return leftIndex.CompareTo(
                    rightIndex);
            }
        }
    }


    // ============================================================
    // SHORTCUT NODE
    // ============================================================

    public class ShortcutNode : INotifyPropertyChanged
    {
        // ========================================================
        // CORE DATA
        // ========================================================

        public string LogKey
        {
            get;
            set;
        }


        public byte MainKey
        {
            get;
            set;
        }


        public byte[] Modifiers
        {
            get;
            set;
        }


        public string TitleEn
        {
            get;
            set;
        }


        public string TitleAr
        {
            get;
            set;
        }


        public string InfoEn
        {
            get;
            set;
        }


        public string InfoAr
        {
            get;
            set;
        }


        // ========================================================
        // CATEGORY CACHE
        // ========================================================

        private bool categoryCacheReady =
            false;


        private ShortcutCategory
            cachedCategories;


        public ShortcutCategory Categories
        {
            get
            {
                if (!categoryCacheReady)
                {
                    cachedCategories =
                        ShortcutsRepository
                        .GetCategories(
                            LogKey,
                            Modifiers,
                            MainKey);


                    categoryCacheReady =
                        true;
                }


                return cachedCategories;
            }
        }


        // ========================================================
        // EXECUTION COUNT
        // ========================================================

        private int executionCount;


        public int ExecutionCount
        {
            get
            {
                return executionCount;
            }

            set
            {
                if (executionCount ==
                    value)
                {
                    return;
                }


                executionCount =
                    value;


                OnPropertyChanged(
                    nameof(ExecutionCount));
            }
        }


        // ========================================================
        // FAVORITE
        // ========================================================

        private bool isFavorite;


        public bool IsFavorite
        {
            get
            {
                return isFavorite;
            }

            set
            {
                if (isFavorite ==
                    value)
                {
                    return;
                }


                isFavorite =
                    value;


                OnPropertyChanged(
                    nameof(IsFavorite));
            }
        }


        // ========================================================
        // VISIBILITY
        // ========================================================

        private bool isVisible =
            true;


        public bool IsVisible
        {
            get
            {
                return isVisible;
            }

            set
            {
                if (isVisible ==
                    value)
                {
                    return;
                }


                isVisible =
                    value;


                OnPropertyChanged(
                    nameof(IsVisible));
            }
        }


        // ========================================================
        // DISPLAY VALUES
        // ========================================================

        public string DisplayTitle
        {
            get;
            private set;
        }


        public string DisplayInfo
        {
            get;
            private set;
        }


        public string ExecuteButtonText
        {
            get;
            private set;
        }


        // ========================================================
        // SORT NAME CACHE
        // ========================================================

        private string sortName =
            string.Empty;


        public string SortName
        {
            get
            {
                if (!string.IsNullOrEmpty(
                    sortName))
                {
                    return sortName;
                }


                string fallback =
                    DisplayTitle;


                if (string.IsNullOrEmpty(
                    fallback))
                {
                    fallback =
                        TitleEn;
                }


                return BuildSortName(
                    fallback);
            }

            private set
            {
                sortName =
                    value ?? string.Empty;
            }
        }


        // ========================================================
        // LANGUAGE
        // ========================================================

        public void ApplyLanguage(
            bool isArabic)
        {
            DisplayTitle =
                isArabic
                    ? TitleAr
                    : TitleEn;


            DisplayInfo =
                isArabic
                    ? InfoAr
                    : InfoEn;


            ExecuteButtonText =
                isArabic
                    ? "تنفيذ"
                    : "EXECUTE";


            SortName =
                BuildSortName(
                    DisplayTitle);


            OnPropertyChanged(
                nameof(DisplayTitle));


            OnPropertyChanged(
                nameof(DisplayInfo));


            OnPropertyChanged(
                nameof(ExecuteButtonText));
        }


        // ========================================================
        // SORT NAME
        // ========================================================

        private static string BuildSortName(
            string title)
        {
            if (string.IsNullOrWhiteSpace(
                title))
            {
                return string.Empty;
            }


            int separatorIndex =
                title.IndexOf(
                    ". ",
                    StringComparison.Ordinal);


            if (separatorIndex >= 0 &&
                separatorIndex + 2 <
                    title.Length)
            {
                return title.Substring(
                    separatorIndex + 2);
            }


            return title;
        }


        // ========================================================
        // PROPERTY CHANGED
        // ========================================================

        public event PropertyChangedEventHandler
            PropertyChanged;


        protected void OnPropertyChanged(
            string propertyName)
        {
            PropertyChangedEventHandler handler =
                PropertyChanged;


            if (handler != null)
            {
                handler(
                    this,
                    new PropertyChangedEventArgs(
                        propertyName));
            }
        }
    }
}