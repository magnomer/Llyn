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
        long sheet = TCard.TCardSheetAdd(desk);

        string rest = card.CCardSituationAdd(sheet, "Hearth, ,  Kitchen, Ga", 0, false).CProfferText;
        string settled = card.CCardSituationAdd(sheet, " Hearth ", 2, true).CProfferText;
        string blank = card.CCardSituationAdd(sheet, "   ", 2, true).CProfferText;

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
        long sheet = TCard.TCardSheetAdd(desk);

        card.CCardSituationInsert(sheet, hearth.LSituationId, 0);
        card.CCardSituationInsert(sheet, hearth.LSituationId, 1);

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
        long sheet = TCard.TCardSheetAdd(desk);
        long other = TCard.TCardSheetAdd(desk);
        card.CCardSituationInsert(sheet, hearth.LSituationId, 0);

        Assert.DoesNotContain(
            card.CCardSituationAdd(sheet, "Hear", 1, false).CProfferRows,
            row => row.CProfferRowId == hearth.LSituationId);
        Assert.Contains(
            card.CCardSituationAdd(other, "Hear", 0, false).CProfferRows,
            row => row.CProfferRowId == hearth.LSituationId);
    }

    [Fact]
    public void SituationAdd_TypedWord_OffersTheStoredSituationSplitAroundTheWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation hearth = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "Hearth", null, LStateValue.LStateValueUnknown));
        (_, CCard card) = TCard.TCardPrepare(engine);

        CProffer offer = card.CCardSituationAdd(0, " HEAR ", 0, false);

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
        (_, CCard card) = TCard.TCardPrepare(engine);

        CProffer blank = card.CCardSituationAdd(0, "   ", 0, false);
        CProffer unmatched = card.CCardSituationAdd(0, "zzq", 0, false);

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

        (_, CCard card) = TCard.TCardPrepare(engine);

        CProffer offer = card.CCardSituationAdd(0, "room", 0, false);

        Assert.Equal(8, offer.CProfferRows.Count);
        Assert.All(offer.CProfferRows, static row => Assert.Equal("Room", row.CProfferRowMark));
    }

    private static IReadOnlyList<LSituationDraft> TCardSituationRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftSituation;
    }
}
