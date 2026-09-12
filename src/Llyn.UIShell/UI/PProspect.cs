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

    private void PProspectSelect(PProspectItem item)
    {
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

        if (_pEditorDraft == 0)
        {
            PProspectHide();
            return;
        }

        LCourt link;
        try
        {
            link = _lEngine.LEngineCourtStart(
                _pEditorDraft,
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

    private void PProspectShow(PCard card, string word, IReadOnlyList<LEntry> found)
    {
        PProspectShow(card, word, found, true);
    }

    private void PProspectShow(PCard card, string word, IReadOnlyList<LEntry> found, bool chosen)
    {
        _pProspectItem.Clear();
        foreach (LEntry entry in found)
        {
            _pProspectItem.Add(new PProspectItem(
                entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage, false));
        }

        foreach (string language in PProspectLanguageRead())
        {
            _pProspectItem.Add(new PProspectItem(0, word, language, true));
        }

        PTwin.PTwinNameApply(
            _pProspectItem, row => row.PProspectItemHeadword, (row, name) => row.PProspectItemName = name);

        _pProspectCard = card;
        PProspect.PlacementTarget = PLinkBoxFind(card) ?? (UIElement)PContents;
        PProspect.IsOpen = true;
        PProspectList.SelectedIndex = chosen ? 0 : -1;
    }

    private void PProspectHide()
    {
        PProspect.IsOpen = false;
        PProspectList.SelectedIndex = -1;
        _pProspectItem.Clear();
        _pProspectCard = null;
    }

    private IReadOnlyList<string> PProspectLanguageRead()
    {
        List<string> languages = [];
        foreach (PLanguageItem item in _pLanguageItem)
        {
            if (!string.Equals(item.PLanguageItemName, _pSpeakerChoice, StringComparison.Ordinal))
            {
                languages.Add(item.PLanguageItemName);
            }
        }

        languages.Add(_pSpeakerChoice);
        return languages;
    }
}
