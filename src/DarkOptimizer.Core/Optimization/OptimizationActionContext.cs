namespace DarkOptimizer.Core.Optimization;

public readonly record struct OptimizationActionContext(
    bool IsElevated,
    DateTimeOffset StartedAtUtc,
    IReadOnlyDictionary<string, string> Parameters);
