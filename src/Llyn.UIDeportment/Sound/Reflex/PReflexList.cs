using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public sealed class PReflexList : ItemsControl
{
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new PSurfacePeer(this);
    }
}
