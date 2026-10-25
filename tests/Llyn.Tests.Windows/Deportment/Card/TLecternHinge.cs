using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternHinge
{
    [Fact]
    public void LeafCardRefine_FoldedCard_CollapsesBodyChecksHingeAndClosesHeader()
    {
        TCardHinge.TCardHingeRun(() =>
        {
            ContentPresenter card = TLecternHingeRead(TCardHinge.TCardHingeTitle, true, true);
            Border header = (Border)TCardHinge.TCardHingeResolve(card, "PCardHeader");

            Assert.Equal(Visibility.Collapsed, TCardHinge.TCardHingeResolve(card, "PCardBody").Visibility);
            Assert.True(((ToggleButton)TCardHinge.TCardHingeResolve(card, "PCardHinge")).IsChecked);
            Assert.Equal(Visibility.Visible, TCardHinge.TCardHingeResolve(card, "PCardHinge").Visibility);
            Assert.Equal(new CornerRadius(12), header.CornerRadius);
            Assert.Equal(new Thickness(0), header.BorderThickness);
        });
    }

    [Fact]
    public void LeafCardRefine_UnfoldedCard_ShowsBodyAndKeepsHeaderLine()
    {
        TCardHinge.TCardHingeRun(() =>
        {
            ContentPresenter card = TLecternHingeRead(TCardHinge.TCardHingeTitle, false, true);
            Border header = (Border)TCardHinge.TCardHingeResolve(card, "PCardHeader");

            Assert.Equal(Visibility.Visible, TCardHinge.TCardHingeResolve(card, "PCardBody").Visibility);
            Assert.False(((ToggleButton)TCardHinge.TCardHingeResolve(card, "PCardHinge")).IsChecked);
            Assert.Equal(new CornerRadius(12, 12, 0, 0), header.CornerRadius);
            Assert.Equal(new Thickness(0, 0, 0, 1), header.BorderThickness);
        });
    }

    [Fact]
    public void LeafCardRefine_FoldedCardWithBlankTitle_ShowsMeaningPeekInTheTitlePlace()
    {
        TCardHinge.TCardHingeRun(() =>
        {
            ContentPresenter card = TLecternHingeRead(TCardHinge.TCardHingeBlank, true, true);

            Assert.Equal(Visibility.Visible, TCardHinge.TCardHingeResolve(card, "PCardPeek").Visibility);
            Assert.Equal("a liquid", ((TextBlock)TCardHinge.TCardHingeResolve(card, "PCardPeek")).Text);
            Assert.Equal(Visibility.Collapsed, TCardHinge.TCardHingeResolve(card, "PCardTitle").Visibility);
            Assert.Equal(Visibility.Collapsed, TCardHinge.TCardHingeResolve(card, "PCardCaption").Visibility);
        });
    }

    [Fact]
    public void LeafCardRefine_FoldedCardWithTitle_ShowsTitleAndHidesPeek()
    {
        TCardHinge.TCardHingeRun(() =>
        {
            ContentPresenter card = TLecternHingeRead(TCardHinge.TCardHingeTitle, true, true);

            Assert.Equal(Visibility.Visible, TCardHinge.TCardHingeResolve(card, "PCardTitle").Visibility);
            Assert.Equal(Visibility.Collapsed, TCardHinge.TCardHingeResolve(card, "PCardPeek").Visibility);
        });
    }

    [Fact]
    public void LeafCardRefine_UnstoredCard_CollapsesHinge()
    {
        TCardHinge.TCardHingeRun(() =>
        {
            ContentPresenter card = TLecternHingeRead(TCardHinge.TCardHingeTitle, false, false);

            Assert.Equal(Visibility.Collapsed, TCardHinge.TCardHingeResolve(card, "PCardHinge").Visibility);
            Assert.Equal(Visibility.Visible, TCardHinge.TCardHingeResolve(card, "PCardBody").Visibility);
        });
    }

    [Fact]
    public void LecternHingeClick_StoredCard_FoldsTheCardByIdThroughTheReadingView()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TCardHinge.TCardHingePrepare(engine);
        long card = engine.TEngineEntryLoad(water.LEntryId)!.LEntryDraftMeanings[0].LCardDraftId;
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water.LEntryId);
        IReadOnlySet<(LCardKind, long)>? folded = null;

        TCardHinge.TCardHingeRun(() =>
        {
            ItemsControl meanings = TCardHinge.TCardHingeLoad(TCardHinge.TCardHingeCreate());
            meanings.Name = "PDisplayMeaning";
            ItemsControl collocations = TCardHinge.TCardHingeLoad(TCardHinge.TCardHingeCreate());
            collocations.Name = "PDisplayCollocation";
            StackPanel surface = new();
            surface.Children.Add(meanings);
            surface.Children.Add(new StackPanel { Name = "PDisplayMeaningSection" });
            surface.Children.Add(collocations);
            surface.Children.Add(new StackPanel { Name = "PDisplayCollocationSection" });
            TInterfaceDeportment.TLecternCardCreate(wing.CWingDisplay, surface).QLecternCardRefine();
            TCardHinge.TCardHingeSettle(surface);

            TCardHinge.TCardHingeToggle(TCardHinge.TCardHingeFind(meanings), true);
            folded = engine.TEngineFoldRead(water.LEntryId);
        });

        Assert.Equal(new HashSet<(LCardKind, long)> { (LCardKind.LCardKindMeaning, card) }, folded);
    }

    [Fact]
    public void LecternHingeClick_RefusedWrite_PutsTheHingeBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TCardHinge.TCardHingePrepare(engine);
        long card = engine.TEngineEntryLoad(water.LEntryId)!.LEntryDraftMeanings[0].LCardDraftId;
        CLeaf leaf = new(
            card,
            false,
            1,
            TCardHinge.TCardHingeTitle,
            TCardHinge.TCardHingeMeaning,
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            false,
            true,
            [],
            [],
            TCardHinge.TCardHingeBlank,
            [],
            string.Empty,
            string.Empty,
            string.Empty);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        bool? shown = null;
        IReadOnlySet<(LCardKind, long)>? folded = null;

        TCardHinge.TCardHingeRun(() =>
        {
            ItemsControl meanings = TCardHinge.TCardHingeLoad(TCardHinge.TCardHingeCreate());
            meanings.Name = "PDisplayMeaning";
            StackPanel surface = new();
            surface.Children.Add(meanings);
            surface.Children.Add(new StackPanel { Name = "PDisplayMeaningSection" });
            surface.Children.Add(new ItemsControl { Name = "PDisplayCollocation" });
            surface.Children.Add(new StackPanel { Name = "PDisplayCollocationSection" });
            _ = TInterfaceDeportment.TLecternCardCreate(wing.CWingDisplay, surface);
            meanings.ItemsSource = new[] { TInterfaceDeportment.TLeafItemCreate(leaf, leaf.CLeafMeaning) };
            TCardHinge.TCardHingeSettle(surface);
            ToggleButton hinge = TCardHinge.TCardHingeFind(meanings);

            TCardHinge.TCardHingeToggle(hinge, true);
            shown = hinge.IsChecked;
            folded = engine.TEngineFoldRead(water.LEntryId);
        });

        Assert.False(shown);
        Assert.Empty(folded!);
    }

    [Fact]
    public void LecternSeeClick_FoldedRow_OpensRowAndNoticesItsId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a liquid", 1)],
            [TInterface.TCardCreate("set off", 1)]));
        long card = engine.TEngineEntryLoad(water.LEntryId)!.LEntryDraftCollocations[0].LCardDraftId;
        engine.TEngineFoldSave(water.LEntryId, LCardKind.LCardKindCollocation, card);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water.LEntryId);
        long? noticed = null;
        IReadOnlySet<(LCardKind, long)>? folded = null;

        TCardHinge.TCardHingeRun(() =>
        {
            ItemsControl meanings = TCardHinge.TCardHingeLoad(new FrameworkElementFactory(typeof(Button)));
            meanings.Name = "PDisplayMeaning";
            StackPanel surface = new();
            surface.Children.Add(meanings);
            surface.Children.Add(new StackPanel { Name = "PDisplayMeaningSection" });
            surface.Children.Add(new ItemsControl { Name = "PDisplayCollocation" });
            surface.Children.Add(new StackPanel { Name = "PDisplayCollocationSection" });
            TInterfaceDeportment.TLecternSeeAttach(
                TInterfaceDeportment.TLecternCardCreate(wing.CWingDisplay, surface), id => noticed = id);
            meanings.ItemsSource = new[] { TInterfaceDeportment.TLeafSeeCreate(card, "→ set off see 1") };
            TCardHinge.TCardHingeSettle(surface);
            ContentPresenter container = (ContentPresenter)meanings.ItemContainerGenerator.ContainerFromIndex(0);
            Button see = (Button)VisualTreeHelper.GetChild(container, 0);

            see.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, see));
            folded = engine.TEngineFoldRead(water.LEntryId);
        });

        Assert.Empty(folded!);
        Assert.Equal(card, noticed);
    }

    [Fact]
    public void LeafRowRefine_FoldedRow_ShowsMarkersAndCollapsesBody()
    {
        TCardHinge.TCardHingeRun(() =>
        {
            ContentPresenter row = TLecternRowRead(true);
            TextBlock meaning = (TextBlock)TCardHinge.TCardHingeResolve(row, "PCardMeaning");
            TextBlock count = (TextBlock)TCardHinge.TCardHingeResolve(row, "PCardCount");
            TextBlock also = (TextBlock)TCardHinge.TCardHingeResolve(row, "PCardAlso");

            Assert.Equal(Visibility.Collapsed, TCardHinge.TCardHingeResolve(row, "PCardBody").Visibility);
            Assert.True(((ToggleButton)TCardHinge.TCardHingeResolve(row, "PCardHinge")).IsChecked);
            Assert.Equal("set off", ((TextBlock)TCardHinge.TCardHingeResolve(row, "PCardPhrase")).Text);
            Assert.Equal(("a liquid", TextWrapping.NoWrap), (meaning.Text, meaning.TextWrapping));
            Assert.Equal(Visibility.Visible, TCardHinge.TCardHingeResolve(row, "PCardFlag").Visibility);
            Assert.Equal(("2 examples", Visibility.Visible), (count.Text, count.Visibility));
            Assert.Equal(("also 2", Visibility.Visible), (also.Text, also.Visibility));
        });
    }

    [Fact]
    public void LeafRowRefine_OpenRow_HidesMarkersAndShowsBody()
    {
        TCardHinge.TCardHingeRun(() =>
        {
            ContentPresenter row = TLecternRowRead(false);

            Assert.Equal(Visibility.Visible, TCardHinge.TCardHingeResolve(row, "PCardBody").Visibility);
            Assert.False(((ToggleButton)TCardHinge.TCardHingeResolve(row, "PCardHinge")).IsChecked);
            Assert.Equal(
                TextWrapping.Wrap, ((TextBlock)TCardHinge.TCardHingeResolve(row, "PCardMeaning")).TextWrapping);
            Assert.Equal(Visibility.Collapsed, TCardHinge.TCardHingeResolve(row, "PCardFlag").Visibility);
            Assert.Equal(Visibility.Collapsed, TCardHinge.TCardHingeResolve(row, "PCardCount").Visibility);
            Assert.Equal(Visibility.Collapsed, TCardHinge.TCardHingeResolve(row, "PCardAlso").Visibility);
        });
    }

    [Fact]
    public void LeafCardRefine_HomedCollocation_ShowsHeadingOverRows()
    {
        TCardHinge.TCardHingeRun(() =>
        {
            CLeaf row = TLecternRowCreate(2, true, true, [], string.Empty);
            CLeaf meaning = TLecternRowCreate(1, false, false, [row], "Collocations");
            FrameworkElementFactory card = new(typeof(StackPanel));
            card.AppendChild(new FrameworkElementFactory(typeof(TextBlock)) { Name = "PCardHeading" });
            card.AppendChild(new FrameworkElementFactory(typeof(ItemsControl)) { Name = "PCardHome" });
            ItemsControl list = TCardHinge.TCardHingeLoad(card);
            list.ItemsSource = new[] { TInterfaceDeportment.TLeafItemCreate(meaning, meaning.CLeafMeaning) };
            TCardHinge.TCardHingeSettle(list);
            ContentPresenter container = (ContentPresenter)list.ItemContainerGenerator.ContainerFromIndex(0);
            TInterfaceDeportment.TLeafCardRefine(container, list.Items[0]);
            TextBlock heading = (TextBlock)TCardHinge.TCardHingeResolve(container, "PCardHeading");

            Assert.Equal(("Collocations", Visibility.Visible), (heading.Text, heading.Visibility));
            Assert.Equal(Visibility.Visible, TCardHinge.TCardHingeResolve(container, "PCardHome").Visibility);
        });
    }

    private static ContentPresenter TLecternRowRead(bool folded)
    {
        CLeaf leaf = TLecternRowCreate(7, true, folded, [], string.Empty);
        FrameworkElementFactory row = new(typeof(StackPanel));
        row.AppendChild(new FrameworkElementFactory(typeof(ToggleButton)) { Name = "PCardHinge" });
        row.AppendChild(new FrameworkElementFactory(typeof(TextBlock)) { Name = "PCardPhrase" });
        row.AppendChild(new FrameworkElementFactory(typeof(TextBlock)) { Name = "PCardMeaning" });
        row.AppendChild(new FrameworkElementFactory(typeof(ItemsControl)) { Name = "PCardFlag" });
        row.AppendChild(new FrameworkElementFactory(typeof(TextBlock)) { Name = "PCardCount" });
        row.AppendChild(new FrameworkElementFactory(typeof(TextBlock)) { Name = "PCardAlso" });
        row.AppendChild(new FrameworkElementFactory(typeof(Grid)) { Name = "PCardBody" });
        ItemsControl list = TCardHinge.TCardHingeLoad(row);
        list.ItemsSource = new[] { TInterfaceDeportment.TLeafItemCreate(leaf, leaf.CLeafPhrase) };
        TCardHinge.TCardHingeSettle(list);
        ContentPresenter container = (ContentPresenter)list.ItemContainerGenerator.ContainerFromIndex(0);
        TInterfaceDeportment.TLeafRowRefine(container, list.Items[0]);
        return container;
    }

    private static CLeaf TLecternRowCreate(
        long id, bool collocated, bool folded, IReadOnlyList<CLeaf> collocation, string heading)
    {
        return new CLeaf(
            id,
            collocated,
            1,
            TCardHinge.TCardHingeBlank,
            TCardHinge.TCardHingeMeaning,
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            folded,
            true,
            collocation,
            [],
            new CStateWording("set off", null, false, null),
            ["French"],
            "2 examples",
            "also 2",
            heading);
    }

    private static ContentPresenter TLecternHingeRead(CStateWording title, bool folded, bool stored)
    {
        CLeaf leaf = new(
            7,
            false,
            1,
            title,
            TCardHinge.TCardHingeMeaning,
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            folded,
            stored,
            [],
            [],
            TCardHinge.TCardHingeBlank,
            [],
            string.Empty,
            string.Empty,
            string.Empty);
        FrameworkElementFactory crown = new(typeof(StackPanel));
        crown.AppendChild(new FrameworkElementFactory(typeof(TextBlock)) { Name = "PCardTitle" });
        crown.AppendChild(new FrameworkElementFactory(typeof(TextBlock)) { Name = "PCardCaption" });
        ItemsControl list = TCardHinge.TCardHingeLoad(TCardHinge.TCardHingeBuild(crown));
        list.ItemsSource = new[] { TInterfaceDeportment.TLeafItemCreate(leaf, leaf.CLeafMeaning) };
        TCardHinge.TCardHingeSettle(list);
        ContentPresenter container = (ContentPresenter)list.ItemContainerGenerator.ContainerFromIndex(0);
        TInterfaceDeportment.TLeafCardRefine(container, list.Items[0]);
        return container;
    }
}
