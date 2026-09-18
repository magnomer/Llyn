using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PYunjing
{
    private readonly ObservableCollection<PXiaoyunItem> _pXiaoyunList = [];

    private LVista? _pXiaoyunVista;

    private void PXiaoyunFind()
    {
        List<long> wanted = [];
        if (_pShengmuVista?.LVistaChosen is long onset)
        {
            wanted.Add(onset);
        }

        if (_pYunmuVista?.LVistaChosen is long rime)
        {
            wanted.Add(rime);
        }

        string query = (PBeacon.Text ?? string.Empty).Trim();
        _pXiaoyunList.Clear();
        if (_pYunjingLanguage is string language && wanted.Count > 0)
        {
            IReadOnlyList<LVistaRow> rows;
            try
            {
                rows = _lEngine.LEngineXiaoyunFind(language, wanted, query);
            }
            catch (Exception exception)
            {
                _pYunjingHost.PWindowFailureShow(PYunjingFailure, exception);
                return;
            }

            foreach (LVistaRow row in rows)
            {
                _pXiaoyunList.Add(new PXiaoyunItem(row));
            }
        }

        PXiaoyunEmpty.SetResourceReference(
            TextBlock.TextProperty,
            wanted.Count == 0 ? "Yunjing.XiaoyunEmpty"
            : query.Length == 0 ? "Yunjing.XiaoyunVacant"
            : "Yunjing.XiaoyunUnmatched");
        PXiaoyunEmpty.Visibility = _pXiaoyunList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        PXiaoyunChosenApply();
    }

    private void PXiaoyunChosenApply()
    {
        long? id = _pXiaoyunVista?.LVistaChosen;
        foreach (PXiaoyunItem item in _pXiaoyunList)
        {
            item.PXiaoyunItemChosen = id is not null && item.PXiaoyunItemId == id;
        }
    }

    private void PXiaoyunHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PXiaoyunItem item)
        {
            return;
        }

        if (!PYunjingLeaveConfirm())
        {
            return;
        }

        PXiaoyunEntryShow(item.PXiaoyunItemId);
    }

    private void PXiaoyunEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pYunjingHost.PWindowFailureShow(PYunjingFailure, exception);
            return;
        }

        if (draft is null)
        {
            PYunjingClear();
            PYunjingLoad();
            return;
        }

        _pXiaoyunVista?.LVistaSelect(id);
        PXiaoyunChosenApply();
        PYunjingBin.IsEnabled = true;
        PYunjingEntryShow(draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PXiaoyunEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinId > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pXiaoyunVista?.LVistaSelect(bulletin.LBulletinId);
            PXiaoyunChosenApply();
            PYunjingBin.IsEnabled = true;
        }

        PYunjingLoad();
    }

    private void PYunjingEntryUpdate()
    {
        if (_pXiaoyunVista?.LVistaChosen is not long shown)
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(shown);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            PYunjingClear();
            return;
        }

        PYunjingEntryShow(draft);
    }

    private void PYunjingEntryShow(LEntryDraft draft)
    {
        PDiweiHide();
        PDisplay.PDisplayShow(draft);
        PYunjingMode.IsEnabled = true;
    }

    private void PYunjingClear()
    {
        PDiweiHide();
        _pXiaoyunVista?.LVistaSelect(null);
        PXiaoyunChosenApply();
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PYunjingScribeShow(false);
        PYunjingMode.IsEnabled = false;
        PYunjingBin.IsEnabled = false;
    }

    private void PYunjingFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PYunjingLeaveConfirm())
        {
            return;
        }

        PYunjingClear();
        PYunjingMode.IsEnabled = true;
        PYunjingScribeShow(true);
    }

    private void PYunjingStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PYunjingBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pXiaoyunVista?.LVistaChosen is not long id)
        {
            return;
        }

        if (!_pYunjingHost.PWindowDeleteConfirm())
        {
            return;
        }

        try
        {
            _lEngine.LEngineEntryDelete(id);
        }
        catch (Exception exception)
        {
            _pYunjingHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PYunjingClear();
    }

    private void PYunjingScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PYunjingScribe);
        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PYunjingLeaveConfirm())
            {
                PYunjingScribeShow(true);
                return;
            }

            PYunjingScribeShow(false);

            if (_pXiaoyunVista?.LVistaChosen is long shown)
            {
                PXiaoyunEntryShow(shown);
                return;
            }

            PYunjingClear();
            return;
        }

        if (_pXiaoyunVista?.LVistaChosen is not long edited)
        {
            PYunjingClear();
            return;
        }

        PEditor.PEditorEntryShow(edited);
        PYunjingScribeShow(true);
    }

    private void PYunjingScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PYunjingViewer.IsChecked = !editing;
        PYunjingScribe.IsChecked = editing;
    }

    internal void PYunjingScribeRestore(bool editing)
    {
        if (editing && _pXiaoyunVista?.LVistaChosen is null)
        {
            return;
        }

        if (editing)
        {
            PYunjingMode.IsEnabled = true;
        }

        PYunjingScribeShow(editing);
    }

    internal bool PYunjingLeaveConfirm()
    {
        return _pYunjingHost.PWindowDiscardConfirm(PYunjingChangeCheck(), PYunjingDraftFinish);
    }
}
