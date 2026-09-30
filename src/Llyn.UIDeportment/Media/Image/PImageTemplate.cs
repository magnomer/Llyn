using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PImageTemplate : ResourceDictionary
{
    internal PImageTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Media/Image/PImageTemplate.xaml", UriKind.Relative)));
    }
}
