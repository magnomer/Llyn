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
            int chosen = PProspectList.SelectedIndex < 0 ? 0 : PProspectList.SelectedIndex;
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
            card.PCardLinkCommit(
                item.PProspectItemId, item.PProspectItemHeadword, item.PProspectItemLanguage);
            card.PCardLinkClear();
            PProspectHide();
            return;
        }

        LEntry created;
        try
        {
            created = _lEngine.LEngineTranslationCreate(
                item.PProspectItemHeadword, item.PProspectItemLanguage);
        }
        catch (Exception exception)
        {
            PProspectHide();
            _pEditorHost.PWindowFailureShow(PLinkFailureKey, exception);
            return;
        }

        _pLinkFresh.Add(created.LEntryId);
        card.PCardLinkCommit(created.LEntryId, created.LEntryHeadword, created.LEntryLanguage);
        card.PCardLinkClear();
        PProspectHide();
    }

    private void PProspectShow(PCard card, string word, IReadOnlyList<LEntry> found)
    {
        _pProspectItem.Clear();
        foreach (LEntry entry in found)
        {
            _pProspectItem.Add(new PProspectItem(
                entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage, false));
        }

        foreach (string language in PProspectLanguageRead())
        {
            _pProspectItem.Add(new PProspectItem(string.Empty, word, language, true));
        }

        _pProspectCard = card;
        PProspect.PlacementTarget = PLinkBoxFind(card) ?? (UIElement)PContents;
        PProspect.IsOpen = true;
        PProspectList.SelectedIndex = 0;
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
        foreach (PTongueItem item in _pTongueItem)
        {
            if (!string.Equals(item.PTongueItemName, _pLanguageChoice, StringComparison.Ordinal))
            {
                languages.Add(item.PTongueItemName);
            }
        }

        languages.Add(_pLanguageChoice);
        return languages;
    }
}
