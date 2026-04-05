using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DarkOptimizer.Core.FreeRam;
using System.Collections.ObjectModel;

namespace DarkOptimizer.App.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Dark optimizer 2026";

    [ObservableProperty]
    private bool _isDashboardLoading = true;

    [ObservableProperty]
    private bool _isAdvancedLoaded;

    [ObservableProperty]
    private string _memorySummary = "RAM optimizer idle";

    public ObservableCollection<NavItemViewModel> SidebarItems { get; } =
    [
        new("\uE80F", "Dashboard", "dashboard"),
        new("\uE945", "Optimize", "optimize"),
        new("\uE9D9", "Free RAM", "free-ram"),
        new("\uE895", "Services", "services"),
        new("\uE8B8", "Registry", "registry"),
        new("\uE7BA", "Supervised", "supervised"),
        new("\uE713", "Settings", "settings")
    ];

    [RelayCommand]
    private async Task InitializeAsync()
    {
        await Task.Delay(450).ConfigureAwait(false);
        IsDashboardLoading = false;
        IsAdvancedLoaded = true;
    }

    [RelayCommand]
    private void SetMemorySummary(MemorySnapshot snapshot)
    {
        MemorySummary = $"Used {snapshot.UsedPercent:F1}% • Avail {(snapshot.AvailablePhysicalBytes / 1024d / 1024d):F0} MB";
    }
}
