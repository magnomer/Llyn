using System;
using System.IO;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TStudySound
{
    [Fact]
    public void StudyStart_SoundOnBack_ReadsAudibleBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long deck = port.TEngineDeckCreate("Study", null, LGrain.LGrainMeaning);
        port.TEngineCueCreate(
            deck,
            [
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bed"),
                TInterface.TCueCellCreate(LCueColumn.LCueColumnSound, 2, "bed.e6ade888.mp3"),
            ]);
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);
        study.CStudySelect(deck);
        study.CStudyStart();

        CSittingState state = Assert.IsType<CSitting>(study.CStudySitting).CSittingSideToggle();

        Assert.Equal(string.Empty, state.CSittingStateBack.CSittingSideText);
        Assert.True(state.CSittingStateBack.CSittingSideAudible);
        Assert.False(state.CSittingStateFront.CSittingSideAudible);
        Assert.Null(state.CSittingStateBlank);
    }

    [Fact]
    public void StudySoundPlay_TwoFrontSounds_PlaysInTurn()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierMediaCreate(engine, TInterfaceConduct.TMediaCreate(engine));
        (CStudy study, string first, string second) = TStudySoundStart(workspace, engine, atelier);

        study.CStudySoundPlay(false);
        workspace.TWorkspacePhonograph.TPhonographFakeFinish();

        Assert.Equal([first, second], workspace.TWorkspacePhonograph.TPhonographFakePlayed);
    }

    [Fact]
    public void SittingAdvance_SoundRunning_StopsRun()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierMediaCreate(engine, TInterfaceConduct.TMediaCreate(engine));
        (CStudy study, string first, _) = TStudySoundStart(workspace, engine, atelier);
        study.CStudySoundPlay(false);
        int stopped = workspace.TWorkspacePhonograph.TPhonographFakeStopped;

        Assert.IsType<CSitting>(study.CStudySitting).CSittingAdvance();
        workspace.TWorkspacePhonograph.TPhonographFakeFinish();

        Assert.Equal(first, Assert.Single(workspace.TWorkspacePhonograph.TPhonographFakePlayed));
        Assert.Equal(stopped + 1, workspace.TWorkspacePhonograph.TPhonographFakeStopped);
    }

    private static (CStudy, string, string) TStudySoundStart(TWorkspace workspace, LEngine engine, CAtelier atelier)
    {
        string folder = Path.Combine(workspace.TWorkspaceFolder, "audio", "english");
        Directory.CreateDirectory(folder);
        string first = Path.Combine(folder, "bed.mp3");
        string second = Path.Combine(folder, "beds.mp3");
        File.WriteAllBytes(first, [0]);
        File.WriteAllBytes(second, [0]);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long deck = port.TEngineDeckCreate("Study", null, LGrain.LGrainMeaning);
        port.TEngineCueCreate(
            deck,
            [
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bed"),
                TInterface.TCueCellCreate(LCueColumn.LCueColumnSound, 1, first + "|" + second),
            ]);
        port.TEngineCueCreate(deck, [TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "cot")]);
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);
        study.CStudySelect(deck);
        study.CStudyStart();
        return (study, first, second);
    }
}
