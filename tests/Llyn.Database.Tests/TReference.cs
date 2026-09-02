using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TReference
{
    [Fact]
    public void AnEntryCitesReferencesInOrderAndAnExampleCitesAtMostOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TReferenceEntryCreate(engine);
        LReference dictionary = TReferenceCreate(engine, "A Dictionary");
        LReference grammar = TReferenceCreate(engine, "A Grammar");

        engine.LEngineReferenceAttach(entry.LEntryId, dictionary.LReferenceId, 0, LOwner.LOwnerEntry);
        engine.LEngineReferenceAttach(entry.LEntryId, grammar.LReferenceId, 0, LOwner.LOwnerEntry);

        Assert.Equal(
            [grammar.LReferenceId, dictionary.LReferenceId],
            engine.LEngineReferenceRead(entry.LEntryId, LOwner.LOwnerEntry)
                .Select(row => row.LReferenceId));

        LExample example = engine.LEngineExampleCreate(
            new LExample(string.Empty, "English", "he said the word", null, null, []));
        engine.LEngineReferenceAttach(
            example.LExampleId, dictionary.LReferenceId, 0, LOwner.LOwnerExample);
        engine.LEngineReferenceAttach(example.LExampleId, grammar.LReferenceId, 0, LOwner.LOwnerExample);

        Assert.Equal(
            grammar.LReferenceId,
            Assert.Single(engine.LEngineReferenceRead(example.LExampleId, LOwner.LOwnerExample))
                .LReferenceId);

        engine.LEngineReferenceDetach(example.LExampleId, grammar.LReferenceId, LOwner.LOwnerExample);
        Assert.Empty(engine.LEngineReferenceRead(example.LExampleId, LOwner.LOwnerExample));
    }

    [Fact]
    public void ASituationCitesAtMostOneReferenceAndHoldsTheSourceFromBeingDeleted()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LReference dictionary = TReferenceCreate(engine, "A Dictionary");
        LReference grammar = TReferenceCreate(engine, "A Grammar");

        LSituation situation = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "in court", null, null, null));

        engine.LEngineReferenceAttach(
            situation.LSituationId, dictionary.LReferenceId, 0, LOwner.LOwnerSituation);
        engine.LEngineReferenceAttach(
            situation.LSituationId, grammar.LReferenceId, 0, LOwner.LOwnerSituation);

        Assert.Equal(
            grammar.LReferenceId,
            Assert.Single(engine.LEngineReferenceRead(situation.LSituationId, LOwner.LOwnerSituation))
                .LReferenceId);

        Assert.Throws<InvalidOperationException>(() =>
            engine.LEngineReferenceDelete(grammar.LReferenceId));

        engine.LEngineReferenceDetach(
            situation.LSituationId, grammar.LReferenceId, LOwner.LOwnerSituation);
        Assert.Empty(engine.LEngineReferenceRead(situation.LSituationId, LOwner.LOwnerSituation));

        engine.LEngineReferenceDelete(grammar.LReferenceId);
        Assert.Null(engine.LEngineReferenceRead(grammar.LReferenceId));
    }

    [Fact]
    public void ACitedReferenceIsNotDeletedUntilTheCitationsAreGone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TReferenceEntryCreate(engine);
        LReference reference = TReferenceCreate(engine, "A Dictionary");
        engine.LEngineReferenceAttach(entry.LEntryId, reference.LReferenceId, 0, LOwner.LOwnerEntry);

        Assert.Throws<InvalidOperationException>(() =>
            engine.LEngineReferenceDelete(reference.LReferenceId));

        engine.LEngineReferenceDetach(entry.LEntryId, reference.LReferenceId, LOwner.LOwnerEntry);
        engine.LEngineReferenceDelete(reference.LReferenceId);
        Assert.Null(engine.LEngineReferenceRead(reference.LReferenceId));
    }

    [Fact]
    public void AnAuthorIsCreditedOnReferencesAndOutlivesEveryCredit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LReference reference = TReferenceCreate(engine, "A Dictionary");
        LAuthor first = engine.LEngineAuthorCreate(new LAuthor(string.Empty, "Kim"));
        LAuthor second = engine.LEngineAuthorCreate(new LAuthor(string.Empty, "Lee"));

        engine.LEngineAuthorAttach(reference.LReferenceId, first.LAuthorId, 0);
        engine.LEngineAuthorAttach(reference.LReferenceId, second.LAuthorId, 1);

        Assert.Equal(
            ["Kim", "Lee"],
            engine.LEngineAuthorRead(reference.LReferenceId, LOwner.LOwnerReference)
                .Select(author => author.LAuthorName));

        engine.LEngineAuthorUpdate(first with { LAuthorName = "Kim Minji" });
        Assert.Equal(
            "Kim Minji",
            engine.LEngineAuthorRead(reference.LReferenceId, LOwner.LOwnerReference)[0].LAuthorName);

        Assert.Throws<InvalidOperationException>(() => engine.LEngineAuthorDelete(first.LAuthorId));
        engine.LEngineAuthorDetach(reference.LReferenceId, first.LAuthorId);
        Assert.NotNull(engine.LEngineAuthorRead(first.LAuthorId));

        engine.LEngineAuthorDelete(first.LAuthorId);
        Assert.Null(engine.LEngineAuthorRead(first.LAuthorId));

        engine.LEngineReferenceDelete(reference.LReferenceId);
        Assert.NotNull(engine.LEngineAuthorRead(second.LAuthorId));
    }

    private static LReference TReferenceCreate(LEngine engine, string title)
    {
        return engine.LEngineReferenceCreate(new LReference(
            string.Empty,
            LStateValue.LStateValueCreate(title),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueCreate("1998"),
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified));
    }

    private static LEntry TReferenceEntryCreate(LEngine engine)
    {
        return engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], string.Empty, [], [])],
            [new LCardDraft(string.Empty, "in a word", "briefly", [], [], string.Empty, [], [])]));
    }
}
