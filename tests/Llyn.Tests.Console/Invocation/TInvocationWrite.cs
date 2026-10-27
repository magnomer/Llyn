using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TInvocationWrite
{
    [Fact]
    public void InvocationRun_NewEntry_PrintsSavedId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(
            console, workspace, "-new:entry", "-headword:水泳", "-language:Japanese", "-note:-ness");

        Assert.Equal(0, code);
        string id = Assert.Single(console.TConsoleFakeOutput);
        Assert.Empty(console.TConsoleFakeError);
        TConsoleFake found = new();
        TInvocation.TInvocationRun(found, workspace, "-find:水泳", "-format:tsv");
        Assert.Equal([$"{id}\t水泳\tJapanese\t"], found.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_NewWithoutHeadword_RefusesSave()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(console, workspace, "-new:entry", "-note:a note");

        Assert.Equal(1, code);
        Assert.Empty(console.TConsoleFakeOutput);
        Assert.Contains("llyn: An entry cannot be saved without a headword.", console.TConsoleFakeError);
    }

    [Fact]
    public void InvocationRun_NewNothingGiven_RefusesAsEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(console, workspace, "-new:entry", "-language:English");

        Assert.Equal(1, code);
        Assert.Empty(console.TConsoleFakeOutput);
        Assert.Equal(
            ["llyn: The entry holds nothing to save. Give it a headword with -headword:value."],
            console.TConsoleFakeError);
    }

    [Theory]
    [InlineData("-new:entry", "-headword:water")]
    [InlineData("-delete:1", "-yes")]
    [InlineData("-import:import.llx", "-yes")]
    [InlineData("-workspace:C:\\Elsewhere", "-yes")]
    public void BootstrapIntroduce_WriteVerbLeaseHeld_ExitsThree(string verb, string qualifier)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine holder = new(
            workspace.TWorkspaceRigCreate(), _ => workspace.TWorkspaceRigCreate(), static _ => { }, true);
        long id = holder.TEngineTranslationCreate("fire", "English").LEntryId;
        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(console, workspace, verb, qualifier);

        Assert.Equal(3, code);
        Assert.Empty(console.TConsoleFakeOutput);
        Assert.Equal(
            "llyn: The workspace database is in use by another program. Close it and try again.",
            console.TConsoleFakeError[0]);
        TConsoleFake found = new();
        TInvocation.TInvocationRun(found, workspace, "-find:fire", "-format:tsv");
        Assert.Equal([$"{id}\tfire\tEnglish\t"], found.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_DeleteYes_DeletesEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineTranslationCreate("water", "English").LEntryId;
        }

        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(console, workspace, $"-delete:{id}", "-yes");

        Assert.Equal(0, code);
        Assert.Empty(console.TConsoleFakeOutput);
        Assert.Empty(console.TConsoleFakeError);
        TConsoleFake found = new();
        TInvocation.TInvocationRun(found, workspace, "-find:water", "-format:tsv");
        Assert.Empty(found.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_DeleteTerminalYes_AsksAndDeletes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineTranslationCreate("water", "English").LEntryId;
        }

        TConsoleFake console = new() { TConsoleFakeTerminal = true, TConsoleFakeAnswer = "y" };

        int code = TInvocation.TInvocationRun(console, workspace, $"-delete:{id}");

        Assert.Equal(0, code);
        Assert.Equal(["llyn: Delete this entry? It cannot be undone. [y/N]"], console.TConsoleFakeError);
        TConsoleFake found = new();
        TInvocation.TInvocationRun(found, workspace, "-find:water", "-format:tsv");
        Assert.Empty(found.TConsoleFakeOutput);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void InvocationRun_DeleteDeclined_KeepsEntry(bool answered)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineTranslationCreate("water", "English").LEntryId;
        }

        TConsoleFake console = new();

        int code = answered
            ? TInvocation.TInvocationRun(console, workspace, $"-delete:{id}", "-no")
            : TInvocation.TInvocationRun(console, workspace, $"-delete:{id}");

        Assert.Equal(1, code);
        Assert.Empty(console.TConsoleFakeOutput);
        Assert.Empty(console.TConsoleFakeError);
        TConsoleFake found = new();
        TInvocation.TInvocationRun(found, workspace, "-find:water", "-format:tsv");
        Assert.Equal([$"{id}\twater\tEnglish\t"], found.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_DeleteMissingId_RefusesId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineTranslationCreate("water", "English").LEntryId;
        }

        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(console, workspace, $"-delete:{id + 100}", "-yes");

        Assert.Equal(1, code);
        Assert.Equal(["llyn: No entry in this workspace has that id."], console.TConsoleFakeError);
        TConsoleFake found = new();
        TInvocation.TInvocationRun(found, workspace, "-find:water", "-format:tsv");
        Assert.Equal([$"{id}\twater\tEnglish\t"], found.TConsoleFakeOutput);
    }
}
