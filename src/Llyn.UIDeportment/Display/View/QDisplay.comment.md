# QDisplay.cs
Hash: `eca8f7ede19390dc`

## `internal sealed class QDisplay`

The driver of the shared read-only entry view.
It is handed the view's page and a lectern, and it paints what the lectern holds.
It never loads a draft itself, and it never decides which entry is shown.
That belongs to the browse-style panel it sits in.
Every handler is the one adapter for its Veneer event and hands the raw value to the lectern.
The sound strip lives in [QSounding](QSounding.comment.md), and the list item fills in [QRoster](QRoster.comment.md).

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
It binds the etymon command and subscribes the header and card events the markup named.
The card lists catch their chip clicks, and the incoming list its row clicks, as bubbling events.
The list item fills go to [QRoster](QRoster.comment.md), which is handed the lists.
It then hands every part to the lectern.

## `private void QDisplayLecternAttach(QWindow host, QLectern lectern)`

The window's atelier and envoy, the notice and the page go to the lectern.
The header, stamp, frequency, speech and note controls follow.
The swath's clear and the star row's two properties are its seams.
The sound strip is introduced next through [QSounding](QSounding.comment.md), which hands its own parts over.
The fanqie and the reading line go to the lectern's sound with the fanqie's show member as a seam.
The fanqie's category and stem notices go to the sound's observers, so a click reaches the shown entry's language.
Its representative notice goes straight to the sound area's gate, since the notice already carries raw values.
The floating contents and its eight sections are handed to the lectern's compass, which places and fills it.
The compass also goes to the lectern's card, beside the page's resource dictionary and the contents.
The card lists, incoming rows and etymology go to the lectern's card, which paints them itself.
The window goes to the lectern's card, whose incoming and word clicks open through it.

## `private void QDisplayFavoriteObserve(object sender, RoutedEventArgs e)`

Hands the heart's press to the lectern, which stores it.

## `private void QDisplayGraspObserve(object sender, RoutedEventArgs e)`

Hands the star row's new step to the lectern, which stores it.

## `private void QDisplayHoverRefine(object sender, RoutedEventArgs e)`

Hands the step under the pointer to the lectern, which words the hover.

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
