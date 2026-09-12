# LSentenceDraft.cs

## `public sealed record LSentenceDraft(`

One card's hold on one Example, as a draft carries it.
The card holds these rows and never holds Examples directly.
An Example is shared data, so two cards quoting one sentence are two rows naming one Example.
Editing the frame on one row never reaches the other.

The row carries the frame the card reads the Example under.
A marker and a role are written by hand, because nothing ships either of them.
A language pack states only which of the two the form writes first.

A row may state a frame and no Example at all.
The frame belongs to the card, not to the Example.
A marker written before any sentence is not the Example's to lose.
The store keeps such a row, and it must survive a load and a save unchanged.

A row stating neither an Example nor a frame records nothing.
The commit skips it rather than writing a row the store would refuse.
The engine still holds such a row in the draft.
The form adds a row blank and fills it afterwards.

Where the row sits among its card's rows is the order of the list holding it.
The save rewrites every stored position from that order.

The row carries the id of the stored row it edits, as every other draft row does.
An empty id means the row has not been stored yet.
A save that kept the frame but issued a fresh id would rewrite a row nothing asked it to touch.

**Parameters**

- `LSentenceDraftExample` — The Example the row quotes, and `null` when the row states a frame and no sentence.
- `LSentenceDraftParticle` — The frame's grammatical marker, and what is known about it.
- `LSentenceDraftDependence` — The role the frame fills, and what is known about it.
- `LSentenceDraftId` — The id of the stored row this one edits, empty until one is given.

## `public bool LSentenceDraftEmpty`

True when the row names no Example and states no frame.
A row whose Example has no sentence and no stored id names no Example.
A minted id is not a stored one, so an Example the engine named but nobody wrote is still nothing.
An Example carrying a stored id names a stored row, however little the sentence says.
Such an empty row records nothing and is never committed.

## `public static LSentenceDraft LSentenceDraftCreate(string text)`

A sentence quoted with nothing else said about it.
The Example has no id yet and cites no Source, and the row states no frame.
