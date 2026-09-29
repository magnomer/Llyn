using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardReference
{
    [Fact]
    public void ReferenceFind_TypedWord_OffersTheStoredSourceSplitAroundTheWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference notes = engine.TEngineCitationCreate("Field notes");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);

        CProffer offer = card.CCardReferenceFind(sheet, sentence, " FIELD ");

        Assert.Equal(" FIELD ", offer.CProfferText);
        Assert.True(offer.CProfferShown);
        Assert.Contains(new CProfferRow(notes.LReferenceId, string.Empty, "Field", " notes"), offer.CProfferRows);
    }

    [Fact]
    public void ReferenceFind_BlankOrUnmatchedText_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineCitationCreate("Field notes");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);

        CProffer blank = card.CCardReferenceFind(sheet, sentence, "   ");
        CProffer unmatched = card.CCardReferenceFind(sheet, sentence, "zzq");

        Assert.Equal("   ", blank.CProfferText);
        Assert.False(blank.CProfferShown);
        Assert.Empty(blank.CProfferRows);
        Assert.False(unmatched.CProfferShown);
        Assert.Empty(unmatched.CProfferRows);
    }

    [Fact]
    public void ReferenceFind_BylineTheSentenceCites_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference notes = engine.TEngineCitationCreate("Field notes");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);
        (long other, long uncited) = TCard.TCardSentenceAdd(desk);
        card.CCardCitationSet(sheet, sentence, "Field notes");
        string byline = card.CCardReferenceFind()
            .Single(row => row.CCatalogReferenceId == notes.LReferenceId).CCatalogReferenceByline;

        CProffer cited = card.CCardReferenceFind(sheet, sentence, " " + byline + " ");

        Assert.False(cited.CProfferShown);
        Assert.Empty(cited.CProfferRows);
        Assert.Contains(
            card.CCardReferenceFind(other, uncited, byline).CProfferRows,
            row => row.CProfferRowId == notes.LReferenceId);
    }

    [Fact]
    public void ReferenceFind_NineMatchingSources_OffersEight()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        for (int index = 1; index <= 9; index++)
        {
            engine.TEngineCitationCreate("Diary " + index);
        }

        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        (long sheet, long sentence) = TCard.TCardSentenceAdd(desk);

        CProffer offer = card.CCardReferenceFind(sheet, sentence, "diary");

        Assert.Equal(8, offer.CProfferRows.Count);
        Assert.All(offer.CProfferRows, static row => Assert.Equal("Diary", row.CProfferRowMark));
    }
}
