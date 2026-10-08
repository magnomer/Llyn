using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed record LClaimStaff(
    LDraftClerk LClaimStaffDraft,
    LChronicleClerk LClaimStaffChronicle,
    LCourtClerk LClaimStaffCourt,
    LClaimClerk LClaimStaffClaim)
{
    internal static LClaimStaff LClaimStaffBuild(
        LRig rig, LIdentity identity, LLanguageCache cache, LCatalogStaff catalog)
    {
        LDraftClerk draft = new(rig, identity, cache);
        LChronicleClerk chronicle = new(rig);
        LCourtClerk court = new(rig, identity, chronicle, catalog.LCatalogStaffTranslation);
        LClaimClerk claim = new(rig, identity, chronicle, court);

        return new LClaimStaff(draft, chronicle, court, claim);
    }
}
