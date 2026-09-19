# PEditorRequest.cs

## `public partial class PEditor`

How one edit leaves the form and how the engine's answer comes back.
The form never assembles a draft.
It sends a request naming the draft, the item and the field, and re-reads what the engine holds.
Every request goes through the tenure, which keys what waits by `LRequestKey`.
So the form keeps no map of its own of which field is still waiting.
Every answer arrives as a draft bulletin, whether this form or another surface caused it.
So there is one render path, and it is the one the bulletin drives.

## `private void PEditorRequestDefer(LRequest request)`

Hands a typed request to the tenure to write once the typing stops.
Nothing is deferred while the form is being rendered or holds no draft.
A halted tenure drops the request itself.

## `private void PEditorRequestSend(LRequest request)`

Hands one request to the tenure to write now, after everything already waiting.
A structural request applied over a waiting text request would render the older text back.
The bulletin renders the answer, so the caller has nothing to show.
A refusal halts the tenure, and the halt is shown once through the state bulletin.

## Tenure subscriptions

`PEditorHold` attaches one handler per subject to the held tenure.
Draft and tenure notices are filtered by draft identity.
Frequency, grasp, inflection and reflex notices use persisted entry identity.
The tenure drops draft notices during preparation and detaches its subscriptions when finished or cancelled.

## `private void PEditorLanguageSend()`

The language the pill shows, sent as one request.
This reads user input: reading the tenure here would send the previous language back and discard the new choice.
A language is chosen, not typed, so there is no pause to wait for.
An empty pill sends nothing, so a form not yet loaded leaves the draft's own language standing.

## `private void PEditorSpeechSend()`

The chips as they stand, including a name typed and not yet entered.
A chip is added or removed whole, so its request goes at once.

## `private void PEditorAudioSend()`

The chosen recording, or none, as one request.
A recording is picked or cleared whole, so its request goes at once.
