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
    public void RegisterAdd_TypedList_AddsCompletedNamesKeepsRest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);

        string rest = caret.CCaretRegisterAdd(false, sheet, "formal,  slang, fo").CProfferText;
        string settled = caret.CCaretRegisterSettle(false, sheet, "slang").CProfferText;

        Assert.Equal("fo", rest);
        Assert.Equal(string.Empty, settled);
        Assert.Equal(
            ["formal", "slang"],
            TCardRegisterRead(desk, sheet).Select(static row => row.LRegisterDraftName.LStateValueShown));
    }

    [Fact]
    public void RegisterInsert_StoredRegister_LinksOnceOfferOmits()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister formal = engine.TEngineRegisterCreate("formal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        long other = TCard.TCardSheetAdd(desk);

        caret.CCaretRegisterInsert(false, sheet, formal.LRegisterId);
        caret.CCaretRegisterInsert(false, sheet, formal.LRegisterId);

        Assert.Equal(formal.LRegisterId, Assert.Single(TCardRegisterRead(desk, sheet)).LRegisterDraftId);
        Assert.DoesNotContain(
            caret.CCaretRegisterAdd(false, sheet, "form").CProfferRows,
            row => row.CProfferRowId == formal.LRegisterId);
        Assert.Contains(
            caret.CCaretRegisterAdd(false, other, "form").CProfferRows,
            row => row.CProfferRowId == formal.LRegisterId);
    }

    [Fact]
    public void RegisterRemove_LinkedRegister_DropsOneKeepsOthers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(false, sheet, "formal, slang");
        IReadOnlyList<LRegisterDraft> linked = TCardRegisterRead(desk, sheet);

        card.CCardRegisterRemove(false, sheet, linked[0].LRegisterDraftId);

        Assert.Equal(
            ["slang"],
            TCardRegisterRead(desk, sheet).Select(static row => row.LRegisterDraftName.LStateValueShown));
    }

    [Fact]
    public void RegisterAdd_TypedPrefix_OffersStoredRegisterSplit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister slang = engine.TEngineRegisterCreate("slangy");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());

        CProffer offer = caret.CCaretRegisterAdd(false, 0, " LAN");

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

        CProffer blank = caret.CCaretRegisterAdd(false, 0, "   ");
        CProffer unmatched = caret.CCaretRegisterAdd(false, 0, "zzq");

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

        CProffer offer = caret.CCaretRegisterAdd(false, 0, "zone");

        Assert.Equal(8, offer.CProfferRows.Count);
        Assert.All(offer.CProfferRows, static row => Assert.Equal("zone", row.CProfferRowMark));
    }

    [Fact]
    public void RegisterAdd_DraftInEnglish_OffersPackRegister()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        desk.TDeskDefer(TInterface.TRequestLanguageCreate(desk.CDeskId, "English"));

        CProffer offer = caret.CCaretRegisterAdd(false, 0, "polit");

        Assert.Contains(
            offer.CProfferRows,
            static row => row.CProfferRowMark + row.CProfferRowTail == "Polite");
    }

    [Fact]
    public void RegisterInsert_StoredRegister_LinksAtCaret()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister stored = engine.TEngineRegisterCreate("formal");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(false, sheet, "slang, wild");
        caret.CCaretRegister.CCaretTrailMove(false, sheet, -1);

        caret.CCaretRegisterInsert(false, sheet, stored.LRegisterId);

        Assert.Equal(
            ["slang", "formal", "wild"],
            TCardRegisterRead(desk, sheet).Select(static row => row.LRegisterDraftName.LStateValueShown));
    }

    [Fact]
    public void RegisterSettle_CaretMovedBack_AddsBeforeAnchor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(false, sheet, "slang, wild");
        caret.CCaretRegister.CCaretTrailMove(false, sheet, -1);

        caret.CCaretRegisterSettle(false, sheet, "formal");

        IReadOnlyList<LRegisterDraft> held = TCardRegisterRead(desk, sheet);
        Assert.Equal(
            ["slang", "formal", "wild"], held.Select(static row => row.LRegisterDraftName.LStateValueShown));
        Assert.Equal(held[2].LRegisterDraftId, caret.CCaretRegister.CCaretTrailRead(false, sheet));
    }

    [Fact]
    public void CaretTrailDelete_StepEitherSide_RemovesBesideCaret()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(false, sheet, "slang, wild, formal");
        caret.CCaretRegister.CCaretTrailMove(false, sheet, -1);

        caret.CCaretRegister.CCaretTrailDelete(false, sheet, -1);
        IReadOnlyList<LRegisterDraft> before = TCardRegisterRead(desk, sheet);
        caret.CCaretRegister.CCaretTrailDelete(false, sheet, 1);
        IReadOnlyList<LRegisterDraft> after = TCardRegisterRead(desk, sheet);
        caret.CCaretRegister.CCaretTrailDelete(false, sheet, 1);

        Assert.Equal(["slang", "formal"], before.Select(static row => row.LRegisterDraftName.LStateValueShown));
        Assert.Equal(["slang"], after.Select(static row => row.LRegisterDraftName.LStateValueShown));
        Assert.Single(TCardRegisterRead(desk, sheet));
        Assert.Null(caret.CCaretRegister.CCaretTrailRead(false, sheet));
    }

    private static IReadOnlyList<LRegisterDraft> TCardRegisterRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftRegister;
    }
}
