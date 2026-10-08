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
    public void DisplayEntryOpen_WingLoadsAnEntry_OpensItsHeader()
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
        Assert.Equal("water", wing.CWingDisplay.LDisplayRule.LDisplaySound.LDisplayShown?.LEntryDraftHeadword);
    }

    [Fact]
    public void DisplayEntryOpen_GoneEntry_ClosesTheView()
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
        Assert.Null(wing.CWingDisplay.LDisplayRule.LDisplaySound.LDisplayShown);
    }

    [Fact]
    public void DisplayEntryClose_ShownEntry_DropsTheDraftAndRaisesClosed()
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

        area.CDisplayEntryClose();

        Assert.Equal(1, closed);
        Assert.Empty(area.CDisplayShown.CLecternHeadword);
        Assert.Null(wing.CWingDisplay.LDisplayRule.LDisplaySound.LDisplayShown);
    }

    [Fact]
    public void DisplayPanelAttach_PanelLoadsAndCloses_OpensAndClosesTheView()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry river = TDisplayEntrySave(engine, "river");
        CLibrary library = CLibrary.CLibraryCreate(
            atelier, static () => true, TEnvoyFake.TEnvoyCreate(false, []), static run => run());
        CDisplay area = library.CLibraryEditor.CEditorDisplay;
        area.CDisplayPanelAttach(library.CLibraryPanel);
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
    public void DisplayEntryResonate_ChosenEntry_ReopensItsDraft()
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
    public void DisplayEntryResonate_NothingChosen_OpensAndClosesNothing()
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
    public void DisplayWorkspaceResonate_ShownEntry_ClosesTheView()
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
        Assert.Null(wing.CWingDisplay.LDisplayRule.LDisplaySound.LDisplayShown);
    }

    [Fact]
    public void DisplayVistaAttach_ChosenEntryMarked_RaisesOnlyTheFavoriteChange()
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
        area.CDisplayFavoriteChanged += favorites.Add;
        area.CDisplayGrasp.CDisplayGraspChanged += _ => grasps++;

        area.CDisplayFavoriteToggle(true);

        Assert.Equal(water.LEntryId, Assert.Single(favorites).CBulletinId);
        Assert.Equal(0, grasps);
    }

    [Fact]
    public void DisplayFavoriteToggle_ChosenEntry_AnswersTheStoredMark()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay;
        wing.CWingEntryOpen(water.LEntryId);

        Assert.True(area.CDisplayFavoriteToggle(true));
        Assert.True(area.CDisplayFavoriteRead());
        Assert.False(area.CDisplayFavoriteToggle(false));
    }

    [Fact]
    public void DisplayFavoriteToggle_NoEntryChosen_StoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier);

        Assert.False(wing.CWingDisplay.CDisplayFavoriteToggle(true));
    }

    [Fact]
    public void DisplayFrequencyRead_EntryWithoutFrequency_LooksUpTheOnceKeyAndAnswersNone()
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
