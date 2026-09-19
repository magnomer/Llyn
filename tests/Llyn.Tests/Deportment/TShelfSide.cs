using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TShelfSide
{
    [Fact]
    public void RowSelect_Source_ShowsColophon()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        LColophon? shown = null;
        shelf.LShelfPanel.LPanelDraftChanged += draft => shown = shelf.TShelfColophonRead(draft);

        shelf.TShelfRowSelect(book.LReferenceId);

        Assert.False(shelf.LShelfEntrySide);
        Assert.True(shelf.LShelfColophonShown);
        Assert.True(shelf.LShelfBinEnabled);
        Assert.Equal("Book", shown?.LColophonTitle);
        Assert.Equal(["Book"], shelf.TShelfRowsRead()
            .Where(row => row.LCatalogReferenceChosen)
            .Select(row => row.LCatalogReferenceName));
    }

    [Fact]
    public void FreshStart_SourceChosen_CreatesEntryUnderIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        long? cited = null;
        shelf.LShelfFootnote.LFootnoteCreated += id => cited = id;
        shelf.TShelfRowSelect(book.LReferenceId);

        shelf.TShelfFreshStart();

        Assert.Equal(book.LReferenceId, cited);
        Assert.True(shelf.LShelfEntrySide);
        Assert.True(shelf.LShelfEditorShown);
        Assert.False(shelf.LShelfBinEnabled);
        Assert.True(shelf.LShelfModeEnabled);
    }

    [Fact]
    public void FreshStart_NothingChosen_OpensSourceDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);

        shelf.TShelfFreshStart();

        Assert.True(shelf.LShelfImprint.LImprintHeld);
        Assert.False(shelf.LShelfImprint.LImprintDesk.LDeskStored);
        Assert.False(shelf.LShelfEntrySide);
        Assert.True(shelf.LShelfImprintShown);
        Assert.True(shelf.LShelfScribeChecked);
    }

    [Fact]
    public void EntrySelect_ScribeOffOnFreshEntry_ReturnsToSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        shelf.TShelfRowSelect(book.LReferenceId);

        shelf.TShelfEntrySelect(water.LEntryId);

        Assert.True(shelf.LShelfDisplayShown);
        Assert.False(shelf.LShelfBinEnabled);
        Assert.False(shelf.LShelfImprint.LImprintHeld);

        shelf.TShelfScribeSet(true);

        Assert.True(shelf.LShelfEditorShown);

        shelf.TShelfFreshStart();
        shelf.TShelfScribeSet(false);

        Assert.False(shelf.LShelfEntrySide);
        Assert.True(shelf.LShelfColophonShown);
        Assert.True(shelf.LShelfBinEnabled);
    }

    [Fact]
    public void Delete_EntrySide_LeavesSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        shelf.TShelfRowSelect(book.LReferenceId);
        shelf.TShelfEntrySelect(water.LEntryId);

        shelf.TShelfDelete();

        Assert.Contains(shelf.TShelfRowsRead(), row => row.LCatalogReferenceName == "Book");

        shelf.TShelfScribeSet(false);
        shelf.LShelfFootnote.LFootnotePanel.TPanelScribeSet(false);
        shelf.TShelfRowSelect(book.LReferenceId);
        shelf.TShelfDelete();

        Assert.DoesNotContain(shelf.TShelfRowsRead(), row => row.LCatalogReferenceName == "Book");
        Assert.False(shelf.LShelfBinEnabled);
    }

    private static LShelf TShelfPrepare(LEngine engine)
    {
        LShelf shelf = TInterfaceDeportment.TShelfCreate(engine, () => true);
        shelf.TShelfVistaRestore(
            engine.TEngineVistaStart("reference", LCatalogOrder.LCatalogOrderName),
            engine.TEngineVistaStart("footnote", LCatalogOrder.LCatalogOrderHeadword));
        return shelf;
    }
}
