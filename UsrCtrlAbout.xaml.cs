using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Click2Key
{
    public partial class UsrCtrlAbout : UserControl
    {
        public UsrCtrlAbout()
        {
            InitializeComponent();
        }

        private void Email_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("mailto:alialojeely@gmail.com") { UseShellExecute = true });
        }

        private void GitHub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/AliAl-ojeely") { UseShellExecute = true });
        }

        private void Portfolio_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://alial-ojeely.github.io/") { UseShellExecute = true });
        }
    }
}