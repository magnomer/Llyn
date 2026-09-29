using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PSlateTemplate _pSlateTemplate;

    private Popup PSlate => (Popup)FindName(nameof(PSlate));

    private Border PSlateSheet => (Border)FindName(nameof(PSlateSheet));

    private ListBox PSlateList => (ListBox)FindName(nameof(PSlateList));

    private void PSlateAttach()
    {
        PSlateList.ItemsSource = _pSlateItem;
        QLookItem.QLookItemAttach(PSlateList, PSlateApply);
        PSlate.CustomPopupPlacementCallback = PSlatePlace;
        PSlate.Closed += PSlateCloseRefine;
    }

    private void PSlateApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PSlateItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Run>(container, "PSlateLead") is Run lead)
        {
            lead.Text = row.PSlateItemLead;
        }

        if (QLook.QLookPartFind<Run>(container, "PSlateMark") is Run mark)
        {
            mark.Text = row.PSlateItemMark;
        }

        if (QLook.QLookPartFind<Run>(container, "PSlateTail") is Run tail)
        {
            tail.Text = row.PSlateItemTail;
        }

        if (QLook.QLookPartFind<Grid>(container, "PSlateRow") is Grid surface)
        {
            surface.PreviewMouseLeftButtonDown -= PSlateMissRefine;
            surface.PreviewMouseLeftButtonDown -= PSlatePickObserve;
            surface.PreviewMouseLeftButtonDown += PSlateMissRefine;
            surface.PreviewMouseLeftButtonDown += PSlatePickObserve;
        }
    }

    private const double PSlateShade = 10;
    private const double PSlateGap = 6;

    private readonly ObservableCollection<PSlateItem> _pSlateItem = [];

    private void PSlateMissRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSlateItem })
        {
            PSlateShutRefine();
        }
    }

    private void PSlatePickObserve(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSlateItem item }
            || Keyboard.FocusedElement is not FrameworkElement { DataContext: PLabelCaret row }
            || PCardLabelFind(row) is not PCard card)
        {
            return;
        }

        _qEditor.QEditorArea.CEditorCard.CCardTagInsert(card.PCardId, item.PSlateItemId, card.PCardLabelPosition);
        e.Handled = true;
        PSlateShutRefine();
        card.PCardLabelClear();
    }

    private void PSlateKeyRefine(object sender, KeyEventArgs e)
    {
        if (!PSlate.IsOpen)
        {
            return;
        }

        if (e.Key == Key.Escape)
        {
            PSlateShutRefine();
            e.Handled = true;
            return;
        }

        int count = _pSlateItem.Count;
        if ((e.Key != Key.Down && e.Key != Key.Up) || count == 0)
        {
            return;
        }

        int step = e.Key == Key.Down ? 1 : count - 1;
        int chosen = PSlateList.SelectedIndex < 0
            ? (e.Key == Key.Down ? count - 1 : 0)
            : PSlateList.SelectedIndex;
        PSlateList.SelectedIndex = (chosen + step) % count;
        PSlateList.ScrollIntoView(PSlateList.SelectedItem);
        e.Handled = true;
    }

    private void PSlateCloseRefine(object? sender, EventArgs e)
    {
        PSlateList.SelectedIndex = -1;
    }

    private void PSlateKeyObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PLabelCaret row }
            || PCardLabelFind(row) is not PCard card)
        {
            return;
        }

        if (PSlateList.SelectedItem is not PSlateItem item)
        {
            return;
        }

        _qEditor.QEditorArea.CEditorCard.CCardTagInsert(card.PCardId, item.PSlateItemId, card.PCardLabelPosition);
        e.Handled = true;
        PSlateShutRefine();
        card.PCardLabelClear();
    }

    private void PSlateRefine(PCard card, CSlate slate)
    {
        card.PCardLabelRefine(slate.CSlateText);
        if (slate.CSlateShown)
        {
            PSlateOpenRefine(card, slate.CSlateRows);
            return;
        }

        PSlateShutRefine();
    }

    private void PSlateOpenRefine(PCard card, IReadOnlyList<CSlateRow> rows)
    {
        _pSlateItem.Clear();
        foreach (CSlateRow row in rows)
        {
            _pSlateItem.Add(new PSlateItem(row.CSlateRowId, row.CSlateRowLead, row.CSlateRowMark, row.CSlateRowTail));
        }

        TextBox? box = PSlateBoxFind(card);

        PSlate.PlacementTarget = PSlateFrameFind(box) ?? box ?? (UIElement)PContents;
        PSlateSheet.SetBinding(
            FrameworkElement.MinWidthProperty,
            new Binding(nameof(FrameworkElement.ActualWidth)) { Source = PSlate.PlacementTarget });
        PSlate.IsOpen = true;
        PSlateList.SelectedIndex = -1;
    }

    private void PSlateShutRefine()
    {
        PSlate.IsOpen = false;
        PSlateList.SelectedIndex = -1;
        _pSlateItem.Clear();
    }

    private TextBox? PSlateBoxFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PLabelCaret row } box &&
            PCardLabelFind(row) == card
            ? box
            : null;
    }

    private static CustomPopupPlacement[] PSlatePlace(Size popup, Size target, Point offset)
    {
        var pBelow = new Point(-PSlateShade, target.Height + PSlateGap - PSlateShade);
        var pAbove = new Point(-PSlateShade, PSlateShade - PSlateGap - popup.Height);
        return
        [
            new CustomPopupPlacement(pBelow, PopupPrimaryAxis.Vertical),
            new CustomPopupPlacement(pAbove, PopupPrimaryAxis.Vertical),
        ];
    }

    private static FrameworkElement? PSlateFrameFind(TextBox? box)
    {
        if (box is null)
        {
            return null;
        }

        box.ApplyTemplate();
        return box.Template?.FindName(QField.QFieldSurfaceName, box) as FrameworkElement;
    }
}
