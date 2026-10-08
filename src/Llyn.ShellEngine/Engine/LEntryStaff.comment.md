# LEntryStaff.cs
Hash: `1f84597a0b428839`

## `internal sealed record LEntryStaff(...)`

The group of clerks for the entry lifecycle, its search, its commit round and its citations.

**Parameters**

- `LEntryStaffMeaning` manages meaning operations.
- `LEntryStaffEntry` manages the entry lifecycle.
- `LEntryStaffQuery` searches and counts stored entries.
- `LEntryStaffGrasp` manages the user's grasp of an entry.
- `LEntryStaffOutcome` runs the commit round of an entry draft.
- `LEntryStaffCitation` manages citations.

## `internal static LEntryStaff LEntryStaffBuild(LRig rig, LIdentity identity, LLanguageCache cache, LRevisionClerk revision, LCatalogStaff catalog, LClaimStaff claim, LLanguageStaff language)`

Builds the entry clerks over one rig after the catalog, claim and language groups.
The card and inflection clerks are built here and kept only inside the meaning and entry clerks.
The entry and citation clerks stamp through the shared revision clerk, so history stays one list.
