# TVaultDraft.cs

## `public sealed class TVaultDraft`

Covers `LDraftArchive` keeping the promise of `LDraftVault` on a real workspace folder.
The test sees only the port, so it proves what the engine may rely on.
A draft read after it was saved comes back under its id with its content.
A scan lists every draft saved.
A deleted draft reads as `null` and no longer appears in the scan.
