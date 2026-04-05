using System.Diagnostics;

namespace DarkOptimizer.Core.Optimization;

public sealed class OptimizationService(IOptimizationRegistry registry)
{
    private readonly IOptimizationRegistry _registry = registry;

    public async ValueTask<OptimizationResult> RunTierAsync(
        OptimizationTier tier,
        bool isElevated,
        IReadOnlyDictionary<string, string>? parameters,
        CancellationToken cancellationToken)
    {
        var tierStopwatch = Stopwatch.StartNew();
        var context = new OptimizationActionContext(
            IsElevated: isElevated,
            StartedAtUtc: DateTimeOffset.UtcNow,
            Parameters: parameters ?? EmptyParameters.Instance);

        var actions = _registry.GetActions(tier);
        var results = new ActionResult[actions.Length];

        for (var i = 0; i < actions.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var action = actions[i];

            if (action.RequiresElevation && !context.IsElevated)
            {
                results[i] = new ActionResult(
                    action.Id,
                    Succeeded: false,
                    OptimizationErrorCode.AccessDenied,
                    "Operation requires elevation.",
                    TimeSpan.Zero,
                    0);
                continue;
            }

            results[i] = await action.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);
        }

        tierStopwatch.Stop();
        return new OptimizationResult
        {
            Tier = tier,
            Actions = results,
            TotalDuration = tierStopwatch.Elapsed
        };
    }

    private sealed class EmptyParameters : Dictionary<string, string>
    {
        public static readonly EmptyParameters Instance = [];
    }
}
