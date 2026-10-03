using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineVistaBulletin
{
    [Fact]
    public void VistaOrderSet_ChangedOrder_RaisesVistaBulletin()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<long> raised = [];
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectVista)
            {
                raised.Add(bulletin.LBulletinId);
            }
        });
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        vista.TVistaOrderSet(LCatalogOrder.LCatalogOrderRecent);

        Assert.Equal([vista.LVistaId], raised);
    }

    [Fact]
    public void VistaOrderSet_SameOrder_RaisesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<long> raised = [];
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectVista)
            {
                raised.Add(bulletin.LBulletinId);
            }
        });
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        vista.TVistaOrderSet(LCatalogOrder.LCatalogOrderHeadword);
        vista.TVistaFilterSet(vista.LVistaFilter);
        vista.TVistaQuerySet(string.Empty);
        vista.TVistaSelect(null);

        Assert.Empty(raised);
    }
}
