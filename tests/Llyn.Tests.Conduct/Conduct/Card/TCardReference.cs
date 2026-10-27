using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardReference
{
    [Fact]
    public void ReferenceFind_TypedWord_OffersStoredSourceSplit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference notes = engine.TEngineCitationCreate("Field notes");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);

        CProffer offer = card.CCardReferenceFind(false, sheet, sentence, " FIELD ");

        Assert.Equal(" FIELD ", offer.CProfferText);
        Assert.True(offer.CProfferShown);
        Assert.Contains(new CProfferRow(notes.LReferenceId, string.Empty, "Field", " notes"), offer.CProfferRows);
    }

    [Fact]
    public void ReferenceFind_BlankOrUnmatchedText_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineCitationCreate("Field notes");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);

        CProffer blank = card.CCardReferenceFind(false, sheet, sentence, "   ");
        CProffer unmatched = card.CCardReferenceFind(false, sheet, sentence, "zzq");

        Assert.Equal("   ", blank.CProfferText);
        Assert.False(blank.CProfferShown);
        Assert.Empty(blank.CProfferRows);
        Assert.False(unmatched.CProfferShown);
        Assert.Empty(unmatched.CProfferRows);
    }

    [Fact]
    public void ReferenceFind_BylineSentenceCites_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference notes = engine.TEngineCitationCreate("Field notes");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);
        (long other, long uncited) = TCard.TCardSentenceAdd(desk);
        card.CCardCitationSet(false, sheet, sentence, "Field notes");
        string byline = card.CCardReferenceRead()
            .Single(row => row.CCatalogReferenceId == notes.LReferenceId).CCatalogReferenceByline;

        CProffer cited = card.CCardReferenceFind(false, sheet, sentence, " " + byline + " ");

        Assert.False(cited.CProfferShown);
        Assert.Empty(cited.CProfferRows);
        Assert.Contains(
            card.CCardReferenceFind(false, other, uncited, byline).CProfferRows,
            row => row.CProfferRowId == notes.LReferenceId);
    }

    [Fact]
    public void CitationRead_CitedAndUncitedSentence_HandsStoredTitle()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference notes = engine.TEngineCitationCreate("Field notes");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);
        (long other, long uncited) = TCard.TCardSentenceAdd(desk);
        card.CCardCitationSet(false, sheet, sentence, "Field notes");
        string byline = card.CCardReferenceRead()
            .Single(row => row.CCatalogReferenceId == notes.LReferenceId).CCatalogReferenceByline;

        Assert.Equal(byline, card.CCardCitationRead(false, sheet, sentence));
        Assert.Equal(string.Empty, card.CCardCitationRead(false, other, uncited));
    }

    [Fact]
    public void CaretReferenceFind_StoredTitle_OffersNoneKeepsLantern()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineCitationCreate("Field notes");
        LTag tag = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);
        card.CCardCitationSet(false, sheet, sentence, "Field notes");
        string title = card.CCardCitationRead(false, sheet, sentence);
        CProffer offer = caret.CCaretTagAdd(false, sheet, "ani");
        caret.CCaretLanternMove(1);

        CProffer? echo = caret.CCaretReferenceFind(false, sheet, sentence, title);

        Assert.Null(echo);
        Assert.True(caret.CCaretLanternLit);
        Assert.Equal(tag.LTagId, offer.CProfferRows[caret.CCaretLanternIndex].CProfferRowId);
    }

    [Fact]
    public void ReferenceFind_NineMatchingSources_OffersEight()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        for (int index = 1; index <= 9; index++)
        {
            engine.TEngineCitationCreate("Diary " + index);
        }

        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);

        CProffer offer = card.CCardReferenceFind(false, sheet, sentence, "diary");

        Assert.Equal(8, offer.CProfferRows.Count);
        Assert.All(offer.CProfferRows, static row => Assert.Equal("Diary", row.CProfferRowMark));
    }

    [Fact]
    public void CitationSet_EngineFails_ShowsCreateFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        List<string> asked = [];
        CDesk desk = TInterfaceCitation.TDeskFailCreate(engine, TEnvoyFake.TEnvoyCreate(false, []));
        desk.CDeskStart(null);
        CCard card = TInterfaceConductCard.TCardCreate(engine, desk, TEnvoyFake.TEnvoyCreate(false, asked));
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);

        card.CCardCitationSet(false, sheet, sentence, "Field notes");

        Assert.Equal(["Reference.CreateFailed"], asked);
        LExampleDraft? example = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet)
            .LCardDraftSentence.Single(row => row.LSentenceDraftId == sentence).LSentenceDraftExample;
        Assert.Null(example?.LExampleDraftReference.LStateAnchorShown);
    }

    [Fact]
    public void ReferenceRead_EngineFails_ShowsLoadFailureAnswersNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        List<string> asked = [];
        CDesk desk = TInterfaceConductDesk.TDeskCreate(
            engine, "Input", TEnvoyFake.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        CCard card = TInterfaceCitation.TCardFailCreate(engine, desk, TEnvoyFake.TEnvoyCreate(false, asked));

        IReadOnlyList<CCatalogReference> read = card.CCardReferenceRead();

        Assert.Empty(read);
        Assert.Equal(["Reference.LoadFailed"], asked);
    }

    [Fact]
    public void ReferenceFind_EngineFails_ShowsFindFailureOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        List<string> asked = [];
        CDesk desk = TInterfaceCitation.TDeskFailCreate(engine, TEnvoyFake.TEnvoyCreate(false, asked));
        desk.CDeskStart(null);
        CCard card = TInterfaceConductCard.TCardCreate(engine, desk, TEnvoyFake.TEnvoyCreate(false, []));

        CProffer first = card.CCardReferenceFind(false, 0, 0, "Field");
        CProffer second = card.CCardReferenceFind(false, 0, 0, "Field");

        Assert.Equal(["Input.FindFailed"], asked);
        Assert.Empty(first.CProfferRows);
        Assert.Empty(second.CProfferRows);
    }

    [Fact]
    public void CitationSet_TypedTitle_CitesResolvedSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);

        card.CCardCitationSet(false, sheet, sentence, "Field notes");

        LExampleDraft? example = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet)
            .LCardDraftSentence.Single(row => row.LSentenceDraftId == sentence).LSentenceDraftExample;
        long? cited = example?.LExampleDraftReference.LStateAnchorShown;
        Assert.NotNull(cited);
        Assert.Equal("Field notes", engine.TEngineCitationRead()[cited!.Value]);
    }

    [Fact]
    public void ReferenceFind_NoWord_ListsEveryStoredSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference notes = engine.TEngineCitationCreate("Field notes");
        (_, CCard card) = TCard.TCardPrepare(engine);

        Assert.Contains(card.CCardReferenceRead(), row => row.CCatalogReferenceId == notes.LReferenceId);
    }
}
