# QLectern.cs
Hash: `876a5995d06d3f7d`

## `public sealed class QLectern`

The reading view's driver, standing between the veneer and the display's area, [CDisplay](../../Llyn.Conduct/Display/CDisplay.comment.md).
It draws the header, the speech, frequency, note and stamp sections, and the empty notice.
Its halves draw the rest, and this class subscribes each half to the area's events.
The area decides when an entry opens or closes, so this class only answers its events.
The veneer names only this class, so no veneer reaches Conduct.

## `public QLectern(CDisplay display)`

Builds a view over `display`, with a half for the accent, sound, playback and card sections.
It attaches to no panel, so the wing's view takes it.

## `public QLectern(CDisplay display, CPanel panel)`

Builds a view that follows `panel`, so each draft the panel loads opens here.
A view following no panel, such as the wing's, takes the other constructor.

## `public CDisplay QLecternArea { get; }`

The display's area, whose gates the view hosting this driver calls directly.
The driver reaches the engine only through this area, so no `L` type crosses into Deportment.
The compass and the card half take the areas it hands out.

## `public CDisplaySound QLecternSoundArea { get; }`

The display's sound area, whose fanqie gate the view hosting this driver calls directly.

## `public QLecternCard QLecternCard { get; }`

The card half, built over the display, which draws the cards, incoming rows and etymology and answers their clicks.

## `public QLecternAccent QLecternAccent { get; }`

The accent half, built over the display's accent area, which draws the primary pronunciation and the accent rows.

## `public QLecternSound QLecternSound { get; }`

The sound half, built over the display's sound area, which draws the transcriptions, glyph row and every phonology section.

## `public QLecternPlayback QLecternPlayback { get; }`

The playback half, built over the display's playback area, which drives the play buttons and the volume slider.

## `public QCompass QLecternCompass { get; private set; } = null!;`

The floating contents, built once the veneer hands over its controls.
Card scrolling goes through its scroll, so a card lands where a row would put it.

## `public void QLecternCompassIntroduce(FrameworkElement view, ScrollViewer contents, FrameworkElement header, FrameworkElement compass, UIElement surface, ToggleButton toggle, ItemsControl list)`

Builds the compass over the display's compass area and the view's controls.
The lectern subscribes the compass to them, so the veneer names no compass handler.
It answers the area's open and close last, once every section's visibility is set.

## `public void QLecternIntroduce(CAtelier atelier, CEnvoy envoy, UIElement empty, UIElement contents, Action swathSeam)`

Takes the atelier, the envoy, the unselected notice, the page and the swath's clear, then answers the area's events.
The envoy is the window's, which the catalog's flag load reports its failure through.
The swath's clear is a seam, since the band a reader drags lies in the veneer.
Opening or closing an entry drops the band first, since the text it spanned is gone.
The header, the halves and the compass then redraw in subscription order.
The card half paints its ready card lists before the compass measures them.
The accent half draws its rows before it loads their flags, so the late flags land on drawn rows.
The sound half draws its reflex rows before their fold and its fanqie block, which writes their anchors.
A reflex notice redraws the rows through the resonate, then the fold, in that order on the page.
The engine's notices arrive on its own thread, so each is marshalled onto `contents` through `QObserver`.
The draft reload and the workspace swap go straight back to the area's resonates.
A lectern no view attached to answers nothing, since panels announce drafts in tests with no veneer.

## `public void QLecternHeaderIntroduce(TextBlock headword, TextBlock language, Image flag, UIElement globe, ToggleButton favorite, DependencyObject grasp, DependencyProperty step, DependencyProperty limit, TextBlock graspLabel)`

Takes the headword, the language pill, the heart and the star row.
The star row is a veneer control, so it is reached through its step and limit properties.
The limit is set here once, from the engine's last grasp step.

## `public void QLecternStampIntroduce(UIElement section, TextBlock added, TextBlock updated)`

Takes the stamp section and its added and updated lines.
The section collapses when the entry carries no stamp.

## `public void QLecternFrequencyIntroduce(UIElement section, FrameworkElement chip, TextBlock name, TextBlock band)`

Takes the frequency section, its chip, and the name and band lines the chip shows.

## `public void QLecternSpeechIntroduce(UIElement section, ItemsControl speech, TextBlock unit)`

Takes the speech section, its list of parts of speech and the line naming the lexical unit.

## `public void QLecternNoteIntroduce(UIElement section, Panel note)`

The note is Markdown, drawn as blocks by `QMarkdownFace` inside the note card.

## `public void QLecternFavoriteObserve()`

Hands the heart's new state to the gate, then draws the heart from the stored value it answers.
A refused mark thus springs the heart back, and the display reports the failure.

## `public void QLecternGraspObserve(int step)`

Hands the star row's new step to the gate, then draws the stars it answers.
A press on the standing step thus comes back cleared, since the gate owns that rule.

## `public void QLecternHoverRefine(int pointed)`

Words the step under the pointer beside the stars, or the set step once the pointer leaves.

## `private void QLecternEntryRefine()`

Draws the shown header, the speech names, and the stamp row, then gives the page to the entry.
A section the entry left empty collapses instead of standing as a blank line.

## `private void QLecternUnitRefine(string key)`

Names the entry's lexical unit above its parts of speech through the key Conduct chose.
An unchosen unit hides the line, so a bare entry shows no empty label.

## `private void QLecternFontRefine()`

The headword takes the pack's font, so the two views never differ in family or size.

## `private async void QLecternFlagRefine()`

The flag comes from `QEnsignImage`, which every tab holding a display reads too.
The first call may await a fetch, so a later entry may be shown before it arrives.
The flag is then drawn for the language the pill shows now, not the one opened.
The globe stands in until then, and wherever no flag is known.
A failed flag load is reported by the catalog, so the globe stays and the view goes on.

## `private void QLecternEmptyRefine()`

Empties the header, the sections and the stamp, and gives the page to the unselected notice.
