namespace Convention.Tests;

internal static class TAuditChainSetting
{
    public const int TAuditGeneration = 10;

    public static readonly IReadOnlyDictionary<string, string[]> TAuditChainReach = new Dictionary<string, string[]>
    {
        ["Llyn.Core"] = [],
        ["Llyn.Application"] = ["Llyn.Core"],
        ["Llyn.ShellEngine"] = ["Llyn.Application"],
        ["Llyn.UIDeportment"] = ["Llyn.ShellEngine"],
        ["Llyn.UIVeneer"] = ["Llyn.UIDeportment"],
        ["Llyn.Infrastructure"] = ["Llyn.Core"],
        ["Llyn.Media"] = ["Llyn.Core"],
    };

    public static readonly string[] TAuditChainRoot =
    [
        "src/Llyn.UIVeneer/App.xaml.cs",
        "src/Llyn.Infrastructure/LRigFactory.cs",
    ];

    public static readonly IReadOnlyDictionary<string, string[]> TAuditChainSurface = new Dictionary<string, string[]>
    {
        ["Llyn.UIDeportment>Llyn.ShellEngine"] =
        [
            "LDraftPort",
            "LEntryPort",
            "LPhonologyPort",
            "LSettingsPort",
            "LMediaPort",
            "LPortraitPort",
            "LTenure",
            "LVista",
            "LForay",
            "LPosture",
        ],
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditChainFloor = new Dictionary<string, int>
    {
        ["Llyn.Application"] = 58,
    };

    public static readonly IReadOnlyDictionary<string, string[]> TAuditChainStray = new Dictionary<string, string[]>
    {
        ["Llyn.Core"] = [@"^LRequest"],
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditChainCeiling = new Dictionary<string, int>
    {
        ["reach:Llyn.ShellEngine>Llyn.Core"] = 54,
    };

    public static readonly string[] TAuditChainWaiver = [];
}
