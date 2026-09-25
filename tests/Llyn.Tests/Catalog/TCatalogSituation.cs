using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogSituation
{
    [Fact]
    public void SituationFind_NameOrder_ReturnsTitleOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogSituationCreate(engine, "in court", null, null);
        TCatalogSituationCreate(engine, "at home", null, null);

        Assert.Equal(
            ["at home", "in court"],
            engine.TEngineSituationFind(string.Empty, LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogSituationStored.LSituationTitle.TStateValueShow()));
    }

    [Fact]
    public void SituationFind_KindOrder_ReturnsKindGrouped()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogSituationCreate(engine, "in court", null, "register");
        TCatalogSituationCreate(engine, "at home", null, "domain");

        Assert.Equal(
            ["domain", "register"],
            engine.TEngineSituationFind(string.Empty, LCatalogOrder.LCatalogOrderKind)
                .Select(row => row.LCatalogSituationStored.LSituationKind.TStateValueShow()));
    }

    [Fact]
    public void SituationFind_UsageOrder_ReturnsMostCitedFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSituation quiet = TCatalogSituationCreate(engine, "quiet place", null, null);
        LSituation busy = TCatalogSituationCreate(engine, "busy place", null, null);

        TCatalogEntryCreate(engine, "apple", [busy, quiet], [busy]);

        IReadOnlyList<LCatalogSituation> read =
            engine.TEngineSituationFind(string.Empty, LCatalogOrder.LCatalogOrderUsage);

        Assert.Equal(
            ["busy place", "quiet place"],
            read.Select(row => row.LCatalogSituationStored.LSituationTitle.TStateValueShow()));
        Assert.Equal([2, 1], read.Select(row => row.LCatalogSituationUsage));
    }

    [Fact]
    public void SituationFind_TypedQuery_ReturnsTitleDescriptionOrKind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogSituationCreate(engine, "at the water", null, null);
        TCatalogSituationCreate(engine, "by the shore", "spoken near water", null);
        TCatalogSituationCreate(engine, "in court", null, "watermark");
        TCatalogSituationCreate(engine, "at home", null, null);

        Assert.Equal(
            ["at the water", "by the shore", "in court"],
            engine.TEngineSituationFind("water", LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogSituationStored.LSituationTitle.TStateValueShow()));
    }

    private static LSituation TCatalogSituationCreate(
        LEngine engine,
        string title,
        string? description,
        string? kind)
    {
        return engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, title, description, kind));
    }

    private static LEntry TCatalogEntryCreate(
        LEngine engine, string headword, IReadOnlyList<LSituation> meaning, IReadOnlyList<LSituation> collocation)
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
                TCatalogDraftRead(meaning),
                [],
                [],
                [],
                1)],
            [TInterface.TCardDraftCreate(
                string.Empty,
                "an expression",
                "a meaning",
                [],
                TCatalogDraftRead(collocation),
                [],
                [],
                [],
                1)]));
    }

    private static List<LSituationDraft> TCatalogDraftRead(IReadOnlyList<LSituation> situations)
    {
        return [.. situations.Select(row => TInterface.TSituationDraftCreate(row.LSituationTitle, row.LSituationId))];
    }
}
