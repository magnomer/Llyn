# PEditor.cs

## `public partial class PEditor : UserControl`

The shared editable entry view as a control: what it is made of, and how its spine reaches the deportment.
Its card and sound behaviour is still split across the files beside this one until their own plans.
This file assembles the parts, wires the editor deportment's notices to control writes, and forwards the rail.

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

## `internal void PEditorAttach(PWindow host, LEditor editor)`

Puts the editor to work on the deportment its tab built, and subscribes each notice to one control write.
The sentence frames, the language menu, the categories and the volume are loaded once here.
They do not follow the draft, so no notice reloads them.

## `internal void PEditorVistaRestore()`

Opens a fresh draft over the entry vista the owner's deportment restored.
The command rail shows only for the editor that owns its entries, which is the input tab's.

## `internal void PEditorClose()`

Stops the editor.
The tenure is let go, searches in flight are called off, and playback is released.

## `private void PEditorObserverAttach(LDesk desk)`

Registers the marshalling observers on the desk once, which puts them on every tenure it starts.
The desk's own draft and state updates ride the same observers, so they run on the window's thread.

## `private void PEditorStartUpdate()`

A tenure started, so the card lists start empty.
The entry sections are read again at once, since a bulletin comes only when they change.

## `private void PEditorDraftUpdate(LDraft held)`

Writes every spine control from the draft the desk announced, then the card and sound parts from it.
The text boxes are written through the guarded writes, so a box being typed into keeps its caret.

## `private void PEditorLanguageUpdate()`

The writes that follow the language: its name and flag, the fonts, the tone contour, and the silent switch.

## `private void PEditorTextHandle(object sender, TextChangedEventArgs e)`

The one routed handler left, for the fields inside cards that come and go with them.
The spine's own boxes report through their own handlers.
A chip field's entry writes its text back into its caret row here, where a two-way binding stood.

## `private void PEditorStateUpdate()`

The controls that follow the desk's state: whether the editor is live, and the store, reset and chronicle buttons.

## Inline notes

### `Resources.MergedDictionaries.Add(_pMeaningTemplate);`

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

### `PCandidate`

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

### `AddHandler(LostFocusEvent, new RoutedEventHandler(PEditorFocusHandle));`

Focus leaving any box is caught as the event bubbles, so unsent typing goes before the focus does.

### `private Border PEditorFrequencyChip => (Border)FindName(nameof(PEditorFrequencyChip));`

The frequency chip sits under the part-of-speech chips, drawn as the reading view draws it.
It is read only here as there, since a frequency is fetched rather than typed.
A form standing on nothing hides it, and nothing beside it moves when it does.

## `internal event Action? PEditorChronicleChanged;`

Says the chronicle may now stand differently, so a host panel can light its own buttons.
An embedded editor hides its own rail, and the panel around it carries undo and redo.

### `private TextBlock PEditorReading => (TextBlock)FindName(nameof(PEditorReading));`

The representative reading of the headword, rewritten with each draft and each rime-book update.
Its markup style hides the line on empty text, so nothing ranked leaves the header as it was.
