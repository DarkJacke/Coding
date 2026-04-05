namespace DarkOptimizer.Core.Optimization;

public interface IOptimizationRegistry
{
    ReadOnlySpan<IOptimizationAction> GetActions(OptimizationTier tier);
}
