using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TReference
{
    [Fact]
    public void ReferenceAttach_ExampleCitation_LimitsToOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TReferenceEntryCreate(engine);
        LReference dictionary = TReferenceCreate(engine, "A Dictionary");
        LReference grammar = TReferenceCreate(engine, "A Grammar");

        engine.TEngineReferenceAttach(entry.LEntryId, dictionary.LReferenceId, 0, LOwner.LOwnerEntry);
        engine.TEngineReferenceAttach(entry.LEntryId, grammar.LReferenceId, 0, LOwner.LOwnerEntry);

        Assert.Equal(
            [grammar.LReferenceId, dictionary.LReferenceId],
            engine.TEngineReferenceRead(entry.LEntryId, LOwner.LOwnerEntry)
                .Select(row => row.LReferenceId));

        LExample example = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(string.Empty, "English", "he said the word", null, null));
        engine.TEngineReferenceAttach(
            example.LExampleId, dictionary.LReferenceId, 0, LOwner.LOwnerExample);
        engine.TEngineReferenceAttach(example.LExampleId, grammar.LReferenceId, 0, LOwner.LOwnerExample);

        Assert.Equal(
            grammar.LReferenceId,
            Assert.Single(engine.TEngineReferenceRead(example.LExampleId, LOwner.LOwnerExample))
                .LReferenceId);

        engine.TEngineReferenceDetach(example.LExampleId, grammar.LReferenceId, LOwner.LOwnerExample);
        Assert.Empty(engine.TEngineReferenceRead(example.LExampleId, LOwner.LOwnerExample));
    }

    [Fact]
    public void ReferenceDelete_CitedReference_RefusesUntilDetached()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TReferenceEntryCreate(engine);
        LReference reference = TReferenceCreate(engine, "A Dictionary");
        engine.TEngineReferenceAttach(entry.LEntryId, reference.LReferenceId, 0, LOwner.LOwnerEntry);

        Assert.Throws<InvalidOperationException>(() =>
            engine.TEngineReferenceDelete(reference.LReferenceId));

        engine.TEngineReferenceDetach(entry.LEntryId, reference.LReferenceId, LOwner.LOwnerEntry);
        engine.TEngineReferenceDelete(reference.LReferenceId);
        Assert.Null(engine.TEngineReferenceRead(reference.LReferenceId));
    }

    [Fact]
    public void AuthorDelete_CreditedAuthor_RefusesUntilDetached()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference reference = TReferenceCreate(engine, "A Dictionary");
        LAuthor first = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(string.Empty, "Kim"));
        LAuthor second = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(string.Empty, "Lee"));

        engine.TEngineAuthorAttach(reference.LReferenceId, first.LAuthorId, 0);
        engine.TEngineAuthorAttach(reference.LReferenceId, second.LAuthorId, 1);

        Assert.Equal(
            ["Kim", "Lee"],
            engine.TEngineAuthorRead(reference.LReferenceId, LOwner.LOwnerReference)
                .Select(author => author.LAuthorName));

        engine.TEngineAuthorUpdate(first with { LAuthorName = "Kim Minji" });
        Assert.Equal(
            "Kim Minji",
            engine.TEngineAuthorRead(reference.LReferenceId, LOwner.LOwnerReference)[0].LAuthorName);

        Assert.Throws<InvalidOperationException>(() => engine.TEngineAuthorDelete(first.LAuthorId));
        engine.TEngineAuthorDetach(reference.LReferenceId, first.LAuthorId);
        Assert.NotNull(engine.TEngineAuthorRead(first.LAuthorId));

        engine.TEngineAuthorDelete(first.LAuthorId);
        Assert.Null(engine.TEngineAuthorRead(first.LAuthorId));

        engine.TEngineReferenceDelete(reference.LReferenceId);
        Assert.NotNull(engine.TEngineAuthorRead(second.LAuthorId));
    }

    private static LReference TReferenceCreate(LEngine engine, string title)
    {
        return engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            string.Empty,
            TInterface.TStateValueCreate(title),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate("1998"),
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified));
    }

    private static LEntry TReferenceEntryCreate(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1)]));
    }
}
