using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineScriptFault
{
    private const string TFaultPack =
        """
        { "script": [
            { "name": "Seal", "url": "https://example.test/seal/search",
              "form": { "Character": "{word}" },
              "match": "<td class=\"Cell\"><img src=\"([^\"]+)\" />(.*?)<td>",
              "image": 1, "caption": 2, "prefix": "https://example.test" } ] }
        """;

    private const string TFaultText = "script source broke";

    private static readonly TimeSpan TFaultLimit = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task ScriptStart_SourceThrows_HandsFaultToOwnerOnce()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TFaultPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new InvalidOperationException(TFaultText)));
        LEntry entry = engine.TEngineEntrySave(
            TEngineFrequency.TFrequencyDraftCreate("整", pack.TLanguageFixtureName));

        engine.TEngineScriptStart(entry.LEntryId);
        DateTime limit = DateTime.UtcNow + TFaultLimit;
        while ((TEngineFanqieFault.TFaultRead(workspace, TFaultText) == 0
                || engine.TEngineScriptCheck(entry.LEntryId))
            && DateTime.UtcNow < limit)
        {
            await Task.Delay(20);
        }

        Assert.Equal(1, TEngineFanqieFault.TFaultRead(workspace, TFaultText));
    }
}
