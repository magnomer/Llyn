using System.Windows;
using System.Windows.Input;

namespace Llyn.UIVeneer;

public partial class PCandidateTemplate : ResourceDictionary
{
    private readonly PEditor _pCandidateHost;

    internal PCandidateTemplate(PEditor host)
    {
        _pCandidateHost = host;
        InitializeComponent();
    }

    private void PCandidateHandle(object sender, MouseButtonEventArgs e)
    {
        _pCandidateHost.PCandidateHandle(sender, e);
    }
}
