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

    private readonly ObservableCollection<QSlateItem> _qSlateItem = [];

    private CCaret _cCaret = null!;

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

    internal void QSlateIntroduce(CCaret caret)
    {
        _cCaret = caret;
    }

    private void QSlateApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QSlateItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Run>(container, "PSlateLead") is Run lead)
        {
            lead.Text = row.QSlateItemLead;
        }

        if (QLook.QLookPartFind<Run>(container, "PSlateMark") is Run mark)
        {
            mark.Text = row.QSlateItemMark;
        }

        if (QLook.QLookPartFind<Run>(container, "PSlateTail") is Run tail)
        {
            tail.Text = row.QSlateItemTail;
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
        if (sender is not FrameworkElement { DataContext: QSlateItem })
        {
            QSlateShutRefine();
        }
    }

    private void QSlatePickObserve(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: QSlateItem item }
            || Keyboard.FocusedElement is not FrameworkElement { DataContext: QInlet<QLabelChip> row }
            || _qSlateLabel.QLabelCardFind(row) is not QPlacard card)
        {
            return;
        }

        _cCaret.CCaretTagInsert(card.QPlacardCollocated, card.QPlacardId, item.QSlateItemId);
        e.Handled = true;
        QSlateShutRefine();
        row.QInletClear();
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
            || sender is not FrameworkElement { DataContext: QInlet<QLabelChip> row }
            || _qSlateLabel.QLabelCardFind(row) is not QPlacard card)
        {
            return;
        }

        if (QSlateList.SelectedItem is not QSlateItem item)
        {
            return;
        }

        _cCaret.CCaretTagInsert(card.QPlacardCollocated, card.QPlacardId, item.QSlateItemId);
        e.Handled = true;
        QSlateShutRefine();
        row.QInletClear();
    }

    internal void QSlateRefine(QPlacard card, CSlate slate)
    {
        card.QPlacardLabel.QInletRefine(slate.CSlateText);
        if (slate.CSlateShown)
        {
            QSlateOpenRefine(card, slate.CSlateRows);
            return;
        }

        QSlateShutRefine();
    }

    private void QSlateOpenRefine(QPlacard card, IReadOnlyList<CSlateRow> rows)
    {
        _qSlateItem.Clear();
        foreach (CSlateRow row in rows)
        {
            _qSlateItem.Add(new QSlateItem(row.CSlateRowId, row.CSlateRowLead, row.CSlateRowMark, row.CSlateRowTail));
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

    private TextBox? QSlateBoxFind(QPlacard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: QInlet<QLabelChip> row } box &&
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
