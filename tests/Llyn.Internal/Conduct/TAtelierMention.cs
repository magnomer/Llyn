using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtelierMention
{
    [Fact]
    public void MentionDivide_LinkedMention_ReturnsSpanPieces()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierMentionCreate(engine);

        IReadOnlyList<CMentionPiece> pieces =
            atelier.CAtelierMention.CMentionDivide("the cat sat", [new CMentionMark(1, 4, 3, 7, 0)]);

        Assert.Equal(3, pieces.Count);
        Assert.Equal("cat", pieces[1].CMentionPieceText);
        Assert.Equal(4, pieces[1].CMentionPieceOffset);
        Assert.Equal(7, pieces[1].CMentionPieceEnd);
        Assert.Null(pieces[0].CMentionPieceLinked);
        Assert.True(pieces[1].CMentionPieceLinked);
    }

    [Fact]
    public void MentionOffsetRead_UnitRead_RoundTrips()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierMentionCreate(engine);
        string text = "\U0001F600 cat";

        int unit = atelier.CAtelierMention.CMentionUnitRead(text, 2);

        Assert.Equal(3, unit);
        Assert.Equal(2, atelier.CAtelierMention.CMentionOffsetRead(text, unit));
    }

    [Fact]
    public void MentionMarkRead_Drafts_ReadsStoredMarks()
    {
        IReadOnlyList<CMentionMark> marks = CMention.CMentionMarkRead(
            [TInterface.TMentionDraftCreate(3, 4, 3, 7, 9)]);

        Assert.Equal([new CMentionMark(3, 4, 3, 7, 9)], marks);
    }

    [Fact]
    public void MentionResultRead_StoredMention_CarriesLink()
    {
        CMentionResult held = TInterfaceConduct.TMentionResultRead(2, TInterface.TMentionCreate(1, 2, 3, 40, 7));

        Assert.Equal(2, held.CMentionResultOffset);
        Assert.Equal(40, held.CMentionResultStored?.CMentionMarkEntry);
        Assert.Equal(7, held.CMentionResultStored?.CMentionMarkSense);
        Assert.False(held.CMentionResultSingle);
        Assert.Equal(0, held.CMentionResultFirst);
    }

    [Fact]
    public void MentionResultRead_NothingStored_CarriesNoMark()
    {
        Assert.Null(TInterfaceConduct.TMentionResultRead(2, null).CMentionResultStored);
    }

    [Fact]
    public void MentionMarkRead_NoDrafts_ReadsEmpty()
    {
        Assert.Empty(CMention.CMentionMarkRead(null));
    }

    [Theory]
    [InlineData(7, true)]
    [InlineData(0, false)]
    public void MentionResultOpen_StoredMention_OpensItsEntryAndRaisesItsSense(long sense, bool raised)
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierMentionCreate(engine);
        List<long> arrived = TMentionLibraryAdd(atelier, true);
        List<long> senses = [];
        atelier.CAtelierMention.CMentionSenseChosen += senses.Add;

        CMentionOffer offer = TInterfaceMention.TMentionResultOpen(
            atelier,
            new CMentionResult(2, new CMentionMark(1, 2, 3, 40, sense), [TMentionTargetCreate(8)]));

        Assert.Equal(2, offer.CMentionOfferOffset);
        Assert.Empty(offer.CMentionOfferEntry);
        Assert.Equal([40L], arrived);
        Assert.Equal(raised ? [sense] : [], senses);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(40, false)]
    public void MentionResultOpen_StoredMentionUnlinkedOrLeaveDeclined_OpensNothingAndRaisesNoSense(
        long entry, bool leave)
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierMentionCreate(engine);
        List<long> arrived = TMentionLibraryAdd(atelier, leave);
        List<long> senses = [];
        atelier.CAtelierMention.CMentionSenseChosen += senses.Add;

        CMentionOffer offer = TInterfaceMention.TMentionResultOpen(
            atelier,
            new CMentionResult(2, new CMentionMark(1, 2, 3, entry, 7), []));

        Assert.Equal(2, offer.CMentionOfferOffset);
        Assert.Empty(offer.CMentionOfferEntry);
        Assert.Empty(arrived);
        Assert.Empty(senses);
    }

    [Fact]
    public void MentionResultOpen_SingleCandidate_OpensItAtOnce()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierMentionCreate(engine);
        List<long> arrived = TMentionLibraryAdd(atelier, true);

        CMentionOffer offer = TInterfaceMention.TMentionResultOpen(
            atelier,
            new CMentionResult(2, null, [TMentionTargetCreate(8)]));

        Assert.Equal(2, offer.CMentionOfferOffset);
        Assert.Empty(offer.CMentionOfferEntry);
        Assert.Equal([8L], arrived);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(0)]
    public void MentionResultOpen_ManyOrNoCandidates_OpensNothingAndOffersTheMany(int count)
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierMentionCreate(engine);
        List<long> arrived = TMentionLibraryAdd(atelier, true);
        List<CTranslationTarget> candidates = [.. Enumerable.Range(8, count).Select(id => TMentionTargetCreate(id))];

        CMentionOffer offer = TInterfaceMention.TMentionResultOpen(
            atelier,
            new CMentionResult(2, null, candidates));

        Assert.Equal(2, offer.CMentionOfferOffset);
        Assert.Equal(candidates, offer.CMentionOfferEntry);
        Assert.Equal("Mention.Title", offer.CMentionOfferKey);
        Assert.Empty(arrived);
    }

    private static List<long> TMentionLibraryAdd(CAtelier atelier, bool leave)
    {
        List<long> arrived = [];
        atelier.CAtelierNavigation.TNavigationTabAdd(
            "Library", () => leave, static () => 0, static _ => { }, arrived.Add);
        return arrived;
    }

    private static CTranslationTarget TMentionTargetCreate(long id)
    {
        return new CTranslationTarget(id, "water", "English");
    }

    private static CAtelier TAtelierMentionCreate(LEngine engine)
    {
        return TInterfaceConduct.TAtelierCreate(engine, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionDivide"] = args =>
                TInterface.TMentionSpanDivide((string)args![0]!, (IReadOnlyList<LMention>)args[1]!),
            ["LEngineSpanRead"] = args =>
                TInterface.TMentionSpanRead((string)args![0]!, (int)args[1]!, (int)args[2]!),
            ["LEngineUnitRead"] = args =>
                TInterface.TMentionUnitRead((string)args![0]!, (int)args[1]!),
            ["LEngineOffsetRead"] = args =>
                TInterface.TMentionOffsetRead((string)args![0]!, (int)args[1]!),
        });
    }
}
