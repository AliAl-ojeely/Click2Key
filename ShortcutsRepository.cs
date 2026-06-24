using System.Collections.Generic;

namespace Click2Key
{
    public class ShortcutData
    {
        public string EnglishTitle { get; set; }
        public string ArabicTitle { get; set; }
        public string LogKey { get; set; }
        public byte[] Modifiers { get; set; }
        public byte MainKey { get; set; }
        public string InfoMessageEn { get; set; }
        public string InfoMessageAr { get; set; }

        public ShortcutData(string engTitle, string arTitle, string logKey,
                            byte[] modifiers, byte mainKey, string infoEn, string infoAr)
        {
            EnglishTitle = engTitle;
            ArabicTitle = arTitle;
            LogKey = logKey;
            Modifiers = modifiers;
            MainKey = mainKey;
            InfoMessageEn = infoEn;
            InfoMessageAr = infoAr;
        }
    }

    public static class ShortcutsRepository
    {
        // Virtual Key Constants
        public const byte VK_LWIN = 0x5B;
        public const byte VK_RWIN = 0x5C;
        public const byte VK_E = 0x45;
        public const byte VK_CONTROL = 0x11;
        public const byte VK_MENU = 0x12;        // Alt
        public const byte VK_SHIFT = 0x10;
        public const byte VK_O = 0x4F;
        public const byte VK_C = 0x43;
        public const byte VK_V = 0x56;
        public const byte VK_X = 0x58;
        public const byte VK_A = 0x41;
        public const byte VK_Z = 0x5A;
        public const byte VK_Y = 0x59;
        public const byte VK_S = 0x53;
        public const byte VK_D = 0x44;
        public const byte VK_F = 0x46;
        public const byte VK_G = 0x47;
        public const byte VK_H = 0x48;
        public const byte VK_I = 0x49;
        public const byte VK_L = 0x4C;
        public const byte VK_M = 0x4D;
        public const byte VK_N = 0x4E;
        public const byte VK_P = 0x50;
        public const byte VK_Q = 0x51;
        public const byte VK_R = 0x52;
        public const byte VK_T = 0x54;
        public const byte VK_U = 0x55;
        public const byte VK_W = 0x57;
        public const byte VK_0 = 0x30;
        public const byte VK_1 = 0x31;
        public const byte VK_2 = 0x32;
        public const byte VK_3 = 0x33;
        public const byte VK_4 = 0x34;
        public const byte VK_5 = 0x35;
        public const byte VK_6 = 0x36;
        public const byte VK_7 = 0x37;
        public const byte VK_8 = 0x38;
        public const byte VK_9 = 0x39;
        public const byte VK_TAB = 0x09;
        public const byte VK_ESCAPE = 0x1B;
        public const byte VK_SPACE = 0x20;
        public const byte VK_RETURN = 0x0D;
        public const byte VK_BACK = 0x08;
        public const byte VK_DELETE = 0x2E;
        public const byte VK_INSERT = 0x2D;
        public const byte VK_HOME = 0x24;
        public const byte VK_END = 0x23;
        public const byte VK_PRIOR = 0x21;      // Page Up
        public const byte VK_NEXT = 0x22;       // Page Down
        public const byte VK_LEFT = 0x25;
        public const byte VK_UP = 0x26;
        public const byte VK_RIGHT = 0x27;
        public const byte VK_DOWN = 0x28;
        public const byte VK_SNAPSHOT = 0x2C;   // Print Screen
        public const byte VK_F1 = 0x70;
        public const byte VK_F2 = 0x71;
        public const byte VK_F3 = 0x72;
        public const byte VK_F4 = 0x73;
        public const byte VK_F5 = 0x74;
        public const byte VK_F6 = 0x75;
        public const byte VK_F7 = 0x76;
        public const byte VK_F8 = 0x77;
        public const byte VK_F9 = 0x78;
        public const byte VK_F10 = 0x79;
        public const byte VK_F11 = 0x7A;
        public const byte VK_F12 = 0x7B;
        public const byte VK_NUMLOCK = 0x90;

