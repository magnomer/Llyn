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
        _cTaxonomy.CTaxonomyPanel.CPanelDraftResonate();
    }

    private void QExplorationHandle(object sender, TextChangedEventArgs e)
    {
        _cTaxonomy.CTaxonomyQuerySet(QExploration.Text ?? string.Empty);
    }

    private void QScoutHandle(object sender, TextChangedEventArgs e)
    {
        _cTaxonomy.CTaxonomyMembershipFind(QScout.Text);
    }

    private void QFunnelHandle(object sender, RoutedEventArgs e)
    {
        QFunnelDropper.IsChecked = false;
        _cTaxonomy.CTaxonomyOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QLatticeHandle(object sender, RoutedEventArgs e)
    {
        _cTaxonomy.CTaxonomyFilterSet(QChoice.QChoiceFilterRead(sender));
        QLatticeRestore();
    }

    internal async void QTaxonomyVistaRestore()
    {
        UserControl surface = _qTaxonomySurface;
        _cTaxonomy.CTaxonomyObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, QDirectoryFind));
        _cTaxonomy.CTaxonomyObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, QTaxonomyWorkspaceUpdate));
        _cTaxonomy.CTaxonomyObserverAttach(
            CSubject.CSubjectTag, LObserver.LObserverCreate<CBulletin>(surface, QTaxonomyTagUpdate));
        _cTaxonomy.CTaxonomyObserverAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(surface, QDirectoryFind));
        _cTaxonomy.CTaxonomyObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(surface, QDirectoryFind));
        CPanel panel = _cTaxonomy.CTaxonomyPanel;
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelEntryResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, QMembershipFind));
        panel.CPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelDraftResonate));
        _cTaxonomy.CTaxonomyObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, QDirectoryFind));
        QTaxonomyEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            QFunnelList,
            "Funnel",
            QFunnelHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
            ]);
        QChoice.QChoiceOrderApply(QFunnelDropdown, _cTaxonomy.CTaxonomyOrder);
        QLatticeRestore();

        await LEnsignImage.LEnsignLoad(_qTaxonomyHost.PWindowAtelier);

        QChoice.QChoiceFilterBuild(
            QLatticeList, _cTaxonomy.CTaxonomyLanguageRead(), _cTaxonomy.CTaxonomyFilter, QLatticeHandle);
        _cTaxonomy.CTaxonomyQuerySet(QExploration.Text ?? string.Empty);
        _cTaxonomy.CTaxonomyMembershipFind(QScout.Text);
        QDirectoryFind();
    }

    private void QLatticeRestore()
    {
        QLatticeMark.Visibility = _cTaxonomy.CTaxonomyFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QDirectoryReset()
    {
        _cTaxonomy.CTaxonomyTagSelect(null);
    }

    private void QDirectoryFind()
    {
        IReadOnlyList<CCatalogTag> read;
        try
        {
            read = _cTaxonomy.CTaxonomyRowsRead();
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

        _cTaxonomy.CTaxonomyTagToggle(item.QDirectoryItemId);
        QDirectoryFind();
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

    private void QDirectoryTagRefine()
    {
        QExploration.Text = string.Empty;
        QScout.Text = string.Empty;
        QDirectoryFind();
    }

    private void QTaxonomyFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!QTaxonomyLeaveConfirm())
        {
            return;
        }

        if (_cTaxonomy.CTaxonomyCoinageAllowed)
        {
            QDirectoryCoinageShow(sender);
            return;
        }

        _cTaxonomy.CTaxonomyEntryCreate();
    }

    private void QDirectoryCoinageShow(object origin)
    {
        if (QSCoinage.QSCoinageShow(Window.GetWindow((DependencyObject)origin), "Coinage.Tag") is not string text)
        {
            return;
        }

        _cTaxonomy.CTaxonomyTagCreate(text);
    }

    private void QTaxonomyScribeHandle(object sender, RoutedEventArgs e)
    {
        _cTaxonomy.CTaxonomyPanel.CPanelScribeToggle(ReferenceEquals(sender, QTaxonomyScribe));
    }

    internal bool QTaxonomyLeaveConfirm()
    {
        return _cTaxonomy.CTaxonomyPanel.CPanelLeaveConfirm();
    }

    private void QTaxonomyStoreHandle(object sender, RoutedEventArgs e)
    {
        QTaxonomyEditor.PEditorEntrySave();
    }

    private void QTaxonomyBinHandle(object sender, RoutedEventArgs e)
    {
        _cTaxonomy.CTaxonomyPanel.CPanelEntryDelete();
    }
}
