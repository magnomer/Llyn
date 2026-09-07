using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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
            _pMarkerChip.Add(new PMarkerChip(typed));
        }

        PCategoryAdd(typed);
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

        if (names is not null)
        {
            foreach (string name in names)
            {
                string typed = name.Trim();
                if (typed.Length > 0 && !PMarkerFind(typed))
                {
                    _pMarkerChip.Add(new PMarkerChip(typed));
                }
            }
        }

        PCategoryUpdate();
    }
}
