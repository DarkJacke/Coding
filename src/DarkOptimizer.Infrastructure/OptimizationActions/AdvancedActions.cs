using DarkOptimizer.Core.Optimization;

namespace DarkOptimizer.Infrastructure.OptimizationActions;

internal sealed class RegistryTweaksAction() : BaseNoOpAction("advanced.registry", OptimizationTier.Advanced, requiresElevation: true)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 6;
}

internal sealed class MemoryCompressionAction() : BaseNoOpAction("advanced.memory-compression", OptimizationTier.Advanced, requiresElevation: true)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 7;
}

internal sealed class IoPriorityAction() : BaseNoOpAction("advanced.io-priority", OptimizationTier.Advanced, requiresElevation: true)
{
    protected override long ExecuteCore(OptimizationActionContext context) => 8;
}
