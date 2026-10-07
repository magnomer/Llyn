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

    [Fact]
    public void EntryFind_StarWildcard_ReturnsHeadwordsAnchoredAroundIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogEntryCreate(engine, "water");
        TCatalogEntryCreate(engine, "watermark");
        TCatalogEntryCreate(engine, "backwater");
        TCatalogEntryCreate(engine, "stone");

        Assert.Equal(
            ["water", "watermark"],
            engine.TEngineEntryFind("wat*", LCatalogOrder.LCatalogOrderHeadword)
                .Select(entry => entry.LEntryHeadword));
        Assert.Equal(
            ["backwater", "water"],
            engine.TEngineEntryFind("*ter", LCatalogOrder.LCatalogOrderHeadword)
                .Select(entry => entry.LEntryHeadword));
    }

    [Fact]
    public void EntryFind_QuestionWildcard_ReturnsHeadwordsOfThatLength()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogEntryCreate(engine, "cat");
        TCatalogEntryCreate(engine, "Cot");
        TCatalogEntryCreate(engine, "cart");
        TCatalogEntryCreate(engine, "concat");

        Assert.Equal(
            ["cat", "Cot"],
            engine.TEngineEntryFind("c?t", LCatalogOrder.LCatalogOrderHeadword)
                .Select(entry => entry.LEntryHeadword));
    }

    [Fact]
    public void EntrySort_EqualHeadwords_ListsByLanguageThenId()
    {
        IReadOnlyList<LEntry> stored =
        [
            TInterface.TEntryCreate(3, "bank", "French", 0, null, null),
            TInterface.TEntryCreate(2, "bank", "English", 0, null, null),
            TInterface.TEntryCreate(1, "bank", "French", 0, null, null),
            TInterface.TEntryCreate(4, "ant", "French", 0, null, null),
        ];

        Assert.Equal(
            [4L, 2L, 1L, 3L],
            TInterface.TCatalogEntrySort(stored, LCatalogOrder.LCatalogOrderHeadword)
                .Select(entry => entry.LEntryId));
        Assert.Equal(
            [2L, 1L, 3L, 4L],
            TInterface.TCatalogEntrySort(stored, LCatalogOrder.LCatalogOrderReverse)
                .Select(entry => entry.LEntryId));
        Assert.Equal(
            [2L, 4L, 1L, 3L],
            TInterface.TCatalogEntrySort(stored, LCatalogOrder.LCatalogOrderRecent)
                .Select(entry => entry.LEntryId));
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
                [],
                [],
                1)],
            []));
    }
}
