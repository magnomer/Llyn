namespace Convention.Tests;

internal static class TAuditFrameSetting
{
    public const int TAuditGeneration = 15;

    public static readonly string[] TAuditFramePure =
    [
        "Llyn.Core",
        "Llyn.Application",
        "Llyn.ShellEngine",
        "Llyn.Conduct",
    ];

    public static readonly string[] TAuditFrameAllowed =
    [
        "System",
        "System.Collections.Generic",
        "System.Linq",
        "System.Text",
        "System.Text.RegularExpressions",
        "System.Globalization",
        "System.Threading",
        "System.Threading.Tasks",
        "System.Diagnostics.CodeAnalysis",
        "System.Runtime.CompilerServices",
    ];

    public static readonly IReadOnlyDictionary<string, string[]> TAuditFrameExtra = new Dictionary<string, string[]>
    {
        ["Llyn.Core"] = [],
        ["Llyn.Application"] = [],
        ["Llyn.ShellEngine"] = ["System.Runtime.ExceptionServices"],
        ["Llyn.Conduct"] = ["System.Runtime.ExceptionServices"],
    };

    public static readonly string[] TAuditFrameAmbient =
    [
        "System.DateTime.Now",
        "System.DateTime.UtcNow",
        "System.DateTime.Today",
        "System.DateTimeOffset.Now",
        "System.DateTimeOffset.UtcNow",
        "System.Environment",
        "System.Guid.NewGuid",
        "System.Random",
        "System.Console",
        "System.IO.File",
        "System.IO.Directory",
        "System.IO.Path",
        "System.Diagnostics.Process",
        "System.Diagnostics.Stopwatch",
    ];

    public static readonly IReadOnlyDictionary<string, int> TAuditFrameCeiling = new Dictionary<string, int>();

    public static readonly string[] TAuditFrameWaiver = [];
}
