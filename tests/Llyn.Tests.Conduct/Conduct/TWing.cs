using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TWing
{
    [Fact]
    public void WingEntryOpen_LeftSide_LoadsTheEntryAndSavesTheLeftSlot()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TWingWaterSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        int loaded = 0;
        wing.CWingLoaded += () => loaded++;

        wing.CWingEntryOpen(water.LEntryId);

        Assert.Equal(1, loaded);
        Assert.Equal(water.LEntryId, wing.CWingDisplay.LDisplayChosen);
        Assert.Equal(new CWorkspaceState(water.LEntryId, null), atelier.TAtelierStateOpen());
    }

    [Fact]
    public void WingDisplayIncomingRead_EntryPointedAt_AnswersTheCitingCardsInTheirShape()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry target = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("eau", "French", "", "", [], []));
        LCardDraft card = TInterface.TCardCreate("liquid", 1) with { LCardDraftTranslation = [target.LEntryId] };
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [card], []));
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);

        wing.CWingEntryOpen(target.LEntryId);

        CUsage usage = Assert.Single(wing.CWingDisplay.CDisplayArea.CDisplayIncomingRead());
        Assert.Equal(water.LEntryId, usage.CUsageEntry);
        Assert.Equal("water", usage.CUsageName);
        Assert.Equal("English", usage.CUsageLanguage);
        Assert.Equal("Display.MeaningSingle", usage.CUsageOwnerKey);
    }

    [Fact]
    public void WingEntryOpen_RightSide_SavesTheRightSlot()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TWingWaterSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), false);

        wing.CWingEntryOpen(water.LEntryId);

        Assert.Equal(new CWorkspaceState(null, water.LEntryId), atelier.TAtelierStateOpen());
    }

    [Fact]
    public void WingEntryRestore_WorkspaceOpened_LoadsTheSideEntryAndSavesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TWingWaterSave(engine);
        CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true).CWingEntryOpen(water.LEntryId);
        CWing left = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        CWing right = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), false);
        int loaded = 0;
        int blank = 0;
        left.CWingLoaded += () => loaded++;
        right.CWingLoaded += () => blank++;

        CWorkspaceState? opened = atelier.TAtelierStateOpen();

        Assert.Equal(new CWorkspaceState(water.LEntryId, null), opened);
        Assert.Equal(1, loaded);
        Assert.Equal(0, blank);
        Assert.Equal(water.LEntryId, left.CWingDisplay.LDisplayChosen);
        Assert.Null(right.CWingDisplay.LDisplayChosen);
        Assert.Equal(opened, atelier.TAtelierStateOpen());
    }

    [Fact]
    public void WingEntryRestore_WorkspaceOpened_ClosesTheShownEntryBeforeLoading()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TWingWaterSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water.LEntryId);
        List<string> heard = [];
        wing.CWingDisplay.CDisplayArea.CDisplayClosed += () => heard.Add("Closed");
        wing.CWingLoaded += () => heard.Add("Loaded");

        atelier.TAtelierStubOpen();

        Assert.Equal(["Closed", "Loaded"], heard);
    }

    [Fact]
    public void WingEntryOpen_NoRow_AnswersFalseAndLoadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        int loaded = 0;
        wing.CWingLoaded += () => loaded++;

        Assert.False(wing.CWingEntryOpen(null));
        Assert.Equal(0, loaded);
        Assert.Equal(new CWorkspaceState(null, null), atelier.TAtelierStateOpen());
    }

    [Fact]
    public void WingVistaRestore_WorkspaceChanged_StartsAFreshVista()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), _ => { });
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingQuerySet("water");
        bool held = wing.CWingQueried;

        atelier.CAtelierWorkspace.CWorkspaceChange(second.TWorkspaceFolder, TEngineFake.TEngineStubCreate<CEnvoy>());

        Assert.True(held);
        Assert.False(wing.CWingQueried);
    }

    [Fact]
    public void WingClose_AtelierClosed_StopsThePlayback()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        int stopped = 0;
        LMediaPort media = TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineRecordingStop"] = _ =>
            {
                stopped++;
                return null;
            },
        });
        using CAtelier atelier = TInterfaceConduct.TAtelierMediaCreate(engine, media);
        CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);

        atelier.CAtelierClose();

        Assert.Equal(1, stopped);
    }

    [Fact]
    public void WingEntryOpen_LoadThrows_ReportsTheFailureAndRaisesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        Dictionary<string, Func<object?[]?, object?>> answers = new()
        {
            ["LEngineEntryLoad"] = _ => throw new InvalidOperationException("gone"),
        };
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, answers);
        List<string> asked = [];
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);
        int loaded = 0;
        wing.CWingLoaded += () => loaded++;

        wing.CWingEntryOpen(7);

        Assert.Equal(["Duplex.LoadFailed"], asked);
        Assert.Equal(0, loaded);
    }

    [Fact]
    public void WingRowsRead_NothingTyped_ReadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TWingWaterSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);

        Assert.Empty(wing.CWingRowsRead());
        Assert.False(wing.CWingQueried);
        Assert.Equal(CCatalogOrder.CCatalogOrderHeadword, wing.CWingOrder);
    }

    [Fact]
    public void WingQuerySet_MatchingText_ListsTheMatchAndAnnounces()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TWingWaterSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        int changed = 0;
        wing.CWingChanged += _ => changed++;

        wing.CWingQuerySet("water");

        Assert.True(wing.CWingQueried);
        Assert.True(changed > 0);
        Assert.Equal(["water"], wing.CWingRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void WingOrderSet_NoOrder_KeepsTheOrdering()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingOrderSet(CCatalogOrder.CCatalogOrderLanguage);

        wing.CWingOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderLanguage, wing.CWingOrder);
    }

    [Fact]
    public void WingFilterSet_HiddenLanguage_MarksTheSideFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);

        wing.CWingFilterSet(new CCatalogFilter(["English"]));

        Assert.True(wing.CWingFiltered);
        Assert.Equal(["English"], wing.CWingFilter.CCatalogFilterHidden);
    }

    [Fact]
    public void WingRowMove_ListedEntry_MarksTheRowWithoutLoading()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TWingWaterSave(engine);
        CWing wing = TWingQueryPrepare(atelier, "water");
        int loaded = 0;
        int changed = 0;
        wing.CWingLoaded += () => loaded++;
        wing.CWingRowsChanged += () => changed++;

        long? moved = wing.CWingRowMove(true);

        Assert.Equal(water.LEntryId, moved);
        Assert.Equal(0, loaded);
        Assert.Equal(1, changed);
        Assert.True(wing.CWingRowsRead().Single().CVistaRowChosen);
        Assert.Equal(new CWorkspaceState(null, null), atelier.TAtelierStateOpen());
    }

    [Fact]
    public void WingRowMove_NoChosenDown_PicksTheFirstRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        IReadOnlyList<LEntry> trio = TWingTrioSave(engine);
        CWing wing = TWingQueryPrepare(atelier, "water");

        Assert.Equal(trio[0].LEntryId, wing.CWingRowMove(true));
    }

    [Fact]
    public void WingRowMove_NoChosenUp_PicksTheFirstRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        IReadOnlyList<LEntry> trio = TWingTrioSave(engine);
        CWing wing = TWingQueryPrepare(atelier, "water");

        Assert.Equal(trio[0].LEntryId, wing.CWingRowMove(false));
    }

    [Fact]
    public void WingRowMove_MiddleChosen_MovesOneRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        IReadOnlyList<LEntry> trio = TWingTrioSave(engine);
        CWing wing = TWingQueryPrepare(atelier, "water");
        wing.CWingRowMove(true);
        wing.CWingRowMove(true);

        Assert.Equal(trio[2].LEntryId, wing.CWingRowMove(true));
        Assert.Equal(trio[1].LEntryId, wing.CWingRowMove(false));
        Assert.Equal(trio[0].LEntryId, wing.CWingRowMove(false));
    }

    [Fact]
    public void WingRowMove_EndChosen_StaysAtTheEnd()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        IReadOnlyList<LEntry> trio = TWingTrioSave(engine);
        CWing wing = TWingQueryPrepare(atelier, "water");
        wing.CWingRowMove(true);
        wing.CWingRowMove(true);
        wing.CWingRowMove(true);

        Assert.Equal(trio[2].LEntryId, wing.CWingRowMove(true));
        wing.CWingRowMove(false);
        wing.CWingRowMove(false);
        Assert.Equal(trio[0].LEntryId, wing.CWingRowMove(false));
        Assert.Equal(trio[0].LEntryId, Assert.Single(wing.CWingRowsRead(), row => row.CVistaRowChosen).CVistaRowId);
    }

    [Fact]
    public void WingRowMove_EmptyList_AnswersNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TWingWaterSave(engine);
        CWing wing = TWingQueryPrepare(atelier, "fire");
        int changed = 0;
        wing.CWingRowsChanged += () => changed++;

        Assert.Null(wing.CWingRowMove(true));
        Assert.Equal(0, changed);
    }

    [Fact]
    public void WingEmpty_QueryMatchingNothing_AnswersEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TWingWaterSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingRowsRead();
        bool unasked = wing.CWingEmpty;

        wing.CWingQuerySet("fire");
        wing.CWingRowsRead();
        bool unmatched = wing.CWingEmpty;
        wing.CWingQuerySet("water");
        wing.CWingRowsRead();

        Assert.False(unasked);
        Assert.True(unmatched);
        Assert.False(wing.CWingEmpty);
    }

    private static CWing TWingQueryPrepare(CAtelier atelier, string query)
    {
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingQuerySet(query);
        wing.CWingRowsRead();
        return wing;
    }

    private static IReadOnlyList<LEntry> TWingTrioSave(LEngine engine)
    {
        return [.. new[] { "water", "waterfall", "waterway" }.Select(headword => engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate(
                headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a liquid", 1)], [])))];
    }

    private static LEntry TWingWaterSave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }
}
