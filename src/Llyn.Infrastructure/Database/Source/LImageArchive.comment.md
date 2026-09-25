# LImageArchive.cs

## `public sealed class LImageArchive`

Persists Images — independent data no Entry, Meaning, or Collocation owns.
An Image is created once with an opaque id.
It is then *referenced* by any number of Meanings, Collocations, and Situations through the association tables.
Each association carries the position the Image takes for that referrer alone.
Attaching and detaching therefore only ever write association rows.
Detaching leaves the Image and its other references untouched.
Updating rewrites the location and never the id.

A referrer's order is a unique index.
So attaching and detaching renumber that referrer's whole set through `LDatabaseOrder`.
A caller names the index it wants.
It never has to find a free position or leave a gap behind.

## `public LImageArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LImage LImageCreate(LImage image)`

Inserts `image` with a fresh opaque id and returns the stored Image with that id filled in.
The new Image is referenced by nothing until it is attached to a referrer.

## `public LImage? LImageRead(long id)`

Reads the Image identified by `id`, or `null` when no such Image exists.

## `public IReadOnlyList<LImage> LImageMeaningRead(long meaningId)`

Reads the Images a Meaning references, in the order that Meaning gives them.

## `public IReadOnlyList<LImage> LImageCollocationRead(long collocationId)`

Reads the Images a Collocation references, in the order that Collocation gives them.

## `public IReadOnlyList<LImage> LImageSituationRead(long situationId)`

Reads the Images a Situation references, in the order that Situation gives them.

## `public void LImageUpdate(LImage image)`

Rewrites the location of the Image `image` names.
Every referrer sees the change at once, because a reference points at the row and never at its text.

## `public void LImageMeaningAttach(long meaningId, long imageId, int position)`

References the Image from a Meaning at the index named, renumbering that Meaning's other references around it.

## `public void LImageCollocationAttach(long collocationId, long imageId, int position)`

References the Image from a Collocation at the index named, renumbering that Collocation's other references around it.

## `public void LImageSituationAttach(long situationId, long imageId, int position)`

References the Image from a Situation at the index named, renumbering that Situation's other references around it.

## `public void LImageMeaningDetach(long meaningId, long imageId)`

Drops a Meaning's reference and closes the gap it leaves.
The Image and every other reference stay.

## `public void LImageCollocationDetach(long collocationId, long imageId)`

Drops a Collocation's reference and closes the gap it leaves.
The Image and every other reference stay.

## `public void LImageSituationDetach(long situationId, long imageId)`

Drops a Situation's reference and closes the gap it leaves.
The Image and every other reference stay.
