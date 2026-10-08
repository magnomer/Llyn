using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LRevisionClerk
{
    private readonly LVault _lRevisionClerkVault;
    private readonly LRevisionVault _lRevisionClerkRevisions;
    private readonly LWorkspaceVault _lRevisionClerkWorkspaces;

    public LRevisionClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lRevisionClerkVault = rig.LRigVault;
        _lRevisionClerkRevisions = rig.LRigRevisions;
        _lRevisionClerkWorkspaces = rig.LRigWorkspaces;
    }

    public LRevision LRevisionClerkRecord(IReadOnlyList<LRevisionDelta> changes)
    {
        ArgumentNullException.ThrowIfNull(changes);

        using LVaultSession session = _lRevisionClerkVault.LVaultSessionStart();

        LRevision revision = _lRevisionClerkRevisions.LRevisionRecord(changes);
        LWorkspaceState state = _lRevisionClerkWorkspaces.LWorkspaceStateRead();
        _lRevisionClerkWorkspaces.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

        session.LVaultSessionCommit();
        return revision;
    }

    internal void LRevisionClerkRecord(long target, string subject, bool fresh, string? summary)
    {
        LRevisionClerkRecord([new LRevisionDelta(target, subject, fresh ? "create" : "update", summary)]);
    }
}
