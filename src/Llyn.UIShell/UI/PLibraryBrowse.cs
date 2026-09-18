using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PLibrary
{
    private readonly ObservableCollection<PIndexItem> _pIndexList = [];

    private LVista? _pLibraryVista;

    private async void PLibraryWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_lEngine);
        PLibraryReset();
    }

    private void PInquiryHandle(object sender, TextChangedEventArgs e)
    {
        _pLibraryVista?.LVistaQuerySet(PInquiry.Text ?? string.Empty);
    }

    private void POrderHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pLibraryVista is null)
        {
            return;
        }

        POrderDropper.IsChecked = false;
        _pLibraryVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pLibraryVista.LVistaOrder));
    }

    private void PSieveHandle(object sender, RoutedEventArgs e)
    {
        if (_pLibraryVista is null)
        {
            return;
        }

        _pLibraryVista.LVistaFilterSet(PChoice.PChoiceFilterRead(PSieveList));
        PSieveRestore();
    }

    internal async void PLibraryVistaRestore(LVista vista)
    {
        _pLibraryVista = vista;
        vista.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PIndexFind));
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PLibraryWorkspaceUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, PIndexEntryUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectReflex, new PObserver(this, PIndexFind));
        vista.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PIndexFind));
        vista.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PLibraryEntryUpdate));
        PDisplay.PDisplayVistaRestore(vista);
        POrderRestore();
        PSieveRestore();

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PSieveList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PSieveHandle);
        vista.LVistaQuerySet(PInquiry.Text ?? string.Empty);
        PIndexFind();
    }

    private void POrderRestore()
    {
        if (_pLibraryVista is not null)
        {
            PChoice.PChoiceOrderApply(POrderDropdown, _pLibraryVista.LVistaOrder);
        }
    }

    private void PSieveRestore()
    {
        bool active = _pLibraryVista?.LVistaFilter.LCatalogFilterActive == true;
        PSieveMark.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PIndexChosenApply()
    {
        long? chosen = _pLibraryVista?.LVistaChosen;
        foreach (PIndexItem item in _pIndexList)
        {
            item.PIndexItemChosen = chosen is not null
                && item.PIndexItemId == chosen;
        }
    }

    private void PIndexFind()
    {
        _pIndexList.Clear();
        if (_pLibraryVista is null)
        {
            return;
        }

        foreach (LVistaRow row in _lEngine.LEngineEntryFind(_pLibraryVista))
        {
            _pIndexList.Add(new PIndexItem(
                row.LVistaRowId,
                row.LVistaRowHeadword,
                row.LVistaRowLanguage,
                row.LVistaRowEpithet ?? string.Empty)
            {
                PIndexItemName = row.LVistaRowName,
                PIndexItemChosen = row.LVistaRowChosen,
            });
        }

        PIndexEmpty.Visibility = _pIndexList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PIndexHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PIndexItem item)
        {
            return;
        }

        if (!PLibraryLeaveConfirm())
        {
            return;
        }

        PIndexEntryShow(item.PIndexItemId);
    }

    internal void PIndexEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pLibraryHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PLibraryClear();
            PIndexFind();
            return;
        }

        _pLibraryVista?.LVistaSelect(id);
        PIndexChosenApply();
        PLibraryEntryShow(draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PIndexEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinId > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pLibraryVista?.LVistaSelect(bulletin.LBulletinId);
        }

        PLibraryCommandApply();
        PIndexFind();
    }

    private void PLibraryEntryUpdate(LBulletin bulletin)
    {
        if (_pLibraryVista?.LVistaChosen is not long shown)
        {
            return;
        }

        LEntry? entry;
        try
        {
            entry = _lEngine.LEngineEntryRead(shown);
        }
        catch (Exception)
        {
            return;
        }

        if (entry is null)
        {
            PLibraryClear();
            return;
        }

        PLibraryMode.IsEnabled = true;
        PLibraryCommandApply();
    }

    private void PLibraryFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PLibraryLeaveConfirm())
        {
            return;
        }

        PLibraryClear();
        PLibraryMode.IsEnabled = true;
        PLibraryScribeShow(true);
    }

    private void PLibraryScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PLibraryScribe);
        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PLibraryLeaveConfirm())
            {
                PLibraryScribeShow(true);
                return;
            }

            PLibraryScribeShow(false);

            if (_pLibraryVista?.LVistaChosen is long chosen)
            {
                PIndexEntryShow(chosen);
                return;
            }

            PLibraryClear();
            return;
        }

        if (_pLibraryVista?.LVistaChosen is not long shown)
        {
            PLibraryClear();
            return;
        }

        PEditor.PEditorEntryShow(shown);
        PLibraryScribeShow(true);
    }

    private void PLibraryScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PLibraryViewer.IsChecked = !editing;
        PLibraryScribe.IsChecked = editing;
    }

    internal void PLibraryScribeRestore(bool editing)
    {
        if (editing && _pLibraryVista?.LVistaChosen is null)
        {
            return;
        }

        if (editing)
        {
            PLibraryMode.IsEnabled = true;
        }

        PLibraryScribeShow(editing);
    }

    internal bool PLibraryLeaveConfirm()
    {
        return _pLibraryHost.PWindowDiscardConfirm(PLibraryChangeCheck(), PLibraryDraftFinish);
    }

    internal long PLibraryVoyageRead()
    {
        return _pLibraryVista?.LVistaChosen ?? 0;
    }

    private void PLibraryEntryShow(LEntryDraft draft)
    {
        PDisplay.PDisplayShow(draft);
        PLibraryMode.IsEnabled = true;
        PLibraryCommandApply();
    }

    internal void PLibraryCommandApply()
    {
        PLibraryBin.IsEnabled = _pLibraryVista?.LVistaChosen is not null;
    }

    private void PLibraryClear()
    {
        _pLibraryVista?.LVistaSelect(null);
        PIndexChosenApply();
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PLibraryScribeShow(false);
        PLibraryMode.IsEnabled = false;
        PLibraryCommandApply();
    }

    private void PLibraryStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PLibraryBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pLibraryVista?.LVistaChosen is not long id)
        {
            return;
        }

        if (!_pLibraryHost.PWindowDeleteConfirm())
        {
            return;
        }

        try
        {
            _lEngine.LEngineEntryDelete(id);
        }
        catch (Exception exception)
        {
            _pLibraryHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PLibraryClear();
    }
}
