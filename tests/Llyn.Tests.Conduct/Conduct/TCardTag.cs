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
        (_, CCard card) = TCard.TCardPrepare(engine);

        CSlate slate = card.CCardTagAdd(0, " ani", 0, false);

        Assert.Equal(" ani", slate.CSlateText);
        Assert.True(slate.CSlateShown);
        Assert.Contains(new CSlateRow(tag.LTagId, string.Empty, "ani", "mal"), slate.CSlateRows);
    }

    [Fact]
    public void TagAdd_BlankOrUnmatchedText_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineTagCreate("animal");
        (_, CCard card) = TCard.TCardPrepare(engine);

        CSlate blank = card.CCardTagAdd(0, "   ", 0, false);
        CSlate unmatched = card.CCardTagAdd(0, "zz", 0, false);

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

        (_, CCard card) = TCard.TCardPrepare(engine);

        CSlate slate = card.CCardTagAdd(0, "tag", 0, false);

        Assert.Equal(8, slate.CSlateRows.Count);
        Assert.Equal("tag 1", slate.CSlateRows[0].CSlateRowMark + slate.CSlateRows[0].CSlateRowTail);
    }

    [Fact]
    public void TagAdd_PaddedText_AddsTheTrimmedTagOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);

        card.CCardTagAdd(sheet, "  animal ", 0, true);
        card.CCardTagAdd(sheet, "animal", 1, true);
        card.CCardTagAdd(sheet, "   ", 1, true);

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
        long sheet = TCard.TCardSheetAdd(desk);

        card.CCardTagInsert(sheet, stored.LTagId, 0);
        card.CCardTagAdd(sheet, "animal", 1, true);

        LTagDraft held = Assert.Single(TCardTagRead(desk, sheet));
        Assert.Equal(stored.LTagId, held.LTagDraftId);
    }

    [Fact]
    public void TagRemove_AddedTag_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);
        card.CCardTagAdd(sheet, "animal", 0, true);

        card.CCardTagRemove(sheet, TCardTagRead(desk, sheet)[0].LTagDraftId);

        Assert.Empty(TCardTagRead(desk, sheet));
    }

    [Fact]
    public void TagAdd_TypedList_AddsTheCompletedTagsAndKeepsTheRest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);

        string kept = card.CCardTagAdd(sheet, " ani", 0, false).CSlateText;
        string rest = card.CCardTagAdd(sheet, "animal, , plant,  wi", 0, false).CSlateText;

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
        long sheet = TCard.TCardSheetAdd(desk);
        long other = TCard.TCardSheetAdd(desk);
        card.CCardTagInsert(sheet, tag.LTagId, 0);

        CSlateRow row = new(tag.LTagId, string.Empty, "ani", "mal");
        Assert.DoesNotContain(row, card.CCardTagAdd(sheet, "ani", 1, false).CSlateRows);
        Assert.Contains(row, card.CCardTagAdd(other, "ani", 0, false).CSlateRows);
    }

    private static IReadOnlyList<LTagDraft> TCardTagRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftTag;
    }
}
