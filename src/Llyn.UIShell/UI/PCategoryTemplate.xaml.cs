using System.Windows;

namespace Llyn.UIShell;

public partial class PCategoryTemplate : ResourceDictionary
{
    private readonly PEditor _pCategoryHost;

    internal PCategoryTemplate(PEditor host)
    {
        _pCategoryHost = host;
        InitializeComponent();
    }

    private void PCategoryHandle(object sender, RoutedEventArgs e)
    {
        _pCategoryHost.PCategoryHandle(sender, e);
    }
}
