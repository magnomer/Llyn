using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TReference
{
    [Fact]
    public void ReferenceDelete_CitedReference_RefusesUnlessDetaching()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference reference = TReferenceCreate(engine, "A Dictionary");
        LExample example = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", "he said the word", null, TInterface.TStateAnchorRead(reference.LReferenceId)));

        Assert.Throws<InvalidOperationException>(() =>
            engine.TEngineReferenceDelete(reference.LReferenceId, false));

        engine.TEngineReferenceDelete(reference.LReferenceId, true);
        Assert.Null(engine.TEngineReferenceRead(reference.LReferenceId));
        Assert.NotNull(engine.TEngineExampleRead(example.LExampleId));
    }

    [Fact]
    public void AuthorDelete_CreditedAuthor_RefusesUnlessDetaching()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference reference = TReferenceCreate(engine, "A Dictionary");
        LAuthor first = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));
        LAuthor second = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Lee"));

        engine.TRequestCreditApply(reference.LReferenceId, first.LAuthorId, 0);
        engine.TRequestCreditApply(reference.LReferenceId, second.LAuthorId, 1);

        Assert.Equal(
            ["Kim", "Lee"],
            engine.TEngineAuthorRead(reference.LReferenceId, LOwner.LOwnerReference)
                .Select(author => author.LAuthorName));

        engine.TEngineAuthorUpdate(first with { LAuthorName = "Kim Minji" });
        Assert.Equal(
            "Kim Minji",
            engine.TEngineAuthorRead(reference.LReferenceId, LOwner.LOwnerReference)[0].LAuthorName);

        Assert.Throws<InvalidOperationException>(() => engine.TEngineAuthorDelete(first.LAuthorId, false));
        Assert.NotNull(engine.TEngineAuthorRead(first.LAuthorId));

        engine.TEngineAuthorDelete(first.LAuthorId, true);
        Assert.Null(engine.TEngineAuthorRead(first.LAuthorId));
        Assert.Equal(
            ["Lee"],
            engine.TEngineAuthorRead(reference.LReferenceId, LOwner.LOwnerReference)
                .Select(author => author.LAuthorName));

        engine.TEngineReferenceDelete(reference.LReferenceId, true);
        Assert.NotNull(engine.TEngineAuthorRead(second.LAuthorId));
    }

    private static LReference TReferenceCreate(LEngine engine, string title)
    {
        return engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate(title),
            TInterface.TStateValueCreate("1998"),
            LReferenceKind.LReferenceKindUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));
    }

    [Fact]
    public void ReferenceUsageRead_CitedThroughExamples_NamesEveryCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference dictionary = TReferenceCreate(engine, "A Dictionary");

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    string.Empty, string.Empty, "a meaning",
                    [
                        TInterface.TSentenceDraftCreate(
                            "he said the word", 0, TInterface.TStateAnchorRead(dictionary.LReferenceId)),
                        TInterface.TSentenceDraftCreate(
                            "not a word was spoken", 0, TInterface.TStateAnchorRead(dictionary.LReferenceId)),
                    ],
                    [], [], [], [], 1),
            ],
            [
                TInterface.TCardDraftCreate(
                    string.Empty, "in a word", "briefly",
                    [
                        TInterface.TSentenceDraftCreate(
                            "in a word, no", 0, TInterface.TStateAnchorRead(dictionary.LReferenceId)),
                    ],
                    [], [], [], [], 1),
            ]));

        IReadOnlyList<LUsage> usages =
            engine.TEngineUsageRead(dictionary.LReferenceId, LOwner.LOwnerReference);

        Assert.Equal(
            [
                LOwner.LOwnerMeaning,
                LOwner.LOwnerCollocation,
                LOwner.LOwnerExample,
                LOwner.LOwnerExample,
                LOwner.LOwnerExample,
            ],
            usages.Select(row => row.LUsageOwner));

        foreach (LUsage card in usages.Where(row => row.LUsageOwner != LOwner.LOwnerExample))
        {
            Assert.Equal(entry.LEntryId, card.LUsageEntry);
            Assert.Equal("word", card.LUsageHeadword);
        }

        Assert.Equal(
            ["a meaning", "in a word"],
            usages.Where(row => row.LUsageOwner != LOwner.LOwnerExample)
                .Select(row => row.LUsageTitle.TStateValueShow()));

        Assert.Equal(3, engine.TEngineUsageRead(LOwner.LOwnerReference)[dictionary.LReferenceId]);
    }

    private static LEntry TReferenceEntryCreate(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], [], [], 1)]));
    }

    [Fact]
    public void AuthorUsageRead_CreditedSources_NamesEveryEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference dictionary = TReferenceCreate(engine, "A Dictionary");
        LReference grammar = TReferenceCreate(engine, "A Grammar");

        LAuthor kim = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));
        LAuthor lee = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Lee"));
        engine.TRequestCreditApply(dictionary.LReferenceId, kim.LAuthorId, 0);
        engine.TRequestCreditApply(grammar.LReferenceId, kim.LAuthorId, 0);
        engine.TRequestCreditApply(grammar.LReferenceId, lee.LAuthorId, 1);

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    string.Empty, string.Empty, "a meaning",
                    [
                        TInterface.TSentenceDraftCreate(
                            "he said the word", 0, TInterface.TStateAnchorRead(dictionary.LReferenceId)),
                        TInterface.TSentenceDraftCreate(
                            "not a word was spoken", 0, TInterface.TStateAnchorRead(dictionary.LReferenceId)),
                    ],
                    [], [], [], [], 1),
            ],
            [
                TInterface.TCardDraftCreate(
                    string.Empty, "in a word", "briefly",
                    [
                        TInterface.TSentenceDraftCreate(
                            "in a word, no", 0, TInterface.TStateAnchorRead(grammar.LReferenceId)),
                    ],
                    [], [], [], [], 1),
            ]));

        IReadOnlyList<LUsage> credited = engine.TEngineUsageRead(kim.LAuthorId, LOwner.LOwnerAuthor);

        Assert.Equal(
            [
                LOwner.LOwnerMeaning,
                LOwner.LOwnerCollocation,
                LOwner.LOwnerExample,
                LOwner.LOwnerExample,
                LOwner.LOwnerExample,
            ],
            credited.Select(row => row.LUsageOwner));

        foreach (LUsage card in credited.Where(row => row.LUsageOwner != LOwner.LOwnerExample))
        {
            Assert.Equal(entry.LEntryId, card.LUsageEntry);
            Assert.Equal("word", card.LUsageHeadword);
        }

        Assert.Equal(
            ["a meaning", "in a word"],
            credited.Where(row => row.LUsageOwner != LOwner.LOwnerExample)
                .Select(row => row.LUsageTitle.TStateValueShow()));

        Assert.Equal(
            [LOwner.LOwnerCollocation, LOwner.LOwnerExample],
            engine.TEngineUsageRead(lee.LAuthorId, LOwner.LOwnerAuthor).Select(row => row.LUsageOwner));
    }

    [Fact]
    public void AuthorUsageRead_UncitedCredit_ReturnsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference dictionary = TReferenceCreate(engine, "A Dictionary");
        LAuthor credited = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));
        LAuthor uncredited = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Lee"));
        engine.TRequestCreditApply(dictionary.LReferenceId, credited.LAuthorId, 0);

        Assert.Empty(engine.TEngineUsageRead(credited.LAuthorId, LOwner.LOwnerAuthor));
        Assert.Empty(engine.TEngineUsageRead(uncredited.LAuthorId, LOwner.LOwnerAuthor));
    }

    [Fact]
    public void ReferenceStart_BrokenAuthorStateWord_ReadsUnreadableAndRefusesCommit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference stored = TReferenceCreate(engine, "A Dictionary");
        workspace.TWorkspaceScriptRun("UPDATE reference SET author_state = 'broken';");

        LDraft draft = engine.TEngineReferenceStart("test", stored.LReferenceId);
        LReference held = draft.LDraftReference!;
        Assert.True(held.LReferenceAuthorState.LStateMarkUnreadable);
        Assert.Equal(LState.LStateUnspecified, held.LReferenceAuthorState.LStateMarkState);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineReferenceCommit(draft.LDraftId));
        Assert.Equal(LRefusal.LRefusalUnreadable, refusal.LRefusalReason);

        engine.TEngineDraftSweep(draft.LDraftId);
        engine.TEngineReferenceCommit(draft.LDraftId);
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reference WHERE author_state = 'unspecified';"));
    }
}
