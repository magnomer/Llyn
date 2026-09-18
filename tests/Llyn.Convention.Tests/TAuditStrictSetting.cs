namespace Convention.Tests;

internal static class TAuditStrictSetting
{
    public const int TAuditGeneration = 9;
    public const bool TAuditStrictEnforced = true;
    public const string TAuditStrictReport = "temp/audit/Truth-{0}.md";

    public static readonly IReadOnlyDictionary<string, int> TAuditStrictCeiling = new Dictionary<string, int>
    {
        ["Storage"] = 128,
        ["Flow"] = 927,
        ["Treat"] = 405,
        ["Reach"] = 64,
        ["Taint"] = 411,
    };

    public static readonly string[] TAuditReachInclude =
    [
        "src/Llyn.UIVeneer/*.xaml",
        "src/Llyn.UIShell/*.xaml",
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
