using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PDuplex : UserControl
{
    public PDuplex()
    {
        InitializeComponent();
    }

    private LEngine _lEngine = null!;

    internal void PDuplexAttach(PWindow host, LEngine engine)
    {
        _lEngine = engine;
        PLeftWing.PWingAttach(host, engine);
        PRightWing.PWingAttach(host, engine);
    }

    internal void PDuplexRestore(LWorkspaceState state)
    {
        PLeftWing.PWingRestore(
            _lEngine.LEngineVistaStart("left", LCatalogOrder.LCatalogOrderHeadword, true), state.LWorkspaceStateLeft);
        PRightWing.PWingRestore(
            _lEngine.LEngineVistaStart("right", LCatalogOrder.LCatalogOrderHeadword, true), state.LWorkspaceStateRight);
    }

    internal void PDuplexClose()
    {
        PLeftWing.PWingClose();
        PRightWing.PWingClose();
    }
}
