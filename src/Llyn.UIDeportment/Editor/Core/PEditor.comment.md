# PEditor.cs

## `public partial class PEditor : UserControl`

The shared editable entry view as a control: what it is made of, and how its spine reaches the deportment.
Its card and sound behaviour is still split across the files beside this one until their own plans.
This file assembles the parts, wires the editor deportment's notices to control writes, and forwards the rail.
The esteem strip lives in `QRegard` and the sound panels under the header in `QCadence`.
This control builds both, attaches them, and routes their bulletins.

One control serves every panel that edits an entry.
The input panel's editor stands on no entry and never puts it on one.
A browse-style panel's editor is opened on the entry its panel edits.
Which is which is a fact of the deportment, read off the vista the tab restored.

## `public PEditor()`

Loads the markup the Veneer holds as its content and takes over its name scope.
The template dictionaries are merged after the load, into this control's own resources.
The markup's dictionaries are scanned for look rows, so the popup rows light without triggers.
Each area wires its lists, commands and clicks through its own attach call.
That keeps this file under its line limit, and each wiring beside the handlers it names.
The header's clicks and icons are wired here, where the markup once named them.
The two measuring twins follow their field's text and hint, where a style binding and trigger stood.
The glyph, accent, transcription and reflex lists are attached to their row fills here, since the template binds nothing.

## `internal TextBox PPronunciationField`

The pronunciation box, found by name.
It is internal because the phonology panel attaches its probe to it.

## `internal void PEditorIntroduce(PWindow host, QEditor driver)`

Takes the editor's driver from the panel that built it, and wires the view's own parts.
The strip and the sound panels are handed the driver's Conduct editor, since they are driver parts too.
The driver then subscribes this view's Refines to the editor's notices, so the view never holds a Conduct type.

## `internal void PEditorClose()`

Stops the editor through its one close gate, then releases playback through `PEditorPlayerRefine`.

## `internal void PEditorPlayerRefine()`

Releases the editor's recording player, the Veneer half of a close.
A panel whose editor the window's exit gate stops in Conduct calls it alone.

## `internal void PEditorStartRefine()`

A tenure started, so the card lists start empty.

## `internal void PEditorDraftRefine(CEntryDraft draft)`

Writes the headword and the note through the guarded writes, so a box being typed into keeps its caret.
The command rail shows only for the editor that owns its entries, which is the input tab's.
The reading line follows the headword just written.

## `internal void PTimbreRefine(CEntryDraft _)`

The writes that follow the language's sound facts: the brackets, the tone contour and the silent switch.

## `internal void PSpeakerRefine(CEntryDraft _)`

The draft's language name and its flag.

## `internal void PGlyphFontRefine(CEntryDraft _)`

The glyph typography goes into the glyph list's resources, so the field takes it and the scheme label does not.

## `internal void PMeaningRefine(CEntryDraft draft)`

The meaning cards, each reading its own links as it is shown.
`PCollocationRefine` does the same for the collocation cards.

## `internal void PPlaybackRefine(CEntryDraft draft)`

The recording, then the playback tray, which also counts the accent rows' audio.

## `private void PContourRefine(object sender, TextChangedEventArgs e)`

The contour draws the pronunciation as typed, beside the gate that hears the same text.

## `private void PEditorTextObserve(object sender, TextChangedEventArgs e)`

The one routed handler left, for the fields inside cards that come and go with them.
The spine's own boxes report through their own handlers.
A chip field's entry hands its text to that field's text observer, which calls one gate.

## `internal void PEditorStateRefine()`

The controls that follow the desk's state: whether the editor is live, and the store, reset and chronicle buttons.

## Inline notes

### `Resources.MergedDictionaries.Add(new PMeaningTemplate());`

Each card and menu row is a template in a dictionary of its own.
So this control's markup stays its own layout.
A template still raises this control's events.
That is why the dictionaries are built here against this instance rather than merged from markup.

### `_pNotationTemplate = new PNotationTemplate(this);`

The notation dictionary is kept in a field, so the notation fill can subscribe its forwarder.
The fill attached to `PNotationList` hands that forwarder to every reading button.

### `PProspectList.ItemsSource = _pProspectItem;`

The Translation dropdown is one popup for the whole editor rather than one per card.
Only one caret is typed into at a time, so only one list of candidates is ever open.
It hangs off the caret it was opened from, which is why the markup names no placement target.
Its shutting drops the selection, so enter never takes a row the dropdown no longer shows.

### `PProffer`

The Situation dropdown hangs under the frame of the caret it was opened from, not under the caret itself.
The frame is the edge a reader sees, and it is drawn outward of the caret it belongs to.
Where it lands is settled in code rather than by an edge and two offsets.
The surface keeps a gutter for its shadow.
That gutter must be taken back exactly.
It is at least as wide as the frame and no wider than the translation dropdown.
A short caret still carries a readable list.
Its rows, its corners and its shadow are cut down from the translation dropdown's.
That one is drawn against a field several times this one's size.
A row carries the wording at the size the chip will carry it.
What is offered is read as what will be taken.
It carries no line around it, because the shadow already says where it ends.
A line as well made it a second card.

### `AddHandler(LostFocusEvent, new RoutedEventHandler(PEditorFocusObserve));`

Focus leaving any box is caught as the event bubbles, so unsent typing goes before the focus does.

## `internal event Action? PEditorChronicleChanged;`

Says the chronicle may now stand differently, so a host panel can light its own buttons.
An embedded editor hides its own rail, and the panel around it carries undo and redo.
