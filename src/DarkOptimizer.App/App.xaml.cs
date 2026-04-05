using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Composition.SystemBackdrops;
using WinRT;

namespace DarkOptimizer.App;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        if (_window.SystemBackdrop is null)
        {
            _window.SystemBackdrop = new MicaBackdrop
            {
                Kind = MicaKind.BaseAlt
            };
        }

        _window.Activate();
    }
}
