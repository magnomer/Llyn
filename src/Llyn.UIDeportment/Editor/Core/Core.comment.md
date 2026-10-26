# Core

Core holds the shared editable entry view and its halves.
One editor serves every panel that edits an entry, and the panels differ only in what they open it on.
The editor is split by role.
The parts are the card half, the sound half, the typefaces, the tab strip and the esteem strip.
Each part holds only the Conduct facets it uses, never the whole editor.

## `QEditor`

The editor driver works over the Veneer `PEditor` every editing panel places.
It holds the plain scope the panel pulled by contract ID, so no Deportment control stands between.
It keeps the headword, the note, the command rail and the chronicle, and routes the editor's notices.
Other parts live elsewhere: `QMarker`, `QCategory`, `QUnit` and `QFreedom`.
The input panel's editor stands on no entry and never puts it on one.
A browse-style panel's editor is opened on the entry its panel edits.
Which is which is a fact of the deportment, read off the vista the tab restored.
The scope is registered as a chronicle host, so undo and redo inside the editor walk its own desk.
The markup's dictionaries are scanned for look rows, so the popup rows light without triggers.

### `QEditorChronicleChanged`

An embedded editor hides its own rail, and the panel around it carries undo and redo.
This notice lets that host panel light its own buttons.
The rail reads the chronicle's kept state, not a fresh read of the tenure.
So a notice costs no Conduct query.

### `QEditorIntroduce`

Each part gets only the facets it uses.
The sound half takes the whole editor, since it is a composer too.
Every subscriber makes at most one Conduct read, so a notice fans out here and never inside a member.
The draft's writes follow the order the draft once showed in, so the sentence frame precedes the cards.
The sentence frame and mention chips stay here, since their order against the card Refines matters.
The editor's own subjects arrive as area events through the marshal Host handed the atelier.

### `QEditorExitRefine`

It stops the editor through its one close gate, then releases playback.
A panel whose editor the window's exit gate stops in Conduct calls `QEditorPlayerRefine` alone.

### `QChronicleUndoObserve`

The step runs inside `QChronicle.QChronicleCaretRefine`, so the caret stays at the end of the focused box.
Redo uses the same wrap.

### `QEditorDraftRefine`

A box that already holds the text keeps its caret.
The note is written only while `CEntryNoteCheck` says the box does not hold it yet.
So a line break just typed at the end of the note survives the draft's echo.
The command rail shows only for the editor that owns its entries, which is the input tab's.

### Inline notes

### QEditor.cs: `surface.AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(QEditorFocusObserve))`

Focus leaving any box is caught as the event bubbles, so unsent typing goes before the focus does.

### QEditor.cs: `new QStack(surface)`

The tab strip wires itself in its constructor and needs nothing after, so the editor keeps no handle to it.
Its own subscriptions on the strip's controls keep it alive.

## `QEditorCard`

The card half composes field, sentence, media, list and etymology drivers over shared presentation collections.
Both card lists share one renderer, while each field keeps its own Conduct gate.
Gloss language choices use one shared collection.
The card driver receives one example filler assembled from sentence, Gloss and citation drivers.
The sentence driver is exposed so the editor can order frame and mention redraws around card rendering.
The prospect driver is exposed for translation and mention offers beyond the card fields.
The etymology driver takes the etymology gate when introduced and shares that prospect driver for its box words.
The list driver holds no card collections, since order and position are Conduct's.
Every chip field, the proffer, the prospect, the slate and the card driver take the caret gate when introduced.

### `QEditorStartRefine`

A new tenure clears both card lists, so no presentation item from the former draft is retained.

### `QEditorMeaningRefine`

The card lists are drawn from the card rows of Conduct's walk, the same rows the console frame steps through.
The editor refines on Conduct's walk change, so a caret move redraws the cards too.
A draft announce alone no longer drives the card lists, since the walk is rebuilt from it.
So both faces show one order and one set of fold verdicts and carets.
Ready meaning cards need no per-card link lookup.
The meaning list is refined with the meaning flag.
The collocation list is refined with the collocation flag.
So each placard learns its kind where it is built.
Their folded peek derives from definition wording, and a collocation's from expression wording.

### `QEditorTextObserve`

The routed text handler accepts only Gloss and image row contexts.
Other fields remain with their own drivers.

### `QEditorFieldObserve`

Keyboard focus is required before raw field text reaches a driver.
The row's type selects Gloss or image handling without reading a field name.

## `QCadence`

The editor's sound panels consume ready sounding blocks.
Box switches use the separate fold facet.
Panels expose switches but own no Conduct dependency.
Contract lookups remain within the retained editor surface.

### `QCadenceIntroduce`

Desk, entry and timbre supply redraw events rather than panel data.
Script decode failures use `Display.ScriptFailed` through the supplied ledger and envoy.
Entry draft notifications repaint box states on draft preparation and on entry-scoped fold bulletins.

### `QCadenceFoldRefine`

Both box states are read from the held entry's fold facet, not from the supplied draft.
The panels paint those stored verdicts without writing them back.

