# LVideoArchive.cs
Hash: `85ebdcb8704b775f`

## `public sealed class LVideoArchive : LVideoVault`

Stores the independent Video and the references that reach it from a Meaning, a Collocation, or a Situation.
It mirrors `LImageArchive` in behaviour, because a Video is kept exactly as an Image is.
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

## `public Dictionary<long, List<LVideoDraft>> LVideoSituationScan()`

Every situation-to-video link joined to its Video, ordered by parent and position, bucketed by parent.
It carries the span beside the location.
The Situation store fills a whole list from it in one query rather than one per Situation.

## `public void LVideoSituationClear(long situationId)`

Drops every Video link of one Situation, and leaves the Video rows standing.
The Situation store calls it inside its delete, so the session nests into that one.
