using DarkOptimizer.Core.Optimization;
using System.Diagnostics;

namespace DarkOptimizer.Infrastructure.OptimizationActions;

internal abstract class BaseOptimizationAction : IOptimizationAction
{
    protected BaseOptimizationAction(string id, OptimizationTier tier, bool requiresElevation = false)
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
        var execution = ExecuteCore(context, cancellationToken);
        sw.Stop();

        return ValueTask.FromResult(new ActionResult(
            Id,
            execution.Succeeded,
            execution.ErrorCode,
            execution.Message,
            sw.Elapsed,
            execution.DeltaValue));
    }

    protected abstract ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken);

    protected readonly record struct ActionExecution(bool Succeeded, OptimizationErrorCode ErrorCode, string Message, long DeltaValue)
    {
        public static ActionExecution Success(string message, long deltaValue)
            => new(true, OptimizationErrorCode.None, message, deltaValue);

        public static ActionExecution Failure(OptimizationErrorCode errorCode, string message)
            => new(false, errorCode, message, 0);
    }
}
