using System;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TInvocationLectern
{
    [Fact]
    public void InvocationRun_Show_PrintsTheReadingView()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                "brook", "English", string.Empty, "Last words.", [TInterface.TCardCreate("a small stream", 1)], []))
                .LEntryId;
        }

        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(console, workspace, $"-show:{id}");

        Assert.Equal(0, code);
        Assert.Empty(console.TConsoleFakeError);
        Assert.StartsWith("brook", console.TConsoleFakeOutput[0], StringComparison.Ordinal);
        int meanings = console.TConsoleFakeOutput.IndexOf("Meanings");
        int note = console.TConsoleFakeOutput.IndexOf("Note");
        Assert.True(meanings > 0 && note > meanings);
        Assert.Contains(
            console.TConsoleFakeOutput, static line => line.Contains("a small stream", StringComparison.Ordinal));
        Assert.Equal("  Last words.", console.TConsoleFakeOutput[note + 1]);
        Assert.All(console.TConsoleFakeOutput, static line => Assert.Equal(line.TrimEnd(' '), line));
        TConsoleFake help = new();
        TInvocation.TInvocationRun(help, workspace, "-help:show");
        Assert.StartsWith("-show", Assert.Single(help.TConsoleFakeOutput), StringComparison.Ordinal);
    }

    [Fact]
    public void InvocationRun_ShowMissingId_RefusesTheId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineTranslationCreate("water", "English").LEntryId;
        }

        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(console, workspace, $"-show:{id + 100}");

        Assert.Equal(1, code);
        Assert.Empty(console.TConsoleFakeOutput);
        Assert.Equal(["llyn: No entry in this workspace has that id."], console.TConsoleFakeError);
    }
}
