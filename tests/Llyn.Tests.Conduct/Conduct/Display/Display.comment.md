# Display

This folder holds the reading view's areas, reads and gates.
They cover card, sense, accent, grasp, mention, playback and sound blocks.

## `TCardSense`

Covers the links from a collocation to meanings, as Conduct hands them to the editor and the read view.
Most cases save one entry with a parent meaning and two children, a second top-level meaning, and two collocations.
One collocation links the first child, then the second top-level meaning.
The other collocation links nothing.

## `CardSenseRead_NestedMeanings_ReturnsDepthAndPath`

The link menu lists every meaning in tree order with its depth and dotted path.
Children sit at depth one under their parent.

## `DisplayCardRead_LinkedCollocation_HomesUnderFirstLink`

A first link to a child homes the collocation under that child's top-level parent in the read view.
The homed row reads its phrase, and the second top-level meaning, though linked too, holds no collocation.
The first meaning's collocation block reads ready to show, and the second meaning's does not.

## `DisplayCardRead_FurtherLink_AddsSeeLine`

The further link adds one "see" line under its own meaning, and the home meaning gets none.
The line names the expression and the full path of the nested home.
It carries the homed row's id for the click.

## `DisplayCardRead_UnlinkedCollocation_ListsOther`

A collocation without links is listed in the Other part, which then shows.

## `DisplayCompassRead_HomedCollocation_AddsMutedHeadingRow`

This case shows its own draft, with one collocation homed at the first meaning and two at the second.
Each meaning with homed collocations gets one muted depth-two row after its own row.
It never gets one row per collocation.
The row points at its meaning's card and carries no number.
Both rows are worded with `Display.CollocationHeading` alone, so neither shows its count.
No collocation is unhomed, so the collocation part is absent.

## `DisplayCompassRead_TwinHeadings_KeepsHeadingsUnnumbered`

Two meanings each home one collocation, so their heading rows read alike.
A caption is not a name, so the twin rule leaves both unnumbered.

## `DisplayCardRead_NoStoredFold_ReadsRowOpen`

No fold is stored for the shared entry, so the homed row and the Other row both read open.

## `DisplayCardRead_HomedCollocation_WordsPlainHeading`

The first meaning, home of one collocation, carries the plain caption with no count.
The second meaning homes none, so its caption stays empty.

## `DisplayCardRead_HomedRow_MapsMarkers`

The homed row carries the layout's phrase and its "also 2" marker, for the further link to the second meaning.
It has no example and no translation, so those markers stay empty, and the meaning leaf carries none.
The verdicts agree, so the row is shared but not quoted.

## `CollocationLayoutRead_HomedCollocation_FillsMarkers`

This case shows its own draft, whose collocation has a title but no expression, two examples and three targets.
The phrase falls back to the title.
The flags name each language once, in first-seen order.
The example count is two.
The further paths name only the second meaning, though the first is linked twice.

## `TCompassDraftCreate`

An unsaved draft for the compass cases, with the meanings 41 and 42, and one collocation per `senses` item.
Each collocation, numbered from 51, links the meanings its item names.

## `TCardSenseSave`

Saves the shared entry, with meaning ids minted for the draft so the links can name them.

## `TDisplay`

Covers the reading view rules in the conduct, driven with no window.
Clearing a shown draft drops both the draft and its entry.
The stored reflexes loaded for the shown entry win over the shown draft's empty ones.
With no entry given, or an entry no source has ranked, no frequency chip shows.
An engine that answers no gauge reads no frequency.
The compass lookup echoes each key but `Display.CollocationSingle`, so every other wording key shows in the names.
The unlinked collocation section is named `Display.CollocationOther`.
A titled card keeps its title, an untitled one its kind, and an uncertain one the unknown mark.
The collocation kind is looked up to the meaning wording, so the two twins carry one name and are numbered.
Each card carries its number, its depth and its place in its section's list.
With no draft shown, the parts are named through the lookup and carry no cards.
A band at or below zero reads zero stars, the unknown tier and no rank.
Its spare stars then fill the whole row.
A band on the scale keeps its stars and tier, and its spare stars fill the row.
An empty source stays empty.
A band past the scale throws nothing and reads the unknown tier with its key and no spare stars.
The band stays non-negative and the source passes through.

## `TDisplayAccent`

