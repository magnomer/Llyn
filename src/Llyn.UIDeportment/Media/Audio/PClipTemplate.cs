using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PClipTemplate : ResourceDictionary
{
    internal PClipTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Media/Audio/PClipTemplate.xaml", UriKind.Relative)));
    }
}
