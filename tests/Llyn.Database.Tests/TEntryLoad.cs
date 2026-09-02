using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TEntryLoad
{
    [Fact]
    public void EntriesAreFoundByHeadwordAndLoadedBackIntoTheDraftTheyWereSavedFrom()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntryDraft word = new(
            "word",
            "English",
            "wɜːd",
            "a note",
            [
                new LCardDraft(
                    string.Empty, string.Empty, "a unit of language",
                    [LExampleDraft.LExampleDraftCreate("he said a word")], [LSituationDraft.LSituationDraftCreate("conversation")], string.Empty, ["spoken"]),
                new LCardDraft(string.Empty, string.Empty, "a promise", [], [], string.Empty, []),
            ],
            [
                new LCardDraft(
                    string.Empty, "in a word", "briefly", [LExampleDraft.LExampleDraftCreate("in a word, no")], [LSituationDraft.LSituationDraftCreate("summary")], string.Empty,
                    ["written"]),
            ]);

        LEntryDraft sword = new(
            "sword",
            "English",
            "sɔːd",
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a bladed weapon", [], [], string.Empty, [])],
            []);

        LEntry stored = engine.LEngineEntrySave(word);
        engine.LEngineEntrySave(sword);

        Assert.Equal(["sword", "word"], engine.LEngineEntryFind(string.Empty).Select(e => e.LEntryHeadword));

        Assert.Equal(["sword", "word"], engine.LEngineEntryFind("WORD").Select(e => e.LEntryHeadword));
        Assert.Equal("sword", Assert.Single(engine.LEngineEntryFind("swo")).LEntryHeadword);
        Assert.Empty(engine.LEngineEntryFind("axe"));

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(word.LEntryDraftHeadword, loaded.LEntryDraftHeadword);
        Assert.Equal(word.LEntryDraftLanguage, loaded.LEntryDraftLanguage);
        Assert.Equal(word.LEntryDraftPronunciation, loaded.LEntryDraftPronunciation);
        Assert.Equal(word.LEntryDraftNote, loaded.LEntryDraftNote);

        TEntryCardMatch(word.LEntryDraftSenses, loaded.LEntryDraftSenses);
        TEntryCardMatch(word.LEntryDraftCollocations, loaded.LEntryDraftCollocations);
    }

    private static IReadOnlyList<string> TEntrySituationRead(IReadOnlyList<LSituationDraft> drafts)
    {
        List<string> texts = new(drafts.Count);
        foreach (LSituationDraft draft in drafts)
        {
            texts.Add(draft.LSituationDraftText.LStateValueShow());
        }

        return texts;
    }

    private static IReadOnlyList<string> TEntryExampleRead(IReadOnlyList<LExampleDraft> drafts)
    {
        List<string> texts = new(drafts.Count);
        foreach (LExampleDraft draft in drafts)
        {
            texts.Add(draft.LExampleDraftText.LStateValueShow());
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
        }
    }

    [Fact]
    public void ADownloadedRecordingIsStoredRelativeToTheWorkspaceAndLoadsBackAsAFullPath()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        string folder = Path.Combine(workspace.TWorkspaceFolder, "audio", "english");
        Directory.CreateDirectory(folder);
        string file = Path.Combine(folder, "word.mp3");
        File.WriteAllBytes(file, [0]);

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [],
            [],
            file,
            "Wiktionary"));

        LPronunciationArchive pronunciations = new(workspace.TWorkspaceDatabase);
        LPronunciation? pronunciation = pronunciations.LPronunciationRead(stored.LEntryId);
        Assert.NotNull(pronunciation);

        LPronunciationAudio? audio = pronunciations.LPronunciationAudioRead(pronunciation.LPronunciationId);
        Assert.NotNull(audio);
        Assert.Equal(Path.Combine("audio", "english", "word.mp3"), audio.LPronunciationAudioFile);
        Assert.Equal("Wiktionary", audio.LPronunciationAudioSource);

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(file, loaded.LEntryDraftAudio);
        Assert.Equal("Wiktionary", loaded.LEntryDraftSource);
    }

    [Fact]
    public void AnEntrySavedWithNoRecordingLoadsBackWithNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "word", "English", "wɜːd", string.Empty, [], []));

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(string.Empty, loaded.LEntryDraftAudio);
        Assert.Null(loaded.LEntryDraftSource);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM pronunciation_audio;"));
    }

    [Fact]
    public void TheTitleTypedOnBothKindsOfCardLoadsBackOnTheCardItWasTypedOn()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft("the plain sense", string.Empty, "a meaning", [], [], string.Empty, [])],
            [new LCardDraft(
                "the set phrase", "in a word", "briefly", [], [], string.Empty, [])]));

        LSense sense = Assert.Single(new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(stored.LEntryId));
        Assert.Equal("the plain sense", sense.LSenseTitle);
        LCollocation collocation = Assert.Single(
            new LCollocationArchive(workspace.TWorkspaceDatabase).LCollocationRead(stored.LEntryId));
        Assert.Equal("the set phrase", collocation.LCollocationTitle);

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal("the plain sense", Assert.Single(loaded.LEntryDraftSenses).LCardDraftTitle);
        Assert.Equal(
            "the set phrase",
            Assert.Single(loaded.LEntryDraftCollocations).LCardDraftTitle);
    }

    [Fact]
    public void EveryExampleSituationAndTagACardReferencesLoadsBackInTheOrderItWasWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(
                string.Empty,
                string.Empty,
                "a unit of language",
                [LExampleDraft.LExampleDraftCreate("he said a word"), LExampleDraft.LExampleDraftCreate("not a word was spoken")],
                [LSituationDraft.LSituationDraftCreate("conversation")],
                string.Empty,
                ["verb", "formal", "spoken"])],
            [new LCardDraft(
                string.Empty,
                "in a word",
                "briefly",
                [LExampleDraft.LExampleDraftCreate("in a word, no")],
                [LSituationDraft.LSituationDraftCreate("summary"), LSituationDraft.LSituationDraftCreate("writing")],
                string.Empty,
                ["written", "idiom"])]));

        LSense sense = Assert.Single(new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(stored.LEntryId));
        Assert.Equal(
            ["verb", "formal", "spoken"],
            new LTagArchive(workspace.TWorkspaceDatabase).LTagSenseRead(sense.LSenseId).Select(tag => tag.LTagText));
        Assert.Equal(
            [0L, 1L, 2L],
            workspace.TWorkspaceColumnRead("SELECT position FROM sense_tag ORDER BY position;"));

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);

        LCardDraft card = Assert.Single(loaded.LEntryDraftSenses);
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
    public void ACardFieldLeftEmptyReferencesNothingAndLoadsBackEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [LExampleDraft.LExampleDraftCreate("  ")], [], string.Empty, [])],
            []));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        LCardDraft card = Assert.Single(loaded.LEntryDraftSenses);
        Assert.Empty(card.LCardDraftExample);
        Assert.Empty(card.LCardDraftSituation);
        Assert.Empty(card.LCardDraftTag);
    }

    [Fact]
    public void AnIdNoEntryCarriesLoadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        Assert.Null(engine.LEngineEntryLoad("no-such-entry"));
    }
}
