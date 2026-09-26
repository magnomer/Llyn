# PDisplay.cs

## `public class PDisplay : UserControl`

The shared read-only entry view as a control.
It is handed a loaded draft and draws it.
It never loads one itself, and it never decides which entry is shown.
That belongs to the browse-style panel it sits in.
Every handler only calls the lectern.
The constructor, the attach and the item fills do the wiring.

## `public PDisplay()`

Loads the view's markup from the Veneer and wears it as its content.
The markup's name scope is copied onto the view, so its named parts answer `FindName`.
It merges the contents dictionary and registers the styles of both for `PLook`.
It binds the etymon, playback and glyph commands and subscribes every event the markup named before.
The volume slider binds two ways to the shared volume catalog, so every tray keeps one level.
The contour's IPA binds to the pronunciation text, whichever form the chip prints.
The card lists catch their chip clicks, and the incoming list its row clicks, as bubbling events.
It attaches the item fills of the speech, card, incoming and contents lists.

## `private TextBlock PDisplayEmpty`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal void PDisplayAttach(PWindow host, LLectern lectern)`

Puts the view on `lectern`, the deportment its owner built over the ports.
The notice, the page, the header, the stamp, frequency, speech and note controls go to the lectern.
The swath's clear and the star row's two properties are its seams.
`PMedia` is set on the view, so the cards below it build picture and video rows through the window.
The play button, the volume tray and the slider are handed to the lectern's playback, which drives them.
The pronunciation surface, the accent and transcription lists and the glyph row are handed over too.
The window's glyph opener is the seam a character chip opens its entry through.
The reflex, fanqie, script and paradigm controls go to the lectern's sound with their show members as seams.
The fanqie's notices are wired back to the sound, so a click reaches the shown entry.
The floating contents and its eight sections are handed to the lectern's compass, which places and fills it.
The card lists, incoming rows and etymology go to the lectern's card.
The converters' fill members and the window's openers and failure notice are its seams.

## `private static void PSpeechApply(FrameworkElement container, object item, string? _)`

Writes one part of speech into its chip.

## `private static void PUsageApply(FrameworkElement container, object item, string? _)`

Fills one incoming row: its icon, source headword, epithet, sentence, badge, flag and language.
The epithet keeps the en space its string format put before it.

## `private void PCompassApply(FrameworkElement container, object item, string? _)`

Fills one contents row with its indent, number and name.
The current row's tag reads `Chosen`, which `PLookDisplay` colours in the accent.
The row's click is subscribed once to the contents dictionary's forwarder.

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

### `private void PDisplayIncomingHandle(object sender, RoutedEventArgs e)`

The click bubbles from the row button to the list, so the lectern reads the row from its original source.

### `private readonly PLeaf _pLeaf = new();`

The card fills and the three converters that keep state between entries live in [PLeaf](../Template/PLeaf.comment.md).
The lectern's card fills the converters through their show members, as it filled the resources before.
