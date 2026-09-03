using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PCategoryItem> _pCategoryItem = [];

    internal void PCategoryLoad()
    {
        _pCategoryItem.Clear();

        IReadOnlyList<LSpeechValue> values;
        try
        {
            values = _lEngine.LEngineSpeechRead(_pLanguageChoice);
        }
        catch (Exception)
        {
            values = [];
        }

        foreach (LSpeechValue value in values)
        {
            _pCategoryItem.Add(new PCategoryItem(value.LSpeechValueName));
        }

        PCategoryNotice.Visibility = _pCategoryItem.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    internal void PCategoryHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCategoryItem { PCategoryItemName: string name } })
        {
            return;
        }

        PSpeechAdd(name);
        PSpeechBase.IsChecked = false;
    }
}
