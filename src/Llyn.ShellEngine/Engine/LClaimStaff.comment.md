# LClaimStaff.cs
Hash: `fc2ed893e2af6438`

## `internal sealed record LClaimStaff(...)`

The group of clerks for held drafts, their history and their claims.

**Parameters**

- `LClaimStaffDraft` manages held drafts.
- `LClaimStaffChronicle` manages draft history.
- `LClaimStaffCourt` manages links from a held draft to a target not yet stored.
- `LClaimStaffClaim` manages held claims.

## `internal static LClaimStaff LClaimStaffBuild(LRig rig, LIdentity identity, LLanguageCache cache, LCatalogStaff catalog)`

Builds the claim clerks over one rig after the catalog group.
The court reads the catalog's translation clerk, so a pending link resolves through translations.
The draft, court and claim clerks share the one identity issuer, so no id is issued twice.
