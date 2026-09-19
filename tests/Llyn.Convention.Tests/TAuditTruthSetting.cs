namespace Convention.Tests;

internal static partial class TAuditTruthSetting
{
    public const int TAuditGeneration = 9;
    public const bool TAuditTruthEnforced = true;
    public const string TAuditTruthReport = "temp/audit/Custody-{0}.md";
    public const string TAuditStateSuffix = "State";
    public const string TAuditBulletinType = "LBulletin";
    public const string TAuditObserverType = "PObserver";

    public static readonly string[] TAuditTruthInclude =
    [
        "src/Llyn.UIVeneer/*.cs",
        "src/Llyn.UIDeportment/*.cs",
    ];

    public static readonly string[] TAuditOrderVerbs =
    [
        "Insert",
        "Move",
        "RemoveAt",
        "Reverse",
        "Sort",
    ];

    public static readonly string[] TAuditFillVerbs =
    [
        "Add",
        "AddRange",
        "Clear",
        "Enqueue",
        "Insert",
        "InsertRange",
        "Move",
        "Push",
        "Remove",
        "RemoveAll",
        "RemoveAt",
        "RemoveRange",
        "Reverse",
        "Sort",
        "TryAdd",
    ];

    public const string TAuditRequestPrefix = "LRequest";

    public static readonly string[] TAuditSendRoots =
    [
        "LTenureRequestApply",
        "LTenureRequestDefer",
    ];

    public static readonly string[] TAuditClockTypes =
    [
        "DispatcherTimer",
        "PeriodicTimer",
        "Stopwatch",
        "Timer",
    ];

    public static readonly string[] TAuditInputMembers =
    [
        "IsChecked",
        "Password",
        "SelectedIndex",
        "SelectedItem",
        "SelectedValue",
        "Text",
        "Value",
    ];

    public static readonly string[] TAuditTruthHandles =
    [
        "LEngine",
        "LForay",
        "LTenure",
        "LVista",
        "LListener",
        "LReceiver",
        "LObserver",
        "PObserver",
    ];

    public static readonly IReadOnlyDictionary<string, int> TAuditTruthCeiling = new Dictionary<string, int>
    {
        ["Argument"] = 244,
        ["Guard"] = 215,
        ["Fork"] = 36,
        ["Mirror"] = 41,
        ["Mutation"] = 20,
        ["Shape"] = 99,
    };

    public static string[] TAuditTruthWaiver => [.. TAuditWaiverArgument, .. TAuditWaiverGuard, .. TAuditWaiverField];
}
