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
        _cCorpus.CCorpusAnthology.CAnthologyQuerySet(QQuery.Text ?? string.Empty);
    }

    private void QRankHandle(object sender, RoutedEventArgs e)
    {
        QRankDropper.IsChecked = false;
        _cCorpus.CCorpusAnthology.CAnthologyOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    internal async void QCorpusVistaRestore()
    {
        CPanel anthology = _cCorpus.CCorpusAnthology.CAnthologyPanel;
        QCorpusObserverAttach();
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
        QChoice.QChoiceOrderApply(QRankDropdown, anthology.CPanelOrder);
        QGauzeRestore();

        await LEnsignImage.LEnsignLoad(_qCorpusHost.PWindowAtelier);

        QGauzeBuild();
        _cCorpus.CCorpusAnthology.CAnthologyQuerySet(QQuery.Text ?? string.Empty);
        _cCorpus.CCorpusQuotation.CQuotationQuerySet(QDredge.Text);
        QSpeakerLoad();
        QCitationFind();
        anthology.CPanelRowsResonate();
    }

    private void QCorpusObserverAttach()
    {
        CPanel anthology = _cCorpus.CCorpusAnthology.CAnthologyPanel;
        CPanel quotation = _cCorpus.CCorpusQuotation.CQuotationPanel;
        UserControl surface = _qCorpusSurface;
        Action<CBulletin> rows = LObserver.LObserverCreate<CBulletin>(surface, anthology.CPanelRowsResonate);
        Action<CBulletin> citation = LObserver.LObserverCreate<CBulletin>(surface, QCitationFind);
        anthology.CPanelObserverAttach(CSubject.CSubjectVista, rows);
        anthology.CPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, QCorpusWorkspaceUpdate));
        anthology.CPanelObserverAttach(CSubject.CSubjectExample, citation);
        anthology.CPanelObserverAttach(CSubject.CSubjectExample, rows);
        anthology.CPanelObserverAttach(CSubject.CSubjectReference, citation);
        anthology.CPanelObserverAttach(CSubject.CSubjectReference, rows);
        anthology.CPanelObserverAttach(CSubject.CSubjectReflex, rows);
        anthology.CPanelObserverAttach(CSubject.CSubjectSettings, rows);
        quotation.CPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, quotation.CPanelEntrySelect));
        quotation.CPanelObserverAttach(CSubject.CSubjectEntry, citation);
        quotation.CPanelObserverAttach(CSubject.CSubjectEntry, rows);
        quotation.CPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, _cCorpus.CCorpusEntryResonate));
        quotation.CPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, quotation.CPanelRowsResonate));
    }

    private void QGauzeRestore()
    {
        QGauzeMark.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusAnthology.CAnthologyFiltered);
    }

    private void QGauzeBuild()
    {
        QChoice.QChoiceFilterBuild(
            QGauzeList,
            _qCorpusHost.PWindowAtelier.CAtelierCatalog.CCatalogLanguageRead(),
            _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelFilter,
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
        _cCorpus.CCorpusQuotation.CQuotationQuerySet(QDredge.Text);
    }

    private void QGauzeHandle(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusAnthology.CAnthologyFilterSet(QChoice.QChoiceFilterRead(sender));
        QGauzeRestore();
    }

    private void QAnthologyFind()
    {
        IReadOnlyList<CCatalogExample> read;
        try
        {
            read = _cCorpus.CCorpusRowsRead(
                QLocalizationCatalog.QLocalizationTextRead("Display.Unknown"),
                QLocalizationCatalog.QLocalizationTextRead("Example.Unwritten"));
            _qAnthologyCount = _cCorpus.CCorpusAnthology.CAnthologyUsageRead();
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

        QExcerptTally.Text = QCorpusTallyRead(_cCorpus.CCorpusAnthology.CAnthologyChosen);
        QTranscriptTally.Text = QCorpusTallyRead(QTranscriptDesk.CDeskStoredRead());
    }

    private void QAnthologyHandle(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusExampleSelect(
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
        _cCorpus.CCorpusExampleOpen(id);
    }

    private void QCorpusBinHandle(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusExampleDelete();
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
        _cCorpus.CCorpusSession.CSessionSave();
    }

    private void QCorpusViewerHandle(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusScribeToggle(false);
    }

    private void QCorpusScribeHandle(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusScribeToggle(true);
    }

    internal void QCorpusScribeRestore(bool editing)
    {
        _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelScribeRestore(editing);
    }

    internal bool QCorpusLeaveConfirm()
    {
        return _cCorpus.CCorpusLeaveConfirm();
    }

    internal long QCorpusVoyageRead()
    {
        return _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelChosenRead();
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
