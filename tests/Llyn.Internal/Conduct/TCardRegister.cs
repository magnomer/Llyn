using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardRegister
{
    [Fact]
    public void RegisterAdd_TypedList_AddsTheCompletedNamesOnceAndKeepsTheRest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);

        string rest = card.CCardRegisterAdd(sheet, "formal,  slang, fo", 0, false).CProfferText;
        string settled = card.CCardRegisterAdd(sheet, "slang", 2, true).CProfferText;

        Assert.Equal("fo", rest);
        Assert.Equal(string.Empty, settled);
        Assert.Equal(
            ["formal", "slang"],
            TCardRegisterRead(desk, sheet).Select(static row => row.LRegisterDraftName.LStateValueShown));
    }

    [Fact]
    public void RegisterInsert_StoredRegister_LinksItOnceAndOfferLeavesItOut()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister formal = engine.TEngineRegisterCreate("formal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);
        long other = TCard.TCardSheetAdd(desk);

        card.CCardRegisterInsert(sheet, formal.LRegisterId, 0);
        card.CCardRegisterInsert(sheet, formal.LRegisterId, 1);

        Assert.Equal(formal.LRegisterId, Assert.Single(TCardRegisterRead(desk, sheet)).LRegisterDraftId);
        Assert.DoesNotContain(
            card.CCardRegisterAdd(sheet, "form", 1, false).CProfferRows,
            row => row.CProfferRowId == formal.LRegisterId);
        Assert.Contains(
            card.CCardRegisterAdd(other, "form", 0, false).CProfferRows,
            row => row.CProfferRowId == formal.LRegisterId);
    }

    [Fact]
    public void RegisterAdd_TypedPrefix_OffersTheStoredRegisterSplitAroundTheWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister slang = engine.TEngineRegisterCreate("slangy");
        (_, CCard card) = TCard.TCardPrepare(engine);

        CProffer offer = card.CCardRegisterAdd(0, " LAN", 0, false);

        Assert.Equal(" LAN", offer.CProfferText);
        Assert.True(offer.CProfferShown);
        Assert.Contains(new CProfferRow(slang.LRegisterId, "s", "lan", "gy"), offer.CProfferRows);
    }

    [Fact]
    public void RegisterAdd_BlankOrUnmatchedText_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRegisterCreate("slangy");
        (_, CCard card) = TCard.TCardPrepare(engine);

        CProffer blank = card.CCardRegisterAdd(0, "   ", 0, false);
        CProffer unmatched = card.CCardRegisterAdd(0, "zzq", 0, false);

        Assert.Equal("   ", blank.CProfferText);
        Assert.False(blank.CProfferShown);
        Assert.Empty(blank.CProfferRows);
        Assert.False(unmatched.CProfferShown);
        Assert.Empty(unmatched.CProfferRows);
    }

    [Fact]
    public void RegisterAdd_NineMatchingRegisters_OffersEight()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        for (int index = 1; index <= 9; index++)
        {
            engine.TEngineRegisterCreate("zone " + index);
        }

        (_, CCard card) = TCard.TCardPrepare(engine);

        CProffer offer = card.CCardRegisterAdd(0, "zone", 0, false);

        Assert.Equal(8, offer.CProfferRows.Count);
        Assert.All(offer.CProfferRows, static row => Assert.Equal("zone", row.CProfferRowMark));
    }

    [Fact]
    public void RegisterAdd_DraftInEnglish_OffersThePackRegisterOfThatLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        desk.TDeskDefer(TInterface.TRequestLanguageCreate(desk.CDeskId, "English"));

        CProffer offer = card.CCardRegisterAdd(0, "polit", 0, false);

        Assert.Contains(
            offer.CProfferRows,
            static row => row.CProfferRowMark + row.CProfferRowTail == "Polite");
    }

    private static IReadOnlyList<LRegisterDraft> TCardRegisterRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftRegister;
    }
}
