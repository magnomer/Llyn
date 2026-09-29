using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PProfferTemplate : ResourceDictionary
{
    internal PProfferTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Pick/PProfferTemplate.xaml", UriKind.Relative)));
    }
}
