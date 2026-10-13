using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TFold
{
    [Fact]
    public void FoldSave_StoredMeaning_AnswersFolded()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryDraft entry = TFoldEntryCreate(engine, "word", out long entryId);
        long meaning = entry.LEntryDraftMeanings[0].LCardDraftId;

        engine.TEngineFoldSave(entryId, meaning);

        Assert.Contains(meaning, engine.TEngineFoldRead(entryId));
        Assert.DoesNotContain(entry.LEntryDraftMeanings[1].LCardDraftId, engine.TEngineFoldRead(entryId));
    }

    [Fact]
    public void FoldSave_StoredCollocation_AnswersFolded()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryDraft entry = TFoldEntryCreate(engine, "word", out long entryId);
        long collocation = entry.LEntryDraftCollocations[0].LCardDraftId;

        engine.TEngineFoldSave(entryId, collocation);

        Assert.Equal([collocation], engine.TEngineFoldRead(entryId));
    }

    [Fact]
    public void FoldDelete_FoldedCards_AnswersUnfolded()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryDraft entry = TFoldEntryCreate(engine, "word", out long entryId);
        long meaning = entry.LEntryDraftMeanings[0].LCardDraftId;
        long collocation = entry.LEntryDraftCollocations[0].LCardDraftId;
        engine.TEngineFoldSave(entryId, meaning);
        engine.TEngineFoldSave(entryId, collocation);

        engine.TEngineFoldDelete(entryId, meaning);
        engine.TEngineFoldDelete(entryId, collocation);

        Assert.Empty(engine.TEngineFoldRead(entryId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_fold;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_fold;"));
    }

    [Fact]
    public void FoldRead_TwoEntriesFolded_ReturnsOnlyEachEntrysOwnFoldedCards()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryDraft first = TFoldEntryCreate(engine, "water", out long firstId);
        LEntryDraft second = TFoldEntryCreate(engine, "stone", out long secondId);
        long firstMeaning = first.LEntryDraftMeanings[1].LCardDraftId;
        long firstCollocation = first.LEntryDraftCollocations[0].LCardDraftId;
        long secondMeaning = second.LEntryDraftMeanings[0].LCardDraftId;
        engine.TEngineFoldSave(firstId, firstMeaning);
        engine.TEngineFoldSave(firstId, firstCollocation);
        engine.TEngineFoldSave(secondId, secondMeaning);

        engine.TEngineFoldDelete(secondId, secondMeaning);

        Assert.Equal(
            new[] { firstMeaning, firstCollocation }.Order(),
            engine.TEngineFoldRead(firstId).Order());
        Assert.Empty(engine.TEngineFoldRead(secondId));
    }

    [Fact]
    public void EntryUpdate_DroppedFoldedMeaning_DeletesItsFoldAndKeepsTheOthers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryDraft entry = TFoldEntryCreate(engine, "word", out long entryId);
        long kept = entry.LEntryDraftMeanings[0].LCardDraftId;
        long dropped = entry.LEntryDraftMeanings[1].LCardDraftId;
        engine.TEngineFoldSave(entryId, kept);
        engine.TEngineFoldSave(entryId, dropped);

        engine.TEngineEntryUpdate(entryId, entry with { LEntryDraftMeanings = [entry.LEntryDraftMeanings[0]] });

        Assert.Equal([kept], engine.TEngineFoldRead(entryId));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_fold;"));
    }

    [Fact]
    public void EntryDelete_FoldedEntry_DropsItsCardFolds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryDraft entry = TFoldEntryCreate(engine, "word", out long entryId);
        long meaning = entry.LEntryDraftMeanings[0].LCardDraftId;
        engine.TEngineFoldSave(entryId, meaning);

        engine.TEngineEntryDelete(entryId);

        Assert.Empty(engine.TEngineFoldRead(entryId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_fold;"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void FoldSave_IdAtOrBelowZero_WritesNothingAndThrowsNothing(long cardId)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFoldEntryCreate(engine, "word", out long entryId);

        engine.TEngineFoldSave(entryId, cardId);
        engine.TEngineFoldDelete(entryId, cardId);

        Assert.Empty(engine.TEngineFoldRead(entryId));
        Assert.Empty(engine.TEngineFoldRead(cardId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_fold;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_fold;"));
    }

    [Fact]
    public void FoldSave_StoredCard_RaisesFoldBulletinOnItsEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryDraft entry = TFoldEntryCreate(engine, "word", out long entryId);
        long meaning = entry.LEntryDraftMeanings[0].LCardDraftId;
        List<long> raised = [];
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectFold)
            {
                raised.Add(bulletin.LBulletinId);
            }
        });

        engine.TEngineFoldSave(entryId, meaning);
        engine.TEngineFoldDelete(entryId, meaning);

        Assert.Equal([entryId, entryId], raised);
    }

    [Fact]
    public void ReflexSpread_OpenedEntry_AnswersOpened()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFoldEntryCreate(engine, "word", out long entryId);

        engine.TEngineReflexSpread(entryId, true);

        Assert.True(engine.TEngineSpreadCheck(entryId));
    }

    [Fact]
    public void ReflexSpread_ClosedAfterOpened_AnswersClosedAndKeepsNoRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFoldEntryCreate(engine, "word", out long entryId);
        engine.TEngineReflexSpread(entryId, true);

        engine.TEngineReflexSpread(entryId, false);

        Assert.False(engine.TEngineSpreadCheck(entryId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reflex_fold;"));
    }

    [Fact]
    public void ReflexSpread_TwoEntries_KeepsEachEntrysOwnState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFoldEntryCreate(engine, "water", out long firstId);
        TFoldEntryCreate(engine, "stone", out long secondId);

        engine.TEngineReflexSpread(firstId, true);

        Assert.True(engine.TEngineSpreadCheck(firstId));
        Assert.False(engine.TEngineSpreadCheck(secondId));

        engine.TEngineReflexSpread(secondId, true);
        engine.TEngineReflexSpread(firstId, false);

        Assert.False(engine.TEngineSpreadCheck(firstId));
        Assert.True(engine.TEngineSpreadCheck(secondId));
    }

    [Fact]
    public void EntryDelete_SpreadEntry_DropsItsReflexFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFoldEntryCreate(engine, "word", out long entryId);
        engine.TEngineReflexSpread(entryId, true);

        engine.TEngineEntryDelete(entryId);

        Assert.False(engine.TEngineSpreadCheck(entryId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reflex_fold;"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ReflexSpread_IdAtOrBelowZero_WritesNothingAndThrowsNothing(long entryId)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFoldEntryCreate(engine, "word", out _);

        engine.TEngineReflexSpread(entryId, true);

        Assert.False(engine.TEngineSpreadCheck(entryId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reflex_fold;"));

        engine.TEngineReflexSpread(entryId, false);

        Assert.False(engine.TEngineSpreadCheck(entryId));
    }

    [Fact]
    public void ReflexSpread_StoredEntry_RaisesFoldBulletinOnItsEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFoldEntryCreate(engine, "word", out long entryId);
        List<long> raised = [];
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectFold)
            {
                raised.Add(bulletin.LBulletinId);
            }
        });

        engine.TEngineReflexSpread(entryId, true);
        engine.TEngineReflexSpread(entryId, false);

        Assert.Equal([entryId, entryId], raised);
    }

    internal static LEntryDraft TFoldEntryCreate(LEngine engine, string headword, out long entryId)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "another meaning", [], [], [], [], [], 2),
            ],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], [], [], 1)]));
        entryId = entry.LEntryId;
        LEntryDraft? loaded = engine.TEngineEntryLoad(entryId);
        Assert.NotNull(loaded);
        return loaded;
    }
}
