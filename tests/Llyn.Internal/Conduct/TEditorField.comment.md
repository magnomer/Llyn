# TEditorField.cs

## `public sealed class TEditorField`

Covers the editor's typed field gates over an input editor on a real workspace, with no delay.
A card's title, expression and definition land as typed once the deferred requests are flushed.
A card's links read back ready, in the card's order, and an unknown card or an empty desk reads nothing.
An image's location, a video's location and span, and a gloss's text land as typed.
The close gate lets the held draft and every search go.
The editor reads its draft's language, and an opened workspace opens a fresh draft in it.
The observers the editor attaches show the draft through the marshal the driver hands.

## `private static CEditor TEditorFieldPrepare(LEngine engine)`

Builds an input editor with no delay and opens a fresh draft in it.

## `private static long TEditorSheetAdd(CEditor editor)`

Appends a Meaning card to the held draft and answers its id.

## `private static CCardDraft TEditorCardRead(CEditor editor, long sheet)`

The card as the editor's draft read shows it.
