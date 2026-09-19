namespace Convention.Tests;

internal static class TAuditRingSetting
{
    public const int TAuditGeneration = 9;

    public static readonly IReadOnlyDictionary<string, int> TAuditRingCeiling = new Dictionary<string, int>
    {
        ["EngineField"] = 23,
        ["EngineHelper"] = 75,
        ["AdapterEngine"] = 253,
    };

    public static readonly IReadOnlyDictionary<string, string[]> TAuditRingEdges = new Dictionary<string, string[]>
    {
        ["Llyn.Core"] = [],
        ["Llyn.Application"] = ["Llyn.Core"],
        ["Llyn.Infrastructure"] = ["Llyn.Core"],
        ["Llyn.Media"] = ["Llyn.Core"],
        ["Llyn.ShellEngine"] = ["Llyn.Core", "Llyn.Application", "Llyn.Infrastructure"],
        ["Llyn.UIDeportment"] = ["Llyn.Core", "Llyn.Application", "Llyn.ShellEngine"],
        ["Llyn.UIVeneer"] = ["Llyn.Core", "Llyn.Media", "Llyn.ShellEngine", "Llyn.UIDeportment"],
    };

    public static readonly IReadOnlyDictionary<string, string[]> TAuditRingRoles = new Dictionary<string, string[]>
    {
        ["Llyn.Core"] =
        [
            "Llyn.Application", "Llyn.Infrastructure", "Llyn.Media", "Llyn.ShellEngine",
            "Llyn.UIDeportment", "Llyn.UIVeneer", "System.IO", "System.Reflection", "System.Net",
        ],
        ["Llyn.Application"] =
        [
            "Llyn.Infrastructure", "Llyn.Media", "Llyn.ShellEngine", "Llyn.UIDeportment", "Llyn.UIVeneer",
            "System.IO", "System.Reflection", "System.Net",
        ],
        ["Llyn.ShellEngine"] = ["Llyn.Infrastructure", "Llyn.Media", "Llyn.UIDeportment", "Llyn.UIVeneer"],
        ["Llyn.Infrastructure"] = ["Llyn.Media", "Llyn.ShellEngine", "Llyn.UIDeportment", "Llyn.UIVeneer"],
        ["Llyn.Media"] = ["Llyn.Infrastructure", "Llyn.ShellEngine", "Llyn.UIDeportment", "Llyn.UIVeneer"],
        ["Llyn.UIDeportment"] = ["Llyn.Infrastructure", "Llyn.Media", "Llyn.UIVeneer"],
        ["Llyn.UIVeneer"] = ["Llyn.Infrastructure"],
    };

    public static readonly string[] TAuditRingExempt =
    [
        "src/Llyn.UIVeneer/App.xaml.cs:Llyn.Infrastructure",
    ];

    public static readonly string[] TAuditRingStream =
    [
        @"\b(File|Directory|Path|FileInfo|DirectoryInfo|FileSystemInfo|FileSystemWatcher)\b",
        @"\b(Stream|FileStream|MemoryStream|BufferedStream|StreamReader|StreamWriter)\b",
        @"\b(BinaryReader|BinaryWriter|StringWriter|TextWriter|SeekOrigin)\b",
        @"\b(FileMode|FileAccess|FileShare|FileOptions|FileAttributes|SearchOption)\b",
        @"\b(IOException|FileNotFoundException|DirectoryNotFoundException|PathTooLongException)\b",
    ];

    public static readonly string[] TAuditRingWaiver =
    [
        "src/Llyn.Application/Ensign/LEnsign.cs:System.IO",
        "src/Llyn.Application/Localization/LLocalization.cs:System.IO",
        "src/Llyn.Application/Localization/LLocalization.cs:System.Reflection",
        "src/Llyn.Application/Localization/LLocalizationReader.cs:System.IO",
        "src/Llyn.ShellEngine/*:Llyn.Infrastructure",
        "src/Llyn.UIVeneer/UI/PDial.cs:Llyn.Infrastructure",
        "src/Llyn.UIVeneer/UI/PMarkdown.cs:Llyn.Infrastructure",
        "src/Llyn.UIVeneer/UI/PThemeLoader.cs:Llyn.Infrastructure",
    ];
}
