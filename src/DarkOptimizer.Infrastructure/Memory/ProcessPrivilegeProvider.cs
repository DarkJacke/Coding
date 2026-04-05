using DarkOptimizer.Core.Abstractions;
using System.Security.Principal;

namespace DarkOptimizer.Infrastructure.Memory;

public sealed class ProcessPrivilegeProvider : IPrivilegeProvider
{
    public bool IsElevated
    {
        get
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    public bool HasProfileSingleProcessPrivilege => IsElevated;
}
