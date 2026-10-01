namespace Convention.Tests;

internal static class TAuditCharterSetting
{
    public const int TAuditGeneration = 17;

    public static readonly IReadOnlyDictionary<string, string[]> TAuditCharterEdges = new Dictionary<string, string[]>
    {
        ["Llyn.Core"] = [],
        ["Llyn.Application"] = ["Llyn.Core"],
        ["Llyn.Infrastructure"] = ["Llyn.Core"],
        ["Llyn.Core.Windows"] = ["Llyn.Core"],
        ["Llyn.Host"] =
        [
            "Llyn.Core", "Llyn.Application", "Llyn.Infrastructure", "Llyn.Core.Windows", "Llyn.ShellEngine",
            "Llyn.Conduct", "Llyn.UIDeportment", "Llyn.UIVeneer", "Llyn.UIDemeanor", "Llyn.UITerminal",
        ],
        ["Llyn.UITerminal"] = ["Llyn.UIDemeanor"],
        ["Llyn.UIDemeanor"] = ["Llyn.Conduct"],
        ["Llyn.ShellEngine"] = ["Llyn.Application"],
        ["Llyn.Conduct"] = ["Llyn.ShellEngine"],
        ["Llyn.UIDeportment"] = ["Llyn.Conduct", "Llyn.UIDeportment.Capsule"],
        ["Llyn.UIDeportment.Capsule"] = [],
        ["Llyn.UIVeneer"] = ["Llyn.UIDeportment"],
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditCharterCeiling = new Dictionary<string, int>
    {
        ["Piggybacking"] = 4,
    };
}
