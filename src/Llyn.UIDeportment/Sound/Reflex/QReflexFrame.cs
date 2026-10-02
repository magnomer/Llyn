using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public sealed class QReflexFrame : Decorator
{
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new QVeilPeer(this);
    }
}
