using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PCategoryItem> _pCategoryItem = [];

    private readonly List<LSpeechValue> _pCategoryValue = [];

    internal void PCategoryLoad()
    {
        _pCategoryValue.Clear();

        IReadOnlyList<LSpeechValue> values;
        try
        {
            values = _lEngine.LEngineSpeechRead(_pSpeakerChoice);
        }
        catch (Exception)
        {
            values = [];
        }

        _pCategoryValue.AddRange(values);
        PCategoryUpdate();
    }

    private LSpeechValue? PCategoryAdd(string name)
    {
        LSpeechValue? held = PCategoryFind(name);
        if (held is not null)
        {
            return held;
        }

        LSpeechValue? created;
        try
        {
            created = _lEngine.LEngineSpeechAdd(_pSpeakerChoice, name);
        }
        catch (Exception)
        {
            return null;
        }

        if (created is null)
        {
            return null;
        }

        held = PCategoryFind(created.LSpeechValueName);
        if (held is not null)
        {
            return held;
        }

        _pCategoryValue.Add(created);
        return created;
    }

    private LSpeechValue? PCategoryFind(string name)
    {
        foreach (LSpeechValue held in _pCategoryValue)
        {
            if (string.Equals(held.LSpeechValueName.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return held;
            }
        }

        return null;
    }

    private void PCategoryUpdate()
    {
        string typed = (PMarkerField.Text ?? string.Empty).Trim();

        _pCategoryItem.Clear();
        foreach (LSpeechValue value in _pCategoryValue)
        {
            string name = value.LSpeechValueName;
            if (typed.Length > 0 && name.IndexOf(typed, StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            _pCategoryItem.Add(new PCategoryItem(value.LSpeechValueId, name, PMarkerFind(name)));
        }

        bool declared = _pCategoryValue.Count > 0;
        PCategoryNotice.Visibility = declared ? Visibility.Collapsed : Visibility.Visible;
        PCategoryAbsent.Visibility = declared && _pCategoryItem.Count == 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    internal void PCategoryHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCategoryItem item })
        {
            return;
        }

        PMarkerAdd(item.PCategoryItemName);
        PMarkerSwitch.IsChecked = false;
    }
}
