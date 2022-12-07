using System.Windows;
using System.Windows.Forms;

namespace FolderWatcherWPF;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly FolderWatcherViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = (FolderWatcherViewModel)DataContext;
    }

    private void ChooseDirectory_Button_Click(object sender, RoutedEventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
        var result = dialog.SelectedPath;
        if (result == null) return;
        _viewModel.Directory = result;
    }

    private void StartWatching_Button_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Logs.Add("开始监听。");
        _viewModel.TryStartingWork();
    }

    private void StopWatching_Button_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Logs.Add("结束监听。");
        _viewModel.StopWork();
    }

    private void Demo_Button_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Logs.Add("Why you click this button?");
    }
}