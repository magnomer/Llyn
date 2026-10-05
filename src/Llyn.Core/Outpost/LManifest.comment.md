# LManifest.cs
Hash: `eac608bed9275105`

## `public sealed record LManifest(IReadOnlyDictionary<string, string> LManifestDigest);`

What Llyn last pushed to Joplin, kept so the next push sends only what changed.
The digest covers everything pushed for one note: title, notebook, body and the sorted tag set.
A change to any of them must push the note again.
A note whose digest matches is skipped.
A note id present here but absent from the workspace is trashed.

**Parameters**

- `LManifestDigest` — Maps each pushed note id to the digest of everything last pushed for it.
