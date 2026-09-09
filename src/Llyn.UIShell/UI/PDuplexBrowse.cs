using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PDuplex
{
    private readonly ObservableCollection<PIndexItem> _pLeftIndex = [];

    private readonly ObservableCollection<PIndexItem> _pRightIndex = [];

    private void PLeftQueryHandle(object sender, TextChangedEventArgs e)
    {
        PDuplexIndexFind(PLeftQuery.Text ?? string.Empty, _pLeftIndex, PLeftIndex);
    }

    private void PRightQueryHandle(object sender, TextChangedEventArgs e)
    {
        PDuplexIndexFind(PRightQuery.Text ?? string.Empty, _pRightIndex, PRightIndex);
    }

    private void PLeftIndexHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PIndexItem item })
        {
            return;
        }

        PLeftIndex.Visibility = Visibility.Collapsed;
        _lEngine.LEngineLeftSave(PDuplexEntryShow(item.PIndexItemId, PLeftDisplay) ? item.PIndexItemId : null);
    }

    private void PRightIndexHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PIndexItem item })
        {
            return;
        }

        PRightIndex.Visibility = Visibility.Collapsed;
        _lEngine.LEngineRightSave(PDuplexEntryShow(item.PIndexItemId, PRightDisplay) ? item.PIndexItemId : null);
    }

    private void PDuplexIndexFind(string query, ObservableCollection<PIndexItem> catalog, ItemsControl index)
    {
        catalog.Clear();

        if (query.Trim().Length > 0)
        {
            foreach (LEntry entry in _lEngine.LEngineEntryFind(query))
            {
                catalog.Add(new PIndexItem(entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
            }
        }

        PTwin.PTwinNameApply(
            catalog, row => row.PIndexItemHeadword, (row, name) => row.PIndexItemName = name);

        index.Visibility = catalog.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private bool PDuplexEntryShow(string id, PDisplay display)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pDuplexHost.PWindowFailureShow("Duplex.LoadFailed", exception);
            return false;
        }

        if (draft is null)
        {
            display.PDisplayClear();
            return false;
        }

        display.PDisplayShow(id, draft);
        return true;
    }
}
