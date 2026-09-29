using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PRegisterTemplate : ResourceDictionary
{
    internal PRegisterTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Register/PRegisterTemplate.xaml", UriKind.Relative)));
    }
}
