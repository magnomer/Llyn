using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PPhonology
{
    private readonly ObservableCollection<PInventoryItem> _pInventoryList = [];

    private string? _pDisplayEntry;

    private LCatalogOrder _pSequenceChoice = LCatalogOrder.LCatalogOrderHeadword;

    private async void PPhonologyHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PInventory.ItemsSource = _pInventoryList;

        await PEnsign.PEnsignLoad(_lEngine);

        PInventoryFind(PProbe.Text ?? string.Empty);
    }

    private void PProbeHandle(object sender, TextChangedEventArgs e)
    {
        PInventoryFind(PProbe.Text ?? string.Empty);
    }

    private void PSequenceHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pSequenceChoice = LCatalog.LCatalogOrderParse(choice, LCatalogOrder.LCatalogOrderHeadword);
        PSequenceDropper.IsChecked = false;
        PInventoryFind(PProbe.Text ?? string.Empty);
    }

    private void PInventoryFind(string query)
    {
        _pInventoryList.Clear();
        foreach (LCatalogPronunciation row in _lEngine.LEnginePronunciationFind(query, _pSequenceChoice))
        {
            _pInventoryList.Add(new PInventoryItem(
                row.LCatalogPronunciationEntry.LEntryId,
                row.LCatalogPronunciationEntry.LEntryHeadword,
                row.LCatalogPronunciationEntry.LEntryLanguage,
                row.LCatalogPronunciationSound));
        }

        PInventoryEmpty.Visibility = _pInventoryList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PInventoryHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PInventoryItem item)
        {
            return;
        }

        if (!PPhonologyLeaveConfirm())
        {
            return;
        }

        PInventoryEntryShow(item.PInventoryItemId);
    }

    private void PInventoryEntryShow(string id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pPhonologyHost.PWindowFailureShow("Sound.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PPhonologyClear();
            PInventoryFind(PProbe.Text ?? string.Empty);
            return;
        }

        _pDisplayEntry = id;
        PPhonologyEntryShow(id, draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PInventoryEntryUpdate(string id)
    {
        _pDisplayEntry = id;
        PInventoryFind(PProbe.Text ?? string.Empty);

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is not null)
        {
            PPhonologyEntryShow(id, draft);
        }
    }

    private void PEditorEntryRestore()
    {
        if (_pDisplayEntry is null)
        {
            PEditor.PEditorReset();
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry);
    }

    private void PPhonologyFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PPhonologyLeaveConfirm())
        {
            return;
        }

        PPhonologyClear();
        PPhonologyScribe.IsEnabled = true;
        PPhonologyScribeShow(true);
    }

    private void PPhonologyScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            if (!PPhonologyLeaveConfirm())
            {
                return;
            }

            PPhonologyScribeShow(false);

            if (_pDisplayEntry is not null)
            {
                PInventoryEntryShow(_pDisplayEntry);
            }

            return;
        }

        if (_pDisplayEntry is null)
        {
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry);
        PPhonologyScribeShow(true);
    }

    private void PPhonologyScribeShow(bool editing)
    {
        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PPhonologyScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    private bool PPhonologyLeaveConfirm()
    {
        return _pPhonologyHost.PWindowDiscardConfirm(PPhonologyChangeCheck());
    }

    private void PPhonologyEntryShow(string id, LEntryDraft draft)
    {
        PDisplay.PDisplayShow(id, draft);
        PPhonologyScribe.IsEnabled = true;
    }

    private void PPhonologyClear()
    {
        _pDisplayEntry = null;
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PPhonologyScribeShow(false);
        PPhonologyScribe.IsEnabled = false;
    }
}
