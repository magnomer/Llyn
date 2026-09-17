namespace Convention.Tests;

internal static class TAuditStrictSetting
{
    public const int TAuditGeneration = 8;
    public const bool TAuditStrictEnforced = false;
    public const string TAuditStrictReport = "temp/audit/Truth-{0}.md";

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
