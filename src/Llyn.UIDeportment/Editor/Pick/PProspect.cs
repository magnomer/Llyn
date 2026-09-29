using System;
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
            surface.PreviewMouseLeftButtonDown -= PProspectMissRefine;
            surface.PreviewMouseLeftButtonDown -= PProspectPickObserve;
            surface.PreviewMouseLeftButtonDown += PProspectMissRefine;
            surface.PreviewMouseLeftButtonDown += PProspectPickObserve;
        }
    }

    private readonly ObservableCollection<PProspectItem> _pProspectItem = [];

    private void PProspectMissRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PProspectItem })
        {
            PProspectShutRefine();
        }
    }

    private void PProspectPickObserve(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PProspectItem item })
        {
            return;
        }

        e.Handled = true;
        switch (PProspect.PlacementTarget)
        {
            case TextBox { DataContext: PLinkCaret caret }:
                if (PCardLinkFind(caret) is PCard owner)
                {
                    _qEditor.QEditorArea.CEditorCard.CCardTranslationInsert(
                        owner.PCardId,
                        item.PProspectItemId,
                        item.PProspectItemHeadword,
                        item.PProspectItemLanguage,
                        owner.PCardLinkPosition);
                    PProspectShutRefine();
                    owner.PCardLinkClear();
                }

                return;
            case TextBox box when box == PEtymologyField.PEtymologyBox:
                _qEditor.QEditorArea.CEditorCard.CCardMentionSave(
                    box.Text, box.SelectionStart, box.SelectionLength, item.PProspectItemId);
                break;
            case TextBox { DataContext: PEtymon caret }:
                _qEditor.QEditorArea.CEditorCard.CCardEtymonAdd(item.PProspectItemId);
                PEtymonCaretRefine(caret);
                break;
            case TextBox { DataContext: PSentence row } box:
                if (PCardSentenceFind(row) is PCard card)
                {
                    _qEditor.QEditorArea.CEditorSentence.CSentenceMentionAdd(
                        card.PCardId,
                        row.PSentenceRow,
                        box.Text,
                        box.SelectionStart,
                        box.SelectionLength,
                        item.PProspectItemId);
                }

                break;
            case TextBox box:
                PProspectPicked?.Invoke(box, item.PProspectItemId);
                break;
        }

        PProspectShutRefine();
    }

    private void PProspectKeyRefine(object sender, KeyEventArgs e)
    {
        if (!PProspect.IsOpen)
        {
            return;
        }

        if (e.Key == Key.Escape)
        {
            PProspectShutRefine();
            e.Handled = true;
            return;
        }

        int count = _pProspectItem.Count;
        if ((e.Key != Key.Down && e.Key != Key.Up) || count == 0)
        {
            return;
        }

        int step = e.Key == Key.Down ? 1 : count - 1;
        int chosen = PProspectList.SelectedIndex < 0
            ? (e.Key == Key.Down ? count - 1 : 0)
            : PProspectList.SelectedIndex;
        PProspectList.SelectedIndex = (chosen + step) % count;
        PProspectList.ScrollIntoView(PProspectList.SelectedItem);
        e.Handled = true;
    }

    private void PProspectCloseRefine(object? sender, EventArgs e)
    {
        PProspectList.SelectedIndex = -1;
    }

    private void PProspectKeyObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PLinkCaret row }
            || PCardLinkFind(row) is not PCard card)
        {
            return;
        }

        if (PProspectList.SelectedItem is not PProspectItem item)
        {
            return;
        }

        _qEditor.QEditorArea.CEditorCard.CCardTranslationInsert(
            card.PCardId,
            item.PProspectItemId,
            item.PProspectItemHeadword,
            item.PProspectItemLanguage,
            card.PCardLinkPosition);
        e.Handled = true;
        PProspectShutRefine();
        card.PCardLinkClear();
    }

    internal event Action<TextBox, long>? PProspectPicked;

    internal void PProspectPlaceRefine(FrameworkElement anchor, Rect place)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        PProspectShutRefine();
        PProspect.PlacementTarget = anchor;
        PProspect.HorizontalOffset = place.X;
        PProspect.VerticalOffset = place.Bottom - anchor.ActualHeight;
    }

    internal void PProspectOpenRefine(CProspect prospect)
    {
        ArgumentNullException.ThrowIfNull(prospect);

        _pProspectItem.Clear();
        if (!prospect.CProspectShown)
        {
            return;
        }

        foreach (CVistaRow entry in prospect.CProspectRows)
        {
            _pProspectItem.Add(new PProspectItem(
                entry.CVistaRowId,
                entry.CVistaRowHeadword,
                entry.CVistaRowLanguage,
                false,
                entry.CVistaRowEpithet, entry.CVistaRowName));
        }

        foreach (string language in prospect.CProspectLanguages)
        {
            _pProspectItem.Add(new PProspectItem(0, prospect.CProspectWord, language, true));
        }

        PProspect.IsOpen = true;
        PProspectList.SelectedIndex = prospect.CProspectChosen ? 0 : -1;
    }

    private void PProspectTranslationRefine(PCard card, CProspect prospect)
    {
        PProspect.HorizontalOffset = 0;
        PProspect.VerticalOffset = 0;
        PProspect.PlacementTarget = PLinkBoxFind(card) ?? (UIElement)PContents;
        PProspectOpenRefine(prospect);
    }

    private void PProspectShutRefine()
    {
        PProspect.IsOpen = false;
        PProspect.HorizontalOffset = 0;
        PProspect.VerticalOffset = 0;
        PProspectList.SelectedIndex = -1;
        _pProspectItem.Clear();
    }
}
