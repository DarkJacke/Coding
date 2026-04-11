using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DarkOptimizer.Core.FreeRam;
using DarkOptimizer.Infrastructure.Memory;
using System.Collections.ObjectModel;

namespace DarkOptimizer.App.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly IFreeRamService _freeRamService = new FreeRamService();
    private readonly FreeRamPolicyEngine _freeRamPolicyEngine;

    public ShellViewModel()
    {
        _freeRamPolicyEngine = new FreeRamPolicyEngine(_freeRamService, new ProcessPrivilegeProvider());
    }

    [ObservableProperty]
    private string _title = "Dark optimizer 2026";

    [ObservableProperty]
    private bool _isDashboardLoading = true;

    [ObservableProperty]
    private bool _isAdvancedLoaded;

    [ObservableProperty]
    private bool _isFreeRamBusy;

    [ObservableProperty]
    private string _memorySummary = "RAM optimizer idle";

    [ObservableProperty]
    private string _freeRamLastResult = "No execution yet";

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
        await Task.Delay(300);
        await RefreshMemoryAsync();
        IsDashboardLoading = false;
        IsAdvancedLoaded = true;
    }

    [RelayCommand]
    private async Task RefreshMemoryAsync()
    {
        try
        {
            var snapshot = await _freeRamService.GetMemorySnapshotAsync(CancellationToken.None);
            SetMemorySummary(snapshot);
        }
        catch (MemorySnapshotUnavailableException ex)
        {
            MemorySummary = "No se pudo leer la RAM del sistema";
            FreeRamLastResult = ex.Message;
        }
    }

    [RelayCommand]
    private async Task RunFreeRamAsync()
    {
        IsFreeRamBusy = true;
        try
        {
            var runs = await _freeRamPolicyEngine.ExecuteBestStrategyAsync(CancellationToken.None);
            var summary = string.Join(" | ", runs.Select(static x =>
                x.Succeeded
                    ? $"{x.Strategy}:OK"
                    : $"{x.Strategy}:FAIL ({x.Message})"));

            FreeRamLastResult = string.IsNullOrWhiteSpace(summary)
                ? "No se pudo ejecutar ninguna estrategia de liberación."
                : summary;

            await RefreshMemoryAsync();
        }
        catch (MemorySnapshotUnavailableException ex)
        {
            FreeRamLastResult = $"Error de snapshot: {ex.Message}";
            MemorySummary = "No se pudo actualizar la memoria por un error de snapshot";
        }
        finally
        {
            IsFreeRamBusy = false;
        }
    }

    [RelayCommand]
    private void SetMemorySummary(MemorySnapshot snapshot)
    {
        MemorySummary = $"Used {snapshot.UsedPercent:F1}% • Avail {(snapshot.AvailablePhysicalBytes / 1024d / 1024d):F0} MB";
    }
}
