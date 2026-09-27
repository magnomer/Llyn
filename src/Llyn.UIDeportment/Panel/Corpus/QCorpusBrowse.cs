using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private readonly ObservableCollection<QAnthologyItem> _qAnthologyList = [];

    private readonly ObservableCollection<QCitationItem> _qCitationCatalog = [];

    private readonly ObservableCollection<PLanguageItem> _qLanguageItem = [];

    private IReadOnlyDictionary<long, int> _qAnthologyCount = new Dictionary<long, int>();

    private async void QCorpusWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_qCorpusHost.PWindowAtelier);
        QSpeakerLoad();
        QCorpusReset();
    }

    private void QQueryHandle(object sender, TextChangedEventArgs e)
    {
        _lCorpus.LCorpusAnthology.LAnthologyQuerySet(QQuery.Text ?? string.Empty);
    }

    private void QRankHandle(object sender, RoutedEventArgs e)
    {
        QRankDropper.IsChecked = false;
        _lCorpus.LCorpusAnthology.LAnthologyRankSet(QChoice.QChoiceOrderRead(sender));
    }

    internal async void QCorpusVistaRestore()
    {
        LPanel anthology = _lCorpus.LCorpusAnthology.LAnthologyPanel;
        QCorpusObserverAttach();
        QCorpusDisplay.PDisplayObserverAttach();
        QCorpusEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            QRankList,
            "Rank",
            QRankHandle,
            [
                CCatalogOrder.CCatalogOrderText,
                CCatalogOrder.CCatalogOrderLanguage,
                CCatalogOrder.CCatalogOrderSource,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(QRankDropdown, anthology.LPanelOrder);
        QGauzeRestore();

        await LEnsignImage.LEnsignLoad(_qCorpusHost.PWindowAtelier);

        QGauzeBuild();
        _lCorpus.LCorpusAnthology.LAnthologyQuerySet(QQuery.Text ?? string.Empty);
        _lCorpus.LCorpusQuotation.LQuotationDredgeSet(QDredge.Text);
        QSpeakerLoad();
        QCitationFind();
        anthology.LPanelRowsUpdate();
    }

    private void QCorpusObserverAttach()
    {
        LPanel anthology = _lCorpus.LCorpusAnthology.LAnthologyPanel;
        LPanel quotation = _lCorpus.LCorpusQuotation.LQuotationPanel;
        UserControl surface = _qCorpusSurface;
        Action<CBulletin> rows = LObserver.LObserverCreate<CBulletin>(surface, anthology.LPanelRowsUpdate);
        Action<CBulletin> citation = LObserver.LObserverCreate<CBulletin>(surface, QCitationFind);
        anthology.LPanelObserverAttach(CSubject.CSubjectVista, rows);
        anthology.LPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, QCorpusWorkspaceUpdate));
        anthology.LPanelObserverAttach(CSubject.CSubjectExample, citation);
        anthology.LPanelObserverAttach(CSubject.CSubjectExample, rows);
        anthology.LPanelObserverAttach(CSubject.CSubjectReference, citation);
        anthology.LPanelObserverAttach(CSubject.CSubjectReference, rows);
        anthology.LPanelObserverAttach(CSubject.CSubjectReflex, rows);
        anthology.LPanelObserverAttach(CSubject.CSubjectSettings, rows);
        quotation.LPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, quotation.LPanelEntrySelect));
        quotation.LPanelObserverAttach(CSubject.CSubjectEntry, citation);
        quotation.LPanelObserverAttach(CSubject.CSubjectEntry, rows);
        quotation.LPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, _lCorpus.LCorpusEntryUpdate));
        quotation.LPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, quotation.LPanelRowsUpdate));
    }

    private void QGauzeRestore()
    {
        QGauzeMark.Visibility = QLook.QLookVisibleRead(_lCorpus.LCorpusAnthology.LAnthologyFiltered);
    }

    private void QGauzeBuild()
    {
        QChoice.QChoiceFilterBuild(
            QGauzeList,
            _qCorpusHost.PWindowAtelier.CAtelierCatalog.CCatalogLanguageRead(),
            _lCorpus.LCorpusAnthology.LAnthologyPanel.LPanelFilter,
            QGauzeHandle);
    }

    private void QQueryClear()
    {
        QQuery.Clear();
        QGauzeBuild();
        QGauzeRestore();
    }

    private void QDredgeHandle(object sender, TextChangedEventArgs e)
    {
        _lCorpus.LCorpusQuotation.LQuotationDredgeSet(QDredge.Text);
    }

    private void QGauzeHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusAnthology.LAnthologyGauzeSet(QChoice.QChoiceFilterRead(sender));
        QGauzeRestore();
    }

    private void QAnthologyFind()
    {
        IReadOnlyList<CCatalogExample> read;
        try
        {
            read = _lCorpus.LCorpusAnthology.LAnthologyRowsRead(
                QLocalizationCatalog.QLocalizationTextRead("Display.Unknown"),
                QLocalizationCatalog.QLocalizationTextRead("Example.Unwritten"));
            _qAnthologyCount = _lCorpus.LCorpusAnthology.LAnthologyUsageRead();
        }
        catch (Exception exception)
        {
            _qCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        List<QAnthologyItem> fresh = [];
        foreach (CCatalogExample row in read)
        {
            fresh.Add(new QAnthologyItem(row, row.CCatalogExampleChosen));
        }

        LSplice.LSpliceApply(
            _qAnthologyList, fresh, QAnthologyItem.QAnthologyItemMatch, QAnthologyItem.QAnthologyItemSync);

        QAnthologyEmpty.Visibility = QLook.QLookVisibleRead(_qAnthologyList.Count == 0);

        QExcerptTally.Text = QCorpusTallyRead(_lCorpus.LCorpusAnthology.LAnthologyChosen);
        QTranscriptTally.Text = QCorpusTallyRead(QTranscriptDesk.CDeskStoredRead());
        _lCorpus.LCorpusRowsApply(read);
    }

    private void QAnthologyHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusSelect(
            QSender.QSenderSourceRead<QAnthologyItem>(e)?.QAnthologyItemId, _qCorpusHost.PVoyageRecord);
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

            row.Click -= QAnthologyHandle;
            row.Click += QAnthologyHandle;
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

    internal void QAnthologyExampleShow(long id)
    {
        _lCorpus.LCorpusExampleShow(id);
    }

    private void QCorpusBinHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusDelete();
    }

    private string QCorpusTallyRead(long? id)
    {
        int count = id is long stored && _qAnthologyCount.TryGetValue(stored, out int usage) ? usage : 0;

        return count switch
        {
            0 => QLocalizationCatalog.QLocalizationTextRead("Example.UsageNone"),
            1 => QLocalizationCatalog.QLocalizationTextRead("Example.UsageOne"),
            _ => string.Concat(
                count.ToString(CultureInfo.CurrentCulture),
                " ",
                QLocalizationCatalog.QLocalizationTextRead("Example.UsageMany")),
        };
    }

    private void QCorpusStoreHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusSession.CSessionSave();
    }

    private void QCorpusViewerHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusScribeSet(false);
    }

    private void QCorpusScribeHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusScribeSet(true);
    }

    internal void QCorpusScribeRestore(bool editing)
    {
        _lCorpus.LCorpusAnthology.LAnthologyPanel.LPanelScribeRestore(editing);
    }

    internal bool QCorpusLeaveConfirm()
    {
        return _lCorpus.LCorpusLeaveConfirm();
    }

    internal long QCorpusVoyageRead()
    {
        return _lCorpus.LCorpusAnthology.LAnthologyPanel.LPanelVoyageRead();
    }

    internal void QCorpusVoyageShow(bool past, bool future)
    {
        QCorpusEarlier.IsEnabled = past;
        QCorpusLater.IsEnabled = future;
    }

    private void QCorpusRetreatHandle(object sender, RoutedEventArgs e)
    {
        _qCorpusHost.PVoyageRetreatRun();
    }

    private void QCorpusAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _qCorpusHost.PVoyageAdvanceRun();
    }
}
