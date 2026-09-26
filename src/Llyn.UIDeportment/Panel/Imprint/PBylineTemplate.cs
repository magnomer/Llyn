using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public class PBylineTemplate : ResourceDictionary
{
    private readonly PImprint _pBylineHost;

    internal PBylineTemplate(PImprint host)
    {
        _pBylineHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Imprint/PBylineTemplate.xaml", UriKind.Relative)));
    }

    internal void PBylineHandle(object sender, MouseButtonEventArgs e)
    {
        _pBylineHost.PBylineHandle(sender, e);
    }
}
