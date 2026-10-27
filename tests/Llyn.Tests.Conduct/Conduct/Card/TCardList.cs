using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardList
{
    [Fact]
    public void CardMeaningAdd_HeldMeanings_AppendsNewCardLast()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 2);
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        card.CCardMeaningAdd();

        IReadOnlyList<long> meanings = TCardMeaningRead(desk);
        Assert.Equal(3, meanings.Count);
        Assert.Equal(held, meanings.Take(2));
        Assert.Equal(3, desk.TDeskRead()!.LDraftContent.LEntryDraftMeanings[^1].LCardDraftPosition);
    }

    [Fact]
    public void CardCollocationAdd_HeldCard_AppendsNewCardLast()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 1);
        card.CCardCollocationAdd();
        IReadOnlyList<long> held = TCardCollocationRead(desk);

        card.CCardCollocationAdd();

        IReadOnlyList<long> collocations = TCardCollocationRead(desk);
        Assert.Equal(held.Count + 1, collocations.Count);
        Assert.Equal(held, collocations.Take(held.Count));
    }

    [Fact]
    public void CardRemove_OneOfTwoMeanings_DropsMeaning()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 2);
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        card.CCardRemove(false, held[0]);

        Assert.Equal([held[1]], TCardMeaningRead(desk));
    }

    [Fact]
    public void CardRemove_LoneMeaning_KeepsMeaning()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 1);
        long lone = Assert.Single(TCardMeaningRead(desk));

        card.CCardRemove(false, lone);

        Assert.Equal([lone], TCardMeaningRead(desk));
    }

    [Theory]
    [InlineData(2, " 1 ", new[] { 2, 0, 1 })]
    [InlineData(0, "9", new[] { 1, 2, 0 })]
    [InlineData(2, "-4", new[] { 2, 0, 1 })]
    public void CardMove_TypedOrdinal_PlacesCardAtClampedSlot(int moved, string ordinal, int[] expected)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 3);
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        card.CCardMove(false, held[moved], ordinal);

        Assert.Equal(expected.Select(index => held[index]), TCardMeaningRead(desk));
    }

    [Fact]
    public void CardMove_DraggedPlace_PlacesCardAtPlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 3);
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        card.CCardMove(false, held[0], 1);

        Assert.Equal([held[1], held[0], held[2]], TCardMeaningRead(desk));
    }

    [Fact]
    public void CardMove_CardMissingFromDraft_KeepsOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 3);
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        card.CCardMove(false, held.Max() + 1, 0);

        Assert.Equal(held, TCardMeaningRead(desk));
    }

    [Theory]
    [InlineData(3, 1, "2")]
    [InlineData(2, 0, "second")]
    [InlineData(3, 0, "2nd")]
    public void CardMove_OrdinalMovingNothing_KeepsOrder(int count, int moved, string ordinal)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, count);
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        card.CCardMove(false, held[moved], ordinal);

        Assert.Equal(held, TCardMeaningRead(desk));
    }

    [Fact]
    public void CardMove_OrdinalForMissingCard_KeepsOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 3);
        IReadOnlyList<long> held = TCardMeaningRead(desk);
        IReadOnlyList<long> collocations = TCardCollocationRead(desk);

        card.CCardMove(false, held.Max() + 1, "1");

        Assert.Equal(held, TCardMeaningRead(desk));
        Assert.Equal(collocations, TCardCollocationRead(desk));
    }

    [Fact]
    public void CardShiftRead_DraggedPlaces_AnswersOnlyRealMove()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, _) = TCardListPrepare(engine, 3);
        LEntryDraft content = desk.TDeskRead()!.LDraftContent;
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        Assert.Equal(2, TInterface.TCardShiftRead(content, held[0], 2));
        Assert.Equal(0, TInterface.TCardShiftRead(content, held[2], 0));
        Assert.Null(TInterface.TCardShiftRead(content, held[1], 1));
        Assert.Null(TInterface.TCardShiftRead(content, held[0], -1));
        Assert.Null(TInterface.TCardShiftRead(content, held[0], 3));
        Assert.Null(TInterface.TCardShiftRead(content, held.Max() + 1, 0));
    }

    [Fact]
    public void CardLoneCheck_ListOfOneOrTwo_MarksOnlyLoneCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 1);
        long lone = Assert.Single(TCardMeaningRead(desk));
        bool single = TInterface.TCardLoneCheck(desk.TDeskRead()!.LDraftContent, lone);
        card.CCardMeaningAdd();

        bool paired = TInterface.TCardLoneCheck(desk.TDeskRead()!.LDraftContent, lone);

        Assert.True(single);
        Assert.False(paired);
    }

    [Fact]
    public void CardEndRead_EachKind_AnswersListLength()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 3);
        card.CCardCollocationAdd();
        LEntryDraft content = desk.TDeskRead()!.LDraftContent;

        Assert.Equal(3, TInterface.TCardEndRead(content, LCardKind.LCardKindMeaning));
        Assert.Equal(
            TCardCollocationRead(desk).Count, TInterface.TCardEndRead(content, LCardKind.LCardKindCollocation));
    }

    private static (CDesk TCardListDesk, CCardList TCardListCard) TCardListPrepare(LEngine engine, int count)
    {
        (CDesk desk, _) = TCard.TCardPrepare(engine);
        CCardList card = TInterfaceConductCard.TCardListCreate(desk);
        while (TCardMeaningRead(desk).Count < count)
        {
            card.CCardMeaningAdd();
        }

        Assert.Equal(count, TCardMeaningRead(desk).Count);
        return (desk, card);
    }

    private static IReadOnlyList<long> TCardMeaningRead(CDesk desk)
    {
        return desk.TDeskRead()!.LDraftContent.LEntryDraftMeanings.Select(held => held.LCardDraftId).ToList();
    }

    private static IReadOnlyList<long> TCardCollocationRead(CDesk desk)
    {
        return desk.TDeskRead()!.LDraftContent.LEntryDraftCollocations.Select(held => held.LCardDraftId).ToList();
    }
}
