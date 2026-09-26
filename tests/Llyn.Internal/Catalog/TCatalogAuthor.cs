using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogAuthor
{
    [Fact]
    public void AuthorFind_NameOrder_CountsWorksAndCitations()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference dictionary = TCatalogWorkCreate(engine, "A Dictionary", LReferenceKind.LReferenceKindBook);
        LReference grammar = TCatalogWorkCreate(engine, "A Grammar", LReferenceKind.LReferenceKindArticle);

        LAuthor kim = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));
        LAuthor lee = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Lee"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ahn"));
        engine.TRequestCreditApply(dictionary.LReferenceId, kim.LAuthorId, 0);
        engine.TRequestCreditApply(grammar.LReferenceId, kim.LAuthorId, 0);
        engine.TRequestCreditApply(grammar.LReferenceId, lee.LAuthorId, 1);

        TCatalogCitationCreate(engine, "he said the word", dictionary.LReferenceId);
        TCatalogCitationCreate(engine, "not a word was spoken", dictionary.LReferenceId);
        TCatalogCitationCreate(engine, "in a word, no", grammar.LReferenceId);

        IReadOnlyList<LCatalogAuthor> read = engine.TEngineAuthorFind(string.Empty, LCatalogOrder.LCatalogOrderName);

        Assert.Equal(["Ahn", "Kim", "Lee"], read.Select(row => row.LCatalogAuthorStored.LAuthorName));
        Assert.Equal([0, 2, 1], read.Select(row => row.LCatalogAuthorWork));
        Assert.Equal([0, 3, 1], read.Select(row => row.LCatalogAuthorUsage));
    }

    [Fact]
    public void AuthorFind_WorkAndUsageOrder_ReturnsBusiestFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference dictionary = TCatalogWorkCreate(engine, "A Dictionary", LReferenceKind.LReferenceKindBook);
        LReference grammar = TCatalogWorkCreate(engine, "A Grammar", LReferenceKind.LReferenceKindBook);

        LAuthor kim = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));
        LAuthor lee = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Lee"));
        engine.TRequestCreditApply(dictionary.LReferenceId, kim.LAuthorId, 0);
        engine.TRequestCreditApply(grammar.LReferenceId, kim.LAuthorId, 0);
        engine.TRequestCreditApply(grammar.LReferenceId, lee.LAuthorId, 1);

        TCatalogCitationCreate(engine, "in a word, no", grammar.LReferenceId);
        TCatalogCitationCreate(engine, "in a word, yes", grammar.LReferenceId);

        Assert.Equal(
            ["Kim", "Lee"],
            engine.TEngineAuthorFind(string.Empty, LCatalogOrder.LCatalogOrderWork)
                .Select(row => row.LCatalogAuthorStored.LAuthorName));
        Assert.Equal(
            ["Kim", "Lee"],
            engine.TEngineAuthorFind(string.Empty, LCatalogOrder.LCatalogOrderUsage)
                .Select(row => row.LCatalogAuthorStored.LAuthorName));
        Assert.Equal(
            ["Lee", "Kim"],
            engine.TEngineAuthorFind(string.Empty, LCatalogOrder.LCatalogOrderReverse)
                .Select(row => row.LCatalogAuthorStored.LAuthorName));
    }

    [Fact]
    public void AuthorFind_TypedQuery_ReturnsAuthorsNamingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim Minji"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Lee Kimball"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Park"));

        Assert.Equal(
            ["Kim Minji", "Lee Kimball"],
            engine.TEngineAuthorFind("kim", LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogAuthorStored.LAuthorName));
    }

    [Fact]
    public void OeuvreFind_ChosenAuthor_ReturnsSourcesCreditingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference dictionary = TCatalogWorkCreate(engine, "A Dictionary", LReferenceKind.LReferenceKindBook);
        LReference grammar = TCatalogWorkCreate(engine, "A Grammar", LReferenceKind.LReferenceKindArticle);
        TCatalogWorkCreate(engine, "Nobody's Notes", LReferenceKind.LReferenceKindWeb);

        LAuthor kim = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));
        LAuthor lee = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Lee"));
        engine.TRequestCreditApply(dictionary.LReferenceId, kim.LAuthorId, 0);
        engine.TRequestCreditApply(grammar.LReferenceId, kim.LAuthorId, 0);
        engine.TRequestCreditApply(grammar.LReferenceId, lee.LAuthorId, 1);

        Assert.Equal(
            ["A Dictionary", "A Grammar"],
            engine.TEngineOeuvreFind(
                kim.LAuthorId, string.Empty, LCatalogFilter.LCatalogFilterEmpty, LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogReferenceName));
        Assert.Equal(
            ["A Grammar"],
            engine.TEngineOeuvreFind(
                lee.LAuthorId, string.Empty, LCatalogFilter.LCatalogFilterEmpty, LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogReferenceName));
        Assert.Equal(
            ["Nobody's Notes", "Unknown"],
            engine.TEngineOeuvreFind(
                0, string.Empty, LCatalogFilter.LCatalogFilterEmpty, LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogReferenceName));
        Assert.Equal(
            ["A Dictionary", "A Grammar", "Nobody's Notes", "Unknown"],
            engine.TEngineOeuvreFind(
                null, string.Empty, LCatalogFilter.LCatalogFilterEmpty, LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogReferenceName));
    }

    [Fact]
    public void OeuvreFind_KindFilterAndQuery_NarrowTheSources()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference dictionary = TCatalogWorkCreate(engine, "A Dictionary", LReferenceKind.LReferenceKindBook);
        LReference grammar = TCatalogWorkCreate(engine, "A Grammar", LReferenceKind.LReferenceKindArticle);
        LReference glossary = TCatalogWorkCreate(engine, "A Glossary", LReferenceKind.LReferenceKindBook);

        LAuthor kim = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));
        engine.TRequestCreditApply(dictionary.LReferenceId, kim.LAuthorId, 0);
        engine.TRequestCreditApply(grammar.LReferenceId, kim.LAuthorId, 0);
        engine.TRequestCreditApply(glossary.LReferenceId, kim.LAuthorId, 0);

        Assert.Equal(
            ["A Dictionary", "A Glossary"],
            engine.TEngineOeuvreFind(
                kim.LAuthorId,
                string.Empty,
                TInterface.TCatalogFilterCreate("article"),
                LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogReferenceName));
        Assert.Equal(
            ["A Glossary"],
            engine.TEngineOeuvreFind(
                kim.LAuthorId, "glos", LCatalogFilter.LCatalogFilterEmpty, LCatalogOrder.LCatalogOrderName)
                .Select(row => row.LCatalogReferenceName));
    }

    private static LReference TCatalogWorkCreate(LEngine engine, string title, LReferenceKind kind)
    {
        return engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate(title),
            LStateValue.LStateValueUnspecified,
            kind,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));
    }

    private static void TCatalogCitationCreate(LEngine engine, string text, long referenceId)
    {
        engine.TEngineExampleCreate(
            TInterface.TExampleCreate(
            0, "English", text, null, TInterface.TStateAnchorRead(referenceId)));
    }
}
