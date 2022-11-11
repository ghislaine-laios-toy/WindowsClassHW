using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Point = System.Windows.Point;

namespace Lesson2Screenshot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _startPoint, _rectPoint;

        private bool _isMouseDown = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            _startPoint = e.GetPosition(null);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMouseDown) return;
            var point = e.GetPosition(null);
            Rect.Width = Math.Abs(point.X - _startPoint.X);
            Rect.Height = Math.Abs(point.Y - _startPoint.Y);
            _rectPoint = new Point(point.X < _startPoint.X ? point.X : _startPoint.X, point.Y < _startPoint.Y ? point.Y : _startPoint.Y);
            Canvas.SetLeft(Rect, _rectPoint.X);
            Canvas.SetTop(Rect, _rectPoint.Y);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            Trace.WriteLine("Mouse up!");
            TakeScreenshot(_rectPoint.X, _rectPoint.Y, Rect.Width, Rect.Height);
            this.Close();
        }

        private void TakeScreenshot(double x, double y, double width, double height)
        {
            using var bmp = new Bitmap((int)width, (int)height);
            using var g = Graphics.FromImage(bmp);
            g.CopyFromScreen((int)x, (int)y, 0, 0, bmp.Size);
            Clipboard.SetDataObject(bmp);
            Clipboard.Flush();
        }
    }
}
