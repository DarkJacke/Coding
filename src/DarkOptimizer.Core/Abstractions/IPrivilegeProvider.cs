namespace DarkOptimizer.Core.Abstractions;

public interface IPrivilegeProvider
{
    bool IsElevated { get; }
    bool HasProfileSingleProcessPrivilege { get; }
}
