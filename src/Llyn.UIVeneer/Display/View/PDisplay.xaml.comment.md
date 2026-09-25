# PDisplay.xaml.cs

## `public partial class PDisplay : UserControl`

The shared read-only entry view as a control.
It is handed a loaded draft and draws it.
It never loads one itself, and it never decides which entry is shown.
That belongs to the browse-style panel it sits in.
Every member only calls the lectern, except the three resource reads.

## `internal void PDisplayAttach(PWindow host, LLectern lectern)`

Puts the view on `lectern`, the deportment its owner built over the ports.
The notice, the page, the header, the stamp, frequency, speech and note controls go to the lectern.
The swath's clear and the star row's two properties are its seams.
The play button, the volume tray and the slider are handed to the lectern's playback, which drives them.
The shared volume catalog's setter is the seam that keeps every other tray on the same level.
The pronunciation surface, the accent and transcription lists and the glyph row are handed over too.
The window's glyph opener is the seam a character chip opens its entry through.
The reflex, fanqie, script and paradigm controls go to the lectern's sound with their show members as seams.
The fanqie's notices are wired back to the sound, so a click reaches the shown entry.
The floating contents and its eight sections are handed to the lectern's compass, which places and fills it.
The card lists, incoming rows and etymology go to the lectern's card.
The converters' fill members and the window's openers and failure notice are its seams.

## `private PSentenceConverter PDisplayFrameRead()`

The sentence converter the example template binds through, held in the view's resources.

## `internal void PDisplayCardScroll(long id)`

Hands the scroll to the lectern's card, since the window reaches the card lists only through this view.
## `internal void PCompassRowHandle(object sender, RoutedEventArgs e)`

Hands a contents row's click to the lectern's compass, which scrolls to the section the row names.
It is internal, because the contents dictionary forwards its row clicks here.

## `private void PReflexFoldHandle(object sender, RoutedEventArgs e)`

Hands the fold toggle's state to the lectern's sound, which sets the fold and redraws the rows.

## `internal void PDisplayObserverAttach()`

Asks the lectern to listen for the bulletins, marshalled onto this view.

## `private void PDisplayFavoriteHandle(object sender, RoutedEventArgs e)`

Hands the heart's press to the lectern, which stores it.

## `private void PDisplayGraspHandle(object sender, RoutedEventArgs e)`

Hands the star row's new step to the lectern, which stores it.

## `private void PDisplayHoverHandle(object sender, RoutedEventArgs e)`

Hands the step under the pointer to the lectern, which words it.

## `private void PPlaybackActionHandle(object sender, RoutedEventArgs e)`

Asks the lectern to play the shown entry's own recording.

## `internal void PDisplayPlaybackHandle(object sender, ExecutedRoutedEventArgs e)`

Hands the accent row to the lectern, which plays its recording through the engine.

## `private void PDisplayGlyphHandle(object sender, ExecutedRoutedEventArgs e)`

Hands the pressed chip to the lectern, which opens the character's entry.

## `internal void PDisplayClose()`

Asks the lectern to stop what the engine plays.

## Inline notes

### `private void PDisplayCardHandle(object sender, RoutedEventArgs e)`

The chips are drawn from a shared dictionary that knows no window.
They reach the cards only through the two lists this sits on.
So the click is handed to the lectern's card.

### `AddHandler(PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(PDisplayMentionHandle));`

Every sentence on every card raises the same bubbling event.
One handler on the control above them all answers it, so a card template stays free of handlers.
The lectern's card asks what stands at the offset, and the window decides what the answer opens.
The window hears only a lookup that succeeded.

### `PDisplaySwath.PSwathAttach(PDisplayContents);`

The band a reader drags across the page lies over the contents, inside the scroll viewer.
It listens on the viewer, so a drag begun anywhere on the page selects.
Showing or clearing an entry drops the band, since the text it spanned is gone.

### `<TextBlock x:Name="PDisplayReading" Grid.Row="1" Style="{StaticResource Theme.Text.Reading}" />`

The representative reading takes the header's second row, under the headword.
The language pill and the star row centre on the headword row alone, not on the whole header.
