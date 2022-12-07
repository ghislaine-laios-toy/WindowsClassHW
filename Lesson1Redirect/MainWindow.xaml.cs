using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace WindowsClassHW;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
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
        process.OutputDataReceived += strOutputHandler;
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
            mystr.cbData = Encoding.ASCII.GetByteCount(outLine.Data);
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

    [StructLayout(LayoutKind.Sequential)]
    private struct COPYDATASTRUCT
    {
        public IntPtr dwData; // Any value the sender chooses.  Perhaps its main window handle?
        public int cbData; // The count of bytes in the message.
        [MarshalAs(UnmanagedType.LPStr)] public string lpData;
    }
}