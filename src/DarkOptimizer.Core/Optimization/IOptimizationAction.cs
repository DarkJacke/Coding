namespace DarkOptimizer.Core.Optimization;

public interface IOptimizationAction
{
    string Id { get; }
    OptimizationTier Tier { get; }
    bool RequiresElevation { get; }

    ValueTask<ActionResult> ExecuteAsync(OptimizationActionContext context, CancellationToken cancellationToken);
}
