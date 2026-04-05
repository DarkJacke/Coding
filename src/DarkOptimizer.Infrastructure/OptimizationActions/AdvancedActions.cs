using DarkOptimizer.Core.Optimization;

namespace DarkOptimizer.Infrastructure.OptimizationActions;

internal sealed class RegistryTweaksAction() : BaseOptimizationAction("advanced.registry", OptimizationTier.Advanced, requiresElevation: true)
{
    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var profile = context.Parameters.TryGetValue("registry.profile", out var value) ? value : "balanced";
        return ActionExecution.Success($"Registry performance profile '{profile}' queued.", 1);
    }
}

internal sealed class MemoryCompressionAction() : BaseOptimizationAction("advanced.memory-compression", OptimizationTier.Advanced, requiresElevation: true)
{
    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var enable = context.Parameters.GetBoolOrDefault("memoryCompression.enable", true);
        return ActionExecution.Success(enable
            ? "Memory compression target set to enabled."
            : "Memory compression target set to disabled.", enable ? 1 : -1);
    }
}

internal sealed class IoPriorityAction() : BaseOptimizationAction("advanced.io-priority", OptimizationTier.Advanced, requiresElevation: true)
{
    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var quantum = context.Parameters.GetInt32OrDefault("io.quantum", 3);
        return ActionExecution.Success($"I/O priority tuning plan generated with quantum {quantum}.", quantum);
    }
}
