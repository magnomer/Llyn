using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogEntry
{
    [Fact]
    public void EntryFind_HeadwordOrder_ReturnsAlphabetical()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogEntryCreate(engine, "stone");
        TCatalogEntryCreate(engine, "apple");
        TCatalogEntryCreate(engine, "Mist");

        Assert.Equal(
            ["apple", "Mist", "stone"],
            engine.TEngineEntryFind(string.Empty, LCatalogOrder.LCatalogOrderHeadword)
                .Select(entry => entry.LEntryHeadword));
    }

    [Fact]
    public void EntryFind_ReverseOrder_ReturnsReverseAlphabetical()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogEntryCreate(engine, "stone");
        TCatalogEntryCreate(engine, "apple");
        TCatalogEntryCreate(engine, "Mist");

        Assert.Equal(
            ["stone", "Mist", "apple"],
            engine.TEngineEntryFind(string.Empty, LCatalogOrder.LCatalogOrderReverse)
                .Select(entry => entry.LEntryHeadword));
    }

    [Fact]
    public void EntryFind_RecentOrder_ReturnsNewestFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogEntryCreate(engine, "apple");
        TCatalogEntryCreate(engine, "stone");
        TCatalogEntryCreate(engine, "cloud");

        Assert.Equal(
            ["cloud", "stone", "apple"],
            engine.TEngineEntryFind(string.Empty, LCatalogOrder.LCatalogOrderRecent)
                .Select(entry => entry.LEntryHeadword));
    }

    [Fact]
    public void EntryFind_EarliestOrder_ReturnsOldestFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogEntryCreate(engine, "apple");
        TCatalogEntryCreate(engine, "stone");
        TCatalogEntryCreate(engine, "cloud");

        Assert.Equal(
            ["apple", "stone", "cloud"],
            engine.TEngineEntryFind(string.Empty, LCatalogOrder.LCatalogOrderEarliest)
                .Select(entry => entry.LEntryHeadword));
    }

    [Fact]
    public void EntryFind_TypedQuery_ReturnsMatchingHeadwords()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogEntryCreate(engine, "water");
        TCatalogEntryCreate(engine, "watermark");
        TCatalogEntryCreate(engine, "stone");

        Assert.Equal(
            ["water", "watermark"],
            engine.TEngineEntryFind("wat", LCatalogOrder.LCatalogOrderHeadword)
                .Select(entry => entry.LEntryHeadword));
    }

    private static LEntry TCatalogEntryCreate(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a meaning",
                [],
                [],
                [],
                string.Empty,
                [],
                [],
                1)],
            []));
    }
}
