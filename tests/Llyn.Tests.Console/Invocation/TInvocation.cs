using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
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

        int code = TInvocationRun(console, string.Empty, _ => throw new InvalidOperationException(), args);

        Assert.Equal(2, code);
        Assert.StartsWith("llyn: " + sentence, Assert.Single(console.TConsoleFakeError), StringComparison.Ordinal);
        Assert.Empty(console.TConsoleFakeOutput);
    }

    [Fact]
    public void BootstrapIntroduce_NoArgumentRedirected_PrintsUsage()
    {
        TConsoleFake console = new();

        int code = TInvocationRun(console, string.Empty, _ => throw new InvalidOperationException());

        Assert.Equal(2, code);
        string usage = "llyn: Name one verb as an option, in the form -name or -name:value.";
        Assert.Equal([usage], console.TConsoleFakeError);
    }

    [Fact]
    public void BootstrapIntroduce_WorkspaceBusy_ExitsThree()
    {
        TConsoleFake console = new();

        int code = TInvocationRun(console, "D:\\Busy", _ => throw new IOException("locked"), "-version");

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
        Assert.Equal(9, console.TConsoleFakeOutput.Count);
        Assert.Equal(
            "Run llyn with one verb and any further options, each written as -name or -name:value.",
            console.TConsoleFakeOutput[0]);
        Assert.Equal("-version    Prints the version line.", console.TConsoleFakeOutput[2]);
        Assert.Equal("-no         Answers no to every question.", console.TConsoleFakeOutput[5]);
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
            ["-workspace  Prints the workspace folder, or moves the workspace to the folder given as its value."],
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
            _ =>
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
            _ =>
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

    private static int TInvocationRun(TConsoleFake console, TWorkspace workspace, params string[] args)
    {
        return TInvocationRun(console, workspace.TWorkspaceFolder, _ => workspace.TWorkspaceEngineStart(), args);
    }

    private static int TInvocationRun(
        TConsoleFake console, string folder, Func<string, LEngine> build, params string[] args)
    {
        SynchronizationContext? context = SynchronizationContext.Current;
        BBootstrap bootstrap = new(console);
        try
        {
            return bootstrap.BBootstrapIntroduce(
                args,
                static () => TInterface.TLocalizationLoad("en"),
                () => folder,
                build,
                static _ => true,
                engine =>
                {
                    using (engine)
                    {
                        return bootstrap.BBootstrapAtelierRun(TInterfaceConduct.TAtelierCreate(engine));
                    }
                });
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
