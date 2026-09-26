using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public class PMentionMenuTemplate : ResourceDictionary
{
    private readonly PWindow _pMentionHost;

    internal PMentionMenuTemplate(PWindow host)
    {
        _pMentionHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Mention/Menu/PMentionMenuTemplate.xaml", UriKind.Relative)));
    }

    internal void PMentionMenuHandle(object sender, MouseButtonEventArgs e)
    {
        _pMentionHost.PMentionMenuHandle(sender, e);
    }
}
