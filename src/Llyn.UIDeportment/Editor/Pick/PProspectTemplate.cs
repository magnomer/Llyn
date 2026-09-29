using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PProspectTemplate : ResourceDictionary
{
    internal PProspectTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Pick/PProspectTemplate.xaml", UriKind.Relative)));
    }
}
