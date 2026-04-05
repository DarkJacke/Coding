using DarkOptimizer.Core.Optimization;

namespace DarkOptimizer.Infrastructure.OptimizationActions;

internal sealed class TempCleanupAction() : BaseNoOpAction("simple.temp-cleanup", OptimizationTier.Simple)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 128L * 1024L * 1024L;
}

internal sealed class VisualEffectsAction() : BaseNoOpAction("simple.visual-effects", OptimizationTier.Simple)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 1;
}

internal sealed class StartupAppsAction() : BaseNoOpAction("simple.startup-apps", OptimizationTier.Simple)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 2;
}
