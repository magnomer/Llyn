namespace Convention.Tests;

internal static class TAuditNameSetting
{
    public const int TAuditGeneration = 26;
    public const string TAuditProject = "Llyn";
    public const string TAuditTestPrefix = "T";
    public const string TAuditComponentPattern = "[A-Z]+(?=[A-Z][a-z]|[0-9]|$)|[A-Z]?[a-z]+|[0-9]+";
    public const int TAuditComponentLimit = 3;
    public const string TAuditXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
    public const string TAuditCancelArgument = "IncludeCancelCommand";
    public const string TAuditAsyncSuffix = "Async";
    public const string TAuditCommandSuffix = "Command";
    public const string TAuditCancelSuffix = "CancelCommand";

    public const int TAuditPrefixCeiling = 0;

    public static readonly string[] TAuditPrefixes =
    [
        "PS",
        "QS",
        "AS",
        "BS",
        "CS",
        "LS",
        "ps",
        "qs",
        "as",
        "bs",
        "cs",
        "ls",
        "P",
        "Q",
        "A",
        "B",
        "C",
        "L",
        "T",
        "p",
        "q",
        "a",
        "b",
        "c",
        "l",
        "t",
    ];

    public static readonly Dictionary<string, string[]> TAuditPrefixTurfs = new(StringComparer.Ordinal)
    {
        ["src/Llyn.Application"] = ["L", "LS"],
        ["src/Llyn.Conduct"] = ["C", "CS"],
        ["src/Llyn.Core"] = ["L", "LS"],
        ["src/Llyn.Core.Windows"] = ["L", "LS"],
        ["src/Llyn.Guestroom"] = ["L", "LS"],
        ["src/Llyn.Host"] = ["L", "LS"],
        ["src/Llyn.Infrastructure"] = ["L", "LS"],
        ["src/Llyn.Launcher"] = ["L", "LS"],
        ["src/Llyn.ShellEngine"] = ["L", "LS"],
        ["src/Llyn.UIDemeanor"] = ["B", "BS"],
        ["src/Llyn.UIDeportment"] = ["Q", "QS"],
        ["src/Llyn.UITerminal"] = ["A", "AS"],
        ["src/Llyn.UIVeneer"] = ["P", "PS"],
        ["tests/Llyn.Tests.Interface"] = ["T"],
        ["tests/Llyn.Tests.Engine"] = ["T"],
        ["tests/Llyn.Tests.Conduct"] = ["T"],
        ["tests/Llyn.Tests.Console"] = ["T"],
        ["tests/Llyn.Tests.Convention"] = ["T"],
        ["tests/Llyn.Tests.Windows"] = ["T"],
    };

    public static readonly string[] TAuditTurfSealed =
    [
        "src/Llyn.Conduct",
    ];

    public static readonly string[] TAuditSourceInclude =
    [
        "*.cs",
        "*.xaml",
    ];

    public static readonly string[] TAuditExcludedSegments =
    [
        ".git",
        "bin",
        "obj",
        "out",
        "publish",
        "snapshots",
        "TestResults",
    ];

    public static readonly string[] TAuditExcludedSuffixes =
    [
        ".md",
        ".g.cs",
        ".g.i.cs",
        ".AssemblyInfo.cs",
        ".GlobalUsings.g.cs",
        ".Designer.cs",
    ];

    public static readonly string[] TAuditExcludedPrefixes =
    [
        "TemporaryGeneratedFile_",
        "GeneratedInternalTypeHelper",
    ];

    public static readonly string[] TAuditSelfExcluded =
    [
        "TAuditComment.cs",
        "TAuditCommentSetting.cs",
        "TAuditConvention.cs",
        "TAuditLine.cs",
        "TAuditLineSetting.cs",
        "TAuditLogicRegistry.cs",
        "TAuditName.cs",
        "TAuditNameFilter.cs",
        "TAuditNameSetting.cs",
        "TAuditNameWalker.cs",
        "TAuditRegistry.cs",
        "TAuditScope.cs",
        "TAuditSharedRegistry.cs",
        "TAuditSource.cs",
        "TAuditTestRegistry.cs",
        "TAuditUIRegistry.cs",
        "TAuditVerbRegistry.cs",
        "TSpecimen.cs",
        "TViolation.cs",
    ];

    public static readonly string[] TAuditMethodKinds =
    [
        "Method",
        "LocalFunction",
    ];

    public static readonly string[] TAuditDataKinds =
    [
        "Field",
        "Property",
        "EnumMember",
        "RecordProperty",
        "XamlName",
        "GeneratedCommand",
        "TupleElement",
        "TypeParameter",
        "AnonymousMember",
        "ClassDeclaration",
        "StructDeclaration",
        "InterfaceDeclaration",
        "RecordDeclaration",
        "RecordStructDeclaration",
        "EnumDeclaration",
        "Delegate",
        "Event",
        "EventField",
    ];

    public static readonly string[] TAuditTestAttributes =
    [
        "Fact",
        "Theory",
    ];

    public static readonly string[] TAuditGeneratedAttributes =
    [
        "GeneratedCode",
        "CompilerGenerated",
    ];

    public static readonly string[] TAuditExternalAttributes =
    [
        "DllImport",
        "LibraryImport",
    ];

    public static readonly string[] TAuditCommandAttributes =
    [
        "RelayCommand",
        "RelayCommandAttribute",
    ];

    public static readonly Dictionary<string, string[]> TAuditFrameworkContracts = new(StringComparer.Ordinal)
    {
        ["IAsyncDisposable"] = ["DisposeAsync"],
        ["IDisposable"] = ["Dispose"],
        ["IEquatable"] = ["Equals"],
        ["IMultiValueConverter"] = ["Convert", "ConvertBack"],
        ["INotifyPropertyChanged"] = ["PropertyChanged"],
        ["INotifyPropertyChanging"] = ["PropertyChanging"],
        ["IProgress"] = ["Report"],
        ["IValueConverter"] = ["Convert", "ConvertBack"],
    };
}
