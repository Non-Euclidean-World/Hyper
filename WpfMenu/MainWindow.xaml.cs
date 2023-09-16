using System;
using System.Windows;
using System.Windows.Input;
using Hyper;
using Window = System.Windows.Window;

namespace WpfMenu;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void NewGameButton_OnClick(object sender, RoutedEventArgs e)
    {
        GamePage.Load((int)ActualWidth, (int)ActualHeight);
    }
    
    private void LoadGameButton_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void QuitButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void GamePage_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (GamePage.Visibility == Visibility.Visible)
        {
            NewGameButton.Visibility = Visibility.Collapsed;
            LoadGameButton.Visibility = Visibility.Collapsed;
            QuitButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            NewGameButton.Visibility = Visibility.Visible;
            LoadGameButton.Visibility = Visibility.Visible;
            QuitButton.Visibility = Visibility.Visible;
            Cursor = Cursors.Arrow;
        }
    }
}
