using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PDuplex : UserControl
{
    public PDuplex()
    {
        InitializeComponent();
    }

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
