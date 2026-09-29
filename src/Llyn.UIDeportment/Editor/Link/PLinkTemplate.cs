using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PLinkTemplate : ResourceDictionary
{
    internal PLinkTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Link/PLinkTemplate.xaml", UriKind.Relative)));
    }
}
