using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private readonly ObservableCollection<QQuotationItem> _qQuotationList = [];

    private void QQuotationFind()
    {
        IReadOnlyList<CVistaRow> read;
        try
        {
            read = _cCorpus.CCorpusQuotation.CQuotationRowsRead();
        }
        catch (Exception exception)
        {
            _qCorpusHost.PWindowFailureRefine("Example.LoadFailed", exception);
            return;
        }

        List<QQuotationItem> fresh = [];
        foreach (CVistaRow entry in read)
        {
            fresh.Add(new QQuotationItem(entry, entry.CVistaRowChosen));
        }

        LSplice.LSpliceApply(
            _qQuotationList, fresh, QQuotationItem.QQuotationItemMatch, QQuotationItem.QQuotationItemSync);

        QQuotationEmpty.SetResourceReference(TextBlock.TextProperty, _cCorpus.CCorpusQuotation.CQuotationEmptyKey);
        QQuotationEmpty.Visibility = QLook.QLookVisibleRead(_qQuotationList.Count == 0);
    }

    private void QQuotationHandle(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusQuotationSelect(QSender.QSenderSourceRead<QQuotationItem>(e)?.QQuotationItemId);
    }

    private void QQuotationApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QQuotationItem quotation)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PQuotationRow") is Button row)
        {
            if (quotation.QQuotationItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QQuotationHandle;
            row.Click += QQuotationHandle;
        }

        if (QLook.QLookPartFind<Image>(container, "PQuotationFlag") is Image flag)
        {
            flag.Source = quotation.QQuotationItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PQuotationName") is Run name)
        {
            name.Text = quotation.QQuotationItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PQuotationEpithet") is Run epithet)
        {
            epithet.Text = " " + quotation.QQuotationItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PQuotationLanguage") is TextBlock language)
        {
            language.Text = quotation.QQuotationItemLanguage;
        }
    }
}
