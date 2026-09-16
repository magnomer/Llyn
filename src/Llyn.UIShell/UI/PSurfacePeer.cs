using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PSurfacePeer : FrameworkElementAutomationPeer
{
    internal PSurfacePeer(FrameworkElement owner)
        : base(owner)
    {
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Group;
    }

    protected override string GetClassNameCore()
    {
        return Owner.GetType().Name;
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
        List<AutomationPeer> children = [];
        PSurfacePeerScan(Owner, children);
        return children;
    }

    private static void PSurfacePeerScan(DependencyObject node, List<AutomationPeer> children)
    {
        int count = VisualTreeHelper.GetChildrenCount(node);
        for (int index = 0; index < count; index++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(node, index);
            if (child is Control { Focusable: true } control)
            {
                if (CreatePeerForElement(control) is AutomationPeer peer)
                {
                    children.Add(peer);
                }

                continue;
            }

            PSurfacePeerScan(child, children);
        }
    }
}
