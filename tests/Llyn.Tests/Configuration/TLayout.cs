using System.Collections.Generic;
using System.IO;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLayout
{
    [Fact]
    public void OrderSave_EveryBrowsePanel_KeepsEachOrderingApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderRecent);
        engine.TEngineVistaStart("phonology", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderSound);
        engine.TEngineVistaStart("favorite", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderMarked);
        engine.TEngineVistaStart("taxonomy", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderUsage);
        engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderKind);
        engine.TEngineVistaStart("reference", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderYear);
        engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderLanguage);
        engine.TEngineVistaStart("tenor", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderReverse);

        IReadOnlyList<LLayout> layout = engine.TEngineSettingsRead().LSettingsLayout ?? [];

        Assert.Equal(LCatalogOrder.LCatalogOrderRecent, TLayoutOrderRead(layout, "library"));
        Assert.Equal(LCatalogOrder.LCatalogOrderSound, TLayoutOrderRead(layout, "phonology"));
        Assert.Equal(LCatalogOrder.LCatalogOrderMarked, TLayoutOrderRead(layout, "favorite"));
        Assert.Equal(LCatalogOrder.LCatalogOrderUsage, TLayoutOrderRead(layout, "taxonomy"));
        Assert.Equal(LCatalogOrder.LCatalogOrderKind, TLayoutOrderRead(layout, "repertoire"));
        Assert.Equal(LCatalogOrder.LCatalogOrderYear, TLayoutOrderRead(layout, "reference"));
        Assert.Equal(LCatalogOrder.LCatalogOrderLanguage, TLayoutOrderRead(layout, "corpus"));
        Assert.Equal(LCatalogOrder.LCatalogOrderReverse, TLayoutOrderRead(layout, "tenor"));
    }

    [Fact]
    public void FilterSave_EveryBrowsePanel_KeepsEachFilterApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("Korean"));
        engine.TEngineVistaStart("phonology", LCatalogOrder.LCatalogOrderName)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("French"));
        engine.TEngineVistaStart("favorite", LCatalogOrder.LCatalogOrderName)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("German"));
        engine.TEngineVistaStart("taxonomy", LCatalogOrder.LCatalogOrderName)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("Spanish"));
        engine.TEngineVistaStart("tenor", LCatalogOrder.LCatalogOrderName)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("Italian"));
        engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("Japanese"));
        engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderName)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("Chinese"));
        engine.TEngineVistaStart("reference", LCatalogOrder.LCatalogOrderName)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("Dutch"));

        IReadOnlyList<LLayout> layout = engine.TEngineSettingsRead().LSettingsLayout ?? [];

        Assert.Equal(["Korean"], TLayoutFilterRead(layout, "library"));
        Assert.Equal(["French"], TLayoutFilterRead(layout, "phonology"));
        Assert.Equal(["German"], TLayoutFilterRead(layout, "favorite"));
        Assert.Equal(["Spanish"], TLayoutFilterRead(layout, "taxonomy"));
        Assert.Equal(["Italian"], TLayoutFilterRead(layout, "tenor"));
        Assert.Equal(["Japanese"], TLayoutFilterRead(layout, "repertoire"));
        Assert.Equal(["Chinese"], TLayoutFilterRead(layout, "corpus"));
        Assert.Equal(["Dutch"], TLayoutFilterRead(layout, "reference"));
    }

    [Fact]
    public void OrderSave_WorkspaceReopened_KeepsChosenOrdering()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
            vista.TVistaOrderSet(LCatalogOrder.LCatalogOrderReverse);
            vista.TVistaFilterSet(TInterface.TCatalogFilterCreate("Korean", "French"));
            engine.TEngineModeSave("Corpus");
            engine.TEngineSplitSave(true);
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        LSettings settings = reopened.TEngineSettingsRead();

        Assert.Equal(LCatalogOrder.LCatalogOrderReverse, TLayoutOrderRead(settings.LSettingsLayout ?? [], "library"));
        Assert.Equal(["Korean", "French"], TLayoutFilterRead(settings.LSettingsLayout ?? [], "library"));
        Assert.Equal("Corpus", settings.LSettingsMode);
        Assert.True(settings.LSettingsSplit);
    }

    [Fact]
    public void OrderSave_AfterLayoutSave_KeepsDraggedWidth()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineLayoutSave(TInterface.TLayoutCreate("tenor", 320, 300));
        engine.TEngineVistaStart("tenor", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderUsage);
        engine.TEngineVistaStart("tenor", LCatalogOrder.LCatalogOrderName)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("Korean"));
        engine.TEngineLayoutSave(TInterface.TLayoutCreate("tenor", 340, 300));

        LLayout tenor = Assert.Single(engine.TEngineSettingsRead().LSettingsLayout ?? []);

        Assert.Equal(340, tenor.LLayoutLeft);
        Assert.Equal(300, tenor.LLayoutMiddle);
        Assert.Equal(LCatalogOrder.LCatalogOrderUsage, tenor.LLayoutOrder);
        Assert.Equal(["Korean"], tenor.LLayoutFilter?.LCatalogFilterHidden);
    }

    [Fact]
    public void LayoutReset_ClearsStoredWidths()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineLayoutSave(TInterface.TLayoutCreate("tenor", 320, 300));
        engine.TEngineVistaStart("tenor", LCatalogOrder.LCatalogOrderName)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderUsage);
        engine.TEngineVistaStart("tenor", LCatalogOrder.LCatalogOrderName)
            .TVistaFilterSet(TInterface.TCatalogFilterCreate("Korean"));
        engine.TEngineLayoutReset();

        LLayout tenor = Assert.Single(engine.TEngineSettingsRead().LSettingsLayout ?? []);

        Assert.Null(tenor.LLayoutLeft);
        Assert.Null(tenor.LLayoutMiddle);
        Assert.Equal(LCatalogOrder.LCatalogOrderUsage, tenor.LLayoutOrder);
        Assert.Equal(["Korean"], tenor.LLayoutFilter?.LCatalogFilterHidden);
    }

    [Fact]
    public void ModeSave_AfterOrderSave_KeepsBothChanges()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderEarliest);
        engine.TEngineModeSave("Library");

        LSettings settings = engine.TEngineSettingsRead();

        Assert.Equal(LCatalogOrder.LCatalogOrderEarliest, TLayoutOrderRead(settings.LSettingsLayout ?? [], "library"));
        Assert.Equal("Library", settings.LSettingsMode);
    }

    [Fact]
    public void SplitSave_ReadArea_ReadsBackAsReadArea()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineSplitSave(true);
        engine.TEngineSplitSave(false);

        Assert.False(engine.TEngineSettingsRead().LSettingsSplit);
        Assert.Contains(
            "\"split\": \"Display\"",
            File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json")));
    }

    [Fact]
    public void SettingsSave_OrderAndFilter_RoundTripsByName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        LLayout[] layout =
        [
            TInterface.TLayoutCreate(
                "corpus",
                order: LCatalogOrder.LCatalogOrderSource,
                filter: TInterface.TCatalogFilterCreate("Korean", "French"))
        ];

        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en", layout: layout));

        LLayout loaded = Assert.Single(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsLayout ?? []);

        Assert.Equal("corpus", loaded.LLayoutTab);
        Assert.Null(loaded.LLayoutLeft);
        Assert.Equal(LCatalogOrder.LCatalogOrderSource, loaded.LLayoutOrder);
        Assert.Equal(["Korean", "French"], loaded.LLayoutFilter?.LCatalogFilterHidden);
        Assert.Contains(
            "\"order\": \"Source\"",
            File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json")));
    }

    [Theory]
    [InlineData("{ \"localization\": \"en\", \"layout\": { \"library\": { \"order\": \"Sideways\" } } }")]
    [InlineData("{ \"localization\": \"en\", \"layout\": { \"library\": { \"order\": 3 } } }")]
    public void SettingsLoad_OrderUnknown_LoadsNoOrder(string json)
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json"), json);

        Assert.Empty(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsLayout ?? []);
    }

    [Theory]
    [InlineData("{ \"localization\": \"en\" }")]
    [InlineData("{ \"localization\": \"en\", \"mode\": \"\", \"split\": \"Display\" }")]
    [InlineData("{ \"localization\": \"en\", \"mode\": 4, \"split\": true }")]
    public void SettingsLoad_ModeAbsentOrUnusable_LoadsNoTab(string json)
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json"), json);

        LSettings settings = TInterface.TSettingsLoad(workspace.TWorkspaceFolder);

        Assert.Null(settings.LSettingsMode);
        Assert.False(settings.LSettingsSplit);
    }

    private static LCatalogOrder? TLayoutOrderRead(IReadOnlyList<LLayout> layout, string tab)
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

    private static IReadOnlyList<string>? TLayoutFilterRead(IReadOnlyList<LLayout> layout, string tab)
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
