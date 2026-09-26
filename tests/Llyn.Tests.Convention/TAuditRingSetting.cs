namespace Convention.Tests;

internal static class TAuditRingSetting
{
    public const int TAuditGeneration = 15;

    public static readonly IReadOnlyDictionary<string, string[]> TAuditRingEdges = new Dictionary<string, string[]>
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
        ["Llyn.UIDeportment"] = ["Llyn.Conduct"],
        ["Llyn.UIVeneer"] = ["Llyn.UIDeportment"],
    };

    public static readonly IReadOnlyDictionary<string, int> TAuditRingCeiling = new Dictionary<string, int>
    {
        ["Transitive"] = 4,
    };
}
