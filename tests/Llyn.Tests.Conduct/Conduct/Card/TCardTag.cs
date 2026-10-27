using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardTag
{
    [Fact]
    public void TagAdd_TypedPrefix_OffersStoredTagSplit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());

        CProffer slate = caret.CCaretTagAdd(false, 0, " ani");

        Assert.Equal(" ani", slate.CProfferText);
        Assert.True(slate.CProfferShown);
        Assert.Contains(new CProfferRow(tag.LTagId, string.Empty, "ani", "mal"), slate.CProfferRows);
    }

    [Fact]
    public void TagAdd_AddFailure_ShowsUpdateFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);
        CDesk desk = TInterfaceConductDesk.TDeskCreate(engine, "Input", envoy, "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(null);
        CCaret caret = new(desk, TInterfaceConductCard.TCardCreate(engine, desk, envoy), new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        TCard.TCardSheetAdd(desk);
        desk.CDeskChronicle.CDeskChronicleUndo();
        bool failing = true;
        engine.TEngineObserverAttach(bulletin =>
        {
            if (failing && bulletin.LBulletinSubject == LSubject.LSubjectTenure)
            {
                throw new InvalidOperationException("The tenure observer failed.");
            }
        });

        caret.CCaretTagAdd(false, sheet, "animal, ");
        failing = false;

        Assert.Equal(["Input.UpdateFailed"], asked);
    }

    [Fact]
    public void TagSettle_ObserverFailsAfterFirstPart_KeepsUntriedPart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);
        CDesk desk = TInterfaceConductDesk.TDeskCreate(engine, "Input", envoy, "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(null);
        CCaret caret = new(desk, TInterfaceConductCard.TCardCreate(engine, desk, envoy), new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        TCard.TCardSheetAdd(desk);
        desk.CDeskChronicle.CDeskChronicleUndo();
        bool failing = true;
        engine.TEngineObserverAttach(bulletin =>
        {
            if (failing && bulletin.LBulletinSubject == LSubject.LSubjectTenure)
            {
                throw new InvalidOperationException("The tenure observer failed.");
            }
        });

        string kept = caret.CCaretTagSettle(false, sheet, "dry,rain").CProfferText;
        failing = false;

        Assert.Equal("rain", kept);
        Assert.Equal(["Input.UpdateFailed"], asked);
        Assert.Equal(["dry"], TCardTagRead(desk, sheet).Select(static tag => tag.LTagDraftText));
    }

    [Fact]
    public void TagSettle_RefusedPart_KeepsPartAsTyped()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);

        string kept = caret.CCaretTagSettle(false, sheet + 100, " dry ").CProfferText;

        Assert.Equal("dry", kept);
        Assert.Empty(TCardTagRead(desk, sheet));
    }

    [Fact]
    public void TagAdd_EchoOfLastAnswer_KeepsLantern()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        CProffer offer = caret.CCaretTagAdd(false, sheet, "ani");
        caret.CCaretLanternMove(1);

        CProffer echo = caret.CCaretTagAdd(false, sheet, offer.CProfferText);

        Assert.Same(offer, echo);
        Assert.True(caret.CCaretLanternLit);
        Assert.Equal(tag.LTagId, echo.CProfferRows[caret.CCaretLanternIndex].CProfferRowId);
        Assert.Empty(TCardTagRead(desk, sheet));
    }

    [Fact]
    public void TagAdd_BlankOrUnmatchedText_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());

        CProffer blank = caret.CCaretTagAdd(false, 0, "   ");
        CProffer unmatched = caret.CCaretTagAdd(false, 0, "zz");

        Assert.Equal("   ", blank.CProfferText);
        Assert.False(blank.CProfferShown);
        Assert.Empty(blank.CProfferRows);
        Assert.False(unmatched.CProfferShown);
        Assert.Empty(unmatched.CProfferRows);
    }

    [Fact]
    public void TagAdd_NineMatchingTags_OffersFirstEight()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        for (int index = 1; index <= 9; index++)
        {
            engine.TEngineTagCreate("tag " + index);
        }

        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());

        CProffer slate = caret.CCaretTagAdd(false, 0, "tag");

        Assert.Equal(8, slate.CProfferRows.Count);
        Assert.Equal("tag 1", slate.CProfferRows[0].CProfferRowMark + slate.CProfferRows[0].CProfferRowTail);
    }

    [Fact]
    public void TagAdd_PaddedText_AddsTrimmedTagOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);

        caret.CCaretTagSettle(false, sheet, "  animal ");
        caret.CCaretTagSettle(false, sheet, "animal");
        caret.CCaretTagSettle(false, sheet, "   ");

        LTagDraft held = Assert.Single(TCardTagRead(desk, sheet));
        Assert.Equal("animal", held.LTagDraftText);
    }

    [Fact]
    public void TagInsert_StoredTag_LinksUnderOwnId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag stored = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);

        caret.CCaretTagInsert(false, sheet, stored.LTagId);
        caret.CCaretTagSettle(false, sheet, "animal");

        LTagDraft held = Assert.Single(TCardTagRead(desk, sheet));
        Assert.Equal(stored.LTagId, held.LTagDraftId);
    }

    [Fact]
    public void TagRemove_AddedTag_DropsTag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretTagSettle(false, sheet, "animal");

        card.CCardTagRemove(false, sheet, TCardTagRead(desk, sheet)[0].LTagDraftId);

        Assert.Empty(TCardTagRead(desk, sheet));
    }

    [Fact]
    public void TagAdd_TypedList_AddsCompletedTagsKeepsRest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);

        string kept = caret.CCaretTagAdd(false, sheet, " ani").CProfferText;
        string rest = caret.CCaretTagAdd(false, sheet, "animal, , plant,  wi").CProfferText;

        Assert.Equal(" ani", kept);
        Assert.Equal("wi", rest);
        Assert.Equal(["animal", "plant"], TCardTagRead(desk, sheet).Select(static tag => tag.LTagDraftText));
    }

    [Fact]
    public void TagAdd_TagHeldOnCard_LeavesOutOfOffer()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        long other = TCard.TCardSheetAdd(desk);
        caret.CCaretTagInsert(false, sheet, tag.LTagId);

        CProfferRow row = new(tag.LTagId, string.Empty, "ani", "mal");
        Assert.DoesNotContain(row, caret.CCaretTagAdd(false, sheet, "ani").CProfferRows);
        Assert.Contains(row, caret.CCaretTagAdd(false, other, "ani").CProfferRows);
    }

    [Fact]
    public void TagInsert_StoredTag_LinksAtCaretOfferLeavesOut()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag stored = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretTagSettle(false, sheet, "plant, wild");
        caret.CCaretTag.CCaretTrailMove(false, sheet, -1);

        caret.CCaretTagInsert(false, sheet, stored.LTagId);
        caret.CCaretTagInsert(false, sheet, stored.LTagId);

        Assert.Equal(["plant", "animal", "wild"], TCardTagRead(desk, sheet).Select(static tag => tag.LTagDraftText));
        Assert.DoesNotContain(
            caret.CCaretTagAdd(false, sheet, "ani").CProfferRows, row => row.CProfferRowId == stored.LTagId);
    }

    [Fact]
    public void TagSettle_CaretMovedBack_AddsBeforeAnchor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretTagSettle(false, sheet, "plant, wild");
        caret.CCaretTag.CCaretTrailMove(false, sheet, -1);

        caret.CCaretTagSettle(false, sheet, "animal");

        IReadOnlyList<LTagDraft> held = TCardTagRead(desk, sheet);
        Assert.Equal(["plant", "animal", "wild"], held.Select(static tag => tag.LTagDraftText));
        Assert.Equal(held[2].LTagDraftId, caret.CCaretTag.CCaretTrailRead(false, sheet));
    }

    [Fact]
    public void CaretTrailDelete_StepEitherSide_RemovesBesideCaret()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretTagSettle(false, sheet, "plant, wild, animal");
        caret.CCaretTag.CCaretTrailMove(false, sheet, -1);

        caret.CCaretTag.CCaretTrailDelete(false, sheet, -1);
        IReadOnlyList<LTagDraft> before = TCardTagRead(desk, sheet);
        caret.CCaretTag.CCaretTrailDelete(false, sheet, 1);
        IReadOnlyList<LTagDraft> after = TCardTagRead(desk, sheet);
        caret.CCaretTag.CCaretTrailDelete(false, sheet, 1);

        Assert.Equal(["plant", "animal"], before.Select(static tag => tag.LTagDraftText));
        Assert.Equal(["plant"], after.Select(static tag => tag.LTagDraftText));
        Assert.Single(TCardTagRead(desk, sheet));
        Assert.Null(caret.CCaretTag.CCaretTrailRead(false, sheet));
    }

    private static IReadOnlyList<LTagDraft> TCardTagRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftTag;
    }
}
