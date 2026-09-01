using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

/// <summary>
/// Covers reading entries back: listing and searching them by headword, and loading one whole entry
/// into the draft it was saved from. The load is the inverse of the save, so what a test writes
/// through <c>LEngineEntrySave</c> is exactly what comes back — headword, language, pronunciation and
/// note, and every card with its definition, example, situation and tag.
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
                new LSenseDraft(1, "a unit of language", "he said a word", "conversation", string.Empty, "spoken"),
                new LSenseDraft(2, "a promise", string.Empty, string.Empty, string.Empty, string.Empty),
            ],
            [
                new LCollocationDraft(
                    1, "in a word", "briefly", "in a word, no", "summary", string.Empty, "written"),
            ]);

        LEntryDraft sword = new(
            "sword",
            "English",
            "sɔːd",
            string.Empty,
            [new LSenseDraft(1, "a bladed weapon", string.Empty, string.Empty, string.Empty, string.Empty)],
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

        // The card lists are compared value by value: LEntryDraft's own equality would compare the two
        // list references, which are never the same object.
        Assert.Equal(word.LEntryDraftSenses, loaded.LEntryDraftSenses);
        Assert.Equal(word.LEntryDraftCollocations, loaded.LEntryDraftCollocations);
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
    public void AnIdNoEntryCarriesLoadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        Assert.Null(engine.LEngineEntryLoad("no-such-entry"));
    }
}
