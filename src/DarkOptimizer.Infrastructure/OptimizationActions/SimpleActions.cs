using DarkOptimizer.Core.Optimization;
using Microsoft.Win32;

namespace DarkOptimizer.Infrastructure.OptimizationActions;

internal sealed class TempCleanupAction() : BaseOptimizationAction("simple.temp-cleanup", OptimizationTier.Simple)
{
    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        var tempPath = Path.GetTempPath();
        if (!Directory.Exists(tempPath))
        {
            return ActionExecution.Success("Temp path not found.", 0);
        }

        var options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            ReturnSpecialDirectories = false
        };

        long reclaimed = 0;
        foreach (var file in Directory.EnumerateFiles(tempPath, "*", options))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var info = new FileInfo(file);
            var length = info.Exists ? info.Length : 0;

            try
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
                reclaimed += length;
            }
            catch (IOException)
            {
                // Ignore locked/transient files.
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore files not owned by current user context.
            }
        }

        return ActionExecution.Success($"Deleted temp files from {tempPath}.", reclaimed);
    }
}

internal sealed class VisualEffectsAction() : BaseOptimizationAction("simple.visual-effects", OptimizationTier.Simple)
{
    private const string ExplorerAdvancedPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";

    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var key = Registry.CurrentUser.OpenSubKey(ExplorerAdvancedPath, writable: true);
        if (key is null)
        {
            return ActionExecution.Failure(OptimizationErrorCode.Unsupported, "Explorer advanced registry path is unavailable.");
        }

        key.SetValue("TaskbarAnimations", 0, RegistryValueKind.DWord);
        key.SetValue("ListviewAlphaSelect", 0, RegistryValueKind.DWord);
        key.SetValue("ListviewShadow", 0, RegistryValueKind.DWord);

        return ActionExecution.Success("Disabled selected shell animation effects.", 3);
    }
}

internal sealed class StartupAppsAction() : BaseOptimizationAction("simple.startup-apps", OptimizationTier.Simple)
{
    private const string CurrentUserRunPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    protected override ActionExecution ExecuteCore(OptimizationActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var key = Registry.CurrentUser.OpenSubKey(CurrentUserRunPath, writable: false);
        if (key is null)
        {
            return ActionExecution.Success("Run key not found.", 0);
        }

        var values = key.GetValueNames();
        return ActionExecution.Success($"Detected {values.Length} current-user startup entries.", values.Length);
    }
}