Covers the reading view's pronunciation block, read and flag load alike, on a real workspace.
The wing opens the entry, so each case stands on its display's accent area.
Each flag load goes through the `TInterfaceConductSound.TDisplayAccentLoad` relay.
An English entry answers square brackets, no contour, its phonetic reading and one row for its second pronunciation.
Each variety carries its label and flag keys, and the pack draws varieties as flags.
With the respelling switch on, the primary reading prints the respelling stored beside it.
With nothing open, the block has no reading, no row and no flag.
A pack that draws its varieties as flags hands the store each flag under its language and variety.
The block then answers again, with the pack's tone and flag verdicts.
An entry opened while the flags load wins, so the late load answers nothing to paint.
A flag store that throws shows `Sound.LoadFailed` once, and the load answers nothing.
With nothing open, nothing loads and nothing answers.
The tonal test pack draws its one variety's flag from a local file, so no flag is fetched.

## `TDisplayArea`

Covers the reading view's area, its gates, reads and change events, driven with no window.
The wing opens and chooses entries, so most cases stand on its display.
A loaded entry opens once, with its headword, language, speech names, note and worded stamp ready to draw.
The note arrives parsed, so its italic span is already a block's span.
An entry without a unit carries an empty unit key.
An entry gone from the store chooses nothing, so the view closes and drops its draft.
Closing drops the shown draft and blanks the header.
The view reads blank before an entry opens, not after, and blank again once it closes.
A followed panel's load opens its entry here, and its clearing closes the view.
The library attaches its display to its panel when it is created, so the test attaches nothing.
A notice about the chosen entry reloads its draft and opens it again.
With nothing chosen the notice changes nothing.
A workspace swap closes the view.
A favourite bulletin about the chosen entry raises the favourite change and no grasp change.
Both the favourite change and the heart's gate live on the display's grasp area.
The heart's gate flips the stored mark and answers what the store now holds.
With no entry chosen the heart stays unmarked, reads unmarked and does not flip.
Conduct chooses the one-off key for the driver's frequency lookup, and an unranked entry shows no chip.
The wing, entry and atelier builders here are shared with `TDisplayGrasp`.
The atelier's media port only answers a stop, since closing the view stops its play.

## `TDisplayCard`

Covers the reading view's card, media and chip reads and click gates.
Most cases run on a real workspace, where the wing opens the entry and the display's area answers.
The media cases call the card reads directly with a real or fake media port.
A Meaning card arrives ready, and the Meaning section shows.
No other collocation is listed, so the Other collocations section stays hidden.
An unknown title carries the unknown mark's key, and the phrase a Meaning has none of arrives blank and muted.
Its link arrives named, and its stored tag arrives worded and ready to open.
A sentence row arrives with its frame and divided sentence ready, and names its line key.
An unknown role reads the unknown mark inside the head's brackets.
A sentence with no citation or gloss answers an empty citation and no gloss.
With nothing open, no meaning or other collocation is answered, and neither section shows.
A picture or video row with no location carries the empty verdict, and a located one does not.
An image or video row carries the address or screen the port answers for its plain location.
It carries none where the port answers none.
The map hands the port the plain text, including a blank one, and settles nothing itself.
A written video span carries its start and end moments.
A row with no span starts at zero and has no end.
Over the real media outlet, a hosted location padded with spaces still resolves and carries its film id.
A plain web file carries no film id, and a workspace path that reaches no file carries no screen.
A blank location carries no screen.
With no entry chosen, no usage is listed.
A stored situation, register or tag raises its own tab.
A link chip raises its entry in the library tab.
A record never saved, a link to no entry and an empty click open nothing.
Each card answers its list and place, and an unknown card or an empty view answers nothing.
A line key read from one entry finds nothing once another entry is shown.
That holds for a click and for the word cursor.

## `TDisplayGrasp`

Covers the reading view's grasp area, its limit, reads and gate, driven with no window.
The hostile cases stand on a fake grasp port, and the gate cases on a wing over the real engine.
Any engine grasp limit reads back unchanged, except a negative one, which reads zero.
So the star control never takes a negative limit, as `CDisplayGrasp` promises.
Any stored step the engine answers reads between zero and the limit, as `CGrasp` promises.
A move past either end stops at the end, and the engine keeps the stepped value.
A negative limit counts as zero, so every step then reads zero.
The label is the engine's wording of the clamped step, not of the stored one.
A new step is stored and answered with its wording, and the standing step pressed again clears it.
A move left from step one stores step zero, so it also clears the grasp.
A move left at step zero keeps step zero.
With no entry chosen the gate stores nothing and words nothing.

## `TGraspPortCreate`

A fake grasp port answering only the grasp limit, the stored step and the step's wording.
The wording is the step itself, so a fact sees which step was worded.
`TEsteem` fakes its grasp reads here too.

## `TDisplayMention`

