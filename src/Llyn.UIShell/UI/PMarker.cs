using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PMarkerChip> _pMarkerChip = [];

    internal void PMarkerChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PMarkerChip chip })
        {
            _pMarkerChip.Remove(chip);
        }
    }

    private void PMarkerFieldHandle(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        PMarkerAdd(PMarkerField.Text ?? string.Empty);
        e.Handled = true;
    }

    private void PMarkerAdd(string name)
    {
        string typed = name.Trim();
        PMarkerField.Text = string.Empty;

        if (typed.Length == 0 || PMarkerFind(typed))
        {
            return;
        }

        _pMarkerChip.Add(new PMarkerChip(typed));
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

    private IReadOnlyList<string> PMarkerRead()
    {
        List<string> names = new(_pMarkerChip.Count + 1);
        foreach (PMarkerChip chip in _pMarkerChip)
        {
            names.Add(chip.PMarkerChipName);
        }

        string typed = (PMarkerField.Text ?? string.Empty).Trim();
        if (typed.Length > 0 && !PMarkerFind(typed))
        {
            names.Add(typed);
        }

        return names;
    }

    private void PMarkerShow(IReadOnlyList<string>? names)
    {
        _pMarkerChip.Clear();
        PMarkerField.Text = string.Empty;

        if (names is null)
        {
            return;
        }

        foreach (string name in names)
        {
            string typed = name.Trim();
            if (typed.Length > 0 && !PMarkerFind(typed))
            {
                _pMarkerChip.Add(new PMarkerChip(typed));
            }
        }
    }
}
