using DarkOptimizer.Core.Optimization;

namespace DarkOptimizer.Infrastructure.OptimizationActions;

internal sealed class ServiceOrchestrationAction() : BaseNoOpAction("intermediate.services", OptimizationTier.Intermediate, requiresElevation: true)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 3;
}

internal sealed class ScheduledMaintenanceAction() : BaseNoOpAction("intermediate.maintenance", OptimizationTier.Intermediate, requiresElevation: true)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 4;
}

internal sealed class TelemetryDisableAction() : BaseNoOpAction("intermediate.telemetry", OptimizationTier.Intermediate, requiresElevation: true)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 5;
}
