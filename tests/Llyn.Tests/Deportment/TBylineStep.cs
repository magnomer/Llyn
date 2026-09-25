using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TBylineStep
{
    [Fact]
    public void BylineWordSet_MatchingAuthors_OffersUncreditedOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        shelf.TShelfRowSelect(book.LReferenceId);
        shelf.TShelfScribeSet(true);
        LImprint imprint = shelf.LShelfImprint;
        imprint.TImprintCreditApply("Add", 0, ada.LAuthorId);

        imprint.TBylineWordSet("Ad", false);

        Assert.Empty(imprint.TBylineRowsRead());
        Assert.False(imprint.LBylineShown);

        imprint.TBylineWordSet("Ad", true);

        Assert.Equal(["Adam"], imprint.TBylineRowsRead().Select(author => author.LAuthorName));
        Assert.True(imprint.LBylineShown);
        Assert.Equal(-1, imprint.LBylineIndex);
        Assert.Equal("Ad", imprint.LBylineWord);

        imprint.TBylineWordSet(" ", true);

        Assert.Empty(imprint.TBylineRowsRead());
        Assert.False(imprint.LBylineShown);
    }

    [Fact]
    public void KeyApply_DownAndUp_WrapAround()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adele"));
        shelf.TShelfFreshStart();
        LImprint imprint = shelf.LShelfImprint;
        imprint.TBylineWordSet("Ad", true);
        Assert.Equal(2, imprint.TBylineRowsRead().Count);

        Assert.True(imprint.TImprintKeyApply("Down", 0, 0, "Ad", null));
        Assert.Equal(0, imprint.LBylineIndex);
        Assert.True(imprint.TImprintKeyApply("Down", 0, 0, "Ad", null));
        Assert.Equal(1, imprint.LBylineIndex);
        Assert.True(imprint.TImprintKeyApply("Down", 0, 0, "Ad", null));
        Assert.Equal(0, imprint.LBylineIndex);
        Assert.True(imprint.TImprintKeyApply("Up", 0, 0, "Ad", null));
        Assert.Equal(1, imprint.LBylineIndex);

        Assert.True(imprint.TImprintKeyApply("Escape", 0, 0, "Ad", null));
        imprint.TBylineRowsRead();

        Assert.False(imprint.LBylineShown);
        Assert.Equal(-1, imprint.LBylineIndex);
        Assert.False(imprint.TImprintKeyApply("Up", 0, 0, "Ad", null));
    }

    [Fact]
    public void KeyApply_EnterOnLitRow_PicksAuthor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        LAuthor adam = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        shelf.TShelfFreshStart();
        LImprint imprint = shelf.LShelfImprint;
        imprint.TBylineWordSet("Ad", true);
        imprint.TBylineRowsRead();

        Assert.True(imprint.TImprintKeyApply("Enter", 0, 0, "Ad", adam.LAuthorId));
        imprint.TBylineRowsRead();

        Assert.False(imprint.LBylineShown);
        Assert.Equal([adam.LAuthorId], imprint.TImprintCreditRead().Select(row => row.LAuthorRowId));
        Assert.True(imprint.LImprintDesk.TDeskChangeCheck());
    }

    [Fact]
    public void BylineSelect_SameAuthorAsRow_OnlyReverts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        shelf.TShelfRowSelect(book.LReferenceId);
        shelf.TShelfScribeSet(true);
        LImprint imprint = shelf.LShelfImprint;
        int reverted = 0;
        imprint.LImprintReverted += () => reverted++;
        imprint.TBylineWordSet("A", true);

        imprint.TBylineSelect(ada.LAuthorId, 0, ada.LAuthorId);

        Assert.Equal(1, reverted);
        Assert.False(imprint.LImprintDesk.TDeskChangeCheck());

        imprint.TBylineSelect(null, 0, ada.LAuthorId);

        Assert.Equal(1, reverted);
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
