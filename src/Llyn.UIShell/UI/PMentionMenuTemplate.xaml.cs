using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PMentionMenuTemplate : ResourceDictionary
{
    private readonly PWindow _pMentionHost;

    internal PMentionMenuTemplate(PWindow host)
    {
        _pMentionHost = host;
        InitializeComponent();
    }

    private void PMentionMenuHandle(object sender, MouseButtonEventArgs e)
    {
        _pMentionHost.PMentionMenuHandle(sender, e);
    }
}
