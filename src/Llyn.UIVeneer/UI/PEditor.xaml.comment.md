# PEditor.xaml.cs

## `public partial class PEditor : UserControl`

The shared editable entry view as a control: what it is made of, and how its spine reaches the deportment.
Its card and sound behaviour is still split across the files beside this one until their own plans.
This file assembles the parts, wires the editor deportment's notices to control writes, and forwards the rail.

One control serves every panel that edits an entry.
The input panel's editor stands on no entry and never puts it on one.
A browse-style panel's editor is opened on the entry its panel edits.
Which is which is a fact of the deportment, read off the vista the tab restored.

## `internal void PEditorAttach(PWindow host, LEditor editor)`

Puts the editor to work on the deportment its tab built, and subscribes each notice to one control write.
The language menu, the categories and the volume are loaded once here, since they do not follow the draft.
The card fields ask the card deportment the editor owns, and the sound rows ask the window deportment.

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

## `private void PEditorStateUpdate()`

The controls that follow the desk's state: whether the editor is live, and the store, reset and chronicle buttons.

## Inline notes

### `Resources.MergedDictionaries.Add(new PMeaningTemplate(this));`

Each card and menu row is a template in a dictionary of its own.
So this control's markup stays its own layout.
A template still raises this control's events.
That is why the dictionaries are built here against this instance rather than merged from markup.

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

### `<ToggleButton x:Name="PSpeaker" Style="{StaticResource Theme.Language.Toggle}">`

The language pill is drawn as the reading view draws it, so one entry reads the same in both.
The toggle keeps its arrow and its menu, because here the language is chosen rather than reported.

### `<Border x:Name="PEditorFrequencyChip" ...>`

The frequency chip sits after the star, drawn as the reading view draws it.
It is read only here as there, since a frequency is fetched rather than typed.
A form standing on nothing hides it, and nothing beside it moves when it does.
