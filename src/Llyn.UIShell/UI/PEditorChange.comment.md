# PEditorChange.cs

## `public partial class PEditor`

When what the user typed reaches the draft the engine holds.
The form no longer keeps a copy of the entry to compare itself against.
A typed field becomes one request, deferred through the tenure until the user stops.
A structural action applies its request at once, because there is no keystroke coming to end it.
The two action buttons follow the tenure's answer, so they cannot disagree with the closing warning.

## `private bool _pEditorFill;`

Filling the controls raises the same events typing does.
So the form says nothing while it is being rendered from the draft the engine answered with.
A render reached from inside a render restores the guard it found rather than dropping it.

## `private PRespelling _pEditorRespelling = PRespelling.PRespellingPlain;`

Which form the pronunciation field prints, read on every render from the switch and the draft's pack.
A typed change is sent as a request for that form, so the edit side mirrors the read side.

## `internal Action<bool>? PEditorChangeNotice;`

Raised with whether the draft can be stored, so the mounting panel settles its rail's save in turn.

## `internal bool PEditorChangeCheck()`

Whether this form holds work a host would be sorry to lose.
Typing still waiting to be written is written first.
Otherwise a window closing within a keystroke of the last change would call it unchanged.

## `private void PCardChangeHandle(PCard card, string field, LStateWritten written)`

The three text fields of a card each become their own request, keyed by the request's own card and field.
The number and the header a card raises are the engine's answers, so they raise no request.

## `internal static string PEditorFieldRead(TextBox box)`

Which value a typed box shows, read from the path its text binds to.
A box inside an editable choice binds to the choice, so the choice's path is the answer there.
A box bound through several values, as the citation field is, answers with the first of them.
Boxes are told apart this way rather than by name.
So a template adds a field without the editor learning it.

## `private void PEditorTextHandle(object sender, TextChangedEventArgs e)`

One handler for every text box in the editor, caught as the event bubbles.
Fields are added to the form often, and each new one would otherwise need remembering.
A pronunciation row's box and a transcription row's box are passed over, as the rows report through their own handlers.
A box standing on a card, a row, a Gloss or a media row is handed to the field handler.
The entry-level boxes become their own requests.
The pronunciation field becomes a respelling request while respellings are shown and a reading request otherwise.

## `private void PEditorFieldHandle(TextBox box)`

Turns the text typed into a card, row, Gloss or media field into its request.
Only a box the keyboard is in has been typed into.
A box the engine's value was drawn into raises the same event, and says nothing here.
The value a box shows binds one way.
What is typed leaves as written text, and the draft answers with the state.

## `private void PEditorFocusHandle(object sender, RoutedEventArgs e)`

Leaving any field writes what is waiting.
Text sitting in a box is an unsent request, not state, so a click elsewhere must not lose it.

## `private void PEditorChangeSave()`

Writes what is waiting in the tenure now.
A render that drops a focused card raises a focus loss, which lands here mid-fill.
Nothing is written then, and the tenure's wait is left running so the waiting requests still go.

## `private void PEditorChangeUpdate()`

Settles discard, store and the change notice from one reading of the tenure's state, then the undo and redo pair.
The save button follows the refusal answer too, so a draft with no headword shows Discard alone.
The form's enabled state is settled from the same reading, so a halted tenure stops the typing.
Every tenure bulletin ends here, so the buttons never say more than the draft can do.
