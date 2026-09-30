using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTimbreEnsign
{
    [Fact]
    public async Task TimbreFlagRead_FlaggedPack_StoresTheVarietyFlagsAndAnswersTheSheet()
    {
        using TLanguageFixture pack = TDisplayAccent.TDisplayPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbre.TTimbreFlaggedPrepare(engine, pack.TLanguageFixtureName);
        List<string> stored = [];

        CTimbreAccent? accent = await editor.CEditorTimbre.CTimbreFlagRead((rows, _) =>
        {
            stored.AddRange(rows.Select(static row => row.CEnsignRowKey));
            return static () => { };
        });

        Assert.Equal([pack.TLanguageFixtureName + "/British"], stored);
        Assert.NotNull(accent);
        Assert.True(accent.CTimbreAccentFlagged);
        Assert.Equal(
            CSounding.CSoundingVarietyRead(pack.TLanguageFixtureName, "British"), accent.CTimbreAccentPrimary);
    }

    [Fact]
    public async Task TimbreFlagRead_LanguageChangedMeanwhile_AnswersNothing()
    {
        using TLanguageFixture pack = TDisplayAccent.TDisplayPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbre.TTimbreFlaggedPrepare(engine, pack.TLanguageFixtureName);

        CTimbreAccent? accent = await editor.CEditorTimbre.CTimbreFlagRead((_, _) =>
        {
            editor.CEditorLanguageSet("English");
            return static () => { };
        });

        Assert.Null(accent);
    }

    [Fact]
    public async Task TimbreFlagRead_StoreFails_AnswersNothing()
    {
        using TLanguageFixture pack = TDisplayAccent.TDisplayPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbre.TTimbreFlaggedPrepare(engine, pack.TLanguageFixtureName);

        CTimbreAccent? accent = await editor.CEditorTimbre.CTimbreFlagRead(
            (_, _) => throw new InvalidOperationException("The flag store failed."));

        Assert.Null(accent);
    }

    [Fact]
    public async Task TimbreFlagRead_UnflaggedPack_AnswersNothingAndLoadsNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        editor.CEditorDesk.TDeskDefer(
            TInterface.TRequestLanguageCreate(editor.CEditorDesk.CDeskId, pack.TLanguageFixtureName));
        Assert.False(editor.CEditorTimbre.CTimbreFlagged);
        List<string> stored = [];

        CTimbreAccent? accent = await editor.CEditorTimbre.CTimbreFlagRead((rows, _) =>
        {
            stored.AddRange(rows.Select(static row => row.CEnsignRowKey));
            return static () => { };
        });

        Assert.Null(accent);
        Assert.Empty(stored);
    }

    [Fact]
    public async Task TimbreFlagRead_EmptyDesk_AnswersNothing()
    {
        CTimbre timbre = TTimbre.TTimbrePrepare([]);

        CTimbreAccent? accent = await timbre.CTimbreFlagRead(
            (_, _) => throw new InvalidOperationException("No draft is held, so nothing loads."));

        Assert.Null(accent);
    }
}
