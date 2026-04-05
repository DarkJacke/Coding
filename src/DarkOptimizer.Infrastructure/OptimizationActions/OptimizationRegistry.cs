using DarkOptimizer.Core.Optimization;

namespace DarkOptimizer.Infrastructure.OptimizationActions;

public sealed class OptimizationRegistry : IOptimizationRegistry
{
    private static readonly IOptimizationAction[] Simple =
    [
        new TempCleanupAction(),
        new VisualEffectsAction(),
        new StartupAppsAction()
    ];

    private static readonly IOptimizationAction[] Intermediate =
    [
        new ServiceOrchestrationAction(),
        new ScheduledMaintenanceAction(),
        new TelemetryDisableAction()
    ];

    private static readonly IOptimizationAction[] Advanced =
    [
        new RegistryTweaksAction(),
        new MemoryCompressionAction(),
        new IoPriorityAction()
    ];

    private static readonly IOptimizationAction[] Supervised =
    [
        new DebloatSystemAppsAction(),
        new BcdTuneAction(),
        new KernelPowerSchemeAction()
    ];

    public ReadOnlySpan<IOptimizationAction> GetActions(OptimizationTier tier) => tier switch
    {
        OptimizationTier.Simple => Simple,
        OptimizationTier.Intermediate => Intermediate,
        OptimizationTier.Advanced => Advanced,
        OptimizationTier.Supervised => Supervised,
        _ => []
    };
}
