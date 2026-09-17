using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PYunjing
{
    private readonly ObservableCollection<PXiaoyunItem> _pXiaoyunList = [];

    private long? _pDisplayEntry;

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
        PXiaoyunSelect(_pDisplayEntry);
    }

    private void PXiaoyunSelect(long? id)
    {
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

        _pDisplayEntry = id;
        PXiaoyunSelect(id);
        PYunjingBin.IsEnabled = true;
        PYunjingEntryShow(id, draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PXiaoyunEntryUpdate(long id)
    {
        if (id > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pDisplayEntry = id;
            PXiaoyunSelect(id);
            PYunjingBin.IsEnabled = true;
        }

        PYunjingLoad();

        if (_pDisplayEntry is not long shown || (id > 0 && shown != id))
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

        PYunjingEntryShow(shown, draft);
    }

    private void PYunjingEntryShow(long id, LEntryDraft draft)
    {
        PDiweiHide();
        PDisplay.PDisplayShow(id, draft);
        PYunjingMode.IsEnabled = true;
    }

    private void PYunjingClear()
    {
        PDiweiHide();
        _pDisplayEntry = null;
        PXiaoyunSelect(null);
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
        if (_pDisplayEntry is not long id)
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

            if (_pDisplayEntry is not null)
            {
                PXiaoyunEntryShow(_pDisplayEntry.Value);
                return;
            }

            PYunjingClear();
            return;
        }

        if (_pDisplayEntry is null)
        {
            PYunjingClear();
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry.Value);
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
        if (editing && _pDisplayEntry is null)
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
