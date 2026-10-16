using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QJotter
{
    private readonly UserControl _qJotterSurface;

    private readonly ListBox _qJotterListing;

    private readonly ObservableCollection<QJotterItem> _qJotterList = [];

    private CCue _cCue = null!;

    internal QJotter(UserControl surface, ListBox listing)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(listing);

        _qJotterSurface = surface;
        _qJotterListing = listing;
        _qJotterListing.SelectionChanged += QJotterChoiceObserve;
        QJotterSlots.ItemsSource = _qJotterList;
        QLookItem.QLookItemAttach(QJotterSlots, QJotterItemRefine);
    }

    private ItemsControl QJotterSlots => QContract.QContractFind<ItemsControl>(_qJotterSurface, "PJotterSlots");

    internal void QJotterIntroduce(CCue cue)
    {
        ArgumentNullException.ThrowIfNull(cue);

        _cCue = cue;
        _cCue.CCueChanged += QJotterRefine;
    }

    internal void QJotterRefine()
    {
        List<long> cues = [];
        foreach (object chosen in _qJotterListing.SelectedItems)
        {
            if (chosen is QCueItem cue)
            {
                cues.Add(cue.QCueItemId);
            }
        }

        IReadOnlyList<CCueCell>? cells = _cCue.CCueCellRead(cues);
        _qJotterSurface.Visibility = QLook.QLookVisibleRead(cells is not null);
        QSplice.QSpliceRefine(
            _qJotterList,
            [.. (cells ?? []).Select(cell => new QJotterItem(cues[0], cell))],
            QJotterItem.QJotterItemMatch,
            QJotterItem.QJotterItemSync);
    }

    private void QJotterItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QJotterItem row)
        {
            return;
        }

        CCueCell cell = row.QJotterItemCell;
        QLook.QLookPartFind<TextBlock>(container, "PJotterCaption")?
            .SetResourceReference(TextBlock.TextProperty, cell.CCueCellCaption);
        if (QLook.QLookPartFind<TextBlock>(container, "PJotterNumber") is TextBlock number)
        {
            number.Text = cell.CCueCellNumber;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PJotterBody") is TextBox body)
        {
            body.TextChanged -= QJotterBodyObserve;
            QField.QFieldTextShow(body, cell.CCueCellValue);
            body.TextChanged += QJotterBodyObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PJotterPicker") is Button picker)
        {
            picker.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("lookup", 24));
            picker.Visibility = QLook.QLookVisibleRead(cell.CCueCellFile);
            picker.Click -= QJotterPickerObserve;
            picker.Click += QJotterPickerObserve;
        }
    }

    private void QJotterChoiceObserve(object sender, SelectionChangedEventArgs e)
    {
        QJotterRefine();
    }

    private void QJotterBodyObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: QJotterItem row } body)
        {
            _cCue.CCueCellSet(
                row.QJotterItemCue,
                row.QJotterItemCell.CCueCellColumn,
                row.QJotterItemCell.CCueCellIndex,
                body.Text);
        }
    }

    private void QJotterPickerObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QJotterItem>(sender) is QJotterItem row)
        {
            _cCue.CCueCellOpen(
                row.QJotterItemCue, row.QJotterItemCell.CCueCellColumn, row.QJotterItemCell.CCueCellIndex);
        }
    }
}
