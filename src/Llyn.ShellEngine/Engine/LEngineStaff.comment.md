# LEngineStaff.cs
Hash: `828d5b3b4e3d5a16`

## `internal sealed record LEngineStaff(...)`

The record keeps every clerk and engine helper that belongs to the current rig.
The clerks sit in five group records, each one concern a reader finds by name.
The engine swaps one record when the workspace changes.
Each facade reads its concern from this record through the group that owns it.

**Parameters**

- `LEngineStaffCatalog` holds the clerks for the catalog rows a card or citation names.
- `LEngineStaffClaim` holds the clerks for held drafts and their claims.
- `LEngineStaffLanguage` holds the clerks for language data and the background fetches.
- `LEngineStaffEntry` holds the clerks for the entry lifecycle, its search and its citations.
- `LEngineStaffWorkspace` holds the clerks for workspace state, markup, portraits and the Joplin push.

## `internal static LEngineStaff LEngineStaffBuild(LRig rig, object gate, Action<LSubject, long> raise, Func<LSettings> settings, IReadOnlySet<long> retired)`

Builds the staff in dependency order over one rig.
It makes the identity issuer, the language cache and the revision clerk the groups share.
Each group is built after every group it reads a clerk from.
The identity issuer receives the stale ids, so the new workspace never issues one a tenure still holds.
One revision clerk stamps for the translation, entry, citation and intake clerks, so history stays one list.
