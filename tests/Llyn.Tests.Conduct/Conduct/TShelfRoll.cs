using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TShelfRoll
{
    [Fact]
    public void ShelfRollRead_CitedSourceChosen_CarriesItsTallyWithTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = TShelfRollPrepare(engine);
        shelf.CShelfReferenceSelect(book.LReferenceId);

        CShelfRoll roll = shelf.CShelfRollRead();

        Assert.False(roll.CShelfRollEmpty);
        Assert.Equal(
            ["Book"],
            roll.CShelfRollRows.Where(row => row.CCatalogReferenceChosen).Select(row => row.CCatalogReferenceName));
        Assert.Equal(TInterface.TLocalizationTextRead("Source.UsageOne"), roll.CShelfRollTally);
    }

    [Fact]
    public void ShelfRollRead_ChosenSourceFilteredOut_WordsTheTallyAfterTheClose()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = TShelfRollPrepare(engine);
        shelf.CShelfReferenceSelect(book.LReferenceId);
        shelf.CShelfQuerySet("zzz");

        CShelfRoll roll = shelf.CShelfRollRead();

        Assert.True(roll.CShelfRollEmpty);
        Assert.Empty(roll.CShelfRollRows);
        Assert.Equal(TInterface.TLocalizationTextRead("Source.UsageNone"), roll.CShelfRollTally);
        Assert.False(shelf.CShelfBinEnabled);
    }

    [Fact]
    public void ShelfRollRead_UnknownAndUnsetValues_WordsEachByItsKey()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        TShelfWordingPrepare(engine, "Lost", LStateValue.LStateValueUnknown, LStateMark.LStateMarkUnknown);
        TShelfWordingPrepare(engine, "Blank", LStateValue.LStateValueUnspecified, LStateMark.LStateMarkUnspecified);

        CShelfRoll roll = shelf.CShelfRollRead();

        CCatalogReference lost = roll.CShelfRollRows.Single(row => row.CCatalogReferenceName == "Lost");
        CCatalogReference blank = roll.CShelfRollRows.Single(row => row.CCatalogReferenceName == "Blank");
        Assert.Equal("Display.Unknown", lost.CCatalogReferenceCredit.CStateWordingKey);
        Assert.Equal("Display.Unknown", lost.CCatalogReferenceYear.CStateWordingKey);
        Assert.Equal("Source.Unset", blank.CCatalogReferenceCredit.CStateWordingKey);
        Assert.Equal("Source.Unset", blank.CCatalogReferenceYear.CStateWordingKey);
    }

    [Fact]
    public void ShelfRollRead_StatedYearAndCredit_ShowsTheirTextWithNoKey()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = TShelfWordingPrepare(
            engine, "Book", TInterfaceState.TStateValueCreate("1999"), LStateMark.LStateMarkUnknown);
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);

        CCatalogReference row = shelf.CShelfRollRead().CShelfRollRows
            .Single(shown => shown.CCatalogReferenceName == "Book");

        Assert.Equal(new CStateWording("Ada", null, false, null), row.CCatalogReferenceCredit);
        Assert.Equal(new CStateWording("1999", null, false, null), row.CCatalogReferenceYear);
    }

    private static LReference TShelfWordingPrepare(LEngine engine, string title, LStateValue year, LStateMark authors)
    {
        return engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterfaceState.TStateValueCreate(title),
            year,
            LReferenceKind.LReferenceKindBook,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            authors));
    }

    private static LReference TShelfRollPrepare(LEngine engine)
    {
        LReference book = engine.TEngineCitationCreate("Book");
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hearth",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a meaning",
                [TInterfaceExample.TSentenceDraftCreate(
                    "a warm hearth", 0, TInterface.TStateAnchorCreate(book.LReferenceId))],
                [],
                [],
                [],
                [],
                1)],
            []));
        return book;
    }
}
