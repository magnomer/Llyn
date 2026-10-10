using System;
using System.Collections.Generic;
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
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        string file = TPlaybackFileSave(workspace, "kindle.mp3");
        editor.TEditorFixtureDesk.TDeskDefer(
            TInterface.TRequestAudioCreate(editor.TEditorFixtureDesk.CDeskId, file, "Forvo"));

        CTimbrePlayback playback = editor.TEditorFixturePlayback.CPlaybackRead();

        Assert.Equal(file, playback.CTimbrePlaybackAudio);
        Assert.True(playback.CTimbrePlaybackAudible);
    }

    [Fact]
    public void PlaybackRead_MissingFile_ReadsNoAudio()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "gone.mp3");
        editor.TEditorFixtureDesk.TDeskDefer(
            TInterface.TRequestAudioCreate(editor.TEditorFixtureDesk.CDeskId, file, "Forvo"));

        CTimbrePlayback playback = editor.TEditorFixturePlayback.CPlaybackRead();

        Assert.Null(playback.CTimbrePlaybackAudio);
        Assert.False(playback.CTimbrePlaybackAudible);
    }

    [Fact]
    public void PlaybackRead_AccentAudioOnly_ReadsAudible()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long draft = editor.TEditorFixtureDesk.CDeskId;
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TPronunciationAdditionCreate(draft, string.Empty, 1));
        long accent = editor.TEditorFixtureTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TPronunciationAudioCreate(draft, accent, "row.mp3", "Forvo"));

        CTimbrePlayback playback = editor.TEditorFixturePlayback.CPlaybackRead();

        Assert.Null(playback.CTimbrePlaybackAudio);
        Assert.True(playback.CTimbrePlaybackAudible);
    }

    [Fact]
    public void PlaybackStart_StoredFile_AnswersItsAddress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPlayback playback = TEditorField.TEditorFieldPrepare(engine).TEditorFixturePlayback;
        string file = TPlaybackFileSave(workspace, "ember.mp3");

        Assert.Equal(new Uri(file), playback.CPlaybackStart(file));
    }

    [Fact]
    public void PlaybackStart_MissingFile_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPlayback playback = TEditorField.TEditorFieldPrepare(engine).TEditorFixturePlayback;
        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "gone.mp3");

        Assert.Null(playback.CPlaybackStart(file));
        Assert.Null(playback.CPlaybackStart(null));
    }

    [Fact]
    public void PlaybackAccentStart_StoredFile_AnswersItsAddressAndKeepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TTimbre.TTimbreAccentPrepare(engine, "a");
        long spoken = editor.TEditorFixtureTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        string file = TPlaybackFileSave(workspace, "row.mp3");
        editor.TEditorFixtureDesk.TDeskDefer(
            TInterface.TPronunciationAudioCreate(editor.TEditorFixtureDesk.CDeskId, spoken, file, "Forvo"));

        Assert.Equal(new Uri(file), editor.TEditorFixturePlayback.CPlaybackAccentStart(spoken));
        Assert.Equal(
            file, Assert.Single(editor.TEditorFixtureTimbre.CTimbreAccentRead().CTimbreAccentRows).CAccentAudio);
    }

    [Fact]
    public void PlaybackAccentStart_MissingFile_AnswersNothingAndClearsTheRowAudio()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TTimbre.TTimbreAccentPrepare(engine, "a");
        long spoken = editor.TEditorFixtureTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "gone.mp3");
        editor.TEditorFixtureDesk.TDeskDefer(
            TInterface.TPronunciationAudioCreate(editor.TEditorFixtureDesk.CDeskId, spoken, file, "Forvo"));

        Assert.Null(editor.TEditorFixturePlayback.CPlaybackAccentStart(spoken));
        Assert.Empty(Assert.Single(editor.TEditorFixtureTimbre.CTimbreAccentRead().CTimbreAccentRows).CAccentAudio);
    }

    [Fact]
    public void PlaybackAccentStart_GoneRowOrEmptyDesk_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPlayback playback = TTimbre.TTimbreAccentPrepare(engine, "a").TEditorFixturePlayback;

        Assert.Null(playback.CPlaybackAccentStart(long.MaxValue));
        Assert.Null(playback.CPlaybackAccentStart(0));
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
        return new TEditorFixture(TInterfaceEditor.TEditorCreate(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                TInterfaceConduct.TEntryBundleCreate([]),
                TInterfaceConduct.TPhonologyBundleCreate([]),
                TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
                {
                }),
                TEngineFake.TEngineStubCreate<LMediaPort>()))
            .TEditorFixturePlayback;
    }
}