Covers the reading view's word click gates over a shown entry on a real workspace.
A line names only its line key, read from the card gate as the driver reads it.
A sentence naming no language is read in the shown entry's language, and French finds nothing there.
A sole entry opens at once through the Library's leave question and leaves an empty menu.
A declined leave opens nothing.
Several entries stay offered under the found word, and nothing opens or asks.
An astral character before the word moves its UTF-16 unit one past its code-point offset.
That holds when the word starts in a Mention piece before the clicked piece.
A word nobody holds offers an empty menu.
A stored Mention opens its entry and raises its sense.
A row the shown entry does not hold offers an empty menu.
Meaning and collocation sentences may share a stored row id, yet their line keys differ.
A cursor started on the collocation sentence's line key finds that sentence.
Its first step then leads with that sentence's first letter.
Redrawing an unchanged card, or reopening its entry, keeps every line key.
A failed find reports `Mention.FindFailed` and offers nothing.
The word cursor starts at the first character of a line's sentence.
Its steps stop on one text element, so an astral character is one stop.
Its seeks stop on each Mention piece and on each word that follows a space.
The lead is the text from the word's head up to the cursor.
A find with no cursor started, or on a sentence the card does not show, answers nothing.
With several entries the find offers them all, opens none, and leaves the lead at the found word.

## `TDisplayPlayback`

Covers the playback area's read and gates on a real workspace.
Where a case must see the player, a fake media port records each file and level handed to it.
It builds its wing and entries through `TDisplaySound.TDisplayWingPrepare`, `TDisplayDraftCreate` and `TDisplayEnglishSave`.
An accent row with audio shows the tray, while the missing own recording hides the button.
Nothing open hides both.
The row is named by its accent id and plays that accent's file, from a draft with two accents.
The button plays the shown draft, each at the level handed in.
Nothing open plays nothing, and the cancel stops the latest play's ticket.
Stopping play with nothing playing leaves the shown entry standing.

## `TDisplaySound`

Covers the reading view's sound area, its reads and gates alike, on a real workspace.
Most cases open entries through a wing.
The reflex-resonate case shows a draft directly through an editor's display.
The playback area's gates live in `TDisplayPlayback`.
The fanqie, script and paradigm blocks and their clicks live in `TDisplaySoundBlock`.
Both build their wing and entries through this class's helpers.
The font read checks the shown entry's Headword and Gloss typography.
Nothing shown answers the blank font, so the view keeps its theme.
A Korean entry with a Hanja row answers the scheme's key and name.
It also answers its linked cells and the pack's glyph font.
With nothing open, or for a language without a glyph section, the glyph row is hidden and empty.
A character raises the entry the engine resolves for it, for the library tab.
A blank cell is refused, shows `Glyph.OpenFailed` and raises nothing.
The transcription read leaves out the Hanja row and an empty row, and nothing open lists nothing.
Only the written reflex row stands, marked as its language's lead.
Its anchor label is empty, and anchoring is unavailable.
Resonating reloads the stored entry, so its rows replace the shown draft's empty list.

## `TDisplaySoundBlock`

Covers the sound area's fanqie, script and paradigm blocks and its cell clicks on a real workspace.
It builds its wing and entries through `TDisplaySound` helpers, with `TDisplayCatSave` its own.
With nothing open, the fanqie, script and paradigm blocks are empty.
The fanqie and script blocks are not pending, and the paradigm carries no font.
An English entry has no rime-book, script or paradigm rows.
With morphology off, saving the entry starts no inflection lookup.
The unanswered plural slot then shows the absent tip.
The source is gated, so a started lookup would show the pending tip instead.
With morphology on, saving the entry starts a lookup that the gated source holds open.
While it is held, the unanswered plural slot shows the pending tip.
The source then answers unavailable and leaves the slot with no form.
The display, which never holds the draft, then shows the lost tip.
Each diwei or stem click is raised in the shown entry's language, and nothing open raises nothing.
The initial flag picks the cell kind.
A blank key of either kind raises nothing and answers false.
Setting a rank before any entry is shown calls nothing.
After entry 7 is shown, setting rank 2 with the raise flag hands the gate `7,3,2,True`.

## Inline notes

### TDisplaySoundBlock.cs: private static LEntry TDisplayCatSave(LEngine engine)

Saves an English noun whose paradigm asks for a plural nothing stores.

## Stamp

Commit: `44d0e95bcfcc`
- `TCardSense.cs` `bc31f6cddd2ca6ae`
- `TDisplay.cs` `c18d439ee37ac348`
- `TDisplayAccent.cs` `bc384431fa7b13d5`
- `TDisplayArea.cs` `660956a4da7b515e`
- `TDisplayCard.cs` `078d33f702ae6442`
- `TDisplayGrasp.cs` `7f3390215a60e0ff`
- `TDisplayMention.cs` `fe2130ecf737059c`
- `TDisplayPlayback.cs` `4841acad0b56316f`
- `TDisplaySound.cs` `3d21b438c154a999`
- `TDisplaySoundBlock.cs` `a7326a7b4d646a9f`
