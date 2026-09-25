namespace Convention.Tests;

internal static class TAuditStrictSetting
{
    public const int TAuditGeneration = 11;
    public const bool TAuditStrictEnforced = true;
    public const string TAuditStrictReport = "temp/audit/Strict-{0}.md";

    public static readonly IReadOnlyDictionary<string, int> TAuditStrictCeiling = new Dictionary<string, int>
    {
        ["Storage"] = 896,
        ["Call"] = 4241,
        ["Engine"] = 1999,
        ["Reach"] = 64,
        ["Trigger"] = 385,
    };

    public static readonly string[] TAuditReachInclude =
    [
        "src/Llyn.UIVeneer/*.xaml",
    ];

    public static readonly string[] TAuditVeneerInclude =
    [
        "src/Llyn.UIVeneer/*.cs",
    ];

    public static readonly string[] TAuditDeportmentInclude =
    [
        "src/Llyn.UIDeportment/*.cs",
    ];

    public const string TAuditDeportmentNamespace = "Llyn.UIDeportment";

    public static readonly string[] TAuditCatalogPatterns =
    [
        @"^\s*using\s+System\.IO\s*;",
        @"^\s*using\s+System\.Text\.Json",
        @"^\s*using\s+System\.Text\.RegularExpressions",
        @"^\s*using\s+System\.Diagnostics\s*;",
        @"\bJsonSerializer\b",
        @"\bJsonDocument\b",
        @"\bRegex\b",
        @"\bProcess\.Start\b",
        @"\bProcessStartInfo\b",
        @"\bTask\.Run\b",
        @"\bFile\.\w+\(",
        @"\bDirectory\.\w+\(",
        @"\bPath\.\w+\(",
    ];

    public static readonly string[] TAuditCatalogExempt =
    [
        "PIcon.cs",
        "PScreenBrowser.cs",
        "PScriptImage.cs",
    ];

    public static readonly string[] TAuditMarkupPatterns =
    [
        @"\bSystem\.IO\b",
    ];

    public static readonly string[] TAuditMarkupExempt = [];

    public static readonly string[] TAuditReachNamespaces =
    [
        "Llyn.Core",
        "Llyn.Application",
        "Llyn.ShellEngine",
        "Llyn.Infrastructure",
        "Llyn.Media",
    ];

    public static readonly string[] TAuditTriggerElements =
    [
        "DataTrigger",
        "MultiDataTrigger",
        "MultiTrigger",
        "Trigger",
    ];

    public static readonly string[] TAuditTriggerSlots =
    [
        "Converter",
        "FallbackValue",
        "StringFormat",
        "TargetNullValue",
    ];
}
