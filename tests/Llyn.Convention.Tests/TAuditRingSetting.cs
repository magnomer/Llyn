namespace Convention.Tests;

internal static class TAuditRingSetting
{
    public const int TAuditGeneration = 10;

    public static readonly IReadOnlyDictionary<string, string[]> TAuditRingEdges = new Dictionary<string, string[]>
    {
        ["Llyn.Core"] = [],
        ["Llyn.Application"] = ["Llyn.Core"],
        ["Llyn.Infrastructure"] = ["Llyn.Core"],
        ["Llyn.Media"] = ["Llyn.Core"],
        ["Llyn.ShellEngine"] = ["Llyn.Core", "Llyn.Application"],
        ["Llyn.UIDeportment"] = ["Llyn.Core", "Llyn.Application", "Llyn.ShellEngine"],
        ["Llyn.UIVeneer"] = ["Llyn.Core", "Llyn.Infrastructure", "Llyn.Media", "Llyn.ShellEngine", "Llyn.UIDeportment"],
    };
}
