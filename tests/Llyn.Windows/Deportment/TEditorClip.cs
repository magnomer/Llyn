using Llyn.Conduct;
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
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "input");
        editor.TEditorOpen(null);
        editor.TEditorLanguageSet(pack.TLanguageFixtureName);
        editor.TEditorHeadwordSet("tomato");
        List<CHarvestStep> steps = [];
        TaskCompletionSource finished = new(TaskCreationOptions.RunContinuationsAsynchronously);

        editor.TEditorClipStart("tomato", 0, step =>
        {
            lock (steps)
            {
                steps.Add(step);
            }

            if (step.CHarvestStepEnded)
            {
                finished.TrySetResult();
            }
        });
        await finished.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(
            [(false, false), (true, false), (false, true)],
            steps.Select(step => (step.CHarvestStepRecording is not null, step.CHarvestStepEnded)));
        Assert.Equal("Tagged", steps[0].CHarvestStepSource);
        Assert.Equal("https://example.test/gb.mp3", steps[1].CHarvestStepRecording?.CRecordingAddress);
        editor.TEditorFinish(false);
    }

    [Fact]
    public void ClipStepHandle_ThreeSteps_RaisesOneNoticeEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LClip clip = TInterfaceDeportment.TClipCreate(engine);
        List<string> notices = [];
        clip.LClipSourceStarted += (source, order) => notices.Add($"source {source} {order}");
        clip.LClipRecordingAdded += recording => notices.Add($"recording {recording.CRecordingAddress}");
        clip.LClipFinished += () => notices.Add("end");
        CRecording recording = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");

        clip.TClipStepHandle(new CHarvestStep("Tagged", 0, null, false));
        clip.TClipStepHandle(new CHarvestStep("Tagged", 0, recording, false));
        clip.TClipStepHandle(new CHarvestStep(string.Empty, 0, null, true));

        Assert.Equal(["source Tagged 0", "recording https://example.test/gb.mp3", "end"], notices);
    }
}
