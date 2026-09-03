using System.Windows;

namespace Llyn.UIShell;

public partial class PTongueTemplate : ResourceDictionary
{
    private readonly PEditor _pTongueHost;

    internal PTongueTemplate(PEditor host)
    {
        _pTongueHost = host;
        InitializeComponent();
    }

    private void PLanguageHandle(object sender, RoutedEventArgs e)
    {
        _pTongueHost.PLanguageHandle(sender, e);
    }
}
