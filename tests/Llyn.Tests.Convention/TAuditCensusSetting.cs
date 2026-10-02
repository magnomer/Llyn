namespace Convention.Tests;

internal static class TAuditCensusSetting
{
    public const int TAuditGeneration = 19;

    public static readonly IReadOnlyDictionary<string, int> TAuditHollowingFloor = new Dictionary<string, int>
    {
        ["Llyn.Application"] = 91,
    };

    public static readonly IReadOnlyDictionary<string, string[]> TAuditSquattingPattern =
        new Dictionary<string, string[]>
        {
            ["Llyn.Core"] = [@"^LRequest"],
        };

    public static readonly IReadOnlyDictionary<string, string[]> TAuditSmugglingWord =
        new Dictionary<string, string[]>
        {
            ["src/Llyn.ShellEngine"] = ["LVaultSessionStart"],
        };
}
