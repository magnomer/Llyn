# PVideo.cs

## `internal sealed class PVideo`

One Video row on a card.
The row carries where the video plays from and the span of it worth watching.
It also carries the address the screen beside it plays.
Both the location and the span are kept with the card.
Both are what the card says about the video.
The address is not kept, and is read off the location whenever the location changes.

A location standing empty is not one thing.
It may never have been written, or it may have been written and be unknown now.
The second is marked rather than shown, and the converter reads the mark off the engine's value.
The row holds the engine's values and nothing typed, so what is typed leaves through the editor as written text.

The timestamp is written the way it is read, as `00:30 - 04:20`.
It says which part of the video the preview plays.
A timestamp that cannot be read is not an error the user is stopped by.
The row falls back to playing from the start.
Half-typed text is a moment in the middle of typing.

## `internal PVideo(LWindow window, LVideoDraft written)`

The row for a stored Video, holding the location and the span as the store knows them.
The window resolves each location to the address the screen plays.

## `internal long PVideoId`

The id of the engine's row this one shows, which every request about it names.

## `internal static string? PVideoOpen(Window owner)`

Asks the user for a video file on this machine and answers its path, or null when they chose none.
It lives on the row for the reason the picture chooser lives on its row.

## `internal void PVideoShow(LVideoDraft written)`

Redraws the row from the engine's row, location and span apart, only where the field says something else.
A field already reading what the engine holds is left alone.
The id is always taken.

## `public LStateValue PVideoLocation`

The location as the draft holds it, set only from the draft.
A changed location reads the address the screen plays again.

## `public LStateValue PVideoTimestamp`

The span as the draft holds it, set only from the draft.
Nothing before the dash means from the start.
Nothing after the dash means to the end.

## `public TimeSpan PVideoFrom`

Where the preview starts, read off the timestamp, and the start of the video when the timestamp says nothing.

## `public TimeSpan? PVideoUntil`

Where the preview goes back to the start.
It is `null` when the timestamp named no end and the video plays out.

## `public bool PVideoPlaying`

Whether the preview should be running.
It is the row's own state rather than the player's.
So it survives the preview being taken down and put back as the card scrolls.

A row begins stopped, in both modes.
Opening a card is not asking to hear it.
A card carrying several videos would otherwise all speak at once.

## `internal static void PVideoRowApply(FrameworkElement container, object item, string? changed)`

The shared fill of a written video row, used by the entry and situation editors alike.
It hides the frame while nothing plays and sets the screen from the row.
Each field's text is rewritten only on a full fill or when its own property changed.
So a change to the other property leaves a field being typed in alone.
It also attaches the list's reveal, so the remove handle shows while the list is hovered or focused.

## `internal static void PVideoLineApply(FrameworkElement container, object item, string? _)`

The fill of a read-only video line, whose row is the video the frame made.

## `private static void PVideoScreenApply(PScreen screen, PVideo row)`

Sets the span, the address and the playing flag the screen shows.
The volume follows the shared catalog live, so every screen answers the one slider.
The playing flag is bound both ways, since the screen's own switch changes it.
