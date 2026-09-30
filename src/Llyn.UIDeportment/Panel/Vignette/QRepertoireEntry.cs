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

    private void QOccurrenceRefine()
    {
        QOccurrenceRefine(_cRepertoire.CRepertoireOccurrence.COccurrenceRowsRead());
    }

    private void QOccurrenceRefine(IReadOnlyList<CVistaRow> rows)
    {
        List<QOccurrenceItem> fresh = [];
        foreach (CVistaRow entry in rows)
        {
            fresh.Add(new QOccurrenceItem(entry, entry.CVistaRowChosen));
        }

        QSplice.QSpliceRefine(
            _qOccurrenceList, fresh, QOccurrenceItem.QOccurrenceItemMatch, QOccurrenceItem.QOccurrenceItemSync);

        QOccurrenceEmpty.SetResourceReference(
            TextBlock.TextProperty, _cRepertoire.CRepertoireOccurrence.COccurrenceEmptyKey);
        QOccurrenceEmpty.Visibility = QLook.QLookVisibleRead(_qOccurrenceList.Count == 0);
    }

    private void QOccurrenceObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireOccurrenceSelect(QSender.QSenderSourceRead<QOccurrenceItem>(e)?.QOccurrenceItemId);
    }

    private void QOccurrenceItemRefine(FrameworkElement container, object item, string? _)
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

            row.Click -= QOccurrenceObserve;
            row.Click += QOccurrenceObserve;
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

    private void QRepertoireStoreObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireSession.CSessionSave();
    }

    private void QRepertoireFreshObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireSituationCreate();
    }
}
