using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCardPosition
{
    private CCardList _cCardList = null!;

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
        if (e.ClickCount < 2 || sender is not FrameworkElement { DataContext: QPlacard card } badge)
        {
            return;
        }

        bool opened = _cCardList.CCardPositionOpen(card.QPlacardCollocated, card.QPlacardId);
        if (!opened)
        {
            return;
        }

        e.Handled = true;
        card.QPlacardPositionActive = true;

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
        if (e.Key is Key.Escape && sender is FrameworkElement { DataContext: QPlacard card })
        {
            e.Handled = true;
            _cCardList.CCardPositionClose(card.QPlacardCollocated, card.QPlacardId);
            card.QPlacardPositionHide();
        }
    }

    private void QCardPositionObserve(object sender, RoutedEventArgs e)
    {
        if (e is KeyEventArgs { Key: not Key.Enter } || sender is not TextBox { DataContext: QPlacard card } box)
        {
            return;
        }

        bool settled = _cCardList.CCardPositionSettle(card.QPlacardCollocated, card.QPlacardId, box.Text);
        e.Handled = settled && e is KeyEventArgs;
        card.QPlacardPositionHide();
    }
}
