using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplay
{
    [Fact]
    public void DisplayStampFormat_UnreadableText_ReturnsEmpty()
    {
        Assert.Empty(TInterfaceConduct.TDisplayStampFormat("not a moment"));
        Assert.Empty(TInterfaceConduct.TDisplayStampFormat(null));
    }

    [Fact]
    public void DisplayFoldSet_CurrentValue_KeepsFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDisplaySound sound = TInterfaceConduct.TDisplaySoundCreate(engine);

        sound.TDisplayFoldSet(true);
        sound.TDisplayFoldSet(true);

        Assert.True(sound.LDisplayFoldOpened);
    }

    [Fact]
    public void DisplaySoundClear_ShownDraft_DropsDraftAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDisplaySound sound = TInterfaceConduct.TDisplaySoundCreate(engine);
        sound.TDisplaySoundShow(7, TDisplayDraftCreate([]));

        sound.TDisplaySoundClear();

        Assert.Null(sound.LDisplayShown);
        Assert.Null(sound.LDisplayEntry);
    }

    [Fact]
    public void DisplayReflexRead_LoadedReflex_PrefersIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDisplaySound sound = TInterfaceConduct.TDisplaySoundCreate(engine);
        LEntryDraft draft = TDisplayDraftCreate([TInterface.TReflexDraftCreate("Korean", "", "a")]);
        long id = TExemplar.TExemplarSave(engine, draft)[0];
        sound.TDisplaySoundShow(id, TDisplayDraftCreate([]));

        sound.TDisplayReflexLoad();

        Assert.Equal("a", Assert.Single(sound.TDisplayReflexRead()).LReflexDraftText);
    }

    [Fact]
    public void DisplayFrequencyRead_NoEntry_ReturnsNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);

        Assert.Null(editor.CEditorDisplay.TDisplayFrequencyRead(null, "once in {0} words"));
    }

    [Fact]
    public void DisplayFrequencyRead_EntryWithoutFrequency_ReturnsNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long id = TExemplar.TExemplarSave(engine, TDisplayDraftCreate([]))[0];
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);

        Assert.Null(editor.CEditorDisplay.TDisplayFrequencyRead(id, "once in {0} words"));
    }

    [Fact]
    public void DisplayCompassRead_ShownCards_NamesEachPartAndCardAndNumbersTwins()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);
        LEntryDraft draft = TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate("to light", string.Empty, "a state", [], [], [], [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "a flame", [], [], [], [], [], 2),
                TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnknown, string.Empty, "a spark", [], [], [], [], [], 3),
            ],
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a fire", [], [], [], [], [], 1)]);
        editor.CEditorDisplay.LDisplaySound.TDisplaySoundShow(7, draft);

        IReadOnlyList<CCompassRow> rows = editor.CEditorDisplay.CDisplayCompassRead(
            [CCompassPart.CCompassPartSpeech, CCompassPart.CCompassPartMeaning, CCompassPart.CCompassPartCollocation],
            key => key == "Display.CollocationSingle" ? "Display.MeaningSingle" : key);

        Assert.Equal(
            [
                "Speech.Title",
                "Display.MeaningPlural",
                "to light",
                "Display.MeaningSingle (1)",
                "Display.Unknown",
                "Display.Collocation",
                "Display.MeaningSingle (2)",
            ],
            rows.Select(static row => row.CCompassRowName));
        Assert.Equal(["", "", "1", "2", "3", "", "1"], rows.Select(static row => row.CCompassRowNumber));
        Assert.Equal([0, 0, 1, 1, 1, 0, 1], rows.Select(static row => row.CCompassRowDepth));
        Assert.Equal([null, null, 0, 1, 2, null, 0], rows.Select(static row => row.CCompassRowCard));
        Assert.Equal(CCompassPart.CCompassPartCollocation, rows[^1].CCompassRowPart);
    }

    [Fact]
    public void DisplayCompassRead_NothingShown_NamesOnlyTheParts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);

        IReadOnlyList<CCompassRow> rows = editor.CEditorDisplay.CDisplayCompassRead(
            [CCompassPart.CCompassPartMeaning, CCompassPart.CCompassPartIncoming, CCompassPart.CCompassPartNote],
            static key => "<" + key + ">");

        Assert.Equal(
            ["<Display.MeaningPlural>", "<Display.Translated>", "<Display.Note>"],
            rows.Select(static row => row.CCompassRowName));
        Assert.All(rows, static row => Assert.Null(row.CCompassRowCard));
    }

    [Fact]
    public void DisplayGraspSet_StandingStepPressedAgain_ClearsTheGrasp()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        wing.CWingVistaRestore("left");
        wing.CWingEntryOpen(water.LEntryId);

        wing.CWingDisplay.CDisplayGraspSet(6);
        Assert.Equal(6, wing.CWingDisplay.TDisplayGraspRead(water.LEntryId));

        wing.CWingDisplay.CDisplayGraspSet(3);
        Assert.Equal(3, engine.TEngineGraspRead(water.LEntryId));

        wing.CWingDisplay.CDisplayGraspSet(3);
        Assert.Equal(0, engine.TEngineGraspRead(water.LEntryId));
    }

    [Fact]
    public void DisplayGraspSet_NoEntryChosen_StoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        wing.CWingVistaRestore("left");

        wing.CWingDisplay.CDisplayGraspSet(4);

        Assert.Equal(0, engine.TEngineGraspRead(water.LEntryId));
    }

    private static LEntryDraft TDisplayDraftCreate(IReadOnlyList<LReflexDraft> reflexes) =>
        TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: reflexes);
}
