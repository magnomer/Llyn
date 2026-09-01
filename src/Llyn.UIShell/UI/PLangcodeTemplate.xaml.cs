using System.Windows;

namespace Llyn.UIShell;

/// <summary>
/// One row of the language dropdown as a template, handing the choice it carries back to the panel.
/// </summary>
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
