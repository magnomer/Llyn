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

    internal void PDuplexAttach(PWindow host, LEngine engine)
    {
        _lPosture = host.PWindowPosture;
        PLeftWing.PWingAttach(host, engine);
        PRightWing.PWingAttach(host, engine);
    }

    internal void PDuplexRestore(LWorkspaceState state)
    {
        PLeftWing.PWingRestore(
            _lPosture.LPostureVistaStart("left", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword, true),
            state.LWorkspaceStateLeft);
        PRightWing.PWingRestore(
            _lPosture.LPostureVistaStart("right", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword, true),
            state.LWorkspaceStateRight);
    }

    internal void PDuplexClose()
    {
        PLeftWing.PWingClose();
        PRightWing.PWingClose();
    }
}
