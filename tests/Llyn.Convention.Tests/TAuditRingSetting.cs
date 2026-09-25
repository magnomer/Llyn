namespace Convention.Tests;

internal static class TAuditRingSetting
{
    public const int TAuditGeneration = 11;

    public static readonly IReadOnlyDictionary<string, string[]> TAuditRingEdges = new Dictionary<string, string[]>
    {
        ["Llyn.Core"] = [],
        ["Llyn.Application"] = ["Llyn.Core"],
        ["Llyn.Infrastructure"] = ["Llyn.Core"],
        ["Llyn.Core.Windows"] = ["Llyn.Core"],
        ["Llyn.Host"] = [],
        ["Llyn.UITerminal"] = ["Llyn.UIDemeanor"],
        ["Llyn.UIDemeanor"] = ["Llyn.Conduct"],
        ["Llyn.ShellEngine"] = ["Llyn.Core", "Llyn.Application"],
        ["Llyn.Conduct"] = ["Llyn.ShellEngine"],
        ["Llyn.UIDeportment"] = ["Llyn.Conduct"],
        ["Llyn.UIVeneer"] =
            ["Llyn.Core", "Llyn.Infrastructure", "Llyn.Core.Windows", "Llyn.ShellEngine", "Llyn.UIDeportment"],
    };
}
