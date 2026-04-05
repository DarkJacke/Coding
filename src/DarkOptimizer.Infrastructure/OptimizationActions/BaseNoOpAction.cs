using DarkOptimizer.Core.Optimization;
using System.Diagnostics;

namespace DarkOptimizer.Infrastructure.OptimizationActions;

internal abstract class BaseNoOpAction : IOptimizationAction
{
    protected BaseNoOpAction(string id, OptimizationTier tier, bool requiresElevation = false)
    {
        Id = id;
        Tier = tier;
        RequiresElevation = requiresElevation;
    }

    public string Id { get; }
    public OptimizationTier Tier { get; }
    public bool RequiresElevation { get; }

    public ValueTask<ActionResult> ExecuteAsync(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var sw = Stopwatch.StartNew();
        var delta = ExecuteCore(context);
        sw.Stop();

        return ValueTask.FromResult(new ActionResult(
            Id,
            Succeeded: true,
            OptimizationErrorCode.None,
            "Completed",
            sw.Elapsed,
            delta));
    }

    protected abstract long ExecuteCore(OptimizationActionContext context);
}
