using Llyn.Core;
using Llyn.ShellEngine;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Llyn.Tests;

public sealed class TPronunciation
{
    [Fact]
    public void PronunciationCreate_WholeRowLife_ReadsBackEachStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TPronunciationEntryCreate(engine);
        LPronunciation stored = engine.TEnginePronunciationCreate(
            TInterface.TPronunciationCreate(string.Empty, entry.LEntryId, null, "/wɜːd/", [], []));

        Assert.NotEmpty(stored.LPronunciationId);
        Assert.Equal("/wɜːd/", engine.TEnginePronunciationRead(entry.LEntryId)?.LPronunciationIpa);

        engine.TEnginePronunciationUpdate(stored with { LPronunciationIpa = "/wɝd/" });
        Assert.Equal("/wɝd/", engine.TEnginePronunciationRead(entry.LEntryId)?.LPronunciationIpa);

        engine.TEnginePronunciationDelete(stored.LPronunciationId);
        Assert.Null(engine.TEnginePronunciationRead(entry.LEntryId));
    }

    [Fact]
    public void AudioSave_WorkspaceFile_StoresRelativeReadsResolved()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TPronunciationEntryCreate(engine);
        LPronunciation pronunciation = engine.TEnginePronunciationCreate(
            TInterface.TPronunciationCreate(string.Empty, entry.LEntryId, null, "/wɜːd/", [], []));

        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "English", "word.mp3");
        engine.TEngineAudioSave(pronunciation.LPronunciationId, file, "Wikipedia");

        Assert.Equal(
            Path.Combine("audio", "English", "word.mp3"),
            TPronunciationFileRead(workspace));
        LPronunciationAudio? audio = engine.TEngineAudioRead(pronunciation.LPronunciationId);
        Assert.Equal(file, audio?.LPronunciationAudioFile);
        Assert.Equal("Wikipedia", audio?.LPronunciationAudioSource);

        engine.TEnginePronunciationDelete(pronunciation.LPronunciationId);
        Assert.Null(engine.TEngineAudioRead(pronunciation.LPronunciationId));
    }

    [Fact]
    public void NoteSave_CreateOrRewrite_KeepsExactlyOneNote()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TPronunciationEntryCreate(engine);
        Assert.Null(engine.TEngineNoteRead(entry.LEntryId));

        engine.TEngineNoteSave(TInterface.TNoteCreate(entry.LEntryId, "first thoughts"));
        engine.TEngineNoteSave(TInterface.TNoteCreate(entry.LEntryId, "second thoughts"));
        Assert.Equal("second thoughts", engine.TEngineNoteRead(entry.LEntryId)?.LNoteText);
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM note;"));

        engine.TEngineNoteDelete(entry.LEntryId);
        Assert.Null(engine.TEngineNoteRead(entry.LEntryId));
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
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1)]));
    }
}
