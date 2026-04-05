namespace DarkOptimizer.Core.Optimization;

public sealed class OptimizationResult
{
    public required OptimizationTier Tier { get; init; }
    public required IReadOnlyList<ActionResult> Actions { get; init; }
    public required TimeSpan TotalDuration { get; init; }

    public bool Succeeded => Actions.All(static x => x.Succeeded);
}
