using System;
using System.Windows;
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
        
        LoadPage.LoadGameEvent += LoadGamePage_OnLoadGameEvent!;
    }

    private void NewGameButton_OnClick(object sender, RoutedEventArgs e)
    {
        GamePage.Load((int)ActualWidth, (int)ActualHeight, SaveNameTextBox.Text);
    }
    
    private void LoadGameButton_OnClick(object sender, RoutedEventArgs e)
    {
        LoadPage.Visibility = Visibility.Visible;
    }

    private void QuitButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void GamePage_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (GamePage.Visibility == Visibility.Visible)
        {
            SaveNameTextBox.Visibility = Visibility.Collapsed;
            NewGameButton.Visibility = Visibility.Collapsed;
            LoadGameButton.Visibility = Visibility.Collapsed;
            QuitButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            SaveNameTextBox.Visibility = Visibility.Visible;
            NewGameButton.Visibility = Visibility.Visible;
            LoadGameButton.Visibility = Visibility.Visible;
            QuitButton.Visibility = Visibility.Visible;
        }
    }

    private void LoadPage_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (LoadPage.Visibility == Visibility.Visible)
        {
            SaveNameTextBox.Visibility = Visibility.Collapsed;
            NewGameButton.Visibility = Visibility.Collapsed;
            LoadGameButton.Visibility = Visibility.Collapsed;
            QuitButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            SaveNameTextBox.Visibility = Visibility.Visible;
            NewGameButton.Visibility = Visibility.Visible;
            LoadGameButton.Visibility = Visibility.Visible;
            QuitButton.Visibility = Visibility.Visible;
        }
    }
    
    private void LoadGamePage_OnLoadGameEvent(object sender, string e)
    {
        LoadPage.Visibility = Visibility.Collapsed;
        GamePage.Load((int)ActualWidth, (int)ActualHeight, e);
    }
}
