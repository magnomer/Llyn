using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PCard> _pMeaningList = [];
    private readonly ObservableCollection<PCard> _pCollocationList = [];

    private void PMeaningHandle(object sender, RoutedEventArgs e)
    {
        PEditorRequestSend(
            new LRequestCardAddition(PEditorDraft, LCardKind.LCardKindMeaning, 0, _pMeaningList.Count));
    }

    private void PCollocationHandle(object sender, RoutedEventArgs e)
    {
        PEditorRequestSend(
            new LRequestCardAddition(PEditorDraft, LCardKind.LCardKindCollocation, 0, _pCollocationList.Count));
    }

    internal void PCardHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCard card })
        {
            return;
        }

        ObservableCollection<PCard>? list = PCardListFind(card);

        if (list is null || list.Count <= 1)
        {
            return;
        }

        PEditorRequestSend(new LRequestCardRemoval(PEditorDraft, card.PCardId));
    }

    internal void PCardPositionHandle(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount < 2 || sender is not FrameworkElement { DataContext: PCard card } badge)
        {
            return;
        }

        ObservableCollection<PCard>? list = PCardListFind(card);

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
                if (PEditorCaretFind(badge) is not TextBox box || box.DataContext != card)
                {
                    return;
                }

                box.Focus();
                box.SelectAll();
            });
    }

    internal void PCardPositionAccept(object sender, KeyEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCard card })
        {
            return;
        }

        if (e.Key is Key.Enter)
        {
            e.Handled = true;
            PCardPositionApply(card);
            return;
        }

        if (e.Key is Key.Escape)
        {
            e.Handled = true;
            card.PCardPositionHide();
        }
    }

    internal void PCardPositionCommit(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card } && card.PCardPositionActive)
        {
            PCardPositionApply(card);
        }
    }

    private void PCardPositionApply(PCard card)
    {
        string written = card.PCardPositionText;
        card.PCardPositionHide();

        ObservableCollection<PCard>? list = PCardListFind(card);

        if (list is null || list.Count <= 1)
        {
            return;
        }

        int current = list.IndexOf(card);

        if (current < 0 ||
            !int.TryParse(written, NumberStyles.Integer, CultureInfo.InvariantCulture, out int wanted))
        {
            return;
        }

        int target = wanted < 1 ? 0 : wanted > list.Count ? list.Count - 1 : wanted - 1;

        if (target == current)
        {
            return;
        }

        PCardMove(card, target);
    }

    private void PCardMove(PCard card, int target)
    {
        PEditorRequestSend(new LRequestCardShift(PEditorDraft, card.PCardId, 0, target));
    }

    private ObservableCollection<PCard>? PCardListFind(PCard card)
    {
        return _pMeaningList.Contains(card) ? _pMeaningList
            : _pCollocationList.Contains(card) ? _pCollocationList
            : null;
    }
}
