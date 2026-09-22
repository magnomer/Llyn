# LDraftClerkEtymology.cs

## `public sealed class LDraftClerkEtymology`

Applies the etymology requests to the entry draft being edited.
It keeps both shapes on the draft and picks neither, which the entry clerk does when saving.
It holds the entry store to check that a link or a span names a real Entry.

## `public LEntryDraft? LEtymologyApply(LEntryDraft content, LRequest request)`

Applies one etymology request and returns the changed draft.
It returns null for a request it does not own, so the draft clerk may try the next row clerk.

## `private static LEtymologyDraft LEtymologyTextApply(LEtymologyDraft etymology, string text)`

Sets the prose and carries the spans across the edit.
`LDraftClerkMention` does the carrying, so etymology prose and sentence text shift alike.

## `private LEtymologyDraft LEtymologyMentionApply(LEtymologyDraft etymology, int offset, int length, long entryId)`

Puts one span over the range given, or clears the range when no Entry is named.
Every span the range touches is dropped first, so no two spans overlap.
A range outside the text is refused, as is a span naming an Entry the store does not hold.

## `private LEtymologyDraft LEtymonAdd(LEtymologyDraft etymology, long entryId, int position)`

Inserts a direct link at the position given, clamped into the list.
A link the draft already holds is refused.

## `private static LEtymologyDraft LEtymonRemove(LEtymologyDraft etymology, long entryId)`

Drops the direct link to the Entry named, leaving the rest in order.

## `private static LEtymologyDraft LEtymonMove(LEtymologyDraft etymology, long entryId, int position)`

Moves one direct link to another place in the list.
A link the draft does not hold is refused.

## `private void LEtymologyValidate(long entryId)`

Refuses an id that is not a positive id of an Entry the store holds.

## `private static int LEtymologyLengthRead(string text)`

The length of the prose in code points, which is what a span offset counts.

## `private static LEntryDraft LEtymologyChange(LEntryDraft content, Func<LEtymologyDraft, LEtymologyDraft> change)`

Puts the changed etymology back on the draft.
