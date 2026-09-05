using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryLoad
{
    [Fact]
    public void EntryLoad_SavedEntry_ReturnsSavedDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntryDraft word = TInterface.TEntryDraftCreate(
            "word",
            "English",
            "wɜːd",
            "a note",
            [
                TInterface.TCardDraftCreate(
                    string.Empty, string.Empty, "a unit of language",
                    [TInterface.TExampleDraftCreate("he said a word")], [TInterface.TSituationDraftCreate("conversation")], [], string.Empty, ["spoken"], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "a promise", [], [], [], string.Empty, [], [], 2),
            ],
            [
                TInterface.TCardDraftCreate(
                    string.Empty, "in a word", "briefly", [TInterface.TExampleDraftCreate("in a word, no")], [TInterface.TSituationDraftCreate("summary")], [], string.Empty,
                    ["written"], [], 1),
            ]);

        LEntryDraft sword = TInterface.TEntryDraftCreate(
            "sword",
            "English",
            "sɔːd",
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a bladed weapon", [], [], [], string.Empty, [], [], 1)],
            []);

        LEntry stored = engine.TEngineEntrySave(word);
        engine.TEngineEntrySave(sword);

        Assert.Equal(["sword", "word"], engine.TEngineEntryFind(string.Empty).Select(e => e.LEntryHeadword));

        Assert.Equal(["sword", "word"], engine.TEngineEntryFind("WORD").Select(e => e.LEntryHeadword));
        Assert.Equal("sword", Assert.Single(engine.TEngineEntryFind("swo")).LEntryHeadword);
        Assert.Empty(engine.TEngineEntryFind("axe"));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(word.LEntryDraftHeadword, loaded.LEntryDraftHeadword);
        Assert.Equal(word.LEntryDraftLanguage, loaded.LEntryDraftLanguage);
        Assert.Equal(word.LEntryDraftPronunciation, loaded.LEntryDraftPronunciation);
        Assert.Equal(word.LEntryDraftNote, loaded.LEntryDraftNote);

        TEntryCardMatch(word.LEntryDraftMeanings, loaded.LEntryDraftMeanings);
        TEntryCardMatch(word.LEntryDraftCollocations, loaded.LEntryDraftCollocations);
    }

    private static IReadOnlyList<string> TEntrySituationRead(IReadOnlyList<LSituationDraft> drafts)
    {
        List<string> texts = new(drafts.Count);
        foreach (LSituationDraft draft in drafts)
        {
            texts.Add(draft.LSituationDraftText.TStateValueShow());
        }

        return texts;
    }

    private static IReadOnlyList<string> TEntryExampleRead(IReadOnlyList<LExampleDraft> drafts)
    {
        List<string> texts = new(drafts.Count);
        foreach (LExampleDraft draft in drafts)
        {
            texts.Add(draft.LExampleDraftText.TStateValueShow());
        }

        return texts;
    }

    private static void TEntryCardMatch(IReadOnlyList<LCardDraft> expected, IReadOnlyList<LCardDraft> actual)
    {
        Assert.Equal(expected.Count, actual.Count);
        for (int index = 0; index < expected.Count; index++)
        {
            Assert.Equal(expected[index].LCardDraftTitle, actual[index].LCardDraftTitle);
            Assert.Equal(expected[index].LCardDraftExpression, actual[index].LCardDraftExpression);
            Assert.Equal(expected[index].LCardDraftMeaning, actual[index].LCardDraftMeaning);
            Assert.Equal(expected[index].LCardDraftSynonym, actual[index].LCardDraftSynonym);
            Assert.Equal(
                TEntryExampleRead(expected[index].LCardDraftExample),
                TEntryExampleRead(actual[index].LCardDraftExample));
            Assert.Equal(
                TEntrySituationRead(expected[index].LCardDraftSituation),
                TEntrySituationRead(actual[index].LCardDraftSituation));
            Assert.Equal(expected[index].LCardDraftTag, actual[index].LCardDraftTag);
            Assert.Equal(
                expected[index].LCardDraftTranslation, actual[index].LCardDraftTranslation);
        }
    }

    [Fact]
    public void EntryLoad_DownloadedRecording_ReturnsResolvedPath()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string folder = Path.Combine(workspace.TWorkspaceFolder, "audio", "english");
        Directory.CreateDirectory(folder);
        string file = Path.Combine(folder, "word.mp3");
        File.WriteAllBytes(file, [0]);

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [],
            [],
            file,
            "Wiktionary"));

        LPronunciationArchive pronunciations = TInterface.TPronunciationArchiveCreate(workspace.TWorkspaceDatabase);
        LPronunciation? pronunciation = pronunciations.TPronunciationRead(stored.LEntryId);
        Assert.NotNull(pronunciation);

        LPronunciationAudio? audio = pronunciations.TPronunciationAudioRead(pronunciation.LPronunciationId);
        Assert.NotNull(audio);
        Assert.Equal(Path.Combine("audio", "english", "word.mp3"), audio.LPronunciationAudioFile);
        Assert.Equal("Wiktionary", audio.LPronunciationAudioSource);

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(file, loaded.LEntryDraftAudio);
        Assert.Equal("Wiktionary", loaded.LEntryDraftSource);
    }

    [Fact]
    public void EntryLoad_NoRecordingSaved_ReturnsNoRecording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word", "English", "wɜːd", string.Empty, [], []));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(string.Empty, loaded.LEntryDraftAudio);
        Assert.Null(loaded.LEntryDraftSource);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM pronunciation_audio;"));
    }

    [Fact]
    public void EntryLoad_TitleOnBothCardKinds_KeepsThemApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate("the plain meaning", string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            [TInterface.TCardDraftCreate(
                "the set phrase", "in a word", "briefly", [], [], [], string.Empty, [], [], 1)]));

        LMeaning meaning = Assert.Single(TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(stored.LEntryId));
        Assert.Equal("the plain meaning", meaning.LMeaningTitle);
        LCollocation collocation = Assert.Single(
            TInterface.TCollocationArchiveCreate(workspace.TWorkspaceDatabase).TCollocationRead(stored.LEntryId));
        Assert.Equal("the set phrase", collocation.LCollocationTitle);

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal("the plain meaning", Assert.Single(loaded.LEntryDraftMeanings).LCardDraftTitle);
        Assert.Equal(
            "the set phrase",
            Assert.Single(loaded.LEntryDraftCollocations).LCardDraftTitle);
    }

    [Fact]
    public void EntryLoad_CardReferences_ReturnsWrittenOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a unit of language",
                [TInterface.TExampleDraftCreate("he said a word"), TInterface.TExampleDraftCreate("not a word was spoken")],
                [TInterface.TSituationDraftCreate("conversation")],
                [],
                string.Empty,
                ["verb", "formal", "spoken"], [], 1)],
            [TInterface.TCardDraftCreate(
                string.Empty,
                "in a word",
                "briefly",
                [TInterface.TExampleDraftCreate("in a word, no")],
                [TInterface.TSituationDraftCreate("summary"), TInterface.TSituationDraftCreate("writing")],
                [],
                string.Empty,
                ["written", "idiom"], [], 1)]));

        LMeaning meaning = Assert.Single(TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(stored.LEntryId));
        Assert.Equal(
            ["verb", "formal", "spoken"],
            TInterface.TTagArchiveCreate(workspace.TWorkspaceDatabase).TTagMeaningRead(meaning.LMeaningId).Select(tag => tag.LTagText));
        Assert.Equal(
            [0L, 1L, 2L],
            workspace.TWorkspaceColumnRead("SELECT position FROM sense_tag ORDER BY position;"));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);

        LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);
        Assert.Equal(["verb", "formal", "spoken"], card.LCardDraftTag);
        Assert.Equal(
            ["he said a word", "not a word was spoken"],
            TEntryExampleRead(card.LCardDraftExample));
        Assert.Equal(["conversation"], TEntrySituationRead(card.LCardDraftSituation));

        LCardDraft phrase = Assert.Single(loaded.LEntryDraftCollocations);
        Assert.Equal(["written", "idiom"], phrase.LCardDraftTag);
        Assert.Equal(["in a word, no"], TEntryExampleRead(phrase.LCardDraftExample));
        Assert.Equal(["summary", "writing"], TEntrySituationRead(phrase.LCardDraftSituation));
    }

    [Fact]
    public void EntryLoad_EmptyCardField_ReturnsEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [TInterface.TExampleDraftCreate("  ")], [], [], string.Empty, [], [], 1)],
            []));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);
        Assert.Empty(card.LCardDraftExample);
        Assert.Empty(card.LCardDraftSituation);
        Assert.Empty(card.LCardDraftTag);
    }

    [Fact]
    public void EntryLoad_CardMoved_ReturnsStoredPosition()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "first", [], [], [], string.Empty, [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "second", [], [], [], string.Empty, [], [], 2),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "third", [], [], [], string.Empty, [], [], 3),
            ],
            []));

        IReadOnlyList<LMeaning> saved = engine.TEngineMeaningRead(stored.LEntryId, LOwner.LOwnerEntry);
        engine.TEngineMeaningMove(saved[2].LMeaningId, 0);

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(
            ["third", "first", "second"],
            loaded.LEntryDraftMeanings.Select(card => card.LCardDraftMeaning.TStateValueShow()));
        Assert.Equal([1, 2, 3], loaded.LEntryDraftMeanings.Select(card => card.LCardDraftPosition));
    }

    [Fact]
    public void EntryLoad_UnknownId_ReturnsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Null(engine.TEngineEntryLoad("no-such-entry"));
    }
}
