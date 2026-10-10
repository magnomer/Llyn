using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TReflexOpening
{
    [Fact]
    public void DisplaySpreadCheck_StoredStateWrittenTwice_ReadsThatEntryOpened()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TReflexOpeningPrepare(engine, "water");
        LDisplaySound sound = TInterfaceConductSound.TDisplaySoundCreate(engine);

        engine.TEngineReflexSpread(water, true);
        engine.TEngineReflexSpread(water, true);

        Assert.True(sound.TDisplaySpreadCheck(water));
        Assert.False(sound.TDisplaySpreadCheck(null));
    }

    [Fact]
    public void DisplayFoldOpened_AnotherEntryOpened_ShowsEachEntrysOwnStoredState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TReflexOpeningPrepare(engine, "water");
        long salt = TReflexOpeningPrepare(engine, "salt");
        engine.TEngineReflexSpread(water, true);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);

        wing.CWingEntryOpen(water);
        bool first = wing.CWingDisplay.CDisplaySound.CDisplayFoldOpened;
        wing.CWingEntryOpen(salt);
        bool second = wing.CWingDisplay.CDisplaySound.CDisplayFoldOpened;
        wing.CWingEntryOpen(water);

        Assert.True(first);
        Assert.False(second);
        Assert.True(wing.CWingDisplay.CDisplaySound.CDisplayFoldOpened);
    }

    [Fact]
    public void DisplayReflexToggle_ShownEntry_StoresTheStateAnswersTrueAndRaisesTheDisplayFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TReflexOpeningPrepare(engine, "water");
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water);
        List<long> heard = [];
        wing.CWingDisplay.CDisplayFoldChanged += bulletin => heard.Add(bulletin.CBulletinId);

        bool taken = wing.CWingDisplay.CDisplaySound.CDisplayReflexToggle(true);

        Assert.True(taken);
        Assert.True(engine.TEngineSpreadCheck(water));
        Assert.True(wing.CWingDisplay.CDisplaySound.CDisplayFoldOpened);
        Assert.Equal([water], heard);
    }

    [Fact]
    public void DisplayReflexToggle_NoEntryShown_AnswersFalseAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TReflexOpeningPrepare(engine, "water");
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);

        bool taken = wing.CWingDisplay.CDisplaySound.CDisplayReflexToggle(true);

        Assert.False(taken);
        Assert.False(engine.TEngineSpreadCheck(water));
        Assert.False(wing.CWingDisplay.CDisplaySound.CDisplayFoldOpened);
    }

    [Fact]
    public void DisplayReflexToggle_FailingPort_ShowsTheNoticeAndAnswersFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TReflexOpeningPrepare(engine, "water");
        List<string> asked = [];
        CDisplay display = TInterfaceConductSound.TDisplayChosenCreate(
            engine,
            TInterfaceConduct.TEntryBundleCreate(engine),
            water,
            TInterfaceConduct.TPhonologyBundleCreate(engine, "LReflexPort.LEngineReflexSpread", true),
            asked);
        display.LDisplayRule.LDisplaySound.TDisplaySoundShow(water, engine.TEngineEntryLoad(water)!);

        bool taken = display.CDisplaySound.CDisplayReflexToggle(true);

        Assert.False(taken);
        Assert.Equal(["Reflex.SpreadFailed"], asked);
        Assert.False(engine.TEngineSpreadCheck(water));
    }

    [Fact]
    public void DisplayReflexToggle_OneEntry_LeavesAnotherEntryClosed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TReflexOpeningPrepare(engine, "water");
        long salt = TReflexOpeningPrepare(engine, "salt");
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water);
        wing.CWingDisplay.CDisplaySound.CDisplayReflexToggle(true);
        wing.CWingEntryOpen(salt);

        wing.CWingDisplay.CDisplaySound.CDisplayReflexToggle(false);

        Assert.True(engine.TEngineSpreadCheck(water));
        Assert.False(engine.TEngineSpreadCheck(salt));
    }

    [Fact]
    public void DisplayReflexToggle_EditorOnTheSameEntry_RefreshesTheEditorReflexFromTheStore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TReflexOpeningPrepare(engine, "water");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);
        List<CEntryDraft> shown = [];
        editor.TEditorFixtureEntry.CEntryDraftChanged += shown.Add;
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water);

        wing.CWingDisplay.CDisplaySound.CDisplayReflexToggle(true);

        Assert.NotEmpty(shown);
        Assert.True(editor.TEditorFixtureKindred.CKindredRead().CTimbreReflexOpened);
    }

    [Fact]
    public void KindredRead_AnotherEntryHeld_ReadsEachHeldEntrysOwnStoredState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TReflexOpeningPrepare(engine, "water");
        long salt = TReflexOpeningPrepare(engine, "salt");
        engine.TEngineReflexSpread(water, true);
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");

        editor.TEditorFixtureOpen(water);
        bool first = editor.TEditorFixtureKindred.CKindredRead().CTimbreReflexOpened;
        editor.TEditorFixtureOpen(salt);

        Assert.True(first);
        Assert.False(editor.TEditorFixtureKindred.CKindredRead().CTimbreReflexOpened);
    }

    [Fact]
    public void KindredSpread_HeldEntry_StoresTheStateAnswersTrueAndRaisesTheDisplayFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TReflexOpeningPrepare(engine, "water");
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water);
        List<long> heard = [];
        wing.CWingDisplay.CDisplayFoldChanged += bulletin => heard.Add(bulletin.CBulletinId);
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);

        bool taken = editor.TEditorFixtureKindred.CKindredSpread(true);

        Assert.True(taken);
        Assert.True(engine.TEngineSpreadCheck(water));
        Assert.Equal([water], heard);
        Assert.True(wing.CWingDisplay.CDisplaySound.CDisplayFoldOpened);
        Assert.True(editor.TEditorFixtureKindred.CKindredRead().CTimbreReflexOpened);
    }

    [Fact]
    public void KindredSpread_NoHeldEntry_AnswersFalseAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TReflexOpeningPrepare(engine, "water");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");

        bool taken = editor.TEditorFixtureKindred.CKindredSpread(true);

        Assert.False(taken);
        Assert.False(engine.TEngineSpreadCheck(water));
    }

    [Fact]
    public void KindredSpread_FailingPort_ShowsTheNoticeAndAnswersFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TReflexOpeningPrepare(engine, "water");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);
        List<string> asked = [];
        CKindred kindred = TInterfaceConductSound.TKindredCreate(
            editor,
            TInterfaceConduct.TPhonologyBundleCreate(engine, "LReflexPort.LEngineReflexSpread", true),
            TEngineFake.TEngineStubCreate<LDraftPort>(),
            asked);

        bool taken = kindred.CKindredSpread(true);

        Assert.False(taken);
        Assert.Equal(["Reflex.SpreadFailed"], asked);
        Assert.False(engine.TEngineSpreadCheck(water));
    }

    private static long TReflexOpeningPrepare(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate(headword, "English", string.Empty, string.Empty, [], [])).LEntryId;
    }
}
