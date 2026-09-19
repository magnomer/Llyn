using System.Collections.Generic;
using System.IO;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPosture
{
    [Fact]
    public void OrderSave_EveryBrowsePanel_KeepsEachOrderingApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();

        posture.TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderRecent);
        posture.TPostureVistaStart("phonology", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderSound);
        posture.TPostureVistaStart("favorite", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderMarked);
        posture.TPostureVistaStart("taxonomy", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderUsage);
        posture.TPostureVistaStart("tenor", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderReverse);

        IReadOnlyList<LLayout> layout = posture.TPostureRead().LPostureStateLayout ?? [];

        Assert.Equal(LCatalogOrder.LCatalogOrderRecent, TPostureOrderRead(layout, "library"));
        Assert.Equal(LCatalogOrder.LCatalogOrderSound, TPostureOrderRead(layout, "phonology"));
        Assert.Equal(LCatalogOrder.LCatalogOrderMarked, TPostureOrderRead(layout, "favorite"));
        Assert.Equal(LCatalogOrder.LCatalogOrderUsage, TPostureOrderRead(layout, "taxonomy"));
        Assert.Equal(LCatalogOrder.LCatalogOrderReverse, TPostureOrderRead(layout, "tenor"));
    }

    [Fact]
    public void FilterSave_TwoPanels_KeepsEachFilterApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();

        posture.TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("Korean"));
        posture.TPostureVistaStart("corpus", LCatalogOrder.LCatalogOrderName)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("Chinese"));

        IReadOnlyList<LLayout> layout = posture.TPostureRead().LPostureStateLayout ?? [];

        Assert.Equal(["Korean"], TPostureFilterRead(layout, "library"));
        Assert.Equal(["Chinese"], TPostureFilterRead(layout, "corpus"));
    }

    [Fact]
    public void OrderSave_WorkspaceReopened_RestartsVistaOnChosenOrdering()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LPosture posture = engine.TPostureStart();
            LVista vista = posture.TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
            vista.TVistaOrderSet(LCatalogOrder.LCatalogOrderReverse);
            vista.TVistaFilterSet(TInterface.TCatalogFilterCreate("Korean", "French"));
            vista.TVistaEditingSet(true);
            posture.TPostureModeSave("Corpus");
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        LPosture again = reopened.TPostureStart();
        LVista restarted = again.TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        Assert.Equal(LCatalogOrder.LCatalogOrderReverse, restarted.LVistaOrder);
        Assert.Equal(["Korean", "French"], restarted.LVistaFilter.LCatalogFilterHidden);
        Assert.True(restarted.LVistaEditing);
        Assert.Equal("Corpus", again.TPostureRead().LPostureStateMode);
        Assert.True(again.TPostureRead().LPostureStateSplit);
    }

    [Fact]
    public void OrderSave_AfterLayoutSave_KeepsDraggedWidth()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();

        posture.TPostureLayoutSave(TInterface.TLayoutCreate("tenor", 320, 300));
        LVista vista = posture.TPostureVistaStart("tenor", LCatalogOrder.LCatalogOrderName);
        vista.TVistaOrderSet(LCatalogOrder.LCatalogOrderUsage);
        vista.TVistaFilterSet(TInterface.TCatalogFilterCreate("Korean"));
        posture.TPostureLayoutSave(TInterface.TLayoutCreate("tenor", 340, 300));

        LLayout tenor = Assert.Single(posture.TPostureRead().LPostureStateLayout ?? []);

        Assert.Equal(340, tenor.LLayoutLeft);
        Assert.Equal(300, tenor.LLayoutMiddle);
        Assert.Equal(LCatalogOrder.LCatalogOrderUsage, tenor.LLayoutOrder);
        Assert.Equal(["Korean"], tenor.LLayoutFilter?.LCatalogFilterHidden);
    }

    [Fact]
    public void LayoutSave_SecondTab_KeepsFirstTab()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();

        posture.TPostureLayoutSave(TInterface.TLayoutCreate("library", 400));
        posture.TPostureLayoutSave(TInterface.TLayoutCreate("corpus", 390), TInterface.TLayoutCreate("library", 410));

        IReadOnlyList<LLayout> layout = posture.TPostureRead().LPostureStateLayout ?? [];

        Assert.Equal(2, layout.Count);
        Assert.Contains(layout, tab => tab.LLayoutTab == "library" && tab.LLayoutLeft == 410);
        Assert.Contains(layout, tab => tab.LLayoutTab == "corpus" && tab.LLayoutLeft == 390);
    }

    [Fact]
    public void LayoutReset_ClearsStoredWidths()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();

        posture.TPostureLayoutSave(TInterface.TLayoutCreate("tenor", 320, 300));
        posture.TPostureVistaStart("tenor", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderUsage);
        posture.TPostureLayoutReset();

        LLayout tenor = Assert.Single(posture.TPostureRead().LPostureStateLayout ?? []);

        Assert.Null(tenor.LLayoutLeft);
        Assert.Null(tenor.LLayoutMiddle);
        Assert.Equal(LCatalogOrder.LCatalogOrderUsage, tenor.LLayoutOrder);
    }

    [Fact]
    public void QuerySet_Typed_WritesNoFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        LVista vista = posture.TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
        string path = Path.Combine(workspace.TWorkspaceFolder, "posture.json");
        DateTime written = File.GetLastWriteTimeUtc(path);
        File.SetLastWriteTimeUtc(path, written.AddMinutes(-5));

        vista.TVistaQuerySet("water");
        vista.TVistaSelect(null);

        Assert.Equal(written.AddMinutes(-5), File.GetLastWriteTimeUtc(path));
        Assert.Empty(posture.TPostureRead().LPostureStateLayout ?? []);
    }

    [Fact]
    public void WindowSave_SameGeometryTwice_WritesFileOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        string path = Path.Combine(workspace.TWorkspaceFolder, "posture.json");

        posture.TPostureWindowSave(TInterface.TWindowStateCreate(10, 20, 800, 600, false));
        DateTime written = File.GetLastWriteTimeUtc(path);
        File.SetLastWriteTimeUtc(path, written.AddMinutes(-5));

        posture.TPostureWindowSave(TInterface.TWindowStateCreate(10, 20, 800, 600, false));

        Assert.Equal(written.AddMinutes(-5), File.GetLastWriteTimeUtc(path));
    }

    [Fact]
    public void WindowSave_WorkspaceReopened_KeepsGeometryAndVolume()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LPosture posture = engine.TPostureStart();
            posture.TPostureWindowSave(TInterface.TWindowStateCreate(10, 20, 640, 480, true));
            posture.TPostureVolumeSave(0.25);
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        LPostureState state = reopened.TPostureStart().TPostureRead();

        Assert.Equal(640, state.LPostureStateWindow?.LWindowStateWidth);
        Assert.True(state.LPostureStateWindow?.LWindowStateMaximized);
        Assert.Equal(0.25, state.LPostureStateVolume);
        Assert.False(File.Exists(Path.Combine(workspace.TWorkspaceFolder, "posture.json.tmp")));
    }

    [Fact]
    public void LinkedSave_False_KeepsLayoutAndReportsChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();

        posture.TPostureLayoutSave(TInterface.TLayoutCreate("tenor", 320, 300));

        Assert.True(posture.TPostureLinkedSave(false));
        Assert.False(posture.TPostureLinkedSave(false));
        Assert.False(posture.TPostureRead().LPostureStateLinked);
        Assert.Single(posture.TPostureRead().LPostureStateLayout ?? []);
    }

    [Fact]
    public void PostureStart_LegacySettingsOnly_MigratesOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        File.WriteAllText(
            Path.Combine(workspace.TWorkspaceFolder, "settings.json"),
            "{ \"localization\": \"en\", \"window\": { \"left\": 1, \"top\": 2, \"width\": 700, \"height\": 500 }, "
            + "\"layout\": { \"library\": { \"left\": 420, \"order\": \"Reverse\" } }, \"mode\": \"Library\", "
            + "\"split\": \"Editor\", \"linked\": false, \"volume\": 0.5 }");
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LPostureState state = engine.TPostureStart().TPostureRead();

        Assert.Equal(700, state.LPostureStateWindow?.LWindowStateWidth);
        Assert.Equal(420, TPostureLeftRead(state.LPostureStateLayout ?? [], "library"));
        Assert.Equal(LCatalogOrder.LCatalogOrderReverse, TPostureOrderRead(state.LPostureStateLayout ?? [], "library"));
        Assert.Equal("Library", state.LPostureStateMode);
        Assert.True(state.LPostureStateSplit);
        Assert.False(state.LPostureStateLinked);
        Assert.Equal(0.5, state.LPostureStateVolume);
        Assert.True(File.Exists(Path.Combine(workspace.TWorkspaceFolder, "posture.json")));
    }

    [Fact]
    public void PostureStart_PostureBesideLegacy_PrefersPosture()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        File.WriteAllText(
            Path.Combine(workspace.TWorkspaceFolder, "settings.json"),
            "{ \"localization\": \"en\", \"mode\": \"Library\" }");
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "posture.json"), "{ \"mode\": \"Corpus\" }");
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Equal("Corpus", engine.TPostureStart().TPostureRead().LPostureStateMode);
    }

    [Fact]
    public void WorkspaceOpen_TargetEmpty_InheritsCurrentPosture()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        using LEngine engine = first.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        posture.TPostureModeSave("Tenor");

        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);

        Assert.Equal("Tenor", posture.TPostureRead().LPostureStateMode);
        Assert.True(File.Exists(Path.Combine(second.TWorkspaceFolder, "posture.json")));
    }

    [Fact]
    public void WorkspaceOpen_TargetHasPosture_KeepsTargetPosture()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(second.TWorkspaceFolder, "posture.json"), "{ \"mode\": \"Corpus\" }");
        using LEngine engine = first.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        posture.TPostureModeSave("Tenor");

        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);

        Assert.Equal("Corpus", posture.TPostureRead().LPostureStateMode);
    }

    [Fact]
    public void PostureLoader_OrderAndFilter_RoundTripsByName()
    {
        LLayout[] layout =
        [
            TInterface.TLayoutCreate(
                "corpus",
                order: LCatalogOrder.LCatalogOrderSource,
                filter: TInterface.TCatalogFilterCreate("Korean", "French"))
        ];

        string text = TInterface.TPostureLoaderFormat(TInterface.TPostureStateCreate(layout: layout, split: true));
        LPostureState loaded = TInterface.TPostureLoaderRead(text);

        LLayout corpus = Assert.Single(loaded.LPostureStateLayout ?? []);
        Assert.Equal("corpus", corpus.LLayoutTab);
        Assert.Null(corpus.LLayoutLeft);
        Assert.Equal(LCatalogOrder.LCatalogOrderSource, corpus.LLayoutOrder);
        Assert.Equal(["Korean", "French"], corpus.LLayoutFilter?.LCatalogFilterHidden);
        Assert.True(loaded.LPostureStateSplit);
        Assert.Contains("\"order\": \"Source\"", text);
        Assert.Contains("\"split\": \"Editor\"", text);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{ not json")]
    [InlineData("[1, 2]")]
    [InlineData("{ \"layout\": 5, \"mode\": 4, \"split\": true, \"linked\": \"no\", \"volume\": \"loud\" }")]
    [InlineData("{ \"layout\": { \"library\": { \"left\": -1, \"order\": \"Sideways\" }, \"tenor\": \"wide\" } }")]
    public void PostureLoader_AbsentOrUnusable_ReadsDefaults(string? text)
    {
        LPostureState state = TInterface.TPostureLoaderRead(text);

        Assert.Null(state.LPostureStateWindow);
        Assert.Empty(state.LPostureStateLayout ?? []);
        Assert.True(state.LPostureStateLinked);
        Assert.Null(state.LPostureStateMode);
        Assert.False(state.LPostureStateSplit);
        Assert.Equal(1, state.LPostureStateVolume);
    }

    [Fact]
    public void PostureLoader_VolumeAboveOne_ClampsToOne()
    {
        Assert.Equal(1, TInterface.TPostureLoaderRead("{ \"volume\": 4 }").LPostureStateVolume);
        Assert.Equal(0, TInterface.TPostureLoaderRead("{ \"volume\": -4 }").LPostureStateVolume);
    }

    [Fact]
    public void PostureLoader_HalfWindow_ReadsNoWindow()
    {
        Assert.Null(TInterface.TPostureLoaderRead("{ \"window\": { \"left\": 1, \"top\": 2 } }").LPostureStateWindow);
    }

    private static LCatalogOrder? TPostureOrderRead(IReadOnlyList<LLayout> layout, string tab)
    {
        foreach (LLayout record in layout)
        {
            if (record.LLayoutTab == tab)
            {
                return record.LLayoutOrder;
            }
        }

        return null;
    }

    private static double? TPostureLeftRead(IReadOnlyList<LLayout> layout, string tab)
    {
        foreach (LLayout record in layout)
        {
            if (record.LLayoutTab == tab)
            {
                return record.LLayoutLeft;
            }
        }

        return null;
    }

    private static IReadOnlyList<string>? TPostureFilterRead(IReadOnlyList<LLayout> layout, string tab)
    {
        foreach (LLayout record in layout)
        {
            if (record.LLayoutTab == tab)
            {
                return record.LLayoutFilter?.LCatalogFilterHidden;
            }
        }

        return null;
    }
}
