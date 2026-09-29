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
        CShelf shelf = TShelf.TShelfPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
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
        CShelf shelf = TShelf.TShelfPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LReference book = TShelfRollPrepare(engine);
        shelf.CShelfReferenceSelect(book.LReferenceId);
        shelf.CShelfQuerySet("zzz");

        CShelfRoll roll = shelf.CShelfRollRead();

        Assert.True(roll.CShelfRollEmpty);
        Assert.Empty(roll.CShelfRollRows);
        Assert.Equal(TInterface.TLocalizationTextRead("Source.UsageNone"), roll.CShelfRollTally);
        Assert.False(shelf.CShelfBinEnabled);
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
                [TInterface.TSentenceDraftCreate(
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
