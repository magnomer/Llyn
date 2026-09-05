using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PCard> _pSenseList = [];
    private readonly ObservableCollection<PCard> _pCollocationList = [];

    private void PSenseHandle(object sender, RoutedEventArgs e)
    {
        PCard card = new("Meaning", _pSenseList.Count + 1, _pSentenceReference);
        PLinkAttach(card);
        PEditorChangeAttach(card);
        _pSenseList.Add(card);
        PEditorChangeSave();
    }

    private void PCollocationHandle(object sender, RoutedEventArgs e)
    {
        PCard card = new("Collocation", _pCollocationList.Count + 1, _pSentenceReference);
        PLinkAttach(card);
        PEditorChangeAttach(card);
        _pCollocationList.Add(card);
        PEditorChangeSave();
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
        PCardOrderApply(list, 0, 0);
    }

    private void PCardOrderApply(ObservableCollection<PCard> list, int from, int target)
    {
        if (_pEditorDraft.Length == 0)
        {
            return;
        }

        IReadOnlyList<LCardDraft> ordered;
        try
        {
            ordered = _lEngine.LEngineDraftMove(
                _pEditorDraft, ReferenceEquals(list, _pCollocationList), from, target);
        }
        catch (Exception)
        {
            return;
        }

        for (int index = 0; index < list.Count && index < ordered.Count; index++)
        {
            list[index].PCardPosition = ordered[index].LCardDraftPosition;
        }

        PEditorChangeUpdate();
    }

    private ObservableCollection<PCard>? PCardListFind(PCard card)
    {
        return _pSenseList.Contains(card) ? _pSenseList
            : _pCollocationList.Contains(card) ? _pCollocationList
            : null;
    }
}
