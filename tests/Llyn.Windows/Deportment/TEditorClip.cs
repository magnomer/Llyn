using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorClip
{
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
