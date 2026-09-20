using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineClock
{
    [Fact]
    public void DraftStart_FrozenClock_StampsFrozenMoment()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        DateTimeOffset moment = new(2026, 3, 4, 5, 6, 7, TimeSpan.Zero);
        workspace.TWorkspaceClockSet(() => moment);

        LDraft started = engine.TEngineDraftStart("editor", null);

        Assert.Equal(moment, started.LDraftMoment);
        Assert.Equal(moment, engine.TEngineDraftRead(started.LDraftId)?.LDraftMoment);
    }

    [Fact]
    public void AuthorStart_FrozenClock_StampsFrozenMoment()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        DateTimeOffset moment = new(2026, 3, 4, 5, 6, 7, TimeSpan.Zero);
        workspace.TWorkspaceClockSet(() => moment);

        LDraft started = engine.TEngineAuthorStart("editor", null);

        Assert.Equal(moment, started.LDraftMoment);
    }
}
