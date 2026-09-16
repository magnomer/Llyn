using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Llyn.UIShell;

public sealed class PReflexList : ItemsControl
{
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new PSurfacePeer(this);
    }
}
