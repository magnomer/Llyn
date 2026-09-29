using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PSlateTemplate : ResourceDictionary
{
    internal PSlateTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Marker/PSlateTemplate.xaml", UriKind.Relative)));
    }
}
