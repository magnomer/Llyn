using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PDuplex : UserControl
{
    public PDuplex()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Library/PDuplex.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));
    }

    private PWing PLeftWing => (PWing)FindName(nameof(PLeftWing));

    private PWing PRightWing => (PWing)FindName(nameof(PRightWing));

    internal void PDuplexAttach(PWindow host)
    {
        PLeftWing.PWingAttach(host);
        PRightWing.PWingAttach(host);
    }

    internal void PDuplexRestore(LWorkspaceState state)
    {
        PLeftWing.PWingRestore("left", state.LWorkspaceStateLeft);
        PRightWing.PWingRestore("right", state.LWorkspaceStateRight);
    }

    internal void PDuplexClose()
    {
        PLeftWing.PWingClose();
        PRightWing.PWingClose();
    }
}
