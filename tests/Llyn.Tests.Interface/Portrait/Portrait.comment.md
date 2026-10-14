# Portrait

This folder holds the export and Joplin relays and the shared exemplar draft.
It also holds the press, board, proof, queue, setting-store and Joplin fakes they print or push to.

## `TExemplar`

The one fully populated entry draft the export coverage tests share.
Every typed field carries its own distinct string, except the gloss, reflex and Ancestor languages, which share "Korean".
So a field an export drops names itself in the failure.
The strings are plain, so no escaping can hide a miss.
The lexical unit and freedom are set too, so an export that drops either fails the coverage test.
It sits in the interface folder because it builds production records directly.

## `TExemplarHidden`

The typed strings the reading view never prints, so the portrait coverage test does not expect them.
The respellings stand behind the English switch, which is off, so the IPA prints in their place.
Audio, source and the syllable cut sit behind a reading, which prints alone.
A situation's description and kind belong to its own page, and an image is a picture, not text.

## `TExemplarCreate`

Saves the translation target, the mention target, the cited reference and its author first.
Then it builds the draft that points at them by id.
Speeches and inflections take their ids from the English pack, since markup names them and import resolves them there.
A third speech is typed by hand, so a custom speech travels too.
The mention names the sense of its target, so the sense path is written and resolved.
The collocation links the second meaning by a minted id, so its meaning link saves, prints and travels.
Apart from the headword, the second form's text and the shared "Korean", no string is a prefix of another.
The noun plural is irregular on purpose, so the paradigm section shows it.
Regular stays false, since the engine owns it.
The etymology map holds two Ancestor boxes.
One links the mention's target entry, and one is typed with a language and gloss.

## `TExemplarSave`

Saves the draft and returns its id first, then every entry it links to.
A markup export of the whole list resolves every name on arrival.

## `TExemplarText`

Reads the strings typed into an exemplar draft, for the export checks that look for each one.

## `TExemplarTextRead`

Returns every string the draft carries.
It is read from the draft itself and never from a second list.
Ids, numbers and unset values are not strings and are not listed.
The adder walks each card and then its children in draft order.
Sentences add their particle and dependence, and examples add text, language and glosses.
Situations, registers, tags, images and videos add their typed strings too.
Unset and unreadable state values stay out.

## `TExemplarNormalize`

Drops every property without a setter from the contract, the same trim the draft archive applies.

## `TExemplarShapeRead`

The draft as indented JSON with every id zeroed, so two workspaces' drafts compare as one shape.
Ids, link ids, Ancestor targets, sense ids and pack value ids are zeroed in place.
The id lists are zeroed too, their counts kept.
The id lists include the collocation's meaning links, since stored meaning ids differ between workspaces.
The pronunciation with no syllables keeps its empty list, so a syllable dropped on import still shows.
Derived properties are left out the way the draft archive leaves them out.
So a derived id never enters the shape.

## TInterfacePortrait

The relay for the export surface and the Joplin livery.
Tests call production operations only through here, so a renamed operation breaks in one file.
Most builders hand their arguments to the constructor unchanged, and most format relays forward to one named production member.

## `TPortraitLabelRead`

A fixed English label set, so a test asserts on known words without the localization files.
Its unit and freedom names are English too.
It words the unlinked collocation heading as "Other collocations" and the stub as "→ {0} see {1}".
It words the caption over a meaning's homed collocations as "Collocations · {0}".

## `TPortraitLegendRead`

A fixed English legend with no reference kind names, so a page read needs no localization files.

## `TLiveryFormat`

Writes a Joplin entry body through `LLiverySheet.LLiveryFormat` over the loaded theme.
It passes a fixed style id and the `note` it is handed.
With no `note`, every entry id stands as its own note id.
It passes the `link` it is handed, or one that answers empty, so every chip stays plain by default.
It answers the whole note, so a test reads the body and the parcels.
The series and rime-table overloads use the matching production overload with the same style id and default `note`.

## `TCourierNoteBuild`

Builds the courier's `note` map through `LCourierNote.LCourierNoteBuild`.
It hands over a `LLiverySheet` over the loaded theme, so the ids match a real push.

## `TCourierSend`

Runs one push through the engine's courier clerk with the readers it is handed.
A test can then shape the entry pages and the language reads without fetching anything.
Its lookup answers each key itself, so a test finds notebooks and notes by their keys.

## `TLiveryLanguageBuild`

A push test hands the language read to `TCourierSend`, so it chooses which reconstruction notes the language carries.

## `TStemPageCreate`

Without members, the page makes one bare member per character.

## `TStemMemberCreate`

A test that has reflex rows hands one guise per row, as the engine does.

## `TLiveryDiweiCreate`

Builds a rime-table category read whose page holds no sections.
A push test needs only the kind and key, since those place the note and address the entry chip.

## `TThemeCreate`

Builds a theme from the colours alone, so a test controls every role `TLiveryStyleFormat` reads.

## `TSentenceDraftCreate`

Wraps an example with an unspecified particle and dependence.

## `TPortraitTextRead`

Lists every string a page shows, in reading order, children recursed.
A writer that carries the page whole prints every one of them.
So a coverage test reads its expectations here and never from the draft.
A section lists its strings, then its children's.
A band, card, kind, list or row prints its heading, so only those roles list it.
An image is a picture and not text, so its location and span are not listed.
A video is a link drawn as text, so its location and span are.
The adders skip empty strings, since an empty string shows nothing.
Lines add their label and then their text.

## `TPortraitSectionCreate`

The short overload builds a section with a note, leaving every later member at its default.
The full overload builds a section with an empty note, a band unless the role says otherwise.

## `TPortraitPageCreate`

The short overload leaves the favorite flag and the lines at their defaults.

## `TOutpostFake`

