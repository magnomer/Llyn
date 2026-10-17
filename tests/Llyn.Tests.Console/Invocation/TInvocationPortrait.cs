using System;
using System.IO;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TInvocationPortrait
{
    [Fact]
    public void InvocationRun_ImportYes_PrintsTheCount()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineTranslationCreate("ember", "English").LEntryId;
        }

        string file = TInterface.TMarkupSave(workspace, TInterface.TMarkupLone);
        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(console, workspace, $"-import:{file}", "-yes");

        Assert.Equal(0, code);
        Assert.Equal(["1"], console.TConsoleFakeOutput);
        Assert.Equal(2, console.TConsoleFakeError.Count);
        Assert.Equal($"llyn: ember (English) Append {id}", console.TConsoleFakeError[0]);
        Assert.Contains("braise", console.TConsoleFakeError[1], StringComparison.Ordinal);
        TConsoleFake found = new();
        TInvocation.TInvocationRun(found, workspace, "-find:ember", "-format:tsv");
        Assert.Equal([$"{id}\tember\tEnglish\t"], found.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_ImportTerminalYes_ShowsTheRowsAndImports()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        string file = TInterface.TMarkupSave(workspace, TInterface.TMarkupLone);
        TConsoleFake console = new() { TConsoleFakeTerminal = true, TConsoleFakeAnswer = "y" };

        int code = TInvocation.TInvocationRun(console, workspace, $"-import:{file}");

        Assert.Equal(0, code);
        Assert.Equal(["1"], console.TConsoleFakeOutput);
        Assert.Equal("llyn: ember (English) Create", console.TConsoleFakeError[0]);
        Assert.Equal("llyn: Import the entries as listed? [y/N]", console.TConsoleFakeError[1]);
        TConsoleFake found = new();
        TInvocation.TInvocationRun(found, workspace, "-find:ember", "-format:tsv");
        Assert.Single(found.TConsoleFakeOutput);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void InvocationRun_ImportDeclined_ImportsNothing(bool answered)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        string file = TInterface.TMarkupSave(workspace, TInterface.TMarkupLone);
        TConsoleFake console = new();

        int code = answered
            ? TInvocation.TInvocationRun(console, workspace, $"-import:{file}", "-no")
            : TInvocation.TInvocationRun(console, workspace, $"-import:{file}");

        Assert.Equal(1, code);
        Assert.Empty(console.TConsoleFakeOutput);
        Assert.Equal(["llyn: ember (English) Create"], console.TConsoleFakeError);
        TConsoleFake found = new();
        TInvocation.TInvocationRun(found, workspace, "-find:ember", "-format:tsv");
        Assert.Empty(found.TConsoleFakeOutput);
    }

    [Fact]
    public void InvocationRun_ExportTo_PrintsThePath()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineTranslationCreate("water", "English").LEntryId;
        }

        string path = Path.Combine(workspace.TWorkspaceFolder, "water.md");
        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(
            console, workspace, $"-export:{id}", $"-to:{path}", "-as:markdown");

        Assert.Equal(0, code);
        Assert.Equal([path], console.TConsoleFakeOutput);
        Assert.Empty(console.TConsoleFakeError);
        Assert.Contains("water", File.ReadAllText(path), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("-yes", 0)]
    [InlineData("-no", 1)]
    public void InvocationRun_ExportExisting_ReplacesOnYesOnly(string answer, int expected)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineTranslationCreate("water", "English").LEntryId;
        }

        string path = Path.Combine(workspace.TWorkspaceFolder, "water.md");
        File.WriteAllText(path, "old");
        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(
            console, workspace, $"-export:{id}", $"-to:{path}", "-as:markdown", answer);

        Assert.Equal(expected, code);
        Assert.Equal(expected == 0 ? [path] : [], console.TConsoleFakeOutput);
        Assert.Empty(console.TConsoleFakeError);
        Assert.Equal(expected == 0, File.ReadAllText(path).Contains("water", StringComparison.Ordinal));
    }

    [Fact]
    public void InvocationRun_ExportMissingId_RefusesTheId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineTranslationCreate("water", "English").LEntryId;
        }

        string path = Path.Combine(workspace.TWorkspaceFolder, "water.md");
        TConsoleFake console = new();

        int code = TInvocation.TInvocationRun(console, workspace, $"-export:{id + 100}", $"-to:{path}");

        Assert.Equal(1, code);
        Assert.Empty(console.TConsoleFakeOutput);
        Assert.Equal(["llyn: No entry in this workspace has that id."], console.TConsoleFakeError);
        Assert.False(File.Exists(path));
    }
}
