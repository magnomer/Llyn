# View

View holds the shell of the shared read-only entry view.
Every browse-style panel that shows an entry hosts it, and the panel decides which entry shows.
Nothing here loads a draft.

## `QDisplayCommand`

These routed commands are bound by every panel that shows an entry.
A command is used rather than a click.
So each panel enables the button by the same check it uses for print.
The portrait command exports a portrait of the entry the panel is reading.

## `QGrasp`

The grasp row is a row of stars the user clicks to say how well they know an entry.
Each step is half a star.
It draws itself, so there is no child element per star and no template to keep in step.
The step is a count of half stars from zero to the limit, in the store's own unit.
The limit comes from the deportment, so the row names no Core constant.
The row only maps a click to a step and raises it, and never decides what the store holds.
Its star driver is exposed so the host's driver can hand it the ready answers.
The row holds no Conduct type and requests no logic.

### `QGraspLimit`

The host sets it once before any step is shown.
End and Right stop at it, but the star count comes from the ready hit answers.

### `QGraspStep`

Conduct promises a step within the limit, and the row's own pointer and keys never set it.
So the row holds no clamp of its own.

### Brushes

Fill, empty, unrated and preview brushes are set from theme resources.
The draw reads them, so a theme change recolours the stars.

### `QGraspPressed`

It carries the step a click landed on and is raised even when that step stands.
The row does not write the step, so the listener sends it to Conduct and Conduct sets it back.

### `QGraspMoved`

It carries a relative move from a key, and Home and End carry the whole limit.
The listener sends the move to Conduct, which keeps the result within the limit.

### `QGraspHovered`

It carries the half-star step under the pointer, or nothing when the pointer leaves the row.
The listener words the step beside the stars and falls back to the standing label on nothing.

### `OnRender`

The row has no child elements, so this override alone puts the stars on screen.
The step, the limit and every brush affect render.

## `QDisplay`

The driver of the shared read-only view is handed the view's page and later a display.
It builds the lectern that paints the page.
It never loads a draft and never decides which entry is shown.
Its constructor wires nothing, so the view stays inert until it is introduced.
The lectern's sections pull and wire their own parts, so this class only joins the lectern to the window.
The lectern is not kept, since a card spotlight now comes from the compass's own sought event.

### `QDisplayIntroduce`

The swath's clear answers the area's open and close before the lectern subscribes anything.
Card notices go to the window menu, since the lectern knows no window.
The page's volume slider attaches to the window's one volume owner, so every tray keeps one level.

## Inline notes

### QDisplay.cs: `QDisplaySwath.QSwathAttach(QDisplayContents)`

The band a reader drags across the page lies over the contents, inside the scroll viewer.
It listens on the viewer, so a drag begun anywhere on the page selects.
Showing or clearing an entry drops the band, since the text it spanned is gone.

## `QRoster`

The roster holds the item fills of the view's lists.
The card and row fills live in `QLeaf`, with the meaning list taking the card fill.
The Other collocations list takes the row fill.
It keeps the compass, since a contents row's click goes to it.

### `QRosterUsageRefine`

The epithet is read through `QLookEpithetRead`, which keeps the en space before it.

### `QRosterCompassRefine`

A muted row's name takes `Theme.Muted`, and any other row clears it so the name follows the row's own ink.
The current row carries the `Chosen` cue, which `QLookDisplay` colours in the accent.
The click is subscribed once.

## Stamp

Commit: `36e31eb71c98`
- `QDisplay.cs` `7f7d0c14cec5fecc`
- `QDisplayCommand.cs` `7955796f874e383d`
- `QGrasp.cs` `ae1cdd497f3e620a`
- `QRoster.cs` `6a992ddf19fac54d`
