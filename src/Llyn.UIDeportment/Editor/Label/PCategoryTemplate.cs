using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PCategoryTemplate : ResourceDictionary
{
    private readonly PEditor _pCategoryHost;

    internal PCategoryTemplate(PEditor host)
    {
        _pCategoryHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Label/PCategoryTemplate.xaml", UriKind.Relative)));
    }

    internal void PCategoryHandle(object sender, RoutedEventArgs e)
    {
        _pCategoryHost.PCategoryHandle(sender, e);
    }
}
