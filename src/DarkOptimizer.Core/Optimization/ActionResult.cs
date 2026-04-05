namespace DarkOptimizer.Core.Optimization;

public readonly record struct ActionResult(
    string ActionId,
    bool Succeeded,
    OptimizationErrorCode ErrorCode,
    string Message,
    TimeSpan Duration,
    long DeltaValue);
