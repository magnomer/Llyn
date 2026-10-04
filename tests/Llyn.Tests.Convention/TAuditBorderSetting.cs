namespace Convention.Tests;

internal static class TAuditBorderSetting
{
    public const int TAuditGeneration = 20;
    public const string TAuditBorderHost = "Llyn.Host";

    public static readonly IReadOnlyDictionary<string, string[]> TAuditBorderNeighbour =
        new Dictionary<string, string[]>
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
            ["Llyn.UIDeportment.Capsule"] = [],
        };

    public static readonly IReadOnlyDictionary<string, string> TAuditBorderCapsule = new Dictionary<string, string>
    {
        ["Llyn.UIDeportment"] = "Llyn.UIDeportment.Capsule",
    };

    public static readonly string[] TAuditBorderCut =
    [
        "Llyn.UIVeneer",
        "Llyn.UIDeportment",
        "Llyn.UIDeportment.Capsule",
        "Llyn.UIDemeanor",
        "Llyn.UITerminal",
    ];

    public static readonly IReadOnlyDictionary<string, string[]> TAuditUnsealingPrefix =
        new Dictionary<string, string[]>
        {
            ["Llyn.UIDeportment"] = ["L"],
        };

    public static readonly IReadOnlyDictionary<string, int> TAuditBorderCeiling = new Dictionary<string, int>
    {
        ["Leaking:Llyn.UIDemeanor>Llyn.Core"] = 0,
        ["Leaking:Llyn.UIDemeanor>Llyn.ShellEngine"] = 0,
        ["Leaking:Llyn.UIDeportment>Llyn.Core"] = 0,
        ["Leaking:Llyn.UIDeportment>Llyn.ShellEngine"] = 0,
        ["Leapfrogging:Llyn.Conduct>Llyn.Core"] = 0,
        ["Leapfrogging:Llyn.ShellEngine>Llyn.Core"] = 0,
        ["Undercutting:Llyn.UIDeportment>Llyn.Application"] = 0,
        ["Undercutting:Llyn.UIDeportment>Llyn.Core"] = 0,
        ["Undercutting:Llyn.UIDeportment>Llyn.ShellEngine"] = 0,
        ["Unsealing:Llyn.UIDeportment>Llyn.Application"] = 0,
        ["Unsealing:Llyn.UIDeportment>Llyn.Core"] = 0,
        ["Unsealing:Llyn.UIDeportment>Llyn.ShellEngine"] = 0,
    };

    public static readonly string[] TAuditBorderExempt = [];
}
