using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplayPlayback
{
    [Fact]
    public void DisplayPlaybackRead_AccentWithAudioOnly_ShowsTheTrayButNotTheButton()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TInterfaceConduct.TMediaCreate(engine));
        CWing wing = TDisplaySound.TDisplayWingPrepare(atelier, []);
        CLecternPlayback blank = wing.CWingDisplay.CDisplayPlayback.CDisplayPlaybackRead();
        LEntry water = engine.TEngineEntrySave(TDisplaySound.TDisplayDraftCreate("water", "English", []) with
        {
            LEntryDraftPronunciations =
            [
                TInterface.TPronunciationDraftCreate("ˈwɔːtə", "British"),
                TInterface.TPronunciationDraftCreate("ˈwɑːtɚ", "American", "missing.ogg"),
            ],
        });
        wing.CWingEntryOpen(water.LEntryId);

        CLecternPlayback playback = wing.CWingDisplay.CDisplayPlayback.CDisplayPlaybackRead();

        Assert.Equal(new CLecternPlayback(false, false), blank);
        Assert.Equal(new CLecternPlayback(false, true), playback);
    }

    [Fact]
    public void DisplayPlaybackStart_AccentRow_PlaysAtTheLevelAndTheCancelStopsThatPlay()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> played = [];
        List<int> stopped = [];
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineRecordingPlay"] = args =>
                {
                    object? file = args![0] is LEntryDraft draft ? draft.LEntryDraftHeadword : args[0];
                    played.Add(string.Concat(file, "@", args[1]));
                    return played.Count + 6;
                },
                ["LEngineRecordingStop"] = args =>
                {
                    stopped.Add((int)args![0]!);
                    return null;
                },
            }));
        CWing wing = TDisplaySound.TDisplayWingPrepare(atelier, []);
        CDisplayPlayback area = wing.CWingDisplay.CDisplayPlayback;
        area.CDisplayPlaybackStart(0.5);
        wing.CWingEntryOpen(TDisplaySound.TDisplayEnglishSave(engine).LEntryId);

        area.CDisplayPlaybackStart("row.ogg", 0.25);
        area.CDisplayPlaybackStart(0.5);
        area.CDisplayPlaybackCancel();

        Assert.Equal(["row.ogg@0.25", "water@0.5"], played);
        Assert.Equal([8], stopped);
    }

    [Fact]
    public void DisplayPlaybackCancel_NothingPlaying_KeepsTheShownEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineRecordingStop"] = _ => null,
            }));
        CWing wing = TDisplaySound.TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(TDisplaySound.TDisplayEnglishSave(engine).LEntryId);

        wing.CWingDisplay.CDisplayPlayback.CDisplayPlaybackCancel();

        Assert.Equal("water", wing.CWingDisplay.CDisplayShown.CLecternHeadword);
    }
}
