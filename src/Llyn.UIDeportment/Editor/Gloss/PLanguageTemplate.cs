using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PLanguageTemplate : ResourceDictionary
{
    internal PLanguageTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Gloss/PLanguageTemplate.xaml", UriKind.Relative)));
    }
}
