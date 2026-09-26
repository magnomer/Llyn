# TEtymology.cs

## `public sealed class TEtymology`

Covers the etymology an entry declares: the two shapes, the spans of the narrative, and what a commit stores.

## `public void EtymologyCommit_ProseBesideLinks_StoresTheProseAlone()`

A draft may carry both shapes, and the store keeps one.

## `public void EtymologyCommit_ProseCleared_LeavesTheLinksAlone()`

Clearing the narrative demotes the etymology to its links, in the order they were added.

## `public void EtymonShift_MovedLink_StoresTheNewOrder()`

A moved link keeps its new place through a commit.

## `public void EtymonRemoval_LastLink_ClearsTheEtymology()`

Dropping the last link leaves the entry saying nothing about its origin.

## `public void EtymonAddition_UnknownOrRepeatedEntry_RefusesTheLink()`

A link names a stored entry once, and either fault refuses rather than writing a broken row.

## `public void EtymologyMention_TextOutsideTheBasicPlane_CountsInCodePoints()`

An offset is a code-point index, so a surrogate pair counts as one.

## `public void EtymologyMention_SpanPastTheText_RefusesTheSpan()`

A span that does not fit the text is refused before it reaches the draft.

## `public void EtymologyText_Shortened_DropsTheSpanItCutAway()`

Rewriting the narrative drops a span the new text no longer holds.

## `public void EtymologyMention_OverAStandingSpan_ReplacesIt()`

A new span takes the place of every span it overlaps.

## `public void EtymologyMention_NamingNoEntry_ClearsTheSpanUnderIt()`

A span of an etymology always names an entry, so naming none is how one is dropped.

## `public void EtymologyCommit_ChangedEtymology_RecordsOneRevisionChange()`

The revision names the etymology once, with the narrative as its summary.

## Inline notes

### `private static long TEtymologyEntryCreate(LEngine engine, string headword)`

One bare stored entry, since only its id matters to a link.

### `private static LEtymologyDraft TEtymologyRead(LEngine engine, long entryId)`

The etymology as the store gives it back, not as the draft held it.

### `private static LEtymologyDraft TEtymologyHeldRead(LEngine engine, long draftId)`

The etymology of the open draft, for the rules that refuse before a commit.
