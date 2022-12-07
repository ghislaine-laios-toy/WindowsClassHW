using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace FolderWatcherWPF;

internal class FolderWatcherViewModel : INotifyPropertyChanged
{
    private readonly ManualResetEvent _stopSignal = new(false);

    private string _directory = "";
    private bool _isWorking;

    public FolderWatcherViewModel()
    {
        Logs.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(Logs));
    }

    public string Directory
    {
        get => _directory;
        set
        {
            Logs.Add($"目录 {value} 已被选中为监听源。");
            SetField(ref _directory, value);
        }
    }

    public ObservableCollection<string> Logs { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public bool TryStartingWork()
    {
        if (_isWorking) return false;
        _isWorking = true;
        var worker = new Thread(WatchDir)
        {
            IsBackground = true
        };
        _stopSignal.Reset();
        worker.Start();
        return true;
    }

    public void StopWork()
    {
        _isWorking = false;
        _stopSignal.Set();
    }

    private void WatchDir()
    {
        var dir = new DirectoryInfo(_directory);
        System.IO.Directory.CreateDirectory($"{_directory}\\ref");

        do
        {
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var newPath = $"{_directory}\\ref\\{file.Name}";
                if (File.Exists(newPath)) continue;
                File.Move(file.FullName, newPath);
                Application.Current.Dispatcher.Invoke(() => { Logs.Add($"监听并移动了文件 {file.FullName}"); });
            }
        } while (!_stopSignal.WaitOne(500));
    }
}