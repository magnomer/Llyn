using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PCategoryItem> _pCategoryItem = [];

    private readonly List<string> _pCategoryName = [];

    internal void PCategoryLoad()
    {
        _pCategoryName.Clear();

        IReadOnlyList<LSpeechValue> values;
        try
        {
            values = _lEngine.LEngineSpeechRead(_pSpeakerChoice);
        }
        catch (Exception)
        {
            values = [];
        }

        foreach (LSpeechValue value in values)
        {
            _pCategoryName.Add(value.LSpeechValueName);
        }

        PCategoryUpdate();
    }

    private void PCategoryAdd(string name)
    {
        if (PCategoryFind(name))
        {
            return;
        }

        LSpeechValue? created;
        try
        {
            created = _lEngine.LEngineSpeechAdd(_pSpeakerChoice, name);
        }
        catch (Exception)
        {
            return;
        }

        if (created is null || PCategoryFind(created.LSpeechValueName))
        {
            return;
        }

        _pCategoryName.Add(created.LSpeechValueName);
    }

    private bool PCategoryFind(string name)
    {
        foreach (string held in _pCategoryName)
        {
            if (string.Equals(held.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private void PCategoryUpdate()
    {
        string typed = (PMarkerField.Text ?? string.Empty).Trim();

        _pCategoryItem.Clear();
        foreach (string name in _pCategoryName)
        {
            if (typed.Length > 0 && name.IndexOf(typed, StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            _pCategoryItem.Add(new PCategoryItem(name, PMarkerFind(name)));
        }

        bool declared = _pCategoryName.Count > 0;
        PCategoryNotice.Visibility = declared ? Visibility.Collapsed : Visibility.Visible;
        PCategoryAbsent.Visibility = declared && _pCategoryItem.Count == 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    internal void PCategoryHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCategoryItem { PCategoryItemName: string name } })
        {
            return;
        }

        PMarkerAdd(name);
        PMarkerSwitch.IsChecked = false;
    }
}
