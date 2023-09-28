using System.Runtime.InteropServices;

namespace WASimpleTray
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CustomContext());
        }
    }

    public class CustomContext : ApplicationContext
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        const uint WM_COMMAND = 0x0111;
        const uint WM_USER = 0x0400;

        const uint IPC_ISPLAYING = 104;
        const uint WINAMP_BUTTON1 = 40044;
        const uint WINAMP_BUTTON2 = 40045;
        const uint WINAMP_BUTTON3 = 40046;
        const uint WINAMP_BUTTON4 = 40047;
        const uint WINAMP_BUTTON5 = 40048;

        private NotifyIcon trayIcon;

        public CustomContext()
        {
            trayIcon = new NotifyIcon();
            trayIcon.Icon = Properties.Resources.winamp;
            trayIcon.ContextMenuStrip = new ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Previous", null, Previous));
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Play/Pause", null, PlayPause));
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Stop", null, Stop));
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Next", null, Next));
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, Exit));
            trayIcon.MouseClick += TrayClick;
            trayIcon.MouseDoubleClick += TrayDoubleClick;

            trayIcon.Visible = true;
        }

        void Previous(object? sender, EventArgs e)
        {
            IntPtr winampWnd = FindWindow("Winamp v1.x", null);
            SendMessage(winampWnd, WM_COMMAND, WINAMP_BUTTON1, 0);
        }

        void PlayPause(object? sender, EventArgs e)
        {
            IntPtr winampWnd = FindWindow("Winamp v1.x", null);
            if ((int)SendMessage(winampWnd, WM_USER, 0, IPC_ISPLAYING) == 1)
                SendMessage(winampWnd, WM_COMMAND, WINAMP_BUTTON3, 0);
            else
                SendMessage(winampWnd, WM_COMMAND, WINAMP_BUTTON2, 0);
        }
        void Stop(object? sender, EventArgs e)
        {
            IntPtr winampWnd = FindWindow("Winamp v1.x", null);
            SendMessage(winampWnd, WM_COMMAND, WINAMP_BUTTON4, 0);
        }
        void Next(object? sender, EventArgs e)
        {
            IntPtr winampWnd = FindWindow("Winamp v1.x", null);
            SendMessage(winampWnd, WM_COMMAND, WINAMP_BUTTON5, 0);
        }

        void Exit(object? sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Application.Exit();
        }

        void TrayClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PlayPause(null, null);
            }
        }

        void TrayDoubleClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Next(null, null);
                PlayPause(null, null);
            }
        }
    }
}
