using System.Globalization;
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

        TCatalogReferenceCreate(engine, "Zebra Book", null, null, null);
        TCatalogReferenceCreate(engine, null, null, "Anchor Show", "https://bird.example");

        Assert.Equal(
            ["https://bird.example", "Zebra Book"],
            engine.TEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogReferenceName));
    }

    [Fact]
    public void ReferenceFind_NamelessSource_ReturnsIdAsName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference stored = TCatalogReferenceCreate(engine, null, null, null, null);

        LCatalogReference row = Assert.Single(
            engine.TEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderName));
        Assert.Equal(stored.LReferenceId.ToString(CultureInfo.InvariantCulture), row.LCatalogReferenceName);
        Assert.Equal(stored.LReferenceId.ToString(CultureInfo.InvariantCulture), stored.TReferenceNameRead());
    }

    [Fact]
    public void ReferenceFind_YearOrder_ReturnsUnstatedYearsFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogReferenceCreate(engine, "Later", "2010", null, null);
        TCatalogReferenceCreate(engine, "Undated", null, null, null);
        TCatalogReferenceCreate(engine, "Earlier", "1999", null, null);

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

        TCatalogReferenceCreate(engine, "Anonymous", null, null, null);
        LReference credited = TCatalogReferenceCreate(engine, "Credited", null, null, null);
        LReference later = TCatalogReferenceCreate(engine, "Also Credited", null, null, null);

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

        LReference quiet = TCatalogReferenceCreate(engine, "Quiet", null, null, null);
        LReference busy = TCatalogReferenceCreate(engine, "Busy", null, null, null);

        TCatalogCitationCreate(engine, "he said the word", busy.LReferenceId);
        TCatalogCitationCreate(engine, "not a word was spoken", busy.LReferenceId);
        TCatalogCitationCreate(engine, "in a word, no", quiet.LReferenceId);

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

        TCatalogReferenceCreate(engine, "Water Rites", null, null, null);
        TCatalogReferenceCreate(engine, "Watermark Weekly", null, null, null);
        TCatalogReferenceCreate(engine, "Riverbank", null, "shot near the water", null);
        TCatalogReferenceCreate(engine, null, null, null, "https://water.example");
        TCatalogReferenceCreate(engine, "Stone Age", null, null, null);

        Assert.Equal(
            ["https://water.example", "Riverbank", "Water Rites", "Watermark Weekly"],
            engine.TEngineReferenceFind("wat", LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogReferenceName));
    }

    [Fact]
    public void ReferenceFind_AuthorQuery_ReturnsCreditedSources()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference credited = TCatalogReferenceCreate(engine, "Credited", null, null, null);
        TCatalogReferenceCreate(engine, "Anonymous", null, null, null);
        TCatalogCreditAttach(engine, credited.LReferenceId, "Ashby");

        LCatalogReference row = Assert.Single(
            engine.TEngineReferenceFind("ashb", LCatalogOrder.LCatalogOrderName));
        Assert.Equal("Credited", row.LCatalogReferenceName);
        Assert.Equal("Ashby", Assert.Single(row.LCatalogReferenceCredit).LAuthorName);
    }

    private static LReference TCatalogReferenceCreate(
        LEngine engine,
        string? title,
        string? year,
        string? note,
        string? url)
    {
        return engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            title,
            year,
            LReferenceKind.LReferenceKindUnspecified,
            note,
            url,
            LStateMark.LStateMarkUnspecified));
    }

    private static void TCatalogCreditAttach(LEngine engine, long referenceId, string name)
    {
        LAuthor author = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, name));
        engine.TEngineAuthorAttach(referenceId, author.LAuthorId, 0);
    }

    private static void TCatalogCitationCreate(LEngine engine, string text, long referenceId)
    {
        engine.TEngineExampleCreate(
            TInterface.TExampleCreate(
            0, "English", text, null, TInterface.TStateAnchorRead(referenceId)));
    }
}
