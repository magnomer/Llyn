using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TByline
{
    [Fact]
    public void BylineWordSet_MatchingAuthors_OffersUncreditedOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        LAuthor ada = TBylineOpen(engine, imprint);
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        imprint.CImprintAuthorAdd(0, ada.LAuthorId);
        CByline byline = imprint.CImprintByline;

        byline.CBylineWordSet("Ad", false);

        Assert.Empty(byline.CBylineRowsRead());
        Assert.False(byline.CBylineShown);

        byline.CBylineWordSet(" Ad ", true);

        Assert.Equal(
            [(string.Empty, "Ad", "am")],
            byline.CBylineRowsRead().Select(author => (author.CAuthorLead, author.CAuthorMark, author.CAuthorTail)));
        Assert.True(byline.CBylineShown);
        Assert.Equal(-1, byline.CBylineIndex);

        byline.CBylineWordSet("dA", true);

        Assert.Equal(
            [("A", "da", "m")],
            byline.CBylineRowsRead().Select(author => (author.CAuthorLead, author.CAuthorMark, author.CAuthorTail)));

        byline.CBylineWordSet(" ", true);

        Assert.Empty(byline.CBylineRowsRead());
        Assert.False(byline.CBylineShown);
    }

    [Fact]
    public void BylineRowsRead_ManyMatches_OffersAtMostEight()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        foreach (int index in Enumerable.Range(0, 10))
        {
            engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada " + index));
        }

        imprint.TImprintOpen(null);
        imprint.CImprintByline.CBylineWordSet("Ada", true);

        Assert.Equal(8, imprint.CImprintByline.CBylineRowsRead().Count);
    }

    [Fact]
    public void BylineMove_DownAndUp_WrapAround()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adele"));
        imprint.TImprintOpen(null);
        CByline byline = imprint.CImprintByline;
        byline.CBylineWordSet("Ad", true);
        Assert.Equal(2, byline.CBylineRowsRead().Count);

        Assert.True(byline.CBylineMove(0, 0, 1));
        Assert.Equal(0, byline.CBylineIndex);
        Assert.True(byline.CBylineMove(0, 0, 1));
        Assert.Equal(1, byline.CBylineIndex);
        Assert.True(byline.CBylineMove(0, 0, 1));
        Assert.Equal(0, byline.CBylineIndex);
        Assert.True(byline.CBylineMove(0, 0, -1));
        Assert.Equal(1, byline.CBylineIndex);

        Assert.True(imprint.CImprintAuthorCancel(0, 0));
        byline.CBylineRowsRead();

        Assert.False(byline.CBylineShown);
        Assert.Equal(-1, byline.CBylineIndex);
        Assert.False(byline.CBylineMove(0, 0, -1));
    }

    [Fact]
    public void ImprintAuthorFinish_LitRowOffered_InsertsItsAuthor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        LAuthor adam = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        imprint.TImprintOpen(null);
        imprint.CImprintByline.CBylineWordSet("Ad", true);
        imprint.CImprintByline.CBylineRowsRead();

        Assert.True(imprint.CImprintAuthorFinish(0, 0, "Ad", adam.LAuthorId));
        imprint.CImprintByline.CBylineRowsRead();

        Assert.False(imprint.CImprintByline.CBylineShown);
        Assert.Equal([adam.LAuthorId], imprint.CImprintCreditRead().Select(row => row.CAuthorRowId));
        Assert.True(imprint.CImprintDesk.TDeskChangeCheck());
    }

    [Fact]
    public void ImprintAuthorFinish_BylineOfferedNothingLit_CreditsTheTypedName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        imprint.TImprintOpen(null);
        imprint.CImprintByline.CBylineWordSet("Ad", true);
        imprint.CImprintByline.CBylineRowsRead();

        Assert.True(imprint.CImprintAuthorFinish(0, 0, "Adele", null));

        Assert.False(imprint.CImprintByline.CBylineShown);
        Assert.Equal(["Adele"], imprint.CImprintCreditRead().Select(row => row.CAuthorRowName));
    }

    [Fact]
    public void ImprintAuthorCancel_BylineOffered_ClosesOnlyTheByline()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        imprint.TImprintOpen(null);
        int reverted = 0;
        imprint.CImprintReverted += () => reverted++;
        imprint.CImprintByline.CBylineWordSet("Ad", true);
        imprint.CImprintByline.CBylineRowsRead();

        Assert.True(imprint.CImprintAuthorCancel(0, 0));

        Assert.Equal(0, reverted);
        Assert.False(imprint.CImprintByline.CBylineShown);
        Assert.Empty(imprint.CImprintByline.CBylineRowsRead());

        Assert.True(imprint.CImprintAuthorCancel(0, 0));

        Assert.Equal(1, reverted);
    }

    [Fact]
    public void BylineSelect_SameAuthorAsRow_OnlyReverts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        LAuthor ada = TBylineOpen(engine, imprint);
        int reverted = 0;
        imprint.CImprintReverted += () => reverted++;
        imprint.CImprintByline.CBylineWordSet("A", true);

        imprint.CImprintByline.CBylineSelect(ada.LAuthorId, 0, ada.LAuthorId);

        Assert.Equal(1, reverted);
        Assert.False(imprint.CImprintDesk.TDeskChangeCheck());

        imprint.CImprintByline.CBylineSelect(null, 0, ada.LAuthorId);

        Assert.Equal(1, reverted);
    }

    private static LAuthor TBylineOpen(LEngine engine, CImprint imprint)
    {
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        imprint.TImprintOpen(book.LReferenceId);
        return ada;
    }
}
