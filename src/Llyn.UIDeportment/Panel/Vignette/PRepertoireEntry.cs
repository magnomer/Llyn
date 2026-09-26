using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Core;


namespace Llyn.UIDeportment;

public partial class PRepertoire
{
    private readonly ObservableCollection<POccurrenceItem> _pOccurrenceList = [];

    private void POccurrenceFind()
    {
        IReadOnlyList<LVistaRow> read;
        try
        {
            read = _lRepertoire.LRepertoireOccurrence.LOccurrenceRowsRead();
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
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
            _pOccurrenceList, fresh, POccurrenceItem.POccurrenceItemMatch, POccurrenceItem.POccurrenceItemSync);

        POccurrenceEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PSortie.Text) ? "Situation.Vacant" : "Situation.Unmatched");
        POccurrenceEmpty.Visibility = _pOccurrenceList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void POccurrenceHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireOccurrenceSelect(QSender.QSenderSourceRead<POccurrenceItem>(e)?.POccurrenceItemId);
    }

    private void POccurrenceApply(FrameworkElement container, object item, string? _)
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

            row.Click -= POccurrenceHandle;
            row.Click += POccurrenceHandle;
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
            epithet.Text = " " + occurrence.POccurrenceItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "POccurrenceLanguage") is TextBlock language)
        {
            language.Text = occurrence.POccurrenceItemLanguage;
        }
    }

    private void PRepertoireStoreHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireSave();
    }

    private void PRepertoireFreshHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireFreshStart();
    }
}
