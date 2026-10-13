using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineVistaRelay
{
    [Fact]
    public void VistaOrderRead_NoVista_ReadsHeadwordOrder()
    {
        Assert.Equal(LCatalogOrder.LCatalogOrderHeadword, TInterface.TVistaOrderRead(null));
    }

    [Fact]
    public void VistaStoredCheck_OrphanRowOrNone_IsNotStored()
    {
        Assert.False(TInterface.TVistaStoredCheck(null));
        Assert.False(TInterface.TVistaStoredCheck(0));
        Assert.True(TInterface.TVistaStoredCheck(7));
    }

    [Fact]
    public void VistaFilterRead_NoVista_ReadsEmptyFilter()
    {
        Assert.Same(LCatalogFilter.LCatalogFilterEmpty, TInterface.TVistaFilterRead(null));
    }

    [Fact]
    public void VistaFilterSet_NothingHidden_KeepsSharedEmptyFilter()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        vista.TVistaFilterSet(["Latin", "Greek"]);
        IReadOnlyList<string> hidden = vista.LVistaFilter.LCatalogFilterHidden;
        vista.TVistaFilterSet([]);

        Assert.Equal(["Latin", "Greek"], hidden);
        Assert.Same(LCatalogFilter.LCatalogFilterEmpty, vista.LVistaFilter);
    }

    [Fact]
    public void VistaUsageRead_CreditedAuthor_CountsItsWorks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        LVista vista = engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName);

        int none = engine.TVistaUsageRead(vista);
        vista.TVistaSelect(ada.LAuthorId);

        Assert.Equal(0, none);
        Assert.Equal(1, engine.TVistaUsageRead(vista));
    }

    [Fact]
    public void VistaUsageRead_ChosenEntry_CountsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "w", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        vista.TVistaSelect(entry.LEntryId);

        Assert.Equal(0, engine.TVistaUsageRead(vista));
    }
}
