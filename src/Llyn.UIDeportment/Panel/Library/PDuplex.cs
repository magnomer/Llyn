using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

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

    internal void PDuplexRestore(CWorkspaceState state)
    {
        PLeftWing.PWingRestore("left", state.CWorkspaceStateLeft);
        PRightWing.PWingRestore("right", state.CWorkspaceStateRight);
    }

    internal void PDuplexClose()
    {
        PLeftWing.PWingClose();
        PRightWing.PWingClose();
    }
}
