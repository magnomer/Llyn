# PDisplay.cs

## `public class PDisplay : UserControl`

The shared read-only entry view as a control.
It is handed a loaded draft and draws it.
It never loads one itself, and it never decides which entry is shown.
That belongs to the browse-style panel it sits in.
Every handler is the one adapter for its Veneer event and hands the raw value to the lectern.
The constructor, the attach and the item fills do the wiring.

## `public PDisplay()`

Loads the view's markup from the Veneer and wears it as its content.
The markup's name scope is copied onto the view, so its named parts answer `FindName`.
It merges the contents dictionary and registers the styles of both for `QLook`.
It binds the etymon, playback and glyph commands and subscribes every event the markup named before.
The volume slider binds two ways to the shared volume catalog, so every tray keeps one level.
The contour's IPA binds to the pronunciation text, whichever form the chip prints.
The card lists catch their chip clicks, and the incoming list its row clicks, as bubbling events.
The card fills live in [PLeaf](../Template/PLeaf.comment.md), which paints the ready cards the lectern hands the lists.
It attaches the item fills of the speech, card, incoming and contents lists.

## `private TextBlock PDisplayEmpty`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal void PDisplayAttach(PWindow host, QLectern lectern)`

Puts the view on `lectern`, the deportment its owner built over the ports.
The notice, the page, the header, the stamp, frequency, speech and note controls go to the lectern.
The swath's clear and the star row's two properties are its seams.
`PMedia` is set on the view, so the cards below it build picture and video rows through the window.
The play button, the volume tray and the slider are handed to the lectern's playback, which drives them.
The pronunciation surface, the accent and transcription lists and the glyph row are handed over too.
The window goes to the lectern's sound, whose glyph, category and stem gates open through its openers.
The reflex, fanqie, script and paradigm controls go to the lectern's sound with their show members as seams.
The fanqie's category and stem notices go to the sound's observers, so a click reaches the shown entry's language.
Its representative notice goes straight to the sound area's gate, since the notice already carries raw values.
The floating contents and its eight sections are handed to the lectern's compass, which places and fills it.
The compass also goes to the contents dictionary, so a row click reaches it directly.
The card lists, incoming rows and etymology go to the lectern's card, which paints them itself.
The window goes to the lectern's card, whose incoming and word clicks open through it.

## `private static void PSpeechRefine(FrameworkElement container, object item, string? _)`

Writes one part of speech into its chip.

## `private static void PUsageRefine(FrameworkElement container, object item, string? _)`

Fills one incoming row: its icon, source headword, epithet, sentence, badge, flag and language.
The epithet keeps the en space its string format put before it.

## `private void PCompassRefine(FrameworkElement container, object item, string? _)`

Fills one contents row with its indent, number and name.
The current row carries the `Chosen` cue, which `QLookDisplay` colours in the accent.
The row's click is subscribed once to the contents dictionary's bridge.

## `private void PReflexFoldObserve(object sender, RoutedEventArgs e)`

Tells the lectern's sound that the fold toggle moved, and it reads the toggle for the fold gate.

## `private void PDisplayFavoriteObserve(object sender, RoutedEventArgs e)`

Hands the heart's press to the lectern, which stores it.

## `private void PDisplayGraspObserve(object sender, RoutedEventArgs e)`

Hands the star row's new step to the lectern, which stores it.

## `private void PDisplayHoverRefine(object sender, RoutedEventArgs e)`

Hands the step under the pointer to the lectern, which words the hover.

## `private void PPlaybackActionObserve(object sender, RoutedEventArgs e)`

Asks the lectern to play the shown entry's own recording.

## `private void PDisplayPlaybackObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the accent row to the lectern, which plays its recording through the engine.

## `private void PDisplayGlyphObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the pressed chip to the lectern, which opens the character's entry.

## `internal void PDisplayClose()`

Calls the sound area's gate that stops this view's own play, since the view is going away.

## Inline notes

### `private void PDisplayCardObserve(object sender, RoutedEventArgs e)`

The chips are drawn from a shared dictionary that knows no window.
They reach the cards only through the two lists this sits on.
So the click is handed to the lectern's card.

### `AddHandler(PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(PDisplayMentionObserve));`

Every sentence on every card raises the same bubbling event.
One handler on the control above them all answers it, so a card template stays free of handlers.
The lectern's card hands the click to its one gate, and the window paints the offer it answers.

### `PDisplaySwath.PSwathAttach(PDisplayContents);`

The band a reader drags across the page lies over the contents, inside the scroll viewer.
It listens on the viewer, so a drag begun anywhere on the page selects.
Showing or clearing an entry drops the band, since the text it spanned is gone.

### `private void PDisplayIncomingObserve(object sender, RoutedEventArgs e)`

The click bubbles from the row button to the list, so the lectern reads the row from its original source.
