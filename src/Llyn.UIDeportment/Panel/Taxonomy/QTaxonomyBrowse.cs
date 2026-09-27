using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QTaxonomy
{
    private readonly ObservableCollection<QDirectoryItem> _qDirectoryList = [];

    private readonly ObservableCollection<QMembershipItem> _qMembershipList = [];

    private async void QTaxonomyWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_qTaxonomyHost.PWindowAtelier);
        QTaxonomyReset();
    }

    private void QTaxonomyTagUpdate()
    {
        QDirectoryFind();
        _lTaxonomy.LTaxonomyPanel.LPanelDraftUpdate();
    }

    private void QExplorationHandle(object sender, TextChangedEventArgs e)
    {
        _lTaxonomy.LTaxonomyExplorationSet(QExploration.Text ?? string.Empty);
    }

    private void QScoutHandle(object sender, TextChangedEventArgs e)
    {
        _lTaxonomy.LTaxonomyScoutSet(QScout.Text);
    }

    private void QFunnelHandle(object sender, RoutedEventArgs e)
    {
        QFunnelDropper.IsChecked = false;
        _lTaxonomy.LTaxonomyFunnelSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QLatticeHandle(object sender, RoutedEventArgs e)
    {
        _lTaxonomy.LTaxonomyLatticeSet(QChoice.QChoiceFilterRead(sender));
        QLatticeRestore();
    }

    internal async void QTaxonomyVistaRestore()
    {
        UserControl surface = _qTaxonomySurface;
        _lTaxonomy.LTaxonomyObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, QDirectoryFind));
        _lTaxonomy.LTaxonomyObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, QTaxonomyWorkspaceUpdate));
        _lTaxonomy.LTaxonomyObserverAttach(
            CSubject.CSubjectTag, LObserver.LObserverCreate<CBulletin>(surface, QTaxonomyTagUpdate));
        _lTaxonomy.LTaxonomyObserverAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(surface, QDirectoryFind));
        _lTaxonomy.LTaxonomyObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(surface, QDirectoryFind));
        LPanel panel = _lTaxonomy.LTaxonomyPanel;
        panel.LPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.LPanelEntryHandle));
        panel.LPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, QMembershipFind));
        panel.LPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.LPanelDraftUpdate));
        _lTaxonomy.LTaxonomyObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, QDirectoryFind));
        QTaxonomyDisplay.PDisplayObserverAttach();
        QTaxonomyEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            QFunnelList,
            "Funnel",
            QFunnelHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
            ]);
        QChoice.QChoiceOrderApply(QFunnelDropdown, _lTaxonomy.LTaxonomyOrder);
        QLatticeRestore();

        await LEnsignImage.LEnsignLoad(_qTaxonomyHost.PWindowAtelier);

        QChoice.QChoiceFilterBuild(
            QLatticeList, _lTaxonomy.LTaxonomyLanguageRead(), _lTaxonomy.LTaxonomyFilter, QLatticeHandle);
        _lTaxonomy.LTaxonomyExplorationSet(QExploration.Text ?? string.Empty);
        _lTaxonomy.LTaxonomyScoutSet(QScout.Text);
        QDirectoryFind();
    }

    private void QLatticeRestore()
    {
        QLatticeMark.Visibility = _lTaxonomy.LTaxonomyFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QDirectoryReset()
    {
        _lTaxonomy.LTaxonomySelect(null);
    }

    private void QDirectoryFind()
    {
        IReadOnlyList<CCatalogTag> read;
        try
        {
            read = _lTaxonomy.LTaxonomyRowsRead();
        }
        catch (Exception exception)
        {
            _qTaxonomyHost.PWindowFailureShow("Tag.LoadFailed", exception);
            return;
        }

        _qDirectoryList.Clear();
        foreach (CCatalogTag row in read)
        {
            _qDirectoryList.Add(new QDirectoryItem(
                row.CCatalogTagStored.CTagId, row.CCatalogTagStored.CTagText, row.CCatalogTagChosen));
        }

        QDirectoryEmpty.Visibility = _qDirectoryList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        QMembershipFind();
    }

    private void QDirectoryHandle(object sender, RoutedEventArgs e)
    {
        QDirectoryRowSelect((sender as FrameworkElement)?.DataContext as QDirectoryItem);
    }

    private void QDirectoryRowSelect(QDirectoryItem? item)
    {
        if (item is null)
        {
            return;
        }

        _qTaxonomyHost.PVoyageRecord();
        QDirectorySelect(item.QDirectoryItemChosen ? null : item.QDirectoryItemId);
    }

    private void QDirectoryApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QDirectoryItem directory)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PDirectoryRow") is Button row)
        {
            if (directory.QDirectoryItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QDirectoryHandle;
            row.Click += QDirectoryHandle;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PDirectoryIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("tag", 16);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PDirectoryText") is TextBlock text)
        {
            text.Text = directory.QDirectoryItemText;
        }
    }

    private void QDirectorySelect(long? id)
    {
        _lTaxonomy.LTaxonomySelect(id);
        QDirectoryFind();
    }

    internal void QDirectoryTagShow(long id)
    {
        QExploration.Text = string.Empty;
        QScout.Text = string.Empty;
        _lTaxonomy.LTaxonomySelect(id);
        QDirectoryFind();
    }

    private void QTaxonomyFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!QTaxonomyLeaveConfirm())
        {
            return;
        }

        if (_lTaxonomy.LTaxonomyCoinageCheck())
        {
            QDirectoryCoinageShow(sender);
            return;
        }

        _lTaxonomy.LTaxonomyEntryCreate();
    }

    private void QDirectoryCoinageShow(object origin)
    {
        if (QSCoinage.QSCoinageShow(Window.GetWindow((DependencyObject)origin), "Coinage.Tag") is not string text)
        {
            return;
        }

        QDirectoryTagCreate(text);
    }

    private void QDirectoryTagCreate(string text)
    {
        try
        {
            QDirectoryTagOpen(_lTaxonomy.LTaxonomyTagCreate(text));
        }
        catch (Exception exception)
        {
            _qTaxonomyHost.PWindowFailureShow("Tag.CreateFailed", exception);
        }
    }

    private void QDirectoryTagOpen(long id)
    {
        _lTaxonomy.LTaxonomyPanel.LPanelClear();
        QDirectoryTagShow(id);
    }

    private void QTaxonomyScribeHandle(object sender, RoutedEventArgs e)
    {
        _lTaxonomy.LTaxonomyPanel.LPanelScribeSet(ReferenceEquals(sender, QTaxonomyScribe));
    }

    internal void QTaxonomyScribeRestore(bool editing)
    {
        _lTaxonomy.LTaxonomyPanel.LPanelScribeRestore(editing);
    }

    internal bool QTaxonomyLeaveConfirm()
    {
        return _lTaxonomy.LTaxonomyPanel.LPanelLeaveConfirm();
    }

    internal long QTaxonomyVoyageRead()
    {
        return _lTaxonomy.LTaxonomyChosen ?? 0;
    }

    private void QTaxonomyStoreHandle(object sender, RoutedEventArgs e)
    {
        QTaxonomyEditor.PEditorEntrySave();
    }

    private void QTaxonomyBinHandle(object sender, RoutedEventArgs e)
    {
        _lTaxonomy.LTaxonomyPanel.LPanelDelete();
    }
}
