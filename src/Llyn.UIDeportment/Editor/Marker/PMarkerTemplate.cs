using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PMarkerTemplate : ResourceDictionary
{
    internal PMarkerTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Marker/PMarkerTemplate.xaml", UriKind.Relative)));
    }
}
