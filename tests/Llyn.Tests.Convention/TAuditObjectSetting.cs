namespace Convention.Tests;

internal static class TAuditObjectSetting
{
    public const int TAuditGeneration = 15;
    public const bool TAuditObjectEnforced = true;
    public const string TAuditObjectReport = "temp/audit/Object-{0}.md";
    public const int TAuditPartLimit = 5;
    public const int TAuditSpanLimit = 1000;
    public const int TAuditHubReach = 5;
    public const double TAuditWeaveLimit = 0.75;
    public const double TAuditDensityLimit = 0.5;
    public const int TAuditLargeLines = 500;
    public const int TAuditLargeMembers = 40;
    public const int TAuditLargeState = 12;

    public static readonly IReadOnlyDictionary<string, int> TAuditObjectCeiling = new Dictionary<string, int>
    {
        ["Monolith"] = 6,
        ["Hub"] = 9,
        ["Large"] = 47,
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditPartCeiling = new Dictionary<string, int>
    {
        ["Llyn.Infrastructure.LEntryArchive"] = 2,
        ["Llyn.Infrastructure.LLanguageLoader"] = 10,
        ["Llyn.Infrastructure.LSituationArchive"] = 3,
        ["Llyn.ShellEngine.LTenure"] = 4,
        ["Llyn.UIDeportment.PArticulation"] = 3,
        ["Llyn.UIDeportment.PCard"] = 9,
        ["Llyn.UIDeportment.PCorpus"] = 9,
        ["Llyn.UIDeportment.PEditor"] = 29,
        ["Llyn.UIDeportment.PFavorite"] = 2,
        ["Llyn.UIDeportment.PField"] = 2,
        ["Llyn.UIDeportment.PRepertoire"] = 7,
        ["Llyn.UIDeportment.PScreen"] = 5,
        ["Llyn.UIDeportment.PSentence"] = 3,
        ["Llyn.UIDeportment.PSettings"] = 10,
        ["Llyn.UIDeportment.PSwath"] = 2,
        ["Llyn.UIDeportment.PTaxonomy"] = 3,
        ["Llyn.UIDeportment.PTenor"] = 3,
        ["Llyn.UIDeportment.PWindow"] = 11,
    };

    public static readonly string[] TAuditObjectInclude =
    [
        "src/*.cs",
    ];
}
