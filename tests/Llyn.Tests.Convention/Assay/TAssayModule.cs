using Xunit;

namespace Convention.Tests;

public sealed class TAssayModule
{
    [Fact]
    public void AssayModule_EntranceAndContractUse_Passes()
    {
        Dictionary<string, string> sources = new()
        {
            ["src/P/Module.cs"] = """
                namespace M;
                internal sealed class Gate { public Contract Open() => new(); }
                internal sealed class Contract { }
                internal sealed class Hidden { }
                """,
            ["src/P/Caller.cs"] = """
                namespace Outside;
                internal sealed class Caller
                {
                    private readonly M.Gate gate = new();
                    private M.Contract Open() => gate.Open();
                }
                """,
        };
        TAuditModuleRow row = TAssayModuleCreate("Audio", "M.Gate", ["M.Contract"], "src/P/Module.cs");

        Assert.Empty(TAssayRun(sources, [row]));
    }

    [Fact]
    public void AssayModule_BoundInternalForms_Bypass()
    {
        Dictionary<string, string> sources = new()
        {
            ["src/P/Module.cs"] = """
                namespace M;
                internal sealed class Gate { }
                internal class Base { }
                internal interface IHidden { }
                internal sealed class Hidden : Base, IHidden { public static void Ping() { } public void Run() { } }
                internal sealed class HiddenAttribute : System.Attribute { }
                """,
            ["src/P/Caller.cs"] = """
                using Alias = M.Hidden;
                using static M.Hidden;
                namespace Outside;
                [M.HiddenAttribute]
                internal sealed class Caller : M.Base, M.IHidden
                {
                    private Alias? value;
                    private void Go()
                    {
                        new global::M.Hidden().Run();
                        Ping();
                        _ = nameof(M.Hidden);
                        _ = typeof(M.Hidden);
                        System.Collections.Generic.List<M.Hidden>? items = null;
                        _ = value is M.Hidden;
                    }
                }
                """,
        };
        IReadOnlyList<TAuditModuleFinding> findings = TAssayRun(
            sources, [TAssayModuleCreate("Audio", "M.Gate", [], "src/P/Module.cs")]);

        Assert.NotEmpty(findings);
        Assert.All(findings, finding => Assert.Equal("Bypassing", finding.TAuditKind));
        Assert.Equal(findings.Select(finding => finding.TAuditKey).Order(StringComparer.Ordinal),
            findings.Select(finding => finding.TAuditKey));
        Assert.Contains(findings, finding => finding.TAuditSymbol.Contains("M.Hidden", StringComparison.Ordinal));
        Assert.Contains(findings, finding => finding.TAuditSymbol.Contains("Ping", StringComparison.Ordinal));
    }

    [Fact]
    public void AssayModule_UnrelatedSameName_Passes()
    {
        Dictionary<string, string> sources = new()
        {
            ["src/P/Module.cs"] = "namespace M; internal sealed class Gate { } internal sealed class Hidden { }",
            ["src/P/Caller.cs"] = """
                namespace Other;
                internal sealed class Hidden { }
                internal sealed class Caller { Hidden value = new(); }
                """,
        };

        Assert.Empty(TAssayRun(sources, [TAssayModuleCreate("Audio", "M.Gate", [], "src/P/Module.cs")]));
    }

    [Fact]
    public void AssayModule_EntranceShapes_ExposeInternalTypesOncePerMember()
    {
        Dictionary<string, string> sources = new()
        {
            ["src/P/Module.cs"] = """
                namespace M;
                internal class HiddenBase { }
                internal interface IHidden { }
                internal class HiddenConstraint { }
                internal sealed class Hidden { }
                internal delegate Hidden HiddenDelegate(Hidden value);
                internal class Gate : HiddenBase, IHidden
                {
                    public event HiddenDelegate? Changed;
                    public (Hidden Item, Hidden[] Items) Value { get; set; }
                    internal Hidden InternalValue { get; set; } = new();
                    private protected Hidden ProtectedValue { get; set; } = new();
                    public System.Threading.Tasks.Task<System.Collections.Generic.List<Hidden>> Read(
                        Hidden value) => throw null!;
                    public U Build<U>() where U : HiddenConstraint => throw null!;
                }
                """,
        };
        IReadOnlyList<TAuditModuleFinding> findings = TAssayRun(
            sources, [TAssayModuleCreate("Audio", "M.Gate", [], "src/P/Module.cs")]);

        Assert.All(findings, finding => Assert.Equal("Exposing", finding.TAuditKind));
        Assert.Equal(8, findings.Count);
        Assert.All(findings, finding => Assert.Contains(
            ":src/P/Module.cs:", finding.TAuditKey, StringComparison.Ordinal));
        Assert.Single(findings, finding =>
            finding.TAuditSymbol.Contains("M.Gate.Value", StringComparison.Ordinal)
            && finding.TAuditSymbol.EndsWith("->M.Hidden", StringComparison.Ordinal));
        Assert.Contains(findings, finding =>
            finding.TAuditSymbol.EndsWith("->M.HiddenBase", StringComparison.Ordinal));
        Assert.Contains(findings, finding =>
            finding.TAuditSymbol.EndsWith("->M.IHidden", StringComparison.Ordinal));
        Assert.Contains(findings, finding =>
            finding.TAuditSymbol.EndsWith("->M.HiddenConstraint", StringComparison.Ordinal));
    }

