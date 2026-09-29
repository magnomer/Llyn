using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PContextTemplate : ResourceDictionary
{
    internal PContextTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Context/PContextTemplate.xaml", UriKind.Relative)));
    }
}
