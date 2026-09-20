using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PDuplex : UserControl
{
    public PDuplex()
    {
        InitializeComponent();
    }

    private LPosture _lPosture = null!;

    internal void PDuplexAttach(PWindow host)
    {
        _lPosture = host.PWindowPosture;
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
