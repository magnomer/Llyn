# PVideo.cs

## `internal sealed class PVideo`

One Video row on a card, and only on a card being written. Nothing stores a Video: the row carries where the video plays from, the span of it worth watching, and the preview that plays it, and all of that is gone when the form is closed. It is a thing to look at while writing a meaning, not a thing the entry keeps.

The timestamp is written the way it is read — `00:30 - 04:20` — and says which part of the video the preview plays. A timestamp that cannot be read is not an error the user is stopped by: the row falls back to playing from the start, because half-typed text is a moment in the middle of typing.

## `public string PVideoTimestamp { get; set; }`

The span as the user wrote it. Both ends are optional: nothing before the dash means from the start, nothing after it means to the end.

## `public TimeSpan PVideoFrom { get; }`

Where the preview starts, read off the timestamp, and the start of the video when the timestamp says nothing.

## `public TimeSpan? PVideoUntil { get; }`

Where the preview goes back to the start, or `null` when the timestamp named no end and the video plays out.

## `public bool PVideoPlaying { get; set; }`

Whether the preview should be running. It is the row's own state rather than the player's, so it survives the preview being taken down and put back as the card scrolls.
