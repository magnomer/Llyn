using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PSpeechItem> _pSpeechItem = [];

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

        PSpeechContents.Text = name;
        PSpeechBase.IsChecked = false;
        PSpeechContents.CaretIndex = PSpeechContents.Text.Length;
    }
}
