# PEditorChange.cs

## `public partial class PEditor`

When what the user typed reaches the draft the engine holds.
The form no longer keeps a copy of the entry to compare itself against.
A typed field becomes one request, held until the user stops, then sent.
A structural action sends its request at once, because there is no keystroke coming to end it.
The chip lists still travel the older way, as one draft save after the same pause.
The two action buttons follow the engine's answer, so they cannot disagree with the closing warning.

## `internal bool PEditorChangeCheck()`

Whether this form holds work a host would be sorry to lose.
Typing still waiting to be sent is sent first.
Otherwise a window closing within a keystroke of the last change would call it unchanged.

## Inline notes

### `private const int PEditorChangeDelay = 250;`

Long enough that ordinary typing writes once rather than once per letter.
Short enough that a crash costs a word, not a card.

### `private bool _pEditorFill;`

Filling the controls raises the same events typing does.
So the form says nothing while it is being rendered from the draft the engine answered with.

### `private void PEditorChangeAttach(PCard card)`

A card reports its own text edits, since its fields are bound rather than typed into directly.
Its lists report through their own attachments, one per list, made when the card is built.
The handlers hold the editor rather than the other way round, so a dropped card is still collectible.

### `private void PCardChangeHandle(object? sender, PropertyChangedEventArgs e)`

The three text fields of a card each become their own request, keyed by card and field.
The number and the header a card raises are the engine's answers, so they raise no request.

### `private void PEditorTextHandle(object sender, TextChangedEventArgs e)`

One handler for every text box in the editor, caught as the event bubbles.
Fields are added to the form often, and each new one would otherwise need remembering.
A card's own boxes are passed over here, because the card already reported them by property.
A pronunciation row's box and a transcription row's box are passed over for the same reason.
The entry-level boxes become their own requests, and anything else is a chip and takes the older path.

### `private void PEditorFocusHandle(object sender, RoutedEventArgs e)`

Leaving any field sends what it holds.
Text sitting in a box is an unsent request, not state, so a click elsewhere must not lose it.

### `_ = PEditorChangeRun(pending.Token);`

The wait is not awaited, because the keystroke that started it must return at once.
Its continuation comes back on the UI thread, which is where the engine is used.
Nobody is left to observe the task, so the write it ends with is guarded inside it.
A refusal or a locked file reaches the window instead of vanishing into a dropped task.

### `private void PEditorChangeSave()`

The one place the pause ends.
A render that drops a focused card raises a focus loss, which lands here mid-fill.
Nothing is written then, and the wait is left running so the waiting requests still go.
Waiting requests go, and nothing else, because the chips and rows reached the engine as requests of their own.
It also settles the buttons, so a write and what the buttons say never drift apart.
A suspended form writes nothing, because its draft is the thing that failed.
