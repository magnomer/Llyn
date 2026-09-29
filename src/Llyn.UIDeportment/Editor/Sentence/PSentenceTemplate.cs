using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PSentenceTemplate : ResourceDictionary
{
    internal PSentenceTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Sentence/PSentenceTemplate.xaml", UriKind.Relative)));
    }
}
