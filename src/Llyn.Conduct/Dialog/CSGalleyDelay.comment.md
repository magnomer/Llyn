# CSGalleyDelay.cs
Hash: `0a9301b2e0feefb8`

## `internal sealed class CSGalleyDelay(int time) : IDisposable`

One wait a person causes, by typing or by scrolling, before the galley acts.
The galley holds one for edits and one for sights, so each wait is cancelled apart.
A newer start cancels the older wait, so only the last request in a burst goes on.
The wait is interaction with no medium, so it lives in Conduct, not in the driver or the engine.

## `private CancellationTokenSource? _csGalleyDelaySource`

The pending wait, replaced whole by each start and emptied by `Dispose`.

## `internal int CSGalleyDelayTime { get; set; }`

The wait in milliseconds, first given by the galley.
Tests set zero, which runs the deferred work at once on the calling thread.
`Timeout.Infinite` keeps a wait pending until a newer start or close cancels it.
A wait reads the value when it starts, so a change applies to the next start.

## `public void Dispose()`

Cancels and releases the pending wait, leaving none.
A second call does nothing more.

## `internal async Task<CancellationToken?> LSGalleyDelayStart()`

Cancels the older wait, starts a fresh one and answers its token once the time has passed.
It answers null when a newer start or `Dispose` cancelled this wait.
The token stays live after the wait, so a long pass can stop once a newer start cancels it.
