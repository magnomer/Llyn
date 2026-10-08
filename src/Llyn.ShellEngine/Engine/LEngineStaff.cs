using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed record LEngineStaff(
    LCatalogStaff LEngineStaffCatalog,
    LClaimStaff LEngineStaffClaim,
    LLanguageStaff LEngineStaffLanguage,
    LEntryStaff LEngineStaffEntry,
    LWorkspaceStaff LEngineStaffWorkspace)
{
    internal static LEngineStaff LEngineStaffBuild(
        LRig rig,
        object gate,
        Action<LSubject, long> raise,
        Func<LSettings> settings,
        IReadOnlySet<long> retired)
    {
        LIdentity identity = new(rig.LRigWorkspaces, retired);
        LLanguageCache cache = new(rig.LRigLanguages);
        LRevisionClerk revision = new(rig);
        LCatalogStaff catalog = LCatalogStaff.LCatalogStaffBuild(rig, cache, revision);
        LClaimStaff claim = LClaimStaff.LClaimStaffBuild(rig, identity, cache, catalog);
        LLanguageStaff language = LLanguageStaff.LLanguageStaffBuild(rig, cache, gate, raise, settings, claim);
        LEntryStaff entry = LEntryStaff.LEntryStaffBuild(rig, identity, cache, revision, catalog, claim, language);
        LWorkspaceStaff workspace = LWorkspaceStaff.LWorkspaceStaffBuild(
            rig, cache, revision, gate, settings, catalog, claim, language, entry);

        return new LEngineStaff(catalog, claim, language, entry, workspace);
    }
}
