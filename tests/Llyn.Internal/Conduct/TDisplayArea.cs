using System;
using System.Collections.Generic;
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
        CDisplay area = wing.CWingDisplay.CDisplayArea;
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
        Assert.Equal("a *note*", shown.CLecternNote);
        Assert.True(shown.CLecternNoted);
        Assert.True(shown.CLecternStamped);
        Assert.NotEmpty(shown.CLecternAdded);
        Assert.NotEmpty(shown.CLecternUpdated);
        Assert.Equal("water", wing.CWingDisplay.LDisplaySound.LDisplayShown?.LEntryDraftHeadword);
    }

    [Fact]
    public void DisplayEntryOpen_GoneEntry_ClosesTheView()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay.CDisplayArea;
        int opened = 0;
        int closed = 0;
        area.CDisplayOpened += () => opened++;
        area.CDisplayClosed += () => closed++;

        wing.CWingEntryOpen(987654);

        Assert.Equal((0, 1), (opened, closed));
        Assert.Empty(area.CDisplayShown.CLecternHeadword);
        Assert.False(area.CDisplayShown.CLecternStamped);
        Assert.Null(wing.CWingDisplay.LDisplaySound.LDisplayShown);
    }

    [Fact]
    public void DisplayEntryClose_ShownEntry_DropsTheDraftAndRaisesClosed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay.CDisplayArea;
        wing.CWingEntryOpen(water.LEntryId);
        int closed = 0;
        area.CDisplayClosed += () => closed++;

        area.CDisplayEntryClose();

        Assert.Equal(1, closed);
        Assert.Empty(area.CDisplayShown.CLecternHeadword);
        Assert.Null(wing.CWingDisplay.LDisplaySound.LDisplayShown);
    }

    [Fact]
    public void DisplayPanelAttach_PanelLoadsAndCloses_OpensAndClosesTheView()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry river = TDisplayEntrySave(engine, "river");
        CLibrary library = CLibrary.CLibraryCreate(
            atelier, static () => true, TInterfaceConduct.TEnvoyCreate(false, []), static run => run());
        CDisplay area = library.CLibraryEditor.CEditorDisplay.CDisplayArea;
        area.CDisplayPanelAttach(library.CLibraryPanel);
        library.CLibraryVistaRestore();
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
        CDisplay area = wing.CWingDisplay.CDisplayArea;
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
        CDisplay area = wing.CWingDisplay.CDisplayArea;
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
        CDisplay area = wing.CWingDisplay.CDisplayArea;
        wing.CWingEntryOpen(water.LEntryId);
        int closed = 0;
        area.CDisplayClosed += () => closed++;

        area.CDisplayWorkspaceResonate();

        Assert.Equal(1, closed);
        Assert.Null(wing.CWingDisplay.LDisplaySound.LDisplayShown);
    }

    [Fact]
    public void DisplayVistaAttach_ChosenEntryMarked_RaisesOnlyTheFavoriteChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay.CDisplayArea;
        wing.CWingEntryOpen(water.LEntryId);
        List<CBulletin> favorites = [];
        int grasps = 0;
        area.CDisplayFavoriteChanged += favorites.Add;
        area.CDisplayGraspChanged += _ => grasps++;

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
        CDisplay area = wing.CWingDisplay.CDisplayArea;
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

        Assert.False(wing.CWingDisplay.CDisplayArea.CDisplayFavoriteToggle(true));
    }

    [Fact]
    public void DisplayGraspSet_StandingStepPressedAgain_ClearsTheGrasp()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay.CDisplayArea;
        wing.CWingEntryOpen(water.LEntryId);

        CGrasp six = area.CDisplayGraspSet(6);
        Assert.Equal(6, six.CGraspStep);
        Assert.Equal(area.CDisplayGraspRead(6), six.CGraspLabel);
        Assert.Equal(6, wing.CWingDisplay.TDisplayGraspRead(water.LEntryId));

        area.CDisplayGraspSet(3);
        Assert.Equal(3, engine.TEngineGraspRead(water.LEntryId));

        Assert.Equal(0, area.CDisplayGraspSet(3).CGraspStep);
        Assert.Equal(0, engine.TEngineGraspRead(water.LEntryId));
    }

    [Fact]
    public void DisplayGraspSet_NoEntryChosen_StoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayWingPrepare(atelier);
        CDisplay area = wing.CWingDisplay.CDisplayArea;

        CGrasp grasp = area.CDisplayGraspSet(4);

        Assert.Equal(new CGrasp(0, string.Empty), grasp);
        Assert.Empty(area.CDisplayGraspRead(4));
        Assert.Equal(0, engine.TEngineGraspRead(water.LEntryId));
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

        CFrequency? frequency = wing.CWingDisplay.CDisplayArea.CDisplayFrequencyRead(key =>
        {
            keys.Add(key);
            return "once in {0} words";
        });

        Assert.Null(frequency);
        Assert.Equal(["Frequency.Once"], keys);
    }

    [Theory]
    [InlineData(false, "  a tale  ", true)]
    [InlineData(false, " \n ", false)]
    [InlineData(true, "a tale", false)]
    public void DisplayNarrativeCheck_ReadSide_ShowsOnlyWords(bool editable, string text, bool expected)
    {
        Assert.Equal(expected, CDisplay.CDisplayNarrativeCheck(editable, text));
    }

    [Theory]
    [InlineData(true, 0, true)]
    [InlineData(false, 0, false)]
    [InlineData(false, 2, true)]
    public void DisplayEtymonCheck_ReadSide_ShowsOnlyLinks(bool editable, int count, bool expected)
    {
        Assert.Equal(expected, CDisplay.CDisplayEtymonCheck(editable, count));
    }

    private static CAtelier TDisplayAtelierCreate(LEngine engine) =>
        TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineRecordingStop"] = _ => null,
            }));

    private static CWing TDisplayWingPrepare(CAtelier atelier)
    {
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []), true);
        return wing;
    }

    private static LEntry TDisplayEntrySave(LEngine engine, string headword) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a thing", 1)], []));
}
