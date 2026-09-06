# PVideo.cs

## `internal sealed class PVideo`

One Video row on a card.
The row carries where the video plays from and the span of it worth watching.
It also carries the preview that plays it.
The location is kept with the card, exactly as an Image's location is.
The span and the preview are not: they are how the row is watched while writing, not what the card says.

A location standing empty is not one thing.
It may never have been written, or it may have been written and be unreadable now.
The second is marked rather than shown, and any edit ends the mark.

The timestamp is written the way it is read, as `00:30 - 04:20`.
It says which part of the video the preview plays.
A timestamp that cannot be read is not an error the user is stopped by.
The row falls back to playing from the start.
Half-typed text is a moment in the middle of typing.

## `internal PVideo(LStateValue location)`

The row for a stored Video, holding the location as the store knows it.

## `internal LStateValue PVideoLocationRead()`

What the row says its location is: nothing written, unreadable, or the text it shows.

## `public string PVideoTimestamp { get; set; }`

The span as the user wrote it.
Both ends are optional: nothing before the dash means from the start, nothing after it means to the end.

## `public TimeSpan PVideoFrom { get; }`

Where the preview starts, read off the timestamp, and the start of the video when the timestamp says nothing.

## `public TimeSpan? PVideoUntil { get; }`

Where the preview goes back to the start.
It is `null` when the timestamp named no end and the video plays out.

## `public bool PVideoPlaying { get; set; }`

Whether the preview should be running.
It is the row's own state rather than the player's.
So it survives the preview being taken down and put back as the card scrolls.
