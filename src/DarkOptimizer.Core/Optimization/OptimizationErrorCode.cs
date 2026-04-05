namespace DarkOptimizer.Core.Optimization;

public enum OptimizationErrorCode : byte
{
    None = 0,
    Cancelled = 1,
    AccessDenied = 2,
    Unsupported = 3,
    Failed = 4
}
