using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TVistaMode
{
    [Fact]
    public void EditingSet_ChangedMode_RaisesOnlyOwningVistaNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
        LVista other = engine.TEngineVistaStart("favorite", LCatalogOrder.LCatalogOrderHeadword);
        TVistaObserver observer = new();
        engine.TEngineObserverAttach(observer);
        bool initial = vista.LVistaEditing;

        vista.TVistaEditingSet(!initial);
        vista.TVistaEditingSet(!initial);

        Assert.Equal(!initial, vista.LVistaEditing);
        Assert.Equal(initial, other.LVistaEditing);
        LBulletin notice = Assert.Single(observer.TVistaNotices);
        Assert.Equal(LSubject.LSubjectVista, notice.LBulletinSubject);
        Assert.Equal(vista.LVistaId, notice.LBulletinId);
    }

    [Fact]
    public void EditingSet_WorkspaceReopened_AllVistasStartFromSavedMode()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        bool editing;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LVista vista = engine.TPostureStart().TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
            editing = !vista.LVistaEditing;
            vista.TVistaEditingSet(editing);
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        LPosture posture = reopened.TPostureStart();
        LVista library = posture.TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
        LVista favorite = posture.TPostureVistaStart("favorite", LCatalogOrder.LCatalogOrderHeadword);
        Assert.Equal(editing, library.LVistaEditing);
        Assert.Equal(editing, favorite.LVistaEditing);
    }

    [Fact]
    public void EditingSet_UnchangedVistaMode_StillUpdatesSharedPreference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        LVista first = posture.TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
        LVista second = posture.TPostureVistaStart("favorite", LCatalogOrder.LCatalogOrderHeadword);
        bool initial = first.LVistaEditing;

        first.TVistaEditingSet(!initial);
        second.TVistaEditingSet(initial);

        Assert.Equal(initial, posture.TPostureVistaStart("corpus", LCatalogOrder.LCatalogOrderText).LVistaEditing);
        Assert.Equal(!initial, first.LVistaEditing);
    }

    private sealed class TVistaObserver : LObserver
    {
        internal List<LBulletin> TVistaNotices { get; } = [];

        public void LObserverBulletinHandle(LBulletin bulletin) => TVistaNotices.Add(bulletin);
    }
}
