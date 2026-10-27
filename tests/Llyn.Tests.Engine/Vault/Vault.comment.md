# Vault

These tests check each archive or loader against the port it promises to keep.
They run on a real workspace folder, or on the packs the test build copies beside it.
Each test sees only the port, so it proves what the engine may rely on.

## `TVaultDraft`

`LDraftArchive` keeps the promise of `LDraftVault`.
A draft read after it was saved comes back under its id with its content.
A scan lists every draft saved.
A deleted draft reads as `null` and no longer appears in the scan.

## `TVaultEntry`

`LEntryArchive` keeps the promise of `LEntryVault` on a real workspace database.
The whole content loaded for an entry agrees with the row read for it.
That content carries the entry's forms and speeches.
A find returns only entries whose headword matches, and every entry for an empty query.
An unknown id reads as `null`.

## `TVaultIdentity`

`LIdentity` mints temporary ids over the promise of `LWorkspaceVault`.
Two ids from one issuer descend, since temporary ids count down from the floor.
A fresh issuer over the same workspace continues below what the earlier one minted.
The floor lives in the store, not in the issuer.

## `TVaultLanguage`

`LLanguageLoader` keeps the promise of `LLanguageVault`.
A shipped pack reads back under its name with its sources.
The scan lists English first and the other listed packs after it.
An unknown pack reads as a blank pack of that name.
A name that would escape the folder is refused.

## `TVaultSettings`

`LSettingsLoader` keeps the promise of `LSettingsVault`.
Settings saved read back field for field, the gloss language among them.
The workspace then reports that it holds its own.
A save after a read that found the file locked is refused, so the stored file stays whole.
A workspace holding nothing answers the defaults and reports no settings of its own.

## Stamp

Commit: `9377e657b395`
- `TVaultDraft.cs` `5188669d10075914`
- `TVaultEntry.cs` `7274dce5508be06f`
- `TVaultIdentity.cs` `b1c9da224009716a`
- `TVaultLanguage.cs` `5a6d98c7c2b22306`
- `TVaultSettings.cs` `425d17d396ce2fe9`
