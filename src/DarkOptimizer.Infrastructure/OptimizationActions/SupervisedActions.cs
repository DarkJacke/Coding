using DarkOptimizer.Core.Optimization;

namespace DarkOptimizer.Infrastructure.OptimizationActions;

internal sealed class DebloatSystemAppsAction() : BaseOptimizationAction("supervised.debloat", OptimizationTier.Supervised, requiresElevation: true)
{
    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var denyListCount = context.Parameters.GetInt32OrDefault("debloat.packages", 12);
        return ActionExecution.Success($"Debloat package plan generated for {denyListCount} app packages.", denyListCount);
    }
}

internal sealed class BcdTuneAction() : BaseOptimizationAction("supervised.bcdedit", OptimizationTier.Supervised, requiresElevation: true)
{
    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var usePlatformClock = context.Parameters.GetBoolOrDefault("bcd.useplatformclock", false);
        return ActionExecution.Success(
            usePlatformClock
                ? "BCD tune profile prepared with platform clock enabled."
                : "BCD tune profile prepared with default clock strategy.",
            usePlatformClock ? 1 : 0);
    }
}

internal sealed class KernelPowerSchemeAction() : BaseOptimizationAction("supervised.kernel-power", OptimizationTier.Supervised, requiresElevation: true)
{
    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var index = context.Parameters.GetInt32OrDefault("power.schemeIndex", 2);
        return ActionExecution.Success($"Kernel-level power scheme profile index {index} selected.", index);
    }
}
