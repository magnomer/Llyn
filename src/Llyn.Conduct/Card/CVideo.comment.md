# CVideo.cs
Hash: `5c798de1e64fe7f4`

## `public sealed class CVideo`

The video rows of the held draft's cards, as the user types into them.
It keeps no state, so the editor and the playwright each build it fresh over their desk.

## `public void CVideoLocationSet(long videoId, string location)`

The user typed a video's location, deferred like every typed field.

## `public void CVideoSpanSet(long videoId, string span)`

The user typed a video's span, deferred like its location.

## `public void CVideoAdd(long? cardId)`

The user pressed add under a card's videos, or under a Situation's with a null card.
Null means no card owns the row.
Conduct maps null to the engine's card 0, so the driver never sends a magic id.
The row lands after the rows held, and the driver hands no count.

## `public void CVideoRemove(long videoId)`

The user dropped a video row, and the engine finds the card holding it.

## `public void CVideoFileSet(long videoId, string? file)`

The user answered the file dialog of a video row.
A chosen file is sent at once rather than deferred.
A cancelled dialog hands null, and nothing is sent.
