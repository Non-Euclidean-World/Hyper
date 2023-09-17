using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common;
using Hyper;

namespace WpfMenu.Pages;

public partial class LoadGame : UserControl
{
    public event EventHandler<string> LoadGameEvent = null!;
    
    private readonly ObservableCollection<string> _saves = new();
    
    public LoadGame()
    {
        InitializeComponent();
        
        Saves.ItemsSource = _saves;
    }

    private void Saves_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true) RefreshList();
    }
    
    private void RefreshList()
    {
        string directoryPath = Settings.SavesLocation;

        _saves.Clear();

        var files = Directory.GetDirectories(directoryPath)
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.LastWriteTime)
            .Select(f => f.Name)
            .ToList();

        foreach (var file in files)
        {
            _saves.Add(file);
        }
    }

    private void Saves_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Saves.SelectedItem is null) return;
        
        Visibility = Visibility.Collapsed;
        LoadGameEvent?.Invoke(this, (string)Saves.SelectedItem!);
    }

    private void ReturnButton_OnClick(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }
}