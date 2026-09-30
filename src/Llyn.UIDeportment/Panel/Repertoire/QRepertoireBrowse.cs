using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private readonly ObservableCollection<QAtlasItem> _qAtlasList = [];

    private async void QRepertoireWorkspaceRefine()
    {
        await _qRepertoireHost.PWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad(QEnsignImage.QEnsignDraw);
    }

    private void QInquestObserve(object sender, TextChangedEventArgs e)
    {
        _cRepertoire.CRepertoireAtlas.CAtlasQuerySet(QInquest.Text ?? string.Empty);
    }

    private void QTierObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireAtlas.CAtlasOrderSet(QChoice.QChoiceOrderRead(sender));
        QTierRefine();
    }

    private void QTierRefine()
    {
        QTierDropper.IsChecked = false;
    }

    internal async void QRepertoireVistaRefine()
    {
        QChoice.QChoiceOrderApply(QTierDropdown, _cRepertoire.CRepertoireAtlas.CAtlasPanel.CPanelOrder);
        QMeshRefine();
        CEnsignSheet<IReadOnlyList<CCatalogSituation>> sheet =
            await _cRepertoire.CRepertoireRowsLoad(QEnsignImage.QEnsignDraw);
        QMeshListRefine(sheet.CEnsignSheetLanguages);
        QAtlasRefine(sheet.CEnsignSheetRows);
    }

    internal async void QOccurrenceVistaRefine()
    {
        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await _cRepertoire.CRepertoireOccurrence.COccurrenceRowsLoad(QEnsignImage.QEnsignDraw);
        QOccurrenceRefine(sheet.CEnsignSheetRows);
    }

    private void QMeshRefine()
    {
        QMeshMark.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireAtlas.CAtlasFiltered);
    }

    private void QMeshListRefine(IReadOnlyList<string> languages)
    {
        QChoice.QChoiceFilterBuild(
            QMeshList, languages, _cRepertoire.CRepertoireAtlas.CAtlasPanel.CPanelFilter, QMeshObserve);
    }

    private void QInquestRefine()
    {
        QInquest.Clear();
        QMeshListRefine(_cRepertoire.CRepertoireAtlas.CAtlasLanguageRead());
        QMeshRefine();
    }

    private void QSortieObserve(object sender, TextChangedEventArgs e)
    {
        _cRepertoire.CRepertoireOccurrence.COccurrenceQuerySet(QSortie.Text);
    }

    private void QMeshObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireAtlas.CAtlasFilterSet(QChoice.QChoiceFilterRead(sender));
        QMeshRefine();
    }

    private void QAtlasRefine()
    {
        QAtlasRefine(_cRepertoire.CRepertoireRowsRead());
    }

    private void QAtlasRefine(IReadOnlyList<CCatalogSituation> rows)
    {
        List<QAtlasItem> fresh = [];
        foreach (CCatalogSituation row in rows)
        {
            fresh.Add(new QAtlasItem(row, row.CCatalogSituationChosen));
        }

        QSplice.QSpliceRefine(_qAtlasList, fresh, QAtlasItem.QAtlasItemMatch, QAtlasItem.QAtlasItemSync);

        QAtlasEmpty.Visibility = QLook.QLookVisibleRead(_qAtlasList.Count == 0);
    }

    private void QRepertoireTallyRefine()
    {
        string tally = _cRepertoire.CRepertoireAtlas.CAtlasPanel.CPanelTallyRead();
        QVignetteTally.Text = tally;
        QScenarioTally.Text = tally;
    }

    private void QAtlasObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireSituationSelect(QSender.QSenderSourceRead<QAtlasItem>(e)?.QAtlasItemId);
    }

    private void QAtlasItemRefine(FrameworkElement container, object item, string? _)
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

            row.Click -= QAtlasObserve;
            row.Click += QAtlasObserve;
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

    private void QRepertoireBinObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireSituationDelete();
    }

    private void QRepertoireViewerObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireScribeToggle(false);
    }

    private void QRepertoireScribeObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireScribeToggle(true);
    }

    internal void QRepertoireVoyageRefine(bool past, bool future)
    {
        QRepertoireEarlier.IsEnabled = past;
        QRepertoireLater.IsEnabled = future;
    }

    private void QRepertoireRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qRepertoireHost.PWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QRepertoireAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qRepertoireHost.PWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }
}
