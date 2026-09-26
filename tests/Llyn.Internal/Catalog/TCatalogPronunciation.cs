using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogPronunciation
{
    [Fact]
    public void PronunciationFind_HeadwordOrder_ReturnsAlphabetical()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogPronunciationSave(engine, "stone", "stəʊn");
        TCatalogPronunciationSave(engine, "apple", "ˈæpəl");

        Assert.Equal(
            ["apple", "stone"],
            engine.TEnginePronunciationFind(string.Empty, LCatalogOrder.LCatalogOrderHeadword)
                .Select(row => row.LCatalogPronunciationEntry.LEntryHeadword));
    }

    [Fact]
    public void PronunciationFind_ReverseOrder_ReturnsReverseAlphabetical()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogPronunciationSave(engine, "apple", "ˈæpəl");
        TCatalogPronunciationSave(engine, "stone", "stəʊn");

        Assert.Equal(
            ["stone", "apple"],
            engine.TEnginePronunciationFind(string.Empty, LCatalogOrder.LCatalogOrderReverse)
                .Select(row => row.LCatalogPronunciationEntry.LEntryHeadword));
    }

    [Fact]
    public void PronunciationFind_SoundOrder_ReturnsWrittenSoundsFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogPronunciationSave(engine, "apple", null);
        TCatalogPronunciationSave(engine, "stone", "b");
        TCatalogPronunciationSave(engine, "cloud", "a");

        Assert.Equal(
            ["cloud", "stone", "apple"],
            engine.TEnginePronunciationFind(string.Empty, LCatalogOrder.LCatalogOrderSound)
                .Select(row => row.LCatalogPronunciationEntry.LEntryHeadword));
    }

    [Fact]
    public void PronunciationFind_PendingOrder_ReturnsUnwrittenSoundsFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogPronunciationSave(engine, "stone", "b");
        TCatalogPronunciationSave(engine, "cloud", null);
        TCatalogPronunciationSave(engine, "apple", null);

        Assert.Equal(
            ["apple", "cloud", "stone"],
            engine.TEnginePronunciationFind(string.Empty, LCatalogOrder.LCatalogOrderPending)
                .Select(row => row.LCatalogPronunciationEntry.LEntryHeadword));
    }

    [Fact]
    public void PronunciationFind_TypedQuery_ReturnsMatchingHeadwords()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogPronunciationSave(engine, "water", "ˈwɔːtə");
        TCatalogPronunciationSave(engine, "stone", "stəʊn");

        LCatalogPronunciation row = Assert.Single(
            engine.TEnginePronunciationFind("wat", LCatalogOrder.LCatalogOrderHeadword));
        Assert.Equal("water", row.LCatalogPronunciationEntry.LEntryHeadword);
        Assert.Equal("ˈwɔːtə", row.LCatalogPronunciationSound);
    }

    private static void TCatalogPronunciationSave(LEngine engine, string headword, string? sound)
    {
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            "English",
            sound ?? string.Empty,
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
