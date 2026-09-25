using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PCorpus
{
    private readonly ObservableCollection<PQuotationItem> _pQuotationList = [];

    private void PQuotationFind()
    {
        IReadOnlyList<LVistaRow> read;
        try
        {
            read = _lCorpus.LCorpusQuotation.LQuotationRowsRead();
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        List<PQuotationItem> fresh = [];
        foreach (LVistaRow entry in read)
        {
            fresh.Add(new PQuotationItem(
                entry.LVistaRowId,
                entry.LVistaRowHeadword,
                entry.LVistaRowLanguage,
                entry.LVistaRowEpithet ?? string.Empty,
                entry.LVistaRowChosen)
            {
                PQuotationItemName = entry.LVistaRowName,
            });
        }

        PSplice.PSpliceApply(
            _pQuotationList, fresh, PQuotationItem.PQuotationItemMatch, PQuotationItem.PQuotationItemSync);

        PQuotationEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PDredge.Text) ? "Example.Vacant" : "Example.Unmatched");
        PQuotationEmpty.Visibility = _pQuotationList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PQuotationHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusQuotationSelect(PSender.PSenderSourceRead<PQuotationItem>(e)?.PQuotationItemId);
    }
}
