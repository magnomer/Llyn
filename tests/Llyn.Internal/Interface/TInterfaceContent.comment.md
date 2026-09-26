# TInterfaceContent.cs

## `internal static partial class TInterface`

The relays for the engine operations over what an entry owns.
That is meanings, collocations, examples, situations, tags, and the rest.
Each relay is transparent and carries no test logic of its own.

## `internal static IReadOnlyList<LPronunciationDraft> TEntryPronunciationRead(this LEngine engine, long entryId)`

The pronunciation rows of a stored entry as the entry form loads them, in their stored order.
A row's id is the stored pronunciation's id, and its audio path is resolved against the workspace.

## `internal static IReadOnlyList<LInflection> TEntryInflectionRead(this LEngine engine, long entryId)`

The inflections of a stored entry as the entry form loads them, in their stored order.

## `internal static LEntry TEntryInflectionSave(`

Rewrites the inflections of a stored entry through the entry update, the only seam that writes them.
The update also regrades each form against its paradigm.

## `internal static LSpeechValue? TSpeechValueFind(this LEngine engine, string language, string name)`

The part of speech named exactly `name` among those the speech picker lists for `language`, or null.

## `internal static LMorphology TParadigmMorphologyRead(this LEngine engine, long entryId, string name)`

The morphology behind the paradigm slot named `name` that the entry's form shows.
A test takes a morphology id from here, as the paradigm grid does.

## `internal static IReadOnlyList<LReflexDraft> TEntryReflexRead(this LEngine engine, long entryId)`

The reflex rows of a stored entry as the entry form loads them, in their stored order.

## `internal static LEntry TEntryAnchorApply(`

Anchors reflex rows of a stored entry to exactly the fanqie rows `anchors` names, as the anchor menu does.
Each row is toggled one fanqie row at a time in a draft, and the draft is committed once.
A `position` limits the change to that row, and without one every row gets the same anchors.
