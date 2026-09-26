using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public class PSlateTemplate : ResourceDictionary
{
    private readonly PEditor _pSlateHost;

    internal PSlateTemplate(PEditor host)
    {
        _pSlateHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Marker/PSlateTemplate.xaml", UriKind.Relative)));
    }

    internal void PSlateHandle(object sender, MouseButtonEventArgs e)
    {
        _pSlateHost.PSlateHandle(sender, e);
    }
}
