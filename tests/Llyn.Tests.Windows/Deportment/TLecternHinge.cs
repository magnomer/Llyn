using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        IReadOnlySet<long>? folded = null;

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

        Assert.Equal(new HashSet<long> { card }, folded);
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
            1,
            TCardHinge.TCardHingeTitle,
            TCardHinge.TCardHingeBlank,
            TCardHinge.TCardHingeMeaning,
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            false,
            true);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        bool? shown = null;
        IReadOnlySet<long>? folded = null;

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

    private static ContentPresenter TLecternHingeRead(CStateWording title, bool folded, bool stored)
    {
        CLeaf leaf = new(
            7,
            1,
            title,
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
            stored);
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
