namespace Convention.Tests;

internal static class TAuditObjectSetting
{
    public const int TAuditGeneration = 18;
    public const bool TAuditObjectEnforced = true;
    public const string TAuditObjectReport = "temp/audit/Object-{0}.md";

    public static readonly IReadOnlyDictionary<string, double> TAuditHydraLimit = new Dictionary<string, double>
    {
        ["Parts"] = 5,
        ["Lines"] = 1000,
        ["Fused"] = 0.75,
        ["Density"] = 0.5,
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditSpiderLimit = new Dictionary<string, int>
    {
        ["Outgoing"] = 25,
        ["Incoming"] = 25,
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditChameleonLimit = new Dictionary<string, int>
    {
        ["Mutable"] = 7,
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditOctopusLimit = new Dictionary<string, int>
    {
        ["Outgoing"] = 40,
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditCentipedeLimit = new Dictionary<string, int>
    {
        ["Members"] = 40,
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditSerpentLimit = new Dictionary<string, int>
    {
        ["Lines"] = 500,
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditHubLimit = new Dictionary<string, int>
    {
        ["Parts"] = 5,
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditObjectCeiling = new Dictionary<string, int>
    {
        ["Hydra"] = 2,
        ["Kraken"] = 8,
        ["Spider"] = 6,
        ["Chameleon"] = 11,
        ["Octopus"] = 14,
        ["Centipede"] = 37,
        ["Serpent"] = 8,
        ["Hub"] = 3,
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditPartsCeiling = new Dictionary<string, int>
    {
        ["Llyn.Infrastructure.LEntryArchive"] = 2,
        ["Llyn.Infrastructure.LLanguageLoader"] = 10,
        ["Llyn.Infrastructure.LSituationArchive"] = 3,
        ["Llyn.ShellEngine.LTenure"] = 4,
        ["Llyn.UIDeportment.QArticulation"] = 3,
        ["Llyn.UIDeportment.PCard"] = 9,
        ["Llyn.UIDeportment.PSentence"] = 3,
        ["Llyn.UIDeportment.PSwath"] = 2,
        ["Llyn.UIDeportment.QWindow"] = 7,
        ["Llyn.UIDeportment.QCorpus"] = 9,
        ["Llyn.UIDeportment.QFavorite"] = 2,
        ["Llyn.UIDeportment.QRepertoire"] = 7,
        ["Llyn.UIDeportment.QTaxonomy"] = 3,
        ["Llyn.UIDeportment.QTenor"] = 3,
    };

    public static readonly string[] TAuditObjectInclude =
    [
        "src/*.cs",
    ];
}
