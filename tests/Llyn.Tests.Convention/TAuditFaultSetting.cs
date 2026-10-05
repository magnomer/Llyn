namespace Convention.Tests;

internal static class TAuditFaultSetting
{
    public const int TAuditGeneration = 21;

    public static readonly string[] TAuditFaultRing =
    [
        "Llyn.Application",
        "Llyn.ShellEngine",
    ];

    public static readonly IReadOnlyDictionary<string, int> TAuditFaultCeiling = new Dictionary<string, int>
    {
        ["Llyn.Application"] = 10,
        ["Llyn.ShellEngine"] = 19,
    };

    public static readonly string[] TAuditFaultExempt =
    [
        "src/Llyn.Application/Workspace/LWorkspaceClerk.cs:LWorkspacePostureRead",
        "src/Llyn.Application/Workspace/LWorkspaceClerk.cs:LWorkspaceFallbackSave",
    ];
}