### `QCadenceSpellingObserve`

The returned verdict goes to `QLook.QLookCheckedRefine`, which restores a refused switch.
Accepted writes repaint through entry draft notifications.
The Script switch follows the same gate and recovery policy.

### `QCadenceScriptRefine`

Image scanning reports decode failures through the panel's deferred notice.

### `QCadenceDiweiObserve`

Raw category identity reaches sounding, which owns navigation rather than the panel.

### `QCadenceReadingRefine`

The editor supplies headword text, while sounding supplies the representative reading to display.

## `QEditorFont`

This class writes every font the language's timbre names for the editor.
The headword and its measuring twin, the example and gloss typography, and the glyph row each take their own role.
Sound keeps only sound, so `QEditorSound` and `QGlyph` read no font.
Each Refine reads its own font from the timbre, so each asks Conduct once.
The glyph typography goes into the glyph list's resources, so the field takes it and the scheme label does not.

## `QEditorSound`

The sound half holds the pronunciation row, its contour, and the drivers of the sound panels.
It keeps the one recording player, which the playback, the volume, the clip preview and the accents share.
The volume slider is painted by the window's one `QVolume`, which every editor shares.
The transcription facet is read once, so the glyph and transcription rows share one.
The contour redraws as the pronunciation is typed, beside the gate that hears the same text.

### `QEditorFailureObserve`

The player's failure is subscribed once, since the clip, the accents and the playback share it.
The clip's own failure handler still runs beside it and only resets a marked preview.
The failure shows under `Sound.PlayFailed`.
The player is this driver's own medium, and only it raises this event.
So no Conduct gate carries it, as with a picture the script decoder refuses.

### `QEditorPronunciationRefine`

It writes the primary reading as the editor reads it, respelled when the pack respells.
The draft is ignored, since only the editor knows the respelling.

### `QEditorTimbreRefine`

These writes follow the language's sound facts: the brackets, the tone contour and the silent switch.
The contour is read again, since a new language may change it over the same text.

### `QEditorContourRefine`

The typed text runs ahead of the draft, so the read takes the field's text.
Conduct's scale goes to the box too, so the box names no Conduct type.

## `QInput`

The input panel is the shared editor standing on no entry at all.
It is the form an entry is created through, and only created.
It never opens on a stored entry, so a store always writes a new entry and leaves the form empty.
Correcting an entry that exists is the browse-style panels' work.
Nothing reaches Conduct before the window introduces it.

### `QInputIntroduce`

A workspace move empties the form in Conduct, since the editor opens a fresh draft on `CWorkspaceOpened`.
A different folder is a different database, so whatever was typed against the old one is begun again.

### Inline notes

### QInput.cs: `_qInputEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, atelier.CAtelierInputCreate(envoy))`

The editor this panel attaches stands on no entry, because this panel creates them.
A form that stood on one would turn the next store into an update of it.
That is how a session's second entry used to overwrite its first.

## `QRegard`

This is the editor's esteem strip: the favourite mark, the grasp steps and their label, and the frequency chip.
It holds no draft state, and every write reads the editor deportment's esteem.
A tenure start reads every part again, one subscriber each, since a bulletin comes only on change.

### `QRegardHoverRefine`

The label previews the step under the pointer, so a click never lands on an unread step.
With no step under the pointer, the label shows the standing grasp's own label.

### `QRegardGraspObserve`

A press sends the pressed step to the esteem gate and keeps no step of its own.

### `QRegardMoveObserve`

A key sends a relative move to the esteem gate, which keeps the step within the limit.

### `QRegardFrequencyChip`

The chip is drawn as the reading view draws it, and a frequency is fetched rather than typed.
A form standing on nothing hides it, and nothing beside it moves when it does.

## `QStack`

The stack holds which region of the editor is open.
The tab strip picks one of meaning, collocation, etymology or note, and collapses the other three.
It follows the navigation strip's selection shape, over the editor's own contents.
The strip needs no Conduct editor, so nothing waits for an introduce.

### `QStackSelected`

Selection lives in each button's cue rather than in a swapped style.
A style swap rebuilds the template, which would drop any running animation and lose the hover state.
The cue also tells selection apart from hover, so the selected tab never paints a hover fill.

### `QStackPillPlace`

One pill sits behind the whole strip and moves to whichever tab is selected.
Separate indicators would blink between positions rather than travel.
The width follows the tab, because tabs size to their own text.
A click glides, and a layout change places the pill outright.

## Stamp

Commit: `c05429890cec`
- `QCadence.cs` `c49dba67646f341c`
- `QEditor.cs` `10fb7989f576c790`
- `QEditorCard.cs` `53ba2c7460e0405c`
- `QEditorFont.cs` `4da791778932c52b`
- `QEditorSound.cs` `37d1a4f52e9d1a42`
- `QInput.cs` `3268e212c7a17a65`
- `QRegard.cs` `b9602ef19076b185`
- `QStack.cs` `5302ff128811c7a8`
