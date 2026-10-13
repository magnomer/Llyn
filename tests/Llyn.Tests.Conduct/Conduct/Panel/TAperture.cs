using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAperture
{
    [Fact]
    public void PanelVistaRestore_EditingVista_SwitchesEditingOff()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TInterfaceConductPanel.TPanelCreate(
            engine, TEnvoyFake.TEnvoyCreate(true, []), "Scribe", static () => false, static _ => true);
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
        vista.TVistaEditingSet(true);

        panel.TPanelVistaRestore(vista);

        Assert.False(vista.LVistaEditing);
        Assert.Same(vista, panel.TPanelVistaRead());
    }
}
