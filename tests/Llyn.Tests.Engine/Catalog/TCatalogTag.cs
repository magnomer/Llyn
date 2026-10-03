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

    [Fact]
    public void CatalogMarkFind_WordInsideText_SplitsAroundTheMatchIgnoringCase()
    {
        Assert.Equal(("phrasal ", "Verb", " form"), TInterface.TCatalogMarkFind("phrasal Verb form", "verb"));
        Assert.Equal(("animal", string.Empty, string.Empty), TInterface.TCatalogMarkFind("animal", "plant"));
        Assert.Equal(("animal", string.Empty, string.Empty), TInterface.TCatalogMarkFind("animal", string.Empty));
    }

    [Fact]
    public void CatalogUsageFormat_UnusedOrUsedRow_ShowsNothingOrTheCount()
    {
        Assert.Equal(string.Empty, TInterface.TCatalogUsageFormat(0));
        Assert.Equal("1", TInterface.TCatalogUsageFormat(1));
        Assert.Equal("12", TInterface.TCatalogUsageFormat(12));
    }

    [Fact]
    public void CatalogTallyFormat_EachCountAndRealm_WordsTheRealmsForm()
    {
        Assert.Equal("text:Source.UsageNone", TInterface.TCatalogTallyFormat(0, "Source"));
        Assert.Equal("text:Example.UsageOne", TInterface.TCatalogTallyFormat(1, "Example"));
        Assert.Equal("3 text:Situation.UsageMany", TInterface.TCatalogTallyFormat(3, "Situation"));
    }

    private static void TCatalogTagSave(LEngine engine, IReadOnlyList<string> texts)
    {
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
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
                texts,
                [],
                1)],
            []));
    }

}
