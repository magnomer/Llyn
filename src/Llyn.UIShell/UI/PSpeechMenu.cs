using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PSpeechItem> _pSpeechItem = [];

    private readonly ObservableCollection<PSpeechChip> _pSpeechChip = [];

    internal void PSpeechLoad()
    {
        _pSpeechItem.Clear();

        IReadOnlyList<LSpeechValue> values;
        try
        {
            values = _lEngine.LEngineSpeechRead(_pLangcodeChoice);
        }
        catch (Exception)
        {
            values = [];
        }

        foreach (LSpeechValue value in values)
        {
            _pSpeechItem.Add(new PSpeechItem(value.LSpeechValueName));
        }

        PSpeechMenuNotice.Visibility = _pSpeechItem.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    internal void PSpeechHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSpeechItem { PSpeechItemName: string name } })
        {
            return;
        }

        PSpeechAdd(name);
        PSpeechBase.IsChecked = false;
    }

    internal void PSpeechChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PSpeechChip chip })
        {
            _pSpeechChip.Remove(chip);
        }
    }

    private void PSpeechContentsHandle(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        PSpeechAdd(PSpeechContents.Text ?? string.Empty);
        e.Handled = true;
    }

    private void PSpeechAdd(string name)
    {
        string typed = name.Trim();
        PSpeechContents.Text = string.Empty;

        if (typed.Length == 0 || PSpeechFind(typed))
        {
            return;
        }

        _pSpeechChip.Add(new PSpeechChip(typed));
    }

    private bool PSpeechFind(string name)
    {
        foreach (PSpeechChip chip in _pSpeechChip)
        {
            if (string.Equals(chip.PSpeechChipName, name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private IReadOnlyList<string> PSpeechRead()
    {
        List<string> names = new(_pSpeechChip.Count + 1);
        foreach (PSpeechChip chip in _pSpeechChip)
        {
            names.Add(chip.PSpeechChipName);
        }

        string typed = (PSpeechContents.Text ?? string.Empty).Trim();
        if (typed.Length > 0 && !PSpeechFind(typed))
        {
            names.Add(typed);
        }

        return names;
    }

    private void PSpeechShow(IReadOnlyList<string>? names)
    {
        _pSpeechChip.Clear();
        PSpeechContents.Text = string.Empty;

        if (names is null)
        {
            return;
        }

        foreach (string name in names)
        {
            string typed = name.Trim();
            if (typed.Length > 0 && !PSpeechFind(typed))
            {
                _pSpeechChip.Add(new PSpeechChip(typed));
            }
        }
    }
}