A Joplin that lives in memory, so a push test needs no running Joplin and no network.
It records every notebook, note, save and trash, so a test reads back what the push did.
It fails only a note save whose title a test lists, so a push fact can watch one note fail.
Its trash keeps Joplin's guard, so a push fact sees which trash the guard refuses.
The find answers a fixed port whatever port is asked, so the push never refuses for an absent Joplin.
The warrant start and check throw, since the push facts never connect.
The tag and parcel saves keep nothing, since no push fact reads them.
The notebook list holds every notebook by id with its parent and title.
A later save replaces an earlier one as Joplin does.
The note list holds every note as last saved, without the trashed ones.
The saved list holds the id of each note save in order.
So a test tells a kept note from a sent one.
The trash list holds each trash asked for in order, whether the guard let it through or refused it.

## `TOutpostFakeRefusal`

The titles whose note save fails, empty unless a test lists one.
A listed title throws before anything is held or recorded.
The throw is no timeout and no refusal, so the push counts the note as failed and goes on.

## `LOutpostNoteRead`

Answers the held body, or null for a note never saved or already trashed.
So the push keeps a note only while the fake still holds it unchanged.

## `LOutpostNoteRemove`

Records the trash ask, then applies the rule `LOutpostHttp.LOutpostNoteRemove` applies.
It refuses a note it does not hold and a note outside the given folders.
It also refuses a body that does not start with the mark.
A refused trash answers false and drops nothing, so a note outside Llyn's notebooks survives.
Otherwise it drops the note and answers true.

## `TPress`

A press that neither prints nor writes a file, and remembers what it was handed.
Tests read back the page, the path and whether a board was opened.
So a print or a PDF export is checked without a browser or a printer.
A save records the page and the path and writes nothing.
A board open records the page, marks the press as opened and answers the board.
So a test can check which sheet a print session opened.
Every open answers the same board, so one press serves one print session in a test.

## `TPress.TPressBoard`

The board property is the one every open answers, a fresh fake unless a test hands its own.
A test sets it before the engine opens a print session.
It is typed as the Core port, so a fault sweep can hand a fault double over a fake board.
It then reads back what the session drew, printed or disposed on that board.

## `TPressBoard`

A board that holds no browser, answered by `TPress` when a print session opens.
Tests reach the session route without a browser engine or a printer.
Every task completes before the call returns unless a test sets a hold.
The board records what it was handed.
Draws and prints run one at a time, as the Windows twin runs them.
The session clerk relies on that order, so the fake keeps it.

## `_tPressBoardTurn`

The turn a draw or a print takes for its whole run, held across any hold.
A second call therefore waits, and records its setting only once its turn starts.

## `TPressBoardStatus`

The status a print answers, done until a test sets it.
So a test can make the printer offline or failing.

## `TPressBoardHold`

A gate that draws and prints wait on until the test releases it.
Null by default, so every task completes at once.
So a test can hold a draw open while a newer setting arrives.

## `TPressBoardCover`

Wraps each proof the board hands out, when a test sets it.
The fault sweep wraps the proof in a fault double, so a page draw can fail on demand.
The proof list still keeps the bare proof, in order, so a test sees which ones were disposed.

## `LPressBoardDraw`

Waits its turn, records the setting and counts the draw, then waits for any hold.
Answers three black pages on the chosen paper, turned when the orientation is landscape.
The print waits its turn, records the setting, waits for any hold and answers the status.
The export records the setting and the path and writes no file.

## `TPressProof`

A proof of three black pages, all on the one paper it was handed.
`TPressBoard` answers it, so a gate draws pages without a browser or a PDF reader.
The page count is always three, so a range can include some pages and leave others out.
Every page answers the one paper, already turned by the orientation.
A page draw waits for any hold, then answers a PNG of the paper at the given dots per inch.
So a test reads the drawn size from the PNG header.
The disposed flag lets a test see a superseded proof released.
The hold lets a test replace the proof while one of its pages is drawing.

## `TPressProofCreate`

Builds a valid one-bit gray PNG of the given size, every pixel black.
It needs no drawing library, so the fake stays in plain .NET.
Each chunk is written with its length and its CRC-32 check.

## `TPressQueue`

A printer queue that asks no spooler and answers fixed printers.
So a print window is checked without a printer installed.
It sits beside `TPress` because the Core port is apart from the press.
The failure property makes every listing raise it as a faulted task, or answers the printers when null.
A test sets it to check that a broken spooler still leaves Save as PDF and reaches the ledger.
`Office Laser` is the default and reports its capabilities as known.
It offers A4 and Letter, single or long-edge sides, and full colour or gray.
It collates and takes at most five copies.
`Plotter` reports nothing and is marked unknown, so the gate falls back.

## `TPressVault`

The print setting store kept in memory, so a print window is checked without a file.
It sits beside `TPress` because the store is apart from the press.
Two print windows handed one instance share the kept setting, as two runs share the file.
The kept setting is null before a save.
A test sets it to stand for a setting an earlier run kept.
The save count lets a fact prove a close saved once or not at all.
A refused save does not count.
The failure property makes every save throw and keep nothing.
A test sets it to stand for a store that cannot be written.

## Stamp

Commit: `c6e60141a172`
- `TExemplar.cs` `113831e430921ef2`
- `TExemplarText.cs` `10b64d46827e6b52`
- `TInterfacePortrait.cs` `04d25771c17ce281`
- `TOutpostFake.cs` `cee6a1e7570dc877`
- `TPress.cs` `655181875f05072d`
- `TPressBoard.cs` `e9775568b2cd1543`
- `TPressProof.cs` `a7d147e6f63d54d9`
- `TPressQueue.cs` `ffe65c85def5c970`
- `TPressVault.cs` `dd0d4c8e3d031dae`
