using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Click2Key
{
    public partial class frmAbout : Form
    {
        public frmAbout(bool isLightTheme, bool isArabic)
        {
            InitializeComponent();

            // 1. Remove any old WinForms controls (just to be safe)
            this.Controls.Clear();

            // 2. Create the WPF user control
            var wpfAbout = new UsrCtrlAbout();

            // 3. Host it – dock it to fill the entire form
            ElementHost host = new ElementHost
            {
                Dock = DockStyle.Fill,
                Child = wpfAbout
            };

            this.Controls.Add(host);

            // 4. Set the exact window size you want
            this.ClientSize = new Size(437, 571);

            // 5. Form appearance
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = isArabic ? "عن المطور" : "Developer Info";

            // 6. Match the form background to the WPF background (dark #09090B)
            this.BackColor = ColorTranslator.FromHtml("#09090B");
        }
    }
}