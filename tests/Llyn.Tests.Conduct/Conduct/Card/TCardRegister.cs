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
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);

        string rest = caret.CCaretRegisterAdd(sheet, "formal,  slang, fo").CProfferText;
        string settled = caret.CCaretRegisterSettle(sheet, "slang").CProfferText;

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
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        long other = TCard.TCardSheetAdd(desk);

        caret.CCaretRegisterInsert(sheet, formal.LRegisterId);
        caret.CCaretRegisterInsert(sheet, formal.LRegisterId);

        Assert.Equal(formal.LRegisterId, Assert.Single(TCardRegisterRead(desk, sheet)).LRegisterDraftId);
        Assert.DoesNotContain(
            caret.CCaretRegisterAdd(sheet, "form").CProfferRows,
            row => row.CProfferRowId == formal.LRegisterId);
        Assert.Contains(
            caret.CCaretRegisterAdd(other, "form").CProfferRows,
            row => row.CProfferRowId == formal.LRegisterId);
    }

    [Fact]
    public void RegisterRemove_LinkedRegister_DropsItAndKeepsTheOthers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(sheet, "formal, slang");
        IReadOnlyList<LRegisterDraft> linked = TCardRegisterRead(desk, sheet);

        card.CCardRegisterRemove(sheet, linked[0].LRegisterDraftId);

        Assert.Equal(
            ["slang"],
            TCardRegisterRead(desk, sheet).Select(static row => row.LRegisterDraftName.LStateValueShown));
    }

    [Fact]
    public void RegisterAdd_TypedPrefix_OffersTheStoredRegisterSplitAroundTheWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister slang = engine.TEngineRegisterCreate("slangy");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());

        CProffer offer = caret.CCaretRegisterAdd(0, " LAN");

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
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());

        CProffer blank = caret.CCaretRegisterAdd(0, "   ");
        CProffer unmatched = caret.CCaretRegisterAdd(0, "zzq");

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

        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());

        CProffer offer = caret.CCaretRegisterAdd(0, "zone");

        Assert.Equal(8, offer.CProfferRows.Count);
        Assert.All(offer.CProfferRows, static row => Assert.Equal("zone", row.CProfferRowMark));
    }

    [Fact]
    public void RegisterAdd_DraftInEnglish_OffersThePackRegisterOfThatLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        desk.TDeskDefer(TInterface.TRequestLanguageCreate(desk.CDeskId, "English"));

        CProffer offer = caret.CCaretRegisterAdd(0, "polit");

        Assert.Contains(
            offer.CProfferRows,
            static row => row.CProfferRowMark + row.CProfferRowTail == "Polite");
    }

    [Fact]
    public void RegisterInsert_StoredRegister_LinksItAtTheCaret()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister stored = engine.TEngineRegisterCreate("formal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(sheet, "slang, wild");
        caret.CCaretRegister.CCaretTrailMove(sheet, -1);

        caret.CCaretRegisterInsert(sheet, stored.LRegisterId);

        Assert.Equal(
            ["slang", "formal", "wild"],
            TCardRegisterRead(desk, sheet).Select(static row => row.LRegisterDraftName.LStateValueShown));
    }

    [Fact]
    public void RegisterSettle_CaretMovedBack_AddsBeforeTheAnchor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(sheet, "slang, wild");
        caret.CCaretRegister.CCaretTrailMove(sheet, -1);

        caret.CCaretRegisterSettle(sheet, "formal");

        IReadOnlyList<LRegisterDraft> held = TCardRegisterRead(desk, sheet);
        Assert.Equal(
            ["slang", "formal", "wild"], held.Select(static row => row.LRegisterDraftName.LStateValueShown));
        Assert.Equal(held[2].LRegisterDraftId, caret.CCaretRegister.CCaretTrailRead(sheet));
    }

    [Fact]
    public void RegisterDelete_StepEitherSide_RemovesTheRegisterBesideTheCaret()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(sheet, "slang, wild, formal");
        caret.CCaretRegister.CCaretTrailMove(sheet, -1);

        caret.CCaretRegister.CCaretTrailDelete(sheet, -1);
        IReadOnlyList<LRegisterDraft> before = TCardRegisterRead(desk, sheet);
        caret.CCaretRegister.CCaretTrailDelete(sheet, 1);
        IReadOnlyList<LRegisterDraft> after = TCardRegisterRead(desk, sheet);
        caret.CCaretRegister.CCaretTrailDelete(sheet, 1);

        Assert.Equal(["slang", "formal"], before.Select(static row => row.LRegisterDraftName.LStateValueShown));
        Assert.Equal(["slang"], after.Select(static row => row.LRegisterDraftName.LStateValueShown));
        Assert.Single(TCardRegisterRead(desk, sheet));
        Assert.Null(caret.CCaretRegister.CCaretTrailRead(sheet));
    }

    private static IReadOnlyList<LRegisterDraft> TCardRegisterRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftRegister;
    }
}
