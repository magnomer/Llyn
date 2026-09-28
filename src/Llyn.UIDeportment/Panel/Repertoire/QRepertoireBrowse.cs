using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private readonly ObservableCollection<QAtlasItem> _qAtlasList = [];

    private IReadOnlyDictionary<long, int> _qAtlasCount = new Dictionary<long, int>();

    private async void QRepertoireWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_qRepertoireHost.PWindowAtelier);
        QRepertoireReset();
    }

    private void QInquestHandle(object sender, TextChangedEventArgs e)
    {
        _lRepertoire.LRepertoireAtlas.CAtlasQuerySet(QInquest.Text ?? string.Empty);
    }

    private void QTierHandle(object sender, RoutedEventArgs e)
    {
        QTierDropper.IsChecked = false;
        _lRepertoire.LRepertoireAtlas.CAtlasOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    internal async void QRepertoireVistaRestore()
    {
        CPanel atlas = _lRepertoire.LRepertoireAtlas.CAtlasPanel;
        QRepertoireObserverAttach();
        QRepertoireDisplay.PDisplayObserverAttach();
        QRepertoireEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            QTierList,
            "Tier",
            QTierHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderKind,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(QTierDropdown, atlas.CPanelOrder);
        QMeshRestore();

        await LEnsignImage.LEnsignLoad(_qRepertoireHost.PWindowAtelier);

        QMeshBuild();
        _lRepertoire.LRepertoireAtlas.CAtlasQuerySet(QInquest.Text ?? string.Empty);
        _lRepertoire.LRepertoireOccurrence.LOccurrenceSortieSet(QSortie.Text);
        atlas.CPanelRowsUpdate();
    }

    private void QRepertoireObserverAttach()
    {
        UserControl surface = _qRepertoireSurface;
        CPanel atlas = _lRepertoire.LRepertoireAtlas.CAtlasPanel;
        CPanel occurrence = _lRepertoire.LRepertoireOccurrence.LOccurrencePanel;
        Action<CBulletin> rows = LObserver.LObserverCreate<CBulletin>(surface, atlas.CPanelRowsUpdate);
        atlas.CPanelObserverAttach(CSubject.CSubjectVista, rows);
        atlas.CPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, QRepertoireWorkspaceUpdate));
        atlas.CPanelObserverAttach(CSubject.CSubjectSituation, rows);
        atlas.CPanelObserverAttach(CSubject.CSubjectReflex, rows);
        atlas.CPanelObserverAttach(CSubject.CSubjectSettings, rows);
        occurrence.CPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, occurrence.CPanelEntrySelect));
        occurrence.CPanelObserverAttach(CSubject.CSubjectEntry, rows);
        occurrence.CPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, _lRepertoire.LRepertoireEntryUpdate));
        occurrence.CPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, occurrence.CPanelRowsUpdate));
    }

    private void QMeshRestore()
    {
        QMeshMark.Visibility = QLook.QLookVisibleRead(_lRepertoire.LRepertoireAtlas.CAtlasFiltered);
    }

    private void QMeshBuild()
    {
        QChoice.QChoiceFilterBuild(
            QMeshList,
            _lRepertoire.LRepertoireAtlas.CAtlasLanguageRead(),
            _lRepertoire.LRepertoireAtlas.CAtlasPanel.CPanelFilter,
            QMeshHandle);
    }

    private void QInquestClear()
    {
        QInquest.Clear();
        QMeshBuild();
        QMeshRestore();
    }

    private void QSortieHandle(object sender, TextChangedEventArgs e)
    {
        _lRepertoire.LRepertoireOccurrence.LOccurrenceSortieSet(QSortie.Text);
    }

    private void QMeshHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireAtlas.CAtlasFilterSet(QChoice.QChoiceFilterRead(sender));
        QMeshRestore();
    }

    private void QAtlasFind()
    {
        IReadOnlyList<CCatalogSituation> read;
        try
        {
            read = _lRepertoire.LRepertoireAtlas.CAtlasRowsRead(
                QLocalizationCatalog.QLocalizationTextRead("Display.Unknown"),
                QLocalizationCatalog.QLocalizationTextRead("Situation.Untitled"));
            _qAtlasCount = _lRepertoire.LRepertoireAtlas.CAtlasUsageRead();
        }
        catch (Exception exception)
        {
            _qRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        string unknown = QLocalizationCatalog.QLocalizationTextRead("Display.Unknown");

        List<QAtlasItem> fresh = [];
        foreach (CCatalogSituation row in read)
        {
            fresh.Add(new QAtlasItem(row, unknown, row.CCatalogSituationChosen));
        }

        LSplice.LSpliceApply(_qAtlasList, fresh, QAtlasItem.QAtlasItemMatch, QAtlasItem.QAtlasItemSync);

        QAtlasEmpty.Visibility = QLook.QLookVisibleRead(_qAtlasList.Count == 0);

        QVignetteTally.Text = QRepertoireTallyRead(_lRepertoire.LRepertoireAtlas.CAtlasChosen);
        QScenarioTally.Text = QRepertoireTallyRead(QScenarioDesk.CDeskStoredRead());
        _lRepertoire.LRepertoireRowsApply(read);
    }

    private void QAtlasHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireSelect(
            QSender.QSenderSourceRead<QAtlasItem>(e)?.QAtlasItemId, _qRepertoireHost.PVoyageRecord);
    }

    private void QAtlasApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QAtlasItem atlas)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PAtlasRow") is Button row)
        {
            if (atlas.QAtlasItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QAtlasHandle;
            row.Click += QAtlasHandle;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PAtlasIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("situation", 16);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAtlasTitle") is TextBlock title)
        {
            title.Text = atlas.QAtlasItemTitle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAtlasKind") is TextBlock kind)
        {
            kind.Text = atlas.QAtlasItemKind;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAtlasTally") is TextBlock tally)
        {
            tally.Text = atlas.QAtlasItemCount;
        }
    }

    internal void QAtlasSituationShow(long id)
    {
        _lRepertoire.LRepertoireSituationShow(id);
    }

    private void QRepertoireBinHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireDelete();
    }

    private void QRepertoireViewerHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireScribeSet(false);
    }

    private void QRepertoireScribeHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireScribeSet(true);
    }

    internal void QRepertoireScribeRestore(bool editing)
    {
        _lRepertoire.LRepertoireAtlas.CAtlasPanel.CPanelScribeRestore(editing);
    }

    internal bool QRepertoireLeaveConfirm()
    {
        return _lRepertoire.LRepertoireLeaveConfirm();
    }

    internal long QRepertoireVoyageRead()
    {
        return _lRepertoire.LRepertoireAtlas.CAtlasPanel.CPanelChosenRead();
    }

    internal void QRepertoireVoyageShow(bool past, bool future)
    {
        QRepertoireEarlier.IsEnabled = past;
        QRepertoireLater.IsEnabled = future;
    }

    private void QRepertoireRetreatHandle(object sender, RoutedEventArgs e)
    {
        _qRepertoireHost.PVoyageRetreatRun();
    }

    private void QRepertoireAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _qRepertoireHost.PVoyageAdvanceRun();
    }
}
