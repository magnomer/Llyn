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
        if (e.ClickCount < 2 || sender is not FrameworkElement { DataContext: PCard card } badge)
        {
            return;
        }

        bool opened = _cCardList.CCardPositionOpen(card.PCardId);
        if (!opened)
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
            _cCardList.CCardPositionClose(card.PCardId);
            card.PCardPositionHide();
        }
    }

    private void QCardPositionObserve(object sender, RoutedEventArgs e)
    {
        if (e is KeyEventArgs { Key: not Key.Enter } || sender is not TextBox { DataContext: PCard card } box)
        {
            return;
        }

        bool settled = _cCardList.CCardPositionSettle(card.PCardId, box.Text);
        e.Handled = settled && e is KeyEventArgs;
        card.PCardPositionHide();
    }
}
