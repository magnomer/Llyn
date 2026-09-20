using System.Windows;
using System.Windows.Input;

namespace Llyn.UIVeneer;

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
