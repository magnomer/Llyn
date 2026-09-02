# LImageArchive.cs

## `public sealed class LImageArchive`

Persists Images — independent data no Entry, Meaning, or Collocation owns. An Image is created once with an opaque id, then *referenced* by any number of Meanings and Collocations through the association tables, each carrying the position the Image takes for that referrer alone. Attaching and detaching therefore only ever write association rows: detaching leaves the Image and its other references untouched, updating rewrites the location and never the id, and `LImageDelete` refuses to run while any reference remains.

A referrer's order is a unique index, so attaching and detaching renumber that referrer's whole set through `LDatabaseOrder`: a caller names the index it wants and never has to find a free position or leave a gap behind.

## `public LImageArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LImage LImageCreate(LImage image)`

Inserts `image` with a fresh opaque id and returns the stored Image with that id filled in. The new Image is referenced by nothing until it is attached to a referrer.

## `public LImage? LImageRead(string id)`

Reads the Image identified by `id`, or `null` when no such Image exists.

## `public IReadOnlyList<LImage> LImageSenseRead(string senseId)`

Reads the Images a Meaning references, in the order that Meaning gives them.

## `public IReadOnlyList<LImage> LImageCollocationRead(string collocationId)`

Reads the Images a Collocation references, in the order that Collocation gives them.

## `public void LImageUpdate(LImage image)`

Rewrites the location of the Image `image` names. Every referrer sees the change at once, because a reference points at the row and never at its text.

## `public int LImageReferenceRead(string id)`

Counts the references standing on the Image, across both sides. It is what a delete asks before it refuses.

## `public void LImageDelete(string id)`

Deletes the Image, refusing while any Meaning or Collocation still references it, so a live reference is never left pointing at a row that is gone.

## `public void LImageSenseAttach(string senseId, string imageId, int position)`

References the Image from a Meaning at the index named, renumbering that Meaning's other references around it.

## `public void LImageCollocationAttach(string collocationId, string imageId, int position)`

References the Image from a Collocation at the index named, renumbering that Collocation's other references around it.

## `public void LImageSenseDetach(string senseId, string imageId)`

Drops a Meaning's reference and closes the gap it leaves. The Image and every other reference stay.

## `public void LImageCollocationDetach(string collocationId, string imageId)`

Drops a Collocation's reference and closes the gap it leaves. The Image and every other reference stay.
