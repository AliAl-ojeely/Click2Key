namespace Click2Key
{
    partial class ShortcutControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel = new System.Windows.Forms.Panel();
            this.lblCounter = new System.Windows.Forms.Label();
            this.lblShortcutText = new System.Windows.Forms.Label();
            this.btnInfo = new ReaLTaiizor.Controls.MaterialButton();
            this.btnExecute = new ReaLTaiizor.Controls.MaterialButton();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.Color.Transparent;
            this.panel.Controls.Add(this.lblCounter);
            this.panel.Controls.Add(this.lblShortcutText);
            this.panel.Controls.Add(this.btnInfo);
            this.panel.Controls.Add(this.btnExecute);
            this.panel.Location = new System.Drawing.Point(3, 11);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(232, 100);
            this.panel.TabIndex = 12;
            // 
            // lblCounter
            // 
            this.lblCounter.AutoSize = true;
            this.lblCounter.Font = new System.Drawing.Font("Cairo", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCounter.ForeColor = System.Drawing.SystemColors.Window;
            this.lblCounter.Location = new System.Drawing.Point(173, 48);
            this.lblCounter.Name = "lblCounter";
            this.lblCounter.Size = new System.Drawing.Size(58, 36);
            this.lblCounter.TabIndex = 16;
            this.lblCounter.Text = "Label";
            // 
            // lblShortcutText
            // 
            this.lblShortcutText.AutoSize = true;
            this.lblShortcutText.Font = new System.Drawing.Font("Cairo", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShortcutText.ForeColor = System.Drawing.SystemColors.Window;
            this.lblShortcutText.Location = new System.Drawing.Point(3, 6);
            this.lblShortcutText.Name = "lblShortcutText";
            this.lblShortcutText.Size = new System.Drawing.Size(58, 36);
            this.lblShortcutText.TabIndex = 12;
            this.lblShortcutText.Text = "Label";
            // 
            // btnInfo
            // 
            this.btnInfo.AutoSize = false;
            this.btnInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnInfo.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnInfo.Depth = 0;
            this.btnInfo.Font = new System.Drawing.Font("Cairo", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInfo.HighEmphasis = true;
            this.btnInfo.Icon = null;
            this.btnInfo.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            this.btnInfo.Location = new System.Drawing.Point(102, 48);
            this.btnInfo.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnInfo.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnInfo.Size = new System.Drawing.Size(64, 36);
            this.btnInfo.TabIndex = 11;
            this.btnInfo.Text = "!";
            this.btnInfo.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnInfo.UseAccentColor = false;
            this.btnInfo.UseMnemonic = false;
            this.btnInfo.UseVisualStyleBackColor = true;
            // 
            // btnExecute
            // 
            this.btnExecute.AutoSize = false;
            this.btnExecute.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnExecute.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnExecute.Depth = 0;
            this.btnExecute.Font = new System.Drawing.Font("Cairo", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExecute.HighEmphasis = true;
            this.btnExecute.Icon = null;
            this.btnExecute.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            this.btnExecute.Location = new System.Drawing.Point(10, 48);
            this.btnExecute.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnExecute.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnExecute.Size = new System.Drawing.Size(84, 36);
            this.btnExecute.TabIndex = 10;
            this.btnExecute.Text = "Button";
            this.btnExecute.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnExecute.UseAccentColor = false;
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // ShortcutControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel);
            this.Name = "ShortcutControl";
            this.Size = new System.Drawing.Size(238, 114);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Label lblCounter;
        private System.Windows.Forms.Label lblShortcutText;
        private ReaLTaiizor.Controls.MaterialButton btnInfo;
        private ReaLTaiizor.Controls.MaterialButton btnExecute;
    }
}
