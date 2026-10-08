# QDisplay.cs
Hash: `b91d123ef344c3e6`

## `internal sealed class QDisplay`

The driver of the shared read-only entry view.
It is handed the view's page and later a display, and it builds the lectern that paints the page.
It never loads a draft itself, and it never decides which entry is shown.
That belongs to the browse-style panel it sits in.
The lectern's sections pull and wire their own parts, so this class only joins the lectern to the window.

## `internal QDisplay(FrameworkElement surface)`

Holds the page the Veneer shell `PDisplay` built.
It wires nothing, so the view stays inert until it is introduced.

## `internal QLectern QDisplayLectern { get; private set; } = null!;`

The lectern this view built over its display, which the library's navigation reaches for card spotlights.

## `private ScrollViewer QDisplayContents`

Each named part of the markup is pulled by its contract ID from the page.
The ID keeps the name the markup gave it.

## `internal void QDisplayVisibleRefine(Visibility visible)`

Shows or hides the whole view for its panel.

## `internal void QDisplayIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu, CDisplay display)`

Builds the lectern over the page, `display` and the window's atelier and envoy.
It registers the page's styles for `QLook` and gives the contents the swath.
The swath's clear answers the area's open and close before the lectern subscribes anything.
The lectern's etymology and card notices go to the window menu's `QMentionOfferRefine`, since the lectern knows no window.
The page's volume slider is attached to the window's one volume owner, so every tray keeps one level.

## Inline notes

### `QDisplaySwath.PSwathAttach(QDisplayContents);`

The band a reader drags across the page lies over the contents, inside the scroll viewer.
It listens on the viewer, so a drag begun anywhere on the page selects.
Showing or clearing an entry drops the band, since the text it spanned is gone.
