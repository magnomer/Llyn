using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PProfferTemplate _pProfferTemplate;

    private Popup PProffer => (Popup)FindName(nameof(PProffer));

    private Border PProfferSheet => (Border)FindName(nameof(PProfferSheet));

    private ListBox PProfferList => (ListBox)FindName(nameof(PProfferList));

    private void PProfferAttach()
    {
        PProfferList.ItemsSource = _pProfferItem;
        QLookItem.QLookItemAttach(PProfferList, PProfferApply);
        PProffer.CustomPopupPlacementCallback = PProfferPlace;
        PProffer.Closed += PProfferCloseRefine;
    }

    private void PProfferApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PProfferItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Run>(container, "PProfferLead") is Run lead)
        {
            lead.Text = row.PProfferItemLead;
        }

        if (QLook.QLookPartFind<Run>(container, "PProfferMark") is Run mark)
        {
            mark.Text = row.PProfferItemMark;
        }

        if (QLook.QLookPartFind<Run>(container, "PProfferTail") is Run tail)
        {
            tail.Text = row.PProfferItemTail;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PProfferCount") is TextBlock count)
        {
            count.Text = row.PProfferItemCount;
        }

        if (QLook.QLookPartFind<Grid>(container, "PProfferRow") is Grid surface)
        {
            surface.PreviewMouseLeftButtonDown -= PProfferMissRefine;
            surface.PreviewMouseLeftButtonDown -= PProfferPickObserve;
            surface.PreviewMouseLeftButtonDown += PProfferMissRefine;
            surface.PreviewMouseLeftButtonDown += PProfferPickObserve;
        }
    }

    private const double PProfferShade = 10;
    private const double PProfferGap = 6;

    private readonly ObservableCollection<PProfferItem> _pProfferItem = [];

    private void PProfferMissRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PProfferItem })
        {
            PProfferShutRefine();
        }
    }

    private void PProfferPickObserve(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PProfferItem item })
        {
            return;
        }

        switch (Keyboard.FocusedElement)
        {
            case FrameworkElement { DataContext: PRegisterCaret caret } when PCardRegisterFind(caret) is PCard card:
                _qEditor.QEditorArea.CEditorCard.CCardRegisterInsert(
                    card.PCardId, item.PProfferItemId, card.PCardRegisterPosition);
                e.Handled = true;
                PProfferShutRefine();
                card.PCardRegisterClear();
                break;
            case FrameworkElement { DataContext: PContextCaret caret } when PCardContextFind(caret) is PCard card:
                _qEditor.QEditorArea.CEditorCard.CCardSituationInsert(
                    card.PCardId, item.PProfferItemId, card.PCardContextPosition);
                e.Handled = true;
                PProfferShutRefine();
                card.PCardContextClear();
                break;
            case FrameworkElement { DataContext: PSentence row } when PCardSentenceFind(row) is PCard card:
                _qEditor.QEditorArea.CEditorSentence.CSentenceCitationSet(
                    card.PCardId, row.PSentenceRow, item.PProfferItemId);
                e.Handled = true;
                PProfferShutRefine();
                break;
        }
    }

    private void PProfferKeyRefine(object sender, KeyEventArgs e)
    {
        if (!PProffer.IsOpen)
        {
            return;
        }

        if (e.Key == Key.Escape)
        {
            PProfferShutRefine();
            e.Handled = true;
            return;
        }

        int count = _pProfferItem.Count;
        if ((e.Key != Key.Down && e.Key != Key.Up) || count == 0)
        {
            return;
        }

        int step = e.Key == Key.Down ? 1 : count - 1;
        int chosen = PProfferList.SelectedIndex < 0
            ? (e.Key == Key.Down ? count - 1 : 0)
            : PProfferList.SelectedIndex;
        PProfferList.SelectedIndex = (chosen + step) % count;
        PProfferList.ScrollIntoView(PProfferList.SelectedItem);
        e.Handled = true;
    }

    private void PProfferCloseRefine(object? sender, EventArgs e)
    {
        PProfferList.SelectedIndex = -1;
    }

    private void PProfferKeyObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        if (PProfferList.SelectedItem is not PProfferItem item)
        {
            return;
        }

        switch (sender)
        {
            case FrameworkElement { DataContext: PRegisterCaret caret } when PCardRegisterFind(caret) is PCard card:
                _qEditor.QEditorArea.CEditorCard.CCardRegisterInsert(
                    card.PCardId, item.PProfferItemId, card.PCardRegisterPosition);
                e.Handled = true;
                PProfferShutRefine();
                card.PCardRegisterClear();
                break;
            case FrameworkElement { DataContext: PContextCaret caret } when PCardContextFind(caret) is PCard card:
                _qEditor.QEditorArea.CEditorCard.CCardSituationInsert(
                    card.PCardId, item.PProfferItemId, card.PCardContextPosition);
                e.Handled = true;
                PProfferShutRefine();
                card.PCardContextClear();
                break;
            case FrameworkElement { DataContext: PSentence row } when PCardSentenceFind(row) is PCard card:
                _qEditor.QEditorArea.CEditorSentence.CSentenceCitationSet(
                    card.PCardId, row.PSentenceRow, item.PProfferItemId);
                e.Handled = true;
                PProfferShutRefine();
                break;
        }
    }

    private void PProfferRegisterRefine(PCard card, CProffer offer)
    {
        card.PCardRegisterRefine(offer.CProfferText);
        if (!offer.CProfferShown)
        {
            PProfferShutRefine();
            return;
        }

        PProfferOpenRefine(PProfferRegisterFind(card), offer.CProfferRows);
    }

    private void PProfferOpenRefine(TextBox? box, IReadOnlyList<CProfferRow> rows)
    {
        _pProfferItem.Clear();
        foreach (CProfferRow row in rows)
        {
            _pProfferItem.Add(new PProfferItem(
                row.CProfferRowId,
                row.CProfferRowLead,
                row.CProfferRowMark,
                row.CProfferRowTail,
                row.CProfferRowCount));
        }

        PProffer.PlacementTarget = PProfferFrameFind(box) ?? box ?? (UIElement)PContents;
        PProfferSheet.SetBinding(
            FrameworkElement.MinWidthProperty,
            new Binding(nameof(FrameworkElement.ActualWidth)) { Source = PProffer.PlacementTarget });
        PProffer.IsOpen = true;
        PProfferList.SelectedIndex = -1;
    }

    private void PProfferSituationRefine(PCard card, CProffer offer)
    {
        card.PCardContextRefine(offer.CProfferText);
        if (!offer.CProfferShown)
        {
            PProfferShutRefine();
            return;
        }

        PProfferOpenRefine(PProfferBoxFind(card), offer.CProfferRows);
    }

    private void PProfferCitationRefine(TextBox box, CProffer offer)
    {
        if (!offer.CProfferShown)
        {
            PProfferShutRefine();
            return;
        }

        PProfferOpenRefine(box, offer.CProfferRows);
    }

    private void PProfferShutRefine()
    {
        PProffer.IsOpen = false;
        PProfferList.SelectedIndex = -1;
        _pProfferItem.Clear();
    }

    private TextBox? PProfferRegisterFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PRegisterCaret row } box &&
            PCardRegisterFind(row) == card
            ? box
            : null;
    }

    private TextBox? PProfferBoxFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PContextCaret row } box &&
            PCardContextFind(row) == card
            ? box
            : null;
    }

    private static CustomPopupPlacement[] PProfferPlace(Size popup, Size target, Point offset)
    {
        var pBelow = new Point(-PProfferShade, target.Height + PProfferGap - PProfferShade);
        var pAbove = new Point(-PProfferShade, PProfferShade - PProfferGap - popup.Height);
        return
        [
            new CustomPopupPlacement(pBelow, PopupPrimaryAxis.Vertical),
            new CustomPopupPlacement(pAbove, PopupPrimaryAxis.Vertical),
        ];
    }

    private static FrameworkElement? PProfferFrameFind(TextBox? box)
    {
        if (box is null)
        {
            return null;
        }

        box.ApplyTemplate();
        return box.Template?.FindName(QField.QFieldSurfaceName, box) as FrameworkElement;
    }
}
