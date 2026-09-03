using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PProspectTemplate : ResourceDictionary
{
    private readonly PEditor _pProspectHost;

    internal PProspectTemplate(PEditor host)
    {
        _pProspectHost = host;
        InitializeComponent();
    }

    private void PProspectHandle(object sender, MouseButtonEventArgs e)
    {
        _pProspectHost.PProspectHandle(sender, e);
    }
}
