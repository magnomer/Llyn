# LLectern.cs

## `public sealed class LLectern`

The reading view's deportment, standing between the veneer and [LDisplay](../../Llyn.Conduct/Display/LDisplay.comment.md).
It draws the header, the speech, frequency, note and stamp sections, and the empty notice.
Its halves draw the rest, and the other members forward to one `LDisplay` member of the same shape.
The veneer names only this class, so no veneer reaches Conduct.

## `public LLecternCard LLecternCard { get; }`

The card half, built over the display, which draws the cards, incoming rows and etymology and answers their clicks.

## `public LLecternAccent LLecternAccent { get; }`

The accent half, built over the display's sound, which draws the primary pronunciation and the accent rows.

## `public LLecternSound LLecternSound { get; }`

The sound half, built over the display's sound, which draws the transcriptions, glyph row and every phonology section.

## `public LLecternPlayback LLecternPlayback { get; }`

The playback half, built over the display's sound, which drives the play buttons and the volume slider.

## `public LCompass LLecternCompass { get; private set; } = null!;`

The floating contents, built once the veneer hands over its controls.
Card scrolling goes through its scroll, so a card lands where a row would put it.

## `public void LLecternCompassAttach(`

Builds the compass over the display and the view's controls.
The compass subscribes to them itself, so the veneer names no compass handler.

## `public void LLecternAttach(LWindow window, UIElement empty, UIElement contents, Action swathSeam)`

Takes the window deportment, the unselected notice, the page and the swath's clear.
The swath's clear is a seam, since the band a reader drags lies in the veneer.
Showing or clearing an entry drops the band, since the text it spanned is gone.

## `private bool LLecternAttachCheck()`

Whether a view attached, false before `LLecternAttach`.
An attached view with no compass throws, since the veneer skipped `LLecternCompassAttach`.

## `public void LLecternHeaderAttach(`

Takes the headword, the language pill, the heart and the star row.
The star row is a veneer control, so it is reached through its step and limit properties.
The limit is set here once, from the engine's last grasp step.

## `public void LLecternNoteAttach(UIElement section, Panel note)`

The note is Markdown, drawn as blocks by `LMarkdownFace` inside the note card.

## `public void LLecternObserverAttach(DispatcherObject surface)`

Attaches every subject the view listens to, each marshalled onto `surface` through `LObserver`.
Favorite, grasp, frequency, inflection, reflex and entry bulletins count only for the chosen entry.
Script and fanqie bulletins redraw their sections, and a workspace swap clears the view.
Example, situation, reference, author, tag, register and settings bulletins reload the draft, since each can change it.

## `public void LLecternShow(LEntryDraft draft)`

A lectern no view attached to does nothing, since panels announce drafts in tests with no veneer.
A vista choosing nothing clears the view instead, since the draft then stands on no entry.
Otherwise it hands the draft to the display, then shows the play button and tray it earns.
It fills the heart and stars, the header, speech, frequency, note and stamp.
A field the draft left empty collapses instead of standing as a blank line.
The accent half then draws the pronunciation and accents.
The sound half draws the transcriptions, glyph row, reflexes, fanqie, script and paradigm.
The card half draws the cards, the incoming rows and the etymology.
The compass rebuilds last, a dispatcher turn later, once every section's visibility is set.

## `public void LLecternClear()`

A lectern no view attached to does nothing, like a show.
Drops the shown draft, which stops playback, and hides the play button and tray.
The accent half collapses the pronunciation, and the sound half empties every section it draws.
The card half empties the cards and the incoming rows.
The compass drops its rows and hides.
The header, sections and stamp empty, and the unselected notice takes the page.

## `public void LLecternClose()`

Stops this view's own play, since the view is going away.
A sound another view started plays on.

## `public void LLecternFavoriteHandle()`

Stores the heart's new state, then redraws it from the stored value.
A refused mark thus springs the heart back, and the display reports the failure.

## `public void LLecternGraspHandle(int step)`

Stores the star row's new step, then redraws the row from the stored value.

## `public void LLecternHoverHandle(int pointed)`

Words the step under the pointer beside the stars, or the set step once the pointer leaves.

## `private async void LLecternLanguageShow(string language)`

The flag comes from `LEnsignImage`, which every tab holding a display reads too.
The first call may await a fetch, so a later entry may be shown before it arrives.
The flag is then drawn for the language the display shows now, not the one asked for.
The globe stands in until then, and wherever no flag is known.
The headword takes the pack's font, so the two views never differ in family or size.

## `private void LLecternEntryUpdate()`

Reloads the chosen entry's draft and shows it again.
A refused load leaves the view as it stands.

## `private void LLecternDraftShow(LEntryDraft? draft)`

Shows a reloaded draft, or clears the view when the entry is gone from the store.

## `public void LLecternLoadedShow()`

Shows the draft the last entry load held, or clears the view when that entry was gone.

## `public void LLecternDraftShow(LDraft draft)`

Shows the content of a draft a panel loaded.
Panel deportments wire their load events here, so no veneer relays a draft.

## `public static bool LLecternNarrativeCheck(bool editable, string text)`

Forwards statically, since the check holds no state.
