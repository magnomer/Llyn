using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogTag
{
    [Fact]
    public void TagFind_NameOrder_ReturnsAlphabetical()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogTagSave(engine, ["stone", "apple", "Mist"]);

        Assert.Equal(
            ["apple", "Mist", "stone"],
            engine.TEngineTagFind(string.Empty, LCatalogOrder.LCatalogOrderName)
                .Select(tag => tag.LTagText));
    }

    [Fact]
    public void TagFind_ReverseOrder_ReturnsReverseAlphabetical()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogTagSave(engine, ["stone", "apple", "Mist"]);

        Assert.Equal(
            ["stone", "Mist", "apple"],
            engine.TEngineTagFind(string.Empty, LCatalogOrder.LCatalogOrderReverse)
                .Select(tag => tag.LTagText));
    }

    [Fact]
    public void TagFind_TypedQuery_ReturnsMatchingTags()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogTagSave(engine, ["water", "watermark", "stone"]);

        Assert.Equal(
            ["water", "watermark"],
            engine.TEngineTagFind("WAT", LCatalogOrder.LCatalogOrderName)
                .Select(tag => tag.LTagText));
    }

    private static void TCatalogTagSave(LEngine engine, IReadOnlyList<string> texts)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "apple",
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

        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        engine.TEngineTagSave(
            meaningId,
            [.. texts.Select(TInterface.TTagCreate)],
            LOwner.LOwnerMeaning);
    }
}
