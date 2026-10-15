namespace Convention.Tests;

internal static class TAuditFaultSetting
{
    public const int TAuditGeneration = 26;

    public static readonly string[] TAuditFaultRing =
    [
        "Llyn.Application",
        "Llyn.ShellEngine",
    ];

    public static readonly IReadOnlyDictionary<string, int> TAuditFaultCeiling = new Dictionary<string, int>
    {
        ["Llyn.Application"] = 0,
        ["Llyn.ShellEngine"] = 0,
    };

    public static readonly string[] TAuditFaultExempt = [];
}
