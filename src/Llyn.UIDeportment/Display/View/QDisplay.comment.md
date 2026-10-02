# QDisplay.cs

## `internal sealed class QDisplay`

The driver of the shared read-only entry view.
It is handed the view's page and a lectern, and it paints what the lectern holds.
It never loads a draft itself, and it never decides which entry is shown.
That belongs to the browse-style panel it sits in.
Every handler is the one adapter for its Veneer event and hands the raw value to the lectern.

## `internal QDisplay(FrameworkElement surface)`

Holds the page the Veneer shell `PDisplay` built.
It wires nothing, so the view stays inert until it is introduced.

## `private TextBlock QDisplayEmpty`

Each named part of the markup is pulled by its contract ID from the page.
The ID keeps the name the markup gave it.

## `internal void QDisplayVisibleRefine(Visibility visible)`

Shows or hides the whole view for its panel.

## `internal void QDisplayIntroduce(QWindow host, QLectern lectern)`

Puts the view on `lectern`, the deportment its owner built over the ports.
It registers the page's styles for `QLook` and gives the contents the swath.
It binds the etymon, playback and glyph commands and subscribes every event the markup named.
The volume slider is attached to the window's one volume owner, so every tray keeps one level.
The card lists catch their chip clicks, and the incoming list its row clicks, as bubbling events.
The card fills live in [PLeaf](../Template/PLeaf.comment.md), which paints the ready cards the lectern hands the lists.
It attaches the item fills of the speech, card, incoming and contents lists.
It then hands every part to the lectern.

## `private void QDisplayLecternAttach(QWindow host, QLectern lectern)`

The notice, the page, the header, the stamp, frequency, speech and note controls go to the lectern.
The swath's clear and the star row's two properties are its seams.
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

## `private static void QDisplaySpeechRefine(FrameworkElement container, object item, string? _)`

Writes one part of speech into its chip.

## `private static void QDisplayUsageRefine(FrameworkElement container, object item, string? _)`

Fills one incoming row: its icon, source headword, epithet, sentence, badge, flag and language.
The epithet keeps the en space its string format put before it.

## `private void QDisplayCompassRefine(FrameworkElement container, object item, string? _)`

Fills one contents row with its indent, number and name.
The current row carries the `Chosen` cue, which `QLookDisplay` colours in the accent.
The row's click is subscribed once to `QDisplayCompassObserve`.

## `private void QDisplayCompassObserve(object sender, RoutedEventArgs e)`

Hands a contents row's click to the lectern's compass, which scrolls to the section the row names.

## `private void QDisplayFoldObserve(object sender, RoutedEventArgs e)`

Tells the lectern's sound that the fold toggle moved, and it reads the toggle for the fold gate.

## `private void QDisplayFavoriteObserve(object sender, RoutedEventArgs e)`

Hands the heart's press to the lectern, which stores it.

## `private void QDisplayGraspObserve(object sender, RoutedEventArgs e)`

Hands the star row's new step to the lectern, which stores it.

## `private void QDisplayHoverRefine(object sender, RoutedEventArgs e)`

Hands the step under the pointer to the lectern, which words the hover.

## `private void QDisplayActionObserve(object sender, RoutedEventArgs e)`

Asks the lectern to play the shown entry's own recording.

## `private void QDisplayPlaybackObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the accent row to the lectern, which plays its recording through the engine.

## `private void QDisplayGlyphObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the pressed chip to the lectern, which opens the character's entry.

## Inline notes

### `private void QDisplayCardObserve(object sender, RoutedEventArgs e)`

The chips are drawn from a shared dictionary that knows no window.
They reach the cards only through the two lists this sits on.
So the click is handed to the lectern's card.

### `QDisplayMeaning.AddHandler(PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(QDisplayMentionObserve));`

Every sentence on every card raises the same bubbling event.
One handler on each card list answers it, so a card template stays free of handlers.
The lectern's card hands the clicked sentence row to its gate, and the window paints the offer.

### `QDisplayEtymology.AddHandler(PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(QDisplayEtymologyObserve));`

The etymology prose raises the same event from its own control.
Its own handler sends it to the etymology gate, so no handler tells the two sources apart.

### `QDisplaySwath.PSwathAttach(QDisplayContents);`

The band a reader drags across the page lies over the contents, inside the scroll viewer.
It listens on the viewer, so a drag begun anywhere on the page selects.
Showing or clearing an entry drops the band, since the text it spanned is gone.

### `private void QDisplayIncomingObserve(object sender, RoutedEventArgs e)`

The click bubbles from the row button to the list, so the lectern reads the row from its original source.
