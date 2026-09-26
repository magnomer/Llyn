namespace Convention.Tests;

internal static class TAuditFakeSetting
{
    public const int TAuditGeneration = 15;
    public const bool TAuditFakeEnforced = true;
    public const string TAuditFakeReport = "temp/audit/Fake-{0}.md";

    public static readonly IReadOnlyDictionary<string, int> TAuditFakeCeiling = new Dictionary<string, int>
    {
        ["Orphan"] = 0,
        ["Tested"] = 0,
    };

    public static readonly string[] TAuditFakeInclude =
    [
        "tests/Llyn.Internal/*.cs",
        "tests/Llyn.Windows/*.cs",
    ];

    public static readonly string[] TAuditMarkupInclude =
    [
        "src/*.xaml",
    ];

    public static readonly string[] TAuditFakeUsing =
    [
        "System",
        "System.Collections.Generic",
        "System.IO",
        "System.Linq",
        "System.Net.Http",
        "System.Threading",
        "System.Threading.Tasks",
    ];
}
