# QLectern.cs
Hash: `9c9517f15a79364b`

## `public sealed class QLectern`

The reading view's driver, standing between the veneer and the display's area, [CDisplay](../../Llyn.Conduct/Display/CDisplay.comment.md).
It gives the page to the entry or the empty notice.
Its sections and compass draw the rest, and each is built once in its constructor.
The area decides when an entry opens or closes, so this class only answers its events.
The veneer names only this class, so no veneer reaches Conduct.

## `public QLectern(FrameworkElement surface, CDisplay display, CAtelier atelier, CEnvoy envoy)`

Builds the view over `display`, pulling every part it draws from `surface` by contract ID.
All header, sound and card sections are built here, followed by the compass.
The header, frequency and entry sections take the display and subscribe their own events.
Every other section takes only the parts it reads, and the lectern subscribes its redraws right after building it.
So the open and close order matches the build order.
The incoming section takes the atelier's navigation, whose gate a clicked row opens through.
The list item fills, [QRoster](View/QRoster.comment.md), are built after the compass, since a contents row's click goes to it.
The panel a view follows is attached in Conduct, so the lectern names no panel.
The envoy is the window's, which the header's flag load and the script box report failures through.
The header, the sections and the compass then redraw in subscription order.
The compass is built and subscribed last, so it answers open and close once every section's visibility is set.
The card section paints its ready card lists before the compass measures them.
A fold written from either mode redraws the card section through the display's fold notice.
The same notice repaints the rime-book and script box openings in the sound section.
The same notice redraws the reflex hinge, so a reflex opening written in the editor shows here.
The accent section draws its rows before it loads their flags, so the late flags land on drawn rows.
The engine's notices arrive on its own thread, so each is marshalled onto `surface` through `QObserver`.
The reflex section is built before the sound section, so a fanqie notice rewrites the anchors first.
The draft reload and the workspace swap go straight back to the area's resonates.

## `public QLecternCard QLecternCard { get; }`

The card section, which the view hosting this driver wires to the window's mention menu.
The lectern knows no window, so the section's offers leave through its notice.

## `public QLecternEtymology QLecternEtymology { get; }`

The etymology section, which the view hosting this driver wires to the window's mention menu.
The lectern knows no window, so the section's offers leave through its notice.

## `public QCompass QLecternCompass { get; }`

The floating contents, built over the display's compass area and the page.
The lectern subscribes it to the area, so the veneer names no compass handler.
Card spotlights go through its scroll, so a card lands where a row would put it.

## `private void QLecternContentsRefine()`

Gives the page to the entry once the area opens one.

## `private void QLecternEmptyRefine()`

Gives the page to the unselected notice once the area closes.
