using System;
using System.Collections.Generic;
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

namespace Lesson3FireEvent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FireAlarm _fireAlarm = new FireAlarm();

        public MainWindow()
        {
            InitializeComponent();
            _fireAlarm.FireEvent += HandleFire;
        }

        private void Button_KitchenFire(object sender, RoutedEventArgs e)
        {
            _fireAlarm.ReportFire("Kitchen", 5);
        }

        private void Button_LivingRoomFire(object sender, RoutedEventArgs e)
        {
            _fireAlarm.ReportFire("Living Room", 10);
        }

        private void HandleFire(object sender, FireEventArgs e)
        {
            AddInfo($"{e.Room} is burning! Ferocity: {e.Ferocity}; Total Ferocity: {e.TotalFerocity}");
        }

        private void AddInfo(string info)
        {
            InfoArea.Dispatcher.Invoke(() =>
            {
                InfoArea.Text += $"{info}\n";
            });
        }
    }

    public class FireEventArgs : EventArgs
    {
        public required string Room { get; init; }
        public required int Ferocity { get; init; }
        public required int TotalFerocity { get; init; }
    }

    public class FireAlarm
    {
        public delegate void FireEventHandler(object sender, FireEventArgs e);

        public event FireEventHandler? FireEvent;

        private int _totalFerocity = 0;
        private readonly ISet<string> _burningRoom = new HashSet<string>();

        public void ReportFire(string room, int ferocity)
        {
            if (_burningRoom.Contains(room)) return;
            _totalFerocity += ferocity;
            _burningRoom.Add(room);
            FireEvent?.Invoke(this, new FireEventArgs {Room = room, Ferocity = ferocity, TotalFerocity = _totalFerocity});
        }
    }
}
