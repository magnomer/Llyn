using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PVideoTemplate : ResourceDictionary
{
    internal PVideoTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Media/Video/PVideoTemplate.xaml", UriKind.Relative)));
    }
}
