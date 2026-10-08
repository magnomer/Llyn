# TEditorField.cs
Hash: `b291a292b2a1046c`

## `public sealed class TEditorField`

Covers the editor's typed field gates over an input editor on a real workspace, with no delay.
A card's title, expression and definition land as typed once the deferred requests are flushed.
A card's links read back in the card's order.
Each link is a stored pick, so its word and language go empty.
A card without links reads none.
An empty desk reads nothing.
An image's location, a video's location and span, and a gloss's text land as typed.
A pressed gloss button appends a gloss in the engine's gloss language, after the glosses held.
A removed gloss leaves, and a picked language retags the gloss that stays.
A gloss with no language carries Conduct's hint key, and a named one carries none.
The close gate lets the held draft and every search go.
An editor closed and then reopened hears a fold toggled in another editor again, once.
The editor reads its draft's language, and an opened workspace opens a fresh draft in it.
The observers the editor attaches show the draft through the marshal the driver hands.

## `internal static CEditor TEditorFieldPrepare(LEngine engine)`

Builds an input editor whose marshal runs each action at once.

## `internal static CEditor TEditorFieldPrepare(LEngine engine, Action<Action> marshal)`

Builds an input editor with no delay and the given marshal, and opens a fresh draft in it.

## `internal static long TEditorSheetAdd(CEditor editor)`

Appends a Meaning card to the held draft and answers its id.

## `internal static CCardDraft TEditorCardRead(CEditor editor, long sheet)`

The card as the editor's draft read shows it.

## `private static long TEditorSentenceAdd(CEditor editor, long sheet)`

Gives the card one written sentence and answers its id, since a gloss needs the sentence's example.

## `private static IReadOnlyList<CGlossDraft> TEditorGlossRead(CEditor editor, long sheet)`

The glosses of the card's first sentence, as the editor's draft read shows them.
