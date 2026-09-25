# LImageVault.cs

## `public interface LImageVault`

The persistence port for the Image rows the engine reads and writes.
It lists exactly what the engine asks of image storage, and nothing about how rows are kept.
`LImageArchive` in Infrastructure is its adapter over the workspace database.

## `LImage LImageCreate(LImage image);`

Inserts `image` with a fresh opaque id and returns the stored Image with that id filled in.
The new Image is referenced by nothing until it is attached to a referrer.

## `LImage? LImageRead(long id);`

Reads the Image identified by `id`, or `null` when no such Image exists.

## `IReadOnlyList<LImage> LImageMeaningRead(long meaningId);`

Reads the Images a Meaning references, in the order that Meaning gives them.

## `IReadOnlyList<LImage> LImageCollocationRead(long collocationId);`

Reads the Images a Collocation references, in the order that Collocation gives them.

## `IReadOnlyList<LImage> LImageSituationRead(long situationId);`

Reads the Images a Situation references, in the order that Situation gives them.

## `void LImageUpdate(LImage image);`

Rewrites the location of the Image `image` names.
Every referrer sees the change at once, because a reference points at the row and never at its text.

## `void LImageMeaningAttach(long meaningId, long imageId, int position);`

References the Image from a Meaning at the index named, renumbering that Meaning's other references around it.

## `void LImageCollocationAttach(long collocationId, long imageId, int position);`

References the Image from a Collocation at the index named, renumbering that Collocation's other references around it.

## `void LImageSituationAttach(long situationId, long imageId, int position);`

References the Image from a Situation at the index named, renumbering that Situation's other references around it.

## `void LImageMeaningDetach(long meaningId, long imageId);`

Drops a Meaning's reference and closes the gap it leaves.
The Image and every other reference stay.

## `void LImageCollocationDetach(long collocationId, long imageId);`

Drops a Collocation's reference and closes the gap it leaves.
The Image and every other reference stay.

## `void LImageSituationDetach(long situationId, long imageId);`

Drops a Situation's reference and closes the gap it leaves.
The Image and every other reference stay.
