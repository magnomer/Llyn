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

        Assert.Empty(anthology.TAnthologyRowsRead());
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
        CEnvoy envoy = TInterfaceConduct.TEnvoyCreate(false, asked);

        Assert.Null(TInterfaceMention.TAnthologyFailRead(engine, atelier, envoy));
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
        TAnthologyExampleSave(engine, TInterface.TStateValueCreate("a cat sat"), null);
        TAnthologyExampleSave(engine, TInterface.TStateValueCreate("a dog ran"), null);
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

    [Fact]
    public void AnthologyDraftRead_TranscriptCitingASource_CarriesItsLine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LReference notes = engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);
        Assert.Equal(string.Empty, anthology.TAnthologyDraftRead(desk.TDeskRead())?.CExampleCitation);

        anthology.CAnthologyCitationSet(notes.LReferenceId);

        CExample? held = anthology.TAnthologyDraftRead(desk.TDeskRead());
        Assert.Equal(notes.LReferenceId, held?.CExampleSource);
        Assert.Equal("Field Notes", held?.CExampleCitation);
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

        Assert.Equal(notes.LReferenceId, anthology.TAnthologyDraftRead(desk.TDeskRead())?.CExampleSource);
    }

    [Fact]
    public void AnthologyCitationSet_EngineFails_ShowsTheCreateFailureAndKeepsTheCitation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TInterfaceConduct.TEnvoyCreate(false, asked);
        CDesk desk = TInterfaceCitation.TDeskFailCreate(engine, envoy, "Example", "Corpus", CSubject.CSubjectExample);
        desk.CDeskStart(null);
        CAnthology anthology = TInterfaceConduct.TAnthologyCreate(atelier, desk, envoy);

        anthology.CAnthologyCitationSet("Field Notes");

        Assert.Equal(["Reference.CreateFailed"], asked);
        Assert.Null(anthology.TAnthologyDraftRead(desk.TDeskRead())?.CExampleSource);
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

        CProffer offer = anthology.CAnthologyCitationRead(" field ");

        Assert.True(offer.CProfferShown);
        CProfferRow row = Assert.Single(offer.CProfferRows);
        Assert.Equal(notes.LReferenceId, row.CProfferRowId);
        Assert.Equal(string.Empty, row.CProfferRowLead);
        Assert.Equal("Field", row.CProfferRowMark);
        Assert.Equal(" Notes", row.CProfferRowTail);
        Assert.Equal(string.Empty, row.CProfferRowCount);
    }

    [Fact]
    public void AnthologyCitationRead_BlankWordOrNoTranscript_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);

        CProffer unheld = anthology.CAnthologyCitationRead("field");
        desk.CDeskStart(null);
        CProffer blank = anthology.CAnthologyCitationRead("   ");

        Assert.False(unheld.CProfferShown);
        Assert.Empty(unheld.CProfferRows);
        Assert.False(blank.CProfferShown);
        Assert.Empty(blank.CProfferRows);
    }

    [Fact]
    public void AnthologyCitationRead_CitedByline_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);
        anthology.CAnthologyCitationSet("Field Notes");
        string byline = anthology.TAnthologyDraftRead(desk.TDeskRead())!.CExampleCitation;

        Assert.Empty(anthology.CAnthologyCitationRead(byline).CProfferRows);
    }

    [Fact]
    public void AnthologyGlossAdd_RowBelow_PlacesTheGlossInTheGlossLanguageAfterIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TGlossAdditionCreate(desk.CDeskId, 0, 0, "French", 0));
        desk.CDeskPersist();
        desk.TDeskDefer(TInterface.TGlossAdditionCreate(desk.CDeskId, 0, 0, "German", 1));
        desk.CDeskPersist();

        anthology.CAnthologyGlossAdd(0);

        Assert.Equal(
            ["French", engine.TEngineGlossRead(), "German"],
            TAnthologyGlossRead(anthology, desk).Select(static row => row.CGlossDraftLanguage));
    }

    [Fact]
    public void AnthologyGlossAdd_BelowTheLastRowAndBeforeTheFirst_PlacesEachAtItsPlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TGlossAdditionCreate(desk.CDeskId, 0, 0, "French", 0));
        desk.CDeskPersist();

        anthology.CAnthologyGlossAdd(0);
        anthology.CAnthologyGlossAdd(-1);

        Assert.Equal(
            [engine.TEngineGlossRead(), "French", engine.TEngineGlossRead()],
            TAnthologyGlossRead(anthology, desk).Select(static row => row.CGlossDraftLanguage));
    }

    [Fact]
    public void AnthologyGlossPrepare_EmptyThenHeld_AddsOnlyTheFirstGloss()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);

        bool first = anthology.CAnthologyGlossPrepare();
        bool second = anthology.CAnthologyGlossPrepare();

        Assert.True(first);
        Assert.False(second);
        CGlossDraft held = Assert.Single(TAnthologyGlossRead(anthology, desk));
        Assert.Equal(engine.TEngineGlossRead(), held.CGlossDraftLanguage);
    }

    [Fact]
    public void AnthologyGlossPrepare_NoTranscriptHeld_AddsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out _);

        Assert.False(anthology.CAnthologyGlossPrepare());
    }

    [Fact]
    public void AnthologyGlossSetAndLanguageSetAndRemove_PickedRows_WriteTheTextRetagAndDropTheGloss()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TGlossAdditionCreate(desk.CDeskId, 0, 0, "French", 0));
        desk.CDeskPersist();
        desk.TDeskDefer(TInterface.TGlossAdditionCreate(desk.CDeskId, 0, 0, "German", 1));
        desk.CDeskPersist();
        IReadOnlyList<CGlossDraft> added = TAnthologyGlossRead(anthology, desk);

        anthology.CAnthologyGlossSet(added[1].CGlossDraftId, "die Katze");
        desk.CDeskPersist();
        anthology.CAnthologyLanguageSet(added[1].CGlossDraftId, "Dutch");
        anthology.CAnthologyGlossRemove(added[0].CGlossDraftId);

        CGlossDraft kept = Assert.Single(TAnthologyGlossRead(anthology, desk));
        Assert.Equal(added[1].CGlossDraftId, kept.CGlossDraftId);
        Assert.Equal("Dutch", kept.CGlossDraftLanguage);
        Assert.Equal("die Katze", kept.CGlossDraftText.CStateValueShown);
    }

    [Fact]
    public void AnthologyTextSetAndSpeakerSet_TypedTextAndPickedLanguage_WriteTheTranscriptAndAnswerTheTextHint()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);

        string hint = anthology.CAnthologyTextSet("a cat");
        desk.CDeskPersist();
        anthology.CAnthologySpeakerSet("French");

        CExample? held = anthology.TAnthologyDraftRead(desk.TDeskRead());
        Assert.Equal("Example.Text", hint);
        Assert.Equal("a cat", held?.CExampleText.CStateValueText);
        Assert.Equal("French", held?.CExampleLanguage);
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
        Assert.Null(TInterfaceConduct.TAnthologyExampleRead(null, string.Empty, string.Empty));
    }

    [Fact]
    public void AnthologyExampleRead_SoundText_LinksExcerpt()
    {
        LExample example = TInterface.TExampleCreate(
                5, "English", TInterface.TStateValueCreate("a cat"), null, TInterface.TStateAnchorRead(9))
            .TExampleMentionAdd(TInterface.TMentionCreate(1, 2, 3, 40));

        CExample? held = TInterfaceConduct.TAnthologyExampleRead(example, "Field Notes", "2 quotes");

        Assert.NotNull(held);
        Assert.Equal("English", held!.CExampleLanguage);
        Assert.Equal("a cat", held.CExampleText.CStateValueText);
        Assert.Equal(9, held.CExampleSource);
        Assert.Equal("Field Notes", held.CExampleCitation);
        Assert.Equal("Example.Text", held.CExampleTextHint);
        Assert.Null(held.CExampleWording);
        Assert.False(held.CExampleMuted);
        Assert.Equal("2 quotes", held.CExampleTally);
        Assert.Equal([40L], held.CExampleExcerpt.Select(mention => mention.CMentionMarkEntry));
    }

    [Fact]
    public void AnthologyExampleRead_UnknownText_LeavesExcerptUnlinked()
    {
        LExample example = TInterface.TExampleCreate(
                5, "English", TInterface.TStateValueResolve(null, true), null, TInterface.TStateAnchorRead(null))
            .TExampleMentionAdd(TInterface.TMentionCreate(1, 2, 3, 40));

        CExample? held = TInterfaceConduct.TAnthologyExampleRead(example, string.Empty, string.Empty);

        Assert.NotNull(held);
        Assert.True(held!.CExampleText.CStateValueUncertain);
        Assert.Equal("Display.Unknown", held.CExampleTextHint);
        Assert.Equal("Display.Unknown", held.CExampleWording);
        Assert.False(held.CExampleMuted);
        Assert.Null(held.CExampleSource);
        Assert.Empty(held.CExampleExcerpt);
    }

    [Fact]
    public void AnthologyExampleRead_UnwrittenText_WordsItUnwrittenAndMuted()
    {
        LExample example = TInterface.TExampleCreate(
            5, "English", TInterface.TStateValueCreate(string.Empty), null, TInterface.TStateAnchorRead(null));

        CExample? held = TInterfaceConduct.TAnthologyExampleRead(example, string.Empty, string.Empty);

        Assert.NotNull(held);
        Assert.Equal("Example.Unwritten", held!.CExampleWording);
        Assert.True(held.CExampleMuted);
    }

    private static CAnthology TAnthologyPrepare(LEngine engine, CAtelier atelier, out CDesk desk)
    {
        CEnvoy envoy = TInterfaceConduct.TEnvoyCreate(false, []);
        desk = TInterfaceConduct.TDeskCreate(engine, "Example", envoy, "Corpus", CSubject.CSubjectExample);
        return TInterfaceConduct.TAnthologyCreate(atelier, desk, envoy);
    }

    private static IReadOnlyList<CGlossDraft> TAnthologyGlossRead(CAnthology anthology, CDesk desk)
    {
        return anthology.TAnthologyDraftRead(desk.TDeskRead())?.CExampleGloss ?? [];
    }

    private static LExample TAnthologyExampleSave(LEngine engine, LStateValue text, long? source)
    {
        return engine.TEngineExampleCreate(
            TInterface.TExampleCreate(0, "English", text, null, TInterface.TStateAnchorRead(source)));
    }
}
