# CCourier.cs
Hash: `36b47020cccf269c`

## `public sealed class CCourier`

The Joplin gates, pushing entries one way and connecting Llyn to Joplin.
Both gates share one busy mark, since the engine refuses a second courier run while one is going.
Each gate sets the mark inside its `try` and clears it in a `finally`.
Each raised state allows both actions exactly when the mark is clear.
A throwing dialog, audit write or subscriber therefore never leaves the courier stuck busy.
A view subscribes once to `CCourierChanged` and repaints from each ready state.

## `private readonly CAtelier _cCourierAtelier;`

The atelier whose portrait and settings ports every gate calls.

## `private CCourierState _cCourierState = new(false, true, string.Empty);`

The state last raised, kept so a gate can tell whether a run is still going.
It starts idle with both actions allowed.

## `private CCourier(CAtelier atelier)`

It is private, so `CCourierCreate` is the one way to build a courier.

## `public static CCourier CCourierCreate(CAtelier atelier)`

Builds the courier over the atelier, so the driver hands it no port.
The atelier does not name the courier, which keeps its outgoing references under the ceiling.
The Joplin view builds one and keeps it, so its two buttons share one busy mark.
Building it is no user action, so it is no gate on the atelier.

## `public event Action<CCourierState>? CCourierChanged;`

Raised on every change of state, on the thread that called the gate.
The awaits keep the caller's context, so a view can repaint without marshalling.

## `public CCourierState CCourierRead()`

Answers the state last raised.
A view reads it once on opening, so it shows a push already going.

## `public async Task CCourierSend(CEnvoy envoy)`

Pushes every entry to Joplin with the label words in the interface language.
A call while a run is going returns at once, so a double click starts nothing.
Once the push succeeds, `LCourierReceiptFormat` words the receipt into the line, outside the push's `try`.
A broken receipt text never surfaces as a fault, since the formatter falls back to a counts line.
The busy mark still clears, since the `finally` runs either way.
While the push runs, the line reads `Courier.Sending`.
A failed push is shown through `envoy` as `Courier.SendFailed`, and the line is cleared.
A refusal such as a rejected token shows its own reason, since the engine reads it from the exception.

## `public async Task CCourierAttach(CEnvoy envoy)`

Asks Joplin for an access token, which the user accepts inside Joplin.
A call while a run is going returns at once, as the push does.
The line reads `Courier.Waiting` while Joplin waits for the user, and `Courier.Attached` once connected.
A failure, including an unanswered request, is shown through `envoy` as `Courier.AttachFailed`.

## `private static string LCourierReceiptFormat(LSettingsPort settings, LReceipt receipt)`

Words the receipt's counts under `Courier.Receipt` with the current culture.
Failed headwords are appended under `Courier.Failed`, joined by commas.
At most ten are named, and an ellipsis follows when there are more.
A `FormatException` from either text falls back to `Joplin: {saved}/{kept}/{removed}/{failed}`.
That fallback line is built from the counts alone with the invariant culture.

## `private static string LCourierLineFormat(LSettingsPort settings, LReceipt receipt)`

Formats the receipt and the failed list from the translated texts.
It may throw `FormatException` when a translation is broken.

## `private void LCourierRaise(CCourierState state)`

Keeps `state` as the current one and raises `CCourierChanged` with it.
