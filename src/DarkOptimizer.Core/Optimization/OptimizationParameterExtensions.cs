namespace DarkOptimizer.Core.Optimization;

public static class OptimizationParameterExtensions
{
    public static int GetInt32OrDefault(this IReadOnlyDictionary<string, string> parameters, string key, int fallback)
        => parameters.TryGetValue(key, out var raw) && int.TryParse(raw, out var parsed)
            ? parsed
            : fallback;

    public static bool GetBoolOrDefault(this IReadOnlyDictionary<string, string> parameters, string key, bool fallback)
        => parameters.TryGetValue(key, out var raw) && bool.TryParse(raw, out var parsed)
            ? parsed
            : fallback;
}
