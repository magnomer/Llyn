using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TOeuvre
{
    [Fact]
    public void OeuvreRowsRead_ChosenAuthor_ListsSourcesCreditingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        engine.TEngineCitationCreate("Orphan");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        (COeuvre oeuvre, LVista roll, _) = TOeuvrePrepare(engine);
        roll.TVistaSelect(ada.LAuthorId);

        IReadOnlyList<CCatalogReference> rows = oeuvre.COeuvreRowsRead();

        Assert.Equal(["Book"], rows.Select(row => row.CCatalogReferenceName));
        Assert.False(oeuvre.COeuvrePanel.CPanelAperture.CApertureEmpty);
    }

    [Fact]
    public void OeuvreRowsRead_NoVista_ReadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineCitationCreate("Orphan");
        COeuvre oeuvre = TInterfaceConductPanel.TOeuvreCreate(engine);

        Assert.Empty(oeuvre.COeuvreRowsRead());
        Assert.True(oeuvre.COeuvrePanel.CPanelAperture.CApertureEmpty);
    }

    [Fact]
    public void OeuvreRowsRead_ChosenSourceLeftTheList_ClosesThePanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        LReference orphan = engine.TEngineCitationCreate("Orphan");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        (COeuvre oeuvre, LVista roll, LVista vista) = TOeuvrePrepare(engine);
        int closed = 0;
        oeuvre.COeuvrePanel.CPanelCleared += () => closed++;
        roll.TVistaSelect(ada.LAuthorId);
        vista.TVistaSelect(book.LReferenceId);

        oeuvre.COeuvreRowsRead();
        vista.TVistaSelect(orphan.LReferenceId);
        oeuvre.COeuvreRowsRead();

        Assert.Equal(1, closed);
        Assert.Equal(0, oeuvre.COeuvrePanel.TPanelChosenRead());
    }

    [Fact]
    public void OeuvreEmptyKey_AuthorChosenAndNarrowed_ReadsUnmatched()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        (COeuvre oeuvre, LVista roll, _) = TOeuvrePrepare(engine);

        string none = oeuvre.COeuvreEmptyKey;
        roll.TVistaSelect(ada.LAuthorId);
        string vacant = oeuvre.COeuvreEmptyKey;
        oeuvre.COeuvrePanel.CPanelAperture.CApertureQuerySet("zzz");

        Assert.Equal("Source.Empty", none);
        Assert.Equal("Guild.Vacant", vacant);
        Assert.Equal("Guild.Unmatched", oeuvre.COeuvreEmptyKey);
    }

    [Fact]
    public void OeuvreQuerySet_Comb_NarrowsTheSources()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineCitationCreate("Atlas");
        engine.TEngineCitationCreate("Grammar");
        (COeuvre oeuvre, _, _) = TOeuvrePrepare(engine);

        oeuvre.COeuvrePanel.CPanelAperture.CApertureQuerySet("gram");

        Assert.Equal(["Grammar"], oeuvre.COeuvreRowsRead().Select(row => row.CCatalogReferenceName));
    }

    [Fact]
    public void OeuvreVistaRestore_FreshVistas_CarriesTheComb()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineCitationCreate("Atlas");
        engine.TEngineCitationCreate("Grammar");
        (COeuvre oeuvre, _, _) = TOeuvrePrepare(engine);
        oeuvre.COeuvrePanel.CPanelAperture.CApertureQuerySet("gram");

        oeuvre.TOeuvreVistaRestore(
            engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName),
            engine.TEngineVistaStart("oeuvre", LCatalogOrder.LCatalogOrderName));

        Assert.Equal(["Grammar"], oeuvre.COeuvreRowsRead().Select(row => row.CCatalogReferenceName));
    }

    [Fact]
    public void OeuvreTallyRead_UncitedSource_WordsNoUsage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        (COeuvre oeuvre, _, LVista vista) = TOeuvrePrepare(engine);
        vista.TVistaSelect(book.LReferenceId);

        Assert.Equal(TInterface.TLocalizationTextRead("Source.UsageNone"), oeuvre.COeuvreTallyRead());
    }

    [Fact]
    public void OeuvrePanelRowOpen_SourceDraft_RaisesItsSheet()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        (COeuvre oeuvre, _, _) = TOeuvrePrepare(engine);
        CColophon? shown = null;
        oeuvre.COeuvreColophonChanged += colophon => shown = colophon;

        oeuvre.COeuvrePanel.CPanelRowOpen(book.LReferenceId);

        Assert.Equal("Book", shown?.CColophonTitle);
        Assert.Equal(TInterface.TLocalizationTextRead("Source.UsageNone"), shown?.CColophonTally);
    }

    [Fact]
    public void OeuvreAuthorRead_OneWork_WordsTheWork()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        COeuvre oeuvre = TInterfaceConductPanel.TOeuvreCreate(engine);

        IReadOnlyList<CCatalogAuthor> rows =
            oeuvre.TOeuvreAuthorRead(engine.TEngineAuthorFind(string.Empty, LCatalogOrder.LCatalogOrderName));

        Assert.Equal(["Ada"], rows.Select(row => row.CCatalogAuthorName));
        Assert.Equal(TInterface.TLocalizationTextRead("Guild.WorkOne"), rows[0].CCatalogAuthorWork);
    }

    private static (COeuvre, LVista, LVista) TOeuvrePrepare(LEngine engine)
    {
        COeuvre oeuvre = TInterfaceConductPanel.TOeuvreCreate(engine);
        LVista roll = engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName);
        LVista vista = engine.TEngineVistaStart("oeuvre", LCatalogOrder.LCatalogOrderName);
        oeuvre.TOeuvreVistaRestore(roll, vista);
        return (oeuvre, roll, vista);
    }
}
