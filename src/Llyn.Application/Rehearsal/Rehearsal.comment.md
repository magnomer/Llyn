# Rehearsal

The Rehearsal folder holds the workflows of flashcard decks.
The stencil clerk builds a card's face from its entry and its deck's template.
The stencil reader reads the values of one template field.
The deck clerk writes decks and their folders.
The cue clerk writes the cards of one deck.
The depot latch marks a running deck export.

## `LStencilClerk`

The clerk takes the clerks it reads through, as the portrait clerk does.
Recordings come resolved, since `LEntryClerkLoad` already runs them through the recording clerk.
Speech names come from a static member, so the clerk holds no vocabulary clerk.

### `LStencilClerkResolve`

The face of one card, one cell per template slot, in slot order.
The caller hands in the label, as for the entry page.
So unit, freedom and the unknown mark read in the user's language.
A free card has no face here, since its cells are its own.
The entry to load comes from `LCueHolderRead`, asked on every resolve.
Only an entry cue stores its entry, so the other grains read it through their own row.
A stored copy of the holder would go stale when a row moves, so none is kept.
A card whose entry no longer loads gets an empty face too.
A slot body joins literal text with field values.
A bare field takes its first value, and an index takes that value.
The all mark joins every value with a bar.
A missing value reads empty.
A bar inside a text value is escaped first, so it never reads as a join.
Picture and Sound values are locations and stay unescaped.

### `LStencilClerkResolve(long, LPortraitLabel)`

The face read by card id finds the card and its deck, then resolves it against that deck's template.
It lives beside the resolve it completes, since this clerk already reads decks, cards and templates.
So the facade relays one call and gathers nothing.
A card id that no deck holds refuses with `Refusal.CueMissing`.

### `LStencilClerkRead` and `LStencilSlotSet`

The read answers a deck's owned template, the one its cards resolve against.
A missing deck refuses with `Refusal.DeckMissing`.
Slot set writes one slot of the owned template, keyed by column and index.
A slot already held takes the new body in place, and a new slot is appended.
Save and apply read the owned template through the same read.

### `LStencilClerkSave` and `LStencilClerkApply`

Each deck owns its template, so editing one deck never changes another.
Save copies the owned template into a new saved row under the given name.
Apply copies a saved row's slots into the deck's owned row.
Later edits to either row leave the other as it was.
Apply takes only a saved row.

## `LStencilReader`

One reader lives for one face, because the label belongs to that call.
It reads each field straight from the draft, never from a portrait section.
A field reads the rows its card stands for.
An entry card reads every meaning at any depth and every collocation.
A meaning, collocation or pair card reads its one row.
An example card reads the row that holds the example, and only that sentence.
Definitions come from meanings, and expression and paraphrase from collocations.
Every state value reads through the portrait text rule, so Unknown shows its mark.
Empty values are dropped, so an index counts only real values.

### Targets

A pair card shows its target only while the translation link stands.
A removed link leaves the target empty, and the card stays listed.
Other cards show every translation target of their rows.

### Images

Images come from the card's rows, never from the entry.
Each location resolves through the trail clerk, a file to its path and a web address as typed.
Videos are never read, so a video alone fills no picture.

### Sources

Citations are read only when the card has examples.
An example with an Unknown source shows the unknown mark.

## `LDeckClerk`

The clerk takes the rig, as the translation clerk does.
It takes the trail clerk for the name rule of file names.
Every write that touches two rows runs in one vault session.
So a failure at any step rolls the whole write back.

### `LDeckClerkCreate`

A new deck gets its own template, its row and two bearings in one session.
The bearings are side 1 to side 2 on set 1, and the reverse on set 2.
The new template starts with the grain's default slots from `LStencilDefault`.
The code is the name made fit for a file name.
A code another deck holds in any case gains a number suffix.

### `LDeckNameSet` and `LDeckClerkUpdate`

A rename keeps the code, because the code is the phone's download name.
The update changes the code on purpose.
It refuses a code another deck holds, with `Refusal.DeckCodeTaken`.
The deck's own code is left out of that check, so keeping it saves.
The update takes the name, lead, schedule, code and Basic option.
Folder, place, grain and template stay as stored.

### `LDeckClerkMove` and `LDeckFolderDelete`

