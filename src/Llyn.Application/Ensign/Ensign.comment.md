# Ensign

The Ensign folder holds the cache half of the flag.
It remembers which flag keys were asked and the SVG path each resolved to.
A key is a language name, or a language and variety joined by a slash.
The engine owns one cache and fills it, and the veneer keeps only the drawings.
Path facts and file deletion are asked of the `LUsher` handed in.
So no file check or deletion happens in this ring or above it.

## `LEnsign`

The usher is an `LUsherFile` in the engine and a recording fake in a test.

### `LEnsignKeyFormat`

The key is the language and the variety name joined by a slash.
`QEnsignImage` spells the same key on its side, so a row hands no logic value down to ask for one.

### `_lEnsignAge`

Counts workspace changes, so a fetch that began before one is dropped when it lands.
`LEnsignClear` forgets every path and advances the age.

### `LEnsignMissingRead`

Answers the keys not yet asked, each once, with the age the caller hands back when the paths arrive.

### `LEnsignPathAdd`

Records the path each key resolved to, null when the usher finds nothing there.
It throws when keys and paths differ in length, since each key needs its own path.
It hands the rows kept to the store seam.
It adds the delete a surface calls for a file it cannot draw.
An age that no longer matches records nothing and calls nothing, since the workspace has moved on.
The seam parses outside the lock, so no reader or clear waits on a file.
It answers a commit, which runs inside the lock only while the age still matches.
So a workspace move between the parse and the commit stores nothing.

### `LEnsignPathDelete`

Forgets every key that resolved to the path, so the next load asks for it again.
It asks the usher to remove a cached SVG the renderer could not read, so the next fetch replaces it.
When the usher judges the exception a lock or a refusal, the file is kept.
Such a lock may be brief, so deleting would throw away a good flag.
The forgotten key lets the next load draw it once the lock is gone.

## `LEnsignRow`

One flag the cache kept, pairing the key it was asked under with the SVG path on disk.
The veneer draws the path and stores the drawing under the key.

## Stamp

Commit: `c2f335c331fe`
- `LEnsign.cs` `ab840c99ea01ed2a`
- `LEnsignRow.cs` `3c89e4a238b67f9b`
