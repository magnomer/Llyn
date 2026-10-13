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
        CTimbre timbre = TTimbre.TTimbreFlaggedPrepare(engine, pack.TLanguageFixtureName).TEditorFixtureTimbre;
        List<string> stored = [];

        CTimbreAccent? accent = await timbre.CTimbreFlagRead((rows, _) =>
        {
            stored.AddRange(rows.Select(static row => row.CEnsignRowKey));
            return static () => { };
        });

        Assert.Equal([pack.TLanguageFixtureName + "/British"], stored);
        Assert.NotNull(accent);
        Assert.True(accent.CTimbreAccentFlagged);
        Assert.Equal(
            CVariety.CVarietyRead(pack.TLanguageFixtureName, "British"), accent.CTimbreAccentPrimary);
    }

    [Fact]
    public async Task TimbreFlagRead_LanguageChangedMeanwhile_AnswersNothing()
    {
        using TLanguageFixture pack = TDisplayAccent.TDisplayPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TTimbre.TTimbreFlaggedPrepare(engine, pack.TLanguageFixtureName);

        CTimbreAccent? accent = await editor.TEditorFixtureTimbre.CTimbreFlagRead((_, _) =>
        {
            editor.TEditorFixtureEntry.CEntryLanguageSet("English");
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
        CTimbre timbre = TTimbre.TTimbreFlaggedPrepare(engine, pack.TLanguageFixtureName).TEditorFixtureTimbre;

        CTimbreAccent? accent = await timbre.CTimbreFlagRead(
            (_, _) => throw new InvalidOperationException("The flag store failed."));

        Assert.Null(accent);
    }

    [Fact]
    public async Task TimbreFlagRead_UnflaggedPack_AnswersNothingAndLoadsNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        editor.TEditorFixtureDesk.TDeskDefer(
            TInterface.TRequestLanguageCreate(editor.TEditorFixtureDesk.CDeskId, pack.TLanguageFixtureName));
        Assert.False(engine.TEngineFlaggedCheck(pack.TLanguageFixtureName));
        List<string> stored = [];

        CTimbreAccent? accent = await editor.TEditorFixtureTimbre.CTimbreFlagRead((rows, _) =>
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
