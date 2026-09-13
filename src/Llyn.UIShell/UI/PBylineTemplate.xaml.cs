using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PBylineTemplate : ResourceDictionary
{
    private readonly PImprint _pBylineHost;

    internal PBylineTemplate(PImprint host)
    {
        _pBylineHost = host;
        InitializeComponent();
    }

    private void PBylineHandle(object sender, MouseButtonEventArgs e)
    {
        _pBylineHost.PBylineHandle(sender, e);
    }
}
