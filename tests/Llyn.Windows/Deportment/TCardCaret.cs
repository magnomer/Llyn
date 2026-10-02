using System;
using System.Collections.Generic;
using System.Threading;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardCaret
{
    [Fact]
    public void CardLabelMove_StepsTheCaretAndLeavesTheTagsInOrder()
    {
        List<(List<long>, int)> reads = [];
        List<bool> moves = [];
        List<long?> finds = [];
        TCardCaretRun(card =>
        {
            TInterfaceDeportment.TCardLabelShow(card, [new CTagDraft(1, "one"), new CTagDraft(2, "two")]);
            reads.Add(TInterfaceDeportment.TCardLabelRead(card));
            moves.Add(TInterfaceDeportment.TCardLabelMove(card, -1));
            reads.Add(TInterfaceDeportment.TCardLabelRead(card));
            finds.Add(TInterfaceDeportment.TCardLabelFind(card, -1));
            finds.Add(TInterfaceDeportment.TCardLabelFind(card, 1));
            moves.Add(TInterfaceDeportment.TCardLabelMove(card, 2));
            moves.Add(TInterfaceDeportment.TCardLabelMove(card, -1));
            moves.Add(TInterfaceDeportment.TCardLabelMove(card, -1));
            reads.Add(TInterfaceDeportment.TCardLabelRead(card));
            finds.Add(TInterfaceDeportment.TCardLabelFind(card, -1));
        });

        Assert.Equal([1L, 2L], reads[0].Item1);
        Assert.Equal(2, reads[0].Item2);
        Assert.Equal([true, false, true, false], moves);
        Assert.Equal([1L, 2L], reads[1].Item1);
        Assert.Equal(1, reads[1].Item2);
        Assert.Equal([1L, 2L], reads[2].Item1);
        Assert.Equal(0, reads[2].Item2);
        Assert.Equal([1L, 2L, null], finds);
    }

    [Fact]
    public void CardLabelShow_KeepsTheCaretBeforeTheTagThatFollowedIt()
    {
        List<(List<long>, int)> seen = [];
        TCardCaretRun(card =>
        {
            TInterfaceDeportment.TCardLabelShow(card, [new CTagDraft(1, "one"), new CTagDraft(2, "two")]);
            TInterfaceDeportment.TCardLabelMove(card, -1);
            TInterfaceDeportment.TCardLabelShow(
                card, [new CTagDraft(1, "one"), new CTagDraft(3, "three"), new CTagDraft(2, "two")]);
            seen.Add(TInterfaceDeportment.TCardLabelRead(card));
            TInterfaceDeportment.TCardLabelShow(card, [new CTagDraft(1, "one"), new CTagDraft(3, "three")]);
            seen.Add(TInterfaceDeportment.TCardLabelRead(card));
            TInterfaceDeportment.TCardLabelShow(card, []);
            seen.Add(TInterfaceDeportment.TCardLabelRead(card));
        });

        Assert.Equal(new List<long> { 1, 3, 2 }, seen[0].Item1);
        Assert.Equal(2, seen[0].Item2);
        Assert.Equal(new List<long> { 1, 3 }, seen[1].Item1);
        Assert.Equal(2, seen[1].Item2);
        Assert.Empty(seen[2].Item1);
        Assert.Equal(0, seen[2].Item2);
    }

    [Fact]
    public void CardLabelShow_PutsTheCaretBeforeTheNextTagWhenItsTagIsGone()
    {
        (List<long>, int)? seen = null;
        TCardCaretRun(card =>
        {
            TInterfaceDeportment.TCardLabelShow(
                card, [new CTagDraft(1, "one"), new CTagDraft(2, "two"), new CTagDraft(3, "three")]);
            TInterfaceDeportment.TCardLabelMove(card, -1);
            TInterfaceDeportment.TCardLabelMove(card, -1);
            TInterfaceDeportment.TCardLabelShow(card, [new CTagDraft(1, "one"), new CTagDraft(3, "three")]);
            seen = TInterfaceDeportment.TCardLabelRead(card);
        });

        Assert.NotNull(seen);
        Assert.Equal([1L, 3L], seen.Value.Item1);
        Assert.Equal(1, seen.Value.Item2);
    }

    private static void TCardCaretRun(Action<object> act)
    {
        Exception? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                act(TInterfaceDeportment.TCardCreate());
            }
            catch (Exception caught)
            {
                failure = caught;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        Assert.Null(failure);
    }
}
