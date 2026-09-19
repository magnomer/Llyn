using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private readonly ObservableCollection<PCategoryItem> _pCategoryItem = [];

    private readonly List<PCategoryItem> _pCategoryPreset = [];

    internal void PCategoryLoad()
    {
        IReadOnlyList<LSpeechValue> values;
        try
        {
            values = _lEngine.LEngineSpeechRead(PSpeakerLanguageRead());
        }
        catch (Exception)
        {
            values = [];
        }

        _pCategoryPreset.Clear();
        foreach (LSpeechValue value in values)
        {
            _pCategoryPreset.Add(new PCategoryItem(value.LSpeechValueId, value.LSpeechValueName, false));
        }

        PCategoryUpdate();
    }

    private LSpeechValue? PCategoryAdd(string name)
    {
        LSpeechValue? created;
        try
        {
            created = _lEngine.LEngineSpeechAdd(PSpeakerLanguageRead(), name);
        }
        catch (Exception)
        {
            return null;
        }

        PCategoryLoad();
        return created;
    }

    private void PCategoryUpdate()
    {
        string typed = (PMarkerField.Text ?? string.Empty).Trim();

        _pCategoryItem.Clear();
        foreach (PCategoryItem preset in _pCategoryPreset)
        {
            string name = preset.PCategoryItemName;
            if (typed.Length > 0 && name.IndexOf(typed, StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            _pCategoryItem.Add(new PCategoryItem(preset.PCategoryItemValue, name, PMarkerFind(name)));
        }

        bool declared = _pCategoryPreset.Count > 0;
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
