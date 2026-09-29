using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PCategoryTemplate : ResourceDictionary
{
    internal PCategoryTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Label/PCategoryTemplate.xaml", UriKind.Relative)));
    }
}
