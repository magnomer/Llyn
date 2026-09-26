using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEstablishment
{
    [Fact]
    public void EstablishmentRead_FreshWorkspace_CountsNothingUnsaved()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEstablishment establishment = engine.TEngineEstablishmentRead();

        Assert.Equal(0, establishment.LEstablishmentUnsaved);
        Assert.Equal(0, establishment.LEstablishmentEntry);
        Assert.True(establishment.LEstablishmentSize > 0);
    }

    [Fact]
    public void EstablishmentRead_TypedDraft_CountsOneUnsaved()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft blank = engine.TEngineDraftStart("Input", null);
        LDraft typed = engine.TEngineDraftStart("Library", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(typed.LDraftId, "kindle"));

        LEstablishment establishment = engine.TEngineEstablishmentRead();

        Assert.Equal(1, establishment.LEstablishmentUnsaved);
        Assert.NotNull(engine.TEngineDraftRead(blank.LDraftId));
    }

    [Fact]
    public void EstablishmentRead_CommittedDraft_CountsEntryNotUnsaved()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TRequestContentApply(started.LDraftId, TInterface.TDraftPlainCreate("kindle"));
        engine.TEngineDraftCommit(started.LDraftId);

        LEstablishment establishment = engine.TEngineEstablishmentRead();

        Assert.Equal(0, establishment.LEstablishmentUnsaved);
        Assert.Equal(1, establishment.LEstablishmentEntry);
    }

    [Fact]
    public void DraftCancel_TypedDraft_RaisesDraftBulletinWithZeroId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "kindle"));
        List<LBulletin> bulletins = [];
        engine.TEngineObserverAttach(bulletins.Add);

        engine.TEngineDraftCancel(started.LDraftId);

        LBulletin bulletin = Assert.Single(bulletins);
        Assert.Equal(LSubject.LSubjectDraft, bulletin.LBulletinSubject);
        Assert.Equal(0, bulletin.LBulletinId);
        Assert.Equal(0, engine.TEngineEstablishmentRead().LEstablishmentUnsaved);
    }
}
