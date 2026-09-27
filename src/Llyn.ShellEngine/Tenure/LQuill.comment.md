# LQuill.cs

## `public sealed class LQuill`

The typed text edits of one tenure, each building exactly one request.
A driver hands it raw values, and it chooses whether the request is sent or deferred.
Which member runs for a field is the caller's decision, not the quill's.

## `private readonly LTenure _lQuillTenure;`

The tenure every request is built for and handed to.

## `public LQuill(LTenure tenure)`

Builds the quill over one tenure, which it never swaps.

## `public void LQuillAuthorSet(string name)`

Defers the Author's typed name.

## `public void LQuillExampleSet(string text)`

Defers the held Example's typed text, always as known.
Typing is what makes an unknown text known.

## `public void LQuillSpeakerSet(string language)`

Sends the held Example's chosen language at once, since it came from a click.

## `public void LQuillReferenceSet(long reference)`

Points the held Example at the picked Source, sent at once.

## `public void LQuillGlossAdd(long card, long sentence, string language, int position)`

Adds a Gloss in the given language at the given position, sent at once.
The card and sentence are zero when the gloss belongs to the held Example itself.

## `public void LQuillGlossRemove(long card, long sentence, long gloss)`

Drops one Gloss, sent at once.

## `public void LQuillGlossSet(long card, long sentence, long gloss, string? language, string? text)`

Sends a chosen language at once when one is given.
Otherwise it defers the typed text, which must then be given.

## `public void LQuillMentionAdd(long card, long sentence, int offset, int length, long entry)`

Links a span of the text to an Entry, or marks it silent when the entry is zero.
The sense starts unset, since the sense picker opens only on a linked Mention.

## `public void LQuillMentionRemove(long card, long sentence, long mention)`

Drops one Mention, sent at once.

## `public void LQuillMentionSet(long card, long sentence, long mention, long sense)`

Points one Mention at a sense of its Entry, sent at once.

## `public void LQuillSituationSet(string title, string description, string kind)`

Defers the scenario's title, description and kind as one body.
The held situation is read without a flush, so a pending keystroke keeps its delay.

## `private void LQuillSituationDefer(LSituation? held, string title, string description, string kind)`

Builds the body over the held situation, taken as a parameter so no engine answer sits in a local.

## `private static LStateWritten LQuillWrittenRead(string text, LStateValue? held)`

An empty field whose held value is unknown stays unknown.
Any other text is written as it stands.
