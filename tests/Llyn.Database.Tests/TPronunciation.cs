using Llyn.Core;
using Llyn.ShellEngine;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TPronunciation
{
    [Fact]
    public void APronunciationIsWrittenReadRewrittenAndDeletedThroughTheEngine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TPronunciationEntryCreate(engine);
        LPronunciation stored = engine.LEnginePronunciationCreate(
            new LPronunciation(string.Empty, entry.LEntryId, null, "/wɜːd/", [], []));

        Assert.NotEmpty(stored.LPronunciationId);
        Assert.Equal("/wɜːd/", engine.LEnginePronunciationRead(entry.LEntryId)?.LPronunciationIpa);

        engine.LEnginePronunciationUpdate(stored with { LPronunciationIpa = "/wɝd/" });
        Assert.Equal("/wɝd/", engine.LEnginePronunciationRead(entry.LEntryId)?.LPronunciationIpa);

        engine.LEnginePronunciationDelete(stored.LPronunciationId);
        Assert.Null(engine.LEnginePronunciationRead(entry.LEntryId));
    }

    [Fact]
    public void AudioIsStoredRelativeToTheWorkspaceAndHandedBackResolved()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TPronunciationEntryCreate(engine);
        LPronunciation pronunciation = engine.LEnginePronunciationCreate(
            new LPronunciation(string.Empty, entry.LEntryId, null, "/wɜːd/", [], []));

        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "English", "word.mp3");
        engine.LEngineAudioSave(pronunciation.LPronunciationId, file, "Wikipedia");

        Assert.Equal(
            Path.Combine("audio", "English", "word.mp3"),
            TPronunciationFileRead(workspace));
        LPronunciationAudio? audio = engine.LEngineAudioRead(pronunciation.LPronunciationId);
        Assert.Equal(file, audio?.LPronunciationAudioFile);
        Assert.Equal("Wikipedia", audio?.LPronunciationAudioSource);

        engine.LEnginePronunciationDelete(pronunciation.LPronunciationId);
        Assert.Null(engine.LEngineAudioRead(pronunciation.LPronunciationId));
    }

    [Fact]
    public void AnEntryKeepsOneNoteThatASaveCreatesOrRewrites()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TPronunciationEntryCreate(engine);
        Assert.Null(engine.LEngineNoteRead(entry.LEntryId));

        engine.LEngineNoteSave(new LNote(entry.LEntryId, "first thoughts"));
        engine.LEngineNoteSave(new LNote(entry.LEntryId, "second thoughts"));
        Assert.Equal("second thoughts", engine.LEngineNoteRead(entry.LEntryId)?.LNoteText);
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM note;"));

        engine.LEngineNoteDelete(entry.LEntryId);
        Assert.Null(engine.LEngineNoteRead(entry.LEntryId));
    }

    private static string TPronunciationFileRead(TWorkspace workspace)
    {
        using SqliteConnection connection = workspace.TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT file FROM pronunciation_audio;";
        return (string)command.ExecuteScalar()!;
    }

    private static LEntry TPronunciationEntryCreate(LEngine engine)
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
