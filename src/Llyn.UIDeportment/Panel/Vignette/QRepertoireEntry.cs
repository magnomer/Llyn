using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private readonly ObservableCollection<QOccurrenceItem> _qOccurrenceList = [];

    private void QOccurrenceFind()
    {
        IReadOnlyList<CVistaRow> read;
        try
        {
            read = _lRepertoire.LRepertoireOccurrence.LOccurrenceRowsRead();
        }
        catch (Exception exception)
        {
            _qRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        List<QOccurrenceItem> fresh = [];
        foreach (CVistaRow entry in read)
        {
            fresh.Add(new QOccurrenceItem(entry, entry.CVistaRowChosen));
        }

        LSplice.LSpliceApply(
            _qOccurrenceList, fresh, QOccurrenceItem.QOccurrenceItemMatch, QOccurrenceItem.QOccurrenceItemSync);

        QOccurrenceEmpty.SetResourceReference(
            TextBlock.TextProperty, _lRepertoire.LRepertoireOccurrence.LOccurrenceEmptyRead(QSortie.Text));
        QOccurrenceEmpty.Visibility = QLook.QLookVisibleRead(_qOccurrenceList.Count == 0);
    }

    private void QOccurrenceHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireOccurrenceSelect(QSender.QSenderSourceRead<QOccurrenceItem>(e)?.QOccurrenceItemId);
    }

    private void QOccurrenceApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QOccurrenceItem occurrence)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "POccurrenceRow") is Button row)
        {
            if (occurrence.QOccurrenceItemChosen)
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
            flag.Source = occurrence.QOccurrenceItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "POccurrenceName") is Run name)
        {
            name.Text = occurrence.QOccurrenceItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "POccurrenceEpithet") is Run epithet)
        {
            epithet.Text = " " + occurrence.QOccurrenceItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "POccurrenceLanguage") is TextBlock language)
        {
            language.Text = occurrence.QOccurrenceItemLanguage;
        }
    }

    private void QRepertoireStoreHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireSession.CSessionSave();
    }

    private void QRepertoireFreshHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireFreshStart();
    }
}
