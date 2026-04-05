using DarkOptimizer.Core.Optimization;

namespace DarkOptimizer.Infrastructure.OptimizationActions;

internal sealed class ServiceOrchestrationAction() : BaseOptimizationAction("intermediate.services", OptimizationTier.Intermediate, requiresElevation: true)
{
    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var serviceBatchSize = context.Parameters.GetInt32OrDefault("services.batch", 6);
        return ActionExecution.Success($"Prepared service orchestration plan for {serviceBatchSize} services.", serviceBatchSize);
    }
}

internal sealed class ScheduledMaintenanceAction() : BaseOptimizationAction("intermediate.maintenance", OptimizationTier.Intermediate, requiresElevation: true)
{
    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var compactLogs = context.Parameters.GetBoolOrDefault("maintenance.compactLogs", true);
        var delta = compactLogs ? 1 : 0;
        return ActionExecution.Success(compactLogs
            ? "Scheduled maintenance includes log compaction and component cleanup."
            : "Scheduled maintenance includes component cleanup.", delta);
    }
}

internal sealed class TelemetryDisableAction() : BaseOptimizationAction("intermediate.telemetry", OptimizationTier.Intermediate, requiresElevation: true)
{
    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var disableFeedback = context.Parameters.GetBoolOrDefault("telemetry.feedback", true);
        return ActionExecution.Success(disableFeedback
            ? "Telemetry reduction profile prepared (diagnostics + feedback)."
            : "Telemetry reduction profile prepared (diagnostics only).", disableFeedback ? 2 : 1);
    }
}
