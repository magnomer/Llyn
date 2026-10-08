# QPlayback.cs
Hash: `2ea4720cebe2b940`

## `internal sealed class QPlayback`

The editor's driver for the play button of the entry's own recording and the tray that holds the volume.
Which recording plays and whether its file still exists are the editor's playback facts in `CPlayback`.
This driver only paints them and plays the address the play gate answers.
The further rows carry their own recordings on their row models.
A recording is audio of one word in one language, and only the engine knows which recordings still fit.
So a headword or language change drops recordings in the engine, and the draft render empties the tray.

## `internal QPlayback(FrameworkElement surface, MediaPlayer player)`

Wires the play button's click and icon once the editor has taken over its markup's name scope.
The player is the editor's one player, shared with the clip preview and the accent rows.

## `internal void QPlaybackIntroduce(CEditor editor)`

Holds the Conduct editor and repaints after each draft change.

## `private void QPlaybackRefine(CEntryDraft _)`

Repaints the play button and the playback tray from the editor's ready playback after each draft change.

## `private void QPlaybackActionObserve(object sender, RoutedEventArgs e)`

Hands the recording the pressed button carries to the play gate, and its answer to the look.

## `private void QPlaybackActionRefine(Uri? address)`

Plays the address the play gate answered.
No address means the file went missing since the button was painted.
The button and the tray are then painted again from a fresh read, which lets the recording go.

## `private void QPlaybackAudioRefine(CTimbrePlayback playback)`

Paints the play button and the volume tray from the editor's ready playback.
The button's tag is its paint memory.
It holds the recording the button offers, handed back when it is pressed.
A recording other than the painted one stops the player and repaints the button.
An unchanged recording is left alone, so an edit elsewhere in the form never cuts a playing sound short.
The comparison is against what the button painted, never against the draft, so it decides no data.
