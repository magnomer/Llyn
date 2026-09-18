# PEditorRequest.cs

## `public partial class PEditor`

How one edit leaves the form and how the engine's answer comes back.
The form never assembles a draft.
It sends a request naming the draft, the item and the field, and re-reads what the engine holds.
Every request goes through the tenure, which keys what waits by `LRequestKey`.
So the form keeps no map of its own of which field is still waiting.
Every answer arrives as a draft bulletin, whether this form or another surface caused it.
So there is one render path, and it is the one the bulletin drives.

## `private PObserver? _pEditorObserver;`

The form's own subscription to the engine, held so it can be detached when the form closes.

## `private void PEditorRequestDefer(LRequest request)`

Hands a typed request to the tenure to write once the typing stops.
Nothing is deferred while the form is being rendered or holds no draft.
A halted tenure drops the request itself.

## `private void PEditorRequestSend(LRequest request)`

Hands one request to the tenure to write now, after everything already waiting.
A structural request applied over a waiting text request would render the older text back.
The bulletin renders the answer, so the caller has nothing to show.
A refusal halts the tenure, and the halt is shown once through the state bulletin.

## `private void PEditorBulletinHandle(LBulletin bulletin)`

Re-reads and renders on a bulletin naming this form's own draft.
While the form asks for its own blank rows, the bulletin only marks the draft stale.
One render follows the asks.
A tenure bulletin for the held draft settles the buttons and the form's enabled state.
A finished frequency fetch for the entry the draft stands on fills the chip alone.
A Source bulletin reloads the offered Sources, since a byline may have changed under a row.
A settings bulletin re-renders the draft, so the reading fields swap to the form now picked.
Every settings switch raises that bulletin, so a language or epithet change re-renders the draft as well.
A reflex bulletin for the entry the draft stands on settles the fetching line and the turning icon.
Every other bulletin is another surface's business.

## `private void PEditorLanguageSend()`

The language the pill shows, sent as one request.
A language is chosen, not typed, so there is no pause to wait for.
An empty pill sends nothing, so a form not yet loaded leaves the draft's own language standing.

## `private void PEditorSpeechSend()`

The chips as they stand, including a name typed and not yet entered.
A chip is added or removed whole, so its request goes at once.

## `private void PEditorAudioSend()`

The chosen recording, or none, as one request.
A recording is picked or cleared whole, so its request goes at once.
