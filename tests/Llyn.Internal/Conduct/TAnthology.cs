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
        TAnthologyExampleSave(engine, TInterface.TStateValueCreate("a cat sat"), null);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);

        Assert.Empty(anthology.CAnthologyRowsRead("?", "-"));
        Assert.False(anthology.CAnthologyNarrowed);
        Assert.Null(anthology.CAnthologyChosen);
    }

    [Fact]
    public void AnthologyRowsRead_UnknownText_WordsTheRowWithTheUnknownWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample unknown = TAnthologyExampleSave(engine, TInterface.TStateValueResolve(null, true), null);
        LExample sound = TAnthologyExampleSave(engine, TInterface.TStateValueCreate("a cat sat"), null);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);
        anthology.CAnthologyVistaRestore(engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText));

        IReadOnlyList<CCatalogExample> rows = anthology.CAnthologyRowsRead("?", "-");

        Assert.Equal("?", rows.Single(row => row.CCatalogExampleId == unknown.LExampleId).CCatalogExampleText);
        Assert.Equal("a cat sat", rows.Single(row => row.CCatalogExampleId == sound.LExampleId).CCatalogExampleText);
    }

    [Fact]
    public void AnthologyQuerySet_MatchingText_NarrowsTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TAnthologyExampleSave(engine, TInterface.TStateValueCreate("a cat sat"), null);
        TAnthologyExampleSave(engine, TInterface.TStateValueCreate("a dog ran"), null);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);
        anthology.CAnthologyVistaRestore(engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText));

        anthology.CAnthologyQuerySet("dog");

        Assert.True(anthology.CAnthologyNarrowed);
        Assert.Equal(["a dog ran"], anthology.CAnthologyRowsRead("?", "-").Select(row => row.CCatalogExampleText));
    }

    [Fact]
    public void AnthologyOrderSet_NoOrderRow_KeepsTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);
        anthology.CAnthologyVistaRestore(engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText));

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
        anthology.CAnthologyVistaRestore(engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText));

        anthology.CAnthologyFilterSet(new CCatalogFilter(["English"]));

        Assert.True(anthology.CAnthologyFiltered);
        Assert.Equal(["English"], anthology.CAnthologyPanel.CPanelFilter.CCatalogFilterHidden);
    }

    [Fact]
    public void AnthologyMentionRead_NoChosenExample_ReadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);
        anthology.CAnthologyVistaRestore(engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText));

        Assert.Null(anthology.CAnthologyMentionRead(2));
    }

    [Fact]
    public void AnthologyReferenceRead_StoredSource_ListsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LReference notes = engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);

        Assert.Contains(anthology.CAnthologyReferenceRead(), row => row.CCatalogReferenceId == notes.LReferenceId);
    }

    [Fact]
    public void AnthologyCitationSet_TypedTitle_CitesTheResolvedSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LReference notes = engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);

        anthology.CAnthologyCitationSet("Field Notes");

        Assert.Equal(
            notes.LReferenceId, CAnthology.CAnthologyExampleRead(desk.TDeskRead()?.LDraftExample)?.CExampleSource);
    }

    [Fact]
    public void AnthologyCitationRead_PartOfTitle_OffersTheSourceSplit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LReference notes = engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);

        CCitationRow row = Assert.Single(anthology.CAnthologyCitationRead(" field "));

        Assert.Equal(notes.LReferenceId, row.CCitationRowId);
        Assert.Equal("Field", row.CCitationRowMark);
    }

    [Fact]
    public void AnthologyCitationRead_BlankWord_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);

        Assert.Empty(anthology.CAnthologyCitationRead("   "));
    }

    [Fact]
    public void AnthologyCitationRead_CitedByline_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LReference notes = engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);
        anthology.CAnthologyCitationSet("Field Notes");
        string byline = anthology.CAnthologyReferenceRead()
            .Single(row => row.CCatalogReferenceId == notes.LReferenceId).CCatalogReferenceByline;

        Assert.Empty(anthology.CAnthologyCitationRead(byline));
    }

    [Fact]
    public void AnthologyTextCheck_BlankField_MatchesAnEmptyText()
    {
        Assert.True(CAnthology.CAnthologyTextCheck("  ", CStateValue.CStateValueEmpty));
        Assert.True(CAnthology.CAnthologyTextCheck("a cat", new CStateValue("a cat", false, true)));
        Assert.False(CAnthology.CAnthologyTextCheck("a cat ", new CStateValue("a cat", false, true)));
    }

    [Fact]
    public void AnthologyExampleRead_NoExample_ReturnsNone()
    {
        Assert.Null(CAnthology.CAnthologyExampleRead(null));
    }

    [Fact]
    public void AnthologyExampleRead_SoundText_LinksExcerpt()
    {
        LExample example = TInterface.TExampleCreate(
                5, "English", TInterface.TStateValueCreate("a cat"), null, TInterface.TStateAnchorRead(9))
            .TExampleMentionAdd(TInterface.TMentionCreate(1, 2, 3, 40));

        CExample? held = CAnthology.CAnthologyExampleRead(example);

        Assert.NotNull(held);
        Assert.Equal("English", held!.CExampleLanguage);
        Assert.Equal("a cat", held.CExampleText.CStateValueText);
        Assert.Equal(9, held.CExampleSource);
        Assert.Equal([40L], held.CExampleMention.Select(mention => mention.CMentionDraftEntry));
        Assert.Equal([40L], held.CExampleExcerpt.Select(mention => mention.CMentionMarkEntry));
    }

    [Fact]
    public void AnthologyExampleRead_UnknownText_LeavesExcerptUnlinked()
    {
        LExample example = TInterface.TExampleCreate(
                5, "English", TInterface.TStateValueResolve(null, true), null, TInterface.TStateAnchorRead(null))
            .TExampleMentionAdd(TInterface.TMentionCreate(1, 2, 3, 40));

        CExample? held = CAnthology.CAnthologyExampleRead(example);

        Assert.NotNull(held);
        Assert.True(held!.CExampleText.CStateValueUncertain);
        Assert.Null(held.CExampleSource);
        Assert.Single(held.CExampleMention);
        Assert.Empty(held.CExampleExcerpt);
    }

    private static CAnthology TAnthologyPrepare(LEngine engine, CAtelier atelier, out CDesk desk)
    {
        CEnvoy envoy = TInterfaceConduct.TEnvoyCreate(false, []);
        desk = TInterfaceConduct.TDeskCreate(engine, "Example", envoy, "Corpus", CSubject.CSubjectExample);
        return CAnthology.CAnthologyCreate(atelier, desk, static () => true, envoy, static _ => true);
    }

    private static LExample TAnthologyExampleSave(LEngine engine, LStateValue text, long? source)
    {
        return engine.TEngineExampleCreate(
            TInterface.TExampleCreate(0, "English", text, null, TInterface.TStateAnchorRead(source)));
    }
}
