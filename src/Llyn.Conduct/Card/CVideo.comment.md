# CVideo.cs
Hash: `4b59c9ab118f2816`

## `public sealed class CVideo`

The video rows of the held draft's cards, as the user types into them.
It keeps no state, so the editor and the repertoire each build it fresh over their desk.

## `public void CVideoLocationSet(long videoId, string location)`

The user typed a video's location, deferred like every typed field.

## `public void CVideoSpanSet(long videoId, string span)`

The user typed a video's span, deferred like its location.

## `public void CVideoAdd(long cardId)`

The user pressed add under a card's videos, or under a Situation's with card zero.
The row lands after the rows held, and the driver hands no count.

## `public void CVideoRemove(long videoId)`

The user dropped a video row, and the engine finds the card holding it.

## `public void CVideoFileSet(long videoId, string? file)`

The user answered the file dialog of a video row.
A chosen file is sent at once rather than deferred.
A cancelled dialog hands null, and nothing is sent.
