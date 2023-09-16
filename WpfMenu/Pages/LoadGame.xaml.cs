using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfMenu.Pages;

public partial class LoadGame : UserControl
{
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
        string directoryPath = @"C:\path\to\directory";

        _saves.Clear();

        var files = Directory.GetFiles(directoryPath)
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.LastWriteTime)
            .Select(f => f.Name)
            .ToList();

        foreach (var file in files)
        {
            _saves.Add(file);
        }
    }
}