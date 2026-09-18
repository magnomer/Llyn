using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PProspectItem> _pProspectItem = [];

    private PCard? _pProspectCard;

    private Action<long>? _pProspectChosen;

    internal void PProspectHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PProspectItem item })
        {
            PProspectHide();
            return;
        }

        PProspectSelect(item);
        e.Handled = true;
    }

    private bool PProspectHandle(Key key)
    {
        if (key == Key.Escape)
        {
            PProspectHide();
            return true;
        }

        if (key == Key.Down || key == Key.Up)
        {
            int count = _pProspectItem.Count;
            if (count == 0)
            {
                return false;
            }

            int step = key == Key.Down ? 1 : count - 1;
            int chosen = PProspectList.SelectedIndex < 0
                ? (key == Key.Down ? count - 1 : 0)
                : PProspectList.SelectedIndex;
            PProspectList.SelectedIndex = (chosen + step) % count;
            PProspectList.ScrollIntoView(PProspectList.SelectedItem);
            return true;
        }

        if (key == Key.Enter && PProspectList.SelectedItem is PProspectItem item)
        {
            PProspectSelect(item);
            return true;
        }

        return false;
    }

    internal void PProspectShow(FrameworkElement anchor, Rect place, string word, string language, Action<long> chosen)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(chosen);

        IReadOnlyList<LEntry> found;
        try
        {
            found = _lEngine.LEngineTranslationFind(word, null);
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Mention.FindFailed", exception);
            return;
        }

        PProspectHide();

        List<PProspectItem> stored = new(found.Count);
        foreach (LEntry entry in found)
        {
            stored.Add(new PProspectItem(
                entry.LEntryId,
                entry.LEntryHeadword,
                entry.LEntryLanguage,
                false,
                _lEngine.LEngineEpithetRead(entry.LEntryId)));
        }

        LTwin.LTwinNameApply(
            stored,
            row => row.PProspectItemHeadword,
            (row, name) => row.PProspectItemName = name,
            row => row.PProspectItemId);

        foreach (PProspectItem item in stored)
        {
            if (language.Length == 0 || string.Equals(item.PProspectItemLanguage, language, StringComparison.Ordinal))
            {
                _pProspectItem.Add(item);
            }
        }

        if (_pProspectItem.Count == 0)
        {
            return;
        }

        _pProspectChosen = chosen;
        PProspect.PlacementTarget = anchor;
        PProspect.HorizontalOffset = place.X;
        PProspect.VerticalOffset = place.Bottom - anchor.ActualHeight;
        PProspect.IsOpen = true;
        PProspectList.SelectedIndex = 0;
    }

    private void PProspectSelect(PProspectItem item)
    {
        if (_pProspectChosen is Action<long> chosen)
        {
            PProspectHide();
            chosen(item.PProspectItemId);
            return;
        }

        PCard? card = _pProspectCard;
        if (card is null)
        {
            PProspectHide();
            return;
        }

        if (!item.PProspectItemFresh)
        {
            PLinkSend(card, item.PProspectItemId);
            card.PCardLinkClear();
            PProspectHide();
            return;
        }

        if (PEditorDraft == 0)
        {
            PProspectHide();
            return;
        }

        LCourt link;
        try
        {
            link = _lEngine.LEngineCourtStart(
                PEditorDraft,
                _pEditorOrigin,
                item.PProspectItemHeadword,
                item.PProspectItemLanguage);
        }
        catch (Exception exception)
        {
            PProspectHide();
            _pEditorHost.PWindowFailureShow(PLinkFailureKey, exception);
            return;
        }

        PLinkSend(card, link.LCourtTargetId);
        card.PCardLinkClear();
        PProspectHide();
    }

    private void PProspectShow(PCard card, string word, IReadOnlyList<LEntry> found, bool chosen)
    {
        _pProspectItem.Clear();
        _pProspectChosen = null;
        PProspect.HorizontalOffset = 0;
        PProspect.VerticalOffset = 0;

        List<PProspectItem> stored = new(found.Count);
        foreach (LEntry entry in found)
        {
            stored.Add(new PProspectItem(
                entry.LEntryId,
                entry.LEntryHeadword,
                entry.LEntryLanguage,
                false,
                _lEngine.LEngineEpithetRead(entry.LEntryId)));
        }

        LTwin.LTwinNameApply(
            stored,
            row => row.PProspectItemHeadword,
            (row, name) => row.PProspectItemName = name,
            row => row.PProspectItemId);

        long? self = PEditorEntryRead();
        foreach (PProspectItem item in stored)
        {
            if (self is null || item.PProspectItemId != self)
            {
                _pProspectItem.Add(item);
            }
        }

        foreach (string language in PProspectLanguageRead())
        {
            _pProspectItem.Add(new PProspectItem(0, word, language, true));
        }

        _pProspectCard = card;
        PProspect.PlacementTarget = PLinkBoxFind(card) ?? (UIElement)PContents;
        PProspect.IsOpen = true;
        PProspectList.SelectedIndex = chosen ? 0 : -1;
    }

    private void PProspectHide()
    {
        PProspect.IsOpen = false;
        PProspect.HorizontalOffset = 0;
        PProspect.VerticalOffset = 0;
        PProspectList.SelectedIndex = -1;
        _pProspectItem.Clear();
        _pProspectCard = null;
        _pProspectChosen = null;
    }

    private IReadOnlyList<string> PProspectLanguageRead()
    {
        List<string> languages = [];
        foreach (PLanguageItem item in _pLanguageItem)
        {
            if (!string.Equals(item.PLanguageItemName, PSpeakerLanguageRead(), StringComparison.Ordinal))
            {
                languages.Add(item.PLanguageItemName);
            }
        }

        languages.Add(PSpeakerLanguageRead());
        return languages;
    }
}
