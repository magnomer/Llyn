# PEditorChange.cs

## `public partial class PEditor`

When what the user typed reaches the draft the engine holds.
The form no longer keeps a copy of the entry to compare itself against.
It writes what it holds instead, and asks the engine whether that differs.
Typing is written once the user stops, because a write per keystroke is a write per keystroke.
A structural action writes at once, because there is no keystroke coming to end it.
The two action buttons follow the same answer, so they cannot disagree with the closing warning.

## `internal bool PEditorChangeCheck()`

Whether this form holds work a host would be sorry to lose.
Typing still waiting to be written is written first.
Otherwise a window closing within a keystroke of the last change would call it unchanged.

## Inline notes

### `private const int PEditorChangeDelay = 250;`

Long enough that ordinary typing writes once rather than once per letter.
Short enough that a crash costs a word, not a card.

### `private bool _pEditorFill;`

Filling the controls raises the same events typing does.
So the form says nothing while it is being filled from a draft it just started.

### `private void PEditorChangeAttach(PCard card)`

A card reports its own edits, since its fields are bound rather than typed into directly.
Each list it carries reports its own additions and removals.
The handlers hold the editor rather than the other way round, so a dropped card is still collectible.

### `private void PEditorTextHandle(object sender, TextChangedEventArgs e)`

One handler for every text box in the editor, caught as the event bubbles.
Fields are added to the form often, and each new one would otherwise need remembering.

### `_ = PEditorChangeRun(pending.Token);`

The wait is not awaited, because the keystroke that started it must return at once.
Its continuation comes back on the UI thread, which is where the engine is used.
Nobody is left to observe the task, so the write it ends with is guarded inside it.
A refusal or a locked file reaches the window instead of vanishing into a dropped task.

### `private void PEditorChangeSave()`

The one place control values reach the held draft outside a store.
It also settles the buttons, so a write and what the buttons say never drift apart.
