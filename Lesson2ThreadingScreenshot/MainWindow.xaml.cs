using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;
using Path = System.IO.Path;
using Size = System.Drawing.Size;

namespace Lesson2ThreadingScreenshot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScreenShotTaker? _screenshotTaker;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _screenshotTaker = new ScreenShotTaker() {Log = Log};
            _screenshotTaker.Start();
        }

        private void TakeScreenShotButton_Click(object sender, RoutedEventArgs e)
        {
            _screenshotTaker?.Take();
        }

        private void TerminateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_screenshotTaker is null) return;
            _screenshotTaker.Terminate();
        }

        private void Log(string message)
        {
            InfoArea.Dispatcher.Invoke(() =>
            {
                InfoArea.Text += message + "\n";
            });
        }
    }

    public class ScreenShotTaker
    {
        private readonly ManualResetEvent _terminate = new ManualResetEvent(false);
        private readonly ManualResetEvent _take = new ManualResetEvent(false);
        private bool _started = false;
        private const string WorkingDir = ".";

        public delegate void LogDelegate(string message);

        public required LogDelegate Log { get; init; }

        public void Start()
        {
            if (_started) return;
            _started = true;
            var worker = new Thread(HandleTake)
            {
                IsBackground = true
            };
            worker.Start();
        }

        private void HandleTake()
        {
            Log("截屏进程开始。");

            var directory = System.IO.Directory.CreateDirectory(Path.Join(WorkingDir, "screenshot"));
            var bounds = Screen.PrimaryScreen!.Bounds;
            var width = bounds.Width; var height = bounds.Height;
            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);
            while (!_terminate.WaitOne(1))
            {
                if (!_take.WaitOne(500)) continue;
                var now = DateTime.Now.ToString("yy-MMM-dd-hh-mm-ss");
                var filename = Path.Join(directory.FullName, $"{now}.png");
                graphics.CopyFromScreen(0, 0, 0, 0, new Size(width, height));
                bitmap.Save(filename, ImageFormat.Png);
                Log($"截图保存至{now}.png");
                _take.Reset();
            }

            Log("截屏进程结束。");
        }

        public void Take()
        {
            _take.Set();
        }

        public void Terminate()
        {
            _terminate.Set();
        }
    }
}
