# CVideo.cs

## `public sealed class CVideo`

The video rows of the held draft's cards, as the user types into them.
It keeps no state, so the editor builds it fresh over its desk.

## `public void CVideoLocationSet(long videoId, string location)`

The user typed a video's location, deferred like every typed field.

## `public void CVideoSpanSet(long videoId, string span)`

The user typed a video's span, deferred like its location.
