using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PProspectTemplate _pProspectTemplate;

    private Popup PProspect => (Popup)FindName(nameof(PProspect));

    private ListBox PProspectList => (ListBox)FindName(nameof(PProspectList));

    private void PProspectApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PProspectItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PProspectMark") is TextBlock mark)
        {
            mark.Visibility = QLook.QLookVisibleRead(row.PProspectItemFresh);
        }

        if (QLook.QLookPartFind<Run>(container, "PProspectName") is Run name)
        {
            name.Text = row.PProspectItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PProspectEpithet") is Run epithet)
        {
            epithet.Text = "\u2002" + row.PProspectItemEpithet;
        }

        if (QLook.QLookPartFind<Image>(container, "PProspectFlag") is Image flag)
        {
            flag.Source = row.PProspectItemFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PProspectLanguage") is TextBlock language)
        {
            language.Text = row.PProspectItemLanguage;
        }

        if (QLook.QLookPartFind<Grid>(container, "PProspectRow") is Grid surface)
        {
            surface.PreviewMouseLeftButtonDown -= _pProspectTemplate.PProspectHandle;
            surface.PreviewMouseLeftButtonDown += _pProspectTemplate.PProspectHandle;
        }
    }

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
        if (word.Trim().Length == 0)
        {
            return;
        }

        IReadOnlyList<CVistaRow> found;
        try
        {
            found = _qEditor.QEditorArea.CEditorCard.CCardProspectFind(word);
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureRefine("Mention.FindFailed", exception);
            return;
        }

        PProspectHide();

        List<PProspectItem> stored = new(found.Count);
        foreach (CVistaRow entry in found)
        {
            stored.Add(new PProspectItem(
                entry.CVistaRowId,
                entry.CVistaRowHeadword,
                entry.CVistaRowLanguage,
                false,
                entry.CVistaRowEpithet, entry.CVistaRowName));
        }

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
            _qEditor.QEditorArea.CEditorCard.CCardTranslationInsert(
                card.PCardId, item.PProspectItemId, card.PCardLinkPosition);
            card.PCardLinkClear();
            PProspectHide();
            return;
        }

        if (PEditorDraft == 0)
        {
            PProspectHide();
            return;
        }

        long target;
        try
        {
            target = _qEditor.QEditorArea.CEditorCard.CCardCourtStart(
                PEditorDraft,
                _qEditor.QEditorArea.CEditorOrigin,
                item.PProspectItemHeadword,
                item.PProspectItemLanguage);
        }
        catch (Exception exception)
        {
            PProspectHide();
            _pEditorHost.PWindowFailureRefine(PLinkFailureKey, exception);
            return;
        }

        _qEditor.QEditorArea.CEditorCard.CCardTranslationInsert(card.PCardId, target, card.PCardLinkPosition);
        card.PCardLinkClear();
        PProspectHide();
    }

    private void PProspectShow(PCard card, string word, IReadOnlyList<CVistaRow> found, bool chosen)
    {
        _pProspectItem.Clear();
        _pProspectChosen = null;
        PProspect.HorizontalOffset = 0;
        PProspect.VerticalOffset = 0;

        List<PProspectItem> stored = new(found.Count);
        foreach (CVistaRow entry in found)
        {
            stored.Add(new PProspectItem(
                entry.CVistaRowId,
                entry.CVistaRowHeadword,
                entry.CVistaRowLanguage,
                false,
                entry.CVistaRowEpithet, entry.CVistaRowName));
        }

        long? self = _qEditor.QEditorArea.CEditorEntry;
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
            if (!item.PLanguageItemMatch(_qEditor.QEditorArea.CEditorLanguage))
            {
                languages.Add(item.PLanguageItemName);
            }
        }

        languages.Add(_qEditor.QEditorArea.CEditorLanguage);
        return languages;
    }
}
