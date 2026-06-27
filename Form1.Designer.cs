namespace Click2Key
{
    partial class frmMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.wpfShortcutCanvas1 = new Click2Key.WpfShortcutCanvas();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(1278, 721);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Child = this.wpfShortcutCanvas1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1278, 721);
            this.Controls.Add(this.elementHost1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Name = "frmMain";
            this.Text = "Click2Key v1.2";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private WpfShortcutCanvas wpfShortcutCanvas1;
    }
}