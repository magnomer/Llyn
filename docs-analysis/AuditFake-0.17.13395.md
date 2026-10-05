# Fake audit 0.17.13395

- Generation: 21
- Enforced: True
- Orphan: 23, ceiling 0
- Tested: 0, ceiling 0
- Above ceiling: 1
- Stale ceilings: 0

A member is live when a live reader, a constructor, an override, generated code, markup or the serializer reads it. Orphan is read by nothing live. Tested is read only by tests.

## Orphan

- `src/Llyn.Core/Configuration/LSettings.cs:13` `LSettingsOutpost`: read by nothing
- `src/Llyn.Core/Configuration/LSettings.cs:14` `LSettingsWarrant`: read by nothing
- `src/Llyn.Core/Outpost/LManifest.cs:5` `LManifestDigest`: read by nothing
- `src/Llyn.Core/Outpost/LManifestVault.cs:5` `LManifestRead`: read by nothing
- `src/Llyn.Core/Outpost/LManifestVault.cs:7` `LManifestSave`: read by nothing
- `src/Llyn.Core/Outpost/LOutpost.cs:9` `LOutpostFind`: read by nothing
- `src/Llyn.Core/Outpost/LOutpost.cs:11` `LOutpostWarrantStart`: read by nothing
- `src/Llyn.Core/Outpost/LOutpost.cs:13` `LOutpostWarrantCheck`: read by nothing
- `src/Llyn.Core/Outpost/LOutpost.cs:15` `LOutpostFolderSave`: read by nothing
- `src/Llyn.Core/Outpost/LOutpost.cs:17` `LOutpostNoteSave`: read by nothing
- `src/Llyn.Core/Outpost/LOutpost.cs:19` `LOutpostNoteRemove`: read by nothing
- `src/Llyn.Core/Outpost/LOutpost.cs:21` `LOutpostTagSave`: read by nothing
- `src/Llyn.Core/Outpost/LOutpost.cs:23` `LOutpostParcelSave`: read by nothing
- `src/Llyn.Core/Outpost/LReceipt.cs:6` `LReceiptSaved`: read by nothing
- `src/Llyn.Core/Outpost/LReceipt.cs:7` `LReceiptKept`: read by nothing
- `src/Llyn.Core/Outpost/LReceipt.cs:8` `LReceiptRemoved`: read by nothing
- `src/Llyn.Core/Outpost/LReceipt.cs:9` `LReceiptFailed`: read by nothing
- `src/Llyn.Core/Outpost/LWarrant.cs:5` `LWarrantHide`: read by nothing
- `src/Llyn.Core/Outpost/LWarrant.cs:7` `LWarrantRestore`: read by nothing
- `src/Llyn.Core/Outpost/LWarrantAnswer.cs:3` `LWarrantAnswerState`: read by nothing
- `src/Llyn.Core/Outpost/LWarrantAnswer.cs:3` `LWarrantAnswerToken`: read by nothing
- `src/Llyn.Core/Vault/Base/LRig.cs:51` `LRigOutpost`: read by nothing
- `src/Llyn.Core/Vault/Base/LRig.cs:58` `LRigWarrant`: read by nothing

## Tested

## Above ceiling

- Orphan: 23 hit(s), ceiling 0

## Stale ceilings
