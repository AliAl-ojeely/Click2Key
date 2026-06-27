using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Click2Key
{
    public static class AppSettings
    {
        private static readonly string settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Click2Key",
            "windowSettings.txt");

        public static void SaveWinodwState(Form form)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
            var lines = new string[]
            {
                form.WindowState.ToString(),
                form.Size.Width.ToString(),
                form.Size.Height.ToString(),
                form.Location.X.ToString(),
                form.Location.Y.ToString(),
            };

            File.WriteAllLines(settingsPath, lines);
        }

        public static void LoadWindowState(Form form)
        {
            if (!File.Exists(settingsPath)) return;

            try
            {
                var lines = File.ReadAllLines(settingsPath);
                if (lines.Length < 5) return;

                FormWindowState state;
                if (Enum.TryParse(lines[0], out state))
                {
                    form.WindowState = state;
                    // If saved as maximized, we don’t need to set size/location

                    if (state == FormWindowState.Normal)
                    {
                        int w = int.Parse(lines[1]);
                        int h = int.Parse(lines[2]);
                        int x = int.Parse(lines[3]);
                        int y = int.Parse(lines[4]);


                        // Ensure the window is within the current screen bounds
                        if (Screen.FromControl(form).WorkingArea.Contains(new Rectangle(x, y, w, h)))
                        {
                            form.Size = new Size(w, h);
                            form.Location = new Point(x, y);
                        }


                        else
                        {
                            // Fallback to adaptive default
                            SetDefaultSize(form);
                        }
                    }
                }
            }
            catch { 
                // Ignore corrputed settings
            }
        }

        private static void SetDefaultSize(Form form)
        {
            Screen screen = Screen.FromControl(form);
            Rectangle area = screen.WorkingArea;

            form.ClientSize = new Size((int)(area.Width * 0.8), (int)(area.Height * 0.8));

            form.Location = new Point
                ((area.Width - form.Width) / 2,
                (area.Height - form.Height) / 2);
        }
    }
}

