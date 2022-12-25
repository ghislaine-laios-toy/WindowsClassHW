using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lesson1Ping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _cmd = "ping www.baidu.com -n 4";

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData; // Any value the sender chooses.  Perhaps its main window handle?
            public int cbData; // The count of bytes in the message.
            [MarshalAs(UnmanagedType.LPStr)] public string lpData;
        }

        private const int WM_COPYDATA = 0x004A;

        private IntPtr targetWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            if (source == null) throw new Exception("Oops!");
            source.AddHook(wndProc);
            targetWindow = FindWindow(null, "demoWindow");
        }

        private IntPtr wndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_COPYDATA)
            {
                var cps = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));
                InfoArea.Text += cps.lpData + "\n";
            }

            return IntPtr.Zero;
        }

        private void ButtonSyncPing_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process()
            {
                StartInfo =
                {
                    FileName = "powershell.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                }
            };
            process.Start();
            process.StandardInput.WriteLine(_cmd);
            process.StandardInput.WriteLine("exit");
            InfoArea.Text = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
        }

        private void ButtonAsyncPing_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process()
            {
                StartInfo =
                {
                    FileName = "powershell.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };
            process.OutputDataReceived += StrOutputHandler;
            process.Start();
            process.BeginOutputReadLine();
            process.StandardInput.WriteLine(_cmd);
            process.StandardInput.WriteLine("exit");
        }

        private void StrOutputHandler(object sender, DataReceivedEventArgs e)
        {
            var mystr = new COPYDATASTRUCT();
            mystr.dwData = (IntPtr)0;
            if (e.Data == null)
            {
                mystr.lpData = "";
                mystr.cbData = 0;
            }
            else
            {
                mystr.lpData = e.Data;
                mystr.cbData = Encoding.ASCII.GetByteCount(e.Data);
            }

            SendMessage(targetWindow, WM_COPYDATA, (IntPtr)0, ref mystr);
        }
    }
}
