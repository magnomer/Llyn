// Generated file. Do not edit by hand: every line is overwritten when it is rebuilt.
//
// AUDITNAMES GENERATION 7 - settings sidecar for Llyn.
// This is the only file in the convention-test project that carries a project-specific
// value. Every other file is identical in every project at this generation.

namespace Convention.Tests;

internal static class TAuditSetting
{
    public const int TAuditGeneration = 7;
    public const string TAuditProject = "Llyn";
    public const string TAuditTestPrefix = "T";
    public const string TAuditComponentPattern = "[A-Z]+(?=[A-Z][a-z]|[0-9]|$)|[A-Z]?[a-z]+|[0-9]+";
    public const int TAuditComponentLimit = 3;
    public const int TAuditComponentReview = 3;
    public const int TAuditLineLimit = 500;
    public const string TAuditXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
    public const string TAuditCommandCancelArgument = "IncludeCancelCommand";
    public const string TAuditCommandAsyncSuffix = "Async";
    public const string TAuditCommandSuffix = "Command";
    public const string TAuditCommandCancelSuffix = "CancelCommand";

    public static readonly string[] TAuditPrefixes =
    [
        "PS",
        "LS",
        "ps",
        "ls",
        "P",
        "L",
        "T",
        "p",
        "l",
        "t",
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
        "TAuditConvention.cs",
        "TAuditName.cs",
        "TAuditRegistry.cs",
        "TAuditSetting.cs",
        "TAuditSize.cs",
        "TAuditSource.cs",
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

    // AUDIT:SIDECAR:BASES:START
    public static readonly string[] TAuditBases =
    [
        "Answer",
        "Anthology",
        "Articulation",
        "Atlas",
        "Audio",
        "Audit",
        "Author",
        "Bootstrap",
        "Brand",
        "Bulletin",
        "Candidate",
        "Caption",
        "Card",
        "Caret",
        "Case",
        "Catalog",
        "Category",
        "Choice",
        "Citation",
        "Claim",
        "Clip",
        "Cohort",
        "Collocation",
        "Colophon",
        "Compass",
        "Consonant",
        "Contents",
        "Context",
        "Corpus",
        "Court",
        "Database",
        "Degree",
        "Directory",
        "Display",
        "Doctor",
        "Downloader",
        "Draft",
        "Duplex",
        "Editor",
        "Engine",
        "Ensign",
        "Entry",
        "Establishment",
        "Example",
        "Excerpt",
        "Exploration",
        "Favorite",
        "Feature",
        "Field",
        "Folio",
        "Font",
        "Footnote",
        "Form",
        "Funnel",
        "Gamut",
        "Grade",
        "Harvest",
        "Headline",
        "Headquarter",
        "Headword",
        "House",
        "Identity",
        "Image",
        "Imprint",
        "Index",
        "Indicator",
        "Inflection",
        "Input",
        "Inquest",
        "Inquiry",
        "Interface",
        "Inventory",
        "Label",
        "Language",
        "Left",
        "Leftover",
        "Library",
        "Link",
        "Listener",
        "Localization",
        "Logo",
        "Lookup",
        "Markdown",
        "Marker",
        "Markup",
        "Meaning",
        "Membership",
        "Mention",
        "Morphology",
        "Navigation",
        "Notation",
        "Note",
        "Observer",
        "Occurrence",
        "Order",
        "Outcome",
        "Outline",
        "Owner",
        "Panel",
        "Phonology",
        "Playback",
        "Portrait",
        "Press",
        "Probe",
        "Pronunciation",
        "Prospect",
        "Query",
        "Quotation",
        "Rail",
        "Rank",
        "Realm",
        "Recall",
        "Receiver",
        "Recording",
        "Reference",
        "Refusal",
        "Register",
        "Repertoire",
        "Request",
        "Revision",
        "Right",
        "Roof",
        "Roster",
        "Scenario",
        "Schema",
        "Screen",
        "Seam",
        "Seeker",
        "Sentence",
        "Sequence",
        "Series",
        "Settings",
        "Sheet",
        "Shelf",
        "Situation",
        "Slate",
        "Sounding",
        "Source",
        "Speaker",
        "Specimen",
        "Speech",
        "Stack",
        "Stamp",
        "State",
        "Subject",
        "Surface",
        "Survey",
        "Syllable",
        "Tag",
        "Target",
        "Taxonomy",
        "Tenor",
        "Theme",
        "Tier",
        "Title",
        "Tombstone",
        "Transcriber",
        "Transcript",
        "Transcription",
        "Translation",
        "Trove",
        "Twin",
        "Usage",
        "Video",
        "Vignette",
        "Violation",
        "Vocabulary",
        "Volume",
        "Vowel",
        "Window",
        "Workspace",
    ];
    // AUDIT:SIDECAR:BASES:END

    // AUDIT:SIDECAR:VERBS:START
    public static readonly string[] TAuditVerbs =
    [
        "Accept",
        "Add",
        "Adjust",
        "Append",
        "Apply",
        "Attach",
        "Build",
        "Cancel",
        "Change",
        "Check",
        "Clamp",
        "Clear",
        "Clone",
        "Close",
        "Commit",
        "Confirm",
        "Copy",
        "Create",
        "Defer",
        "Delete",
        "Describe",
        "Detach",
        "Dispatch",
        "Dispose",
        "Divide",
        "Draw",
        "Exist",
        "Export",
        "Find",
        "Finish",
        "Format",
        "Handle",
        "Hide",
        "Hook",
        "Import",
        "Insert",
        "Interrupt",
        "Load",
        "Match",
        "Move",
        "Normalize",
        "Open",
        "Parse",
        "Pause",
        "Persist",
        "Place",
        "Play",
        "Prepare",
        "Propagate",
        "Publish",
        "Raise",
        "Read",
        "Rebuild",
        "Record",
        "Redo",
        "Remove",
        "Reset",
        "Resolve",
        "Restore",
        "Resume",
        "Run",
        "Save",
        "Scan",
        "Scroll",
        "Seek",
        "Select",
        "Send",
        "Set",
        "Settle",
        "Shorten",
        "Show",
        "Sort",
        "Start",
        "Stop",
        "Suspend",
        "Sweep",
        "Sync",
        "Tick",
        "Toggle",
        "Undo",
        "Update",
        "Validate",
        "Zoom",
    ];
    // AUDIT:SIDECAR:VERBS:END

    // AUDIT:SIDECAR:EXEMPT:START
    public static readonly Dictionary<string, string[]> TAuditExempt = new(StringComparer.Ordinal)
    {
        ["PART_EditableTextBox"] = ["*"],
        ["PART_Popup"] = ["*"],
        ["PART_Track"] = ["*"],
    };
    // AUDIT:SIDECAR:EXEMPT:END
}
