using System;
using System.Collections.Generic;
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
        _pMeaningList.Add(PCardCreate("Meaning", _pMeaningList.Count + 1));
        PEditorChangeSave();
    }

    private void PCollocationHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationList.Add(PCardCreate("Collocation", _pCollocationList.Count + 1));
        PEditorChangeSave();
    }

    private PCard PCardCreate(string prefix, int position)
    {
        PCard card = new(prefix, position, _pEditorCitation, _pEditorParticle, _pEditorDependence)
        {
            PCardId = _lEngine.LEngineCardCreate()
        };

        card.PCardSentenceApply(_pEditorSentenceOrder);
        PLinkAttach(card);
        PContextAttach(card);
        PLabelAttach(card);
        PEditorChangeAttach(card);
        return card;
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

        list.Remove(card);
        PEditorChangeSave();
        PCardOrderApply(list);
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

        PEditorChangeSave();
        list.Move(current, target);
        PCardOrderApply(list, current, target);
    }

    private void PCardOrderApply(ObservableCollection<PCard> list, int from, int target)
    {
        if (_pEditorDraft == 0)
        {
            return;
        }

        IReadOnlyList<LCardDraft> ordered;
        try
        {
            ordered = _lEngine.LEngineDraftMove(
                _pEditorDraft, ReferenceEquals(list, _pCollocationList), from, target);
        }
        catch (Exception exception)
        {
            PCardOrderRestore(exception);
            return;
        }

        PCardOrderShow(list, ordered);
    }

    private void PCardOrderApply(ObservableCollection<PCard> list)
    {
        if (_pEditorDraft == 0)
        {
            return;
        }

        IReadOnlyList<LCardDraft> ordered;
        try
        {
            ordered = _lEngine.LEngineDraftNormalize(
                _pEditorDraft, ReferenceEquals(list, _pCollocationList));
        }
        catch (Exception exception)
        {
            PCardOrderRestore(exception);
            return;
        }

        PCardOrderShow(list, ordered);
    }

    private void PCardOrderShow(ObservableCollection<PCard> list, IReadOnlyList<LCardDraft> ordered)
    {
        if (ordered.Count != list.Count)
        {
            PCardOrderRestore(null);
            return;
        }

        List<PCard> shown = new(ordered.Count);
        foreach (LCardDraft card in ordered)
        {
            PCard? found = PCardFind(list, shown, card.LCardDraftId);
            if (found is null)
            {
                PCardOrderRestore(null);
                return;
            }

            found.PCardPosition = card.LCardDraftPosition;
            shown.Add(found);
        }

        for (int index = 0; index < shown.Count; index++)
        {
            int current = list.IndexOf(shown[index]);
            if (current != index)
            {
                list.Move(current, index);
            }
        }

        PEditorChangeUpdate();
    }

    private void PCardOrderRestore(Exception? exception)
    {
        if (exception is null)
        {
            _pEditorHost.PWindowFailureShow("Input.OrderFailed");
        }
        else
        {
            _pEditorHost.PWindowFailureShow("Input.OrderFailed", exception);
        }

        PEditorDraftRestore();
    }

    private static PCard? PCardFind(
        ObservableCollection<PCard> list, List<PCard> shown, long id)
    {
        foreach (PCard card in list)
        {
            if (card.PCardId == id && !shown.Contains(card))
            {
                return card;
            }
        }

        return null;
    }

    private ObservableCollection<PCard>? PCardListFind(PCard card)
    {
        return _pMeaningList.Contains(card) ? _pMeaningList
            : _pCollocationList.Contains(card) ? _pCollocationList
            : null;
    }
}
