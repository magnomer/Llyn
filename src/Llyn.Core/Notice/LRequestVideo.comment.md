# LRequestVideo.cs

The video requests, shaped like the image requests with a second field for the span.

## `public sealed record LRequestVideoAddition(`

Adds a new video row with `LRequestValue` as its location, empty for a row still to be filled.

## `public sealed record LRequestVideoPick(`

Links an existing video to the card at `LRequestPosition`, copying the stored row under its positive id.

## `public sealed record LRequestVideoRemoval(long LRequestDraftId, long LRequestCardId, long LRequestVideoId)`

Unlinks one video from the card.

## `public sealed record LRequestVideoShift(`

Moves one video row to `LRequestPosition` inside its card.

## `public sealed record LRequestVideoLocation(long LRequestDraftId, long LRequestVideoId, LStateValue LRequestValue)`

Replaces the location of the video, wherever the draft holds it.

## `public sealed record LRequestVideoSpan(long LRequestDraftId, long LRequestVideoId, LStateValue LRequestValue)`

Replaces the span of the video, wherever the draft holds it.
