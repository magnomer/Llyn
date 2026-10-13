using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardSituation
{
    [Fact]
    public void SituationAdd_TypedList_AddsTheCompletedTitlesOnceAndKeepsTheRest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card);
        long sheet = TCard.TCardSheetAdd(desk);

        string rest = caret.CCaretSituationAdd(sheet, "Hearth, ,  Kitchen, Ga").CProfferText;
        string settled = caret.CCaretSituationSettle(sheet, " Hearth ").CProfferText;
        string blank = caret.CCaretSituationSettle(sheet, "   ").CProfferText;

        Assert.Equal("Ga", rest);
        Assert.Equal(string.Empty, settled);
        Assert.Equal(string.Empty, blank);
        Assert.Equal(
            ["Hearth", "Kitchen"],
            TCardSituationRead(desk, sheet).Select(static row => row.LSituationDraftTitle.LStateValueShown));
    }

    [Fact]
    public void SituationInsert_StoredSituation_LinksItOnceAndRemoveDropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation hearth = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "Hearth", null, null));
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card);
        long sheet = TCard.TCardSheetAdd(desk);

        caret.CCaretSituationInsert(sheet, hearth.LSituationId);
        caret.CCaretSituationInsert(sheet, hearth.LSituationId);

        Assert.Equal(hearth.LSituationId, Assert.Single(TCardSituationRead(desk, sheet)).LSituationDraftId);

        card.CCardSituationRemove(sheet, hearth.LSituationId);

        Assert.Empty(TCardSituationRead(desk, sheet));
    }

    [Fact]
    public void SituationInsert_StoredSituation_LinksItOnceAndOfferLeavesItOut()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation hearth = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "Hearth", null, null));
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card);
        long sheet = TCard.TCardSheetAdd(desk);
        long other = TCard.TCardSheetAdd(desk);
        caret.CCaretSituationInsert(sheet, hearth.LSituationId);

        Assert.DoesNotContain(
            caret.CCaretSituationAdd(sheet, "Hear").CProfferRows,
            row => row.CProfferRowId == hearth.LSituationId);
        Assert.Contains(
            caret.CCaretSituationAdd(other, "Hear").CProfferRows,
            row => row.CProfferRowId == hearth.LSituationId);
    }

    [Fact]
    public void SituationAdd_TypedWord_OffersTheStoredSituationSplitAroundTheWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation hearth = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "Hearth", null, LStateValue.LStateValueUnknown));
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card);

        CProffer offer = caret.CCaretSituationAdd(0, " HEAR ");

        Assert.Equal(" HEAR ", offer.CProfferText);
        Assert.True(offer.CProfferShown);
        Assert.Contains(new CProfferRow(hearth.LSituationId, string.Empty, "Hear", "th"), offer.CProfferRows);
    }

    [Fact]
    public void SituationAdd_BlankOrUnmatchedText_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "Hearth", null, null));
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card);

        CProffer blank = caret.CCaretSituationAdd(0, "   ");
        CProffer unmatched = caret.CCaretSituationAdd(0, "zzq");

        Assert.Equal("   ", blank.CProfferText);
        Assert.False(blank.CProfferShown);
        Assert.Empty(blank.CProfferRows);
        Assert.False(unmatched.CProfferShown);
        Assert.Empty(unmatched.CProfferRows);
    }

    [Fact]
    public void SituationAdd_NineMatchingSituations_OffersEight()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        for (int index = 1; index <= 9; index++)
        {
            engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "Room " + index, null, null));
        }

        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card);

        CProffer offer = caret.CCaretSituationAdd(0, "room");

        Assert.Equal(8, offer.CProfferRows.Count);
        Assert.All(offer.CProfferRows, static row => Assert.Equal("Room", row.CProfferRowMark));
    }

    [Fact]
    public void SituationSettle_CaretMovedBack_AddsBeforeTheAnchor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card);
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretSituationSettle(sheet, "Hearth, Kitchen");
        caret.CCaretSituation.CCaretTrailMove(sheet, -1);

        caret.CCaretSituationSettle(sheet, "Garden");

        IReadOnlyList<LSituationDraft> held = TCardSituationRead(desk, sheet);
        Assert.Equal(
            ["Hearth", "Garden", "Kitchen"],
            held.Select(static row => row.LSituationDraftTitle.LStateValueShown));
        Assert.Equal(held[2].LSituationDraftId, caret.CCaretSituation.CCaretTrailRead(sheet));
    }

    [Fact]
    public void SituationDelete_StepEitherSide_RemovesTheSituationBesideTheCaret()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card);
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretSituationSettle(sheet, "Hearth, Kitchen, Garden");
        caret.CCaretSituation.CCaretTrailMove(sheet, -1);

        caret.CCaretSituation.CCaretTrailDelete(sheet, -1);
        IReadOnlyList<LSituationDraft> before = TCardSituationRead(desk, sheet);
        caret.CCaretSituation.CCaretTrailDelete(sheet, 1);
        IReadOnlyList<LSituationDraft> after = TCardSituationRead(desk, sheet);
        caret.CCaretSituation.CCaretTrailDelete(sheet, 1);

        Assert.Equal(["Hearth", "Garden"], before.Select(static row => row.LSituationDraftTitle.LStateValueShown));
        Assert.Equal(["Hearth"], after.Select(static row => row.LSituationDraftTitle.LStateValueShown));
        Assert.Single(TCardSituationRead(desk, sheet));
        Assert.Null(caret.CCaretSituation.CCaretTrailRead(sheet));
    }

    private static IReadOnlyList<LSituationDraft> TCardSituationRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftSituation;
    }
}
