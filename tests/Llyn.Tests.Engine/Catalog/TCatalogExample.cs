using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogExample
{
    [Fact]
    public void ExampleFind_TextOrder_ReturnsAlphabetical()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogExampleCreate(engine, "stone falls", "English", null);
        TCatalogExampleCreate(engine, "apple falls", "English", null);

        Assert.Equal(
            ["apple falls", "stone falls"],
            engine.TEngineExampleFind(string.Empty, LCatalogOrder.LCatalogOrderText)
                .Select(row => row.LCatalogExampleStored.LExampleText.TStateValueShow()));
    }

    [Fact]
    public void ExampleFind_LanguageOrder_ReturnsLanguageGrouped()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogExampleCreate(engine, "apple falls", "Korean", null);
        TCatalogExampleCreate(engine, "stone falls", "English", null);

        Assert.Equal(
            ["English", "Korean"],
            engine.TEngineExampleFind(string.Empty, LCatalogOrder.LCatalogOrderLanguage)
                .Select(row => row.LCatalogExampleStored.LExampleLanguage));
    }

    [Fact]
    public void ExampleFind_SourceOrder_ReturnsCitedNameOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference zebra = TCatalogSourceCreate(engine, "Zebra Book");
        LReference anchor = TCatalogSourceCreate(engine, "Anchor Book");

        TCatalogExampleCreate(engine, "stone falls", "English", zebra.LReferenceId);
        TCatalogExampleCreate(engine, "apple falls", "English", anchor.LReferenceId);

        Assert.Equal(
            ["Anchor Book", "Zebra Book"],
            engine.TEngineExampleFind(string.Empty, LCatalogOrder.LCatalogOrderSource)
                .Select(row => row.LCatalogExampleSource));
    }

    [Fact]
    public void ExampleFind_UsageOrder_ReturnsMostCitedFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LExample quiet = TCatalogExampleCreate(engine, "quiet line", "English", null);
        LExample busy = TCatalogExampleCreate(engine, "busy line", "English", null);

        LEntry apple = TCatalogEntryCreate(engine, "apple");
        LEntry stone = TCatalogEntryCreate(engine, "stone");

        engine.TRequestQuoteApply(apple.LEntryId, busy.LExampleId);
        engine.TRequestQuoteApply(stone.LEntryId, busy.LExampleId);
        engine.TRequestQuoteApply(apple.LEntryId, quiet.LExampleId);


        IReadOnlyList<LCatalogExample> read =
            engine.TEngineExampleFind(string.Empty, LCatalogOrder.LCatalogOrderUsage);

        Assert.Equal(
            ["busy line", "quiet line"],
            read.Select(row => row.LCatalogExampleStored.LExampleText.TStateValueShow()));
        Assert.Equal([2, 1], read.Select(row => row.LCatalogExampleUsage));
        Assert.Equal(["2", "1"], read.Select(row => row.LCatalogExampleCount));
    }

    [Fact]
    public void ExampleCount_CopyWithOtherUsage_ReturnsUpdatedCount()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogExampleCreate(engine, "stone falls", "English", null);
        LCatalogExample row = Assert.Single(
            engine.TEngineExampleFind(string.Empty, LCatalogOrder.LCatalogOrderText));

        Assert.Equal("7", (row with { LCatalogExampleUsage = 7 }).LCatalogExampleCount);
    }

    [Fact]
    public void ExampleFind_TypedQuery_ReturnsTextTranslationOrSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference cited = TCatalogSourceCreate(engine, "Water Rites");

        TCatalogExampleCreate(engine, "watered stone", "English", null);
        TCatalogExampleCreate(engine, "bare stone", "English", null, "the water is cold");
        TCatalogExampleCreate(engine, "clear line", "English", cited.LReferenceId);
        TCatalogExampleCreate(engine, "dry stone", "English", null);

        Assert.Equal(
            ["bare stone", "clear line", "watered stone"],
            engine.TEngineExampleFind("water", LCatalogOrder.LCatalogOrderText)
                .Select(row => row.LCatalogExampleStored.LExampleText.TStateValueShow()));
    }

    [Fact]
    public void CitationRead_StoredSources_ReturnsDerivedNames()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference stored = TCatalogSourceCreate(engine, "Anchor Book");

        Assert.Equal("Anchor Book", engine.TEngineCitationRead()[stored.LReferenceId]);
    }

    [Fact]
    public void CitationRead_ShownEntry_WritesEveryCitedSourceItsLine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference stored = TCatalogSourceCreate(engine, "Anchor Book");
        LCardDraft child = TCatalogCardCreate(TCatalogSentenceCreate(999));
        LCardDraft meaning = TCatalogCardCreate(
                TCatalogSentenceCreate(stored.LReferenceId), TCatalogSentenceCreate(null))
            .TCardChildSet(child);
        LCardDraft collocation = TCatalogCardCreate(TCatalogSentenceCreate(stored.LReferenceId));
        LEntryDraft shown = TInterface.TEntryDraftCreate(
            "stone", "English", string.Empty, string.Empty, [meaning], [collocation]);

        IReadOnlyDictionary<long, string> lines = engine.TEngineCitationRead(shown);

        Assert.Equal([stored.LReferenceId, 999L], shown.TEntryCitationRead().Order());
        Assert.Equal(2, lines.Count);
        Assert.Equal("Anchor Book", lines[stored.LReferenceId]);
        Assert.Equal("999", lines[999]);
    }

    private static LCardDraft TCatalogCardCreate(params LSentenceDraft[] sentences) =>
        TInterface.TCardDraftCreate(
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            sentences,
            [],
            [],
            [],
            [],
            0);

    private static LSentenceDraft TCatalogSentenceCreate(long? source) =>
        TInterface.TSentenceDraftCreate(TInterface.TStateValueCreate("a line"), 0, TInterface.TStateAnchorRead(source));

    private static LExample TCatalogExampleCreate(
        LEngine engine,
        string text,
        string language,
        long? source,
        string? translation = null)
    {
        return engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0,
            language,
            text,
            translation,
            TInterface.TStateAnchorRead(source)));
    }

    private static LReference TCatalogSourceCreate(LEngine engine, string title)
    {
        string? unstated = null;

        return engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            title,
            unstated,
            LReferenceKind.LReferenceKindUnspecified,
            unstated,
            unstated,
            LStateMark.LStateMarkUnspecified));
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
