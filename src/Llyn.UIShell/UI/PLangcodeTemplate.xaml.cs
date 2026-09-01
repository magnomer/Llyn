using System.Windows;

namespace Llyn.UIShell;

public partial class PLangcodeTemplate : ResourceDictionary
{
    private readonly PEditor _pLangcodeHost;

    internal PLangcodeTemplate(PEditor host)
    {
        _pLangcodeHost = host;
        InitializeComponent();
    }

    private void PLangcodeHandle(object sender, RoutedEventArgs e)
    {
        _pLangcodeHost.PLangcodeHandle(sender, e);
    }
}
