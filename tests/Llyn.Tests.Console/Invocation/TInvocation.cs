using Llyn.Conduct;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TInvocation
{
    [Theory]
    [InlineData(new[] { "help" }, "The argument help is not an option. Write each option as a dash")]
    [InlineData(new[] { "--help" }, "The argument --help is not an option. Write each option as a dash")]
    [InlineData(new[] { "/help" }, "The argument /help is not an option. Write each option as a dash")]
    [InlineData(new[] { "-help=version" }, "The argument -help=version is not an option.")]
    [InlineData(new[] { "-Help" }, "The argument -Help is not an option.")]
    [InlineData(new[] { "-frobnicate" }, "The option -frobnicate is unknown. Run llyn -help to list the options.")]
    [InlineData(new[] { "-ver" }, "The option -ver is unknown. Run llyn -help to list the options.")]
    [InlineData(new[] { "-color", "-version" }, "The option -color needs a value after a colon.")]
    [InlineData(new[] { "-workspace:" }, "The option -workspace needs a value after a colon.")]
    [InlineData(new[] { "-find:" }, "The option -find needs a value after a colon.")]
    [InlineData(new[] { "-find" }, "The option -find needs a value after a colon.")]
    [InlineData(new[] { "-new" }, "The option -new needs a value after a colon.")]
    [InlineData(new[] { "-new:word" }, "The option -new does not accept the value word.")]
    [InlineData(new[] { "-new:entry", "-headword" }, "The option -headword needs a value after a colon.")]
    [InlineData(new[] { "-delete:abc" }, "The option -delete does not accept the value abc.")]
    [InlineData(new[] { "-delete:-4" }, "The option -delete does not accept the value -4.")]
    [InlineData(new[] { "-delete:0" }, "The option -delete does not accept the value 0.")]
    [InlineData(new[] { "-import" }, "The option -import needs a value after a colon.")]
    [InlineData(new[] { "-export:5" }, "The option -export needs the option -to.")]
    [InlineData(new[] { "-export:abc", "-to:a.md" }, "The option -export does not accept the value abc.")]
    [InlineData(new[] { "-export:5", "-to:a.md", "-as:rtf" }, "The option -as does not accept the value rtf.")]
    [InlineData(new[] { "-export:5", "-to:" }, "The option -to needs a value after a colon.")]
    [InlineData(new[] { "-format:pdf", "-version" }, "The option -format does not accept the value pdf.")]
    [InlineData(new[] { "-version:1" }, "The option -version takes no value.")]
    [InlineData(new[] { "-color:pink", "-version" }, "The option -color does not accept the value pink.")]
    [InlineData(new[] { "-help:frobnicate" }, "The option -help does not accept the value frobnicate.")]
    [InlineData(new[] { "-yes", "-yes", "-version" }, "The option -yes may be given only once.")]
    [InlineData(new[] { "-help", "-version" }, "The options -help and -version are both verbs, and only one verb")]
    [InlineData(new[] { "-yes", "-no", "-version" }, "The options -yes and -no cannot be given together.")]
    [InlineData(new[] { "-quiet" }, "Name one verb as an option, in the form -name or -name:value.")]
    public void BootstrapIntroduce_UsageError_PrintsTheSentence(string[] args, string sentence)
    {
        TConsoleFake console = new();

        int code = TInvocationRun(console, string.Empty, (_, _) => throw new InvalidOperationException(), args);

        Assert.Equal(2, code);
        Assert.StartsWith("llyn: " + sentence, Assert.Single(console.TConsoleFakeError), StringComparison.Ordinal);
        Assert.Empty(console.TConsoleFakeOutput);
    }

    [Fact]
    public void BootstrapIntroduce_NoArgumentRedirected_PrintsUsage()
    {
        TConsoleFake console = new();

        int code = TInvocationRun(console, string.Empty, (_, _) => throw new InvalidOperationException());

        Assert.Equal(2, code);
        string usage = "llyn: Name one verb as an option, in the form -name or -name:value.";
        Assert.Equal([usage], console.TConsoleFakeError);
    }

    [Fact]
    public void BootstrapIntroduce_WorkspaceBusy_ExitsThree()
    {
        TConsoleFake console = new();

        int code = TInvocationRun(console, "D:\\Busy", (_, _) => throw new IOException("locked"), "-version");

        Assert.Equal(3, code);
        string busy = "llyn: The workspace database is in use by another program. Close it and try again.";
        Assert.Equal([busy, "D:\\Busy", "locked"], console.TConsoleFakeError);
        Assert.Empty(console.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_Help_ListsEveryOption()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new();

        int code = TInvocationRun(console, workspace, "-help");

        Assert.Equal(0, code);
        Assert.Equal(20, console.TConsoleFakeOutput.Count);
        Assert.Equal(
            "Run llyn with one verb and any further options, each written as -name or -name:value.",
            console.TConsoleFakeOutput[0]);
        Assert.Equal("-version        Prints the version line.", console.TConsoleFakeOutput[2]);
        Assert.Equal("-to             Names the file that -export writes.", console.TConsoleFakeOutput[13]);
        Assert.Equal("-no             Answers no to every question.", console.TConsoleFakeOutput[16]);
        Assert.Empty(console.TConsoleFakeError);
    }

    [Fact]
    public void InvocationRun_HelpVerb_ListsThatVerbOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new();

        int code = TInvocationRun(console, workspace, "-help:workspace");

        Assert.Equal(0, code);
        Assert.Equal(
            ["-workspace      Prints the workspace folder, or moves the workspace to the folder given as its value."],
            console.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_Version_PrintsTheVersionLine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new();

        int code = TInvocationRun(console, workspace, "-version", "-quiet");

        Assert.Equal(0, code);
        Assert.Equal(
            ["Llyn Version " +typeof(BBootstrap).Assembly.GetName().Version!.ToString(3)],
            console.TConsoleFakeOutput);
        Assert.Empty(console.TConsoleFakeError);
    }

    [Fact]
    public void InvocationRun_Workspace_PrintsTheFolder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new();

        int code = TInvocationRun(console, workspace, "-workspace");

        Assert.Equal(0, code);
        Assert.Equal([workspace.TWorkspaceFolder], console.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_WorkspacePath_MovesTheWorkspace()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        List<string> pointed = [];
        TConsoleFake console = new();

        int code = TInvocationRun(
            console,
            first.TWorkspaceFolder,
            (_, _) =>
            {
                LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), pointed.Add);
                return engine;
            },
            "-workspace:" + second.TWorkspaceFolder);

        Assert.Equal(0, code);
        Assert.Equal([second.TWorkspaceFolder], pointed);
        Assert.Equal([second.TWorkspaceFolder], console.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_WorkspaceMoveFailed_ExitsOne()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new();

        int code = TInvocationRun(
            console,
            first.TWorkspaceFolder,
            (_, _) =>
            {
                Func<string, LRig> refused = _ => throw new IOException("locked");
                LEngine engine = new(first.TWorkspaceRigCreate(), refused, static _ => { });
                return engine;
            },
            "-workspace:" + second.TWorkspaceFolder);

        Assert.Equal(1, code);
        Assert.Equal("llyn: The workspace could not be opened.", console.TConsoleFakeError[0]);
        Assert.Equal([first.TWorkspaceFolder], console.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_FindText_AlignsColumnsByCells()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long latin;
        long han;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            latin = engine.TEngineTranslationCreate("abcde", "English").LEntryId;
            han = engine.TEngineTranslationCreate("ab水水", "Chinese").LEntryId;
        }

        TConsoleFake console = new();

        int code = TInvocationRun(console, workspace, "-find:ab");

        Assert.Equal(0, code);
        Assert.Equal(
            [$"{latin}  abcde   English", $"{han}  ab水水  Chinese"],
            console.TConsoleFakeOutput.OrderBy(static line => line, StringComparer.Ordinal));
        Assert.Empty(console.TConsoleFakeError);
    }

    [Fact]
    public void InvocationRun_FindTsv_PrintsTabbedFields()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineTranslationCreate("ab水水", "Chinese").LEntryId;
        }

        TConsoleFake console = new();

        int code = TInvocationRun(console, workspace, "-format:tsv", "-find:ab");

        Assert.Equal(0, code);
        Assert.Equal([$"{id}\tab水水\tChinese\t"], console.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_FindJson_PrintsInvariantArray()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineTranslationCreate("ab水水", "Chinese").LEntryId;
        }

        TConsoleFake console = new();

        int code = TInvocationRun(console, workspace, "-find:ab", "-format:json");

        Assert.Equal(0, code);
        Assert.Equal(
            [$$"""[{"id":{{id}},"headword":"ab水水","language":"Chinese","epithet":""}]"""],
            console.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_FindNoMatch_PrintsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            engine.TEngineTranslationCreate("abcde", "English");
        }

        TConsoleFake console = new();

        int code = TInvocationRun(console, workspace, "-find:zzz", "-format:json");

        Assert.Equal(0, code);
        Assert.Empty(console.TConsoleFakeOutput);
        Assert.Empty(console.TConsoleFakeError);
    }

    [Fact]
    public void InvocationRun_FindLeasedWorkspace_PrintsTheMatch()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine holder = new(
            workspace.TWorkspaceRigCreate(), _ => workspace.TWorkspaceRigCreate(), static _ => { }, true);
        long id = holder.TEngineTranslationCreate("ab水水", "Chinese").LEntryId;
        TConsoleFake console = new();

        int code = TInvocationRun(console, workspace, "-format:tsv", "-find:ab");

        Assert.Equal(0, code);
        Assert.Equal([$"{id}\tab水水\tChinese\t"], console.TConsoleFakeOutput);
    }

    [Fact]
    public void BootstrapAtelierRun_AnyVerb_DetachesStoredLayout()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        using (LPosture seed = engine.TPostureStart())
        {
            seed.TPostureLayoutSave(TInterface.TLayoutCreate("library", LCatalogOrder.LCatalogOrderReverse));
        }

        string path = Path.Combine(workspace.TWorkspaceFolder, "posture.json");
        string before = File.ReadAllText(path);
        LCatalogOrder? read = null;
        string? after = null;

        TInvocationRun(
            new TConsoleFake(),
            workspace.TWorkspaceFolder,
            (_, _) => workspace.TWorkspaceEngineStart(),
            (engine, detached) =>
            {
                CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, detached);
                LVista vista = atelier.TAtelierVistaStart("library", CCatalogOrder.CCatalogOrderHeadword);
                read = vista.LVistaOrder;
                vista.TVistaOrderSet(LCatalogOrder.LCatalogOrderRecent);
                after = File.ReadAllText(path);
                return atelier;
            },
            "-version");

        Assert.Equal(LCatalogOrder.LCatalogOrderHeadword, read);
        Assert.Equal(before, after);
    }

    internal static int TInvocationRun(TConsoleFake console, TWorkspace workspace, params string[] args)
    {
        return TInvocationRun(
            console,
            workspace.TWorkspaceFolder,
            (_, leased) => leased
                ? new LEngine(
                    workspace.TWorkspaceRigCreate(), _ => workspace.TWorkspaceRigCreate(), static _ => { }, true)
                : workspace.TWorkspaceEngineStart(),
            args);
    }

    internal static int TInvocationRun(
        TConsoleFake console, string folder, Func<string, bool, LEngine> build, params string[] args)
    {
        return TInvocationRun(console, folder, build, TInterfaceConduct.TAtelierCreate, args);
    }

    internal static int TInvocationRun(
        TConsoleFake console,
        string folder,
        Func<string, bool, LEngine> build,
        Func<LEngine, bool, CAtelier> create,
        params string[] args)
    {
        return TInvocationRun(console, "en", folder, build, create, args);
    }

    internal static int TInvocationRun(
        TConsoleFake console,
        string language,
        string folder,
        Func<string, bool, LEngine> build,
        Func<LEngine, bool, CAtelier> create,
        params string[] args)
    {
        Func<string, Func<string, string>> theme = name => LThemeLoader.LThemeLoaderLoad(name).LThemeColorRead;
        return TInvocationRun(console, theme, language, folder, build, create, args);
    }

    internal static int TInvocationRun(
        TConsoleFake console,
        Func<string, Func<string, string>> theme,
        string language,
        string folder,
        Func<string, bool, LEngine> build,
        Func<LEngine, bool, CAtelier> create,
        params string[] args)
    {
        SynchronizationContext? context = SynchronizationContext.Current;
        BBootstrap bootstrap = new(console, new ABootstrap());
        try
        {
            return bootstrap.BBootstrapIntroduce(
                args,
                theme,
                () => TInterface.TLocalizationLoad(language),
                () => folder,
                build,
                static _ => true,
                engine =>
                {
                    using (engine)
                    {
                        return bootstrap.BBootstrapAtelierRun(detached => create(engine, detached), File.Exists);
                    }
                });
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
