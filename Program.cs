using System;
using System.Threading;
using System.Windows.Forms;

namespace Click2Key
{
    internal static class Program
    {
        private const string AppMutexName = "Click2Key_SingleInstance_Mutex";
        private const string AppEventName = "Click2Key_RestoreEvent";
        private const int WM_RESTORE_APP = 0x8001;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Try to create the mutex
            using (Mutex mutex = new Mutex(true, AppMutexName, out bool createdNew))
            {
                if (!createdNew)
                {
                    // Another instance is already running → signal it to restore
                    try
                    {
                        using (var restoreEvent = new EventWaitHandle(false, EventResetMode.AutoReset, AppEventName))
                        {
                            restoreEvent.Set();
                        }
                    }
                    catch { }
                    return; // exit this instance
                }

                // First instance – start the main form
                frmMain mainForm = new frmMain();
                Application.Run(mainForm);

                // Keep the mutex alive until the app exits
            }
        }
    }
}
