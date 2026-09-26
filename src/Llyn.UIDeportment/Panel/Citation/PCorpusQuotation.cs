using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Core;


namespace Llyn.UIDeportment;

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

        LSplice.LSpliceApply(
            _pQuotationList, fresh, PQuotationItem.PQuotationItemMatch, PQuotationItem.PQuotationItemSync);

        PQuotationEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PDredge.Text) ? "Example.Vacant" : "Example.Unmatched");
        PQuotationEmpty.Visibility = _pQuotationList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PQuotationHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusQuotationSelect(QSender.QSenderSourceRead<PQuotationItem>(e)?.PQuotationItemId);
    }

    private void PQuotationApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PQuotationItem quotation)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PQuotationRow") is Button row)
        {
            if (quotation.PQuotationItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= PQuotationHandle;
            row.Click += PQuotationHandle;
        }

        if (QLook.QLookPartFind<Image>(container, "PQuotationFlag") is Image flag)
        {
            flag.Source = quotation.PQuotationItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PQuotationName") is Run name)
        {
            name.Text = quotation.PQuotationItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PQuotationEpithet") is Run epithet)
        {
            epithet.Text = " " + quotation.PQuotationItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PQuotationLanguage") is TextBlock language)
        {
            language.Text = quotation.PQuotationItemLanguage;
        }
    }
}
