using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Hyper;

namespace WpfMenu.Pages;

public partial class LoadGame : UserControl
{
    private readonly ObservableCollection<string> _saves = new();

    public string selectedSave = null!;
    
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
        string directoryPath = Game.SavesLocation;

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
        selectedSave = (string)Saves.SelectedItem!;
        Visibility = Visibility.Collapsed;
    }
}