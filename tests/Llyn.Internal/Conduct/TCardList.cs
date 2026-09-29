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
    public void CardMeaningAdd_HeldMeanings_AppendsTheNewCardLast()
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
    public void CardCollocationAdd_HeldCard_AppendsTheNewCardLast()
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
    public void CardRemove_OneOfTwoMeanings_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 2);
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        card.CCardRemove(held[0]);

        Assert.Equal([held[1]], TCardMeaningRead(desk));
    }

    [Fact]
    public void CardRemove_LoneMeaning_KeepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 1);
        long lone = Assert.Single(TCardMeaningRead(desk));

        card.CCardRemove(lone);

        Assert.Equal([lone], TCardMeaningRead(desk));
    }

    [Fact]
    public void CardMove_TypedOrdinal_PlacesTheCardThere()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 3);
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        card.CCardMove(held[2], " 1 ");

        Assert.Equal([held[2], held[0], held[1]], TCardMeaningRead(desk));
    }

    [Fact]
    public void CardMove_OrdinalPastTheEnd_PlacesTheCardLast()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 3);
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        card.CCardMove(held[0], "9");

        Assert.Equal([held[1], held[2], held[0]], TCardMeaningRead(desk));
    }

    [Fact]
    public void CardMove_DraggedPlace_PlacesTheCardThere()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 3);
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        card.CCardMove(held[0], 1);

        Assert.Equal([held[1], held[0], held[2]], TCardMeaningRead(desk));
    }

    [Fact]
    public void CardMove_UnreadableOrdinal_KeepsTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCardList card) = TCardListPrepare(engine, 2);
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        card.CCardMove(held[0], "second");

        Assert.Equal(held, TCardMeaningRead(desk));
    }

    [Fact]
    public void CardOrdinalRead_TypedNumbers_ClampsToTheList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, _) = TCardListPrepare(engine, 3);
        LEntryDraft content = desk.TDeskRead()!.LDraftContent;
        IReadOnlyList<long> held = TCardMeaningRead(desk);

        Assert.Equal(0, TInterface.TCardOrdinalRead(content, held[2], "-4"));
        Assert.Equal(2, TInterface.TCardOrdinalRead(content, held[0], "12"));
        Assert.Equal(1, TInterface.TCardOrdinalRead(content, held[0], " 2 "));
        Assert.Null(TInterface.TCardOrdinalRead(content, held[1], "2"));
        Assert.Null(TInterface.TCardOrdinalRead(content, held[1], "2nd"));
        Assert.Null(TInterface.TCardOrdinalRead(content, 0, "1"));
    }

    [Fact]
    public void CardLoneCheck_ListOfOneOrTwo_MarksOnlyTheLoneCard()
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
    public void CardEndRead_EachKind_AnswersItsListLength()
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
        CCardList card = TInterfaceConduct.TCardListCreate(desk);
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