    [Fact]
    public void AssayModule_EntranceCycle_UsesCanonicalComponentKey()
    {
        Dictionary<string, string> sources = new()
        {
            ["src/P/A.cs"] = "namespace M; internal sealed class A { public void Run() => new B().Run(); }",
            ["src/P/B.cs"] = "namespace M; internal sealed class B { public void Run() => new C().Run(); }",
            ["src/P/C.cs"] = "namespace M; internal sealed class C { public void Run() => new A().Run(); }",
        };
        TAuditModuleRow[] rows =
        [
            TAssayModuleCreate("A", "M.A", [], "src/P/A.cs"),
            TAssayModuleCreate("B", "M.B", [], "src/P/B.cs"),
            TAssayModuleCreate("C", "M.C", [], "src/P/C.cs"),
        ];

        IReadOnlyList<TAuditModuleFinding> findings = TAssayRun(sources, rows);

        TAuditModuleFinding cycle = Assert.Single(findings);
        Assert.Equal("Cycling:P/A+B+C", cycle.TAuditKey);
    }

    [Fact]
    public void AssayModule_InvalidConfigurations_RefuseBeforeCounting()
    {
        Dictionary<string, string> sources = new()
        {
            ["src/P/A.cs"] = "namespace M; internal partial class Gate { } internal sealed class Contract { }",
            ["src/P/B.cs"] = "namespace M; internal partial class Gate { } internal sealed class Elsewhere { }",
        };
        TAuditModuleRow[] invalid =
        [
            TAssayModuleCreate("MissingFile", "M.Gate", [], "src/P/Missing.cs"),
            TAssayModuleCreate("MissingType", "M.Absent", [], "src/P/A.cs", "src/P/B.cs"),
            TAssayModuleCreate("DuplicateFile", "M.Gate", [], "src/P/A.cs", "src/P/A.cs", "src/P/B.cs"),
            TAssayModuleCreate("EntranceOutside", "M.Elsewhere", [], "src/P/A.cs"),
            TAssayModuleCreate("ContractOutside", "M.Gate", ["M.Elsewhere"], "src/P/A.cs"),
            TAssayModuleCreate("SplitPartial", "M.Gate", [], "src/P/A.cs"),
        ];

        foreach (TAuditModuleRow row in invalid)
        {
            InvalidOperationException error = Assert.Throws<InvalidOperationException>(() => TAssayRun(sources, [row]));
            Assert.NotEmpty(error.Message);
        }
    }

    [Fact]
    public void AssayModule_UnannouncedLegacyCaller_BypassesAnnouncedInternal()
    {
        Dictionary<string, string> sources = new()
        {
            ["src/P/Module.cs"] = """
                namespace M;
                internal sealed class Gate { }
                internal sealed class Hidden { public void Run() { } }
                """,
            ["src/P/Legacy.cs"] = """
                namespace Legacy;
                internal sealed class Caller { void Run() => new M.Hidden().Run(); }
                """,
        };
        IReadOnlyList<TAuditModuleFinding> findings = TAssayRun(
            sources, [TAssayModuleCreate("Audio", "M.Gate", [], "src/P/Module.cs")]);

        Assert.NotEmpty(findings);
        Assert.All(findings, finding => Assert.Equal("Bypassing", finding.TAuditKind));
    }

    [Fact]
    public void AssayModule_RuntimeOwnershipClaims_RemainSemanticLimits()
    {
        Dictionary<string, string> sources = new()
        {
            ["src/P/Module.cs"] = """
                namespace M;
                internal sealed class Snapshot { public int Value { get; init; } }
                internal sealed class Gate
                {
                    public Snapshot Copy() => new();
                    public void Cache(Snapshot value) { }
                    public void Transfer(Snapshot value) { }
                    public void Complete(Snapshot value) { }
                }
                """,
            ["src/P/Caller.cs"] = """
                namespace Outside;
                internal sealed class Caller
                {
                    private readonly M.Gate gate = new();
                    private M.Snapshot? retained;
                    public void Run()
                    {
                        retained = gate.Copy();
                        gate.Cache(retained);
                        gate.Transfer(retained);
                        gate.Complete(retained);
                    }
                }
                """,
        };
        TAuditModuleRow row = TAssayModuleCreate(
            "Audio", "M.Gate", ["M.Snapshot"], "src/P/Module.cs");

        Assert.Empty(TAssayRun(sources, [row]));
    }

    private static TAuditModuleRow TAssayModuleCreate(
        string name,
        string entrance,
        IReadOnlyList<string> contracts,
        params string[] files) => new("P", name, entrance, files, contracts);

    private static IReadOnlyList<TAuditModuleFinding> TAssayRun(
        IReadOnlyDictionary<string, string> sources,
        IReadOnlyList<TAuditModuleRow> rows) =>
        TAuditBinder.TAuditAssayRun(sources, () => TAuditModuleWalker.TAuditRun(rows));
}

