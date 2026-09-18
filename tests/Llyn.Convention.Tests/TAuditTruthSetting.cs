namespace Convention.Tests;

internal static partial class TAuditTruthSetting
{
    public const int TAuditGeneration = 9;
    public const bool TAuditTruthEnforced = true;
    public const string TAuditTruthReport = "temp/audit/Custody-{0}.md";
    public const string TAuditStateSuffix = "State";

    public static readonly string[] TAuditTruthInclude =
    [
        "src/Llyn.UIShell/*.cs",
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
        ["Argument"] = 392,
        ["Guard"] = 296,
        ["Fork"] = 39,
        ["Mirror"] = 45,
        ["Mutation"] = 26,
        ["Shape"] = 137,
    };

    public static string[] TAuditTruthWaiver => [.. TAuditWaiverArgument, .. TAuditWaiverGuard, .. TAuditWaiverField];
}
