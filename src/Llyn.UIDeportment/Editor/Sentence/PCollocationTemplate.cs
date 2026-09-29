using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PCollocationTemplate : ResourceDictionary
{
    internal PCollocationTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Sentence/PCollocationTemplate.xaml", UriKind.Relative)));
    }
}
