using DarkOptimizer.Core.Abstractions;

namespace DarkOptimizer.Core.FreeRam;

public sealed class FreeRamPolicyEngine(IFreeRamService freeRamService, IPrivilegeProvider privilegeProvider)
{
    private readonly IFreeRamService _freeRamService = freeRamService;
    private readonly IPrivilegeProvider _privilegeProvider = privilegeProvider;

    public async ValueTask<IReadOnlyList<FreeRamRunResult>> ExecuteBestStrategyAsync(CancellationToken cancellationToken)
    {
        var results = new List<FreeRamRunResult>(capacity: 3);
        results.Add(await _freeRamService.ReduceProcessWorkingSetsAsync(cancellationToken).ConfigureAwait(false));

        if (_privilegeProvider.IsElevated && _privilegeProvider.HasProfileSingleProcessPrivilege)
        {
            results.Add(await _freeRamService.PurgeStandbyListAsync(cancellationToken).ConfigureAwait(false));
            results.Add(await _freeRamService.CombinedAggressiveTrimAsync(cancellationToken).ConfigureAwait(false));
        }

        return results;
    }
}
