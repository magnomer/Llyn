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

internal sealed class QSlate
{
    private const double QSlateShade = 10;
    private const double QSlateGap = 6;

    private readonly FrameworkElement _qSlateSurface;

    private readonly QLabel _qSlateLabel;

    private readonly ObservableCollection<PSlateItem> _qSlateItem = [];

    private CEditor _cEditor = null!;

    internal QSlate(FrameworkElement surface, QLabel label)
    {
        _qSlateSurface = surface;
        _qSlateLabel = label;
        QSlateList.ItemsSource = _qSlateItem;
        QLookItem.QLookItemAttach(QSlateList, QSlateApply);
        QSlatePopup.CustomPopupPlacementCallback = QSlatePlace;
        QSlatePopup.Closed += QSlateCloseRefine;
    }

    private Popup QSlatePopup => QContract.QContractFind<Popup>(_qSlateSurface, "PSlate");

    private Border QSlateSheet => QContract.QContractFind<Border>(_qSlateSurface, "PSlateSheet");

    private ListBox QSlateList => QContract.QContractFind<ListBox>(_qSlateSurface, "PSlateList");

    private Border QSlateContents => QContract.QContractFind<Border>(_qSlateSurface, "PContents");

    internal void QSlateIntroduce(CEditor editor)
    {
        _cEditor = editor;
    }

    private void QSlateApply(FrameworkElement container, object item, string? _)
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
            surface.PreviewMouseLeftButtonDown -= QSlateMissRefine;
            surface.PreviewMouseLeftButtonDown -= QSlatePickObserve;
            surface.PreviewMouseLeftButtonDown += QSlateMissRefine;
            surface.PreviewMouseLeftButtonDown += QSlatePickObserve;
        }
    }

    private void QSlateMissRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSlateItem })
        {
            QSlateShutRefine();
        }
    }

    private void QSlatePickObserve(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSlateItem item }
            || Keyboard.FocusedElement is not FrameworkElement { DataContext: PLabelCaret row }
            || _qSlateLabel.QLabelCardFind(row) is not PCard card)
        {
            return;
        }

        _cEditor.CEditorCard.CCardTagInsert(card.PCardId, item.PSlateItemId, card.PCardLabelPosition);
        e.Handled = true;
        QSlateShutRefine();
        card.PCardLabelClear();
    }

    internal void QSlateKeyRefine(object sender, KeyEventArgs e)
    {
        int? lit = e.Key switch
        {
            Key.Down => CLantern.CLanternMove(QSlateList.SelectedIndex, QSlateList.Items.Count, 1),
            Key.Up => CLantern.CLanternMove(QSlateList.SelectedIndex, QSlateList.Items.Count, -1),
            _ => null,
        };
        if (lit is int chosen)
        {
            QSlateList.SelectedIndex = chosen;
            QSlateList.ScrollIntoView(QSlateList.SelectedItem);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape && QSlatePopup.IsOpen)
        {
            QSlateShutRefine();
            e.Handled = true;
        }
    }

    private void QSlateCloseRefine(object? sender, EventArgs e)
    {
        QSlateList.SelectedIndex = -1;
        _qSlateItem.Clear();
    }

    internal void QSlateKeyObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PLabelCaret row }
            || _qSlateLabel.QLabelCardFind(row) is not PCard card)
        {
            return;
        }

        if (QSlateList.SelectedItem is not PSlateItem item)
        {
            return;
        }

        _cEditor.CEditorCard.CCardTagInsert(card.PCardId, item.PSlateItemId, card.PCardLabelPosition);
        e.Handled = true;
        QSlateShutRefine();
        card.PCardLabelClear();
    }

    internal void QSlateRefine(PCard card, CSlate slate)
    {
        card.PCardLabelRefine(slate.CSlateText);
        if (slate.CSlateShown)
        {
            QSlateOpenRefine(card, slate.CSlateRows);
            return;
        }

        QSlateShutRefine();
    }

    private void QSlateOpenRefine(PCard card, IReadOnlyList<CSlateRow> rows)
    {
        _qSlateItem.Clear();
        foreach (CSlateRow row in rows)
        {
            _qSlateItem.Add(new PSlateItem(row.CSlateRowId, row.CSlateRowLead, row.CSlateRowMark, row.CSlateRowTail));
        }

        TextBox? box = QSlateBoxFind(card);

        QSlatePopup.PlacementTarget = QSlateFrameFind(box) ?? box ?? (UIElement)QSlateContents;
        QSlateSheet.SetBinding(
            FrameworkElement.MinWidthProperty,
            new Binding(nameof(FrameworkElement.ActualWidth)) { Source = QSlatePopup.PlacementTarget });
        QSlatePopup.IsOpen = true;
        QSlateList.SelectedIndex = -1;
    }

    internal void QSlateShutRefine()
    {
        QSlatePopup.IsOpen = false;
        QSlateList.SelectedIndex = -1;
        _qSlateItem.Clear();
    }

    private TextBox? QSlateBoxFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PLabelCaret row } box &&
            _qSlateLabel.QLabelCardFind(row) == card
            ? box
            : null;
    }

    private static CustomPopupPlacement[] QSlatePlace(Size popup, Size target, Point offset)
    {
        var pBelow = new Point(-QSlateShade, target.Height + QSlateGap - QSlateShade);
        var pAbove = new Point(-QSlateShade, QSlateShade - QSlateGap - popup.Height);
        return
        [
            new CustomPopupPlacement(pBelow, PopupPrimaryAxis.Vertical),
            new CustomPopupPlacement(pAbove, PopupPrimaryAxis.Vertical),
        ];
    }

    private static FrameworkElement? QSlateFrameFind(TextBox? box)
    {
        if (box is null)
        {
            return null;
        }

        box.ApplyTemplate();
        return box.Template?.FindName(QField.QFieldSurfaceName, box) as FrameworkElement;
    }
}
