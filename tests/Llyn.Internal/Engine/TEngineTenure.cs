using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineTenure
{
    private const int TTenureHold = 600000;

    [Fact]
    public void TenureDefer_SameKey_AppliesLast()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("one")));
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("two")));

        Assert.Equal(string.Empty, tenure.TTenureRead()?.LDraftExample?.LExampleText.TStateValueShow());

        tenure.TTenurePersist();

        Assert.Equal("two", tenure.TTenureRead()?.LDraftExample?.LExampleText.TStateValueShow());
        Assert.True(tenure.TTenureStateRead().LTenureStateChanged);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureFinish_Unchanged_Cancels()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        long held = tenure.LTenureId;

        Assert.Null(tenure.TTenureFinish(true));
        Assert.Null(engine.TEngineDraftRead(held));
        Assert.Null(tenure.TTenureRead());
        Assert.Empty(engine.TEngineExampleFind(string.Empty, LCatalogOrder.LCatalogOrderEarliest));
    }

    [Fact]
    public void TenureFinish_Changed_CommitsExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("ember")));

        long? stored = tenure.TTenureFinish(true);

        Assert.NotNull(stored);
        Assert.Equal("ember", engine.TEngineExampleRead(stored.Value)?.LExampleText.TStateValueShow());
        Assert.Null(engine.TEngineDraftRead(tenure.LTenureId));
    }

    [Fact]
    public void TenureDefer_ApplyThrows_MarksHalted()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        engine.TEngineDraftCancel(tenure.LTenureId);

        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("lost")));

        Assert.True(tenure.TTenureStateRead().LTenureStateHalted);
        Assert.Throws<LRefusal>(() => tenure.TTenureFinish(true));
        Assert.Null(tenure.TTenureUndo());
    }

    [Fact]
    public void TenureDefer_Refused_KeepsRunning()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        List<long> drafts = [];
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
            {
                drafts.Add(bulletin.LBulletinId);
            }
        });

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestDefer(
            TInterface.TRequestAdditionCreate(tenure.LTenureId, LCardKind.LCardKindCollocation, 5, 0));
        tenure.TTenureRequestDefer(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "ember"));

        Assert.False(tenure.TTenureStateRead().LTenureStateHalted);
        Assert.Equal("ember", tenure.TTenureRead()?.LDraftContent.LEntryDraftHeadword);
        Assert.Contains(tenure.LTenureId, drafts);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureFinish_Discard_LeavesWaiting()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);
        List<long> drafts = [];
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
            {
                drafts.Add(bulletin.LBulletinId);
            }
        });

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("gone")));

        Assert.Null(tenure.TTenureFinish(false));
        Assert.Null(tenure.TTenureRead());
        Assert.DoesNotContain(tenure.LTenureId, drafts);
    }

    [Fact]
    public void TenureUndo_AfterDefer_PersistsFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);
        DateTimeOffset moment = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        workspace.TWorkspaceClockSet(() => moment);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestApply(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("one")));
        moment += TimeSpan.FromMilliseconds(2000);
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("two")));

        LDraft? restored = tenure.TTenureUndo();

        Assert.Equal("one", restored?.LDraftExample?.LExampleText.TStateValueShow());
        Assert.True(tenure.TTenureStateRead().LTenureStateForward);
        Assert.Equal("two", tenure.TTenureRedo()?.LDraftExample?.LExampleText.TStateValueShow());
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureFinish_Changed_CommitsEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestDefer(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "kindle"));

        long? stored = tenure.TTenureFinish(true);

        Assert.NotNull(stored);
        Assert.Equal("kindle", engine.TEngineEntryRead(stored.Value)?.LEntryHeadword);
        Assert.Null(engine.TEngineDraftRead(tenure.LTenureId));
    }

    [Fact]
    public void TenureFinish_Changed_CommitsSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectSituation, null);
        LSituation sent = TInterface.TSituationCreate(
            0,
            TInterface.TStateValueCreate("around a hearth"),
            TInterface.TStateValueCreate("a fireside"),
            TInterface.TStateValueCreate("place"));
        tenure.TTenureRequestDefer(TInterface.TSituationBodyCreate(tenure.LTenureId, 0, sent));

        long? stored = tenure.TTenureFinish(true);

        Assert.NotNull(stored);
        Assert.Equal("around a hearth", engine.TEngineSituationRead(stored.Value)?.LSituationTitle.TStateValueShow());
        Assert.Null(engine.TEngineDraftRead(tenure.LTenureId));
    }

    [Fact]
    public void TenureFinish_Changed_CommitsReference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectReference, null);
        tenure.TTenureRequestDefer(
            TInterface.TReferenceTitleCreate(tenure.LTenureId, TInterface.TStateValueCreate("the evening news")));

        long? stored = tenure.TTenureFinish(true);

        Assert.NotNull(stored);
        Assert.Equal("the evening news", engine.TEngineReferenceRead(stored.Value)?.LReferenceTitle.TStateValueShow());
        Assert.Null(engine.TEngineDraftRead(tenure.LTenureId));
    }

    [Fact]
    public void TenureFinish_Changed_CommitsAuthor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectAuthor, null);
        tenure.TTenureRequestDefer(TInterface.TAuthorNameCreate(tenure.LTenureId, "Ada Lovelace"));

        long? stored = tenure.TTenureFinish(true);

        Assert.NotNull(stored);
        Assert.Equal("Ada Lovelace", engine.TEngineAuthorRead(stored.Value)?.LAuthorName);
        Assert.Null(engine.TEngineDraftRead(tenure.LTenureId));
    }

    [Fact]
    public void TenureReadyCheck_AuthorNamedLater_ReadyOnceTheNameIsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectAuthor, null);

        Assert.False(tenure.TTenureReadyCheck());

        tenure.TTenureRequestDefer(TInterface.TAuthorNameCreate(tenure.LTenureId, "Ada"));

        Assert.True(tenure.TTenureReadyCheck());
        Assert.Equal("Ada", tenure.TTenureRead()?.LDraftAuthorHeld?.LAuthorName);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureFinish_Renamed_UpdatesAuthor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LAuthor author = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectAuthor, author.LAuthorId);
        Assert.Equal("Ada", tenure.TTenureRead()?.LDraftAuthorHeld?.LAuthorName);
        Assert.False(tenure.TTenureStateRead().LTenureStateChanged);

        tenure.TTenureRequestDefer(TInterface.TAuthorNameCreate(tenure.LTenureId, "Ada Lovelace"));
        tenure.TTenurePersist();
        Assert.True(tenure.TTenureStateRead().LTenureStateChanged);

        Assert.Equal(author.LAuthorId, tenure.TTenureFinish(true));
        Assert.Equal("Ada Lovelace", engine.TEngineAuthorRead(author.LAuthorId)?.LAuthorName);
        Assert.Single(engine.TEngineAuthorFind(string.Empty));
    }

    [Fact]
    public void OccurrenceStart_SituationGiven_StartsAFreshEntryAlreadyLinked()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LStateValue text = TInterface.TStateValueCreate("at the market");
        LSituation market = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, text, text, text));
        LVista vista = engine.TEngineVistaStart("occurrence", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineOccurrenceStart(vista, market.LSituationId);

        LEntryDraft? content = tenure.TTenureRead()?.LDraftContent;
        Assert.Null(tenure.TTenureRead()?.LDraftStored);
        Assert.Contains(
            content?.LEntryDraftMeanings[0].LCardDraftSituation ?? [],
            row => row.LSituationDraftId == market.LSituationId);
        tenure.TTenureCancel();
    }

    [Fact]
    public void OccurrenceStart_NoSituation_StartsABlankEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista vista = engine.TEngineVistaStart("occurrence", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineOccurrenceStart(vista, null);

        Assert.False(tenure.TTenureStateRead().LTenureStateChanged);
        Assert.All(
            tenure.TTenureRead()?.LDraftContent.LEntryDraftMeanings ?? [],
            card => Assert.Empty(card.LCardDraftSituation));
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureStorable_UnnamedOccurrence_AnswersFalseUntilTheHeadwordIsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);
        LStateValue text = TInterface.TStateValueCreate("at the market");
        LSituation market = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, text, text, text));
        LVista vista = engine.TEngineVistaStart("occurrence", LCatalogOrder.LCatalogOrderHeadword);
        LTenure tenure = engine.TEngineOccurrenceStart(vista, market.LSituationId);

        Assert.True(tenure.TTenureChangeCheck());
        Assert.False(tenure.TTenureStorableRead());

        tenure.TTenureRequestDefer(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "stall"));

        Assert.False(tenure.TTenureStorableRead());
        Assert.True(tenure.TTenureChangeCheck());
        Assert.True(tenure.TTenureStorableRead());
        Assert.Equal("stall", tenure.TTenureRead()?.LDraftContent.LEntryDraftHeadword);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureChangeCheck_FreshEntry_AnswersUnchanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);

        Assert.False(tenure.TTenureChangeCheck());
        Assert.False(tenure.TTenureStorableRead());
        tenure.TTenureCancel();
    }

    [Fact]
    public void QuotationStart_ExampleGiven_StartsAFreshEntryCitingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LExample example = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", TInterface.TStateValueCreate("Water is wet."), null, LStateAnchor.LStateAnchorUnspecified));
        LVista vista = engine.TEngineVistaStart("quotation", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineQuotationStart(vista, example.LExampleId);

        LExampleDraft? cited = tenure.TTenureRead()?.LDraftContent
            .LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftExample;
        Assert.Equal(example.LExampleId, cited?.LExampleDraftId);
        tenure.TTenureCancel();
    }

    [Fact]
    public void FootnoteStart_SourceGiven_StartsAFreshEntryCitingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        LVista vista = engine.TEngineVistaStart("footnote", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineFootnoteStart(vista, book.LReferenceId);

        LExampleDraft? cited = tenure.TTenureRead()?.LDraftContent
            .LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftExample;
        Assert.Equal(book.LReferenceId, cited?.LExampleDraftReference.LStateAnchorShown);
        tenure.TTenureCancel();
    }

    [Fact]
    public void FootnoteStart_NoSource_StartsABlankEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista vista = engine.TEngineVistaStart("footnote", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineFootnoteStart(vista, null);

        Assert.False(tenure.TTenureChangeCheck());
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureDefer_GlossTwice_OneChronicleStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);
        DateTimeOffset moment = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        workspace.TWorkspaceClockSet(() => moment);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        long draft = tenure.LTenureId;
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(draft, "kindle"));
        tenure.TTenureRequestApply(TInterface.TRequestAdditionCreate(draft, LCardKind.LCardKindMeaning, 0, 0));
        long card = tenure.TTenureRead()!.LDraftContent.LEntryDraftMeanings[0].LCardDraftId;
        tenure.TTenureRequestApply(TInterface.TSentenceAdditionCreate(draft, card, 0));
        long sentence = TInterface.TRequestCardFind(tenure.TTenureRead()!.LDraftContent, card)
            .LCardDraftSentence[0].LSentenceDraftId;
        tenure.TTenureRequestApply(
            TInterface.TSentenceTextCreate(draft, card, sentence, TInterface.TStateValueCreate("she knelt")));
        tenure.TTenureRequestApply(TInterface.TGlossAdditionCreate(draft, card, sentence, "French", 0));
        long gloss = TInterface.TRequestCardFind(tenure.TTenureRead()!.LDraftContent, card)
            .LCardDraftSentence[0].LSentenceDraftExample!.LExampleDraftGloss[0].LGlossDraftId;
        moment += TimeSpan.FromMilliseconds(60000);

        tenure.TTenureRequestDefer(
            TInterface.TGlossTextCreate(draft, card, sentence, gloss, TInterface.TStateValueCreate("elle")));
        tenure.TTenureRequestDefer(TInterface.TGlossTextCreate(
            draft, card, sentence, gloss, TInterface.TStateValueCreate("elle s'agenouilla")));
        tenure.TTenurePersist();

        LDraft? restored = tenure.TTenureUndo();

        Assert.True(TInterface.TRequestCardFind(restored!.LDraftContent, card)
            .LCardDraftSentence[0].LSentenceDraftExample!.LExampleDraftGloss[0].LGlossDraftText.LStateValueEmpty);
        Assert.True(tenure.TTenureStateRead().LTenureStateForward);
        tenure.TTenureCancel();
    }
}
