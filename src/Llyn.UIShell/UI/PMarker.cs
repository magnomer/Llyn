using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PMarkerChip> _pMarkerChip = [];

    internal void PMarkerChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PMarkerChip chip })
        {
            _pMarkerChip.Remove(chip);
            PCategoryUpdate();
        }
    }

    private void PMarkerFieldHandle(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            PMarkerSwitch.IsChecked = false;
            e.Handled = true;
            return;
        }

        if (e.Key != Key.Enter)
        {
            return;
        }

        PMarkerAdd(PMarkerField.Text ?? string.Empty);
        e.Handled = true;
    }

    private void PMarkerTextHandle(object sender, TextChangedEventArgs e)
    {
        if (_pEditorFill)
        {
            return;
        }

        PCategoryUpdate();

        string typed = (PMarkerField.Text ?? string.Empty).Trim();
        PMarkerSwitch.IsChecked = typed.Length > 0 && _pCategoryItem.Count > 0;
    }

    private void PMarkerAdd(string name)
    {
        string typed = name.Trim();
        PMarkerField.Text = string.Empty;

        if (typed.Length == 0)
        {
            return;
        }

        if (!PMarkerFind(typed))
        {
            LSpeechValue? value = PCategoryAdd(typed);
            _pMarkerChip.Add(value is null
                ? new PMarkerChip(0, typed)
                : new PMarkerChip(value.LSpeechValueId, value.LSpeechValueName));
        }

        PCategoryUpdate();
    }

    private bool PMarkerFind(string name)
    {
        foreach (PMarkerChip chip in _pMarkerChip)
        {
            if (string.Equals(chip.PMarkerChipName, name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private IReadOnlyList<LSpeechDraft> PMarkerRead()
    {
        List<LSpeechDraft> drafts = new(_pMarkerChip.Count + 1);
        foreach (PMarkerChip chip in _pMarkerChip)
        {
            drafts.Add(chip.PMarkerChipValue > 0
                ? LSpeechDraft.LSpeechDraftCreate(chip.PMarkerChipValue, chip.PMarkerChipName)
                : LSpeechDraft.LSpeechDraftCreate(chip.PMarkerChipName));
        }

        string typed = (PMarkerField.Text ?? string.Empty).Trim();
        if (typed.Length > 0 && !PMarkerFind(typed))
        {
            drafts.Add(LSpeechDraft.LSpeechDraftCreate(typed));
        }

        return drafts;
    }

    private void PMarkerShow(IReadOnlyList<LSpeechDraft>? speeches)
    {
        _pMarkerChip.Clear();
        PMarkerField.Text = string.Empty;

        if (speeches is not null)
        {
            foreach (LSpeechDraft speech in speeches)
            {
                string name = speech.LSpeechDraftName.Trim();
                if (name.Length > 0 && !PMarkerFind(name))
                {
                    _pMarkerChip.Add(new PMarkerChip(speech.LSpeechDraftValue, name));
                }
            }
        }

        PCategoryUpdate();
    }
}
