using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternEtymology
{
    private readonly CDisplayCard _qLecternEtymologyArea;

    private readonly CDisplayRoute _qLecternEtymologyRoute;

    private readonly ObservableCollection<QEtymologyItem> _qLecternEtymologyItem = [];

    private readonly UIElement _qLecternEtymologySection;

    private readonly TextBlock _qLecternEtymologyHeadword;

    public QLecternEtymology(FrameworkElement surface, CDisplayCard area, CDisplayRoute route)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);
        ArgumentNullException.ThrowIfNull(route);

        _qLecternEtymologyArea = area;
        _qLecternEtymologyRoute = route;
        ItemsControl map = QContract.QContractFind<ItemsControl>(surface, "PDisplayEtymology");
        _qLecternEtymologySection = QContract.QContractFind<StackPanel>(surface, "PDisplayEtymologySection");
        _qLecternEtymologyHeadword = QContract.QContractFind<TextBlock>(surface, "PDisplayEtymologyHeadword");
        map.ItemsSource =_qLecternEtymologyItem;
        QLookItem.QLookItemAttach(map, QLecternEtymologyApply);

        map.AddHandler(Hyperlink.ClickEvent, new RoutedEventHandler(QLecternEtymologyObserve));
        _qLecternEtymologySection.PreviewMouseWheel += QLecternEtymologyScroll;
    }

    public void QLecternEtymologyRefine()
    {
        CEtymologyState state = _qLecternEtymologyArea.CDisplayEtymologyRead();
        QEtymologyItem.QEtymologyItemShow(_qLecternEtymologyItem, state);
        _qLecternEtymologyHeadword.Text = state.CEtymologyStateHeadword;
        _qLecternEtymologySection.Visibility = QLook.QLookVisibleRead(_qLecternEtymologyItem.Count > 0);
    }

    internal static void QLecternEtymologyApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QEtymologyItem row)
        {
            return;
        }

        CAncestor ancestor = row.QEtymologyItemAncestor;
        if (QLook.QLookPartFind<ItemsControl>(container, "PEtymologySources") is ItemsControl sources)
        {
            sources.ItemsSource = row.QEtymologyItemSources;
            QLookItem.QLookItemAttach(sources, QLecternEtymologyApply);

            if (QLook.QLookPartFind<Rectangle>(container, "PEtymologyTrunk") is Rectangle trunk)
            {
                trunk.SetBinding(UIElement.VisibilityProperty, new Binding(nameof(ItemsControl.HasItems))
                {
                    Source = sources,
                    Converter = new BooleanToVisibilityConverter(),
                });
            }
        }

        if (QLook.QLookPartFind<Rectangle>(container, "PEtymologyPrior") is Rectangle prior)
        {
            prior.Visibility = QLook.QLookVisibleRead(!row.QEtymologyItemFirst);
        }

        if (QLook.QLookPartFind<Rectangle>(container, "PEtymologyNext") is Rectangle next)
        {
            next.Visibility = QLook.QLookVisibleRead(!row.QEtymologyItemLast);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PEtymologyTongue") is TextBlock tongue)
        {
            tongue.Text = ancestor.CAncestorLanguage;
            tongue.Visibility = QLook.QLookVisibleRead(ancestor.CAncestorLanguage.Length > 0);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PEtymologyText") is TextBlock text)
        {
            Inline word = new Run(ancestor.CAncestorText);
            if (ancestor.CAncestorLinked)
            {
                word = new Hyperlink(word);
                word.SetResourceReference(TextElement.ForegroundProperty, "Theme.Accent");
            }

            text.Inlines.Clear();
            text.Inlines.Add(word);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PEtymologyGloss") is TextBlock gloss)
        {
            gloss.Text = ancestor.CAncestorGloss;
            gloss.Visibility = QLook.QLookVisibleRead(ancestor.CAncestorGloss.Length > 0);
        }

        if (QLook.QLookPartFind<Border>(container, "PEtymologyFrame") is Border frame)
        {
            frame.Visibility = QLook.QLookVisibleRead(!ancestor.CAncestorBorrowed);
            frame.SetResourceReference(
                Border.BorderBrushProperty, ancestor.CAncestorLinked ? "Theme.Accent" : "Theme.Line");
        }

        if (QLook.QLookPartFind<Rectangle>(container, "PEtymologyTrace") is Rectangle trace)
        {
            trace.Visibility = QLook.QLookVisibleRead(ancestor.CAncestorBorrowed);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PEtymologyMark") is TextBlock mark)
        {
            mark.Visibility = QLook.QLookVisibleRead(ancestor.CAncestorLinked);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PEtymologyRelation") is TextBlock relation)
        {
            relation.Text = (ancestor.CAncestorRelation is null
                ? null
                : row.QEtymologyItemRelations.FirstOrDefault(choice =>
                    choice.QEtymologyChoiceAncestor.CAncestorChoiceId == ancestor.CAncestorRelation))?.ToString()
                ?? string.Empty;
            relation.Visibility = QLook.QLookVisibleRead(relation.Text.Length > 0);
        }
    }

    private void QLecternEtymologyObserve(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkContentElement { DataContext: QEtymologyItem row })
        {
            _qLecternEtymologyRoute.CDisplayChipOpen(row.QEtymologyItemAncestor.CAncestorEntry);
        }
    }

    private void QLecternEtymologyScroll(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;
        _qLecternEtymologySection.RaiseEvent(
            new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) { RoutedEvent = UIElement.MouseWheelEvent });
    }
}
