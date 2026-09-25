namespace Convention.Tests;

internal static class TAuditChainSetting
{
    public const int TAuditGeneration = 11;

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

    public static readonly string[] TAuditChainRoot =
    [
        "src/Llyn.Host/LHost.cs",
        "src/Llyn.Infrastructure/LRigFactory.cs",
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

    public static readonly IReadOnlyDictionary<string, int> TAuditChainCeiling = new Dictionary<string, int>
    {
        ["cross:Llyn.UIDeportment>Llyn.Application"] = 7,
        ["cross:Llyn.UIDeportment>Llyn.Core"] = 49,
        ["cross:Llyn.UIDeportment>Llyn.ShellEngine"] = 27,
        ["cross:Llyn.UIVeneer>Llyn.Application"] = 27,
        ["cross:Llyn.UIVeneer>Llyn.Core"] = 129,
        ["expose:Llyn.UIDeportment>Llyn.ShellEngine"] = 2,
        ["expose:Llyn.UIDeportment>Llyn.Core"] = 2,
        ["expose:Llyn.UIDemeanor>Llyn.ShellEngine"] = 2,
        ["expose:Llyn.UIDemeanor>Llyn.Core"] = 2,
    };

    public static readonly string[] TAuditChainWaiver = [];
}
