namespace Convention.Tests;

internal static class TAuditTruthSetting
{
    public const int TAuditGeneration = 8;
    public const bool TAuditTruthEnforced = false;
    public const string TAuditTruthReport = "temp/audit/Custody-{0}.md";
    public const string TAuditFieldPrefix = "_p";

    public static readonly string[] TAuditTruthInclude =
    [
        "src/Llyn.UIShell/*.cs",
    ];

    public static readonly string[] TAuditTruthHandles =
    [
        "LEngine",
        "LTenure",
        "LVista",
        "PObserver",
    ];

    public static readonly string[] TAuditTruthWaiver = [];
}
