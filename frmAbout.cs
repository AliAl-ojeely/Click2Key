using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ReaLTaiizor.Forms;

namespace Click2Key
{
    public partial class frmAbout : CrownForm
    {
        private bool isLightTheme;
        private bool isArabic;

        public frmAbout(bool currentThemeIsLight, bool currentLangIsArabic)
        {
            InitializeComponent();

            this.isLightTheme = currentThemeIsLight;
            this.isArabic = currentLangIsArabic;
            this.ResizeRedraw = true;

            // Apply theme immediately
            ApplyGradientBackground();

            // Manually wire link click events (redundant if done in designer, but guarantees correctness)
            lnklblEmail.LinkClicked += lnklblEmail_LinkClicked;
            lnklblGithub.LinkClicked += lnklblGithub_LinkClicked;
            lnklblPortfolio.LinkClicked += lnklblPortfolio_LinkClicked;

            // Optional: remove static underline for a cleaner look
            lnklblNmae.LinkBehavior = LinkBehavior.HoverUnderline;
            lnklblEmail.LinkBehavior = LinkBehavior.HoverUnderline;
            lnklblGithub.LinkBehavior = LinkBehavior.HoverUnderline;
            lnklblPortfolio.LinkBehavior = LinkBehavior.HoverUnderline;
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            SetupLanguage();
        }

        private void SetupLanguage()
        {
            if (isArabic)
            {
                this.Text = "عن المطور";
                lnklblNmae.Text = "المطور: علي العجيلي | Ali Al-ojeely";
                lnklblEmail.Text = "البريد: alialojeely@gmail.com";
                lnklblGithub.Text = "حساب GitHub";
                lnklblPortfolio.Text = "معرض الأعمال (Portfolio)";
                lnklblVersion.Text = "اصدار: 1.1";

                lnklblNmae.RightToLeft = RightToLeft.Yes;
                lnklblEmail.RightToLeft = RightToLeft.Yes;
                lnklblGithub.RightToLeft = RightToLeft.Yes;
                lnklblPortfolio.RightToLeft = RightToLeft.Yes;
                lnklblVersion.RightToLeft = RightToLeft.Yes;

            }
            else
            {
                this.Text = "Developer Info";
                lnklblNmae.Text = "Developer: Ali Al-ojeely";
                lnklblEmail.Text = "Email: alialojeely@gmail.com";
                lnklblGithub.Text = "GitHub Profile";
                lnklblPortfolio.Text = "Portfolio Website";
                lnklblVersion.Text = "Version: 1.1";

                lnklblNmae.RightToLeft = RightToLeft.No;
                lnklblEmail.RightToLeft = RightToLeft.No;
                lnklblGithub.RightToLeft = RightToLeft.No;
                lnklblPortfolio.RightToLeft = RightToLeft.No;
                lnklblVersion.RightToLeft = RightToLeft.No;
            }
        }

        private void lnklblEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "mailto:alialojeely@gmail.com",
                UseShellExecute = true
            });
        }

        private void lnklblGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/AliAl-ojeely",
                UseShellExecute = true
            });
        }

        private void lnklblPortfolio_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://alial-ojeely.github.io/",
                UseShellExecute = true
            });
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyGradientBackground();
        }

        private void ApplyGradientBackground()
        {
            if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0) return;

            Bitmap gradientBmp = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
            using (Graphics g = Graphics.FromImage(gradientBmp))
            {
                Color startColor = isLightTheme ? Color.White : Color.FromArgb(10, 25, 55);
                Color endColor = isLightTheme ? Color.FromArgb(150, 180, 195) : Color.FromArgb(30, 50, 50);

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    this.ClientRectangle, startColor, endColor, LinearGradientMode.ForwardDiagonal))
                {
                    g.FillRectangle(brush, this.ClientRectangle);
                }
            }

            this.BackgroundImage = gradientBmp;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            Color textColor = isLightTheme ? Color.Black : Color.White;
            Color separatorColor = isLightTheme ? Color.FromArgb(180, 180, 180) : Color.White;

            UpdateThemeColors(this, textColor, separatorColor);
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

                if (c is LinkLabel link)
                {
                    link.BackColor = Color.Transparent;
                    link.LinkColor = textColor;
                    link.ActiveLinkColor = Color.Orange;
                }

                if (c.Tag != null && c.Tag.ToString() == "Separator")
                {
                    c.BackColor = separatorColor;
                }

                if (c.HasChildren)
                    UpdateThemeColors(c, textColor, separatorColor);
            }
        }
    }
}