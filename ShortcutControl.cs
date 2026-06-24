using System;
using System.Drawing;
using System.Windows.Forms;

namespace Click2Key
{
    public partial class ShortcutControl : UserControl
    {
        public string LogKey { get; private set; }
        public byte MainKey { get; private set; }
        public byte[] Modifiers { get; private set; }

        private string descEn;
        private string descAr;
        private ToolTip tooltipInstance;

        public event EventHandler<ShortcutExecutionArgs> OnExecuteRequested;

        public ShortcutControl()
        {
            InitializeComponent();
        }

        public void Setup(string englishTitle, string arabicTitle, string logKey,
                          byte[] modifiers, byte mainKey, string infoEn, string infoAr,
                          ToolTip sharedToolTip)
        {
            this.LogKey = logKey;
            this.Modifiers = modifiers;
            this.MainKey = mainKey;

            this.descEn = infoEn;
            this.descAr = infoAr;
            this.tooltipInstance = sharedToolTip;

            lblShortcutText.Tag = new string[] { englishTitle, arabicTitle };
            lblCounter.Text = ClickLogger.GetCount(logKey).ToString();
        }

        public void ApplyLocalization(bool isArabic)
        {
            if (lblShortcutText.Tag is string[] titles)
            {
                lblShortcutText.Text = isArabic ? titles[1] : titles[0];
                btnExecute.Text = isArabic ? "تنفيذ" : "EXECUTE";
            }

            if (tooltipInstance != null && btnInfo != null)
            {
                tooltipInstance.SetToolTip(btnInfo, isArabic ? descAr : descEn);
            }
        }

        public void ApplyTheme(Color textColor)
        {
            lblShortcutText.ForeColor = textColor;
            lblCounter.ForeColor = textColor;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            OnExecuteRequested?.Invoke(this, new ShortcutExecutionArgs(this, lblCounter));
        }
    }

    public class ShortcutExecutionArgs : EventArgs
    {
        public ShortcutControl ControlInstance { get; }
        public Label CounterLabel { get; }

        public ShortcutExecutionArgs(ShortcutControl controlInstance, Label counterLabel)
        {
            ControlInstance = controlInstance;
            CounterLabel = counterLabel;
        }
    }
}