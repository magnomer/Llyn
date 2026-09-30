using System;
using System.Collections.Generic;
using System.IO;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTimbre
{
    [Fact]
    public void TimbrePhonemic_RespellingPhonemicPack_ReadsTrue()
    {
        CTimbre timbre = TTimbrePrepare(new()
        {
            ["LEngineRespellingCheck"] = _ => true,
            ["LEnginePhonemicCheck"] = _ => true,
        });

        Assert.True(timbre.CTimbrePhonemic);
    }

    [Fact]
    public void TimbrePhonemic_PhonemicPackWithoutRespelling_ReadsFalse()
    {
        CTimbre timbre = TTimbrePrepare(new()
        {
            ["LEngineRespellingCheck"] = _ => false,
            ["LEnginePhonemicCheck"] = _ => true,
        });

        Assert.False(timbre.CTimbrePhonemic);
    }

    [Fact]
    public void TimbreSpoken_SilentPack_ReadsFalse()
    {
        CTimbre timbre = TTimbrePrepare(new() { ["LEngineSilentCheck"] = _ => true });

        Assert.False(timbre.CTimbreSpoken);
    }

    [Fact]
    public void TimbreTonal_EmptyDesk_AsksThePackForNoLanguage()
    {
        List<string> asked = [];
        CTimbre timbre = TTimbrePrepare(new()
        {
            ["LEngineTonalCheck"] = args =>
            {
                asked.Add((string)args![0]!);
                return true;
            },
        });

        Assert.True(timbre.CTimbreTonal);
        Assert.Equal([string.Empty], asked);
    }

    [Fact]
    public void TimbrePlaybackRead_EmptyDesk_ReadsNoAudio()
    {
        CTimbre timbre = TTimbrePrepare([]);

        CTimbrePlayback playback = timbre.CTimbrePlaybackRead();

        Assert.Null(playback.CTimbrePlaybackAudio);
        Assert.False(playback.CTimbrePlaybackAudible);
    }

    [Fact]
    public void TimbrePlaybackRead_StoredAudio_ReadsItsFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        string file = TTimbreFileSave(workspace, "kindle.mp3");
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestAudioCreate(editor.CEditorDesk.CDeskId, file, "Forvo"));

        CTimbrePlayback playback = editor.CEditorTimbre.CTimbrePlaybackRead();

        Assert.Equal(file, playback.CTimbrePlaybackAudio);
        Assert.True(playback.CTimbrePlaybackAudible);
    }

    [Fact]
    public void TimbrePlaybackRead_MissingFile_ReadsNoAudio()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "gone.mp3");
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestAudioCreate(editor.CEditorDesk.CDeskId, file, "Forvo"));

        CTimbrePlayback playback = editor.CEditorTimbre.CTimbrePlaybackRead();

        Assert.Null(playback.CTimbrePlaybackAudio);
        Assert.False(playback.CTimbrePlaybackAudible);
    }

    [Fact]
    public void TimbrePlaybackRead_AccentAudioOnly_ReadsAudible()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        long draft = editor.CEditorDesk.CDeskId;
        editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationAdditionCreate(draft, string.Empty, 1));
        long accent = editor.CEditorDraftRead()!.CEntryDraftAccents[0].CPronunciationDraftId;
        editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationAudioCreate(draft, accent, "row.mp3", "Forvo"));

        CTimbrePlayback playback = editor.CEditorTimbre.CTimbrePlaybackRead();

        Assert.Null(playback.CTimbrePlaybackAudio);
        Assert.True(playback.CTimbrePlaybackAudible);
    }

    [Fact]
    public void TimbrePlaybackStart_StoredFile_AnswersItsAddress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        string file = TTimbreFileSave(workspace, "ember.mp3");

        Assert.Equal(new Uri(file), editor.CEditorTimbre.CTimbrePlaybackStart(file));
    }

    [Fact]
    public void TimbrePlaybackStart_MissingFile_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "gone.mp3");

        Assert.Null(editor.CEditorTimbre.CTimbrePlaybackStart(file));
        Assert.Null(editor.CEditorTimbre.CTimbrePlaybackStart(null));
    }

    private static string TTimbreFileSave(TWorkspace workspace, string name)
    {
        string folder = Path.Combine(workspace.TWorkspaceFolder, "audio", "english");
        Directory.CreateDirectory(folder);
        string file = Path.Combine(folder, name);
        File.WriteAllBytes(file, [0]);
        return file;
    }

    private static CTimbre TTimbrePrepare(Dictionary<string, Func<object?[]?, object?>> answers)
    {
        return TInterfaceConduct.TEditorCreate(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                TEngineFake.TEngineStubCreate<LEntryPort>(),
                TEngineFake.TEngineCreate<LPhonologyPort>(answers),
                TEngineFake.TEngineStubCreate<LSettingsPort>(),
                TEngineFake.TEngineStubCreate<LMediaPort>())
            .CEditorTimbre;
    }
}
