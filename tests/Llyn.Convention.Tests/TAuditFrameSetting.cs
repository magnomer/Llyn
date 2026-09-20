namespace Convention.Tests;

internal static class TAuditFrameSetting
{
    public const int TAuditGeneration = 10;

    public static readonly string[] TAuditFramePure =
    [
        "Llyn.Core",
        "Llyn.Application",
        "Llyn.ShellEngine",
        "Llyn.UIDeportment",
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
        "System.Runtime.ExceptionServices",
    ];

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

    public static readonly IReadOnlyDictionary<string, int> TAuditFrameCeiling = new Dictionary<string, int>
    {
        ["frame:Llyn.Application>System.IO"] = 2,
        ["frame:Llyn.Application>System.Reflection"] = 1,
        ["frame:Llyn.Application>System.Text.Json"] = 1,
        ["frame:Llyn.Core>System.IO"] = 2,
        ["frame:Llyn.Core>System.Text.Json.Serialization"] = 2,
        ["frame:Llyn.Core>System.Xml"] = 2,
        ["frame:Llyn.Core>System.Xml.Linq"] = 5,
        ["frame:Llyn.ShellEngine>System.IO"] = 8,
        ["frame:Llyn.ShellEngine>System.Text.Json"] = 1,
        ["ambient:Llyn.ShellEngine>System.DateTime.UtcNow"] = 2,
        ["ambient:Llyn.ShellEngine>System.DateTimeOffset.UtcNow"] = 7,
        ["ambient:Llyn.ShellEngine>System.Environment"] = 1,
        ["ambient:Llyn.ShellEngine>System.IO.Path"] = 6,
    };

    public static readonly string[] TAuditFrameWaiver = [];
}
