using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TImprintField
{
    [Fact]
    public void TitleSet_RawText_HoldsSpecifiedTitle()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        shelf.TShelfFreshStart();
        LImprint imprint = shelf.LShelfImprint;

        imprint.TImprintTitleSet("Book");
        imprint.TImprintYearSet(" ");

        LReference held = imprint.TImprintReferenceRead(imprint.LImprintDesk.TDeskRead()!);
        Assert.Equal("Book", held.LReferenceTitle.TStateValueShow());
        Assert.True(held.LReferenceYear.LStateValueEmpty);
        Assert.Equal("Source.Year", held.LReferenceYearHint);
        Assert.True(imprint.LImprintDesk.TDeskChangeCheck());
        Assert.True(shelf.LShelfStoreEnabled);
    }

    [Fact]
    public void KindSet_Tag_RoundTripsThroughDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        shelf.TShelfFreshStart();
        LImprint imprint = shelf.LShelfImprint;

        imprint.TImprintKindSet("journal");
        imprint.TImprintKindSet(null);

        LReference held = imprint.TImprintReferenceRead(imprint.LImprintDesk.TDeskRead()!);
        Assert.Equal("journal", held.LReferenceKindTag);
        Assert.True(imprint.LImprintDesk.TDeskChronicleRead().LDeskBackward);
    }

    [Fact]
    public void Save_UnchangedStoredSource_KeepsDraftHeld()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        shelf.TShelfRowSelect(book.LReferenceId);
        shelf.TShelfScribeSet(true);
        LImprint imprint = shelf.LShelfImprint;

        imprint.TImprintSave();

        Assert.True(imprint.LImprintHeld);
        Assert.Equal("Source.UsageNone", imprint.TImprintTallyRead());

        imprint.TImprintTitleSet("Tome");
        imprint.TImprintSave();

        Assert.False(imprint.LImprintHeld);
        Assert.True(shelf.LShelfColophonShown);
        Assert.Equal("Tome", engine.TEngineReferenceRead(book.LReferenceId)?.LReferenceTitle.TStateValueShow());
    }

    private static LShelf TShelfPrepare(LEngine engine)
    {
        LShelf shelf = TInterfaceDeportment.TShelfCreate(engine, () => true);
        shelf.TShelfVistaRestore(
            engine.TEngineVistaStart("reference", LCatalogOrder.LCatalogOrderName),
            engine.TEngineVistaStart("footnote", LCatalogOrder.LCatalogOrderHeadword));
        return shelf;
    }
}
