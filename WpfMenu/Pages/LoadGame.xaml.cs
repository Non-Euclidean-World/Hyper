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
        _saves.Clear();

        foreach (var file in SaveManager.GetSaves())
        {
            _saves.Add(file);
        }
    }

    private void ReturnButton_OnClick(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }

    private void LoadButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (Saves.SelectedItem is null) return;

        Visibility = Visibility.Collapsed;
        LoadGameEvent?.Invoke(this, (string)Saves.SelectedItem!);
    }

    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (Saves.SelectedItem is null) return;

        SaveManager.DeleteSave(Saves.SelectedItem.ToString()!);
        RefreshList();
    }
}