using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PSlateTemplate : ResourceDictionary
{
    private readonly PEditor _pSlateHost;

    internal PSlateTemplate(PEditor host)
    {
        _pSlateHost = host;
        InitializeComponent();
    }

    private void PSlateHandle(object sender, MouseButtonEventArgs e)
    {
        _pSlateHost.PSlateHandle(sender, e);
    }
}
