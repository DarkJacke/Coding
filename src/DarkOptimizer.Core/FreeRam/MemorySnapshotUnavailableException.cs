namespace DarkOptimizer.Core.FreeRam;

public sealed class MemorySnapshotUnavailableException(string message, Exception? innerException = null)
    : Exception(message, innerException);
