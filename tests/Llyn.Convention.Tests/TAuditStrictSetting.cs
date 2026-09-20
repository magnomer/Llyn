namespace Convention.Tests;

internal static class TAuditStrictSetting
{
    public const int TAuditGeneration = 9;
    public const bool TAuditStrictEnforced = true;
    public const string TAuditStrictReport = "temp/audit/Truth-{0}.md";

    public static readonly IReadOnlyDictionary<string, int> TAuditStrictCeiling = new Dictionary<string, int>
    {
        ["Storage"] = 73,
        ["Flow"] = 634,
        ["Treat"] = 79,
        ["Reach"] = 64,
        ["Taint"] = 195,
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
        @"\bSystem\.Windows\b",
        @"\bDispatcher\b",
        @"\bSystem\.IO\b",
    ];

    public static readonly string[] TAuditReachNamespaces =
    [
        "Llyn.Core",
        "Llyn.ShellEngine",
        "Llyn.Infrastructure",
    ];


    public static readonly string[] TAuditVeneerBases =
    [
        "Adorner",
        "Button",
        "ContentControl",
        "Control",
        "Decorator",
        "FrameworkElement",
        "ItemsControl",
        "Panel",
        "UserControl",
        "Window",
    ];

    public static readonly string[] TAuditTreatVerbs =
    [
        "Aggregate",
        "All",
        "Any",
        "Average",
        "Concat",
        "Contains",
        "Count",
        "Distinct",
        "DistinctBy",
        "Except",
        "First",
        "FirstOrDefault",
        "GroupBy",
        "GroupJoin",
        "Intersect",
        "Join",
        "Last",
        "LastOrDefault",
        "Max",
        "MaxBy",
        "Min",
        "MinBy",
        "OrderBy",
        "OrderByDescending",
        "Reverse",
        "Select",
        "SelectMany",
        "Single",
        "SingleOrDefault",
        "Skip",
        "SkipWhile",
        "Sum",
        "Take",
        "TakeWhile",
        "ThenBy",
        "ThenByDescending",
        "ToDictionary",
        "ToHashSet",
        "ToLookup",
        "Union",
        "Where",
        "Zip",
    ];
}
