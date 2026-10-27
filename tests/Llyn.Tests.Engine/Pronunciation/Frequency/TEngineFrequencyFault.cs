using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFrequencyFault
{
    private const string TFaultText = "frequency source broke";

    [Fact]
    public async Task FrequencyStart_SourceThrows_HandsFaultToOwnerOnce()
    {
        using TLanguageFixture pack =
            TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new InvalidOperationException(TFaultText)));

        engine.TEngineEntrySave(TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        DateTime limit = DateTime.UtcNow + TEngineFrequency.TEngineFrequencyPatience;
        while (TEngineFanqieFault.TFaultRead(workspace, TFaultText) == 0
            && DateTime.UtcNow < limit)
        {
            await Task.Delay(20);
        }

        await Task.Delay(300);
        Assert.Equal(1, TEngineFanqieFault.TFaultRead(workspace, TFaultText));
    }
}
