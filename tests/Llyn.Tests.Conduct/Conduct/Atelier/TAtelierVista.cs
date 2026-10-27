using System;
using System.IO;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtelierVista
{
    [Fact]
    public void AtelierVistaStart_SubjectsAndOrders_ReachesEngineByName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        foreach (CCatalogOrder order in Enum.GetValues<CCatalogOrder>())
        {
            LVista vista = atelier.CAtelierVistaStart("order" + order, CSubject.CSubjectReference, order);

            Assert.Equal(order.ToString()[1..], vista.LVistaOrder.ToString()[1..]);
        }

        foreach (CSubject subject in Enum.GetValues<CSubject>())
        {
            LVista vista = atelier.CAtelierVistaStart("subject" + subject, subject, CCatalogOrder.CCatalogOrderName);

            Assert.Equal(subject.ToString()[1..], vista.LVistaSubject?.ToString()[1..]);
        }

        Assert.Null(atelier.CAtelierVistaStart("bare", null, CCatalogOrder.CCatalogOrderName).LVistaSubject);
    }

    [Fact]
    public void AtelierVistaStart_LayoutDetached_SkipsStoredLayout()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using (LPosture seed = engine.TPostureStart())
        {
            seed.TPostureLayoutSave(TInterface.TLayoutCreate(
                "library", LCatalogOrder.LCatalogOrderReverse, TInterface.TCatalogFilterCreate("Korean")));
        }

        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, true);
        string path = Path.Combine(workspace.TWorkspaceFolder, "posture.json");
        string before = File.ReadAllText(path);

        LVista vista = atelier.TAtelierVistaStart("library", CCatalogOrder.CCatalogOrderHeadword);
        LCatalogOrder read = vista.LVistaOrder;
        int hidden = vista.LVistaFilter.LCatalogFilterHidden.Count;
        bool editing = vista.LVistaEditing;
        vista.TVistaOrderSet(LCatalogOrder.LCatalogOrderRecent);
        vista.TVistaFilterSet(TInterface.TCatalogFilterCreate("French"));
        vista.TVistaEditingSet(true);

        Assert.Equal(LCatalogOrder.LCatalogOrderHeadword, read);
        Assert.Equal(0, hidden);
        Assert.False(editing);
        Assert.Equal(before, File.ReadAllText(path));
        Assert.False(TInterfaceConduct.TAtelierSplitRead(atelier));
    }

    [Fact]
    public void AtelierVistaStart_LayoutAttached_ReadsAndWritesLayout()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using (LPosture seed = engine.TPostureStart())
        {
            seed.TPostureLayoutSave(TInterface.TLayoutCreate(
                "library", LCatalogOrder.LCatalogOrderReverse, TInterface.TCatalogFilterCreate("Korean")));
        }

        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        LVista vista = atelier.TAtelierVistaStart("library", CCatalogOrder.CCatalogOrderHeadword);
        LCatalogOrder read = vista.LVistaOrder;
        vista.TVistaOrderSet(LCatalogOrder.LCatalogOrderRecent);

        using CAtelier reopened = TInterfaceConduct.TAtelierCreate(engine);
        Assert.Equal(LCatalogOrder.LCatalogOrderReverse, read);
        Assert.Equal(["Korean"], vista.LVistaFilter.LCatalogFilterHidden);
        Assert.Equal(
            LCatalogOrder.LCatalogOrderRecent,
            reopened.TAtelierVistaStart("library", CCatalogOrder.CCatalogOrderHeadword).LVistaOrder);
    }
}
