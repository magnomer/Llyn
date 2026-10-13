using System.IO;
using System.Net;
using System.Threading;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TForayFailure
{
    private const string TForayLookupPack =
        """
        { "pronunciation": [ { "name": "Stub", "attempts": [
            { "urls": ["https://example.test/{word}"], "strategy": "regex", "match": "m=(.+)", "group": 1 } ] } ] }
        """;

    private static readonly string TForayFault =
        TRigFake.TRigFaultRead(nameof(LSourceFactory.LSourceFactoryCreate));

    private static readonly TimeSpan TForayLimit = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task ForayRun_SearchThrows_RecordsTheFaultAndFinishes()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TForay.TForayPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new InvalidOperationException("foray source broke")));
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, pack.TLanguageFixtureName));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        tenure.TTenureRecordingStart(0, listener.TListenerStubHandle);
        await TForaySettle(() => TForayAuditRead(workspace).Contains("foray source broke", StringComparison.Ordinal)
            && listener.TListenerStubFinished > 0);

        Assert.Contains("foray source broke", TForayAuditRead(workspace));
        Assert.Equal(1, listener.TListenerStubFinished);
        tenure.TTenureCancel();
    }

    [Fact]
    public async Task ForayRun_LookupThrows_RecordsTheFaultAndFinishesOnce()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TForayLookupPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new InvalidOperationException("lookup source broke")));
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, pack.TLanguageFixtureName));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        int finished = 0;

        tenure.TTenureTranscriptionStart(
            0,
            string.Empty,
            step =>
            {
                if (step.LLookupStepKind == LLookupKind.LLookupKindEnd)
                {
                    Interlocked.Increment(ref finished);
                }
            });
        await TForaySettle(() => TForayAuditRead(workspace).Contains("lookup source broke", StringComparison.Ordinal)
            && Volatile.Read(ref finished) > 0);

        Assert.Contains("lookup source broke", TForayAuditRead(workspace));
        Assert.Equal(1, Volatile.Read(ref finished));
        tenure.TTenureCancel();
    }

    [Fact]
    public async Task ForayRun_SourceAnswers_FinishesOnce()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TForay.TForayPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=https://example.test/gb.mp3", HttpStatusCode.OK));
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, pack.TLanguageFixtureName));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        tenure.TTenureRecordingStart(0, listener.TListenerStubHandle);
        await TForaySettle(() => listener.TListenerStubFinished > 0);

        Assert.Equal(["Tagged"], listener.TListenerStubSources);
        Assert.Equal(1, listener.TListenerStubFinished);
        Assert.False(File.Exists(Path.Combine(workspace.TWorkspaceFolder, "audit.log")));
        tenure.TTenureCancel();
    }

    [Fact]
    public async Task ForayRun_RecordingSourcesUnbuilt_RecordsTheFaultAndFinishesOnce()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TForay.TForayPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TRigFake.TRigSourceCreate());
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, pack.TLanguageFixtureName));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        tenure.TTenureRecordingStart(0, listener.TListenerStubHandle);
        await TForaySettle(() => TForayAuditRead(workspace).Contains(TForayFault, StringComparison.Ordinal)
            && listener.TListenerStubFinished > 0);

        Assert.Contains(TForayFault, TForayAuditRead(workspace));
        Assert.Empty(listener.TListenerStubSources);
        Assert.Equal(1, listener.TListenerStubFinished);
        tenure.TTenureCancel();
    }

    [Fact]
    public async Task ForayRun_TranscriptionSourcesUnbuilt_RecordsTheFaultAndFinishesOnce()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TForayLookupPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TRigFake.TRigSourceCreate());
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, pack.TLanguageFixtureName));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        int sources = 0;
        int finished = 0;

        tenure.TTenureTranscriptionStart(
            0,
            string.Empty,
            step =>
            {
                if (step.LLookupStepKind == LLookupKind.LLookupKindSource)
                {
                    Interlocked.Increment(ref sources);
                }
                else if (step.LLookupStepKind == LLookupKind.LLookupKindEnd)
                {
                    Interlocked.Increment(ref finished);
                }
            });
        await TForaySettle(() => TForayAuditRead(workspace).Contains(TForayFault, StringComparison.Ordinal)
            && Volatile.Read(ref finished) > 0);

        Assert.Contains(TForayFault, TForayAuditRead(workspace));
        Assert.Equal(0, Volatile.Read(ref sources));
        Assert.Equal(1, Volatile.Read(ref finished));
        tenure.TTenureCancel();
    }

    [Fact]
    public async Task ForayRun_SinkThrowsOnEnd_RecordsBothFaultsAndEndsOnce()
    {
        const string sinkFault = "The test sink refuses the end.";
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TForayLookupPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TRigFake.TRigSourceCreate());
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, pack.TLanguageFixtureName));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        int finished = 0;

        tenure.TTenureTranscriptionStart(
            0,
            string.Empty,
            step =>
            {
                if (step.LLookupStepKind == LLookupKind.LLookupKindEnd)
                {
                    Interlocked.Increment(ref finished);
                    throw new InvalidOperationException(sinkFault);
                }
            });
        await TForaySettle(() => TForayAuditRead(workspace).Contains(sinkFault, StringComparison.Ordinal)
            && Volatile.Read(ref finished) > 0);

        string written = TForayAuditRead(workspace);
        Assert.Contains(TForayFault, written);
        Assert.Contains(sinkFault, written);
        Assert.Equal(1, Volatile.Read(ref finished));
        tenure.TTenureCancel();
    }

    private static async Task TForaySettle(Func<bool> condition)
    {
        DateTime limit = DateTime.UtcNow + TForayLimit;
        while (!condition() && DateTime.UtcNow < limit)
        {
            await Task.Delay(20);
        }

        Assert.True(condition(), $"The awaited condition did not hold within {TForayLimit.TotalSeconds} seconds.");
    }

    private static string TForayAuditRead(TWorkspace workspace)
    {
        string audit = Path.Combine(workspace.TWorkspaceFolder, "audit.log");
        try
        {
            return File.Exists(audit) ? File.ReadAllText(audit) : string.Empty;
        }
        catch (IOException)
        {
            return string.Empty;
        }
    }
}
