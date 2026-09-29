using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PMeaningTemplate : ResourceDictionary
{
    internal PMeaningTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Gloss/PMeaningTemplate.xaml", UriKind.Relative)));
    }
}
