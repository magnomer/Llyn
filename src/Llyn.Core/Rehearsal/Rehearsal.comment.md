# Rehearsal

This folder holds the records and pure rules of flashcard decks.
A deck is studied on a phone flashcard app, and Llyn writes the deck file that app reads.
Every record is positional and immutable, so a change replaces the record whole.
Every rule here reads only its arguments.
Times arrive as stored strings, so nothing here reads the clock.

## `LGrain`

The kind of unit a deck's cards stand for.
A free card has no source, and every other grain names the one row its face reads.
A pair reads a meaning or a collocation, plus the target entry of its translation.
The numbers are stored in `cue.grain`, so a member never changes its number.

## `LCueColumn`

The kind of phone column a cell or a template slot fills.
Text, Picture and Sound repeat once per side.
Category and Distractor repeat as numbered columns too.
The numbers are stored in `column_kind`, so a member never changes its number.

## `LBearingLead`

Which side the phone shows first, side one, side two or each in turn.
The numbers are stored in `side_first`, so a member never changes its number.

## `LAlmanacOrder`

The phone's card order of a deck.
The numbers are stored in `study_order`, so a member never changes its number.

## `LStencilField`

The lexical values a template slot can name.
A slot body names them as text keys, so a stored body never depends on these numbers.

## `LDeckFolder`

One folder of the deck list.
A deck in no folder sits at the top level.

## `LDeckSchedule`

The phone's card-order options for one deck.
Every option is deck-specific in the phone, so the deck carries them.
The defaults are the phone's own.
`LDeckScheduleGoal` is the round size in Goal order.
`LDeckScheduleSpaced` is the round size in spaced order.
`LDeckScheduleTarget` counts hours.
`LDeckScheduleQuorum` is the phone's Min Cards in Set for Goal order.
A null quorum means `LDeckScheduleFloor`.
The floor waits for the phone probe.
So the probe changes a constant and no schema.

## `LDeck`

One flashcard deck.
`LDeckCode` is the export file name without its extension.
It is set at create, and a rename keeps it.
So a renamed deck still writes the file the phone downloaded.
The deck owns its template, named by `LDeckStencilId`.
Editing one deck's sides therefore never changes another deck.
`LDeckSyncedUtc` is null until the first export.

## `LCue`

One flashcard of a deck.
Each source kind has its own nullable reference, so a rebuild follows every reference.
`LCueText1` and `LCueText2` hold the side texts last exported.
They let a later import match a phone row that carries no id.

## `LCueOrphaned`

A card is orphaned when a row its face reads is gone.
It is derived at read time, so a live delete and a rebuild never disagree on it.
A free card is never orphaned.
A pair is orphaned when its target is gone, or when both its meaning and its collocation are gone.
A removed translation link alone leaves both references set, so it orphans nothing.
An unknown grain counts as orphaned, so it never reaches the phone.

## `LCueResting`

A resting card is left out of the export and out of study.
An excluded card rests, and so does an orphaned one.

## `LCueCell`

One cell of a free card, a column and its index.

## `LCueFace`

The cells one card shows, built from its entry and its deck's template.
It sits in Core because the export and the desktop study read the same face.
A cell's text is already in the phone's markup.

## `LStencil`

A side template.
A saved template is a reusable copy, and every other template belongs to one deck.

## `LStencilSlot`

One column of a template, written as a body of literal text and field tokens.

## `LStencilDefault`

The slots a new deck's own template starts with, chosen by the deck's grain.
Side 1 holds what the grain asks, and side 2 holds its answer.
- Entry asks the headword and answers every definition.
- Meaning asks the headword and answers its definition.
- Collocation asks the expression and answers its paraphrase.
- Example asks the sentence and answers its gloss.
- Pair asks the meaning's headword and answers the target headword.

Picture and Sound sit on side 2, with the answer.
So the side shown first carries no hint of the answer.
A free grain has no default, since a free cue's cells are its own.

## `LStencilToken`

One piece of a slot body.
A literal token carries text and no field.
A field token carries the field, plus either an index or the all mark.
A field token with neither means the field's first value.

## `LStencilTokenParse`

Splits a slot body into literal runs and field tokens.
A field token is written `{key}`, `{key:n}` or `{key:all}`.
Keys ignore case and surrounding space.
The index counts from one.
An unknown key, a bad suffix or an unclosed brace stays literal text.
So a typo shows on the card instead of failing the export.
Literal pieces next to each other join into one token.

