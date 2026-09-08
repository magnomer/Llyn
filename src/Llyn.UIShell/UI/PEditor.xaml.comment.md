# PEditor.xaml.cs

## `public partial class PEditor : UserControl`

The shared editable entry view as a control: what it is made of, and when it starts and stops.
Its behaviour is split across the files beside this one.
Those are the card lists, the drag, the menus, and the form as a value.
This file only assembles the parts.
It takes the engine the window opened, and shuts the editor down.

One control serves every panel that edits an entry.
The input panel mounts it standing on no entry and never puts it on one.
That is a form that only creates them.
A browse-style panel mounts it over an entry it loaded.
That is a form that modifies that one.
What differs between them is not the editing structure.
It is only which entry the form was opened on, which the editor already knows from its own draft.
A store is announced by the engine, so no host is wired to hear about it here.
A discard falls back on the entry the draft named, which is nothing at all for a form that creates one.

## `internal void PEditorAttach(PWindow host, LEngine engine, string origin, string? entry)`

Puts the editor to work on `engine`, the workspace the window opened.
`origin` names the surface this form sits on, and every draft it starts records it.
Two panels editing at once are told apart by it, and a recovered draft says where it came from.
It opens the form on `entry`, the entry the host wants edited.
That is nothing at all for a form that creates one.
The entry is shown before the language menu is built.
So the language it carries is already the chosen one when that menu decides on a fallback.
The workspace's volume is put on the grip last, once there is a form for it to sit on.

## `internal void PEditorClose()`

Stops the editor: searches in flight are called off and playback is released.

## Inline notes

### `private PWindow _pEditorHost = null!;`

The window this editor sits in.
An editor does not put up its own dialogs.
A failure is the program speaking, so it is asked for through the window.

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
Where it lands is settled in code rather than by an edge and two offsets, because the surface keeps a gutter for its shadow and that gutter must be taken back exactly.
It is at least as wide as the frame and no wider than the translation dropdown, so a short caret still carries a readable list.
Its rows, its corners and its shadow are cut down from the translation dropdown's, which is drawn against a field several times this one's size.
A row carries the wording at the size the chip will carry it, so what is offered is read as what will be taken.
It carries no line around it, because the shadow already says where it ends and a line as well made it a second card.

### `PMarkerLoad();`

After the language is settled, because the presets on offer are that language's.

### `AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(PEditorTextHandle));`

Every text box in the editor reports through one handler, caught as the event bubbles.
Cards and their rows come and go, and none of them has to be subscribed to by hand.

### `<ToggleButton x:Name="PSpeaker" Style="{StaticResource Theme.Language.Toggle}">`

The language pill is drawn as the reading view draws it, so one entry reads the same in both.
The toggle keeps its arrow and its menu, because here the language is chosen rather than reported.
