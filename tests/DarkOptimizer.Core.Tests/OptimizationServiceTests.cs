using DarkOptimizer.Core.Optimization;

namespace DarkOptimizer.Core.Tests;

public sealed class OptimizationServiceTests
{
    [Fact]
    public async Task RunTierAsync_ReturnsAccessDeniedWhenNotElevated()
    {
        var service = new OptimizationService(new TestRegistry(new TestAction("a", OptimizationTier.Simple, true)));
        var result = await service.RunTierAsync(OptimizationTier.Simple, isElevated: false, parameters: null, CancellationToken.None);
        Assert.False(result.Succeeded);
        Assert.Equal(OptimizationErrorCode.AccessDenied, result.Actions[0].ErrorCode);
    }

    private sealed class TestRegistry(params IOptimizationAction[] actions) : IOptimizationRegistry
    {
        private readonly IOptimizationAction[] _actions = actions;
        public ReadOnlySpan<IOptimizationAction> GetActions(OptimizationTier tier) => _actions;
    }

    private sealed class TestAction(string id, OptimizationTier tier, bool requiresElevation) : IOptimizationAction
    {
        public string Id { get; } = id;
        public OptimizationTier Tier { get; } = tier;
        public bool RequiresElevation { get; } = requiresElevation;

        public ValueTask<ActionResult> ExecuteAsync(OptimizationActionContext context, CancellationToken cancellationToken)
            => ValueTask.FromResult(new ActionResult(Id, true, OptimizationErrorCode.None, "ok", TimeSpan.Zero, 0));
    }
}
