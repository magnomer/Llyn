# LVideoVault.cs

## `public interface LVideoVault`

The persistence port for the Video rows the engine reads and writes.
It lists exactly what the engine asks of video storage, and nothing about how rows are kept.
`LVideoArchive` in Infrastructure is its adapter over the workspace database.

## `LVideo LVideoCreate(LVideo video);`

Inserts `video` with a fresh opaque id and returns the stored Video with that id filled in.
The new Video is referenced by nothing until it is attached to a referrer.

## `LVideo? LVideoRead(long id);`

Reads the Video identified by `id`, or `null` when no such Video exists.

## `IReadOnlyList<LVideo> LVideoMeaningRead(long meaningId);`

Reads the Videos a Meaning references, in the order that Meaning gives them.

## `IReadOnlyList<LVideo> LVideoCollocationRead(long collocationId);`

Reads the Videos a Collocation references, in the order that Collocation gives them.

## `IReadOnlyList<LVideo> LVideoSituationRead(long situationId);`

Reads the Videos a Situation references, in the order that Situation gives them.

## `void LVideoUpdate(LVideo video);`

Rewrites the location of the Video `video` names.
Every referrer sees the change at once, because a reference points at the row and never at its text.

## `void LVideoMeaningAttach(long meaningId, long videoId, int position);`

References the Video from a Meaning at the index named, renumbering that Meaning's other references around it.

## `void LVideoCollocationAttach(long collocationId, long videoId, int position);`

References the Video from a Collocation at the index named, renumbering that Collocation's other references around it.

## `void LVideoSituationAttach(long situationId, long videoId, int position);`

References the Video from a Situation at the index named, renumbering that Situation's other references around it.

## `void LVideoMeaningDetach(long meaningId, long videoId);`

Drops a Meaning's reference and closes the gap it leaves.
The Video and every other reference stay.

## `void LVideoCollocationDetach(long collocationId, long videoId);`

Drops a Collocation's reference and closes the gap it leaves.
The Video and every other reference stay.

## `void LVideoSituationDetach(long situationId, long videoId);`

Drops a Situation's reference and closes the gap it leaves.
The Video and every other reference stay.
