using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAnthology
{
    [Fact]
    public void AnthologyRowsRead_NoVista_ReadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TAnthologyExampleSave(engine, TInterfaceState.TStateValueCreate("a cat sat"), null);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);

        Assert.Empty(anthology.TAnthologyRowsRead());
        Assert.Null(anthology.CAnthologyChosen);
    }

    [Fact]
    public void AnthologyRowsRead_UnknownText_WordsTheRowWithTheUnknownWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample unknown = TAnthologyExampleSave(engine, TInterfaceState.TStateValueResolve(null, true), null);
        LExample sound = TAnthologyExampleSave(engine, TInterfaceState.TStateValueCreate("a cat sat"), null);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);
        anthology.TAnthologyVistaRestore(engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText));

        IReadOnlyList<CCatalogExample> rows = anthology.TAnthologyRowsRead();

        Assert.Equal(
            TInterface.TLocalizationTextRead("Display.Unknown"),
            rows.Single(row => row.CCatalogExampleId == unknown.LExampleId).CCatalogExampleText);
        Assert.Equal("a cat sat", rows.Single(row => row.CCatalogExampleId == sound.LExampleId).CCatalogExampleText);
        Assert.All(rows, row => Assert.Equal(string.Empty, row.CCatalogExampleCount));
    }

    [Fact]
    public void AnthologyRowsRead_EngineFails_ShowsTheLoadFailureAndAnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        Assert.Null(TInterfaceMention.TAnthologyFailRead(engine, atelier, envoy));
        Assert.Equal(["Example.LoadFailed"], asked);
    }

    [Fact]
    public void AnthologyPanelTallyRead_EngineRefuses_ShowsTheLoadFailureAndAnswersEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        CDesk desk = TInterfaceConductDesk.TDeskCreate(engine, "Example", envoy, "Corpus", CSubject.CSubjectExample);
        CAnthology anthology = TInterfaceConductPanel.TAnthologyCreate(atelier, desk, envoy);
        anthology.TAnthologyVistaRestore(engine.TEngineVistaStart("occurrence", LCatalogOrder.LCatalogOrderHeadword));

        Assert.Equal(string.Empty, anthology.CAnthologyPanel.CPanelTallyRead());
        Assert.Equal(["Example.LoadFailed"], asked);
    }

    [Fact]
    public void AnthologyOrderRead_Menu_OffersTextLanguageSourceAndUsage()
    {
        Assert.Equal(
            [
                CCatalogOrder.CCatalogOrderText,
                CCatalogOrder.CCatalogOrderLanguage,
                CCatalogOrder.CCatalogOrderSource,
                CCatalogOrder.CCatalogOrderUsage,
            ],
            CAnthology.CAnthologyOrderRead());
    }

    [Fact]
    public void AnthologyQuerySet_MatchingText_NarrowsTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TAnthologyExampleSave(engine, TInterfaceState.TStateValueCreate("a cat sat"), null);
        TAnthologyExampleSave(engine, TInterfaceState.TStateValueCreate("a dog ran"), null);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);
        anthology.TAnthologyVistaRestore(engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText));

        anthology.CAnthologyQuerySet("dog");

        Assert.Equal(["a dog ran"], anthology.TAnthologyRowsRead().Select(row => row.CCatalogExampleText));
    }

    [Fact]
    public void AnthologyOrderSet_NoOrderRow_KeepsTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);
        anthology.TAnthologyVistaRestore(engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText));

        anthology.CAnthologyOrderSet(CCatalogOrder.CCatalogOrderUsage);
        anthology.CAnthologyOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderUsage, anthology.CAnthologyPanel.CPanelOrder);
    }

    [Fact]
    public void AnthologyFilterSet_HiddenLanguage_MarksTheListFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);
        anthology.TAnthologyVistaRestore(engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText));

        anthology.CAnthologyFilterSet(new CCatalogFilter(["English"]));

        Assert.True(anthology.CAnthologyFiltered);
        Assert.Equal(["English"], anthology.CAnthologyPanel.CPanelFilter.CCatalogFilterHidden);
    }

    internal static CAnthology TAnthologyPrepare(LEngine engine, CAtelier atelier, out CDesk desk)
    {
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, []);
        desk = TInterfaceConductDesk.TDeskCreate(engine, "Example", envoy, "Corpus", CSubject.CSubjectExample);
        return TInterfaceConductPanel.TAnthologyCreate(atelier, desk, envoy);
    }

    private static LExample TAnthologyExampleSave(LEngine engine, LStateValue text, long? source)
    {
        return engine.TEngineExampleCreate(
            TInterfaceExample.TExampleCreate(0, "English", text, null, TInterfaceState.TStateAnchorRead(source)));
    }
}
