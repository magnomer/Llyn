using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogReference
{
    [Fact]
    public void ReferenceFind_NameOrder_ReturnsStatedFieldByPrecedence()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogReferenceCreate(engine, "Zebra Book", null, null, null, null);
        TCatalogReferenceCreate(engine, null, "Anchor Show", null, null, null);
        TCatalogReferenceCreate(engine, null, null, "Morning Channel", null, null);
        TCatalogReferenceCreate(engine, null, null, null, null, "https://bird.example");

        Assert.Equal(
            ["Anchor Show", "https://bird.example", "Morning Channel", "Zebra Book"],
            engine.TEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogReferenceName));
    }

    [Fact]
    public void ReferenceFind_NamelessSource_ReturnsIdAsName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference stored = TCatalogReferenceCreate(engine, null, null, null, null, null);

        LCatalogReference row = Assert.Single(
            engine.TEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderName));
        Assert.Equal(stored.LReferenceId, row.LCatalogReferenceName);
        Assert.Equal(stored.LReferenceId, stored.TReferenceNameRead());
    }

    [Fact]
    public void ReferenceFind_YearOrder_ReturnsUnstatedYearsFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogReferenceCreate(engine, "Later", null, null, "2010", null);
        TCatalogReferenceCreate(engine, "Undated", null, null, null, null);
        TCatalogReferenceCreate(engine, "Earlier", null, null, "1999", null);

        Assert.Equal(
            ["Undated", "Earlier", "Later"],
            engine.TEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderYear)
                .Select(row => row.LCatalogReferenceName));
    }

    [Fact]
    public void ReferenceFind_AuthorOrder_ReturnsCreditedAfterUncredited()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogReferenceCreate(engine, "Anonymous", null, null, null, null);
        LReference credited = TCatalogReferenceCreate(engine, "Credited", null, null, null, null);
        LReference later = TCatalogReferenceCreate(engine, "Also Credited", null, null, null, null);

        TCatalogCreditAttach(engine, credited.LReferenceId, "Ashby");
        TCatalogCreditAttach(engine, later.LReferenceId, "Zeller");

        Assert.Equal(
            ["Anonymous", "Credited", "Also Credited"],
            engine.TEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderAuthor)
                .Select(row => row.LCatalogReferenceName));
    }

    [Fact]
    public void ReferenceFind_UsageOrder_ReturnsMostCitedFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference quiet = TCatalogReferenceCreate(engine, "Quiet", null, null, null, null);
        LReference busy = TCatalogReferenceCreate(engine, "Busy", null, null, null, null);

        LEntry first = TCatalogEntryCreate(engine, "apple");
        LEntry second = TCatalogEntryCreate(engine, "stone");

        engine.TEngineReferenceAttach(first.LEntryId, busy.LReferenceId, 0, LOwner.LOwnerEntry);
        engine.TEngineReferenceAttach(second.LEntryId, busy.LReferenceId, 0, LOwner.LOwnerEntry);
        engine.TEngineReferenceAttach(first.LEntryId, quiet.LReferenceId, 1, LOwner.LOwnerEntry);

        IReadOnlyList<LCatalogReference> read =
            engine.TEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderUsage);

        Assert.Equal(["Busy", "Quiet"], read.Select(row => row.LCatalogReferenceName));
        Assert.Equal([2, 1], read.Select(row => row.LCatalogReferenceUsage));
    }

    [Fact]
    public void ReferenceFind_TypedQuery_ReturnsSourcesStatingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogReferenceCreate(engine, "Water Rites", null, null, null, null);
        TCatalogReferenceCreate(engine, null, "Watermark Weekly", null, null, null);
        TCatalogReferenceCreate(engine, null, null, "Riverbank", null, null);
        TCatalogReferenceCreate(engine, null, null, null, null, "https://water.example");
        TCatalogReferenceCreate(engine, "Stone Age", null, null, null, null);

        Assert.Equal(
            ["https://water.example", "Water Rites", "Watermark Weekly"],
            engine.TEngineReferenceFind("wat", LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogReferenceName));
    }

    [Fact]
    public void ReferenceFind_AuthorQuery_ReturnsCreditedSources()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference credited = TCatalogReferenceCreate(engine, "Credited", null, null, null, null);
        TCatalogReferenceCreate(engine, "Anonymous", null, null, null, null);
        TCatalogCreditAttach(engine, credited.LReferenceId, "Ashby");

        LCatalogReference row = Assert.Single(
            engine.TEngineReferenceFind("ashb", LCatalogOrder.LCatalogOrderName));
        Assert.Equal("Credited", row.LCatalogReferenceName);
        Assert.Equal("Ashby", Assert.Single(row.LCatalogReferenceCredit).LAuthorName);
    }

    private static LReference TCatalogReferenceCreate(
        LEngine engine,
        string? title,
        string? program,
        string? channel,
        string? year,
        string? url)
    {
        return engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            string.Empty,
            title,
            program,
            channel,
            year,
            url,
            LState.LStateUnspecified));
    }

    private static void TCatalogCreditAttach(LEngine engine, string referenceId, string name)
    {
        LAuthor author = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(string.Empty, name));
        engine.TEngineAuthorAttach(referenceId, author.LAuthorId, 0);
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
