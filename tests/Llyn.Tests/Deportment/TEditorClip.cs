using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorClip
{
    private const string TEditorClipPack =
        """
        { "varieties": { "list": [ { "name": "British" } ] },
          "audio": [
            { "name": "Tagged", "attempts": [ { "urls": ["https://example.test/{word}"],
                "strategy": "regex", "match": "uk=(\\S+)", "group": 1 } ] } ] }
        """;

    [Fact]
    public async Task ClipStart_HeldDraft_DeliversStepsToLambda()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEditorClipPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=https://example.test/gb.mp3", Task.CompletedTask));
        engine.TEngineDelaySet(0);
        LEditor editor = TEditorOpen.TEditorPrepare(engine, "input");
        editor.TEditorOpen(null);
        editor.TEditorLanguageSet(pack.TLanguageFixtureName);
        editor.TEditorHeadwordSet("tomato");
        List<LHarvestStep> steps = [];
        TaskCompletionSource finished = new(TaskCreationOptions.RunContinuationsAsynchronously);

        editor.TEditorClipStart("tomato", 0, step =>
        {
            lock (steps)
            {
                steps.Add(step);
            }

            if (step.LHarvestStepEnded)
            {
                finished.TrySetResult();
            }
        });
        await finished.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(
            [LHarvestKind.LHarvestKindSource, LHarvestKind.LHarvestKindRecording, LHarvestKind.LHarvestKindEnd],
            steps.Select(step => step.LHarvestStepKind));
        Assert.Equal("Tagged", steps[0].LHarvestStepSource);
        Assert.Equal("https://example.test/gb.mp3", steps[1].LHarvestStepRecording?.LRecordingAddress);
        editor.TEditorFinish(false);
    }

    [Fact]
    public void ClipStepHandle_ThreeSteps_RaisesOneNoticeEach()
    {
        LClip clip = TInterfaceDeportment.TClipCreate();
        List<string> notices = [];
        clip.LClipSourceStarted += (source, order) => notices.Add($"source {source} {order}");
        clip.LClipRecordingAdded += recording => notices.Add($"recording {recording.LRecordingAddress}");
        clip.LClipFinished += () => notices.Add("end");
        LRecording recording = TInterface.TRecordingCreate("Tagged", "https://example.test/gb.mp3", 0, true, "British");

        clip.TClipStepHandle(TInterface.THarvestStepCreate(LHarvestKind.LHarvestKindSource, "Tagged", 0, null));
        clip.TClipStepHandle(TInterface.THarvestStepCreate(LHarvestKind.LHarvestKindRecording, "Tagged", 0, recording));
        clip.TClipStepHandle(TInterface.THarvestStepCreate(LHarvestKind.LHarvestKindEnd, string.Empty, 0, null));

        Assert.Equal(["source Tagged 0", "recording https://example.test/gb.mp3", "end"], notices);
    }
}
