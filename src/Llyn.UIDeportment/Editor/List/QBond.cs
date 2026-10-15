using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QBond
{
    private CCardField _cCardField = null!;

    internal void QBondIntroduce(CCardField field)
    {
        _cCardField = field;
    }

    internal void QBondApply(FrameworkElement container, object item, string? _)
    {
        if (item is QPlacard card)
        {
            if (QLook.QLookPartFind<ItemsControl>(container, "PCardBond") is not ItemsControl bond)
            {
                return;
            }

            QLookItem.QLookItemAttach(bond, QBondApply);
            bond.ItemsSource = card.QPlacardBond;
            if (QLook.QLookPartFind<Button>(bond, "PBondChooser") is Button chooser)
            {
                chooser.Click -= QBondChooserObserve;
                chooser.Click += QBondChooserObserve;
                chooser.PreviewKeyDown -= QBondKeyObserve;
                chooser.PreviewKeyDown += QBondKeyObserve;
            }

            if (QLook.QLookPartFind<ItemsControl>(bond, "PBondList") is ItemsControl list)
            {
                QLookItem.QLookItemAttach(list, QBondRowRefine);
                list.MouseLeftButtonUp -= QBondMenuObserve;
                list.MouseLeftButtonUp += QBondMenuObserve;
            }

            return;
        }

        if (item is not QBondItem chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PBondNumber") is TextBlock number)
        {
            number.Text = chip.QBondItemPath;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PBondText") is TextBlock text)
        {
            text.Text = chip.QBondItemName;
        }

        if (QLook.QLookPartFind<Button>(container, "PBondEraser") is Button eraser)
        {
            eraser.Click -= QBondEraserObserve;
            eraser.Click += QBondEraserObserve;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PBondIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("unlink", 12);
            }
        }
    }

    internal static void QBondRefine(QPlacard card, IReadOnlyList<CMeaning> senses)
    {
        QLookItem.QLookItemShow(
            card.QPlacardBond,
            QBondItem.QBondItemCreate(card.QPlacardId, senses),
            static row => row.QBondItemSense,
            static bond => bond.QBondItemSense,
            static bond => bond,
            static (row, bond) =>
                row.QBondItemPath == bond.QBondItemPath && row.QBondItemName == bond.QBondItemName ? row : bond);
    }

    private static void QBondMenuRefine(ItemsControl bond, IReadOnlyList<QBondItem> rows)
    {
        if (QLook.QLookPartFind<Popup>(bond, "PBondMenu") is not Popup menu
            || QLook.QLookPartFind<ItemsControl>(bond, "PBondList") is not ItemsControl list)
        {
            return;
        }

        list.ItemsSource = rows;
        menu.PlacementTarget = QLook.QLookPartFind<Button>(bond, "PBondChooser");
        menu.IsOpen = rows.Count > 0;
    }

    private static void QBondRowRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QBondItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PBondNumber") is TextBlock number)
        {
            number.Text = row.QBondItemPath;
            number.Margin = row.QBondItemIndent;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PBondText") is TextBlock text)
        {
            text.Text = row.QBondItemName;
        }
    }

    private void QBondChooserObserve(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: QPlacard card, TemplatedParent: ItemsControl bond })
        {
            QBondMenuRefine(bond, QBondItem.QBondItemCreate(card.QPlacardId, _cCardField.CCardSenseRead()));
        }
    }

    private void QBondMenuObserve(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ItemsControl { TemplatedParent: ItemsControl bond }
            || e.OriginalSource is not FrameworkElement { DataContext: QBondItem row })
        {
            return;
        }

        QBondMenuRefine(bond, []);
        _cCardField.CCardSenseAttach(row.QBondItemCard, row.QBondItemSense);
        e.Handled = true;
    }

    private void QBondKeyObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape
            || sender is not Button { TemplatedParent: ItemsControl bond }
            || QLook.QLookPartFind<Popup>(bond, "PBondMenu") is not { IsOpen: true })
        {
            return;
        }

        QBondMenuRefine(bond, []);
        e.Handled = true;
    }

    private void QBondEraserObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QBondItem chip })
        {
            _cCardField.CCardSenseDetach(chip.QBondItemCard, chip.QBondItemSense);
        }
    }
}