## `LCueMarkup`

Turns Llyn text into the phone's markup.
It sits in Core because the export and the desktop study share it.
So the desktop never shows more than the phone draws.

## `LCueMarkupResolve`

Reduces Markdown to what the phone draws.
Bold becomes `<b>` and italic becomes `<i>`.
Code and links keep their plain text.
Every block and every line break becomes `|`, the phone's line break.
An item keeps its bullet or number as a plain lead.
A rule is dropped.

## `LCueMarkupFormat`

Escapes plain text for a phone cell.
A literal `|` would read as a line break, so it becomes the look-alike `∣` (U+2223).
The phone's own help advises that sign for a literal bar.
`<` and `&` pass unchanged until the phone probe pins their rule.

## `LBearing`

One study direction of a deck.
`LBearingFront` and `LBearingBack` are side bitmasks from 1 to 31.
`LBearingStatistics` is the phone's statistics set, from 1 to 4.

## `LDeckReceipt`

The tally of one deck export, shown to the user when it ends.
It counts cards written, media copied and media removed.
A failed file does not stop the export, so failures are listed rather than raised.
It is apart from `LReceipt`, which counts Joplin notes.

## `LDeckCode`

The rule that keeps deck codes apart.
Two decks must never write one file name.
It sits in Core because it is a pure rule over realm values.
Both members compare with `StringComparer.OrdinalIgnoreCase`, which folds non-ASCII case too.
`taken` holds the codes of the other decks only.
So a save that keeps a deck's own code passes.

## `LDeckCodeResolve`

Returns the normalized name, or that name with ` 2`, ` 3` and so on appended until no code matches.

## `LDeckCodeCheck`

Answers false when a code in `taken` matches, ignoring case.

## `LDepot`

The outward port for the folder the phone app syncs.
Each deck is one file named by its code, with its media in a sibling folder.
The port names a deck by folder and code, and knows nothing of file formats.
The save is asynchronous, since its adapter writes through the workspace commit.
The read and the digest answer null when the deck has no file there.
The digest lets a workflow tell a file Llyn wrote from one the phone wrote.
A media name is a plain file name inside the deck's media folder.
The media resolve names a source file by its content, so Application needs no file system.

## `LDeckSheet`

One deck file as cells, before any quoting.
The settings come first as key and value pairs.
The header names the columns, and each row holds one card's cells in header order.
Quoting, breaks and column order belong to the file adapter, so the sheet holds plain text.

## `LDeckManifest`

What Llyn last wrote for each deck, keyed by deck id.
Each deck keeps the file digest, the media names and the time written.
Only names listed here may be deleted from a media folder.
A deck id is stable, while a code can change, so the id is the key.

## `LDeckManifestVault`

The store for the deck manifest.
A read answers an empty manifest when nothing was saved.
A save replaces the manifest whole, so the manifest has one writer.

## Stamp

Commit: `aae5349dbb94`
- `LAlmanacOrder.cs` `e6f4236b098a1575`
- `LBearing.cs` `dfb5d059b3fca107`
- `LBearingLead.cs` `7441fe1ee544c02b`
- `LCue.cs` `73bd9d7d7a394017`
- `LCueCell.cs` `1674fada9b964088`
- `LCueColumn.cs` `b29a2835b433dbc4`
- `LCueFace.cs` `5eb4e06ec86c8a5c`
- `LCueMarkup.cs` `20b9eca55f258f08`
- `LDeck.cs` `0b3c3deccdd6f2f6`
- `LDeckCode.cs` `a91aa39796229f3d`
- `LDeckFolder.cs` `82bda161d83ab78e`
- `LDeckManifest.cs` `bb1a6b2adf77f68a`
- `LDeckManifestVault.cs` `9e92ead069bcee95`
- `LDeckReceipt.cs` `ce0f0d44640eb380`
- `LDeckSchedule.cs` `167195639f7c58ba`
- `LDeckSheet.cs` `662227a4ac0c3f1b`
- `LDepot.cs` `17f69930ec25011f`
- `LGrain.cs` `7cb27e3e6371db24`
- `LStencil.cs` `03806030470bf085`
- `LStencilDefault.cs` `fe79cebfe7cb128e`
- `LStencilField.cs` `9559807691aa972b`
- `LStencilSlot.cs` `da6dcf2601148958`
- `LStencilToken.cs` `972349c9c41812b4`
