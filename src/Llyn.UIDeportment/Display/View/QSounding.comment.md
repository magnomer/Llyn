# QSounding.cs
Hash: `cb1c4a235132fd66`

## `internal sealed class QSounding`

The sound strip of the shared read-only entry view.
It owns the pronunciation, playback, accent, glyph, reflex, script and paradigm parts.
[QDisplay](QDisplay.comment.md) builds it over the same page and introduces it with the lectern.
The fanqie stays with `QDisplay`, since its representative notice is wired straight to the sound area's gate.
Every handler is the one adapter for its Veneer event and hands the raw value to the lectern.

## `internal QSounding(FrameworkElement surface)`

Holds the page the Veneer shell `PDisplay` built.
It wires nothing, so the strip stays inert until it is introduced.

## `private ItemsControl QSoundingReflex`

Each named part of the markup is pulled by its contract ID from the page.
The ID keeps the name the markup gave it.

## `internal void QSoundingIntroduce(QWindow host, QLectern lectern)`

Binds the playback and glyph commands and subscribes the fold toggle and the play button.
The volume slider is attached to the window's one volume owner, so every tray keeps one level.
The play button, the volume tray and the slider are handed to the lectern's playback, which drives them.
The pronunciation surface, the accent and transcription lists and the glyph row are handed over too.
The reflex, script and paradigm controls go to the lectern's sound with their show members as seams.

## `private void QSoundingFoldObserve(object sender, RoutedEventArgs e)`

Tells the lectern's sound that the fold toggle moved, and it reads the toggle for the fold gate.

## `private void QSoundingActionObserve(object sender, RoutedEventArgs e)`

Asks the lectern to play the shown entry's own recording.

## `private void QSoundingPlaybackObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the accent row to the lectern, which plays its recording through the engine.

## `private void QSoundingGlyphObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the pressed chip to the lectern, which opens the character's entry.
