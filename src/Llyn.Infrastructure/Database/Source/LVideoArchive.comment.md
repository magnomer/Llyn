# LVideoArchive.cs
Hash: `2f3b4677e2b515dd`

## `public sealed class LVideoArchive : LVideoVault`

Stores the independent Video and the references that reach it from a Meaning, a Collocation, or a Situation.
It mirrors `LImageArchive` row for row, because a Video is kept exactly as an Image is.
It keeps one thing an Image has no use for, the span of the film worth watching.
The span is held as written and read as written.
A Video is owned by nothing, so a reference is a link and never containment.
The order a Video appears in lives on each reference, and every write renumbers that referrer's set.

## `public LVideo LVideoCreate(LVideo video)`

Returns `video` with the id the store gave it, and the new Video is referenced by nothing yet.
An unreadable location or span is refused before any row is written.

## `public void LVideoUpdate(LVideo video)`

Rewrites the location and span of the Video `video` names, and never its id.
An id no row carries throws, so a lost row is never mistaken for a saved one.

## `public void LVideoMeaningAttach(long meaningId, long videoId, int position)`

A Video the Meaning already references keeps its single link and moves to `position`.
A position outside the set clamps to its nearest end.
The Collocation and Situation attaches follow the same rule.

## `public void LVideoMeaningDetach(long meaningId, long videoId)`

Detaching a Video the Meaning does not reference is no error.
The Video row stays even when no referrer remains.
The Collocation and Situation detaches follow the same rule.
