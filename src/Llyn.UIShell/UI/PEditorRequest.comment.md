# PEditorRequest.cs

## `public partial class PEditor`

How one edit leaves the form and how the engine's answer comes back.
The form never assembles a draft.
It sends a request naming the draft, the item and the field, and re-reads what the engine holds.
Every answer arrives as a draft bulletin, whether this form or another surface caused it.
So there is one render path, and it is the one the bulletin drives.

## Inline notes

### `private readonly Dictionary<string, LRequest> _pEditorRequestPending = [];`

One waiting request per field, the last keystroke winning.
The key names the field, so a second keystroke replaces the first rather than queueing behind it.
A field with a key here is skipped by the render.
The draft is about to change to what that field holds.

### `private static string PEditorRequestFormat(PCard card, long rowId, string field)`

The key of a waiting request for one row inside a card, so two rows' texts never share a slot.

### `private static string PEditorRequestFormat(PCard card, string field)`

The key for one field of one card.
A card is named by its engine id, so a card moved in its list keeps its waiting request.

### `private void PEditorRequestDefer(string key, LRequest request)`

Parks a typed request behind the typing pause.
Nothing is parked while the form is being rendered, is suspended, or holds no draft.

### `private void PEditorRequestSend(LRequest request)`

Sends one request at once, after everything already waiting.
A structural request sent over a waiting text request would render the older text back.
The bulletin renders the answer before this call returns, so the caller has nothing to show.
A refusal suspends the form, since the draft and the form no longer agree about what is held.

### `private void PEditorRequestPersist()`

Sends every waiting request in the order the fields were typed.
Each key is dropped just before its request goes.
So the renders in between still skip the fields yet to be sent.
One failure drops the rest, because a form writing into a draft that refused it would drift further.

### `private void PEditorBulletinHandle(LBulletin bulletin)`

Re-reads and renders on a bulletin naming this form's own draft.
Every other bulletin is another surface's business.

### `private void PEditorLanguageSend()`

The chosen language as one request.
A language is chosen, not typed, so there is no pause to wait for.

### `private void PEditorSpeechSend()`

The chips as they stand, including a name typed and not yet entered.
A chip is added or removed whole, so its request goes at once.

### `private void PEditorAudioSend()`

The chosen recording, or none, as one request.
A recording is picked or cleared whole, so its request goes at once.
