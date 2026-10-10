using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TFold
{
    [Fact]
    public void FoldOpened_AnotherEntryHeld_ReadsEachHeldEntrysOwnStoredState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TFoldEntrySave(engine, "water");
        long salt = TFoldEntrySave(engine, "salt");
        engine.TEngineBoxSpread(water, LFoldBox.LFoldBoxFanqie, true);
        engine.TEngineBoxSpread(salt, LFoldBox.LFoldBoxScript, true);
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");

        editor.TEditorFixtureOpen(water);
        (bool, bool) first =
            (editor.TEditorFixtureFold.CFoldFanqieOpened, editor.TEditorFixtureFold.CFoldScriptOpened);
        editor.TEditorFixtureOpen(salt);

        Assert.Equal((true, false), first);
        Assert.False(editor.TEditorFixtureFold.CFoldFanqieOpened);
        Assert.True(editor.TEditorFixtureFold.CFoldScriptOpened);
    }

    [Fact]
    public void FoldSpread_HeldEntry_StoresBothBoxesAnswersTrueAndRefreshesTheEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TFoldEntrySave(engine, "water");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);
        int shown = 0;
        editor.TEditorFixtureEntry.CEntryDraftChanged += _ => shown++;

        bool fanqie = editor.TEditorFixtureFold.CFoldFanqieSpread(true);
        bool script = editor.TEditorFixtureFold.CFoldScriptSpread(true);

        Assert.True(fanqie);
        Assert.True(script);
        Assert.Equal(2, shown);
        Assert.True(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.True(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
        Assert.True(editor.TEditorFixtureFold.CFoldFanqieOpened);
        Assert.True(editor.TEditorFixtureFold.CFoldScriptOpened);

        editor.TEditorFixtureFold.CFoldFanqieSpread(false);

        Assert.False(editor.TEditorFixtureFold.CFoldFanqieOpened);
        Assert.True(editor.TEditorFixtureFold.CFoldScriptOpened);
    }

    [Fact]
    public void FoldSpread_NoStoredEntryHeld_AnswersFalseAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TFoldEntrySave(engine, "water");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(null);

        bool fanqie = editor.TEditorFixtureFold.CFoldFanqieSpread(true);
        bool script = editor.TEditorFixtureFold.CFoldScriptSpread(true);

        Assert.False(fanqie);
        Assert.False(script);
        Assert.False(editor.TEditorFixtureFold.CFoldFanqieOpened);
        Assert.False(editor.TEditorFixtureFold.CFoldScriptOpened);
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
    }

    [Fact]
    public void FoldSpread_FailingPort_ShowsTheNoticeAnswersFalseAndTellsNoOtherEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TFoldEntrySave(engine, "water");
        engine.TEngineBoxSpread(water, LFoldBox.LFoldBoxScript, true);
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);
        TEditorFixture second = TEditor.TEditorPrepare(engine, "library");
        second.TEditorFixtureOpen(water);
        int shown = 0;
        second.TEditorFixtureEntry.CEntryDraftChanged += _ => shown++;
        List<string> asked = [];
        CFold fold = TInterfaceConductSound.TFoldCreate(
            editor, TInterfaceConduct.TPhonologyBundleCreate(engine, "LReflexPort.LEngineBoxSpread", true), asked);

        bool fanqie = fold.CFoldFanqieSpread(true);
        bool script = fold.CFoldScriptSpread(false);

        Assert.False(fanqie);
        Assert.False(script);
        Assert.Equal(["Box.SpreadFailed", "Box.SpreadFailed"], asked);
        Assert.Equal(0, shown);
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.True(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
        Assert.False(fold.CFoldFanqieOpened);
        Assert.True(fold.CFoldScriptOpened);
    }

    [Fact]
    public void FoldOpened_FaultingPort_AnswersFoldedAndShowsTheReadNoticeOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TFoldEntrySave(engine, "water");
        engine.TEngineBoxSpread(water, LFoldBox.LFoldBoxFanqie, true);
        engine.TEngineBoxSpread(water, LFoldBox.LFoldBoxScript, true);
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);
        List<string> asked = [];
        CFold fold = TInterfaceConductSound.TFoldCreate(
            editor, TInterfaceConduct.TPhonologyBundleCreate(engine, "LReflexPort.LEngineBoxCheck", true), asked);

        bool fanqie = fold.CFoldFanqieOpened;
        bool script = fold.CFoldScriptOpened;

        Assert.False(fanqie);
        Assert.False(script);
        Assert.Equal(["Box.SpreadReadFailed"], asked);
        Assert.True(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
    }

    [Fact]
    public void FoldSpread_TwoEntriesAndTwoBoxes_KeepsEachStateApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TFoldEntrySave(engine, "water");
        long salt = TFoldEntrySave(engine, "salt");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);
        editor.TEditorFixtureFold.CFoldFanqieSpread(true);
        editor.TEditorFixtureOpen(salt);

        editor.TEditorFixtureFold.CFoldScriptSpread(true);

        Assert.True(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
        Assert.False(engine.TEngineBoxCheck(salt, LFoldBox.LFoldBoxFanqie));
        Assert.True(engine.TEngineBoxCheck(salt, LFoldBox.LFoldBoxScript));
    }

    [Fact]
    public void FoldSpread_FreshEditorOnTheSameEntry_ReadsTheStoredState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TFoldEntrySave(engine, "water");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);
        editor.TEditorFixtureFold.CFoldFanqieSpread(true);
        editor.TEditorFixtureFold.CFoldScriptSpread(true);

        TEditorFixture fresh = TEditor.TEditorPrepare(engine, "library");
        fresh.TEditorFixtureOpen(water);

        Assert.True(fresh.TEditorFixtureFold.CFoldFanqieOpened);
        Assert.True(fresh.TEditorFixtureFold.CFoldScriptOpened);
    }

    [Fact]
    public void FoldSpread_SecondEditorOnTheSameEntry_RefreshesThatEditorFromTheStore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TFoldEntrySave(engine, "water");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);
        TEditorFixture second = TEditor.TEditorPrepare(engine, "library");
        second.TEditorFixtureOpen(water);
        int shown = 0;
        second.TEditorFixtureEntry.CEntryDraftChanged += _ => shown++;

        editor.TEditorFixtureFold.CFoldFanqieSpread(true);
        editor.TEditorFixtureFold.CFoldScriptSpread(true);

        Assert.Equal(2, shown);
        Assert.True(second.TEditorFixtureFold.CFoldFanqieOpened);
        Assert.True(second.TEditorFixtureFold.CFoldScriptOpened);
    }

    [Fact]
    public void FoldSpread_SecondEditorClosed_RefreshesNothingThere()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TFoldEntrySave(engine, "water");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);
        TEditorFixture second = TEditor.TEditorPrepare(engine, "library");
        second.TEditorFixtureOpen(water);
        int shown = 0;
        second.TEditorFixtureEntry.CEntryDraftChanged += _ => shown++;

        second.TEditorFixtureClose();
        editor.TEditorFixtureFold.CFoldFanqieSpread(true);

        Assert.Equal(0, shown);
        Assert.False(second.TEditorFixtureFold.CFoldFanqieOpened);
    }

    [Fact]
    public void DisplayBoxOpened_AnotherEntryShown_ReadsEachShownEntrysOwnStoredState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldEntrySave(engine, "water");
        long salt = TFoldEntrySave(engine, "salt");
        engine.TEngineBoxSpread(water, LFoldBox.LFoldBoxFanqie, true);
        engine.TEngineBoxSpread(salt, LFoldBox.LFoldBoxScript, true);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);

        wing.CWingEntryOpen(water);
        (bool TFanqie, bool TScript) first = (
            wing.CWingDisplay.CDisplayFold.CFoldFanqieOpened,
            wing.CWingDisplay.CDisplayFold.CFoldScriptOpened);
        wing.CWingEntryOpen(salt);

        Assert.Equal((true, false), first);
        Assert.False(wing.CWingDisplay.CDisplayFold.CFoldFanqieOpened);
        Assert.True(wing.CWingDisplay.CDisplayFold.CFoldScriptOpened);
    }

    [Fact]
    public void DisplayBoxSpread_ShownEntry_StoresBothBoxesAnswersTrueAndRaisesTheDisplayFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldEntrySave(engine, "water");
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water);
        List<long> heard = [];
        wing.CWingDisplay.CDisplayFoldChanged += bulletin => heard.Add(bulletin.CBulletinId);

        bool fanqie = wing.CWingDisplay.CDisplayFold.CFoldFanqieSpread(true);
        bool script = wing.CWingDisplay.CDisplayFold.CFoldScriptSpread(true);

        Assert.True(fanqie);
        Assert.True(script);
        Assert.Equal([water, water], heard);
        Assert.True(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.True(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
        Assert.True(wing.CWingDisplay.CDisplayFold.CFoldFanqieOpened);
        Assert.True(wing.CWingDisplay.CDisplayFold.CFoldScriptOpened);
    }

    [Fact]
    public void DisplayBoxSpread_NoEntryShown_AnswersFalseAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldEntrySave(engine, "water");
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);

        bool fanqie = wing.CWingDisplay.CDisplayFold.CFoldFanqieSpread(true);
        bool script = wing.CWingDisplay.CDisplayFold.CFoldScriptSpread(true);

        Assert.False(fanqie);
        Assert.False(script);
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
        Assert.False(wing.CWingDisplay.CDisplayFold.CFoldFanqieOpened);
        Assert.False(wing.CWingDisplay.CDisplayFold.CFoldScriptOpened);
    }

    [Fact]
    public void DisplayBoxSpread_FailingPort_ShowsTheNoticeAndAnswersFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TFoldEntrySave(engine, "water");
        List<string> asked = [];
        CDisplay display = TInterfaceConductSound.TDisplayChosenCreate(
            engine,
            TInterfaceConduct.TEntryBundleCreate(engine),
            water,
            TInterfaceConduct.TPhonologyBundleCreate(engine, "LReflexPort.LEngineBoxSpread", true),
            asked);
        display.LDisplayRule.LDisplaySound.TDisplaySoundShow(water, engine.TEngineEntryLoad(water)!);

        bool fanqie = display.CDisplayFold.CFoldFanqieSpread(true);
        bool script = display.CDisplayFold.CFoldScriptSpread(true);

        Assert.False(fanqie);
        Assert.False(script);
        Assert.Equal(["Box.SpreadFailed", "Box.SpreadFailed"], asked);
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
    }

    [Fact]
    public void DisplayBoxSpread_EditorOnTheSameEntry_RefreshesTheEditorAndReadsBackThere()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldEntrySave(engine, "water");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);
        int shown = 0;
        editor.TEditorFixtureEntry.CEntryDraftChanged += _ => shown++;
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water);

        wing.CWingDisplay.CDisplayFold.CFoldFanqieSpread(true);
        wing.CWingDisplay.CDisplayFold.CFoldScriptSpread(true);

        Assert.Equal(2, shown);
        Assert.True(editor.TEditorFixtureFold.CFoldFanqieOpened);
        Assert.True(editor.TEditorFixtureFold.CFoldScriptOpened);
    }

    [Fact]
    public void FoldSpread_DisplayOnTheSameEntry_RaisesTheDisplayFoldAndReadsBackThere()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldEntrySave(engine, "water");
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water);
        wing.CWingDisplay.CDisplayFold.CFoldFanqieSpread(true);
        List<long> heard = [];
        wing.CWingDisplay.CDisplayFoldChanged += bulletin => heard.Add(bulletin.CBulletinId);
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water);

        editor.TEditorFixtureFold.CFoldFanqieSpread(false);
        editor.TEditorFixtureFold.CFoldScriptSpread(true);

        Assert.Equal([water, water], heard);
        Assert.False(wing.CWingDisplay.CDisplayFold.CFoldFanqieOpened);
        Assert.True(wing.CWingDisplay.CDisplayFold.CFoldScriptOpened);
    }

    [Fact]
    public void DisplayBoxSpread_TwoEntriesAndTwoBoxes_KeepsEachStateApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldEntrySave(engine, "water");
        long salt = TFoldEntrySave(engine, "salt");
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water);
        wing.CWingDisplay.CDisplayFold.CFoldFanqieSpread(true);
        wing.CWingEntryOpen(salt);

        wing.CWingDisplay.CDisplayFold.CFoldScriptSpread(true);

        Assert.True(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
        Assert.False(engine.TEngineBoxCheck(salt, LFoldBox.LFoldBoxFanqie));
        Assert.True(engine.TEngineBoxCheck(salt, LFoldBox.LFoldBoxScript));
    }

    private static long TFoldEntrySave(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], [])).LEntryId;
    }
}
