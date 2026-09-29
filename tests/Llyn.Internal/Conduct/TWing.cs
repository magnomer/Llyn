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
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        wing.CWingVistaRestore("left");
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
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        wing.CWingVistaRestore("left");

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
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        wing.CWingVistaRestore("right");

        wing.CWingEntryOpen(water.LEntryId);

        Assert.Equal(new CWorkspaceState(null, water.LEntryId), atelier.TAtelierStateOpen());
    }

    [Fact]
    public void WingEntryRestore_StoredEntry_LoadsItAndSavesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TWingWaterSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        wing.CWingVistaRestore("left");
        int loaded = 0;
        wing.CWingLoaded += () => loaded++;

        wing.CWingEntryRestore(water.LEntryId);
        wing.CWingEntryRestore(null);

        Assert.Equal(1, loaded);
        Assert.Equal(water.LEntryId, wing.CWingDisplay.LDisplayChosen);
        Assert.Equal(new CWorkspaceState(null, null), atelier.TAtelierStateOpen());
    }

    [Fact]
    public void WingEntryOpen_LoadThrows_ReportsTheFailureAndRaisesNothing()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        Dictionary<string, Func<object?[]?, object?>> answers = new()
        {
            ["LEngineEntryLoad"] = _ => throw new InvalidOperationException("gone"),
        };
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, answers);
        List<string> asked = [];
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));
        int loaded = 0;
        wing.CWingLoaded += () => loaded++;

        wing.CWingEntryOpen(7);

        Assert.Equal(["Duplex.LoadFailed"], asked);
        Assert.Equal(0, loaded);
    }

    [Fact]
    public void WingRowsRead_NoVista_ReadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TWingWaterSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

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
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        wing.CWingVistaRestore("left");
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
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        wing.CWingVistaRestore("left");
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
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        wing.CWingVistaRestore("left");

        wing.CWingFilterSet(new CCatalogFilter(["English"]));

        Assert.True(wing.CWingFiltered);
        Assert.Equal(["English"], wing.CWingFilter.CCatalogFilterHidden);
    }

    [Fact]
    public void WingEntrySelect_StoredEntry_MarksTheRowWithoutLoading()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TWingWaterSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        wing.CWingVistaRestore("left");
        wing.CWingQuerySet("water");
        int loaded = 0;
        wing.CWingLoaded += () => loaded++;

        wing.CWingEntrySelect(water.LEntryId);

        Assert.Equal(0, loaded);
        Assert.True(wing.CWingRowsRead().Single().CVistaRowChosen);
        Assert.Equal(new CWorkspaceState(null, null), atelier.TAtelierStateOpen());
    }

    private static LEntry TWingWaterSave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }
}