        public static List<ShortcutData> GetAll()
        {
            List<ShortcutData> list = new List<ShortcutData>();

            // 1-5: Basic Shortcuts
            list.Add(new ShortcutData("1. Ctrl + C", "1. Ctrl + C", "Shortcut_Ctrl_C", new byte[] { VK_CONTROL }, VK_C, "Copy selected item.", "نسخ العنصر المحدد."));
            list.Add(new ShortcutData("2. Win + E", "2. Win + E", "Shortcut_Win_E", new byte[] { VK_LWIN }, VK_E, "Open File Explorer.", "فتح مستكشف الملفات."));
            list.Add(new ShortcutData("3. Win + Ctrl + O", "3. Win + Ctrl + O", "Shortcut_Win_Ctrl_O", new byte[] { VK_LWIN, VK_CONTROL }, VK_O, "Open On-Screen Keyboard.", "فتح لوحة المفاتيح على الشاشة."));
            list.Add(new ShortcutData("4. Ctrl + V", "4. Ctrl + V", "Shortcut_Ctrl_V", new byte[] { VK_CONTROL }, VK_V, "Paste copied item.", "لصق العنصر المنسوخ."));
            list.Add(new ShortcutData("5. Ctrl + X", "5. Ctrl + X", "Shortcut_Ctrl_X", new byte[] { VK_CONTROL }, VK_X, "Cut selected item.", "قص العنصر المحدد."));

            // 6-27: Windows Key Combinations
            list.Add(new ShortcutData("6. Win + D", "6. Win + D", "Shortcut_Win_D", new byte[] { VK_LWIN }, VK_D, "Show/Hide desktop.", "إظهار/إخفاء سطح المكتب."));
            list.Add(new ShortcutData("7. Win + L", "7. Win + L", "Shortcut_Win_L", new byte[] { VK_LWIN }, VK_L, "Lock your PC.", "قفل جهاز الكمبيوتر."));
            list.Add(new ShortcutData("8. Win + M", "8. Win + M", "Shortcut_Win_M", new byte[] { VK_LWIN }, VK_M, "Minimize all windows.", "تصغير جميع النوافذ."));
            list.Add(new ShortcutData("9. Win + Shift + M", "9. Win + Shift + M", "Shortcut_Win_Shift_M", new byte[] { VK_LWIN, VK_SHIFT }, VK_M, "Restore minimized windows.", "استعادة النوافذ المصغرة."));
            list.Add(new ShortcutData("10. Win + R", "10. Win + R", "Shortcut_Win_R", new byte[] { VK_LWIN }, VK_R, "Open Run dialog.", "فتح مربع التشغيل."));
            list.Add(new ShortcutData("11. Win + S", "11. Win + S", "Shortcut_Win_S", new byte[] { VK_LWIN }, VK_S, "Open Windows Search.", "فتح بحث Windows."));
            list.Add(new ShortcutData("12. Win + Q", "12. Win + Q", "Shortcut_Win_Q", new byte[] { VK_LWIN }, VK_Q, "Open Search (alternative).", "فتح البحث (بديل)."));
            list.Add(new ShortcutData("13. Win + I", "13. Win + I", "Shortcut_Win_I", new byte[] { VK_LWIN }, VK_I, "Open Settings.", "فتح الإعدادات."));
            list.Add(new ShortcutData("14. Win + A", "14. Win + A", "Shortcut_Win_A", new byte[] { VK_LWIN }, VK_A, "Open Action Center.", "فتح مركز الإجراءات."));
            list.Add(new ShortcutData("15. Win + W", "15. Win + W", "Shortcut_Win_W", new byte[] { VK_LWIN }, VK_W, "Open Widgets panel.", "فتح لوحة الأدوات."));
            list.Add(new ShortcutData("16. Win + Tab", "16. Win + Tab", "Shortcut_Win_Tab", new byte[] { VK_LWIN }, VK_TAB, "Open Task View.", "فتح عرض المهام."));
            list.Add(new ShortcutData("17. Win + Up Arrow", "17. Win + Up Arrow", "Shortcut_Win_Up", new byte[] { VK_LWIN }, VK_UP, "Maximize window.", "تكبير النافذة."));
            list.Add(new ShortcutData("18. Win + Down Arrow", "18. Win + Down Arrow", "Shortcut_Win_Down", new byte[] { VK_LWIN }, VK_DOWN, "Restore/Minimize window.", "استعادة أو تصغير النافذة."));
            list.Add(new ShortcutData("19. Win + Left Arrow", "19. Win + Left Arrow", "Shortcut_Win_Left", new byte[] { VK_LWIN }, VK_LEFT, "Snap window to left.", "تثبيت النافذة لليسار."));
            list.Add(new ShortcutData("20. Win + Right Arrow", "20. Win + Right Arrow", "Shortcut_Win_Right", new byte[] { VK_LWIN }, VK_RIGHT, "Snap window to right.", "تثبيت النافذة لليمين."));
            list.Add(new ShortcutData("21. Win + Home", "21. Win + Home", "Shortcut_Win_Home", new byte[] { VK_LWIN }, VK_HOME, "Minimize/restore all but active window.", "تصغير/استعادة الكل ما عدا النافذة النشطة."));
            list.Add(new ShortcutData("22. Win + Pause", "22. Win + Pause", "Shortcut_Win_Pause", new byte[] { VK_LWIN }, 0x13, "Open System Properties.", "فتح خصائص النظام."));
            list.Add(new ShortcutData("23. Win + Shift + S", "23. Win + Shift + S", "Shortcut_Win_Shift_S", new byte[] { VK_LWIN, VK_SHIFT }, VK_S, "Open Snipping Tool.", "فتح أداة القصاصة."));
            list.Add(new ShortcutData("24. Win + V", "24. Win + V", "Shortcut_Win_V", new byte[] { VK_LWIN }, VK_V, "Open Clipboard history.", "فتح محفوظات الحافظة."));
            list.Add(new ShortcutData("25. Win + Period (.)", "25. Win + Period (.)", "Shortcut_Win_Period", new byte[] { VK_LWIN }, 0xBE, "Open Emoji panel.", "فتح لوحة الرموز التعبيرية."));
            list.Add(new ShortcutData("26. Win + Comma (,)", "26. Win + Comma (,)", "Shortcut_Win_Comma", new byte[] { VK_LWIN }, 0xBC, "Peek at desktop.", "نظرة سريعة على سطح المكتب."));
            list.Add(new ShortcutData("27. Win + 1", "27. Win + 1", "Shortcut_Win_1", new byte[] { VK_LWIN }, VK_1, "Open app pinned to taskbar (1).", "فتح التطبيق الأول المثبت في شريط المهام."));

            // 28-46: Ctrl Key Combinations
            list.Add(new ShortcutData("28. Ctrl + Z", "28. Ctrl + Z", "Shortcut_Ctrl_Z", new byte[] { VK_CONTROL }, VK_Z, "Undo last action.", "تراجع عن الإجراء الأخير."));
            list.Add(new ShortcutData("29. Ctrl + Y", "29. Ctrl + Y", "Shortcut_Ctrl_Y", new byte[] { VK_CONTROL }, VK_Y, "Redo last action.", "إعادة تنفيذ الإجراء الأخير."));
            list.Add(new ShortcutData("30. Ctrl + A", "30. Ctrl + A", "Shortcut_Ctrl_A", new byte[] { VK_CONTROL }, VK_A, "Select all items.", "تحديد جميع العناصر."));
            list.Add(new ShortcutData("31. Ctrl + S", "31. Ctrl + S", "Shortcut_Ctrl_S", new byte[] { VK_CONTROL }, VK_S, "Save current document.", "حفظ المستند الحالي."));
            list.Add(new ShortcutData("32. Ctrl + P", "32. Ctrl + P", "Shortcut_Ctrl_P", new byte[] { VK_CONTROL }, VK_P, "Print current page/document.", "طباعة الصفحة/المستند الحالي."));
            list.Add(new ShortcutData("33. Ctrl + F", "33. Ctrl + F", "Shortcut_Ctrl_F", new byte[] { VK_CONTROL }, VK_F, "Find text in document/page.", "البحث عن نص."));
            list.Add(new ShortcutData("34. Ctrl + H", "34. Ctrl + H", "Shortcut_Ctrl_H", new byte[] { VK_CONTROL }, VK_H, "Find and replace text.", "بحث واستبدال النص."));
            list.Add(new ShortcutData("35. Ctrl + N", "35. Ctrl + N", "Shortcut_Ctrl_N", new byte[] { VK_CONTROL }, VK_N, "Open new window/document.", "فتح نافذة/مستند جديد."));
            list.Add(new ShortcutData("36. Ctrl + W", "36. Ctrl + W", "Shortcut_Ctrl_W", new byte[] { VK_CONTROL }, VK_W, "Close current tab/window.", "إغلاق علامة التبويب/النافذة الحالية."));
            list.Add(new ShortcutData("37. Ctrl + Shift + N", "37. Ctrl + Shift + N", "Shortcut_Ctrl_Shift_N", new byte[] { VK_CONTROL, VK_SHIFT }, VK_N, "Create new folder.", "إنشاء مجلد جديد."));
            list.Add(new ShortcutData("38. Ctrl + Shift + Esc", "38. Ctrl + Shift + Esc", "Shortcut_Ctrl_Shift_Esc", new byte[] { VK_CONTROL, VK_SHIFT }, VK_ESCAPE, "Open Task Manager.", "فتح مدير المهام."));
            list.Add(new ShortcutData("39. Ctrl + Esc", "39. Ctrl + Esc", "Shortcut_Ctrl_Esc", new byte[] { VK_CONTROL }, VK_ESCAPE, "Open Start menu.", "فتح قائمة ابدأ."));
            list.Add(new ShortcutData("40. Ctrl + Shift + T", "40. Ctrl + Shift + T", "Shortcut_Ctrl_Shift_T", new byte[] { VK_CONTROL, VK_SHIFT }, VK_T, "Reopen last closed tab.", "إعادة فتح آخر علامة تبويب مغلقة."));
            list.Add(new ShortcutData("41. Ctrl + Tab", "41. Ctrl + Tab", "Shortcut_Ctrl_Tab", new byte[] { VK_CONTROL }, VK_TAB, "Switch to next tab.", "الانتقال إلى علامة التبويب التالية."));
            list.Add(new ShortcutData("42. Ctrl + Shift + Tab", "42. Ctrl + Shift + Tab", "Shortcut_Ctrl_Shift_Tab", new byte[] { VK_CONTROL, VK_SHIFT }, VK_TAB, "Switch to previous tab.", "الانتقال إلى علامة التبويب السابقة."));
            list.Add(new ShortcutData("43. Ctrl + D", "43. Ctrl + D", "Shortcut_Ctrl_D", new byte[] { VK_CONTROL }, VK_D, "Bookmark current page.", "إضافة الصفحة الحالية للمفضلة."));
            list.Add(new ShortcutData("44. Ctrl + Shift + Delete", "44. Ctrl + Shift + Delete", "Shortcut_Ctrl_Shift_Del", new byte[] { VK_CONTROL, VK_SHIFT }, VK_DELETE, "Clear browsing data.", "مسح بيانات التصفح."));
            list.Add(new ShortcutData("45. Ctrl + L", "45. Ctrl + L", "Shortcut_Ctrl_L", new byte[] { VK_CONTROL }, VK_L, "Focus address bar.", "التركيز على شريط العنوان."));
            list.Add(new ShortcutData("46. Ctrl + T", "46. Ctrl + T", "Shortcut_Ctrl_T", new byte[] { VK_CONTROL }, VK_T, "Open new tab in browser/explorer.", "فتح علامة تبويب جديدة."));

            // 47-54: Alt Key Combinations
            list.Add(new ShortcutData("47. Alt + Tab", "47. Alt + Tab", "Shortcut_Alt_Tab", new byte[] { VK_MENU }, VK_TAB, "Switch between open windows.", "التبديل بين النوافذ المفتوحة."));
            list.Add(new ShortcutData("48. Alt + F4", "48. Alt + F4", "Shortcut_Alt_F4", new byte[] { VK_MENU }, VK_F4, "Close active window/app.", "إغلاق النافذة/التطبيق النشط."));
            list.Add(new ShortcutData("49. Alt + Enter", "49. Alt + Enter", "Shortcut_Alt_Enter", new byte[] { VK_MENU }, VK_RETURN, "Show properties of selected item.", "عرض خصائص العنصر المحدد."));
            list.Add(new ShortcutData("50. Alt + Space", "50. Alt + Space", "Shortcut_Alt_Space", new byte[] { VK_MENU }, VK_SPACE, "Open window system menu.", "فتح قائمة النظام للنافذة."));
            list.Add(new ShortcutData("51. Alt + Left Arrow", "51. Alt + Left Arrow", "Shortcut_Alt_Left", new byte[] { VK_MENU }, VK_LEFT, "Go back (browser/explorer).", "الرجوع للخلف."));
            list.Add(new ShortcutData("52. Alt + Right Arrow", "52. Alt + Right Arrow", "Shortcut_Alt_Right", new byte[] { VK_MENU }, VK_RIGHT, "Go forward.", "التقدم للأمام."));
            list.Add(new ShortcutData("53. Alt + Up Arrow", "53. Alt + Up Arrow", "Shortcut_Alt_Up", new byte[] { VK_MENU }, VK_UP, "Go up one folder level.", "الانتقال للمجلد الأعلى."));
            list.Add(new ShortcutData("54. Alt + Esc", "54. Alt + Esc", "Shortcut_Alt_Esc", new byte[] { VK_MENU }, VK_ESCAPE, "Cycle through windows in order.", "التنقل بين النوافذ بالترتيب."));

            // 55-62: Function Key Combinations
            list.Add(new ShortcutData("55. F1", "55. F1", "Shortcut_F1", null, VK_F1, "Open Help.", "فتح التعليمات."));
            list.Add(new ShortcutData("56. F2", "56. F2", "Shortcut_F2", null, VK_F2, "Rename selected item.", "إعادة تسمية العنصر المحدد."));
            list.Add(new ShortcutData("57. F3", "57. F3", "Shortcut_F3", null, VK_F3, "Search for a file/folder.", "البحث عن ملف/مجلد."));
            list.Add(new ShortcutData("58. F4", "58. F4", "Shortcut_F4", null, VK_F4, "Focus address bar in Explorer.", "التركيز على شريط العنوان في المستكشف."));
            list.Add(new ShortcutData("59. F5", "59. F5", "Shortcut_F5", null, VK_F5, "Refresh current page/window.", "تحديث الصفحة/النافذة الحالية."));
            list.Add(new ShortcutData("60. F6", "60. F6", "Shortcut_F6", null, VK_F6, "Cycle through screen elements.", "التنقل بين عناصر الشاشة."));
            list.Add(new ShortcutData("61. F10", "61. F10", "Shortcut_F10", null, VK_F10, "Activate menu bar.", "تنشيط شريط القوائم."));
            list.Add(new ShortcutData("62. F11", "62. F11", "Shortcut_F11", null, VK_F11, "Toggle full screen.", "تبديل وضع ملء الشاشة."));

            // 63-70: Win Key Combinations Continued
            list.Add(new ShortcutData("63. Win + X", "63. Win + X", "Shortcut_Win_X", new byte[] { VK_LWIN }, VK_X, "Open Quick Link menu.", "فتح قائمة الخبير."));
            list.Add(new ShortcutData("64. Win + U", "64. Win + U", "Shortcut_Win_U", new byte[] { VK_LWIN }, VK_U, "Open Ease of Access Center.", "فتح مركز سهولة الوصول."));
            list.Add(new ShortcutData("65. Win + P", "65. Win + P", "Shortcut_Win_P", new byte[] { VK_LWIN }, VK_P, "Project screen options.", "خيارات عرض الشاشة."));
            list.Add(new ShortcutData("66. Win + K", "66. Win + K", "Shortcut_Win_K", new byte[] { VK_LWIN }, 0x4B, "Open Connect panel.", "فتح لوحة الاتصال."));
            list.Add(new ShortcutData("67. Win + F", "67. Win + F", "Shortcut_Win_F", new byte[] { VK_LWIN }, VK_F, "Open Feedback Hub.", "فتح مركز الملاحظات."));
            list.Add(new ShortcutData("68. Win + Ctrl + D", "68. Win + Ctrl + D", "Shortcut_Win_Ctrl_D", new byte[] { VK_LWIN, VK_CONTROL }, VK_D, "Add new virtual desktop.", "إضافة سطح مكتب افتراضي جديد."));
            list.Add(new ShortcutData("69. Win + Ctrl + F4", "69. Win + Ctrl + F4", "Shortcut_Win_Ctrl_F4", new byte[] { VK_LWIN, VK_CONTROL }, VK_F4, "Close current virtual desktop.", "إغلاق سطح المكتب الافتراضي الحالي."));
            list.Add(new ShortcutData("70. Win + Ctrl + Right", "70. Win + Ctrl + Right", "Shortcut_Win_Ctrl_Right", new byte[] { VK_LWIN, VK_CONTROL }, VK_RIGHT, "Switch between virtual desktops.", "التبديل بين أسطح المكتب الافتراضية."));

            // 71-75: More Ctrl Combinations
            list.Add(new ShortcutData("71. Ctrl + Shift + N", "71. Ctrl + Shift + N", "Shortcut_Ctrl_Shift_N2", new byte[] { VK_CONTROL, VK_SHIFT }, VK_N, "Create new folder (duplicate for clarity).", "إنشاء مجلد جديد."));
            list.Add(new ShortcutData("72. Ctrl + Plus (+)", "72. Ctrl + Plus (+)", "Shortcut_Ctrl_Plus", new byte[] { VK_CONTROL }, 0xBB, "Zoom in.", "تكبير."));
            list.Add(new ShortcutData("73. Ctrl + Minus (-)", "73. Ctrl + Minus (-)", "Shortcut_Ctrl_Minus", new byte[] { VK_CONTROL }, 0xBD, "Zoom out.", "تصغير."));
            list.Add(new ShortcutData("74. Ctrl + 0", "74. Ctrl + 0", "Shortcut_Ctrl_0", new byte[] { VK_CONTROL }, VK_0, "Reset zoom to default.", "إعادة التكبير/التصغير إلى الوضع الافتراضي."));
            list.Add(new ShortcutData("75. Ctrl + Shift + E", "75. Ctrl + Shift + E", "Shortcut_Ctrl_Shift_E", new byte[] { VK_CONTROL, VK_SHIFT }, VK_E, "Expand all folders in Explorer navigation pane.", "توسيع كافة المجلدات في جزء التنقل."));

            // 76-85: Other Useful Shortcuts
            list.Add(new ShortcutData("76. Win + B", "76. Win + B", "Shortcut_Win_B", new byte[] { VK_LWIN }, 0x42, "Focus system tray.", "التركيز على منطقة الإعلام."));
            list.Add(new ShortcutData("77. Shift + Delete", "77. Shift + Delete", "Shortcut_Shift_Delete", new byte[] { VK_SHIFT }, VK_DELETE, "Permanently delete selected item.", "حذف نهائي للعنصر المحدد."));
            list.Add(new ShortcutData("78. Alt + Print Screen", "78. Alt + Print Screen", "Shortcut_Alt_PrintScrn", new byte[] { VK_MENU }, VK_SNAPSHOT, "Screenshot of active window.", "التقاط صورة للنافذة النشطة."));
            list.Add(new ShortcutData("79. Win + Print Screen", "79. Win + Print Screen", "Shortcut_Win_PrintScrn", new byte[] { VK_LWIN }, VK_SNAPSHOT, "Save screenshot to Pictures folder.", "حفظ لقطة الشاشة في مجلد الصور."));
            list.Add(new ShortcutData("80. Win + G", "80. Win + G", "Shortcut_Win_G", new byte[] { VK_LWIN }, VK_G, "Open Game Bar.", "فتح شريط الألعاب."));
            list.Add(new ShortcutData("81. Win + Alt + R", "81. Win + Alt + R", "Shortcut_Win_Alt_R", new byte[] { VK_LWIN, VK_MENU }, VK_R, "Start/stop screen recording.", "بدء/إيقاف تسجيل الشاشة."));
            list.Add(new ShortcutData("82. Win + Alt + PrtSc", "82. Win + Alt + PrtSc", "Shortcut_Win_Alt_PrtSc", new byte[] { VK_LWIN, VK_MENU }, VK_SNAPSHOT, "Take screenshot of game.", "التقاط لقطة شاشة للعبة."));
            list.Add(new ShortcutData("83. Win + ;", "83. Win + ;", "Shortcut_Win_Semi", new byte[] { VK_LWIN }, 0xBA, "Open Emoji panel (alternative).", "فتح لوحة الرموز التعبيرية."));
            list.Add(new ShortcutData("84. Win + Shift + Up", "84. Win + Shift + Up", "Shortcut_Win_Shift_Up", new byte[] { VK_LWIN, VK_SHIFT }, VK_UP, "Stretch window to top and bottom.", "تمديد النافذة لأعلى وأسفل."));
            list.Add(new ShortcutData("85. Win + Shift + Down", "85. Win + Shift + Down", "Shortcut_Win_Shift_Down", new byte[] { VK_LWIN, VK_SHIFT }, VK_DOWN, "Restore vertically maximized window.", "استعادة تمديد النافذة العمودي."));

            // 86-90: System & Dialog
            list.Add(new ShortcutData("86. Win + Shift + 1", "86. Win + Shift + 1", "Shortcut_Win_Shift_1", new byte[] { VK_LWIN, VK_SHIFT }, VK_1, "Open new instance of taskbar app.", "فتح نسخة جديدة من التطبيق المثبت."));
            list.Add(new ShortcutData("87. Ctrl + Alt + Tab", "87. Ctrl + Alt + Tab", "Shortcut_Ctrl_Alt_Tab", new byte[] { VK_CONTROL, VK_MENU }, VK_TAB, "Open persistent task switcher.", "فتح أداة التبديل الدائمة."));
            list.Add(new ShortcutData("88. Win + Pause", "88. Win + Pause", "Shortcut_Win_Pause2", new byte[] { VK_LWIN }, 0x13, "Open System Information.", "فتح معلومات النظام."));
            list.Add(new ShortcutData("89. Ctrl + Alt + Del", "89. Ctrl + Alt + Del", "Shortcut_Ctrl_Alt_Del", new byte[] { VK_CONTROL, VK_MENU }, VK_DELETE, "Access security options.", "الوصول لخيارات الأمان."));
            list.Add(new ShortcutData("90. Win + Ctrl + Q", "90. Win + Ctrl + Q", "Shortcut_Win_Ctrl_Q", new byte[] { VK_LWIN, VK_CONTROL }, VK_Q, "Open Quick Assist.", "فتح المساعدة السريعة."));

            // 91-93: Explorer Navigation
            list.Add(new ShortcutData("91. Alt + D", "91. Alt + D", "Shortcut_Alt_D", new byte[] { VK_MENU }, VK_D, "Focus address bar.", "التركيز على شريط العنوان."));
            list.Add(new ShortcutData("92. Backspace", "92. Backspace", "Shortcut_Backspace", null, VK_BACK, "Go back in Explorer/browser.", "الرجوع للخلف."));
            list.Add(new ShortcutData("93. Ctrl + Scroll", "93. Ctrl + Scroll", "Shortcut_Ctrl_Scroll", new byte[] { VK_CONTROL }, VK_UP, "Change icon size in Explorer (simulated).", "تغيير حجم الرموز (محاكاة)."));

            // 94-100: More Text Editing / OS Features
            list.Add(new ShortcutData("94. Ctrl + B", "94. Ctrl + B", "Shortcut_Ctrl_B", new byte[] { VK_CONTROL }, 0x42, "Bold selected text.", "تنسيق النص عريض."));
            list.Add(new ShortcutData("95. Ctrl + I", "95. Ctrl + I", "Shortcut_Ctrl_I", new byte[] { VK_CONTROL }, VK_I, "Italicize selected text.", "تنسيق النص مائل."));
            list.Add(new ShortcutData("96. Ctrl + U", "96. Ctrl + U", "Shortcut_Ctrl_U", new byte[] { VK_CONTROL }, VK_U, "Underline selected text.", "تسطير النص المحدد."));
            list.Add(new ShortcutData("97. Win + H", "97. Win + H", "Shortcut_Win_H", new byte[] { VK_LWIN }, VK_H, "Start voice typing.", "بدء الإملاء الصوتي."));
            list.Add(new ShortcutData("98. Win + N", "98. Win + N", "Shortcut_Win_N", new byte[] { VK_LWIN }, VK_N, "Open notification center.", "فتح مركز الإشعارات."));
            list.Add(new ShortcutData("99. Win + Z", "99. Win + Z", "Shortcut_Win_Z", new byte[] { VK_LWIN }, VK_Z, "Open snap layouts.", "فتح تخطيطات النوافذ."));
            list.Add(new ShortcutData("100. Win + F1", "100. Win + F1", "Shortcut_Win_F1", new byte[] { VK_LWIN }, VK_F1, "Open Windows help.", "فتح تعليمات Windows."));

            // 101-105: Extras
            list.Add(new ShortcutData("101. Win + Shift + Right", "101. Win + Shift + Right", "Shortcut_Win_Shift_Right", new byte[] { VK_LWIN, VK_SHIFT }, VK_RIGHT, "Move window to next monitor.", "نقل النافذة إلى الشاشة التالية."));
            list.Add(new ShortcutData("102. Ctrl + Alt + D", "102. Ctrl + Alt + D", "Shortcut_Ctrl_Alt_D", new byte[] { VK_CONTROL, VK_MENU }, VK_D, "Toggle dictation mode.", "تبديل وضع الإملاء."));
            list.Add(new ShortcutData("103. Win + Alt + D", "103. Win + Alt + D", "Shortcut_Win_Alt_D", new byte[] { VK_LWIN, VK_MENU }, VK_D, "Open date/time flyout.", "فتح نافذة التاريخ والوقت."));
            list.Add(new ShortcutData("104. Win + Shift + C", "104. Win + Shift + C", "Shortcut_Win_Shift_C", new byte[] { VK_LWIN, VK_SHIFT }, VK_C, "Open charms menu (Windows 8 legacy).", "فتح قائمة التشارم (قديم)."));
            list.Add(new ShortcutData("105. Ctrl + Shift + Q", "105. Ctrl + Shift + Q", "Shortcut_Ctrl_Shift_Q", new byte[] { VK_CONTROL, VK_SHIFT }, VK_Q, "Sign out of Windows (if configured).", "تسجيل الخروج من Windows (إذا تم تكوينه)."));

            // 106-108: New Added Shortcuts
            list.Add(new ShortcutData("106. Win + T", "106. Win + T", "Shortcut_Win_T", new byte[] { VK_LWIN }, VK_T, "Cycle through apps on the taskbar.", "التنقل بين التطبيقات في شريط المهام."));
            list.Add(new ShortcutData("107. Win + Y", "107. Win + Y", "Shortcut_Win_Y", new byte[] { VK_LWIN }, VK_Y, "Switch input between desktop and Mixed Reality.", "تبديل الإدخال بين سطح المكتب والواقع المختلط."));
            list.Add(new ShortcutData("108. Ctrl + Shift + S", "108. Ctrl + Shift + S", "Shortcut_Ctrl_Shift_S", new byte[] { VK_CONTROL, VK_SHIFT }, VK_S, "Save As command in many applications.", "أمر الحفظ باسم في العديد من التطبيقات."));

            return list;
        }
    }
}