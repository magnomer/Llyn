# QEditor.cs
Hash: `acc94345d7e05196`

## `internal sealed class QEditor : QChronicleHost`

The driver of the shared editable entry view, over the Veneer `PEditor` every editing panel places.
It holds the plain scope the panel pulled by contract ID, so no Deportment control stands between.
It builds the card half `QEditorCard`, the sound half `QEditorSound`, the typefaces `QEditorFont` and the tab strip `QStack`.
The esteem strip lives in `QRegard`, the part-of-speech chips and field in `QMarker` and its menu in `QCategory`.
The lexical unit dropper lives in `QUnit`.
It keeps the headword, the note, the command rail and the chronicle, and routes the editor's notices.
It follows `QDisplay`, the display's driver over its Veneer scope.

One editor serves every panel that edits an entry.
The input panel's editor stands on no entry and never puts it on one.
A browse-style panel's editor is opened on the entry its panel edits.
Which is which is a fact of the deportment, read off the vista the tab restored.

## `internal QEditor(FrameworkElement surface)`

Holds the scope and wires every Veneer part, as the editor control once did when its markup loaded.
The markup's dictionaries are scanned for look rows, so the popup rows light without triggers.
The scope is registered as a chronicle host, so undo and redo inside the editor walk its own desk.
The header's clicks and icons are wired here, where the markup once named them.
The headword's measuring twin follows its field's text and hint, where a style binding and trigger stood.

## `internal QProspect QEditorProspect`

The Translation and mention dropdown, which the corpus opens over its transcript.

## `internal event Action? QEditorChronicleChanged;`

Says the chronicle may now stand differently, so a host panel can light its own buttons.
An embedded editor hides its own rail, and the panel around it carries undo and redo.

## `internal void QEditorIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu, CEditor editor)`

Takes the Conduct editor the panel's area built, and hands each part only the facets it uses.
The sound half is a composer too, so it takes the whole editor.
The card half takes the editor's entry, card, sentence, list, field, image and video facets.
The atelier, envoy, volume and mention menu are the only window facets its parts use.
It then subscribes each of the editor's notices to one Refine of a part.
Every subscriber makes at most one Conduct read, so a notice fans out here and never inside a member.
The draft's writes follow the order the draft once showed in, so the sentence frame precedes the cards.
The sentence frame and mention chips stay here, since their order against the card Refines matters.
The editor's own subjects arrive as area events through the marshal Host handed the atelier.

## `internal void QEditorVisibleRefine(Visibility visible)`

Shows or collapses the editor the panel places, by the panel's ready verdict.

## `internal void QEditorExitRefine()`

Stops the editor through its one close gate, then releases playback through `QEditorPlayerRefine`.

## `internal void QEditorPlayerRefine()`

Releases the sound half's recording player.
A panel whose editor the window's exit gate stops in Conduct calls it alone.

## `public void QChronicleUndoObserve()`

Steps the entry draft one snapshot back, through the desk.
The step runs inside `QChronicle.QChronicleCaretRefine`, so the caret stays at the end of the focused box.

## `public void QChronicleRedoObserve()`

Steps the entry draft one snapshot forward again, in the same wrap.

## `private void QEditorDraftRefine(CEntryDraft draft)`

Writes the headword and the note through `QFieldTextShow`, and a box that already holds the text keeps its caret.
The note is written only while the ready verdict `CEntryNoteCheck` says the box does not hold it yet.
So a line break just typed at the end of the note survives the draft's echo.
The command rail shows only for the editor that owns its entries, which is the input tab's.
The reading line follows the headword just written.

## `private void QEditorStateRefine()`

The controls that follow the desk's state: whether the editor is live, and the store, reset and chronicle buttons.

## Inline notes

### `surface.AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(QEditorFocusObserve));`

Focus leaving any box is caught as the event bubbles, so unsent typing goes before the focus does.

### `new QStack(surface);`

The tab strip wires itself in its constructor and needs nothing after, so the editor keeps no handle to it.
Its own subscriptions on the strip's controls keep it alive.
