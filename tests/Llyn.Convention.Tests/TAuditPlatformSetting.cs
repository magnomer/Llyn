namespace Convention.Tests;

internal static class TAuditPlatformSetting
{
    public const int TAuditGeneration = 14;
    public const bool TAuditPlatformEnforced = true;
    public const string TAuditPlatformRoot = "src/";
    public const string TAuditPlatformPortable = "net10.0";
    public const string TAuditPlatformTwin = "net10.0-windows";
    public const string TAuditPlatformRule = "CA1416";

    public static readonly string[] TAuditPlatformShell = ["Llyn.Conduct"];

    public static readonly string[] TAuditPlatformKinds =
    [
        "Unmapped", "Absent", "Framework", "Reference", "Column", "Analyzer", "Windows", "Empty",
        "Suppress", "Implicit", "Domain",
    ];

    public static readonly IReadOnlyDictionary<string, string> TAuditPlatformSilencers =
        new Dictionary<string, string>
        {
            ["AnalysisLevel"] = "none",
            ["AnalysisMode"] = "None",
            ["EnableNETAnalyzers"] = "false",
            ["RunAnalyzers"] = "false",
            ["RunAnalyzersDuringBuild"] = "false",
        };

    public static readonly IReadOnlyDictionary<string, string> TAuditPlatformColumn = new Dictionary<string, string>
    {
        ["Llyn.Core"] = "Llyn.Core",
        ["Llyn.Core.Windows"] = "Llyn.Core",
        ["Llyn.Application"] = "Llyn.Application",
        ["Llyn.Infrastructure"] = "Llyn.Infrastructure",
        ["Llyn.ShellEngine"] = "Llyn.ShellEngine",
        ["Llyn.Conduct"] = "Llyn.Conduct",
        ["Llyn.UIDeportment"] = "Llyn.Conduct",
        ["Llyn.UIVeneer"] = "Llyn.Conduct",
        ["Llyn.UIDemeanor"] = "Llyn.UIDemeanor",
        ["Llyn.UITerminal"] = "Llyn.UITerminal",
        ["Llyn.Host"] = "",
    };

    public static readonly string[] TAuditPlatformProperties = ["UseWPF", "UseWindowsForms", "UseWinUI"];

    public static readonly string[] TAuditPlatformPackages =
    [
        "Microsoft.Web.WebView2",
        "Microsoft.WindowsAppSDK",
        "Microsoft.Windows.Compatibility",
        "SharpVectors.Wpf",
    ];

    public static readonly string[] TAuditPlatformPatterns =
    [
        @"\bSystem\.Windows\b",
        @"\bMicrosoft\.Win32\b",
        @"\bWindows\.(Win32|UI|Storage|Media|System|Foundation|Graphics|Devices)\b",
        @"\[\s*(assembly\s*:\s*)?(DllImport|LibraryImport|SupportedOSPlatform)\b",
        @"\bOperatingSystem\.IsWindows\w*\b",
        @"\bRuntimeInformation\.IsOSPlatform\b",
    ];

    public static readonly IReadOnlyDictionary<string, int> TAuditPlatformCeiling = new Dictionary<string, int>
    {
        ["Unmapped"] = 0,
        ["Absent"] = 0,
        ["Column"] = 0,
    };
}
