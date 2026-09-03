using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PSound
{
    private readonly ObservableCollection<PInventoryItem> _pInventoryList = [];

    private string? _pDisplayEntry;

    private string _pSequenceChoice = "Headword";

    private async void PSoundHandle(object sender, DependencyPropertyChangedEventArgs e)
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

        _pSequenceChoice = choice;
        PSequenceBase.IsChecked = false;
        PInventoryFind(PProbe.Text ?? string.Empty);
    }

    private IEnumerable<PInventoryItem> PSequenceSort(IReadOnlyList<PInventoryItem> items)
    {
        return _pSequenceChoice switch
        {
            "Reverse" => items.OrderByDescending(
                item => item.PInventoryItemHeadword, StringComparer.CurrentCultureIgnoreCase),
            "Sound" => items
                .OrderBy(item => item.PInventoryItemSound.Length == 0)
                .ThenBy(item => item.PInventoryItemSound, StringComparer.Ordinal),
            "Pending" => items
                .OrderByDescending(item => item.PInventoryItemSound.Length == 0)
                .ThenBy(item => item.PInventoryItemHeadword, StringComparer.CurrentCultureIgnoreCase),
            _ => items
        };
    }

    private void PInventoryFind(string query)
    {
        List<PInventoryItem> found = [];
        foreach (LEntry entry in _lEngine.LEngineEntryFind(query))
        {
            LPronunciation? pronunciation = _lEngine.LEnginePronunciationRead(entry.LEntryId);
            found.Add(new PInventoryItem(
                entry.LEntryId,
                entry.LEntryHeadword,
                entry.LEntryLanguage,
                pronunciation?.LPronunciationIpa ?? string.Empty));
        }

        _pInventoryList.Clear();
        foreach (PInventoryItem item in PSequenceSort(found))
        {
            _pInventoryList.Add(item);
        }

        PInventoryEmpty.Visibility = _pInventoryList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PInventoryHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PInventoryItem item)
        {
            return;
        }

        if (!PSoundLeaveConfirm())
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
            _pSoundHost.PWindowFailureShow("Sound.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PSoundClear();
            PInventoryFind(PProbe.Text ?? string.Empty);
            return;
        }

        _pDisplayEntry = id;
        PSoundEntryShow(id, draft);

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
            PSoundEntryShow(id, draft);
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

    private void PSoundFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PSoundLeaveConfirm())
        {
            return;
        }

        PSoundClear();
        PSoundScribe.IsEnabled = true;
        PSoundScribeShow(true);
    }

    private void PSoundScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            if (!PSoundLeaveConfirm())
            {
                return;
            }

            PSoundScribeShow(false);

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
        PSoundScribeShow(true);
    }

    private void PSoundScribeShow(bool editing)
    {
        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PSoundScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    private bool PSoundLeaveConfirm()
    {
        return _pSoundHost.PWindowDiscardConfirm(PSoundChangeCheck());
    }

    private void PSoundEntryShow(string id, LEntryDraft draft)
    {
        PDisplay.PDisplayShow(id, draft);
        PSoundScribe.IsEnabled = true;
    }

    private void PSoundClear()
    {
        _pDisplayEntry = null;
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PSoundScribeShow(false);
        PSoundScribe.IsEnabled = false;
    }
}
