using DarkOptimizer.Core.Optimization;

namespace DarkOptimizer.Infrastructure.OptimizationActions;

internal sealed class DebloatSystemAppsAction() : BaseNoOpAction("supervised.debloat", OptimizationTier.Supervised, requiresElevation: true)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 9;
}

internal sealed class BcdTuneAction() : BaseNoOpAction("supervised.bcdedit", OptimizationTier.Supervised, requiresElevation: true)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 10;
}

internal sealed class KernelPowerSchemeAction() : BaseNoOpAction("supervised.kernel-power", OptimizationTier.Supervised, requiresElevation: true)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 11;
}
