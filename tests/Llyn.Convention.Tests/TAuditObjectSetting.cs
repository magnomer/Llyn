namespace Convention.Tests;

internal static class TAuditObjectSetting
{
    public const int TAuditGeneration = 14;
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
        ["Monolith"] = 3,
        ["Hub"] = 8,
        ["Large"] = 43,
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditPartCeiling = new Dictionary<string, int>
    {
        ["Llyn.Infrastructure.LEntryArchive"] = 2,
        ["Llyn.Infrastructure.LLanguageLoader"] = 10,
        ["Llyn.Infrastructure.LSituationArchive"] = 3,
        ["Llyn.ShellEngine.LTenure"] = 4,
        ["Llyn.UIVeneer.PArticulation"] = 3,
        ["Llyn.UIVeneer.PCard"] = 9,
        ["Llyn.UIVeneer.PCorpus"] = 9,
        ["Llyn.UIVeneer.PEditor"] = 29,
        ["Llyn.UIVeneer.PFavorite"] = 2,
        ["Llyn.UIVeneer.PField"] = 2,
        ["Llyn.UIVeneer.PRepertoire"] = 7,
        ["Llyn.UIVeneer.PScreen"] = 5,
        ["Llyn.UIVeneer.PSentence"] = 3,
        ["Llyn.UIVeneer.PSettings"] = 10,
        ["Llyn.UIVeneer.PSwath"] = 2,
        ["Llyn.UIVeneer.PTaxonomy"] = 3,
        ["Llyn.UIVeneer.PTenor"] = 3,
        ["Llyn.UIVeneer.PWindow"] = 11,
    };

    public static readonly string[] TAuditObjectInclude =
    [
        "src/*.cs",
    ];
}
