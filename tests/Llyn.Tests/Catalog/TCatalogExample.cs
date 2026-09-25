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
