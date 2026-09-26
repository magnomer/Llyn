namespace Convention.Tests;

internal static class TAuditChainSetting
{
    public const int TAuditGeneration = 12;
    public const string TAuditChainHost = "Llyn.Host";

    public static readonly IReadOnlyDictionary<string, string[]> TAuditChainReach = new Dictionary<string, string[]>
    {
        ["Llyn.Core"] = [],
        ["Llyn.Application"] = ["Llyn.Core"],
        ["Llyn.ShellEngine"] = ["Llyn.Application"],
        ["Llyn.Conduct"] = ["Llyn.ShellEngine"],
        ["Llyn.UIDeportment"] = ["Llyn.Conduct"],
        ["Llyn.UIVeneer"] = ["Llyn.UIDeportment"],
        ["Llyn.UIDemeanor"] = ["Llyn.Conduct"],
        ["Llyn.UITerminal"] = ["Llyn.UIDemeanor"],
        ["Llyn.Infrastructure"] = ["Llyn.Core"],
        ["Llyn.Core.Windows"] = ["Llyn.Core"],
    };

    public static readonly string[] TAuditChainCut =
    [
        "Llyn.UIVeneer",
        "Llyn.UIDeportment",
        "Llyn.UIDemeanor",
        "Llyn.UITerminal",
    ];

    public static readonly IReadOnlyDictionary<string, string[]> TAuditChainSurface = new Dictionary<string, string[]>
    {
        ["Llyn.Conduct>Llyn.ShellEngine"] =
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
        ["Llyn.UIDeportment>Llyn.Conduct"] =
        [
            "LDisplay",
            "LDisplaySound",
            "LDisplayStamp",
        ],
        ["Llyn.UIDemeanor>Llyn.Conduct"] =
        [
            "LDisplay",
            "LDisplaySound",
            "LDisplayStamp",
        ],
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditChainFloor = new Dictionary<string, int>
    {
        ["Llyn.Application"] = 91,
    };

    public static readonly IReadOnlyDictionary<string, string[]> TAuditChainStray = new Dictionary<string, string[]>
    {
        ["Llyn.Core"] = [@"^LRequest"],
    };

    public static readonly IReadOnlyDictionary<string, string[]> TAuditChainBanned = new Dictionary<string, string[]>
    {
        ["src/Llyn.ShellEngine"] = ["LVaultSessionStart"],
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditChainCeiling = new Dictionary<string, int>
    {
        ["cross:Llyn.UIDeportment>Llyn.Application"] = 44,
        ["cross:Llyn.UIDeportment>Llyn.Core"] = 754,
        ["cross:Llyn.UIDeportment>Llyn.ShellEngine"] = 659,
        ["cross:Llyn.UIVeneer>Llyn.Application"] = 128,
        ["cross:Llyn.UIVeneer>Llyn.Core"] = 1330,
        ["cross:Llyn.UIVeneer>Llyn.ShellEngine"] = 14,
        ["expose:Llyn.UIDemeanor>Llyn.Core"] = 34,
        ["expose:Llyn.UIDemeanor>Llyn.ShellEngine"] = 9,
        ["expose:Llyn.UIDeportment>Llyn.Core"] = 34,
        ["expose:Llyn.UIDeportment>Llyn.ShellEngine"] = 9,
        ["reach:Llyn.Conduct>Llyn.Core"] = 1,
        ["reach:Llyn.ShellEngine>Llyn.Core"] = 15,
    };

    public static readonly string[] TAuditChainWaiver = [];
}
