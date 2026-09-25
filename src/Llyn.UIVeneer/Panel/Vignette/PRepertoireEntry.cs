using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

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
        _lRepertoire.LRepertoireOccurrenceSelect(PSender.PSenderSourceRead<POccurrenceItem>(e)?.POccurrenceItemId);
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