A moved deck takes the next free place in its new folder.
Its old folder then closes the gap.
A folder delete first moves its decks to the top level in folder order.
Each takes the next free top-level place, so no two decks share a place.
Then the folder row goes, all in one session.

### `LDeckFolderRead`

The deck list reads its folders here.

## `LCueClerk`

The clerk reads entries through the entry clerk, so a card sees the saved draft.

### `LCueClerkRead`

Reads one deck's cards straight from the cue vault, in their stored order.
A deck never stored reads no cards.

### `LCueClerkScan`

The scan returns one row per unit of the deck's grain.
An entry deck takes the entry, and a meaning deck every meaning at any depth.
A collocation deck takes each collocation, and an example deck each distinct example.
A pair deck takes one row per translation target of each meaning or collocation.
A free grain scans to nothing.

### `LCueClerkAdd`

Add skips any row the deck already binds, so adding twice adds nothing.
Only an entry card holds the entry id, as the schema allows.
An entry that no longer loads adds nothing.

### `LCueCellSave`

Only a free card holds its own cells.
A bound card refuses with `Refusal.CueBound`, since its cells come from its entry.
Picture and Sound cells keep a location as typed.

### `LCueClerkFind`

The cue vault reads cards by deck only.
So a card found by id is sought through every deck.
The stencil clerk's face read seeks a card the same way.

## `LDepotLatch`

The latch holds the busy flag of one workspace's deck export and import.
It is an instance field, so two workspaces never block each other.
The clerks take the latch, so none of them holds a flag of its own.

### `LDepotLatchStart`

It swaps idle to busy in one step and answers whether this caller won.
A caller that loses must refuse, and the winner owns the finish.

### `LDepotLatchFinish`

It sets the latch idle again.
The winner calls it in a `finally`, so a failed run never leaves the latch held.

### `LDepotLatchCheck`

It answers whether a run holds the latch, for the busy read of the deck view.

## `LDepotClerk`

The clerk writes one deck to the phone folder and copies its media beside it.
It copies the courier's locking.
The reads and the manifest save hold the engine gate, and file work runs outside it.
The workspace staff hands it the workspace latch, so it holds no busy flag of its own.

### `LDepotClerkExport`

A run that loses the latch refuses as busy and changes nothing.
The phone folder comes from the settings and must stay under the workspace root.
The setting passes `LTrailPathValidate` again, so a stored path outside the root is refused.
An unset or unresolvable folder refuses with `Refusal.DepotMissing` before the latch work begins.
  A refusal reaches the user as a sentence, where a plain throw would read as a fault.

A deck file whose digest differs from the manifest's is a phone export.
The run refuses it, because Llyn never overwrites a file it has not read.
A file present with no manifest entry is refused the same way.

Resting cues are left out, so an orphaned or excluded cue never reaches the phone.
A free cue reads its own cells, and a bound cue reads its stencil face.

### Media

A local picture or sound is copied under the name the depot gives its content.
The same content is copied once, and an existing name is never rewritten.
An http picture is written as its address.
A video address and an http sound are left empty, because the phone plays neither.
A location that resolves to nothing is named in the receipt's failures.

The media are copied before the deck file is saved.
So a saved file never names a media file that is missing.

### Ownership

The manifest lists only the names this deck copied.
A name that was already in the folder is used but never claimed.
After the save, an owned name the deck no longer uses is deleted.
So a file the phone or the user placed there is never deleted.

The manifest is saved in a `finally`, so a failed run still records what it copied.
A failed save keeps the digest Llyn last wrote.
A failed delete keeps its name, so the next run tries again.

### `LDepotHeaderResolve`

The header holds `Text 1`, every column some cue fills, and `ID` last.
Columns follow side order, then notes, extra info and categories.
Wrong answers and statistics stay out in phase 1.

## Stamp

Commit: `f6dae2392935`
- `LCueClerk.cs` `afe1228a6db37fca`
- `LDeckClerk.cs` `78691b90d1a74996`
- `LDepotClerk.cs` `efa9a9dc0d991ae9`
- `LDepotLatch.cs` `fc94d5346f3c0f68`
- `LStencilClerk.cs` `b1ddc9a051d8e835`
- `LStencilReader.cs` `e300f7e601e1ca04`
