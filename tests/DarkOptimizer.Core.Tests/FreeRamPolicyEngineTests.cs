using DarkOptimizer.Core.Abstractions;
using DarkOptimizer.Core.FreeRam;

namespace DarkOptimizer.Core.Tests;

public sealed class FreeRamPolicyEngineTests
{
    [Fact]
    public async Task ExecuteBestStrategyAsync_NonElevated_OnlyRunsWorkingSetTrim()
    {
        var service = new RecordingFreeRamService();
        var engine = new FreeRamPolicyEngine(service, new FixedPrivilegeProvider(isElevated: false, hasProfilePrivilege: false));

        var result = await engine.ExecuteBestStrategyAsync(CancellationToken.None);

        Assert.Single(result);
        Assert.Contains("ReduceProcessWorkingSets", service.Calls);
        Assert.DoesNotContain("PurgeStandbyList", service.Calls);
    }

    [Fact]
    public async Task ExecuteBestStrategyAsync_ElevatedWithPrivilege_RunsFullSequence()
    {
        var service = new RecordingFreeRamService();
        var engine = new FreeRamPolicyEngine(service, new FixedPrivilegeProvider(isElevated: true, hasProfilePrivilege: true));

        var result = await engine.ExecuteBestStrategyAsync(CancellationToken.None);

        Assert.Equal(5, result.Count);
        Assert.Equal(
            ["ReduceProcessWorkingSets", "EmptyWorkingSets", "PurgeStandbyList", "PurgeModifiedPageList", "PurgeLowPriorityStandbyList"],
            service.Calls);
    }

    private sealed class FixedPrivilegeProvider(bool isElevated, bool hasProfilePrivilege) : IPrivilegeProvider
    {
        public bool IsElevated { get; } = isElevated;
        public bool HasProfileSingleProcessPrivilege { get; } = hasProfilePrivilege;
    }

    private sealed class RecordingFreeRamService : IFreeRamService
    {
        public List<string> Calls { get; } = [];

        public ValueTask<MemorySnapshot> GetMemorySnapshotAsync(CancellationToken cancellationToken)
            => ValueTask.FromResult(default(MemorySnapshot));

        public ValueTask<FreeRamRunResult> ReduceProcessWorkingSetsAsync(CancellationToken cancellationToken)
            => Record("ReduceProcessWorkingSets");

        public ValueTask<FreeRamRunResult> EmptyWorkingSetsAsync(CancellationToken cancellationToken)
            => Record("EmptyWorkingSets");

        public ValueTask<FreeRamRunResult> PurgeStandbyListAsync(CancellationToken cancellationToken)
            => Record("PurgeStandbyList");

        public ValueTask<FreeRamRunResult> PurgeModifiedPageListAsync(CancellationToken cancellationToken)
            => Record("PurgeModifiedPageList");

        public ValueTask<FreeRamRunResult> PurgeLowPriorityStandbyListAsync(CancellationToken cancellationToken)
            => Record("PurgeLowPriorityStandbyList");

        public ValueTask<FreeRamRunResult> CombinedAggressiveTrimAsync(CancellationToken cancellationToken)
            => Record("CombinedAggressiveTrim");

        private ValueTask<FreeRamRunResult> Record(string call)
        {
            Calls.Add(call);
            return ValueTask.FromResult(new FreeRamRunResult(call, true, 0, TimeSpan.Zero, "ok"));
        }
    }
}
