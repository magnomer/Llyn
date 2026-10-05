# LManifest.cs
Hash: `43c8b7aa272af521`

## `public sealed record LManifest(IReadOnlyDictionary<string, string> LManifestDigest, string LManifestRealm);`

What Llyn last pushed to Joplin, kept so the next push sends only what changed.
The digest covers everything pushed for one note: title, notebook, body and the sorted tag set.
A change to any of them must push the note again.
A note whose digest matches is skipped.
A note id present here but absent from the workspace is trashed.
The realm binds the manifest to the workspace that wrote it.
A rebuilt database or a foreign file must never drive trashing.

**Parameters**

- `LManifestDigest` — Maps each pushed note id to the digest of everything last pushed for it.
- `LManifestRealm` — The workspace realm in `Guid` "N" format, or empty when unknown.
