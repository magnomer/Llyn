using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplayArea
{
    [Fact]
    public void DisplayEntryOpen_WingLoadsEntry_OpensHeader()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            "a *note*",
            [TInterface.TCardCreate("a liquid", 1)],
            [],
            speeches: ["noun"]));
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay;
        int opened = 0;
        int closed = 0;
        area.CDisplayOpened += () => opened++;
        area.CDisplayClosed += () => closed++;

        wing.CWingEntryOpen(water.LEntryId);

        CLectern shown = area.CDisplayShown;
        Assert.Equal((1, 0), (opened, closed));
        Assert.Equal("water", shown.CLecternHeadword);
        Assert.Equal("English", shown.CLecternLanguage);
        Assert.Equal(["Noun"], shown.CLecternSpeeches);
        Assert.True(shown.CLecternMarked);
        Assert.Equal(string.Empty, shown.CLecternUnit);
        CMarkdownBlock note = Assert.Single(shown.CLecternNote);
        Assert.Equal(["a ", "note"], note.CMarkdownBlockSpan.Select(static span => span.CMarkdownSpanText));
        Assert.True(note.CMarkdownBlockSpan[1].CMarkdownSpanItalic);
        Assert.True(shown.CLecternNoted);
        Assert.True(shown.CLecternStamped);
        Assert.NotEmpty(shown.CLecternAdded);
        Assert.NotEmpty(shown.CLecternUpdated);
        Assert.Equal("water", wing.CWingDisplay.LDisplayRule.CVoice.CVoiceShown?.LEntryDraftHeadword);
    }

    [Fact]
    public void DisplayEntryOpen_GoneEntry_ClosesView()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay;
        int opened = 0;
        int closed = 0;
        area.CDisplayOpened += () => opened++;
        area.CDisplayClosed += () => closed++;

        wing.CWingEntryOpen(987654);

        Assert.Equal((0, 1), (opened, closed));
        Assert.Empty(area.CDisplayShown.CLecternHeadword);
        Assert.False(area.CDisplayShown.CLecternStamped);
        Assert.Null(wing.CWingDisplay.LDisplayRule.CVoice.CVoiceShown);
    }

    [Fact]
    public void DisplayEntryClose_ShownEntry_DropsDraftAndRaisesClosed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay;
        Assert.True(area.CDisplayBlank);
        wing.CWingEntryOpen(water.LEntryId);
        Assert.False(area.CDisplayBlank);
        int closed = 0;
        area.CDisplayClosed += () => closed++;

        area.CDisplayEntryClose();

        Assert.True(area.CDisplayBlank);
        Assert.Equal(1, closed);
        Assert.Empty(area.CDisplayShown.CLecternHeadword);
        Assert.Null(wing.CWingDisplay.LDisplayRule.CVoice.CVoiceShown);
    }

    [Fact]
    public void DisplayPanelAttach_PanelOpenAndClose_OpensAndClosesView()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry river = TDisplayEntrySave(engine, "river");
        CLibrary library = CLibrary.CLibraryCreate(
            atelier, static () => true, TEnvoyFake.TEnvoyCreate(false, []), static run => run());
        CDisplay area = library.CLibraryEditor.CEditorDisplay;
        library.TLibraryVistaRestore();
        int opened = 0;
        int closed = 0;
        area.CDisplayOpened += () => opened++;
        area.CDisplayClosed += () => closed++;

        library.CLibraryPanel.CPanelRowOpen(river.LEntryId);

        Assert.Equal((1, 0), (opened, closed));
        Assert.Equal("river", area.CDisplayShown.CLecternHeadword);

        library.CLibraryPanel.CPanelEntryClose();

        Assert.Equal((1, 1), (opened, closed));
        Assert.Empty(area.CDisplayShown.CLecternHeadword);
    }

    [Fact]
    public void DisplayEntryResonate_ChosenEntry_ReopensDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay;
        wing.CWingEntryOpen(water.LEntryId);
        int opened = 0;
        area.CDisplayOpened += () => opened++;

        area.CDisplayEntryResonate();

        Assert.Equal(1, opened);
        Assert.Equal("water", area.CDisplayShown.CLecternHeadword);
    }

    [Fact]
    public void DisplayEntryResonate_NothingChosen_RaisesNoEvents()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay;
        int changed = 0;
        area.CDisplayOpened += () => changed++;
        area.CDisplayClosed += () => changed++;

        area.CDisplayEntryResonate();

        Assert.Equal(0, changed);
    }

    [Fact]
    public void DisplayWorkspaceResonate_ShownEntry_ClosesView()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay;
        wing.CWingEntryOpen(water.LEntryId);
        int closed = 0;
        area.CDisplayClosed += () => closed++;

        area.CDisplayWorkspaceResonate();

        Assert.Equal(1, closed);
        Assert.Null(wing.CWingDisplay.LDisplayRule.CVoice.CVoiceShown);
    }

    [Fact]
    public void DisplayFavoriteToggle_MarkedEntry_RaisesFavoriteOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay;
        wing.CWingEntryOpen(water.LEntryId);
        List<CBulletin> favorites = [];
        int grasps = 0;
        area.CDisplayGrasp.CDisplayFavoriteChanged += favorites.Add;
        area.CDisplayGrasp.CDisplayGraspChanged += _ => grasps++;

        area.CDisplayGrasp.CDisplayFavoriteToggle();

        Assert.Equal(water.LEntryId, Assert.Single(favorites).CBulletinId);
        Assert.Equal(0, grasps);
    }

    [Fact]
    public void DisplayFavoriteToggle_ChosenEntry_AnswersStoredMark()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay;
        wing.CWingEntryOpen(water.LEntryId);

        Assert.True(area.CDisplayGrasp.CDisplayFavoriteToggle());
        Assert.True(area.CDisplayGrasp.CDisplayFavoriteRead());
        Assert.False(area.CDisplayGrasp.CDisplayFavoriteToggle());
        Assert.False(area.CDisplayGrasp.CDisplayFavoriteRead());
    }

    [Fact]
    public void DisplayFavoriteToggle_NoEntryChosen_StoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier);

        Assert.False(wing.CWingDisplay.CDisplayGrasp.CDisplayFavoriteToggle());
    }

    [Fact]
    public void DisplayFrequencyRead_NoFrequency_LooksUpOnceKey()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayWingPrepare(atelier);
        wing.CWingEntryOpen(water.LEntryId);
        List<string> keys = [];

        CFrequency? frequency = wing.CWingDisplay.CDisplayFrequencyRead(key =>
        {
            keys.Add(key);
            return "once in {0} words";
        });

        Assert.Null(frequency);
        Assert.Equal(["Frequency.Once"], keys);
    }

    [Fact]
    public void CompassRead_HiddenPartSharesCardLabel_NamesShownRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry lamp = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "lamp",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate("Speech.Title", string.Empty, "a light", [], [], [], [], [], 1)],
            []));
        CWing wing = TDisplayWingPrepare(atelier);
        wing.CWingEntryOpen(lamp.LEntryId);
        wing.CWingDisplay.CDisplayCard.CDisplayCardRead();

        IReadOnlyList<CCompassRow> rows = wing.CWingDisplay.CDisplayCompass.CCompassRead(static key => key);

        Assert.Equal(["Display.MeaningPlural", "Speech.Title"], rows.Select(static row => row.CCompassRowName));
        Assert.Equal(
            [CCompassPart.CCompassPartMeaning, CCompassPart.CCompassPartMeaning],
            rows.Select(static row => row.CCompassRowPart));
    }

    [Fact]
    public void CompassShown_EntryWithSomeParts_KeepsPartsInPageOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry kindle = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            "a note",
            [],
            [TInterface.TCardCreate("a fire", 1)],
            speeches: ["verb"]));
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay;

        wing.CWingEntryOpen(kindle.LEntryId);
        area.CDisplayCard.CDisplayCardRead();
        area.CDisplayCard.CDisplayIncomingRead();
        area.CDisplayFrequencyRead(static key => key);

        Assert.Equal(
            [
                CCompassPart.CCompassPartSpeech,
                CCompassPart.CCompassPartCollocation,
                CCompassPart.CCompassPartNote,
                CCompassPart.CCompassPartStamp,
            ],
            area.CDisplayCompass.CCompassShown);
        area.CDisplayEntryClose();
        area.CDisplayCard.CDisplayCardRead();
        Assert.Empty(area.CDisplayCompass.CCompassShown);
    }

    [Fact]
    public void CompassShown_OtherEntryOpened_DropsPreviousParts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry kindle = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            "a note",
            [],
            [TInterface.TCardCreate("a fire", 1)],
            speeches: ["verb"]));
        LEntry bare = TDisplayEntrySave(engine, "bare");
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay;
        wing.CWingEntryOpen(kindle.LEntryId);
        area.CDisplayCard.CDisplayCardRead();
        area.CDisplayCard.CDisplayIncomingRead();
        area.CDisplayFrequencyRead(static key => key);

        wing.CWingEntryOpen(bare.LEntryId);

        Assert.DoesNotContain(CCompassPart.CCompassPartSpeech, area.CDisplayCompass.CCompassShown);
        Assert.DoesNotContain(CCompassPart.CCompassPartCollocation, area.CDisplayCompass.CCompassShown);
    }

    internal static CAtelier TDisplayAtelierCreate(LEngine engine) =>
        TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineRecordingStop"] = _ => null,
            }));

    internal static CWing TDisplayWingPrepare(CAtelier atelier)
    {
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        return wing;
    }

    internal static LEntry TDisplayEntrySave(LEngine engine, string headword) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a thing", 1)], []));
}
