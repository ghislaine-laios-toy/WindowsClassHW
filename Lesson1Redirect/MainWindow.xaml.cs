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

namespace WindowsClassHW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [StructLayout(LayoutKind.Sequential)]
        struct COPYDATASTRUCT
        {
            public IntPtr dwData;    // Any value the sender chooses.  Perhaps its main window handle?
            public int cbData;       // The count of bytes in the message.
            [MarshalAs(UnmanagedType.LPStr)] public string lpData;
        }

        const int WM_COPYDATA = 0x004A;

        private IntPtr targetWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource? source = PresentationSource.FromVisual(this) as HwndSource;
            if (source == null) throw new Exception("Oops!");
            source.AddHook(wndProc);
            targetWindow = FindWindow(null, "demoWindow");
        }

        private IntPtr wndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_COPYDATA)
            {
                var cps = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));
                outputBlock.Text += cps.lpData + "\n";
            }
            return IntPtr.Zero;
        }

        private void runCommand(string command)
        {
            outputBlock.Text = "";
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.OutputDataReceived += new DataReceivedEventHandler(strOutputHandler);
            process.Start();
            process.BeginOutputReadLine();
            process.StandardInput.WriteLine(command);
            process.StandardInput.WriteLine("exit");
        }

        private void strOutputHandler(object sender, DataReceivedEventArgs outLine)
        {
            Trace.WriteLine(outLine.Data);
            var mystr = new COPYDATASTRUCT();
            mystr.dwData = (IntPtr)0;
            if (outLine.Data == null)
            {
                mystr.lpData = "";
                mystr.cbData = 0;
            }
            else
            {
                mystr.lpData = outLine.Data;
                mystr.cbData = System.Text.Encoding.ASCII.GetByteCount(outLine.Data);
            }
            SendMessage(targetWindow, WM_COPYDATA, (IntPtr)0, ref mystr);
        }

        private void getmac_Button_Click(object sender, RoutedEventArgs e)
        {
            runCommand("getmac");
        }

        private void shutdown_Button_Click(object sender, RoutedEventArgs e)
        {
            runCommand("shutdown /s");
        }
    }
}
