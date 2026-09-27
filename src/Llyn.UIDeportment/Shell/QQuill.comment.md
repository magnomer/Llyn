# QQuill.cs

## `public sealed class QQuill`

The gate for the text rows of one desk.
A driver hands it the texts it gathered, and the gate builds the request and every written state inside.
It is sealed, so its public members name only .NET types and Conduct shapes.
It takes the `Q` prefix until the gates move to Conduct, where it becomes `CQuill`.

## `internal QQuill(LDesk desk)`

Only the desk builds one, over itself.

## `public void QQuillSituationChange(string title, string description, string kind)`

Defers the scenario's title, description and kind as one body, as the replaced typing request did.
The draft is read through `LDeskDraft` without a flush, so a pending keystroke keeps its delay.
A flush here would raise the draft bulletin mid-keystroke and redraw the field under the caret.

## `private void QQuillSituationDefer(CSituationDraft? held, string title, string description, string kind)`

Builds the body over the held situation, taken as a parameter so no engine answer sits in a local.

## `private static LStateWritten QQuillWrittenRead(string text, CStateValue? held)`

An empty field whose held value is unknown stays unknown.
Any other text is written as it stands.

## `public void QQuillAuthorSet(string name)`

Defers the Author's typed name, as the replaced keystroke request did.

## `public void QQuillExampleChange(CExampleField field, string value)`

Changes the held Example's language or text, as the corpus transcript gathered it.
A chosen language is sent at once, since it came from a click.
Typed text is deferred and always written as known, since typing is what makes an unknown text known.

## `public void QQuillCitationSet(long reference)`

Points the held Example at the Source picked from the citation drawer, sent at once.

## `public void QQuillGlossAdd(long card, long sentence, string language, int position)`

Adds a Gloss in the given language at the given position, sent at once.
The card and sentence are zero when the gloss belongs to the Example the desk holds itself.

## `public void QQuillGlossRemove(long card, long sentence, long gloss)`

Drops one Gloss, sent at once.

## `public void QQuillGlossChange(long card, long sentence, long gloss, CGlossField field, string value)`

Changes one Gloss's language or text.
A chosen language is sent at once, and typed text is deferred, as the requests they replace were.

## `public void QQuillMentionAdd(long card, long sentence, int offset, int length, long entry)`

Links a span of the text to an Entry, or marks it silent when the entry is zero.
The sense starts unset, since the sense picker opens only on a linked Mention.

## `public void QQuillMentionRemove(long card, long sentence, long mention)`

Drops one Mention, sent at once.

## `public void QQuillMentionChange(long card, long sentence, long mention, long sense)`

Points one Mention at a sense of its Entry, sent at once.
