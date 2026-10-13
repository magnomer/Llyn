using System;
using System.Threading;
using System.Windows.Controls;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardOrdinal
{
    [Fact]
    public void CardRowApply_BadgeOpened_ShowsStoredNumber()
    {
        string? shown = null;
        TCardOrdinalRun((card, box) =>
        {
            box.Text = "7";
            TInterfaceDeportment.TCardPositionShow(card);
            shown = box.Text;
        });

        Assert.Equal("3", shown);
    }

    [Fact]
    public void CardPositionHide_TypedText_ShowsStoredNumber()
    {
        string? typed = null;
        string? shown = null;
        TCardOrdinalRun((card, box) =>
        {
            TInterfaceDeportment.TCardPositionShow(card);
            box.Text = "abc";
            typed = box.Text;
            TInterfaceDeportment.TCardPositionHide(card);
            shown = box.Text;
        });

        Assert.Equal("abc", typed);
        Assert.Equal("3", shown);
    }

    [Theory]
    [InlineData("9")]
    [InlineData("x")]
    public void CardPositionHide_RefusedMove_ShowsStoredNumber(string text)
    {
        string? sent = null;
        string? shown = null;
        TCardOrdinalRun((card, box) =>
        {
            TInterfaceDeportment.TCardPositionShow(card);
            box.Text = text;
            sent = box.Text;
            TInterfaceDeportment.TCardPositionHide(card);
            shown = box.Text;
        });

        Assert.Equal(text, sent);
        Assert.Equal("3", shown);
    }

    [Fact]
    public void CardPositionHide_AcceptedMove_ShowsNewNumber()
    {
        string? sent = null;
        string? shown = null;
        TCardOrdinalRun((card, box) =>
        {
            TInterfaceDeportment.TCardPositionShow(card);
            box.Text = "1";
            sent = box.Text;
            TInterfaceDeportment.TCardPositionSet(card, 1);
            TInterfaceDeportment.TCardPositionHide(card);
            shown = box.Text;
        });

        Assert.Equal("1", sent);
        Assert.Equal("1", shown);
    }

    [Fact]
    public void CardRowApply_MoveWhileOpen_RepaintsNewNumber()
    {
        string? shown = null;
        TCardOrdinalRun((card, box) =>
        {
            TInterfaceDeportment.TCardPositionShow(card);
            box.Text = "abc";
            TInterfaceDeportment.TCardPositionSet(card, 2);
            shown = box.Text;
        });

        Assert.Equal("2", shown);
    }

    private static void TCardOrdinalRun(Action<object, TextBox> act)
    {
        Exception? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                object card = TInterfaceDeportment.TCardCreate();
                TInterfaceDeportment.TCardPositionSet(card, 3);
                act(card, TInterfaceDeportment.TCardPositionAttach(card));
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
