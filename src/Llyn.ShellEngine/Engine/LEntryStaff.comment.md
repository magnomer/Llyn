# LEntryStaff.cs
Hash: `1a9f50bd21fe012e`

## `internal sealed record LEntryStaff(LMeaningClerk LEntryStaffMeaning, LEntryClerk LEntryStaffEntry, LEntryQueryClerk LEntryStaffQuery, LGraspClerk LEntryStaffGrasp, LOutcomeClerk LEntryStaffOutcome, LCitationClerk LEntryStaffCitation, LFoldClerk LEntryStaffFold)`

The entry lifecycle, search, commit, citation and fold clerks share this staff group.

**Parameters**

- `LEntryStaffMeaning` manages meaning operations.
- `LEntryStaffEntry` manages the entry lifecycle.
- `LEntryStaffQuery` searches and counts stored entries.
- `LEntryStaffGrasp` manages the user's grasp of an entry.
- `LEntryStaffOutcome` runs the commit round of an entry draft.
- `LEntryStaffCitation` manages citations.
- `LEntryStaffFold` keeps card folds, per-entry "More readings" state and editor-box state.

## `internal static LEntryStaff LEntryStaffBuild(LRig rig, LIdentity identity, LLanguageCache cache, LRevisionClerk revision, LCatalogStaff catalog, LClaimStaff claim, LLanguageStaff language)`

Builds the entry clerks over one rig after the catalog, claim and language groups.
The card and inflection clerks are built here and kept only inside the meaning and entry clerks.
The entry and citation clerks stamp through the shared revision clerk, so history stays one list.
