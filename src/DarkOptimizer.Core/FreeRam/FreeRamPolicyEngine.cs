using DarkOptimizer.Core.Abstractions;

namespace DarkOptimizer.Core.FreeRam;

public sealed class FreeRamPolicyEngine(IFreeRamService freeRamService, IPrivilegeProvider privilegeProvider)
{
    private readonly IFreeRamService _freeRamService = freeRamService;
    private readonly IPrivilegeProvider _privilegeProvider = privilegeProvider;

    public async ValueTask<IReadOnlyList<FreeRamRunResult>> ExecuteBestStrategyAsync(CancellationToken cancellationToken)
    {
        var results = new List<FreeRamRunResult>(capacity: 5);

        var workingSet = await ExecuteStrategySafelyAsync(
            static (service, token) => service.ReduceProcessWorkingSetsAsync(token),
            "ReduceProcessWorkingSets",
            cancellationToken).ConfigureAwait(false);
        results.Add(workingSet);

        if (!workingSet.Succeeded)
        {
            return results;
        }

        if (!_privilegeProvider.IsElevated)
        {
            return results;
        }

        results.Add(await ExecuteStrategySafelyAsync(
            static (service, token) => service.EmptyWorkingSetsAsync(token),
            "EmptyWorkingSets",
            cancellationToken).ConfigureAwait(false));
        results.Add(await ExecuteStrategySafelyAsync(
            static (service, token) => service.PurgeStandbyListAsync(token),
            "PurgeStandbyList",
            cancellationToken).ConfigureAwait(false));

        if (_privilegeProvider.HasProfileSingleProcessPrivilege)
        {
            results.Add(await ExecuteStrategySafelyAsync(
                static (service, token) => service.PurgeModifiedPageListAsync(token),
                "PurgeModifiedPageList",
                cancellationToken).ConfigureAwait(false));
            results.Add(await ExecuteStrategySafelyAsync(
                static (service, token) => service.PurgeLowPriorityStandbyListAsync(token),
                "PurgeLowPriorityStandbyList",
                cancellationToken).ConfigureAwait(false));
        }

        return results;
    }

    private async ValueTask<FreeRamRunResult> ExecuteStrategySafelyAsync(
        Func<IFreeRamService, CancellationToken, ValueTask<FreeRamRunResult>> strategyAction,
        string strategy,
        CancellationToken cancellationToken)
    {
        try
        {
            return await strategyAction(_freeRamService, cancellationToken).ConfigureAwait(false);
        }
        catch (MemorySnapshotUnavailableException ex)
        {
            return new FreeRamRunResult(
                strategy,
                false,
                0,
                TimeSpan.Zero,
                $"No se pudo capturar el estado de memoria: {ex.Message}");
        }
    }
}
