using System.IO;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFanqieFault
{
    private const string TFaultPack =
        """
        { "fanqie": [
            { "name": "Broad", "url": "https://example.test/broad",
              "form": { "word": "{word}", "t": "g" },
              "match": "<span class='string'>{word}</span>", "busy": "Too fast" } ] }
        """;

    private const string TFaultText = "fanqie source broke";

    private static readonly TimeSpan TFaultLimit = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task FanqieStart_SourceThrows_HandsTheFaultToTheOwnerOnce()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TFaultPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new InvalidOperationException(TFaultText)));
        LEntry entry = engine.TEngineEntrySave(TEngineFanqie.TFanqieDraftCreate("吳", pack.TLanguageFixtureName));

        engine.TEngineFanqieStart(entry.LEntryId);
        DateTime limit = DateTime.UtcNow + TFaultLimit;
        while (TFaultRead(workspace, TFaultText) == 0 && DateTime.UtcNow < limit)
        {
            await Task.Delay(20);
        }

        await TEngineFanqie.TFanqieSettle(engine, entry.LEntryId);
        Assert.Equal(1, TFaultRead(workspace, TFaultText));
    }

    internal static int TFaultRead(TWorkspace workspace, string text)
    {
        string audit = Path.Combine(workspace.TWorkspaceFolder, "audit.log");
        try
        {
            return File.Exists(audit)
                ? File.ReadAllText(audit).Split(text, StringSplitOptions.None).Length - 1
                : 0;
        }
        catch (IOException)
        {
            return 0;
        }
    }
}
