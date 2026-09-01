using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

/// <summary>
/// Covers reading entries back: listing and searching them by headword, and loading one whole entry
/// into the draft it was saved from. The load is the inverse of the save, so what a test writes
/// through <c>LEngineEntrySave</c> is exactly what comes back — headword, language, pronunciation and
/// note, and every card with its definition and the whole ordered set of Examples, Situations and Tags
/// it references.
/// </summary>
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
                    ["he said a word"], ["conversation"], string.Empty, ["spoken"]),
                new LCardDraft(string.Empty, string.Empty, "a promise", [], [], string.Empty, []),
            ],
            [
                new LCardDraft(
                    string.Empty, "in a word", "briefly", ["in a word, no"], ["summary"], string.Empty,
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

        // An empty query lists everything, ordered by headword.
        Assert.Equal(["sword", "word"], engine.LEngineEntryFind(string.Empty).Select(e => e.LEntryHeadword));

        // A partial headword narrows it, case-insensitively, and matches anywhere in the headword —
        // "word" is inside "sword" too, so a query that picks exactly one has to be one of its own.
        Assert.Equal(["sword", "word"], engine.LEngineEntryFind("WORD").Select(e => e.LEntryHeadword));
        Assert.Equal("sword", Assert.Single(engine.LEngineEntryFind("swo")).LEntryHeadword);
        Assert.Empty(engine.LEngineEntryFind("axe"));

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(word.LEntryDraftHeadword, loaded.LEntryDraftHeadword);
        Assert.Equal(word.LEntryDraftLanguage, loaded.LEntryDraftLanguage);
        Assert.Equal(word.LEntryDraftPronunciation, loaded.LEntryDraftPronunciation);
        Assert.Equal(word.LEntryDraftNote, loaded.LEntryDraftNote);

        // The cards are compared field by field: a draft holds its cards, and a card its Examples,
        // Situations and Tags, in lists, and list equality is reference equality — never true across a
        // round trip through the store.
        TEntryCardMatch(word.LEntryDraftSenses, loaded.LEntryDraftSenses);
        TEntryCardMatch(word.LEntryDraftCollocations, loaded.LEntryDraftCollocations);
    }

    // Asserts that two card lists carry the same cards in the same order, every field included: the
    // save and the load are inverses, so a card that was written comes back exactly as it went in.
    private static void TEntryCardMatch(IReadOnlyList<LCardDraft> expected, IReadOnlyList<LCardDraft> actual)
    {
        Assert.Equal(expected.Count, actual.Count);
        for (int index = 0; index < expected.Count; index++)
        {
            Assert.Equal(expected[index].LCardDraftTitle, actual[index].LCardDraftTitle);
            Assert.Equal(expected[index].LCardDraftExpression, actual[index].LCardDraftExpression);
            Assert.Equal(expected[index].LCardDraftMeaning, actual[index].LCardDraftMeaning);
            Assert.Equal(expected[index].LCardDraftSynonym, actual[index].LCardDraftSynonym);
            Assert.Equal(expected[index].LCardDraftExample, actual[index].LCardDraftExample);
            Assert.Equal(expected[index].LCardDraftSituation, actual[index].LCardDraftSituation);
            Assert.Equal(expected[index].LCardDraftTag, actual[index].LCardDraftTag);
        }
    }

    [Fact]
    public void ADownloadedRecordingIsStoredRelativeToTheWorkspaceAndLoadsBackAsAFullPath()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        // The file the downloader would have written: under the workspace, in its audio bucket.
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

        // Stored relative, so a workspace that is moved or copied keeps its audio. The row is read
        // through the archive rather than as SQL text, so the path separator stays the platform's.
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

        // The titles are columns of the cards themselves, so they are on the rows before any load.
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
                ["he said a word", "not a word was spoken"],
                ["conversation"],
                string.Empty,
                ["verb", "formal", "spoken"])],
            [new LCardDraft(
                string.Empty,
                "in a word",
                "briefly",
                ["in a word, no"],
                ["summary", "writing"],
                string.Empty,
                ["written", "idiom"])]));

        // The rows are stored at the positions the lists held, so nothing shares a position and the
        // order is a fact of the store rather than of the read.
        LSense sense = Assert.Single(new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(stored.LEntryId));
        Assert.Equal(
            ["verb", "formal", "spoken"],
            new LTagArchive(workspace.TWorkspaceDatabase).LTagSenseRead(sense.LSenseId).Select(tag => tag.LTagText));
        Assert.Equal(
            [0L, 1L, 2L],
            workspace.TWorkspaceColumnRead("SELECT position FROM sense_tag ORDER BY position;"));

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);

        // Three tags and two examples on the one card, all five back in the order they were written.
        LCardDraft card = Assert.Single(loaded.LEntryDraftSenses);
        Assert.Equal(["verb", "formal", "spoken"], card.LCardDraftTag);
        Assert.Equal(["he said a word", "not a word was spoken"], card.LCardDraftExample);
        Assert.Equal(["conversation"], card.LCardDraftSituation);

        // The collocation card references its own sets on the same terms.
        LCardDraft phrase = Assert.Single(loaded.LEntryDraftCollocations);
        Assert.Equal(["written", "idiom"], phrase.LCardDraftTag);
        Assert.Equal(["in a word, no"], phrase.LCardDraftExample);
        Assert.Equal(["summary", "writing"], phrase.LCardDraftSituation);
    }

    [Fact]
    public void ACardFieldLeftEmptyReferencesNothingAndLoadsBackEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        // A blank value in a list is not a row: an empty field detaches, and the independent tables
        // stay empty rather than collecting rows with no text.
        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", ["  "], [], string.Empty, [])],
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
