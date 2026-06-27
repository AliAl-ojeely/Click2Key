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

        /// <summary>
        /// Saves the current form size, location, and window state.
        /// </summary>
        public static void SaveWindowState(Form form)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
            var lines = new string[]
            {
                form.WindowState.ToString(),
                form.Size.Width.ToString(),
                form.Size.Height.ToString(),
                form.Location.X.ToString(),
                form.Location.Y.ToString()
            };
            File.WriteAllLines(settingsPath, lines);
        }

        /// <summary>
        /// Loads the saved window state. Returns true if successful.
        /// </summary>
        public static bool LoadWindowState(Form form)
        {
            if (!File.Exists(settingsPath))
                return false;

            try
            {
                var lines = File.ReadAllLines(settingsPath);
                if (lines.Length < 5) return false;

                // Parse state
                FormWindowState state;
                if (!Enum.TryParse(lines[0], out state))
                    return false;

                form.WindowState = state;

                // If normal, set size and location
                if (state == FormWindowState.Normal)
                {
                    int w = int.Parse(lines[1]);
                    int h = int.Parse(lines[2]);
                    int x = int.Parse(lines[3]);
                    int y = int.Parse(lines[4]);

                    // Make sure it's within current screen bounds
                    Rectangle newRect = new Rectangle(x, y, w, h);
                    if (Screen.FromControl(form).WorkingArea.IntersectsWith(newRect))
                    {
                        form.Size = new Size(w, h);
                        form.Location = new Point(x, y);
                    }
                    else
                    {
                        // Fallback: center 80% default
                        ApplyDefaultSize(form);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void ApplyDefaultSize(Form form)
        {
            Screen screen = Screen.FromControl(form);
            Rectangle area = screen.WorkingArea;
            form.ClientSize = new Size((int)(area.Width * 0.8), (int)(area.Height * 0.8));
            form.Location = new Point((area.Width - form.Width) / 2,
                                      (area.Height - form.Height) / 2);
        }
    }
}