using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCardPosition
{
    private readonly ObservableCollection<PCard> _qCardPositionMeaning;

    private readonly ObservableCollection<PCard> _qCardPositionCollocation;

    private CCardList _cCardList = null!;

    internal QCardPosition(ObservableCollection<PCard> meanings, ObservableCollection<PCard> collocations)
    {
        _qCardPositionMeaning = meanings;
        _qCardPositionCollocation = collocations;
    }

    internal void QCardPositionIntroduce(CCardList list)
    {
        _cCardList = list;
    }

    internal void QCardPositionApply(FrameworkElement container)
    {
        if (QLook.QLookPartFind<Border>(container, "PCardPosition") is Border position)
        {
            position.MouseLeftButtonDown -= QCardPositionRefine;
            position.MouseLeftButtonDown += QCardPositionRefine;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardPositionText") is TextBox ordinal)
        {
            ordinal.KeyDown -= QCardPositionRefine;
            ordinal.KeyDown -= QCardPositionObserve;
            ordinal.LostFocus -= QCardPositionObserve;
            ordinal.KeyDown += QCardPositionRefine;
            ordinal.KeyDown += QCardPositionObserve;
            ordinal.LostFocus += QCardPositionObserve;
        }
    }

    private void QCardPositionRefine(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount < 2 || sender is not FrameworkElement { DataContext: PCard card } badge)
        {
            return;
        }

        ObservableCollection<PCard>? list = QCardPositionFind(card);

        if (list is null || list.Count <= 1)
        {
            return;
        }

        e.Handled = true;
        card.PCardPositionActive = true;

        badge.Dispatcher.BeginInvoke(
            DispatcherPriority.Input,
            () =>
            {
                if (QField.QFieldCaretFind(badge) is not TextBox box || box.DataContext != card)
                {
                    return;
                }

                box.Focus();
                box.SelectAll();
            });
    }

    private void QCardPositionRefine(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Escape && sender is FrameworkElement { DataContext: PCard card })
        {
            e.Handled = true;
            card.PCardPositionHide();
        }
    }

    private void QCardPositionObserve(object sender, RoutedEventArgs e)
    {
        if (e is KeyEventArgs { Key: not Key.Enter }
            || sender is not TextBox { DataContext: PCard card } box
            || !card.PCardPositionActive)
        {
            return;
        }

        _cCardList.CCardMove(card.PCardId, box.Text);
        if (e is KeyEventArgs)
        {
            e.Handled = true;
        }

        card.PCardPositionHide();
    }

    private ObservableCollection<PCard>? QCardPositionFind(PCard card)
    {
        return _qCardPositionMeaning.Contains(card) ? _qCardPositionMeaning
            : _qCardPositionCollocation.Contains(card) ? _qCardPositionCollocation
            : null;
    }
}
