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
    public void TagAdd_TypedPrefix_OffersTheStoredTagSplitAroundTheWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());

        CSlate slate = caret.CCaretTagAdd(0, " ani");

        Assert.Equal(" ani", slate.CSlateText);
        Assert.True(slate.CSlateShown);
        Assert.Contains(new CSlateRow(tag.LTagId, string.Empty, "ani", "mal"), slate.CSlateRows);
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

        caret.CCaretTagAdd(sheet, "animal, ");
        failing = false;

        Assert.Equal(["Input.UpdateFailed"], asked);
    }

    [Fact]
    public void TagAdd_BlankOrUnmatchedText_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());

        CSlate blank = caret.CCaretTagAdd(0, "   ");
        CSlate unmatched = caret.CCaretTagAdd(0, "zz");

        Assert.Equal("   ", blank.CSlateText);
        Assert.False(blank.CSlateShown);
        Assert.Empty(blank.CSlateRows);
        Assert.False(unmatched.CSlateShown);
        Assert.Empty(unmatched.CSlateRows);
    }

    [Fact]
    public void TagAdd_NineMatchingTags_OffersTheFirstEight()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        for (int index = 1; index <= 9; index++)
        {
            engine.TEngineTagCreate("tag " + index);
        }

        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());

        CSlate slate = caret.CCaretTagAdd(0, "tag");

        Assert.Equal(8, slate.CSlateRows.Count);
        Assert.Equal("tag 1", slate.CSlateRows[0].CSlateRowMark + slate.CSlateRows[0].CSlateRowTail);
    }

    [Fact]
    public void TagAdd_PaddedText_AddsTheTrimmedTagOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);

        caret.CCaretTagSettle(sheet, "  animal ");
        caret.CCaretTagSettle(sheet, "animal");
        caret.CCaretTagSettle(sheet, "   ");

        LTagDraft held = Assert.Single(TCardTagRead(desk, sheet));
        Assert.Equal("animal", held.LTagDraftText);
    }

    [Fact]
    public void TagInsert_StoredTag_LinksItUnderItsOwnId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag stored = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);

        caret.CCaretTagInsert(sheet, stored.LTagId);
        caret.CCaretTagSettle(sheet, "animal");

        LTagDraft held = Assert.Single(TCardTagRead(desk, sheet));
        Assert.Equal(stored.LTagId, held.LTagDraftId);
    }

    [Fact]
    public void TagRemove_AddedTag_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretTagSettle(sheet, "animal");

        card.CCardTagRemove(sheet, TCardTagRead(desk, sheet)[0].LTagDraftId);

        Assert.Empty(TCardTagRead(desk, sheet));
    }

    [Fact]
    public void TagAdd_TypedList_AddsTheCompletedTagsAndKeepsTheRest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);

        string kept = caret.CCaretTagAdd(sheet, " ani").CSlateText;
        string rest = caret.CCaretTagAdd(sheet, "animal, , plant,  wi").CSlateText;

        Assert.Equal(" ani", kept);
        Assert.Equal("wi", rest);
        Assert.Equal(["animal", "plant"], TCardTagRead(desk, sheet).Select(static tag => tag.LTagDraftText));
    }

    [Fact]
    public void TagAdd_TagHeldOnTheCard_LeavesItOutOfTheOffer()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        long other = TCard.TCardSheetAdd(desk);
        caret.CCaretTagInsert(sheet, tag.LTagId);

        CSlateRow row = new(tag.LTagId, string.Empty, "ani", "mal");
        Assert.DoesNotContain(row, caret.CCaretTagAdd(sheet, "ani").CSlateRows);
        Assert.Contains(row, caret.CCaretTagAdd(other, "ani").CSlateRows);
    }

    [Fact]
    public void TagInsert_StoredTag_LinksItAtTheCaretAndOfferLeavesItOut()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag stored = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretTagSettle(sheet, "plant, wild");
        caret.CCaretTag.CCaretTrailMove(sheet, -1);

        caret.CCaretTagInsert(sheet, stored.LTagId);
        caret.CCaretTagInsert(sheet, stored.LTagId);

        Assert.Equal(["plant", "animal", "wild"], TCardTagRead(desk, sheet).Select(static tag => tag.LTagDraftText));
        Assert.DoesNotContain(
            caret.CCaretTagAdd(sheet, "ani").CSlateRows, row => row.CSlateRowId == stored.LTagId);
    }

    [Fact]
    public void TagSettle_CaretMovedBack_AddsBeforeTheAnchor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretTagSettle(sheet, "plant, wild");
        caret.CCaretTag.CCaretTrailMove(sheet, -1);

        caret.CCaretTagSettle(sheet, "animal");

        IReadOnlyList<LTagDraft> held = TCardTagRead(desk, sheet);
        Assert.Equal(["plant", "animal", "wild"], held.Select(static tag => tag.LTagDraftText));
        Assert.Equal(held[2].LTagDraftId, caret.CCaretTag.CCaretTrailRead(sheet));
    }

    [Fact]
    public void TagDelete_StepEitherSide_RemovesTheTagBesideTheCaret()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretTagSettle(sheet, "plant, wild, animal");
        caret.CCaretTag.CCaretTrailMove(sheet, -1);

        caret.CCaretTag.CCaretTrailDelete(sheet, -1);
        IReadOnlyList<LTagDraft> before = TCardTagRead(desk, sheet);
        caret.CCaretTag.CCaretTrailDelete(sheet, 1);
        IReadOnlyList<LTagDraft> after = TCardTagRead(desk, sheet);
        caret.CCaretTag.CCaretTrailDelete(sheet, 1);

        Assert.Equal(["plant", "animal"], before.Select(static tag => tag.LTagDraftText));
        Assert.Equal(["plant"], after.Select(static tag => tag.LTagDraftText));
        Assert.Single(TCardTagRead(desk, sheet));
        Assert.Null(caret.CCaretTag.CCaretTrailRead(sheet));
    }

    private static IReadOnlyList<LTagDraft> TCardTagRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftTag;
    }
}
