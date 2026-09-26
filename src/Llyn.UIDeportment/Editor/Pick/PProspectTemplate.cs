using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public class PProspectTemplate : ResourceDictionary
{
    private readonly PEditor _pProspectHost;

    internal PProspectTemplate(PEditor host)
    {
        _pProspectHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Pick/PProspectTemplate.xaml", UriKind.Relative)));
    }

    internal void PProspectHandle(object sender, MouseButtonEventArgs e)
    {
        _pProspectHost.PProspectHandle(sender, e);
    }
}
