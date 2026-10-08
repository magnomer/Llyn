using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QQuotation
{
    private readonly UserControl _qQuotationScope;

    private readonly ObservableCollection<QQuotationItem> _qQuotationList = [];

    private CCorpus _cCorpus = null!;

    internal QQuotation(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qQuotationScope = scope;

        QDredge.SetResourceReference(QField.QFieldHintProperty, "Dredge.Search");
        QDredge.TextChanged += QDredgeObserve;
    }

    private TextBox QDredge => QContract.QContractFind<TextBox>(_qQuotationScope, "PDredge");

    private ItemsControl QQuotationView => QContract.QContractFind<ItemsControl>(_qQuotationScope, "PQuotation");

    private TextBlock QQuotationEmpty => QContract.QContractFind<TextBlock>(_qQuotationScope, "PQuotationEmpty");

    internal void QQuotationIntroduce(CCorpus corpus, CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(corpus);
        ArgumentNullException.ThrowIfNull(atelier);

        _cCorpus = corpus;
        _cCorpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureRowsChanged += QQuotationRefine;
        atelier.CAtelierWorkspace.CWorkspaceOpened += QQuotationVistaRefine;

        QQuotationView.ItemsSource = _qQuotationList;
        QLookItem.QLookItemAttach(QQuotationView, QQuotationApply);
    }

    private async void QQuotationVistaRefine()
    {
        QQuotationRefine(
            (await _cCorpus.CCorpusQuotation.CQuotationRowsLoad(QEnsignImage.QEnsignDraw)).CEnsignSheetRows);
    }

    private void QDredgeObserve(object sender, TextChangedEventArgs e)
    {
        _cCorpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureQuerySet(QDredge.Text);
    }

    private void QQuotationRefine()
    {
        QQuotationRefine(_cCorpus.CCorpusQuotation.CQuotationRowsRead());
    }

    private void QQuotationRefine(IReadOnlyList<CVistaRow> rows)
    {
        List<QQuotationItem> fresh = [];
        foreach (CVistaRow entry in rows)
        {
            fresh.Add(new QQuotationItem(entry, entry.CVistaRowChosen));
        }

        QSplice.QSpliceRefine(
            _qQuotationList, fresh, QQuotationItem.QQuotationItemMatch, QQuotationItem.QQuotationItemSync);

        QQuotationEmpty.SetResourceReference(
            TextBlock.TextProperty, _cCorpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureKey);
        QQuotationEmpty.Visibility = QLook.QLookVisibleRead(_qQuotationList.Count == 0);
    }

    private void QQuotationObserve(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusDiptych.CDiptychChildSelect(
            QSender.QSenderSourceRead<QQuotationItem>(e)?.QQuotationItemId);
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

            row.Click -= QQuotationObserve;
            row.Click += QQuotationObserve;
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
            epithet.Text = QLook.QLookEpithetRead(quotation.QQuotationItemEpithet);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PQuotationLanguage") is TextBlock language)
        {
            language.Text = quotation.QQuotationItemLanguage;
        }
    }
}
