# Configuration

Configuration holds the courier gates and the courier, flag-load and workspace status records.
These records and gates serve session-wide consumers rather than owning a catalog tab.
Views subscribe once and are handed one ready state, so they never ask the engine piece by piece.

## `CCourier`

The Joplin gates, pushing entries one way and connecting Llyn to Joplin.
Both gates share one busy mark, since the engine refuses a second courier run while one is going.
Each gate sets the mark inside its `try` and clears it in a `finally`.
So a throwing dialog, audit write or subscriber never leaves the courier stuck busy.
Each raised state allows both actions exactly when the mark is clear.

`CCourierCreate` is the one way to build a courier, since the constructor is private.
The atelier does not name the courier, which keeps its outgoing references under the ceiling.
The status bar builds one and keeps it, so its two buttons share one busy mark.
Building it is no user action, so it is no gate on the atelier.
The courier rereads the connection on every workspace open and every settings bulletin.
Each workspace keeps its own token.

The initial state is idle, both actions allowed and no connection, until the first workspace opens.
A gate raises `CCourierChanged` on the calling thread, but a settings bulletin raises on the engine's thread.
So a view marshals each state onto its own thread.
A view reads `CCourierRead` once on opening, so it shows a push already going.

`CCourierSend` returns at once while a run is going, so a double click starts nothing.
The line reads `Courier.Sending` during the push.
A failed push shows `Courier.SendFailed`, and the line is cleared.
A refusal such as a rejected token shows its own reason, since the engine reads it from the exception.
The engine drops a rejected token, so the view offers Connect again.
The receipt is worded after success, outside the push's `try`.
A broken receipt text never surfaces as a fault, because `LCourierReceiptFormat` falls back to a counts line.
That fallback is built from the counts alone with the invariant culture.
The receipt names at most ten failed headwords, joined by commas, with an ellipsis when there are more.

`CCourierAttach` asks Joplin for an access token, which the user accepts inside Joplin.
The line reads `Courier.Waiting` meanwhile.
The stored token raises a settings bulletin, which marks the courier connected.
A failure, including an unanswered request, shows `Courier.AttachFailed`.

`LCourierWarrantRaise` keeps the busy mark and the line, so a bulletin during a push keeps it showing.
The busy-and-line overload keeps the connection as last read, so progress updates do not alter the connection verdict.

## `CCourierState`

What the status bar shows of the courier, raised whole.
The connected state lets the view word itself as connected and drop its connect action.

## `CEnsignRow` and `CEnsignSheet`

`CEnsignRow` is one flag image keyed by language or variety.
`CEnsignSheet` is what an area answers once the flag fill has run.
The rows are read only after the fill, so every row a driver paints finds its flag drawn.
One answer carries both languages and rows, so one request serves a menu and a list.
The sheet carries the caller's language and row answers without deciding how a flag failure is handled.

## `CEstablishment`

The workspace's size and unsaved work for the status strip.
Verdicts come from the engine and keys from the atelier, so the strip judges nothing itself.

## Plain records

- `CWorkspaceState` holds the entries the two wings of the duplex panel last showed, null for none.

## Stamp

Commit: `c2f335c331fe`
- `CCourier.cs` `1a9a4e2f287a020d`
- `CCourierState.cs` `5f87ef7985ad4552`
- `CEnsignRow.cs` `d37a5b74883a579a`
- `CEnsignSheet.cs` `0094aa012192c46f`
- `CEstablishment.cs` `faa9ed337890ca0b`
- `CWorkspaceState.cs` `839ed72bc34e2337`
