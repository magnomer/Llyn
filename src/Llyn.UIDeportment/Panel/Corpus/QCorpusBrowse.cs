using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private readonly ObservableCollection<QAnthologyItem> _qAnthologyList = [];

    private readonly ObservableCollection<PLanguageItem> _qLanguageItem = [];

    private async void QCorpusWorkspaceRefine()
    {
        QSpeakerRefine(
            await _qCorpusHost.QWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad(QEnsignImage.QEnsignDraw));
    }

    private void QQueryObserve(object sender, TextChangedEventArgs e)
    {
        _cCorpus.CCorpusAnthology.CAnthologyQuerySet(QQuery.Text ?? string.Empty);
    }

    private void QRankObserve(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusAnthology.CAnthologyOrderSet(QChoice.QChoiceOrderRead(sender));
        QRankDropperRefine();
    }

    private void QRankDropperRefine()
    {
        QRankDropper.IsChecked = false;
    }

    internal async void QCorpusVistaRefine()
    {
        QChoice.QChoiceOrderApply(QRankDropdown, _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelOrder);
        QGauzeRefine();
        CEnsignSheet<IReadOnlyList<CCatalogExample>> sheet =
            await _cCorpus.CCorpusRowsLoad(QEnsignImage.QEnsignDraw);
        QGauzeBuild(sheet.CEnsignSheetLanguages);
        QSpeakerRefine(sheet.CEnsignSheetLanguages);
        QAnthologyRefine(sheet.CEnsignSheetRows);
    }

    internal async void QQuotationVistaRefine()
    {
        QQuotationRefine(
            (await _cCorpus.CCorpusQuotation.CQuotationRowsLoad(QEnsignImage.QEnsignDraw)).CEnsignSheetRows);
    }

    private void QGauzeRefine()
    {
        QGauzeMark.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusAnthology.CAnthologyFiltered);
    }

    private void QGauzeBuild(IReadOnlyList<string> languages)
    {
        QChoice.QChoiceFilterBuild(
            QGauzeList, languages, _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelFilter, QGauzeObserve);
    }

    private void QQueryClear()
    {
        QQuery.Clear();
        QGauzeBuild(_qCorpusHost.QWindowAtelier.CAtelierCatalog.CCatalogLanguageRead());
        QGauzeRefine();
    }

    private void QDredgeObserve(object sender, TextChangedEventArgs e)
    {
        _cCorpus.CCorpusQuotation.CQuotationQuerySet(QDredge.Text);
    }

    private void QGauzeObserve(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusAnthology.CAnthologyFilterSet(QChoice.QChoiceFilterRead(sender));
        QGauzeRefine();
    }

    private void QAnthologyRefine()
    {
        QAnthologyRefine(_cCorpus.CCorpusRowsRead());
    }

    private void QAnthologyRefine(IReadOnlyList<CCatalogExample> rows)
    {
        List<QAnthologyItem> fresh = [];
        foreach (CCatalogExample row in rows)
        {
            fresh.Add(new QAnthologyItem(row, row.CCatalogExampleChosen));
        }

        QSplice.QSpliceRefine(
            _qAnthologyList, fresh, QAnthologyItem.QAnthologyItemMatch, QAnthologyItem.QAnthologyItemSync);

        QAnthologyEmpty.Visibility = QLook.QLookVisibleRead(_qAnthologyList.Count == 0);
    }

    private void QCorpusTallyRefine()
    {
        string tally = _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelTallyRead();
        QExcerptTally.Text = tally;
        QTranscriptTally.Text = tally;
    }

    private void QAnthologyObserve(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusExampleSelect(QSender.QSenderSourceRead<QAnthologyItem>(e)?.QAnthologyItemId);
    }

    private void QAnthologyApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QAnthologyItem anthology)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PAnthologyRow") is Button row)
        {
            if (anthology.QAnthologyItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QAnthologyObserve;
            row.Click += QAnthologyObserve;
        }

        if (QLook.QLookPartFind<Image>(container, "PAnthologyFlag") is Image flag)
        {
            flag.Source = anthology.QAnthologyItemFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAnthologyName") is TextBlock name)
        {
            name.Text = anthology.QAnthologyItemName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAnthologyLanguage") is TextBlock language)
        {
            language.Text = anthology.QAnthologyItemLanguage;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAnthologyCount") is TextBlock count)
        {
            count.Text = anthology.QAnthologyItemCount;
        }
    }

    private void QCorpusBinObserve(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusExampleDelete();
    }

    private void QCorpusStoreObserve(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusSession.CSessionSave();
    }

    private void QCorpusViewerObserve(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusScribeToggle(false);
    }

    private void QCorpusScribeObserve(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusScribeToggle(true);
    }

    internal void QCorpusVoyageShow(bool past, bool future)
    {
        QCorpusEarlier.IsEnabled = past;
        QCorpusLater.IsEnabled = future;
    }

    private void QCorpusRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qCorpusHost.QWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QCorpusAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qCorpusHost.QWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }
}
