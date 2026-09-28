using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QTenor
{
    private const string QTenorFailure = "Register.LoadFailed";

    private readonly ObservableCollection<QGamutItem> _qGamutList = [];

    private readonly ObservableCollection<QCohortItem> _qCohortList = [];

    private async void QTenorWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_qTenorHost.PWindowAtelier);
        QTenorReset();
    }

    private void QTenorRegisterUpdate()
    {
        QGamutFind();
        _lTenor.LTenorPanel.CPanelDraftUpdate();
    }

    private void QSoundingHandle(object sender, TextChangedEventArgs e)
    {
        _lTenor.LTenorSoundingSet(QSounding.Text ?? string.Empty);
    }

    private void QQuestHandle(object sender, TextChangedEventArgs e)
    {
        _lTenor.LTenorQuestSet(QQuest.Text);
    }

    private void QDegreeHandle(object sender, RoutedEventArgs e)
    {
        QDegreeDropper.IsChecked = false;
        _lTenor.LTenorDegreeSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QGrilleHandle(object sender, RoutedEventArgs e)
    {
        _lTenor.LTenorGrilleSet(QChoice.QChoiceFilterRead(sender));
        QGrilleRestore();
    }

    internal async void QTenorVistaRestore()
    {
        UserControl surface = _qTenorSurface;
        _lTenor.LTenorObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, QGamutFind));
        _lTenor.LTenorObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, QTenorWorkspaceUpdate));
        _lTenor.LTenorObserverAttach(
            CSubject.CSubjectRegister, LObserver.LObserverCreate<CBulletin>(surface, QTenorRegisterUpdate));
        _lTenor.LTenorObserverAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(surface, QGamutFind));
        _lTenor.LTenorObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(surface, QGamutFind));
        CPanel panel = _lTenor.LTenorPanel;
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelEntryUpdate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, QCohortFind));
        panel.CPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelDraftUpdate));
        _lTenor.LTenorObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, QGamutFind));
        QTenorDisplay.PDisplayObserverAttach();
        QTenorEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            QDegreeList,
            "Degree",
            QDegreeHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(QDegreeDropdown, _lTenor.LTenorOrder);
        QGrilleRestore();

        await LEnsignImage.LEnsignLoad(_qTenorHost.PWindowAtelier);

        QChoice.QChoiceFilterBuild(
            QGrilleList, _lTenor.LTenorLanguageRead(), _lTenor.LTenorFilter, QGrilleHandle);
        _lTenor.LTenorSoundingSet(QSounding.Text ?? string.Empty);
        _lTenor.LTenorQuestSet(QQuest.Text);
        QGamutFind();
    }

    private void QGrilleRestore()
    {
        QGrilleMark.Visibility = _lTenor.LTenorFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QGamutReset()
    {
        _lTenor.LTenorSelect(null);
    }

    private void QGamutFind()
    {
        IReadOnlyList<CCatalogRegister> read;
        try
        {
            read = _lTenor.LTenorRowsRead();
        }
        catch (Exception exception)
        {
            _qTenorHost.PWindowFailureShow(QTenorFailure, exception);
            return;
        }

        _qGamutList.Clear();
        foreach (CCatalogRegister row in read)
        {
            _qGamutList.Add(new QGamutItem(
                row.CCatalogRegisterStored.CRegisterId,
                row.CCatalogRegisterStored.CRegisterName,
                row.CCatalogRegisterUsage,
                row.CCatalogRegisterChosen));
        }

        QGamutEmpty.Visibility = _qGamutList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        QCohortFind();
    }

    private void QGamutHandle(object sender, RoutedEventArgs e)
    {
        QGamutRowSelect((sender as FrameworkElement)?.DataContext as QGamutItem);
    }

    private void QGamutRowSelect(QGamutItem? item)
    {
        if (item is null)
        {
            return;
        }

        _qTenorHost.PVoyageRecord();
        QGamutSelect(item.QGamutItemChosen ? null : item.QGamutItemId);
    }

    private void QGamutApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QGamutItem gamut)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PGamutRow") is Button row)
        {
            if (gamut.QGamutItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QGamutHandle;
            row.Click += QGamutHandle;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PGamutIcon") is QIconImage icon)
        {
            icon.QIconSource = gamut.QGamutItemIcon;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGamutName") is TextBlock name)
        {
            name.Text = gamut.QGamutItemName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGamutCount") is TextBlock count)
        {
            count.Text = gamut.QGamutItemCount;
        }
    }

    private void QGamutSelect(long? id)
    {
        _lTenor.LTenorSelect(id);
        QGamutFind();
    }

    internal void QGamutRegisterShow(long id)
    {
        QSounding.Text = string.Empty;
        QQuest.Text = string.Empty;
        _lTenor.LTenorSelect(id);
        QGamutFind();
    }

    private void QTenorFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!QTenorLeaveConfirm())
        {
            return;
        }

        if (_lTenor.LTenorCoinageCheck())
        {
            QGamutCoinageShow(sender);
            return;
        }

        _lTenor.LTenorEntryCreate();
    }

    private void QGamutCoinageShow(object origin)
    {
        if (QSCoinage.QSCoinageShow(Window.GetWindow((DependencyObject)origin), "Coinage.Register") is not string name)
        {
            return;
        }

        QGamutRegisterCreate(name);
    }

    private void QGamutRegisterCreate(string name)
    {
        try
        {
            QGamutRegisterOpen(_lTenor.LTenorRegisterCreate(name));
        }
        catch (Exception exception)
        {
            _qTenorHost.PWindowFailureShow("Register.CreateFailed", exception);
        }
    }

    private void QGamutRegisterOpen(long id)
    {
        _lTenor.LTenorPanel.CPanelEntryClose();
        QGamutRegisterShow(id);
    }

    private void QTenorScribeHandle(object sender, RoutedEventArgs e)
    {
        _lTenor.LTenorPanel.CPanelScribeToggle(ReferenceEquals(sender, QTenorScribe));
    }

    internal void QTenorScribeRestore(bool editing)
    {
        _lTenor.LTenorPanel.CPanelScribeRestore(editing);
    }

    internal bool QTenorLeaveConfirm()
    {
        return _lTenor.LTenorPanel.CPanelLeaveConfirm();
    }

    internal long QTenorVoyageRead()
    {
        return _lTenor.LTenorChosen ?? 0;
    }

    private void QTenorStoreHandle(object sender, RoutedEventArgs e)
    {
        QTenorEditor.PEditorEntrySave();
    }

    private void QTenorBinHandle(object sender, RoutedEventArgs e)
    {
        _lTenor.LTenorPanel.CPanelEntryDelete();
    }
}
