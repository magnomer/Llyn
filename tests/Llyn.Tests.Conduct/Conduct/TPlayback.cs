using System;
using System.IO;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPlayback
{
    [Fact]
    public void PlaybackRead_EmptyDesk_ReadsNoAudio()
    {
        CPlayback playback = TPlaybackPrepare();

        CTimbrePlayback read = playback.CPlaybackRead();

        Assert.Null(read.CTimbrePlaybackAudio);
        Assert.False(read.CTimbrePlaybackAudible);
    }

    [Fact]
    public void PlaybackRead_StoredAudio_ReadsItsFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        string file = TPlaybackFileSave(workspace, "kindle.mp3");
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestAudioCreate(editor.CEditorDesk.CDeskId, file, "Forvo"));

        CTimbrePlayback playback = editor.CEditorPlayback.CPlaybackRead();

        Assert.Equal(file, playback.CTimbrePlaybackAudio);
        Assert.True(playback.CTimbrePlaybackAudible);
    }

    [Fact]
    public void PlaybackRead_MissingFile_ReadsNoAudio()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "gone.mp3");
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestAudioCreate(editor.CEditorDesk.CDeskId, file, "Forvo"));

        CTimbrePlayback playback = editor.CEditorPlayback.CPlaybackRead();

        Assert.Null(playback.CTimbrePlaybackAudio);
        Assert.False(playback.CTimbrePlaybackAudible);
    }

    [Fact]
    public void PlaybackRead_AccentAudioOnly_ReadsAudible()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        long draft = editor.CEditorDesk.CDeskId;
        editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationAdditionCreate(draft, string.Empty, 1));
        long accent = editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationAudioCreate(draft, accent, "row.mp3", "Forvo"));

        CTimbrePlayback playback = editor.CEditorPlayback.CPlaybackRead();

        Assert.Null(playback.CTimbrePlaybackAudio);
        Assert.True(playback.CTimbrePlaybackAudible);
    }

    [Fact]
    public void PlaybackStart_StoredFile_AnswersItsAddress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        string file = TPlaybackFileSave(workspace, "ember.mp3");

        Assert.Equal(new Uri(file), editor.CEditorPlayback.CPlaybackStart(file));
    }

    [Fact]
    public void PlaybackStart_MissingFile_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "gone.mp3");

        Assert.Null(editor.CEditorPlayback.CPlaybackStart(file));
        Assert.Null(editor.CEditorPlayback.CPlaybackStart(null));
    }

    [Fact]
    public void PlaybackAccentStart_StoredFile_AnswersItsAddressAndKeepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbre.TTimbreAccentPrepare(engine, "a");
        long spoken = editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        string file = TPlaybackFileSave(workspace, "row.mp3");
        editor.CEditorDesk.TDeskDefer(
            TInterface.TPronunciationAudioCreate(editor.CEditorDesk.CDeskId, spoken, file, "Forvo"));

        Assert.Equal(new Uri(file), editor.CEditorPlayback.CPlaybackAccentStart(spoken));
        Assert.Equal(file, Assert.Single(editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows).CAccentAudio);
    }

    [Fact]
    public void PlaybackAccentStart_MissingFile_AnswersNothingAndClearsTheRowAudio()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbre.TTimbreAccentPrepare(engine, "a");
        long spoken = editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "gone.mp3");
        editor.CEditorDesk.TDeskDefer(
            TInterface.TPronunciationAudioCreate(editor.CEditorDesk.CDeskId, spoken, file, "Forvo"));

        Assert.Null(editor.CEditorPlayback.CPlaybackAccentStart(spoken));
        Assert.Empty(Assert.Single(editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows).CAccentAudio);
    }

    [Fact]
    public void PlaybackAccentStart_GoneRowOrEmptyDesk_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbre.TTimbreAccentPrepare(engine, "a");

        Assert.Null(editor.CEditorPlayback.CPlaybackAccentStart(long.MaxValue));
        Assert.Null(editor.CEditorPlayback.CPlaybackAccentStart(0));
        Assert.Null(TPlaybackPrepare().CPlaybackAccentStart(1));
    }

    private static string TPlaybackFileSave(TWorkspace workspace, string name)
    {
        string folder = Path.Combine(workspace.TWorkspaceFolder, "audio", "english");
        Directory.CreateDirectory(folder);
        string file = Path.Combine(folder, name);
        File.WriteAllBytes(file, [0]);
        return file;
    }

    private static CPlayback TPlaybackPrepare()
    {
        return TInterfaceEditor.TEditorCreate(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                TEngineFake.TEngineStubCreate<LEntryPort>(),
                TEngineFake.TEngineStubCreate<LPhonologyPort>(),
                TEngineFake.TEngineStubCreate<LSettingsPort>(),
                TEngineFake.TEngineStubCreate<LMediaPort>())
            .CEditorPlayback;
    }
}
