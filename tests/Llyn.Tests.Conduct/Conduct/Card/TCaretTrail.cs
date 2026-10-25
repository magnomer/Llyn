using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCaretTrail
{
    [Fact]
    public void TrailMove_StepPastEitherEnd_RefusesAndKeepsTheAnchor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(false, sheet, "slang, wild");
        long slang = TCaretTrailRead(desk, sheet)[0];

        (bool CCaretMoved, long? CCaretAnchor) forward = caret.CCaretRegister.CCaretTrailMove(false, sheet, 1);
        (bool CCaretMoved, long? CCaretAnchor) start = caret.CCaretRegister.CCaretTrailMove(false, sheet, -2);
        (bool CCaretMoved, long? CCaretAnchor) backward = caret.CCaretRegister.CCaretTrailMove(false, sheet, -1);

        Assert.Equal((false, (long?)null), forward);
        Assert.Equal((true, (long?)slang), start);
        Assert.Equal((false, (long?)slang), backward);
        Assert.Equal(slang, caret.CCaretRegister.CCaretTrailRead(false, sheet));
    }

    [Fact]
    public void TrailRead_AnchorRemoved_ReadsTheNextAnchor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(false, sheet, "slang, wild, formal");
        IReadOnlyList<long> held = TCaretTrailRead(desk, sheet);
        caret.CCaretRegister.CCaretTrailMove(false, sheet, -2);

        card.CCardRegisterRemove(false, sheet, held[1]);

        Assert.Equal(held[2], caret.CCaretRegister.CCaretTrailRead(false, sheet));
        Assert.Equal((true, (long?)null), caret.CCaretRegister.CCaretTrailMove(false, sheet, 1));
    }

    [Fact]
    public void TrailRead_RegisterAddedBeforeTheAnchor_KeepsTheAnchor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        CCaret other = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(false, sheet, "slang, wild");
        long wild = TCaretTrailRead(desk, sheet)[1];
        caret.CCaretRegister.CCaretTrailMove(false, sheet, -1);
        other.CCaretRegister.CCaretTrailMove(false, sheet, -2);

        other.CCaretRegisterSettle(false, sheet, "formal");

        Assert.Equal(wild, TCaretTrailRead(desk, sheet)[2]);
        Assert.Equal(wild, caret.CCaretRegister.CCaretTrailRead(false, sheet));
        card.CCardRegisterRemove(false, sheet, wild);
        Assert.Null(caret.CCaretRegister.CCaretTrailRead(false, sheet));
    }

    [Fact]
    public void TrailDelete_StepPastEitherEnd_RemovesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(false, sheet, "slang, wild");
        IReadOnlyList<long> held = TCaretTrailRead(desk, sheet);

        caret.CCaretRegister.CCaretTrailDelete(false, sheet, 1);
        caret.CCaretRegister.CCaretTrailMove(false, sheet, -2);
        caret.CCaretRegister.CCaretTrailDelete(false, sheet, -1);

        Assert.Equal(held, TCaretTrailRead(desk, sheet));
        Assert.Equal(held[0], caret.CCaretRegister.CCaretTrailRead(false, sheet));
    }

    [Fact]
    public void CaretRead_RegisterMovedBack_MapsEveryTrail()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        CCaret caret = new(desk, card, new CLedgerNoticed());
        long sheet = TCard.TCardSheetAdd(desk);
        caret.CCaretRegisterSettle(false, sheet, "slang, wild");
        caret.CCaretRegister.CCaretTrailMove(false, sheet, -1);

        IReadOnlyDictionary<CCaretTrail, long?> anchor = caret.CCaretRead(false, sheet);

        Assert.Equal(4, anchor.Count);
        Assert.Equal(TCaretTrailRead(desk, sheet)[1], anchor[caret.CCaretRegister]);
        Assert.Null(anchor[caret.CCaretSituation]);
        Assert.Null(anchor[caret.CCaretTag]);
        Assert.Null(anchor[caret.CCaretTranslation]);
    }

    private static IReadOnlyList<long> TCaretTrailRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet)
            .LCardDraftRegister.Select(static row => row.LRegisterDraftId)
            .ToList();
    }
}
