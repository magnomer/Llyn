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
    public void FilterSave_AfterOrderSave_KeepsOrdering()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();

        posture.TPostureLayoutSave(TInterface.TLayoutCreate("tenor", order: LCatalogOrder.LCatalogOrderUsage));
        posture.TPostureLayoutSave(
            TInterface.TLayoutCreate("tenor", filter: TInterface.TCatalogFilterCreate("Korean")));

        LLayout tenor = Assert.Single(posture.TPostureRead().LPostureStateLayout ?? []);

        Assert.Equal(LCatalogOrder.LCatalogOrderUsage, tenor.LLayoutOrder);
        Assert.Equal(["Korean"], tenor.LLayoutFilter?.LCatalogFilterHidden);
    }

    [Fact]
    public void LayoutSave_SecondTab_KeepsFirstTab()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();

        posture.TPostureLayoutSave(TInterface.TLayoutCreate("library", order: LCatalogOrder.LCatalogOrderName));
        posture.TPostureLayoutSave(
            TInterface.TLayoutCreate("corpus", order: LCatalogOrder.LCatalogOrderText),
            TInterface.TLayoutCreate("library", order: LCatalogOrder.LCatalogOrderRecent));

        IReadOnlyList<LLayout> layout = posture.TPostureRead().LPostureStateLayout ?? [];

        Assert.Equal(2, layout.Count);
        Assert.Equal(LCatalogOrder.LCatalogOrderRecent, TPostureOrderRead(layout, "library"));
        Assert.Equal(LCatalogOrder.LCatalogOrderText, TPostureOrderRead(layout, "corpus"));
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
    public void PostureVolumeSave_SameLevel_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        string path = Path.Combine(workspace.TWorkspaceFolder, "posture.json");

        posture.TPostureVolumeSave(0.25);
        DateTime written = File.GetLastWriteTimeUtc(path);
        File.SetLastWriteTimeUtc(path, written.AddMinutes(-5));

        posture.TPostureVolumeSave(0.25);

        Assert.Equal(written.AddMinutes(-5), File.GetLastWriteTimeUtc(path));
    }

    [Fact]
    public void PostureVolumeSave_NotANumber_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        string path = Path.Combine(workspace.TWorkspaceFolder, "posture.json");

        posture.TPostureVolumeSave(0.25);
        DateTime written = File.GetLastWriteTimeUtc(path);
        File.SetLastWriteTimeUtc(path, written.AddMinutes(-5));

        posture.TPostureVolumeSave(double.NaN);

        Assert.Equal(0.25, posture.TPostureRead().LPostureStateVolume);
        Assert.Equal(written.AddMinutes(-5), File.GetLastWriteTimeUtc(path));
    }

    [Fact]
    public void PostureVolumeSet_NoSave_ReadsTheNewLevelAndWritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        string path = Path.Combine(workspace.TWorkspaceFolder, "posture.json");

        posture.TPostureVolumeSave(0.25);
        DateTime written = File.GetLastWriteTimeUtc(path);
        File.SetLastWriteTimeUtc(path, written.AddMinutes(-5));

        posture.TPostureVolumeSet(0.5);

        Assert.Equal(0.5, posture.TPostureRead().LPostureStateVolume);
        Assert.Equal(written.AddMinutes(-5), File.GetLastWriteTimeUtc(path));
    }

    [Fact]
    public void VolumeSave_WorkspaceReopened_KeepsVolume()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LPosture posture = engine.TPostureStart();
            posture.TPostureVolumeSave(0.25);
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        LPostureState state = reopened.TPostureStart().TPostureRead();

        Assert.Equal(0.25, state.LPostureStateVolume);
        Assert.False(File.Exists(Path.Combine(workspace.TWorkspaceFolder, "posture.json.tmp")));
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

        Assert.Equal(LCatalogOrder.LCatalogOrderReverse, TPostureOrderRead(state.LPostureStateLayout ?? [], "library"));
        Assert.Equal("Library", state.LPostureStateMode);
        Assert.True(state.LPostureStateSplit);
        Assert.Equal(0.5, state.LPostureStateVolume);
        Assert.True(File.Exists(Path.Combine(workspace.TWorkspaceFolder, "posture.json")));
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

        Assert.Empty(state.LPostureStateLayout ?? []);
        Assert.Null(state.LPostureStateMode);
        Assert.False(state.LPostureStateSplit);
        Assert.Equal(1, state.LPostureStateVolume);
    }

    [Fact]
    public void PostureSave_FaultingStore_RecordsEachFaultAndRaisesTheFailureOnce()
    {
        List<Exception> thrown = [];
        List<Exception> recorded = [];
        List<Exception> failed = [];
        LPostureVault faulting = TEngineFake.TEngineCreate<LPostureVault>(
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LPostureRead"] = _ => TInterface.TPostureStateCreate(),
                ["LPostureSave"] = _ =>
                {
                    LVaultFault fault = new(new IOException("The disk is full."));
                    thrown.Add(fault);
                    throw fault;
                },
            });
        LAuditVault audit = TEngineFake.TEngineCreate<LAuditVault>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LAuditRecord"] = args =>
            {
                recorded.Add((Exception)args![0]!);
                return null;
            },
        });
        using LEngine engine = TRigFake.TRigFakeStart(
            TRigFake.TRigFakeBuild() with { LRigPosture = faulting, LRigAudit = audit });
        LPosture posture = engine.TPostureStart();
        posture.LPostureSaveFailed += failed.Add;

        posture.TPostureModeSave("Corpus");
        posture.TPostureModeSave("Library");

        Assert.Equal(2, thrown.Count);
        Assert.Equal(thrown, recorded);
        Assert.Same(thrown[0], Assert.Single(failed));
        Assert.Equal("Library", posture.TPostureRead().LPostureStateMode);
    }

    [Fact]
    public void PostureLoader_VolumeAboveOne_ClampsToOne()
    {
        Assert.Equal(1, TInterface.TPostureLoaderRead("{ \"volume\": 4 }").LPostureStateVolume);
        Assert.Equal(0, TInterface.TPostureLoaderRead("{ \"volume\": -4 }").LPostureStateVolume);
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
