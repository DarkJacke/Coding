using DarkOptimizer.App.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DarkOptimizer.App.Views;

public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel { get; } = new();

    public ShellPage()
    {
        InitializeComponent();
        _ = ViewModel.InitializeAsyncCommand.ExecuteAsync(null);
    }

    private void OnNavigationClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string route })
        {
            ViewModel.Title = $"Dark optimizer 2026 • {route}";
        }
    }
}
