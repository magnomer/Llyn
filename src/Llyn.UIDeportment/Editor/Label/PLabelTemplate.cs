using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PLabelTemplate : ResourceDictionary
{
    internal PLabelTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Label/PLabelTemplate.xaml", UriKind.Relative)));
    }
}
