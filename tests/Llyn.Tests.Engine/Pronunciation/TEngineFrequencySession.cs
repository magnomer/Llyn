using System.Net;
using System.Net.Http;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFrequencySession
{
    [Fact]
    public async Task FrequencyRead_SourcesSilent_AsksOncePerSession()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("nothing here", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));

        LEntryDraft draft = TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName);
        LEntry entry = engine.TEngineEntrySave(draft);
        await TFrequencyCountCheck(handler, 3);
        await Task.Delay(200);

        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        await Task.Delay(200);

        Assert.Equal(3, handler.TSourceHandlerCount);
    }

    [Fact]
    public async Task FrequencyRead_FetchPending_KeepsFetchRunning()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TSourceHandler handler = new("a=7", HttpStatusCode.OK, gate.Task);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        TEngineFrequency.TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer.TFrequencyObserverHandle);

        LEntryDraft draft = TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName);
        LEntry entry = engine.TEngineEntrySave(draft);
        await TFrequencyCountCheck(handler, 1);
        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        await Task.Delay(200);
        gate.SetResult();
        await observer.TFrequencyObserverRaised.WaitAsync(TEngineFrequency.TEngineFrequencyPatience);

        Assert.Equal(3, handler.TSourceHandlerCount);
    }

    [Fact]
    public async Task FrequencyRead_SourcesUnreachable_AsksAgain()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(string.Empty, HttpStatusCode.ServiceUnavailable);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));

        LEntryDraft draft = TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName);
        LEntry entry = engine.TEngineEntrySave(draft);
        await TFrequencyCountCheck(handler, 3);
        await Task.Delay(200);

        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        await TFrequencyCountCheck(handler, 6);
    }

    private static async Task TFrequencyCountCheck(TSourceHandler handler, int count)
    {
        DateTime deadline = DateTime.UtcNow + TEngineFrequency.TEngineFrequencyPatience;
        while (handler.TSourceHandlerCount < count)
        {
            Assert.True(DateTime.UtcNow < deadline, $"Waited for {count} requests, saw {handler.TSourceHandlerCount}.");
            await Task.Delay(20);
        }
    }
}
