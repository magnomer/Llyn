using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardHinge
{
    internal static readonly CStateWording TCardHingeBlank = new(string.Empty, null, true, null);

    internal static readonly CStateWording TCardHingeTitle = new("Weather", null, false, null);

    internal static readonly CStateWording TCardHingeMeaning = new("a liquid", null, false, null);

    [Fact]
    public void CardRowRefine_FoldedCard_CollapsesBodyChecksHingeAndClosesHeader()
    {
        TCardHingeRun(() =>
        {
            ContentPresenter card = TCardHingeRefine(TCardHingeTitle, true, true);
            Border header = (Border)TCardHingeResolve(card, "PCardHeader");

            Assert.Equal(Visibility.Collapsed, TCardHingeResolve(card, "PCardBody").Visibility);
            Assert.True(((ToggleButton)TCardHingeResolve(card, "PCardHinge")).IsChecked);
            Assert.Equal(Visibility.Visible, TCardHingeResolve(card, "PCardHinge").Visibility);
            Assert.Equal(new CornerRadius(12), header.CornerRadius);
            Assert.Equal(new Thickness(0), header.BorderThickness);
        });
    }

    [Fact]
    public void CardRowRefine_UnfoldedCard_ShowsBodyAndTitleBox()
    {
        TCardHingeRun(() =>
        {
            ContentPresenter card = TCardHingeRefine(TCardHingeBlank, false, true);
            Border header = (Border)TCardHingeResolve(card, "PCardHeader");

            Assert.Equal(Visibility.Visible, TCardHingeResolve(card, "PCardBody").Visibility);
            Assert.False(((ToggleButton)TCardHingeResolve(card, "PCardHinge")).IsChecked);
            Assert.Equal(Visibility.Visible, TCardHingeResolve(card, "PTitle").Visibility);
            Assert.Equal(Visibility.Collapsed, TCardHingeResolve(card, "PCardPeek").Visibility);
            Assert.Equal(new CornerRadius(12, 12, 0, 0), header.CornerRadius);
        });
    }

    [Fact]
    public void CardRowRefine_FoldedCardWithBlankTitle_ShowsPeekBehindTheKeptTitleBox()
    {
        TCardHingeRun(() =>
        {
            ContentPresenter card = TCardHingeRefine(TCardHingeBlank, true, true);

            Assert.Equal(Visibility.Visible, TCardHingeResolve(card, "PCardPeek").Visibility);
            Assert.Equal("a liquid", ((TextBlock)TCardHingeResolve(card, "PCardPeek")).Text);
            Assert.Equal(Visibility.Visible, TCardHingeResolve(card, "PTitle").Visibility);
            Assert.True(TCardHingeResolve(card, "PTitle").Focusable);
        });
    }

    [Fact]
    public void CardRowRefine_FoldedCardWithTitle_ShowsTitleBoxAndHidesPeek()
    {
        TCardHingeRun(() =>
        {
            ContentPresenter card = TCardHingeRefine(TCardHingeTitle, true, true);

            Assert.Equal(Visibility.Visible, TCardHingeResolve(card, "PTitle").Visibility);
            Assert.Equal(Visibility.Collapsed, TCardHingeResolve(card, "PCardPeek").Visibility);
        });
    }

    [Fact]
    public void CardRowRefine_UnstoredCard_CollapsesHinge()
    {
        TCardHingeRun(() =>
        {
            ContentPresenter card = TCardHingeRefine(TCardHingeTitle, false, false);

            Assert.Equal(Visibility.Collapsed, TCardHingeResolve(card, "PCardHinge").Visibility);
            Assert.Equal(Visibility.Visible, TCardHingeResolve(card, "PCardBody").Visibility);
        });
    }

    [Fact]
    public void CardHingeClick_StoredCard_FoldsTheCardByIdThroughTheEditorList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = TCardHingePrepare(engine);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(water.LEntryId);
        CCardDraft draft = editor.TEditorDraftRead()!.CEntryDraftMeanings[0];
        IReadOnlySet<long>? folded = null;
        IReadOnlySet<long>? unfolded = null;

        TCardHingeRun(() =>
        {
            ItemsControl list = TCardHingeLoad(TCardHingeCreate());
            list.ItemsSource = new[] { TInterfaceDeportment.TCardFoldCreate(draft) };
            TCardHingeSettle(list);
            TInterfaceDeportment.TCardHingeAttach(list, editor.TEditorFixtureEditor);
            ToggleButton hinge = TCardHingeFind(list);

            TCardHingeToggle(hinge, true);
            folded = engine.TEngineFoldRead(water.LEntryId);
            TCardHingeToggle(hinge, false);
            unfolded = engine.TEngineFoldRead(water.LEntryId);
        });

        Assert.Equal(new HashSet<long> { draft.CCardDraftId }, folded);
        Assert.Empty(unfolded!);
    }

    [Fact]
    public void CardHingeClick_RefusedWrite_PutsTheHingeBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        CCardDraft draft = new(
            7, 1, TCardHingeTitle, TCardHingeBlank, TCardHingeMeaning, [], [], [], [], [], [], [], false, true);
        bool? shown = null;

        TCardHingeRun(() =>
        {
            ItemsControl list = TCardHingeLoad(TCardHingeCreate());
            list.ItemsSource = new[] { TInterfaceDeportment.TCardFoldCreate(draft) };
            TCardHingeSettle(list);
            TInterfaceDeportment.TCardHingeAttach(list, editor.TEditorFixtureEditor);
            ToggleButton hinge = TCardHingeFind(list);

            TCardHingeToggle(hinge, true);
            shown = hinge.IsChecked;
        });

        Assert.False(shown);
    }

    [Fact]
    public void CardHingeLook_UnfoldedCard_TurnsTheExpandIconHalfway()
    {
        TWindow.TWindowRun(() =>
        {
            TCardLookPrepare();
            object? icon = TInterfaceDeportment.TLookSettingRead(
                "Theme.Card.Hinge", "PSurfaceMark", QIconImage.QIconSourceProperty, false);
            object? turn = TInterfaceDeportment.TLookSettingRead(
                "Theme.Card.Hinge", "PSurfaceMark", UIElement.RenderTransformProperty, false);
            object? mask = TInterfaceDeportment.TLookSettingRead(
                "Theme.Card.Hinge", "PSurfaceMark", UIElement.OpacityMaskProperty, false);

            Assert.NotNull(icon);
            Assert.Same(QIcon.QIconResolve("expand", 12), icon);
            Assert.Equal(180, Assert.IsType<RotateTransform>(turn).Angle);
            Assert.Null(mask);
        });
    }

    [Fact]
    public void CardHingeLook_FoldedCard_LeavesTheExpandIconUnturned()
    {
        TWindow.TWindowRun(() =>
        {
            TCardLookPrepare();
            object? icon = TInterfaceDeportment.TLookSettingRead(
                "Theme.Card.Hinge", "PSurfaceMark", QIconImage.QIconSourceProperty, true);
            object? turn = TInterfaceDeportment.TLookSettingRead(
                "Theme.Card.Hinge", "PSurfaceMark", UIElement.RenderTransformProperty, true);

            Assert.Same(QIcon.QIconResolve("expand", 12), icon);
            Assert.True(Assert.IsAssignableFrom<Transform>(turn).Value.IsIdentity);
        });
    }

    private static ContentPresenter TCardHingeRefine(CStateWording title, bool folded, bool stored)
    {
        CCardDraft draft = new(
            7, 1, title, TCardHingeBlank, TCardHingeMeaning, [], [], [], [], [], [], [], folded, stored);
        FrameworkElementFactory crown = new(typeof(StackPanel));
        crown.AppendChild(new FrameworkElementFactory(typeof(TextBox)) { Name = "PTitle" });
        ItemsControl list = TCardHingeLoad(TCardHingeBuild(crown));
        list.ItemsSource = new[] { TInterfaceDeportment.TCardFoldCreate(draft) };
        TCardHingeSettle(list);
        ContentPresenter container = (ContentPresenter)list.ItemContainerGenerator.ContainerFromIndex(0);
        TInterfaceDeportment.TCardRowRefine(container, list.Items[0]);
        return container;
    }

    internal static FrameworkElement TCardHingeResolve(ContentPresenter container, string name)
    {
        return (FrameworkElement)container.ContentTemplate.FindName(name, container);
    }

    internal static FrameworkElementFactory TCardHingeBuild(FrameworkElementFactory crown)
    {
        FrameworkElementFactory header = new(typeof(Border)) { Name = "PCardHeader" };
        header.SetValue(Border.CornerRadiusProperty, new CornerRadius(12, 12, 0, 0));
        header.SetValue(Border.BorderThicknessProperty, new Thickness(0, 0, 0, 1));
        crown.AppendChild(new FrameworkElementFactory(typeof(TextBlock)) { Name = "PCardPeek" });
        crown.AppendChild(TCardHingeCreate());
        header.AppendChild(crown);
        FrameworkElementFactory card = new(typeof(StackPanel));
        card.AppendChild(header);
        card.AppendChild(new FrameworkElementFactory(typeof(Grid)) { Name = "PCardBody" });
        return card;
    }

    internal static FrameworkElementFactory TCardHingeCreate()
    {
        FrameworkElementFactory shell = new(typeof(StackPanel));
        shell.AppendChild(new FrameworkElementFactory(typeof(ToggleButton)) { Name = "PCardHinge" });
        return shell;
    }

    internal static ItemsControl TCardHingeLoad(FrameworkElementFactory card)
    {
        ItemsControl list = new()
        {
            Template = new ControlTemplate(typeof(ItemsControl))
            {
                VisualTree = new FrameworkElementFactory(typeof(ItemsPresenter)),
            },
            ItemTemplate = new DataTemplate { VisualTree = card },
        };
        list.Resources["Theme.Card.FoldRadius"] = new CornerRadius(12);
        list.Resources["Theme.Card.FoldEdge"] = new Thickness(0);
        return list;
    }

    internal static void TCardHingeSettle(FrameworkElement surface)
    {
        surface.Measure(new Size(400, 400));
        surface.Arrange(new Rect(0, 0, 400, 400));
        surface.UpdateLayout();
    }

    internal static ToggleButton TCardHingeFind(ItemsControl list)
    {
        ContentPresenter container = (ContentPresenter)list.ItemContainerGenerator.ContainerFromIndex(0);
        return (ToggleButton)TCardHingeResolve(container, "PCardHinge");
    }

    internal static void TCardHingeToggle(ToggleButton hinge, bool folded)
    {
        hinge.IsChecked = folded;
        hinge.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, hinge));
    }

    internal static LEntry TCardHingePrepare(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }

    private static void TCardLookPrepare()
    {
        System.Windows.Application application =
            System.Windows.Application.Current ?? new System.Windows.Application();
        application.Resources["PIconRoot"] = "pack://application:,,,/Llyn.Tests.Windows;component/icons/";
        application.Resources["Theme.Popup.ProgressBar.Sweep"] = new DoubleAnimation().GetAsFrozen();
        application.Resources["Theme.Sound.Rebuild.Spin"] = new DoubleAnimation().GetAsFrozen();
    }

    internal static void TCardHingeRun(Action body)
    {
        Exception? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                body();
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
