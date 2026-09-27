using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private readonly ObservableCollection<POccurrenceItem> _qOccurrenceList = [];

    private void QOccurrenceFind()
    {
        IReadOnlyList<LVistaRow> read;
        try
        {
            read = _lRepertoire.LRepertoireOccurrence.LOccurrenceRowsRead();
        }
        catch (Exception exception)
        {
            _qRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        List<POccurrenceItem> fresh = [];
        foreach (LVistaRow entry in read)
        {
            fresh.Add(new POccurrenceItem(
                entry.LVistaRowId,
                entry.LVistaRowHeadword,
                entry.LVistaRowLanguage,
                entry.LVistaRowEpithet ?? string.Empty,
                entry.LVistaRowChosen)
            {
                POccurrenceItemName = entry.LVistaRowName,
            });
        }

        LSplice.LSpliceApply(
            _qOccurrenceList, fresh, POccurrenceItem.POccurrenceItemMatch, POccurrenceItem.POccurrenceItemSync);

        QOccurrenceEmpty.SetResourceReference(
            TextBlock.TextProperty, _lRepertoire.LRepertoireOccurrence.LOccurrenceEmptyRead(QSortie.Text));
        QOccurrenceEmpty.Visibility = QLook.QLookVisibleRead(_qOccurrenceList.Count == 0);
    }

    private void QOccurrenceHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireOccurrenceSelect(QSender.QSenderSourceRead<POccurrenceItem>(e)?.POccurrenceItemId);
    }

    private void QOccurrenceApply(FrameworkElement container, object item, string? _)
    {
        if (item is not POccurrenceItem occurrence)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "POccurrenceRow") is Button row)
        {
            if (occurrence.POccurrenceItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QOccurrenceHandle;
            row.Click += QOccurrenceHandle;
        }

        if (QLook.QLookPartFind<Image>(container, "POccurrenceFlag") is Image flag)
        {
            flag.Source = occurrence.POccurrenceItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "POccurrenceName") is Run name)
        {
            name.Text = occurrence.POccurrenceItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "POccurrenceEpithet") is Run epithet)
        {
            epithet.Text = " " + occurrence.POccurrenceItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "POccurrenceLanguage") is TextBlock language)
        {
            language.Text = occurrence.POccurrenceItemLanguage;
        }
    }

    private void QRepertoireStoreHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireSave();
    }

    private void QRepertoireFreshHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireFreshStart();
    }
}
